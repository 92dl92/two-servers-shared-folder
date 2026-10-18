using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using BlazorApp.StoreHelper;

namespace BlazorApp.Services.Tests.Storage;

/// <summary>The output of one helper process.</summary>
/// <param name="Writer">Its writer name.</param>
/// <param name="Begin">When it started writing (UTC ticks).</param>
/// <param name="End">When it finished (UTC ticks).</param>
/// <param name="Acks">Saves it was told succeeded.</param>
/// <param name="Conflicts">Conflicts it retried.</param>
/// <param name="BrokenReads">Store calls that failed on broken data.</param>
public sealed record HelperRun(string Writer, long Begin, long End, IReadOnlyList<Ack> Acks, int Conflicts, int BrokenReads);

/// <summary>
/// Starts several copies of <c>BlazorApp.StoreHelper</c> as separate processes on one store,
/// releases them together, and collects what each was told.
/// </summary>
public static class MultiProcessRun
{
    private static readonly TimeSpan Timeout = TimeSpan.FromMinutes(3);

    /// <summary>Runs the workload in <paramref name="processes"/> separate processes at once.</summary>
    /// <param name="strategy">Storage strategy name.</param>
    /// <param name="path">The shared data folder.</param>
    /// <param name="records">Existing records to write to.</param>
    /// <param name="processes">How many processes.</param>
    /// <param name="iterations">Changes per process.</param>
    /// <param name="plugins">Extra assemblies the helper should search for the strategy.</param>
    /// <returns>One result per process.</returns>
    /// <exception cref="InvalidOperationException">A process failed; the message includes its output.</exception>
    public static async Task<IReadOnlyList<HelperRun>> RunAsync(
        string strategy, string path, IReadOnlyList<Guid> records, int processes, int iterations, IEnumerable<Assembly> plugins)
    {
        using var control = new TempFolder(); // the go-file lives outside the data folder
        var goFile = System.IO.Path.Combine(control.Path, "go");
        var helpers = Enumerable.Range(1, processes)
            .Select(i => Helper.Start(strategy, path, records, $"p{i}", iterations, goFile, plugins))
            .ToList();
        try
        {
            using var timeout = new CancellationTokenSource(Timeout);
            await Task.WhenAll(helpers.Select(h => h.Ready.WaitAsync(timeout.Token)));
            await File.WriteAllTextAsync(goFile, "go", timeout.Token);
            await Task.WhenAll(helpers.Select(h => h.Process.WaitForExitAsync(timeout.Token)));

            return [.. helpers.Select(h => h.Result())];
        }
        finally
        {
            foreach (var helper in helpers)
            {
                helper.Dispose();
            }
        }
    }

    /// <summary>One running helper process and the lines it has printed.</summary>
    private sealed class Helper : IDisposable
    {
        private readonly ConcurrentQueue<string> _lines = new();
        private readonly ConcurrentQueue<string> _errors = new();
        private readonly TaskCompletionSource _ready = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private readonly string _writer;

        private Helper(Process process, string writer)
        {
            Process = process;
            _writer = writer;
        }

        public Process Process { get; }

        public Task Ready => _ready.Task;

        public static Helper Start(
            string strategy, string path, IReadOnlyList<Guid> records, string writer, int iterations, string goFile, IEnumerable<Assembly> plugins)
        {
            var start = new ProcessStartInfo("dotnet")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            };
            string[] args =
            [
                HelperDll(), "--strategy", strategy, "--path", path, "--records", string.Join(",", records),
                "--writer", writer, "--iterations", iterations.ToString(CultureInfo.InvariantCulture), "--go-file", goFile,
            ];
            foreach (var arg in args.Concat(plugins.SelectMany(p => new[] { "--plugin", p.Location })))
            {
                start.ArgumentList.Add(arg);
            }

            var helper = new Helper(new Process { StartInfo = start }, writer);
            helper.Process.OutputDataReceived += (_, e) => helper.OnLine(e.Data);
            helper.Process.ErrorDataReceived += (_, e) =>
            {
                if (e.Data is not null)
                {
                    helper._errors.Enqueue(e.Data);
                }
            };
            helper.Process.Exited += (_, _) =>
            {
                if (!helper._ready.Task.IsCompleted)
                {
                    helper.Process.WaitForExit(); // drain stdout/stderr so the message includes them
                    helper._ready.TrySetException(helper.Failure("exited before READY"));
                }
            };
            helper.Process.EnableRaisingEvents = true;
            helper.Process.Start();
            helper.Process.BeginOutputReadLine();
            helper.Process.BeginErrorReadLine();
            return helper;
        }

        public HelperRun Result()
        {
            // WaitForExitAsync can return before the last output lines are delivered; WaitForExit() drains them.
            Process.WaitForExit();
            if (Process.ExitCode != 0)
            {
                throw Failure($"exited with code {Process.ExitCode}");
            }

            long begin = 0, end = 0;
            var conflicts = 0;
            var brokenReads = 0;
            var acks = new List<Ack>();
            foreach (var parts in _lines.Select(l => l.Split(' ')))
            {
                switch (parts[0])
                {
                    case "BEGIN":
                        begin = long.Parse(parts[1], CultureInfo.InvariantCulture);
                        break;
                    case "ACK":
                        acks.Add(new Ack(Guid.Parse(parts[1]), int.Parse(parts[2], CultureInfo.InvariantCulture), parts[3]));
                        break;
                    case "END":
                        end = long.Parse(parts[1], CultureInfo.InvariantCulture);
                        conflicts = int.Parse(parts[3], CultureInfo.InvariantCulture);
                        brokenReads = int.Parse(parts[4], CultureInfo.InvariantCulture);
                        Assert.Equal(acks.Count, int.Parse(parts[2], CultureInfo.InvariantCulture));
                        break;
                }
            }

            return end == 0 ? throw Failure("printed no END line") : new HelperRun(_writer, begin, end, acks, conflicts, brokenReads);
        }

        public void Dispose()
        {
            if (!Process.HasExited)
            {
                Process.Kill(entireProcessTree: true);
            }

            Process.Dispose();
        }

        private void OnLine(string? line)
        {
            if (line is null)
            {
                return;
            }

            _lines.Enqueue(line);
            if (line == "READY")
            {
                _ready.TrySetResult();
            }
        }

        private InvalidOperationException Failure(string what) => new(
            $"Store helper {_writer} {what}.\nstdout (last lines):\n{string.Join("\n", _lines.TakeLast(10))}\nstderr:\n{string.Join("\n", _errors)}");

        private static string HelperDll() =>
            typeof(MultiProcessRun).Assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
                .Single(a => a.Key == "StoreHelperDll").Value!;
    }
}
