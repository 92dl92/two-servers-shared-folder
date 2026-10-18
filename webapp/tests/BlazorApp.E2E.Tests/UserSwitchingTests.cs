using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace BlazorApp.E2E.Tests;

/// <summary>"Acting as": switching user changes what the UI offers.</summary>
/// <param name="app">Shared server and browser.</param>
[Collection(AppCollection.Name)]
public sealed class UserSwitchingTests(AppFixture app)
{
    [Fact]
    public async Task Switching_user_changes_which_buttons_are_shown()
    {
        var page = await app.OpenAsync("register");
        var next = page.GetByRole(AriaRole.Button, new() { Name = "Next" });
        var status = page.GetByRole(AriaRole.Status);

        // Nobody selected: a prompt, no form.
        await Expect(status).ToContainTextAsync("Acting as");
        await Expect(next).ToHaveCountAsync(0);

        await AppFixture.ActAsAsync(page, "Alice (Registrant)");
        await Expect(next).ToBeVisibleAsync();

        await AppFixture.ActAsAsync(page, "Rita (Reviewer)");
        await Expect(next).ToHaveCountAsync(0);
        await Expect(status).ToContainTextAsync("Rita (Reviewer) can't create registrations");

        await AppFixture.ActAsAsync(page, "Ada (Admin)");
        await Expect(next).ToBeVisibleAsync();
    }

    [Fact]
    public async Task The_choice_survives_a_reload_and_reaches_the_server()
    {
        var page = await app.OpenAsync("records");
        await Expect(page.GetByRole(AriaRole.Status)).ToContainTextAsync("Pick a user");

        await AppFixture.ActAsAsync(page, "Rita (Reviewer)");
        await AppFixture.GotoAsync(page, "records");

        await Expect(page.GetByLabel("Acting as").Locator("option:checked")).ToHaveTextAsync("Rita (Reviewer)");
        await Expect(page.GetByRole(AriaRole.Status)).ToHaveCountAsync(0);
        await Expect(page.GetByRole(AriaRole.Heading, new() { Name = "Records" })).ToBeVisibleAsync();
    }
}
