# Stress run: LockedFile, 3 server(s): FAILED

- Commit `1b49659b6d0d95e1a10858057ef4082c27040edb`, 2026-10-17 22:19:09 UTC, 5.5 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 5 clients, 6 records, 40 changes per client, seed 3
- Clients by role: 6 Registrant, 6 Reviewer, 3 Admin
- Data folder `/mnt/share/runs/nfs-two-light-LockedFile-3/data`; run folder `stress-runs/nfs/nfs-two-light-LockedFile-3`
- 1 of the servers on `user@mac.example`, data folder `/Volumes/share/runs/nfs-two-light-LockedFile-3/data` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 160 | 68 | 12.3 | 58 | 18 | 0 | 114.9 | 356.9 | 801.7 |

Writes by outcome: conflict 58, http500 16, saved 68, wrongStatus 18. Saves by action: Create 6, Update 24, Submit 4, Approve 1, Reject 1, Comment 32. Reads: 600 (http500 419, notFound 27).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 43327 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 37913 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server mac-host.local:35917 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |

storage:
- unreadable: JsonException: 'n' is invalid after a single JSON value. Expected end of data. Path: $[5] | LineNumber: 485 | BytePositionInLine: 2.

server 43327:
- unreadable: listing records answered 500

server 37913:
- unreadable: listing records answered 500

server mac-host.local:35917:
- unreadable: listing records answered 500

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| LockedFile | 3 | 68 | 12.3 | 58 | 18 | 0 | 4 | 0 | 356.9 | no |
