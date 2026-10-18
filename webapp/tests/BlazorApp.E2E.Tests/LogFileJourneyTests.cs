using System.Text.Json;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace BlazorApp.E2E.Tests;

/// <summary>
/// A save made in the browser shows up in the server's log file: the button press and result
/// from the client and the HTTP line and record event from the server, under one correlation id
/// (plan section 8).
/// </summary>
/// <param name="app">Shared server and browser.</param>
[Collection(AppCollection.Name)]
public sealed class LogFileJourneyTests(AppFixture app)
{
    [Fact]
    public async Task A_save_in_the_browser_is_in_the_log_file_from_both_sides_with_one_correlation_id()
    {
        var id = await app.CreateRecordAsync("alice", city: "London");
        var page = await app.OpenAsAsync($"records/{id}", "Alice (Registrant)");

        await page.GetByRole(AriaRole.Button, new() { Name = "Edit" }).ClickAsync();
        await page.GetByLabel("City", new() { Exact = true }).FillAsync("Leeds");
        await page.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();
        await Expect(page.Locator(".record-version")).ToHaveTextAsync("2");

        // Client events arrive in batches (at least once a second), so wait for the result line.
        var record = id.ToString();
        var lines = await WaitForAsync(l =>
            Source(l) == "client" && Field(l, "Action") == "Save" && Field(l, "Result") == "Saved" && Field(l, "RecordId") == record);
        var correlation = lines.Single(l =>
            Source(l) == "client" && Field(l, "Action") == "Save" && Field(l, "Result") == "Saved" && Field(l, "RecordId") == record)
            .GetProperty("CorrelationId").GetString();
        Assert.False(string.IsNullOrEmpty(correlation));

        var story = lines.Where(l => l.GetProperty("CorrelationId").GetString() == correlation).ToList();
        Assert.Contains(story, l => Source(l) == "client" && Field(l, "Event") == "ButtonPressed" && Field(l, "Action") == "Save");
        Assert.Contains(story, l => Source(l) == "client" && Field(l, "Event") == "ResultShown" && Field(l, "Result") == "Saved");
        Assert.Contains(story, l => Source(l) == "server" && l.GetProperty("EventId").GetInt32() == 3001 && Field(l, "Method") == "PUT");
        var recordEvent = Assert.Single(story, l => l.GetProperty("EventId").GetInt32() == 1200);
        Assert.Equal("server", Source(recordEvent));
        Assert.Equal(record, Field(recordEvent, "RecordId"));
        Assert.Equal("2", Field(recordEvent, "NewVersion"));
        Assert.Equal($"{app.Server.BaseAddress.Port}-", recordEvent.GetProperty("ServerId").GetString()![..($"{app.Server.BaseAddress.Port}-".Length)]);

        // Ids, not values: the new city is nowhere in the file.
        Assert.DoesNotContain("Leeds", await ReadAllAsync(), StringComparison.Ordinal);
    }

    private static string? Source(JsonElement line) => line.GetProperty("Source").GetString();

    private static string? Field(JsonElement line, string name) =>
        line.GetProperty("Fields").TryGetProperty(name, out var value)
            ? value.ValueKind == JsonValueKind.String ? value.GetString() : value.ToString()
            : null;

    private async Task<List<JsonElement>> WaitForAsync(Func<JsonElement, bool> wanted)
    {
        var deadline = DateTime.UtcNow.AddSeconds(15);
        while (true)
        {
            var lines = (await ReadAllAsync())
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(l => JsonDocument.Parse(l).RootElement)
                .ToList();
            if (lines.Any(wanted))
            {
                return lines;
            }

            Assert.True(DateTime.UtcNow < deadline, $"The expected line never reached {app.LogFile}.");
            await Task.Delay(200);
        }
    }

    private async Task<string> ReadAllAsync()
    {
        if (!File.Exists(app.LogFile))
        {
            return string.Empty;
        }

        await using var stream = new FileStream(app.LogFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }
}
