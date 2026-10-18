# Stress run: LockedFile, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:37:35 UTC, 37.5 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 3 records, 200 changes per client, seed 1
- Clients by role: 3 Registrant, 18 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r03-LockedFile-1/data`; run folder `stress-runs/sweep/sweep-3x8-r03-LockedFile-1`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4803 | 1928 | 51.5 | 2374 | 501 | 0 | 11.7 | 68.3 | 226.5 |

Writes by outcome: conflict 2374, saved 1928, wrongStatus 501. Saves by action: Create 3, Update 10, Submit 3, Approve 2, Reject 1, Comment 1909. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 3/3 | 0 | 0 | 0 | 0 | 0 |
| server 41781 | pass | 3/3 | 0 | 0 | 0 | 0 | 0 |
| server 38841 | pass | 3/3 | 0 | 0 | 0 | 0 | 0 |
| server 38669 | pass | 3/3 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| LockedFile | 3 | 1928 | 51.5 | 2374 | 501 | 0 | 0 | 0 | 68.3 | yes |
