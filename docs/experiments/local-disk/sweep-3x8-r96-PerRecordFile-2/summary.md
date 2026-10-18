# Stress run: PerRecordFile, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:54:31 UTC, 3.1 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 96 records, 200 changes per client, seed 2
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r96-PerRecordFile-2/data`; run folder `stress-runs/sweep/sweep-3x8-r96-PerRecordFile-2`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4896 | 2864 | 914.4 | 343 | 1689 | 0 | 10.8 | 20.1 | 123.0 |

Writes by outcome: conflict 343, saved 2864, wrongStatus 1689. Saves by action: Create 96, Update 666, Submit 96, Approve 54, Reject 42, Comment 1910. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 96/96 | 0 | 0 | 0 | 0 | 0 |
| server 40763 | pass | 96/96 | 0 | 0 | 0 | 0 | 0 |
| server 44593 | pass | 96/96 | 0 | 0 | 0 | 0 | 0 |
| server 35809 | pass | 96/96 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 3 | 2864 | 914.4 | 343 | 1689 | 0 | 0 | 0 | 20.1 | yes |
