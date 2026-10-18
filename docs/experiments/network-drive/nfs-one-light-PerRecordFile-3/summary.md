# Stress run: PerRecordFile, 3 server(s): PASSED

- Commit `1b49659b6d0d95e1a10858057ef4082c27040edb`, 2026-10-17 21:58:23 UTC, 1.8 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 5 clients, 6 records, 40 changes per client, seed 3
- Clients by role: 6 Registrant, 6 Reviewer, 3 Admin
- Data folder `/mnt/share/runs/nfs-one-light-PerRecordFile-3/data`; run folder `stress-runs/nfs/nfs-one-light-PerRecordFile-3`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 606 | 206 | 115.9 | 272 | 128 | 0 | 26.8 | 65.4 | 111.2 |

Writes by outcome: conflict 272, saved 206, wrongStatus 128. Saves by action: Create 6, Update 74, Submit 6, Approve 2, Reject 3, Comment 115. Reads: 600.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 39571 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 32769 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 46563 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 3 | 206 | 115.9 | 272 | 128 | 0 | 0 | 0 | 65.4 | yes |
