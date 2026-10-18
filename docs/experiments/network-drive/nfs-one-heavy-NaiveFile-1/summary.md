# Stress run: NaiveFile, 3 server(s): FAILED

- Commit `1b49659b6d0d95e1a10858057ef4082c27040edb`, 2026-10-17 21:58:42 UTC, 2.8 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 12 records, 200 changes per client, seed 1
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `/mnt/share/runs/nfs-one-heavy-NaiveFile-1/data`; run folder `stress-runs/nfs/nfs-one-heavy-NaiveFile-1`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 16 | 15 | 5.3 | 0 | 0 | 0 | 116.1 | 124.1 | 124.1 |

Writes by outcome: http500 1, saved 15. Saves by action: Create 12, Update 1, Comment 2. Reads: 4800 (http500 4738, notFound 58).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 44817 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 42391 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 38593 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |

storage:
- unreadable: JsonException: '}' is invalid after a single JSON value. Expected end of data. Path: $[1] | LineNumber: 39 | BytePositionInLine: 3.

server 44817:
- unreadable: listing records answered 500

server 42391:
- unreadable: listing records answered 500

server 38593:
- unreadable: listing records answered 500

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 3 | 15 | 5.3 | 0 | 0 | 0 | 4 | 0 | 124.1 | no |
