# Stress run: LockedFile, 3 server(s): PASSED

- Commit `1b49659b6d0d95e1a10858057ef4082c27040edb`, 2026-10-17 21:57:32 UTC, 12.3 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 5 clients, 6 records, 40 changes per client, seed 1
- Clients by role: 6 Registrant, 6 Reviewer, 3 Admin
- Data folder `/mnt/share/runs/nfs-one-light-LockedFile-1/data`; run folder `stress-runs/nfs/nfs-one-light-LockedFile-1`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 606 | 211 | 17.2 | 235 | 160 | 0 | 145.4 | 323.0 | 496.3 |

Writes by outcome: conflict 235, saved 211, wrongStatus 160. Saves by action: Create 6, Update 27, Submit 6, Approve 2, Reject 4, Comment 166. Reads: 600.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 32813 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 41125 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 45077 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| LockedFile | 3 | 211 | 17.2 | 235 | 160 | 0 | 0 | 0 | 323.0 | yes |
