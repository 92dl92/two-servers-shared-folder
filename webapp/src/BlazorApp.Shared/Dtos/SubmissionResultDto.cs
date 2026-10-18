namespace BlazorApp.Shared.Dtos;

/// <summary>Outcome of submitting a <see cref="RegistrationDto"/>.</summary>
/// <param name="Success">True when the registration was accepted and stored.</param>
/// <param name="Id">Identifier of the stored registration, when successful.</param>
/// <param name="Errors">Validation errors keyed by property name, when unsuccessful.</param>
public sealed record SubmissionResultDto(
    bool Success,
    Guid? Id,
    IReadOnlyDictionary<string, string[]> Errors)
{
    /// <summary>Creates a successful result.</summary>
    /// <param name="id">Identifier of the stored registration.</param>
    /// <returns>A result with <see cref="Success"/> set and no errors.</returns>
    public static SubmissionResultDto Ok(Guid id) =>
        new(true, id, new Dictionary<string, string[]>());

    /// <summary>Creates a failed result.</summary>
    /// <param name="errors">Validation errors keyed by property name.</param>
    /// <returns>A result with <see cref="Success"/> cleared.</returns>
    public static SubmissionResultDto Failed(IReadOnlyDictionary<string, string[]> errors) =>
        new(false, null, errors);
}
