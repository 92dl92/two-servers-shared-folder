# Strategy A: NaiveFile (the control)

Status: **passes the reversed shared suite**, once the suite counts broken reads (see
[The shared-suite change](#the-shared-suite-change)). The first version of this branch was
blocked: the suite crashed on A's broken file before it could count anything.

## How it works

`webapp/src/BlazorApp.Services/Storage/NaiveFile/`, selected with `--Storage:Strategy=NaiveFile`.

- One file, `records.json` in the data folder, holding a JSON array of every record
  (`RecordJson.Options`, so the same format as the other strategies).
- Every operation reads the whole file (`File.ReadAllBytes`), changes it in memory and, for a
  successful write, writes the whole file back in place (`File.WriteAllBytes`: truncate, then
  write).
- **No lock, no temp file, no rename, no cache.** Each call reads from disk, so a single writer
  with a stale copy still gets `Conflict`. The version check and the write are separate steps
  nothing holds together, so two writers that read the same version can both be told `Saved`.
- A missing file means no records. A file that doesn't parse throws `JsonException` (or
  `EndOfStreamException`, see below); nothing is retried or hidden.
- Reads, lists, writes and conflicts are logged at Debug through `StoreLog`, with the file
  path as detail.

## What the suite showed

`NaiveFileStoreTests` derives from `PersistentRegistrationStoreContract` with
`ExpectsLostUpdates => true`. With the shared-suite change, all 16 tests pass in 10 of 10 runs
(the whole storage suite: 58 of 58 in each of 10 runs).

To see what the reversed tests actually find, A was also run once against the normal
(non-reversed) assertions, five times. It failed all three concurrency tests every time, in
one of two ways:

| Test | File survived | File left permanently broken |
|---|---|---|
| Threads, 8 × 40 changes | 138 and 135 of 320 acknowledged saves lost; ~200 broken reads | 3 of 3 records unreadable; thousands of broken reads (8 writers giving up) |
| Processes, 4 × 40 changes | 28 and 39 of 160 lost; ~40 broken reads | 2 of 2 records unreadable; thousands of broken reads |
| 32 writers from the same version | — | 8–23 of the 32 writes found the file broken |

### Before the shared-suite change

| Tests | Result (10 runs) |
|---|---|
| The 13 single-writer tests (save/load, version checks, delete, list, copies, restart, two instances on one folder, logging) | Pass, 10/10 |
| `Many_threads_saving_at_once_lose_nothing` (reversed) | **Fails 10/10**: `JsonException` thrown out of the workload |
| `Many_threads_from_the_same_version_exactly_one_wins` (reversed) | **Fails 10/10**: `JsonException` ("does not contain any JSON tokens": an empty file) |
| `Several_processes_saving_at_once_lose_nothing` (reversed) | **Fails 10/10**: a helper process exits with code 1 on a `JsonException` |

A failed these tests by **breaking the file before the suite got to count lost
updates**. The suite expected a store that loses updates to keep answering; A instead throws,
which the workload, the helper and `MultiProcessRun` treated as a crash.

### Diagnostic run (not committed)

To see what is underneath, a throwaway test ran the threads workload (8 threads × 40 changes
on 3 records, as in the suite) through a wrapper that retried any read error up to 1000 times
in a row. Five runs:

| Run | Read errors seen | End state | Acknowledged saves lost |
|---|---|---|---|
| 0 | 6201 empty file, 92 `EndOfStreamException`, 41 truncated JSON, 8 trailing garbage | file valid | **206 of 320** |
| 1 | 518 empty, 7781 trailing garbage | **file permanently broken**; all 8 threads gave up | — |
| 2 | 10520 empty, 691 trailing garbage, 54 truncated, 16 `EndOfStream` | file valid | **246 of 320** |
| 3 | 1662 empty, 7990 trailing garbage, 149 wrong shape, … | **file permanently broken** | — |
| 4 | 4414 empty, 7222 trailing garbage, … | **file permanently broken** | — |

Three kinds of breakage:

1. **Empty file** (by far the most common): a reader comes in between a writer's truncate and
   its write.
2. **Truncated JSON / `EndOfStreamException`**: a reader comes in part-way through a write, or
   the file shrinks between `File.ReadAllBytes` checking its length and reading it.
3. **Trailing garbage, permanent**: two writers truncate, then both write from offset 0; when
   the first content is longer than the second, its tail stays after the second's closing
   `]`. Nothing ever repairs it, because every writer must read the file successfully before
   it can write. From then on the store is unusable. This happened in 3 of 5 runs.

When the file survived, about two thirds of acknowledged saves were lost — A does lose
updates massively, as hypothesised. It just also breaks the file, which the plan predicts
only for a crash mid-write; in practice concurrent writers alone are enough.

### Spot-check (test rules)

A was temporarily made to hold an exclusive OS lock (`FileShare.None` on `records.json.lock`)
around each operation. All three reversed tests then failed for the right reason —
"Expected lost updates, but every save survived: acks=320 … lost=0", "acks=160 … lost=0" and
"Expected several writers to 'win' from the same version, got 1" — so the reversed assertions
do detect a store that doesn't lose updates. Reverted.

## The shared-suite change

The shared suite was fixed in 4a, so this branch stopped and reported (plan section 7). The owner
approved changing it in its own PR. Making A less naive would have defeated the control, and
retrying reads inside A wouldn't even have worked, because the file gets permanently broken.

What changed, so that **a broken file counts as a failure the suite reports, for every store**:

- `StoreWorkload.IsBrokenData`: a `JsonException` or `EndOfStreamException` from a store means
  the stored data was broken (the only two A produced). Anything else, such as a missing folder,
  still stops the workload with the store's own exception.
- `StoreWorkload.RunAsync` counts such a store call as a broken read and retries the change
  (not acknowledged, so it can't count as lost). After `GiveUpOnBrokenDataAfter` (one second) of
  nothing but broken reads the writer stops early, so a file that stays broken can't hang a test.
  Broken-read retries don't count towards the conflict limit. `WorkloadResult` has a `BrokenReads` count; the helper prints it on its `END` line
  and `MultiProcessRun` reads it into `HelperRun.BrokenReads`.
- `WorkloadReport` has `BrokenReads`, and `NothingLost` is false when it isn't zero. The contract's
  read-back (`CheckAsync`) counts a read that fails on broken data as one more and leaves that
  record missing, instead of throwing.
- `Many_threads_from_the_same_version_exactly_one_wins` catches broken writes too: for the control
  it passes on several winners **or** a broken write; for every other store a broken write fails it.
  Since phase 6b the control gets up to 5 rounds, each on a fresh folder, and must show this in
  at least one: on a busy machine its 32 threads sometimes ran one after another and A happened
  to lose nothing (owner's approval, plan Decisions).
- `Several_processes_saving_at_once_lose_nothing` skips its final "everything still loads" check
  for the control only (A may or may not leave the file broken; the report has already failed it).

For B–E this is stricter, not weaker: before, a broken read crashed the test with a stack trace;
now it is counted, and any count above zero fails `NothingLost`.

Known answers, in `MultiProcessHelperTests`, with a new test-only `BrokenReadsTestStore`
(every n-th read throws `JsonException`):

- One writer, every third read broken, 20 changes: exactly 20 acks and 9 broken reads.
- Every read broken: the writer gives up after about a second, without throwing.
- A read that throws `DirectoryNotFoundException` is not broken data: the workload throws it.
- One change meeting 5,100 conflicts, each followed by a broken read, is still saved (broken
  reads don't count towards the 10,000-conflict limit).
- Two helper processes on `TestBrokenReads`: both exit normally, each reports broken reads, and
  the report is not "nothing lost" although no marker was lost.

## Surprising

- The breakage is not rare: an empty-file read shows up within the first few milliseconds in
  every run, and permanent corruption in more than half the runs. The control fails "loudly"
  far more often than it fails "quietly".
- On Linux, .NET's `FileShare.Read` in `File.WriteAllBytes`/`File.ReadAllBytes` gives no
  protection between writers at all (both take a shared advisory lock), so two writers in the
  same or different processes overlap freely.
