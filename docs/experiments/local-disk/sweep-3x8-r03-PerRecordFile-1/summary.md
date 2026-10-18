# Stress run: PerRecordFile, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:39:32 UTC, 5.2 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 3 records, 200 changes per client, seed 1
- Clients by role: 3 Registrant, 18 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r03-PerRecordFile-1/data`; run folder `stress-runs/sweep/sweep-3x8-r03-PerRecordFile-1`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4803 | 784 | 151.4 | 3464 | 555 | 0 | 12.6 | 22.0 | 120.1 |

Writes by outcome: conflict 3464, saved 784, wrongStatus 555. Saves by action: Create 3, Update 13, Submit 3, Approve 2, Reject 1, Comment 762. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 3/3 | 0 | 0 | 0 | 0 | 0 |
| server 45857 | pass | 3/3 | 0 | 0 | 0 | 0 | 0 |
| server 44033 | pass | 3/3 | 0 | 0 | 0 | 0 | 0 |
| server 42873 | pass | 3/3 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 3 | 784 | 151.4 | 3464 | 555 | 0 | 0 | 0 | 22.0 | yes |
