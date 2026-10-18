# Stress run: PerRecordFile, 2 server(s): PASSED

- Commit `1e1cb644088b08a5829d59b74606a6aa4529ac37`, 2026-10-18 13:44:16 UTC, 3.1 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 5 clients, 6 records, 40 changes per client, seed 2
- Clients by role: 4 Registrant, 4 Reviewer, 2 Admin
- Data folder `stress-runs/win-retry-w2-light-PerRecordFile-2/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-retry-w2-light-PerRecordFile-2`
- 1 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-retry-w2-light-PerRecordFile-2` there
- 1 of the servers on `Administrator@ws2.example`, data folder `\\ws1.example\share\runs\win-retry-w2-light-PerRecordFile-2` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 406 | 196 | 63.0 | 129 | 81 | 0 | 31.9 | 81.9 | 185.3 |

Writes by outcome: conflict 129, saved 196, wrongStatus 81. Saves by action: Create 6, Update 60, Submit 6, Approve 2, Reject 3, Comment 119. Reads: 400.

Stale reads: 0 (older than a version already acknowledged); 116 reads followed a save through another server.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server ws1.example:40935 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server ws2.example:33361 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 2 | 196 | 63.0 | 129 | 81 | 0 | 0 | 0 | 81.9 | yes |
