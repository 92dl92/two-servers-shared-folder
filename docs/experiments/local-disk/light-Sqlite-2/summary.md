# Stress run: Sqlite, 3 server(s): PASSED

- Commit `ee6af83bef739621009cd4be91192b48f8df58df`, 2026-10-17 19:08:22 UTC, 1.9 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 5 clients, 6 records, 40 changes per client, seed 2
- Data folder `stress-runs/light-Sqlite-2/data`; run folder `stress-runs/light-Sqlite-2`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 606 | 300 | 158.8 | 131 | 175 | 0 | 10.8 | 31.2 | 180.4 |

Writes by outcome: conflict 131, saved 300, wrongStatus 175. Saves by action: Create 6, Update 67, Submit 6, Approve 4, Reject 2, Comment 215. Reads: 600.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 33287 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 34011 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 38779 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| Sqlite | 3 | 300 | 158.8 | 131 | 175 | 0 | 0 | 0 | 31.2 | yes |
