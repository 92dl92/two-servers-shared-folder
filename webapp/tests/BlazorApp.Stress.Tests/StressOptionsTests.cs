namespace BlazorApp.Stress.Tests;

/// <summary>The stress tool's command line.</summary>
public sealed class StressOptionsTests
{
    [Fact]
    public void Defaults_run_three_in_memory_servers()
    {
        var options = StressOptions.Parse([]);

        Assert.Equal("InMemory", options.Strategy);
        Assert.Equal(3, options.Servers);
        Assert.Equal(5, options.ClientsPerServer);
        Assert.Null(options.OutputDirectory);
        Assert.Null(options.DataPath);
        Assert.Empty(options.ServerArgs);
    }

    [Fact]
    public void Every_option_is_read()
    {
        var options = StressOptions.Parse(
        [
            "--strategy", "PerRecordFile", "--servers", "4", "--clients", "7", "--records", "9", "--ops", "11", "--seed", "42",
            "--out", "run", "--data", "/shared", "--server-dll", "server.dll",
            "--server-arg", "--Logging:LogLevel:BlazorApp=Debug", "--server-arg", "--x=1",
        ]);

        Assert.Equal(
            new StressOptions
            {
                Strategy = "PerRecordFile",
                Servers = 4,
                ClientsPerServer = 7,
                Records = 9,
                OpsPerClient = 11,
                Seed = 42,
                OutputDirectory = "run",
                DataPath = "/shared",
                ServerDll = "server.dll",
                ServerArgs = options.ServerArgs,
            },
            options);
        Assert.Equal(["--Logging:LogLevel:BlazorApp=Debug", "--x=1"], options.ServerArgs);
    }

    [Theory]
    [InlineData("--nope")]
    [InlineData("--servers")]
    [InlineData("--servers", "0")]
    [InlineData("--clients", "-1")]
    [InlineData("--records", "many")]
    public void Bad_options_are_refused(params string[] args) =>
        Assert.Throws<ArgumentException>(() => StressOptions.Parse(args));

    [Fact]
    public void The_workload_and_think_time_are_read()
    {
        var options = StressOptions.Parse(["--workload", "pipeline", "--think-ms", "250"]);

        Assert.Equal(StressOptions.Pipeline, options.Workload);
        Assert.Equal(250, options.ThinkMilliseconds);
        Assert.Equal(StressOptions.Contention, StressOptions.Parse([]).Workload);
    }

    [Theory]
    [InlineData("--workload", "fast")]
    [InlineData("--think-ms", "-5")]
    // Settings the chosen workload wouldn't use, and a pipeline with nothing to do.
    [InlineData("--think-ms", "500")]
    [InlineData("--workload", "pipeline", "--records", "6")]
    [InlineData("--workload", "pipeline", "--ops", "0")]
    public void Bad_workload_options_are_refused(params string[] args) =>
        Assert.Throws<ArgumentException>(() => StressOptions.Parse(args));

    [Fact]
    public void Prefill_and_list_are_read()
    {
        var options = StressOptions.Parse(["--workload", "pipeline", "--prefill", "6000", "--list"]);

        Assert.Equal(6000, options.Prefill);
        Assert.True(options.List);
        Assert.Equal((0, false), (StressOptions.Parse([]).Prefill, StressOptions.Parse([]).List));
    }

    [Fact]
    public void The_list_is_for_the_pipeline() =>
        Assert.Throws<ArgumentException>(() => StressOptions.Parse(["--list"]));

    [Fact]
    public void Faults_are_read()
    {
        var options = StressOptions.Parse(["--servers", "2", "--fault", "kill:2@40", "--fault", "run:./vm-link.sh ws2 10 me@host@45.5"]);

        Assert.Equal(
            [new Fault(Fault.Kill, 2, null, 40), new Fault(Fault.Run, 0, "./vm-link.sh ws2 10 me@host", 45.5)],
            options.Faults);
        Assert.Empty(StressOptions.Parse([]).Faults);
    }

    [Theory]
    [InlineData("kill:3@10")]   // only 2 servers
    [InlineData("kill:0@10")]
    [InlineData("kill:x@10")]
    [InlineData("kill:1")]      // no time
    [InlineData("kill:1@-1")]
    [InlineData("run:@10")]     // no command
    [InlineData("reboot:1@10")]
    [InlineData("kill:1@100000000")] // more than a day
    public void Bad_faults_are_refused(string fault) =>
        Assert.Throws<ArgumentException>(() => StressOptions.Parse(["--servers", "2", "--fault", fault]));

    [Fact]
    public void Killing_every_server_is_refused() =>
        // The lock check and the read-backs would have no server left to go through.
        Assert.Throws<ArgumentException>(() => StressOptions.Parse(["--servers", "2", "--fault", "kill:1@5", "--fault", "kill:2@9"]));

    [Fact]
    public void Remote_options_are_read()
    {
        var options = StressOptions.Parse(
        [
            "--servers", "4", "--data", "/home/user/share/run", "--remote", "user@mac.example", "--remote-servers", "2",
            "--remote-data", "/Users/user/share/run",
        ]);

        Assert.Equal([new RemoteTarget("user@mac.example", "/Users/user/share/run")], options.Remotes);
        Assert.Equal(2, options.RemoteServers);
    }

    [Fact]
    public void Every_server_can_run_on_remotes_each_with_its_own_path_to_the_folder()
    {
        // Phase 6b: two Windows machines, one with the share on its own disk.
        var options = StressOptions.Parse(
        [
            "--servers", "2", "--remote", "Administrator@ws1", "--remote-data", @"C:\share\run",
            "--remote", "Administrator@ws2", "--remote-data", @"\\ws1\share\run", "--remote-servers", "1",
        ]);

        Assert.Equal(
            [new RemoteTarget("Administrator@ws1", @"C:\share\run"), new RemoteTarget("Administrator@ws2", @"\\ws1\share\run")],
            options.Remotes);
        Assert.Equal(1, options.RemoteServers);
        Assert.Null(options.DataPath);
    }

    [Fact]
    public void Without_a_remote_every_server_is_local()
    {
        var options = StressOptions.Parse([]);

        Assert.Empty(options.Remotes);
        Assert.Equal(0, options.RemoteServers);
    }

    [Theory]
    // A remote needs its path to the folder, and at least one server; servers left here need --data.
    [InlineData("--data", "/d", "--remote", "h", "--remote-servers", "1")]
    [InlineData("--remote", "h", "--remote-servers", "1", "--remote-data", "/r")]
    [InlineData("--data", "/d", "--remote", "h", "--remote-data", "/r")]
    [InlineData("--servers", "2", "--data", "/d", "--remote", "h", "--remote-servers", "0", "--remote-data", "/r")]
    // More servers on the remotes than --servers.
    [InlineData("--servers", "2", "--data", "/d", "--remote", "h", "--remote-servers", "3", "--remote-data", "/r")]
    [InlineData("--servers", "3", "--data", "/d", "--remote", "a", "--remote-data", "/a", "--remote", "b", "--remote-data", "/b", "--remote-servers", "2")]
    [InlineData("--servers", "2", "--remote", "a", "--remote-data", "/a", "--remote", "b", "--remote-data", "/b", "--remote-servers", "1073741825")]
    // Every server remote: the data is copied back from the first remote, so --data would mean nothing.
    [InlineData("--servers", "2", "--data", "/d", "--remote", "h", "--remote-servers", "2", "--remote-data", "/r")]
    // Each --remote needs its own --remote-data.
    [InlineData("--servers", "3", "--data", "/d", "--remote", "a", "--remote", "b", "--remote-servers", "1", "--remote-data", "/r")]
    // The remote options mean nothing without a remote.
    [InlineData("--remote-servers", "1")]
    [InlineData("--data", "/d", "--remote-data", "/r")]
    public void Incomplete_remote_options_are_refused(params string[] args) =>
        Assert.Throws<ArgumentException>(() => StressOptions.Parse(args));

    [Fact]
    public void A_remote_server_is_started_by_a_shell_command_that_prints_its_pid_first()
    {
        var command = RemoteHost.ServerCommand(
            "/Users/user", 5123, "/Users/user/claudetest-stress/runs/r1/logs",
            ["--Storage:Strategy", "Sqlite", "--Storage:Path", "/Users/user/share it's/data"]);

        Assert.StartsWith("echo PID $$ && cd '/Users/user/claudetest-stress/server' && ", command, StringComparison.Ordinal);
        Assert.Contains(" exec \"$d\" BlazorApp.Server.dll ", command, StringComparison.Ordinal);
        Assert.Contains("'--urls' 'http://0.0.0.0:5123'", command, StringComparison.Ordinal);
        Assert.Contains("'--Logging:File:Directory' '/Users/user/claudetest-stress/runs/r1/logs'", command, StringComparison.Ordinal);
        Assert.Contains("'--Storage:Path' '/Users/user/share it'\\''s/data'", command, StringComparison.Ordinal);
        Assert.Contains("ASPNETCORE_ENVIRONMENT=Development", command, StringComparison.Ordinal);
    }

    [Fact]
    public void A_windows_server_connects_to_the_share_then_prints_its_pid_first()
    {
        var script = RemoteHost.WindowsServerScript(
            @"C:\Users\Administrator\claudetest-stress\server", 5123, @"C:\Users\Administrator\claudetest-stress\runs\r1\logs",
            ["--Storage:Strategy", "Sqlite", "--Storage:Path", @"\\ws1\share it's\data"]);

        Assert.StartsWith("Write-Output \"PID $PID\"; ", script, StringComparison.Ordinal);
        Assert.Contains(RemoteHost.WindowsConnect, script, StringComparison.Ordinal);
        Assert.Contains(@"Set-Location 'C:\Users\Administrator\claudetest-stress\server'", script, StringComparison.Ordinal);
        Assert.Contains("$env:ASPNETCORE_ENVIRONMENT = 'Development'", script, StringComparison.Ordinal);
        Assert.Contains("& dotnet BlazorApp.Server.dll '--urls' 'http://0.0.0.0:5123'", script, StringComparison.Ordinal);
        Assert.Contains(@"'--Storage:Path' '\\ws1\share it''s\data'", script, StringComparison.Ordinal);
    }

    [Fact]
    public void A_windows_command_goes_over_ssh_as_encoded_powershell()
    {
        // The Windows ssh server hands a command to its shell through a command line that loses
        // nested quotes; base64 of UTF-16 keeps any script intact.
        var command = RemoteHost.EncodedPowerShell("Write-Output 'it''s'");

        Assert.StartsWith("powershell -NoProfile -NonInteractive -EncodedCommand ", command, StringComparison.Ordinal);
        Assert.Equal(
            "Write-Output 'it''s'",
            System.Text.Encoding.Unicode.GetString(Convert.FromBase64String(command.Split(' ')[^1])));
    }

    [Theory]
    // The size is the last line; a connect script or a shell profile may print before it.
    [InlineData("588800", 588800)]
    [InlineData("588800\r\n", 588800)]
    [InlineData("The command completed successfully.\r\n\r\n588800\r\n", 588800)]
    [InlineData("hello from .bashrc\n  1024\n", 1024)]
    public void The_archive_size_is_the_last_line_the_remote_printed(string output, long size) =>
        Assert.Equal(size, RemoteHost.LastNumber(output));

    [Fact]
    public void An_archive_size_that_is_not_a_number_stops_the_run() =>
        Assert.Throws<InvalidOperationException>(() => RemoteHost.LastNumber("tar: oops\n"));

    [Theory]
    [InlineData(@"C:\Users\Administrator", true)]
    [InlineData("/Users/user", false)]
    [InlineData("/home/user", false)]
    public void A_remote_is_windows_when_its_home_has_a_drive_letter(string home, bool windows) =>
        Assert.Equal(windows, RemoteHost.IsWindowsHome(home));

    [Fact]
    public async Task Bad_options_exit_with_code_2_and_the_usage()
    {
        using var error = new StringWriter();

        var code = await StressProgram.MainAsync(["--servers", "0"], TextWriter.Null, error);

        Assert.Equal(StressProgram.CouldNotRun, code);
        Assert.Contains("Usage:", error.ToString(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task An_unknown_strategy_exits_with_code_2_before_starting_servers()
    {
        using var folder = new TempFolder();
        using var output = new StringWriter();
        using var error = new StringWriter();

        var code = await StressProgram.MainAsync(["--strategy", "NoSuchStrategy", "--out", folder.Path], output, error);

        Assert.Equal(StressProgram.CouldNotRun, code);
        Assert.Contains("NoSuchStrategy", error.ToString(), StringComparison.Ordinal);
        Assert.DoesNotContain("Starting", output.ToString(), StringComparison.Ordinal);
    }

    [Fact]
    public void Each_client_gets_its_own_user_and_the_reader_is_an_admin()
    {
        var users = StressRun.MakeUsers(servers: 2, clientsPerServer: 5, records: 4);

        Assert.Equal(11, users.Count);
        Assert.Equal(users.Count, users.Select(u => u.Id).Distinct().Count());
        Assert.Equal("s1c1-registrant", users[0].Id);
        Assert.Equal("s2c4-admin", users[8].Id);
        Assert.Equal([BlazorApp.Shared.Dtos.Role.Admin], users[^1].Roles);
        Assert.Equal(StressRun.ReaderId, users[^1].Id);
    }

    [Theory]
    [InlineData(1, new[] { "s1c1" })]
    [InlineData(3, new[] { "s1c1", "s2c1", "s3c1" })]
    [InlineData(4, new[] { "s1c1", "s1c3", "s2c1", "s3c1" })]
    public void Registrants_beyond_the_records_become_reviewers(int records, string[] registrants)
    {
        // 3 × 8 clients have 12 registrants. With fewer records, the registrants that would get
        // one (one per server in turn, as the workload hands them out) stay; the rest review,
        // so every client still writes.
        var users = StressRun.MakeUsers(servers: 3, clientsPerServer: 8, records);

        Assert.Equal(25, users.Count);
        Assert.Equal(
            registrants.Select(r => $"{r}-registrant"),
            users.Where(u => u.Roles.Contains(BlazorApp.Shared.Dtos.Role.Registrant)).Select(u => u.Id));
        Assert.Equal(3, users.Count(u => u.Id != StressRun.ReaderId && u.Roles.Contains(BlazorApp.Shared.Dtos.Role.Admin)));
    }

    [Fact]
    public void Every_registrant_stays_when_there_are_enough_records()
    {
        // Ids name the role, e.g. s1c3-registrant.
        Assert.Equal(StressRun.MakeUsers(3, 8, records: 12).Select(u => u.Id), StressRun.MakeUsers(3, 8, records: 96).Select(u => u.Id));
        Assert.Equal(12, StressRun.MakeUsers(3, 8, records: 12).Count(u => u.Id.EndsWith("-registrant", StringComparison.Ordinal)));
    }
}
