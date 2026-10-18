using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace BlazorApp.E2E.Tests;

/// <summary>
/// Runs the built Server as a separate operating-system process on a free local port,
/// the same way <c>dotnet run</c> would.
/// </summary>
public sealed class ServerProcess : IAsyncDisposable
{
    private readonly Process _process;
    private readonly StringBuilder _output = new();

    private ServerProcess(Process process, Uri baseAddress)
    {
        _process = process;
        BaseAddress = baseAddress;
    }

    /// <summary>Root URL of the running server, ending in a slash.</summary>
    public Uri BaseAddress { get; }

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

    /// <summary>Starts the server and waits until it answers HTTP requests.</summary>
    /// <param name="extraArgs">Additional command-line arguments, e.g. configuration overrides.</param>
    /// <returns>The running server.</returns>
    public static async Task<ServerProcess> StartAsync(params string[] extraArgs)
    {
        var port = FreePort();
        var baseAddress = new Uri($"http://127.0.0.1:{port}/");

        var start = new ProcessStartInfo("dotnet")
        {
            WorkingDirectory = Metadata("ServerProjectDir"),
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };
        start.ArgumentList.Add(Metadata("ServerDll"));
        start.ArgumentList.Add("--urls");
        start.ArgumentList.Add(baseAddress.ToString().TrimEnd('/'));
        foreach (var arg in extraArgs)
        {
            start.ArgumentList.Add(arg);
        }

        // Development turns on static web assets from the build output, as dotnet run does.
        start.Environment["ASPNETCORE_ENVIRONMENT"] = "Development";

        var process = new Process { StartInfo = start };
        var server = new ServerProcess(process, baseAddress);
        process.OutputDataReceived += (_, e) => server.Append(e.Data);
        process.ErrorDataReceived += (_, e) => server.Append(e.Data);
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await server.WaitUntilReadyAsync();
        return server;
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (!_process.HasExited)
        {
            _process.Kill(entireProcessTree: true);
            await _process.WaitForExitAsync();
        }

        _process.Dispose();
    }

    private async Task WaitUntilReadyAsync()
    {
        using var http = new HttpClient { BaseAddress = BaseAddress, Timeout = TimeSpan.FromSeconds(5) };
        var deadline = DateTime.UtcNow.AddSeconds(60);
        while (DateTime.UtcNow < deadline)
        {
            if (_process.HasExited)
            {
                throw new InvalidOperationException($"Server exited with code {_process.ExitCode}:\n{Output}");
            }

            try
            {
                using var response = await http.GetAsync("/");
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

        throw new TimeoutException($"Server did not start within 60 s:\n{Output}");
    }

    private void Append(string? line)
    {
        if (line is null)
        {
            return;
        }

        lock (_output)
        {
            _output.AppendLine(line);
        }
    }

    private static int FreePort()
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        return ((IPEndPoint)listener.LocalEndpoint).Port;
    }

    private static string Metadata(string key) =>
        typeof(ServerProcess).Assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
            .Single(a => a.Key == key).Value
        ?? throw new InvalidOperationException($"Assembly metadata '{key}' is missing.");
}
