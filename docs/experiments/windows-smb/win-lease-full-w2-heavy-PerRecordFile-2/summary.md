# Stress run: PerRecordFile, 2 server(s): PASSED

- Commit `a8d23669d0f322853d3ffd61c6303a649659db79`, 2026-10-18 15:16:41 UTC, 56.7 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 12 clients, 12 records, 200 changes per client, seed 2
- Clients by role: 10 Registrant, 10 Reviewer, 4 Admin
- Data folder `stress-runs/win-lease-full-w2-heavy-PerRecordFile-2/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-lease-full-w2-heavy-PerRecordFile-2`
- 1 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-lease-full-w2-heavy-PerRecordFile-2` there
- 1 of the servers on `Administrator@ws2.example`, data folder `\\ws1.example\share\runs\win-lease-full-w2-heavy-PerRecordFile-2` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4812 | 1780 | 31.4 | 1274 | 1755 | 0 | 41.0 | 131.1 | 35617.5 |

Writes by outcome: conflict 1274, http500 3, saved 1780, wrongStatus 1755. Saves by action: Create 12, Update 105, Submit 12, Approve 4, Reject 8, Comment 1639. Reads: 4800.

Longest stall: 24.1 s with no save acknowledged, from 19.3 s into the run.

Stale reads: 0 (older than a version already acknowledged); 1736 reads followed a save through another server.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server ws1.example:41009 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server ws2.example:44685 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 2 | 1780 | 31.4 | 1274 | 1755 | 0 | 0 | 0 | 131.1 | yes |
