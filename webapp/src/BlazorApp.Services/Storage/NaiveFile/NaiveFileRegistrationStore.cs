using System.Text.Json;
using BlazorApp.Shared.Dtos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace BlazorApp.Services.Storage.NaiveFile;

/// <summary>
/// Strategy A, the control: every record in one JSON file (<c>records.json</c>) in the data
/// folder. Each operation reads the whole file, changes it in memory and writes the whole file
/// back, with no lock and no temp file. Expected to lose updates when several writers save at
/// once (plan section 3, row A).
/// </summary>
/// <remarks>
/// <para>Deliberately naive. Nothing is cached: every call reads the file, so a single writer
/// working from a stale copy still gets <see cref="WriteResult.Conflict"/>. But the version
/// check and the write are separate steps that nothing holds together, so two writers that read
/// the same version can both pass the check and both be told <see cref="WriteResult.Saved"/>;
/// the later write replaces the whole file and the earlier change is gone.</para>
/// <para>The file is written in place (truncated, then written), so a reader that comes in
/// between can see an empty or half-written file. That is part of what this control shows,
/// and it is not hidden: a read of such a file throws <see cref="JsonException"/>.</para>
/// </remarks>
/// <param name="folder">The data folder; created if missing.</param>
/// <param name="logger">For Debug logging of reads, writes and conflicts; none when null.</param>
public sealed class NaiveFileRegistrationStore(string folder, ILogger<NaiveFileRegistrationStore>? logger = null) : IRegistrationStore
{
    /// <summary>The file name inside the data folder.</summary>
    public const string FileName = "records.json";

    private readonly string _file = Path.Combine(Directory.CreateDirectory(folder).FullName, FileName);
    private readonly ILogger _logger = logger ?? NullLogger<NaiveFileRegistrationStore>.Instance;

    /// <inheritdoc />
    public Task<RegistrationRecordDto?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var found = ReadAll().GetValueOrDefault(id);
        StoreLog.LogRead(_logger, id, found is not null, _file);
        return Task.FromResult(found);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<RegistrationRecordDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<RegistrationRecordDto> all = [.. ReadAll().Values];
        StoreLog.LogList(_logger, all.Count, _file);
        return Task.FromResult(all);
    }

    /// <inheritdoc />
    public Task<WriteResult> TryCreateAsync(RegistrationRecordDto record, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(record);
        var records = ReadAll();
        var result = records.TryAdd(record.Id, RecordJson.Copy(record)) ? WriteResult.Saved : WriteResult.Conflict;
        if (result == WriteResult.Saved)
        {
            WriteAll(records);
        }

        StoreLog.LogWrite(_logger, StoreOperation.Create, record.Id, null, result, _file);
        return Task.FromResult(result);
    }

    /// <inheritdoc />
    public Task<WriteResult> TryReplaceAsync(RegistrationRecordDto updated, int expectedVersion, CancellationToken cancellationToken = default)
    {
        RecordJson.EnsureNextVersion(updated, expectedVersion);
        var records = ReadAll();
        var result = CheckVersion(records, updated.Id, expectedVersion);
        if (result == WriteResult.Saved)
        {
            records[updated.Id] = RecordJson.Copy(updated);
            WriteAll(records);
        }

        StoreLog.LogWrite(_logger, StoreOperation.Replace, updated.Id, expectedVersion, result, _file);
        return Task.FromResult(result);
    }

    /// <inheritdoc />
    public Task<WriteResult> TryDeleteAsync(Guid id, int expectedVersion, CancellationToken cancellationToken = default)
    {
        var records = ReadAll();
        var result = CheckVersion(records, id, expectedVersion);
        if (result == WriteResult.Saved)
        {
            records.Remove(id);
            WriteAll(records);
        }

        StoreLog.LogWrite(_logger, StoreOperation.Delete, id, expectedVersion, result, _file);
        return Task.FromResult(result);
    }

    /// <summary>Whether a conditional write may go ahead, judged from the copy just read (nothing stops it changing before the write).</summary>
    private static WriteResult CheckVersion(Dictionary<Guid, RegistrationRecordDto> records, Guid id, int expectedVersion) =>
        !records.TryGetValue(id, out var stored) ? WriteResult.NotFound
        : stored.Version != expectedVersion ? WriteResult.Conflict
        : WriteResult.Saved;

    /// <summary>Reads every record from the file; no file means no records.</summary>
    /// <exception cref="JsonException">The file is not complete JSON, e.g. another writer is half-way through.</exception>
    private Dictionary<Guid, RegistrationRecordDto> ReadAll()
    {
        if (!File.Exists(_file))
        {
            return [];
        }

        var list = JsonSerializer.Deserialize<List<RegistrationRecordDto>>(File.ReadAllBytes(_file), RecordJson.Options)
            ?? throw new JsonException($"{_file} does not hold a list of records.");
        return list.ToDictionary(r => r.Id);
    }

    /// <summary>Writes every record back over the file, in place: truncate, then write.</summary>
    private void WriteAll(Dictionary<Guid, RegistrationRecordDto> records) =>
        File.WriteAllBytes(_file, JsonSerializer.SerializeToUtf8Bytes(records.Values, RecordJson.Options));
}
