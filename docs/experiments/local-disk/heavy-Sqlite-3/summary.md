# Stress run: Sqlite, 3 server(s): PASSED

- Commit `ee6af83bef739621009cd4be91192b48f8df58df`, 2026-10-17 19:11:50 UTC, 9.2 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 12 records, 200 changes per client, seed 3
- Data folder `stress-runs/heavy-Sqlite-3/data`; run folder `stress-runs/heavy-Sqlite-3`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4812 | 2061 | 222.9 | 689 | 2062 | 0 | 14.1 | 27.6 | 200.0 |

Writes by outcome: conflict 689, saved 2061, wrongStatus 2062. Saves by action: Create 12, Update 358, Submit 12, Approve 7, Reject 5, Comment 1667. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 38317 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 41157 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 45391 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| Sqlite | 3 | 2061 | 222.9 | 689 | 2062 | 0 | 0 | 0 | 27.6 | yes |
