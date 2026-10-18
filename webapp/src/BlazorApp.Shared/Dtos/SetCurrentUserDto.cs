namespace BlazorApp.Shared.Dtos;

/// <summary>Body of the request that selects the acting user.</summary>
/// <param name="UserId">Id of the user to act as, or null for nobody.</param>
public sealed record SetCurrentUserDto(string? UserId);
