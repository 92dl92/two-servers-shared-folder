# Stress run: PerRecordFile, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:46:14 UTC, 3.2 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 24 records, 200 changes per client, seed 2
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r24-PerRecordFile-2/data`; run folder `stress-runs/sweep/sweep-3x8-r24-PerRecordFile-2`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4824 | 1897 | 588.0 | 900 | 2027 | 0 | 11.1 | 21.8 | 116.8 |

Writes by outcome: conflict 900, saved 1897, wrongStatus 2027. Saves by action: Create 24, Update 200, Submit 24, Approve 12, Reject 12, Comment 1625. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 24/24 | 0 | 0 | 0 | 0 | 0 |
| server 43161 | pass | 24/24 | 0 | 0 | 0 | 0 | 0 |
| server 42361 | pass | 24/24 | 0 | 0 | 0 | 0 | 0 |
| server 38607 | pass | 24/24 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 3 | 1897 | 588.0 | 900 | 2027 | 0 | 0 | 0 | 21.8 | yes |
