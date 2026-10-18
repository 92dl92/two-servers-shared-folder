# Stress run: LockedFile, 3 server(s): FAILED

- Commit `1b49659b6d0d95e1a10858057ef4082c27040edb`, 2026-10-17 22:21:09 UTC, 15.8 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 12 records, 200 changes per client, seed 1
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `/mnt/share/runs/nfs-two-heavy-LockedFile-1/data`; run folder `stress-runs/nfs/nfs-two-heavy-LockedFile-1`
- 1 of the servers on `user@mac.example`, data folder `/Volumes/share/runs/nfs-two-heavy-LockedFile-1/data` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 12 | 5 | 0.3 | 0 | 0 | 0 | 148.5 | 251.4 | 251.4 |

Writes by outcome: http500 7, saved 5. Saves by action: Create 5. Reads: 3400 (http500 3400).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 44553 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server 39673 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server mac-host.local:35541 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |

storage:
- unreadable: JsonException: '}' is invalid after a single JSON value. Expected end of data. Path: $[5] | LineNumber: 157 | BytePositionInLine: 2.

server 44553:
- unreadable: listing records answered 500

server 39673:
- unreadable: listing records answered 500

server mac-host.local:35541:
- unreadable: listing records answered 500

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| LockedFile | 3 | 5 | 0.3 | 0 | 0 | 0 | 4 | 0 | 251.4 | no |
