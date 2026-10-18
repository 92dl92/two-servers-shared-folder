# Stress run: NaiveFile, 3 server(s): FAILED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:51:23 UTC, 2.7 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 96 records, 200 changes per client, seed 3
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r96-NaiveFile-3/data`; run folder `stress-runs/sweep/sweep-3x8-r96-NaiveFile-3`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 147 | 118 | 43.7 | 1 | 2 | 0 | 3.0 | 105.7 | 106.3 |

Writes by outcome: conflict 1, http500 26, saved 118, wrongStatus 2. Saves by action: Create 84, Update 12, Submit 4, Comment 18. Reads: 4800 (http500 4586, notFound 163).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 36829 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 36691 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 33933 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |

storage:
- unreadable: JsonException: '}' is invalid after a single JSON value. Expected end of data. Path: $[23] | LineNumber: 763 | BytePositionInLine: 3.

server 36829:
- unreadable: listing records answered 500

server 36691:
- unreadable: listing records answered 500

server 33933:
- unreadable: listing records answered 500

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 3 | 118 | 43.7 | 1 | 2 | 0 | 4 | 0 | 105.7 | no |
