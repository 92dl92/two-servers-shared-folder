# Stress run: PerRecordFile, 2 server(s): PASSED

- Commit `b3d3ed79bafe0240c50f2f8396ba3e9f87c53ac4`, 2026-10-18 19:24:53 UTC, 165.0 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 5 clients, pipeline with 1000 ms think time, 120 changes per client, seed 2
- Clients by role: 4 Registrant, 4 Reviewer, 2 Admin
- Data folder `stress-runs/win-vol-w2-real-PerRecordFile-2/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-vol-w2-real-PerRecordFile-2`
- 1 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-vol-w2-real-PerRecordFile-2` there
- 1 of the servers on `Administrator@ws2.example`, data folder `\\ws1.example\share\runs\win-vol-w2-real-PerRecordFile-2` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 754 | 738 | 4.5 | 16 | 0 | 0 | 15.2 | 153.8 | 15312.4 |

Read p50/p95/max ms: 1.8 / 44.4 / 2777.1. List p50/p95/max ms (328 lists): 64.1 / 235.8 / 51997.0. Prefilled with 6000 records.

Writes by outcome: conflict 16, saved 738. Saves by action: Create 159, Update 166, Submit 159, Approve 111, Reject 48, Comment 95. Reads: 597.

Longest stall: 9.1 s with no save acknowledged, from 14.3 s into the run (think time 1000 ms, so pauses up to about 1.5 s are normal).

Stale reads: 0 (older than a version already acknowledged); 125 reads followed a save through another server.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 6159/6159 | 0 | 0 | 0 | 0 | 0 |
| server ws1.example:33213 | pass | 6159/6159 | 0 | 0 | 0 | 0 | 0 |
| server ws2.example:44817 | pass | 6159/6159 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 2 | 738 | 4.5 | 16 | 0 | 0 | 0 | 0 | 153.8 | yes |
