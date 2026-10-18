# Stress run: Sqlite, 3 server(s): PASSED

- Commit `1b49659b6d0d95e1a10858057ef4082c27040edb`, 2026-10-17 22:01:27 UTC, 69.2 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 12 records, 200 changes per client, seed 3
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `/mnt/share/runs/nfs-one-heavy-Sqlite-3/data`; run folder `stress-runs/nfs/nfs-one-heavy-Sqlite-3`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4812 | 1455 | 21.0 | 1148 | 2209 | 0 | 195.5 | 458.4 | 5025.1 |

Writes by outcome: conflict 1148, saved 1455, wrongStatus 2209. Saves by action: Create 12, Update 191, Submit 12, Approve 4, Reject 8, Comment 1228. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 46695 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 44521 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 41437 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| Sqlite | 3 | 1455 | 21.0 | 1148 | 2209 | 0 | 0 | 0 | 458.4 | yes |
