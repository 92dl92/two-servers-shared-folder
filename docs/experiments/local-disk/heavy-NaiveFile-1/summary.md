# Stress run: NaiveFile, 3 server(s): FAILED

- Commit `ee6af83bef739621009cd4be91192b48f8df58df`, 2026-10-17 19:08:32 UTC, 2.8 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 12 records, 200 changes per client, seed 1
- Data folder `stress-runs/heavy-NaiveFile-1/data`; run folder `stress-runs/heavy-NaiveFile-1`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 31 | 23 | 8.1 | 4 | 1 | 0 | 115.7 | 116.3 | 116.7 |

Writes by outcome: conflict 4, http500 3, saved 23, wrongStatus 1. Saves by action: Create 12, Update 5, Comment 6. Reads: 4800 (http500 4316, notFound 465).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 46771 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 35869 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 36677 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |

storage:
- unreadable: JsonException: ']' is invalid after a single JSON value. Expected end of data. Path: $[1] | LineNumber: 67 | BytePositionInLine: 5.

server 46771:
- unreadable: listing records answered 500

server 35869:
- unreadable: listing records answered 500

server 36677:
- unreadable: listing records answered 500

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 3 | 23 | 8.1 | 4 | 1 | 0 | 4 | 0 | 116.3 | no |
