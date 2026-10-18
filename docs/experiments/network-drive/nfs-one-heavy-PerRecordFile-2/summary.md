# Stress run: PerRecordFile, 3 server(s): PASSED

- Commit `1b49659b6d0d95e1a10858057ef4082c27040edb`, 2026-10-17 22:16:34 UTC, 16.2 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 12 records, 200 changes per client, seed 2
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `/mnt/share/runs/nfs-one-heavy-PerRecordFile-2/data`; run folder `stress-runs/nfs/nfs-one-heavy-PerRecordFile-2`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4812 | 1527 | 94.1 | 1476 | 1809 | 0 | 67.3 | 93.8 | 158.7 |

Writes by outcome: conflict 1476, saved 1527, wrongStatus 1809. Saves by action: Create 12, Update 456, Submit 12, Approve 7, Reject 5, Comment 1035. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 41889 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 41881 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 40365 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 3 | 1527 | 94.1 | 1476 | 1809 | 0 | 0 | 0 | 93.8 | yes |
