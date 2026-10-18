using System.Net;
using System.Net.Http.Json;
using BlazorApp.Shared.Contracts;
using BlazorApp.Shared.Dtos;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;

namespace BlazorApp.Server.Tests;

/// <summary>Correlation ids, HTTP lines, client events, user and startup logging (plan section 8).</summary>
public sealed class LoggingApiTests : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory = new WebApplicationFactory<Program>().WithFakeLogging();

    /// <inheritdoc />
    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task Sent_correlation_id_is_on_the_http_line_and_the_record_event()
    {
        var client = _factory.ClientAs("alice");
        using var request = new HttpRequestMessage(HttpMethod.Post, ApiRoutes.Registrations)
        {
            Content = JsonContent.Create(RecordApi.Valid()),
        };
        request.Headers.Add(CorrelationId.HeaderName, "click-42");

        using var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("click-42", Assert.Single(response.Headers.GetValues(CorrelationId.HeaderName)));
        var logs = _factory.Logs();
        var http = Assert.Single(logs, r => r.Id.Id == 3001);
        Assert.Equal("click-42", http.GetStructuredStateValue("CorrelationId"));
        Assert.Equal("POST", http.GetStructuredStateValue("Method"));
        Assert.Equal("api/registrations/", http.GetStructuredStateValue("Route"));
        Assert.Equal("201", http.GetStructuredStateValue("StatusCode"));
        Assert.Equal(LogLevel.Information, http.Level);
        var recordEvent = Assert.Single(logs, r => r.Id.Id == 1100);
        Assert.Equal("click-42", recordEvent.ScopeValue("CorrelationId"));
    }

    [Fact]
    public async Task Without_a_header_the_server_makes_an_id_and_both_lines_share_it()
    {
        using var response = await _factory.ClientAs("alice").PostAsJsonAsync(ApiRoutes.Registrations, RecordApi.Valid());

        var id = Assert.Single(response.Headers.GetValues(CorrelationId.HeaderName));
        Assert.True(CorrelationId.IsValid(id));
        var logs = _factory.Logs();
        Assert.Equal(id, Assert.Single(logs, r => r.Id.Id == 3001).GetStructuredStateValue("CorrelationId"));
        Assert.Equal(id, Assert.Single(logs, r => r.Id.Id == 1100).ScopeValue("CorrelationId"));
    }

    [Theory]
    [InlineData("has spaces")]
    [InlineData("line\nbreak")]
    [InlineData("0123456789012345678901234567890123456789012345678901234567890123456789")]
    public async Task An_unsafe_correlation_id_is_replaced(string sent)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, ApiRoutes.Users);
        request.Headers.TryAddWithoutValidation(CorrelationId.HeaderName, sent);

        using var response = await _factory.CreateClient().SendAsync(request);

        var id = Assert.Single(response.Headers.GetValues(CorrelationId.HeaderName));
        Assert.NotEqual(sent, id);
        Assert.True(CorrelationId.IsValid(id));
    }

    [Fact]
    public async Task Each_api_request_gets_one_line_with_its_route_template()
    {
        var api = new RecordApi(_factory);
        var record = await api.CreateInStatusAsync("alice", RecordStatus.Draft);
        var before = _factory.Logs().Count(r => r.Id.Id == 3001);

        using var response = await api.SendAsync("rita", RecordAction.Approve, record.Id, record.Version);

        var lines = _factory.Logs().Where(r => r.Id.Id == 3001).Skip(before).ToList();
        var line = Assert.Single(lines);
        Assert.Equal("api/registrations/{id:guid}/approve", line.GetStructuredStateValue("Route"));
        Assert.Equal("422", line.GetStructuredStateValue("StatusCode"));
    }

    [Fact]
    public async Task Client_events_are_logged_as_client_with_their_own_correlation_ids()
    {
        var record = Guid.NewGuid();
        ClientLogEntryDto[] batch =
        [
            new(DateTimeOffset.UtcNow, "ButtonPressed", "records/{id}", "Save", null, record, "action-1"),
            new(DateTimeOffset.UtcNow, "ResultShown", "records/{id}", "Save", "Conflict", record, "action-1"),
            new(DateTimeOffset.UtcNow, "PageLoaded", "records", CorrelationId: "not valid!"),
        ];

        using var response = await _factory.CreateClient().PostAsJsonAsync(ApiRoutes.ClientLogs, batch);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var events = _factory.Logs().Where(r => r.Id.Id == 4001).ToList();
        Assert.Equal(3, events.Count);
        Assert.All(events, e => Assert.Equal("client", e.ScopeValue("Source")));
        Assert.Equal(["action-1", "action-1", null], events.Select(e => e.ScopeValue("CorrelationId")));
        Assert.Equal("Conflict", events[1].GetStructuredStateValue("Result"));
        Assert.Equal(record.ToString(), events[1].GetStructuredStateValue("RecordId"));
        Assert.DoesNotContain(_factory.Logs(), r => r.Id.Id == 3001 && r.GetStructuredStateValue("Route")!.Contains("client-logs", StringComparison.Ordinal));
    }

    [Fact]
    public async Task Too_big_a_client_batch_is_refused()
    {
        var batch = Enumerable.Range(0, 101).Select(_ => new ClientLogEntryDto(DateTimeOffset.UtcNow, "PageLoaded")).ToList();

        using var response = await _factory.CreateClient().PostAsJsonAsync(ApiRoutes.ClientLogs, batch);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.DoesNotContain(_factory.Logs(), r => r.Id.Id == 4001);
    }

    [Fact]
    public async Task Acting_as_changes_and_unknown_user_ids_are_logged()
    {
        var client = _factory.CreateClient();

        (await client.PutAsJsonAsync(ApiRoutes.CurrentUser, new SetCurrentUserDto("rita"))).Dispose();
        (await client.PutAsJsonAsync(ApiRoutes.CurrentUser, new SetCurrentUserDto("mallory"))).Dispose();
        (await _factory.ClientAs("nobody-here").GetAsync(ApiRoutes.Registrations)).Dispose();

        var logs = _factory.Logs();
        var changed = Assert.Single(logs, r => r.Id.Id == 3101);
        Assert.Equal("rita", changed.GetStructuredStateValue("UserId"));
        Assert.Equal(LogLevel.Information, changed.Level);
        var unknown = logs.Where(r => r.Id.Id == 3102).ToList();
        Assert.Equal(["mallory", "nobody-here"], unknown.Select(r => r.GetStructuredStateValue("RequestedId")));
        Assert.All(unknown, r => Assert.Equal(LogLevel.Warning, r.Level));
    }

    [Fact]
    public void Startup_is_logged_with_the_storage_settings()
    {
        _ = _factory.Services; // starts the server

        var started = Assert.Single(_factory.Logs(), r => r.Id.Id == 3201);
        Assert.Equal("InMemory", started.GetStructuredStateValue("Strategy"));
        Assert.EndsWith("users.json", started.GetStructuredStateValue("UsersFile"), StringComparison.Ordinal);
        Assert.Equal(Environment.ProcessId.ToString(System.Globalization.CultureInfo.InvariantCulture), started.GetStructuredStateValue("ProcessId"));
    }
}
