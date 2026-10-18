namespace BlazorApp.Shared.Dtos;

/// <summary>Body of a workflow action (submit, approve, reject).</summary>
/// <param name="ExpectedVersion">The version the user started from; a different stored version is a conflict.</param>
public sealed record ExpectedVersionDto(int ExpectedVersion);

/// <summary>Body of an update.</summary>
/// <param name="ExpectedVersion">The version the user started editing from.</param>
/// <param name="Data">The new form data.</param>
public sealed record UpdateRecordDto(int ExpectedVersion, RegistrationDto Data);

/// <summary>Body of a new comment.</summary>
/// <param name="ExpectedVersion">The version the user was looking at.</param>
/// <param name="Text">The comment.</param>
public sealed record CommentRequestDto(int ExpectedVersion, string Text);
