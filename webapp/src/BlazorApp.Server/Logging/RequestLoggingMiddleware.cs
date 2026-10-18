using System.Diagnostics;
using BlazorApp.Shared.Contracts;

namespace BlazorApp.Server.Logging;

/// <summary>
/// Gives every request a correlation id and writes one line per API request (plan section 8).
/// </summary>
/// <remarks>
/// The id comes from the <see cref="CorrelationId.HeaderName"/> header when it is valid, and is
/// made up otherwise. It is echoed in the response header and put in a logging scope, so the
/// HTTP line, the record event and anything else logged while handling the request carry it.
/// </remarks>
/// <param name="next">The rest of the pipeline.</param>
/// <param name="logger">Where the HTTP lines go.</param>
public sealed class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    private static readonly Action<ILogger, string, string, int, long, string, Exception?> RequestLine =
        LoggerMessage.Define<string, string, int, long, string>(
            LogLevel.Information,
            LogEvents.HttpRequest,
            "HTTP {Method} {Route} responded {StatusCode} in {DurationMs} ms ({CorrelationId})");

    /// <summary>Key under which the request's correlation id is kept in <see cref="HttpContext.Items"/>.</summary>
    public const string ItemKey = "CorrelationId";

    /// <summary>Handles one request.</summary>
    /// <param name="context">The request.</param>
    /// <returns>A task that completes when the request has been handled.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        var sent = context.Request.Headers[CorrelationId.HeaderName].ToString();
        var id = CorrelationId.IsValid(sent) ? sent : CorrelationId.New();
        context.Items[ItemKey] = id;
        context.Response.Headers[CorrelationId.HeaderName] = id;

        using var scope = logger.BeginScope(new Dictionary<string, object?> { ["CorrelationId"] = id });
        var started = Stopwatch.GetTimestamp();
        var failed = false;
        try
        {
            await next(context);
        }
        catch
        {
            // The response isn't 500 yet: an outer handler turns the exception into one.
            failed = true;
            throw;
        }
        finally
        {
            var path = context.Request.Path;

            // Client log batches would otherwise log a line about every batch of log lines.
            if (path.StartsWithSegments("/api") && !path.StartsWithSegments("/" + ApiRoutes.ClientLogs))
            {
                // The route template (api/registrations/{id:guid}) rather than the path, so lines group by endpoint.
                var route = (context.GetEndpoint() as RouteEndpoint)?.RoutePattern.RawText ?? path.ToString();
                RequestLine(
                    logger,
                    context.Request.Method,
                    route,
                    failed ? StatusCodes.Status500InternalServerError : context.Response.StatusCode,
                    (long)Stopwatch.GetElapsedTime(started).TotalMilliseconds,
                    id,
                    null);
            }
        }
    }
}
