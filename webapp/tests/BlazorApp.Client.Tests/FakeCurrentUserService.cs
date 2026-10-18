using BlazorApp.Shared.Contracts;
using BlazorApp.Shared.Dtos;

namespace BlazorApp.Client.Tests;

/// <summary>Test double for <see cref="ICurrentUserService"/> with a fixed list of users.</summary>
public sealed class FakeCurrentUserService : ICurrentUserService
{
    /// <summary>A registrant.</summary>
    public static readonly UserDto Alice = new("alice", "Alice", [Role.Registrant]);

    /// <summary>A reviewer.</summary>
    public static readonly UserDto Rita = new("rita", "Rita", [Role.Reviewer]);

    /// <summary>An admin.</summary>
    public static readonly UserDto Ada = new("ada", "Ada", [Role.Admin]);

    /// <summary>The acting user; null for nobody.</summary>
    public UserDto? Current { get; set; }

    /// <summary>Ids passed to <see cref="SetCurrentUserAsync"/>, in order.</summary>
    public List<string?> Changes { get; } = [];

    /// <inheritdoc />
    public Task<IReadOnlyList<UserDto>> GetUsersAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<UserDto>>([Alice, Rita, Ada]);

    /// <inheritdoc />
    public Task<UserDto?> GetCurrentUserAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(Current);

    /// <inheritdoc />
    public Task SetCurrentUserAsync(string? userId, CancellationToken cancellationToken = default)
    {
        Changes.Add(userId);
        return Task.CompletedTask;
    }
}
