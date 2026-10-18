# Stress run: PerRecordFile, 2 server(s): PASSED

- Commit `22febeb1b9f435203813da53e455263d96f64aea`, 2026-10-18 16:34:38 UTC, 130.1 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 5 clients, pipeline with 1000 ms think time, 120 changes per client, seed 3
- Clients by role: 4 Registrant, 4 Reviewer, 2 Admin
- Data folder `stress-runs/win-f2-w2-real-PerRecordFile-3/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-f2-w2-real-PerRecordFile-3`
- 1 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-f2-w2-real-PerRecordFile-3` there
- 1 of the servers on `Administrator@ws2.example`, data folder `\\ws1.example\share\runs\win-f2-w2-real-PerRecordFile-3` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 682 | 497 | 3.8 | 27 | 0 | 0 | 36.6 | 71.1 | 529.9 |

Writes by outcome: conflict 27, refused 158, saved 497. Saves by action: Create 113, Update 100, Submit 112, Approve 84, Reject 28, Comment 60. Reads: 517 (refused 105).

Longest stall: 1.2 s with no save acknowledged, from 128.9 s into the run (think time 1000 ms, so pauses up to about 1.5 s are normal).

Stale reads: 0 (older than a version already acknowledged); 47 reads followed a save through another server.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 113/113 | 0 | 0 | 0 | 0 | 0 |
| server ws1.example:44955 | skipped: killed by --fault | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server ws2.example:40021 | pass | 113/113 | 0 | 0 | 0 | 0 | 0 |

Faults:
- kill server 1 at 40 s: at 40.0 s, server ws1.example:44955 killed

Lock check: pass: 113 of 113 record and server pairs could still be changed, 0 stuck, 0 unreadable.

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 2 | 497 | 3.8 | 27 | 0 | 0 | 0 | 0 | 71.1 | yes |
