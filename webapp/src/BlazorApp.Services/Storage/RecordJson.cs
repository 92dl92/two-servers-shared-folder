using System.Text.Json;
using System.Text.Json.Serialization;
using BlazorApp.Shared.Dtos;

namespace BlazorApp.Services.Storage;

/// <summary>
/// The JSON format stores use for records: web casing, enums as names, indented so files are
/// readable when inspecting a data folder by hand.
/// </summary>
public static class RecordJson
{
    /// <summary>Serializer options for stored records.</summary>
    public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() },
    };

    /// <summary>Serializes a record.</summary>
    /// <param name="record">The record.</param>
    /// <returns>UTF-8 JSON.</returns>
    public static byte[] Serialize(RegistrationRecordDto record) => JsonSerializer.SerializeToUtf8Bytes(record, Options);

    /// <summary>Deserializes a record.</summary>
    /// <param name="json">UTF-8 JSON.</param>
    /// <returns>The record.</returns>
    /// <exception cref="JsonException">The data is not a complete record (e.g. a half-written file).</exception>
    public static RegistrationRecordDto Deserialize(ReadOnlySpan<byte> json) =>
        JsonSerializer.Deserialize<RegistrationRecordDto>(json, Options)
        ?? throw new JsonException("The data is not a record.");

    /// <summary>A deep copy, so stored records never share the mutable <see cref="RegistrationDto"/> with callers.</summary>
    /// <param name="record">The record.</param>
    /// <returns>An independent copy.</returns>
    public static RegistrationRecordDto Copy(RegistrationRecordDto record) => Deserialize(Serialize(record));

    /// <summary>Throws unless <paramref name="updated"/> is exactly one version after <paramref name="expectedVersion"/>.</summary>
    /// <param name="updated">The record about to be written.</param>
    /// <param name="expectedVersion">The version it replaces.</param>
    /// <exception cref="ArgumentException">The versions don't follow on.</exception>
    public static void EnsureNextVersion(RegistrationRecordDto updated, int expectedVersion)
    {
        ArgumentNullException.ThrowIfNull(updated);
        if (updated.Version != expectedVersion + 1)
        {
            throw new ArgumentException(
                $"A replacement must have version {expectedVersion + 1}, not {updated.Version}.", nameof(updated));
        }
    }
}
