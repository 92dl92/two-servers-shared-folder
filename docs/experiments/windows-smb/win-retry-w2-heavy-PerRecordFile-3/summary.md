# Stress run: PerRecordFile, 2 server(s): PASSED

- Commit `1e1cb644088b08a5829d59b74606a6aa4529ac37`, 2026-10-18 13:47:37 UTC, 21.3 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 12 clients, 12 records, 200 changes per client, seed 3
- Clients by role: 10 Registrant, 10 Reviewer, 4 Admin
- Data folder `stress-runs/win-retry-w2-heavy-PerRecordFile-3/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-retry-w2-heavy-PerRecordFile-3`
- 1 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-retry-w2-heavy-PerRecordFile-3` there
- 1 of the servers on `Administrator@ws2.example`, data folder `\\ws1.example\share\runs\win-retry-w2-heavy-PerRecordFile-3` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4812 | 1745 | 81.9 | 1279 | 1788 | 0 | 42.5 | 148.7 | 387.6 |

Writes by outcome: conflict 1279, saved 1745, wrongStatus 1788. Saves by action: Create 12, Update 51, Submit 12, Approve 7, Reject 5, Comment 1658. Reads: 4800.

Stale reads: 0 (older than a version already acknowledged); 1646 reads followed a save through another server.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server ws1.example:41373 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server ws2.example:39081 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 2 | 1745 | 81.9 | 1279 | 1788 | 0 | 0 | 0 | 148.7 | yes |
