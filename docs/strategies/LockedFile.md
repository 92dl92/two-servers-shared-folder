# Strategy B: `LockedFile` (single file + file lock)

Phase 4b, plan section 3 row B. Code: `webapp/src/BlazorApp.Services/Storage/LockedFile/`.
Tests: `webapp/tests/BlazorApp.Services.Tests/Storage/LockedFileStoreTests.cs`.

Settings: `--Storage:Strategy=LockedFile --Storage:Path=<folder>`.

## How it works

- Every record lives in one file, `<folder>/registrations.json`: a JSON array of records in
  the `RecordJson` format.
- Every operation (get, list, create, replace, delete) does the following while holding an
  exclusive OS lock on `<folder>/registrations.lock`: read the whole file, check the version,
  change the list, write it back.
- A missing data file means no records yet. A data file that doesn't parse throws a
  `JsonException` and is left as it is. It is never treated as empty, because the next write
  would then wipe every record.
- Nothing is cached in memory; every call reads the file from disk.

## Design choices

| Choice | Decision | Why |
|---|---|---|
| What to lock | A separate lock file, `registrations.lock`, never replaced or deleted | Writes replace the data file by renaming a temp file over it, so after each write the data file is a new file (new inode). A lock on the data file would be a lock on the old one: a process that opened the old file while waiting would get a lock no one else checks, and read data that has already been replaced. Every process locks the same lock file. |
| How to lock | `new FileStream(lock, OpenOrCreate, ReadWrite, FileShare.None)`, kept open for the whole read-modify-write | On Linux .NET turns `FileShare.None` into `flock(LOCK_EX \| LOCK_NB)`; on Windows it is a share-mode lock. Both work across processes, and also between two handles in one process. |
| Waiting | Poll: on "already locked", wait 1 ms and try again; cancellable | .NET has no blocking file-lock call. Only the "lock is held" error is retried (`EWOULDBLOCK` 11 on Linux, 35 on macOS; sharing or lock violation on Windows). Any other error, such as a missing folder, is thrown instead of retried for ever. |
| Reads under the lock too | Yes | Part of strategy B's definition (the task asks for it). With temp file + rename a reader would always see a complete file even without the lock, so this costs throughput for consistency only: a read never sees a state that's about to change in the middle of someone's read-modify-write. |
| Writes | Temp file (`registrations.json.tmp`), flushed to disk (`Flush(true)`), then `File.Move(..., overwrite: true)` (`rename(2)` on Linux) | A crash mid-write leaves the old file complete, never half-written. One fixed temp name is enough because only the lock holder writes it; a leftover from a crash is overwritten next time. Flushing before the rename means a power cut can't leave the name pointing at data never written to disk. |
| Rename refused on Windows (phase 6b) | Tried again for up to 3 s while Windows reports the data file in use (`Storage/FileReplace.cs`), one warning per replace that needed retries (`StoreReplaceRetried`, 2005, with the outcome) | Readers hold the lock, so no reader of this store can have the file open, but Windows still refused the rename once in phase 6b's SMB runs (a 500 to the user, nothing lost): most likely ws2's SMB client keeping its handle open for caching for a moment after the read, or an antivirus scan. |
| Durability | Survives a process crash at any point; after a **power cut** the last save may be undone | The temp file is fsynced, but the folder isn't after the rename (.NET has no call for that; it would need P/Invoke). Strictly, a save reported as `Saved` could then be lost. Accepted for this experiment, which kills processes but doesn't cut power. |
| Lock file lifetime | Must never be deleted while servers run | A server that recreated it would lock a different inode than the others, and both would write at once. Nothing detects that. |
| Waiting without a token | Waits for as long as it takes | A server that hangs while holding the lock (paused in a debugger, stuck on a network drive) stalls every other server. Callers can pass a `CancellationToken`. |
| No in-process lock as well | Not added | Threads in one process wait on the same file lock as other processes. A `SemaphoreSlim` in front would reduce polling within one process, but it would also hide whether the file lock alone is enough, which is what the experiment tests. |

Platform note (plan section 3): on Linux the lock is **advisory**. It stops only programs that
ask for the same lock. Every access in this store takes it, but any other tool touching
`registrations.json` must too. The environment variable `DOTNET_SYSTEM_IO_DISABLEFILELOCKING=1`
makes .NET skip `flock` altogether; this store then has no protection at all (see the
spot-checks). On NFS, `flock` may be emulated with `fcntl` locks or be local to one machine,
depending on the client and mount options; that is for phase 6.

## Logging

Every read, list and write goes through `StoreLog` at Debug. The detail field says
`registrations.json, lock wait <ms> ms, <n> attempts`, so lock waits can be counted and
graphed per server from the JSON log.

## What the suite showed (local disk, cloud session, 4 cores)

The full shared suite (16 tests) plus 7 of the strategy's own pass (one file, reads and writes
wait for a held lock, waiting can be cancelled, three kinds of broken data file): 23 in total. The concurrency
tests (many threads, several processes, exactly one winner from one version) passed in 5 of 5
repeated runs.

A separate measurement run, not committed, ran the same workloads as the suite:

| Workload | Saves acknowledged | Conflicts (retried) | Lost | Throughput |
|---|---|---|---|---|
| 4 processes × 40 changes on 2 records (3 runs) | 160 each | 19–32 | 0 | 157–190 saves/s over the writing period |
| 8 threads × 40 changes on 3 records, one store | 320 | 22–25 | 0 | ~610 saves/s |

Lock waits in the 8-thread run (687 operations): median 0.0 ms, p95 3.4 ms, **max 240 ms
after 61 attempts**; 94% got the lock on the first try.

What stood out:

- **Conflicts are few, but not zero.** Saves are serialised, but the workload reads, then
  builds its change, then saves, with the lock released in between. Another writer can get
  in between, so the version check still refuses some saves. That's correct: the lock makes
  each *operation* atomic, and the version check is what protects the read-modify-write that
  spans two operations.
- **Polling isn't fair.** Most operations get the lock at once, but an unlucky one can lose
  the race dozens of times in a row (240 ms, 61 attempts). The stress tool's latency numbers
  will show a long tail for this strategy. A fair queue would need a blocking lock call that
  .NET doesn't offer.
- **The fsync is cheap here.** With `Flush(false)` instead of `Flush(true)` the same run gave
  143–218 saves/s across processes and ~790 saves/s across threads. On this disk the cost of
  the fsync is smaller than the noise between process runs. It may cost more on a network
  drive.
- **Every save rewrites every record.** With two or three records the file is small; the cost
  grows with the number of records, which the stress tool should show.

## Spot-checks (plan section 5)

1. **Lock removed** (the body ran without taking the lock, and each write given its own temp
   file name so the writes don't collide): the multi-process test reported
   `acks=160 lost=78`, the thread test `acks=320 lost=179`, and the own lock tests failed.
   The "32 threads from one version, exactly one wins" test still passed in that run: the
   threads didn't overlap closely enough to show it. The workload tests are the ones that
   catch a missing lock reliably.
   With the lock removed but the shared temp name kept, writers crash on each other's temp file
   instead (`IOException` / `FileNotFoundException`), so the helper processes exit with
   errors.
2. **OS locking switched off** without changing code (`DOTNET_SYSTEM_IO_DISABLEFILELOCKING=1`
   for the test run and the helper processes): 6 tests fail. The lock tests see operations go
   ahead while the lock is held, and the concurrent writers fail on each other's temp file.
   This shows the store's safety rests entirely on the OS lock.

Both were reverted; the code as committed passes everything.

## Self-review (`/code-review high`)

Fixed:

- A data file holding `[null]` crashed with a `NullReferenceException` instead of the promised
  `JsonException` (test case added, seen failing first).
- The log detail string was built even with Debug logging off; now only when Debug is on.
- The lock tests were named "another process" but hold the lock with a second handle in the
  test process; renamed to "another handle" (flock treats both the same; the shared
  multi-process test covers real processes). The tests now use the store's file name constants.

Not fixed, with the reason:

- **No folder fsync after the rename:** needs P/Invoke; documented above (power cut only).
- **The plan's row B says "a lock on the file", but a separate lock file is locked:** the lock
  is still an OS file lock around the whole read-modify-write; the reason is in the design
  table above. This session may not edit the plan's Decisions table, so the owner decides
  whether to record it there.
- **Polling every 1 ms with no backoff, each miss a thrown `IOException`:** kept simple on
  purpose. The measured cost is small on local disk, and backoff would make the unfairness
  noted above worse for the waiter that backs off. The stress tool will show whether it matters.
- **No time limit on the wait; synchronous file I/O inside the async methods:** documented
  above. The work under the lock is short on local disk, and a time limit would turn a slow
  server into lost requests rather than slow ones. Phase 6 (network drive) may change this.
- **Deleting the lock file while servers run splits the lock:** detecting it needs the file's
  inode, which .NET doesn't expose; documented above.
