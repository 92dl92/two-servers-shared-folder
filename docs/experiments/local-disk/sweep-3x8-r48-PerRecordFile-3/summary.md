# Stress run: PerRecordFile, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:50:08 UTC, 3.1 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 48 records, 200 changes per client, seed 3
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r48-PerRecordFile-3/data`; run folder `stress-runs/sweep/sweep-3x8-r48-PerRecordFile-3`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4848 | 2320 | 755.7 | 579 | 1949 | 0 | 10.6 | 20.6 | 114.4 |

Writes by outcome: conflict 579, saved 2320, wrongStatus 1949. Saves by action: Create 48, Update 383, Submit 48, Approve 22, Reject 26, Comment 1793. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 48/48 | 0 | 0 | 0 | 0 | 0 |
| server 33387 | pass | 48/48 | 0 | 0 | 0 | 0 | 0 |
| server 42275 | pass | 48/48 | 0 | 0 | 0 | 0 | 0 |
| server 39691 | pass | 48/48 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 3 | 2320 | 755.7 | 579 | 1949 | 0 | 0 | 0 | 20.6 | yes |
