using BlazorApp.Shared.Contracts;
using BlazorApp.Shared.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BlazorApp.Server.Endpoints;

/// <summary>HTTP API for the "Acting as" picker, consumed by the Client's <c>HttpCurrentUserService</c>.</summary>
public static class UserEndpoints
{
    /// <summary>Maps <c>GET</c> on <see cref="ApiRoutes.Users"/> and <c>GET</c>/<c>PUT</c> on <see cref="ApiRoutes.CurrentUser"/>.</summary>
    /// <param name="app">The endpoint route builder.</param>
    /// <returns>The same builder, for chaining.</returns>
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiRoutes.Users, GetUsersAsync).WithTags("Users");
        app.MapGet(ApiRoutes.CurrentUser, GetCurrentAsync).WithTags("Users");
        app.MapPut(ApiRoutes.CurrentUser, SetCurrentAsync).WithTags("Users");
        return app;
    }

    /// <summary>Lists every user that can be picked.</summary>
    /// <param name="users">The current-user service.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <returns>200 with the users.</returns>
    private static async Task<Ok<IReadOnlyList<UserDto>>> GetUsersAsync(
        ICurrentUserService users,
        CancellationToken cancellationToken) =>
        TypedResults.Ok(await users.GetUsersAsync(cancellationToken));

    /// <summary>Gets the acting user.</summary>
    /// <param name="users">The current-user service.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <returns>200 with the user, or 204 when none (or an unknown one) is selected.</returns>
    private static async Task<Results<Ok<UserDto>, NoContent>> GetCurrentAsync(
        ICurrentUserService users,
        CancellationToken cancellationToken) =>
        await users.GetCurrentUserAsync(cancellationToken) is { } user
            ? TypedResults.Ok(user)
            : TypedResults.NoContent();

    /// <summary>Selects the acting user by setting the cookie.</summary>
    /// <param name="request">The id to select; null selects nobody.</param>
    /// <param name="users">The current-user service.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <returns>204 on success; 400 for an unknown id.</returns>
    private static async Task<Results<NoContent, ProblemHttpResult>> SetCurrentAsync(
        SetCurrentUserDto request,
        ICurrentUserService users,
        CancellationToken cancellationToken)
    {
        try
        {
            await users.SetCurrentUserAsync(request.UserId, cancellationToken);
            return TypedResults.NoContent();
        }
        catch (ArgumentException e)
        {
            return TypedResults.Problem(e.Message, statusCode: StatusCodes.Status400BadRequest);
        }
    }
}
