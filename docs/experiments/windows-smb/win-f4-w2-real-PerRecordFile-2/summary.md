# Stress run: PerRecordFile, 2 server(s): PASSED

- Commit `22febeb1b9f435203813da53e455263d96f64aea`, 2026-10-18 16:50:39 UTC, 191.7 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 5 clients, pipeline with 1000 ms think time, 120 changes per client, seed 2
- Clients by role: 4 Registrant, 4 Reviewer, 2 Admin
- Data folder `stress-runs/win-f4-w2-real-PerRecordFile-2/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-f4-w2-real-PerRecordFile-2`
- 1 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-f4-w2-real-PerRecordFile-2` there
- 1 of the servers on `Administrator@ws2.example`, data folder `\\ws1.example\share\runs\win-f4-w2-real-PerRecordFile-2` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 741 | 715 | 3.7 | 23 | 0 | 1 | 16.2 | 68.1 | 183.3 |

Writes by outcome: conflict 23, http500 2, noResponse 1, saved 715. Saves by action: Create 158, Update 164, Submit 157, Approve 112, Reject 45, Comment 79. Reads: 589 (http500 6, noResponse 1).

Longest stall: 1.3 s with no save acknowledged, from 178.4 s into the run (think time 1000 ms, so pauses up to about 1.5 s are normal).

Stale reads: 0 (older than a version already acknowledged); 109 reads followed a save through another server.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | pass | 158/158 | 0 | 0 | 0 | 0 | 0 |
| server ws1.example:33747 | pass | 158/158 | 0 | 0 | 0 | 0 | 0 |
| server ws2.example:35129 | pass | 158/158 | 0 | 0 | 0 | 0 | 0 |

Faults:
- run `webapp/scripts/vm-link.sh ws2 60` at 40 s: at 40.0 s, exit 0: ws2 vnet3 down for 60 s ws2 vnet3 up again: up

Lock check: pass: 316 of 316 record and server pairs could still be changed, 0 stuck, 0 unreadable.

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 2 | 715 | 3.7 | 23 | 0 | 0 | 0 | 0 | 68.1 | yes |
