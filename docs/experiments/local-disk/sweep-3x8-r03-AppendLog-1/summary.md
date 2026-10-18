# Stress run: AppendLog, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:39:54 UTC, 7.7 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 3 records, 200 changes per client, seed 1
- Clients by role: 3 Registrant, 18 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r03-AppendLog-1/data`; run folder `stress-runs/sweep/sweep-3x8-r03-AppendLog-1`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4803 | 1044 | 135.2 | 3173 | 586 | 0 | 12.6 | 25.4 | 209.2 |

Writes by outcome: conflict 3173, saved 1044, wrongStatus 586. Saves by action: Create 3, Update 13, Submit 3, Approve 1, Reject 2, Comment 1022. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 3/3 | 0 | 0 | 0 | 0 | 0 |
| server 40197 | pass | 3/3 | 0 | 0 | 0 | 0 | 0 |
| server 43681 | pass | 3/3 | 0 | 0 | 0 | 0 | 0 |
| server 34349 | pass | 3/3 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| AppendLog | 3 | 1044 | 135.2 | 3173 | 586 | 0 | 0 | 0 | 25.4 | yes |
