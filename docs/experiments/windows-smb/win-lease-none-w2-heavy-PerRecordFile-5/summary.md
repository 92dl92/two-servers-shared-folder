# Stress run: PerRecordFile, 2 server(s): PASSED

- Commit `a8d23669d0f322853d3ffd61c6303a649659db79`, 2026-10-18 15:36:13 UTC, 47.2 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 12 clients, 12 records, 200 changes per client, seed 5
- Clients by role: 10 Registrant, 10 Reviewer, 4 Admin
- Data folder `stress-runs/win-lease-none-w2-heavy-PerRecordFile-5/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-lease-none-w2-heavy-PerRecordFile-5`
- 1 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-lease-none-w2-heavy-PerRecordFile-5` there
- 1 of the servers on `Administrator@ws2.example`, data folder `\\ws1.example\share\runs\win-lease-none-w2-heavy-PerRecordFile-5` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4812 | 1838 | 38.9 | 1267 | 1701 | 0 | 71.0 | 307.7 | 1910.4 |

Writes by outcome: conflict 1267, http500 6, saved 1838, wrongStatus 1701. Saves by action: Create 12, Update 96, Submit 12, Approve 6, Reject 6, Comment 1706. Reads: 4800.

Longest stall: 0.3 s with no save acknowledged, from 18.8 s into the run.

Stale reads: 0 (older than a version already acknowledged); 1076 reads followed a save through another server.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server ws1.example:39661 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server ws2.example:38669 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 2 | 1838 | 38.9 | 1267 | 1701 | 0 | 0 | 0 | 307.7 | yes |
