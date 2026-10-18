# Stress run: Sqlite, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:40:33 UTC, 7.2 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 3 records, 200 changes per client, seed 2
- Clients by role: 3 Registrant, 18 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r03-Sqlite-2/data`; run folder `stress-runs/sweep/sweep-3x8-r03-Sqlite-2`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4803 | 1055 | 147.4 | 3108 | 640 | 0 | 12.0 | 23.8 | 132.4 |

Writes by outcome: conflict 3108, saved 1055, wrongStatus 640. Saves by action: Create 3, Update 11, Submit 3, Approve 2, Reject 1, Comment 1035. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 3/3 | 0 | 0 | 0 | 0 | 0 |
| server 37685 | pass | 3/3 | 0 | 0 | 0 | 0 | 0 |
| server 34177 | pass | 3/3 | 0 | 0 | 0 | 0 | 0 |
| server 38851 | pass | 3/3 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| Sqlite | 3 | 1055 | 147.4 | 3108 | 640 | 0 | 0 | 0 | 23.8 | yes |
