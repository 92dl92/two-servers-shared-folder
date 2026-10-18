# Stress run: LockedFile, 5 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:56:10 UTC, 72.2 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 5 servers × 8 clients, 20 records, 200 changes per client, seed 1
- Clients by role: 20 Registrant, 15 Reviewer, 5 Admin
- Data folder `stress-runs/sweep/sweep-5x8-r20-LockedFile-1/data`; run folder `stress-runs/sweep/sweep-5x8-r20-LockedFile-1`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 8020 | 2996 | 41.5 | 1750 | 3274 | 0 | 15.7 | 427.5 | 1526.6 |

Writes by outcome: conflict 1750, saved 2996, wrongStatus 3274. Saves by action: Create 20, Update 124, Submit 20, Approve 11, Reject 9, Comment 2812. Reads: 8000.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |
| server 44273 | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |
| server 38579 | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |
| server 37955 | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |
| server 35099 | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |
| server 41329 | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| LockedFile | 5 | 2996 | 41.5 | 1750 | 3274 | 0 | 0 | 0 | 427.5 | yes |
