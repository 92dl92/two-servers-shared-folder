# Stress run: Sqlite, 3 server(s): PASSED

- Commit `1b49659b6d0d95e1a10858057ef4082c27040edb`, 2026-10-17 21:58:57 UTC, 61.4 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 12 records, 200 changes per client, seed 1
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `/mnt/share/runs/nfs-one-heavy-Sqlite-1/data`; run folder `stress-runs/nfs/nfs-one-heavy-Sqlite-1`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4812 | 1425 | 23.2 | 1150 | 2237 | 0 | 202.8 | 443.2 | 2366.4 |

Writes by outcome: conflict 1150, saved 1425, wrongStatus 2237. Saves by action: Create 12, Update 157, Submit 12, Approve 8, Reject 4, Comment 1232. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 35215 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 40905 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 45593 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| Sqlite | 3 | 1425 | 23.2 | 1150 | 2237 | 0 | 0 | 0 | 443.2 | yes |
