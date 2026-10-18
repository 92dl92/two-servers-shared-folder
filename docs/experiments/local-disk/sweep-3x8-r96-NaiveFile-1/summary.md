# Stress run: NaiveFile, 3 server(s): FAILED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:51:15 UTC, 2.4 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 96 records, 200 changes per client, seed 1
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r96-NaiveFile-1/data`; run folder `stress-runs/sweep/sweep-3x8-r96-NaiveFile-1`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 96 | 42 | 17.3 | 0 | 0 | 0 | 9.2 | 109.4 | 110.2 |

Writes by outcome: http500 54, saved 42. Saves by action: Create 42. Reads: 4800 (http500 4800).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 42003 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 45453 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 44251 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |

storage:
- unreadable: JsonException: '}' is invalid after a single JSON value. Expected end of data. Path: $[7] | LineNumber: 218 | BytePositionInLine: 1.

server 42003:
- unreadable: listing records answered 500

server 45453:
- unreadable: listing records answered 500

server 44251:
- unreadable: listing records answered 500

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 3 | 42 | 17.3 | 0 | 0 | 0 | 4 | 0 | 109.4 | no |
