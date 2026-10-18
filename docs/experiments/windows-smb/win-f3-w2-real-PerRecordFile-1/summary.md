# Stress run: PerRecordFile, 2 server(s): PASSED

- Commit `22febeb1b9f435203813da53e455263d96f64aea`, 2026-10-18 16:37:27 UTC, 144.2 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 5 clients, pipeline with 1000 ms think time, 120 changes per client, seed 1
- Clients by role: 4 Registrant, 4 Reviewer, 2 Admin
- Data folder `stress-runs/win-f3-w2-real-PerRecordFile-1/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-f3-w2-real-PerRecordFile-1`
- 1 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-f3-w2-real-PerRecordFile-1` there
- 1 of the servers on `Administrator@ws2.example`, data folder `\\ws1.example\share\runs\win-f3-w2-real-PerRecordFile-1` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 782 | 749 | 5.2 | 33 | 0 | 0 | 26.8 | 70.0 | 14238.3 |

Writes by outcome: conflict 33, saved 749. Saves by action: Create 164, Update 154, Submit 164, Approve 124, Reject 40, Comment 103. Reads: 622.

Longest stall: 1.3 s with no save acknowledged, from 133.5 s into the run (think time 1000 ms, so pauses up to about 1.5 s are normal).

Stale reads: 0 (older than a version already acknowledged); 148 reads followed a save through another server.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 164/164 | 0 | 0 | 0 | 0 | 0 |
| server ws1.example:39605 | pass | 164/164 | 0 | 0 | 0 | 0 | 0 |
| server ws2.example:34709 | pass | 164/164 | 0 | 0 | 0 | 0 | 0 |

Faults:
- run `webapp/scripts/vm-link.sh ws2 10` at 40 s: at 40.0 s, exit 0: ws2 vnet3 down for 10 s ws2 vnet3 up again: up

Lock check: pass: 328 of 328 record and server pairs could still be changed, 0 stuck, 0 unreadable.

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 2 | 749 | 5.2 | 33 | 0 | 0 | 0 | 0 | 70.0 | yes |
