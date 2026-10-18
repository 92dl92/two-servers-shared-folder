# Stress run: Sqlite, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:40:42 UTC, 7.4 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 3 records, 200 changes per client, seed 3
- Clients by role: 3 Registrant, 18 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r03-Sqlite-3/data`; run folder `stress-runs/sweep/sweep-3x8-r03-Sqlite-3`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4803 | 1151 | 156.1 | 3042 | 610 | 0 | 11.6 | 22.1 | 133.4 |

Writes by outcome: conflict 3042, saved 1151, wrongStatus 610. Saves by action: Create 3, Update 14, Submit 3, Approve 2, Reject 1, Comment 1128. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 3/3 | 0 | 0 | 0 | 0 | 0 |
| server 37437 | pass | 3/3 | 0 | 0 | 0 | 0 | 0 |
| server 44861 | pass | 3/3 | 0 | 0 | 0 | 0 | 0 |
| server 46529 | pass | 3/3 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| Sqlite | 3 | 1151 | 156.1 | 3042 | 610 | 0 | 0 | 0 | 22.1 | yes |
