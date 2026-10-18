namespace BlazorApp.Shared.Dtos;

/// <summary>A user from <c>users.json</c>. Anyone can act as any user: there is no login.</summary>
/// <param name="Id">Stable identifier, stored in the "Acting as" cookie.</param>
/// <param name="Name">Display name.</param>
/// <param name="Roles">Roles the user holds.</param>
public sealed record UserDto(string Id, string Name, IReadOnlyList<Role> Roles);
