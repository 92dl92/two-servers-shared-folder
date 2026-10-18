# Stress run: PerRecordFile, 3 server(s): PASSED

- Commit `ee6af83bef739621009cd4be91192b48f8df58df`, 2026-10-17 19:10:51 UTC, 3.4 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 12 records, 200 changes per client, seed 3
- Data folder `stress-runs/heavy-PerRecordFile-3/data`; run folder `stress-runs/heavy-PerRecordFile-3`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4812 | 1390 | 406.2 | 1381 | 2041 | 0 | 11.8 | 20.3 | 115.7 |

Writes by outcome: conflict 1381, saved 1390, wrongStatus 2041. Saves by action: Create 12, Update 84, Submit 12, Approve 6, Reject 6, Comment 1270. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 36401 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 40355 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 34953 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 3 | 1390 | 406.2 | 1381 | 2041 | 0 | 0 | 0 | 20.3 | yes |
