using BlazorApp.Shared.Contracts;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BlazorApp.Server.Tests;

/// <summary>Helpers for calling the API as a given "Acting as" user.</summary>
public static class ClientExtensions
{
    /// <summary>Creates a client that sends the "Acting as" cookie for <paramref name="userId"/>.</summary>
    /// <param name="factory">The app under test.</param>
    /// <param name="userId">User id to act as; null sends no cookie.</param>
    /// <returns>An HTTP client for the in-process server.</returns>
    public static HttpClient ClientAs(this WebApplicationFactory<Program> factory, string? userId)
    {
        var client = factory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = false });
        if (userId is not null)
        {
            client.DefaultRequestHeaders.Add("Cookie", $"{UserCookie.Name}={userId}");
        }

        return client;
    }
}
