# Stress run: AppendLog, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:50:14 UTC, 3.7 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 48 records, 200 changes per client, seed 1
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r48-AppendLog-1/data`; run folder `stress-runs/sweep/sweep-3x8-r48-AppendLog-1`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4848 | 2270 | 613.4 | 509 | 2069 | 0 | 7.6 | 41.7 | 204.0 |

Writes by outcome: conflict 509, saved 2270, wrongStatus 2069. Saves by action: Create 48, Update 301, Submit 48, Approve 25, Reject 23, Comment 1825. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 48/48 | 0 | 0 | 0 | 0 | 0 |
| server 41583 | pass | 48/48 | 0 | 0 | 0 | 0 | 0 |
| server 39247 | pass | 48/48 | 0 | 0 | 0 | 0 | 0 |
| server 45633 | pass | 48/48 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| AppendLog | 3 | 2270 | 613.4 | 509 | 2069 | 0 | 0 | 0 | 41.7 | yes |
