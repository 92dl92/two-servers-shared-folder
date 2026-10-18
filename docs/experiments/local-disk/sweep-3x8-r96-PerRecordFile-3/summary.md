# Stress run: PerRecordFile, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:54:37 UTC, 3.2 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 96 records, 200 changes per client, seed 3
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r96-PerRecordFile-3/data`; run folder `stress-runs/sweep/sweep-3x8-r96-PerRecordFile-3`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4896 | 2850 | 894.0 | 346 | 1700 | 0 | 10.8 | 19.9 | 150.1 |

Writes by outcome: conflict 346, saved 2850, wrongStatus 1700. Saves by action: Create 96, Update 660, Submit 96, Approve 50, Reject 46, Comment 1902. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 96/96 | 0 | 0 | 0 | 0 | 0 |
| server 39973 | pass | 96/96 | 0 | 0 | 0 | 0 | 0 |
| server 39741 | pass | 96/96 | 0 | 0 | 0 | 0 | 0 |
| server 34583 | pass | 96/96 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 3 | 2850 | 894.0 | 346 | 1700 | 0 | 0 | 0 | 19.9 | yes |
