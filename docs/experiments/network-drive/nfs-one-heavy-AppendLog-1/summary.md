# Stress run: AppendLog, 3 server(s): PASSED

- Commit `1b49659b6d0d95e1a10858057ef4082c27040edb`, 2026-10-17 22:17:09 UTC, 20.6 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 12 records, 200 changes per client, seed 1
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `/mnt/share/runs/nfs-one-heavy-AppendLog-1/data`; run folder `stress-runs/nfs/nfs-one-heavy-AppendLog-1`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4812 | 1460 | 71.0 | 1159 | 2193 | 0 | 72.1 | 172.6 | 434.9 |

Writes by outcome: conflict 1159, saved 1460, wrongStatus 2193. Saves by action: Create 12, Update 209, Submit 12, Approve 5, Reject 7, Comment 1215. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 41927 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 46259 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 37285 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| AppendLog | 3 | 1460 | 71.0 | 1159 | 2193 | 0 | 0 | 0 | 172.6 | yes |
