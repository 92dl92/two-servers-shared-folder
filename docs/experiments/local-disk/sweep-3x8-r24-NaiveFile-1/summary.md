# Stress run: NaiveFile, 3 server(s): FAILED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:44:00 UTC, 2.4 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 24 records, 200 changes per client, seed 1
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r24-NaiveFile-1/data`; run folder `stress-runs/sweep/sweep-3x8-r24-NaiveFile-1`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 51 | 38 | 15.6 | 7 | 1 | 0 | 8.5 | 107.1 | 107.9 |

Writes by outcome: conflict 7, http500 5, saved 38, wrongStatus 1. Saves by action: Create 24, Update 4, Submit 1, Comment 9. Reads: 4800 (http500 4249, notFound 524).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 45181 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 42763 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 33869 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |

storage:
- unreadable: JsonException: ']' is invalid after a single JSON value. Expected end of data. Path: $[2] | LineNumber: 120 | BytePositionInLine: 4.

server 45181:
- unreadable: listing records answered 500

server 42763:
- unreadable: listing records answered 500

server 33869:
- unreadable: listing records answered 500

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 3 | 38 | 15.6 | 7 | 1 | 0 | 4 | 0 | 107.1 | no |
