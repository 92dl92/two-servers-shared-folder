# Stress run: Sqlite, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:43:48 UTC, 9.5 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 12 records, 200 changes per client, seed 3
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r12-Sqlite-3/data`; run folder `stress-runs/sweep/sweep-3x8-r12-Sqlite-3`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4812 | 2174 | 229.5 | 683 | 1955 | 0 | 14.5 | 28.1 | 183.8 |

Writes by outcome: conflict 683, saved 2174, wrongStatus 1955. Saves by action: Create 12, Update 472, Submit 12, Approve 7, Reject 5, Comment 1666. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 44017 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 43797 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 39265 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| Sqlite | 3 | 2174 | 229.5 | 683 | 1955 | 0 | 0 | 0 | 28.1 | yes |
