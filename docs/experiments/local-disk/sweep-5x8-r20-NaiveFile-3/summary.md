# Stress run: NaiveFile, 5 server(s): FAILED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:56:02 UTC, 4.7 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 5 servers × 8 clients, 20 records, 200 changes per client, seed 3
- Clients by role: 20 Registrant, 15 Reviewer, 5 Admin
- Data folder `stress-runs/sweep/sweep-5x8-r20-NaiveFile-3/data`; run folder `stress-runs/sweep/sweep-5x8-r20-NaiveFile-3`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 20 | 20 | 4.3 | 0 | 0 | 0 | 125.7 | 141.6 | 142.0 |

Writes by outcome: saved 20. Saves by action: Create 20. Reads: 8000 (http500 8000).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 39291 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 40677 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 33737 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 44259 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 32955 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |

storage:
- unreadable: JsonException: ']' is invalid after a single JSON value. Expected end of data. Path: $[1] | LineNumber: 33 | BytePositionInLine: 0.

server 39291:
- unreadable: listing records answered 500

server 40677:
- unreadable: listing records answered 500

server 33737:
- unreadable: listing records answered 500

server 44259:
- unreadable: listing records answered 500

server 32955:
- unreadable: listing records answered 500

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 5 | 20 | 4.3 | 0 | 0 | 0 | 6 | 0 | 141.6 | no |
