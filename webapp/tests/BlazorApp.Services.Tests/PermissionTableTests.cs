using BlazorApp.Shared.Dtos;
using BlazorApp.Shared.Rules;
using static BlazorApp.Shared.Dtos.RecordAction;
using static BlazorApp.Shared.Rules.PermissionScope;

namespace BlazorApp.Services.Tests;

/// <summary>The complete role × action permission table (plan section 1).</summary>
public sealed class PermissionTableTests
{
    /// <summary>
    /// Expected scope for every role and action, written out by hand from the plan. Columns are
    /// View, Create, Update, Submit, Approve, Reject, Comment, Delete.
    /// </summary>
    private static readonly Dictionary<Role, PermissionScope[]> Expected = new()
    {
        [Role.Registrant] = [Own, Any, Own, Own, None, None, None, None],
        [Role.Reviewer] = [Any, None, None, None, Any, Any, Any, None],
        [Role.Admin] = [Any, Any, Any, Any, Any, Any, Any, Any],
    };

    private static readonly RecordAction[] Columns = [View, Create, Update, Submit, Approve, Reject, Comment, Delete];

    /// <summary>Every role × action pair with its expected scope.</summary>
    public static TheoryData<Role, RecordAction, PermissionScope> Table()
    {
        var data = new TheoryData<Role, RecordAction, PermissionScope>();
        foreach (var (role, scopes) in Expected)
        {
            for (var i = 0; i < Columns.Length; i++)
            {
                data.Add(role, Columns[i], scopes[i]);
            }
        }

        return data;
    }

    [Fact]
    public void Table_covers_every_role_and_action()
    {
        Assert.Equal(Enum.GetValues<Role>().Order(), Expected.Keys.Order());
        Assert.Equal(Enum.GetValues<RecordAction>().Order(), Columns.Order());
    }

    [Theory]
    [MemberData(nameof(Table))]
    public void Role_grants_the_expected_scope(Role role, RecordAction action, PermissionScope expected)
    {
        Assert.Equal(expected, Permissions.For(role, action));

        var user = new UserDto("u", "U", [role]);
        Assert.Equal(expected, Permissions.For(user, action));
        Assert.Equal(expected != None, Permissions.Allows(user, action));
    }

    [Theory]
    [MemberData(nameof(Table))]
    public void Nobody_selected_is_never_allowed(Role role, RecordAction action, PermissionScope expected)
    {
        _ = (role, expected);
        Assert.False(Permissions.Allows(null, action));
    }

    [Fact]
    public void Several_roles_get_the_widest_scope_of_any_of_them()
    {
        var both = new UserDto("u", "U", [Role.Registrant, Role.Reviewer]);

        Assert.Equal(Any, Permissions.For(both, View));
        Assert.Equal(Any, Permissions.For(both, Create));
        Assert.Equal(Own, Permissions.For(both, Update));
        Assert.Equal(Any, Permissions.For(both, Approve));
        Assert.Equal(None, Permissions.For(both, Delete));
    }

    [Fact]
    public void A_user_with_no_roles_can_do_nothing()
    {
        var nobody = new UserDto("u", "U", []);

        Assert.All(Enum.GetValues<RecordAction>(), action => Assert.Equal(None, Permissions.For(nobody, action)));
    }
}
