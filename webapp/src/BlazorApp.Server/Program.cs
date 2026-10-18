using BlazorApp.Server.Components;
using BlazorApp.Server.Endpoints;
using BlazorApp.Server.Logging;
using BlazorApp.Server.Users;
using BlazorApp.Services;
using BlazorApp.Services.Storage;
using BlazorApp.Services.Users;
using BlazorApp.Shared.Contracts;

var builder = WebApplication.CreateBuilder(args);

// Logging (plan section 8): console as usual, plus one JSON-lines file per server in
// Logging:File:Directory (default logs/), named after the port. Servers without a configured
// port (in-memory test servers) and Logging:File:Enabled=false get no file.
var identity = ServerIdentity.From(builder.Configuration["urls"]);
string? logFile = null;
if (builder.Configuration.GetValue("Logging:File:Enabled", true) && identity.LogFileName is { } fileName)
{
    var directory = Path.Combine(builder.Environment.ContentRootPath, builder.Configuration["Logging:File:Directory"] ?? "logs");
    logFile = Path.Combine(directory, fileName);
    builder.Logging.AddProvider(new JsonLinesLoggerProvider(logFile, identity.ServerId));
}

builder.Services.AddSingleton(identity);

// Blazor: render components on the server (prerendering) and run them interactively in WebAssembly.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

// Business logic from BlazorApp.Services. Used by the API endpoints and by prerendered components.
// Storage is chosen in settings, e.g. --Storage:Strategy=PerRecordFile --Storage:Path=/shared/data.
builder.Services.AddAppServices(
    builder.Configuration["Storage:Strategy"] ?? StoreStrategies.Default,
    builder.Configuration["Storage:Path"]);

// Users from users.json (path configurable with --Users:Path), and the acting user from the cookie.
var usersPath = Path.Combine(builder.Environment.ContentRootPath, builder.Configuration["Users:Path"] ?? "users.json");
builder.Services.AddSingleton<IUserDirectory>(UserDirectory.LoadFromFile(usersPath));
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CookieCurrentUser>();
builder.Services.AddScoped<ICurrentUser>(sp => sp.GetRequiredService<CookieCurrentUser>());
builder.Services.AddScoped<ICurrentUserService>(sp => sp.GetRequiredService<CookieCurrentUser>());
builder.Services.AddSingleton<IClientEventLog, PrerenderClientEventLog>();

var app = builder.Build();

// Open the store now, so a data folder that can't be used stops the server at startup
// instead of failing every request later.
app.Services.GetRequiredService<IRegistrationStore>();

// Outermost (inside only the development exception page, which ASP.NET adds first), so
// everything logged while handling a request, the exception handler's lines included,
// carries its correlation id.
app.UseMiddleware<RequestLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();

app.MapRegistrationEndpoints();
app.MapUserEndpoints();
app.MapClientLogEndpoints();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorApp.Client._Imports).Assembly);

var startup = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("BlazorApp.Server.Startup");
app.Lifetime.ApplicationStarted.Register(() => startup.LogInformation(
    LogEvents.ServerStarted,
    "Server {ServerId} started: port {Port}, process {ProcessId}, storage {Strategy} in {StoragePath}, users from {UsersFile}, log file {LogFile}",
    identity.ServerId,
    identity.Port,
    identity.ProcessId,
    builder.Configuration["Storage:Strategy"] ?? StoreStrategies.Default,
    builder.Configuration["Storage:Path"],
    usersPath,
    logFile));

app.Run();

/// <summary>Entry point. Declared partial and public so tests can use <c>WebApplicationFactory&lt;Program&gt;</c>.</summary>
public partial class Program;
