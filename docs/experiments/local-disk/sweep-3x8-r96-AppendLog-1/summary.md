# Stress run: AppendLog, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:54:42 UTC, 3.8 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 96 records, 200 changes per client, seed 1
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r96-AppendLog-1/data`; run folder `stress-runs/sweep/sweep-3x8-r96-AppendLog-1`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4896 | 2895 | 759.3 | 328 | 1673 | 0 | 7.4 | 49.5 | 201.2 |

Writes by outcome: conflict 328, saved 2895, wrongStatus 1673. Saves by action: Create 96, Update 698, Submit 96, Approve 46, Reject 50, Comment 1909. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 96/96 | 0 | 0 | 0 | 0 | 0 |
| server 40667 | pass | 96/96 | 0 | 0 | 0 | 0 | 0 |
| server 43993 | pass | 96/96 | 0 | 0 | 0 | 0 | 0 |
| server 46229 | pass | 96/96 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| AppendLog | 3 | 2895 | 759.3 | 328 | 1673 | 0 | 0 | 0 | 49.5 | yes |
