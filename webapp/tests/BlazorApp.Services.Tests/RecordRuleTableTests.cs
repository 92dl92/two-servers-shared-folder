using BlazorApp.Shared.Dtos;
using BlazorApp.Shared.Rules;
using static BlazorApp.Shared.Dtos.RecordAction;

namespace BlazorApp.Services.Tests;

/// <summary>The complete role × action × status table, for the record's owner and for someone else.</summary>
public sealed class RecordRuleTableTests
{
    /// <summary>
    /// Written out by hand from the plan. Each string has one letter per status, in the order
    /// Draft, Submitted, Approved, Rejected: Y = allowed, F = forbidden (403), S = wrong status (422).
    /// The first string is for a record the user created, the second for someone else's.
    /// </summary>
    private static readonly Dictionary<(Role Role, RecordAction Action), (string Own, string Other)> Expected = new()
    {
        [(Role.Registrant, View)] = ("YYYY", "FFFF"),
        [(Role.Registrant, Update)] = ("YYSS", "FFFF"),
        [(Role.Registrant, Submit)] = ("YSSS", "FFFF"),
        [(Role.Registrant, Approve)] = ("FFFF", "FFFF"),
        [(Role.Registrant, Reject)] = ("FFFF", "FFFF"),
        [(Role.Registrant, Comment)] = ("FFFF", "FFFF"),
        [(Role.Registrant, Delete)] = ("FFFF", "FFFF"),

        [(Role.Reviewer, View)] = ("YYYY", "YYYY"),
        [(Role.Reviewer, Update)] = ("FFFF", "FFFF"),
        [(Role.Reviewer, Submit)] = ("FFFF", "FFFF"),
        [(Role.Reviewer, Approve)] = ("SYSS", "SYSS"),
        [(Role.Reviewer, Reject)] = ("SYSS", "SYSS"),
        [(Role.Reviewer, Comment)] = ("YYYY", "YYYY"),
        [(Role.Reviewer, Delete)] = ("FFFF", "FFFF"),

        [(Role.Admin, View)] = ("YYYY", "YYYY"),
        [(Role.Admin, Update)] = ("YYSS", "YYSS"),
        [(Role.Admin, Submit)] = ("YSSS", "YSSS"),
        [(Role.Admin, Approve)] = ("SYSS", "SYSS"),
        [(Role.Admin, Reject)] = ("SYSS", "SYSS"),
        [(Role.Admin, Comment)] = ("YYYY", "YYYY"),
        [(Role.Admin, Delete)] = ("YYYY", "YYYY"),
    };

    /// <summary>Create has no record or status: allowed per role only.</summary>
    private static readonly Dictionary<Role, RuleOutcome> ExpectedCreate = new()
    {
        [Role.Registrant] = RuleOutcome.Allowed,
        [Role.Reviewer] = RuleOutcome.Forbidden,
        [Role.Admin] = RuleOutcome.Allowed,
    };

    private static readonly RecordStatus[] StatusOrder =
        [RecordStatus.Draft, RecordStatus.Submitted, RecordStatus.Approved, RecordStatus.Rejected];

    /// <summary>Every role × action × status × ownership combination with its expected outcome.</summary>
    public static TheoryData<Role, RecordAction, RecordStatus, bool, RuleOutcome> Table()
    {
        var data = new TheoryData<Role, RecordAction, RecordStatus, bool, RuleOutcome>();
        foreach (var ((role, action), (own, other)) in Expected)
        {
            for (var i = 0; i < StatusOrder.Length; i++)
            {
                data.Add(role, action, StatusOrder[i], true, Outcome(own[i]));
                data.Add(role, action, StatusOrder[i], false, Outcome(other[i]));
            }
        }

        return data;
    }

    [Fact]
    public void Table_covers_every_role_action_and_status()
    {
        var expectedKeys =
            from role in Enum.GetValues<Role>()
            from action in Enum.GetValues<RecordAction>()
            where action != Create
            select (role, action);

        Assert.Equal(expectedKeys.Order(), Expected.Keys.Order());
        Assert.Equal(Enum.GetValues<RecordStatus>().Order(), StatusOrder.Order());
        Assert.Equal(Enum.GetValues<Role>().Order(), ExpectedCreate.Keys.Order());
    }

    [Theory]
    [MemberData(nameof(Table))]
    public void Outcome_matches_the_table(Role role, RecordAction action, RecordStatus status, bool owner, RuleOutcome expected)
    {
        var user = new UserDto("me", "Me", [role]);

        var outcome = RecordRules.Evaluate(user, action, status, owner ? "me" : "someone-else");

        Assert.Equal(expected, outcome);
    }

    [Theory]
    [MemberData(nameof(Table))]
    public void Nobody_selected_is_always_NoUser(Role role, RecordAction action, RecordStatus status, bool owner, RuleOutcome expected)
    {
        _ = (role, expected);

        Assert.Equal(RuleOutcome.NoUser, RecordRules.Evaluate(null, action, status, owner ? "me" : "someone-else"));
    }

    [Theory]
    [InlineData(Role.Registrant)]
    [InlineData(Role.Reviewer)]
    [InlineData(Role.Admin)]
    public void Create_depends_on_the_role_only(Role role)
    {
        var user = new UserDto("me", "Me", [role]);

        Assert.Equal(ExpectedCreate[role], RecordRules.Evaluate(user, Create, null, null));
    }

    /// <summary>Status after each action that succeeds; written out by hand.</summary>
    public static TheoryData<RecordAction, RecordStatus, RecordStatus> Transitions => new()
    {
        { Submit, RecordStatus.Draft, RecordStatus.Submitted },
        { Approve, RecordStatus.Submitted, RecordStatus.Approved },
        { Reject, RecordStatus.Submitted, RecordStatus.Rejected },
        { Update, RecordStatus.Draft, RecordStatus.Draft },
        { Update, RecordStatus.Submitted, RecordStatus.Submitted },
        { Comment, RecordStatus.Draft, RecordStatus.Draft },
        { Comment, RecordStatus.Submitted, RecordStatus.Submitted },
        { Comment, RecordStatus.Approved, RecordStatus.Approved },
        { Comment, RecordStatus.Rejected, RecordStatus.Rejected },
    };

    [Theory]
    [MemberData(nameof(Transitions))]
    public void Actions_move_the_workflow_as_expected(RecordAction action, RecordStatus before, RecordStatus after)
    {
        Assert.Equal(after, RecordRules.StatusAfter(action, before));
    }

    private static RuleOutcome Outcome(char letter) => letter switch
    {
        'Y' => RuleOutcome.Allowed,
        'F' => RuleOutcome.Forbidden,
        'S' => RuleOutcome.WrongStatus,
        _ => throw new ArgumentOutOfRangeException(nameof(letter)),
    };
}
