using BlazorApp.Shared.Dtos;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlazorApp.Stress;

/// <summary>The settings a run used, recorded with its results.</summary>
/// <param name="Strategy">Storage strategy.</param>
/// <param name="DataPath">The shared data folder.</param>
/// <param name="Servers">Server processes.</param>
/// <param name="ClientsPerServer">Clients per server.</param>
/// <param name="ClientsByRole">How many clients have each role.</param>
/// <param name="Records">Records asked for; registrants create them, so a failed create leaves fewer. For the pipeline, the records created.</param>
/// <param name="OpsPerClient">Changes each client attempted.</param>
/// <param name="Seed">Seed of the clients' choices.</param>
/// <param name="ServerArgs">Extra server arguments.</param>
/// <param name="Remotes">The other machines that ran servers, with the folder as each saw it; null when every server ran here.</param>
/// <param name="RemoteServers">How many servers ran on each of them.</param>
/// <param name="DataCopied">Whether <paramref name="DataPath"/> is a copy of the shared folder, made from the first remote after the servers stopped (no server ran here).</param>
/// <param name="Workload">The workload: contention or pipeline.</param>
/// <param name="ThinkMilliseconds">Pipeline: the mean pause before each step.</param>
/// <param name="Prefill">Finished records put in the folder before the run.</param>
public sealed record StressSettings(
    string Strategy,
    string DataPath,
    int Servers,
    int ClientsPerServer,
    IReadOnlyDictionary<Role, int> ClientsByRole,
    int Records,
    int OpsPerClient,
    int Seed,
    IReadOnlyList<string> ServerArgs,
    IReadOnlyList<RemoteTarget>? Remotes = null,
    int RemoteServers = 0,
    bool DataCopied = false,
    string Workload = StressOptions.Contention,
    int? ThinkMilliseconds = null,
    int Prefill = 0);

/// <summary>Where the run happened.</summary>
/// <param name="Name">Machine name.</param>
/// <param name="Os">Operating system.</param>
/// <param name="ProcessorCount">Logical processors.</param>
/// <param name="Runtime">.NET runtime.</param>
public sealed record MachineInfo(string Name, string Os, int ProcessorCount, string Runtime)
{
    /// <summary>This machine.</summary>
    public static MachineInfo Current => new(
        Environment.MachineName,
        System.Runtime.InteropServices.RuntimeInformation.OSDescription,
        Environment.ProcessorCount,
        System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription);
}

/// <summary>One server of the run.</summary>
/// <param name="Port">Its port, which names it in acks and reports.</param>
/// <param name="LogFile">Its JSON-lines log (a remote server's is copied back here after the run).</param>
/// <param name="Host">The remote machine it ran on; null for this one.</param>
public sealed record ServerInfo(int Port, string LogFile, string? Host = null);

/// <summary>One read-back of the data and whether it passed.</summary>
/// <param name="Passed">Whether the checks passed (or the source was skipped).</param>
/// <param name="Report">The details.</param>
public sealed record SourceCheck(bool Passed, SourceReport Report);

/// <summary>Everything a run produced: written as <c>result.json</c>, summarised in <c>summary.md</c>.</summary>
/// <param name="Passed">Whether every check passed.</param>
/// <param name="Commit">The commit the tool was built from (<c>-dirty</c> with uncommitted changes).</param>
/// <param name="StartedAt">When the run started.</param>
/// <param name="FinishedAt">When it finished.</param>
/// <param name="Machine">Where it ran.</param>
/// <param name="Settings">What it was asked to do.</param>
/// <param name="RunFolder">Where its data, logs and results are.</param>
/// <param name="Servers">The servers it started.</param>
/// <param name="Numbers">Saves, refusals, rate and latency.</param>
/// <param name="Checks">The checks on each read-back: the folder through the strategy, then each server's API.</param>
/// <param name="Faults">The failures caused on purpose and what they did; null without <c>--fault</c>.</param>
/// <param name="LockCheck">Whether every record could still be changed afterwards; null without <c>--fault</c>.</param>
public sealed record StressResult(
    bool Passed,
    string Commit,
    DateTimeOffset StartedAt,
    DateTimeOffset FinishedAt,
    MachineInfo Machine,
    StressSettings Settings,
    string RunFolder,
    IReadOnlyList<ServerInfo> Servers,
    StressNumbers Numbers,
    IReadOnlyList<SourceCheck> Checks,
    IReadOnlyList<FaultOutcome>? Faults = null,
    LockReport? LockCheck = null)
{
    /// <summary>How <c>result.json</c> is written: indented, enums as names.</summary>
    public static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() },
    };

    /// <summary>The result as JSON.</summary>
    /// <returns>Indented JSON.</returns>
    public string ToJson() => JsonSerializer.Serialize(this, JsonOptions);

    /// <summary>A short human-readable summary in Markdown, ending with a row for the results table in <c>docs/experiments/</c>.</summary>
    /// <returns>The summary.</returns>
    public string ToSummary()
    {
        var n = Numbers;
        var s = Settings;
        var text = new StringBuilder();
        void Line(FormattableString line) => text.AppendLine(line.ToString(CultureInfo.InvariantCulture));

        Line($"# Stress run: {s.Strategy}, {s.Servers} server(s): {(Passed ? "PASSED" : "FAILED")}");
        Line($"");
        Line($"- Commit `{Commit}`, {StartedAt:yyyy-MM-dd HH:mm:ss} UTC, {Numbers.DurationSeconds:0.0} s");
        Line($"- Machine {Machine.Name}: {Machine.Os}, {Machine.ProcessorCount} CPUs, {Machine.Runtime}");
        Line($"- Settings: {s.Servers} servers × {s.ClientsPerServer} clients, {(s.Workload == StressOptions.Pipeline ? $"pipeline with {s.ThinkMilliseconds} ms think time" : $"{s.Records} records")}, {s.OpsPerClient} changes per client, seed {s.Seed}");
        Line($"- Clients by role: {string.Join(", ", s.ClientsByRole.Select(r => $"{r.Value} {r.Key}"))}");
        Line($"- Data folder `{s.DataPath}`{(s.DataCopied ? $" (a copy of the shared folder, made on `{s.Remotes![0].Target}` after the servers stopped)" : "")}; run folder `{RunFolder}`");
        foreach (var remote in s.Remotes ?? [])
        {
            Line($"- {s.RemoteServers} of the servers on `{remote.Target}`, data folder `{remote.DataPath}` there");
        }

        if (s.ServerArgs.Count > 0)
        {
            Line($"- Server arguments: `{string.Join(" ", s.ServerArgs)}`");
        }

        Line($"");
        Line($"| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |");
        Line($"|---|---|---|---|---|---|---|---|---|");
        Line($"| {n.Writes} | {n.Saves} | {n.SavesPerSecond:0.0} | {n.Conflicts} | {n.WrongStatus} | {n.NoResponse} | {n.SaveLatencyMs.P50:0.0} | {n.SaveLatencyMs.P95:0.0} | {n.SaveLatencyMs.Max:0.0} |");
        if (n.ReadLatencyMs is { } reads)
        {
            Line($"");
            var listFailures = n.ListFailures is { Count: > 0 } failed ? $", failed: {string.Join(", ", failed.Select(f => $"{f.Key} {f.Value}"))}" : "";
            Line($"Read p50/p95/max ms: {reads.P50:0.0} / {reads.P95:0.0} / {reads.Max:0.0}{(n.Lists > 0 ? $". List p50/p95/max ms ({n.Lists} lists{listFailures}): {(n.ListLatencyMs is { } lists ? $"{lists.P50:0.0} / {lists.P95:0.0} / {lists.Max:0.0}" : "none succeeded")}" : "")}{(s.Prefill > 0 ? $". Prefilled with {s.Prefill} records" : "")}.");
        }
        Line($"");
        var outcomes = string.Join(", ", n.WritesByOutcome.Select(o => $"{o.Key} {o.Value}"));
        var actions = string.Join(", ", n.SavesByAction.Select(o => $"{o.Key} {o.Value}"));
        var readFailures = n.ReadFailures.Count > 0 ? $" ({string.Join(", ", n.ReadFailures.Select(o => $"{o.Key} {o.Value}"))})" : "";
        Line($"Writes by outcome: {outcomes}. Saves by action: {actions}. Reads: {n.Reads}{readFailures}.");
        if (n.LongestStall is { } stall)
        {
            Line($"");
            Line($"Longest stall: {stall.Seconds:0.0} s with no save acknowledged, from {stall.AfterSeconds:0.0} s into the run{(s.ThinkMilliseconds is { } think ? $" (think time {think} ms, so pauses up to about {think * 1.5 / 1000:0.0} s are normal)" : "")}.");
        }
        if (n.StaleReads is { } stale)
        {
            Line($"");
            Line($"Stale reads: {stale.Count} (older than a version already acknowledged); {stale.ReadsAfterAnotherServer} reads followed a save through another server.");
            foreach (var r in stale.Samples)
            {
                Line($"- record {r.RecordId} through {r.Server}: version {r.VersionRead}, {r.MillisecondsSinceAck:0.0} ms after version {r.VersionAcknowledged} was acknowledged by {r.AcknowledgedBy}");
            }
        }
        Line($"");
        Line($"| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |");
        Line($"|---|---|---|---|---|---|---|---|");
        foreach (var (passed, r) in Checks)
        {
            var result = r.Skipped is not null ? $"skipped: {r.Skipped}" : r.Error is not null ? "UNREADABLE" : passed ? "pass" : "FAIL";
            Line($"| {r.Source} | {result} | {r.RecordsFound}/{r.RecordsExpected} | {r.MissingRecords} | {r.LostUpdates} | {r.WrongVersion} | {r.Unacknowledged} | {r.RuleViolations} |");
        }

        foreach (var (_, r) in Checks.Where(c => !c.Passed))
        {
            Line($"");
            Line($"{r.Source}:");
            foreach (var problem in (r.Error is null ? [] : new[] { $"unreadable: {r.Error}" }).Concat(r.Examples))
            {
                Line($"- {problem}");
            }
        }

        if (Faults is not null)
        {
            Line($"");
            Line($"Faults:");
            foreach (var f in Faults)
            {
                Line($"- {f.Fault}: {(f.AtSeconds is { } at ? $"at {at:0.0} s, {f.Result}" : f.Result)}");
            }
        }

        if (LockCheck is { } locks)
        {
            Line($"");
            Line($"Lock check: {(locks.Passed ? "pass" : "FAIL")}: {locks.Changed} of {locks.Changes} record and server pairs could still be changed, {locks.Stuck} stuck, {locks.Unreadable} unreadable{(locks.StoppedEarly ? $"; stopped after {BlazorApp.Stress.LockCheck.StopAfterStuck} stuck, the rest untried" : "")}.");
            foreach (var example in locks.Examples)
            {
                Line($"- {example}");
            }
        }

        var lost = Checks.Max(c => c.Report.LostUpdates);
        var rules = Checks.Max(c => c.Report.RuleViolations);
        var unreadable = Checks.Count(c => c.Report.Error is not null);
        Line($"");
        Line($"Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):");
        Line($"");
        Line($"| {s.Strategy} | {s.Servers} | {n.Saves} | {n.SavesPerSecond:0.0} | {n.Conflicts} | {n.WrongStatus} | {lost} | {unreadable} | {rules} | {n.SaveLatencyMs.P95:0.0} | {(Passed ? "yes" : "no")} |");
        return text.ToString();
    }
}
