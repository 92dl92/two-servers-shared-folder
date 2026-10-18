# Stress run: AppendLog, 3 server(s): PASSED

- Commit `ee6af83bef739621009cd4be91192b48f8df58df`, 2026-10-17 19:08:06 UTC, 0.7 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 5 clients, 6 records, 40 changes per client, seed 1
- Data folder `stress-runs/light-AppendLog-1/data`; run folder `stress-runs/light-AppendLog-1`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 606 | 226 | 332.5 | 235 | 145 | 0 | 3.1 | 22.8 | 173.4 |

Writes by outcome: conflict 235, saved 226, wrongStatus 145. Saves by action: Create 6, Update 48, Submit 6, Approve 5, Reject 1, Comment 160. Reads: 600.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 37329 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 42997 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 40147 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| AppendLog | 3 | 226 | 332.5 | 235 | 145 | 0 | 0 | 0 | 22.8 | yes |
