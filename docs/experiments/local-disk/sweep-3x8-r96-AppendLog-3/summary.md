# Stress run: AppendLog, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:54:56 UTC, 3.9 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 96 records, 200 changes per client, seed 3
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r96-AppendLog-3/data`; run folder `stress-runs/sweep/sweep-3x8-r96-AppendLog-3`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4896 | 2881 | 737.8 | 327 | 1688 | 0 | 7.9 | 49.0 | 193.9 |

Writes by outcome: conflict 327, saved 2881, wrongStatus 1688. Saves by action: Create 96, Update 670, Submit 96, Approve 52, Reject 44, Comment 1923. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 96/96 | 0 | 0 | 0 | 0 | 0 |
| server 39649 | pass | 96/96 | 0 | 0 | 0 | 0 | 0 |
| server 43881 | pass | 96/96 | 0 | 0 | 0 | 0 | 0 |
| server 43571 | pass | 96/96 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| AppendLog | 3 | 2881 | 737.8 | 327 | 1688 | 0 | 0 | 0 | 49.0 | yes |
