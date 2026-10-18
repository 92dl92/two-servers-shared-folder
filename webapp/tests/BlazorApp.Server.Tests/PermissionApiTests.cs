using System.Net;
using System.Net.Http.Json;
using BlazorApp.Shared.Contracts;
using BlazorApp.Shared.Dtos;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BlazorApp.Server.Tests;

/// <summary>The API enforces the role × action table and respects the "Acting as" cookie.</summary>
public sealed class PermissionApiTests : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory = new();

    /// <inheritdoc />
    public void Dispose() => _factory.Dispose();

    /// <summary>
    /// Expected status for every user in users.json and every action the API exposes, written
    /// out by hand. Create is POST, View is GET on the registrations route.
    /// </summary>
    public static TheoryData<string, string, HttpStatusCode> Table => new()
    {
        { "alice", "Create", HttpStatusCode.Created },
        { "alice", "View", HttpStatusCode.OK },
        { "bob", "Create", HttpStatusCode.Created },
        { "bob", "View", HttpStatusCode.OK },
        { "rita", "Create", HttpStatusCode.Forbidden },
        { "rita", "View", HttpStatusCode.OK },
        { "ravi", "Create", HttpStatusCode.Forbidden },
        { "ravi", "View", HttpStatusCode.OK },
        { "ada", "Create", HttpStatusCode.Created },
        { "ada", "View", HttpStatusCode.OK },
    };

    [Theory]
    [MemberData(nameof(Table))]
    public async Task Each_user_gets_the_expected_status_for_each_action(string user, string action, HttpStatusCode expected)
    {
        using var response = await SendAsync(_factory.ClientAs(user), action);

        Assert.Equal(expected, response.StatusCode);
    }

    [Theory]
    [InlineData("Create")]
    [InlineData("View")]
    public async Task Missing_user_gets_401(string action)
    {
        using var response = await SendAsync(_factory.ClientAs(null), action);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Contains("No user selected", await response.Content.ReadAsStringAsync());
    }

    [Theory]
    [InlineData("Create")]
    [InlineData("View")]
    public async Task Unknown_user_gets_401_naming_the_id(string action)
    {
        using var response = await SendAsync(_factory.ClientAs("mallory"), action);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Contains("mallory", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Forbidden_create_stores_nothing()
    {
        using var response = await SendAsync(_factory.ClientAs("rita"), "Create");

        var all = await _factory.ClientAs("ada").GetFromJsonAsync<List<RegistrationSummaryDto>>(ApiRoutes.Registrations);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Empty(all!);
    }

    [Fact]
    public async Task The_cookie_decides_who_created_a_record_and_who_sees_it()
    {
        (await SendAsync(_factory.ClientAs("alice"), "Create")).Dispose();
        (await SendAsync(_factory.ClientAs("bob"), "Create")).Dispose();

        var alice = await _factory.ClientAs("alice").GetFromJsonAsync<List<RegistrationSummaryDto>>(ApiRoutes.Registrations);
        var rita = await _factory.ClientAs("rita").GetFromJsonAsync<List<RegistrationSummaryDto>>(ApiRoutes.Registrations);

        Assert.Equal(["alice"], alice!.Select(r => r.CreatedBy));
        Assert.Equal(["alice", "bob"], rita!.Select(r => r.CreatedBy).Order());
    }

    [Fact]
    public async Task Picking_a_user_sets_the_cookie_that_later_requests_use()
    {
        var client = _factory.CreateClient(); // keeps cookies, like a browser

        Assert.Equal(HttpStatusCode.NoContent, (await client.GetAsync(ApiRoutes.CurrentUser)).StatusCode);

        using var put = await client.PutAsJsonAsync(ApiRoutes.CurrentUser, new SetCurrentUserDto("rita"));
        Assert.Equal(HttpStatusCode.NoContent, put.StatusCode);
        Assert.Contains(put.Headers.GetValues("Set-Cookie"), c => c.StartsWith($"{UserCookie.Name}=rita", StringComparison.Ordinal));

        var current = await client.GetFromJsonAsync<UserDto>(ApiRoutes.CurrentUser);
        Assert.Equal("rita", current!.Id);
        Assert.Equal([Role.Reviewer], current.Roles);
        Assert.Equal(HttpStatusCode.Forbidden, (await SendAsync(client, "Create")).StatusCode);

        using var clear = await client.PutAsJsonAsync(ApiRoutes.CurrentUser, new SetCurrentUserDto(null));
        Assert.Equal(HttpStatusCode.NoContent, clear.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, (await client.GetAsync(ApiRoutes.CurrentUser)).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await SendAsync(client, "View")).StatusCode);
    }

    [Fact]
    public async Task Picking_an_unknown_user_is_a_400_and_changes_nothing()
    {
        var client = _factory.CreateClient();
        (await client.PutAsJsonAsync(ApiRoutes.CurrentUser, new SetCurrentUserDto("alice"))).Dispose();

        using var put = await client.PutAsJsonAsync(ApiRoutes.CurrentUser, new SetCurrentUserDto("mallory"));

        Assert.Equal(HttpStatusCode.BadRequest, put.StatusCode);
        Assert.Equal("alice", (await client.GetFromJsonAsync<UserDto>(ApiRoutes.CurrentUser))!.Id);
    }

    [Fact]
    public async Task Users_list_is_the_users_file()
    {
        var users = await _factory.ClientAs(null).GetFromJsonAsync<List<UserDto>>(ApiRoutes.Users);

        Assert.Equal(["alice", "bob", "rita", "ravi", "ada"], users!.Select(u => u.Id));
    }

    private static Task<HttpResponseMessage> SendAsync(HttpClient client, string action) => action switch
    {
        "Create" => client.PostAsJsonAsync(ApiRoutes.Registrations, Valid()),
        "View" => client.GetAsync(ApiRoutes.Registrations),
        _ => throw new ArgumentOutOfRangeException(nameof(action)),
    };

    private static RegistrationDto Valid() => new()
    {
        FirstName = "Ada",
        LastName = "Lovelace",
        Email = "ada@example.com",
        Street = "12 Analytical Row",
        City = "London",
        PostalCode = "N1 9GU",
        Country = "United Kingdom",
    };
}
