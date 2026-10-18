using BlazorApp.Shared.Dtos;

namespace BlazorApp.Shared.Rules;

/// <summary>
/// Which roles may perform which actions. Used by the server to enforce permissions and by
/// the UI to hide buttons; only the server check counts.
/// </summary>
public static class Permissions
{
    /// <summary>The scope a single role grants for an action.</summary>
    /// <param name="role">The role.</param>
    /// <param name="action">The action.</param>
    /// <returns>Which records the role may perform the action on.</returns>
    public static PermissionScope For(Role role, RecordAction action) => (role, action) switch
    {
        (Role.Admin, _) => PermissionScope.Any,

        (Role.Registrant, RecordAction.Create) => PermissionScope.Any,
        (Role.Registrant, RecordAction.View or RecordAction.Update or RecordAction.Submit) => PermissionScope.Own,

        (Role.Reviewer, RecordAction.View or RecordAction.Approve or RecordAction.Reject or RecordAction.Comment) =>
            PermissionScope.Any,

        _ => PermissionScope.None,
    };

    /// <summary>The widest scope any of the user's roles grants for an action.</summary>
    /// <param name="user">The acting user.</param>
    /// <param name="action">The action.</param>
    /// <returns>Which records the user may perform the action on.</returns>
    public static PermissionScope For(UserDto user, RecordAction action)
    {
        ArgumentNullException.ThrowIfNull(user);
        return user.Roles.Select(role => For(role, action)).DefaultIfEmpty(PermissionScope.None).Max();
    }

    /// <summary>Whether the user may perform the action on at least some records.</summary>
    /// <param name="user">The acting user, or null when nobody is selected.</param>
    /// <param name="action">The action.</param>
    /// <returns>True if the user's scope for the action is not <see cref="PermissionScope.None"/>.</returns>
    public static bool Allows(UserDto? user, RecordAction action) =>
        user is not null && For(user, action) != PermissionScope.None;
}
