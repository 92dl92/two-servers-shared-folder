# Stress run: LockedFile, 3 server(s): PASSED

- Commit `ee6af83bef739621009cd4be91192b48f8df58df`, 2026-10-17 19:08:46 UTC, 36.7 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 12 records, 200 changes per client, seed 1
- Data folder `stress-runs/heavy-LockedFile-1/data`; run folder `stress-runs/heavy-LockedFile-1`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4812 | 1847 | 50.4 | 963 | 2002 | 0 | 12.5 | 196.5 | 792.0 |

Writes by outcome: conflict 963, saved 1847, wrongStatus 2002. Saves by action: Create 12, Update 79, Submit 12, Approve 5, Reject 7, Comment 1732. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 33569 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 39463 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 38405 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| LockedFile | 3 | 1847 | 50.4 | 963 | 2002 | 0 | 0 | 0 | 196.5 | yes |
