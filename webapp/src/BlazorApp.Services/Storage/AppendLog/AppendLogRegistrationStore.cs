using System.Buffers;
using System.Diagnostics;
using System.Text.Json;
using BlazorApp.Shared.Dtos;
using Microsoft.Extensions.Logging;
using Microsoft.Win32.SafeHandles;

namespace BlazorApp.Services.Storage.AppendLog;

/// <summary>
/// Strategy D: an append-only log. Every create, replace and delete is added as one JSON line to
/// <see cref="LogFileName"/> in the data folder, and the current state is rebuilt by replaying
/// the log. Nothing is ever rewritten in place.
/// </summary>
/// <remarks>
/// <para><b>Conditional appends.</b> A writer holds an exclusive lock on <see cref="LockFileName"/>
/// (opened with <see cref="FileShare.None"/>, which .NET turns into an OS lock other processes
/// respect) while it replays the log to the end, checks the version and appends its line. So the
/// check and the append are one step for every process using the folder, and appends never
/// overlap. Readers take no lock.</para>
/// <para><b>Replay rule.</b> A line is applied only if it passes the same check its writer made
/// (<see cref="Check"/>): a create of a new id, or a replace or delete of the version currently
/// replayed. Under the lock every acknowledged line passes; a line that doesn't (only possible if
/// something bypassed the lock) is skipped and counted, so all instances agree on the state.</para>
/// <para><b>Torn lines.</b> A line is complete only once its newline is written. Readers ignore an
/// incomplete last line, since a writer may still be writing it. A writer holding the lock knows
/// nobody is writing, so an incomplete last line was left by a crash: it first seals it with a
/// newline and replays it (normally it no longer parses and is skipped), then appends its own.</para>
/// <para><b>No stale state.</b> The replayed state is kept between calls, together with how many
/// bytes of the log it covers. A read first checks the log's size (a stat) and replays only the
/// lines added since; a writer, holding the lock, always opens the log and takes its length from
/// the open file rather than from a possibly cached stat. A log that got shorter or was deleted
/// is replayed from the start. A log replaced by one of the same or greater length is not
/// noticed: the store never rewrites the log, and replacing it under running servers is not
/// supported.</para>
/// <para><b>Durability.</b> <see cref="WriteResult.Saved"/> means the line is in the file, so it
/// survives the process crashing. It is not flushed to the disk itself (no fsync), so a power cut
/// can still lose the most recent lines.</para>
/// </remarks>
public sealed class AppendLogRegistrationStore : IRegistrationStore
{
    /// <summary>The log's file name in the data folder.</summary>
    public const string LogFileName = "registrations.jsonl";

    /// <summary>The file writers lock while they check and append.</summary>
    public const string LockFileName = "registrations.lock";

    /// <summary><see cref="RecordJson.Options"/> on one line, so each entry is one line.</summary>
    private static readonly JsonSerializerOptions LineOptions = new(RecordJson.Options) { WriteIndented = false };

    /// <summary>How much of the log is read at a time.</summary>
    private const int ReadChunkSize = 1 << 16;

    private readonly string _logPath;
    private readonly string _lockPath;
    private readonly ILogger _logger;

    /// <summary>One writer at a time per instance, so threads don't spin on the file lock.</summary>
    private readonly SemaphoreSlim _writeGate = new(1, 1);

    /// <summary>Guards the replayed state below.</summary>
    private readonly Lock _gate = new();

    private readonly Dictionary<Guid, RegistrationRecordDto> _records = [];

    /// <summary>Bytes of the log replayed so far; always just after a newline (or 0).</summary>
    private long _replayed;

    /// <summary>The log's length when it was last read; more than <see cref="_replayed"/> while the last line is incomplete.</summary>
    private long _seen;

    /// <summary>Lines skipped so far: torn, unreadable, or not following on.</summary>
    private int _skipped;

    /// <summary>Opens (and if needed creates) the store in a folder.</summary>
    /// <param name="folder">The shared data folder.</param>
    /// <param name="logger">For Debug logging of reads, writes and conflicts.</param>
    public AppendLogRegistrationStore(string folder, ILogger logger)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(folder);
        ArgumentNullException.ThrowIfNull(logger);
        Directory.CreateDirectory(folder);
        _logPath = Path.Combine(folder, LogFileName);
        _lockPath = Path.Combine(folder, LockFileName);
        _logger = logger;
    }

    /// <inheritdoc />
    public Task<RegistrationRecordDto?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        RegistrationRecordDto? found;
        Replay replay;
        lock (_gate)
        {
            replay = CatchUp();
            found = _records.TryGetValue(id, out var record) ? RecordJson.Copy(record) : null;
        }

        StoreLog.LogRead(_logger, id, found is not null, Describe(replay));
        return Task.FromResult(found);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<RegistrationRecordDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<RegistrationRecordDto> all;
        Replay replay;
        lock (_gate)
        {
            replay = CatchUp();
            all = [.. _records.Values.Select(RecordJson.Copy)];
        }

        StoreLog.LogList(_logger, all.Count, Describe(replay));
        return Task.FromResult(all);
    }

    /// <inheritdoc />
    public Task<WriteResult> TryCreateAsync(RegistrationRecordDto record, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(record);
        return AppendAsync(new LogEntry(StoreOperation.Create, record.Id, null, record), cancellationToken);
    }

    /// <inheritdoc />
    public Task<WriteResult> TryReplaceAsync(RegistrationRecordDto updated, int expectedVersion, CancellationToken cancellationToken = default)
    {
        RecordJson.EnsureNextVersion(updated, expectedVersion);
        return AppendAsync(new LogEntry(StoreOperation.Replace, updated.Id, expectedVersion, updated), cancellationToken);
    }

    /// <inheritdoc />
    public Task<WriteResult> TryDeleteAsync(Guid id, int expectedVersion, CancellationToken cancellationToken = default) =>
        AppendAsync(new LogEntry(StoreOperation.Delete, id, expectedVersion, null), cancellationToken);

    /// <summary>Under the file lock: replays to the end, checks, and appends the entry if the check passes.</summary>
    private async Task<WriteResult> AppendAsync(LogEntry entry, CancellationToken cancellationToken)
    {
        var waiting = Stopwatch.StartNew();
        WriteResult result;
        string detail;
        await _writeGate.WaitAsync(cancellationToken);
        try
        {
            var (fileLock, tries) = await LockAsync(cancellationToken);
            using (fileLock)
            {
                var waited = waiting.ElapsedMilliseconds;
                lock (_gate)
                {
                    var replay = CatchUp(revalidate: true);
                    var sealedTornLine = false;
                    if (replay.Length > _replayed)
                    {
                        // An incomplete last line, and nobody else can be writing: a crashed append.
                        // Seal it so it can't swallow the next line, and replay it like any other.
                        Write([(byte)'\n'], replay.Length);
                        replay += CatchUp(revalidate: true);
                        sealedTornLine = true;
                    }

                    // Stamped now, under the lock, so the log's times are in append order.
                    var line = JsonSerializer.SerializeToUtf8Bytes(entry with { Pid = Environment.ProcessId, At = DateTimeOffset.UtcNow }, LineOptions);
                    result = Check(entry.Op, entry.Id, entry.Expected);
                    if (result == WriteResult.Saved)
                    {
                        Write([.. line, (byte)'\n'], replay.Length);
                        replay += CatchUp(revalidate: true);
                        if (!IsApplied(entry))
                        {
                            throw new InvalidOperationException(
                                $"The appended {entry.Op} of {entry.Id} did not replay; something wrote to {LogFileName} without holding {LockFileName}.");
                        }
                    }

                    detail = $"lock wait {waited} ms, {tries} tries; line {line.Length + 1} bytes; {Describe(replay)}"
                        + (sealedTornLine ? "; sealed a torn last line" : string.Empty);
                }
            }
        }
        finally
        {
            _writeGate.Release();
        }

        StoreLog.LogWrite(_logger, entry.Op, entry.Id, entry.Expected, result, detail);
        return result;
    }

    /// <summary>
    /// Whether a write may go ahead against the replayed state. Used by writers before appending,
    /// and by replay to decide whether a line applies, so both always agree.
    /// </summary>
    private WriteResult Check(StoreOperation op, Guid id, int? expectedVersion)
    {
        var exists = _records.TryGetValue(id, out var stored);
        return op == StoreOperation.Create ? (exists ? WriteResult.Conflict : WriteResult.Saved)
            : !exists ? WriteResult.NotFound
            : stored!.Version != expectedVersion ? WriteResult.Conflict
            : WriteResult.Saved;
    }

    /// <summary>Whether the replayed state now shows <paramref name="entry"/>.</summary>
    private bool IsApplied(LogEntry entry) =>
        entry.Op == StoreOperation.Delete
            ? !_records.ContainsKey(entry.Id)
            : _records.TryGetValue(entry.Id, out var stored) && stored.Version == entry.Record!.Version;

    /// <summary>
    /// Replays the complete lines added to the log since the last call; call while holding
    /// <see cref="_gate"/>. An incomplete last line is left for a later call.
    /// </summary>
    /// <param name="revalidate">Open the log even if a stat says nothing changed (writers, under the lock).</param>
    /// <returns>The log's length and what was replayed.</returns>
    private Replay CatchUp(bool revalidate = false)
    {
        if (!revalidate)
        {
            // The cheap check first: a stat, no open, when the log hasn't changed size.
            var info = new FileInfo(_logPath);
            var size = info.Exists ? info.Length : 0;
            if (size == _seen)
            {
                return new Replay(size, 0, 0);
            }
        }

        SafeFileHandle handle;
        try
        {
            handle = File.OpenHandle(_logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
        }
        catch (FileNotFoundException)
        {
            Reset(); // never written, or deleted
            return new Replay(0, 0, 0);
        }

        using (handle)
        {
            var length = RandomAccess.GetLength(handle);
            if (length < _replayed)
            {
                Reset(); // the log got shorter, so it was replaced: what was replayed no longer holds
            }

            // Read in chunks, so neither memory nor array size limits the log's size.
            var chunk = ArrayPool<byte>.Shared.Rent(ReadChunkSize);
            try
            {
                var partial = new ArrayBufferWriter<byte>(); // a line that runs on into the next chunk
                var position = _replayed;
                var lines = 0;
                var skipped = 0;
                while (position < length)
                {
                    var read = RandomAccess.Read(handle, chunk.AsSpan(0, (int)Math.Min(chunk.Length, length - position)), position);
                    if (read == 0)
                    {
                        break;
                    }

                    position += read;
                    var rest = chunk.AsSpan(0, read);
                    for (var end = rest.IndexOf((byte)'\n'); end >= 0; end = rest.IndexOf((byte)'\n'))
                    {
                        ReadOnlySpan<byte> line = rest[..end];
                        if (partial.WrittenCount > 0)
                        {
                            partial.Write(line);
                            line = partial.WrittenSpan;
                        }

                        lines++;
                        if (!TryApply(line))
                        {
                            skipped++;
                        }

                        partial.ResetWrittenCount();
                        rest = rest[(end + 1)..];
                    }

                    if (!rest.IsEmpty)
                    {
                        partial.Write(rest);
                    }
                }

                // An incomplete last line stays unread until it is finished (or sealed).
                _replayed = position - partial.WrittenCount;
                _seen = position;
                _skipped += skipped;
                return new Replay(position, lines, skipped);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(chunk);
            }
        }
    }

    /// <summary>Forgets everything replayed, so the log is replayed from the start.</summary>
    private void Reset()
    {
        _records.Clear();
        _replayed = 0;
        _seen = 0;
        _skipped = 0;
    }

    /// <summary>Applies one line if it parses and passes <see cref="Check"/>.</summary>
    private bool TryApply(ReadOnlySpan<byte> line)
    {
        LogEntry? entry;
        try
        {
            entry = JsonSerializer.Deserialize<LogEntry>(line, LineOptions);
        }
        catch (JsonException)
        {
            return false; // torn or otherwise unreadable
        }

        if (entry is null
            || Check(entry.Op, entry.Id, entry.Expected) != WriteResult.Saved
            || (entry.Op != StoreOperation.Delete
                && (entry.Record is null || entry.Record.Id != entry.Id || (entry.Expected is { } v && entry.Record.Version != v + 1))))
        {
            return false;
        }

        if (entry.Op == StoreOperation.Delete)
        {
            _records.Remove(entry.Id);
        }
        else
        {
            _records[entry.Id] = entry.Record!;
        }

        return true;
    }

    /// <summary>Writes bytes at an offset of the log, creating it if needed; call while holding the file lock.</summary>
    private void Write(ReadOnlySpan<byte> bytes, long offset)
    {
        using var handle = File.OpenHandle(_logPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete);
        RandomAccess.Write(handle, bytes, offset);
    }

    /// <summary>Waits for the exclusive lock on <see cref="LockFileName"/>.</summary>
    /// <returns>The open lock file (dispose to release) and how many attempts it took.</returns>
    private async Task<(FileStream Lock, int Tries)> LockAsync(CancellationToken cancellationToken)
    {
        for (var tries = 1; ; tries++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                return (new FileStream(_lockPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None), tries);
            }
            catch (IOException e) when (IsLockHeld(e))
            {
                await Task.Delay(1, cancellationToken); // held by another process or instance
            }
        }
    }

    /// <summary>
    /// Whether opening the lock file failed only because someone else holds it: EWOULDBLOCK on
    /// Linux (11) and macOS (35), a sharing or lock violation on Windows. Anything else (folder
    /// gone, disk full) is an error to report, not to wait out.
    /// </summary>
    private static bool IsLockHeld(IOException e) =>
        e.HResult is 11 or 35 or unchecked((int)0x80070020) or unchecked((int)0x80070021);

    private string Describe(Replay replay) =>
        $"{LogFileName} {replay.Length} bytes, {replay.Lines} new line(s) replayed, {replay.Skipped} skipped, {_skipped} skipped in total";

    /// <summary>One line of the log.</summary>
    /// <param name="Op">Which write.</param>
    /// <param name="Id">The record.</param>
    /// <param name="Expected">The version replaced or deleted; null for a create.</param>
    /// <param name="Record">The record written; null for a delete.</param>
    /// <param name="Pid">The writing process, for diagnosing several servers.</param>
    /// <param name="At">When it was written.</param>
    private sealed record LogEntry(StoreOperation Op, Guid Id, int? Expected, RegistrationRecordDto? Record, int Pid = 0, DateTimeOffset At = default);

    /// <summary>What a <see cref="CatchUp"/> found.</summary>
    /// <param name="Length">The log's length in bytes.</param>
    /// <param name="Lines">Complete lines replayed.</param>
    /// <param name="Skipped">Of those, lines not applied.</param>
    private readonly record struct Replay(long Length, int Lines, int Skipped)
    {
        public static Replay operator +(Replay a, Replay b) => new(b.Length, a.Lines + b.Lines, a.Skipped + b.Skipped);
    }
}
