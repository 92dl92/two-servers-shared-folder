using System.Diagnostics;

namespace BlazorApp.Stress;

/// <summary>
/// A read that returned an older version than one already acknowledged before the read was sent.
/// </summary>
/// <remarks>A read that found no record at all has <see cref="VersionRead"/> 0.</remarks>
/// <param name="RecordId">The record read.</param>
/// <param name="Server">The server that answered the read.</param>
/// <param name="VersionRead">The version it returned.</param>
/// <param name="VersionAcknowledged">The newest version acknowledged before the read was sent.</param>
/// <param name="AcknowledgedBy">The server that acknowledged that version.</param>
/// <param name="MillisecondsSinceAck">How long before the read was sent that version was acknowledged.</param>
public sealed record StaleRead(
    Guid RecordId,
    string Server,
    int VersionRead,
    int VersionAcknowledged,
    string AcknowledgedBy,
    double MillisecondsSinceAck);

/// <summary>What the stale-read check found.</summary>
/// <param name="Count">Stale reads.</param>
/// <param name="ReadsAfterAnotherServer">Reads whose newest acknowledged version came from another
/// server: the handoffs where a server's own view could be out of date.</param>
/// <param name="Samples">The first stale reads, for finding them in the logs.</param>
public sealed record StaleReadReport(int Count, int ReadsAfterAnotherServer, IReadOnlyList<StaleRead> Samples);

/// <summary>
/// The stale-read check (plan phase 6b): every acknowledged save is noted with the time its
/// answer arrived, and every read is compared with the newest version acknowledged before it
/// was sent. On a share whose clients cache, a server can show a user an old version for a
/// while after another server saved a newer one. Nothing is lost by that alone (a change sent
/// from the old version gets a 409), so it is reported rather than failing the run.
/// </summary>
/// <remarks>Shared by every client, so it locks; times are <see cref="Stopwatch"/> timestamps
/// on this machine, where the clients run.</remarks>
public sealed class StaleReadCheck
{
    /// <summary>How many stale reads are kept as samples.</summary>
    public const int MaxSamples = 20;

    private readonly Dictionary<Guid, List<(int Version, string Server, long At)>> _acks = [];
    private readonly List<StaleRead> _samples = [];
    private readonly Lock _lock = new();
    private int _count;
    private int _afterAnotherServer;

    /// <summary>Notes an acknowledged save.</summary>
    /// <param name="recordId">The record.</param>
    /// <param name="version">The version the answer returned.</param>
    /// <param name="server">The server that answered.</param>
    /// <param name="answeredAt">When the answer arrived (<see cref="Stopwatch.GetTimestamp"/>).</param>
    public void Acknowledged(Guid recordId, int version, string server, long answeredAt)
    {
        lock (_lock)
        {
            if (!_acks.TryGetValue(recordId, out var list))
            {
                _acks[recordId] = list = [];
            }

            list.Add((version, server, answeredAt));
        }
    }

    /// <summary>Checks a successful read.</summary>
    /// <param name="recordId">The record.</param>
    /// <param name="version">The version returned.</param>
    /// <param name="server">The server that answered.</param>
    /// <param name="sentAt">When the read was sent (<see cref="Stopwatch.GetTimestamp"/>).</param>
    public void Read(Guid recordId, int version, string server, long sentAt) => Check(recordId, version, server, sentAt);

    /// <summary>
    /// Checks a read that found no record: stale if its create was acknowledged before the read
    /// was sent (as a cached "not found" on a share would give).
    /// </summary>
    /// <param name="recordId">The record.</param>
    /// <param name="server">The server that answered.</param>
    /// <param name="sentAt">When the read was sent (<see cref="Stopwatch.GetTimestamp"/>).</param>
    public void NotFound(Guid recordId, string server, long sentAt) => Check(recordId, 0, server, sentAt);

    private void Check(Guid recordId, int version, string server, long sentAt)
    {
        lock (_lock)
        {
            if (!_acks.TryGetValue(recordId, out var list))
            {
                return;
            }

            // Only saves whose answer had arrived before the read was sent count: a save still
            // under way may or may not show.
            (int Version, string Server, long At)? newest = null;
            foreach (var ack in list)
            {
                if (ack.At < sentAt && (newest is null || ack.Version > newest.Value.Version))
                {
                    newest = ack;
                }
            }

            if (newest is not { } known)
            {
                return;
            }

            if (known.Server != server)
            {
                _afterAnotherServer++;
            }

            if (version < known.Version)
            {
                _count++;
                if (_samples.Count < MaxSamples)
                {
                    _samples.Add(new StaleRead(
                        recordId, server, version, known.Version, known.Server,
                        Math.Round(Stopwatch.GetElapsedTime(known.At, sentAt).TotalMilliseconds, 1)));
                }
            }
        }
    }

    /// <summary>What was found.</summary>
    /// <returns>The report.</returns>
    public StaleReadReport Report()
    {
        lock (_lock)
        {
            return new StaleReadReport(_count, _afterAnotherServer, [.. _samples]);
        }
    }
}
