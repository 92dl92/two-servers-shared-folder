# Stress run: PerRecordFile, 2 server(s): PASSED

- Commit `b3d3ed79bafe0240c50f2f8396ba3e9f87c53ac4`, 2026-10-18 19:32:47 UTC, 169.3 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 5 clients, pipeline with 1000 ms think time, 120 changes per client, seed 3
- Clients by role: 4 Registrant, 4 Reviewer, 2 Admin
- Data folder `stress-runs/win-vol-w2-real-PerRecordFile-3/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-vol-w2-real-PerRecordFile-3`
- 1 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-vol-w2-real-PerRecordFile-3` there
- 1 of the servers on `Administrator@ws2.example`, data folder `\\ws1.example\share\runs\win-vol-w2-real-PerRecordFile-3` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 756 | 737 | 4.4 | 19 | 0 | 0 | 14.7 | 71.3 | 19949.3 |

Read p50/p95/max ms: 1.8 / 27.5 / 18777.2. List p50/p95/max ms (323 lists): 64.1 / 239.4 / 46241.9. Prefilled with 6000 records.

Writes by outcome: conflict 19, saved 737. Saves by action: Create 168, Update 149, Submit 168, Approve 127, Reject 41, Comment 84. Reads: 588.

Longest stall: 6.7 s with no save acknowledged, from 15.5 s into the run (think time 1000 ms, so pauses up to about 1.5 s are normal).

Stale reads: 0 (older than a version already acknowledged); 118 reads followed a save through another server.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 6168/6168 | 0 | 0 | 0 | 0 | 0 |
| server ws1.example:44959 | pass | 6168/6168 | 0 | 0 | 0 | 0 | 0 |
| server ws2.example:35937 | pass | 6168/6168 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 2 | 737 | 4.4 | 19 | 0 | 0 | 0 | 0 | 71.3 | yes |
