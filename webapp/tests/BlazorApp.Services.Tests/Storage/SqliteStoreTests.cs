using BlazorApp.Services.Storage;
using BlazorApp.Services.Storage.Sqlite;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;

namespace BlazorApp.Services.Tests.Storage;

/// <summary>
/// Strategy E, <c>Sqlite</c>: one SQLite database file in the data folder, the "known good"
/// comparison on local disk. It must pass the whole shared suite, including several processes,
/// plus a few checks of its own settings (docs/strategies/Sqlite.md).
/// </summary>
public sealed class SqliteStoreTests : PersistentRegistrationStoreContract
{
    /// <inheritdoc />
    protected override string Strategy => "Sqlite";

    private string DatabaseFile => Path.Combine(Folder.Path, SqliteRegistrationStore.FileName);

    [Fact]
    public async Task Database_is_one_file_in_wal_mode_with_id_and_version_columns()
    {
        var store = OpenStore(Folder.Path);
        var record = Sample();
        await store.TryCreateAsync(record);
        await store.TryReplaceAsync(Next(record, "second"), 1);

        using var connection = OpenRaw();
        using var mode = connection.CreateCommand();
        mode.CommandText = "PRAGMA journal_mode;";
        Assert.Equal("wal", mode.ExecuteScalar() as string);

        using var row = connection.CreateCommand();
        row.CommandText = "SELECT id, version FROM records;";
        using var reader = row.ExecuteReader();
        Assert.True(reader.Read());
        Assert.Equal(record.Id.ToString("D"), reader.GetString(0));
        Assert.Equal(2, reader.GetInt32(1));
        Assert.False(reader.Read());
    }

    [Fact]
    public async Task Writes_log_how_long_they_queued_and_waited_for_the_lock()
    {
        var collector = new FakeLogCollector();
        using var loggerFactory = LoggerFactory.Create(b => b.SetMinimumLevel(LogLevel.Debug).AddProvider(new FakeLoggerProvider(collector)));
        var store = OpenStore(Folder.Path, loggerFactory);

        await store.TryCreateAsync(Sample());

        var detail = collector.GetSnapshot().Single(r => r.Id == StoreLog.Write).GetStructuredStateValue("Detail");
        Assert.Contains(SqliteRegistrationStore.FileName, detail, StringComparison.Ordinal);
        Assert.Contains("queue wait", detail, StringComparison.Ordinal);
        Assert.Contains("lock wait", detail, StringComparison.Ordinal);
    }

    /// <summary>
    /// While another process holds the write lock, one writer per process waits for it inside
    /// SQLite; further writers wait in an async queue, which a cancellation ends at once,
    /// without writing.
    /// </summary>
    [Fact]
    public async Task A_writer_queued_behind_a_held_lock_can_be_cancelled_and_writes_nothing()
    {
        var store = OpenStore(Folder.Path);
        var record = Sample();
        await store.TryCreateAsync(record);

        using var other = OpenRaw();
        var held = other.BeginTransaction(deferred: false);
        Task<WriteResult> first;
        try
        {
            first = Task.Run(() => store.TryReplaceAsync(Next(record, "first"), 1));
            await Task.Delay(200);
            using var cancel = new CancellationTokenSource(TimeSpan.FromMilliseconds(200));
            var second = Task.Run(() => store.TryReplaceAsync(Next(record, "second"), 1, cancel.Token));

            var finished = await Task.WhenAny(second, Task.Delay(TimeSpan.FromSeconds(10)));

            Assert.Same(second, finished);
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => second);
            Assert.False(first.IsCompleted, "The first writer should still be waiting for the lock.");
        }
        finally
        {
            held.Rollback();
            held.Dispose();
        }

        Assert.Equal(WriteResult.Saved, await first);
        Assert.Equal("first", (await store.GetAsync(record.Id))!.History[^1].Comment);
    }

    /// <summary>A separate, unpooled connection to the store's database, as another process would have.</summary>
    private SqliteConnection OpenRaw()
    {
        var connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = DatabaseFile, Pooling = false }.ToString());
        connection.Open();
        return connection;
    }
}
