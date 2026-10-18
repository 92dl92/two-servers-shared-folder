using System.Diagnostics;
using BlazorApp.Services.Storage;
using BlazorApp.Shared.Dtos;

namespace BlazorApp.StoreHelper;

/// <summary>A save the store reported as successful: this marker is now in this record at this version.</summary>
/// <param name="RecordId">The record written.</param>
/// <param name="Version">The version written.</param>
/// <param name="Marker">Unique text written into the new history entry, e.g. <c>p2:17</c>.</param>
public sealed record Ack(Guid RecordId, int Version, string Marker);

/// <summary>Counts from one run of <see cref="StoreWorkload.RunAsync"/>.</summary>
/// <param name="Acks">Saves the store reported as successful.</param>
/// <param name="Conflicts">Saves refused because someone else wrote first (each was retried).</param>
/// <param name="BrokenReads">Store calls that failed because the stored data was broken (see <see cref="StoreWorkload.IsBrokenData"/>).</param>
public sealed record WorkloadResult(int Acks, int Conflicts, int BrokenReads);

/// <summary>
/// The read-modify-write loop several writers run against one store at the same time:
/// read a record, add a history entry carrying a unique marker, save it with the version that
/// was read, and retry on a conflict. Every acknowledged marker must survive.
/// </summary>
/// <remarks>
/// A store call that fails on broken data (<see cref="IsBrokenData"/>, e.g. a half-written file)
/// is counted as a broken read and the change is retried, so a store that breaks its files is
/// reported rather than crashing the run. A correct store never has any.
/// </remarks>
public static class StoreWorkload
{
    /// <summary>Gives up on one change after this many conflicts in a row, so a broken store can't hang a test.</summary>
    public const int MaxAttemptsPerChange = 10_000;

    /// <summary>
    /// Stops the writer once it has seen nothing but broken reads for this long, so a file that
    /// stays broken can't hang a test. Long enough for writers in other processes to have started.
    /// </summary>
    public static readonly TimeSpan GiveUpOnBrokenDataAfter = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Whether an exception from a store means the stored data was broken: not valid JSON, or
    /// shorter than expected (a half-written or truncated file). Other errors, such as a missing
    /// folder, are not broken data and still stop the workload.
    /// </summary>
    /// <param name="exception">What the store threw.</param>
    /// <returns>True for <see cref="System.Text.Json.JsonException"/> and <see cref="EndOfStreamException"/>.</returns>
    public static bool IsBrokenData(Exception exception) => exception is System.Text.Json.JsonException or EndOfStreamException;

    /// <summary>Runs the loop.</summary>
    /// <param name="store">The store to write to.</param>
    /// <param name="records">Records to take turns on; they must already exist.</param>
    /// <param name="writer">This writer's name, used in markers and as the user id.</param>
    /// <param name="iterations">How many changes to make.</param>
    /// <param name="onAck">Called for every save the store reports as successful.</param>
    /// <param name="cancellationToken">Stops the loop.</param>
    /// <returns>
    /// How many saves succeeded, how many conflicts were retried, and how many store calls failed on
    /// broken data. After <see cref="GiveUpOnBrokenDataAfter"/> of nothing but broken reads the
    /// writer stops early and returns what it has.
    /// </returns>
    /// <exception cref="InvalidOperationException">A record is missing, or a change kept conflicting.</exception>
    public static async Task<WorkloadResult> RunAsync(
        IRegistrationStore store,
        IReadOnlyList<Guid> records,
        string writer,
        int iterations,
        Action<Ack> onAck,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(store);
        ArgumentNullException.ThrowIfNull(records);
        ArgumentNullException.ThrowIfNull(onAck);

        var acks = 0;
        var conflicts = 0;
        var brokenReads = 0;
        Stopwatch? brokenSince = null;
        for (var i = 0; i < iterations; i++)
        {
            var id = records[i % records.Count];
            var marker = $"{writer}:{i}";
            var changeConflicts = 0;
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                RegistrationRecordDto updated;
                WriteResult result;
                try
                {
                    var current = await store.GetAsync(id, cancellationToken)
                        ?? throw new InvalidOperationException($"Record {id} is missing.");
                    updated = Next(current, writer, marker);
                    result = await store.TryReplaceAsync(updated, current.Version, cancellationToken);
                }
                catch (Exception e) when (IsBrokenData(e))
                {
                    // Not acknowledged, so it can't count as lost. Retry: the writer that broke it may finish.
                    brokenReads++;
                    brokenSince ??= Stopwatch.StartNew();
                    if (brokenSince.Elapsed >= GiveUpOnBrokenDataAfter)
                    {
                        return new WorkloadResult(acks, conflicts, brokenReads);
                    }

                    await Task.Yield();
                    continue;
                }

                brokenSince = null;
                if (result == WriteResult.Saved)
                {
                    acks++;
                    onAck(new Ack(id, updated.Version, marker));
                    break;
                }

                if (result != WriteResult.Conflict)
                {
                    throw new InvalidOperationException($"Saving {marker} to {id} returned {result}.");
                }

                conflicts++;
                if (++changeConflicts >= MaxAttemptsPerChange)
                {
                    throw new InvalidOperationException($"Gave up on {marker} after {changeConflicts} conflicts.");
                }
            }
        }

        return new WorkloadResult(acks, conflicts, brokenReads);
    }

    /// <summary>The next version of <paramref name="current"/>, with a history entry carrying <paramref name="marker"/>.</summary>
    private static RegistrationRecordDto Next(RegistrationRecordDto current, string writer, string marker)
    {
        var version = current.Version + 1;
        var now = DateTimeOffset.UtcNow;
        return current with
        {
            Version = version,
            UpdatedBy = writer,
            UpdatedAt = now,
            History = [.. current.History, new HistoryEntryDto(version, RecordAction.Comment, writer, now, marker)],
        };
    }

    /// <summary>Builds a fresh record at version 1 for a workload to write to.</summary>
    /// <param name="id">Record id.</param>
    /// <returns>A draft with valid data and a single Create history entry.</returns>
    public static RegistrationRecordDto Seed(Guid id)
    {
        var now = DateTimeOffset.UtcNow;
        return new RegistrationRecordDto(
            id,
            1,
            RecordStatus.Draft,
            "seed",
            now,
            "seed",
            now,
            new RegistrationDto
            {
                FirstName = "Seed",
                LastName = "Record",
                Email = "seed@example.com",
                Street = "1 Test Street",
                City = "Testville",
                PostalCode = "00000",
                Country = "Nowhere",
            },
            [new HistoryEntryDto(1, RecordAction.Create, "seed", now)]);
    }
}
