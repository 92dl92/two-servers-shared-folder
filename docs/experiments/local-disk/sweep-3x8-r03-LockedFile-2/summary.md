# Stress run: LockedFile, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:38:15 UTC, 36.0 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 3 records, 200 changes per client, seed 2
- Clients by role: 3 Registrant, 18 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r03-LockedFile-2/data`; run folder `stress-runs/sweep/sweep-3x8-r03-LockedFile-2`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4803 | 1851 | 51.4 | 2456 | 496 | 0 | 11.7 | 71.5 | 183.0 |

Writes by outcome: conflict 2456, saved 1851, wrongStatus 496. Saves by action: Create 3, Update 17, Submit 3, Approve 2, Reject 1, Comment 1825. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 3/3 | 0 | 0 | 0 | 0 | 0 |
| server 45521 | pass | 3/3 | 0 | 0 | 0 | 0 | 0 |
| server 39041 | pass | 3/3 | 0 | 0 | 0 | 0 | 0 |
| server 45677 | pass | 3/3 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| LockedFile | 3 | 1851 | 51.4 | 2456 | 496 | 0 | 0 | 0 | 71.5 | yes |
