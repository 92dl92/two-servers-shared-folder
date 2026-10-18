# Stress run: LockedFile, 3 server(s): PASSED

- Commit `ee6af83bef739621009cd4be91192b48f8df58df`, 2026-10-17 19:07:36 UTC, 3.6 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 5 clients, 6 records, 40 changes per client, seed 1
- Data folder `stress-runs/light-LockedFile-1/data`; run folder `stress-runs/light-LockedFile-1`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 606 | 275 | 76.4 | 150 | 181 | 0 | 8.1 | 76.7 | 191.9 |

Writes by outcome: conflict 150, saved 275, wrongStatus 181. Saves by action: Create 6, Update 29, Submit 6, Approve 4, Reject 2, Comment 228. Reads: 600.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 36735 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 46203 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 44815 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| LockedFile | 3 | 275 | 76.4 | 150 | 181 | 0 | 0 | 0 | 76.7 | yes |
