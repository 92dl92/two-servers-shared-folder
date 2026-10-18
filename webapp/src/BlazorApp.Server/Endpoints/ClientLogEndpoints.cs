using BlazorApp.Server.Logging;
using BlazorApp.Shared.Contracts;
using BlazorApp.Shared.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BlazorApp.Server.Endpoints;

/// <summary>
/// Receives client events from the browser and writes them to the server's log, marked
/// <c>"Source": "client"</c> (decision L2), so one file tells the whole story of a user action.
/// </summary>
public static class ClientLogEndpoints
{
    /// <summary>Most entries accepted in one batch.</summary>
    public const int MaxBatch = 100;

    /// <summary>Longest text kept from any field; longer values are cut, and control characters become spaces.</summary>
    public const int MaxFieldLength = 100;

    private const string Template =
        "Client {Event} on {Page}: action {Action}, result {Result}, record {RecordId}, status {StatusCode}, at {ClientTime}";

    /// <summary>Maps <c>POST</c> on <see cref="ApiRoutes.ClientLogs"/>.</summary>
    /// <param name="app">The endpoint route builder.</param>
    /// <returns>The same builder, for chaining.</returns>
    public static IEndpointRouteBuilder MapClientLogEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost(ApiRoutes.ClientLogs, Receive).WithTags("Logging");
        return app;
    }

    /// <summary>Logs each entry with its own correlation id.</summary>
    /// <param name="entries">The batch.</param>
    /// <param name="loggerFactory">For the <c>BlazorApp.Client</c> category.</param>
    /// <returns>204, or 400 when the batch is too big or has empty entries.</returns>
    private static Results<NoContent, ProblemHttpResult> Receive(List<ClientLogEntryDto?> entries, ILoggerFactory loggerFactory)
    {
        if (entries.Count > MaxBatch)
        {
            return TypedResults.Problem($"At most {MaxBatch} entries per batch.", statusCode: StatusCodes.Status400BadRequest);
        }

        if (entries.Any(e => e is null))
        {
            return TypedResults.Problem("A batch can't contain null entries.", statusCode: StatusCodes.Status400BadRequest);
        }

        var logger = loggerFactory.CreateLogger("BlazorApp.Client");
        foreach (var entry in entries.OfType<ClientLogEntryDto>())
        {
            // The entry's own id replaces the batch request's; an entry without a valid one gets none.
            var scope = new Dictionary<string, object?>
            {
                ["Source"] = "client",
                ["CorrelationId"] = CorrelationId.IsValid(entry.CorrelationId) ? entry.CorrelationId : null,
            };

            using (logger.BeginScope(scope))
            {
                logger.Log(
                    LogLevel.Information,
                    LogEvents.ClientEvent,
                    Template,
                    Clean(entry.Event) ?? "Unknown",
                    Clean(entry.Page),
                    Clean(entry.Action),
                    Clean(entry.Result),
                    entry.RecordId,
                    entry.StatusCode,
                    entry.At);
            }
        }

        return TypedResults.NoContent();
    }

    /// <summary>
    /// Shortens client text and turns control characters (line breaks, tabs, escapes) into
    /// spaces, so a client can't forge extra lines in the console output.
    /// </summary>
    private static string? Clean(string? value)
    {
        if (value is null)
        {
            return null;
        }

        var cut = value.Length > MaxFieldLength ? value[..MaxFieldLength] : value;
        return string.Create(cut.Length, cut, (span, text) =>
        {
            for (var i = 0; i < text.Length; i++)
            {
                span[i] = char.IsControl(text[i]) ? ' ' : text[i];
            }
        });
    }
}
