# Stress run: Sqlite, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:40:23 UTC, 7.7 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 3 records, 200 changes per client, seed 1
- Clients by role: 3 Registrant, 18 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r03-Sqlite-1/data`; run folder `stress-runs/sweep/sweep-3x8-r03-Sqlite-1`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4803 | 1221 | 158.9 | 2955 | 627 | 0 | 11.4 | 22.6 | 126.9 |

Writes by outcome: conflict 2955, saved 1221, wrongStatus 627. Saves by action: Create 3, Update 10, Submit 3, Approve 1, Reject 2, Comment 1202. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 3/3 | 0 | 0 | 0 | 0 | 0 |
| server 35361 | pass | 3/3 | 0 | 0 | 0 | 0 | 0 |
| server 33445 | pass | 3/3 | 0 | 0 | 0 | 0 | 0 |
| server 33333 | pass | 3/3 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| Sqlite | 3 | 1221 | 158.9 | 2955 | 627 | 0 | 0 | 0 | 22.6 | yes |
