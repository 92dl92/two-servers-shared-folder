# Stress run: PerRecordFile, 5 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 21:00:08 UTC, 7.2 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 5 servers × 8 clients, 20 records, 200 changes per client, seed 2
- Clients by role: 20 Registrant, 15 Reviewer, 5 Admin
- Data folder `stress-runs/sweep/sweep-5x8-r20-PerRecordFile-2/data`; run folder `stress-runs/sweep/sweep-5x8-r20-PerRecordFile-2`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 8020 | 2427 | 338.0 | 2381 | 3212 | 0 | 21.5 | 51.6 | 172.6 |

Writes by outcome: conflict 2381, saved 2427, wrongStatus 3212. Saves by action: Create 20, Update 183, Submit 20, Approve 7, Reject 13, Comment 2184. Reads: 8000.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |
| server 39511 | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |
| server 33165 | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |
| server 34993 | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |
| server 40447 | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |
| server 33755 | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 5 | 2427 | 338.0 | 2381 | 3212 | 0 | 0 | 0 | 51.6 | yes |
