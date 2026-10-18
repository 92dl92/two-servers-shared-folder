using BlazorApp.Services.Storage;
using BlazorApp.Shared.Dtos;
using BlazorApp.StoreHelper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Testing;

namespace BlazorApp.Services.Tests.Storage;

/// <summary>
/// The shared test suite every <see cref="IRegistrationStore"/> must pass: save and load,
/// version checks, delete, list, and many threads saving at once. A strategy's test class
/// derives from this (or from <see cref="PersistentRegistrationStoreContract"/> when the
/// store keeps data in a folder) and says how to open the store.
/// </summary>
/// <remarks>
/// Fixed in phase 4a. A strategy that needs a change here stops and reports rather than
/// editing it on its own branch (plan section 7). Changed twice since, each with the owner's
/// approval (plan, Decisions): in 4b, when strategy A broke its file, store calls that fail on
/// broken data are counted as broken reads (<see cref="StoreWorkload.IsBrokenData"/>) and any
/// broken read is a failure; after phase 6b, the control gets up to 5 rounds of
/// <see cref="Many_threads_from_the_same_version_exactly_one_wins"/>, since under load its
/// threads sometimes ran one after another and it happened to lose nothing.
/// </remarks>
public abstract class RegistrationStoreContract : IDisposable
{
    /// <summary>A fresh folder for this test's data.</summary>
    protected TempFolder Folder { get; } = new();

    /// <summary>
    /// True only for a strategy that is expected to lose updates (strategy A, the control):
    /// the concurrency tests then assert that updates <em>are</em> lost, proving the suite can see it.
    /// </summary>
    protected virtual bool ExpectsLostUpdates => false;

    /// <summary>Opens the store under test on a folder.</summary>
    /// <param name="folder">The data folder; stores that don't use one ignore it.</param>
    /// <param name="loggerFactory">Where the store writes its Debug logging (<see cref="StoreLog"/>).</param>
    /// <returns>An open store.</returns>
    protected abstract IRegistrationStore OpenStore(string folder, ILoggerFactory loggerFactory);

    /// <summary>Opens the store under test without logging.</summary>
    /// <param name="folder">The data folder.</param>
    /// <returns>An open store.</returns>
    protected IRegistrationStore OpenStore(string folder) => OpenStore(folder, NullLoggerFactory.Instance);

    /// <inheritdoc />
    public void Dispose()
    {
        Folder.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Save_and_load_return_the_same_data()
    {
        var store = OpenStore(Folder.Path);
        var record = Sample();

        Assert.Equal(WriteResult.Saved, await store.TryCreateAsync(record));

        Assert.Equal(record, await store.GetAsync(record.Id), RecordComparer.Instance);
    }

    [Fact]
    public async Task Replace_with_the_current_version_saves()
    {
        var store = OpenStore(Folder.Path);
        var record = Sample();
        await store.TryCreateAsync(record);
        var updated = Next(record, "second");

        Assert.Equal(WriteResult.Saved, await store.TryReplaceAsync(updated, 1));

        Assert.Equal(updated, await store.GetAsync(record.Id), RecordComparer.Instance);
    }

    [Fact]
    public async Task Out_of_date_version_is_refused_and_changes_nothing()
    {
        var store = OpenStore(Folder.Path);
        var record = Sample();
        await store.TryCreateAsync(record);
        var second = Next(record, "second");
        await store.TryReplaceAsync(second, 1);

        var fromStale = Next(record, "from a stale copy");

        Assert.Equal(WriteResult.Conflict, await store.TryReplaceAsync(fromStale, 1));
        Assert.Equal(second, await store.GetAsync(record.Id), RecordComparer.Instance);
    }

    [Fact]
    public async Task Replace_must_be_exactly_one_version_ahead()
    {
        var store = OpenStore(Folder.Path);
        var record = Sample();
        await store.TryCreateAsync(record);

        await Assert.ThrowsAnyAsync<ArgumentException>(() => store.TryReplaceAsync(record with { Version = 3 }, 1));
        Assert.Equal(record, await store.GetAsync(record.Id), RecordComparer.Instance);
    }

    [Fact]
    public async Task Replace_or_delete_of_a_missing_record_is_not_found()
    {
        var store = OpenStore(Folder.Path);
        var record = Sample();

        Assert.Equal(WriteResult.NotFound, await store.TryReplaceAsync(Next(record, "x"), 1));
        Assert.Equal(WriteResult.NotFound, await store.TryDeleteAsync(record.Id, 1));
        Assert.Null(await store.GetAsync(record.Id));
    }

    [Fact]
    public async Task Creating_an_existing_id_conflicts_and_keeps_the_first()
    {
        var store = OpenStore(Folder.Path);
        var record = Sample();
        await store.TryCreateAsync(record);

        Assert.Equal(WriteResult.Conflict, await store.TryCreateAsync(record with { CreatedBy = "someone-else" }));
        Assert.Equal("alice", (await store.GetAsync(record.Id))!.CreatedBy);
    }

    [Fact]
    public async Task Delete_with_the_current_version_removes_the_record()
    {
        var store = OpenStore(Folder.Path);
        var record = Sample();
        await store.TryCreateAsync(record);

        Assert.Equal(WriteResult.Saved, await store.TryDeleteAsync(record.Id, 1));

        Assert.Null(await store.GetAsync(record.Id));
        Assert.Empty(await store.ListAsync());
    }

    [Fact]
    public async Task Delete_with_an_out_of_date_version_is_refused()
    {
        var store = OpenStore(Folder.Path);
        var record = Sample();
        await store.TryCreateAsync(record);
        await store.TryReplaceAsync(Next(record, "second"), 1);

        Assert.Equal(WriteResult.Conflict, await store.TryDeleteAsync(record.Id, 1));
        Assert.Equal(2, (await store.GetAsync(record.Id))!.Version);
    }

    [Fact]
    public async Task List_returns_every_record()
    {
        var store = OpenStore(Folder.Path);
        var records = Enumerable.Range(0, 5).Select(_ => Sample()).ToList();
        foreach (var record in records)
        {
            await store.TryCreateAsync(record);
        }

        await store.TryDeleteAsync(records[2].Id, 1);
        await store.TryReplaceAsync(Next(records[4], "changed"), 1);

        var listed = (await store.ListAsync()).OrderBy(r => r.Id).ToList();
        var expected = records.Where((_, i) => i != 2).Select(r => r.Id).Order().ToList();
        Assert.Equal(expected, listed.Select(r => r.Id));
        Assert.Equal(2, listed.Single(r => r.Id == records[4].Id).Version);
    }

    [Fact]
    public async Task Records_in_and_out_are_copies()
    {
        var store = OpenStore(Folder.Path);
        var record = Sample();
        await store.TryCreateAsync(record);

        record.Data.City = "changed after saving";
        (await store.GetAsync(record.Id))!.Data.City = "changed after loading";

        Assert.Equal("Zürich", (await store.GetAsync(record.Id))!.Data.City);
    }

    /// <summary>Many threads run the read-modify-write workload on a few records; every acknowledged save must survive.</summary>
    [Fact]
    public async Task Many_threads_saving_at_once_lose_nothing()
    {
        var store = OpenStore(Folder.Path);
        var records = await SeedAsync(store, 3);
        var acks = new System.Collections.Concurrent.ConcurrentBag<Ack>();
        using var start = new Barrier(8);

        var results = await Task.WhenAll(Enumerable.Range(1, 8).Select(t => Task.Factory.StartNew(
            () =>
            {
                start.SignalAndWait();
                return StoreWorkload.RunAsync(store, records, $"t{t}", 40, acks.Add);
            },
            CancellationToken.None,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default).Unwrap()));

        var report = await CheckAsync(OpenStore(Folder.Path), [.. acks], results.Sum(r => r.Conflicts), records, results.Sum(r => r.BrokenReads));
        AssertLostUpdates(report);
    }

    /// <summary>
    /// Many threads try to replace one record from the same version at once: exactly one may win.
    /// For <see cref="ExpectsLostUpdates"/>, several winners or a write that found the data broken
    /// shows the store fails. Such a store only fails if the threads really overlap, and on a
    /// busy machine they sometimes run one after another, so it gets up to
    /// <see cref="RoundsForTheControl"/> rounds, each on a fresh folder, and must fail in at least
    /// one (approved by the owner, plan Decisions). A correct store gets one round, as before.
    /// </summary>
    [Fact]
    public async Task Many_threads_from_the_same_version_exactly_one_wins()
    {
        var rounds = new List<string>();
        for (var round = 1; round <= (ExpectsLostUpdates ? RoundsForTheControl : 1); round++)
        {
            var folder = ExpectsLostUpdates ? Directory.CreateDirectory(Path.Combine(Folder.Path, $"round-{round}")).FullName : Folder.Path;
            var (store, record, results) = await RaceAsync(folder);
            var winners = results.Count(r => !r.Broken && r.Value == WriteResult.Saved);
            var broken = results.Count(r => r.Broken);
            if (!ExpectsLostUpdates)
            {
                Assert.Equal(0, broken);
                Assert.Equal(1, winners);
                Assert.Equal(RaceThreads - 1, results.Count(r => r.Value == WriteResult.Conflict));
                Assert.Equal(2, (await store.GetAsync(record.Id))!.Version);
                return;
            }

            if (winners > 1 || broken > 0)
            {
                return;
            }

            // The full split, so a failure shows whether the threads overlapped at all.
            var answers = string.Join(", ", results.Where(r => !r.Broken).GroupBy(r => r.Value).Select(g => $"{g.Count()} {g.Key}"));
            rounds.Add($"round {round}: {answers}, {broken} broken");
        }

        Assert.Fail($"Expected several writers to 'win' from the same version or find the data broken in at least one of {RoundsForTheControl} rounds, got {string.Join("; ", rounds)}.");
    }

    /// <summary>Threads in <see cref="Many_threads_from_the_same_version_exactly_one_wins"/>.</summary>
    private const int RaceThreads = 32;

    /// <summary>How many rounds a store that <see cref="ExpectsLostUpdates"/> gets to show it.</summary>
    private const int RoundsForTheControl = 5;

    /// <summary>One round: a new record in <paramref name="folder"/>, then every thread replaces it from version 1 at once.</summary>
    private async Task<(IRegistrationStore Store, RegistrationRecordDto Record, (WriteResult Value, bool Broken)[] Results)> RaceAsync(string folder)
    {
        var store = OpenStore(folder);
        var record = Sample();
        Assert.Equal(WriteResult.Saved, await store.TryCreateAsync(record));
        using var start = new Barrier(RaceThreads);

        var results = await Task.WhenAll(Enumerable.Range(0, RaceThreads).Select(i => Task.Factory.StartNew(
            () =>
            {
                start.SignalAndWait();
                return TryOrBrokenAsync(() => store.TryReplaceAsync(Next(record, $"thread {i}"), 1));
            },
            CancellationToken.None,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default).Unwrap()));
        return (store, record, results);
    }

    /// <summary>
    /// Every store logs its reads, writes and conflicts at Debug through <see cref="StoreLog"/>,
    /// with the same event ids, so the experiment can count them per strategy (plan section 8).
    /// </summary>
    [Fact]
    public async Task Store_logs_reads_writes_and_conflicts_at_debug()
    {
        var collector = new FakeLogCollector();
        using var loggerFactory = LoggerFactory.Create(b => b.SetMinimumLevel(LogLevel.Debug).AddProvider(new FakeLoggerProvider(collector)));
        var store = OpenStore(Folder.Path, loggerFactory);
        var record = Sample();

        await store.TryCreateAsync(record);
        await store.GetAsync(record.Id);
        await store.ListAsync();
        await store.TryReplaceAsync(Next(record, "second"), 1);
        await store.TryReplaceAsync(Next(record, "stale"), 1);
        await store.TryDeleteAsync(record.Id, 2);

        var events = collector.GetSnapshot().Where(r => r.Id.Id is >= 2000 and < 2100).ToList();
        Assert.All(events, e => Assert.Equal(LogLevel.Debug, e.Level));

        string? Field(FakeLogRecord r, string name) => r.GetStructuredStateValue(name);
        Assert.Contains(events, e => e.Id == StoreLog.Read && Field(e, "RecordId") == record.Id.ToString() && Field(e, "Found") == "True");
        Assert.Contains(events, e => e.Id == StoreLog.List && Field(e, "Count") == "1");
        Assert.Contains(events, e => e.Id == StoreLog.Write && Field(e, "Operation") == "Create" && Field(e, "Result") == "Saved");
        Assert.Contains(events, e => e.Id == StoreLog.Write && Field(e, "Operation") == "Replace" && Field(e, "Result") == "Saved");
        Assert.Contains(events, e => e.Id == StoreLog.Conflict && Field(e, "Operation") == "Replace" && Field(e, "ExpectedVersion") == "1");
        Assert.Contains(events, e => e.Id == StoreLog.Write && Field(e, "Operation") == "Delete" && Field(e, "Result") == "Saved");
    }

    /// <summary>Asserts the workload lost nothing, or (for <see cref="ExpectsLostUpdates"/>) that it did.</summary>
    /// <param name="report">The checked workload.</param>
    protected void AssertLostUpdates(WorkloadReport report)
    {
        ArgumentNullException.ThrowIfNull(report);
        Assert.True(report.Acks > 0, "The workload saved nothing at all.");
        if (ExpectsLostUpdates)
        {
            Assert.False(report.NothingLost, $"Expected lost updates, but every save survived: {report}");
        }
        else
        {
            Assert.True(report.NothingLost, report.ToString());
        }
    }

    /// <summary>Creates fresh records for a workload.</summary>
    /// <param name="store">The store.</param>
    /// <param name="count">How many.</param>
    /// <returns>Their ids.</returns>
    protected static async Task<IReadOnlyList<Guid>> SeedAsync(IRegistrationStore store, int count)
    {
        ArgumentNullException.ThrowIfNull(store);
        var ids = Enumerable.Range(0, count).Select(_ => Guid.NewGuid()).ToList();
        foreach (var id in ids)
        {
            Assert.Equal(WriteResult.Saved, await store.TryCreateAsync(StoreWorkload.Seed(id)));
        }

        return ids;
    }

    /// <summary>Reads the records back and compares them with the acknowledgements.</summary>
    /// <param name="store">A store to read with (ideally freshly opened).</param>
    /// <param name="acks">All acknowledged saves.</param>
    /// <param name="conflicts">Total conflicts retried.</param>
    /// <param name="records">The records written to.</param>
    /// <param name="brokenReads">Store calls during the workload that failed on broken data.</param>
    /// <returns>
    /// The report. A read-back that fails on broken data adds a broken read and leaves that record
    /// missing, rather than throwing.
    /// </returns>
    protected static async Task<WorkloadReport> CheckAsync(
        IRegistrationStore store, IReadOnlyList<Ack> acks, int conflicts, IReadOnlyList<Guid> records, int brokenReads)
    {
        ArgumentNullException.ThrowIfNull(store);
        ArgumentNullException.ThrowIfNull(records);
        var finals = new Dictionary<Guid, RegistrationRecordDto?>();
        foreach (var id in records)
        {
            var final = await TryOrBrokenAsync(() => store.GetAsync(id));
            if (final.Broken)
            {
                brokenReads++;
            }

            finals[id] = final.Value;
        }

        return WorkloadReport.Check(acks, conflicts, records, finals, brokenReads);
    }

    /// <summary>Runs a store call, catching a failure on broken data (<see cref="StoreWorkload.IsBrokenData"/>) instead of throwing.</summary>
    /// <typeparam name="T">The call's result type.</typeparam>
    /// <param name="call">The store call.</param>
    /// <returns>The result and false, or default and true if the data was broken; check <c>Broken</c>, not the value.</returns>
    protected static async Task<(T? Value, bool Broken)> TryOrBrokenAsync<T>(Func<Task<T>> call)
    {
        ArgumentNullException.ThrowIfNull(call);
        try
        {
            return (await call(), false);
        }
        catch (Exception e) when (StoreWorkload.IsBrokenData(e))
        {
            return (default, true);
        }
    }

    /// <summary>A record with every field set, including non-ASCII text.</summary>
    /// <returns>A new record at version 1.</returns>
    protected static RegistrationRecordDto Sample()
    {
        var at = new DateTimeOffset(2026, 5, 6, 7, 8, 9, TimeSpan.FromHours(2));
        return new RegistrationRecordDto(
            Guid.NewGuid(),
            1,
            RecordStatus.Draft,
            "alice",
            at,
            "alice",
            at,
            new RegistrationDto
            {
                FirstName = "Zoë",
                LastName = "Ångström-\"Quote\"",
                Email = "zoe@example.com",
                Phone = "+41 44 000 00 00",
                Street = "Bahnhofstrasse 1",
                City = "Zürich",
                PostalCode = "8001",
                Country = "Schweiz",
                PreferredContact = ContactMethod.Sms,
                SubscribeToNewsletter = true,
                Notes = "Line one\nline two — 日本語",
            },
            [new HistoryEntryDto(1, RecordAction.Create, "alice", at)]);
    }

    /// <summary>The next version of a record, with a comment entry.</summary>
    /// <param name="record">The current version.</param>
    /// <param name="comment">Comment text for the new history entry.</param>
    /// <returns>A record one version ahead.</returns>
    protected static RegistrationRecordDto Next(RegistrationRecordDto record, string comment)
    {
        ArgumentNullException.ThrowIfNull(record);
        var version = record.Version + 1;
        return record with
        {
            Version = version,
            UpdatedBy = "rita",
            History = [.. record.History, new HistoryEntryDto(version, RecordAction.Comment, "rita", record.UpdatedAt, comment)],
        };
    }
}
