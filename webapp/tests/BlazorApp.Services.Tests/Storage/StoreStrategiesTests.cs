using BlazorApp.Services.Storage;
using BlazorApp.Services.Storage.InMemory;

namespace BlazorApp.Services.Tests.Storage;

/// <summary>Choosing a store by name, as <c>--Storage:Strategy</c> does.</summary>
public sealed class StoreStrategiesTests
{
    [Fact]
    public void InMemory_is_built_in_and_the_default()
    {
        Assert.IsType<InMemoryRegistrationStore>(StoreStrategies.Create(StoreStrategies.Default, null));
        Assert.Contains("InMemory", StoreStrategies.Discover().Keys);
    }

    [Fact]
    public void Names_are_case_insensitive()
    {
        Assert.IsType<InMemoryRegistrationStore>(StoreStrategies.Create("inmemory", null));
    }

    [Fact]
    public void Unknown_name_lists_the_known_ones()
    {
        var e = Assert.Throws<ArgumentException>(() => StoreStrategies.Create("Nope", null));

        Assert.Contains("Unknown storage strategy 'Nope'", e.Message);
        Assert.Contains("InMemory", e.Message);
    }

    [Fact]
    public void Extra_assemblies_add_strategies()
    {
        var all = StoreStrategies.Discover(typeof(LockedFolderTestStore).Assembly);

        Assert.Contains("TestLockedFolder", all.Keys);
        Assert.Contains("TestLossy", all.Keys);
        Assert.DoesNotContain("TestLockedFolder", StoreStrategies.Discover().Keys);
    }

    [Fact]
    public void A_folder_strategy_needs_a_path()
    {
        var e = Assert.Throws<ArgumentException>(() =>
            StoreStrategies.Create("TestLockedFolder", " ", typeof(LockedFolderTestStore).Assembly));

        Assert.Contains("needs a folder", e.Message);
    }
}
