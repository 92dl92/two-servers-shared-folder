using BlazorApp.Shared.Dtos;

namespace BlazorApp.Shared.Contracts;

/// <summary>
/// Registration records and their workflow, as used by the UI.
/// </summary>
/// <remarks>
/// <para>
/// Implemented twice: by <c>BlazorApp.Services</c> (in-process, used by the Server
/// and during prerendering) and by <c>BlazorApp.Client</c> (over HTTP, used in the
/// browser). Components depend only on this interface, so they work in both places.
/// </para>
/// <para>
/// Every method acts as the current "Acting as" user. Failures are exceptions, checked in this
/// order: <see cref="AccessDeniedException"/> (no user: 401), <see cref="RecordNotFoundException"/>
/// (404), <see cref="AccessDeniedException"/> (role or ownership: 403),
/// <see cref="RecordValidationException"/> (400), <see cref="VersionConflictException"/>
/// (out-of-date version: 409), <see cref="WrongStatusException"/> (422).
/// Every successful change raises the version by exactly 1 and adds one history entry.
/// </para>
/// </remarks>
public interface IRegistrationService
{
    /// <summary>Validates and stores a new record as a draft, owned by the acting user.</summary>
    /// <param name="registration">The form data to submit.</param>
    /// <param name="cancellationToken">Cancels the operation.</param>
    /// <returns>The outcome, including any validation errors.</returns>
    Task<SubmissionResultDto> CreateAsync(
        RegistrationDto registration,
        CancellationToken cancellationToken = default);

    /// <summary>Lists the records the acting user may see, newest first.</summary>
    /// <param name="cancellationToken">Cancels the operation.</param>
    /// <returns>Summaries of all visible records.</returns>
    Task<IReadOnlyList<RegistrationSummaryDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    /// <summary>Gets one record.</summary>
    /// <param name="id">Record id.</param>
    /// <param name="cancellationToken">Cancels the operation.</param>
    /// <returns>The record.</returns>
    Task<RegistrationRecordDto> GetAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Replaces a record's form data.</summary>
    /// <param name="id">Record id.</param>
    /// <param name="expectedVersion">The version the user started editing from.</param>
    /// <param name="data">The new form data.</param>
    /// <param name="cancellationToken">Cancels the operation.</param>
    /// <returns>The updated record.</returns>
    Task<RegistrationRecordDto> UpdateAsync(
        Guid id, int expectedVersion, RegistrationDto data, CancellationToken cancellationToken = default);

    /// <summary>Sends a draft for review.</summary>
    /// <param name="id">Record id.</param>
    /// <param name="expectedVersion">The version the user was looking at.</param>
    /// <param name="cancellationToken">Cancels the operation.</param>
    /// <returns>The updated record.</returns>
    Task<RegistrationRecordDto> SubmitAsync(Guid id, int expectedVersion, CancellationToken cancellationToken = default);

    /// <summary>Approves a submitted record.</summary>
    /// <param name="id">Record id.</param>
    /// <param name="expectedVersion">The version the user was looking at.</param>
    /// <param name="cancellationToken">Cancels the operation.</param>
    /// <returns>The updated record.</returns>
    Task<RegistrationRecordDto> ApproveAsync(Guid id, int expectedVersion, CancellationToken cancellationToken = default);

    /// <summary>Rejects a submitted record.</summary>
    /// <param name="id">Record id.</param>
    /// <param name="expectedVersion">The version the user was looking at.</param>
    /// <param name="cancellationToken">Cancels the operation.</param>
    /// <returns>The updated record.</returns>
    Task<RegistrationRecordDto> RejectAsync(Guid id, int expectedVersion, CancellationToken cancellationToken = default);

    /// <summary>Adds a comment to a record's history.</summary>
    /// <param name="id">Record id.</param>
    /// <param name="expectedVersion">The version the user was looking at.</param>
    /// <param name="text">The comment (required, at most 1000 characters).</param>
    /// <param name="cancellationToken">Cancels the operation.</param>
    /// <returns>The updated record.</returns>
    Task<RegistrationRecordDto> CommentAsync(
        Guid id, int expectedVersion, string text, CancellationToken cancellationToken = default);

    /// <summary>Deletes a record.</summary>
    /// <param name="id">Record id.</param>
    /// <param name="expectedVersion">The version the user was looking at.</param>
    /// <param name="cancellationToken">Cancels the operation.</param>
    /// <returns>A task that completes when the record is gone.</returns>
    Task DeleteAsync(Guid id, int expectedVersion, CancellationToken cancellationToken = default);
}
