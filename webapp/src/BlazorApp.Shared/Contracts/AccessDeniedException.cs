namespace BlazorApp.Shared.Contracts;

/// <summary>Why an action was refused before it was attempted.</summary>
public enum AccessDenial
{
    /// <summary>No user is selected, or the selected user is not in <c>users.json</c>. HTTP 401.</summary>
    NoUser,

    /// <summary>The user's roles do not allow the action. HTTP 403.</summary>
    Forbidden,
}

/// <summary>Thrown when the acting user may not perform an action.</summary>
/// <param name="reason">Why access was denied.</param>
/// <param name="message">Human-readable explanation.</param>
public sealed class AccessDeniedException(AccessDenial reason, string message) : Exception(message)
{
    /// <summary>Why access was denied.</summary>
    public AccessDenial Reason { get; } = reason;
}
