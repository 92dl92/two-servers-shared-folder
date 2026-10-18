# Stress run: NaiveFile, 3 server(s): FAILED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:44:08 UTC, 2.2 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 24 records, 200 changes per client, seed 3
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r24-NaiveFile-3/data`; run folder `stress-runs/sweep/sweep-3x8-r24-NaiveFile-3`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 24 | 12 | 5.4 | 0 | 0 | 0 | 115.6 | 116.2 | 116.2 |

Writes by outcome: http500 12, saved 12. Saves by action: Create 12. Reads: 4800 (http500 4800).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 35589 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 42111 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 33349 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |

storage:
- unreadable: JsonException: ']' is invalid after a single JSON value. Expected end of data. Path: $[1] | LineNumber: 33 | BytePositionInLine: 0.

server 35589:
- unreadable: listing records answered 500

server 42111:
- unreadable: listing records answered 500

server 33349:
- unreadable: listing records answered 500

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 3 | 12 | 5.4 | 0 | 0 | 0 | 4 | 0 | 116.2 | no |
