# Stress run: LockedFile, 3 server(s): FAILED

- Commit `1b49659b6d0d95e1a10858057ef4082c27040edb`, 2026-10-17 22:21:29 UTC, 23.8 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 12 records, 200 changes per client, seed 2
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `/mnt/share/runs/nfs-two-heavy-LockedFile-2/data`; run folder `stress-runs/nfs/nfs-two-heavy-LockedFile-2`
- 1 of the servers on `user@mac.example`, data folder `/Volumes/share/runs/nfs-two-heavy-LockedFile-2/data` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 159 | 70 | 2.9 | 55 | 12 | 0 | 225.6 | 568.5 | 1269.2 |

Writes by outcome: conflict 55, http500 22, saved 70, wrongStatus 12. Saves by action: Create 10, Update 19, Submit 7, Approve 3, Comment 31. Reads: 4400 (http500 4208, notFound 45).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 38535 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 39043 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server mac-host.local:46275 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |

storage:
- unreadable: JsonException: ']' is invalid after a single JSON value. Expected end of data. Path: $[9] | LineNumber: 483 | BytePositionInLine: 1.

server 38535:
- unreadable: listing records answered 500

server 39043:
- unreadable: listing records answered 500

server mac-host.local:46275:
- unreadable: listing records answered 500

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| LockedFile | 3 | 70 | 2.9 | 55 | 12 | 0 | 4 | 0 | 568.5 | no |
