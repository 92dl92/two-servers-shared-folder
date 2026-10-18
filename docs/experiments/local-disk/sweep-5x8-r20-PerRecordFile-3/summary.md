# Stress run: PerRecordFile, 5 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 21:00:18 UTC, 8.1 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 5 servers × 8 clients, 20 records, 200 changes per client, seed 3
- Clients by role: 20 Registrant, 15 Reviewer, 5 Admin
- Data folder `stress-runs/sweep/sweep-5x8-r20-PerRecordFile-3/data`; run folder `stress-runs/sweep/sweep-5x8-r20-PerRecordFile-3`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 8020 | 2440 | 301.3 | 2412 | 3168 | 0 | 23.7 | 60.0 | 189.5 |

Writes by outcome: conflict 2412, saved 2440, wrongStatus 3168. Saves by action: Create 20, Update 198, Submit 20, Approve 7, Reject 13, Comment 2182. Reads: 8000.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |
| server 39227 | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |
| server 40969 | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |
| server 33601 | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |
| server 45717 | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |
| server 35743 | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 5 | 2440 | 301.3 | 2412 | 3168 | 0 | 0 | 0 | 60.0 | yes |
