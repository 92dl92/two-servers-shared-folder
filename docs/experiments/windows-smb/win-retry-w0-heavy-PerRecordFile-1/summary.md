# Stress run: PerRecordFile, 2 server(s): PASSED

- Commit `1e1cb644088b08a5829d59b74606a6aa4529ac37`, 2026-10-18 13:41:10 UTC, 16.4 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 12 clients, 12 records, 200 changes per client, seed 1
- Clients by role: 10 Registrant, 10 Reviewer, 4 Admin
- Data folder `stress-runs/win-retry-w0-heavy-PerRecordFile-1/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-retry-w0-heavy-PerRecordFile-1`
- 2 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-retry-w0-heavy-PerRecordFile-1` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4812 | 1671 | 101.9 | 1664 | 1477 | 0 | 36.6 | 92.8 | 174.1 |

Writes by outcome: conflict 1664, saved 1671, wrongStatus 1477. Saves by action: Create 12, Update 49, Submit 12, Approve 5, Reject 7, Comment 1586. Reads: 4800.

Stale reads: 0 (older than a version already acknowledged); 2214 reads followed a save through another server.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server ws1.example:41763 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server ws1.example:45315 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 2 | 1671 | 101.9 | 1664 | 1477 | 0 | 0 | 0 | 92.8 | yes |
