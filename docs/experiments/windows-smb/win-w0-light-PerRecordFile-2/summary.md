# Stress run: PerRecordFile, 2 server(s): PASSED

- Commit `c71df76c37c18ca38a44e27c02961e268fbb1c09`, 2026-10-18 11:35:58 UTC, 2.1 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 5 clients, 6 records, 40 changes per client, seed 2
- Clients by role: 4 Registrant, 4 Reviewer, 2 Admin
- Data folder `stress-runs/win-w0-light-PerRecordFile-2/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-w0-light-PerRecordFile-2`
- 2 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-w0-light-PerRecordFile-2` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 406 | 171 | 79.7 | 134 | 96 | 0 | 20.5 | 52.5 | 169.8 |

Writes by outcome: conflict 134, http500 5, saved 171, wrongStatus 96. Saves by action: Create 6, Update 34, Submit 6, Approve 4, Reject 2, Comment 119. Reads: 400.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 12/6 | 0 | 0 | 0 | 0 | 0 |
| server ws1.example:33921 | pass | 12/6 | 0 | 0 | 0 | 0 | 0 |
| server ws1.example:37175 | pass | 12/6 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 2 | 171 | 79.7 | 134 | 96 | 0 | 0 | 0 | 52.5 | yes |
