namespace BlazorApp.Shared.Dtos;

/// <summary>Lightweight view of a stored registration, used for listings.</summary>
/// <param name="Id">Registration identifier.</param>
/// <param name="FullName">First and last name combined.</param>
/// <param name="Email">Registrant email address.</param>
/// <param name="Country">Registrant country.</param>
/// <param name="CreatedAt">When the registration was created.</param>
/// <param name="CreatedBy">Id of the user who created it.</param>
/// <param name="Version">Current version.</param>
/// <param name="Status">Current workflow status.</param>
/// <param name="UpdatedAt">When it last changed.</param>
public sealed record RegistrationSummaryDto(
    Guid Id,
    string FullName,
    string Email,
    string Country,
    DateTimeOffset CreatedAt,
    string CreatedBy,
    int Version,
    RecordStatus Status,
    DateTimeOffset UpdatedAt)
{
    /// <summary>Builds the summary of a record.</summary>
    /// <param name="record">The record.</param>
    /// <returns>Its summary.</returns>
    public static RegistrationSummaryDto From(RegistrationRecordDto record)
    {
        ArgumentNullException.ThrowIfNull(record);
        return new(
            record.Id,
            $"{record.Data.FirstName} {record.Data.LastName}".Trim(),
            record.Data.Email,
            record.Data.Country,
            record.CreatedAt,
            record.CreatedBy,
            record.Version,
            record.Status,
            record.UpdatedAt);
    }
}
