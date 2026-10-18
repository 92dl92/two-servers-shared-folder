# Stress run: PerRecordFile, 3 server(s): PASSED

- Commit `1b49659b6d0d95e1a10858057ef4082c27040edb`, 2026-10-17 22:16:52 UTC, 14.5 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 12 records, 200 changes per client, seed 3
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `/mnt/share/runs/nfs-one-heavy-PerRecordFile-3/data`; run folder `stress-runs/nfs/nfs-one-heavy-PerRecordFile-3`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4812 | 1387 | 96.0 | 1392 | 2033 | 0 | 60.0 | 85.8 | 137.9 |

Writes by outcome: conflict 1392, saved 1387, wrongStatus 2033. Saves by action: Create 12, Update 248, Submit 12, Approve 7, Reject 5, Comment 1103. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 44255 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 44477 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 38369 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 3 | 1387 | 96.0 | 1392 | 2033 | 0 | 0 | 0 | 85.8 | yes |
