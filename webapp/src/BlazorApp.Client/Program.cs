using BlazorApp.Client.Logging;
using BlazorApp.Client.Services;
using BlazorApp.Shared.Contracts;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Logging;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

var baseAddress = new Uri(builder.HostEnvironment.BaseAddress);

// Client events (plan section 8): to the browser console and, in batches, to the server's log.
// The log sends batches with its own plain HttpClient, so sending logs never logs itself.
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton(sp => new ClientEventLog(
    new HttpClient { BaseAddress = baseAddress },
    sp.GetRequiredService<ILogger<ClientEventLog>>(),
    sp.GetRequiredService<TimeProvider>()));
builder.Services.AddSingleton<IClientEventLog>(sp => sp.GetRequiredService<ClientEventLog>());

// In the browser, the contracts are fulfilled by calling the Server's HTTP API. Every call
// carries the correlation id of the user action that caused it.
builder.Services.AddScoped(sp => new HttpClient(new CorrelationHandler(sp.GetRequiredService<IClientEventLog>()) { InnerHandler = new HttpClientHandler() })
{
    BaseAddress = baseAddress,
});
builder.Services.AddScoped<IRegistrationService, HttpRegistrationService>();
builder.Services.AddScoped<ICurrentUserService, HttpCurrentUserService>();

await builder.Build().RunAsync();
