# Stress run: AppendLog, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:46:25 UTC, 4.3 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 24 records, 200 changes per client, seed 1
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r24-AppendLog-1/data`; run folder `stress-runs/sweep/sweep-3x8-r24-AppendLog-1`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4824 | 1891 | 440.1 | 776 | 2157 | 0 | 8.2 | 39.0 | 185.1 |

Writes by outcome: conflict 776, saved 1891, wrongStatus 2157. Saves by action: Create 24, Update 161, Submit 24, Approve 13, Reject 11, Comment 1658. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 24/24 | 0 | 0 | 0 | 0 | 0 |
| server 40399 | pass | 24/24 | 0 | 0 | 0 | 0 | 0 |
| server 41575 | pass | 24/24 | 0 | 0 | 0 | 0 | 0 |
| server 44637 | pass | 24/24 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| AppendLog | 3 | 1891 | 440.1 | 776 | 2157 | 0 | 0 | 0 | 39.0 | yes |
