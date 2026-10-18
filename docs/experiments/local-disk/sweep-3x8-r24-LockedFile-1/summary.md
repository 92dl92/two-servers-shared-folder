# Stress run: LockedFile, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:44:12 UTC, 35.7 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 24 records, 200 changes per client, seed 1
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r24-LockedFile-1/data`; run folder `stress-runs/sweep/sweep-3x8-r24-LockedFile-1`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4824 | 2104 | 58.9 | 657 | 2063 | 0 | 11.4 | 276.1 | 1017.1 |

Writes by outcome: conflict 657, saved 2104, wrongStatus 2063. Saves by action: Create 24, Update 145, Submit 24, Approve 13, Reject 11, Comment 1887. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 24/24 | 0 | 0 | 0 | 0 | 0 |
| server 46607 | pass | 24/24 | 0 | 0 | 0 | 0 | 0 |
| server 41773 | pass | 24/24 | 0 | 0 | 0 | 0 | 0 |
| server 40383 | pass | 24/24 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| LockedFile | 3 | 2104 | 58.9 | 657 | 2063 | 0 | 0 | 0 | 276.1 | yes |
