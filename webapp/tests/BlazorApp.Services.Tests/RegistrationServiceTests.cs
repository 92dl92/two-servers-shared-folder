using BlazorApp.Services.Storage.InMemory;
using BlazorApp.Shared.Dtos;
using Microsoft.Extensions.Time.Testing;

namespace BlazorApp.Services.Tests;

/// <summary>Validation and storage behaviour of <see cref="RegistrationService"/> over the in-memory store.</summary>
/// <remarks>Since phase 2 every call acts as a user; these tests act as a registrant.</remarks>
public sealed class RegistrationServiceTests
{
    private readonly FakeTimeProvider _clock = new(new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero));
    private readonly SwitchableCurrentUser _user = new(TestUsers.Alice);

    [Fact]
    public async Task Valid_registration_is_accepted_and_listed()
    {
        var service = new RegistrationService(_clock, _user, new InMemoryRegistrationStore());

        var result = await service.CreateAsync(ValidRegistration.Create());

        Assert.True(result.Success);
        Assert.NotNull(result.Id);
        Assert.Empty(result.Errors);

        var stored = Assert.Single(await service.GetAllAsync());
        Assert.Equal(result.Id, stored.Id);
        Assert.Equal("Ada Lovelace", stored.FullName);
        Assert.Equal("ada@example.com", stored.Email);
        Assert.Equal("United Kingdom", stored.Country);
        Assert.Equal(_clock.GetUtcNow(), stored.CreatedAt);
        Assert.Equal("alice", stored.CreatedBy);
    }

    /// <summary>Every way a single field can be invalid. Written out by hand, not derived from the DTO.</summary>
    public static TheoryData<string, Action<RegistrationDto>> InvalidFields => new()
    {
        // Required
        { nameof(RegistrationDto.FirstName), r => r.FirstName = "" },
        { nameof(RegistrationDto.LastName), r => r.LastName = "" },
        { nameof(RegistrationDto.Email), r => r.Email = "" },
        { nameof(RegistrationDto.Street), r => r.Street = "" },
        { nameof(RegistrationDto.City), r => r.City = "" },
        { nameof(RegistrationDto.PostalCode), r => r.PostalCode = "" },
        { nameof(RegistrationDto.Country), r => r.Country = "" },

        // Too long
        { nameof(RegistrationDto.FirstName), r => r.FirstName = new string('a', 51) },
        { nameof(RegistrationDto.LastName), r => r.LastName = new string('a', 51) },
        { nameof(RegistrationDto.Street), r => r.Street = new string('a', 101) },
        { nameof(RegistrationDto.City), r => r.City = new string('a', 61) },
        { nameof(RegistrationDto.PostalCode), r => r.PostalCode = new string('1', 13) },
        { nameof(RegistrationDto.Country), r => r.Country = new string('a', 61) },
        { nameof(RegistrationDto.Notes), r => r.Notes = new string('a', 501) },

        // Bad format
        { nameof(RegistrationDto.Email), r => r.Email = "not-an-email" },
        { nameof(RegistrationDto.Phone), r => r.Phone = "call me maybe" },
    };

    [Theory]
    [MemberData(nameof(InvalidFields))]
    public async Task Invalid_field_is_rejected_with_an_error_under_its_name(
        string field, Action<RegistrationDto> breakIt)
    {
        var service = new RegistrationService(_clock, _user, new InMemoryRegistrationStore());
        var registration = ValidRegistration.Create();
        breakIt(registration);

        var result = await service.CreateAsync(registration);

        Assert.False(result.Success);
        Assert.Null(result.Id);
        var (key, messages) = Assert.Single(result.Errors);
        Assert.Equal(field, key);
        Assert.NotEmpty(messages);
        Assert.Empty(await service.GetAllAsync());
    }

    [Fact]
    public async Task Values_at_the_maximum_length_are_accepted()
    {
        var service = new RegistrationService(_clock, _user, new InMemoryRegistrationStore());
        var registration = ValidRegistration.Create();
        registration.FirstName = new string('a', 50);
        registration.Street = new string('a', 100);
        registration.PostalCode = new string('1', 12);
        registration.Notes = new string('a', 500);

        var result = await service.CreateAsync(registration);

        Assert.True(result.Success);
    }

    [Fact]
    public async Task List_is_newest_first()
    {
        var service = new RegistrationService(_clock, _user, new InMemoryRegistrationStore());
        var first = await service.CreateAsync(ValidRegistration.Create());
        _clock.Advance(TimeSpan.FromMinutes(1));
        var second = await service.CreateAsync(ValidRegistration.Create());

        var all = await service.GetAllAsync();

        Assert.Equal([second.Id, first.Id], all.Select(r => (Guid?)r.Id));
    }
}
