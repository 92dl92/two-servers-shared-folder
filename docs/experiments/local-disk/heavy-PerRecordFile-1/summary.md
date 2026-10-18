# Stress run: PerRecordFile, 3 server(s): PASSED

- Commit `ee6af83bef739621009cd4be91192b48f8df58df`, 2026-10-17 19:10:38 UTC, 3.3 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 12 records, 200 changes per client, seed 1
- Data folder `stress-runs/heavy-PerRecordFile-1/data`; run folder `stress-runs/heavy-PerRecordFile-1`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4812 | 1438 | 429.8 | 1440 | 1934 | 0 | 11.4 | 19.4 | 112.0 |

Writes by outcome: conflict 1440, saved 1438, wrongStatus 1934. Saves by action: Create 12, Update 181, Submit 12, Approve 4, Reject 8, Comment 1221. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 33989 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 43881 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 44687 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 3 | 1438 | 429.8 | 1440 | 1934 | 0 | 0 | 0 | 19.4 | yes |
