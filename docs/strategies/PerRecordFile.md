# Strategy C: `PerRecordFile`

One JSON file per record, a version check on every write, and writes that appear by
rename (plan section 3, row C). Code: `webapp/src/BlazorApp.Services/Storage/PerRecordFile/`.
Tests: `webapp/tests/BlazorApp.Services.Tests/Storage/PerRecordFileStoreTests.cs`.

Select it with `--Storage:Strategy=PerRecordFile --Storage:Path=<folder>`.

## Folder layout

| Path | What |
|---|---|
| `records/{id}.json` | One record, indented JSON via `RecordJson`. Only ever a complete file, because it appears by rename. |
| `tmp/{id}.{random}.tmp` | A write in progress. Renamed into `records/` once complete and flushed. |
| `locks/{id}.lock` | An empty file per record, held with an exclusive OS lock during a check-and-write. Never deleted. |

## Design choices

### Making check-and-rename atomic across processes

A rename on its own is atomic, but "read the version, compare, rename" is not: two
processes can both read version 4, both pass the check, and both rename. The second
rename silently replaces the first, and an update that was reported as saved is lost.

So every create, replace and delete holds a **per-record lock file** for the whole
check-and-write. The lock is taken by opening `locks/{id}.lock` with `FileShare.None`,
which .NET turns into `flock(LOCK_EX)` on Linux (a sharing lock on Windows). That lock
is respected by other processes and by other handles in the same process, so it also
covers threads within one server. It is released when the stream is disposed, or by the
OS if the process dies, so a crashed server never leaves a record locked.

- **Waiting:** if the lock is held, the writer retries every 1 ms until it gets it, or until
  the caller cancels. Only real contention is retried (a plain `IOException` with HResult
  11 `EWOULDBLOCK` on Linux, a sharing or lock violation on Windows). Any other error,
  such as a missing folder, a full disk or no permission, is thrown immediately rather than
  retried forever (code review finding, with a test).
- **Only the same record waits:** each record has its own lock file, so writers to different
  records never wait for each other.
- **Create** checks that `records/{id}.json` doesn't exist under the same lock, so two
  creates of one id can't both succeed. **Delete** checks the version under the lock, then
  deletes the file.
- **Lock files are never deleted.** Deleting one is unsafe: a waiter that already opened
  the old file would lock a file that is no longer in the folder, while the next writer
  creates and locks a new one, and both would think they hold the lock. The cost is one
  empty file per record ever written, including deleted records.

### Reads take no lock

`GetAsync` and `ListAsync` read straight from disk with no lock and no cache. Because a
record file is only ever replaced by rename, a reader sees either the old file or the new
one, never a mix. Record files are opened with `FileShare.ReadWrite | FileShare.Delete`, so
that an open reader doesn't block a writer's rename or delete.

**Corrected in phase 6b:** on Windows that isn't enough. `File.Move(..., overwrite: true)`
was still refused with "access denied" while a reader had the old file open: 3–5 saves per
light run and 45–68 per heavy run on Windows' own disk, about three times as many over SMB,
each a 500 to the user (nothing lost). The rename is now tried again for up to 3 s (first 1 s; raised after heavy runs with the share's leasing off still gave up a few times)
while Windows reports the file in use (`Storage/FileReplace.cs`). Each replace that needed
retries logs one warning (`StoreReplaceRetried`, 2005) with the count, the wait and whether
it then succeeded or gave up. Linux and macOS allow the rename regardless, so there access
denied is thrown at once. Known gap: deleting a record file has no such retry; no workload
deletes, so it is untested on Windows.

### The list cache (phase 6b)

The volume test (6,000 records, layout 2) showed that listing read every record file: about
0.6 s on ws1's own disk, and more than 60 s (every list timed out) on ws2 over SMB. The owner
chose a cache in each server's store, keeping one file per record on the share:

- `ListAsync` lists the folder once; the listing gives each file's creation time, last write
  time and size without opening it. Records whose values match what the store remembers are
  taken from memory; only new or changed files are read (16 at a time), and deleted ones drop
  out. Every write creates a new file (temp file and rename), but Windows can give a file
  renamed over another the old one's creation time (file name tunneling), so two saves in one
  clock tick with the same size could look identical. A copy read within 3 s of its file's last
  write is therefore always read again (git's "racily clean" rule); the servers' clocks must
  agree to well within that.
- Lists come one at a time, so a list that waited finds the records already read.
- `GetAsync` and every write's version check still read the file itself, so a record's page
  and the 409 rule never use the cache. Over SMB the client may cache directory listings for
  a few seconds, so a list can be that far behind.

Measured on W2 with 6,000 records: once warm, a list took 33 ms (median) on ws1 and 127 ms on
ws2. **Known limitation, accepted by the owner:** the first list after a server starts reads
every file, about 21 s on ws1 and 53 s on ws2 on these 2-processor VMs, and slows other requests
while it runs (the parallel reads block thread pool threads, which grows slowly on 2 processors).
It happens once per server start. Reading on dedicated threads, or warming the cache when the
server starts, would address it if needed.

### Temp files: naming and clean-up

- Temp files go in their own folder, `tmp/`, not next to the records, so `records/` only
  ever holds complete files. The rename from `tmp/` to `records/` stays on the same file
  system, so it is still one atomic step.
- Name: `{id}.{new Guid}.tmp`, created with `FileMode.CreateNew`, written, then flushed
  with `Flush(flushToDisk: true)` before the rename. Without that flush, a crash soon after a
  save could leave a renamed but empty file.
- If writing or renaming throws, the temp file is deleted on a best-effort basis, without
  hiding the original error (code review finding).
- **Orphans** from a writer that crashed between creating and renaming its temp file are
  removed by the next write to the same record. Only the holder of a record's lock writes
  that record's temp files, so any `tmp/{id}.*.tmp` found while holding the lock is an orphan.
  That costs one scan of `tmp/` per write, and `tmp/` normally holds only writes in
  progress, so the scan is small.

### How List copes with stray files

`ListAsync` enumerates `records/` and reads only names of the form `{guid}.json`; anything
else, such as an editor backup or a stray `.tmp`, is skipped. A file that is listed and
then deleted before it can be read counts as deleted, not as an error (the count is in
the log detail as `deletedWhileListing`). A record file that exists but isn't valid JSON
**does** make List throw. That is deliberate: the shared suite uses List to check for
broken files, and skipping one quietly would hide exactly what the experiment is looking for.

### Logging

Every call logs through `StoreLog` at Debug:

- read: `file={id}.json`
- list: `folder=records, deletedWhileListing=N`
- create / replace / delete: `file={id}.json, lockWaitMs=…, lockAttempts=…, orphanedTempFilesRemoved=…`

Conflicts are logged as `StoreConflict` (2004) by `StoreLog.LogWrite`. `lockWaitMs` and
`lockAttempts` show how much writers to the same record waited for each other, which is
the behaviour this strategy is meant to show.

## What the suite showed

- All 16 shared tests pass (`PersistentRegistrationStoreContract`), including 4 separate
  processes × 40 changes on 2 records, and 8 threads × 40 changes on 3 records. The
  strategy's tests passed in 5 repeat runs in a row (local disk, ext4). There are 3 tests of
  its own (stray files skipped, orphans removed, lock errors not retried); each was seen
  failing, either before the fix or with the feature deliberately broken.
- **Spot-check: the lock is what prevents lost updates.** With the lock removed (rename
  kept, orphan clean-up off as well, since without the lock it would delete other writers'
  temp files), two runs gave:
  - 8 threads: `acks=320 lost=141` and `acks=320 lost=116`
  - 4 processes: `acks=160 lost=62` and `acks=160 lost=24`
  - many threads from one version: several "winners" (one of the two runs)

  `broken=0` every time: the rename alone prevents half-written files but not lost updates.
  That is the difference between this strategy's two halves.

## Surprises and limits

- **The first spot-check failed for the wrong reason.** With the lock removed but the orphan
  clean-up kept, writers deleted each other's in-progress temp files and crashed with
  `FileNotFoundException`, which hid the lost updates. The clean-up depends on the lock
  just as much as the version check does.
- **Durability:** the temp file is flushed, but the `records/` folder itself is not fsynced
  after the rename or delete (.NET can't open a folder as a stream without native calls).
  After a power cut, a save reported as done could in principle come back as the previous
  version. A process crash doesn't cause this; only losing the OS page cache does. Not
  fixed; out of scope for the concurrency experiment.
- **Locks that silently do nothing:** .NET ignores some `flock` errors (e.g. `ENOLCK`), and the
  environment variable `DOTNET_SYSTEM_IO_DISABLEFILELOCKING` turns `FileShare` locking off
  entirely. On a file system without working `flock` (some NFS/SMB/FUSE mounts), every
  writer would "get" the lock, and updates would be lost with nothing logged. Phase 6
  (network drive) is where this will show.
- **Directory scans during renames:** POSIX leaves it open whether a scan sees an entry
  that is renamed over while the scan runs, so List could in theory return a record twice
  or miss one that is being replaced. A test listing 50 records while they were replaced
  continuously for 2 s, on both ext4 and tmpfs (kernel 6.18), never saw a duplicate, so
  no code was added for it.
- **Windows** wasn't tested. Reads open files with `FileShare.Delete` so renames aren't
  blocked, but a read of a file whose delete is still pending may throw
  `UnauthorizedAccessException` there.
- **Clean-up only happens on the next write to the same record:** orphans for a record
  that is never written again (e.g. a crashed create), and lock files for deleted records,
  stay in the folder. They are harmless to correctness.
