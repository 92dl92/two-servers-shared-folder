# Stress run: NaiveFile, 2 server(s): FAILED

- Commit `5d54eb86e993b9c29b4e326d9494a1811ac767b9 (from the build)`, 2026-10-18 22:21:31 UTC, 2.5 s
- Machine WS1: Microsoft Windows 10.0.26100, 2 CPUs, .NET 10.0.12
- Settings: 2 servers × 5 clients, 6 records, 40 changes per client, seed 3
- Clients by role: 4 Registrant, 4 Reviewer, 2 Admin
- Data folder `C:\share\6c\lab-NaiveFile-random-3\data`; run folder `C:\Users\Administrator\claudetest-6c\runs\lab-NaiveFile-random-3`
- Servers started by hand: `http://ws1.example:8080/`, `http://ws2.example:8080/`
- Routing random. Requests by server: ws2.example:8080 321, ws1.example:8080 283

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 244 | 36 | 14.5 | 24 | 45 | 0 | 10.6 | 130.3 | 164.3 |

Read p50/p95/max ms: 13.4 / 99.0 / 169.0.

Writes by outcome: conflict 24, http500 139, saved 36, wrongStatus 45. Saves by action: Create 4, Update 5, Submit 2, Approve 1, Reject 1, Comment 23. Reads: 360 (http500 24, notFound 98).

Longest stall: 0.4 s with no save acknowledged, from 1.1 s into the run.

Stale reads: 100 (older than a version already acknowledged); 169 reads followed a save through another server.
- record 360f7084-c001-43b3-8a2a-78c71b78e7db through ws2.example:8080: version 0, 49.6 ms after version 1 was acknowledged by ws1.example:8080
- record 360f7084-c001-43b3-8a2a-78c71b78e7db through ws2.example:8080: version 0, 45.8 ms after version 1 was acknowledged by ws1.example:8080
- record 360f7084-c001-43b3-8a2a-78c71b78e7db through ws2.example:8080: version 0, 49.9 ms after version 1 was acknowledged by ws1.example:8080
- record 360f7084-c001-43b3-8a2a-78c71b78e7db through ws2.example:8080: version 0, 50.1 ms after version 1 was acknowledged by ws1.example:8080
- record 360f7084-c001-43b3-8a2a-78c71b78e7db through ws1.example:8080: version 0, 197.4 ms after version 1 was acknowledged by ws1.example:8080
- record 360f7084-c001-43b3-8a2a-78c71b78e7db through ws1.example:8080: version 0, 197.7 ms after version 1 was acknowledged by ws1.example:8080
- record 360f7084-c001-43b3-8a2a-78c71b78e7db through ws1.example:8080: version 0, 212.7 ms after version 1 was acknowledged by ws1.example:8080
- record 360f7084-c001-43b3-8a2a-78c71b78e7db through ws1.example:8080: version 0, 219.8 ms after version 1 was acknowledged by ws1.example:8080
- record 360f7084-c001-43b3-8a2a-78c71b78e7db through ws1.example:8080: version 0, 238.4 ms after version 1 was acknowledged by ws1.example:8080
- record 360f7084-c001-43b3-8a2a-78c71b78e7db through ws2.example:8080: version 0, 221.4 ms after version 1 was acknowledged by ws1.example:8080
- record 360f7084-c001-43b3-8a2a-78c71b78e7db through ws2.example:8080: version 0, 216.6 ms after version 1 was acknowledged by ws1.example:8080
- record 360f7084-c001-43b3-8a2a-78c71b78e7db through ws1.example:8080: version 0, 351.1 ms after version 1 was acknowledged by ws1.example:8080
- record 360f7084-c001-43b3-8a2a-78c71b78e7db through ws1.example:8080: version 0, 350.9 ms after version 1 was acknowledged by ws1.example:8080
- record 360f7084-c001-43b3-8a2a-78c71b78e7db through ws1.example:8080: version 0, 353.0 ms after version 1 was acknowledged by ws1.example:8080
- record 360f7084-c001-43b3-8a2a-78c71b78e7db through ws1.example:8080: version 0, 354.5 ms after version 1 was acknowledged by ws1.example:8080
- record 360f7084-c001-43b3-8a2a-78c71b78e7db through ws2.example:8080: version 0, 350.5 ms after version 1 was acknowledged by ws1.example:8080
- record 360f7084-c001-43b3-8a2a-78c71b78e7db through ws2.example:8080: version 0, 353.3 ms after version 1 was acknowledged by ws1.example:8080
- record 360f7084-c001-43b3-8a2a-78c71b78e7db through ws2.example:8080: version 0, 356.2 ms after version 1 was acknowledged by ws1.example:8080
- record 360f7084-c001-43b3-8a2a-78c71b78e7db through ws1.example:8080: version 0, 465.0 ms after version 1 was acknowledged by ws1.example:8080
- record 360f7084-c001-43b3-8a2a-78c71b78e7db through ws2.example:8080: version 0, 428.7 ms after version 1 was acknowledged by ws1.example:8080

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | FAIL | 3/4 | 1 | 3 | 0 | 0 | 0 |
| server ws1.example:8080 | FAIL | 3/4 | 1 | 3 | 0 | 0 | 0 |
| server ws2.example:8080 | FAIL | 3/4 | 1 | 3 | 0 | 0 | 0 |

storage:
- missing record 360f7084-c001-43b3-8a2a-78c71b78e7db: missing (1 acknowledged changes)
- lost update 7ef3ff6d-3c52-4d5c-8aa6-5c255ff6842b v6 Comment by s1c5-reviewer 's1c5-reviewer:9' via ws1.example:8080 (stress-cc965ef0da574a4abcfff4c86a55cbf6): version 6 holds Approve by s1c4-admin
- lost update c38bbc73-9b50-49e5-94d5-1bd47fbb3547 v5 Comment by s2c2-reviewer 's2c2-reviewer:8' via ws1.example:8080 (stress-f4c7eeec6398423680bd9ad3bdbd194e): version 5 holds Comment by s1c4-admin
- lost update 360f7084-c001-43b3-8a2a-78c71b78e7db v1 Create by s2c3-registrant via ws1.example:8080 (stress-e4b1ef02144f41119a5cf779bdedb4d0): record missing

server ws1.example:8080:
- missing record 360f7084-c001-43b3-8a2a-78c71b78e7db: missing (1 acknowledged changes)
- lost update 7ef3ff6d-3c52-4d5c-8aa6-5c255ff6842b v6 Comment by s1c5-reviewer 's1c5-reviewer:9' via ws1.example:8080 (stress-cc965ef0da574a4abcfff4c86a55cbf6): version 6 holds Approve by s1c4-admin
- lost update c38bbc73-9b50-49e5-94d5-1bd47fbb3547 v5 Comment by s2c2-reviewer 's2c2-reviewer:8' via ws1.example:8080 (stress-f4c7eeec6398423680bd9ad3bdbd194e): version 5 holds Comment by s1c4-admin
- lost update 360f7084-c001-43b3-8a2a-78c71b78e7db v1 Create by s2c3-registrant via ws1.example:8080 (stress-e4b1ef02144f41119a5cf779bdedb4d0): record missing

server ws2.example:8080:
- missing record 360f7084-c001-43b3-8a2a-78c71b78e7db: missing (1 acknowledged changes)
- lost update 7ef3ff6d-3c52-4d5c-8aa6-5c255ff6842b v6 Comment by s1c5-reviewer 's1c5-reviewer:9' via ws1.example:8080 (stress-cc965ef0da574a4abcfff4c86a55cbf6): version 6 holds Approve by s1c4-admin
- lost update c38bbc73-9b50-49e5-94d5-1bd47fbb3547 v5 Comment by s2c2-reviewer 's2c2-reviewer:8' via ws1.example:8080 (stress-f4c7eeec6398423680bd9ad3bdbd194e): version 5 holds Comment by s1c4-admin
- lost update 360f7084-c001-43b3-8a2a-78c71b78e7db v1 Create by s2c3-registrant via ws1.example:8080 (stress-e4b1ef02144f41119a5cf779bdedb4d0): record missing

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 2 | 36 | 14.5 | 24 | 45 | 3 | 0 | 0 | 130.3 | no |
