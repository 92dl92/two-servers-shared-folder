# Stress run: Sqlite, 3 server(s): PASSED

- Commit `1b49659b6d0d95e1a10858057ef4082c27040edb`, 2026-10-17 21:56:40 UTC, 8.7 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 5 clients, 6 records, 40 changes per client, seed 1
- Clients by role: 6 Registrant, 6 Reviewer, 3 Admin
- Data folder `/mnt/share/runs/nfs-one-light-Sqlite-1/data`; run folder `stress-runs/nfs/nfs-one-light-Sqlite-1`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 606 | 204 | 23.4 | 205 | 197 | 0 | 102.7 | 263.6 | 671.7 |

Writes by outcome: conflict 205, saved 204, wrongStatus 197. Saves by action: Create 6, Update 34, Submit 6, Approve 4, Reject 2, Comment 152. Reads: 600.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 36711 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 46567 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 37749 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| Sqlite | 3 | 204 | 23.4 | 205 | 197 | 0 | 0 | 0 | 263.6 | yes |
