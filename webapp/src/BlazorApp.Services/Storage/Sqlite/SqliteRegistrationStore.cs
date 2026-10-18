using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;
using BlazorApp.Shared.Dtos;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace BlazorApp.Services.Storage.Sqlite;

/// <summary>
/// Strategy E: <see cref="IRegistrationStore"/> on one SQLite database file in the data folder,
/// <c>registrations.db</c>, table <c>records(id, version, json)</c>. The "known good" comparison
/// on local disk; SQLite's own locking makes it safe for several processes on one machine.
/// </summary>
/// <remarks>
/// <para>Settings, chosen for dependability over speed (see <c>docs/strategies/Sqlite.md</c>):</para>
/// <list type="bullet">
/// <item><b>WAL journal mode</b>, set once when the database is opened: readers never block the
/// writer and the writer never blocks readers; writers still take turns. WAL needs every process
/// on the same machine (shared memory), which is the local-disk round of the experiment.</item>
/// <item><b>synchronous = FULL</b>: every commit is flushed to disk before it is reported as
/// saved, so a save survives a power cut, not only a process crash.</item>
/// <item><b>Busy handling</b>: every write starts with <c>BEGIN IMMEDIATE</c>, taking the write lock
/// up front, and SQLite's busy handler (<c>busy_timeout</c>) waits for it up to
/// <see cref="BusyTimeout"/>, so writers from several processes queue instead of failing with
/// <c>SQLITE_BUSY</c>. Taking the lock first also avoids WAL's <c>SQLITE_BUSY_SNAPSHOT</c>, which a
/// read-then-write transaction gets without waiting. If the wait ever exceeds the timeout the
/// exception is thrown, not swallowed: a save is never reported that did not happen.</item>
/// <item><b>One waiting writer per process</b>: writers in this process queue on an async
/// semaphore (one per database file) before taking SQLite's lock. Only one thread per process
/// then blocks in SQLite's busy handler, which cannot be cancelled; the rest wait without
/// holding a thread and can be cancelled. SQLite's lock still does the real work, between
/// processes.</item>
/// </list>
/// <para>Every operation opens its own (pooled) connection, so nothing is cached between calls:
/// each read sees the latest commit from any process.</para>
/// </remarks>
public sealed class SqliteRegistrationStore : IRegistrationStore
{
    /// <summary>The database file's name in the data folder.</summary>
    public const string FileName = "registrations.db";

    /// <summary>How long an operation waits for another connection's lock before giving up.</summary>
    public static readonly TimeSpan BusyTimeout = TimeSpan.FromSeconds(30);

    /// <summary>The in-process writer queue for each database file, shared by every store instance on it.</summary>
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> WriterQueues = new(StringComparer.Ordinal);

    private readonly string _connectionString;
    private readonly SemaphoreSlim _writerQueue;
    private readonly ILogger _logger;

    /// <summary>Opens (or creates) the database in <paramref name="folder"/> and makes sure the table exists.</summary>
    /// <param name="folder">The data folder; created if missing.</param>
    /// <param name="logger">For Debug logging of reads, writes and conflicts.</param>
    /// <exception cref="InvalidOperationException">SQLite would not switch the database to WAL mode.</exception>
    public SqliteRegistrationStore(string folder, ILogger logger)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(folder);
        ArgumentNullException.ThrowIfNull(logger);
        Directory.CreateDirectory(folder);
        _logger = logger;
        var file = Path.GetFullPath(Path.Combine(folder, FileName));
        _writerQueue = WriterQueues.GetOrAdd(file, _ => new SemaphoreSlim(1, 1));
        _connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = file,
            Mode = SqliteOpenMode.ReadWriteCreate,
            Pooling = true,

            // Microsoft.Data.Sqlite's own retry on SQLITE_BUSY (a backstop to busy_timeout below).
            DefaultTimeout = (int)BusyTimeout.TotalSeconds,
        }.ToString();

        using var connection = Open();

        // Persistent in the file: once one process has switched it, every connection uses WAL.
        var mode = Scalar(connection, "PRAGMA journal_mode = WAL;") as string;
        if (!string.Equals(mode, "wal", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"SQLite kept journal mode '{mode}' for {FileName} instead of WAL.");
        }

        Execute(connection, """
            CREATE TABLE IF NOT EXISTS records (
                id      TEXT    NOT NULL PRIMARY KEY,
                version INTEGER NOT NULL,
                json    TEXT    NOT NULL
            );
            """);
    }

    /// <inheritdoc />
    public async Task<RegistrationRecordDto?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var timer = Stopwatch.StartNew();
        await using var connection = Open();
        await using var command = Command(connection, "SELECT json FROM records WHERE id = @id;", ("@id", Key(id)));
        var json = await command.ExecuteScalarAsync(cancellationToken) as string;
        var record = json is null ? null : RecordJson.Deserialize(System.Text.Encoding.UTF8.GetBytes(json));
        StoreLog.LogRead(_logger, id, record is not null, Detail(timer));
        return record;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<RegistrationRecordDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        var timer = Stopwatch.StartNew();
        await using var connection = Open();
        await using var command = Command(connection, "SELECT json FROM records;");
        var all = new List<RegistrationRecordDto>();
        await using (var reader = await command.ExecuteReaderAsync(cancellationToken))
        {
            while (await reader.ReadAsync(cancellationToken))
            {
                all.Add(RecordJson.Deserialize(System.Text.Encoding.UTF8.GetBytes(reader.GetString(0))));
            }
        }

        StoreLog.LogList(_logger, all.Count, Detail(timer));
        return all;
    }

    /// <inheritdoc />
    public async Task<WriteResult> TryCreateAsync(RegistrationRecordDto record, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(record);
        var json = Json(record);
        var (result, detail) = await WriteAsync(
            async (connection, transaction) =>
            {
                await using var insert = Command(
                    connection,
                    "INSERT INTO records (id, version, json) VALUES (@id, @version, @json) ON CONFLICT (id) DO NOTHING;",
                    ("@id", Key(record.Id)),
                    ("@version", record.Version),
                    ("@json", json));
                insert.Transaction = transaction;
                return await insert.ExecuteNonQueryAsync(cancellationToken) == 1 ? WriteResult.Saved : WriteResult.Conflict;
            },
            cancellationToken);
        StoreLog.LogWrite(_logger, StoreOperation.Create, record.Id, null, result, detail);
        return result;
    }

    /// <inheritdoc />
    public async Task<WriteResult> TryReplaceAsync(RegistrationRecordDto updated, int expectedVersion, CancellationToken cancellationToken = default)
    {
        RecordJson.EnsureNextVersion(updated, expectedVersion);
        var json = Json(updated);
        var (result, detail) = await WriteAsync(
            async (connection, transaction) =>
            {
                // The version check and the write are one statement, so nothing can come between them.
                await using var update = Command(
                    connection,
                    "UPDATE records SET version = @version, json = @json WHERE id = @id AND version = @expected;",
                    ("@id", Key(updated.Id)),
                    ("@version", updated.Version),
                    ("@json", json),
                    ("@expected", expectedVersion));
                update.Transaction = transaction;
                return await update.ExecuteNonQueryAsync(cancellationToken) == 1
                    ? WriteResult.Saved
                    : await WhyNotAsync(connection, transaction, updated.Id);
            },
            cancellationToken);
        StoreLog.LogWrite(_logger, StoreOperation.Replace, updated.Id, expectedVersion, result, detail);
        return result;
    }

    /// <inheritdoc />
    public async Task<WriteResult> TryDeleteAsync(Guid id, int expectedVersion, CancellationToken cancellationToken = default)
    {
        var (result, detail) = await WriteAsync(
            async (connection, transaction) =>
            {
                await using var delete = Command(
                    connection,
                    "DELETE FROM records WHERE id = @id AND version = @expected;",
                    ("@id", Key(id)),
                    ("@expected", expectedVersion));
                delete.Transaction = transaction;
                return await delete.ExecuteNonQueryAsync(cancellationToken) == 1
                    ? WriteResult.Saved
                    : await WhyNotAsync(connection, transaction, id);
            },
            cancellationToken);
        StoreLog.LogWrite(_logger, StoreOperation.Delete, id, expectedVersion, result, detail);
        return result;
    }

    /// <summary>
    /// Runs one write in a <c>BEGIN IMMEDIATE</c> transaction, committing only when it saved.
    /// The detail reports the time spent in this process's writer queue and the time spent
    /// waiting for SQLite's write lock (writers in other processes).
    /// </summary>
    private async Task<(WriteResult Result, string Detail)> WriteAsync(
        Func<SqliteConnection, SqliteTransaction, Task<WriteResult>> body, CancellationToken cancellationToken)
    {
        var timer = Stopwatch.StartNew();
        await _writerQueue.WaitAsync(cancellationToken);
        try
        {
            var queueWait = timer.Elapsed;
            cancellationToken.ThrowIfCancellationRequested();
            await using var connection = Open();
            var lockTimer = Stopwatch.StartNew();
            await using var transaction = connection.BeginTransaction(deferred: false);
            var lockWait = lockTimer.Elapsed;
            var result = await body(connection, transaction);

            // Not cancellable from here on: the statement has run, so its result must be kept
            // (committed when saved) and reported, not turned into a cancellation.
            if (result == WriteResult.Saved)
            {
                await transaction.CommitAsync(CancellationToken.None);
            }
            else
            {
                await transaction.RollbackAsync(CancellationToken.None);
            }

            return (result, $"{Detail(timer)}, queue wait {queueWait.TotalMilliseconds:0.0} ms, lock wait {lockWait.TotalMilliseconds:0.0} ms");
        }
        finally
        {
            _writerQueue.Release();
        }
    }

    /// <summary>After a conditional write changed no row: NotFound if the record is missing, else Conflict.</summary>
    /// <remarks>
    /// Runs inside the write transaction, so it sees the same state the write did. Not
    /// cancellable: the write has been decided, so its outcome is reported and logged.
    /// </remarks>
    private static async Task<WriteResult> WhyNotAsync(SqliteConnection connection, SqliteTransaction transaction, Guid id)
    {
        await using var exists = Command(connection, "SELECT 1 FROM records WHERE id = @id;", ("@id", Key(id)));
        exists.Transaction = transaction;
        return await exists.ExecuteScalarAsync(CancellationToken.None) is null ? WriteResult.NotFound : WriteResult.Conflict;
    }

    /// <summary>Opens a connection and applies the per-connection settings.</summary>
    private SqliteConnection Open()
    {
        var connection = new SqliteConnection(_connectionString);
        try
        {
            connection.Open();
            Execute(connection, $"PRAGMA busy_timeout = {(int)BusyTimeout.TotalMilliseconds}; PRAGMA synchronous = FULL;");
            return connection;
        }
        catch
        {
            connection.Dispose();
            throw;
        }
    }

    private static SqliteCommand Command(SqliteConnection connection, string sql, params (string Name, object Value)[] parameters)
    {
        var command = connection.CreateCommand();
        command.CommandText = sql;
        foreach (var (name, value) in parameters)
        {
            command.Parameters.AddWithValue(name, value);
        }

        return command;
    }

    private static void Execute(SqliteConnection connection, string sql)
    {
        using var command = Command(connection, sql);
        command.ExecuteNonQuery();
    }

    private static object? Scalar(SqliteConnection connection, string sql)
    {
        using var command = Command(connection, sql);
        return command.ExecuteScalar();
    }

    /// <summary>The id as stored: text, not the BLOB Microsoft.Data.Sqlite would bind a Guid as, so the file is readable.</summary>
    private static string Key(Guid id) => id.ToString("D");

    /// <summary>The record as <see cref="RecordJson"/> text (the same format, as a string for the TEXT column).</summary>
    private static string Json(RegistrationRecordDto record) => JsonSerializer.Serialize(record, RecordJson.Options);

    private static string Detail(Stopwatch timer) => $"{FileName}, {timer.Elapsed.TotalMilliseconds:0.0} ms";
}
