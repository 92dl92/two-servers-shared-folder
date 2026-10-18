using BlazorApp.Services.Users;
using BlazorApp.Shared.Dtos;

namespace BlazorApp.Services.Tests;

/// <summary>Users for tests, one per role plus a second registrant.</summary>
public static class TestUsers
{
    /// <summary>A registrant.</summary>
    public static readonly UserDto Alice = new("alice", "Alice", [Role.Registrant]);

    /// <summary>Another registrant, for "own records" checks.</summary>
    public static readonly UserDto Bob = new("bob", "Bob", [Role.Registrant]);

    /// <summary>A reviewer.</summary>
    public static readonly UserDto Rita = new("rita", "Rita", [Role.Reviewer]);

    /// <summary>An admin.</summary>
    public static readonly UserDto Ada = new("ada", "Ada", [Role.Admin]);

    /// <summary>One user for each role.</summary>
    public static readonly IReadOnlyDictionary<Role, UserDto> ByRole = new Dictionary<Role, UserDto>
    {
        [Role.Registrant] = Alice,
        [Role.Reviewer] = Rita,
        [Role.Admin] = Ada,
    };
}

/// <summary><see cref="ICurrentUser"/> that a test can switch between users.</summary>
/// <param name="user">The initial acting user, or null for nobody.</param>
public sealed class SwitchableCurrentUser(UserDto? user) : ICurrentUser
{
    /// <inheritdoc />
    public UserDto? User { get; set; } = user;

    /// <inheritdoc />
    public string? RequestedId => RequestedIdOverride ?? User?.Id;

    /// <summary>When set, an id the client sent that is not a known user.</summary>
    public string? RequestedIdOverride { get; set; }
}
