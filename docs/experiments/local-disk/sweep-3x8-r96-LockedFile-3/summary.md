# Stress run: LockedFile, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:53:23 UTC, 58.4 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 96 records, 200 changes per client, seed 3
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r96-LockedFile-3/data`; run folder `stress-runs/sweep/sweep-3x8-r96-LockedFile-3`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4896 | 2868 | 49.1 | 228 | 1800 | 0 | 15.4 | 579.3 | 4183.9 |

Writes by outcome: conflict 228, saved 2868, wrongStatus 1800. Saves by action: Create 96, Update 549, Submit 96, Approve 41, Reject 53, Comment 2033. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 96/96 | 0 | 0 | 0 | 0 | 0 |
| server 39729 | pass | 96/96 | 0 | 0 | 0 | 0 | 0 |
| server 38235 | pass | 96/96 | 0 | 0 | 0 | 0 | 0 |
| server 34217 | pass | 96/96 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| LockedFile | 3 | 2868 | 49.1 | 228 | 1800 | 0 | 0 | 0 | 579.3 | yes |
