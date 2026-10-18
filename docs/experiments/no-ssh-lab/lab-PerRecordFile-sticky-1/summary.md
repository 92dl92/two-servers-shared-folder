# Stress run: PerRecordFile, 2 server(s): PASSED

- Commit `5d54eb86e993b9c29b4e326d9494a1811ac767b9 (from the build)`, 2026-10-18 22:19:15 UTC, 3.8 s
- Machine WS1: Microsoft Windows 10.0.26100, 2 CPUs, .NET 10.0.12
- Settings: 2 servers × 5 clients, 6 records, 40 changes per client, seed 1
- Clients by role: 4 Registrant, 4 Reviewer, 2 Admin
- Data folder `C:\share\6c\lab-PerRecordFile-sticky-1\data`; run folder `C:\Users\Administrator\claudetest-6c\runs\lab-PerRecordFile-sticky-1`
- Servers started by hand: `http://ws1.example:8080/`, `http://ws2.example:8080/`
- Routing sticky. Requests by server: ws2.example:8080 403, ws1.example:8080 403

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 406 | 187 | 48.8 | 131 | 88 | 0 | 38.8 | 101.4 | 212.6 |

Read p50/p95/max ms: 12.4 / 58.4 / 183.3.

Writes by outcome: conflict 131, saved 187, wrongStatus 88. Saves by action: Create 6, Update 49, Submit 6, Approve 3, Reject 3, Comment 120. Reads: 400.

Longest stall: 0.2 s with no save acknowledged, from 0.0 s into the run.

Stale reads: 0 (older than a version already acknowledged); 129 reads followed a save through another server.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server ws1.example:8080 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |
| server ws2.example:8080 | pass | 6/6 | 0 | 0 | 0 | 0 | 0 |

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 2 | 187 | 48.8 | 131 | 88 | 0 | 0 | 0 | 101.4 | yes |
