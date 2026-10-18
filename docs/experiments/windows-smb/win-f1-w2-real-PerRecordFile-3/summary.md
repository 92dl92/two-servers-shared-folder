# Stress run: PerRecordFile, 2 server(s): PASSED

- Commit `22febeb1b9f435203813da53e455263d96f64aea`, 2026-10-18 16:26:19 UTC, 125.7 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 5 clients, pipeline with 1000 ms think time, 120 changes per client, seed 3
- Clients by role: 4 Registrant, 4 Reviewer, 2 Admin
- Data folder `stress-runs/win-f1-w2-real-PerRecordFile-3/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-f1-w2-real-PerRecordFile-3`
- 1 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-f1-w2-real-PerRecordFile-3` there
- 1 of the servers on `Administrator@ws2.example`, data folder `\\ws1.example\share\runs\win-f1-w2-real-PerRecordFile-3` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 652 | 479 | 3.8 | 13 | 0 | 1 | 8.9 | 49.8 | 304.4 |

Writes by outcome: conflict 13, noResponse 1, refused 159, saved 479. Saves by action: Create 113, Update 95, Submit 111, Approve 85, Reject 26, Comment 49. Reads: 444 (refused 63).

Longest stall: 1.4 s with no save acknowledged, from 124.3 s into the run (think time 1000 ms, so pauses up to about 1.5 s are normal).

Stale reads: 0 (older than a version already acknowledged); 45 reads followed a save through another server.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 113/113 | 0 | 0 | 0 | 1 | 0 |
| server ws1.example:36425 | pass | 113/113 | 0 | 0 | 0 | 1 | 0 |
| server ws2.example:36877 | skipped: killed by --fault | 0/0 | 0 | 0 | 0 | 0 | 0 |

Faults:
- kill server 2 at 40 s: at 40.0 s, server ws2.example:36877 killed

Lock check: pass: 113 of 113 record and server pairs could still be changed, 0 stuck, 0 unreadable.

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 2 | 479 | 3.8 | 13 | 0 | 0 | 0 | 0 | 49.8 | yes |
