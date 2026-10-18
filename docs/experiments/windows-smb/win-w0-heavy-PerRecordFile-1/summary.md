# Stress run: PerRecordFile, 2 server(s): PASSED

- Commit `c71df76c37c18ca38a44e27c02961e268fbb1c09`, 2026-10-18 11:52:22 UTC, 19.6 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 12 clients, 12 records, 200 changes per client, seed 1
- Clients by role: 10 Registrant, 10 Reviewer, 4 Admin
- Data folder `stress-runs/win-w0-heavy-PerRecordFile-1/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-w0-heavy-PerRecordFile-1`
- 2 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-w0-heavy-PerRecordFile-1` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4812 | 1710 | 87.3 | 1662 | 1395 | 0 | 40.3 | 96.9 | 230.8 |

Writes by outcome: conflict 1662, http500 45, saved 1710, wrongStatus 1395. Saves by action: Create 12, Update 51, Submit 12, Approve 7, Reject 5, Comment 1623. Reads: 4800.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server ws1.example:46815 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server ws1.example:36147 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 2 | 1710 | 87.3 | 1662 | 1395 | 0 | 0 | 0 | 96.9 | yes |
