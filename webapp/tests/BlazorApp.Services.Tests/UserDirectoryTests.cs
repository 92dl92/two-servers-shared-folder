using BlazorApp.Services.Users;
using BlazorApp.Shared.Dtos;

namespace BlazorApp.Services.Tests;

/// <summary>Loading <c>users.json</c>.</summary>
public sealed class UserDirectoryTests : IDisposable
{
    private readonly string _path = Path.Combine(Path.GetTempPath(), $"users-{Guid.NewGuid():N}.json");

    /// <inheritdoc />
    public void Dispose() => File.Delete(_path);

    [Fact]
    public void Loads_users_with_roles_by_name()
    {
        File.WriteAllText(_path, """
            [
              { "id": "alice", "name": "Alice", "roles": ["Registrant"] },
              { "id": "sam", "name": "Sam", "roles": ["Reviewer", "Admin"] }
            ]
            """);

        var directory = UserDirectory.LoadFromFile(_path);

        Assert.Equal(["alice", "sam"], directory.All.Select(u => u.Id));
        Assert.Equal([Role.Reviewer, Role.Admin], directory.Find("sam")!.Roles);
        Assert.Null(directory.Find("nobody"));
        Assert.Null(directory.Find(null));
    }

    [Fact]
    public void Duplicate_ids_are_rejected()
    {
        Assert.ThrowsAny<ArgumentException>(() => new UserDirectory(
        [
            new UserDto("a", "A", [Role.Admin]),
            new UserDto("a", "A again", [Role.Admin]),
        ]));
    }

    [Fact]
    public void The_shipped_users_file_has_every_role()
    {
        var shipped = Path.Combine(AppContext.BaseDirectory, "users.json");

        var directory = UserDirectory.LoadFromFile(shipped);

        Assert.Equal(
            Enum.GetValues<Role>().Order(),
            directory.All.SelectMany(u => u.Roles).Distinct().Order());
    }
}
