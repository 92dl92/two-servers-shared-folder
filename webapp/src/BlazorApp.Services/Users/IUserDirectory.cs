using BlazorApp.Shared.Dtos;

namespace BlazorApp.Services.Users;

/// <summary>The known users, loaded from <c>users.json</c> at startup.</summary>
public interface IUserDirectory
{
    /// <summary>All users, in file order.</summary>
    IReadOnlyList<UserDto> All { get; }

    /// <summary>Looks up a user by id.</summary>
    /// <param name="id">User id; null or unknown ids give null.</param>
    /// <returns>The user, or null.</returns>
    UserDto? Find(string? id);
}
