# Stress run: PerRecordFile, 2 server(s): PASSED

- Commit `22febeb1b9f435203813da53e455263d96f64aea`, 2026-10-18 16:43:44 UTC, 143.9 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 5 clients, pipeline with 1000 ms think time, 120 changes per client, seed 3
- Clients by role: 4 Registrant, 4 Reviewer, 2 Admin
- Data folder `stress-runs/win-f3-w2-real-PerRecordFile-3/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-f3-w2-real-PerRecordFile-3`
- 1 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-f3-w2-real-PerRecordFile-3` there
- 1 of the servers on `Administrator@ws2.example`, data folder `\\ws1.example\share\runs\win-f3-w2-real-PerRecordFile-3` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 767 | 729 | 5.1 | 38 | 0 | 0 | 24.8 | 65.1 | 15511.4 |

Writes by outcome: conflict 38, saved 729. Saves by action: Create 168, Update 149, Submit 168, Approve 121, Reject 47, Comment 76. Reads: 602.

Longest stall: 1.3 s with no save acknowledged, from 48.9 s into the run (think time 1000 ms, so pauses up to about 1.5 s are normal).

Stale reads: 0 (older than a version already acknowledged); 145 reads followed a save through another server.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 168/168 | 0 | 0 | 0 | 0 | 0 |
| server ws1.example:39665 | pass | 168/168 | 0 | 0 | 0 | 0 | 0 |
| server ws2.example:38339 | pass | 168/168 | 0 | 0 | 0 | 0 | 0 |

Faults:
- run `webapp/scripts/vm-link.sh ws2 10` at 40 s: at 40.0 s, exit 0: ws2 vnet3 down for 10 s ws2 vnet3 up again: up

Lock check: pass: 336 of 336 record and server pairs could still be changed, 0 stuck, 0 unreadable.

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 2 | 729 | 5.1 | 38 | 0 | 0 | 0 | 0 | 65.1 | yes |
