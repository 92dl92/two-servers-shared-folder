# Plan: roles, shared records, and file-storage experiments

Status: **final** (decisions recorded at the end)

## Motivation

The point of this app is an experiment. Several copies of the server run at once on
different ports, all saving records to one shared folder, while clients use each of
them. The goal is to try different ways of storing records in files and find out which
ones lose updates, leave broken files, or have other problems when several servers
change the same records at the same time.

The roles, records and UI exist to give the experiment realistic data and realistic
collisions: several people changing the same record at the same time.

## 1. Users and roles (no login)

The app needs to know who is acting, but nobody proves it.

- **Where users are defined:** `users.json`, e.g.
  `{ "id": "alice", "name": "Alice", "roles": ["Reviewer"] }`. Each server loads it at
  startup.
- **How you pick a user:** an "Acting as" dropdown in the top bar. The choice is saved
  in a cookie, so the server knows the user both when it first draws a page and when
  the browser calls the API.
- **How permissions are enforced:**
  - The server checks permissions on every action and refuses with "403 Forbidden" if
    the user's role isn't allowed.
  - The UI hides or disables buttons the user can't use, for convenience only. The
    server check is what counts.

Roles:

| Role | Can do |
|---|---|
| Registrant | Create records, edit their own records while they're still drafts or submitted |
| Reviewer | View all records, approve or reject them, add comments |
| Admin | Everything, including delete |

> This gives roles and permissions to experiment with, but **no security**: anyone can
> pick any user. That's acceptable for this app, but it is not real access control.

## 2. Records that several people update

A registration becomes a record with an id, a version number, and a status. Each record
holds:

- `Id`
- `Version`: goes up by 1 on every change
- `Status`: Draft → Submitted → Approved or Rejected
- `CreatedBy`, `UpdatedBy`, `UpdatedAt`
- the form data
- `History`: a list of who did what, and when

**Actions:** Create, Update, Submit, Approve, Reject, Comment, Delete. Each action is
allowed only for certain roles, and only when the record is in certain statuses.

**Rule for two people editing at once (optimistic concurrency):**

- Every update sends the version the person started editing from.
- If the record has changed since, the server refuses with "409 Conflict" instead of
  quietly overwriting the other person's work.
- The UI then shows "Someone else changed this record" with a Reload button.

This rule is what makes storage bugs visible. If a save that the app reported as
successful later disappears, the storage strategy has a bug. A 409 is the system
working correctly.

**UI changes:**

- The Register page creates new records.
- A new Records list and Record detail/edit page.
- Action buttons shown according to the user's role.
- The conflict message described above.

## 3. File storage you can swap

There's one storage interface, `IRegistrationStore`, with several implementations,
chosen in settings, e.g.
`--Storage:Strategy=PerRecordFile --Storage:Path=/shared/data`.
Each strategy is a hypothesis to test:

| Strategy | How it works | Expected behaviour with several servers |
|---|---|---|
| **A. Naive single file** | Read the whole JSON file, change it, write it back | Two servers save at once and one change is lost; a crash mid-write can leave a half-written file. This is the control. |
| **B. Single file + file lock** | Same as A, but holding an OS-level lock on the file while reading and writing | Changes aren't lost, but all saves wait their turn |
| **C. One file per record + version check** | `records/{id}.json`; write to a temp file, then rename it into place in one step; refuse the save if the version has changed | No half-written files; servers only wait for each other when they touch the same record |
| **D. Append-only log** | Each change is added as a line to a `.jsonl` file; current state is rebuilt by reading the log | Tests whether appends from several processes ever interleave; reading gets slower as the log grows |
| **E. SQLite** | A single-file database | The "known good" comparison on local disk. SQLite's own docs warn its locking is unreliable on network drives, so it may fail there. |

Two traps to design around from the start:

- **Locks inside one server don't stop other servers.** The current
  `InMemoryRegistrationService` uses a C# `Lock`, which only coordinates threads within
  one process. With several servers, only a lock held on the file itself works across
  them.
- **Copies kept in memory go stale.** Each server must read from disk every time (or
  check that its copy is still current), or it will show and overwrite old data.

Platform note: on Linux, .NET's file locks are *advisory*: they only work if every
program touching the file asks for the lock. Network drives (NFS/SMB) behave differently
again: locks, renames and appends may not be honoured or may be cached per machine. The
experiment runs on local disk first, then repeats on a network drive (section 4).

## 4. The multi-server experiment

- **Run script:** `run-cluster.sh 3 PerRecordFile` starts 3 servers (ports 5081, 5082,
  5083) sharing one data folder. Open each port in a different browser tab for manual
  testing.
- **Stress tool:** a console app, `BlazorApp.Stress`, that sends many updates to all the
  servers at once, then checks:
  - **Nothing lost:** every update a server reported as saved is actually in the data.
  - **No broken files:** every file is complete, valid JSON.
  - **Rules respected:** versions only go up, and no status change breaks the workflow
    rules.
  - **Numbers recorded:** how many updates were refused as conflicts, saves per second,
    and how long saves took.
- **Two rounds:**
  1. Local disk: all servers on one machine, shared folder on local disk.
  2. Network drive: the shared folder on an NFS or SMB share, ideally with servers on
     more than one machine, since per-machine caching is where network drives tend to
     break.
- **Output:** a results table comparing every strategy on both kinds of storage, saved
  in `docs/experiments/`.
- **Real servers, not simulated ones:** the stress tool starts real separate server
  processes. Simulating several servers inside one process wouldn't reproduce the
  problems being tested.

## 5. Tests

| Project | Tool | Covers |
|---|---|---|
| `Services.Tests` | xUnit | Permissions and status rules; the shared storage test suite that every strategy must pass, including many threads and several processes saving at once |
| `Server.Tests` | xUnit + `WebApplicationFactory` | The API: 403 for the wrong role, 409 for an out-of-date version, and that the selected user (from the cookie) is respected |
| `Client.Tests` | xUnit + bUnit | Components: tabs, nav tray, buttons shown per role, the conflict message |
| `E2E.Tests` | xUnit + Playwright for .NET | A few main user journeys in a real browser against the running app. Catches layout, CSS and browser↔server problems that the other tests can't see. |
| Stress tool | — (section 4) | Separate servers sharing files: the experiment itself, not run on every build |

All of them run with one command: `dotnet test webapp/BlazorApp.slnx`.

### Test rules

- **Rules are tested as complete tables.** Permissions and status changes are tested for
  every combination of role × action × status, not a few examples. The expected table
  is written out in the test itself, not read from the code being tested, so a mistake
  in the code can't also be copied into the test.
- **New behaviour gets a test that fails first.** Write the test, see it fail, then
  write the code. A test that has never failed might not be testing anything.
- **Spot-check that tests can catch bugs.** For each phase's key rule, deliberately
  break the code once (e.g. let a Reviewer delete) and confirm a test fails, then
  revert. Note it in the commit message.
- **Never weaken old tests to get green.** No skipping, deleting or loosening an earlier
  test. If a requirement really changed, update the test and say why in the commit
  message.

## 6. Order of work

### Gate for every phase

A phase is finished only when all of these pass. The next phase never starts on a
failing gate.

1. `dotnet build webapp/BlazorApp.slnx` succeeds; warnings count as errors.
2. `dotnet test webapp/BlazorApp.slnx` passes: every test from this phase **and all
   earlier phases**, with none skipped.
3. The phase's own "done when" conditions below are met, and `docs/methodology.md` is
   up to date with what the phase changed or found (its section 11).
4. `/code-review high` on the phase's changes; confirmed findings are fixed (with a
   test where the finding is a bug) and the gate is rerun. Findings judged not worth
   fixing are listed in the commit message with the reason.
5. The commit message lists what was checked, including the spot-check from the test
   rules and the review outcome.
6. After pushing, the CI run for the commit passes (from phase 1 onwards). A red CI run
   is treated like a failing gate.

**Stop rule:** if a gate can't be met, stop. Push the work in progress, describe what's
failing, and wait for a decision rather than building the next phase on top of it.
Mistakes that pile up across phases are the main risk of working without a review
between phases, and the gate is what prevents that.

### Phases

1. **Test projects.** Create the four test projects and cover the existing code.
   Done when:
   - xUnit: `InMemoryRegistrationService` accepts a valid registration and rejects each
     invalid field (required, too long, bad email) with an error under that field's name.
   - bUnit: `TabControl` switches tabs and sets the accessibility attributes;
     `NavTray` collapses and closes on navigation; `Register` jumps to the first tab
     with an error and shows server errors inline.
   - E2E: register successfully, see validation errors after an empty submit, use the
     mobile drawer. These are the checks from the throwaway Playwright run, now saved.
   - CI: `.github/workflows/ci.yml` builds and runs all tests (including E2E, with a
     step that installs Chromium) on every push to `main` and every PR, and the first
     run passes.
2. **Users and roles.** `users.json`, the "Acting as" picker, permission checks, 403s.
   Done when:
   - A complete role × action table test passes.
   - The API returns 403 for every action a role may not do, and a defined response for
     a missing or unknown user.
   - bUnit: buttons are shown or hidden correctly for each role.
   - E2E: switching user changes which buttons are shown.
3. **Records.** Record model, status workflow, version check, history, Records list and
   edit pages; still stored in memory.
   Done when:
   - A complete role × action × status table test passes.
   - Saving with an out-of-date version returns 409; every successful change raises
     the version by exactly 1 and adds one history entry.
   - Many threads updating one record from the same starting version: exactly one
     succeeds, the rest get 409.
   - E2E: two browser windows edit the same record; the second save shows the conflict
     message.
4. **File storage**, in two steps:
   - **4a. Storage interface and shared tests.** `IRegistrationStore`, the shared test
     suite, and the in-memory store moved behind the interface. The shared suite
     checks: save and load return the same data; out-of-date versions are refused;
     delete; list; many threads saving at once; data survives restarting the store on
     the same folder; and **several separate processes saving at once**, using a small
     helper program the tests start. Done when the in-memory store passes everything
     that applies to it and the multi-process helper is proven to work.
   - **4L. Logging and record events** (section 8). Done when the tests listed in
     section 8 pass, and the logging hooks every store uses are in place before 4b
     starts.
   - **4b. Strategies A–E and the stress tool**, built in parallel (see section 7).
     Done when each strategy B–E passes the full shared suite. Strategy A runs the same
     tests with its concurrency checks reversed: they must show that A **does** lose
     updates. This proves the tests can detect lost updates at all.
5. **Experiment, local disk.** Run script, stress-tool runs, results table.
   Done when:
   - The stress tool has been checked against known answers first: it finds lost
     updates with A, and finds none with E (SQLite) on local disk. A tool that can't
     tell these apart can't be trusted on the others.
   - Each strategy has been run at least 3 times. The results record the commit, the
     settings, and the numbers from each run.
6. **Experiment, network drive.** Repeat phase 5 with the shared folder on NFS/SMB.
   Done when the results table has a network-drive column for every strategy, with the
   same known-answer check first.
   - **6b. Windows servers on an SMB share, realistic load.** Production would be 2
     Windows Server machines, at most 10 clients, working as a pipeline (one role
     creates and submits, another responds), so true concurrent writes are rare. Phase 6's
     failures came from the macOS NFS client, so they don't settle whether a share works
     here. Setup: two Windows Server evaluation VMs on linux-host (KVM/libvirt), with the
     stress tool and clients staying on linux-host. Two share layouts: **(1)** the NAS over
     SMB, both VMs as clients; **(2)** VM 1 shares its own folder, so one server writes to
     local disk and the other over SMB. The stress tool's `--remote` learns to start and
     stop servers on Windows over OpenSSH (no `rsync` there). New workload alongside the
     light and heavy runs: a **realistic profile** of 2 servers and 10 clients in total, creators
     and responders handing records over with think time between steps, over a longer
     run. New check: **stale reads**, where a read returns an older version than one
     already acknowledged (the pipeline's risky moment is the handoff between servers).
     Done when:
     - the known answer holds with both servers on one VM (A fails, E passes), then on
       each layout;
     - each strategy has 3 runs (seeds 1–3) of light, heavy and realistic on each layout,
       with the commit, settings and numbers recorded;
     - the results give each layout a verdict: safe for the realistic load, or how often
       it lost or broke something, and the stale reads counted.
     - **failure tests** on layout 2, realistic load, strategy C, seeds 1–3: F1
       ws2's server killed mid-run; F2 ws1's server killed mid-run; F3 ws2's network cut for
       10 s (`virsh domif-setlink`) and restored; F4 the same for 60 s. Each must keep every
       acknowledged save and leave no broken file (saves that got no answer may go either
       way, as in every run). The stress tool learns to run a planned failure at a set time
       and to skip the read-back through a server it killed;
     - a **volume test**: the share filled with ~6,000 records (three years of production)
       before the realistic load on layout 2, measuring save, read and list times for C;
     - a **soak**: the realistic load for one hour for C on layout 2, on the filled share,
       checked as any run, with server errors, stale reads and the save latency over time
       reported.
7. **Docs.** The purpose of the app and the results in the README; the new layout in
   `CLAUDE.md`.

## 7. How the work is run

| Phases | Where | How |
|---|---|---|
| 1 → 4a | One cloud session | Phases run in order on one branch. Each phase ends with its gate, then a commit and push, so the history can be reviewed phase by phase. One PR at the end. |
| 4L | One cloud session | Logging (section 8) on its own branch and PR, merged before 4b starts. |
| 4b | Parallel cloud sessions | One session per strategy (A–E) plus one for the stress tool, each started from the merged 4L code on its own branch, each with its own PR. |
| 5 | One cloud session | Several servers and the stress tool on one cloud machine. |
| 6 | Local or own machines | Needs a real network share, ideally used by more than one machine. A cloud session is a single machine. |
| 7 | Any | |

Rules that keep parallel sessions from colliding:

- Each strategy lives in its own folder (`Storage/<Strategy>/`) with its own
  registration method, so parallel branches don't edit the same files.
- 4a fixes the interface and the shared tests. A 4b session that needs to change them
  stops and reports rather than changing them on its own branch.

GitHub checks and reviews:

- **CI (phase 1):** every push and PR is built and tested on a clean GitHub machine,
  independently of the session that wrote the code.
- **Branch protection (manual, after phase 1):** in the repo settings, require the CI
  check to pass before merging into `main`. This is a GitHub setting and can't be
  committed.
- **Self-review in the gate:** each phase runs `/code-review high` on its own changes
  before committing (section 6).
- **Claude GitHub Action (optional, for 4b PRs):** a workflow that reviews each PR when
  it opens, using the `code-review` plugin, plus `@claude` in PR comments for
  questions or fixes. Set up with `/install-github-app` in a local session, using a
  subscription token from `claude setup-token`. It gives a second opinion from an agent
  that didn't write the code, which matters most when several parallel branches merge.
  Anthropic's managed Code Review is the alternative on Team/Enterprise plans.

Shared rules for every session:

- **This plan is the source of truth.** Any change of direction is written into the
  Decisions table in the same commit.
- **Cloud environment setup:** the .NET SDK isn't pre-installed.
  `webapp/scripts/setup-cloud.sh` installs it from Ubuntu's own packages (Microsoft's
  download host was blocked by the default network policy), and `CLAUDE.md` tells
  sessions to run it when `dotnet` is missing. The first cloud session (phase 1) also
  registers it as a startup hook in `.claude/settings.json`, so later sessions set
  themselves up automatically. Chromium for the E2E tests is pre-installed in cloud
  sessions.
- **Teleport** (`claude --teleport`) brings any cloud session to a local machine to run
  or debug it; it fetches the branch itself. It only goes from cloud to local. To hand
  work back, push and start a new cloud session.

## 8. Logging and record events (phase 4L)

Phase **4L** runs after 4a and before 4b, so every storage strategy is built with logging
from the start and the experiment has a record of what each server did.

Store-level logging needs a logger inside each store, which the 4a factory
(`IRegistrationStoreFactory.Create(path)`) doesn't pass. 4L extends it, for example by
passing an `ILoggerFactory`, and updates the in-memory store and the shared suite to
match. Doing this before 4b keeps the 4b rule that the interface and the shared tests
are fixed.

### What gets logged

| Where | What | Level |
|---|---|---|
| Server, `RegistrationService` | **One record event per command attempt** (Create, Update, Submit, Approve, Reject, Comment, Delete, and View): user, record id, expected version, new version, old → new status, outcome (`Saved`, `NoUser`, `Forbidden`, `NotFound`, `Invalid`, `Conflict`, `WrongStatus`), the fields that were invalid, and the duration | Information when saved; Warning when refused |
| Server, every store | Reads, writes and conflicts, with the store's own detail (file name, lock wait time, number of retries) | Debug |
| Server, HTTP | One line per API request: method, route, status code, duration, correlation id | Information |
| Server, users | "Acting as" changes, and unknown user ids that were sent | Information / Warning |
| Server, startup | Server id, port, process id, storage strategy and folder, the users file that was loaded | Information |
| Client | Page loads, every button the user presses (Edit, Save, Submit, …), the result shown (saved, the conflict message, an error), and failed API calls | Information |

Each record event has its own fixed `EventId` and a message template, so a log can be
filtered and counted per action and outcome. The service logs the event in one place,
so the API and prerendered pages both produce it.

### Tying events together

- **Correlation id:** the client creates an id for each user action and sends it as an
  `X-Correlation-Id` header. The server puts it in a logging scope, so the button press
  in the browser, the HTTP request and the record event all share it. A request without
  the header gets a new id from the server.
- **Server id:** every server line carries a server id (port and process id), so the logs
  of several servers sharing one folder can be merged and still told apart.

### Where logs go

- **Server:** structured JSON lines, one file per server
  (`logs/server-<port>.jsonl`), plus the normal console output. Levels come from
  `appsettings.json` and can be overridden on the command line
  (`--Logging:LogLevel:BlazorApp=Debug`).
- **Client:** the browser console, and client events are also sent to the server
  (`POST api/client-logs`, in batches), which writes them to the same file marked as
  `"Source": "client"`. One file then tells the whole story of a user action.

### Privacy

The form data holds names, emails and phone numbers. Logs record ids, versions, statuses
and **field names** only, never field values. Comment text is not logged either.

### Tests (phase gate)

- A complete table: every action × every outcome produces exactly one record event with
  the expected `EventId`, level and fields. It is checked with the built-in
  `FakeLogger` (`Microsoft.Extensions.Diagnostics.Testing`), and the expected table is
  written out in the test.
- No log line contains a value from the form data. The test fills the form with
  recognisable values and searches every captured line for them.
- API: the correlation id sent in the header shows up in the HTTP line and in the
  record event; a new id is created when none is sent.
- bUnit: pressing each button logs a client event with the action and the result.
- E2E: a save made in the browser appears in the server's log file, client event and
  server event together, with one correlation id.
- Spot-check: remove the event from one action and confirm the table test fails.

### Decisions for this phase

| # | Question | Decision |
|---|---|---|
| L1 | Logging library | Built-in `Microsoft.Extensions.Logging`, plus a small JSON-lines file writer of our own (no Serilog), following CLAUDE.md's "prefer built-in" |
| L2 | Client logs | Sent to the server in batches (`POST api/client-logs`) and written to the same file marked `"Source": "client"`; also shown in the browser console |
| L3 | View (read) actions | Logged as record events at Debug, so they are off by default and can be switched on for the experiment |
| L4 | Purpose of logs | Diagnostics only, so they can be deleted or rotated. The record's `History` is the audit trail |
| L5 | Phase | 4L, after 4a and before 4b |

## Decisions

| Topic | Decision |
|---|---|
| Roles | Registrant / Reviewer / Admin, with the permissions in section 1 |
| Record actions | Full status workflow: Draft → Submitted → Approved/Rejected, plus edit, comment, delete |
| Experiment style | Both: run script for manual browser testing, and the automated stress tool |
| Storage strategies | A naive single file, B single file + lock, C file per record, D append-only log, E SQLite |
| Storage locations | Local disk first, then a network drive (NFS/SMB) |
| Working method | One cloud session for phases 1 → 4a with a gate per phase; parallel cloud sessions for 4b (section 7) |
| Test tooling (phase 1) | xUnit 2 in all test projects; bUnit 2; `Microsoft.Playwright` pinned to 1.56.0 to match the cloud's pre-installed Chromium (revision 1194); E2E tests run the built Server as a separate process on a free port and wait for a `data-interactive="true"` marker on the layout before clicking |
| Users and permissions (phase 2) | Permission table in `Shared/Rules/Permissions` (used by server and UI) gives each role a scope per action: Registrant View/Update/Submit **own**, Create any; Reviewer View/Approve/Reject/Comment any; Admin everything. Users with several roles get the widest scope. Missing or unknown user → **401** (problem details naming the id); role not allowed → **403**; both checked before validation. Services enforce it (in-process callers included). Changing "Acting as" saves the cookie via `PUT api/users/current` and reloads the page. Phase 2's API exposes Create and View only, so its 403 table covers those; the full role × action × status table over the API comes with the record endpoints in phase 3 |
| Records (phase 3) | `Register` creates a **Draft** at version 1 with one `Create` history entry. Every change (update, submit, approve, reject, comment, delete) sends the expected version. Server checks in this order: no user 401 → not found 404 → role/ownership 403 → invalid data 400 → out-of-date version **409** → status not allowed **422** (kept apart from 409 so the stress tool counts only real conflicts). Rules live in `Shared/Rules/RecordRules` (permission + status) and are shared by server and UI. The version is checked twice: before building the change, and again atomically by the store when writing, so a race between read and write is still a 409. The Submissions page became the Records list (`/records`), with a record page at `/records/{id}` |
| Storage interface (phase 4a) | `IRegistrationStore`: `GetAsync`, `ListAsync`, `TryCreateAsync`, `TryReplaceAsync(updated, expectedVersion)` (updated must be expected + 1), `TryDeleteAsync(id, expectedVersion)`, returning `Saved`/`Conflict`/`NotFound`. `RegistrationService` holds the rules and uses the store. A strategy is a folder `Services/Storage/<Strategy>/` with a public `IRegistrationStoreFactory` (name, uses-folder flag, `Create(path)`) found by reflection, so 4b branches add files without editing shared ones. `RecordJson` gives every store the same file format (indented JSON, enums as names) |
| Shared storage suite (phase 4a) | `RegistrationStoreContract` (every store) and `PersistentRegistrationStoreContract` (folder-based stores: restart, two instances on one folder, several processes). The in-memory store runs the first only. Multi-process runs use `BlazorApp.StoreHelper`, started as separate `dotnet` processes that open the store by strategy name, wait for a go-file, run a read-modify-write loop and print every acknowledged save; the suite checks every acknowledged marker is in the data at the acknowledged version, histories are versions 1..N, and the processes overlapped in time. Strategy A sets `ExpectsLostUpdates`, which flips the concurrency assertions. Known answers in 4a: a test-only locked-folder store passes everything (positive control), and a test-only store that fakes every fifth save is reported with exactly the expected number of lost updates (negative control). These two control stores live in the test project and are not strategies under study. **Changed in 4b (strategy A):** a store call that throws `JsonException`/`EndOfStreamException` counts as a *broken read*: the workload retries it (and stops a writer after one second of nothing but broken reads), the helper reports the count, and any broken read makes `NothingLost` false. A, which breaks its own file under concurrent writes, then passes the reversed tests on lost updates **or** broken reads, and B–E fail on any broken read instead of crashing. A third test-only store, `BrokenReadsTestStore`, is the known answer for it. Approved by the owner after the A branch stopped on it (`docs/strategies/NaiveFile.md`) |
| Test strategy | Gate per phase, all earlier tests rerun, complete rule tables, known-answer checks for the stress tool and the shared storage tests (sections 5–6) |
| Logging (phase 4L) | Built-in logging with a JSON-lines file per server; client events sent to the server; one record event per command with a fixed `EventId`; correlation id per user action; ids and field names only, never form values; diagnostics only, `History` stays the audit trail (section 8) |
| Logging as built (phase 4L) | Record events: `EventId` = action base (Create 1100, Update 1200, Submit 1300, Approve 1400, Reject 1500, Comment 1600, Delete 1700, View 1800) + outcome (Saved 0, NoUser 1, Forbidden 2, NotFound 3, Invalid 4, Conflict 5, WrongStatus 6, and **Read 7** for a successful View, which saves nothing), e.g. `RecordUpdateConflict` 1205. An unexpected exception (a 500) gets no record event; ASP.NET logs it. Store events: `StoreLog` (StoreRead 2001, StoreList 2002, StoreWrite 2003, StoreConflict 2004, all Debug, with free-text detail), required of every store by a contract test; `IRegistrationStoreFactory.Create(path, ILoggerFactory)`. Server lines: HttpRequest 3001 (route template, not path), ActingAsChanged 3101, UnknownUser 3102, ServerStarted 3201, ClientEvent 4001. Correlation id: `X-Correlation-Id`, 1–64 of `[A-Za-z0-9-_.]`, otherwise replaced; echoed in the response. File: `logs/server-<port>.jsonl` under the content root (`Logging:File:Directory`, `Logging:File:Enabled`); port 0 adds the process id; servers without a configured port (in-memory test servers) write no file. Client events: `PageLoaded` (after the fetch, with what was shown), `ButtonPressed`, `ResultShown`, `ApiFailed`; batches every second or 20 events, flushed before the "Acting as" reload; text from the client is cut to 100 characters with control characters replaced |
| Experiment, local disk (phase 5) | `webapp/scripts/run-cluster.sh <n> <strategy> [data]` for manual multi-server testing. Stress runs at two sizes (light: 3 servers × 5 clients × 40 changes, 6 records; heavy: 3 × 8 × 200, 12 records), seeds 1–3, all on one commit with a clean tree; each run's `result.json` and `summary.md` kept in `docs/experiments/local-disk/`. Known answer: as in the shared suite (#18), the control's expected failure is **lost updates or a broken file**; A failed 6/6 (4 with lost updates, 2 with a permanently unreadable file), E passed 6/6. Results in `docs/experiments/local-disk.md` |
| Contention sweep (phase 5) | Added to phase 5 to vary how many clients share each record. A registrant may only view and change its own records, so the stress tool no longer raises `--records` to the number of registrants: with fewer records, the surplus registrant users are made reviewers. Every client keeps writing, so the number of clients at work stays the same; the role mix, and so the mix of actions, shifts towards reviewers and is recorded as `ClientsByRole`. A registrant whose create failed sits out instead of falling back to others' records (which it may not view). With at least as many records as registrants, nothing changes, so the phase 5 runs stay comparable. Run on `f161ef4`: 3 × 8 clients on 1–96 records and 5 × 8 on 20, seeds 1–3, 105 runs. B–E passed all 84, including every client on one record; A failed all 21. B stays at about 50 saves/s whatever the records; C scales from B's level on one record to 914 saves/s on 96; E levels off at about 240. Results in `docs/experiments/local-disk.md` |
| Network drive, tool (phase 6) | Servers on a second machine: `--remote <user@host> --remote-servers <n> --remote-data <folder>` starts the last *n* servers there over ssh, on its own path to the same share; the tool copies the built Server (so both machines run one commit), stops them by process id and copies their logs back; clients and every check stay on the first machine (`StressTool.md`). Share: Synology NAS nas.example, `/volume1/share`, NFSv3 `hard`, locks sent to the NAS (NLM), all users mapped to one NAS user. Machines: `linux-host` (Debian 12, 16 CPUs) and `mac-host` (macOS 26, Apple silicon, 10 CPUs, .NET 10.0.12 runtime in `~/.dotnet`) |
| Experiment, network drive (phase 6) | Phase 5's light and heavy runs (seeds 1–3, all strategies) on the NFS share, on commit `1b49659`: with all 3 servers on one Linux machine, A failed 6/6 and E passed 6/6 (known answer), and B–E passed every run, 2.4–11× slower than local disk. With 1 of 3 servers on a Mac, every strategy failed: B broke its file or lost changes, C and D lost acknowledged changes (C's heavy run hung, its Linux servers locked out), E couldn't start (WAL needs one machine). Cause, confirmed with separate tests: the macOS NFS client reads stale data under a lock, its `flock` doesn't exclude handles within one process, and it reports a busy lock as errno 35 (the strategies retry only on Linux's 11). Two Linux machines, NFSv4, SMB and `noac` weren't tested. Results in `docs/experiments/network-drive.md` |
| Windows and SMB (phase 6b) | Before moving away from a share, test the setup production would use: 2 Windows Server machines, ≤10 clients, a creator → responder pipeline. SMB differs from NFSv3 where phase 6 failed: the file server enforces share-mode and byte-range locks for every handle, and leases keep file contents coherent between clients. The Windows client still caches directory listings, file info and "not found" for a few seconds. Test machines: two Windows Server evaluation VMs (180 days) on linux-host. Share layouts: the NAS over SMB, and one VM sharing its own folder; a dedicated Windows file server is left out because, for locking, it behaves like the VM share. Where the production share will be hosted is still open. Alternative architectures (rqlite, SQLite + Litestream) wait until 6b has a result |
| Rename on Windows (phase 6b) | B and C save by renaming a temp file over the old one. Windows refuses that rename while any other handle has the old file open (another server reading it, an SMB client holding its handle, perhaps an antivirus scan); W0 and W2 showed it as 500s to the user, mostly in C, with nothing lost. The owner chose to add a retry, for up to a second at first and 3 s after the leasing comparison (`Storage/FileReplace.cs`, logged as `StoreReplaceRetried`), and rerun B and C on W0 and W2 to see whether the errors go (narrowed to C by the production requirement row below). The runs before the change stay in the results as the "before" |
| Failure tests and soak (phase 6b) | Added by the owner after W2, where B, C and D lost nothing over SMB on a perfect network with no failures: a server killed mid-run on each machine, and ws2's network cut for 10 s and 60 s, all under the realistic load; then one hour of that load for each of B, C and D (narrowed to C by the production requirement row below). Shared-folder setups usually break on failures (a lock held by a machine that is cut off), not on load |
| Production requirement (phase 6b) | Owner, 2026-10-18: about 2,000 records a year, each updated 5–10 times, kept 3 years (~6,000 records). Each record must have its own file(s) on the share; one file holding every record is not workable in production. That rules out B (one data file) and D (one shared log); C (a file per record) is the candidate, with A and E kept only as controls. The retry rerun, the failure tests and the soak run C only. Added: a **volume test**, the share filled with ~6,000 records before the realistic load, measuring save, read and list times, since C's list reads every record file |
| Share leasing (phase 6b) | C on W2 with the retry, leasing on vs off on the share (`Set-SmbShare -LeasingMode`), 6 heavy and 3 realistic runs each: with leasing on, 2 of 6 heavy runs froze for 24–34 s (a rename waited 35.5 s, the SMB lease-break timeout; no server saved meanwhile); with it off, no freeze, but about 3× the retries, half the saves per second and double the p95 under heavy load, and 0–10 saves per heavy run still gave up after 1 s. Both were clean under the realistic load (no errors, no pause over 1.4 s). The owner chose leasing off as the production candidate, and the retry limit raised to 3 s; the failure tests, volume test and soak run that way |
| List cache for C (phase 6b) | The volume test's first trial (W2, 6,000 records) found that C's list read every record file: ~0.6 s on ws1, more than 60 s on ws2 over SMB (every list timed out). Of an archive folder, a list cache per server and an index file, the owner chose the cache: the list re-reads only files whose creation time, write time or size changed, lists arriving together share one refresh, and single reads and writes still read the file. Warm lists: 33 ms on ws1, 127 ms on ws2. The first list after a server start still reads everything (~21 s on ws1, ~53 s on ws2 on 2-processor VMs) and slows other requests meanwhile; the owner accepted that rather than spend more on it now |
| Methodology document | `docs/methodology.md` describes the experiment on its own, so that it can be repeated in another environment with only the repository: question, hypotheses and their status, checks, load profiles, environment requirements, procedure, results and limitations. It is updated in the same PR as any change to those, and is part of every phase's gate (decided 2026-10-18) |
| Flaky control race (after phase 6b) | `Many_threads_from_the_same_version_exactly_one_wins` sometimes failed for strategy A (the control) when every test project ran at once: its 32 threads ran one after another, so A happened not to lose anything. On 2026-10-18 it failed in 2 of 5 full test runs on linux-host (16 CPUs) and in none of 9 runs of its project alone. Approved by the owner as a change to the fixed shared suite: a store that `ExpectsLostUpdates` gets up to 5 rounds, each on a fresh folder, and must fail in at least one; a correct store still gets one round with the same assertions. Afterwards 6 of 6 full runs passed. Spot-check: A made safe with a lock failed all 5 rounds, so the test still catches it. The rounds aren't fully independent (a busy machine stays busy); a failure now lists each round's answers, to show whether the threads overlapped |
| Checks and reviews | CI on every push and PR from phase 1; `/code-review high` in every phase gate; Claude GitHub Action reviews optional for 4b PRs (section 7) |

## Needed before phase 6

- A network share to test against: NFS or SMB, where it's hosted, and whether a second
  machine is available to run servers on. Not needed until phase 6, so it doesn't
  block phases 1–5.
