# Stress run: PerRecordFile, 3 server(s): PASSED

- Commit `1b49659b6d0d95e1a10858057ef4082c27040edb`, 2026-10-17 21:58:16 UTC, 1.8 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 5 clients, 6 records, 40 changes per client, seed 1
- Clients by role: 6 Registrant, 6 Reviewer, 3 Admin
- Data folder `/mnt/share/runs/nfs-one-light-PerRecordFile-1/data`; run folder `stress-runs/nfs/nfs-one-light-PerRecordFile-1`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 606 | 199 | 113.7 | 261 | 146 | 0 | 25.9 | 55.6 | 110.4 |

Writes by outcome: conflict 261, saved 199, wrongStatus 146. Saves by action: Create 6, Update 58, Submit 6, Approve 2, Reject 3, Comment 124. Reads: 600.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 36211 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 38799 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 44551 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 3 | 199 | 113.7 | 261 | 146 | 0 | 0 | 0 | 55.6 | yes |
