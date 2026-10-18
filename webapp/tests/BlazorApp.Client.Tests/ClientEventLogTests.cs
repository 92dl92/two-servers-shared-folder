using System.Net;
using System.Net.Http.Json;
using BlazorApp.Client.Logging;
using BlazorApp.Shared.Contracts;
using BlazorApp.Shared.Dtos;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;

namespace BlazorApp.Client.Tests;

/// <summary>The browser's client event log: batches to the server, correlation ids on API calls.</summary>
public sealed class ClientEventLogTests : IAsyncDisposable
{
    private readonly RecordingHandler _server = new();
    private readonly FakeTimeProvider _clock = new();
    private readonly ClientEventLog _log;

    /// <summary>A log that sends to a recording handler.</summary>
    public ClientEventLogTests() =>
        _log = new ClientEventLog(new HttpClient(_server) { BaseAddress = new Uri("http://app/") }, NullLogger<ClientEventLog>.Instance, _clock);

    /// <inheritdoc />
    public async ValueTask DisposeAsync() => await _log.DisposeAsync();

    [Fact]
    public async Task A_button_press_and_its_result_are_sent_together_with_one_correlation_id()
    {
        var record = Guid.NewGuid();
        using (var action = _log.ButtonPressed("records/{id}", "Save", record))
        {
            action.Result("Conflict");
        }

        await _log.FlushAsync();

        var (path, batch) = Assert.Single(_server.Batches);
        Assert.Equal("/" + ApiRoutes.ClientLogs, path);
        Assert.Equal(["ButtonPressed", "ResultShown"], batch.Select(e => e.Event));
        Assert.All(batch, e => Assert.Equal(("records/{id}", "Save", record), (e.Page, e.Action, e.RecordId)));
        Assert.Equal("Conflict", batch[1].Result);
        Assert.Equal(batch[0].CorrelationId, batch[1].CorrelationId);
        Assert.True(CorrelationId.IsValid(batch[0].CorrelationId));
    }

    [Fact]
    public async Task Nothing_queued_sends_nothing()
    {
        await _log.FlushAsync();

        Assert.Empty(_server.Batches);
    }

    [Fact]
    public async Task Queued_events_are_sent_by_the_timer()
    {
        _log.PageLoaded("records");

        _clock.Advance(ClientEventLog.FlushInterval);
        await _server.WaitForBatchesAsync(1);

        Assert.Equal("PageLoaded", Assert.Single(Assert.Single(_server.Batches).Entries).Event);
    }

    [Fact]
    public async Task A_full_batch_is_sent_at_once()
    {
        for (var i = 0; i < ClientEventLog.BatchSize; i++)
        {
            _log.PageLoaded("records");
        }

        await _server.WaitForBatchesAsync(1);

        Assert.Equal(ClientEventLog.BatchSize, Assert.Single(_server.Batches).Entries.Count);
    }

    [Fact]
    public async Task A_batch_that_cannot_be_sent_is_dropped_without_breaking_the_page()
    {
        _server.Fail = true;
        _log.PageLoaded("records");

        await _log.FlushAsync();

        _server.Fail = false;
        await _log.FlushAsync();
        Assert.Empty(_server.Batches);
    }

    [Fact]
    public async Task A_timed_out_send_does_not_break_the_page()
    {
        _server.Throw = new TaskCanceledException("timed out");
        _log.PageLoaded("records");

        await _log.FlushAsync(); // must not throw
    }

    [Fact]
    public async Task Flush_waits_for_a_send_already_in_flight()
    {
        _server.Hold = new TaskCompletionSource();
        _log.PageLoaded("records");
        _clock.Advance(ClientEventLog.FlushInterval); // the timer takes the batch and starts sending

        await _server.WaitForArrivalAsync();
        var flush = _log.FlushAsync();
        await Task.Delay(100);
        Assert.False(flush.IsCompleted, "FlushAsync returned while the timer's batch was still being sent.");

        _server.Hold.SetResult();
        await flush;
        Assert.Single(_server.Batches);
    }

    [Fact]
    public async Task Api_calls_carry_the_action_id_only_while_the_action_runs()
    {
        var api = new RecordingHandler { Status = HttpStatusCode.Conflict };
        using var http = new HttpClient(new CorrelationHandler(_log) { InnerHandler = api }) { BaseAddress = new Uri("http://app/") };

        string pressed;
        using (var action = _log.ButtonPressed("records/{id}", "Approve"))
        {
            pressed = action.CorrelationId;
            Assert.Equal(pressed, _log.CurrentCorrelationId);
            (await http.PostAsync("api/registrations/x/approve", null)).Dispose();
        }

        (await http.GetAsync("api/registrations")).Dispose();

        Assert.Null(_log.CurrentCorrelationId);
        Assert.Equal([pressed, null], api.CorrelationIds);

        // Both calls answered 409: each is logged as a failed API call, the first with the action's id.
        await _log.FlushAsync();
        var failed = Assert.Single(_server.Batches).Entries.Where(e => e.Event == "ApiFailed").ToList();
        Assert.Equal([("/api/registrations/x/approve", "POST", 409, pressed), ("/api/registrations", "GET", 409, (string?)null)],
            failed.Select(e => (e.Page, e.Action, e.StatusCode!.Value, e.CorrelationId)));
    }

    /// <summary>Stands in for the server: records requests and answers with a fixed status.</summary>
    private sealed class RecordingHandler : HttpMessageHandler
    {
        private readonly SemaphoreSlim _arrived = new(0);

        public List<(string Path, List<ClientLogEntryDto> Entries)> Batches { get; } = [];

        public List<string?> CorrelationIds { get; } = [];

        public HttpStatusCode Status { get; set; } = HttpStatusCode.NoContent;

        public bool Fail { get; set; }

        public Exception? Throw { get; set; }

        /// <summary>When set, client-log requests wait for it before answering.</summary>
        public TaskCompletionSource? Hold { get; set; }

        private readonly SemaphoreSlim _started = new(0);

        public async Task WaitForArrivalAsync() =>
            Assert.True(await _started.WaitAsync(TimeSpan.FromSeconds(10)), "No request arrived.");

        public async Task WaitForBatchesAsync(int count)
        {
            for (var i = 0; i < count; i++)
            {
                Assert.True(await _arrived.WaitAsync(TimeSpan.FromSeconds(10)), "No batch arrived.");
            }
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (Fail)
            {
                throw new HttpRequestException("offline");
            }

            if (Throw is { } error)
            {
                throw error;
            }

            _started.Release();
            if (Hold is { } hold)
            {
                await hold.Task;
            }

            CorrelationIds.Add(request.Headers.TryGetValues(CorrelationId.HeaderName, out var ids) ? ids.Single() : null);
            if (request.RequestUri!.AbsolutePath.EndsWith(ApiRoutes.ClientLogs, StringComparison.Ordinal))
            {
                var entries = await request.Content!.ReadFromJsonAsync<List<ClientLogEntryDto>>(cancellationToken);
                lock (Batches)
                {
                    Batches.Add((request.RequestUri.AbsolutePath, entries!));
                }

                _arrived.Release();
            }

            return new HttpResponseMessage(Status);
        }
    }
}
