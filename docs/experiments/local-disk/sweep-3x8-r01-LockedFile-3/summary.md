# Stress run: LockedFile, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:34:52 UTC, 32.5 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 1 records, 200 changes per client, seed 3
- Clients by role: 1 Registrant, 20 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r01-LockedFile-3/data`; run folder `stress-runs/sweep/sweep-3x8-r01-LockedFile-3`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4801 | 1185 | 36.4 | 3434 | 182 | 0 | 13.0 | 37.8 | 149.3 |

Writes by outcome: conflict 3434, saved 1185, wrongStatus 182. Saves by action: Create 1, Update 1, Submit 1, Reject 1, Comment 1181. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 1/1 | 0 | 0 | 0 | 0 | 0 |
| server 44117 | pass | 1/1 | 0 | 0 | 0 | 0 | 0 |
| server 43125 | pass | 1/1 | 0 | 0 | 0 | 0 | 0 |
| server 33947 | pass | 1/1 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| LockedFile | 3 | 1185 | 36.4 | 3434 | 182 | 0 | 0 | 0 | 37.8 | yes |
