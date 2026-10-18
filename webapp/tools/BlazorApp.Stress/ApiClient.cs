using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BlazorApp.Shared.Contracts;
using BlazorApp.Shared.Dtos;

namespace BlazorApp.Stress;

/// <summary>The answer to one request.</summary>
/// <typeparam name="T">The body type of a successful answer.</typeparam>
/// <param name="Status">The HTTP status; null when no response arrived (the outcome is unknown).</param>
/// <param name="Body">The body of a 2xx answer; otherwise default.</param>
/// <param name="Elapsed">How long the request took.</param>
/// <param name="CorrelationId">The <c>X-Correlation-Id</c> sent.</param>
/// <param name="Refused">No connection could be made (e.g. a killed server's port), so nothing was sent: unlike other missing answers, the outcome is known.</param>
public sealed record ApiResponse<T>(HttpStatusCode? Status, T? Body, TimeSpan Elapsed, string CorrelationId, bool Refused = false)
{
    /// <summary>Whether the server answered 2xx.</summary>
    public bool Succeeded => Status is { } status && (int)status is >= 200 and < 300;
}

/// <summary>
/// Calls the record API as one "Acting as" user (the cookie), the way the browser does. Every
/// request carries a new <c>X-Correlation-Id</c>, so it can be found in the server's log. A
/// client keeps to its own server, or with random routing sends each request to a server
/// picked at random, as a load balancer without session affinity would.
/// </summary>
/// <remarks>Not thread-safe: each client sends one request at a time.</remarks>
public sealed class ApiClient
{
    private readonly (HttpClient Http, string Name)[] _servers;
    private readonly Random? _random;
    private readonly ConcurrentDictionary<string, int>? _requestsByServer;

    /// <summary>A client that keeps to one server.</summary>
    /// <param name="http">A client for the server; shared, since every request sets its own headers.</param>
    /// <param name="server">The server's name in the results (its port).</param>
    /// <param name="userId">The acting user.</param>
    public ApiClient(HttpClient http, string server, string userId)
        : this([(http, server)], userId, random: null, requestsByServer: null)
    {
    }

    /// <summary>A client that can reach several servers.</summary>
    /// <param name="servers">Every server, its own (home) server first.</param>
    /// <param name="userId">The acting user.</param>
    /// <param name="random">Picks a server for each request; null keeps to the home server.</param>
    /// <param name="requestsByServer">Counts the requests sent to each server, shared by the clients; null for none.</param>
    public ApiClient(IReadOnlyList<(HttpClient Http, string Name)> servers, string userId, Random? random, ConcurrentDictionary<string, int>? requestsByServer)
    {
        ArgumentOutOfRangeException.ThrowIfZero(servers.Count);
        _servers = [.. servers];
        _random = random;
        _requestsByServer = requestsByServer;
        UserId = userId;
        Server = Home;
    }

    /// <summary>The client's own server, where sticky routing keeps it.</summary>
    public string Home => _servers[0].Name;

    /// <summary>The server the latest request went to (the home server before any).</summary>
    public string Server { get; private set; }

    /// <summary>The acting user.</summary>
    public string UserId { get; }

    /// <summary>Lists the records the user may see.</summary>
    /// <param name="cancellationToken">Cancels the request.</param>
    /// <returns>The answer.</returns>
    public Task<ApiResponse<List<RegistrationSummaryDto>>> ListAsync(CancellationToken cancellationToken) =>
        SendAsync<List<RegistrationSummaryDto>>(HttpMethod.Get, ApiRoutes.Registrations, null, cancellationToken);

    /// <summary>Gets one record.</summary>
    /// <param name="id">Record id.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    /// <returns>The answer.</returns>
    public Task<ApiResponse<RegistrationRecordDto>> GetAsync(Guid id, CancellationToken cancellationToken) =>
        SendAsync<RegistrationRecordDto>(HttpMethod.Get, ApiRoutes.Registration(id), null, cancellationToken);

    /// <summary>Creates a draft record.</summary>
    /// <param name="data">Its form data.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    /// <returns>The answer.</returns>
    public Task<ApiResponse<SubmissionResultDto>> CreateAsync(RegistrationDto data, CancellationToken cancellationToken) =>
        SendAsync<SubmissionResultDto>(HttpMethod.Post, ApiRoutes.Registrations, data, cancellationToken);

    /// <summary>Sends one change: update, submit, approve, reject or comment.</summary>
    /// <param name="action">The change.</param>
    /// <param name="id">Record id.</param>
    /// <param name="expectedVersion">The version the change starts from.</param>
    /// <param name="data">New form data, for <see cref="RecordAction.Update"/>.</param>
    /// <param name="comment">Comment text, for <see cref="RecordAction.Comment"/>.</param>
    /// <param name="cancellationToken">Cancels the request.</param>
    /// <returns>The answer, with the changed record on success.</returns>
    public Task<ApiResponse<RegistrationRecordDto>> ChangeAsync(
        RecordAction action, Guid id, int expectedVersion, RegistrationDto? data, string? comment, CancellationToken cancellationToken) =>
        action switch
        {
            RecordAction.Update => SendAsync<RegistrationRecordDto>(
                HttpMethod.Put, ApiRoutes.Registration(id), new UpdateRecordDto(expectedVersion, data!), cancellationToken),
            RecordAction.Submit or RecordAction.Approve or RecordAction.Reject => SendAsync<RegistrationRecordDto>(
                HttpMethod.Post, ApiRoutes.RegistrationAction(id, action.ToString().ToLowerInvariant()), new ExpectedVersionDto(expectedVersion), cancellationToken),
            RecordAction.Comment => SendAsync<RegistrationRecordDto>(
                HttpMethod.Post, ApiRoutes.RegistrationAction(id, "comments"), new CommentRequestDto(expectedVersion, comment!), cancellationToken),
            _ => throw new ArgumentOutOfRangeException(nameof(action), action, "The workload doesn't send this action."),
        };

    private async Task<ApiResponse<T>> SendAsync<T>(HttpMethod method, string url, object? body, CancellationToken cancellationToken)
    {
        var (http, server) = _random is null ? _servers[0] : _servers[_random.Next(_servers.Length)];
        Server = server;
        _requestsByServer?.AddOrUpdate(server, 1, (_, n) => n + 1);
        var correlationId = $"stress-{CorrelationId.New()}";
        using var request = new HttpRequestMessage(method, url);
        request.Headers.Add("Cookie", $"{UserCookie.Name}={UserId}");
        request.Headers.Add(CorrelationId.HeaderName, correlationId);
        if (body is not null)
        {
            request.Content = JsonContent.Create(body, body.GetType());
        }

        var started = Stopwatch.GetTimestamp();
        try
        {
            using var response = await http.SendAsync(request, cancellationToken);
            var result = response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NoContent
                ? await response.Content.ReadFromJsonAsync<T>(cancellationToken)
                : default;
            return new ApiResponse<T>(response.StatusCode, result, Stopwatch.GetElapsedTime(started), correlationId);
        }
        catch (HttpRequestException e) when (e.HttpRequestError == HttpRequestError.ConnectionError)
        {
            // No connection at all: the request never reached a server.
            return new ApiResponse<T>(null, default, Stopwatch.GetElapsedTime(started), correlationId, Refused: true);
        }
        catch (Exception e) when (e is HttpRequestException or JsonException
                                  || (e is TaskCanceledException && !cancellationToken.IsCancellationRequested))
        {
            // No (usable) answer: the server may or may not have saved the change.
            return new ApiResponse<T>(null, default, Stopwatch.GetElapsedTime(started), correlationId);
        }
    }
}
