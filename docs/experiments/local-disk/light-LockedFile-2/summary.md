# Stress run: LockedFile, 3 server(s): PASSED

- Commit `ee6af83bef739621009cd4be91192b48f8df58df`, 2026-10-17 19:07:43 UTC, 3.8 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 5 clients, 6 records, 40 changes per client, seed 2
- Data folder `stress-runs/light-LockedFile-2/data`; run folder `stress-runs/light-LockedFile-2`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 606 | 279 | 72.8 | 171 | 156 | 0 | 8.9 | 96.0 | 218.9 |

Writes by outcome: conflict 171, saved 279, wrongStatus 156. Saves by action: Create 6, Update 50, Submit 6, Approve 3, Reject 3, Comment 211. Reads: 600.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 45063 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 38673 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 43111 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| LockedFile | 3 | 279 | 72.8 | 171 | 156 | 0 | 0 | 0 | 96.0 | yes |
