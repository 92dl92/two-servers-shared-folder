# Stress run: PerRecordFile, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:50:03 UTC, 3.2 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 48 records, 200 changes per client, seed 2
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r48-PerRecordFile-2/data`; run folder `stress-runs/sweep/sweep-3x8-r48-PerRecordFile-2`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4848 | 2340 | 737.5 | 580 | 1928 | 0 | 10.9 | 19.7 | 116.6 |

Writes by outcome: conflict 580, saved 2340, wrongStatus 1928. Saves by action: Create 48, Update 382, Submit 48, Approve 26, Reject 22, Comment 1814. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 48/48 | 0 | 0 | 0 | 0 | 0 |
| server 45627 | pass | 48/48 | 0 | 0 | 0 | 0 | 0 |
| server 39231 | pass | 48/48 | 0 | 0 | 0 | 0 | 0 |
| server 41623 | pass | 48/48 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 3 | 2340 | 737.5 | 580 | 1928 | 0 | 0 | 0 | 19.7 | yes |
