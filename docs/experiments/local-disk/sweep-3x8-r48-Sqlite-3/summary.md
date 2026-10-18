# Stress run: Sqlite, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:51:01 UTC, 11.4 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 48 records, 200 changes per client, seed 3
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r48-Sqlite-3/data`; run folder `stress-runs/sweep/sweep-3x8-r48-Sqlite-3`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4848 | 2741 | 240.3 | 265 | 1842 | 0 | 17.7 | 34.1 | 607.8 |

Writes by outcome: conflict 265, saved 2741, wrongStatus 1842. Saves by action: Create 48, Update 634, Submit 48, Approve 24, Reject 24, Comment 1963. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 48/48 | 0 | 0 | 0 | 0 | 0 |
| server 35807 | pass | 48/48 | 0 | 0 | 0 | 0 | 0 |
| server 36565 | pass | 48/48 | 0 | 0 | 0 | 0 | 0 |
| server 33337 | pass | 48/48 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| Sqlite | 3 | 2741 | 240.3 | 265 | 1842 | 0 | 0 | 0 | 34.1 | yes |
