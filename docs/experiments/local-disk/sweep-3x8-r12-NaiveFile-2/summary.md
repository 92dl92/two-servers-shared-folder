# Stress run: NaiveFile, 3 server(s): FAILED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:40:55 UTC, 2.6 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 12 records, 200 changes per client, seed 2
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r12-NaiveFile-2/data`; run folder `stress-runs/sweep/sweep-3x8-r12-NaiveFile-2`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 12 | 12 | 4.7 | 0 | 0 | 0 | 111.8 | 113.6 | 113.6 |

Writes by outcome: saved 12. Saves by action: Create 12. Reads: 4800 (http500 4800).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 41381 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 33543 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 44347 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |

storage:
- unreadable: JsonException: ']' is invalid after a single JSON value. Expected end of data. Path: $[1] | LineNumber: 33 | BytePositionInLine: 0.

server 41381:
- unreadable: listing records answered 500

server 33543:
- unreadable: listing records answered 500

server 44347:
- unreadable: listing records answered 500

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 3 | 12 | 4.7 | 0 | 0 | 0 | 4 | 0 | 113.6 | no |
