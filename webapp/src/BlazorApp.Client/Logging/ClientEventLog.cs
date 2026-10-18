using System.Net.Http.Json;
using BlazorApp.Shared.Contracts;
using BlazorApp.Shared.Dtos;
using Microsoft.Extensions.Logging;

namespace BlazorApp.Client.Logging;

/// <summary>
/// Browser-side <see cref="IClientEventLog"/>: writes each event to the browser console and
/// sends them to the server in batches (decision L2), so the server's log file also has the
/// user's side of every action.
/// </summary>
/// <remarks>
/// Events are sent when <see cref="BatchSize"/> are queued, every <see cref="FlushInterval"/>,
/// and on <see cref="FlushAsync"/>. A batch that can't be sent is dropped with a console
/// warning: logs are diagnostics only (decision L4) and must never break the page.
/// </remarks>
public sealed class ClientEventLog : IClientEventLog, IAsyncDisposable
{
    /// <summary>Send once this many events are queued.</summary>
    public const int BatchSize = 20;

    /// <summary>Send queued events at least this often.</summary>
    public static readonly TimeSpan FlushInterval = TimeSpan.FromSeconds(1);

    private static readonly AsyncLocal<string?> Current = new();

    private readonly HttpClient _http;
    private readonly ILogger<ClientEventLog> _logger;
    private readonly TimeProvider _clock;
    private readonly List<ClientLogEntryDto> _queue = [];
    private readonly Lock _gate = new();
    private readonly ITimer _timer;

    // One send at a time, so FlushAsync also waits for a batch the timer is already sending.
    private readonly SemaphoreSlim _sending = new(1, 1);

    /// <summary>Creates the log.</summary>
    /// <param name="http">Sends batches; must not add correlation headers or log failures itself.</param>
    /// <param name="logger">The browser console.</param>
    /// <param name="clock">Timestamps and the flush timer.</param>
    public ClientEventLog(HttpClient http, ILogger<ClientEventLog> logger, TimeProvider clock)
    {
        _http = http;
        _logger = logger;
        _clock = clock;
        _timer = clock.CreateTimer(_ => _ = FlushAsync(), null, FlushInterval, FlushInterval);
    }

    /// <inheritdoc />
    public string? CurrentCorrelationId => Current.Value;

    /// <inheritdoc />
    public void PageLoaded(string page, Guid? recordId = null, string? result = null) =>
        Add(new ClientLogEntryDto(_clock.GetUtcNow(), "PageLoaded", page, Result: result, RecordId: recordId, CorrelationId: CorrelationId.New()));

    /// <inheritdoc />
    public IClientAction ButtonPressed(string page, string action, Guid? recordId = null)
    {
        var id = CorrelationId.New();
        var previous = Current.Value;

        // Set from a synchronous method, so the value stays in the caller's async flow and the
        // API calls it makes until the action is disposed carry this id.
        Current.Value = id;
        Add(new ClientLogEntryDto(_clock.GetUtcNow(), "ButtonPressed", page, action, RecordId: recordId, CorrelationId: id));
        return new ClientAction(this, page, action, recordId, id, previous);
    }

    /// <inheritdoc />
    public void ApiFailed(string method, string path, int statusCode) =>
        Add(new ClientLogEntryDto(_clock.GetUtcNow(), "ApiFailed", path, method, CorrelationId: Current.Value, StatusCode: statusCode));

    /// <inheritdoc />
    public async Task FlushAsync()
    {
        await _sending.WaitAsync();
        try
        {
            List<ClientLogEntryDto> batch;
            lock (_gate)
            {
                if (_queue.Count == 0)
                {
                    return;
                }

                batch = [.. _queue];
                _queue.Clear();
            }

            try
            {
                using var response = await _http.PostAsJsonAsync(ApiRoutes.ClientLogs, batch);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Client log batch of {Count} refused: {StatusCode}", batch.Count, (int)response.StatusCode);
                }
            }
#pragma warning disable CA1031 // Sending logs must never break the page, whatever goes wrong (decision L4).
            catch (Exception e)
#pragma warning restore CA1031
            {
                _logger.LogWarning(e, "Client log batch of {Count} could not be sent", batch.Count);
            }
        }
        finally
        {
            _sending.Release();
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await _timer.DisposeAsync();
        await FlushAsync();
        _sending.Dispose();
    }

    private void Add(ClientLogEntryDto entry)
    {
        _logger.LogInformation(
            "{Event} {Page} {Action} {Result} {RecordId} {StatusCode} ({CorrelationId})",
            entry.Event, entry.Page, entry.Action, entry.Result, entry.RecordId, entry.StatusCode, entry.CorrelationId);

        bool full;
        lock (_gate)
        {
            _queue.Add(entry);
            full = _queue.Count >= BatchSize;
        }

        if (full)
        {
            _ = FlushAsync();
        }
    }

    /// <summary>A user action in progress.</summary>
    private sealed class ClientAction(
        ClientEventLog log, string page, string action, Guid? recordId, string correlationId, string? previous) : IClientAction
    {
        public string CorrelationId => correlationId;

        public void Result(string result) =>
            log.Add(new ClientLogEntryDto(log._clock.GetUtcNow(), "ResultShown", page, action, result, recordId, correlationId));

        public void Dispose()
        {
            if (Current.Value == correlationId)
            {
                Current.Value = previous;
            }
        }
    }
}
