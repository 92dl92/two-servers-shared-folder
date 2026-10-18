# Stress run: PerRecordFile, 2 server(s): PASSED

- Commit `22febeb1b9f435203813da53e455263d96f64aea`, 2026-10-18 16:21:04 UTC, 126.0 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 5 clients, pipeline with 1000 ms think time, 120 changes per client, seed 1
- Clients by role: 4 Registrant, 4 Reviewer, 2 Admin
- Data folder `stress-runs/win-f1-w2-real-PerRecordFile-1/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-f1-w2-real-PerRecordFile-1`
- 1 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-f1-w2-real-PerRecordFile-1` there
- 1 of the servers on `Administrator@ws2.example`, data folder `\\ws1.example\share\runs\win-f1-w2-real-PerRecordFile-1` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 665 | 496 | 3.9 | 8 | 0 | 2 | 9.2 | 51.2 | 297.5 |

Writes by outcome: conflict 8, noResponse 2, refused 159, saved 496. Saves by action: Create 110, Update 99, Submit 109, Approve 81, Reject 28, Comment 69. Reads: 478 (refused 81).

Longest stall: 1.3 s with no save acknowledged, from 122.9 s into the run (think time 1000 ms, so pauses up to about 1.5 s are normal).

Stale reads: 0 (older than a version already acknowledged); 54 reads followed a save through another server.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 110/110 | 0 | 0 | 0 | 0 | 0 |
| server ws1.example:46207 | pass | 110/110 | 0 | 0 | 0 | 0 | 0 |
| server ws2.example:35493 | skipped: killed by --fault | 0/0 | 0 | 0 | 0 | 0 | 0 |

Faults:
- kill server 2 at 40 s: at 40.0 s, server ws2.example:35493 killed

Lock check: pass: 110 of 110 record and server pairs could still be changed, 0 stuck, 0 unreadable.

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 2 | 496 | 3.9 | 8 | 0 | 0 | 0 | 0 | 51.2 | yes |
