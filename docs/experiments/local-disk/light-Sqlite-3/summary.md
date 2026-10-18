# Stress run: Sqlite, 3 server(s): PASSED

- Commit `ee6af83bef739621009cd4be91192b48f8df58df`, 2026-10-17 19:08:26 UTC, 2.2 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 5 clients, 6 records, 40 changes per client, seed 3
- Data folder `stress-runs/light-Sqlite-3/data`; run folder `stress-runs/light-Sqlite-3`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 606 | 354 | 160.7 | 158 | 94 | 0 | 13.4 | 28.5 | 179.5 |

Writes by outcome: conflict 158, saved 354, wrongStatus 94. Saves by action: Create 6, Update 151, Submit 6, Approve 5, Reject 1, Comment 185. Reads: 600.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 44167 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 42681 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 44627 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| Sqlite | 3 | 354 | 160.7 | 158 | 94 | 0 | 0 | 0 | 28.5 | yes |
