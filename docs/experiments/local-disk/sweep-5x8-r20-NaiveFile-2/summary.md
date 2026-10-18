# Stress run: NaiveFile, 5 server(s): FAILED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:55:55 UTC, 4.2 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 5 servers × 8 clients, 20 records, 200 changes per client, seed 2
- Clients by role: 20 Registrant, 15 Reviewer, 5 Admin
- Data folder `stress-runs/sweep/sweep-5x8-r20-NaiveFile-2/data`; run folder `stress-runs/sweep/sweep-5x8-r20-NaiveFile-2`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 80 | 42 | 9.9 | 33 | 4 | 0 | 27.4 | 129.7 | 152.0 |

Writes by outcome: conflict 33, http500 1, saved 42, wrongStatus 4. Saves by action: Create 20, Update 5, Submit 1, Reject 1, Comment 15. Reads: 8000 (http500 6291, notFound 1649).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 34793 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 45029 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 46751 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 43697 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 42565 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |

storage:
- unreadable: JsonException: ']' is invalid after a single JSON value. Expected end of data. Path: $[1] | LineNumber: 145 | BytePositionInLine: 0.

server 34793:
- unreadable: listing records answered 500

server 45029:
- unreadable: listing records answered 500

server 46751:
- unreadable: listing records answered 500

server 43697:
- unreadable: listing records answered 500

server 42565:
- unreadable: listing records answered 500

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 5 | 42 | 9.9 | 33 | 4 | 0 | 6 | 0 | 129.7 | no |
