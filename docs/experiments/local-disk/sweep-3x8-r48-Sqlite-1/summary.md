# Stress run: Sqlite, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:50:32 UTC, 12.1 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 48 records, 200 changes per client, seed 1
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r48-Sqlite-1/data`; run folder `stress-runs/sweep/sweep-3x8-r48-Sqlite-1`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4848 | 2918 | 240.2 | 312 | 1618 | 0 | 20.6 | 35.4 | 1140.6 |

Writes by outcome: conflict 312, saved 2918, wrongStatus 1618. Saves by action: Create 48, Update 843, Submit 48, Approve 22, Reject 26, Comment 1931. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 48/48 | 0 | 0 | 0 | 0 | 0 |
| server 34225 | pass | 48/48 | 0 | 0 | 0 | 0 | 0 |
| server 37865 | pass | 48/48 | 0 | 0 | 0 | 0 | 0 |
| server 38989 | pass | 48/48 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| Sqlite | 3 | 2918 | 240.2 | 312 | 1618 | 0 | 0 | 0 | 35.4 | yes |
