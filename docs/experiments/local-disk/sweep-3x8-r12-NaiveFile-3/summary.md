# Stress run: NaiveFile, 3 server(s): FAILED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:41:00 UTC, 2.4 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 12 records, 200 changes per client, seed 3
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r12-NaiveFile-3/data`; run folder `stress-runs/sweep/sweep-3x8-r12-NaiveFile-3`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 12 | 12 | 4.9 | 0 | 0 | 0 | 109.7 | 111.2 | 111.2 |

Writes by outcome: saved 12. Saves by action: Create 12. Reads: 4800 (http500 4800).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 46233 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 34533 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 37947 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |

storage:
- unreadable: JsonException: ']' is invalid after a single JSON value. Expected end of data. Path: $[1] | LineNumber: 33 | BytePositionInLine: 0.

server 46233:
- unreadable: listing records answered 500

server 34533:
- unreadable: listing records answered 500

server 37947:
- unreadable: listing records answered 500

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 3 | 12 | 4.9 | 0 | 0 | 0 | 4 | 0 | 111.2 | no |
