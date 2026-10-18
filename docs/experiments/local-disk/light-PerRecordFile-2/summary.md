# Stress run: PerRecordFile, 3 server(s): PASSED

- Commit `ee6af83bef739621009cd4be91192b48f8df58df`, 2026-10-17 19:07:59 UTC, 0.8 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 5 clients, 6 records, 40 changes per client, seed 2
- Data folder `stress-runs/light-PerRecordFile-2/data`; run folder `stress-runs/light-PerRecordFile-2`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 606 | 242 | 288.1 | 242 | 122 | 0 | 8.4 | 22.2 | 107.6 |

Writes by outcome: conflict 242, saved 242, wrongStatus 122. Saves by action: Create 6, Update 94, Submit 6, Approve 3, Reject 3, Comment 130. Reads: 600.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 42639 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 42143 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 36039 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 3 | 242 | 288.1 | 242 | 122 | 0 | 0 | 0 | 22.2 | yes |
