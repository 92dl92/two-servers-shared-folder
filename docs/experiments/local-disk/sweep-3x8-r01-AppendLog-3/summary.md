# Stress run: AppendLog, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:36:42 UTC, 11.7 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 1 records, 200 changes per client, seed 3
- Clients by role: 1 Registrant, 20 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r01-AppendLog-3/data`; run folder `stress-runs/sweep/sweep-3x8-r01-AppendLog-3`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4801 | 382 | 32.6 | 4208 | 211 | 0 | 21.0 | 51.8 | 151.2 |

Writes by outcome: conflict 4208, saved 382, wrongStatus 211. Saves by action: Create 1, Update 1, Submit 1, Approve 1, Comment 378. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 1/1 | 0 | 0 | 0 | 0 | 0 |
| server 36105 | pass | 1/1 | 0 | 0 | 0 | 0 | 0 |
| server 36875 | pass | 1/1 | 0 | 0 | 0 | 0 | 0 |
| server 35515 | pass | 1/1 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| AppendLog | 3 | 382 | 32.6 | 4208 | 211 | 0 | 0 | 0 | 51.8 | yes |
