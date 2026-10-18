# Stress run: PerRecordFile, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:39:39 UTC, 5.2 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 3 records, 200 changes per client, seed 2
- Clients by role: 3 Registrant, 18 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r03-PerRecordFile-2/data`; run folder `stress-runs/sweep/sweep-3x8-r03-PerRecordFile-2`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4803 | 765 | 146.1 | 3489 | 549 | 0 | 13.5 | 24.6 | 121.6 |

Writes by outcome: conflict 3489, saved 765, wrongStatus 549. Saves by action: Create 3, Update 9, Submit 3, Approve 2, Reject 1, Comment 747. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 3/3 | 0 | 0 | 0 | 0 | 0 |
| server 33289 | pass | 3/3 | 0 | 0 | 0 | 0 | 0 |
| server 39495 | pass | 3/3 | 0 | 0 | 0 | 0 | 0 |
| server 35473 | pass | 3/3 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 3 | 765 | 146.1 | 3489 | 549 | 0 | 0 | 0 | 24.6 | yes |
