# Stress run: PerRecordFile, 2 server(s): PASSED

- Commit `a8d23669d0f322853d3ffd61c6303a649659db79`, 2026-10-18 15:37:31 UTC, 44.7 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 12 clients, 12 records, 200 changes per client, seed 6
- Clients by role: 10 Registrant, 10 Reviewer, 4 Admin
- Data folder `stress-runs/win-lease-none-w2-heavy-PerRecordFile-6/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-lease-none-w2-heavy-PerRecordFile-6`
- 1 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-lease-none-w2-heavy-PerRecordFile-6` there
- 1 of the servers on `Administrator@ws2.example`, data folder `\\ws1.example\share\runs\win-lease-none-w2-heavy-PerRecordFile-6` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4812 | 1788 | 40.0 | 1236 | 1778 | 0 | 71.9 | 290.4 | 1666.7 |

Writes by outcome: conflict 1236, http500 10, saved 1788, wrongStatus 1778. Saves by action: Create 12, Update 69, Submit 12, Approve 9, Reject 3, Comment 1683. Reads: 4800.

Longest stall: 0.2 s with no save acknowledged, from 10.8 s into the run.

Stale reads: 0 (older than a version already acknowledged); 1192 reads followed a save through another server.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server ws1.example:44793 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |
| server ws2.example:40351 | pass | 12/12 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 2 | 1788 | 40.0 | 1236 | 1778 | 0 | 0 | 0 | 290.4 | yes |
