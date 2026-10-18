# Stress run: AppendLog, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:40:13 UTC, 7.8 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 3 records, 200 changes per client, seed 3
- Clients by role: 3 Registrant, 18 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r03-AppendLog-3/data`; run folder `stress-runs/sweep/sweep-3x8-r03-AppendLog-3`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4803 | 1050 | 135.2 | 3161 | 592 | 0 | 12.1 | 26.5 | 173.9 |

Writes by outcome: conflict 3161, saved 1050, wrongStatus 592. Saves by action: Create 3, Update 4, Submit 3, Approve 2, Reject 1, Comment 1037. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 3/3 | 0 | 0 | 0 | 0 | 0 |
| server 37367 | pass | 3/3 | 0 | 0 | 0 | 0 | 0 |
| server 38847 | pass | 3/3 | 0 | 0 | 0 | 0 | 0 |
| server 38527 | pass | 3/3 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| AppendLog | 3 | 1050 | 135.2 | 3161 | 592 | 0 | 0 | 0 | 26.5 | yes |
