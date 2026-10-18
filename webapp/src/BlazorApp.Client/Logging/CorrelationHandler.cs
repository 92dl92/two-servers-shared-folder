using BlazorApp.Shared.Contracts;

namespace BlazorApp.Client.Logging;

/// <summary>
/// Adds the current user action's correlation id to every API call
/// (<see cref="CorrelationId.HeaderName"/>) and logs calls that fail.
/// </summary>
/// <param name="log">The client event log, which knows the action in progress.</param>
public sealed class CorrelationHandler(IClientEventLog log) : DelegatingHandler
{
    /// <inheritdoc />
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (log.CurrentCorrelationId is { } id)
        {
            request.Headers.TryAddWithoutValidation(CorrelationId.HeaderName, id);
        }

        var response = await base.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            log.ApiFailed(request.Method.Method, request.RequestUri?.AbsolutePath ?? "?", (int)response.StatusCode);
        }

        return response;
    }
}
