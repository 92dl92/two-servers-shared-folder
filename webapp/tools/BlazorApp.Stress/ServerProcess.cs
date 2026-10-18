using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using BlazorApp.Shared.Contracts;

namespace BlazorApp.Stress;

/// <summary>
/// Runs the built Server as a separate operating-system process on a free local port, as the
/// E2E tests do: the experiment needs real processes, not simulated servers (plan section 4).
/// A remote server is the same, with <c>ssh</c> as the local process (<see cref="RemoteHost"/>).
/// </summary>
public sealed class ServerProcess : IAsyncDisposable
{
    private static readonly TimeSpan StartTimeout = TimeSpan.FromSeconds(90);

    /// <summary>Ports already given to a server by this process, so two servers never get the same one.</summary>
    private static readonly HashSet<int> UsedPorts = [];

    private readonly Process _process;
    private readonly StringBuilder _output = new();
    private readonly string _outputFile;
    private readonly RemoteHost? _remote;
    private int? _remotePid;

    private ServerProcess(Process process, int port, string outputFile, RemoteHost? remote)
    {
        _process = process;
        Port = port;
        BaseAddress = new Uri($"http://{remote?.Host ?? "127.0.0.1"}:{port}/");
        _outputFile = outputFile;
        _remote = remote;
    }

    /// <summary>The port it listens on.</summary>
    public int Port { get; }

    /// <summary>Its name in the results: the port, or <c>host:port</c> for a remote server.</summary>
    public string Name => _remote is null ? Port.ToString(System.Globalization.CultureInfo.InvariantCulture) : $"{_remote.Host}:{Port}";

    /// <summary>The remote machine it runs on; null when local.</summary>
    public string? Host => _remote?.Host;

    /// <summary>Root URL, ending in a slash.</summary>
    public Uri BaseAddress { get; }

    /// <summary>Whether the process has stopped.</summary>
    public bool HasExited => _process.HasExited;

    /// <summary>Gets a value indicating whether a failure test killed it on purpose.</summary>
    public bool Killed { get; private set; }

    /// <summary>Kills it abruptly, as a crash would (a failure test).</summary>
    /// <returns>A task that completes when it's gone.</returns>
    /// <exception cref="InvalidOperationException">A remote server's process id isn't known, or ssh failed.</exception>
    public async Task KillAsync()
    {
        // A server that already died by itself is a crash, not this fault: the run must say so.
        if (HasExited)
        {
            throw new InvalidOperationException($"Server {Name} had already exited before it was to be killed:\n{Output}");
        }

        Killed = true;
        if (_remote is not null)
        {
            await _remote.KillAsync(_remotePid ?? throw new InvalidOperationException($"Server {Name} never printed its process id."));
        }
        else
        {
            _process.Kill(entireProcessTree: true);
        }

        using var wait = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        await _process.WaitForExitAsync(wait.Token);
    }

    /// <summary>Everything the server has written to stdout and stderr so far.</summary>
    public string Output
    {
        get
        {
            lock (_output)
            {
                return _output.ToString();
            }
        }
    }

    /// <summary>The Server built alongside this tool.</summary>
    public static string DefaultServerDll => Metadata("ServerDll");

    /// <summary>Starts a server and waits until its API answers.</summary>
    /// <param name="serverDll">The built <c>BlazorApp.Server.dll</c>.</param>
    /// <param name="logDirectory">Folder for its log file (<c>Logging:File:Directory</c>) and console output.</param>
    /// <param name="args">Its settings, e.g. <c>--Storage:Strategy</c>; with remote paths for a remote server.</param>
    /// <param name="remote">The machine to run it on (a copy of the Server prepared there); null for this one.</param>
    /// <returns>The running server.</returns>
    /// <exception cref="InvalidOperationException">It exited or didn't answer in time; the message includes its output.</exception>
    public static async Task<ServerProcess> StartAsync(string serverDll, string logDirectory, IEnumerable<string> args, RemoteHost? remote = null)
    {
        // A free port here is only likely to be free on a remote machine; a server that can't
        // bind it exits, which stops the run.
        var port = FreePort();
        var start = new ProcessStartInfo(remote is null ? "dotnet" : "ssh")
        {
            // The project folder when it's the default build (static web assets resolve as with
            // dotnet run); otherwise the folder of the given dll.
            WorkingDirectory = serverDll == DefaultServerDll ? Metadata("ServerProjectDir") : Path.GetDirectoryName(serverDll)!,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };
        IEnumerable<string> allArgs = remote is null
            ? [serverDll, "--urls", $"http://127.0.0.1:{port}", "--Logging:File:Directory", logDirectory, .. args]
            : remote.StartArguments(port, args);
        foreach (var arg in allArgs)
        {
            start.ArgumentList.Add(arg);
        }

        start.Environment["ASPNETCORE_ENVIRONMENT"] = "Development";

        var process = new Process { StartInfo = start };
        var server = new ServerProcess(process, port, Path.Combine(logDirectory, $"server-{port}.console.txt"), remote);
        process.OutputDataReceived += (_, e) => server.Append(e.Data);
        process.ErrorDataReceived += (_, e) => server.Append(e.Data);
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        try
        {
            await server.WaitUntilReadyAsync();
        }
        catch
        {
            await server.DisposeAsync();
            throw;
        }

        return server;
    }

    /// <summary>Stops the server and saves its console output next to its log file.</summary>
    /// <returns>A task that completes when it has stopped.</returns>
    public async ValueTask DisposeAsync()
    {
        if (_remote is not null && _remotePid is { } pid && !Killed)
        {
            // Killing the local ssh alone may leave the remote server running. One killed by a
            // fault is gone, and its process id may belong to another process by now.
            await _remote.StopAsync(pid);
            using var wait = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            try
            {
                await _process.WaitForExitAsync(wait.Token);
            }
            catch (OperationCanceledException)
            {
                // Killed below.
            }
        }

        if (!_process.HasExited)
        {
            _process.Kill(entireProcessTree: true);
            await _process.WaitForExitAsync();
        }

        _process.Dispose();
        try
        {
            await File.WriteAllTextAsync(_outputFile, Output);
        }
        catch (IOException)
        {
            // The console copy is a convenience; the server's own log file has the same lines.
        }
    }

    private async Task WaitUntilReadyAsync()
    {
        using var http = new HttpClient { BaseAddress = BaseAddress, Timeout = TimeSpan.FromSeconds(5) };
        var deadline = DateTime.UtcNow + StartTimeout;
        while (DateTime.UtcNow < deadline)
        {
            if (_process.HasExited)
            {
                throw new InvalidOperationException($"Server {Name} exited with code {_process.ExitCode}:\n{Output}");
            }

            try
            {
                using var response = await http.GetAsync(ApiRoutes.Users);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return;
                }
            }
            catch (HttpRequestException)
            {
                // Not listening yet.
            }
            catch (TaskCanceledException)
            {
                // Slow first request.
            }

            await Task.Delay(200);
        }

        throw new InvalidOperationException($"Server {Name} did not start within {StartTimeout.TotalSeconds} s:\n{Output}");
    }

    private void Append(string? line)
    {
        if (line is null)
        {
            return;
        }

        // A remote server's first line is its process id (RemoteHost.ServerCommand, or
        // WindowsServerScript); the terminal ssh forces on Linux and macOS ends lines with \r.
        if (_remote is not null && _remotePid is null && line.StartsWith("PID ", StringComparison.Ordinal)
            && int.TryParse(line.AsSpan(4).TrimEnd('\r'), System.Globalization.CultureInfo.InvariantCulture, out var pid))
        {
            _remotePid = pid;
        }

        lock (_output)
        {
            _output.AppendLine(line);
        }
    }

    private static int FreePort()
    {
        while (true)
        {
            using var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            lock (UsedPorts)
            {
                if (UsedPorts.Add(port))
                {
                    return port;
                }
            }
        }
    }

    private static string Metadata(string key) =>
        typeof(ServerProcess).Assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
            .Single(a => a.Key == key).Value
        ?? throw new InvalidOperationException($"Assembly metadata '{key}' is missing.");
}
