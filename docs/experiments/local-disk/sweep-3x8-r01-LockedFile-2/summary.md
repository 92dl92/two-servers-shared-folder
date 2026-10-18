# Stress run: LockedFile, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:34:17 UTC, 32.0 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 1 records, 200 changes per client, seed 2
- Clients by role: 1 Registrant, 20 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r01-LockedFile-2/data`; run folder `stress-runs/sweep/sweep-3x8-r01-LockedFile-2`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4801 | 1147 | 35.8 | 3476 | 178 | 0 | 13.7 | 38.1 | 238.4 |

Writes by outcome: conflict 3476, saved 1147, wrongStatus 178. Saves by action: Create 1, Update 1, Submit 1, Reject 1, Comment 1143. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 1/1 | 0 | 0 | 0 | 0 | 0 |
| server 42543 | pass | 1/1 | 0 | 0 | 0 | 0 | 0 |
| server 38493 | pass | 1/1 | 0 | 0 | 0 | 0 | 0 |
| server 45587 | pass | 1/1 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| LockedFile | 3 | 1147 | 35.8 | 3476 | 178 | 0 | 0 | 0 | 38.1 | yes |
