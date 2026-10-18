# Stress run: PerRecordFile, 3 server(s): PASSED

- Commit `1b49659b6d0d95e1a10858057ef4082c27040edb`, 2026-10-17 22:16:17 UTC, 14.2 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 12 records, 200 changes per client, seed 1
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `/mnt/share/runs/nfs-one-heavy-PerRecordFile-1/data`; run folder `stress-runs/nfs/nfs-one-heavy-PerRecordFile-1`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4812 | 1368 | 96.6 | 1415 | 2029 | 0 | 59.0 | 86.5 | 138.2 |

Writes by outcome: conflict 1415, saved 1368, wrongStatus 2029. Saves by action: Create 12, Update 236, Submit 12, Approve 4, Reject 8, Comment 1096. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 46823 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 35483 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 35123 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 3 | 1368 | 96.6 | 1415 | 2029 | 0 | 0 | 0 | 86.5 | yes |
