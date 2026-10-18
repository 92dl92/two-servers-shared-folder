# Stress run: NaiveFile, 3 server(s): FAILED

- Commit `1b49659b6d0d95e1a10858057ef4082c27040edb`, 2026-10-17 22:20:28 UTC, 7.7 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 12 records, 200 changes per client, seed 1
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `/mnt/share/runs/nfs-two-heavy-NaiveFile-1/data`; run folder `stress-runs/nfs/nfs-two-heavy-NaiveFile-1`
- 1 of the servers on `user@mac.example`, data folder `/Volumes/share/runs/nfs-two-heavy-NaiveFile-1/data` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 54 | 35 | 4.5 | 8 | 0 | 0 | 31.5 | 125.5 | 153.9 |

Writes by outcome: conflict 8, http500 11, saved 35. Saves by action: Create 11, Update 4, Submit 1, Comment 19. Reads: 4600 (http500 3915, notFound 643).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 46249 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 42027 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server mac-host.local:44399 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |

storage:
- unreadable: JsonException: '{' is invalid after a single JSON value. Expected end of data. Path: $[1] | LineNumber: 53 | BytePositionInLine: 2.

server 46249:
- unreadable: listing records answered 500

server 42027:
- unreadable: listing records answered 500

server mac-host.local:44399:
- unreadable: listing records answered 500

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 3 | 35 | 4.5 | 8 | 0 | 0 | 4 | 0 | 125.5 | no |
