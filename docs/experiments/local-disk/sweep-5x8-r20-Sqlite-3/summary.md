# Stress run: Sqlite, 5 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 21:01:45 UTC, 16.8 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 5 servers × 8 clients, 20 records, 200 changes per client, seed 3
- Clients by role: 20 Registrant, 15 Reviewer, 5 Admin
- Data folder `stress-runs/sweep/sweep-5x8-r20-Sqlite-3/data`; run folder `stress-runs/sweep/sweep-5x8-r20-Sqlite-3`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 8020 | 3343 | 199.2 | 959 | 3718 | 0 | 14.8 | 45.7 | 366.9 |

Writes by outcome: conflict 959, saved 3343, wrongStatus 3718. Saves by action: Create 20, Update 242, Submit 20, Approve 9, Reject 11, Comment 3041. Reads: 8000.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |
| server 39953 | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |
| server 33121 | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |
| server 44277 | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |
| server 40401 | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |
| server 33411 | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| Sqlite | 5 | 3343 | 199.2 | 959 | 3718 | 0 | 0 | 0 | 45.7 | yes |
