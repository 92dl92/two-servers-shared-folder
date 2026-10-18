# Stress run: PerRecordFile, 3 server(s): PASSED

- Commit `1b49659b6d0d95e1a10858057ef4082c27040edb`, 2026-10-17 21:58:20 UTC, 1.6 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 5 clients, 6 records, 40 changes per client, seed 2
- Clients by role: 6 Registrant, 6 Reviewer, 3 Admin
- Data folder `/mnt/share/runs/nfs-one-light-PerRecordFile-2/data`; run folder `stress-runs/nfs/nfs-one-light-PerRecordFile-2`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 606 | 183 | 114.8 | 258 | 165 | 0 | 26.6 | 59.7 | 112.6 |

Writes by outcome: conflict 258, saved 183, wrongStatus 165. Saves by action: Create 6, Update 47, Submit 6, Approve 6, Comment 118. Reads: 600.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 35153 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 45833 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 45911 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 3 | 183 | 114.8 | 258 | 165 | 0 | 0 | 0 | 59.7 | yes |
