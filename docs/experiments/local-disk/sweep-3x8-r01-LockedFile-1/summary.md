# Stress run: LockedFile, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:33:40 UTC, 34.9 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 1 records, 200 changes per client, seed 1
- Clients by role: 1 Registrant, 20 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r01-LockedFile-1/data`; run folder `stress-runs/sweep/sweep-3x8-r01-LockedFile-1`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4801 | 1308 | 37.5 | 3306 | 187 | 0 | 12.8 | 34.8 | 159.4 |

Writes by outcome: conflict 3306, saved 1308, wrongStatus 187. Saves by action: Create 1, Submit 1, Reject 1, Comment 1305. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 1/1 | 0 | 0 | 0 | 0 | 0 |
| server 44555 | pass | 1/1 | 0 | 0 | 0 | 0 | 0 |
| server 40683 | pass | 1/1 | 0 | 0 | 0 | 0 | 0 |
| server 45069 | pass | 1/1 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| LockedFile | 3 | 1308 | 37.5 | 3306 | 187 | 0 | 0 | 0 | 34.8 | yes |
