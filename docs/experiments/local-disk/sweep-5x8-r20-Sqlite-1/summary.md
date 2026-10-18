# Stress run: Sqlite, 5 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 21:01:06 UTC, 16.6 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 5 servers × 8 clients, 20 records, 200 changes per client, seed 1
- Clients by role: 20 Registrant, 15 Reviewer, 5 Admin
- Data folder `stress-runs/sweep/sweep-5x8-r20-Sqlite-1/data`; run folder `stress-runs/sweep/sweep-5x8-r20-Sqlite-1`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 8020 | 3544 | 213.7 | 819 | 3657 | 0 | 14.5 | 33.9 | 355.3 |

Writes by outcome: conflict 819, saved 3544, wrongStatus 3657. Saves by action: Create 20, Update 421, Submit 20, Approve 12, Reject 8, Comment 3063. Reads: 8000.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |
| server 37505 | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |
| server 41709 | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |
| server 35341 | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |
| server 40281 | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |
| server 40677 | pass | 20/20 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| Sqlite | 5 | 3544 | 213.7 | 819 | 3657 | 0 | 0 | 0 | 33.9 | yes |
