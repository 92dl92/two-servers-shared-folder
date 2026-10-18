# Stress run: PerRecordFile, 2 server(s): PASSED

- Commit `22febeb1b9f435203813da53e455263d96f64aea`, 2026-10-18 16:23:42 UTC, 125.4 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 5 clients, pipeline with 1000 ms think time, 120 changes per client, seed 2
- Clients by role: 4 Registrant, 4 Reviewer, 2 Admin
- Data folder `stress-runs/win-f1-w2-real-PerRecordFile-2/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-f1-w2-real-PerRecordFile-2`
- 1 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-f1-w2-real-PerRecordFile-2` there
- 1 of the servers on `Administrator@ws2.example`, data folder `\\ws1.example\share\runs\win-f1-w2-real-PerRecordFile-2` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 658 | 487 | 3.9 | 9 | 0 | 1 | 9.4 | 53.2 | 171.5 |

Writes by outcome: conflict 9, noResponse 1, refused 161, saved 487. Saves by action: Create 107, Update 106, Submit 105, Approve 74, Reject 31, Comment 64. Reads: 469 (refused 78).

Longest stall: 3.8 s with no save acknowledged, from 121.7 s into the run (think time 1000 ms, so pauses up to about 1.5 s are normal).

Stale reads: 0 (older than a version already acknowledged); 54 reads followed a save through another server.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 107/107 | 0 | 0 | 0 | 0 | 0 |
| server ws1.example:36655 | pass | 107/107 | 0 | 0 | 0 | 0 | 0 |
| server ws2.example:42387 | skipped: killed by --fault | 0/0 | 0 | 0 | 0 | 0 | 0 |

Faults:
- kill server 2 at 40 s: at 40.0 s, server ws2.example:42387 killed

Lock check: pass: 107 of 107 record and server pairs could still be changed, 0 stuck, 0 unreadable.

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 2 | 487 | 3.9 | 9 | 0 | 0 | 0 | 0 | 53.2 | yes |
