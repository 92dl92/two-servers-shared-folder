# Stress run: PerRecordFile, 2 server(s): PASSED

- Commit `a8d23669d0f322853d3ffd61c6303a649659db79`, 2026-10-18 15:23:28 UTC, 126.6 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 5 clients, pipeline with 1000 ms think time, 120 changes per client, seed 1
- Clients by role: 4 Registrant, 4 Reviewer, 2 Admin
- Data folder `stress-runs/win-lease-full-w2-real-PerRecordFile-1/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-lease-full-w2-real-PerRecordFile-1`
- 1 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-lease-full-w2-real-PerRecordFile-1` there
- 1 of the servers on `Administrator@ws2.example`, data folder `\\ws1.example\share\runs\win-lease-full-w2-real-PerRecordFile-1` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 739 | 712 | 5.6 | 27 | 0 | 0 | 18.8 | 48.6 | 195.4 |

Writes by outcome: conflict 27, saved 712. Saves by action: Create 164, Update 154, Submit 164, Approve 124, Reject 40, Comment 66. Reads: 577.

Longest stall: 1.0 s with no save acknowledged, from 0.0 s into the run (think time 1000 ms, so pauses up to about 1.5 s are normal).

Stale reads: 0 (older than a version already acknowledged); 130 reads followed a save through another server.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 164/164 | 0 | 0 | 0 | 0 | 0 |
| server ws1.example:38977 | pass | 164/164 | 0 | 0 | 0 | 0 | 0 |
| server ws2.example:44853 | pass | 164/164 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 2 | 712 | 5.6 | 27 | 0 | 0 | 0 | 0 | 48.6 | yes |
