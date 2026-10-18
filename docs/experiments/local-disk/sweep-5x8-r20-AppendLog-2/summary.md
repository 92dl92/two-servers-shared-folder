# Stress run: AppendLog, 5 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 21:00:42 UTC, 8.6 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 5 servers × 8 clients, 20 records, 200 changes per client, seed 2
- Clients by role: 20 Registrant, 15 Reviewer, 5 Admin
- Data folder `stress-runs/sweep/sweep-5x8-r20-AppendLog-2/data`; run folder `stress-runs/sweep/sweep-5x8-r20-AppendLog-2`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 8020 | 2525 | 292.7 | 1967 | 3528 | 0 | 16.3 | 72.4 | 329.9 |

Writes by outcome: conflict 1967, saved 2525, wrongStatus 3528. Saves by action: Create 20, Update 134, Submit 20, Approve 11, Reject 9, Comment 2331. Reads: 8000.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |
| server 42987 | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |
| server 45505 | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |
| server 40217 | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |
| server 43189 | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |
| server 42151 | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| AppendLog | 5 | 2525 | 292.7 | 1967 | 3528 | 0 | 0 | 0 | 72.4 | yes |
