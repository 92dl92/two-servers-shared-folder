# Stress run: LockedFile, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:45:29 UTC, 37.0 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 24 records, 200 changes per client, seed 3
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r24-LockedFile-3/data`; run folder `stress-runs/sweep/sweep-3x8-r24-LockedFile-3`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4824 | 2165 | 58.6 | 612 | 2047 | 0 | 11.4 | 279.1 | 1134.6 |

Writes by outcome: conflict 612, saved 2165, wrongStatus 2047. Saves by action: Create 24, Update 154, Submit 24, Approve 9, Reject 15, Comment 1939. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 24/24 | 0 | 0 | 0 | 0 | 0 |
| server 39043 | pass | 24/24 | 0 | 0 | 0 | 0 | 0 |
| server 43083 | pass | 24/24 | 0 | 0 | 0 | 0 | 0 |
| server 34095 | pass | 24/24 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| LockedFile | 3 | 2165 | 58.6 | 612 | 2047 | 0 | 0 | 0 | 279.1 | yes |
