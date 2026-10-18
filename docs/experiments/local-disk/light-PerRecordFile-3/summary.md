# Stress run: PerRecordFile, 3 server(s): PASSED

- Commit `ee6af83bef739621009cd4be91192b48f8df58df`, 2026-10-17 19:08:03 UTC, 0.8 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 5 clients, 6 records, 40 changes per client, seed 3
- Data folder `stress-runs/light-PerRecordFile-3/data`; run folder `stress-runs/light-PerRecordFile-3`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 606 | 237 | 279.7 | 245 | 124 | 0 | 8.1 | 19.0 | 116.0 |

Writes by outcome: conflict 245, saved 237, wrongStatus 124. Saves by action: Create 6, Update 79, Submit 6, Approve 3, Reject 3, Comment 140. Reads: 600.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 34199 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 41007 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 33727 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 3 | 237 | 279.7 | 245 | 124 | 0 | 0 | 0 | 19.0 | yes |
