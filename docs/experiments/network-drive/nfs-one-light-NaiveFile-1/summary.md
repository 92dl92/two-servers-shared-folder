# Stress run: NaiveFile, 3 server(s): FAILED

- Commit `1b49659b6d0d95e1a10858057ef4082c27040edb`, 2026-10-17 21:56:32 UTC, 0.7 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 5 clients, 6 records, 40 changes per client, seed 1
- Clients by role: 6 Registrant, 6 Reviewer, 3 Admin
- Data folder `/mnt/share/runs/nfs-one-light-NaiveFile-1/data`; run folder `stress-runs/nfs/nfs-one-light-NaiveFile-1`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 54 | 31 | 42.1 | 13 | 0 | 0 | 18.8 | 116.2 | 116.4 |

Writes by outcome: conflict 13, http500 10, saved 31. Saves by action: Create 6, Update 14, Submit 1, Comment 10. Reads: 600 (http500 140, notFound 412).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 39433 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 39463 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 41243 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |

storage:
- unreadable: JsonException: ']' is invalid after a single JSON value. Expected end of data. Path: $[1] | LineNumber: 130 | BytePositionInLine: 3.

server 39433:
- unreadable: listing records answered 500

server 39463:
- unreadable: listing records answered 500

server 41243:
- unreadable: listing records answered 500

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 3 | 31 | 42.1 | 13 | 0 | 0 | 4 | 0 | 116.2 | no |
