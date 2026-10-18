using BlazorApp.Services.Storage;
using BlazorApp.Shared.Dtos;
using Microsoft.Extensions.Logging;

namespace BlazorApp.Services.Tests.Storage;

/// <summary>
/// Test-only positive control for the shared suite, not one of the strategies under study:
/// one file per record, every operation done while holding an exclusive lock on one lock
/// file, writes via temp file + rename. Simple and slow, but correct on local disk, so the
/// suite (and the multi-process helper) can be shown to pass when a store is right.
/// </summary>
/// <param name="folder">The data folder.</param>
/// <param name="logger">For the store's Debug logging.</param>
public class LockedFolderTestStore(string folder, ILogger logger) : IRegistrationStore
{
    private readonly string _records = Directory.CreateDirectory(Path.Combine(folder, "records")).FullName;
    private readonly string _lockFile = Path.Combine(folder, "store.lock");

    /// <summary>The store's logger.</summary>
    protected ILogger Logger { get; } = logger;

    /// <inheritdoc />
    public async Task<RegistrationRecordDto?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var record = await LockedAsync(() => Read(id), cancellationToken);
        StoreLog.LogRead(Logger, id, record is not null, FileFor(id));
        return record;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<RegistrationRecordDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        var all = await LockedAsync<IReadOnlyList<RegistrationRecordDto>>(
            () => [.. Directory.EnumerateFiles(_records, "*.json").Select(f => RecordJson.Deserialize(File.ReadAllBytes(f)))],
            cancellationToken);
        StoreLog.LogList(Logger, all.Count, _records);
        return all;
    }

    /// <inheritdoc />
    public async Task<WriteResult> TryCreateAsync(RegistrationRecordDto record, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(record);
        var result = await LockedAsync(
            () =>
            {
                if (File.Exists(FileFor(record.Id)))
                {
                    return WriteResult.Conflict;
                }

                Write(record);
                return WriteResult.Saved;
            },
            cancellationToken);
        StoreLog.LogWrite(Logger, StoreOperation.Create, record.Id, null, result, FileFor(record.Id));
        return result;
    }

    /// <inheritdoc />
    public virtual async Task<WriteResult> TryReplaceAsync(RegistrationRecordDto updated, int expectedVersion, CancellationToken cancellationToken = default)
    {
        RecordJson.EnsureNextVersion(updated, expectedVersion);
        var result = await LockedAsync(
            () =>
            {
                var stored = Read(updated.Id);
                if (stored is null)
                {
                    return WriteResult.NotFound;
                }

                if (stored.Version != expectedVersion)
                {
                    return WriteResult.Conflict;
                }

                Write(updated);
                return WriteResult.Saved;
            },
            cancellationToken);
        StoreLog.LogWrite(Logger, StoreOperation.Replace, updated.Id, expectedVersion, result, FileFor(updated.Id));
        return result;
    }

    /// <inheritdoc />
    public async Task<WriteResult> TryDeleteAsync(Guid id, int expectedVersion, CancellationToken cancellationToken = default)
    {
        var result = await LockedAsync(
            () =>
            {
                var stored = Read(id);
                if (stored is null)
                {
                    return WriteResult.NotFound;
                }

                if (stored.Version != expectedVersion)
                {
                    return WriteResult.Conflict;
                }

                File.Delete(FileFor(id));
                return WriteResult.Saved;
            },
            cancellationToken);
        StoreLog.LogWrite(Logger, StoreOperation.Delete, id, expectedVersion, result, FileFor(id));
        return result;
    }

    private string FileFor(Guid id) => Path.Combine(_records, $"{id}.json");

    private RegistrationRecordDto? Read(Guid id) =>
        File.Exists(FileFor(id)) ? RecordJson.Deserialize(File.ReadAllBytes(FileFor(id))) : null;

    private void Write(RegistrationRecordDto record)
    {
        var temp = Path.Combine(_records, $"{record.Id}.{Guid.NewGuid():N}.tmp");
        File.WriteAllBytes(temp, RecordJson.Serialize(record));
        File.Move(temp, FileFor(record.Id), overwrite: true);
    }

    /// <summary>
    /// Runs <paramref name="body"/> while holding the lock file open with FileShare.None, which
    /// .NET turns into an exclusive OS lock that other processes (and other handles in this
    /// process) respect.
    /// </summary>
    private async Task<T> LockedAsync<T>(Func<T> body, CancellationToken cancellationToken)
    {
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            FileStream? held = null;
            try
            {
                held = new FileStream(_lockFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                await Task.Delay(1, cancellationToken);
                continue;
            }

            using (held)
            {
                return body();
            }
        }
    }
}

/// <summary>
/// Test-only negative control: <see cref="LockedFolderTestStore"/>, except it reports every fifth
/// change of each writer as <see cref="WriteResult.Saved"/> without writing it. A suite or
/// helper that can't catch this can't catch a real lost update either.
/// </summary>
/// <remarks>
/// Which changes are faked comes from the workload's marker (<c>writer:i</c>, faked when
/// <c>i % 5 == 4</c>), and only once the version check would have passed. So the number of lost
/// updates is fixed at 4 per 20 changes, however many conflicts happen in between.
/// </remarks>
/// <param name="folder">The data folder.</param>
/// <param name="logger">For the store's Debug logging.</param>
public sealed class LossyTestStore(string folder, ILogger logger) : LockedFolderTestStore(folder, logger)
{
    /// <inheritdoc />
    public override async Task<WriteResult> TryReplaceAsync(RegistrationRecordDto updated, int expectedVersion, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(updated);
        if (!IsFifthChange(updated))
        {
            return await base.TryReplaceAsync(updated, expectedVersion, cancellationToken);
        }

        // Answer exactly as a real save would, but skip the write.
        var stored = await GetAsync(updated.Id, cancellationToken);
        return stored is null ? WriteResult.NotFound
            : stored.Version != expectedVersion ? WriteResult.Conflict
            : WriteResult.Saved;
    }

    private static bool IsFifthChange(RegistrationRecordDto updated) =>
        updated.History[^1].Comment is { } marker
        && int.TryParse(marker[(marker.LastIndexOf(':') + 1)..], out var i)
        && i % 5 == 4;
}

/// <summary>Registers <see cref="LockedFolderTestStore"/> as the test-only strategy <c>TestLockedFolder</c>.</summary>
public sealed class LockedFolderTestStoreFactory : IRegistrationStoreFactory
{
    /// <inheritdoc />
    public string Name => "TestLockedFolder";

    /// <inheritdoc />
    public bool UsesFolder => true;

    /// <inheritdoc />
    public IRegistrationStore Create(string? path, ILoggerFactory loggerFactory) =>
        new LockedFolderTestStore(path!, loggerFactory.CreateLogger<LockedFolderTestStore>());
}

/// <summary>Registers <see cref="LossyTestStore"/> as the test-only strategy <c>TestLossy</c>.</summary>
public sealed class LossyTestStoreFactory : IRegistrationStoreFactory
{
    /// <inheritdoc />
    public string Name => "TestLossy";

    /// <inheritdoc />
    public bool UsesFolder => true;

    /// <inheritdoc />
    public IRegistrationStore Create(string? path, ILoggerFactory loggerFactory) =>
        new LossyTestStore(path!, loggerFactory.CreateLogger<LossyTestStore>());
}

/// <summary>
/// Test-only control for broken data: wraps another store and makes every
/// <paramref name="everyNth"/>-th read throw (by default <see cref="System.Text.Json.JsonException"/>,
/// as a read of a half-written file would). Writes pass straight through. With
/// <paramref name="everyNth"/> 1 every read fails, like a file that stays broken.
/// </summary>
public sealed class BrokenReadsTestStore : IRegistrationStore
{
    private readonly IRegistrationStore _inner;
    private readonly int _everyNth;
    private readonly Func<Exception> _error;
    private int _reads;

    /// <summary>Wraps a store.</summary>
    /// <param name="inner">The store that does the real work.</param>
    /// <param name="everyNth">Which reads fail: the n-th, 2n-th, … of this instance.</param>
    /// <param name="error">What a failing read throws; a <see cref="System.Text.Json.JsonException"/> when null.</param>
    public BrokenReadsTestStore(IRegistrationStore inner, int everyNth, Func<Exception>? error = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(everyNth);
        _inner = inner;
        _everyNth = everyNth;
        _error = error ?? (() => new System.Text.Json.JsonException("Test store: this read sees a broken file."));
    }

    /// <inheritdoc />
    public Task<RegistrationRecordDto?> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
        Interlocked.Increment(ref _reads) % _everyNth == 0 ? throw _error() : _inner.GetAsync(id, cancellationToken);

    /// <inheritdoc />
    public Task<IReadOnlyList<RegistrationRecordDto>> ListAsync(CancellationToken cancellationToken = default) => _inner.ListAsync(cancellationToken);

    /// <inheritdoc />
    public Task<WriteResult> TryCreateAsync(RegistrationRecordDto record, CancellationToken cancellationToken = default) =>
        _inner.TryCreateAsync(record, cancellationToken);

    /// <inheritdoc />
    public Task<WriteResult> TryReplaceAsync(RegistrationRecordDto updated, int expectedVersion, CancellationToken cancellationToken = default) =>
        _inner.TryReplaceAsync(updated, expectedVersion, cancellationToken);

    /// <inheritdoc />
    public Task<WriteResult> TryDeleteAsync(Guid id, int expectedVersion, CancellationToken cancellationToken = default) =>
        _inner.TryDeleteAsync(id, expectedVersion, cancellationToken);
}

/// <summary>Registers <see cref="BrokenReadsTestStore"/> over <see cref="LockedFolderTestStore"/>, every third read broken, as <c>TestBrokenReads</c>.</summary>
public sealed class BrokenReadsTestStoreFactory : IRegistrationStoreFactory
{
    /// <inheritdoc />
    public string Name => "TestBrokenReads";

    /// <inheritdoc />
    public bool UsesFolder => true;

    /// <inheritdoc />
    public IRegistrationStore Create(string? path, ILoggerFactory loggerFactory) =>
        new BrokenReadsTestStore(new LockedFolderTestStore(path!, loggerFactory.CreateLogger<LockedFolderTestStore>()), everyNth: 3);
}
