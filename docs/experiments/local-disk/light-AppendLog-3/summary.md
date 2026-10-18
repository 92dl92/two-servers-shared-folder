# Stress run: AppendLog, 3 server(s): PASSED

- Commit `ee6af83bef739621009cd4be91192b48f8df58df`, 2026-10-17 19:08:13 UTC, 0.7 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 5 clients, 6 records, 40 changes per client, seed 3
- Data folder `stress-runs/light-AppendLog-3/data`; run folder `stress-runs/light-AppendLog-3`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 606 | 232 | 330.4 | 203 | 171 | 0 | 3.0 | 17.1 | 213.6 |

Writes by outcome: conflict 203, saved 232, wrongStatus 171. Saves by action: Create 6, Update 38, Submit 6, Approve 3, Reject 3, Comment 176. Reads: 600.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 35911 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 34111 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 34293 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| AppendLog | 3 | 232 | 330.4 | 203 | 171 | 0 | 0 | 0 | 17.1 | yes |
