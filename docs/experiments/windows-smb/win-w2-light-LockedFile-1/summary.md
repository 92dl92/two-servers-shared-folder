# Stress run: LockedFile, 2 server(s): PASSED

- Commit `c71df76c37c18ca38a44e27c02961e268fbb1c09`, 2026-10-18 12:06:22 UTC, 8.6 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 5 clients, 6 records, 40 changes per client, seed 1
- Clients by role: 4 Registrant, 4 Reviewer, 2 Admin
- Data folder `stress-runs/win-w2-light-LockedFile-1/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-w2-light-LockedFile-1`
- 1 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-w2-light-LockedFile-1` there
- 1 of the servers on `Administrator@ws2.example`, data folder `\\ws1.example\share\runs\win-w2-light-LockedFile-1` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 406 | 223 | 25.9 | 76 | 107 | 0 | 26.4 | 245.9 | 405.0 |

Writes by outcome: conflict 76, saved 223, wrongStatus 107. Saves by action: Create 6, Update 45, Submit 6, Approve 4, Reject 2, Comment 160. Reads: 400.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server ws1.example:36463 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server ws2.example:38301 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| LockedFile | 2 | 223 | 25.9 | 76 | 107 | 0 | 0 | 0 | 245.9 | yes |
