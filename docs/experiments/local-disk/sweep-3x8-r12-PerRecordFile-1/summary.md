# Stress run: PerRecordFile, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:42:47 UTC, 3.3 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 12 records, 200 changes per client, seed 1
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r12-PerRecordFile-1/data`; run folder `stress-runs/sweep/sweep-3x8-r12-PerRecordFile-1`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4812 | 1441 | 431.4 | 1455 | 1916 | 0 | 11.5 | 19.7 | 112.6 |

Writes by outcome: conflict 1455, saved 1441, wrongStatus 1916. Saves by action: Create 12, Update 192, Submit 12, Approve 5, Reject 7, Comment 1213. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 42045 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 32995 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 41615 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 3 | 1441 | 431.4 | 1455 | 1916 | 0 | 0 | 0 | 19.7 | yes |
