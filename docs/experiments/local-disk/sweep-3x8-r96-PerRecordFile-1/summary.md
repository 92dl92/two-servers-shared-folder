# Stress run: PerRecordFile, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:54:26 UTC, 3.0 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 96 records, 200 changes per client, seed 1
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r96-PerRecordFile-1/data`; run folder `stress-runs/sweep/sweep-3x8-r96-PerRecordFile-1`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4896 | 2863 | 958.7 | 357 | 1676 | 0 | 10.5 | 18.9 | 115.7 |

Writes by outcome: conflict 357, saved 2863, wrongStatus 1676. Saves by action: Create 96, Update 670, Submit 96, Approve 48, Reject 48, Comment 1905. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 96/96 | 0 | 0 | 0 | 0 | 0 |
| server 42429 | pass | 96/96 | 0 | 0 | 0 | 0 | 0 |
| server 33493 | pass | 96/96 | 0 | 0 | 0 | 0 | 0 |
| server 33671 | pass | 96/96 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 3 | 2863 | 958.7 | 357 | 1676 | 0 | 0 | 0 | 18.9 | yes |
