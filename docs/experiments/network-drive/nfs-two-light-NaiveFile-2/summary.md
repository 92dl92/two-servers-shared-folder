# Stress run: NaiveFile, 3 server(s): FAILED

- Commit `1b49659b6d0d95e1a10858057ef4082c27040edb`, 2026-10-17 22:18:25 UTC, 1.8 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 5 clients, 6 records, 40 changes per client, seed 2
- Clients by role: 6 Registrant, 6 Reviewer, 3 Admin
- Data folder `/mnt/share/runs/nfs-two-light-NaiveFile-2/data`; run folder `stress-runs/nfs/nfs-two-light-NaiveFile-2`
- 1 of the servers on `user@mac.example`, data folder `/Volumes/share/runs/nfs-two-light-NaiveFile-2/data` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 60 | 39 | 21.7 | 7 | 1 | 0 | 24.3 | 147.2 | 163.0 |

Writes by outcome: conflict 7, http500 13, saved 39, wrongStatus 1. Saves by action: Create 6, Update 6, Submit 5, Approve 1, Comment 21. Reads: 600 (http500 209, notFound 337).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 36915 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 33227 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server mac-host.local:34083 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |

storage:
- unreadable: JsonException: '}' is invalid after a single JSON value. Expected end of data. Path: $[1] | LineNumber: 95 | BytePositionInLine: 2.

server 36915:
- unreadable: listing records answered 500

server 33227:
- unreadable: listing records answered 500

server mac-host.local:34083:
- unreadable: listing records answered 500

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 3 | 39 | 21.7 | 7 | 1 | 0 | 4 | 0 | 147.2 | no |
