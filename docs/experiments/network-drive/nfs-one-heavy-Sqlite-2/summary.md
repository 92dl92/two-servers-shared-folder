# Stress run: Sqlite, 3 server(s): PASSED

- Commit `1b49659b6d0d95e1a10858057ef4082c27040edb`, 2026-10-17 22:00:01 UTC, 83.6 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 12 records, 200 changes per client, seed 2
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `/mnt/share/runs/nfs-one-heavy-Sqlite-2/data`; run folder `stress-runs/nfs/nfs-one-heavy-Sqlite-2`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4812 | 1462 | 17.5 | 1157 | 2193 | 0 | 195.0 | 514.5 | 14801.8 |

Writes by outcome: conflict 1157, saved 1462, wrongStatus 2193. Saves by action: Create 12, Update 184, Submit 12, Approve 7, Reject 5, Comment 1242. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 43047 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 37561 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 36033 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| Sqlite | 3 | 1462 | 17.5 | 1157 | 2193 | 0 | 0 | 0 | 514.5 | yes |
