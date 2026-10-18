# Stress run: NaiveFile, 3 server(s): FAILED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:37:25 UTC, 2.7 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 3 records, 200 changes per client, seed 2
- Clients by role: 3 Registrant, 18 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r03-NaiveFile-2/data`; run folder `stress-runs/sweep/sweep-3x8-r03-NaiveFile-2`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 39 | 17 | 6.3 | 17 | 0 | 0 | 15.5 | 105.1 | 105.1 |

Writes by outcome: conflict 17, http500 5, saved 17. Saves by action: Create 3, Update 2, Submit 1, Comment 11. Reads: 4800 (http500 4669, notFound 95).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 46041 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 36061 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 43587 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |

storage:
- unreadable: JsonException: ']' is invalid after a single JSON value. Expected end of data. Path: $[1] | LineNumber: 53 | BytePositionInLine: 4.

server 46041:
- unreadable: listing records answered 500

server 36061:
- unreadable: listing records answered 500

server 43587:
- unreadable: listing records answered 500

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 3 | 17 | 6.3 | 17 | 0 | 0 | 4 | 0 | 105.1 | no |
