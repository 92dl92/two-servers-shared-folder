using BlazorApp.Shared.Contracts;
using BlazorApp.Shared.Dtos;
using BlazorApp.Shared.Rules;

namespace BlazorApp.Client.Tests;

/// <summary>Test double for <see cref="IRegistrationService"/> with one record and scripted failures.</summary>
public sealed class FakeRegistrationService : IRegistrationService
{
    /// <summary>What <see cref="CreateAsync"/> returns.</summary>
    public SubmissionResultDto NextResult { get; set; } = SubmissionResultDto.Ok(Guid.NewGuid());

    /// <summary>Registrations passed to <see cref="CreateAsync"/>, in order.</summary>
    public List<RegistrationDto> Submitted { get; } = [];

    /// <summary>The record <see cref="GetAsync"/> returns; changes replace it.</summary>
    public RegistrationRecordDto? Record { get; set; }

    /// <summary>Thrown by the next change instead of applying it, then cleared.</summary>
    public Exception? NextError { get; set; }

    /// <summary>Each change attempted, as "Action@version".</summary>
    public List<string> Changes { get; } = [];

    /// <summary>When set, the correlation id current at each change is recorded in <see cref="CorrelationIds"/>.</summary>
    public IClientEventLog? Log { get; set; }

    /// <summary>The correlation id that was current when each change was made.</summary>
    public List<string?> CorrelationIds { get; } = [];

    /// <summary>Thrown by every <see cref="GetAsync"/> while set.</summary>
    public Exception? GetError { get; set; }

    /// <summary>How many times <see cref="GetAsync"/> was called.</summary>
    public int Loads { get; private set; }

    /// <summary>Builds a record in a given state.</summary>
    /// <param name="status">Its status.</param>
    /// <param name="owner">Who created it.</param>
    /// <param name="version">Its version.</param>
    /// <returns>A record with valid data.</returns>
    public static RegistrationRecordDto MakeRecord(RecordStatus status, string owner = "alice", int version = 3) => new(
        Guid.Parse("11111111-2222-3333-4444-555555555555"),
        version,
        status,
        owner,
        DateTimeOffset.UnixEpoch,
        owner,
        DateTimeOffset.UnixEpoch,
        new RegistrationDto
        {
            FirstName = "Ada",
            LastName = "Lovelace",
            Email = "ada@example.com",
            Street = "12 Analytical Row",
            City = "London",
            PostalCode = "N1 9GU",
            Country = "United Kingdom",
        },
        [new HistoryEntryDto(1, RecordAction.Create, owner, DateTimeOffset.UnixEpoch)]);

    /// <inheritdoc />
    public Task<SubmissionResultDto> CreateAsync(RegistrationDto registration, CancellationToken cancellationToken = default)
    {
        Submitted.Add(registration);
        CorrelationIds.Add(Log?.CurrentCorrelationId);
        return Task.FromResult(NextResult);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<RegistrationSummaryDto>> GetAllAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<RegistrationSummaryDto>>(Record is null ? [] : [RegistrationSummaryDto.From(Record)]);

    /// <inheritdoc />
    public Task<RegistrationRecordDto> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Loads++;
        if (GetError is { } error)
        {
            throw error;
        }

        return Record?.Id == id ? Task.FromResult(Record) : throw new RecordNotFoundException(id);
    }

    /// <inheritdoc />
    public Task<RegistrationRecordDto> UpdateAsync(Guid id, int expectedVersion, RegistrationDto data, CancellationToken cancellationToken = default) =>
        Apply(RecordAction.Update, expectedVersion, r => r with { Data = data });

    /// <inheritdoc />
    public Task<RegistrationRecordDto> SubmitAsync(Guid id, int expectedVersion, CancellationToken cancellationToken = default) =>
        Apply(RecordAction.Submit, expectedVersion);

    /// <inheritdoc />
    public Task<RegistrationRecordDto> ApproveAsync(Guid id, int expectedVersion, CancellationToken cancellationToken = default) =>
        Apply(RecordAction.Approve, expectedVersion);

    /// <inheritdoc />
    public Task<RegistrationRecordDto> RejectAsync(Guid id, int expectedVersion, CancellationToken cancellationToken = default) =>
        Apply(RecordAction.Reject, expectedVersion);

    /// <inheritdoc />
    public Task<RegistrationRecordDto> CommentAsync(Guid id, int expectedVersion, string text, CancellationToken cancellationToken = default) =>
        Apply(RecordAction.Comment, expectedVersion);

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id, int expectedVersion, CancellationToken cancellationToken = default)
    {
        await Apply(RecordAction.Delete, expectedVersion);
        Record = null;
    }

    private Task<RegistrationRecordDto> Apply(
        RecordAction action, int expectedVersion, Func<RegistrationRecordDto, RegistrationRecordDto>? change = null)
    {
        Changes.Add($"{action}@{expectedVersion}");
        CorrelationIds.Add(Log?.CurrentCorrelationId);
        if (NextError is { } error)
        {
            NextError = null;
            throw error;
        }

        var record = Record!;
        var updated = (change?.Invoke(record) ?? record) with
        {
            Version = record.Version + 1,
            Status = RecordRules.StatusAfter(action, record.Status),
        };
        Record = updated;
        return Task.FromResult(updated);
    }
}
