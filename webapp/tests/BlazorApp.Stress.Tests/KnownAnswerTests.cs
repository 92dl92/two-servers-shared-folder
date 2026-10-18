using System.Text.Json;

namespace BlazorApp.Stress.Tests;

/// <summary>
/// Known answers the stress tool must give before it can be trusted (plan section 6, phase 5):
/// with the in-memory store, one server loses nothing; several servers each keep their own
/// memory, so reading back through any one of them must show records and saves missing.
/// These start real Server processes, kept small so the test run stays short.
/// </summary>
public sealed class KnownAnswerTests
{
    private const int Records = 6;
    private const int Ops = 15;
    private const int Clients = 5;

    [Fact]
    public async Task One_in_memory_server_loses_nothing()
    {
        using var folder = new TempFolder();

        var (code, result) = await RunAsync(folder, servers: 1);

        Assert.True(code == StressProgram.Passed, result.ToSummary());
        Assert.True(result.Passed);
        Assert.True(result.Numbers.Saves > Records, result.ToSummary());
        Assert.Equal(result.Numbers.Saves, result.Numbers.WritesByOutcome[StressNumbers.Saved]);
        Assert.Equal(Records, result.Numbers.SavesByAction[BlazorApp.Shared.Dtos.RecordAction.Create]);
        Assert.Equal(0, result.Numbers.NoResponse);

        // The folder can't be read back for a store that keeps nothing in it; the server can.
        Assert.NotNull(result.Checks[0].Report.Skipped);
        var server = Assert.Single(result.Checks.Skip(1));
        Assert.True(server.Passed);
        Assert.Equal(Records, server.Report.RecordsFound);
        Assert.Equal(0, server.Report.LostUpdates + server.Report.MissingRecords + server.Report.Unacknowledged + server.Report.RuleViolations);

        // Every server writes its log into the run folder, where the requests' correlation ids can be found.
        var log = await File.ReadAllTextAsync(Assert.Single(result.Servers).LogFile!);
        Assert.Contains("\"CorrelationId\":\"stress-", log, StringComparison.Ordinal);
        Assert.True(File.Exists(Path.Combine(folder.Path, "summary.md")));
    }

    [Fact]
    public async Task Several_in_memory_servers_are_reported_as_losing_records_and_updates()
    {
        using var folder = new TempFolder();

        var (code, result) = await RunAsync(folder, servers: 3);

        Assert.Equal(StressProgram.ChecksFailed, code);
        Assert.False(result.Passed);
        var servers = result.Checks.Skip(1).ToList();
        Assert.Equal(3, servers.Count);
        Assert.All(servers, check =>
        {
            // Each server holds only the records created through it: two of the six.
            Assert.False(check.Passed);
            Assert.Equal(Records - 2, check.Report.MissingRecords);
            Assert.True(check.Report.LostUpdates > 0);
            Assert.Equal(0, check.Report.RuleViolations);
            Assert.Equal(0, check.Report.Unacknowledged);
        });
        Assert.Contains("FAILED", result.ToSummary(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task Every_client_writes_to_a_single_shared_record()
    {
        // Two registrants per five clients, but only one record asked for, so every client
        // shares it (the contention sweep needs fewer records than registrants). The second
        // registrant would own nothing and may neither view nor change others' records, so it
        // runs as a reviewer: no 403s, and all five clients still send their changes.
        using var folder = new TempFolder();

        var (code, result) = await RunAsync(folder, servers: 1, records: 1);

        Assert.Equal(StressProgram.Passed, code);
        Assert.Equal(1, result.Settings.Records);
        Assert.Equal(1, result.Settings.ClientsByRole[BlazorApp.Shared.Dtos.Role.Registrant]);
        Assert.Equal(3, result.Settings.ClientsByRole[BlazorApp.Shared.Dtos.Role.Reviewer]);
        Assert.Equal(1, result.Numbers.SavesByAction[BlazorApp.Shared.Dtos.RecordAction.Create]);
        Assert.DoesNotContain("forbidden", result.Numbers.WritesByOutcome.Keys);
        Assert.Equal(Clients * Ops, result.Numbers.Reads);
        Assert.Equal(1 + (Clients * Ops), result.Numbers.Writes);
        Assert.Empty(result.Numbers.ReadFailures);
    }

    [Fact]
    public async Task A_pipeline_through_one_in_memory_server_loses_nothing_and_reads_nothing_stale()
    {
        // Phase 6b's positive control: one server can't show an old version, so the stale-read
        // check must find none, and records must get all the way through the pipeline.
        using var folder = new TempFolder();

        var (code, result) = await RunAsync(folder, servers: 1, records: null, extra: ["--workload", "pipeline", "--think-ms", "0"]);

        Assert.True(code == StressProgram.Passed, result.ToSummary());
        Assert.Equal(StressOptions.Pipeline, result.Settings.Workload);
        Assert.Equal(0, result.Settings.ThinkMilliseconds);
        var saves = result.Numbers.SavesByAction;
        Assert.True(saves[BlazorApp.Shared.Dtos.RecordAction.Create] > 1, result.ToSummary());
        Assert.True(saves[BlazorApp.Shared.Dtos.RecordAction.Submit] > 0, result.ToSummary());
        Assert.True(saves.GetValueOrDefault(BlazorApp.Shared.Dtos.RecordAction.Approve) + saves.GetValueOrDefault(BlazorApp.Shared.Dtos.RecordAction.Reject) > 0, result.ToSummary());
        Assert.Equal(0, result.Numbers.StaleReads!.Count);
        Assert.Equal(0, result.Numbers.StaleReads.ReadsAfterAnotherServer);
        Assert.Contains("Stale reads: 0", result.ToSummary(), StringComparison.Ordinal);

        // The longest stall reaches the result; with no think time and one server it is short.
        Assert.InRange(result.Numbers.LongestStall!.Seconds, 0, 5);
        Assert.Contains("Longest stall: ", result.ToSummary(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task A_server_killed_mid_run_loses_nothing_and_leaves_every_record_changeable()
    {
        // Phase 6b's failure tests, on this machine: two servers on one folder (C), the second
        // killed early in the run. Its read-back is skipped; the other must see everything, and
        // every record must still be changeable through it.
        using var folder = new TempFolder();
        using var data = new TempFolder();

        var (code, result) = await RunAsync(
            folder, servers: 2, records: null,
            extra: ["--strategy", "PerRecordFile", "--data", data.Path, "--workload", "pipeline", "--think-ms", "20", "--fault", "kill:2@0.3"]);

        Assert.True(code == StressProgram.Passed, result.ToSummary());
        var fault = Assert.Single(result.Faults!);
        Assert.True(fault.Happened);
        Assert.Contains("killed", fault.Result, StringComparison.Ordinal);
        Assert.Equal("killed by --fault", result.Checks[^1].Report.Skipped);
        Assert.True(result.LockCheck!.Passed);
        Assert.Equal(result.Settings.Records, result.LockCheck.Changes);
        Assert.Contains("Lock check: pass", result.ToSummary(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task Prefilled_records_are_on_the_share_and_checked_as_existing_data()
    {
        using var folder = new TempFolder();
        using var data = new TempFolder();

        var (code, result) = await RunAsync(
            folder, servers: 2, records: null,
            extra: ["--strategy", "PerRecordFile", "--data", data.Path, "--workload", "pipeline", "--think-ms", "0", "--prefill", "50", "--list"]);

        Assert.True(code == StressProgram.Passed, result.ToSummary());
        Assert.Equal(50, result.Settings.Prefill);
        // Expected (each must still be there at its version), not merely found.
        Assert.All(result.Checks, c => Assert.Equal((0, result.Settings.Records + 50), (c.Report.RecordsUnexpected, c.Report.RecordsExpected)));
        Assert.True(result.Numbers.Lists > 0);
        Assert.Contains("List p50", result.ToSummary(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task A_fault_that_does_not_happen_means_the_run_could_not_be_done()
    {
        using var folder = new TempFolder();
        using var output = new StringWriter();
        using var error = new StringWriter();

        var code = await StressProgram.MainAsync(
            ["--servers", "1", "--clients", "2", "--ops", "3", "--out", folder.Path, "--fault", "run:exit 3@0"], output, error);

        Assert.Equal(StressProgram.CouldNotRun, code);
        Assert.Contains("exit 3", error.ToString(), StringComparison.Ordinal);
    }

    /// <summary>Runs the tool as its command line does, then reads back the result.json it wrote.</summary>
    /// <param name="records">--records; null leaves it out (the pipeline refuses it).</param>
    private static async Task<(int Code, StressResult Result)> RunAsync(TempFolder folder, int servers, int? records = Records, string[]? extra = null)
    {
        using var output = new StringWriter();
        using var error = new StringWriter();
        var code = await StressProgram.MainAsync(
            ["--servers", $"{servers}", "--clients", $"{Clients}", .. records is null ? [] : new[] { "--records", $"{records}" }, "--ops", $"{Ops}", "--seed", "7", "--out", folder.Path, .. extra ?? []],
            output,
            error);

        Assert.True(code != StressProgram.CouldNotRun, $"{output}\n{error}");
        var json = await File.ReadAllTextAsync(Path.Combine(folder.Path, "result.json"));
        return (code, JsonSerializer.Deserialize<StressResult>(json, StressResult.JsonOptions)!);
    }
}
