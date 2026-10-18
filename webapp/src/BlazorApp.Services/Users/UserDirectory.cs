using System.Text.Json;
using System.Text.Json.Serialization;
using BlazorApp.Shared.Dtos;

namespace BlazorApp.Services.Users;

/// <summary><see cref="IUserDirectory"/> over a fixed list of users.</summary>
public sealed class UserDirectory : IUserDirectory
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() },
    };

    private readonly Dictionary<string, UserDto> _byId;

    /// <summary>Creates a directory from a list of users.</summary>
    /// <param name="users">The users; ids must be unique and non-empty.</param>
    public UserDirectory(IEnumerable<UserDto> users)
    {
        ArgumentNullException.ThrowIfNull(users);
        All = [.. users];
        if (All.Any(u => string.IsNullOrWhiteSpace(u.Id)))
        {
            throw new ArgumentException("Every user needs an id.", nameof(users));
        }

        _byId = All.ToDictionary(u => u.Id, StringComparer.Ordinal);
    }

    /// <inheritdoc />
    public IReadOnlyList<UserDto> All { get; }

    /// <inheritdoc />
    public UserDto? Find(string? id) => id is not null && _byId.TryGetValue(id, out var user) ? user : null;

    /// <summary>Loads users from a JSON array such as <c>[{ "id": "alice", "name": "Alice", "roles": ["Registrant"] }]</c>.</summary>
    /// <param name="path">Path to the JSON file.</param>
    /// <returns>The loaded directory.</returns>
    public static UserDirectory LoadFromFile(string path)
    {
        using var stream = File.OpenRead(path);
        var users = JsonSerializer.Deserialize<List<UserDto>>(stream, JsonOptions)
            ?? throw new InvalidDataException($"{path} does not contain a list of users.");
        return new UserDirectory(users);
    }
}
