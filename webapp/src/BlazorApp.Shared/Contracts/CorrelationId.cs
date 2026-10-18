namespace BlazorApp.Shared.Contracts;

/// <summary>
/// The id that ties one user action together across the browser, the HTTP request and the
/// server's record event (plan section 8). The client makes one per button press and sends it
/// in <see cref="HeaderName"/>; the server makes one when a request has none.
/// </summary>
public static class CorrelationId
{
    /// <summary>The HTTP header that carries the id, in requests and responses.</summary>
    public const string HeaderName = "X-Correlation-Id";

    /// <summary>Longest id accepted from a client.</summary>
    public const int MaxLength = 64;

    /// <summary>Makes a new id.</summary>
    /// <returns>32 lowercase hex characters.</returns>
    public static string New() => Guid.NewGuid().ToString("N");

    /// <summary>
    /// Whether a client-supplied id can be used as is: 1 to <see cref="MaxLength"/> letters,
    /// digits, '-', '_' or '.', so it can't smuggle anything into a log line.
    /// </summary>
    /// <param name="value">The id from the header.</param>
    /// <returns>True if it is safe to use.</returns>
    public static bool IsValid(string? value) =>
        !string.IsNullOrEmpty(value)
        && value.Length <= MaxLength
        && value.All(c => char.IsAsciiLetterOrDigit(c) || c is '-' or '_' or '.');
}
