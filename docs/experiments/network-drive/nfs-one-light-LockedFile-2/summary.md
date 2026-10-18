# Stress run: LockedFile, 3 server(s): PASSED

- Commit `1b49659b6d0d95e1a10858057ef4082c27040edb`, 2026-10-17 21:57:46 UTC, 12.4 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 5 clients, 6 records, 40 changes per client, seed 2
- Clients by role: 6 Registrant, 6 Reviewer, 3 Admin
- Data folder `/mnt/share/runs/nfs-one-light-LockedFile-2/data`; run folder `stress-runs/nfs/nfs-one-light-LockedFile-2`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 606 | 212 | 17.0 | 246 | 148 | 0 | 150.9 | 337.0 | 515.6 |

Writes by outcome: conflict 246, saved 212, wrongStatus 148. Saves by action: Create 6, Update 33, Submit 6, Approve 5, Reject 1, Comment 161. Reads: 600.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 35567 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 42785 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 46385 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| LockedFile | 3 | 212 | 17.0 | 246 | 148 | 0 | 0 | 0 | 337.0 | yes |
