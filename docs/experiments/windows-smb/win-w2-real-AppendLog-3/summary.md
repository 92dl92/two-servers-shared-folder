# Stress run: AppendLog, 2 server(s): PASSED

- Commit `fe53fea387646cb0ee65f1cccd5a309abb2291d2`, 2026-10-18 13:21:04 UTC, 127.6 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 5 clients, pipeline with 1000 ms think time, 120 changes per client, seed 3
- Clients by role: 4 Registrant, 4 Reviewer, 2 Admin
- Data folder `stress-runs/win-w2-real-AppendLog-3/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-w2-real-AppendLog-3`
- 1 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-w2-real-AppendLog-3` there
- 1 of the servers on `Administrator@ws2.example`, data folder `\\ws1.example\share\runs\win-w2-real-AppendLog-3` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 748 | 732 | 5.7 | 16 | 0 | 0 | 22.7 | 69.4 | 169.6 |

Writes by outcome: conflict 16, saved 732. Saves by action: Create 168, Update 149, Submit 168, Approve 132, Reject 36, Comment 79. Reads: 594.

Stale reads: 0 (older than a version already acknowledged); 131 reads followed a save through another server.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 168/168 | 0 | 0 | 0 | 0 | 0 |
| server ws1.example:38663 | pass | 168/168 | 0 | 0 | 0 | 0 | 0 |
| server ws2.example:34643 | pass | 168/168 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| AppendLog | 2 | 732 | 5.7 | 16 | 0 | 0 | 0 | 0 | 69.4 | yes |
