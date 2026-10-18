using Bunit;
using BlazorApp.Shared.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorApp.Client.Tests;

/// <summary>bUnit context that renders components as they run in the browser: interactive WebAssembly.</summary>
public abstract class ClientTestContext : BunitContext
{
    /// <summary>Registers services, then sets the renderer info that components read through <c>RendererInfo</c>.</summary>
    /// <remarks>
    /// Setting the renderer info builds the service provider, after which bUnit refuses new
    /// services, so <see cref="ConfigureServices"/> runs first. Derived-class field initializers
    /// have already run at this point, so overrides may use them.
    /// </remarks>
    protected ClientTestContext()
    {
        ConfigureServices(Services);
        Services.AddSingleton<IClientEventLog>(ClientLog);
        SetRendererInfo(new RendererInfo("WebAssembly", isInteractive: true));
    }

    /// <summary>Client events the components log (since phase 4L every page logs).</summary>
    protected FakeClientEventLog ClientLog { get; } = new();

    /// <summary>Adds the services the components under test need.</summary>
    /// <param name="services">The bUnit service collection.</param>
    protected virtual void ConfigureServices(BunitServiceProvider services)
    {
    }
}
