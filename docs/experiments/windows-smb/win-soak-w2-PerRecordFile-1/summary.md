# Stress run: PerRecordFile, 2 server(s): PASSED

- Commit `7d9ce5987d3760f67fe5124fd2a5ca40fdae60e7`, 2026-10-18 19:41:39 UTC, 3544.4 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 5 clients, pipeline with 1000 ms think time, 3300 changes per client, seed 1
- Clients by role: 4 Registrant, 4 Reviewer, 2 Admin
- Data folder `stress-runs/win-soak-w2-PerRecordFile-1/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-soak-w2-PerRecordFile-1`
- 1 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-soak-w2-PerRecordFile-1` there
- 1 of the servers on `Administrator@ws2.example`, data folder `\\ws1.example\share\runs\win-soak-w2-PerRecordFile-1` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 20321 | 19828 | 5.6 | 493 | 0 | 0 | 20.5 | 63.0 | 27121.3 |

Read p50/p95/max ms: 3.8 / 29.6 / 18810.9. List p50/p95/max ms (8948 lists): 123.9 / 303.8 / 58790.2. Prefilled with 6000 records.

Writes by outcome: conflict 493, saved 19828. Saves by action: Create 4401, Update 4398, Submit 4401, Approve 3312, Reject 1089, Comment 2227. Reads: 15968.

Longest stall: 6.4 s with no save acknowledged, from 3.4 s into the run (think time 1000 ms, so pauses up to about 1.5 s are normal).

Stale reads: 0 (older than a version already acknowledged); 3619 reads followed a save through another server.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 10401/10401 | 0 | 0 | 0 | 0 | 0 |
| server ws1.example:37745 | pass | 10401/10401 | 0 | 0 | 0 | 0 | 0 |
| server ws2.example:39493 | pass | 10401/10401 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 2 | 19828 | 5.6 | 493 | 0 | 0 | 0 | 0 | 63.0 | yes |
