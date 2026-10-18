# Stress run: AppendLog, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:40:04 UTC, 7.7 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 3 records, 200 changes per client, seed 2
- Clients by role: 3 Registrant, 18 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r03-AppendLog-2/data`; run folder `stress-runs/sweep/sweep-3x8-r03-AppendLog-2`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4803 | 1069 | 138.9 | 3139 | 595 | 0 | 11.9 | 25.2 | 172.8 |

Writes by outcome: conflict 3139, saved 1069, wrongStatus 595. Saves by action: Create 3, Update 13, Submit 3, Approve 1, Reject 2, Comment 1047. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 3/3 | 0 | 0 | 0 | 0 | 0 |
| server 37943 | pass | 3/3 | 0 | 0 | 0 | 0 | 0 |
| server 33613 | pass | 3/3 | 0 | 0 | 0 | 0 | 0 |
| server 46709 | pass | 3/3 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| AppendLog | 3 | 1069 | 138.9 | 3139 | 595 | 0 | 0 | 0 | 25.2 | yes |
