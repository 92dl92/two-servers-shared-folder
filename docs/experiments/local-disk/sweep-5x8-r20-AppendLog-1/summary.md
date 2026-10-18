# Stress run: AppendLog, 5 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 21:00:29 UTC, 9.5 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 5 servers × 8 clients, 20 records, 200 changes per client, seed 1
- Clients by role: 20 Registrant, 15 Reviewer, 5 Admin
- Data folder `stress-runs/sweep/sweep-5x8-r20-AppendLog-1/data`; run folder `stress-runs/sweep/sweep-5x8-r20-AppendLog-1`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 8020 | 2381 | 251.6 | 2129 | 3510 | 0 | 23.1 | 83.6 | 320.6 |

Writes by outcome: conflict 2129, saved 2381, wrongStatus 3510. Saves by action: Create 20, Update 126, Submit 20, Approve 12, Reject 8, Comment 2195. Reads: 8000.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |
| server 39399 | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |
| server 40619 | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |
| server 42699 | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |
| server 46273 | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |
| server 44015 | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| AppendLog | 5 | 2381 | 251.6 | 2129 | 3510 | 0 | 0 | 0 | 83.6 | yes |
