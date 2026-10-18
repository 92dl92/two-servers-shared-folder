using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace BlazorApp.E2E.Tests;

/// <summary>Records in a real browser: the workflow, and two people editing the same record.</summary>
/// <param name="app">Shared server and browser.</param>
[Collection(AppCollection.Name)]
public sealed class RecordJourneyTests(AppFixture app)
{
    [Fact]
    public async Task Two_windows_edit_the_same_record_and_the_second_save_shows_the_conflict()
    {
        var id = await app.CreateRecordAsync("alice", city: "London");
        var first = await app.OpenAsAsync($"records/{id}", "Alice (Registrant)");
        var second = await app.OpenAsAsync($"records/{id}", "Ada (Admin)");

        // Both start editing version 1.
        await first.GetByRole(AriaRole.Button, new() { Name = "Edit" }).ClickAsync();
        await second.GetByRole(AriaRole.Button, new() { Name = "Edit" }).ClickAsync();
        await first.GetByLabel("City", new() { Exact = true }).FillAsync("Paris");
        await second.GetByLabel("City", new() { Exact = true }).FillAsync("Rome");

        await first.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();
        await Expect(first.Locator(".record-address")).ToContainTextAsync("Paris");
        await Expect(first.Locator(".record-version")).ToHaveTextAsync("2");

        await second.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();
        var alert = second.GetByRole(AriaRole.Alert);
        await Expect(alert).ToContainTextAsync("Someone else changed this record");

        await alert.GetByRole(AriaRole.Button, new() { Name = "Reload" }).ClickAsync();
        await Expect(alert).ToHaveCountAsync(0);
        await Expect(second.Locator(".record-address")).ToContainTextAsync("Paris");
        await Expect(second.Locator(".record-version")).ToHaveTextAsync("2");
    }

    [Fact]
    public async Task Registrant_submits_and_reviewer_approves()
    {
        var id = await app.CreateRecordAsync("alice");
        var alice = await app.OpenAsAsync($"records/{id}", "Alice (Registrant)");

        await alice.GetByRole(AriaRole.Button, new() { Name = "Submit for review" }).ClickAsync();
        await Expect(alice.Locator(".record-status")).ToHaveTextAsync("Submitted");
        await Expect(alice.GetByRole(AriaRole.Button, new() { Name = "Submit for review" })).ToHaveCountAsync(0);

        var rita = await app.OpenAsAsync($"records/{id}", "Rita (Reviewer)");
        await Expect(rita.GetByRole(AriaRole.Button, new() { Name = "Edit" })).ToHaveCountAsync(0);
        await rita.GetByLabel("Comment").FillAsync("All in order.");
        await rita.GetByRole(AriaRole.Button, new() { Name = "Add comment" }).ClickAsync();
        await Expect(rita.Locator("table.history")).ToContainTextAsync("All in order.");
        await rita.GetByRole(AriaRole.Button, new() { Name = "Approve" }).ClickAsync();

        await Expect(rita.Locator(".record-status")).ToHaveTextAsync("Approved");
        await Expect(rita.Locator(".record-version")).ToHaveTextAsync("4");
        await Expect(rita.GetByRole(AriaRole.Button, new() { Name = "Approve" })).ToHaveCountAsync(0);
    }
}
