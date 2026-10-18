# Stress run: PerRecordFile, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:46:09 UTC, 3.2 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 24 records, 200 changes per client, seed 1
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r24-PerRecordFile-1/data`; run folder `stress-runs/sweep/sweep-3x8-r24-PerRecordFile-1`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4824 | 1874 | 582.4 | 905 | 2045 | 0 | 11.3 | 21.1 | 123.7 |

Writes by outcome: conflict 905, saved 1874, wrongStatus 2045. Saves by action: Create 24, Update 223, Submit 24, Approve 14, Reject 10, Comment 1579. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 24/24 | 0 | 0 | 0 | 0 | 0 |
| server 39347 | pass | 24/24 | 0 | 0 | 0 | 0 | 0 |
| server 41555 | pass | 24/24 | 0 | 0 | 0 | 0 | 0 |
| server 44601 | pass | 24/24 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 3 | 1874 | 582.4 | 905 | 2045 | 0 | 0 | 0 | 21.1 | yes |
