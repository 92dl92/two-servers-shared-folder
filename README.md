# Two servers, one shared folder, no database

Two app servers, one shared folder, no database: a file-per-record design that survives
concurrent writes on Windows SMB, with the tests that prove it.

This repository is a proof of concept and a recipe. It shows how several application
servers can keep their data as **one JSON file per record in a shared folder** (local disk,
NFS or an SMB share) without losing updates or leaving broken files, and how to prove that
for a given environment. The records are a toy (made-up registrations); the mechanism is
what is meant to be reused.

## Start here

**[`docs/handover-solution-c.md`](docs/handover-solution-c.md)** is written for an agent or
developer who has to apply this design to a real application on two Windows servers with an
SMB share: the evidence, the architecture and its rules, Windows/SMB specifics, what the
proof of concept did not solve, open questions, what to verify, and how to rerun the tests
in a new environment.

Then, as needed:

| What | Where |
|---|---|
| The method: hypotheses, checks, load profiles, how to repeat it anywhere, results | [`docs/methodology.md`](docs/methodology.md) |
| The design and caveats of each storage strategy (C is `PerRecordFile`) | [`docs/strategies/`](docs/strategies/) |
| The stress tool: what a run does, checks, failure tests | [`docs/strategies/StressTool.md`](docs/strategies/StressTool.md) |
| Running the tests where there is no ssh (planned) | [`docs/strategies/StressToolWindows.md`](docs/strategies/StressToolWindows.md) |
| Results: local disk, NFS and Windows SMB (with every run's `result.json` and `summary.md`) | [`docs/experiments/`](docs/experiments/) |
| The plan and decisions the proof of concept was built by | [`docs/plan.md`](docs/plan.md) |
| The code | [`webapp/`](webapp/) (see [`webapp/README.md`](webapp/README.md)) |

## The design in one paragraph

Each record lives in `records/<id>.json` and carries a version number. Every save takes an
exclusive lock on `locks/<id>.lock`, re-reads the record **under the lock**, refuses the save
(HTTP 409) if the version has moved on, writes the new version to a temp file, flushes it, and
renames it over the record in one step. Reads take no lock: files only ever appear complete.
On Windows the rename is retried briefly while another handle has the old file open. The
reference implementation is
[`webapp/src/BlazorApp.Services/Storage/PerRecordFile/PerRecordFileStore.cs`](webapp/src/BlazorApp.Services/Storage/PerRecordFile/PerRecordFileStore.cs).

## What was found

| Storage | Result for C (one file per record) |
|---|---|
| Local disk, several servers on one Linux machine | Nothing lost under any contention |
| NFS share, one Linux machine | Nothing lost, slower |
| NFS share, Linux + macOS clients | **Lost updates**: the macOS NFS client reads stale data under a lock |
| SMB share, two Windows Server machines (VMs) | Nothing lost in light, heavy, realistic, failure (server killed, network cut), 6,000-record and one-hour runs, with a rename retry and SMB leasing off |

A correct strategy is necessary but not sufficient: the file-sharing client on each machine
decides whether locks and version checks see the truth, so each production combination has
to be tested itself. The handover says how.

## Build and test

Needs the .NET 10 SDK. From the repository root:

```bash
dotnet build webapp/BlazorApp.slnx     # warnings count as errors
dotnet test  webapp/BlazorApp.slnx     # all test projects
```

The browser tests (`BlazorApp.E2E.Tests`) need Chromium once:
`pwsh webapp/tests/BlazorApp.E2E.Tests/bin/Debug/net10.0/playwright.ps1 install chromium`.

A stress run (three real server processes on one folder, colliding updates, then every
check):

```bash
dotnet run --project webapp/tools/BlazorApp.Stress -- --strategy PerRecordFile --servers 3 --clients 5 --ops 40 --seed 1
```

## Status

A snapshot of the proof of concept on 2026-10-18, after its Windows round (phase 6b). The
no-ssh test harness for locked-down Windows environments is designed but not built yet
(`docs/strategies/StressToolWindows.md`). Host names, addresses and paths from the original test machines
are replaced by placeholders (`linux-host`, `mac-host`, `nas.example`, `ws1.example`, ...).
The project was built with Claude Code; some docs refer to its working conventions.

## License

[MIT](LICENSE)
