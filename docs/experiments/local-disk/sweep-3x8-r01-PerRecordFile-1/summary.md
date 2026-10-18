# Stress run: PerRecordFile, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:35:27 UTC, 13.8 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 1 records, 200 changes per client, seed 1
- Clients by role: 1 Registrant, 20 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r01-PerRecordFile-1/data`; run folder `stress-runs/sweep/sweep-3x8-r01-PerRecordFile-1`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4801 | 590 | 42.7 | 3948 | 263 | 0 | 12.9 | 31.8 | 178.6 |

Writes by outcome: conflict 3948, saved 590, wrongStatus 263. Saves by action: Create 1, Update 6, Submit 1, Approve 1, Comment 581. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 1/1 | 0 | 0 | 0 | 0 | 0 |
| server 40303 | pass | 1/1 | 0 | 0 | 0 | 0 | 0 |
| server 46627 | pass | 1/1 | 0 | 0 | 0 | 0 | 0 |
| server 40541 | pass | 1/1 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 3 | 590 | 42.7 | 3948 | 263 | 0 | 0 | 0 | 31.8 | yes |
