# Stress run: NaiveFile, 3 server(s): FAILED

- Commit `1b49659b6d0d95e1a10858057ef4082c27040edb`, 2026-10-17 21:56:35 UTC, 0.8 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 5 clients, 6 records, 40 changes per client, seed 2
- Clients by role: 6 Registrant, 6 Reviewer, 3 Admin
- Data folder `/mnt/share/runs/nfs-one-light-NaiveFile-2/data`; run folder `stress-runs/nfs/nfs-one-light-NaiveFile-2`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 15 | 13 | 17.3 | 1 | 0 | 0 | 27.3 | 120.6 | 120.6 |

Writes by outcome: conflict 1, http500 1, saved 13. Saves by action: Create 6, Update 2, Submit 1, Comment 4. Reads: 600 (http500 507, notFound 84).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 45483 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 43823 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 41247 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |

storage:
- unreadable: JsonException: '}' is invalid after a single JSON value. Expected end of data. Path: $[1] | LineNumber: 39 | BytePositionInLine: 3.

server 45483:
- unreadable: listing records answered 500

server 43823:
- unreadable: listing records answered 500

server 41247:
- unreadable: listing records answered 500

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 3 | 13 | 17.3 | 1 | 0 | 0 | 4 | 0 | 120.6 | no |
