# Stress run: LockedFile, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:42:13 UTC, 31.6 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 12 records, 200 changes per client, seed 3
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r12-LockedFile-3/data`; run folder `stress-runs/sweep/sweep-3x8-r12-LockedFile-3`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4812 | 1828 | 57.8 | 963 | 2021 | 0 | 10.7 | 159.0 | 622.5 |

Writes by outcome: conflict 963, saved 1828, wrongStatus 2021. Saves by action: Create 12, Update 64, Submit 12, Approve 7, Reject 5, Comment 1728. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 34647 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 37907 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 44915 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| LockedFile | 3 | 1828 | 57.8 | 963 | 2021 | 0 | 0 | 0 | 159.0 | yes |
