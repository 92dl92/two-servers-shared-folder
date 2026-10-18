# Stress run: NaiveFile, 3 server(s): FAILED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:33:35 UTC, 2.9 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 1 records, 200 changes per client, seed 3
- Clients by role: 1 Registrant, 20 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r01-NaiveFile-3/data`; run folder `stress-runs/sweep/sweep-3x8-r01-NaiveFile-3`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 31 | 11 | 3.9 | 0 | 0 | 0 | 10.2 | 88.5 | 88.5 |

Writes by outcome: http500 20, saved 11. Saves by action: Create 1, Submit 3, Comment 7. Reads: 4800 (http500 4770).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 43283 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 37363 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 40311 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |

storage:
- unreadable: JsonException: '}' is invalid after a single JSON value. Expected end of data. Path: $[1] | LineNumber: 46 | BytePositionInLine: 4.

server 43283:
- unreadable: listing records answered 500

server 37363:
- unreadable: listing records answered 500

server 40311:
- unreadable: listing records answered 500

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 3 | 11 | 3.9 | 0 | 0 | 0 | 4 | 0 | 88.5 | no |
