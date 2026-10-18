# Stress run: AppendLog, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:54:49 UTC, 3.8 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 96 records, 200 changes per client, seed 2
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r96-AppendLog-2/data`; run folder `stress-runs/sweep/sweep-3x8-r96-AppendLog-2`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4896 | 2923 | 760.7 | 339 | 1634 | 0 | 7.8 | 51.1 | 205.0 |

Writes by outcome: conflict 339, saved 2923, wrongStatus 1634. Saves by action: Create 96, Update 715, Submit 96, Approve 50, Reject 46, Comment 1920. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 96/96 | 0 | 0 | 0 | 0 | 0 |
| server 39875 | pass | 96/96 | 0 | 0 | 0 | 0 | 0 |
| server 33199 | pass | 96/96 | 0 | 0 | 0 | 0 | 0 |
| server 39885 | pass | 96/96 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| AppendLog | 3 | 2923 | 760.7 | 339 | 1634 | 0 | 0 | 0 | 51.1 | yes |
