using BlazorApp.Services.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;

namespace BlazorApp.Services.Tests.Storage;

/// <summary>
/// Strategy C, one file per record with a version check, against the full shared suite,
/// including several processes saving at once, plus checks of its own folder handling.
/// </summary>
public sealed class PerRecordFileStoreTests : PersistentRegistrationStoreContract
{
    /// <inheritdoc />
    protected override string Strategy => "PerRecordFile";

    [Fact]
    public async Task List_skips_files_in_records_that_are_not_records()
    {
        var store = OpenStore(Folder.Path);
        var record = Sample();
        await store.TryCreateAsync(record);
        var records = Path.Combine(Folder.Path, "records");
        await File.WriteAllTextAsync(Path.Combine(records, $"{record.Id}.json.bak"), "not json");
        await File.WriteAllTextAsync(Path.Combine(records, $"{Guid.NewGuid()}.tmp"), "{ half");
        await File.WriteAllTextAsync(Path.Combine(records, "notes.json"), "not a record");

        var listed = await store.ListAsync();

        Assert.Equal(record.Id, Assert.Single(listed).Id);
    }

    [Fact]
    public async Task A_write_removes_temp_files_a_crashed_writer_left_for_that_record()
    {
        var store = OpenStore(Folder.Path);
        var record = Sample();
        await store.TryCreateAsync(record);
        var temp = Path.Combine(Folder.Path, "tmp");
        var orphan = Path.Combine(temp, $"{record.Id}.{Guid.NewGuid():N}.tmp");
        var otherRecords = Path.Combine(temp, $"{Guid.NewGuid()}.{Guid.NewGuid():N}.tmp");
        await File.WriteAllTextAsync(orphan, "{ half");
        await File.WriteAllTextAsync(otherRecords, "{ half");

        Assert.Equal(WriteResult.Saved, await store.TryReplaceAsync(Next(record, "second"), 1));

        Assert.False(File.Exists(orphan));
        Assert.True(File.Exists(otherRecords), "Another record's temp file may be in use by its lock holder.");
    }

    /// <summary>
    /// Phase 6b's volume test: listing ~6,000 records read every file, which over SMB took more
    /// than a minute. The list now reads only files that are new or changed since its last list.
    /// </summary>
    [Fact]
    public async Task A_list_reads_only_files_that_changed_since_the_last_list()
    {
        var collector = new FakeLogCollector();
        using var loggerFactory = LoggerFactory.Create(b => b.SetMinimumLevel(LogLevel.Debug).AddProvider(new FakeLoggerProvider(collector)));
        var store = OpenStore(Folder.Path, loggerFactory);
        var records = Enumerable.Range(0, 5).Select(_ => Sample()).ToList();
        foreach (var record in records)
        {
            await store.TryCreateAsync(record);
        }

        AgeRecordFiles();
        Assert.Equal(5, (await store.ListAsync()).Count);
        Assert.Contains("filesRead=5", LastList(collector), StringComparison.Ordinal);

        Assert.Equal(5, (await store.ListAsync()).Count);
        Assert.Contains("filesRead=0", LastList(collector), StringComparison.Ordinal);

        await store.TryReplaceAsync(Next(records[2], "second"), 1);
        var listed = await store.ListAsync();
        Assert.Contains("filesRead=1", LastList(collector), StringComparison.Ordinal);
        Assert.Equal(2, listed.Single(r => r.Id == records[2].Id).Version);
    }

    [Fact]
    public async Task Lists_at_the_same_moment_read_the_files_once()
    {
        // Phase 6b: three users opening the list on a cold server each read all 6,000 files.
        var collector = new FakeLogCollector();
        using var loggerFactory = LoggerFactory.Create(b => b.SetMinimumLevel(LogLevel.Debug).AddProvider(new FakeLoggerProvider(collector)));
        var store = OpenStore(Folder.Path, loggerFactory);
        for (var i = 0; i < 50; i++)
        {
            await store.TryCreateAsync(Sample());
        }

        AgeRecordFiles();
        var lists = await Task.WhenAll(Enumerable.Range(0, 4).Select(_ => Task.Run(() => store.ListAsync())));

        Assert.All(lists, l => Assert.Equal(50, l.Count));
        var read = collector.GetSnapshot().Where(r => r.Id == StoreLog.List)
            .Sum(r => int.Parse(r.Message.Split("filesRead=")[1].Split(',')[0], System.Globalization.CultureInfo.InvariantCulture));
        Assert.Equal(50, read);
    }

    [Fact]
    public async Task A_change_that_leaves_the_file_looking_the_same_is_still_listed()
    {
        // The cache knows a file by creation time, write time and size. Windows can give a file
        // renamed over another the old one's creation time, and two saves in one clock tick with
        // the same size look identical. Simulated: the record is changed, then the file's times
        // set back to what the cache saw. A list soon after the file was written must read it again.
        var lister = OpenStore(Folder.Path);
        var writer = OpenStore(Folder.Path);
        var record = Sample();
        await writer.TryCreateAsync(record);
        var path = Path.Combine(Folder.Path, "records", $"{record.Id}.json");
        Assert.Equal(1, Assert.Single(await lister.ListAsync()).Version);
        var (created, written, length) = (File.GetCreationTimeUtc(path), File.GetLastWriteTimeUtc(path), new FileInfo(path).Length);

        // Version 1 to 2: the same size, so only the content differs.
        await File.WriteAllBytesAsync(path, RecordJson.Serialize(record with { Version = 2 }));
        Assert.Equal(length, new FileInfo(path).Length);
        File.SetCreationTimeUtc(path, created);
        File.SetLastWriteTimeUtc(path, written);

        Assert.Equal(2, Assert.Single(await lister.ListAsync()).Version);
    }

    [Fact]
    public async Task A_list_shows_what_another_server_changed_or_deleted()
    {
        // Two instances on one folder, as two servers: one lists (and remembers), the other writes.
        var lister = OpenStore(Folder.Path);
        var writer = OpenStore(Folder.Path);
        var kept = Sample();
        var deleted = Sample();
        await writer.TryCreateAsync(kept);
        await writer.TryCreateAsync(deleted);
        Assert.Equal(2, (await lister.ListAsync()).Count);

        await writer.TryReplaceAsync(Next(kept, "second"), 1);
        await writer.TryDeleteAsync(deleted.Id, 1);
        var added = Sample();
        await writer.TryCreateAsync(added);

        var listed = (await lister.ListAsync()).ToDictionary(r => r.Id);
        Assert.Equal(new[] { kept.Id, added.Id }.Order(), listed.Keys.Order());
        Assert.Equal(2, listed[kept.Id].Version);
    }

    /// <summary>
    /// Files written a moment ago are always read again by a list (a change can look the same
    /// within a clock tick); this makes them a minute old, as most files are when a list runs.
    /// </summary>
    private void AgeRecordFiles()
    {
        foreach (var file in Directory.EnumerateFiles(Path.Combine(Folder.Path, "records")))
        {
            File.SetLastWriteTimeUtc(file, DateTime.UtcNow.AddMinutes(-1));
        }
    }

    private static string LastList(FakeLogCollector collector) =>
        collector.GetSnapshot().Last(r => r.Id == StoreLog.List).Message;

    /// <summary>Only a held lock is waited for; an error that will never clear fails the write instead of retrying forever.</summary>
    [Fact]
    public async Task A_lock_error_other_than_contention_fails_instead_of_retrying()
    {
        var store = OpenStore(Folder.Path);
        var record = Sample();
        await store.TryCreateAsync(record);
        Directory.Delete(Path.Combine(Folder.Path, "locks"), recursive: true);
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        await Assert.ThrowsAsync<DirectoryNotFoundException>(() => store.TryReplaceAsync(Next(record, "second"), 1, timeout.Token));
    }
}
