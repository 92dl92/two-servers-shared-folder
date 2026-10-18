# Stress run: NaiveFile, 3 server(s): FAILED

- Commit `1b49659b6d0d95e1a10858057ef4082c27040edb`, 2026-10-17 22:20:49 UTC, 4.6 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 12 records, 200 changes per client, seed 3
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `/mnt/share/runs/nfs-two-heavy-NaiveFile-3/data`; run folder `stress-runs/nfs/nfs-two-heavy-NaiveFile-3`
- 1 of the servers on `user@mac.example`, data folder `/Volumes/share/runs/nfs-two-heavy-NaiveFile-3/data` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 12 | 8 | 1.8 | 0 | 0 | 0 | 125.3 | 128.4 | 128.4 |

Writes by outcome: http500 4, saved 8. Saves by action: Create 8. Reads: 4000 (http500 4000).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 40127 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 39729 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server mac-host.local:38787 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |

storage:
- unreadable: JsonException: ']' is invalid after a single JSON value. Expected end of data. Path: $[1] | LineNumber: 33 | BytePositionInLine: 0.

server 40127:
- unreadable: listing records answered 500

server 39729:
- unreadable: listing records answered 500

server mac-host.local:38787:
- unreadable: listing records answered 500

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 3 | 8 | 1.8 | 0 | 0 | 0 | 4 | 0 | 128.4 | no |
