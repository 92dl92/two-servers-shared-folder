using BlazorApp.Shared.Dtos;

namespace BlazorApp.Services.Tests;

/// <summary>Builds registrations that pass validation, for tests to start from.</summary>
public static class ValidRegistration
{
    /// <summary>Creates a fully valid registration.</summary>
    /// <returns>A new <see cref="RegistrationDto"/> with every required field filled in.</returns>
    public static RegistrationDto Create() => new()
    {
        FirstName = "Ada",
        LastName = "Lovelace",
        Email = "ada@example.com",
        Phone = "+44 20 7946 0000",
        Street = "12 Analytical Row",
        City = "London",
        PostalCode = "N1 9GU",
        Country = "United Kingdom",
        PreferredContact = ContactMethod.Email,
        SubscribeToNewsletter = true,
        Notes = "Prefers mornings.",
    };
}
