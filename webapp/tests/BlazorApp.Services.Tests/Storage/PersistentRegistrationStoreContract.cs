using System.Reflection;
using BlazorApp.Services.Storage;
using BlazorApp.StoreHelper;
using Microsoft.Extensions.Logging;

namespace BlazorApp.Services.Tests.Storage;

/// <summary>
/// The rest of the shared suite, for stores that keep data in a folder several servers share:
/// data survives restarting the store, separate store instances see each other's writes
/// (nothing stale is served from memory), and several separate processes saving at once lose
/// nothing.
/// </summary>
public abstract class PersistentRegistrationStoreContract : RegistrationStoreContract
{
    /// <summary>The strategy name, as used in <c>Storage:Strategy</c> and by the helper process.</summary>
    protected abstract string Strategy { get; }

    /// <summary>Assemblies the helper must search for the strategy besides BlazorApp.Services (test-only stores).</summary>
    protected virtual IEnumerable<Assembly> PluginAssemblies => [];

    /// <summary>Opens the store the same way the server and the helper do: by strategy name.</summary>
    /// <param name="folder">The data folder.</param>
    /// <param name="loggerFactory">Where the store logs.</param>
    /// <returns>An open store.</returns>
    protected override IRegistrationStore OpenStore(string folder, ILoggerFactory loggerFactory) =>
        StoreStrategies.Create(Strategy, folder, loggerFactory, [.. PluginAssemblies]);

    [Fact]
    public async Task Data_survives_restarting_the_store_on_the_same_folder()
    {
        var first = OpenStore(Folder.Path);
        var kept = Sample();
        var changed = Sample();
        var deleted = Sample();
        await first.TryCreateAsync(kept);
        await first.TryCreateAsync(changed);
        await first.TryCreateAsync(deleted);
        var second = Next(changed, "after restart?");
        await first.TryReplaceAsync(second, 1);
        await first.TryDeleteAsync(deleted.Id, 1);

        var restarted = OpenStore(Folder.Path);

        Assert.Equal(kept, await restarted.GetAsync(kept.Id), RecordComparer.Instance);
        Assert.Equal(second, await restarted.GetAsync(changed.Id), RecordComparer.Instance);
        Assert.Null(await restarted.GetAsync(deleted.Id));
        Assert.Equal(2, (await restarted.ListAsync()).Count);
    }

    [Fact]
    public async Task Two_store_instances_on_one_folder_see_each_others_writes()
    {
        var a = OpenStore(Folder.Path);
        var b = OpenStore(Folder.Path);
        var record = Sample();

        await a.TryCreateAsync(record);
        Assert.NotNull(await b.GetAsync(record.Id));

        Assert.Equal(WriteResult.Saved, await b.TryReplaceAsync(Next(record, "by b"), 1));
        Assert.Equal(2, (await a.GetAsync(record.Id))!.Version);

        // a's copy is now stale; the write must be refused, not overwrite b's change.
        Assert.Equal(WriteResult.Conflict, await a.TryReplaceAsync(Next(record, "by a, stale"), 1));
        Assert.Equal("by b", (await b.GetAsync(record.Id))!.History[^1].Comment);
    }

    /// <summary>Several separate processes run the workload on the same records at once; every acknowledged save must survive.</summary>
    [Fact]
    public async Task Several_processes_saving_at_once_lose_nothing()
    {
        var records = await SeedAsync(OpenStore(Folder.Path), 2);

        var runs = await MultiProcessRun.RunAsync(Strategy, Folder.Path, records, processes: 4, iterations: 40, PluginAssemblies);

        AssertRanConcurrently(runs);
        var report = await CheckAsync(
            OpenStore(Folder.Path), [.. runs.SelectMany(r => r.Acks)], runs.Sum(r => r.Conflicts), records, runs.Sum(r => r.BrokenReads));
        AssertLostUpdates(report);

        // No broken files: everything in the folder still loads. The control may leave them broken
        // or not; either way AssertLostUpdates has already seen it fail.
        if (!ExpectsLostUpdates)
        {
            Assert.Equal(2, (await OpenStore(Folder.Path).ListAsync()).Count);
        }
    }

    /// <summary>Checks the processes' writing periods overlapped, so they really ran at the same time.</summary>
    /// <param name="runs">The helper results.</param>
    protected static void AssertRanConcurrently(IReadOnlyList<HelperRun> runs)
    {
        ArgumentNullException.ThrowIfNull(runs);
        Assert.True(runs.Max(r => r.Begin) < runs.Min(r => r.End), "The helper processes did not overlap in time.");
    }
}
