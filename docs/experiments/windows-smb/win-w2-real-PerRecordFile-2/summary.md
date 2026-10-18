# Stress run: PerRecordFile, 2 server(s): PASSED

- Commit `fe53fea387646cb0ee65f1cccd5a309abb2291d2`, 2026-10-18 13:10:18 UTC, 131.2 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 5 clients, pipeline with 1000 ms think time, 120 changes per client, seed 2
- Clients by role: 4 Registrant, 4 Reviewer, 2 Admin
- Data folder `stress-runs/win-w2-real-PerRecordFile-2/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-w2-real-PerRecordFile-2`
- 1 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-w2-real-PerRecordFile-2` there
- 1 of the servers on `Administrator@ws2.example`, data folder `\\ws1.example\share\runs\win-w2-real-PerRecordFile-2` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 773 | 731 | 5.6 | 38 | 0 | 0 | 21.7 | 75.6 | 217.0 |

Writes by outcome: conflict 38, http500 4, saved 731. Saves by action: Create 159, Update 166, Submit 159, Approve 116, Reject 43, Comment 88. Reads: 616.

Stale reads: 0 (older than a version already acknowledged); 137 reads followed a save through another server.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 159/159 | 0 | 0 | 0 | 0 | 0 |
| server ws1.example:41291 | pass | 159/159 | 0 | 0 | 0 | 0 | 0 |
| server ws2.example:36021 | pass | 159/159 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 2 | 731 | 5.6 | 38 | 0 | 0 | 0 | 0 | 75.6 | yes |
