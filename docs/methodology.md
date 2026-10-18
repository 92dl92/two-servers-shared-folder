# Methodology: can several app servers safely share one data folder?

This document stands on its own. With it and this repository, the experiment can be
repeated in any environment: another home lab, a company network, a cloud. It says what
question is being tested, how, how to run it, and what has been found so far.

It is kept current with the work. When the method, a hypothesis, a setting, an environment
or a result changes, this file changes in the same pull request (see
[Keeping this document current](#11-keeping-this-document-current)).

| | |
|---|---|
| **Last updated** | 2026-10-18, after phase 6 (NFS) and the planning of phase 6b (Windows + SMB) |
| **Rounds completed** | Local disk (phase 5, with a contention sweep); NFS share from one and two machines (phase 6) |
| **Next round** | Phase 6b: Windows servers on an SMB share, realistic load (planned; test machines being set up) |

## Contents

1. [The question](#1-the-question)
2. [Hypotheses](#2-hypotheses)
3. [The system under test](#3-the-system-under-test)
4. [How a run measures safety](#4-how-a-run-measures-safety)
5. [Experimental design](#5-experimental-design)
6. [What an environment needs](#6-what-an-environment-needs)
7. [Procedure](#7-procedure)
8. [Results so far](#8-results-so-far)
9. [Planned rounds](#9-planned-rounds)
10. [Limitations](#10-limitations)
11. [Keeping this document current](#11-keeping-this-document-current)

## 1. The question

A small business app runs on **two or more servers** behind one address. To keep
deployment simple, every server keeps its data as **files in one shared folder** (local
disk, or a network share such as NFS or SMB) instead of a database server.

**Question:** under which storage designs and which environments do the servers lose or
corrupt data when they change the same records, and is any combination safe enough for a
realistic load?

"Safe" has one strict meaning here: **every change the app reported to a user as saved is
still there, at the version it was saved as, and every file can still be read.** Slowness
and refusals ("someone else changed this record, reload") are acceptable. A silent loss is
not.

**The production case this is a proof of concept for:** 2 Windows Server machines, at most
about 10 users, working as a pipeline: one role creates and submits a record, another
responds to it. True simultaneous writes to one record are rare, but records pass from
one server to the other between steps.

## 2. Hypotheses

### Per storage design

Each storage strategy is a hypothesis about what keeps a shared folder safe:

| Strategy | How it stores records | Hypothesis |
|---|---|---|
| **A NaiveFile** | One JSON file; read it whole, change it, write it back | **Unsafe (the control).** Two servers saving at once lose one change; overlapping writes can leave a broken file. |
| **B LockedFile** | As A, but holding an OS file lock while reading and writing | Safe, but every save in every server waits for one lock. |
| **C PerRecordFile** | One file per record; write a temp file, rename it into place; per-record lock and version check | Safe, with no half-written files; servers wait for each other only on the same record. |
| **D AppendLog** | Each change appended as a line to one log; state rebuilt by reading the log | Safe if appends from several processes never interleave; reads slow down as the log grows. |
| **E Sqlite** | SQLite database file (WAL mode) | **Safe on local disk (the known good).** SQLite's own docs warn against network file systems, so it may fail there. |

Design notes for each are in `docs/strategies/<Name>.md`.

### Per environment

| # | Hypothesis | Status |
|---|---|---|
| H1 | On local disk, with all servers on one machine, B–E lose nothing under any contention | **Supported** (phase 5) |
| H2 | On a network share used from one machine, B–E stay safe, only slower | **Supported** for NFSv3 from Linux (phase 6) |
| H3 | On a network share used from several machines, safety depends on each machine's file-sharing client (its caching and locking), not on the strategy alone | **Supported** for NFSv3 with a Linux and a macOS client: every strategy failed (phase 6) |
| H4 | Windows servers on an SMB share are safe at the realistic load, because the SMB server enforces locks and keeps file contents coherent between clients | **Open** (phase 6b) |

## 3. The system under test

A Blazor web app (.NET 10) in `webapp/`. What matters for the experiment:

- **Records** have an id, a version, a status (Draft → Submitted → Approved/Rejected) and a
  history of who did what.
- **Optimistic concurrency:** every change sends the version it started from. If the
  record has changed since, the server answers **409 Conflict** instead of overwriting.
  A 409 is the system working; it is how a correct strategy turns a race into a refusal.
- **Status rules** refuse a change the current status doesn't allow with **422**.
- **Every server** is a separate process, started with
  `--Storage:Strategy <name> --Storage:Path <folder>`. It never trusts a copy kept in
  memory: it reads the files on every request, or (D) first checks that the file hasn't
  changed since it last read it.
- **Storage strategies** implement one interface, `IRegistrationStore`, and all pass one
  shared test suite (`webapp/tests/BlazorApp.Services.Tests/Storage/`), including several
  separate processes writing at once. Strategy A runs that suite reversed: it must be seen
  to lose updates, which proves the suite can detect them.

The version is checked twice: once by the service before it builds the change, and again
by the store at the moment it writes. Only the second check can stop two servers racing,
so it is the one the strategies differ in.

### 3.1 A lost update, step by step

Record R is at version 4. Ann (on server 1) and Bob (on server 2) both change it.

**Without a lock (strategy A):** each server checks the version against its own read, and
nothing stops the other server writing in between.

```text
time  server 1 (Ann)                     server 2 (Bob)                    file holds
t1    read R: version 4                                                     v4
t2                                       read R: version 4                  v4
t3    check: 4 == 4, ok                                                     v4
t4                                       check: 4 == 4, ok                  v4
t5    write v5 (Ann's change)                                               v5 Ann
t6                                       write v5 (Bob's change)            v5 Bob
      → 200 "saved"                      → 200 "saved"
```

Both were told "saved". Only Bob's change is in the file; Ann's is gone, and nobody was
told. That is a **lost update**. If the two writes overlap inside the file itself, the
result can also be a **broken file** that no longer parses.

**With a lock around check and write (B–E):** the second server has to wait, and then sees
the new version.

```text
t3    take lock: ok
t4                                       take lock: busy, waits
t5    read v4, check ok, write v5,
      release lock          → 200
t6                                       take lock: ok
t7                                       read v5, check: 5 != 4
                                         → 409 "someone else changed it"
```

Bob reloads and tries again. Nothing is lost: the race became a 409.

**Where it still goes wrong on a network share:** the lock works, but the read under it
comes from the machine's own cache.

```text
t5    write v5 to the share,
      release lock          → 200
t6                                       take lock: ok
t7                                       read: version 4 (stale cached copy!)
                                         check ok, write v5  → 200
                                         Ann's change is gone
```

This is what the macOS NFS client did in phase 6. So a strategy is only as safe as **the
lock and a fresh read under it**, and the second depends on the file-sharing client, not on
the app.

### 3.2 What the files hold

Every strategy stores the same record (`RegistrationRecordDto` in
`webapp/src/BlazorApp.Shared/Dtos/`) as the same JSON (`RecordJson`: indented, camelCase,
enums as names). What differs is how records are laid out in the folder.

**A and B: one file holding every record as a JSON array.** This is a real
`registrations.json` (strategy B) after a tiny run: one record, created by
`s1c1-registrant`, updated three times by them, then commented on twice by `s1c2-reviewer`:

```json
[
  {
    "id": "3955202f-c538-46f1-8400-27c68626a7dc",
    "version": 6,
    "status": "Draft",
    "createdBy": "s1c1-registrant",
    "createdAt": "2026-10-18T12:04:38.1627161+00:00",
    "updatedBy": "s1c2-reviewer",
    "updatedAt": "2026-10-18T12:04:38.3216608+00:00",
    "data": {
      "firstName": "Stress",
      "lastName": "Record 1",
      "email": "stress1@example.com",
      "phone": null,
      "street": "1 Load Street",
      "city": "Testville",
      "postalCode": "12345",
      "country": "Testland",
      "preferredContact": "Email",
      "subscribeToNewsletter": false,
      "notes": "s1c1-registrant:2"
    },
    "history": [
      { "version": 1, "action": "Create",  "userId": "s1c1-registrant", "at": "2026-10-18T12:04:38.1627161+00:00", "comment": null },
      { "version": 2, "action": "Update",  "userId": "s1c1-registrant", "at": "2026-10-18T12:04:38.2835922+00:00", "comment": null },
      { "version": 3, "action": "Update",  "userId": "s1c1-registrant", "at": "2026-10-18T12:04:38.2918352+00:00", "comment": null },
      { "version": 4, "action": "Update",  "userId": "s1c1-registrant", "at": "2026-10-18T12:04:38.3034612+00:00", "comment": null },
      { "version": 5, "action": "Comment", "userId": "s1c2-reviewer",   "at": "2026-10-18T12:04:38.3156538+00:00", "comment": "s1c2-reviewer:1" },
      { "version": 6, "action": "Comment", "userId": "s1c2-reviewer",   "at": "2026-10-18T12:04:38.3216608+00:00", "comment": "s1c2-reviewer:2" }
    ]
  }
]
```

(The history entries are shown one per line here; the file has one field per line. A writes the same content to `records.json`.) With 12
records the array has 12 such objects, and every save rewrites all of them.

- `version` is the record's current version, and the `history` has exactly one entry per
  version, 1 to N. This is what the checks rely on (section 4).
- `notes` and `comment` hold the stress tool's **markers** (`<user>:<iteration>`): they show
  which acknowledged request wrote the data or the comment (section 4.1). In the real app
  they hold whatever the user typed.

**What `all` holds after `ReadAll()`.** The JSON array is deserialised into C# objects. A
keys them by id; B keeps the list and finds the record's index:

```csharp
// A: Dictionary<Guid, RegistrationRecordDto>     B: List<RegistrationRecordDto>
all = {
  [3955202f-…] = RegistrationRecordDto {
      Id        = 3955202f-…,
      Version   = 6,
      Status    = RecordStatus.Draft,
      CreatedBy = "s1c1-registrant",   UpdatedBy = "s1c2-reviewer",
      Data      = RegistrationDto { FirstName = "Stress", LastName = "Record 1", …,
                                    Notes = "s1c1-registrant:2" },
      History   = [ HistoryEntryDto { Version = 1, Action = Create,  UserId = "s1c1-registrant" },
                    …
                    HistoryEntryDto { Version = 6, Action = Comment, UserId = "s1c2-reviewer",
                                      Comment = "s1c2-reviewer:2" } ]
  },
  [next record id] = …,
}
```

**What a save does to it.** Suppose Ann approves this record from version 6. The service
builds the whole new record, `updated`: version 7, status `Approved`, the same history plus
`{ "version": 7, "action": "Approve", "userId": "ann" }`. The store then:

```csharp
// all[id].Version is 6 and expected is 6: the check passes
all[id] = updated;                  // the whole record object is replaced, history included
WriteAll(all);                      // and the whole array is written back to the file
```

So if another server had saved version 7 in between, from its own older `all`, one of the
two records replaces the other entirely, including its history entry. That is why a lost
update leaves no trace in the file: the history just shows the winner at version 7.

**C: one file per record.** The same object, without the array, in
`records/<id>.json`, next to an empty lock file per record:

```text
data/
├── records/cc4ab0ab-346c-4175-82f3-5fee37885a65.json   { "id": "cc4ab0ab-…", "version": 6, … }
├── locks/cc4ab0ab-346c-4175-82f3-5fee37885a65.lock    (empty; only ever locked)
└── tmp/                                                (new versions are written here first)
```

**D: a log with one line per change.** `registrations.jsonl`; each line holds the operation,
the version it expected, and the whole record after the change (shortened here):

```text
{"op":"Create", "id":"322227e7-…","expected":null,"record":{"id":"322227e7-…","version":1,…},"pid":345675,"at":"2026-10-18T12:04:58.866…"}
{"op":"Replace","id":"322227e7-…","expected":1,   "record":{"id":"322227e7-…","version":2,…},"pid":345675,"at":"…"}
…
{"op":"Replace","id":"322227e7-…","expected":5,   "record":{"id":"322227e7-…","version":6,…},"pid":345675,"at":"…"}
```

Replaying the log rebuilds the same `all` as above: each line replaces the record if its
`expected` matches the version replayed so far. `pid` is the process that wrote the line.

**E: a SQLite database**, `registrations.db`, with one row per record: the id, the version
as its own column (so the `UPDATE … WHERE version = @expected` in section 3.3 can check it)
and the same JSON in a text column.

### 3.3 The five strategies in code

Simplified from the real save paths (`TryReplaceAsync` in
`webapp/src/BlazorApp.Services/Storage/<Strategy>/`). Logging, retries and error handling
are left out.

**A NaiveFile:** no lock. Check and write are separate steps, and the write rewrites the
file in place.

```csharp
var all = ReadAll();                                  // the whole records.json
if (all[id].Version != expected) return Conflict;     // judged from this server's read
all[id] = updated;                                    // another server may write meanwhile
File.WriteAllBytes("records.json", Serialize(all));   // truncate + write: readers can see half a file
return Saved;
```

**B LockedFile:** the same, inside one exclusive lock for the whole folder, and the new file
is swapped in with a rename.

```csharp
// FileShare.None = an exclusive OS lock: flock on Linux/macOS, a share mode on Windows.
// Opening fails while another process holds it; the store retries until it gets it.
using var held = new FileStream("registrations.lock", FileMode.OpenOrCreate,
                                FileAccess.ReadWrite, FileShare.None);
var all = ReadAll();
if (all[i].Version != expected) return Conflict;
all[i] = updated;
WriteAndFlush("registrations.json.tmp", all);                   // complete file, on disk
File.Move("registrations.json.tmp", "registrations.json", overwrite: true); // one step
return Saved;                                                   // lock released here
```

**C PerRecordFile:** one file and one lock per record, so servers only wait for each other
on the same record.

```csharp
using var held = Open($"locks/{id}.lock", FileShare.None);      // this record only
var stored = Read($"records/{id}.json");
if (stored.Version != expected) return Conflict;
var temp = $"tmp/{id}.{Guid.NewGuid():N}.tmp";
WriteAndFlush(temp, updated);
File.Move(temp, $"records/{id}.json", overwrite: true);
return Saved;
```

**D AppendLog:** nothing is rewritten. Every change is one line added to a log, and the
current state is the log replayed.

```csharp
using var held = Open("registrations.lock", FileShare.None);
var state = ReplayToEnd("registrations.jsonl");      // only the lines added since last time
if (state[id].Version != expected) return Conflict;
AppendLine("registrations.jsonl", Serialize(new LogEntry(Replace, id, expected, updated)));
return Saved;
```

**E Sqlite:** the database does the locking. Check and write are one SQL statement.

```sql
BEGIN IMMEDIATE;   -- take SQLite's write lock first; other writers wait (busy_timeout 30 s)
UPDATE records SET version = @version, json = @json
 WHERE id = @id AND version = @expected;   -- 1 row changed: Saved; 0 rows: Conflict
COMMIT;
```

The database runs in WAL mode, which keeps an index in shared memory and so needs every
process on one machine.

**What each one relies on**, and so what an environment has to provide:

| Strategy | Relies on |
|---|---|
| A | Nothing (it is the control: it should fail) |
| B, C | An exclusive lock that every server and every thread respects; a fresh read under the lock; rename replacing the file in one step |
| D | The same lock; the file's true length under the lock, so a line lands at the real end |
| E | SQLite's own locking and shared memory: all servers on one machine |

## 4. How a run measures safety

The **stress tool** (`webapp/tools/BlazorApp.Stress`, full design in
`docs/strategies/StressTool.md`) runs one experiment:

1. **Starts N real server processes** on one data folder, optionally some of them on a
   second machine over ssh. Real processes matter: servers simulated inside one process
   would share memory and locks and hide the very problems being tested.
2. **Creates records**, then lets every client (each acting as its own user) read a
   random record and send a change with the version it just read, all at the same time.
3. **Records every acknowledged change** (any 2xx answer): record, version, action, user,
   a unique marker written into the record, server and correlation id.
4. **Reads everything back** through every server's API and, after the servers stop,
   through the strategy itself from the folder.
5. **Checks** each read-back:

| Check | Fails when |
|---|---|
| **Nothing lost** | An acknowledged change isn't in the history at its version, its marker is at another version, a created record is missing, or the data isn't from the last update |
| **No broken files** | The folder can't be read through the strategy, or a server can't list or return a record |
| **Rules respected** | History isn't versions 1..N, or replaying it finds a change the status or role didn't allow |
| **Unacknowledged entries** | More history entries than acknowledgements can account for (allowing for requests that timed out) |

A run **passes** only when every check passes on every read-back. Exit code 0 = passed,
1 = a check failed, 2 = the run couldn't be done.

**Numbers recorded** (`result.json`, `summary.md`): saves, 409s, 422s, 500s, unanswered
requests, saves per second, save latency (p50/p95/max), the commit (with `-dirty` for
uncommitted changes), machine, times and every setting including the seed.

**Known answer.** Before any other result in a round counts, the tool must tell a broken
strategy from a correct one in that environment: **A must fail** (lost updates or a broken
file) **and E must pass**. If the environment can't run E at all (as on NFS from two
machines), the known answer is checked with all servers on one machine of that
environment first.

### 4.1 Worked example: how a loss is caught

From a real run: strategy A, light, seed 1, on local disk
(`docs/experiments/local-disk/light-NaiveFile-1/`).

**What a client sends and what the tool remembers.** Each client acts as its own user, named
after its server and number: `s2c1-registrant` is client 1 on server 2. It reads a record,
then sends a change with the version it read:

```http
POST /api/registrations/b46f6860-…/submit
X-Correlation-Id: stress-2ed09189…
Content-Type: application/json

{ "expectedVersion": 1 }
```

(Routes are in `webapp/src/BlazorApp.Server/Endpoints/RegistrationEndpoints.cs`.) The
server answers 200 with the record at version 2, so the tool notes one
**acknowledgement**:

| Record | Version | Action | User | Server | Correlation id |
|---|---|---|---|---|---|
| b46f6860… | 2 | Submit | s2c1-registrant | 37761 | stress-2ed09189… |

A client sends one change at a time, so only that request can have written "Submit by
s2c1-registrant at version 2". No timestamps are compared.

**What the check does after the run.** For every acknowledgement it looks up the history
entry at that version in the record read back (simplified from
`webapp/tools/BlazorApp.Stress/DataCheck.cs`):

```csharp
foreach (var ack in acknowledged)
{
    if (!readBack.TryGetValue(ack.RecordId, out var record))
        lost.Add($"{ack}: record missing");
    else if (record.History.FirstOrDefault(h => h.Version == ack.Version) is not { } entry)
        lost.Add($"{ack}: no history entry at that version");
    else if (entry.Action != ack.Action || entry.UserId != ack.UserId)
        lost.Add($"{ack}: version {ack.Version} holds {entry.Action} by {entry.UserId}");
}
```

**What it found.** Seven different changes had all been told "saved as version 2" of record
b46f…, and version 2 held only one of them:

```text
lost update b46f… v2 Submit  by s2c1-registrant via 37761: version 2 holds Comment by s3c5-reviewer
lost update b46f… v2 Comment by s2c2-reviewer   via 37761: version 2 holds Comment by s3c5-reviewer
lost update b46f… v2 Submit  by s2c4-admin      via 37761: version 2 holds Comment by s3c5-reviewer
…
lost update 56dc… v1 Create  by s1c1-registrant via 35137: record missing
```

The last line is a whole record that vanished: a server rewrote `records.json` from a copy
read before that record was created. Every line carries the correlation id, so the request
can be found in that server's log (`logs/server-<port>.jsonl`).

With B–E on the same settings the same check finds nothing, because each of those six
racing saves got a 409 instead.

## 5. Experimental design

### Variables

| Kind | Variable | Values used so far |
|---|---|---|
| Independent | Storage strategy | A, B, C, D, E |
| Independent | Where the folder lives | Local disk; NFSv3 share on a NAS; (planned) SMB share on a NAS and on a Windows server |
| Independent | Machines and their OS | 1 Linux; 2 Linux + 1 macOS; (planned) 2 Windows Server |
| Independent | Load: servers × clients per server, records, changes per client | See the profiles below |
| Controlled | Commit (clean tree), .NET version, seeds 1–3, run time limit | Recorded in every `result.json` |
| Measured | Pass/fail per check, lost changes, broken files, 409/422/500, saves/s, latency | |

**Contention** is clients per record: the lower the number of records for the same
clients, the more often two changes collide on one record.

### Load profiles

| Profile | Options | Purpose |
|---|---|---|
| **light** | `--servers 3 --clients 5 --records 6 --ops 40` | Quick round, about 2.5 clients per record |
| **heavy** | `--servers 3 --clients 8 --records 12 --ops 200` | Sustained load, about 2 clients per record |
| **sweep** | `--servers 3 --clients 8 --ops 200 --records 1,3,12,24,48,96`, plus `--servers 5 --clients 8 --records 20 --ops 200` | Contention from 24 clients on one record to 1 client per 4 records |
| **realistic** (planned, phase 6b) | 2 servers, 10 clients in total (5 per server), creators and responders handing records over with think time, longer run | The production case in section 1 |

### Repetition

Every strategy × profile × environment is run with **seeds 1, 2 and 3**. A seed fixes each
client's choices; the timing of requests still varies, which is the point. A strategy
counts as safe in a setting only if **all** its runs pass. Speeds are compared as the median
of the 3 runs, and only between runs that passed.

## 6. What an environment needs

**Driver machine** (runs the stress tool, the clients, the checks, and some servers):

- .NET 10 SDK; git; this repository at a known commit
- tested so far on Linux (Debian 12); other OSes untested as the driver

**Shared folder:** any folder all servers can reach. For a network round, mount it on every
machine that runs servers and note each machine's path to it.

**Second machine** (only for multi-machine rounds):

- the .NET 10 runtime (ASP.NET Core), in `~/.dotnet` or on the path
- ssh with key login from the driver (the tool uses `BatchMode`)
- the share mounted
- inbound access from the driver to the ports the servers listen on
- today the tool drives a Linux or macOS second machine (it copies with `rsync` and stops
  servers with `kill`); a Windows second machine needs the phase 6b changes

**Record about the environment**, for the results file of each round (copy this list):

- each machine: name, OS and version, CPU count, .NET version
- the share: server or NAS model and OS, protocol and version (NFSv3/v4, SMB 2/3), export
  or share settings (sync/async writes, user mapping)
- each client's mount: mount options as actually in effect (`mount`, `nfsstat -m`,
  `Get-SmbConnection`), attribute and lock caching settings
- the network between them (same switch, Wi-Fi, VPN)
- the commit and that the tree was clean

## 7. Procedure

Run from the repository root on the driver machine.

### 7.1 Build and test

```bash
git status                              # must be clean: a dirty tree marks every run "-dirty"
dotnet build webapp/BlazorApp.slnx      # warnings count as errors
dotnet test  webapp/BlazorApp.slnx      # every test must pass, none skipped
```

The tests include the shared storage suite with several processes on local disk and the
stress tool's own known-answer tests. Don't run experiments on a commit where they fail.

### 7.2 One run

```bash
dotnet run --project webapp/tools/BlazorApp.Stress --no-build -- \
  --strategy PerRecordFile --servers 3 --clients 5 --records 6 --ops 40 --seed 1 \
  --out stress-runs/local-light-PerRecordFile-1
```

Strategy names: `NaiveFile` (A), `LockedFile` (B), `PerRecordFile` (C), `AppendLog` (D),
`Sqlite` (E). The run folder gets `data/`, `logs/`, `result.json` and `summary.md`.

Add, depending on the round:

- **Folder on a share:** `--data <share path on this machine>/runs/<run name>/data` (a new
  folder per run, so no run starts from another's data)
- **Servers on a second machine:** `--remote <user@host> --remote-servers <n>
  --remote-data <the same folder as that machine sees it>`. The host must resolve for
  HTTP too (a name or address, not an ssh config alias).

### 7.3 One round

A round is every strategy × seeds 1–3 for each profile, in one environment, on one commit.
Start with the known answer (A and E), and stop if it doesn't hold.

```bash
round=local; data_root=""        # for a share: data_root=/mnt/share/runs
for size in light heavy; do
  case $size in
    light) opts="--servers 3 --clients 5 --records 6 --ops 40" ;;
    heavy) opts="--servers 3 --clients 8 --records 12 --ops 200" ;;
  esac
  for strategy in NaiveFile Sqlite LockedFile PerRecordFile AppendLog; do
    for seed in 1 2 3; do
      name=$round-$size-$strategy-$seed
      data_opt=${data_root:+--data $data_root/$name/data}
      timeout 30m dotnet run --project webapp/tools/BlazorApp.Stress --no-build -- \
        --strategy $strategy $opts --seed $seed --out stress-runs/$name $data_opt
      echo "$name exit $?"
    done
  done
done
```

The 30-minute limit catches a run that hangs (exit 124 from `timeout`); record it as
"stopped, hung", and check that no server is left running (`pgrep -f "BlazorApp[.]Server[.]dll"`)
on every machine before the next run.

**While a round runs, don't edit or rebuild the repository**: a rebuild swaps the tool
mid-round and an edit marks later runs `-dirty`.

### 7.4 Recording a round

1. Copy each run's `result.json` and `summary.md` (not the data or logs) to
   `docs/experiments/<round>/<run name>/`.
2. Write `docs/experiments/<round>.md` with: the commit, the environment record (section
   6), the settings, the known-answer outcome, a summary table (strategy × passed runs), the
   speed medians, a table of every run, and an explanation of every failure, confirmed
   with a separate small test where possible (see `network-drive.md`, "The client tests").
3. Update [Results so far](#8-results-so-far) and the hypothesis status in section 2 of
   this document.

### 7.5 Reading a run's output

The tool prints the summary and writes it to `summary.md` in the run folder. A passing run
(C, light, seed 1, local disk):

```text
# Stress run: PerRecordFile, 3 server(s): PASSED

- Commit `ee6af83…`, 2026-10-17 19:07:56 UTC, 0.8 s
- Settings: 3 servers × 5 clients, 6 records, 40 changes per client, seed 1

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
| 606    | 225   | 276.8   | 244           | 137              | 0           | 8.4         | 22.9   | 108.3  |

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
| storage        | pass   | 6/6                    | 0               | 0            | 0             | 0              | 0            |
| server 35137   | pass   | 6/6                    | 0               | 0            | 0             | 0              | 0            |
…
```

How to read it:

- **Writes** = every change sent. **Saves** = the ones acknowledged. The rest were refused:
  **409** (someone got there first, the version check working) or **422** (the status no
  longer allows it, e.g. approving a record that is already rejected).
- **244 conflicts is good news.** It means the clients really collided, and every
  collision was refused instead of overwriting. A run with almost no 409s didn't test much.
- **Read back from:** the same data read four ways: through the strategy from the folder,
  and through each server's API. All must pass.
- **Lost updates, missing records, wrong version, unacknowledged:** any number above 0 is a
  failure. The lines below the table give up to 20 examples each, as in section 4.1.
- **No response** counts requests that got no answer (a timeout). They are allowed for when
  counting unacknowledged entries, because such a request may still have been saved.

The same run of A starts `FAILED` and shows the problems in its table:

```text
| Read back from | Result | Records found/expected | Missing records | Lost updates | …
| storage        | FAIL   | 1/6                    | 5               | 14           | …
```

A broken file shows instead as `unreadable` sources: the strategy or the servers can't read
the folder at all ("'}' is invalid after a single JSON value").

The exit code says the same thing for scripts: 0 passed, 1 a check failed, 2 the run
couldn't be done (a server didn't start, a bad option). `result.json` holds every number,
setting and example in machine-readable form.

## 8. Results so far

### Phase 5: local disk, one Linux machine (`docs/experiments/local-disk.md`)

- **Known answer holds:** A failed 6/6 (lost updates in 4, a permanently broken file in 2);
  E passed 6/6.
- **B–E passed every run**: 30 runs at light and heavy, and all 84 runs of the contention
  sweep, including 24 clients on a single record.
- **Speed** depends on the design: B is capped by its single lock (about 36–59 saves/s);
  C scales with the number of records (43 saves/s on one record, 914 on 96); D nearly as
  well (759); E levels off at about 240 saves/s.

### Phase 6: NFSv3 share on a NAS (`docs/experiments/network-drive.md`)

| Strategy | Local disk | NFS, one Linux machine | NFS, Linux + macOS |
|---|---|---|---|
| A NaiveFile | failed 6/6 | failed 6/6 | failed 6/6 |
| B LockedFile | passed 6/6 | passed 6/6 | **failed 6/6** (broken file or lost changes) |
| C PerRecordFile | passed 6/6 | passed 6/6 | **failed** 3/3 light; heavy hung |
| D AppendLog | passed 6/6 | passed 6/6 | **failed 6/6** (130–1,476 lost per run) |
| E Sqlite | passed 6/6 | passed 6/6 | **couldn't start** (WAL needs one machine) |

- From one machine, the share was as safe as local disk, 2.4–11× slower.
- With a Mac as one of the clients, every strategy failed. Separate tests on the share
  found three macOS NFS client behaviours behind it: it reads stale data even under a
  lock; its exclusive lock doesn't exclude other handles in the same process; and it
  reports a busy lock with a different error code than Linux.
- Every loss was silent: the client had been told "saved".

### What this means so far

- A correct strategy is necessary but not sufficient: **the file-sharing client on each
  machine decides whether locks and version checks see the truth.**
- Results from one client OS or protocol don't carry over to another; each production
  combination has to be tested itself.
- Not yet tested: two Linux machines, NFSv4, SMB, Windows clients, macOS with caching off
  (`noac`), and a realistic (low-contention) load.

## 9. Planned rounds

### Phase 6b: Windows servers on an SMB share, realistic load

Tests H4, the production case. Plan in `docs/plan.md` (phase 6b and its Decisions row).

- **Machines:** two Windows Server 2025 machines running the servers; the stress tool and
  clients on the driver. In the reference environment these are evaluation VMs on the Linux
  driver (KVM), installed unattended (Server Core, OpenSSH on, ASP.NET Core 10 runtime).
- **Share layouts:** (1) a NAS share over SMB used by both servers; (2) one server sharing
  its own folder, so one writes to local disk and the other over SMB.
- **Workloads:** light and heavy as before, plus the realistic profile.
- **New check:** stale reads: a read that returns an older version than one already
  acknowledged, especially when a record passes between servers.
- **Tool changes needed first:** `--remote` for Windows machines (copy without `rsync`,
  stop servers without `kill`), the realistic profile and the stale-read check.
- **Done when:** the known answer holds on each layout, every strategy has 3 runs of each
  profile on each layout, and each layout gets a verdict: safe at the realistic load, or
  how often it lost or broke something.

### Later

Depending on 6b: architectures where one authority decides every write (a primary server,
a database server, replicated SQLite such as rqlite), tested with the same tool and checks.

## 10. Limitations

- **Absence of failure is not proof of safety.** A pass means no loss was seen in these
  runs. Rare races, crashes mid-write, network drops and failover are not exercised.
- **The load is artificial.** Clients change records as fast as they can, which raises
  the chance of collisions far above a real office's. The realistic profile (6b) addresses
  this.
- **Timing varies.** The seed fixes choices, not interleaving; numbers vary between runs,
  and speeds depend on the hardware.
- **One share, one network.** Results describe the NAS, OS versions and mount options
  recorded with each round. Another vendor's server or other mount options can behave
  differently; that is why section 6 asks for them to be recorded.
- **Latency includes HTTP** through the real servers, not just the storage.

## 11. Keeping this document current

This file is the single description of the method. It changes in the **same pull
request** as any change to:

- a hypothesis, or its status after a round
- the checks, the numbers recorded, or the pass rule (the stress tool)
- a load profile, the seeds or the repetition rule
- what an environment needs or what must be recorded about it
- the procedure (commands, scripts, options)
- results: a round finished, or an earlier result corrected

Then update the "Last updated" table at the top. `docs/plan.md` stays the source of truth
for the work plan and its decisions; this file describes the method and findings that came
out of it. Where the two disagree, fix whichever is wrong in the same pull request.
