using BlazorApp.Shared.Dtos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace BlazorApp.Services.Storage.InMemory;

/// <summary>
/// <see cref="IRegistrationStore"/> that keeps records in this process's memory. Data is lost
/// when the process ends and is not shared with other servers, so it is only suitable for a
/// single server (and as the reference for the in-process parts of the shared test suite).
/// </summary>
/// <remarks>
/// Writes are conditional on the version the caller read, checked and applied under one lock,
/// so two writers that start from the same version cannot both succeed. That lock only
/// coordinates threads in this process; it means nothing to other servers.
/// </remarks>
/// <param name="logger">For Debug logging of reads, writes and conflicts; none when null.</param>
public sealed class InMemoryRegistrationStore(ILogger<InMemoryRegistrationStore>? logger = null) : IRegistrationStore
{
    private readonly Dictionary<Guid, RegistrationRecordDto> _items = [];
    private readonly Lock _gate = new();
    private readonly ILogger _logger = logger ?? NullLogger<InMemoryRegistrationStore>.Instance;

    /// <inheritdoc />
    public Task<RegistrationRecordDto?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        RegistrationRecordDto? found;
        lock (_gate)
        {
            found = _items.TryGetValue(id, out var record) ? RecordJson.Copy(record) : null;
        }

        StoreLog.LogRead(_logger, id, found is not null);
        return Task.FromResult(found);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<RegistrationRecordDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<RegistrationRecordDto> all;
        lock (_gate)
        {
            all = [.. _items.Values.Select(RecordJson.Copy)];
        }

        StoreLog.LogList(_logger, all.Count);
        return Task.FromResult(all);
    }

    /// <inheritdoc />
    public Task<WriteResult> TryCreateAsync(RegistrationRecordDto record, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(record);
        var copy = RecordJson.Copy(record);
        WriteResult result;
        lock (_gate)
        {
            result = _items.TryAdd(record.Id, copy) ? WriteResult.Saved : WriteResult.Conflict;
        }

        StoreLog.LogWrite(_logger, StoreOperation.Create, record.Id, null, result);
        return Task.FromResult(result);
    }

    /// <inheritdoc />
    public Task<WriteResult> TryReplaceAsync(RegistrationRecordDto updated, int expectedVersion, CancellationToken cancellationToken = default)
    {
        RecordJson.EnsureNextVersion(updated, expectedVersion);
        var copy = RecordJson.Copy(updated);
        WriteResult result;
        lock (_gate)
        {
            result = CheckVersion(updated.Id, expectedVersion);
            if (result == WriteResult.Saved)
            {
                _items[updated.Id] = copy;
            }
        }

        StoreLog.LogWrite(_logger, StoreOperation.Replace, updated.Id, expectedVersion, result);
        return Task.FromResult(result);
    }

    /// <inheritdoc />
    public Task<WriteResult> TryDeleteAsync(Guid id, int expectedVersion, CancellationToken cancellationToken = default)
    {
        WriteResult result;
        lock (_gate)
        {
            result = CheckVersion(id, expectedVersion);
            if (result == WriteResult.Saved)
            {
                _items.Remove(id);
            }
        }

        StoreLog.LogWrite(_logger, StoreOperation.Delete, id, expectedVersion, result);
        return Task.FromResult(result);
    }

    /// <summary>Whether a conditional write may go ahead; call while holding the lock.</summary>
    private WriteResult CheckVersion(Guid id, int expectedVersion) =>
        !_items.TryGetValue(id, out var stored) ? WriteResult.NotFound
        : stored.Version != expectedVersion ? WriteResult.Conflict
        : WriteResult.Saved;
}
