using System.Net.Http.Json;
using BlazorApp.Shared.Contracts;
using BlazorApp.Shared.Dtos;
using Microsoft.Playwright;

namespace BlazorApp.E2E.Tests;

/// <summary>One running server and one headless Chromium, shared by every browser test.</summary>
public sealed class AppFixture : IAsyncLifetime
{
    private IPlaywright? _playwright;

    /// <summary>The server under test.</summary>
    public ServerProcess Server { get; private set; } = default!;

    /// <summary>Where the server writes its JSON-lines log.</summary>
    public string LogDirectory { get; } = Path.Combine(Path.GetTempPath(), $"e2e-logs-{Guid.NewGuid():N}");

    /// <summary>The server's log file, named after its port.</summary>
    public string LogFile => Path.Combine(LogDirectory, $"server-{Server.BaseAddress.Port}.jsonl");

    /// <summary>The browser that tests open pages in.</summary>
    public IBrowser Browser { get; private set; } = default!;

    /// <inheritdoc />
    public async Task InitializeAsync()
    {
        // Logs go to a temp folder so the tests can read the server's file (plan section 8).
        Directory.CreateDirectory(LogDirectory);
        Server = await ServerProcess.StartAsync("--Logging:File:Directory", LogDirectory);
        _playwright = await Playwright.CreateAsync();
        Browser = await _playwright.Chromium.LaunchAsync(new() { Headless = true });
    }

    /// <inheritdoc />
    public async Task DisposeAsync()
    {
        if (Browser is not null)
        {
            await Browser.DisposeAsync();
        }

        _playwright?.Dispose();
        if (Server is not null)
        {
            await Server.DisposeAsync();
        }

        if (Directory.Exists(LogDirectory))
        {
            Directory.Delete(LogDirectory, recursive: true);
        }
    }

    /// <summary>Creates a draft record through the API, as <paramref name="userId"/>.</summary>
    /// <param name="userId">Who creates it.</param>
    /// <param name="city">City to put in the form data.</param>
    /// <returns>The new record's id.</returns>
    public async Task<Guid> CreateRecordAsync(string userId, string city = "London")
    {
        using var http = new HttpClient { BaseAddress = Server.BaseAddress };
        http.DefaultRequestHeaders.Add("Cookie", $"{UserCookie.Name}={userId}");
        using var response = await http.PostAsJsonAsync(ApiRoutes.Registrations, new RegistrationDto
        {
            FirstName = "Grace",
            LastName = "Hopper",
            Email = "grace@example.com",
            Street = "1 Compiler Way",
            City = city,
            PostalCode = "10001",
            Country = "USA",
        });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<SubmissionResultDto>())!.Id!.Value;
    }

    /// <summary>Opens a page in a fresh browser context, acting as <paramref name="userLabel"/>.</summary>
    /// <param name="path">Path relative to the server root.</param>
    /// <param name="userLabel">"Acting as" option text, e.g. <c>Alice (Registrant)</c>.</param>
    /// <returns>The loaded page, with that user selected.</returns>
    public async Task<IPage> OpenAsAsync(string path, string userLabel)
    {
        var page = await OpenAsync(path);
        await ActAsAsync(page, userLabel);
        return page;
    }

    /// <summary>Opens a page in a fresh browser context (own cookies) and waits until it is interactive.</summary>
    /// <param name="path">Path relative to the server root, e.g. <c>register</c>.</param>
    /// <param name="viewport">Optional viewport size; defaults to a desktop window.</param>
    /// <returns>The loaded page.</returns>
    public async Task<IPage> OpenAsync(string path, ViewportSize? viewport = null)
    {
        var context = await Browser.NewContextAsync(new()
        {
            BaseURL = Server.BaseAddress.ToString(),
            ViewportSize = viewport ?? new ViewportSize { Width = 1280, Height = 800 },
        });
        var page = await context.NewPageAsync();
        await GotoAsync(page, path);
        return page;
    }

    /// <summary>Picks a user in "Acting as" and waits for the page to reload with them.</summary>
    /// <param name="page">An interactive page.</param>
    /// <param name="optionLabel">The option text, e.g. <c>Alice (Registrant)</c>.</param>
    /// <returns>A task that completes when the reloaded page is interactive.</returns>
    public static async Task ActAsAsync(IPage page, string optionLabel)
    {
        var picker = page.GetByLabel("Acting as");

        // The app reloads the page after a change; a flag on window tells the old page from the new one.
        await page.EvaluateAsync("() => window.__beforeUserSwitch = true");
        await picker.SelectOptionAsync(new SelectOptionValue { Label = optionLabel });
        await page.WaitForFunctionAsync(
            "() => !window.__beforeUserSwitch && document.querySelector('.shell[data-interactive=true]') !== null",
            null,
            new() { Timeout = 30_000 });
        await Assertions.Expect(picker.Locator("option:checked")).ToHaveTextAsync(optionLabel);
    }

    /// <summary>Navigates and waits for WebAssembly to take over from the prerendered HTML.</summary>
    /// <param name="page">The page to navigate.</param>
    /// <param name="path">Path relative to the server root.</param>
    /// <returns>A task that completes when the page is interactive.</returns>
    public static async Task GotoAsync(IPage page, string path)
    {
        await page.GotoAsync(path);
        await Assertions.Expect(page.Locator(".shell[data-interactive=true]"))
            .ToBeAttachedAsync(new() { Timeout = 30_000 });
    }
}

/// <summary>Groups the browser tests so they share one <see cref="AppFixture"/>.</summary>
[CollectionDefinition(Name)]
public sealed class AppCollection : ICollectionFixture<AppFixture>
{
    /// <summary>Collection name used by <see cref="CollectionAttribute"/>.</summary>
    public const string Name = "App";
}
