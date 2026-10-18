# Stress run: PerRecordFile, 2 server(s): PASSED

- Commit `1e1cb644088b08a5829d59b74606a6aa4529ac37`, 2026-10-18 13:39:28 UTC, 1.9 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 5 clients, 6 records, 40 changes per client, seed 1
- Clients by role: 4 Registrant, 4 Reviewer, 2 Admin
- Data folder `stress-runs/win-retry-w0-light-PerRecordFile-1/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-retry-w0-light-PerRecordFile-1`
- 2 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-retry-w0-light-PerRecordFile-1` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 406 | 169 | 87.4 | 128 | 109 | 0 | 15.8 | 50.4 | 130.5 |

Writes by outcome: conflict 128, saved 169, wrongStatus 109. Saves by action: Create 6, Update 22, Submit 6, Approve 4, Reject 2, Comment 129. Reads: 400.

Stale reads: 0 (older than a version already acknowledged); 161 reads followed a save through another server.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server ws1.example:44981 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server ws1.example:39997 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 2 | 169 | 87.4 | 128 | 109 | 0 | 0 | 0 | 50.4 | yes |
