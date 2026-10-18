# Stress run: AppendLog, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:43:10 UTC, 5.1 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 12 records, 200 changes per client, seed 2
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r12-AppendLog-2/data`; run folder `stress-runs/sweep/sweep-3x8-r12-AppendLog-2`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4812 | 1498 | 294.8 | 1212 | 2102 | 0 | 11.6 | 40.5 | 210.8 |

Writes by outcome: conflict 1212, saved 1498, wrongStatus 2102. Saves by action: Create 12, Update 91, Submit 12, Approve 8, Reject 4, Comment 1371. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 44569 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 32831 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 40493 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| AppendLog | 3 | 1498 | 294.8 | 1212 | 2102 | 0 | 0 | 0 | 40.5 | yes |
