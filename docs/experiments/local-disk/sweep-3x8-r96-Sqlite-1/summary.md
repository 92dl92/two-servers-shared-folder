# Stress run: Sqlite, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:55:02 UTC, 13.5 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 96 records, 200 changes per client, seed 1
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r96-Sqlite-1/data`; run folder `stress-runs/sweep/sweep-3x8-r96-Sqlite-1`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4896 | 3309 | 244.7 | 178 | 1409 | 0 | 24.0 | 37.3 | 345.4 |

Writes by outcome: conflict 178, saved 3309, wrongStatus 1409. Saves by action: Create 96, Update 1017, Submit 96, Approve 54, Reject 41, Comment 2005. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 96/96 | 0 | 0 | 0 | 0 | 0 |
| server 42589 | pass | 96/96 | 0 | 0 | 0 | 0 | 0 |
| server 42337 | pass | 96/96 | 0 | 0 | 0 | 0 | 0 |
| server 37947 | pass | 96/96 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| Sqlite | 3 | 3309 | 244.7 | 178 | 1409 | 0 | 0 | 0 | 37.3 | yes |
