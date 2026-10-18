# Stress run: LockedFile, 3 server(s): PASSED

- Commit `ee6af83bef739621009cd4be91192b48f8df58df`, 2026-10-17 19:09:25 UTC, 33.5 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 12 records, 200 changes per client, seed 2
- Data folder `stress-runs/heavy-LockedFile-2/data`; run folder `stress-runs/heavy-LockedFile-2`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4812 | 1895 | 56.7 | 920 | 1997 | 0 | 10.8 | 179.4 | 656.4 |

Writes by outcome: conflict 920, saved 1895, wrongStatus 1997. Saves by action: Create 12, Update 74, Submit 12, Approve 6, Reject 6, Comment 1785. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 33655 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 46533 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 34571 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| LockedFile | 3 | 1895 | 56.7 | 920 | 1997 | 0 | 0 | 0 | 179.4 | yes |
