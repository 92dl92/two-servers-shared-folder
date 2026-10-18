using BlazorApp.Shared.Contracts;
using BlazorApp.Shared.Dtos;

namespace BlazorApp.Client.Tests;

/// <summary>Test double for <see cref="IClientEventLog"/> that keeps every event in memory.</summary>
public sealed class FakeClientEventLog : IClientEventLog
{
    /// <summary>Everything logged, in order.</summary>
    public List<ClientLogEntryDto> Entries { get; } = [];

    /// <summary>How many times <see cref="FlushAsync"/> was called.</summary>
    public int Flushes { get; private set; }

    /// <inheritdoc />
    public string? CurrentCorrelationId { get; private set; }

    /// <summary>Button presses only.</summary>
    public IEnumerable<ClientLogEntryDto> Presses => Entries.Where(e => e.Event == "ButtonPressed");

    /// <summary>Results shown only.</summary>
    public IEnumerable<ClientLogEntryDto> Results => Entries.Where(e => e.Event == "ResultShown");

    /// <inheritdoc />
    public void PageLoaded(string page, Guid? recordId = null, string? result = null) =>
        Entries.Add(new ClientLogEntryDto(DateTimeOffset.UnixEpoch, "PageLoaded", page, Result: result, RecordId: recordId));

    /// <inheritdoc />
    public IClientAction ButtonPressed(string page, string action, Guid? recordId = null)
    {
        var id = $"c{Entries.Count}";
        var previous = CurrentCorrelationId;
        CurrentCorrelationId = id;
        Entries.Add(new ClientLogEntryDto(DateTimeOffset.UnixEpoch, "ButtonPressed", page, action, RecordId: recordId, CorrelationId: id));
        return new Action(this, page, action, recordId, id, previous);
    }

    /// <inheritdoc />
    public void ApiFailed(string method, string path, int statusCode) =>
        Entries.Add(new ClientLogEntryDto(DateTimeOffset.UnixEpoch, "ApiFailed", path, method, StatusCode: statusCode));

    /// <inheritdoc />
    public Task FlushAsync()
    {
        Flushes++;
        return Task.CompletedTask;
    }

    private sealed class Action(FakeClientEventLog log, string page, string action, Guid? recordId, string id, string? previous) : IClientAction
    {
        public string CorrelationId => id;

        public void Result(string result) =>
            log.Entries.Add(new ClientLogEntryDto(DateTimeOffset.UnixEpoch, "ResultShown", page, action, result, recordId, id));

        public void Dispose() => log.CurrentCorrelationId = previous;
    }
}
