# Stress run: PerRecordFile, 2 server(s): PASSED

- Commit `b3d3ed79bafe0240c50f2f8396ba3e9f87c53ac4`, 2026-10-18 19:16:42 UTC, 175.1 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 5 clients, pipeline with 1000 ms think time, 120 changes per client, seed 1
- Clients by role: 4 Registrant, 4 Reviewer, 2 Admin
- Data folder `stress-runs/win-vol-w2-real-PerRecordFile-1/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-vol-w2-real-PerRecordFile-1`
- 1 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-vol-w2-real-PerRecordFile-1` there
- 1 of the servers on `Administrator@ws2.example`, data folder `\\ws1.example\share\runs\win-vol-w2-real-PerRecordFile-1` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 745 | 726 | 4.2 | 19 | 0 | 0 | 24.5 | 74.4 | 25567.5 |

Read p50/p95/max ms: 4.5 / 35.6 / 14862.5. List p50/p95/max ms (318 lists): 99.3 / 244.6 / 53114.2. Prefilled with 6000 records.

Writes by outcome: conflict 19, saved 726. Saves by action: Create 164, Update 154, Submit 164, Approve 118, Reject 46, Comment 80. Reads: 584.

Longest stall: 7.6 s with no save acknowledged, from 2.3 s into the run (think time 1000 ms, so pauses up to about 1.5 s are normal).

Stale reads: 0 (older than a version already acknowledged); 136 reads followed a save through another server.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 6164/6164 | 0 | 0 | 0 | 0 | 0 |
| server ws1.example:43857 | pass | 6164/6164 | 0 | 0 | 0 | 0 | 0 |
| server ws2.example:38745 | pass | 6164/6164 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 2 | 726 | 4.2 | 19 | 0 | 0 | 0 | 0 | 74.4 | yes |
