# Stress run: LockedFile, 3 server(s): FAILED

- Commit `1b49659b6d0d95e1a10858057ef4082c27040edb`, 2026-10-17 22:18:48 UTC, 4.0 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 5 clients, 6 records, 40 changes per client, seed 1
- Clients by role: 6 Registrant, 6 Reviewer, 3 Admin
- Data folder `/mnt/share/runs/nfs-two-light-LockedFile-1/data`; run folder `stress-runs/nfs/nfs-two-light-LockedFile-1`
- 1 of the servers on `user@mac.example`, data folder `/Volumes/share/runs/nfs-two-light-LockedFile-1/data` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 55 | 23 | 5.8 | 19 | 4 | 0 | 121.3 | 341.2 | 392.3 |

Writes by outcome: conflict 19, http500 9, saved 23, wrongStatus 4. Saves by action: Create 5, Update 5, Submit 2, Reject 1, Comment 10. Reads: 560 (http500 494, notFound 17).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 44697 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 36147 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server mac-host.local:39287 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |

storage:
- unreadable: JsonException: ':' is invalid after a single JSON value. Expected end of data. Path: $[5] | LineNumber: 219 | BytePositionInLine: 1.

server 44697:
- unreadable: listing records answered 500

server 36147:
- unreadable: listing records answered 500

server mac-host.local:39287:
- unreadable: listing records answered 500

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| LockedFile | 3 | 23 | 5.8 | 19 | 4 | 0 | 4 | 0 | 341.2 | no |
