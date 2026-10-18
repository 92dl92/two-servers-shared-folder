# Stress run: AppendLog, 2 server(s): PASSED

- Commit `c71df76c37c18ca38a44e27c02961e268fbb1c09`, 2026-10-18 11:39:38 UTC, 2.5 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 5 clients, 6 records, 40 changes per client, seed 3
- Clients by role: 4 Registrant, 4 Reviewer, 2 Admin
- Data folder `stress-runs/win-w0-light-AppendLog-3/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-w0-light-AppendLog-3`
- 2 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-w0-light-AppendLog-3` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 406 | 187 | 73.7 | 105 | 114 | 0 | 23.9 | 72.2 | 217.4 |

Writes by outcome: conflict 105, saved 187, wrongStatus 114. Saves by action: Create 6, Update 23, Submit 6, Approve 3, Reject 3, Comment 146. Reads: 400.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server ws1.example:45601 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server ws1.example:33001 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| AppendLog | 2 | 187 | 73.7 | 105 | 114 | 0 | 0 | 0 | 72.2 | yes |
