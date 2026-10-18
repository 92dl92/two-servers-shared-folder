using System.Text.Json;

namespace BlazorApp.Stress.Tests;

/// <summary>
/// Servers the tool didn't start (<c>--external</c>, phase 6c): started here as real processes,
/// as a person would start them with <c>ClaudeTestServer.ps1</c>, after <c>--prepare</c> wrote
/// the users file they read. The known answers must hold as when the tool starts its own.
/// </summary>
public sealed class ExternalServerTests
{
    private const int Clients = 5;

    [Fact]
    public async Task One_external_in_memory_server_loses_nothing()
    {
        using var run = new TempFolder();
        using var data = new TempFolder();

        var (code, result, _) = await RunAsync(run, data, servers: 1, "InMemory");

        Assert.True(code == StressProgram.Passed, result.ToSummary());
        Assert.Equal(1, result.Settings.Servers);
        Assert.NotNull(result.Settings.External);
        Assert.Contains("started by hand", result.ToSummary(), StringComparison.Ordinal);
        Assert.All(result.Servers, s => Assert.Null(s.LogFile));
    }

    [Fact]
    public async Task Two_external_in_memory_servers_are_reported_as_losing_records()
    {
        // Each keeps its own memory, so each read-back misses what was created through the other.
        using var run = new TempFolder();
        using var data = new TempFolder();

        var (code, result, _) = await RunAsync(run, data, servers: 2, "InMemory");

        Assert.Equal(StressProgram.ChecksFailed, code);
        Assert.All(result.Checks.Skip(1), c => Assert.True(c.Report.MissingRecords > 0, result.ToSummary()));
    }

    [Fact]
    public async Task One_file_per_record_passes_with_random_routing_and_uses_both_servers()
    {
        using var run = new TempFolder();
        using var data = new TempFolder();

        var (code, result, _) = await RunAsync(run, data, servers: 2, "PerRecordFile", ["--routing", "random"]);

        Assert.True(code == StressProgram.Passed, result.ToSummary());
        Assert.Equal(StressOptions.Random, result.Settings.Routing);

        // The folder itself was read back after the servers stopped, and both servers took requests.
        Assert.Null(result.Checks[0].Report.Skipped);
        Assert.Equal(2, result.RequestsByServer!.Count);
        Assert.All(result.RequestsByServer.Values, n => Assert.True(n > 0));
        Assert.Contains("Requests by server", result.ToSummary(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task The_naive_control_loses_updates_through_external_servers()
    {
        // As for the shared suite's control (decision "Flaky control race"): the control may get
        // lucky on a short run, so it gets up to three runs and must lose something in one.
        var failed = false;
        for (var round = 0; round < 3 && !failed; round++)
        {
            using var run = new TempFolder();
            using var data = new TempFolder();
            var (code, _, _) = await RunAsync(run, data, servers: 2, "NaiveFile", ["--routing", "random"], ops: 40);
            failed = code == StressProgram.ChecksFailed;
        }

        Assert.True(failed, "NaiveFile passed three runs through external servers; the checks would miss a broken store.");
    }

    [Fact]
    public async Task A_run_without_prepare_could_not_be_done()
    {
        using var run = new TempFolder();
        using var data = new TempFolder();
        using var output = new StringWriter();
        using var error = new StringWriter();

        var code = await StressProgram.MainAsync(
            ["--external", "http://127.0.0.1:1", "--data", data.Path, "--out", run.Path, "--clients", "2", "--ops", "1"], output, error);

        Assert.Equal(StressProgram.CouldNotRun, code);
        Assert.Contains("--prepare", error.ToString(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task A_run_with_other_settings_than_its_prepare_could_not_be_done()
    {
        // The users --prepare wrote depend on the servers, clients and records: a run with other
        // numbers would hand clients the wrong users.
        using var run = new TempFolder();
        using var data = new TempFolder();
        using var output = new StringWriter();
        using var error = new StringWriter();
        string[] common = ["--external", "http://127.0.0.1:1,http://127.0.0.1:2", "--data", data.Path, "--out", run.Path, "--records", "6", "--ops", "1"];
        Assert.Equal(StressProgram.Passed, await StressProgram.MainAsync([.. common, "--clients", "3", "--prepare", "--users-out", Path.Combine(run.Path, "u.json")], output, error));

        var code = await StressProgram.MainAsync([.. common, "--clients", "5"], output, error);

        Assert.Equal(StressProgram.CouldNotRun, code);
        Assert.Contains("--prepare", error.ToString(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task Waiting_for_a_server_to_be_stopped_can_be_cancelled()
    {
        // Ctrl+C must end the wait for a server nobody stops, not leave the tool hanging an hour.
        using var listener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback, 0);
        listener.Start();
        var server = new ExternalServer(new Uri($"http://127.0.0.1:{((System.Net.IPEndPoint)listener.LocalEndpoint).Port}/"), TimeSpan.FromMinutes(60));
        using var cancel = new CancellationTokenSource(TimeSpan.FromSeconds(2));

        var waiting = server.WaitUntilStoppedAsync(cancel.Token);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => waiting.WaitAsync(TimeSpan.FromSeconds(30)));
        Assert.False(server.Stopped);
    }

    [Fact]
    public async Task A_server_that_refuses_connections_counts_as_stopped()
    {
        using var listener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback, 0);
        listener.Start();
        var port = ((System.Net.IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        var server = new ExternalServer(new Uri($"http://127.0.0.1:{port}/"), TimeSpan.FromSeconds(30));

        await server.WaitUntilStoppedAsync(CancellationToken.None);

        Assert.True(server.Stopped);
    }

    [Fact]
    public async Task Servers_started_with_another_users_file_are_refused()
    {
        // A server started with an older run's users file wouldn't know this run's users: every
        // request would be a 401 and the run would mean nothing.
        using var run = new TempFolder();
        using var data = new TempFolder();
        using var other = new TempFolder();
        var wrongUsers = Path.Combine(other.Path, "users.json");
        await File.WriteAllTextAsync(wrongUsers, JsonSerializer.Serialize(StressRun.MakeUsers(1, 1, 1), StressResult.JsonOptions));

        var (code, _, error) = await RunAsync(run, data, servers: 1, "InMemory", usersFileForServers: wrongUsers);

        Assert.Equal(StressProgram.CouldNotRun, code);
        Assert.Contains("users file", error, StringComparison.Ordinal);
    }

    /// <summary>
    /// Prepares (with placeholder addresses: only their count matters then), starts the servers
    /// as a person would, runs the tool against them, and stops them when the tool asks.
    /// </summary>
    private static async Task<(int Code, StressResult Result, string Error)> RunAsync(
        TempFolder run, TempFolder data, int servers, string strategy, string[]? extra = null, int ops = 15, string? usersFileForServers = null)
    {
        var placeholders = string.Join(",", Enumerable.Range(1, servers).Select(i => $"http://127.0.0.1:{i}"));
        var usersFile = Path.Combine(data.Path, "..", $"{Path.GetFileName(run.Path)}-users.json");
        string[] common = ["--strategy", strategy, "--data", data.Path, "--out", run.Path, "--clients", $"{Clients}", "--records", "6", "--ops", $"{ops}", "--seed", "7", .. extra ?? []];

        using (var prepareOutput = new StringWriter())
        using (var prepareError = new StringWriter())
        {
            var prepared = await StressProgram.MainAsync(["--external", placeholders, .. common, "--prepare", "--users-out", usersFile], prepareOutput, prepareError);
            Assert.True(prepared == StressProgram.Passed, $"{prepareOutput}\n{prepareError}");
        }

        var logs = Directory.CreateDirectory(Path.Combine(run.Path, "server-logs")).FullName;
        var started = new List<ServerProcess>();
        var stopped = false;
        async Task StopAsync()
        {
            if (!stopped)
            {
                stopped = true;
                await Task.WhenAll(started.Select(s => s.DisposeAsync().AsTask()));
            }
        }

        try
        {
            for (var i = 0; i < servers; i++)
            {
                started.Add(await ServerProcess.StartAsync(
                    ServerProcess.DefaultServerDll, logs, ["--Storage:Strategy", strategy, "--Storage:Path", data.Path, "--Users:Path", usersFileForServers ?? usersFile]));
            }

            var output = new SignalWriter(StressRun.StopServersPrompt);
            using var error = new StringWriter();
            var running = StressProgram.MainAsync(["--external", string.Join(",", started.Select(s => s.BaseAddress.ToString())), .. common], output, error);

            // Stop the servers when the tool asks, as the person running it would.
            if (await Task.WhenAny(running, output.Seen) == output.Seen)
            {
                await StopAsync();
            }

            var code = await running;
            var resultFile = Path.Combine(run.Path, "result.json");
            var result = File.Exists(resultFile)
                ? JsonSerializer.Deserialize<StressResult>(await File.ReadAllTextAsync(resultFile), StressResult.JsonOptions)!
                : null!;
            Assert.True(code == StressProgram.CouldNotRun || result is not null, $"{output}\n{error}");
            return (code, result, error.ToString());
        }
        finally
        {
            await StopAsync();
            File.Delete(usersFile);
        }
    }

    /// <summary>Collects the tool's output and signals when a line contains <paramref name="text"/>.</summary>
    private sealed class SignalWriter(string text) : StringWriter
    {
        private readonly TaskCompletionSource _seen = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public Task Seen => _seen.Task;

        public override void WriteLine(string? value)
        {
            lock (this)
            {
                base.WriteLine(value);
            }

            if (value?.Contains(text, StringComparison.Ordinal) == true)
            {
                _seen.TrySetResult();
            }
        }
    }
}
