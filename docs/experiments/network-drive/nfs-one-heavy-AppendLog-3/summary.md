# Stress run: AppendLog, 3 server(s): PASSED

- Commit `1b49659b6d0d95e1a10858057ef4082c27040edb`, 2026-10-17 22:17:56 UTC, 20.1 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 12 records, 200 changes per client, seed 3
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `/mnt/share/runs/nfs-one-heavy-AppendLog-3/data`; run folder `stress-runs/nfs/nfs-one-heavy-AppendLog-3`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4812 | 1417 | 70.5 | 1136 | 2259 | 0 | 67.2 | 172.6 | 338.1 |

Writes by outcome: conflict 1136, saved 1417, wrongStatus 2259. Saves by action: Create 12, Update 143, Submit 12, Approve 6, Reject 6, Comment 1238. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 44055 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 45331 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 38681 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| AppendLog | 3 | 1417 | 70.5 | 1136 | 2259 | 0 | 0 | 0 | 172.6 | yes |
