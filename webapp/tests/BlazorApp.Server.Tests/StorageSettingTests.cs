using BlazorApp.Services.Storage;
using BlazorApp.Services.Storage.InMemory;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorApp.Server.Tests;

/// <summary>The server opens the store named by <c>Storage:Strategy</c>.</summary>
public sealed class StorageSettingTests
{
    [Fact]
    public void Default_is_the_in_memory_store()
    {
        using var factory = new WebApplicationFactory<Program>();

        Assert.IsType<InMemoryRegistrationStore>(factory.Services.GetRequiredService<IRegistrationStore>());
    }

    [Fact]
    public void Strategy_setting_is_used()
    {
        using var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(b => b.UseSetting("Storage:Strategy", "inmemory"));

        Assert.IsType<InMemoryRegistrationStore>(factory.Services.GetRequiredService<IRegistrationStore>());
    }

    [Fact]
    public void Unknown_strategy_stops_the_server_from_starting()
    {
        using var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(b => b.UseSetting("Storage:Strategy", "NoSuchStrategy"));

        var e = Assert.ThrowsAny<Exception>(() => factory.Services);

        Assert.Contains("Unknown storage strategy 'NoSuchStrategy'", e.ToString());
    }
}
