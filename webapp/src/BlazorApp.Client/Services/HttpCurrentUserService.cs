using System.Net;
using System.Net.Http.Json;
using BlazorApp.Shared.Contracts;
using BlazorApp.Shared.Dtos;

namespace BlazorApp.Client.Services;

/// <summary>Browser-side <see cref="ICurrentUserService"/> that calls the Server API.</summary>
/// <remarks>The server keeps the choice in a cookie, which the browser sends with every request.</remarks>
/// <param name="http">HTTP client whose base address is the Server origin.</param>
public sealed class HttpCurrentUserService(HttpClient http) : ICurrentUserService
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<UserDto>> GetUsersAsync(CancellationToken cancellationToken = default) =>
        await http.GetFromJsonAsync<List<UserDto>>(ApiRoutes.Users, cancellationToken) ?? [];

    /// <inheritdoc />
    public async Task<UserDto?> GetCurrentUserAsync(CancellationToken cancellationToken = default)
    {
        using var response = await http.GetAsync(ApiRoutes.CurrentUser, cancellationToken);
        response.EnsureSuccessStatusCode();
        return response.StatusCode == HttpStatusCode.NoContent
            ? null
            : await response.Content.ReadFromJsonAsync<UserDto>(cancellationToken);
    }

    /// <inheritdoc />
    public async Task SetCurrentUserAsync(string? userId, CancellationToken cancellationToken = default)
    {
        using var response = await http.PutAsJsonAsync(ApiRoutes.CurrentUser, new SetCurrentUserDto(userId), cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
