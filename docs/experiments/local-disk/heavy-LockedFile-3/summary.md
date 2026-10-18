# Stress run: LockedFile, 3 server(s): PASSED

- Commit `ee6af83bef739621009cd4be91192b48f8df58df`, 2026-10-17 19:10:02 UTC, 33.6 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 12 records, 200 changes per client, seed 3
- Data folder `stress-runs/heavy-LockedFile-3/data`; run folder `stress-runs/heavy-LockedFile-3`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4812 | 1873 | 55.8 | 965 | 1974 | 0 | 10.7 | 175.5 | 452.8 |

Writes by outcome: conflict 965, saved 1873, wrongStatus 1974. Saves by action: Create 12, Update 82, Submit 12, Approve 4, Reject 8, Comment 1755. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 41489 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 37097 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server 33635 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| LockedFile | 3 | 1873 | 55.8 | 965 | 1974 | 0 | 0 | 0 | 175.5 | yes |
