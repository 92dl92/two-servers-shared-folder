using System.Text.RegularExpressions;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace BlazorApp.E2E.Tests;

/// <summary>Main user journeys through the real app in a real browser.</summary>
/// <remarks>Since phase 2 registering needs an acting user, so the journeys pick Alice first.</remarks>
/// <param name="app">Shared server and browser.</param>
[Collection(AppCollection.Name)]
public sealed partial class RegistrationJourneyTests(AppFixture app)
{
    [Fact]
    public async Task Register_successfully_and_see_it_listed()
    {
        var page = await app.OpenAsync("register");
        await AppFixture.ActAsAsync(page, "Alice (Registrant)");
        var email = $"ada-{Guid.NewGuid():N}@example.com";

        await page.GetByLabel("First name", new() { Exact = true }).FillAsync("Ada");
        await page.GetByLabel("Last name", new() { Exact = true }).FillAsync("Lovelace");
        await page.GetByLabel("Email", new() { Exact = true }).FillAsync(email);
        await page.GetByRole(AriaRole.Button, new() { Name = "Next" }).ClickAsync();
        await page.GetByLabel("Street", new() { Exact = true }).FillAsync("12 Analytical Row");
        await page.GetByLabel("City", new() { Exact = true }).FillAsync("London");
        await page.GetByLabel("Postal code", new() { Exact = true }).FillAsync("N1 9GU");
        await page.GetByLabel("Country", new() { Exact = true }).FillAsync("United Kingdom");
        await page.GetByRole(AriaRole.Button, new() { Name = "Next" }).ClickAsync();
        await page.GetByRole(AriaRole.Button, new() { Name = "Next" }).ClickAsync();
        await Expect(page.GetByRole(AriaRole.Tab, new() { Name = "4. Review" }))
            .ToHaveAttributeAsync("aria-selected", "true");
        await page.GetByRole(AriaRole.Button, new() { Name = "Submit" }).ClickAsync();

        await Expect(page.GetByRole(AriaRole.Status)).ToContainTextAsync("Thanks, Ada!");

        await AppFixture.GotoAsync(page, "records");
        var row = page.GetByRole(AriaRole.Row).Filter(new() { HasText = email });
        await Expect(row).ToContainTextAsync("Ada Lovelace");
        await Expect(row).ToContainTextAsync("alice");
    }

    [Fact]
    public async Task Empty_submit_shows_validation_errors_on_the_first_tab()
    {
        var page = await app.OpenAsync("register");
        await AppFixture.ActAsAsync(page, "Alice (Registrant)");

        for (var i = 0; i < 3; i++)
        {
            await page.GetByRole(AriaRole.Button, new() { Name = "Next" }).ClickAsync();
        }

        await page.GetByRole(AriaRole.Button, new() { Name = "Submit" }).ClickAsync();

        await Expect(page.GetByRole(AriaRole.Tab, new() { Name = "1. Personal" }))
            .ToHaveAttributeAsync("aria-selected", "true");
        var personal = page.GetByRole(AriaRole.Tabpanel, new() { Name = "1. Personal" });
        await Expect(personal.Locator(".validation-message").First).ToBeVisibleAsync();
        await Expect(personal.Locator(".validation-message")).ToHaveCountAsync(3);
        await Expect(page).ToHaveURLAsync(RegisterUrl());
    }

    [Fact]
    public async Task Mobile_drawer_opens_and_closes_on_navigation()
    {
        var page = await app.OpenAsync("", new ViewportSize { Width = 390, Height = 844 });
        var tray = page.GetByRole(AriaRole.Navigation, new() { Name = "Main navigation" });
        var menu = page.GetByRole(AriaRole.Button, new() { Name = "Open navigation" });

        await Expect(menu).ToBeVisibleAsync();
        await Expect(tray).Not.ToBeInViewportAsync();

        await menu.ClickAsync();
        await Expect(tray).ToBeInViewportAsync();
        await Expect(menu).ToHaveAttributeAsync("aria-expanded", "true");

        await tray.GetByRole(AriaRole.Link, new() { Name = "Register" }).ClickAsync();

        await Expect(page).ToHaveURLAsync(RegisterUrl());
        await Expect(page.GetByRole(AriaRole.Heading, new() { Name = "Register", Level = 1 })).ToBeVisibleAsync();
        await Expect(tray).Not.ToBeInViewportAsync();
        await Expect(menu).ToHaveAttributeAsync("aria-expanded", "false");
    }

    [GeneratedRegex("/register$")]
    private static partial Regex RegisterUrl();
}
