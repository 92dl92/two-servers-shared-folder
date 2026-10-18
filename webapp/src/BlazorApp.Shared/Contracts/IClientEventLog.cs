namespace BlazorApp.Shared.Contracts;

/// <summary>
/// Client events: page loads, buttons pressed and the results shown (plan section 8). In the
/// browser they go to the console and, in batches, to the server's log file. Components use
/// this to log what the user does; it never receives form values.
/// </summary>
public interface IClientEventLog
{
    /// <summary>The correlation id of the user action in progress, sent with API calls; null when none.</summary>
    string? CurrentCorrelationId { get; }

    /// <summary>Logs that a page finished loading in the browser.</summary>
    /// <param name="page">The page's route template, e.g. <c>records/{id}</c>.</param>
    /// <param name="recordId">The record shown, if any.</param>
    /// <param name="result">What the page showed when it is not simply loaded, e.g. <c>NotFound</c>.</param>
    void PageLoaded(string page, Guid? recordId = null, string? result = null);

    /// <summary>
    /// Logs a button press and starts a user action with a new correlation id, which API calls
    /// made until it is disposed carry in their header.
    /// </summary>
    /// <param name="page">The page's route template.</param>
    /// <param name="action">The button, e.g. <c>Save</c>.</param>
    /// <param name="recordId">The record involved, if any.</param>
    /// <returns>The action; call <see cref="IClientAction.Result"/> with what the user was shown.</returns>
    IClientAction ButtonPressed(string page, string action, Guid? recordId = null);

    /// <summary>Logs an API call that failed.</summary>
    /// <param name="method">HTTP method.</param>
    /// <param name="path">Request path, without the query.</param>
    /// <param name="statusCode">The status code.</param>
    void ApiFailed(string method, string path, int statusCode);

    /// <summary>Sends any queued events now, e.g. before the page reloads.</summary>
    /// <returns>A task that completes when they have been sent (or sending failed).</returns>
    Task FlushAsync();
}

/// <summary>One user action started by a button press.</summary>
public interface IClientAction : IDisposable
{
    /// <summary>The action's correlation id.</summary>
    string CorrelationId { get; }

    /// <summary>Logs the result the user was shown, e.g. <c>Saved</c> or <c>Conflict</c>.</summary>
    /// <param name="result">The result.</param>
    void Result(string result);
}
