# Stress run: PerRecordFile, 2 server(s): PASSED

- Commit `22febeb1b9f435203813da53e455263d96f64aea`, 2026-10-18 16:28:56 UTC, 129.8 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 5 clients, pipeline with 1000 ms think time, 120 changes per client, seed 1
- Clients by role: 4 Registrant, 4 Reviewer, 2 Admin
- Data folder `stress-runs/win-f2-w2-real-PerRecordFile-1/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-f2-w2-real-PerRecordFile-1`
- 1 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-f2-w2-real-PerRecordFile-1` there
- 1 of the servers on `Administrator@ws2.example`, data folder `\\ws1.example\share\runs\win-f2-w2-real-PerRecordFile-1` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 681 | 494 | 3.8 | 27 | 0 | 0 | 38.3 | 84.4 | 335.3 |

Writes by outcome: conflict 27, refused 160, saved 494. Saves by action: Create 108, Update 106, Submit 107, Approve 83, Reject 24, Comment 66. Reads: 507 (refused 91).

Longest stall: 1.3 s with no save acknowledged, from 68.7 s into the run (think time 1000 ms, so pauses up to about 1.5 s are normal).

Stale reads: 0 (older than a version already acknowledged); 55 reads followed a save through another server.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 108/108 | 0 | 0 | 0 | 0 | 0 |
| server ws1.example:46867 | skipped: killed by --fault | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server ws2.example:36023 | pass | 108/108 | 0 | 0 | 0 | 0 | 0 |

Faults:
- kill server 1 at 40 s: at 40.0 s, server ws1.example:46867 killed

Lock check: pass: 108 of 108 record and server pairs could still be changed, 0 stuck, 0 unreadable.

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 2 | 494 | 3.8 | 27 | 0 | 0 | 0 | 0 | 84.4 | yes |
