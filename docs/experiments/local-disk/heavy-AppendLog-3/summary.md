# Stress run: AppendLog, 3 server(s): PASSED

- Commit `ee6af83bef739621009cd4be91192b48f8df58df`, 2026-10-17 19:11:15 UTC, 6.5 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 12 records, 200 changes per client, seed 3
- Data folder `stress-runs/heavy-AppendLog-3/data`; run folder `stress-runs/heavy-AppendLog-3`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4812 | 1493 | 231.1 | 1283 | 2036 | 0 | 15.3 | 47.8 | 224.6 |

Writes by outcome: conflict 1283, saved 1493, wrongStatus 2036. Saves by action: Create 12, Update 129, Submit 12, Approve 6, Reject 6, Comment 1328. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 39749 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 44063 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 42181 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| AppendLog | 3 | 1493 | 231.1 | 1283 | 2036 | 0 | 0 | 0 | 47.8 | yes |
