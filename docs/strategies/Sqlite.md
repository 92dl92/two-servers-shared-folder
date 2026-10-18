# Strategy E: SQLite (`Sqlite`)

The "known good" comparison (plan section 3, row E). Phase 5 uses it as the known answer
for the stress tool on local disk: the tool must find **no** lost updates with E.

```
--Storage:Strategy=Sqlite --Storage:Path=/shared/data
```

Code: `webapp/src/BlazorApp.Services/Storage/Sqlite/` (`SqliteRegistrationStore`,
`SqliteStoreFactory`). Tests: `webapp/tests/BlazorApp.Services.Tests/Storage/SqliteStoreTests.cs`
(the full shared suite, `PersistentRegistrationStoreContract`). Package:
`Microsoft.Data.Sqlite` 10.0.13, which bundles its own native SQLite (`e_sqlite3`) for every
platform, so the server and the StoreHelper processes don't depend on a system SQLite.

## Schema

One file in the data folder, `registrations.db` (plus `registrations.db-wal` and
`registrations.db-shm` while it is in use):

```sql
CREATE TABLE IF NOT EXISTS records (
    id      TEXT    NOT NULL PRIMARY KEY,  -- Guid, "D" format (text, so the file is readable)
    version INTEGER NOT NULL,
    json    TEXT    NOT NULL               -- RecordJson, the same format as the file-based stores
);
```

The id and version have their own columns so the database can check the version; the JSON
is the record itself. Both are written from the same record in the same statement.

## Writes

Each write is one statement inside a `BEGIN IMMEDIATE` transaction:

| Operation | Statement | 0 rows changed means |
|---|---|---|
| Create | `INSERT ... ON CONFLICT (id) DO NOTHING` | Conflict (id exists) |
| Replace | `UPDATE records SET version = @version, json = @json WHERE id = @id AND version = @expected` | NotFound or Conflict |
| Delete | `DELETE FROM records WHERE id = @id AND version = @expected` | NotFound or Conflict |

The version check and the write are one statement, so nothing can come between them. When
no row changed, a `SELECT` in the same transaction tells NotFound from Conflict. Only a
`Saved` write is committed; the others roll back.

Reads (`GetAsync`, `ListAsync`) are single `SELECT`s, each a consistent snapshot. Every
operation opens its own connection (pooled by Microsoft.Data.Sqlite), so nothing is cached
in memory between calls.

## Settings

| Setting | Value | Why |
|---|---|---|
| Journal mode | **WAL** (`PRAGMA journal_mode = WAL`, set when the store opens; it is stored in the file) | Readers don't block the writer and the writer doesn't block readers. Writers still take turns. The store refuses to open if SQLite won't switch to WAL, so it never silently runs with other settings |
| `synchronous` | **FULL** (set on every connection) | Each commit is flushed to disk before it is reported as saved, so a save survives a power cut, not only a process crash. Slower than `NORMAL`, but E is meant to be dependable, not fast |
| Busy timeout | **30 s** (`PRAGMA busy_timeout = 30000` on every connection) | SQLite's own busy handler waits and retries when another connection holds the lock, instead of failing at once with `SQLITE_BUSY` |
| `DefaultTimeout` | 30 s (connection string) | Microsoft.Data.Sqlite's own retry loop on `SQLITE_BUSY`, a backstop behind the busy timeout |
| Write transactions | `BEGIN IMMEDIATE` | Takes the write lock up front, where the busy handler waits for it. A deferred read-then-write transaction in WAL can get `SQLITE_BUSY_SNAPSHOT`, which fails without waiting. Every write here starts with its writing statement, so a deferred transaction would also be safe (tried: the suite passes either way), but `IMMEDIATE` keeps it safe if a read is ever added first, and separates the time spent waiting for the lock |
| Writer queue in each process | `SemaphoreSlim(1)` per database file, awaited before `BEGIN IMMEDIATE` | SQLite's busy handler blocks the calling thread and can't be cancelled. With the queue only one thread per process waits inside SQLite; the others wait asynchronously (no thread-pool starvation when a server gets many saves at once) and a cancelled request leaves the queue without writing. Between processes, SQLite's lock still does the work |

How `SQLITE_BUSY` is handled: several processes writing at once **wait**, queued by SQLite's
lock (and, within one process, by the writer queue). A write is never retried by the store itself after the fact, and an error is never
swallowed: if a lock is held for more than 30 s, the `SqliteException` reaches the caller (a
500 from the server), and the save is not reported as saved. Once the statement has run, the
commit or rollback ignores cancellation, so a decided outcome is always kept and reported.

## Logging

Through `StoreLog` at Debug, like every store. The detail gives the file and the total time,
and for writes the time spent in the process's writer queue and the time spent waiting for
SQLite's write lock (the busy wait, i.e. writers in other processes), e.g.

```
Store Replace <id> expecting version 1: Saved (registrations.db, <total> ms, queue wait <ms> ms, lock wait <ms> ms)
Store list: 3 records (registrations.db, <total> ms)
```

SQLite's busy handler doesn't report how many times it retried, so the lock wait time is the
measure of contention.

## What the suite showed (local disk, cloud session)

- The full shared suite passes: 16 tests, including 8 threads × 40 changes, 32 threads
  replacing from the same version (exactly one wins) and 4 separate StoreHelper processes ×
  40 changes on 2 records. Three tests of E's own: the file is in WAL mode with the id and
  version in their columns; writes log their queue and lock waits; a writer queued behind a
  lock held by another connection can be cancelled and writes nothing. 10 runs in a
  row passed, about 2 s per run.
- **Spot-checks:** with `AND version = @expected` dropped from the `UPDATE`, 6 tests fail:
  the stale-version test, two instances on one folder, exactly-one-wins, many threads,
  several processes, and the logging test (no conflict logged). Without the WAL switch, the
  WAL test fails. Both reverted.
- **Starting at once:** 12 StoreHelper processes opening a new, empty folder at the same
  moment (5 rounds) all opened the store without errors. Creating the file, switching to WAL
  and creating the table race safely.
- The real server with `--Storage:Strategy=Sqlite` creates and lists records, and its log
  file has the store lines above.

## Network drives (phase 6)

SQLite's own documentation warns that its file locking is unreliable on network file
systems (NFS, SMB): locks may not be honoured, or may be cached per machine, and the result
can be a corrupt database or lost updates. Plan section 3 expects E may fail there.

WAL makes this stricter: it uses a shared-memory file (`-shm`), **requires every process to
be on the same machine**, and SQLite's docs say plainly that WAL does not work over a network
file system, independently of how well the share does locking. For phase 6:

- Expect E in WAL mode to be wrong on a share, certainly across machines. That is a
  documented limit, not a bug in the store.
- A fairer network test of SQLite's locking would use the rollback journal
  (`journal_mode = DELETE`), which locks the database file itself. That is a one-line change
  in the constructor plus the WAL check, and should be recorded as a separate variant if it
  is tried.
