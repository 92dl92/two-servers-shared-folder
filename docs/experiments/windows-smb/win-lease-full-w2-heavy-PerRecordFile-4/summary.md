# Stress run: PerRecordFile, 2 server(s): PASSED

- Commit `a8d23669d0f322853d3ffd61c6303a649659db79`, 2026-10-18 15:19:01 UTC, 28.9 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 12 clients, 12 records, 200 changes per client, seed 4
- Clients by role: 10 Registrant, 10 Reviewer, 4 Admin
- Data folder `stress-runs/win-lease-full-w2-heavy-PerRecordFile-4/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-lease-full-w2-heavy-PerRecordFile-4`
- 1 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-lease-full-w2-heavy-PerRecordFile-4` there
- 1 of the servers on `Administrator@ws2.example`, data folder `\\ws1.example\share\runs\win-lease-full-w2-heavy-PerRecordFile-4` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4812 | 1781 | 61.6 | 1313 | 1718 | 0 | 55.3 | 203.4 | 501.7 |

Writes by outcome: conflict 1313, saved 1781, wrongStatus 1718. Saves by action: Create 12, Update 161, Submit 12, Approve 9, Reject 3, Comment 1584. Reads: 4800.

Longest stall: 0.2 s with no save acknowledged, from 0.0 s into the run.

Stale reads: 0 (older than a version already acknowledged); 1653 reads followed a save through another server.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server ws1.example:39525 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server ws2.example:42973 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 2 | 1781 | 61.6 | 1313 | 1718 | 0 | 0 | 0 | 203.4 | yes |
