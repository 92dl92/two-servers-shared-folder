using BlazorApp.Shared.Dtos;
using Microsoft.Extensions.Logging;

namespace BlazorApp.Services.Logging;

/// <summary>How a command on a record ended, as recorded in its record event.</summary>
public enum RecordOutcome
{
    /// <summary>A change was saved.</summary>
    Saved = 0,

    /// <summary>No user, or an unknown one (401).</summary>
    NoUser = 1,

    /// <summary>The user's role or ownership rules it out (403).</summary>
    Forbidden = 2,

    /// <summary>The record does not exist (404).</summary>
    NotFound = 3,

    /// <summary>The data was invalid (400).</summary>
    Invalid = 4,

    /// <summary>Out-of-date version (409).</summary>
    Conflict = 5,

    /// <summary>The status doesn't allow the action (422).</summary>
    WrongStatus = 6,

    /// <summary>A View succeeded (nothing was saved).</summary>
    Read = 7,
}

/// <summary>
/// The record event: one log entry per command attempt on a record, written by
/// <see cref="RegistrationService"/> (plan section 8). Each action and outcome has its own
/// fixed <see cref="EventId"/>: the action's base number plus the outcome's number, e.g.
/// Update (1200) + Conflict (5) = 1205 "RecordUpdateConflict".
/// </summary>
public static class RecordEvents
{
    /// <summary>The message template; its placeholders are the event's fields.</summary>
    public const string Template =
        "Record {Action} {Outcome} by {UserId}: record {RecordId}, version {ExpectedVersion} -> {NewVersion}, " +
        "status {OldStatus} -> {NewStatus}, invalid fields [{InvalidFields}], {DurationMs} ms";

    /// <summary>The fixed event id for an action and outcome.</summary>
    /// <param name="action">The command.</param>
    /// <param name="outcome">How it ended.</param>
    /// <returns>The event id, named e.g. <c>RecordUpdateConflict</c>.</returns>
    public static EventId IdFor(RecordAction action, RecordOutcome outcome) =>
        new(BaseFor(action) + (int)outcome, $"Record{action}{outcome}");

    /// <summary>
    /// The level for an action and outcome: Information when saved, Warning when refused,
    /// and Debug for every View, so reads are off by default (decision L3).
    /// </summary>
    /// <param name="action">The command.</param>
    /// <param name="outcome">How it ended.</param>
    /// <returns>The log level.</returns>
    public static LogLevel LevelFor(RecordAction action, RecordOutcome outcome) =>
        action == RecordAction.View ? LogLevel.Debug
        : outcome == RecordOutcome.Saved ? LogLevel.Information
        : LogLevel.Warning;

    /// <summary>Writes one record event.</summary>
    /// <param name="logger">The service's logger.</param>
    /// <param name="info">What happened.</param>
    public static void Log(ILogger logger, RecordEventInfo info)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(info);
        var level = LevelFor(info.Action, info.Outcome);
        if (!logger.IsEnabled(level))
        {
            return;
        }

        // The level varies by action and outcome, and there are ten fields, so LoggerMessage.Define doesn't fit.
        logger.Log(
            level,
            IdFor(info.Action, info.Outcome),
            Template,
            info.Action,
            info.Outcome,
            info.UserId,
            info.RecordId,
            info.ExpectedVersion,
            info.NewVersion,
            info.OldStatus,
            info.NewStatus,
            string.Join(", ", info.InvalidFields.Order(StringComparer.Ordinal)),
            (long)info.Duration.TotalMilliseconds);
    }

    private static int BaseFor(RecordAction action) => action switch
    {
        RecordAction.Create => 1100,
        RecordAction.Update => 1200,
        RecordAction.Submit => 1300,
        RecordAction.Approve => 1400,
        RecordAction.Reject => 1500,
        RecordAction.Comment => 1600,
        RecordAction.Delete => 1700,
        RecordAction.View => 1800,
        _ => throw new ArgumentOutOfRangeException(nameof(action)),
    };
}

/// <summary>
/// The fields of one record event, filled in as a command runs. Ids, versions, statuses and
/// field names only: never form values or comment text (plan section 8, Privacy).
/// </summary>
/// <param name="action">The command.</param>
/// <param name="recordId">The record, when the command is about one.</param>
/// <param name="expectedVersion">The version the caller sent, if any.</param>
public sealed class RecordEventInfo(RecordAction action, Guid? recordId, int? expectedVersion)
{
    /// <summary>The command.</summary>
    public RecordAction Action { get; } = action;

    /// <summary>The record; set by Create once the id exists.</summary>
    public Guid? RecordId { get; set; } = recordId;

    /// <summary>The version the caller sent.</summary>
    public int? ExpectedVersion { get; } = expectedVersion;

    /// <summary>The id the caller acted as (even if unknown).</summary>
    public string? UserId { get; set; }

    /// <summary>How it ended; set when the command finishes.</summary>
    public RecordOutcome Outcome { get; set; }

    /// <summary>The version written, when something was saved.</summary>
    public int? NewVersion { get; set; }

    /// <summary>The record's status before the command, once loaded.</summary>
    public RecordStatus? OldStatus { get; set; }

    /// <summary>The status after a saved change.</summary>
    public RecordStatus? NewStatus { get; set; }

    /// <summary>Names of the invalid fields, for <see cref="RecordOutcome.Invalid"/>.</summary>
    public IReadOnlyList<string> InvalidFields { get; set; } = [];

    /// <summary>How long the command took.</summary>
    public TimeSpan Duration { get; set; }
}
