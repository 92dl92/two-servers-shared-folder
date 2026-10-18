using Bunit;
using BlazorApp.Client.Layout;
using BlazorApp.Client.Pages;
using BlazorApp.Shared.Contracts;
using BlazorApp.Shared.Dtos;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorApp.Client.Tests;

/// <summary>Pressing each button logs a client event with the action and the result shown (plan section 8).</summary>
public sealed class ClientEventTests : ClientTestContext
{
    private static readonly UserDto Rita = new("rita", "Rita", [Role.Reviewer]);

    private readonly FakeRegistrationService _service = new();
    private readonly FakeCurrentUserService _users = new() { Current = FakeCurrentUserService.Alice };

    /// <inheritdoc />
    protected override void ConfigureServices(BunitServiceProvider services)
    {
        services.AddSingleton<IRegistrationService>(_service);
        services.AddSingleton<ICurrentUserService>(_users);
    }

    /// <summary>
    /// Record page buttons, written out by hand: who presses, in which status, which button,
    /// what goes wrong (if anything), and the result that must be logged.
    /// </summary>
    public static TheoryData<string, RecordStatus, string, string, string> RecordButtons => new()
    {
        { "alice", RecordStatus.Draft, "Submit for review", "none", "Saved" },
        { "rita", RecordStatus.Submitted, "Approve", "none", "Saved" },
        { "rita", RecordStatus.Submitted, "Reject", "none", "Saved" },
        { "ada", RecordStatus.Approved, "Delete", "none", "Saved" },
        { "rita", RecordStatus.Draft, "Add comment", "none", "Saved" },
        { "alice", RecordStatus.Draft, "Edit", "none", "Editing" },
        { "rita", RecordStatus.Submitted, "Approve", "conflict", "Conflict" },
        { "rita", RecordStatus.Submitted, "Approve", "wrong-status", "WrongStatus" },
        { "rita", RecordStatus.Submitted, "Reject", "forbidden", "Forbidden" },
        { "alice", RecordStatus.Draft, "Submit for review", "not-found", "NotFound" },
        { "rita", RecordStatus.Draft, "Add comment", "invalid", "Invalid" },
    };

    [Theory]
    [MemberData(nameof(RecordButtons))]
    public void Record_page_buttons_log_the_press_and_the_result(
        string user, RecordStatus status, string button, string failure, string result)
    {
        _service.Record = FakeRegistrationService.MakeRecord(status);
        _service.Log = ClientLog;
        _service.NextError = failure switch
        {
            "conflict" => new VersionConflictException(_service.Record.Id, 3, 4),
            "wrong-status" => new WrongStatusException(RecordAction.Approve, null, "no"),
            "forbidden" => new AccessDeniedException(AccessDenial.Forbidden, "no"),
            "not-found" => new RecordNotFoundException(_service.Record.Id),
            "invalid" => new RecordValidationException(new Dictionary<string, string[]> { ["Text"] = ["no"] }),
            _ => null,
        };
        var cut = RenderRecord(user);
        if (button == "Add comment")
        {
            cut.Find("textarea").Input("Looks fine.");
        }

        Click(cut, button);

        var press = Assert.Single(ClientLog.Presses);
        var shown = Assert.Single(ClientLog.Results);
        Assert.Equal(("records/{id}", button, _service.Record?.Id ?? press.RecordId), (press.Page, press.Action, press.RecordId));
        Assert.Equal((button, result), (shown.Action, shown.Result));
        Assert.Equal(press.CorrelationId, shown.CorrelationId);
        Assert.NotNull(press.CorrelationId);

        // The service call happened while the action's correlation id was current, so it went out in the header.
        Assert.All(_service.CorrelationIds, id => Assert.Equal(press.CorrelationId, id));
        Assert.Null(ClientLog.CurrentCorrelationId);
    }

    [Fact]
    public void An_unexpected_failure_is_logged_as_an_error_not_as_saved()
    {
        _service.Record = FakeRegistrationService.MakeRecord(RecordStatus.Draft);
        _service.NextError = new HttpRequestException("500 Internal Server Error");
        var cut = RenderRecord("alice");

        Assert.ThrowsAny<Exception>(() => Click(cut, "Submit for review"));

        Assert.Equal(("Submit for review", "Error"), (ClientLog.Results.Single().Action, ClientLog.Results.Single().Result));
    }

    [Fact]
    public void Reload_that_is_refused_is_not_logged_as_reloaded()
    {
        _service.Record = FakeRegistrationService.MakeRecord(RecordStatus.Submitted);
        _service.NextError = new VersionConflictException(_service.Record.Id, 3, 4);
        var cut = RenderRecord("rita");
        Click(cut, "Approve");

        _service.GetError = new AccessDeniedException(AccessDenial.Forbidden, "no");
        cut.Find("[role=alert] button").Click();

        Assert.Equal(("Reload", "Forbidden"), (ClientLog.Results.Last().Action, ClientLog.Results.Last().Result));
    }

    [Theory]
    [InlineData("found", "Loaded")]
    [InlineData("missing", "NotFound")]
    [InlineData("forbidden", "Forbidden")]
    public void Record_page_load_is_logged_after_the_fetch_with_its_outcome(string kind, string result)
    {
        _service.Record = FakeRegistrationService.MakeRecord(RecordStatus.Draft);
        var id = kind == "missing" ? Guid.NewGuid() : _service.Record.Id;
        if (kind == "forbidden")
        {
            _service.GetError = new AccessDeniedException(AccessDenial.Forbidden, "no");
        }

        Render<RecordDetail>(p => p.Add(x => x.Id, id).AddCascadingValue(new ActingUser(FakeCurrentUserService.Alice)));

        var load = Assert.Single(ClientLog.Entries, e => e.Event == "PageLoaded");
        Assert.Equal(("records/{id}", id, result), (load.Page, load.RecordId, load.Result));
    }

    [Fact]
    public void Save_cancel_and_reload_are_logged()
    {
        _service.Record = FakeRegistrationService.MakeRecord(RecordStatus.Draft);
        var cut = RenderRecord("alice");

        Click(cut, "Edit");
        Click(cut, "Cancel");
        Click(cut, "Edit");
        cut.Find("form").Submit();
        Click(cut, "Edit");
        _service.NextError = new VersionConflictException(_service.Record.Id, 4, 5);
        cut.Find("form").Submit();
        cut.Find("[role=alert] button").Click();

        Assert.Equal(
            [("Edit", "Editing"), ("Cancel", "Cancelled"), ("Edit", "Editing"), ("Save", "Saved"), ("Edit", "Editing"), ("Save", "Conflict"), ("Reload", "Reloaded")],
            ClientLog.Results.Select(r => (r.Action!, r.Result!)));
        Assert.Equal(ClientLog.Presses.Select(p => p.Action), ClientLog.Results.Select(r => r.Action));
    }

    [Fact]
    public void Invalid_edit_is_logged_with_its_result()
    {
        _service.Record = FakeRegistrationService.MakeRecord(RecordStatus.Draft);
        var cut = RenderRecord("alice");
        Click(cut, "Edit");

        cut.Find("input[autocomplete=email]").Change("not-an-email");
        cut.Find("form").Submit();

        Assert.Equal(("Save", "Invalid"), (ClientLog.Results.Last().Action, ClientLog.Results.Last().Result));
        Assert.Empty(_service.Changes);
    }

    [Fact]
    public void Record_page_load_is_logged_once_with_the_record()
    {
        _service.Record = FakeRegistrationService.MakeRecord(RecordStatus.Draft);

        var cut = RenderRecord("alice");
        cut.Render();

        var load = Assert.Single(ClientLog.Entries, e => e.Event == "PageLoaded");
        Assert.Equal(("records/{id}", _service.Record.Id), (load.Page, load.RecordId));
    }

    [Fact]
    public void Prerendering_logs_no_page_load()
    {
        SetRendererInfo(new RendererInfo("Static", isInteractive: false));
        _service.Record = FakeRegistrationService.MakeRecord(RecordStatus.Draft);

        RenderRecord("alice");

        Assert.DoesNotContain(ClientLog.Entries, e => e.Event == "PageLoaded");
    }

    [Theory]
    [InlineData("valid", "Saved")]
    [InlineData("empty", "Invalid")]
    [InlineData("server-errors", "Invalid")]
    public void Register_submit_logs_the_result(string kind, string result)
    {
        _service.Log = ClientLog;
        if (kind == "server-errors")
        {
            _service.NextResult = SubmissionResultDto.Failed(new Dictionary<string, string[]> { ["Email"] = ["taken"] });
        }

        var cut = Render<Register>(p => p.AddCascadingValue(new ActingUser(FakeCurrentUserService.Alice)));
        if (kind != "empty")
        {
            cut.Find("input[autocomplete=given-name]").Change("Ada");
            cut.Find("input[autocomplete=family-name]").Change("Lovelace");
            cut.Find("input[autocomplete=email]").Change("ada@example.com");
            cut.Find("input[autocomplete=street-address]").Change("1 Row");
            cut.Find("input[autocomplete=address-level2]").Change("London");
            cut.Find("input[autocomplete=postal-code]").Change("N1");
            cut.Find("input[autocomplete=country-name]").Change("UK");
        }

        cut.Find("form").Submit();

        var press = Assert.Single(ClientLog.Presses);
        var shown = Assert.Single(ClientLog.Results);
        Assert.Equal(("register", "Submit"), (press.Page, press.Action));
        Assert.Equal(result, shown.Result);
        Assert.Equal(press.CorrelationId, shown.CorrelationId);
        Assert.All(_service.CorrelationIds, id => Assert.Equal(press.CorrelationId, id));
    }

    [Fact]
    public void Register_tab_buttons_are_logged()
    {
        var cut = Render<Register>(p => p.AddCascadingValue(new ActingUser(FakeCurrentUserService.Alice)));

        Click(cut, "Next");
        Click(cut, "Next");
        Click(cut, "Back");

        Assert.Equal(["Next", "Next", "Back"], ClientLog.Presses.Select(p => p.Action));
        Assert.Single(ClientLog.Entries, e => e.Event == "PageLoaded" && e.Page == "register");
    }

    [Fact]
    public void Changing_user_is_logged_and_flushed_before_the_reload()
    {
        var cut = Render<MainLayout>(p => p.Add(x => x.Body, (RenderFragment)(b => b.AddContent(0, "page"))));

        cut.Find(".acting-as select").Change("rita");

        var press = Assert.Single(ClientLog.Presses);
        Assert.Equal("ActingAs", press.Action);
        Assert.Equal("Changed", Assert.Single(ClientLog.Results).Result);
        Assert.Equal(1, ClientLog.Flushes);
    }

    [Fact]
    public void Records_list_load_is_logged()
    {
        Render<Records>();

        Assert.Single(ClientLog.Entries, e => e.Event == "PageLoaded" && e.Page == "records");
    }

    private IRenderedComponent<RecordDetail> RenderRecord(string user)
    {
        var acting = user switch
        {
            "alice" => FakeCurrentUserService.Alice,
            "rita" => Rita,
            _ => FakeCurrentUserService.Ada,
        };
        return Render<RecordDetail>(p => p.Add(x => x.Id, _service.Record!.Id).AddCascadingValue(new ActingUser(acting)));
    }

    private static void Click<T>(IRenderedComponent<T> cut, string label)
        where T : IComponent =>
        cut.FindAll("button").First(b => b.TextContent.Trim() == label).Click();
}
