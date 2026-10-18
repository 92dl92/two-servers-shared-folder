# Stress run: Sqlite, 3 server(s): PASSED

- Commit `ee6af83bef739621009cd4be91192b48f8df58df`, 2026-10-17 19:11:25 UTC, 8.9 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 12 records, 200 changes per client, seed 1
- Data folder `stress-runs/heavy-Sqlite-1/data`; run folder `stress-runs/heavy-Sqlite-1`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4812 | 2009 | 224.8 | 691 | 2112 | 0 | 13.6 | 28.6 | 192.1 |

Writes by outcome: conflict 691, saved 2009, wrongStatus 2112. Saves by action: Create 12, Update 309, Submit 12, Approve 8, Reject 4, Comment 1664. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 35809 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 40777 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 38671 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| Sqlite | 3 | 2009 | 224.8 | 691 | 2112 | 0 | 0 | 0 | 28.6 | yes |
