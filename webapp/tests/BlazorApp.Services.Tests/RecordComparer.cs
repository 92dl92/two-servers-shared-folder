using System.Text.Json;
using BlazorApp.Shared.Dtos;

namespace BlazorApp.Services.Tests;

/// <summary>
/// Compares records by value, including the form data and history lists (which the record's
/// own equality compares by reference).
/// </summary>
public sealed class RecordComparer : IEqualityComparer<RegistrationRecordDto?>
{
    /// <summary>The shared instance.</summary>
    public static readonly RecordComparer Instance = new();

    /// <inheritdoc />
    public bool Equals(RegistrationRecordDto? x, RegistrationRecordDto? y) =>
        JsonSerializer.Serialize(x) == JsonSerializer.Serialize(y);

    /// <inheritdoc />
    public int GetHashCode([System.Diagnostics.CodeAnalysis.DisallowNull] RegistrationRecordDto? obj) => JsonSerializer.Serialize(obj).GetHashCode(StringComparison.Ordinal);
}
