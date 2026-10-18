# Strategy D: append-only log (`AppendLog`)

Code: `webapp/src/BlazorApp.Services/Storage/AppendLog/`. Tests:
`webapp/tests/BlazorApp.Services.Tests/Storage/AppendLogStoreTests.cs` (the full shared suite plus
the tests named below). Select it with `--Storage:Strategy=AppendLog --Storage:Path=<folder>`.

## Files

| File | Purpose |
|---|---|
| `registrations.jsonl` | The log. One line per acknowledged create, replace or delete; never rewritten |
| `registrations.lock` | Empty. Writers hold an exclusive lock on it while they check and append |

## Line format

One JSON object per line, ended by `\n`, using `RecordJson.Options` (web casing, enums as names)
**without indentation**. Indented JSON spans several lines, so it can't be one line per entry.
This is the only way the format differs from the phase 4a decision ("`RecordJson` gives every
store the same file format (indented JSON, …)"). Recording that in the plan's Decisions table is
left to the owner, because 4b sessions don't edit it.

```json
{"op":"Replace","id":"…","expected":3,"record":{"id":"…","version":4,…,"history":[…]},"pid":4242,"at":"2026-10-17T09:12:01.123+00:00"}
```

| Field | Meaning |
|---|---|
| `op` | `Create`, `Replace` or `Delete` |
| `id` | The record |
| `expected` | The version replaced or deleted; `null` for a create |
| `record` | The whole new record (with its full history); `null` for a delete |
| `pid`, `at` | The writing process and when it appended (stamped under the lock, so in append order). For diagnosis only; replay ignores them |

Each line carries the **whole record**, not a diff. That keeps replay trivial, but the log grows
roughly with *changes × record size*, and a record's size grows with its history. So a record
changed *n* times costs O(n²) bytes in the log (see the numbers below).

## How conditional appends are made safe

The contract needs a replace or delete to be `Saved` only if the version it expected is still
current, even while other processes append. Writing a line is not enough by itself: two
processes can both check version 3 and both append "replace 3 → 4".

**Writers lock across read, check and append.** A writer:

1. waits for an exclusive lock on `registrations.lock`. It is opened with `FileShare.None`, which
   .NET on Linux turns into `flock(LOCK_EX)`, and other processes respect it. Only "held by someone
   else" (EWOULDBLOCK, HResult 11) is retried, every 1 ms. Any other error (folder gone, disk full)
   is thrown instead of waited out.
2. opens the log and replays every line added since its last read. It takes the length from the
   open file, not from a stat that could be cached.
3. seals an incomplete last line, if there is one (see below).
4. checks the version against the replayed state. On a conflict or not-found it appends nothing.
5. appends its line with one positioned write (`RandomAccess.Write` at the end offset it just
   read). While it holds the lock nobody else appends, so that offset is still the end.
6. replays its own line and checks that it applied (otherwise it throws, see below), then releases
   the lock and returns `Saved`.

Threads in one process also queue on a `SemaphoreSlim` first, so they don't spin on the file lock.
Readers take no lock: they only ever read complete lines (below).

**Replay applies the writer's check again.** A line is applied only if it passes the same check
the writer made (`Check`): a create of an id that doesn't exist, or a replace or delete of the
version currently replayed (and a replace must be exactly that version + 1). Under the lock every
appended line passes, so an acknowledged write is never ignored on replay. A line that doesn't
pass could only come from something that bypassed the lock. It is skipped and counted, the same
way by every instance, so all servers agree on the state. As a guard, a writer replays its own
line after appending. If it didn't apply, the writer throws instead of returning `Saved`, so a
broken lock shows up as an error and never as a silently lost acknowledgement.

**Reads never go stale.** Each instance keeps the replayed state plus how many bytes of the log
it covers. A read first does a stat. If the size is unchanged, the state is current (the file only
ever grows). Otherwise it reads and replays only the new bytes, in 64 KB chunks. A log that got
shorter, or was deleted, is replayed from the start. A log *replaced* by one of the same or
greater length is not noticed. The store never rewrites the log, and replacing it while servers
are running is not supported.

## Torn lines (a crash mid-append)

A line counts only once its `\n` is written.

- **Readers** ignore an incomplete last line, because a writer may still be in the middle of it.
  It is read again once the file grows.
- **Writers** hold the lock, so nobody else can be mid-append. An incomplete last line must have
  been left by a writer that crashed (or failed) before returning, so it was **never
  acknowledged**. The next writer appends a `\n` to seal it and replays it like any other line.
  Usually it no longer parses and is skipped and counted (`skipped in total` in the Debug detail).
  If the crash cut off exactly the `\n`, the line is complete JSON and is applied, consistently by
  every instance. That is harmless, because its writer had passed the check under the lock. The
  writer then checks against that state and appends its own line on a fresh line.

Test: `A_torn_last_line_is_ignored_and_sealed_by_the_next_write`. Removing the seal makes it fail,
because the next line is glued to the torn bytes and lost.

**Durability:** `Saved` means the line is in the file, so it survives a *process* crash. There is
no fsync, so after a power cut or OS crash the last lines can still be missing, or zero-filled
(which then counts as a torn line). Adding `Flush(flushToDisk: true)` would close that gap at the
cost of a disk flush per save.

## Did appends ever interleave?

**With the lock: never.** `Appends_from_several_processes_never_interleave` runs 4 processes ×
40 changes on 2 records. It then checks that the log ends in `\n`, that every line parses as one
JSON object, and that there is **exactly one line per acknowledged save** (no lines lost or
duplicated). It passed in every run (more than 15 runs during development, and 5 in a row for the
gate).

**Without the lock (spot-check): yes, and saves were lost.** With the lock replaced by a private
per-call file, each run had at least one failing multi-process test:

- `Several_processes_saving_at_once_lose_nothing`: `lost=2`, `lost=3`, `lost=5` of 160 acks.
- the log had 161 or 163 lines where 162 were expected. Writers that read the same end offset
  wrote over each other: a longer line over a shorter one leaves an extra fragment, and an
  equal-length one replaces a line outright.
- the self-check after appending fired ("did not replay; something wrote … without holding …").

The appends overwrite each other because the store writes at an explicit offset (`pwrite`), not
with `O_APPEND`. .NET's `FileMode.Append` doesn't use `O_APPEND` on Unix either: it seeks to the
end when the file is opened. So without a lock, "append" in .NET is not atomic across processes.
Even with `O_APPEND`, the line would still be unconditional, so the version check would still need
the lock.

## Read cost versus log size

Measured on the cloud session's local disk (Release build, one process). The workload was
*records × changes*: each record is created, then replaced *changes − 1* times, adding one
history entry each time. "Cold" is a new store instance's first `ListAsync`, which replays the
whole log. That is also the cost of every read for a design that re-reads the log on each call.
"Warm" is a `GetAsync` when nothing changed: a stat plus copying one record.

| Records | Changes each | Lines | Log size | Cold replay | Warm read | Write (avg) |
|---|---|---|---|---|---|---|
| 100 | 10 | 1,000 | 1.0 MB | 36 ms | 0.06 ms | 0.28 ms |
| 1,000 | 10 | 10,000 | 10.3 MB | 87–351 ms | 0.04 ms | 0.17 ms |
| 10,000 | 5 | 50,000 | 40.2 MB | 407 ms | 0.01 ms | 0.05 ms |
| 10 | 100 | 1,000 | 5.3 MB | 30 ms | 0.09 ms | 0.20 ms |
| 100 | 100 | 10,000 | 52.8 MB | 373–397 ms | 0.10 ms | 0.12–0.15 ms |
| 10 | 1,000 | 10,000 | 485 MB | 3.1–6.7 s | 1.5–2.0 ms | 1.3 ms |

What the numbers show:

- **A full replay is linear in bytes, at about 130–160 MB/s.** That is mostly JSON
  deserialization of records the replay will overwrite anyway. Re-reading the whole log on every
  call would cost this on *every* read and write, so the incremental catch-up is what keeps the
  strategy usable.
- **With incremental catch-up, reads cost about the same whatever the log size.** They depend on
  the record size (the copy handed out), not on the log. Writes cost about one record
  serialization plus replaying the lines others appended since.
- **What grows is the cold start, and the log itself.** Because each line holds the whole record,
  10 records changed 1,000 times make a 485 MB log, and a new server needs seconds to start.
  An early benchmark with 100 records × 1,000 changes passed 2 GB and broke the first version,
  which read the new bytes into one array. Reads now go in 64 KB chunks. A real system would
  compact (snapshot plus a fresh log), or log only the change instead of the whole record.

## Network drives (phase 6, not yet tested)

What this strategy relies on, and where NFS/SMB may not provide it:

- `flock` across machines. On NFSv4 Linux emulates it with byte-range locks, which may or may not
  work. Over SMB it depends on the mount options.
- A writer reading an up-to-date length after taking the lock. The writer opens the file, which
  triggers NFS close-to-open revalidation, rather than trusting a stat. Readers do use a stat, and
  NFS attribute caching (`acregmin`, about 3 s) can make them see the log as unchanged for a few
  seconds. That gives stale reads, but no lost acknowledged writes.
- Data written by one machine being visible to another once the lock moves between them.
