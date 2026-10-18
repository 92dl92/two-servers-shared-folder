using BlazorApp.Shared.Contracts;
using BlazorApp.Shared.Dtos;
using BlazorApp.Shared.Rules;

namespace BlazorApp.Services.Users;

/// <summary>Permission checks that throw <see cref="AccessDeniedException"/>.</summary>
public static class Access
{
    /// <summary>Returns the acting user and the scope they have for an action, or throws.</summary>
    /// <param name="current">The acting user for this request.</param>
    /// <param name="action">The action about to be performed.</param>
    /// <returns>The user and a scope that is never <see cref="PermissionScope.None"/>.</returns>
    /// <exception cref="AccessDeniedException">No known user, or the user's roles do not allow the action.</exception>
    public static (UserDto User, PermissionScope Scope) Require(ICurrentUser current, RecordAction action)
    {
        var user = RequireUser(current);
        var scope = Permissions.For(user, action);
        return scope == PermissionScope.None
            ? throw new AccessDeniedException(AccessDenial.Forbidden, $"{user.Name} may not {action.ToString().ToLowerInvariant()} records.")
            : (user, scope);
    }

    /// <summary>Returns the acting user, or throws when there is none.</summary>
    /// <param name="current">The acting user for this request.</param>
    /// <returns>The known acting user.</returns>
    /// <exception cref="AccessDeniedException">No user selected, or the id is unknown.</exception>
    public static UserDto RequireUser(ICurrentUser current)
    {
        ArgumentNullException.ThrowIfNull(current);
        return current.User ?? throw new AccessDeniedException(
            AccessDenial.NoUser,
            current.RequestedId is null ? "No user selected." : $"Unknown user '{current.RequestedId}'.");
    }
}
