using BlazorApp.Shared.Dtos;

namespace BlazorApp.Shared.Rules;

/// <summary>
/// The full rule for acting on a record: who may (roles from <see cref="Permissions"/>, plus
/// ownership for <see cref="PermissionScope.Own"/>) and when (which statuses allow the action).
/// Used by the server to enforce and by the UI to decide which buttons to show.
/// </summary>
public static class RecordRules
{
    /// <summary>Whether the user may perform the action on a record owned by <paramref name="ownerId"/>, ignoring status.</summary>
    /// <param name="user">The acting user, or null.</param>
    /// <param name="action">The action.</param>
    /// <param name="ownerId">Who created the record; null for <see cref="RecordAction.Create"/>.</param>
    /// <returns><see cref="RuleOutcome.Allowed"/>, <see cref="RuleOutcome.NoUser"/> or <see cref="RuleOutcome.Forbidden"/>.</returns>
    public static RuleOutcome CheckPermission(UserDto? user, RecordAction action, string? ownerId)
    {
        if (user is null)
        {
            return RuleOutcome.NoUser;
        }

        return Permissions.For(user, action) switch
        {
            PermissionScope.Any => RuleOutcome.Allowed,
            PermissionScope.Own when action == RecordAction.Create || ownerId == user.Id => RuleOutcome.Allowed,
            _ => RuleOutcome.Forbidden,
        };
    }

    /// <summary>Whether a record with the given status allows the action, whoever does it.</summary>
    /// <param name="action">The action; <see cref="RecordAction.Create"/> has no record and is always allowed.</param>
    /// <param name="status">The record's current status.</param>
    /// <returns>True if the workflow allows the action in that status.</returns>
    public static bool StatusAllows(RecordAction action, RecordStatus status) => action switch
    {
        RecordAction.Update => status is RecordStatus.Draft or RecordStatus.Submitted,
        RecordAction.Submit => status is RecordStatus.Draft,
        RecordAction.Approve or RecordAction.Reject => status is RecordStatus.Submitted,
        _ => true, // View, Create, Comment and Delete work in any status.
    };

    /// <summary>Permission and status together, checked in that order.</summary>
    /// <param name="user">The acting user, or null.</param>
    /// <param name="action">The action.</param>
    /// <param name="status">The record's status; null for <see cref="RecordAction.Create"/>.</param>
    /// <param name="ownerId">Who created the record; null for <see cref="RecordAction.Create"/>.</param>
    /// <returns>The outcome.</returns>
    public static RuleOutcome Evaluate(UserDto? user, RecordAction action, RecordStatus? status, string? ownerId)
    {
        var permission = CheckPermission(user, action, ownerId);
        if (permission != RuleOutcome.Allowed)
        {
            return permission;
        }

        return status is { } current && !StatusAllows(action, current) ? RuleOutcome.WrongStatus : RuleOutcome.Allowed;
    }

    /// <summary>The status a record has after a successful action.</summary>
    /// <param name="action">The action that succeeded.</param>
    /// <param name="current">The status before it.</param>
    /// <returns>The new status; unchanged for actions that do not move the workflow.</returns>
    public static RecordStatus StatusAfter(RecordAction action, RecordStatus current) => action switch
    {
        RecordAction.Submit => RecordStatus.Submitted,
        RecordAction.Approve => RecordStatus.Approved,
        RecordAction.Reject => RecordStatus.Rejected,
        _ => current,
    };
}
