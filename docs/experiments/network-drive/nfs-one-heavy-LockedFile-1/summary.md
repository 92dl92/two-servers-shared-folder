# Stress run: LockedFile, 3 server(s): PASSED

- Commit `1b49659b6d0d95e1a10858057ef4082c27040edb`, 2026-10-17 22:02:39 UTC, 261.0 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 12 records, 200 changes per client, seed 1
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `/mnt/share/runs/nfs-one-heavy-LockedFile-1/data`; run folder `stress-runs/nfs/nfs-one-heavy-LockedFile-1`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4812 | 1532 | 5.9 | 1376 | 1904 | 0 | 572.2 | 1761.1 | 4166.6 |

Writes by outcome: conflict 1376, saved 1532, wrongStatus 1904. Saves by action: Create 12, Update 82, Submit 12, Approve 3, Reject 9, Comment 1414. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 36851 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 40223 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 34023 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| LockedFile | 3 | 1532 | 5.9 | 1376 | 1904 | 0 | 0 | 0 | 1761.1 | yes |
