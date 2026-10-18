namespace BlazorApp.Shared.Dtos;

/// <summary>
/// One event from the browser, sent to the server in batches and written to the server's log
/// file marked <c>"Source": "client"</c>. Names, ids and status codes only, never form values.
/// </summary>
/// <param name="At">When it happened in the browser.</param>
/// <param name="Event">What happened: <c>PageLoaded</c>, <c>ButtonPressed</c>, <c>ResultShown</c>, <c>ApiFailed</c>.</param>
/// <param name="Page">The page's route template, e.g. <c>records/{id}</c>.</param>
/// <param name="Action">The button pressed, e.g. <c>Save</c>; null for page loads.</param>
/// <param name="Result">The result shown, e.g. <c>Saved</c>, <c>Conflict</c>, <c>Invalid</c>.</param>
/// <param name="RecordId">The record involved, if any.</param>
/// <param name="CorrelationId">The user action's correlation id.</param>
/// <param name="StatusCode">The HTTP status of a failed API call.</param>
public sealed record ClientLogEntryDto(
    DateTimeOffset At,
    string Event,
    string? Page = null,
    string? Action = null,
    string? Result = null,
    Guid? RecordId = null,
    string? CorrelationId = null,
    int? StatusCode = null);
