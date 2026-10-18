# Stress run: PerRecordFile, 3 server(s): PASSED

- Commit `ee6af83bef739621009cd4be91192b48f8df58df`, 2026-10-17 19:10:45 UTC, 3.4 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 12 records, 200 changes per client, seed 2
- Data folder `stress-runs/heavy-PerRecordFile-2/data`; run folder `stress-runs/heavy-PerRecordFile-2`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4812 | 1430 | 419.2 | 1409 | 1973 | 0 | 11.7 | 21.5 | 123.0 |

Writes by outcome: conflict 1409, saved 1430, wrongStatus 1973. Saves by action: Create 12, Update 129, Submit 12, Approve 8, Reject 4, Comment 1265. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 35751 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 39307 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 42043 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 3 | 1430 | 419.2 | 1409 | 1973 | 0 | 0 | 0 | 21.5 | yes |
