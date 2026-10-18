using System.Collections.Concurrent;
using System.Diagnostics;
using BlazorApp.Shared.Dtos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace BlazorApp.Services.Storage.PerRecordFile;

/// <summary>
/// Strategy C: <see cref="IRegistrationStore"/> with one JSON file per record
/// (<c>records/{id}.json</c>) in a folder several servers share. Writes go to a temp file that is
/// renamed into place in one step, and are refused if the stored version has changed.
/// </summary>
/// <remarks>
/// <para>Folder layout:</para>
/// <list type="bullet">
/// <item><c>records/{id}.json</c>: the records; only ever complete files, because they appear by rename.</item>
/// <item><c>tmp/{id}.{random}.tmp</c>: a write in progress, renamed into <c>records/</c> when complete.</item>
/// <item><c>locks/{id}.lock</c>: an empty file per record, held with an exclusive OS lock while a
/// write checks the version and renames. Never deleted (see <see cref="LockAsync"/>).</item>
/// </list>
/// <para>The version check and the rename must happen as one step across processes, or two
/// writers could both pass the check before either renames. So every create, replace and
/// delete holds the record's lock file for the whole check-and-write. Writers to different
/// records never wait for each other. Reads take no lock: a rename replaces the file in one
/// step, so a reader sees either the old or the new file, never a mix.</para>
/// <para><see cref="GetAsync"/> and every write read the disk. <see cref="ListAsync"/> keeps each
/// record it has read with the file's creation time, last write time and size, and re-reads only
/// files whose values differ (phase 6b: listing ~6,000 records over SMB read every file and took
/// more than a minute). One directory listing gives all three without opening a file; changed
/// files are read 16 at a time, and lists come one at a time, so a list that waited finds the
/// records already read. A copy read within <see cref="RacyWindow"/> of its file's last write
/// is read again, since a change can leave all three values the same. Over SMB the client may
/// cache directory listings for a few seconds, so a list can be that far behind; a record's own
/// page and every write's version check read the file itself.</para>
/// </remarks>
public sealed class PerRecordFileStore : IRegistrationStore
{
    private const string RecordExtension = ".json";

    private readonly string _records;
    private readonly string _temp;
    private readonly string _locks;
    private readonly ILogger _logger;

    /// <summary>Records <see cref="ListAsync"/> has read, by file name, with the file's identity then and when it was read.</summary>
    private readonly ConcurrentDictionary<string, (FileStamp Stamp, RegistrationRecordDto Record, DateTime ReadAt)> _listed = new(StringComparer.Ordinal);

    /// <summary>
    /// How long after a file's last write a cached copy is still read again. Two saves in one
    /// clock tick with the same size, after Windows gave the new file the old one's creation time
    /// (file name tunneling), would look identical; a copy read this soon after the write is
    /// therefore never trusted. Like git's "racily clean" index entries. The servers' clocks must
    /// agree to well within this.
    /// </summary>
    private static readonly TimeSpan RacyWindow = TimeSpan.FromSeconds(3);

    /// <summary>Held by the one list that refreshes <see cref="_listed"/>.</summary>
    private readonly SemaphoreSlim _refresh = new(1, 1);

    /// <summary>Files a list reads at once.</summary>
    private const int ParallelReads = 16;

    /// <summary>Opens the store on a folder, creating its sub-folders if needed.</summary>
    /// <param name="folder">The shared data folder.</param>
    /// <param name="logger">For Debug logging of reads, writes and conflicts; none when null.</param>
    public PerRecordFileStore(string folder, ILogger<PerRecordFileStore>? logger = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(folder);
        _records = Directory.CreateDirectory(Path.Combine(folder, "records")).FullName;
        _temp = Directory.CreateDirectory(Path.Combine(folder, "tmp")).FullName;
        _locks = Directory.CreateDirectory(Path.Combine(folder, "locks")).FullName;
        _logger = logger ?? NullLogger<PerRecordFileStore>.Instance;
    }

    /// <inheritdoc />
    public Task<RegistrationRecordDto?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var record = Read(id);
        StoreLog.LogRead(_logger, id, record is not null, $"file={FileName(id)}");
        return Task.FromResult(record);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<RegistrationRecordDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        // One refresh at a time: lists that arrive together wait and then find the records
        // already read, rather than each reading every changed file.
        await _refresh.WaitAsync(cancellationToken);
        try
        {
            return Refresh(cancellationToken);
        }
        finally
        {
            _refresh.Release();
        }
    }

    /// <summary>The list's work, one at a time: list the folder, read new and changed files, drop deleted ones.</summary>
    private List<RegistrationRecordDto> Refresh(CancellationToken cancellationToken)
    {
        var current = new List<(string Name, string Path, FileStamp Stamp)>();
        int goneBeforeRead = 0, vanished = 0;
        foreach (var file in new DirectoryInfo(_records).EnumerateFiles())
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Only records/{guid}.json is a record. Temp files live in tmp/, but anything else
            // that turns up here (a stray file, an editor backup) is skipped rather than read.
            if (!IsRecordFile(file.FullName))
            {
                continue;
            }

            try
            {
                current.Add((file.Name, file.FullName, new FileStamp(file.CreationTimeUtc, file.LastWriteTimeUtc, file.Length)));
            }
            catch (FileNotFoundException)
            {
                // Deleted between the listing and reading its times (Linux reads them lazily).
                goneBeforeRead++;
            }
        }

        // New and changed files are read several at a time: over SMB each costs a network round
        // trip rather than work, so reading in parallel takes a cold list from minutes to seconds.
        var changed = current.Where(f => !(_listed.TryGetValue(f.Name, out var known) && known.Stamp == f.Stamp && f.Stamp.Written < known.ReadAt - RacyWindow)).ToList();
        try
        {
            Parallel.ForEach(changed, new ParallelOptions { MaxDegreeOfParallelism = ParallelReads, CancellationToken = cancellationToken }, file =>
            {
                // A file listed a moment ago may have been deleted since; that record is gone.
                var readAt = DateTime.UtcNow;
                if (TryReadAll(file.Path) is { } bytes)
                {
                    _listed[file.Name] = (file.Stamp, RecordJson.Deserialize(bytes), readAt);
                }
                else
                {
                    Interlocked.Increment(ref vanished);
                    _listed.TryRemove(file.Name, out _);
                }
            });
        }
        catch (AggregateException e) when (e.InnerExceptions.Count == 1)
        {
            // Report the read's own error (e.g. a damaged file), not the parallel loop's wrapper.
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw(e.InnerExceptions[0]);
        }

        var all = new List<RegistrationRecordDto>(current.Count);
        var names = new HashSet<string>(StringComparer.Ordinal);
        foreach (var file in current)
        {
            names.Add(file.Name);
            if (_listed.TryGetValue(file.Name, out var entry))
            {
                all.Add(entry.Record);
            }
        }

        // Records deleted since the last list.
        foreach (var name in _listed.Keys.Where(name => !names.Contains(name)))
        {
            _listed.TryRemove(name, out _);
        }

        var read = changed.Count - vanished;
        StoreLog.LogList(_logger, all.Count, $"folder=records, filesRead={read}, cached={all.Count - read}, deletedWhileListing={goneBeforeRead + vanished}");
        return all;
    }

    /// <inheritdoc />
    public async Task<WriteResult> TryCreateAsync(RegistrationRecordDto record, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(record);
        var bytes = RecordJson.Serialize(record);
        var (result, detail) = await UnderLockAsync(
            record.Id,
            () =>
            {
                if (File.Exists(RecordPath(record.Id)))
                {
                    return WriteResult.Conflict;
                }

                Write(record.Id, bytes);
                return WriteResult.Saved;
            },
            cancellationToken);
        StoreLog.LogWrite(_logger, StoreOperation.Create, record.Id, null, result, detail);
        return result;
    }

    /// <inheritdoc />
    public async Task<WriteResult> TryReplaceAsync(RegistrationRecordDto updated, int expectedVersion, CancellationToken cancellationToken = default)
    {
        RecordJson.EnsureNextVersion(updated, expectedVersion);
        var bytes = RecordJson.Serialize(updated);
        var (result, detail) = await UnderLockAsync(
            updated.Id,
            () =>
            {
                var check = CheckVersion(updated.Id, expectedVersion);
                if (check == WriteResult.Saved)
                {
                    Write(updated.Id, bytes);
                }

                return check;
            },
            cancellationToken);
        StoreLog.LogWrite(_logger, StoreOperation.Replace, updated.Id, expectedVersion, result, detail);
        return result;
    }

    /// <inheritdoc />
    public async Task<WriteResult> TryDeleteAsync(Guid id, int expectedVersion, CancellationToken cancellationToken = default)
    {
        var (result, detail) = await UnderLockAsync(
            id,
            () =>
            {
                var check = CheckVersion(id, expectedVersion);
                if (check == WriteResult.Saved)
                {
                    File.Delete(RecordPath(id));
                }

                return check;
            },
            cancellationToken);
        StoreLog.LogWrite(_logger, StoreOperation.Delete, id, expectedVersion, result, detail);
        return result;
    }

    private static string FileName(Guid id) => $"{id}{RecordExtension}";

    private static bool IsRecordFile(string file) =>
        Path.GetExtension(file) == RecordExtension && Guid.TryParse(Path.GetFileNameWithoutExtension(file), out _);

    /// <summary>
    /// Reads a whole file, or returns null if it doesn't exist. The file is opened with
    /// <see cref="FileShare.Delete"/> so an open reader never stops a writer renaming over it or
    /// deleting it (on Windows that would otherwise fail).
    /// </summary>
    private static byte[]? TryReadAll(string path)
    {
        try
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
            using var buffer = new MemoryStream();
            stream.CopyTo(buffer);
            return buffer.ToArray();
        }
        catch (FileNotFoundException)
        {
            return null;
        }
    }

    private string RecordPath(Guid id) => Path.Combine(_records, FileName(id));

    private RegistrationRecordDto? Read(Guid id) =>
        TryReadAll(RecordPath(id)) is { } bytes ? RecordJson.Deserialize(bytes) : null;

    /// <summary>Whether a conditional write may go ahead; call while holding the record's lock.</summary>
    private WriteResult CheckVersion(Guid id, int expectedVersion) =>
        Read(id) is not { } stored ? WriteResult.NotFound
        : stored.Version != expectedVersion ? WriteResult.Conflict
        : WriteResult.Saved;

    /// <summary>
    /// Writes a record's file in one step: the bytes go to a new temp file, which is flushed to
    /// disk (so a save reported as done survives a crash) and then renamed over
    /// <c>records/{id}.json</c>, tried again while Windows reports it in use (a reader holds it:
    /// <see cref="FileReplace"/>). Call while holding the record's lock.
    /// </summary>
    private void Write(Guid id, byte[] bytes)
    {
        var temp = Path.Combine(_temp, $"{id}.{Guid.NewGuid():N}.tmp");
        try
        {
            using (var stream = new FileStream(temp, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                stream.Write(bytes);
                stream.Flush(flushToDisk: true);
            }

            FileReplace.Move(temp, RecordPath(id), _logger);
        }
        catch
        {
            // Don't leave a half-written temp file behind; records/ was not touched. Best
            // effort, so a failing clean-up never hides the original error; an orphan left here
            // is removed by the next write to this record.
            try
            {
                File.Delete(temp);
            }
            catch (IOException)
            {
            }

            throw;
        }
    }

    /// <summary>
    /// Runs a check-and-write for one record while holding its lock, and describes the run for
    /// the log: the file, how long the lock took, and any temp files a crashed writer left.
    /// </summary>
    private async Task<(WriteResult Result, string Detail)> UnderLockAsync(
        Guid id, Func<WriteResult> body, CancellationToken cancellationToken)
    {
        var waited = Stopwatch.StartNew();
        var (held, attempts) = await LockAsync(id, cancellationToken);
        using (held)
        {
            var lockWait = waited.ElapsedMilliseconds;
            var orphans = RemoveOrphanedTempFiles(id);
            var result = body();
            return (result, $"file={FileName(id)}, lockWaitMs={lockWait}, lockAttempts={attempts}, orphanedTempFilesRemoved={orphans}");
        }
    }

    /// <summary>
    /// Takes the exclusive lock on <c>locks/{id}.lock</c>, retrying until it is free. Opening the
    /// file with <see cref="FileShare.None"/> makes .NET take an exclusive OS lock (<c>flock</c> on
    /// Linux) that other processes and other handles in this process respect. It is released
    /// when the stream is disposed, or by the OS if the process dies.
    /// </summary>
    /// <remarks>
    /// Lock files are never deleted. Deleting one is unsafe: a waiter that opened the old file
    /// would lock a file no longer in the folder, while the next writer creates and locks a new
    /// one, and both would think they hold the lock.
    /// </remarks>
    private async Task<(FileStream Held, int Attempts)> LockAsync(Guid id, CancellationToken cancellationToken)
    {
        var path = Path.Combine(_locks, $"{id}.lock");
        for (var attempts = 1; ; attempts++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                return (new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None), attempts);
            }
            catch (IOException e) when (IsLockHeld(e))
            {
                // Held by another writer to this record.
                await Task.Delay(1, cancellationToken);
            }
        }
    }

    /// <summary>
    /// Whether opening the lock file failed only because another handle holds it, which is worth
    /// waiting for. Anything else (folder missing, disk full, no permission) will not clear by
    /// retrying, so it is thrown. Contention is a plain <see cref="IOException"/> whose HResult
    /// is <c>EWOULDBLOCK</c> (11) from <c>flock</c> on Linux, or a sharing or lock violation on Windows.
    /// </summary>
    private static bool IsLockHeld(IOException e) =>
        e.GetType() == typeof(IOException)
        && e.HResult is 11 or unchecked((int)0x80070020) or unchecked((int)0x80070021);

    /// <summary>
    /// Deletes temp files for this record left by a writer that crashed between creating the
    /// temp file and renaming it. Only the holder of the record's lock writes its temp files, so
    /// while holding the lock any that exist are orphans. Call while holding the record's lock.
    /// </summary>
    /// <returns>How many were removed.</returns>
    private int RemoveOrphanedTempFiles(Guid id)
    {
        var removed = 0;
        foreach (var orphan in Directory.EnumerateFiles(_temp, $"{id}.*.tmp"))
        {
            File.Delete(orphan);
            removed++;
        }

        return removed;
    }

    /// <summary>What identifies one version of a record file without opening it.</summary>
    private readonly record struct FileStamp(DateTime Created, DateTime Written, long Length);
}
