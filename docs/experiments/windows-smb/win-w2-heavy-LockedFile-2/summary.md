# Stress run: LockedFile, 2 server(s): PASSED

- Commit `c71df76c37c18ca38a44e27c02961e268fbb1c09`, 2026-10-18 12:22:50 UTC, 106.2 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 12 clients, 12 records, 200 changes per client, seed 2
- Clients by role: 10 Registrant, 10 Reviewer, 4 Admin
- Data folder `stress-runs/win-w2-heavy-LockedFile-2/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-w2-heavy-LockedFile-2`
- 1 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-w2-heavy-LockedFile-2` there
- 1 of the servers on `Administrator@ws2.example`, data folder `\\ws1.example\share\runs\win-w2-heavy-LockedFile-2` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4812 | 2156 | 20.3 | 992 | 1664 | 0 | 34.1 | 504.1 | 1338.4 |

Writes by outcome: conflict 992, saved 2156, wrongStatus 1664. Saves by action: Create 12, Update 55, Submit 12, Approve 7, Reject 5, Comment 2065. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server ws1.example:37653 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server ws2.example:39945 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| LockedFile | 2 | 2156 | 20.3 | 992 | 1664 | 0 | 0 | 0 | 504.1 | yes |
