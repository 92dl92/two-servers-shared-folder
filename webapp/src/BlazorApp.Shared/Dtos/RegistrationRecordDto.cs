namespace BlazorApp.Shared.Dtos;

/// <summary>A stored registration with its workflow state, version and history.</summary>
/// <param name="Id">Record identifier.</param>
/// <param name="Version">Starts at 1 and goes up by exactly 1 on every change.</param>
/// <param name="Status">Where the record is in the workflow.</param>
/// <param name="CreatedBy">Id of the user who created it (its owner).</param>
/// <param name="CreatedAt">When it was created.</param>
/// <param name="UpdatedBy">Id of the user who made the latest change.</param>
/// <param name="UpdatedAt">When the latest change was made.</param>
/// <param name="Data">The form data.</param>
/// <param name="History">Every change, oldest first; one entry per version.</param>
public sealed record RegistrationRecordDto(
    Guid Id,
    int Version,
    RecordStatus Status,
    string CreatedBy,
    DateTimeOffset CreatedAt,
    string UpdatedBy,
    DateTimeOffset UpdatedAt,
    RegistrationDto Data,
    IReadOnlyList<HistoryEntryDto> History);
