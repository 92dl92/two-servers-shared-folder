# Stress run: LockedFile, 3 server(s): PASSED

- Commit `1b49659b6d0d95e1a10858057ef4082c27040edb`, 2026-10-17 22:07:03 UTC, 276.9 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 12 records, 200 changes per client, seed 2
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `/mnt/share/runs/nfs-one-heavy-LockedFile-2/data`; run folder `stress-runs/nfs/nfs-one-heavy-LockedFile-2`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4812 | 1592 | 5.8 | 1408 | 1812 | 0 | 643.4 | 1849.1 | 5286.2 |

Writes by outcome: conflict 1408, saved 1592, wrongStatus 1812. Saves by action: Create 12, Update 132, Submit 12, Approve 7, Reject 5, Comment 1424. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 36157 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 37487 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 33365 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| LockedFile | 3 | 1592 | 5.8 | 1408 | 1812 | 0 | 0 | 0 | 1849.1 | yes |
