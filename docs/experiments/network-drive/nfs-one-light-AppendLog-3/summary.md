# Stress run: AppendLog, 3 server(s): PASSED

- Commit `1b49659b6d0d95e1a10858057ef4082c27040edb`, 2026-10-17 21:58:37 UTC, 2.9 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 5 clients, 6 records, 40 changes per client, seed 3
- Clients by role: 6 Registrant, 6 Reviewer, 3 Admin
- Data folder `/mnt/share/runs/nfs-one-light-AppendLog-3/data`; run folder `stress-runs/nfs/nfs-one-light-AppendLog-3`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 606 | 222 | 76.8 | 181 | 203 | 0 | 23.5 | 109.5 | 254.5 |

Writes by outcome: conflict 181, saved 222, wrongStatus 203. Saves by action: Create 6, Update 30, Submit 6, Approve 4, Reject 2, Comment 174. Reads: 600.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 44755 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 41445 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server 45077 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| AppendLog | 3 | 222 | 76.8 | 181 | 203 | 0 | 0 | 0 | 109.5 | yes |
