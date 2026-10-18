# Stress run: NaiveFile, 3 server(s): FAILED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:37:30 UTC, 2.5 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 3 records, 200 changes per client, seed 3
- Clients by role: 3 Registrant, 18 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r03-NaiveFile-3/data`; run folder `stress-runs/sweep/sweep-3x8-r03-NaiveFile-3`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 450 | 156 | 61.5 | 196 | 27 | 0 | 6.6 | 18.3 | 113.4 |

Writes by outcome: conflict 196, http500 71, saved 156, wrongStatus 27. Saves by action: Create 3, Update 9, Submit 5, Approve 1, Reject 5, Comment 133. Reads: 4800 (http500 4094, notFound 259).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 34939 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 44395 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 40315 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |

storage:
- unreadable: JsonException: '"' is invalid after a single JSON value. Expected end of data. Path: $[2] | LineNumber: 511 | BytePositionInLine: 5.

server 34939:
- unreadable: listing records answered 500

server 44395:
- unreadable: listing records answered 500

server 40315:
- unreadable: listing records answered 500

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 3 | 156 | 61.5 | 196 | 27 | 0 | 4 | 0 | 18.3 | no |
