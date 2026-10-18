# Stress run: LockedFile, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:44:50 UTC, 37.1 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 24 records, 200 changes per client, seed 2
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r24-LockedFile-2/data`; run folder `stress-runs/sweep/sweep-3x8-r24-LockedFile-2`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4824 | 2124 | 57.2 | 692 | 2008 | 0 | 11.4 | 259.1 | 1255.0 |

Writes by outcome: conflict 692, saved 2124, wrongStatus 2008. Saves by action: Create 24, Update 153, Submit 24, Approve 12, Reject 12, Comment 1899. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 24/24 | 0 | 0 | 0 | 0 | 0 |
| server 34465 | pass | 24/24 | 0 | 0 | 0 | 0 | 0 |
| server 42811 | pass | 24/24 | 0 | 0 | 0 | 0 | 0 |
| server 34943 | pass | 24/24 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| LockedFile | 3 | 2124 | 57.2 | 692 | 2008 | 0 | 0 | 0 | 259.1 | yes |
