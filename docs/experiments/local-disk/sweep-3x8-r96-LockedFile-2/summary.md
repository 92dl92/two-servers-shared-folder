# Stress run: LockedFile, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:52:26 UTC, 54.4 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 96 records, 200 changes per client, seed 2
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r96-LockedFile-2/data`; run folder `stress-runs/sweep/sweep-3x8-r96-LockedFile-2`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4896 | 2915 | 53.6 | 219 | 1762 | 0 | 13.7 | 513.5 | 1772.8 |

Writes by outcome: conflict 219, saved 2915, wrongStatus 1762. Saves by action: Create 96, Update 590, Submit 96, Approve 47, Reject 49, Comment 2037. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 96/96 | 0 | 0 | 0 | 0 | 0 |
| server 39341 | pass | 96/96 | 0 | 0 | 0 | 0 | 0 |
| server 42883 | pass | 96/96 | 0 | 0 | 0 | 0 | 0 |
| server 39023 | pass | 96/96 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| LockedFile | 3 | 2915 | 53.6 | 219 | 1762 | 0 | 0 | 0 | 513.5 | yes |
