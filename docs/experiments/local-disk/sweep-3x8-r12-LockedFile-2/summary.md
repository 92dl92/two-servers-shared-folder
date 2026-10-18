# Stress run: LockedFile, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:41:39 UTC, 31.8 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 12 records, 200 changes per client, seed 2
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r12-LockedFile-2/data`; run folder `stress-runs/sweep/sweep-3x8-r12-LockedFile-2`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4812 | 1857 | 58.4 | 931 | 2024 | 0 | 10.5 | 177.5 | 621.2 |

Writes by outcome: conflict 931, saved 1857, wrongStatus 2024. Saves by action: Create 12, Update 78, Submit 12, Approve 4, Reject 8, Comment 1743. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 37607 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 34849 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 44881 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| LockedFile | 3 | 1857 | 58.4 | 931 | 2024 | 0 | 0 | 0 | 177.5 | yes |
