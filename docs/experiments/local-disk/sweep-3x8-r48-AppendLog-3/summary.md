# Stress run: AppendLog, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:50:26 UTC, 3.8 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 48 records, 200 changes per client, seed 3
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r48-AppendLog-3/data`; run folder `stress-runs/sweep/sweep-3x8-r48-AppendLog-3`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4848 | 2355 | 622.0 | 542 | 1951 | 0 | 7.1 | 44.4 | 232.6 |

Writes by outcome: conflict 542, saved 2355, wrongStatus 1951. Saves by action: Create 48, Update 392, Submit 48, Approve 19, Reject 29, Comment 1819. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 48/48 | 0 | 0 | 0 | 0 | 0 |
| server 45795 | pass | 48/48 | 0 | 0 | 0 | 0 | 0 |
| server 39307 | pass | 48/48 | 0 | 0 | 0 | 0 | 0 |
| server 33377 | pass | 48/48 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| AppendLog | 3 | 2355 | 622.0 | 542 | 1951 | 0 | 0 | 0 | 44.4 | yes |
