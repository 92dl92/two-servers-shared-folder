using Bunit;
using BlazorApp.Client.Pages;
using BlazorApp.Shared.Contracts;
using BlazorApp.Shared.Dtos;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorApp.Client.Tests;

/// <summary>The record page: buttons per role and status, changes, and the conflict message.</summary>
public sealed class RecordDetailTests : ClientTestContext
{
    private readonly FakeRegistrationService _service = new();

    /// <inheritdoc />
    protected override void ConfigureServices(BunitServiceProvider services) =>
        services.AddSingleton<IRegistrationService>(_service);

    /// <summary>
    /// Buttons each role sees on a record alice created, by status; written out by hand.
    /// The registrant is alice herself.
    /// </summary>
    public static TheoryData<Role, RecordStatus, string[]> Buttons => new()
    {
        { Role.Registrant, RecordStatus.Draft, ["Edit", "Submit for review"] },
        { Role.Registrant, RecordStatus.Submitted, ["Edit"] },
        { Role.Registrant, RecordStatus.Approved, [] },
        { Role.Registrant, RecordStatus.Rejected, [] },
        { Role.Reviewer, RecordStatus.Draft, ["Add comment"] },
        { Role.Reviewer, RecordStatus.Submitted, ["Approve", "Reject", "Add comment"] },
        { Role.Reviewer, RecordStatus.Approved, ["Add comment"] },
        { Role.Reviewer, RecordStatus.Rejected, ["Add comment"] },
        { Role.Admin, RecordStatus.Draft, ["Edit", "Submit for review", "Delete", "Add comment"] },
        { Role.Admin, RecordStatus.Submitted, ["Edit", "Approve", "Reject", "Delete", "Add comment"] },
        { Role.Admin, RecordStatus.Approved, ["Delete", "Add comment"] },
        { Role.Admin, RecordStatus.Rejected, ["Delete", "Add comment"] },
    };

    [Theory]
    [MemberData(nameof(Buttons))]
    public void Buttons_follow_role_and_status(Role role, RecordStatus status, string[] expected)
    {
        var user = role == Role.Registrant ? FakeCurrentUserService.Alice : new UserDto(role.ToString(), role.ToString(), [role]);
        _service.Record = FakeRegistrationService.MakeRecord(status);

        var cut = RenderAs(user);

        Assert.Equal(expected, ButtonLabels(cut));
    }

    [Fact]
    public void Another_registrant_sees_no_buttons()
    {
        _service.Record = FakeRegistrationService.MakeRecord(RecordStatus.Draft, owner: "bob");

        var cut = RenderAs(FakeCurrentUserService.Alice);

        Assert.Empty(ButtonLabels(cut));
    }

    [Fact]
    public void Submit_sends_the_version_shown_and_shows_the_new_state()
    {
        _service.Record = FakeRegistrationService.MakeRecord(RecordStatus.Draft, version: 3);
        var cut = RenderAs(FakeCurrentUserService.Alice);

        Click(cut, "Submit for review");

        Assert.Equal(["Submit@3"], _service.Changes);
        Assert.Equal("Submitted", cut.Find(".record-status").TextContent);
        Assert.Equal("4", cut.Find(".record-version").TextContent);
        Assert.Empty(cut.FindAll("[role=alert]"));
    }

    [Fact]
    public void Edit_and_save_sends_the_changes_with_the_starting_version()
    {
        _service.Record = FakeRegistrationService.MakeRecord(RecordStatus.Draft, version: 5);
        var cut = RenderAs(FakeCurrentUserService.Alice);

        Click(cut, "Edit");
        cut.Find("input[autocomplete=address-level2]").Change("Paris");
        cut.Find("form").Submit();

        Assert.Equal(["Update@5"], _service.Changes);
        Assert.Equal("Paris", _service.Record!.Data.City);
        Assert.Contains("Paris", cut.Find(".record-address").TextContent);
        Assert.Empty(cut.FindAll("form"));
    }

    [Fact]
    public void Conflict_on_save_shows_the_message_and_reload_fetches_the_latest()
    {
        _service.Record = FakeRegistrationService.MakeRecord(RecordStatus.Draft, version: 2);
        var cut = RenderAs(FakeCurrentUserService.Alice);
        Click(cut, "Edit");
        cut.Find("input[autocomplete=address-level2]").Change("Paris");

        // Meanwhile someone else saved version 3.
        _service.Record = _service.Record with { Version = 3, Data = new RegistrationDto { FirstName = "Ada", LastName = "Lovelace", City = "Rome" } };
        _service.NextError = new VersionConflictException(_service.Record.Id, 2, 3);
        cut.Find("form").Submit();

        var alert = cut.Find("[role=alert]");
        Assert.Contains("Someone else changed this record", alert.TextContent);
        Assert.Single(cut.FindAll("form")); // the unsaved edit is still on screen

        var loadsBefore = _service.Loads;
        alert.QuerySelector("button")!.Click();

        Assert.Equal(loadsBefore + 1, _service.Loads);
        Assert.Empty(cut.FindAll("[role=alert]"));
        Assert.Empty(cut.FindAll("form"));
        Assert.Equal("3", cut.Find(".record-version").TextContent);
        Assert.Contains("Rome", cut.Find(".record-address").TextContent);
    }

    [Fact]
    public void Conflict_on_a_workflow_action_shows_the_same_message()
    {
        _service.Record = FakeRegistrationService.MakeRecord(RecordStatus.Submitted);
        _service.NextError = new VersionConflictException(_service.Record.Id, 3, 4);
        var cut = RenderAs(new UserDto("rita", "Rita", [Role.Reviewer]));

        Click(cut, "Approve");

        Assert.Contains("Someone else changed this record", cut.Find("[role=alert]").TextContent);
        Assert.Equal("Reload", cut.Find("[role=alert] button").TextContent.Trim());
    }

    [Fact]
    public void Other_refusals_show_the_server_message()
    {
        _service.Record = FakeRegistrationService.MakeRecord(RecordStatus.Submitted);
        _service.NextError = new WrongStatusException(RecordAction.Approve, null, "Can't approve a record that is approved.");
        var cut = RenderAs(new UserDto("rita", "Rita", [Role.Reviewer]));

        Click(cut, "Approve");

        Assert.Equal("Can't approve a record that is approved.", cut.Find("[role=alert]").TextContent.Trim());
    }

    [Fact]
    public void Missing_record_says_so()
    {
        var cut = Render<RecordDetail>(p => p
            .Add(x => x.Id, Guid.NewGuid())
            .AddCascadingValue(new ActingUser(FakeCurrentUserService.Ada)));

        Assert.Contains("does not exist", cut.Find("[role=status]").TextContent);
    }

    private IRenderedComponent<RecordDetail> RenderAs(UserDto user) =>
        Render<RecordDetail>(p => p
            .Add(x => x.Id, _service.Record!.Id)
            .AddCascadingValue(new ActingUser(user)));

    private static string[] ButtonLabels(IRenderedComponent<RecordDetail> cut) =>
        [.. cut.FindAll("button").Select(b => b.TextContent.Trim())];

    private static void Click(IRenderedComponent<RecordDetail> cut, string label) =>
        cut.FindAll("button").Single(b => b.TextContent.Trim() == label).Click();
}
