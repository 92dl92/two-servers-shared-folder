# Stress run: Sqlite, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:46:58 UTC, 9.9 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 24 records, 200 changes per client, seed 2
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r24-Sqlite-2/data`; run folder `stress-runs/sweep/sweep-3x8-r24-Sqlite-2`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4824 | 2359 | 237.7 | 358 | 2107 | 0 | 14.4 | 32.0 | 277.8 |

Writes by outcome: conflict 358, saved 2359, wrongStatus 2107. Saves by action: Create 24, Update 329, Submit 24, Approve 17, Reject 7, Comment 1958. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 24/24 | 0 | 0 | 0 | 0 | 0 |
| server 40617 | pass | 24/24 | 0 | 0 | 0 | 0 | 0 |
| server 46205 | pass | 24/24 | 0 | 0 | 0 | 0 | 0 |
| server 34839 | pass | 24/24 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| Sqlite | 3 | 2359 | 237.7 | 358 | 2107 | 0 | 0 | 0 | 32.0 | yes |
