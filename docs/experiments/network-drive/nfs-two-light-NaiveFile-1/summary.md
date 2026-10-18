# Stress run: NaiveFile, 3 server(s): FAILED

- Commit `1b49659b6d0d95e1a10858057ef4082c27040edb`, 2026-10-17 22:18:19 UTC, 1.6 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 5 clients, 6 records, 40 changes per client, seed 1
- Clients by role: 6 Registrant, 6 Reviewer, 3 Admin
- Data folder `/mnt/share/runs/nfs-two-light-NaiveFile-1/data`; run folder `stress-runs/nfs/nfs-two-light-NaiveFile-1`
- 1 of the servers on `user@mac.example`, data folder `/Volumes/share/runs/nfs-two-light-NaiveFile-1/data` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 57 | 33 | 20.2 | 6 | 1 | 0 | 18.6 | 289.7 | 290.2 |

Writes by outcome: conflict 6, http500 17, saved 33, wrongStatus 1. Saves by action: Create 6, Update 12, Submit 2, Comment 13. Reads: 600 (http500 345, notFound 204).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 39443 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 36709 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server mac-host.local:38359 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |

storage:
- unreadable: JsonException: '}' is invalid after a single JSON value. Expected end of data. Path: $[2] | LineNumber: 98 | BytePositionInLine: 1.

server 39443:
- unreadable: listing records answered 500

server 36709:
- unreadable: listing records answered 500

server mac-host.local:38359:
- unreadable: listing records answered 500

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 3 | 33 | 20.2 | 6 | 1 | 0 | 4 | 0 | 289.7 | no |
