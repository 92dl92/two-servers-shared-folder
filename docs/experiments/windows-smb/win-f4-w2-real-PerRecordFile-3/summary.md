# Stress run: PerRecordFile, 2 server(s): PASSED

- Commit `22febeb1b9f435203813da53e455263d96f64aea`, 2026-10-18 16:54:31 UTC, 188.9 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 5 clients, pipeline with 1000 ms think time, 120 changes per client, seed 3
- Clients by role: 4 Registrant, 4 Reviewer, 2 Admin
- Data folder `stress-runs/win-f4-w2-real-PerRecordFile-3/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-f4-w2-real-PerRecordFile-3`
- 1 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-f4-w2-real-PerRecordFile-3` there
- 1 of the servers on `Administrator@ws2.example`, data folder `\\ws1.example\share\runs\win-f4-w2-real-PerRecordFile-3` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 776 | 741 | 3.9 | 29 | 0 | 0 | 13.9 | 59.1 | 152.0 |

Writes by outcome: conflict 29, http500 6, saved 741. Saves by action: Create 167, Update 145, Submit 166, Approve 120, Reject 46, Comment 97. Reads: 612 (http500 6).

Longest stall: 1.3 s with no save acknowledged, from 187.6 s into the run (think time 1000 ms, so pauses up to about 1.5 s are normal).

Stale reads: 0 (older than a version already acknowledged); 118 reads followed a save through another server.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 167/167 | 0 | 0 | 0 | 0 | 0 |
| server ws1.example:39151 | pass | 167/167 | 0 | 0 | 0 | 0 | 0 |
| server ws2.example:43825 | pass | 167/167 | 0 | 0 | 0 | 0 | 0 |

Faults:
- run `webapp/scripts/vm-link.sh ws2 60` at 40 s: at 40.0 s, exit 0: ws2 vnet3 down for 60 s ws2 vnet3 up again: up

Lock check: pass: 334 of 334 record and server pairs could still be changed, 0 stuck, 0 unreadable.

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 2 | 741 | 3.9 | 29 | 0 | 0 | 0 | 0 | 59.1 | yes |
