# Stress run: LockedFile, 2 server(s): PASSED

- Commit `fe53fea387646cb0ee65f1cccd5a309abb2291d2`, 2026-10-18 13:02:03 UTC, 134.0 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 5 clients, pipeline with 1000 ms think time, 120 changes per client, seed 2
- Clients by role: 4 Registrant, 4 Reviewer, 2 Admin
- Data folder `stress-runs/win-w2-real-LockedFile-2/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-w2-real-LockedFile-2`
- 1 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-w2-real-LockedFile-2` there
- 1 of the servers on `Administrator@ws2.example`, data folder `\\ws1.example\share\runs\win-w2-real-LockedFile-2` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 740 | 729 | 5.4 | 11 | 0 | 0 | 30.9 | 124.7 | 210.3 |

Writes by outcome: conflict 11, saved 729. Saves by action: Create 159, Update 166, Submit 159, Approve 115, Reject 44, Comment 86. Reads: 615.

Stale reads: 0 (older than a version already acknowledged); 157 reads followed a save through another server.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 159/159 | 0 | 0 | 0 | 0 | 0 |
| server ws1.example:44489 | pass | 159/159 | 0 | 0 | 0 | 0 | 0 |
| server ws2.example:34775 | pass | 159/159 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| LockedFile | 2 | 729 | 5.4 | 11 | 0 | 0 | 0 | 0 | 124.7 | yes |
