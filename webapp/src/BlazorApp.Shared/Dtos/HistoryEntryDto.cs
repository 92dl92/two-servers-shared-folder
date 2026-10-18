namespace BlazorApp.Shared.Dtos;

/// <summary>One change to a record: who did what, and when.</summary>
/// <param name="Version">The record's version after this change.</param>
/// <param name="Action">What was done.</param>
/// <param name="UserId">Who did it.</param>
/// <param name="At">When it happened.</param>
/// <param name="Comment">The comment text, for <see cref="RecordAction.Comment"/>; otherwise null.</param>
public sealed record HistoryEntryDto(int Version, RecordAction Action, string UserId, DateTimeOffset At, string? Comment = null);
