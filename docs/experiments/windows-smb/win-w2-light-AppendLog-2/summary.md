# Stress run: AppendLog, 2 server(s): PASSED

- Commit `c71df76c37c18ca38a44e27c02961e268fbb1c09`, 2026-10-18 12:12:47 UTC, 4.9 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 5 clients, 6 records, 40 changes per client, seed 2
- Clients by role: 4 Registrant, 4 Reviewer, 2 Admin
- Data folder `stress-runs/win-w2-light-AppendLog-2/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-w2-light-AppendLog-2`
- 1 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-w2-light-AppendLog-2` there
- 1 of the servers on `Administrator@ws2.example`, data folder `\\ws1.example\share\runs\win-w2-light-AppendLog-2` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 406 | 227 | 45.9 | 106 | 73 | 0 | 38.3 | 112.4 | 226.7 |

Writes by outcome: conflict 106, saved 227, wrongStatus 73. Saves by action: Create 6, Update 66, Submit 6, Approve 3, Reject 3, Comment 143. Reads: 400.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server ws1.example:35003 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server ws2.example:33773 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| AppendLog | 2 | 227 | 45.9 | 106 | 73 | 0 | 0 | 0 | 112.4 | yes |
