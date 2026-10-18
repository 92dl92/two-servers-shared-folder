using System.Text;
using System.Text.Json;
using BlazorApp.Services.Storage;
using BlazorApp.Services.Storage.AppendLog;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;

namespace BlazorApp.Services.Tests.Storage;

/// <summary>
/// Strategy D, the append-only log: the full shared suite, plus what is particular to a log:
/// a torn last line (a crash mid-append), lines that don't follow on, whether appends from
/// several processes ever interleave, and reading only what was added since the last read.
/// </summary>
public sealed class AppendLogStoreTests : PersistentRegistrationStoreContract
{
    /// <inheritdoc />
    protected override string Strategy => "AppendLog";

    private string LogPath => Path.Combine(Folder.Path, AppendLogRegistrationStore.LogFileName);

    [Fact]
    public async Task A_torn_last_line_is_ignored_and_sealed_by_the_next_write()
    {
        var store = OpenStore(Folder.Path);
        var record = Sample();
        await store.TryCreateAsync(record);
        var other = Sample();
        await store.TryCreateAsync(other);

        // A crash mid-append: the first half of a line that would delete "other", no newline.
        var lines = File.ReadAllLines(LogPath);
        var torn = lines[1].Replace("\"op\":\"Create\"", "\"op\":\"Delete\"", StringComparison.Ordinal);
        await File.AppendAllTextAsync(LogPath, torn[..(torn.Length / 2)]);

        var reopened = OpenStore(Folder.Path);
        Assert.Equal(record, await reopened.GetAsync(record.Id), RecordComparer.Instance);
        Assert.Equal(2, (await reopened.ListAsync()).Count);

        var second = Next(record, "after the crash");
        Assert.Equal(WriteResult.Saved, await reopened.TryReplaceAsync(second, 1));

        var restarted = OpenStore(Folder.Path);
        Assert.Equal(second, await restarted.GetAsync(record.Id), RecordComparer.Instance);
        Assert.NotNull(await restarted.GetAsync(other.Id));

        // The torn line was sealed with a newline, so the new line starts on its own line.
        var after = File.ReadAllLines(LogPath);
        Assert.Equal(4, after.Length);
        Assert.ThrowsAny<JsonException>(() => JsonDocument.Parse(after[2]));
        Assert.Equal("Replace", JsonDocument.Parse(after[3]).RootElement.GetProperty("op").GetString());
    }

    [Fact]
    public async Task A_line_that_does_not_follow_on_is_ignored_on_replay()
    {
        var store = OpenStore(Folder.Path);
        var record = Sample();
        await store.TryCreateAsync(record);

        // Written by something that skipped the lock: replaces version 4, which never existed.
        var bogus = Next(Next(Next(Next(record, "2"), "3"), "4"), "5");
        var line = JsonSerializer.Serialize(
            new { op = "Replace", id = record.Id, expected = 4, record = bogus },
            new JsonSerializerOptions(RecordJson.Options) { WriteIndented = false });
        await File.AppendAllTextAsync(LogPath, line + "\n");

        var reopened = OpenStore(Folder.Path);
        Assert.Equal(record, await reopened.GetAsync(record.Id), RecordComparer.Instance);
        Assert.Equal(WriteResult.Saved, await reopened.TryReplaceAsync(Next(record, "second"), 1));
        Assert.Equal(2, (await OpenStore(Folder.Path).GetAsync(record.Id))!.Version);
    }

    /// <summary>
    /// The plan's question for this strategy: do appends from several processes ever interleave?
    /// After four processes write at once, every line must be one complete entry, and there must
    /// be exactly one line per acknowledged save (conflicts write nothing).
    /// </summary>
    [Fact]
    public async Task Appends_from_several_processes_never_interleave()
    {
        var records = await SeedAsync(OpenStore(Folder.Path), 2);

        var runs = await MultiProcessRun.RunAsync(Strategy, Folder.Path, records, processes: 4, iterations: 40, PluginAssemblies);

        AssertRanConcurrently(runs);
        var bytes = await File.ReadAllBytesAsync(LogPath);
        Assert.Equal((byte)'\n', bytes[^1]);
        var lines = Encoding.UTF8.GetString(bytes).Split('\n')[..^1];
        Assert.Equal(records.Count + runs.Sum(r => r.Acks.Count), lines.Length);
        Assert.All(lines, l => Assert.Equal(JsonValueKind.Object, JsonDocument.Parse(l).RootElement.ValueKind));
    }

    [Fact]
    public async Task A_read_replays_only_the_lines_added_since_the_last_read()
    {
        var collector = new FakeLogCollector();
        using var loggerFactory = LoggerFactory.Create(b => b.SetMinimumLevel(LogLevel.Debug).AddProvider(new FakeLoggerProvider(collector)));
        var reader = OpenStore(Folder.Path, loggerFactory);
        var writer = OpenStore(Folder.Path);
        var record = Sample();
        await writer.TryCreateAsync(record);
        await writer.TryCreateAsync(Sample());
        await reader.ListAsync();

        await writer.TryReplaceAsync(Next(record, "second"), 1);
        collector.Clear();
        Assert.Equal(2, (await reader.GetAsync(record.Id))!.Version);

        var read = Assert.Single(collector.GetSnapshot(), r => r.Id == StoreLog.Read);
        Assert.Contains("1 new line", read.GetStructuredStateValue("Detail"), StringComparison.Ordinal);
    }

    [Fact]
    public async Task A_deleted_log_reads_as_empty_and_the_next_write_starts_a_new_one()
    {
        var store = OpenStore(Folder.Path);
        var old = Sample();
        await store.TryCreateAsync(old);
        Assert.Single(await store.ListAsync());

        File.Delete(LogPath); // e.g. the data folder was reset

        Assert.Null(await store.GetAsync(old.Id));
        Assert.Empty(await store.ListAsync());
        var record = Sample();
        Assert.Equal(WriteResult.Saved, await store.TryCreateAsync(record));
        Assert.Equal(record.Id, Assert.Single(await OpenStore(Folder.Path).ListAsync()).Id);
    }

    [Fact]
    public async Task A_write_fails_instead_of_waiting_when_the_folder_is_gone()
    {
        var store = OpenStore(Folder.Path);
        Directory.Delete(Folder.Path, recursive: true);
        using var patience = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        // Only a lock held by someone else is worth waiting for; anything else is an error.
        await Assert.ThrowsAnyAsync<IOException>(() => store.TryCreateAsync(Sample(), patience.Token));
    }

    [Fact]
    public async Task Lines_longer_than_a_read_chunk_are_replayed_whole()
    {
        var store = OpenStore(Folder.Path);
        var small = Sample();
        var large = Sample();
        large.Data.Notes = string.Concat(Enumerable.Repeat("Zoë — 日本語 ", 20_000)); // ~350 KB, several chunks
        await store.TryCreateAsync(small);
        await store.TryCreateAsync(large);
        await store.TryReplaceAsync(Next(small, "after the large line"), 1);

        var reopened = OpenStore(Folder.Path);
        Assert.Equal(large, await reopened.GetAsync(large.Id), RecordComparer.Instance);
        Assert.Equal(2, (await reopened.GetAsync(small.Id))!.Version);
    }

    [Fact]
    public async Task A_log_replaced_by_a_shorter_one_is_read_again_from_the_start()
    {
        var store = OpenStore(Folder.Path);
        var kept = Sample();
        await store.TryCreateAsync(kept);
        var keptLog = await File.ReadAllBytesAsync(LogPath);
        await store.TryCreateAsync(Sample());
        Assert.Equal(2, (await store.ListAsync()).Count);

        // E.g. restored from a backup: the store must not keep serving what it had replayed.
        await File.WriteAllBytesAsync(LogPath, keptLog);

        var listed = Assert.Single(await store.ListAsync());
        Assert.Equal(kept.Id, listed.Id);
    }
}
