# Stress run: NaiveFile, 3 server(s): FAILED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:33:22 UTC, 4.6 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 1 records, 200 changes per client, seed 1
- Clients by role: 1 Registrant, 20 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r01-NaiveFile-1/data`; run folder `stress-runs/sweep/sweep-3x8-r01-NaiveFile-1`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 1326 | 270 | 58.4 | 904 | 27 | 0 | 11.7 | 32.5 | 142.9 |

Writes by outcome: conflict 904, http500 125, saved 270, wrongStatus 27. Saves by action: Create 1, Update 6, Submit 1, Approve 3, Reject 2, Comment 257. Reads: 4800 (http500 3475).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 46129 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 38391 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 37397 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |

storage:
- unreadable: JsonException: ']' is invalid after a single JSON value. Expected end of data. Path: $[1] | LineNumber: 900 | BytePositionInLine: 3.

server 46129:
- unreadable: listing records answered 500

server 38391:
- unreadable: listing records answered 500

server 37397:
- unreadable: listing records answered 500

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 3 | 270 | 58.4 | 904 | 27 | 0 | 4 | 0 | 32.5 | no |
