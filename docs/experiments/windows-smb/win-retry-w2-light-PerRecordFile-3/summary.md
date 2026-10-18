# Stress run: PerRecordFile, 2 server(s): PASSED

- Commit `1e1cb644088b08a5829d59b74606a6aa4529ac37`, 2026-10-18 13:44:48 UTC, 3.1 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 5 clients, 6 records, 40 changes per client, seed 3
- Clients by role: 4 Registrant, 4 Reviewer, 2 Admin
- Data folder `stress-runs/win-retry-w2-light-PerRecordFile-3/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-retry-w2-light-PerRecordFile-3`
- 1 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-retry-w2-light-PerRecordFile-3` there
- 1 of the servers on `Administrator@ws2.example`, data folder `\\ws1.example\share\runs\win-retry-w2-light-PerRecordFile-3` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 406 | 192 | 61.9 | 106 | 108 | 0 | 26.0 | 91.0 | 209.9 |

Writes by outcome: conflict 106, saved 192, wrongStatus 108. Saves by action: Create 6, Update 34, Submit 6, Approve 5, Reject 1, Comment 140. Reads: 400.

Stale reads: 0 (older than a version already acknowledged); 103 reads followed a save through another server.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server ws1.example:42763 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server ws2.example:40939 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 2 | 192 | 61.9 | 106 | 108 | 0 | 0 | 0 | 91.0 | yes |
