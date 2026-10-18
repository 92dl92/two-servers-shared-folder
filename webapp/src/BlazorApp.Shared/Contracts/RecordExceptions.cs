using BlazorApp.Shared.Dtos;

namespace BlazorApp.Shared.Contracts;

/// <summary>The record does not exist (or was deleted). HTTP 404.</summary>
/// <param name="id">The id that was asked for.</param>
/// <param name="message">Explanation; a default is built when null.</param>
public sealed class RecordNotFoundException(Guid id, string? message = null)
    : Exception(message ?? $"Record {id} does not exist.")
{
    /// <summary>The id that was asked for.</summary>
    public Guid Id { get; } = id;
}

/// <summary>
/// The record changed since the user loaded it: the stored version is not the one they started
/// from. HTTP 409. This is optimistic concurrency working, not an error in the system.
/// </summary>
/// <param name="id">The record.</param>
/// <param name="expectedVersion">The version the user started from.</param>
/// <param name="actualVersion">The stored version, when known.</param>
/// <param name="message">Explanation; a default is built when null.</param>
public sealed class VersionConflictException(Guid id, int expectedVersion, int? actualVersion, string? message = null)
    : Exception(message ?? $"Record {id} has changed: expected version {expectedVersion}, found {actualVersion?.ToString() ?? "a newer one"}.")
{
    /// <summary>The record.</summary>
    public Guid Id { get; } = id;

    /// <summary>The version the user started from.</summary>
    public int ExpectedVersion { get; } = expectedVersion;

    /// <summary>The stored version, when known.</summary>
    public int? ActualVersion { get; } = actualVersion;
}

/// <summary>The workflow does not allow the action in the record's current status. HTTP 422.</summary>
/// <param name="action">The refused action.</param>
/// <param name="status">The record's status, when known.</param>
/// <param name="message">Explanation; a default is built when null.</param>
public sealed class WrongStatusException(RecordAction action, RecordStatus? status, string? message = null)
    : Exception(message ?? $"Can't {action.ToString().ToLowerInvariant()} a record that is {status?.ToString().ToLowerInvariant() ?? "in this status"}.")
{
    /// <summary>The refused action.</summary>
    public RecordAction Action { get; } = action;

    /// <summary>The record's status, when known.</summary>
    public RecordStatus? Status { get; } = status;
}

/// <summary>The submitted data is invalid. HTTP 400.</summary>
/// <param name="errors">Messages keyed by field name.</param>
public sealed class RecordValidationException(IReadOnlyDictionary<string, string[]> errors)
    : Exception("The data is invalid.")
{
    /// <summary>Messages keyed by field name.</summary>
    public IReadOnlyDictionary<string, string[]> Errors { get; } = errors;
}
