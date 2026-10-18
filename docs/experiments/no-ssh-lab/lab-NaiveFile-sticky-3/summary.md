# Stress run: NaiveFile, 2 server(s): FAILED

- Commit `5d54eb86e993b9c29b4e326d9494a1811ac767b9 (from the build)`, 2026-10-18 22:20:23 UTC, 1.9 s
- Machine WS1: Microsoft Windows 10.0.26100, 2 CPUs, .NET 10.0.12
- Settings: 2 servers × 5 clients, 6 records, 40 changes per client, seed 3
- Clients by role: 4 Registrant, 4 Reviewer, 2 Admin
- Data folder `C:\share\6c\lab-NaiveFile-sticky-3\data`; run folder `C:\Users\Administrator\claudetest-6c\runs\lab-NaiveFile-sticky-3`
- Servers started by hand: `http://ws1.example:8080/`, `http://ws2.example:8080/`
- Routing sticky. Requests by server: ws2.example:8080 280, ws1.example:8080 242

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 202 | 41 | 21.6 | 19 | 18 | 0 | 29.8 | 71.3 | 114.1 |

Read p50/p95/max ms: 11.0 / 41.9 / 91.7.

Writes by outcome: conflict 19, http500 124, saved 41, wrongStatus 18. Saves by action: Create 3, Update 3, Submit 2, Approve 1, Reject 1, Comment 31. Reads: 320 (http500 13, notFound 111).

Longest stall: 0.1 s with no save acknowledged, from 0.0 s into the run.

Stale reads: 111 (older than a version already acknowledged); 53 reads followed a save through another server.
- record 5ab38461-4f13-40dd-af6d-aabcfad9de67 through ws1.example:8080: version 0, 59.9 ms after version 1 was acknowledged by ws1.example:8080
- record 5ab38461-4f13-40dd-af6d-aabcfad9de67 through ws1.example:8080: version 0, 105.6 ms after version 1 was acknowledged by ws1.example:8080
- record 5ab38461-4f13-40dd-af6d-aabcfad9de67 through ws1.example:8080: version 0, 122.3 ms after version 1 was acknowledged by ws1.example:8080
- record 5ab38461-4f13-40dd-af6d-aabcfad9de67 through ws1.example:8080: version 0, 122.3 ms after version 1 was acknowledged by ws1.example:8080
- record 5ab38461-4f13-40dd-af6d-aabcfad9de67 through ws1.example:8080: version 0, 124.3 ms after version 1 was acknowledged by ws1.example:8080
- record 5ab38461-4f13-40dd-af6d-aabcfad9de67 through ws1.example:8080: version 0, 126.5 ms after version 1 was acknowledged by ws1.example:8080
- record 5ab38461-4f13-40dd-af6d-aabcfad9de67 through ws1.example:8080: version 0, 131.7 ms after version 1 was acknowledged by ws1.example:8080
- record 5ab38461-4f13-40dd-af6d-aabcfad9de67 through ws1.example:8080: version 0, 133.5 ms after version 1 was acknowledged by ws1.example:8080
- record 5ab38461-4f13-40dd-af6d-aabcfad9de67 through ws1.example:8080: version 0, 137.8 ms after version 1 was acknowledged by ws1.example:8080
- record 5ab38461-4f13-40dd-af6d-aabcfad9de67 through ws1.example:8080: version 0, 148.0 ms after version 1 was acknowledged by ws1.example:8080
- record 5ab38461-4f13-40dd-af6d-aabcfad9de67 through ws1.example:8080: version 0, 156.7 ms after version 1 was acknowledged by ws1.example:8080
- record 5ab38461-4f13-40dd-af6d-aabcfad9de67 through ws1.example:8080: version 0, 159.0 ms after version 1 was acknowledged by ws1.example:8080
- record 5ab38461-4f13-40dd-af6d-aabcfad9de67 through ws1.example:8080: version 0, 161.9 ms after version 1 was acknowledged by ws1.example:8080
- record 5ab38461-4f13-40dd-af6d-aabcfad9de67 through ws1.example:8080: version 0, 165.8 ms after version 1 was acknowledged by ws1.example:8080
- record 5ab38461-4f13-40dd-af6d-aabcfad9de67 through ws1.example:8080: version 0, 167.7 ms after version 1 was acknowledged by ws1.example:8080
- record 5ab38461-4f13-40dd-af6d-aabcfad9de67 through ws1.example:8080: version 0, 170.1 ms after version 1 was acknowledged by ws1.example:8080
- record 5ab38461-4f13-40dd-af6d-aabcfad9de67 through ws1.example:8080: version 0, 174.6 ms after version 1 was acknowledged by ws1.example:8080
- record 5ab38461-4f13-40dd-af6d-aabcfad9de67 through ws1.example:8080: version 0, 176.8 ms after version 1 was acknowledged by ws1.example:8080
- record 5ab38461-4f13-40dd-af6d-aabcfad9de67 through ws1.example:8080: version 0, 181.1 ms after version 1 was acknowledged by ws1.example:8080
- record 5ab38461-4f13-40dd-af6d-aabcfad9de67 through ws1.example:8080: version 0, 183.7 ms after version 1 was acknowledged by ws1.example:8080

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | FAIL | 2/3 | 1 | 1 | 0 | 0 | 0 |
| server ws1.example:8080 | FAIL | 2/3 | 1 | 1 | 0 | 0 | 0 |
| server ws2.example:8080 | FAIL | 2/3 | 1 | 1 | 0 | 0 | 0 |

storage:
- missing record 5ab38461-4f13-40dd-af6d-aabcfad9de67: missing (1 acknowledged changes)
- lost update 5ab38461-4f13-40dd-af6d-aabcfad9de67 v1 Create by s1c1-registrant via ws1.example:8080 (stress-fba280142b344e8eb3278df82b934ff1): record missing

server ws1.example:8080:
- missing record 5ab38461-4f13-40dd-af6d-aabcfad9de67: missing (1 acknowledged changes)
- lost update 5ab38461-4f13-40dd-af6d-aabcfad9de67 v1 Create by s1c1-registrant via ws1.example:8080 (stress-fba280142b344e8eb3278df82b934ff1): record missing

server ws2.example:8080:
- missing record 5ab38461-4f13-40dd-af6d-aabcfad9de67: missing (1 acknowledged changes)
- lost update 5ab38461-4f13-40dd-af6d-aabcfad9de67 v1 Create by s1c1-registrant via ws1.example:8080 (stress-fba280142b344e8eb3278df82b934ff1): record missing

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 2 | 41 | 21.6 | 19 | 18 | 1 | 0 | 0 | 71.3 | no |
