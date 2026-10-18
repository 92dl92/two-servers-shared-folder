using BlazorApp.Shared.Dtos;

namespace BlazorApp.Stress;

/// <summary>Save latency in milliseconds (nearest-rank percentiles).</summary>
/// <param name="P50">Median.</param>
/// <param name="P95">95th percentile.</param>
/// <param name="Max">Slowest.</param>
/// <param name="Mean">Average.</param>
public sealed record Latency(double P50, double P95, double Max, double Mean)
{
    /// <summary>Summarises a list of durations.</summary>
    /// <param name="milliseconds">The durations; may be empty.</param>
    /// <returns>The summary; all zero when empty.</returns>
    public static Latency From(IReadOnlyCollection<double> milliseconds)
    {
        ArgumentNullException.ThrowIfNull(milliseconds);
        if (milliseconds.Count == 0)
        {
            return new Latency(0, 0, 0, 0);
        }

        var sorted = milliseconds.Order().ToArray();
        double Percentile(double p) => sorted[Math.Max(0, (int)Math.Ceiling(p / 100 * sorted.Length) - 1)];
        return new Latency(Round(Percentile(50)), Round(Percentile(95)), Round(sorted[^1]), Round(sorted.Average()));
    }

    private static double Round(double value) => Math.Round(value, 2);
}

/// <summary>The numbers of a run (plan section 4): saves, refusals by kind, rate and latency.</summary>
/// <param name="Writes">Changes sent, creates included.</param>
/// <param name="Saves">Changes the API reported as saved.</param>
/// <param name="Conflicts">409: someone else changed the record first. The system working correctly.</param>
/// <param name="WrongStatus">422: the status didn't allow the change.</param>
/// <param name="NoResponse">Changes that got no answer; whether they were saved is unknown.</param>
/// <param name="WritesByOutcome">Every write outcome, including 404, 403 and other statuses.</param>
/// <param name="SavesByAction">Saves per action.</param>
/// <param name="Reads">Records read before changing them.</param>
/// <param name="ReadFailures">Reads that didn't return the record, by outcome.</param>
/// <param name="DurationSeconds">Wall-clock time of the workload.</param>
/// <param name="SavesPerSecond">Saves divided by the duration.</param>
/// <param name="SaveLatencyMs">How long saved changes took.</param>
/// <param name="StaleReads">Reads that returned an older version than one already acknowledged (phase 6b); null in results from before the check.</param>
/// <param name="LongestStall">The longest time no save was acknowledged (phase 6b); null in results from before it was measured.</param>
/// <param name="ReadLatencyMs">How long successful reads took (phase 6b); null in results from before it was measured.</param>
/// <param name="Lists">Record lists sent (pipeline with <c>--list</c>).</param>
/// <param name="ListLatencyMs">How long successful lists took; null when no list succeeded.</param>
/// <param name="ListFailures">Lists that didn't return the records, by outcome.</param>
public sealed record StressNumbers(
    int Writes,
    int Saves,
    int Conflicts,
    int WrongStatus,
    int NoResponse,
    IReadOnlyDictionary<string, int> WritesByOutcome,
    IReadOnlyDictionary<RecordAction, int> SavesByAction,
    int Reads,
    IReadOnlyDictionary<string, int> ReadFailures,
    double DurationSeconds,
    double SavesPerSecond,
    Latency SaveLatencyMs,
    StaleReadReport? StaleReads = null,
    Stall? LongestStall = null,
    Latency? ReadLatencyMs = null,
    int Lists = 0,
    Latency? ListLatencyMs = null,
    IReadOnlyDictionary<string, int>? ListFailures = null)
{
    /// <summary>Outcome name of a 2xx answer.</summary>
    public const string Saved = "saved";

    /// <summary>Outcome name of a 409.</summary>
    public const string Conflict = "conflict";

    /// <summary>Outcome name of a 422.</summary>
    public const string WrongStatusName = "wrongStatus";

    /// <summary>Outcome name of a request that got no answer.</summary>
    public const string NoResponseName = "noResponse";

    /// <summary>Outcome name of a request that couldn't connect (a killed server): nothing was sent, so it isn't counted as no response.</summary>
    public const string RefusedName = "refused";

    /// <summary>
    /// The longest time with no acknowledged save, and when it began: between two saves, or from
    /// the start to the first save, or from the last save to the end (a freeze the run never
    /// recovered from).
    /// </summary>
    /// <param name="savedAt">When each save's answer arrived (<see cref="System.Diagnostics.Stopwatch.GetTimestamp"/>), in any order.</param>
    /// <param name="started">When the workload started.</param>
    /// <param name="ended">When it ended.</param>
    /// <returns>The stall.</returns>
    public static Stall FindLongestStall(IReadOnlyCollection<long> savedAt, long started, long ended)
    {
        ArgumentNullException.ThrowIfNull(savedAt);
        long[] times = [started, .. savedAt.Order(), ended];
        var longest = 0;
        for (var i = 1; i < times.Length; i++)
        {
            if (times[i] - times[i - 1] > times[longest + 1] - times[longest])
            {
                longest = i - 1;
            }
        }

        return new Stall(Math.Round(Seconds(times[longest + 1] - times[longest]), 2), Math.Round(Seconds(times[longest] - started), 2));

        static double Seconds(long ticks) => (double)ticks / System.Diagnostics.Stopwatch.Frequency;
    }

    internal static StressNumbers From(IReadOnlyList<Tally> tallies, long started, StaleReadReport staleReads)
    {
        var ended = System.Diagnostics.Stopwatch.GetTimestamp();
        var elapsed = System.Diagnostics.Stopwatch.GetElapsedTime(started, ended);
        var outcomes = Sum(tallies.Select(t => t.Outcomes));
        var saves = outcomes.GetValueOrDefault(Saved);
        return new StressNumbers(
            outcomes.Values.Sum(),
            saves,
            outcomes.GetValueOrDefault(Conflict),
            outcomes.GetValueOrDefault(WrongStatusName),
            outcomes.GetValueOrDefault(NoResponseName),
            outcomes,
            Sum(tallies.Select(t => t.SavesByAction)),
            tallies.Sum(t => t.Reads),
            Sum(tallies.Select(t => t.ReadFailures)),
            Math.Round(elapsed.TotalSeconds, 3),
            elapsed > TimeSpan.Zero ? Math.Round(saves / elapsed.TotalSeconds, 2) : 0,
            Latency.From([.. tallies.SelectMany(t => t.SaveMilliseconds)]),
            staleReads,
            FindLongestStall([.. tallies.SelectMany(t => t.SavedAt)], started, ended),
            Latency.From([.. tallies.SelectMany(t => t.ReadMilliseconds)]),
            tallies.Sum(t => t.Lists),
            tallies.Any(t => t.ListMilliseconds.Count > 0) ? Latency.From([.. tallies.SelectMany(t => t.ListMilliseconds)]) : null,
            Sum(tallies.Select(t => t.ListFailures)));
    }

    private static SortedDictionary<TKey, int> Sum<TKey>(IEnumerable<Dictionary<TKey, int>> counts)
        where TKey : notnull
    {
        var total = new SortedDictionary<TKey, int>();
        foreach (var (key, count) in counts.SelectMany(c => c))
        {
            total[key] = total.GetValueOrDefault(key) + count;
        }

        return total;
    }
}

/// <summary>
/// The longest time no save was acknowledged by any server (phase 6b: a share that freezes shows
/// here). In the pipeline it includes the clients' think time, so compare it with that.
/// </summary>
/// <param name="Seconds">How long.</param>
/// <param name="AfterSeconds">When it began, in seconds from the start of the workload.</param>
public sealed record Stall(double Seconds, double AfterSeconds);
