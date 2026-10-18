using Microsoft.Extensions.Logging.Abstractions;
using BlazorApp.Services.Storage;
using BlazorApp.Shared.Dtos;
using BlazorApp.StoreHelper;

namespace BlazorApp.Services.Tests.Storage;

/// <summary>
/// Known-answer checks that the multi-process helper and the lost-update check work: they must
/// report a store that silently drops saves, and fail loudly on bad input.
/// </summary>
public sealed class MultiProcessHelperTests : IDisposable
{
    private readonly TempFolder _folder = new();

    /// <inheritdoc />
    public void Dispose() => _folder.Dispose();

    [Fact]
    public async Task Lost_updates_from_separate_processes_are_detected()
    {
        var plugins = new[] { typeof(LossyTestStore).Assembly };
        var seedStore = StoreStrategies.Create("TestLockedFolder", _folder.Path, plugins);
        var records = new[] { Guid.NewGuid(), Guid.NewGuid() };
        foreach (var id in records)
        {
            await seedStore.TryCreateAsync(StoreWorkload.Seed(id));
        }

        var runs = await MultiProcessRun.RunAsync("TestLossy", _folder.Path, records, processes: 3, iterations: 20, plugins);

        var acks = runs.SelectMany(r => r.Acks).ToList();
        var finals = new Dictionary<Guid, RegistrationRecordDto?>();
        foreach (var id in records)
        {
            finals[id] = await seedStore.GetAsync(id);
        }

        var report = WorkloadReport.Check(acks, runs.Sum(r => r.Conflicts), records, finals, runs.Sum(r => r.BrokenReads));

        // Each process fakes every fifth replace: 4 of its 20 acks. Those markers were never written.
        Assert.Equal(3 * 20, report.Acks);
        Assert.Equal(3 * 4, report.Lost.Count);
        Assert.False(report.NothingLost);
        Assert.Equal(3, runs.Select(r => r.Writer).Distinct().Count());
    }

    /// <summary>
    /// The lossy store must fake the same saves however many conflicts happen in between, or
    /// the known answer above depends on timing. Each change here is preceded by a conflicting
    /// attempt with a wrong version.
    /// </summary>
    [Fact]
    public async Task Lossy_store_fakes_every_fifth_change_regardless_of_conflicts()
    {
        var store = new LossyTestStore(_folder.Path, NullLogger.Instance);
        var id = Guid.NewGuid();
        await store.TryCreateAsync(StoreWorkload.Seed(id));
        var stale = new List<WriteResult>();
        var saved = new List<WriteResult>();

        for (var i = 0; i < 10; i++)
        {
            var current = (await store.GetAsync(id))!;
            stale.Add(await store.TryReplaceAsync(WithMarker(current, current.Version + 5, $"w:{i}"), current.Version + 5));
            saved.Add(await store.TryReplaceAsync(WithMarker(current, current.Version, $"w:{i}"), current.Version));
        }

        var markers = (await store.GetAsync(id))!.History.Select(h => h.Comment).OfType<string>().ToList();
        Assert.All(stale, r => Assert.Equal(WriteResult.Conflict, r));
        Assert.All(saved, r => Assert.Equal(WriteResult.Saved, r));
        Assert.Equal(["w:0", "w:1", "w:2", "w:3", "w:5", "w:6", "w:7", "w:8"], markers);
    }

    [Fact]
    public async Task Each_process_reports_its_own_acks_and_they_add_up()
    {
        var plugins = new[] { typeof(LockedFolderTestStore).Assembly };
        var store = StoreStrategies.Create("TestLockedFolder", _folder.Path, plugins);
        var record = Guid.NewGuid();
        await store.TryCreateAsync(StoreWorkload.Seed(record));

        var runs = await MultiProcessRun.RunAsync("TestLockedFolder", _folder.Path, [record], processes: 3, iterations: 15, plugins);

        Assert.All(runs, r => Assert.Equal(15, r.Acks.Count));
        Assert.Equal(1 + (3 * 15), (await store.GetAsync(record))!.Version);
        Assert.Equal(Enumerable.Range(2, 45), runs.SelectMany(r => r.Acks).Select(a => a.Version).Order());
    }

    /// <summary>
    /// A read that fails on broken data is counted and retried, not a crash. One writer, no
    /// conflicts: 20 changes need 20 good reads, and with every third read broken the 20th good
    /// read is read number 29, after 9 broken ones.
    /// </summary>
    [Fact]
    public async Task Workload_counts_broken_reads_and_retries_them()
    {
        var store = new BrokenReadsTestStore(new LockedFolderTestStore(_folder.Path, NullLogger.Instance), everyNth: 3);
        var id = Guid.NewGuid();
        await store.TryCreateAsync(StoreWorkload.Seed(id));

        var result = await StoreWorkload.RunAsync(store, [id], "w", 20, _ => { });

        Assert.Equal(new WorkloadResult(Acks: 20, Conflicts: 0, BrokenReads: 9), result);
        Assert.Equal(21, (await new LockedFolderTestStore(_folder.Path, NullLogger.Instance).GetAsync(id))!.Version);
    }

    /// <summary>A file that stays broken must not hang the workload: it gives up after a fixed time of nothing but broken reads.</summary>
    [Fact]
    public async Task Workload_gives_up_on_a_store_that_never_reads()
    {
        var store = new BrokenReadsTestStore(new LockedFolderTestStore(_folder.Path, NullLogger.Instance), everyNth: 1);
        var id = Guid.NewGuid();
        await store.TryCreateAsync(StoreWorkload.Seed(id));
        var clock = System.Diagnostics.Stopwatch.StartNew();

        var result = await StoreWorkload.RunAsync(store, [id], "w", 20, _ => { });

        Assert.InRange(clock.Elapsed, StoreWorkload.GiveUpOnBrokenDataAfter, StoreWorkload.GiveUpOnBrokenDataAfter * 10);
        Assert.Equal(0, result.Acks);
        Assert.Equal(0, result.Conflicts);
        Assert.True(result.BrokenReads > 1, $"Expected repeated broken reads, got {result.BrokenReads}.");
    }

    /// <summary>
    /// Only broken data counts as a broken read. Anything else a store throws, such as a missing
    /// folder, still stops the workload with the store's own exception.
    /// </summary>
    [Fact]
    public async Task Workload_fails_loudly_on_errors_that_are_not_broken_data()
    {
        var store = new BrokenReadsTestStore(
            new LockedFolderTestStore(_folder.Path, NullLogger.Instance), everyNth: 1, () => new DirectoryNotFoundException("no data folder"));
        var id = Guid.NewGuid();
        await store.TryCreateAsync(StoreWorkload.Seed(id));

        var e = await Assert.ThrowsAsync<DirectoryNotFoundException>(() => StoreWorkload.RunAsync(store, [id], "w", 1, _ => { }));

        Assert.Equal("no data folder", e.Message);
    }

    /// <summary>
    /// Broken-read retries are not conflicts: they don't count towards
    /// <see cref="StoreWorkload.MaxAttemptsPerChange"/>. Here one change meets more than half that
    /// many conflicts, each followed by a broken read, and must still be saved.
    /// </summary>
    [Fact]
    public async Task Broken_reads_do_not_count_towards_the_conflict_limit()
    {
        var conflicts = (StoreWorkload.MaxAttemptsPerChange / 2) + 100;
        var store = new ConflictingTestStore(
            new BrokenReadsTestStore(new LockedFolderTestStore(_folder.Path, NullLogger.Instance), everyNth: 2), conflicts);
        var id = Guid.NewGuid();
        await store.TryCreateAsync(StoreWorkload.Seed(id));

        var result = await StoreWorkload.RunAsync(store, [id], "w", 1, _ => { });

        Assert.Equal(new WorkloadResult(Acks: 1, Conflicts: conflicts, BrokenReads: conflicts), result);
    }

    /// <summary>
    /// Separate processes report their broken reads instead of exiting with an error, and a report
    /// with broken reads is never "nothing lost", even when every acknowledged marker survived.
    /// </summary>
    [Fact]
    public async Task Broken_reads_in_separate_processes_are_reported()
    {
        var plugins = new[] { typeof(BrokenReadsTestStore).Assembly };
        var seedStore = StoreStrategies.Create("TestLockedFolder", _folder.Path, plugins);
        var records = new[] { Guid.NewGuid(), Guid.NewGuid() };
        foreach (var id in records)
        {
            await seedStore.TryCreateAsync(StoreWorkload.Seed(id));
        }

        var runs = await MultiProcessRun.RunAsync("TestBrokenReads", _folder.Path, records, processes: 2, iterations: 10, plugins);

        Assert.All(runs, r => Assert.Equal(10, r.Acks.Count));
        Assert.All(runs, r => Assert.True(r.BrokenReads > 0, $"{r.Writer} reported no broken reads."));
        var finals = new Dictionary<Guid, RegistrationRecordDto?>();
        foreach (var id in records)
        {
            finals[id] = await seedStore.GetAsync(id);
        }

        var report = WorkloadReport.Check([.. runs.SelectMany(r => r.Acks)], runs.Sum(r => r.Conflicts), records, finals, runs.Sum(r => r.BrokenReads));
        Assert.Empty(report.Lost);
        Assert.Equal(runs.Sum(r => r.BrokenReads), report.BrokenReads);
        Assert.False(report.NothingLost);
    }

    [Fact]
    public async Task Unknown_strategy_makes_the_helper_fail_with_a_message()
    {
        var e = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            MultiProcessRun.RunAsync("NoSuchStrategy", _folder.Path, [Guid.NewGuid()], processes: 1, iterations: 1, []));

        Assert.Contains("Unknown storage strategy 'NoSuchStrategy'", e.Message);
    }

    [Fact]
    public async Task Missing_record_makes_the_helper_fail_with_a_message()
    {
        var plugins = new[] { typeof(LockedFolderTestStore).Assembly };

        var e = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            MultiProcessRun.RunAsync("TestLockedFolder", _folder.Path, [Guid.NewGuid()], processes: 1, iterations: 1, plugins));

        Assert.Contains("is missing", e.Message);
    }

    /// <summary>Refuses the first <paramref name="conflicts"/> replaces as conflicts, then passes them through.</summary>
    private sealed class ConflictingTestStore(IRegistrationStore inner, int conflicts) : IRegistrationStore
    {
        private int _replaces;

        public Task<RegistrationRecordDto?> GetAsync(Guid id, CancellationToken cancellationToken = default) => inner.GetAsync(id, cancellationToken);

        public Task<IReadOnlyList<RegistrationRecordDto>> ListAsync(CancellationToken cancellationToken = default) => inner.ListAsync(cancellationToken);

        public Task<WriteResult> TryCreateAsync(RegistrationRecordDto record, CancellationToken cancellationToken = default) =>
            inner.TryCreateAsync(record, cancellationToken);

        public Task<WriteResult> TryReplaceAsync(RegistrationRecordDto updated, int expectedVersion, CancellationToken cancellationToken = default) =>
            ++_replaces <= conflicts ? Task.FromResult(WriteResult.Conflict) : inner.TryReplaceAsync(updated, expectedVersion, cancellationToken);

        public Task<WriteResult> TryDeleteAsync(Guid id, int expectedVersion, CancellationToken cancellationToken = default) =>
            inner.TryDeleteAsync(id, expectedVersion, cancellationToken);
    }

    /// <summary>The next version after <paramref name="expectedVersion"/>, with a workload-style marker.</summary>
    private static RegistrationRecordDto WithMarker(RegistrationRecordDto current, int expectedVersion, string marker)
    {
        var version = expectedVersion + 1;
        var entry = new HistoryEntryDto(version, RecordAction.Comment, "w", current.UpdatedAt, marker);
        return current with { Version = version, History = [.. current.History, entry] };
    }
}
