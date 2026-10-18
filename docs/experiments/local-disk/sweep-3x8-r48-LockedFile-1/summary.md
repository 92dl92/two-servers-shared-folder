# Stress run: LockedFile, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:47:36 UTC, 44.9 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 48 records, 200 changes per client, seed 1
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r48-LockedFile-1/data`; run folder `stress-runs/sweep/sweep-3x8-r48-LockedFile-1`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4848 | 2398 | 53.4 | 390 | 2060 | 0 | 12.5 | 411.9 | 1675.3 |

Writes by outcome: conflict 390, saved 2398, wrongStatus 2060. Saves by action: Create 48, Update 260, Submit 48, Approve 26, Reject 22, Comment 1994. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 48/48 | 0 | 0 | 0 | 0 | 0 |
| server 40475 | pass | 48/48 | 0 | 0 | 0 | 0 | 0 |
| server 43753 | pass | 48/48 | 0 | 0 | 0 | 0 | 0 |
| server 33095 | pass | 48/48 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| LockedFile | 3 | 2398 | 53.4 | 390 | 2060 | 0 | 0 | 0 | 411.9 | yes |
