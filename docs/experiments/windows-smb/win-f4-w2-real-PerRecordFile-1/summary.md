# Stress run: PerRecordFile, 2 server(s): PASSED

- Commit `22febeb1b9f435203813da53e455263d96f64aea`, 2026-10-18 16:46:48 UTC, 189.1 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 5 clients, pipeline with 1000 ms think time, 120 changes per client, seed 1
- Clients by role: 4 Registrant, 4 Reviewer, 2 Admin
- Data folder `stress-runs/win-f4-w2-real-PerRecordFile-1/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-f4-w2-real-PerRecordFile-1`
- 1 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-f4-w2-real-PerRecordFile-1` there
- 1 of the servers on `Administrator@ws2.example`, data folder `\\ws1.example\share\runs\win-f4-w2-real-PerRecordFile-1` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 754 | 722 | 3.8 | 27 | 0 | 1 | 14.3 | 64.0 | 167.2 |

Writes by outcome: conflict 27, http500 4, noResponse 1, saved 722. Saves by action: Create 163, Update 151, Submit 162, Approve 123, Reject 39, Comment 84. Reads: 594 (http500 5, noResponse 3).

Longest stall: 1.2 s with no save acknowledged, from 88.6 s into the run (think time 1000 ms, so pauses up to about 1.5 s are normal).

Stale reads: 0 (older than a version already acknowledged); 128 reads followed a save through another server.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 163/163 | 0 | 0 | 0 | 0 | 0 |
| server ws1.example:32993 | pass | 163/163 | 0 | 0 | 0 | 0 | 0 |
| server ws2.example:41053 | pass | 163/163 | 0 | 0 | 0 | 0 | 0 |

Faults:
- run `webapp/scripts/vm-link.sh ws2 60` at 40 s: at 40.0 s, exit 0: ws2 vnet3 down for 60 s ws2 vnet3 up again: up

Lock check: pass: 326 of 326 record and server pairs could still be changed, 0 stuck, 0 unreadable.

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 2 | 722 | 3.8 | 27 | 0 | 0 | 0 | 0 | 64.0 | yes |
