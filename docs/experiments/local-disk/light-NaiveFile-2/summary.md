# Stress run: NaiveFile, 3 server(s): FAILED

- Commit `ee6af83bef739621009cd4be91192b48f8df58df`, 2026-10-17 19:07:30 UTC, 0.5 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 5 clients, 6 records, 40 changes per client, seed 2
- Data folder `stress-runs/light-NaiveFile-2/data`; run folder `stress-runs/light-NaiveFile-2`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 73 | 37 | 78.7 | 19 | 12 | 0 | 3.5 | 109.1 | 109.9 |

Writes by outcome: conflict 19, http500 5, saved 37, wrongStatus 12. Saves by action: Create 6, Update 2, Submit 1, Reject 1, Comment 27. Reads: 600 (http500 62, notFound 471).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 44259 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 37311 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 42279 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |

storage:
- unreadable: JsonException: ']' is invalid after a single JSON value. Expected end of data. Path: $[1] | LineNumber: 165 | BytePositionInLine: 3.

server 44259:
- unreadable: listing records answered 500

server 37311:
- unreadable: listing records answered 500

server 42279:
- unreadable: listing records answered 500

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 3 | 37 | 78.7 | 19 | 12 | 0 | 4 | 0 | 109.1 | no |
