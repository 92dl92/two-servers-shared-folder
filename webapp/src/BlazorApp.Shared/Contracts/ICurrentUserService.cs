using BlazorApp.Shared.Dtos;

namespace BlazorApp.Shared.Contracts;

/// <summary>The "Acting as" user: who the app thinks is using it. There is no login.</summary>
/// <remarks>
/// The choice is kept in a cookie (<see cref="UserCookie.Name"/>) so the server knows the user
/// both when it prerenders a page and when the browser calls the API.
/// </remarks>
public interface ICurrentUserService
{
    /// <summary>Lists every user that can be picked.</summary>
    /// <param name="cancellationToken">Cancels the operation.</param>
    /// <returns>All users from <c>users.json</c>.</returns>
    Task<IReadOnlyList<UserDto>> GetUsersAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets the user currently acting.</summary>
    /// <param name="cancellationToken">Cancels the operation.</param>
    /// <returns>The user, or null when none is selected or the selected id is unknown.</returns>
    Task<UserDto?> GetCurrentUserAsync(CancellationToken cancellationToken = default);

    /// <summary>Selects the acting user.</summary>
    /// <param name="userId">Id of a user from <see cref="GetUsersAsync"/>, or null to select nobody.</param>
    /// <param name="cancellationToken">Cancels the operation.</param>
    /// <returns>A task that completes when the choice has been saved.</returns>
    Task SetCurrentUserAsync(string? userId, CancellationToken cancellationToken = default);
}
