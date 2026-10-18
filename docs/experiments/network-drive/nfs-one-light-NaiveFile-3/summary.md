# Stress run: NaiveFile, 3 server(s): FAILED

- Commit `1b49659b6d0d95e1a10858057ef4082c27040edb`, 2026-10-17 21:56:38 UTC, 0.7 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 5 clients, 6 records, 40 changes per client, seed 3
- Clients by role: 6 Registrant, 6 Reviewer, 3 Admin
- Data folder `/mnt/share/runs/nfs-one-light-NaiveFile-3/data`; run folder `stress-runs/nfs/nfs-one-light-NaiveFile-3`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 51 | 27 | 39.1 | 12 | 1 | 0 | 22.0 | 111.2 | 111.5 |

Writes by outcome: conflict 12, http500 11, saved 27, wrongStatus 1. Saves by action: Create 6, Update 6, Submit 2, Comment 13. Reads: 600 (http500 244, notFound 311).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 35769 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 39033 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 44947 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |

storage:
- unreadable: JsonException: ']' is invalid after a single JSON value. Expected end of data. Path: $[1] | LineNumber: 102 | BytePositionInLine: 5.

server 35769:
- unreadable: listing records answered 500

server 39033:
- unreadable: listing records answered 500

server 44947:
- unreadable: listing records answered 500

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 3 | 27 | 39.1 | 12 | 1 | 0 | 4 | 0 | 111.2 | no |
