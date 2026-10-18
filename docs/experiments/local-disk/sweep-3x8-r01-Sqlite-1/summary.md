# Stress run: Sqlite, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:36:56 UTC, 5.9 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 1 records, 200 changes per client, seed 1
- Clients by role: 1 Registrant, 20 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r01-Sqlite-1/data`; run folder `stress-runs/sweep/sweep-3x8-r01-Sqlite-1`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4801 | 301 | 51.1 | 4280 | 220 | 0 | 14.1 | 28.9 | 149.3 |

Writes by outcome: conflict 4280, saved 301, wrongStatus 220. Saves by action: Create 1, Submit 1, Approve 1, Comment 298. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 1/1 | 0 | 0 | 0 | 0 | 0 |
| server 38183 | pass | 1/1 | 0 | 0 | 0 | 0 | 0 |
| server 37519 | pass | 1/1 | 0 | 0 | 0 | 0 | 0 |
| server 33341 | pass | 1/1 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| Sqlite | 3 | 301 | 51.1 | 4280 | 220 | 0 | 0 | 0 | 28.9 | yes |
