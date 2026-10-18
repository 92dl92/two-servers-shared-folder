# Stress run: AppendLog, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:46:38 UTC, 4.2 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 24 records, 200 changes per client, seed 3
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r24-AppendLog-3/data`; run folder `stress-runs/sweep/sweep-3x8-r24-AppendLog-3`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4824 | 1914 | 459.3 | 787 | 2123 | 0 | 8.7 | 35.2 | 212.7 |

Writes by outcome: conflict 787, saved 1914, wrongStatus 2123. Saves by action: Create 24, Update 179, Submit 24, Approve 10, Reject 14, Comment 1663. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 24/24 | 0 | 0 | 0 | 0 | 0 |
| server 36957 | pass | 24/24 | 0 | 0 | 0 | 0 | 0 |
| server 44149 | pass | 24/24 | 0 | 0 | 0 | 0 | 0 |
| server 33103 | pass | 24/24 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| AppendLog | 3 | 1914 | 459.3 | 787 | 2123 | 0 | 0 | 0 | 35.2 | yes |
