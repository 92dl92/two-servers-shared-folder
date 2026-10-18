# Stress run: NaiveFile, 3 server(s): FAILED

- Commit `1b49659b6d0d95e1a10858057ef4082c27040edb`, 2026-10-17 21:58:47 UTC, 2.9 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 12 records, 200 changes per client, seed 2
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `/mnt/share/runs/nfs-one-heavy-NaiveFile-2/data`; run folder `stress-runs/nfs/nfs-one-heavy-NaiveFile-2`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 41 | 25 | 8.7 | 12 | 0 | 0 | 34.1 | 113.4 | 114.2 |

Writes by outcome: conflict 12, http500 4, saved 25. Saves by action: Create 12, Update 4, Submit 1, Comment 8. Reads: 4800 (http500 4299, notFound 472).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 42013 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 39475 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 40299 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |

storage:
- unreadable: JsonException: '}' is invalid after a single JSON value. Expected end of data. Path: $[1] | LineNumber: 81 | BytePositionInLine: 1.

server 42013:
- unreadable: listing records answered 500

server 39475:
- unreadable: listing records answered 500

server 40299:
- unreadable: listing records answered 500

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 3 | 25 | 8.7 | 12 | 0 | 0 | 4 | 0 | 113.4 | no |
