using BlazorApp.Server.Logging;
using BlazorApp.Services.Users;
using BlazorApp.Shared.Contracts;
using BlazorApp.Shared.Dtos;

namespace BlazorApp.Server.Users;

/// <summary>
/// The acting user for one request, read from the "Acting as" cookie. Used by the Services
/// layer (<see cref="ICurrentUser"/>), by the API, and by components while they prerender
/// (<see cref="ICurrentUserService"/>).
/// </summary>
/// <param name="httpContextAccessor">Access to the current request.</param>
/// <param name="directory">The known users.</param>
/// <param name="logger">Logs "Acting as" changes and unknown user ids.</param>
public sealed class CookieCurrentUser(
    IHttpContextAccessor httpContextAccessor, IUserDirectory directory, ILogger<CookieCurrentUser> logger)
    : ICurrentUser, ICurrentUserService
{
    private static readonly Action<ILogger, string?, Exception?> Changed =
        LoggerMessage.Define<string?>(LogLevel.Information, LogEvents.ActingAsChanged, "Acting as changed to {UserId}");

    private static readonly Action<ILogger, string, Exception?> Unknown =
        LoggerMessage.Define<string>(LogLevel.Warning, LogEvents.UnknownUser, "Unknown user id {RequestedId} was sent");

    private bool _unknownLogged;

    /// <inheritdoc />
    public string? RequestedId => httpContextAccessor.HttpContext?.Request.Cookies[UserCookie.Name];

    /// <inheritdoc />
    /// <remarks>An unknown id in the cookie is logged once per request.</remarks>
    public UserDto? User
    {
        get
        {
            var requested = RequestedId;
            var user = directory.Find(requested);
            if (user is null && requested is not null && !_unknownLogged)
            {
                _unknownLogged = true;
                Unknown(logger, requested, null);
            }

            return user;
        }
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<UserDto>> GetUsersAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(directory.All);

    /// <inheritdoc />
    public Task<UserDto?> GetCurrentUserAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(User);

    /// <inheritdoc />
    /// <exception cref="ArgumentException"><paramref name="userId"/> is not a known user.</exception>
    public Task SetCurrentUserAsync(string? userId, CancellationToken cancellationToken = default)
    {
        var response = httpContextAccessor.HttpContext?.Response
            ?? throw new InvalidOperationException("The acting user can only be changed during an HTTP request.");

        if (userId is null)
        {
            response.Cookies.Delete(UserCookie.Name);
            Changed(logger, null, null);
            return Task.CompletedTask;
        }

        if (directory.Find(userId) is null)
        {
            Unknown(logger, userId, null);
            throw new ArgumentException($"Unknown user '{userId}'.", nameof(userId));
        }

        response.Cookies.Append(UserCookie.Name, userId, new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Lax,
            IsEssential = true,
            MaxAge = TimeSpan.FromDays(30),
        });
        Changed(logger, userId, null);
        return Task.CompletedTask;
    }
}
