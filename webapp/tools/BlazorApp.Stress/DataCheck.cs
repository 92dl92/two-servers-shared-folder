using BlazorApp.Shared.Dtos;
using BlazorApp.Shared.Rules;

namespace BlazorApp.Stress;

/// <summary>
/// The checks on one read-back of the data (the folder read through the strategy, or one
/// server's API). Lists hold the first <see cref="DataCheck.MaxExamples"/> descriptions of
/// each problem; the counts are complete.
/// </summary>
public sealed record SourceReport
{
    /// <summary>Where the data was read from, e.g. <c>storage</c> or <c>server 5081</c>.</summary>
    public required string Source { get; init; }

    /// <summary>Why this source wasn't checked (e.g. a strategy without a folder); null when it was.</summary>
    public string? Skipped { get; init; }

    /// <summary>Why the data couldn't be read at all (a broken file, a failing server); null when it could.</summary>
    public string? Error { get; init; }

    /// <summary>Records the workload created.</summary>
    public int RecordsExpected { get; init; }

    /// <summary>Records found in this source.</summary>
    public int RecordsFound { get; init; }

    /// <summary>Records found that the workload didn't create (e.g. a reused data folder); checked for rules only.</summary>
    public int RecordsUnexpected { get; init; }

    /// <summary>Created records missing from this source.</summary>
    public int MissingRecords { get; init; }

    /// <summary>Acknowledged changes not in the data at the version acknowledged.</summary>
    public int LostUpdates { get; init; }

    /// <summary>Acknowledged comments found, but at another version than acknowledged.</summary>
    public int WrongVersion { get; init; }

    /// <summary>History entries no acknowledged change accounts for.</summary>
    public int Unacknowledged { get; init; }

    /// <summary>Broken histories and status changes or users the workflow rules don't allow.</summary>
    public int RuleViolations { get; init; }

    /// <summary>Examples of each problem, for the report.</summary>
    public IReadOnlyList<string> Examples { get; init; } = [];

    /// <summary>
    /// Whether this source passed: readable, nothing missing or lost, rules respected, and no
    /// more unacknowledged entries than requests whose outcome is unknown (a request that
    /// failed in transit may still have been saved).
    /// </summary>
    /// <param name="unknownOutcomes">Write requests that got no HTTP response.</param>
    /// <returns>True if the source passed or was skipped.</returns>
    public bool Passed(int unknownOutcomes) =>
        Skipped is not null
        || (Error is null && MissingRecords == 0 && LostUpdates == 0 && WrongVersion == 0 && RuleViolations == 0
            && Unacknowledged <= unknownOutcomes);
}

/// <summary>Compares acknowledged changes with the records read back, and checks every history against <c>RecordRules</c>.</summary>
public static class DataCheck
{
    /// <summary>How many examples a report keeps.</summary>
    public const int MaxExamples = 20;

    /// <summary>Checks one read-back of the data.</summary>
    /// <param name="source">Where the data came from, for the report.</param>
    /// <param name="acks">Every acknowledged change, creates included.</param>
    /// <param name="created">Ids of the records the workload created.</param>
    /// <param name="records">Every record read back.</param>
    /// <param name="users">The users the servers know, by id.</param>
    /// <param name="prefilled">Records put in the folder before the run, with the version each was left at: each must still be there, at that version (nothing in the workload changes them).</param>
    /// <param name="listedOnly">Records the source listed but didn't read in full, with their listed versions (a server's read-back reads in full only the records the run changed).</param>
    /// <returns>The report.</returns>
    public static SourceReport Check(
        string source,
        IReadOnlyCollection<Ack> acks,
        IReadOnlyCollection<Guid> created,
        IReadOnlyCollection<RegistrationRecordDto> records,
        IReadOnlyDictionary<string, UserDto> users,
        IReadOnlyDictionary<Guid, int>? prefilled = null,
        IReadOnlyDictionary<Guid, int>? listedOnly = null)
    {
        prefilled ??= new Dictionary<Guid, int>();
        listedOnly ??= new Dictionary<Guid, int>();
        ArgumentNullException.ThrowIfNull(acks);
        ArgumentNullException.ThrowIfNull(created);
        ArgumentNullException.ThrowIfNull(records);
        ArgumentNullException.ThrowIfNull(users);

        var missing = new Problems();
        var lost = new Problems();
        var wrongVersion = new Problems();
        var unacknowledged = new Problems();
        var rules = new Problems();

        var byId = records.GroupBy(r => r.Id).ToDictionary(g => g.Key, g => g.First());
        var expected = created.ToHashSet();
        foreach (var duplicate in records.GroupBy(r => r.Id).Where(g => g.Count() > 1))
        {
            rules.Add($"{duplicate.Key}: listed {duplicate.Count()} times");
        }

        var acksById = acks.ToLookup(a => a.RecordId);
        foreach (var id in expected.Concat(acksById.Select(g => g.Key)).Distinct())
        {
            if (!byId.TryGetValue(id, out var record))
            {
                missing.Add($"{id}: missing ({acksById[id].Count()} acknowledged changes)");
                foreach (var ack in acksById[id])
                {
                    lost.Add($"{Describe(ack)}: record missing");
                }

                continue;
            }

            CheckAcks(record, [.. acksById[id]], lost, wrongVersion, unacknowledged);
        }

        foreach (var (id, version) in prefilled)
        {
            int? found = byId.TryGetValue(id, out var record) ? record.Version : listedOnly.TryGetValue(id, out var listed) ? listed : null;
            if (found is null)
            {
                missing.Add($"{id}: prefilled record missing");
            }
            else if (found != version)
            {
                lost.Add($"{id}: prefilled at version {version}, found at version {found}");
            }
        }

        foreach (var record in byId.Values)
        {
            CheckRules(record, users, rules);
        }

        return new SourceReport
        {
            Source = source,
            RecordsExpected = expected.Count + prefilled.Keys.Count(id => !expected.Contains(id)),
            RecordsFound = byId.Keys.Union(listedOnly.Keys).Count(),
            RecordsUnexpected = byId.Keys.Union(listedOnly.Keys).Count(id => !expected.Contains(id) && !prefilled.ContainsKey(id)),
            MissingRecords = missing.Count,
            LostUpdates = lost.Count,
            WrongVersion = wrongVersion.Count,
            Unacknowledged = unacknowledged.Count,
            RuleViolations = rules.Count,
            Examples =
            [
                .. missing.Examples.Select(e => $"missing record {e}"),
                .. lost.Examples.Select(e => $"lost update {e}"),
                .. wrongVersion.Examples.Select(e => $"wrong version {e}"),
                .. unacknowledged.Examples.Select(e => $"unacknowledged {e}"),
                .. rules.Examples.Select(e => $"rule broken {e}"),
            ],
        };
    }

    /// <summary>
    /// Every ack must be the history entry at its version, and the form data must be the last
    /// acknowledged update's. An entry no ack matched is unacknowledged, unless it is a comment
    /// whose ack is reported at another version.
    /// </summary>
    private static void CheckAcks(
        RegistrationRecordDto record, IReadOnlyList<Ack> acks, Problems lost, Problems wrongVersion, Problems unacknowledged)
    {
        var entries = record.History.ToLookup(h => h.Version);
        var comments = record.History.Where(h => h.Comment is not null).ToLookup(h => h.Comment!, StringComparer.Ordinal);
        var matched = new HashSet<HistoryEntryDto>(ReferenceEqualityComparer.Instance);
        var misplaced = new HashSet<string>(StringComparer.Ordinal);
        foreach (var ack in acks)
        {
            if (entries[ack.Version].FirstOrDefault(h => !matched.Contains(h) && Matches(ack, h)) is { } entry)
            {
                matched.Add(entry);
                continue;
            }

            if (ack is { Action: RecordAction.Comment, Marker: { } marker } && comments[marker].FirstOrDefault() is { } elsewhere)
            {
                misplaced.Add(marker);
                wrongVersion.Add($"{Describe(ack)}: stored as version {elsewhere.Version}");
            }
            else
            {
                var found = entries[ack.Version].FirstOrDefault();
                lost.Add(found is null
                    ? $"{Describe(ack)}: no history entry at that version"
                    : $"{Describe(ack)}: version {ack.Version} holds {found.Action} by {found.UserId}");
            }
        }

        foreach (var entry in record.History.Where(h => !matched.Contains(h)))
        {
            if (entry.Comment is null || !misplaced.Remove(entry.Comment))
            {
                unacknowledged.Add($"{record.Id} v{entry.Version}: {entry.Action} by {entry.UserId}");
            }
        }

        // The data must be the last update's: its history entry alone doesn't show the data was written.
        var lastUpdate = record.History.LastOrDefault(h => h.Action == RecordAction.Update);
        var lastUpdateAck = lastUpdate is null ? null : acks.FirstOrDefault(a => Matches(a, lastUpdate));
        if (lastUpdateAck?.Marker is { } notes && !string.Equals(record.Data?.Notes, notes, StringComparison.Ordinal))
        {
            lost.Add($"{Describe(lastUpdateAck)}: the form data is from another update (notes '{record.Data?.Notes}')");
        }
    }

    /// <summary>
    /// The history must be versions 1..N with N the record's version, start with a Create by
    /// the owner, and replay through <c>RecordRules</c>: every change allowed for its user and
    /// for the status at that point, ending in the record's status.
    /// </summary>
    private static void CheckRules(RegistrationRecordDto record, IReadOnlyDictionary<string, UserDto> users, Problems rules)
    {
        var id = record.Id;
        var versions = record.History.Select(h => h.Version).ToList();
        if (record.Version != versions.Count || !versions.SequenceEqual(Enumerable.Range(1, versions.Count)))
        {
            rules.Add($"{id}: version {record.Version} with history versions [{string.Join(",", versions)}]");
        }

        if (record.History.Count == 0)
        {
            return;
        }

        var first = record.History[0];
        if (first.Action != RecordAction.Create || first.UserId != record.CreatedBy)
        {
            rules.Add($"{id}: history starts with {first.Action} by {first.UserId}, not Create by owner {record.CreatedBy}");
        }

        var status = RecordStatus.Draft;
        foreach (var entry in record.History.Skip(1))
        {
            var user = users.GetValueOrDefault(entry.UserId);
            if (entry.Action == RecordAction.Create || !RecordRules.StatusAllows(entry.Action, status))
            {
                rules.Add($"{id} v{entry.Version}: {entry.Action} of a {status} record");
            }
            else if (RecordRules.CheckPermission(user, entry.Action, record.CreatedBy) != RuleOutcome.Allowed)
            {
                rules.Add($"{id} v{entry.Version}: {entry.Action} by {entry.UserId}, who may not");
            }

            status = RecordRules.StatusAfter(entry.Action, status);
        }

        if (record.Status != status)
        {
            rules.Add($"{id}: status {record.Status}, but its history leads to {status}");
        }
    }

    private static bool Matches(Ack ack, HistoryEntryDto entry) =>
        entry.Version == ack.Version && entry.Action == ack.Action && entry.UserId == ack.UserId
        && (ack.Action != RecordAction.Comment || string.Equals(entry.Comment, ack.Marker, StringComparison.Ordinal));

    private static string Describe(Ack ack) =>
        $"{ack.RecordId} v{ack.Version} {ack.Action} by {ack.UserId}{(ack.Marker is null ? "" : $" '{ack.Marker}'")} via {ack.Server} ({ack.CorrelationId})";

    /// <summary>A count of problems plus the first few descriptions.</summary>
    private sealed class Problems
    {
        private readonly List<string> _examples = [];

        public int Count { get; private set; }

        public IEnumerable<string> Examples => _examples;

        public void Add(string description)
        {
            Count++;
            if (_examples.Count < MaxExamples)
            {
                _examples.Add(description);
            }
        }
    }
}
