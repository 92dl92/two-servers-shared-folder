# Stress run: AppendLog, 3 server(s): PASSED

- Commit `ee6af83bef739621009cd4be91192b48f8df58df`, 2026-10-17 19:11:07 UTC, 5.4 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 12 records, 200 changes per client, seed 2
- Data folder `stress-runs/heavy-AppendLog-2/data`; run folder `stress-runs/heavy-AppendLog-2`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4812 | 1494 | 278.4 | 1217 | 2101 | 0 | 11.5 | 42.8 | 240.2 |

Writes by outcome: conflict 1217, saved 1494, wrongStatus 2101. Saves by action: Create 12, Update 105, Submit 12, Approve 5, Reject 7, Comment 1353. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 44419 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 39765 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 45849 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| AppendLog | 3 | 1494 | 278.4 | 1217 | 2101 | 0 | 0 | 0 | 42.8 | yes |
