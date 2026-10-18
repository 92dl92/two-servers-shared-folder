# Stress run: AppendLog, 3 server(s): PASSED

- Commit `1b49659b6d0d95e1a10858057ef4082c27040edb`, 2026-10-17 22:17:33 UTC, 19.7 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 12 records, 200 changes per client, seed 2
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `/mnt/share/runs/nfs-one-heavy-AppendLog-2/data`; run folder `stress-runs/nfs/nfs-one-heavy-AppendLog-2`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4812 | 1409 | 71.4 | 1118 | 2285 | 0 | 66.3 | 163.6 | 344.8 |

Writes by outcome: conflict 1118, saved 1409, wrongStatus 2285. Saves by action: Create 12, Update 122, Submit 12, Approve 4, Reject 8, Comment 1251. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 37671 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 42291 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 46459 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| AppendLog | 3 | 1409 | 71.4 | 1118 | 2285 | 0 | 0 | 0 | 163.6 | yes |
