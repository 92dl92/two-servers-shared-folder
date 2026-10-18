using BlazorApp.Shared.Dtos;

namespace BlazorApp.Services.Users;

/// <summary>The acting user for the current request, as the Services layer sees it.</summary>
/// <remarks>The Server implements this from the "Acting as" cookie.</remarks>
public interface ICurrentUser
{
    /// <summary>The acting user, or null when none is selected or the id is unknown.</summary>
    UserDto? User { get; }

    /// <summary>The id the client sent, even if unknown; null when none was sent.</summary>
    string? RequestedId { get; }
}
