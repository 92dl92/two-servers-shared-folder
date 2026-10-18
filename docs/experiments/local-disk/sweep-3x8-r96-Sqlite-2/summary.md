# Stress run: Sqlite, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:55:18 UTC, 13.4 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 96 records, 200 changes per client, seed 2
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r96-Sqlite-2/data`; run folder `stress-runs/sweep/sweep-3x8-r96-Sqlite-2`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4896 | 3288 | 245.4 | 230 | 1378 | 0 | 24.3 | 38.2 | 1054.6 |

Writes by outcome: conflict 230, saved 3288, wrongStatus 1378. Saves by action: Create 96, Update 1032, Submit 96, Approve 49, Reject 45, Comment 1970. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 96/96 | 0 | 0 | 0 | 0 | 0 |
| server 33017 | pass | 96/96 | 0 | 0 | 0 | 0 | 0 |
| server 46627 | pass | 96/96 | 0 | 0 | 0 | 0 | 0 |
| server 43645 | pass | 96/96 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| Sqlite | 3 | 3288 | 245.4 | 230 | 1378 | 0 | 0 | 0 | 38.2 | yes |
