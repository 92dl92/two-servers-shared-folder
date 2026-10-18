# Stress run: PerRecordFile, 2 server(s): PASSED

- Commit `fe53fea387646cb0ee65f1cccd5a309abb2291d2`, 2026-10-18 13:07:37 UTC, 127.8 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 5 clients, pipeline with 1000 ms think time, 120 changes per client, seed 1
- Clients by role: 4 Registrant, 4 Reviewer, 2 Admin
- Data folder `stress-runs/win-w2-real-PerRecordFile-1/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-w2-real-PerRecordFile-1`
- 1 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-w2-real-PerRecordFile-1` there
- 1 of the servers on `Administrator@ws2.example`, data folder `\\ws1.example\share\runs\win-w2-real-PerRecordFile-1` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 766 | 728 | 5.7 | 36 | 0 | 0 | 22.1 | 73.0 | 176.7 |

Writes by outcome: conflict 36, http500 2, saved 728. Saves by action: Create 164, Update 154, Submit 164, Approve 116, Reject 48, Comment 82. Reads: 606.

Stale reads: 0 (older than a version already acknowledged); 167 reads followed a save through another server.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 164/164 | 0 | 0 | 0 | 0 | 0 |
| server ws1.example:34615 | pass | 164/164 | 0 | 0 | 0 | 0 | 0 |
| server ws2.example:39495 | pass | 164/164 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 2 | 728 | 5.7 | 36 | 0 | 0 | 0 | 0 | 73.0 | yes |
