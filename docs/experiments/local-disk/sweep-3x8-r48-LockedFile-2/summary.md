# Stress run: LockedFile, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:48:23 UTC, 45.6 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 48 records, 200 changes per client, seed 2
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r48-LockedFile-2/data`; run folder `stress-runs/sweep/sweep-3x8-r48-LockedFile-2`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4848 | 2448 | 53.7 | 392 | 2008 | 0 | 12.7 | 386.2 | 2103.5 |

Writes by outcome: conflict 392, saved 2448, wrongStatus 2008. Saves by action: Create 48, Update 284, Submit 48, Approve 22, Reject 26, Comment 2020. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 48/48 | 0 | 0 | 0 | 0 | 0 |
| server 35245 | pass | 48/48 | 0 | 0 | 0 | 0 | 0 |
| server 46101 | pass | 48/48 | 0 | 0 | 0 | 0 | 0 |
| server 41393 | pass | 48/48 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| LockedFile | 3 | 2448 | 53.7 | 392 | 2008 | 0 | 0 | 0 | 386.2 | yes |
