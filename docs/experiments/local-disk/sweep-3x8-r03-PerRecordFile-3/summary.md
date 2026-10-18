# Stress run: PerRecordFile, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:39:46 UTC, 5.3 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 3 records, 200 changes per client, seed 3
- Clients by role: 3 Registrant, 18 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r03-PerRecordFile-3/data`; run folder `stress-runs/sweep/sweep-3x8-r03-PerRecordFile-3`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4803 | 765 | 143.2 | 3515 | 523 | 0 | 13.4 | 23.4 | 129.2 |

Writes by outcome: conflict 3515, saved 765, wrongStatus 523. Saves by action: Create 3, Update 18, Submit 3, Approve 3, Comment 738. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 3/3 | 0 | 0 | 0 | 0 | 0 |
| server 33063 | pass | 3/3 | 0 | 0 | 0 | 0 | 0 |
| server 39999 | pass | 3/3 | 0 | 0 | 0 | 0 | 0 |
| server 37445 | pass | 3/3 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 3 | 765 | 143.2 | 3515 | 523 | 0 | 0 | 0 | 23.4 | yes |
