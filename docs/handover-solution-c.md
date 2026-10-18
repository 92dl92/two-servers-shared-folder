# Handover: implement solution C (one file per record) on two Windows servers and an SMB share

**For:** an agent (or developer) starting in a new environment, whose job is to turn this
proposal into an **implementation plan** for a real application, and then carry it out.
**From:** a proof of concept (the code in this repository, called "ClaudeTest" in its docs).
Commit ids quoted below are from the original, private repository: they identify which code
produced which result, and are not in this repository's history.
**Status:** 2026-10-18, after phase 6b. The Windows results in section 3 are final and written
up in `docs/experiments/windows-smb.md`, with every run's `result.json` and `summary.md` in
`docs/experiments/windows-smb/`. All code this document cites is on `main`.

## 0. How to use this document

1. Read it fully once. Sections 4–6 are the architecture; sections 7–9 say what is still
   unknown and what the plan must cover; section 10 says what test harness exists for the
   target environment, what must be built first, and how to run it. Appendix A is the ssh
   path phase 6b used, for a lab only.
2. Look at the reference implementation it points to (section 11). It is a toy app: the
   records are made-up registrations. **Copy the mechanism, not the data model.**
3. Before planning, ask the owner the open questions in section 8. Do not guess them.
4. Produce a plan in the shape of section 12, and get it approved before writing code.

Words used here: a **record** is one business object that users create and change over
time (in the toy app, a registration). A **save** is any change to one record. An
**acknowledged** save is one the user was told succeeded.

## 1. The problem

Two application servers serve the same users. To keep operations simple there is **no
database server**: the data lives as files in **one shared folder on an SMB share** that
both servers use. The danger is that two servers change the same record at about the same
time and one change silently disappears (a lost update), or a file is left half-written.

**Solution C:** one file per record, an exclusive lock file per record, a version number in
every record, and every save done as "lock, re-read, check version, write a temp file,
rename it into place, unlock". A save that started from an old version is refused (HTTP 409)
and the user reloads; it never overwrites.

## 2. The production environment (as known on 2026-10-18)

| Item | Value |
|---|---|
| App servers | 2 × Windows Server, both running the app, behind one address |
| Shared storage | An SMB share both servers mount. **Where it is hosted is still open** (a Windows file server, one of the two app servers, or a NAS) |
| Users | At most about 10 at a time |
| Workflow | A pipeline: one role creates and submits a record, another responds (approves, rejects, comments). Two people changing the same record at the same moment is rare; a record passing from one server to the other between steps is normal |
| Volume | About 2,000 new records a year, each changed 5–10 times, kept 3 years: **about 6,000 records** on the share |
| Hard requirement | **Each record has its own file(s) on the share.** One file holding every record, or one shared log, is ruled out |
| Load balancer | In front of the two servers; **routing unknown** (random per request, or sticky per user). Test both (section 9) |
| Access to the servers | **No ssh; PowerShell remoting assumed unavailable.** Files can be uploaded and scripts run by hand on each server |
| Firewalls | **Closed, enterprise-managed.** Nothing may change firewall rules. Every port is supplied by the owner (one per server, open from the owner's PC; to be confirmed on the day, 10.3) |
| .NET on the servers | **.NET 8** (perhaps 10). The proof of concept targets .NET 10, so anything deployed for testing is published **self-contained** |
| Owner's PC | Windows; can open the share directly; the owner can clean the share's test folder |
| How the real app is hosted | **Unknown** (IIS, a Windows service, plain Kestrel): it decides which account opens the share (section 5, question 8) |
| Fallback if the share can't be made safe | One server owns the data (the other forwards writes to it), or SQL Server Express |

## 3. Why C: the evidence

The proof of concept ran five storage designs under a stress tool that starts real server
processes on one folder, makes clients collide on the same records, and then checks that
every acknowledged save is still there at its version and every file can be read. The full
method is in `docs/methodology.md`.

| Design | Local disk, 1 Linux machine | NFS share, 1 Linux machine | NFS share, Linux + macOS | SMB, 2 Windows Server VMs |
|---|---|---|---|---|
| A one file, no lock (control) | failed 6/6 | failed 6/6 | failed 6/6 | failed 9/9 |
| B one file + lock | passed | passed | **failed** | passed 9/9 (slowest) |
| **C one file per record** | **passed** | **passed** | **failed** | **passed 9/9**, but with save errors (HTTP 500) until the rename retry; with it, passed every later run (12 + 18 + 12 failure tests) |
| D one shared log | passed | passed | **failed** | passed 9/9 |
| E SQLite on the share | passed | passed | couldn't start | **failed 9/9, lost data** |

**The first Windows round** (`docs/experiments/windows-smb.md`, rounds 1–2; light and heavy on
commit `c71df76`, pipeline on `fe53fea`):

- Two Windows Server 2025 VMs (Server Core). **Share layout tested: one VM shares its own
  folder** (`C:\share`); its server writes to local disk, the other server writes over SMB
  (`\\<vm1>\share`). The stress tool and its checks ran on a Linux machine.
- Workloads, 3 seeds each: **light** (2 servers × 5 clients, 6 records), **heavy**
  (2 × 12 clients, 12 records, 200 changes each), and **pipeline**: 2 servers × 5 clients
  (4 creators, 4 responders, 2 admins), 1 s think time between steps, 120 changes per client,
  about 730 saves per run over about 2 minutes, about 160 reads per run that followed a save
  made through the other server.
- **C lost nothing and broke no file in any run**, and had 0 stale reads (a read returning an
  older version than one already acknowledged). In the pipeline runs: 19–38 conflicts refused
  with 409, save p95 66–76 ms; heavy: about 65–71 saves/s, p95 150–176 ms. **But some saves
  failed with a server error (HTTP 500):** over SMB 9–19 per light run, 111–141 per heavy run
  and 1–4 per pipeline run (3–5 per light run with both servers on one VM). See the rename
  finding below.
- A failed every run, including the pipeline (and showed 2–4 stale reads per pipeline run),
  so the checks do detect problems in this setup.
- E (SQLite) **silently lost data** across the two machines: 34–44 saves per light run,
  67–386 per heavy run and 376–715 per pipeline run (read from the folder after the servers
  stopped), and missing records. Its WAL mode needs
  every process on one machine, as SQLite's docs say. Never put SQLite (or another embedded
  database) on the share for two servers.
- With **both servers on one VM** (same share folder), B–E all passed and A failed: the
  known answer for this environment.

**What phase 6b found after that** (commits `1e1cb64` and `22febeb`):

- **Windows refuses to rename over an open file.** `File.Move(temp, record, overwrite:
  true)` fails with "access denied" while any other handle has the record file open:
  another server reading it, the SMB client holding a handle for caching, an antivirus scan.
  Opening readers with `FileShare.Delete` does not prevent it. That was the cause of C's
  server errors above. **Nothing was lost** (each was a visible error, not a silent loss), but
  users would have seen failed saves.
- **Fix: retry the rename** (`Storage/FileReplace.cs`): on Windows only, on access denied or a
  sharing/lock violation, try again every 5–50 ms for up to **3 s**, still holding the
  record's lock, and log a warning (`StoreReplaceRetried`) for every save that needed it.
  With the retry, C's light and heavy runs passed 12/12 on one VM and on two; heavy over SMB
  went from 111–141 errors per run to 0–1.
- **SMB leasing on the share** (`Set-SmbShare -LeasingMode`), C with the retry, 6 heavy and 3
  pipeline runs each, all 18 passed with 0 stale reads:
  - **leasing on (the default):** 2 of 6 heavy runs **froze for 24–34 s**: one rename waited
    35.5 s, the SMB lease-break timeout, and no server saved meanwhile;
  - **leasing off:** no freeze, but about 3× the rename retries, half the saves per second
    and double the p95 under heavy load, and a few saves per heavy run still gave up after
    1 s (hence the 3 s limit);
  - under the **pipeline load both were clean** (no errors, no pause over 1.4 s).

  The owner chose **leasing off** as the production candidate; the failure tests below ran
  that way.
- **Failure tests** (C, pipeline load, leasing off, seeds 1–3 each, with the stress tool's
  `--fault` option): **F1** server 2 killed 40 s into the run; **F2** server 1 (on the VM that
  hosts the share) killed at 40 s; **F3** server 2's network cut for 10 s; **F4** the same for
  60 s. **All 12 passed**: nothing lost, no broken file, 0 stale reads, and afterwards every
  record could still be changed through every surviving server (the "lock check": 0 stuck).
  Users would still have seen a few failures: 2–6 saves per run failed with a server error
  during the 60 s cut (F4), 1 in one 10 s-cut run, and 1–2 requests got no answer when a
  server was killed (F1). In two of the three 10 s-cut runs one save took 14–15.5 s to
  complete.
- **Volume test** (C, pipeline load, leasing off, the share **prefilled with 6,000 records**,
  seeds 1–3). The first trial found C's list view reading every record file: about 0.6 s on
  the server with the share on its own disk, **more than 60 s over SMB**, so every list timed
  out. The owner chose a **list cache per server** (commit `b3d3ed7`, see 6.2). With it, **all
  3 runs passed**: nothing lost, 0 stale reads, save p95 71–154 ms, list p50 64–99 ms and p95
  about 240 ms. But the **first list after a server starts still reads every file: 46–53 s**,
  and it slows other requests meanwhile (one read took 18.8 s). The owner accepted that for
  now.
- **One-hour soak** (commit `7d9ce59`, C, pipeline load, leasing off, 6,000 prefilled
  records, list views on): **passed**. About 59 minutes, 19,828 saves, **nothing lost, no
  broken file, 0 server errors, 0 stale reads** (3,619 reads followed a save through the
  other server), 493 conflicts refused with 409; save p50 20.5 ms, p95 63 ms; list p50
  124 ms, p95 304 ms. The longest pause with no save was 6.4 s, 3.4 s into the run, while
  the list cache was filling. The slowest save took 27.1 s and the slowest list 58.8 s; the
  results don't record when, but the 58.8 s matches a first list reading all 6,000 files.

**Why C fails on NFS with a Mac but passes on Windows SMB:** C needs two things from the
file-sharing client: an exclusive lock that every server *and every thread* respects, and a
**fresh read** under that lock. The macOS NFS client gave neither (stale cached reads under a
lock; locks not exclusive within one process). The SMB server enforces share modes for every
handle, and keeps file contents coherent between machines (through leases when they are on,
and by not letting clients cache when they are off).

**What has not been tested** (the plan must cover these, section 9):

- a **real network between real machines**: both Windows servers were VMs on one host,
  talking over its virtual network, so latency, packet loss and a real switch were never in
  play;
- the share **hosted on a NAS** or on a separate Windows file server (only "one app server
  shares its folder" was tested);
- 6,000 records **beyond the pipeline load** (the volume test ran the pipeline only), and the
  first list after a start at production server sizes (the test VMs had 2 processors);
- the **share host rebooting** or the share itself going away (F1–F4 killed app servers and
  cut one server's network; the share host stayed up);
- **routing like a load balancer**: in every run each simulated user stayed on one server;
- antivirus, backup software or Windows Search touching the files;
- runs longer than one hour (days of use, cache growth over time);
- leasing settings on a share host other than Windows Server 2025.

## 4. The architecture

### 4.1 Folder layout

```text
<share>\<app data root>\
├── records\<id>.json              one complete file per record; replaced only by rename
├── locks\<id>.lock                 one empty file per record; only ever opened exclusively; never deleted
└── tmp\<id>.<random>.tmp           a save in progress; renamed into records\ when complete
```

- `<id>` is a GUID generated by the server that creates the record. File names never come
  from user input.
- `records\`, `locks\` and `tmp\` must be **on the same share and volume**, so the rename is
  a single atomic operation.
- Nothing else lives in `records\`; anything there that isn't `<guid>.json` is ignored.

### 4.2 The record file

One JSON document per record, holding at least:

```json
{
  "id": "3955202f-c538-46f1-8400-27c68626a7dc",
  "version": 6,
  "status": "Submitted",
  "createdBy": "user-a", "createdAt": "2026-10-18T12:04:38+00:00",
  "updatedBy": "user-b", "updatedAt": "2026-10-18T12:09:02+00:00",
  "data": { "...": "the business fields of the real application" },
  "history": [
    { "version": 1, "action": "Create", "userId": "user-a", "at": "…", "comment": null },
    { "version": 2, "action": "Submit", "userId": "user-a", "at": "…", "comment": null }
  ]
}
```

- `version` starts at 1 and goes up by exactly 1 on every save. It is the concurrency
  token: **without it, C does not work.**
- `history` (one entry per version) is optional for the mechanism but strongly
  recommended: it is the audit trail, and it is how a test can prove that no acknowledged
  save was lost.
- `data` is where the real application's fields go. In the toy app they are a person's
  name and address; in production they will be different. Nothing in the mechanism
  depends on them.
- If a record has attachments, store them as separate files named after the record (for
  example `attachments\<id>\…`) and write them **before** the record file that refers to
  them; the record file's rename is what makes the change visible.

### 4.3 The save protocol (update, and every status change)

The service layer, before touching storage:

1. Load the record, check permissions and business rules.
2. If `record.Version != expectedVersion` (the version the user's page was showing),
   refuse with **409** straight away.
3. Build the complete new record: `version + 1`, new `updatedBy/updatedAt`, history plus
   one entry, changed data.

Then the store, which is what makes it safe across servers:

```csharp
// Simplified from PerRecordFileStore.TryReplaceAsync (reference implementation, section 11).
using var held = await LockAsync(id);                 // 1. exclusive lock on locks\<id>.lock
RemoveOrphanedTempFiles(id);                          // 2. leftovers of a crashed save of this record
var stored = Read($"records\\{id}.json");             // 3. re-read the file NOW, under the lock
if (stored is null) return NotFound;
if (stored.Version != expectedVersion) return Conflict;   // 4. someone saved since: refuse (409)
var temp = $"tmp\\{id}.{Guid.NewGuid():N}.tmp";
using (var s = new FileStream(temp, FileMode.CreateNew, FileAccess.Write, FileShare.None))
{
    s.Write(Serialize(updated));                      // 5. write the complete new file
    s.Flush(flushToDisk: true);                       //    and make it durable before anyone can see it
}
FileReplace.Move(temp, $"records\\{id}.json", logger); // 6. replace in one step; on Windows,
                                                       //    retried for up to 3 s while the old
                                                       //    file is open elsewhere (4.5, rule 8)
return Saved;                                          // 7. lock released by Dispose
```

**Delete** checks the version under the lock and deletes the record file; its lock file
stays.

### 4.3.1 Create

The service builds the complete first version; the store writes it under the new record's
own lock. Simplified from `RegistrationService.CreateCoreAsync` and
`PerRecordFileStore.TryCreateAsync`:

```csharp
// Service: validate, then build version 1 with its first history entry.
var now = timeProvider.GetUtcNow();
var record = new RegistrationRecordDto(
    Id: Guid.NewGuid(),                       // the server makes the id; never from user input
    Version: 1,
    Status: RecordStatus.Draft,
    CreatedBy: user.Id, CreatedAt: now,
    UpdatedBy: user.Id, UpdatedAt: now,
    Data: form,                               // the real application's fields go here
    History: [new HistoryEntryDto(1, RecordAction.Create, user.Id, now)]);
if (await store.TryCreateAsync(record) != WriteResult.Saved)
    throw new InvalidOperationException();    // the id was taken: should never happen

// Store: the same steps as a save, except the check is "must not exist yet".
using var held = await LockAsync(record.Id);                    // locks\<id>.lock, created now
RemoveOrphanedTempFiles(record.Id);
if (File.Exists($"records\\{record.Id}.json")) return Conflict; // under the lock, so it's fresh
var temp = $"tmp\\{record.Id}.{Guid.NewGuid():N}.tmp";
using (var s = new FileStream(temp, FileMode.CreateNew, FileAccess.Write, FileShare.None))
{
    s.Write(Serialize(record));
    s.Flush(flushToDisk: true);
}
FileReplace.Move(temp, $"records\\{record.Id}.json", logger);   // the record appears, complete
return Saved;
```

With a fresh GUID the "already exists" branch is practically never taken; it is there so that
two creates of the same id can never both succeed.

### 4.3.2 Read one record, and list records

Reads take **no lock**. A reader sees either the old or the new file, never a mix, because
files only appear by rename. Simplified from `PerRecordFileStore.GetAsync`, `TryReadAll` and
`ListAsync`:

```csharp
// One record: open, read the whole file, close at once.
RegistrationRecordDto? Get(Guid id)
{
    try
    {
        using var stream = new FileStream(
            $"records\\{id}.json", FileMode.Open, FileAccess.Read,
            FileShare.ReadWrite | FileShare.Delete);  // let writers replace or delete it meanwhile
        using var buffer = new MemoryStream();
        stream.CopyTo(buffer);                        // the whole file, then close the handle
        return Deserialize(buffer.ToArray());
    }
    catch (FileNotFoundException)
    {
        return null;                                  // 404 (see the note on "not found" below)
    }
}

// All records: list the folder, read each file the same way.
IReadOnlyList<RegistrationRecordDto> List()
{
    var all = new List<RegistrationRecordDto>();
    foreach (var file in Directory.EnumerateFiles("records"))
    {
        if (!IsRecordFile(file)) continue;            // only <guid>.json; skip anything else
        var record = Get(IdFromFileName(file));
        if (record is not null) all.Add(record);      // null: deleted since the folder was listed
    }
    return all;
}
```

Things to know about reads on Windows and SMB:

- **Keep handles short.** Read the whole file into memory and close it before parsing.
  While any handle has a record file open, a save to that record can't rename over it and
  has to retry (step 6 above). `FileShare.ReadWrite | FileShare.Delete` is needed but **not
  enough** to prevent this: phase 6b saw it as save errors before the retry was added.
- **"Not found" right after a create.** The Windows SMB client remembers "file not found" for
  about 5 s. A record created through server 1 may briefly be "not found" through server 2
  (section 5 says how to handle it: one retry, or `FileNotFoundCacheLifetime` set to 0).
- **Listings can lag.** The folder listing may be up to about 10 s old on the other server
  (`DirectoryCacheLifetime`), so a new record can be missing from a list for a few seconds.
  A record file that exists is read with its current contents (the "not found" case above
  is the exception). Neither lag was seen in any C run of phase 6b (0 stale reads, no
  "not found" after a create), so both are known SMB behaviour rather than observed
  problems; keep the handling anyway.
- **Listing every file is too slow at 6,000 records over SMB** (over 60 s in phase 6b). The
  reference now keeps a list cache per server and re-reads only changed files; see 6.2.
- **Never decide a write from a read made outside the lock.** A read is for showing; the
  version check in a save always re-reads under the lock (4.5, rule 2).

### 4.4 The lock

```csharp
// Opening with FileShare.None is the lock: on Windows the SMB server refuses any other open
// of the file, from any machine or any handle in this process, until this handle is closed.
new FileStream(lockPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
```

- If the open fails with a **sharing violation (HRESULT 0x80070020)** or **lock violation
  (0x80070021)**, another save holds it: wait briefly and retry. Any other error is real and
  must be thrown.
- The lock is released when the handle is closed, and by the SMB server if the process dies
  or the connection drops. Phase 6b's failure tests support this: after a server was killed,
  and after a 10 s and a 60 s network cut, every record could still be changed (0 stuck).
  Verify it again on the production share host.
- **Never delete lock files.** A waiter could open the old file while another writer creates
  a new one, and both would think they hold the lock.

### 4.5 Rules the implementation must keep (invariants)

1. **Every write holds the record's lock from re-read to rename.** No write path skips it.
2. **The version check uses a fresh read made under the lock**, never a copy in memory, a
   cache, or a read made before the lock was taken.
3. **No record data is cached across requests** for anything that decides a write. (A cache
   for list views is allowed only if every write still follows rules 1–2; see 6.2.)
4. **Files become visible only by rename**; a record file is never written in place.
5. **One record, one file.** Nothing that every save has to rewrite (no global index file
   updated on every save, no shared counter file).
6. **A refused save is a 409 shown to the user** ("someone else changed this record, reload"),
   never an automatic retry that rebuilds the change on top of the newer version without
   the user seeing it.
7. **Acknowledge only after the rename succeeded.**
8. **Retry a refused rename, briefly, under the lock.** On Windows, treat access denied and
   sharing/lock violations on the rename as "file in use" and try again (every 5–50 ms, up to
   3 s, as `FileReplace.cs` does); after that, fail the save with an error, never acknowledge
   it. Log every save that needed a retry, so a rising count is visible in production.

**Known durability gap** (from `docs/strategies/PerRecordFile.md`): the temp file is flushed
to disk, but the `records\` folder is not flushed after the rename. After a power cut on the
share host, a save already acknowledged could in principle come back as the previous
version. A process crash doesn't cause this. The plan should decide whether that is
acceptable (a UPS and a share host with a write-through or battery-backed cache make it
unlikely), and list it as a risk.

## 5. Windows and SMB specifics

What the plan has to settle, check or configure. Items marked *(tested)* were exercised in
the Windows round; the rest are known SMB behaviour that **must be verified on the
production share**.

- **Lock and rename semantics** *(tested, share on a Windows Server VM)*: `FileShare.None`
  excludes every other handle; `File.Move(…, overwrite: true)` replaces the file atomically,
  but is **refused while the old file is open anywhere**, so it needs the retry of 4.5,
  rule 8.
- **SMB leasing on the share** *(tested, phase 6b)*. With leasing on (the default), a rename can
  wait for the SMB lease-break timeout (35 s): 2 of 6 heavy runs froze for 24–34 s, and no
  save went through meanwhile. With leasing off (`Set-SmbShare -Name <share> -LeasingMode
  None` on the share host), no freezes, at the cost of more rename retries and about half the
  throughput under heavy load; under the realistic load both were clean. **Production
  candidate: leasing off**, chosen by the owner. Verify on the production share host (the
  setting exists on Windows Server shares; a NAS has its own equivalent, if any), and
  remember that it affects every client of that share, not just this app.
- **Client-side metadata caches.** The Windows SMB client caches, by default for a few
  seconds: directory listings (`DirectoryCacheLifetime`, 10 s), file attributes
  (`FileInfoCacheLifetime`, 10 s) and "file not found" (`FileNotFoundCacheLifetime`, 5 s),
  under `HKLM\System\CurrentControlSet\Services\LanmanWorkstation\Parameters`. File
  **contents** read through an open handle are coherent; **listings and existence checks
  may lag**. Consequences:
  - a record created on server 1 may be missing from a list on server 2 for a few seconds
    (acceptable for a list view; say so to the owner);
  - never decide a write from `File.Exists` or a directory listing **outside** the lock;
  - opening a record that was just created on the other server could briefly say "not
    found": the plan should decide between retrying once or setting
    `FileNotFoundCacheLifetime` to 0 on both app servers.
- **Offline Files / client-side caching must be off** for the share (share setting "No files
  or programs from the share are available offline"). Cached offline copies would break
  rule 2.
- **No replication between two copies** of the folder (DFS Replication, sync tools). Both
  servers must use **one** copy; replication is a second writer with its own view.
- **Antivirus and indexing.** A scanner opening `records\*.json` holds the file open and makes
  the rename wait (rule 8). Exclude the data folder from real-time scanning and Windows Search
  if policy allows; either way, test with the production antivirus on, and watch the retry
  warnings.
- **Backup.** Back up from a VSS snapshot of the share (consistent per file). Restoring a
  single record file is safe only while the app is stopped or under that record's lock.
- **Hosting and identity.** How the real app is hosted (IIS, a Windows service, Kestrel)
  decides which account opens the share: an IIS application pool or a service needs a
  **domain account** with access to the UNC path (built-in accounts like
  `ApplicationPoolIdentity` reach the network as the machine account, if at all), no mapped
  drives, and for IIS usually `loadUserProfile` on. Settle it with the owner (question 8).
- **Permissions.** Both app servers' service accounts need Modify on the data root. Nobody
  else should write there; a person editing a file by hand bypasses the lock.
- **SMB version.** Use SMB 3.x; check `Get-SmbConnection` on both servers (dialect) and
  that SMB signing/encryption settings match policy.
- **Paths.** Use the UNC path (`\\fileserver\share\appdata`), not a mapped drive letter
  (drive letters are per logon session and are not visible to a service account).
- **If one app server hosts the share**, that server writes to local disk and the other over
  SMB. This was the tested layout, but it makes the host a single point of failure for both
  servers. A NAS or a separate file server was not tested and must be (section 9).

## 6. Things the toy app did not have to solve

### 6.1 Waiting for a lock without limit

The reference `LockAsync` retries forever. On NFS with a Mac this hung every server. In
production, wait for a bounded time, then fail the save with a clear "busy, try again" (HTTP
503), **without writing**. Log every long wait. Choose the limit with the rename retry in
mind: a save holds the record's lock for up to 3 s of rename retries, and with leasing on a
rename waited 35 s, so a limit of about 10 s fits leasing off but would turn a lease-break
freeze into 503s. Measured with leasing off: the slowest save took 1–1.9 s under heavy load,
but **14–15.5 s during a 10 s network cut** (F3), so a 10 s limit would also have refused saves
then. Pick the limit, and what users see, with these numbers in front of the owner.

### 6.2 Listing 6,000 records

The original `ListAsync` opened and parsed every file in `records\` on every call. Phase 6b's
volume test measured it with 6,000 records: about 0.6 s on local disk, **more than 60 s over
SMB**. Of three options (an archive folder for closed records, a list cache per server, an
index file), the owner chose the **list cache** (commit `b3d3ed7`, `PerRecordFileStore`):

```csharp
// Simplified from PerRecordFileStore.ListAsync with the cache (phase 6b).
// One folder listing; creation time, write time and size come with it, no file is opened.
foreach (var entry in new DirectoryInfo("records").EnumerateFiles("*.json"))
{
    if (cache.TryGetValue(entry.Name, out var cached)
        && cached.SameAs(entry)                          // same creation time, write time, size
        && cached.ReadAt - entry.LastWriteTimeUtc > TimeSpan.FromSeconds(3))
        keep(cached.Record);                             // unchanged: use the copy in memory
    else
        toRead.Add(entry);                               // new or changed: read the file
}
// read toRead 16 at a time; drop cache entries whose file is gone; lists run one at a time,
// so a list that waited finds the records the previous one just read.
```

- A copy read within 3 s of its file's last write is read again: NTFS can keep a
  renamed-over file's creation time, and two saves in one clock tick with the same size
  would otherwise look unchanged.
- **Only the list uses the cache.** Opening a record and every save still read the record's
  own file (4.5, rules 2–3).
- Measured with it: list p50 64–99 ms, p95 about 240 ms over SMB. **The first list after a
  server starts reads everything (46–53 s over SMB on 2-processor VMs) and slows other
  requests meanwhile.** The owner accepted that for now; the plan for the real application
  should decide whether to warm the cache at start-up, before the server takes traffic.

The real application can choose differently (a folder per year or an `open\`/`closed\` split
to scan less, or a disposable search index), but whatever it chooses, a record view and every
save must still read the record's own file.

### 6.3 Real data and real rules

The toy has three roles, a Draft → Submitted → Approved/Rejected workflow and comments. The
real application's fields, roles, statuses and validation replace these. The mechanism needs
only: an id, a version, and a complete new record built from the stored one plus the
user's change.

### 6.4 Crashes and recovery

The reference handles a crash between writing the temp file and renaming it (the next save
of that record deletes the orphan). It does not handle disk-full, a failing share, or an
app server losing the share for minutes. The plan should define what users see then, and a
health check that refuses writes when the share is unreachable.

### 6.5 What to log and watch in production

The toy logs these at Debug; production should log them at a level someone sees, and
alert on the starred ones:

- rename retries (`StoreReplaceRetried`): count per hour, and every **"failed"** outcome (*);
- lock waits: time and attempts per save, and every save refused because the wait limit
  passed (6.1) (*);
- 409 conflicts per hour (a sudden rise means users really are colliding, or a client
  sends stale versions);
- a health check that the share is reachable and writable from each server (*);
- orphaned temp files removed (a sign of crashes mid-save).

Every log line carries the record id and version, never the record's contents.

## 7. Mapping the proof of concept to the real application

| Proof of concept | Real application |
|---|---|
| `RegistrationRecordDto` (`Id`, `Version`, `Status`, `Data`, `History`) | The real record type: keep `Id`, `Version`, `History` (or an audit trail); replace `Data` and the statuses |
| `IRegistrationStore` (`GetAsync`, `ListAsync`, `TryCreateAsync`, `TryReplaceAsync(updated, expectedVersion)`, `TryDeleteAsync`) returning `Saved`/`Conflict`/`NotFound` | A storage interface of the same shape for the real record type; one implementation, the per-record-file store |
| `RegistrationService`: rules, first version check, builds the new record | The real business service; keep the "check, build complete new record, call `TryReplaceAsync`, map `Conflict` to 409" pattern |
| `PerRecordFileStore` | Port as is, then add section 6 (bounded lock wait, list strategy, health check) |
| The shared contract tests and multi-process tests | Port and run against the real store (section 9) |
| Stress tool | Reuse it against a test copy of the real app, or write an equivalent for the real API (section 9) |

## 8. Open questions for the owner

Ask these before planning; the answers change the design.

1. Where will the share be hosted: a Windows file server, one of the two app servers, or a
   NAS (which model and OS)?
2. What is in a record: size, attachments, and do any fields change independently enough to
   deserve their own files?
3. Which list views and searches do users need, and how fresh must they be (is a few seconds'
   delay after a save on the other server acceptable)?
4. How does the load balancer route users to the two servers (random per request, sticky
   per user), and what should happen if one server is down? Until this is known, test both
   routings (section 9).
5. Backup, retention (3 years?) and archiving: may old records move to an archive folder?
6. What must users see when the share is unreachable or a save is refused as busy?
7. Is the real application built on .NET? If not, the lock and rename calls need their
   equivalents in that stack, checked against SMB behaviour the same way.
8. How is the real application hosted (IIS, a Windows service, plain Kestrel), and under
   which account? That account opens the share (section 5, "Hosting and identity").

## 9. What the plan must verify, and how

1. **The contract suite** from `webapp/tests/BlazorApp.Services.Tests/Storage/`
   (`RegistrationStoreContract`, `PersistentRegistrationStoreContract`, and the
   multi-process tests with `BlazorApp.StoreHelper`), ported to the real store and run on
   local disk **and on the production-like share** from both servers. The servers have only
   the .NET 8 runtime and no ssh: publish the test project and `BlazorApp.StoreHelper`
   **self-contained** (or ship a test runner that is), upload them, and run them by hand on
   each server.
2. **A known-answer check first:** a deliberately broken store (like strategy A: no lock)
   must be seen to fail the same tests on that share, or the tests prove nothing there.
3. **The stress tool's checks** (nothing lost, no broken files, rules respected, stale reads)
   against 2 real servers on the real share layout, with:
   - the pipeline workload at production load (about 10 users, think time),
   - **about 6,000 records** in the folder,
   - a heavy run as the worst case,
   - **both routings**: random per request and sticky per user (until the load balancer's is
     known),
   - 3 seeds each, and at least one run of an hour or more.
4. **Failure tests**, repeating phase 6b's F1–F4 (an app server killed mid-run, one server's
   network cut for 10 s and 60 s) on the production layout, plus what phase 6b didn't do:
   **reboot the share host during a run**. After each, the checks must still pass for every
   save that was acknowledged, and every record must still be changeable. Without ssh these
   are done by hand (stop the app process, unplug or disable a network adapter) at an agreed
   time during a long run.
5. **Operational checks:** antivirus on and off, a backup running during a run, a list view
   timed with 6,000 records.
6. **Leasing on the production share host:** the heavy and pipeline profiles with the
   leasing mode production will use (leasing off is the candidate, section 5), watching
   for long pauses and for saves that gave up after the rename retry.

The method, checks and pass rules are described in `docs/methodology.md`; follow it so the
results are comparable with the proof of concept. Section 10 says what harness exists for
this, and what must be built first.

## 10. Testing in the target environment

The plan's verification (section 9) runs the proof of concept's tests on the
production-like servers and share first, with the toy app, before any real code exists:
"is C safe on this share host, at this load, with this load balancer?" This section says
what exists for that today, what has to be built first, and how to run it once it is.

### 10.1 What exists today, and what doesn't

| Piece | State | Where |
|---|---|---|
| Stress tool: workloads (light, heavy, pipeline), every check, stale reads, `--fault`, the lock check | **Exists** | `webapp/tools/BlazorApp.Stress/`; `docs/strategies/StressTool.md` |
| Driving Windows servers **over ssh** from a Linux machine (`--remote`), and `prepare-test-server.ps1` | **Exists, lab only**: needs ssh, a firewall rule and a Linux driver, none of which the target allows | Appendix A |
| `ClaudeTestServer.ps1`: start and stop a test server by hand on each server, on a port you give it | **Designed, not built** | `docs/strategies/StressToolWindows.md` |
| Stress tool on a **Windows PC** using running servers (`--external`), with **`--routing random\|sticky`** and `--prepare` | **Designed, not built**; the tool has never run on Windows | same |
| Fallback: one tool per server against `localhost`, merged by `--check` (if no port from the PC can be opened) | **Designed, not built** | same |
| Filling the share with records before a run (`--prefill <n>`) and timing list views (`--list`) | **Exists** (phase 6b's volume test) | the stress tool |
| Self-contained builds of the server, the tool, the contract tests and `BlazorApp.StoreHelper` | **Not done** | — |

**If you are reading this before those pieces exist, building them is the first phase of
your plan** (10.2). Check the repository first: they may have been built since this was
written (look for `webapp/scripts/windows/ClaudeTestServer.ps1` and `--external` in
`webapp/tools/BlazorApp.Stress/StressOptions.cs`).

### 10.2 What to build first

Following `docs/strategies/StressToolWindows.md`, with its tests written and seen failing
first:

1. `ClaudeTestServer.ps1` (`Install`, `Start -Port <n>`, `Stop`, `Status`, `CollectLogs`,
   `Uninstall`): no firewall changes, no services, every port a required parameter.
2. Stress tool options `--external`, `--routing random|sticky`, `--prepare --users-out`, and
   the read-back order (API while the servers run, then the folder once they are stopped).
3. Self-contained `win-x64` publishes of the Server and the stress tool, packaged with the
   script.
4. **A lab check before the enterprise round:** the tool on a Windows machine against two
   Windows Server VMs started with the script (no ssh), a light run of `NaiveFile` and
   `PerRecordFile` with both routings: A must fail, C must pass.
5. Make `--prefill` and `--list` work with `--external`. Today the prefill creates the records
   on the tool's machine through the app's rules and copies them into the data folder before
   it starts the servers; with external servers that copy must land on the share **before
   the servers are started by hand** (for example as part of `--prepare`).

### 10.3 Running in the target environment (once built)

```text
your Windows PC (stress tool, 10 simulated users)
   ├── http://server1:<port> ──▶ test server on server 1 ─┐
   ├── http://server2:<port> ──▶ test server on server 2 ─┤ SMB
   └── reads \\fileserver\share\stress\<run> ◀─────────────┘
```

1. **Upload** the package to both servers and run `ClaudeTestServer.ps1 Install` on each.
2. **Confirm the port.** Start a test server on the first port the owner names, on server 1,
   then on the PC: `Test-NetConnection server1 -Port <port>`. `TcpTestSucceeded : True` means
   it is open (the test only succeeds while something listens). If not, stop it and try the
   next port the owner names. Repeat for server 2.
3. **Record the environment** (below) before the first run.
4. **Each run:** `--prepare` on the PC (writes the users file to the share) → `Start` on
   both servers with the run's data folder → the tool on the PC with `--external` and
   `--routing` → `Stop` on both when the tool asks → the tool checks the folder and writes
   `result.json` and `summary.md`. The exact commands are in `StressToolWindows.md`.
5. **Known answer first.** Both test servers on server 1 (two `Start`s on two ports) with a
   data folder on server 1's **local disk**: `NaiveFile` must fail and
   `PerRecordFile` must pass, light, seeds 1–3, before any run on the share. If not, stop:
   the harness is not doing what this document assumes. If only one port is open on
   server 1, rely on the lab check (10.2, step 4) for the harness, and on `NaiveFile`
   failing on the share in the round below.
6. **The round**, so the results compare with phase 6b:

   | Profile | Options |
   |---|---|
   | light | `--clients 5 --records 6 --ops 40` |
   | heavy | `--clients 12 --records 12 --ops 200` |
   | pipeline | `--clients 5 --workload pipeline --think-ms 1000 --ops 120` |

   `NaiveFile` (the control) and `PerRecordFile`, each profile, **each routing**, seeds 1–3.
   Then the pipeline with the share filled to ~6,000 records (`--prefill 6000 --list`), an
   hour-long pipeline run, and
   the failure tests of section 9 (done by hand during a long run).

   That is 2 strategies × 3 profiles × 2 routings × 3 seeds = **36 runs**, each with a start
   and stop on both servers if done one run at a time. The shortcut in
   `StressToolWindows.md`: start each strategy's servers once, on one fresh data folder for
   that strategy, run all its profiles, routings and seeds against them, and restart only
   between strategies (the checks already handle records an earlier run created: they are
   counted and checked for rules only). That is 2 restarts instead of 36. Decide when
   building 10.2 whether results stay comparable that way; the users file must then cover
   every run's users.
7. Clean the share's test folder between strategies.

#### Record the environment

Before the first run, write down what `docs/methodology.md` section 6 lists, plus, on each
server:

```powershell
Get-SmbConnection | Select ServerName, ShareName, Dialect          # SMB version in use
Get-ItemProperty HKLM:\System\CurrentControlSet\Services\LanmanWorkstation\Parameters |
  Select DirectoryCacheLifetime, FileInfoCacheLifetime, FileNotFoundCacheLifetime   # empty = defaults
Get-MpPreference | Select DisableRealtimeMonitoring, ExclusionPath  # Defender; or the AV in use
```

and, for the share: its host (model, OS), its offline-files setting, its **leasing mode**
(on a Windows share host: `Get-SmbShare -Name <share> | Select Name, LeasingMode`), and
whether anything replicates or scans it.

### 10.4 Reading and recording the results

- `PerRecordFile` must pass **every** run. Any lost update, missing record or unreadable
  file in any run means C is not safe on this share as configured: find out why (the
  failing lines name the record, version, server and correlation id, to look up in the
  servers' logs, `CollectLogs`) before going further.
- `NaiveFile` must fail; if it passes, the load didn't produce collisions and the round
  proves nothing.
- **Server errors (HTTP 500)** are not lost data but are failed saves for users: count them.
  Phase 6b's baseline with leasing off and the then 1 s rename retry: 0–10 per heavy run and
  0 per pipeline run (outside the failure tests). The 3 s retry has not yet been run under
  heavy load, so expect fewer; treat more than a handful as a sign of antivirus, leasing or
  the share host.
- **Stale reads** are reported, not failed. Phase 6b saw none for C. If some appear right
  after a create on the other server, the likely cause is the SMB "not found" cache
  (section 5): a prediction, not something observed. Record the number for the plan's
  decision on that cache.
- Keep each run's `result.json` and `summary.md` and write the round up as
  `docs/methodology.md` section 7.4 describes, with the environment recorded in 10.3.

## 11. Reference implementation (this repository)

| What | Where |
|---|---|
| The store | `webapp/src/BlazorApp.Services/Storage/PerRecordFile/PerRecordFileStore.cs` |
| The rename retry for Windows (phase 6b) | `webapp/src/BlazorApp.Services/Storage/FileReplace.cs` |
| Its design notes and caveats | `docs/strategies/PerRecordFile.md` |
| The storage interface and results | `webapp/src/BlazorApp.Services/Storage/IRegistrationStore.cs`, `WriteResult.cs` |
| JSON format (indented, camelCase, enums as names) | `webapp/src/BlazorApp.Services/Storage/RecordJson.cs` |
| Service: rules, first version check, building the new record, 409 mapping | `webapp/src/BlazorApp.Services/RegistrationService.cs` |
| Record type | `webapp/src/BlazorApp.Shared/Dtos/RegistrationRecordDto.cs` |
| Contract and multi-process tests | `webapp/tests/BlazorApp.Services.Tests/Storage/` and `webapp/tests/BlazorApp.StoreHelper/` |
| Preparing a Windows test server over ssh (lab only) | `webapp/scripts/windows/prepare-test-server.ps1` (Appendix A.3) |
| The planned no-ssh harness | `docs/strategies/StressToolWindows.md` (section 10) |
| Stress tool and its checks | `webapp/tools/BlazorApp.Stress/` (`DataCheck.cs`), `docs/strategies/StressTool.md` |
| Method, hypotheses and all results | `docs/methodology.md`, `docs/experiments/` |
| Plan and decisions of the proof of concept | `docs/plan.md` |

## 12. Expected output: the implementation plan

The plan should have:

1. **Answers** to section 8, or the assumptions made where the owner chose not to answer.
2. **Architecture decisions** for the real application: record layout, folder structure,
   list strategy (6.2), lock wait limit (6.1), share configuration (section 5), error
   handling and what users see.
3. **Phases**, each small enough to review, each with "done when" conditions and a gate:
   build with warnings as errors, all tests (none skipped), a spot-check that a test fails
   when the key rule is broken, a code review, green CI.
4. **The verification of section 9**, as phases of its own, with the environment record
   from `docs/methodology.md` section 6 filled in for the production-like share.
5. **Rollback and fallback:** what triggers moving to the fallback in section 2, and how
   data would move there (one JSON file per record makes that a straightforward import).
6. **Risks**, at least: the untested share host, antivirus/backup interference, the list
   view at 6,000 records, and a single share host as a single point of failure.

## Appendix A. The ssh lab path (how phase 6b ran; not for the target environment)

Use this only in a lab where ssh to the Windows servers and a firewall rule for the test
servers are allowed, for example to repeat phase 6b on your own VMs. The tool runs on Linux,
starts and stops the servers over ssh, and copies the data back. The failure options
(`--fault kill:<n>@<s>`, `--fault run:<cmd>@<s>`) and the lock check work in this path
(`docs/strategies/StressTool.md`, "Failure tests").

### A.1 What you need

| Machine | Role | Needs |
|---|---|---|
| **Driver** | Runs the stress tool: the clients, the read-backs and every check. Starts and stops the servers over ssh | Linux with the .NET 10 SDK, git, and OpenSSH 9.0 or later (`ssh -V`). **Only a Linux driver has been tested** (Debian 12). A Windows driver may work but is untested; if only Windows is available, use a small Linux VM on the same network |
| **Server 1, Server 2** | Run copies of the app's server, copied there by the tool for every run | Windows Server; prepared with `webapp/scripts/windows/prepare-test-server.ps1` (A.3); an administrator account for ssh |
| **Share** | The folder under test | The production-like SMB share, with a test folder both servers' accounts can modify |

Network: the driver reaches both servers on ssh (22) and on any TCP port to `dotnet.exe`;
both servers reach the share. Use a test folder on the share that nothing else uses.

**Code:** everything this appendix uses is on `main`.

### A.2 The driver

```bash
git clone https://github.com/92dl92/two-servers-shared-folder.git && cd two-servers-shared-folder
dotnet build webapp/BlazorApp.slnx             # warnings count as errors
dotnet test  webapp/BlazorApp.slnx             # all must pass, none skipped
ssh-keygen -t ed25519 -f ~/.ssh/claudetest -N ""   # a key for the test servers
cat ~/.ssh/claudetest.pub                      # paste into A.3
```

The E2E tests need Chromium once: `pwsh webapp/tests/BlazorApp.E2E.Tests/bin/Debug/net10.0/playwright.ps1 install chromium`.
Add the key to `~/.ssh/config` for both servers (`Host server1.example` … `IdentityFile
~/.ssh/claudetest`); the tool connects with `BatchMode`, so a password prompt fails.

### A.3 Each Windows server

Copy `webapp/scripts/windows/prepare-test-server.ps1` to the server and run it **as the
account the tool will log in as** (an administrator), in Windows PowerShell:

```powershell
powershell -ExecutionPolicy Bypass -File prepare-test-server.ps1 `
  -AuthorizedKey 'ssh-ed25519 AAAA... claudetest' -DriverAddress <driver IP>
```

It turns on the OpenSSH server with PowerShell as its default shell (the tool sends every
command as `powershell -EncodedCommand`), adds the key, installs the ASP.NET Core 10 runtime in
`C:\dotnet` and puts it on the path, and allows inbound connections to `dotnet.exe` from the
driver only. It hasn't been run in this exact form yet (it is adapted from the test VMs'
setup); check its output.

It installs ssh and adds a firewall rule, so it is **for a lab only**; never run it in the
target environment. It installs the framework-dependent ASP.NET Core 10 runtime, which is
fine in a lab; the target path ships everything self-contained instead.

**Share credentials.** A key login over ssh has no network credentials, so the server can't
open the share in that session. Before every command that touches the share, the tool runs
`%USERPROFILE%\claudetest-stress\connect.ps1` if it exists. Create it on each server (it
holds a password, so it stays on the machine, never in the repository):

```powershell
# %USERPROFILE%\claudetest-stress\connect.ps1
net use \\fileserver\share /user:DOMAIN\svc-claudetest '<password>' 2>&1 | Out-Null
```

**Check from the driver:**

```bash
ssh Administrator@server1.example 'dotnet --list-runtimes'
ssh Administrator@server2.example '& ~/claudetest-stress/connect.ps1; Test-Path \\fileserver\share\stress'
```

The first must list `Microsoft.AspNetCore.App 10.x`; the second must print `True`.

### A.4 One run

Every server runs remotely, so leave out `--data`: after the servers stop, the tool copies
the folder back from the first remote (with `tar` and `scp`) and checks that copy. Give each
run its own folder on the share. With the share on a separate host, both servers use the
same UNC path:

```bash
run=prod-light-PerRecordFile-1
dotnet run --project webapp/tools/BlazorApp.Stress --no-build -- \
  --strategy PerRecordFile --servers 2 --remote-servers 1 \
  --remote Administrator@server1.example --remote-data "\\\\fileserver\\share\\stress\\$run" \
  --remote Administrator@server2.example --remote-data "\\\\fileserver\\share\\stress\\$run" \
  --clients 5 --records 6 --ops 40 --seed 1 --out stress-runs/$run
```

`--remote` and `--remote-data` pair up in order; `--remote-servers` servers run on each.
The host in `--remote` must resolve for HTTP too (a name or address, not an ssh alias). If
one of the servers hosts the share, give it its local path (`C:\share\stress\<run>`) and the
other the UNC path, as in the proof of concept.

Exit code 0 = every check passed, 1 = a check failed, 2 = the run couldn't be done. The
summary is printed and saved in `stress-runs/<run>/summary.md`.

### A.5 The round

Same settings as the proof of concept's Windows round, so the results compare:

| Profile | Options |
|---|---|
| light | `--clients 5 --records 6 --ops 40` |
| heavy | `--clients 12 --records 12 --ops 200` |
| pipeline | `--clients 5 --workload pipeline --think-ms 1000 --ops 120` (no `--records`) |

1. **Known answer first**, both servers on server 1 with a folder on its local disk, as in
   the proof of concept (`--remote Administrator@server1.example --remote-data
   'C:\stress\<run>' --remote-servers 2`): `NaiveFile` must fail and `PerRecordFile` and
   `Sqlite` must pass, light, seeds 1–3. If not, stop: the harness is not doing what this
   document assumes.
2. **The two-server round on the share:** `NaiveFile` (the control) and `PerRecordFile`,
   each profile, seeds 1–3. `NaiveFile` failing here is the known answer on the share. The
   other strategies are optional; `Sqlite` across two servers is expected to lose data.

```bash
s1=Administrator@server1.example; s2=Administrator@server2.example
share='\\fileserver\share\stress'
for profile in light heavy pipeline; do
  case $profile in
    light)    opts="--clients 5 --records 6 --ops 40" ;;
    heavy)    opts="--clients 12 --records 12 --ops 200" ;;
    pipeline) opts="--clients 5 --workload pipeline --think-ms 1000 --ops 120" ;;
  esac
  for strategy in NaiveFile PerRecordFile; do
    for seed in 1 2 3; do
      run=prod-$profile-$strategy-$seed
      timeout 30m dotnet run --project webapp/tools/BlazorApp.Stress --no-build -- \
        --strategy $strategy --servers 2 --remote-servers 1 \
        --remote $s1 --remote-data "$share\\$run" --remote $s2 --remote-data "$share\\$run" \
        $opts --seed $seed --out stress-runs/$run
      echo "$run exit $?"
    done
  done
done
```

Don't change or rebuild the repository while a round runs. A run that hits the 30-minute
limit counts as hung; check that no `dotnet` server is left on either Windows server
(`Get-Process dotnet`) before the next run (the tool also stops leftovers itself).
