# Stress run: NaiveFile, 3 server(s): FAILED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:37:20 UTC, 2.8 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 3 records, 200 changes per client, seed 1
- Clients by role: 3 Registrant, 18 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r03-NaiveFile-1/data`; run folder `stress-runs/sweep/sweep-3x8-r03-NaiveFile-1`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 124 | 49 | 17.7 | 46 | 2 | 0 | 7.4 | 106.8 | 126.4 |

Writes by outcome: conflict 46, http500 27, saved 49, wrongStatus 2. Saves by action: Create 3, Update 5, Submit 2, Approve 1, Comment 38. Reads: 4800 (http500 4611, notFound 68).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 40365 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 34783 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 39245 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |

storage:
- unreadable: JsonException: 'n' is invalid after a single JSON value. Expected end of data. Path: $[2] | LineNumber: 196 | BytePositionInLine: 1.

server 40365:
- unreadable: listing records answered 500

server 34783:
- unreadable: listing records answered 500

server 39245:
- unreadable: listing records answered 500

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 3 | 49 | 17.7 | 46 | 2 | 0 | 4 | 0 | 106.8 | no |
