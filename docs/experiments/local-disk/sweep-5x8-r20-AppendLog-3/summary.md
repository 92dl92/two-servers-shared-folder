# Stress run: AppendLog, 5 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 21:00:54 UTC, 8.6 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 5 servers × 8 clients, 20 records, 200 changes per client, seed 3
- Clients by role: 20 Registrant, 15 Reviewer, 5 Admin
- Data folder `stress-runs/sweep/sweep-5x8-r20-AppendLog-3/data`; run folder `stress-runs/sweep/sweep-5x8-r20-AppendLog-3`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 8020 | 2461 | 285.6 | 2058 | 3501 | 0 | 18.2 | 79.9 | 351.5 |

Writes by outcome: conflict 2058, saved 2461, wrongStatus 3501. Saves by action: Create 20, Update 171, Submit 20, Approve 11, Reject 9, Comment 2230. Reads: 8000.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |
| server 32771 | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |
| server 39631 | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |
| server 41587 | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |
| server 45323 | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |
| server 36397 | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| AppendLog | 5 | 2461 | 285.6 | 2058 | 3501 | 0 | 0 | 0 | 79.9 | yes |
