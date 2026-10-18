using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using BlazorApp.Shared.Dtos;
using Microsoft.Extensions.Logging;

namespace BlazorApp.Services.Storage.LockedFile;

/// <summary>
/// Strategy B (plan section 3): every record in one JSON file, <c>registrations.json</c>, read
/// whole, changed and written back, with every operation (reads too) done while holding an
/// exclusive OS lock on a separate lock file, <c>registrations.lock</c>. Several processes on
/// one folder therefore take turns: nothing is lost, but every operation waits for the others.
/// </summary>
/// <remarks>
/// <para><b>Why a separate lock file.</b> The data file is replaced by renaming a temp file over
/// it, so it is a new file after every write. A lock on the data file would be a lock on the old
/// file: a process that opened it while waiting would then hold a lock nobody else checks, and
/// read data that has already been replaced. The lock file is never replaced or deleted, so
/// every process locks the same file.</para>
/// <para><b>Why temp file + rename.</b> A crash in the middle of a write leaves the old data file
/// complete (and at most a stray temp file, overwritten by the next write), never a half-written
/// one. The temp file is flushed to disk before the rename, so a power cut can't leave the new
/// name pointing at data that never reached the disk. The folder itself is not flushed (.NET has
/// no call for it), so after a power cut the rename, and with it the latest save, may be undone;
/// a process crash loses nothing.</para>
/// <para>The lock file must never be deleted while servers run: a server that recreated it
/// would lock a different file from the others, and both would write at once.</para>
/// <para><b>The lock is advisory on Linux.</b> .NET turns <see cref="FileShare.None"/> into
/// <c>flock(LOCK_EX)</c>, which only stops programs that ask for the same lock. Every access
/// in this class goes through <see cref="LockedAsync{T}"/>; any other program touching the
/// data file must take the lock too. Setting <c>DOTNET_SYSTEM_IO_DISABLEFILELOCKING</c> turns
/// the lock off and this store then loses updates.</para>
/// <para>Nothing is cached: every call reads the file from disk.</para>
/// </remarks>
public sealed class LockedFileRegistrationStore : IRegistrationStore
{
    /// <summary>Name of the file holding every record.</summary>
    public const string DataFileName = "registrations.json";

    /// <summary>Name of the file every process locks.</summary>
    public const string LockFileName = "registrations.lock";

    /// <summary>How long to wait between attempts to take the lock.</summary>
    private static readonly TimeSpan RetryDelay = TimeSpan.FromMilliseconds(1);

    private readonly string _dataFile;
    private readonly string _tempFile;
    private readonly string _lockFile;
    private readonly ILogger _logger;

    /// <summary>Opens the store on a folder, creating the folder if needed.</summary>
    /// <param name="folder">The shared data folder.</param>
    /// <param name="logger">For Debug logging of reads, writes and conflicts.</param>
    public LockedFileRegistrationStore(string folder, ILogger<LockedFileRegistrationStore> logger)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(folder);
        ArgumentNullException.ThrowIfNull(logger);
        var full = Directory.CreateDirectory(folder).FullName;
        _dataFile = Path.Combine(full, DataFileName);
        _tempFile = _dataFile + ".tmp"; // one fixed name is enough: only the lock holder writes it
        _lockFile = Path.Combine(full, LockFileName);
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<RegistrationRecordDto?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var (record, wait) = await LockedAsync(() => ReadAll().Find(r => r.Id == id), cancellationToken);
        StoreLog.LogRead(_logger, id, record is not null, Detail(wait));
        return record;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<RegistrationRecordDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        var (all, wait) = await LockedAsync(ReadAll, cancellationToken);
        StoreLog.LogList(_logger, all.Count, Detail(wait));
        return all;
    }

    /// <inheritdoc />
    public async Task<WriteResult> TryCreateAsync(RegistrationRecordDto record, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(record);
        var copy = RecordJson.Copy(record);
        var (result, wait) = await LockedAsync(
            () =>
            {
                var all = ReadAll();
                if (all.Exists(r => r.Id == copy.Id))
                {
                    return WriteResult.Conflict;
                }

                all.Add(copy);
                WriteAll(all);
                return WriteResult.Saved;
            },
            cancellationToken);
        StoreLog.LogWrite(_logger, StoreOperation.Create, record.Id, null, result, Detail(wait));
        return result;
    }

    /// <inheritdoc />
    public async Task<WriteResult> TryReplaceAsync(RegistrationRecordDto updated, int expectedVersion, CancellationToken cancellationToken = default)
    {
        RecordJson.EnsureNextVersion(updated, expectedVersion);
        var copy = RecordJson.Copy(updated);
        var (result, wait) = await LockedAsync(
            () =>
            {
                var all = ReadAll();
                var index = all.FindIndex(r => r.Id == copy.Id);
                var check = CheckVersion(all, index, expectedVersion);
                if (check == WriteResult.Saved)
                {
                    all[index] = copy;
                    WriteAll(all);
                }

                return check;
            },
            cancellationToken);
        StoreLog.LogWrite(_logger, StoreOperation.Replace, updated.Id, expectedVersion, result, Detail(wait));
        return result;
    }

    /// <inheritdoc />
    public async Task<WriteResult> TryDeleteAsync(Guid id, int expectedVersion, CancellationToken cancellationToken = default)
    {
        var (result, wait) = await LockedAsync(
            () =>
            {
                var all = ReadAll();
                var index = all.FindIndex(r => r.Id == id);
                var check = CheckVersion(all, index, expectedVersion);
                if (check == WriteResult.Saved)
                {
                    all.RemoveAt(index);
                    WriteAll(all);
                }

                return check;
            },
            cancellationToken);
        StoreLog.LogWrite(_logger, StoreOperation.Delete, id, expectedVersion, result, Detail(wait));
        return result;
    }

    /// <summary>Whether a conditional write may go ahead on the record at <paramref name="index"/> (-1 when missing).</summary>
    private static WriteResult CheckVersion(List<RegistrationRecordDto> all, int index, int expectedVersion) =>
        index < 0 ? WriteResult.NotFound
        : all[index].Version != expectedVersion ? WriteResult.Conflict
        : WriteResult.Saved;

    /// <summary>Reads every record; call while holding the lock. A missing file means no records yet.</summary>
    /// <exception cref="JsonException">The file is not a complete list of records; it is left as it is.</exception>
    private List<RegistrationRecordDto> ReadAll()
    {
        byte[] json;
        try
        {
            json = File.ReadAllBytes(_dataFile);
        }
        catch (FileNotFoundException)
        {
            return [];
        }

        // Read as nullable items: "[null]" parses, and must be refused like any other broken file.
        var all = JsonSerializer.Deserialize<List<RegistrationRecordDto?>>(json, RecordJson.Options);
        return all is null || all.Contains(null)
            ? throw new JsonException($"{DataFileName} does not hold a list of records.")
            : all.ConvertAll(r => r!);
    }

    /// <summary>
    /// Writes every record to a temp file, flushes it to disk and renames it over the data file,
    /// tried again while Windows reports it in use (<see cref="FileReplace"/>); call while holding the lock.
    /// </summary>
    private void WriteAll(List<RegistrationRecordDto> all)
    {
        using (var temp = new FileStream(_tempFile, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            JsonSerializer.Serialize(temp, all, RecordJson.Options);
            temp.Flush(flushToDisk: true);
        }

        FileReplace.Move(_tempFile, _dataFile, _logger);
    }

    /// <summary>
    /// Runs <paramref name="body"/> while holding the lock file open with <see cref="FileShare.None"/>,
    /// which other processes, and other handles in this process, respect. The lock is polled rather
    /// than waited on, because .NET has no blocking file-lock call. It waits as long as it takes
    /// unless <paramref name="cancellationToken"/> is cancelled.
    /// </summary>
    /// <returns>The body's result, and how long it took to get the lock.</returns>
    private async Task<(T Result, LockWait Wait)> LockedAsync<T>(Func<T> body, CancellationToken cancellationToken)
    {
        var waited = Stopwatch.StartNew();
        var attempts = 0;
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            attempts++;
            FileStream held;
            try
            {
                held = new FileStream(_lockFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException e) when (IsLockHeld(e))
            {
                await Task.Delay(RetryDelay, cancellationToken);
                continue;
            }

            using (held)
            {
                var wait = new LockWait(waited.Elapsed, attempts);
                return (body(), wait);
            }
        }
    }

    /// <summary>
    /// Whether opening the lock file failed only because someone else holds it: EWOULDBLOCK from
    /// <c>flock</c> on Linux (11) and macOS (35), a sharing or lock violation on Windows. Any other
    /// error (missing folder, full disk) is thrown rather than retried for ever.
    /// </summary>
    private static bool IsLockHeld(IOException e) =>
        e.GetType() == typeof(IOException)
        && e.HResult is 11 or 35 or unchecked((int)0x80070020) or unchecked((int)0x80070021);

    /// <summary>Log detail: the file, and how long the lock took; not built when Debug is off.</summary>
    private string? Detail(LockWait wait) => !_logger.IsEnabled(LogLevel.Debug) ? null : string.Create(
        CultureInfo.InvariantCulture, $"{DataFileName}, lock wait {wait.Elapsed.TotalMilliseconds:0.0} ms, {wait.Attempts} attempts");

    /// <summary>How long getting the lock took.</summary>
    /// <param name="Elapsed">Time from the first attempt until the lock was held.</param>
    /// <param name="Attempts">Attempts made, including the one that succeeded.</param>
    private readonly record struct LockWait(TimeSpan Elapsed, int Attempts);
}
