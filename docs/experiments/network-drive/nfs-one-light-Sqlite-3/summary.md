# Stress run: Sqlite, 3 server(s): PASSED

- Commit `1b49659b6d0d95e1a10858057ef4082c27040edb`, 2026-10-17 21:57:21 UTC, 8.6 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 5 clients, 6 records, 40 changes per client, seed 3
- Clients by role: 6 Registrant, 6 Reviewer, 3 Admin
- Data folder `/mnt/share/runs/nfs-one-light-Sqlite-3/data`; run folder `stress-runs/nfs/nfs-one-light-Sqlite-3`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 606 | 204 | 23.7 | 210 | 192 | 0 | 108.0 | 239.4 | 404.3 |

Writes by outcome: conflict 210, saved 204, wrongStatus 192. Saves by action: Create 6, Update 35, Submit 6, Approve 2, Reject 4, Comment 151. Reads: 600.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 34013 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 37391 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 40215 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| Sqlite | 3 | 204 | 23.7 | 210 | 192 | 0 | 0 | 0 | 239.4 | yes |
