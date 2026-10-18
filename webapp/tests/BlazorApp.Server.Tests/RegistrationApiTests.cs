using System.Net;
using System.Net.Http.Json;
using BlazorApp.Shared.Contracts;
using BlazorApp.Shared.Dtos;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BlazorApp.Server.Tests;

/// <summary>The registrations HTTP API, called the way the browser client calls it.</summary>
/// <remarks>
/// Each test gets its own factory, and so its own in-memory store. Since phase 2 every call
/// needs an acting user; these tests act as Ada (Admin), who may do everything.
/// </remarks>
public sealed class RegistrationApiTests : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory = new();

    /// <inheritdoc />
    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task Valid_registration_returns_201_and_appears_in_the_list()
    {
        var client = _factory.ClientAs("ada");

        using var response = await client.PostAsJsonAsync(ApiRoutes.Registrations, Valid());

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<SubmissionResultDto>();
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(response.Headers.Location);

        var list = await client.GetFromJsonAsync<List<RegistrationSummaryDto>>(ApiRoutes.Registrations);
        var item = Assert.Single(list!);
        Assert.Equal(result.Id, item.Id);
        Assert.Equal("Ada Lovelace", item.FullName);
    }

    [Fact]
    public async Task Invalid_registration_returns_400_with_errors_by_field()
    {
        var client = _factory.ClientAs("ada");
        var registration = Valid();
        registration.Email = "not-an-email";
        registration.City = "";

        using var response = await client.PostAsJsonAsync(ApiRoutes.Registrations, registration);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<SubmissionResultDto>();
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal(
            [nameof(RegistrationDto.City), nameof(RegistrationDto.Email)],
            result.Errors.Keys.Order());

        var list = await client.GetFromJsonAsync<List<RegistrationSummaryDto>>(ApiRoutes.Registrations);
        Assert.Empty(list!);
    }

    [Fact]
    public async Task List_starts_empty()
    {
        var client = _factory.ClientAs("ada");

        var list = await client.GetFromJsonAsync<List<RegistrationSummaryDto>>(ApiRoutes.Registrations);

        Assert.Empty(list!);
    }

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
