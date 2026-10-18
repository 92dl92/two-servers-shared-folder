# Stress run: PerRecordFile, 2 server(s): PASSED

- Commit `1e1cb644088b08a5829d59b74606a6aa4529ac37`, 2026-10-18 13:42:52 UTC, 18.4 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 12 clients, 12 records, 200 changes per client, seed 3
- Clients by role: 10 Registrant, 10 Reviewer, 4 Admin
- Data folder `stress-runs/win-retry-w0-heavy-PerRecordFile-3/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-retry-w0-heavy-PerRecordFile-3`
- 2 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-retry-w0-heavy-PerRecordFile-3` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4812 | 1729 | 94.1 | 1696 | 1387 | 0 | 37.6 | 95.4 | 221.7 |

Writes by outcome: conflict 1696, saved 1729, wrongStatus 1387. Saves by action: Create 12, Update 89, Submit 12, Approve 5, Reject 7, Comment 1604. Reads: 4800.

Stale reads: 0 (older than a version already acknowledged); 2178 reads followed a save through another server.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server ws1.example:33209 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server ws1.example:44063 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 2 | 1729 | 94.1 | 1696 | 1387 | 0 | 0 | 0 | 95.4 | yes |
