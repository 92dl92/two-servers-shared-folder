# Stress run: Sqlite, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:46:45 UTC, 10.6 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 24 records, 200 changes per client, seed 1
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r24-Sqlite-1/data`; run folder `stress-runs/sweep/sweep-3x8-r24-Sqlite-1`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4824 | 2459 | 232.8 | 382 | 1983 | 0 | 15.8 | 35.2 | 263.6 |

Writes by outcome: conflict 382, saved 2459, wrongStatus 1983. Saves by action: Create 24, Update 478, Submit 24, Approve 12, Reject 12, Comment 1909. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 24/24 | 0 | 0 | 0 | 0 | 0 |
| server 41987 | pass | 24/24 | 0 | 0 | 0 | 0 | 0 |
| server 35587 | pass | 24/24 | 0 | 0 | 0 | 0 | 0 |
| server 42139 | pass | 24/24 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| Sqlite | 3 | 2459 | 232.8 | 382 | 1983 | 0 | 0 | 0 | 35.2 | yes |
