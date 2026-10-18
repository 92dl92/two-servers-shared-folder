# Stress run: PerRecordFile, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:35:43 UTC, 12.7 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 1 records, 200 changes per client, seed 2
- Clients by role: 1 Registrant, 20 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r01-PerRecordFile-2/data`; run folder `stress-runs/sweep/sweep-3x8-r01-PerRecordFile-2`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4801 | 472 | 37.1 | 4078 | 251 | 0 | 16.3 | 35.2 | 168.2 |

Writes by outcome: conflict 4078, saved 472, wrongStatus 251. Saves by action: Create 1, Submit 1, Reject 1, Comment 469. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 1/1 | 0 | 0 | 0 | 0 | 0 |
| server 33773 | pass | 1/1 | 0 | 0 | 0 | 0 | 0 |
| server 43561 | pass | 1/1 | 0 | 0 | 0 | 0 | 0 |
| server 34311 | pass | 1/1 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 3 | 472 | 37.1 | 4078 | 251 | 0 | 0 | 0 | 35.2 | yes |
