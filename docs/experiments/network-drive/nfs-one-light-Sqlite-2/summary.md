# Stress run: Sqlite, 3 server(s): PASSED

- Commit `1b49659b6d0d95e1a10858057ef4082c27040edb`, 2026-10-17 21:56:51 UTC, 27.3 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 5 clients, 6 records, 40 changes per client, seed 2
- Clients by role: 6 Registrant, 6 Reviewer, 3 Admin
- Data folder `/mnt/share/runs/nfs-one-light-Sqlite-2/data`; run folder `stress-runs/nfs/nfs-one-light-Sqlite-2`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 606 | 206 | 7.6 | 217 | 183 | 0 | 102.3 | 245.5 | 13934.8 |

Writes by outcome: conflict 217, saved 206, wrongStatus 183. Saves by action: Create 6, Update 40, Submit 6, Approve 2, Reject 4, Comment 148. Reads: 600.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 36737 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 40255 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 44663 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| Sqlite | 3 | 206 | 7.6 | 217 | 183 | 0 | 0 | 0 | 245.5 | yes |
