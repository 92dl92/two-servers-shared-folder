# Stress run: LockedFile, 5 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:57:25 UTC, 77.3 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 5 servers × 8 clients, 20 records, 200 changes per client, seed 2
- Clients by role: 20 Registrant, 15 Reviewer, 5 Admin
- Data folder `stress-runs/sweep/sweep-5x8-r20-LockedFile-2/data`; run folder `stress-runs/sweep/sweep-5x8-r20-LockedFile-2`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 8020 | 3099 | 40.1 | 1684 | 3237 | 0 | 16.4 | 428.5 | 1966.2 |

Writes by outcome: conflict 1684, saved 3099, wrongStatus 3237. Saves by action: Create 20, Update 141, Submit 20, Approve 9, Reject 11, Comment 2898. Reads: 8000.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |
| server 33217 | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |
| server 43961 | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |
| server 36455 | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |
| server 36699 | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |
| server 33171 | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| LockedFile | 5 | 3099 | 40.1 | 1684 | 3237 | 0 | 0 | 0 | 428.5 | yes |
