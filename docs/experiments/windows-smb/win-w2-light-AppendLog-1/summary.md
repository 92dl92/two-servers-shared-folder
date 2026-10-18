# Stress run: AppendLog, 2 server(s): PASSED

- Commit `c71df76c37c18ca38a44e27c02961e268fbb1c09`, 2026-10-18 12:11:54 UTC, 4.2 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 5 clients, 6 records, 40 changes per client, seed 1
- Clients by role: 4 Registrant, 4 Reviewer, 2 Admin
- Data folder `stress-runs/win-w2-light-AppendLog-1/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-w2-light-AppendLog-1`
- 1 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-w2-light-AppendLog-1` there
- 1 of the servers on `Administrator@ws2.example`, data folder `\\ws1.example\share\runs\win-w2-light-AppendLog-1` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 406 | 197 | 46.4 | 102 | 107 | 0 | 32.8 | 124.2 | 232.9 |

Writes by outcome: conflict 102, saved 197, wrongStatus 107. Saves by action: Create 6, Update 32, Submit 6, Approve 2, Reject 4, Comment 147. Reads: 400.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server ws1.example:35139 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server ws2.example:33155 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| AppendLog | 2 | 197 | 46.4 | 102 | 107 | 0 | 0 | 0 | 124.2 | yes |
