using Bunit;
using BlazorApp.Client.Pages;
using BlazorApp.Shared.Contracts;
using BlazorApp.Shared.Dtos;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorApp.Client.Tests;

/// <summary>The tabbed Register page: client validation, server errors and success.</summary>
/// <remarks>Since phase 2 the form needs an acting user who may create; these tests act as a registrant.</remarks>
public sealed class RegisterTests : ClientTestContext
{
    private readonly FakeRegistrationService _service = new();

    /// <inheritdoc />
    protected override void ConfigureServices(BunitServiceProvider services) =>
        services.AddSingleton<IRegistrationService>(_service);

    [Fact]
    public void Empty_submit_selects_the_first_tab_and_sends_nothing()
    {
        var cut = RenderAsRegistrant();
        GoToTab(cut, "4. Review");

        cut.Find("form").Submit();

        Assert.Equal("1. Personal", ActiveTab(cut));
        Assert.Empty(_service.Submitted);
        Assert.NotEmpty(cut.FindAll(".validation-message"));
    }

    [Fact]
    public void Submit_jumps_to_the_first_tab_with_an_error()
    {
        var cut = RenderAsRegistrant();
        FillPersonal(cut);
        GoToTab(cut, "4. Review");

        cut.Find("form").Submit();

        Assert.Equal("2. Address", ActiveTab(cut));
        Assert.Empty(_service.Submitted);
    }

    [Fact]
    public void Server_errors_are_shown_inline_on_their_tab()
    {
        _service.NextResult = SubmissionResultDto.Failed(new Dictionary<string, string[]>
        {
            [nameof(RegistrationDto.Email)] = ["That address is already registered."],
        });
        var cut = RenderAsRegistrant();
        FillPersonal(cut);
        FillAddress(cut);
        GoToTab(cut, "4. Review");

        cut.Find("form").Submit();

        Assert.Single(_service.Submitted);
        Assert.Equal("1. Personal", ActiveTab(cut));
        var emailLabel = cut.Find("input[autocomplete=email]").ParentElement!;
        Assert.Equal(
            "That address is already registered.",
            emailLabel.QuerySelector(".validation-message")?.TextContent.Trim());
    }

    [Fact]
    public void Server_error_clears_when_the_field_is_edited()
    {
        _service.NextResult = SubmissionResultDto.Failed(new Dictionary<string, string[]>
        {
            [nameof(RegistrationDto.Email)] = ["That address is already registered."],
        });
        var cut = RenderAsRegistrant();
        FillPersonal(cut);
        FillAddress(cut);
        cut.Find("form").Submit();

        cut.Find("input[autocomplete=email]").Change("other@example.com");

        Assert.Empty(cut.FindAll(".validation-message"));
    }

    [Fact]
    public void Successful_submit_shows_a_confirmation()
    {
        var id = Guid.Parse("12345678-0000-0000-0000-000000000000");
        _service.NextResult = SubmissionResultDto.Ok(id);
        var cut = RenderAsRegistrant();
        FillPersonal(cut);
        FillAddress(cut);

        cut.Find("form").Submit();

        var sent = Assert.Single(_service.Submitted);
        Assert.Equal("Ada", sent.FirstName);
        Assert.Equal("London", sent.City);
        Assert.Contains("Thanks, Ada!", cut.Find(".alert.success").TextContent);
        Assert.Contains("12345678", cut.Find(".alert.success").TextContent);
    }

    private IRenderedComponent<Register> RenderAsRegistrant() =>
        Render<Register>(p => p.AddCascadingValue(new ActingUser(FakeCurrentUserService.Alice)));

    private static void FillPersonal(IRenderedComponent<Register> cut)
    {
        cut.Find("input[autocomplete=given-name]").Change("Ada");
        cut.Find("input[autocomplete=family-name]").Change("Lovelace");
        cut.Find("input[autocomplete=email]").Change("ada@example.com");
    }

    private static void FillAddress(IRenderedComponent<Register> cut)
    {
        cut.Find("input[autocomplete=street-address]").Change("12 Analytical Row");
        cut.Find("input[autocomplete=address-level2]").Change("London");
        cut.Find("input[autocomplete=postal-code]").Change("N1 9GU");
        cut.Find("input[autocomplete=country-name]").Change("United Kingdom");
    }

    private static void GoToTab(IRenderedComponent<Register> cut, string title) =>
        cut.FindAll("[role=tab]").Single(t => t.TextContent.Trim() == title).Click();

    private static string ActiveTab(IRenderedComponent<Register> cut) =>
        cut.Find("[role=tab][aria-selected=true]").TextContent.Trim();
}
