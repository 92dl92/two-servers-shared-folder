using BlazorApp.Shared.Dtos;
using BlazorApp.StoreHelper;

namespace BlazorApp.Services.Tests.Storage;

/// <summary>
/// What a concurrent workload left behind, compared with what the store acknowledged.
/// Each list holds human-readable descriptions, so a failing assertion says what went wrong.
/// </summary>
/// <param name="Acks">Saves the store reported as successful.</param>
/// <param name="Conflicts">Saves refused as conflicts (and retried).</param>
/// <param name="Lost">Acknowledged markers missing from the final data: lost updates.</param>
/// <param name="WrongVersion">Acknowledged markers found at a different version than acknowledged.</param>
/// <param name="Unacknowledged">Markers in the data that no writer was told were saved.</param>
/// <param name="BrokenRecords">Records missing, or whose history isn't exactly versions 1..N with unique markers.</param>
/// <param name="BrokenReads">Store calls that failed because the stored data was broken (half-written or unreadable files).</param>
public sealed record WorkloadReport(
    int Acks,
    int Conflicts,
    int BrokenReads,
    IReadOnlyList<string> Lost,
    IReadOnlyList<string> WrongVersion,
    IReadOnlyList<string> Unacknowledged,
    IReadOnlyList<string> BrokenRecords)
{
    /// <summary>
    /// True when every acknowledged save is in the data exactly as acknowledged, nothing else is,
    /// and no read ever found the data broken.
    /// </summary>
    public bool NothingLost => BrokenReads == 0 && Lost.Count == 0 && WrongVersion.Count == 0 && Unacknowledged.Count == 0 && BrokenRecords.Count == 0;

    /// <summary>Compares acknowledged saves with the records as they are now.</summary>
    /// <param name="acks">Every acknowledged save, from all writers.</param>
    /// <param name="conflicts">Total conflicts the writers retried.</param>
    /// <param name="records">The records the workload wrote to.</param>
    /// <param name="finals">Those records as read back afterwards (null where missing).</param>
    /// <param name="brokenReads">Store calls during the workload (and the read-back) that failed on broken data.</param>
    /// <returns>The report.</returns>
    public static WorkloadReport Check(
        IReadOnlyList<Ack> acks,
        int conflicts,
        IReadOnlyList<Guid> records,
        IReadOnlyDictionary<Guid, RegistrationRecordDto?> finals,
        int brokenReads)
    {
        var lost = new List<string>();
        var wrongVersion = new List<string>();
        var unacknowledged = new List<string>();
        var broken = new List<string>();

        foreach (var id in records)
        {
            if (finals.GetValueOrDefault(id) is not { } final)
            {
                broken.Add($"{id}: missing");
                continue;
            }

            var versions = final.History.Select(h => h.Version).ToList();
            if (final.Version != final.History.Count || !versions.SequenceEqual(Enumerable.Range(1, final.Version)))
            {
                broken.Add($"{id}: version {final.Version} with history versions [{string.Join(",", versions)}]");
            }

            var stored = new Dictionary<string, int>(StringComparer.Ordinal);
            foreach (var entry in final.History.Where(h => h.Comment is not null))
            {
                if (!stored.TryAdd(entry.Comment!, entry.Version))
                {
                    broken.Add($"{id}: marker {entry.Comment} appears twice");
                }
            }

            var acked = acks.Where(a => a.RecordId == id).ToList();
            foreach (var ack in acked)
            {
                if (!stored.TryGetValue(ack.Marker, out var version))
                {
                    lost.Add($"{id}: {ack.Marker} (acknowledged as version {ack.Version})");
                }
                else if (version != ack.Version)
                {
                    wrongVersion.Add($"{id}: {ack.Marker} acknowledged as version {ack.Version}, stored as {version}");
                }
            }

            var ackedMarkers = acked.Select(a => a.Marker).ToHashSet(StringComparer.Ordinal);
            unacknowledged.AddRange(stored.Keys.Where(m => !ackedMarkers.Contains(m)).Select(m => $"{id}: {m}"));
        }

        return new WorkloadReport(acks.Count, conflicts, brokenReads, lost, wrongVersion, unacknowledged, broken);
    }

    /// <inheritdoc />
    public override string ToString() =>
        $"acks={Acks} conflicts={Conflicts} brokenReads={BrokenReads} lost={Lost.Count} wrongVersion={WrongVersion.Count} " +
        $"unacknowledged={Unacknowledged.Count} broken={BrokenRecords.Count}\n" +
        string.Join("\n", Lost.Concat(WrongVersion).Concat(Unacknowledged).Concat(BrokenRecords).Take(20));
}
