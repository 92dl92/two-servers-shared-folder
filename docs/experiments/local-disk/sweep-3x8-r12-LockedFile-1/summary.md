# Stress run: LockedFile, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:41:04 UTC, 32.9 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 12 records, 200 changes per client, seed 1
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r12-LockedFile-1/data`; run folder `stress-runs/sweep/sweep-3x8-r12-LockedFile-1`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4812 | 1857 | 56.5 | 974 | 1981 | 0 | 10.5 | 174.1 | 644.8 |

Writes by outcome: conflict 974, saved 1857, wrongStatus 1981. Saves by action: Create 12, Update 101, Submit 12, Approve 8, Reject 4, Comment 1720. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 43473 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 45927 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 40145 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| LockedFile | 3 | 1857 | 56.5 | 974 | 1981 | 0 | 0 | 0 | 174.1 | yes |
