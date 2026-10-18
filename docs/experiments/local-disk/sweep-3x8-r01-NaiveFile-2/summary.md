# Stress run: NaiveFile, 3 server(s): FAILED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:33:29 UTC, 3.5 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 1 records, 200 changes per client, seed 2
- Clients by role: 1 Registrant, 20 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r01-NaiveFile-2/data`; run folder `stress-runs/sweep/sweep-3x8-r01-NaiveFile-2`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 274 | 70 | 19.8 | 180 | 4 | 0 | 4.4 | 22.0 | 116.1 |

Writes by outcome: conflict 180, http500 20, saved 70, wrongStatus 4. Saves by action: Create 1, Update 4, Submit 4, Comment 61. Reads: 4800 (http500 4527).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 38963 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 37375 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 44059 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |

storage:
- unreadable: JsonException: ']' is invalid after a single JSON value. Expected end of data. Path: $[1] | LineNumber: 235 | BytePositionInLine: 1.

server 38963:
- unreadable: listing records answered 500

server 37375:
- unreadable: listing records answered 500

server 44059:
- unreadable: listing records answered 500

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 3 | 70 | 19.8 | 180 | 4 | 0 | 4 | 0 | 22.0 | no |
