# Stress run: LockedFile, 5 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:58:46 UTC, 69.7 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 5 servers × 8 clients, 20 records, 200 changes per client, seed 3
- Clients by role: 20 Registrant, 15 Reviewer, 5 Admin
- Data folder `stress-runs/sweep/sweep-5x8-r20-LockedFile-3/data`; run folder `stress-runs/sweep/sweep-5x8-r20-LockedFile-3`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 8020 | 3030 | 43.5 | 1699 | 3291 | 0 | 15.1 | 378.1 | 2163.9 |

Writes by outcome: conflict 1699, saved 3030, wrongStatus 3291. Saves by action: Create 20, Update 128, Submit 20, Approve 11, Reject 9, Comment 2842. Reads: 8000.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |
| server 38817 | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |
| server 38565 | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |
| server 38561 | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |
| server 41607 | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |
| server 42829 | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| LockedFile | 5 | 3030 | 43.5 | 1699 | 3291 | 0 | 0 | 0 | 378.1 | yes |
