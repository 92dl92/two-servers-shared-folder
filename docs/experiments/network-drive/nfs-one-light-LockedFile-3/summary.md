# Stress run: LockedFile, 3 server(s): PASSED

- Commit `1b49659b6d0d95e1a10858057ef4082c27040edb`, 2026-10-17 21:58:01 UTC, 13.0 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 5 clients, 6 records, 40 changes per client, seed 3
- Clients by role: 6 Registrant, 6 Reviewer, 3 Admin
- Data folder `/mnt/share/runs/nfs-one-light-LockedFile-3/data`; run folder `stress-runs/nfs/nfs-one-light-LockedFile-3`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 606 | 239 | 18.3 | 256 | 111 | 0 | 145.6 | 333.4 | 515.9 |

Writes by outcome: conflict 256, saved 239, wrongStatus 111. Saves by action: Create 6, Update 65, Submit 6, Approve 4, Reject 1, Comment 157. Reads: 600.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 41521 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 38703 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 43475 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| LockedFile | 3 | 239 | 18.3 | 256 | 111 | 0 | 0 | 0 | 333.4 | yes |
