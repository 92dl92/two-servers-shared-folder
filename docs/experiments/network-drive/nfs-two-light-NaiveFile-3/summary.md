# Stress run: NaiveFile, 3 server(s): FAILED

- Commit `1b49659b6d0d95e1a10858057ef4082c27040edb`, 2026-10-17 22:18:31 UTC, 1.6 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 5 clients, 6 records, 40 changes per client, seed 3
- Clients by role: 6 Registrant, 6 Reviewer, 3 Admin
- Data folder `/mnt/share/runs/nfs-two-light-NaiveFile-3/data`; run folder `stress-runs/nfs/nfs-two-light-NaiveFile-3`
- 1 of the servers on `user@mac.example`, data folder `/Volumes/share/runs/nfs-two-light-NaiveFile-3/data` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 65 | 36 | 22.6 | 7 | 6 | 0 | 19.2 | 196.6 | 197.1 |

Writes by outcome: conflict 7, http500 16, saved 36, wrongStatus 6. Saves by action: Create 6, Update 5, Submit 4, Approve 1, Comment 20. Reads: 600 (http500 364, notFound 177).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 41169 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 37429 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server mac-host.local:37611 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |

storage:
- unreadable: JsonException: '}' is invalid after a single JSON value. Expected end of data. Path: $[2] | LineNumber: 119 | BytePositionInLine: 6.

server 41169:
- unreadable: listing records answered 500

server 37429:
- unreadable: listing records answered 500

server mac-host.local:37611:
- unreadable: listing records answered 500

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 3 | 36 | 22.6 | 7 | 6 | 0 | 4 | 0 | 196.6 | no |
