# Stress run: Sqlite, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:47:10 UTC, 10.2 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 24 records, 200 changes per client, seed 3
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r24-Sqlite-3/data`; run folder `stress-runs/sweep/sweep-3x8-r24-Sqlite-3`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4824 | 2372 | 231.7 | 447 | 2005 | 0 | 14.9 | 30.2 | 447.5 |

Writes by outcome: conflict 447, saved 2372, wrongStatus 2005. Saves by action: Create 24, Update 416, Submit 24, Approve 16, Reject 8, Comment 1884. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 24/24 | 0 | 0 | 0 | 0 | 0 |
| server 35247 | pass | 24/24 | 0 | 0 | 0 | 0 | 0 |
| server 46033 | pass | 24/24 | 0 | 0 | 0 | 0 | 0 |
| server 37573 | pass | 24/24 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| Sqlite | 3 | 2372 | 231.7 | 447 | 2005 | 0 | 0 | 0 | 30.2 | yes |
