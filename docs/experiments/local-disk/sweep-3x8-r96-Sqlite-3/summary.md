# Stress run: Sqlite, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:55:34 UTC, 13.1 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 96 records, 200 changes per client, seed 3
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r96-Sqlite-3/data`; run folder `stress-runs/sweep/sweep-3x8-r96-Sqlite-3`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4896 | 3229 | 246.6 | 234 | 1433 | 0 | 22.6 | 40.1 | 2183.4 |

Writes by outcome: conflict 234, saved 3229, wrongStatus 1433. Saves by action: Create 96, Update 974, Submit 96, Approve 51, Reject 44, Comment 1968. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 96/96 | 0 | 0 | 0 | 0 | 0 |
| server 33913 | pass | 96/96 | 0 | 0 | 0 | 0 | 0 |
| server 35415 | pass | 96/96 | 0 | 0 | 0 | 0 | 0 |
| server 44105 | pass | 96/96 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| Sqlite | 3 | 3229 | 246.6 | 234 | 1433 | 0 | 0 | 0 | 40.1 | yes |
