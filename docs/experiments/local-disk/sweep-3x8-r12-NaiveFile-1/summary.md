# Stress run: NaiveFile, 3 server(s): FAILED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:40:52 UTC, 1.5 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 12 records, 200 changes per client, seed 1
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r12-NaiveFile-1/data`; run folder `stress-runs/sweep/sweep-3x8-r12-NaiveFile-1`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 282 | 110 | 72.0 | 110 | 47 | 0 | 4.9 | 111.0 | 111.9 |

Writes by outcome: conflict 110, http500 15, saved 110, wrongStatus 47. Saves by action: Create 12, Update 7, Submit 1, Reject 2, Comment 88. Reads: 4800 (http500 308, notFound 4222).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 43619 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 37877 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 35871 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |

storage:
- unreadable: JsonException: ']' is invalid after a single JSON value. Expected end of data. Path: $[1] | LineNumber: 579 | BytePositionInLine: 0.

server 43619:
- unreadable: listing records answered 500

server 37877:
- unreadable: listing records answered 500

server 35871:
- unreadable: listing records answered 500

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 3 | 110 | 72.0 | 110 | 47 | 0 | 4 | 0 | 111.0 | no |
