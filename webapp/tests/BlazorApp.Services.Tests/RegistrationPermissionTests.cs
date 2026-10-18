using BlazorApp.Services.Storage.InMemory;
using BlazorApp.Shared.Contracts;
using BlazorApp.Shared.Dtos;
using Microsoft.Extensions.Time.Testing;

namespace BlazorApp.Services.Tests;

/// <summary>The service enforces permissions itself, so in-process callers are checked too.</summary>
public sealed class RegistrationPermissionTests
{
    private readonly SwitchableCurrentUser _user = new(null);
    private readonly RegistrationService _service;

    /// <summary>Creates a service over an empty store.</summary>
    public RegistrationPermissionTests() =>
        _service = new RegistrationService(new FakeTimeProvider(), _user, new InMemoryRegistrationStore());

    [Fact]
    public async Task Nobody_selected_is_refused_with_NoUser()
    {
        var create = await Assert.ThrowsAsync<AccessDeniedException>(() => _service.CreateAsync(ValidRegistration.Create()));
        var list = await Assert.ThrowsAsync<AccessDeniedException>(() => _service.GetAllAsync());

        Assert.Equal(AccessDenial.NoUser, create.Reason);
        Assert.Equal(AccessDenial.NoUser, list.Reason);
    }

    [Fact]
    public async Task Unknown_user_is_refused_with_NoUser_and_named()
    {
        _user.RequestedIdOverride = "mallory";

        var e = await Assert.ThrowsAsync<AccessDeniedException>(() => _service.GetAllAsync());

        Assert.Equal(AccessDenial.NoUser, e.Reason);
        Assert.Contains("mallory", e.Message);
    }

    [Theory]
    [InlineData(Role.Registrant, true)]
    [InlineData(Role.Reviewer, false)]
    [InlineData(Role.Admin, true)]
    public async Task Create_is_allowed_only_for_roles_with_Create(Role role, bool allowed)
    {
        _user.User = TestUsers.ByRole[role];

        if (allowed)
        {
            Assert.True((await _service.CreateAsync(ValidRegistration.Create())).Success);
        }
        else
        {
            var e = await Assert.ThrowsAsync<AccessDeniedException>(() => _service.CreateAsync(ValidRegistration.Create()));
            Assert.Equal(AccessDenial.Forbidden, e.Reason);
        }
    }

    [Fact]
    public async Task Permission_is_checked_before_validation()
    {
        _user.User = TestUsers.Rita;

        var e = await Assert.ThrowsAsync<AccessDeniedException>(() => _service.CreateAsync(new RegistrationDto()));

        Assert.Equal(AccessDenial.Forbidden, e.Reason);
    }

    [Fact]
    public async Task Registrants_see_only_their_own_registrations_others_see_all()
    {
        _user.User = TestUsers.Alice;
        await _service.CreateAsync(ValidRegistration.Create());
        _user.User = TestUsers.Bob;
        await _service.CreateAsync(ValidRegistration.Create());
        _user.User = TestUsers.Ada;
        await _service.CreateAsync(ValidRegistration.Create());

        Assert.Equal(["alice"], await CreatorsVisibleTo(TestUsers.Alice));
        Assert.Equal(["bob"], await CreatorsVisibleTo(TestUsers.Bob));
        Assert.Equal(["ada", "alice", "bob"], await CreatorsVisibleTo(TestUsers.Rita));
        Assert.Equal(["ada", "alice", "bob"], await CreatorsVisibleTo(TestUsers.Ada));
    }

    private async Task<string[]> CreatorsVisibleTo(UserDto user)
    {
        _user.User = user;
        return [.. (await _service.GetAllAsync()).Select(r => r.CreatedBy).Order()];
    }
}
