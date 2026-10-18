# Stress run: LockedFile, 3 server(s): PASSED

- Commit `ee6af83bef739621009cd4be91192b48f8df58df`, 2026-10-17 19:07:49 UTC, 3.8 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 5 clients, 6 records, 40 changes per client, seed 3
- Data folder `stress-runs/light-LockedFile-3/data`; run folder `stress-runs/light-LockedFile-3`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 606 | 279 | 72.5 | 158 | 169 | 0 | 8.9 | 115.3 | 267.9 |

Writes by outcome: conflict 158, saved 279, wrongStatus 169. Saves by action: Create 6, Update 43, Submit 6, Approve 4, Reject 2, Comment 218. Reads: 600.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 33635 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 41967 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 41111 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| LockedFile | 3 | 279 | 72.5 | 158 | 169 | 0 | 0 | 0 | 115.3 | yes |
