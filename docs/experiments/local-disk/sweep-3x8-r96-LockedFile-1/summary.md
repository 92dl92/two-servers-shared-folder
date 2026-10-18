# Stress run: LockedFile, 3 server(s): PASSED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:51:28 UTC, 54.5 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 96 records, 200 changes per client, seed 1
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r96-LockedFile-1/data`; run folder `stress-runs/sweep/sweep-3x8-r96-LockedFile-1`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4896 | 2825 | 51.9 | 225 | 1846 | 0 | 14.1 | 521.8 | 2030.0 |

Writes by outcome: conflict 225, saved 2825, wrongStatus 1846. Saves by action: Create 96, Update 502, Submit 96, Approve 53, Reject 43, Comment 2035. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 96/96 | 0 | 0 | 0 | 0 | 0 |
| server 43609 | pass | 96/96 | 0 | 0 | 0 | 0 | 0 |
| server 46717 | pass | 96/96 | 0 | 0 | 0 | 0 | 0 |
| server 43079 | pass | 96/96 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| LockedFile | 3 | 2825 | 51.9 | 225 | 1846 | 0 | 0 | 0 | 521.8 | yes |
