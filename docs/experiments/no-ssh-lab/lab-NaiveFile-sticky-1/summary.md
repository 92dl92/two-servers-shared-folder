# Stress run: NaiveFile, 2 server(s): FAILED

- Commit `5d54eb86e993b9c29b4e326d9494a1811ac767b9 (from the build)`, 2026-10-18 22:19:43 UTC, 1.6 s
- Machine WS1: Microsoft Windows 10.0.26100, 2 CPUs, .NET 10.0.12
- Settings: 2 servers × 5 clients, 6 records, 40 changes per client, seed 1
- Clients by role: 4 Registrant, 4 Reviewer, 2 Admin
- Data folder `C:\share\6c\lab-NaiveFile-sticky-1\data`; run folder `C:\Users\Administrator\claudetest-6c\runs\lab-NaiveFile-sticky-1`
- Servers started by hand: `http://ws1.example:8080/`, `http://ws2.example:8080/`
- Routing sticky. Requests by server: ws2.example:8080 234, ws1.example:8080 205

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 119 | 24 | 14.9 | 13 | 22 | 0 | 30.8 | 62.2 | 97.6 |

Read p50/p95/max ms: 6.5 / 41.4 / 165.4.

Writes by outcome: conflict 13, http500 60, saved 24, wrongStatus 22. Saves by action: Create 3, Update 6, Submit 1, Reject 1, Comment 13. Reads: 320 (http500 12, notFound 195).

Longest stall: 0.2 s with no save acknowledged, from 1.4 s into the run.

Stale reads: 195 (older than a version already acknowledged); 85 reads followed a save through another server.
- record d5fe2a8d-71b7-4471-8b63-4135925f0169 through ws1.example:8080: version 0, 68.3 ms after version 1 was acknowledged by ws1.example:8080
- record bc372301-c8e8-4064-a172-9d8d60dc29af through ws1.example:8080: version 0, 48.6 ms after version 1 was acknowledged by ws1.example:8080
- record d5fe2a8d-71b7-4471-8b63-4135925f0169 through ws1.example:8080: version 0, 93.0 ms after version 1 was acknowledged by ws1.example:8080
- record d5fe2a8d-71b7-4471-8b63-4135925f0169 through ws1.example:8080: version 0, 113.7 ms after version 1 was acknowledged by ws1.example:8080
- record d5fe2a8d-71b7-4471-8b63-4135925f0169 through ws1.example:8080: version 0, 113.7 ms after version 1 was acknowledged by ws1.example:8080
- record bc372301-c8e8-4064-a172-9d8d60dc29af through ws1.example:8080: version 0, 93.0 ms after version 1 was acknowledged by ws1.example:8080
- record d5fe2a8d-71b7-4471-8b63-4135925f0169 through ws1.example:8080: version 0, 116.7 ms after version 1 was acknowledged by ws1.example:8080
- record bc372301-c8e8-4064-a172-9d8d60dc29af through ws1.example:8080: version 0, 95.2 ms after version 1 was acknowledged by ws1.example:8080
- record bc372301-c8e8-4064-a172-9d8d60dc29af through ws1.example:8080: version 0, 95.4 ms after version 1 was acknowledged by ws1.example:8080
- record d5fe2a8d-71b7-4471-8b63-4135925f0169 through ws1.example:8080: version 0, 122.3 ms after version 1 was acknowledged by ws1.example:8080
- record bc372301-c8e8-4064-a172-9d8d60dc29af through ws1.example:8080: version 0, 99.1 ms after version 1 was acknowledged by ws1.example:8080
- record d5fe2a8d-71b7-4471-8b63-4135925f0169 through ws1.example:8080: version 0, 124.2 ms after version 1 was acknowledged by ws1.example:8080
- record bc372301-c8e8-4064-a172-9d8d60dc29af through ws1.example:8080: version 0, 103.7 ms after version 1 was acknowledged by ws1.example:8080
- record bc372301-c8e8-4064-a172-9d8d60dc29af through ws1.example:8080: version 0, 103.4 ms after version 1 was acknowledged by ws1.example:8080
- record bc372301-c8e8-4064-a172-9d8d60dc29af through ws2.example:8080: version 0, 48.7 ms after version 1 was acknowledged by ws1.example:8080
- record bc372301-c8e8-4064-a172-9d8d60dc29af through ws2.example:8080: version 0, 48.8 ms after version 1 was acknowledged by ws1.example:8080
- record d5fe2a8d-71b7-4471-8b63-4135925f0169 through ws1.example:8080: version 0, 131.2 ms after version 1 was acknowledged by ws1.example:8080
- record d5fe2a8d-71b7-4471-8b63-4135925f0169 through ws1.example:8080: version 0, 135.5 ms after version 1 was acknowledged by ws1.example:8080
- record bc372301-c8e8-4064-a172-9d8d60dc29af through ws1.example:8080: version 0, 108.2 ms after version 1 was acknowledged by ws1.example:8080
- record d5fe2a8d-71b7-4471-8b63-4135925f0169 through ws1.example:8080: version 0, 143.9 ms after version 1 was acknowledged by ws1.example:8080

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | FAIL | 1/3 | 2 | 2 | 0 | 0 | 0 |
| server ws1.example:8080 | FAIL | 1/3 | 2 | 2 | 0 | 0 | 0 |
| server ws2.example:8080 | FAIL | 1/3 | 2 | 2 | 0 | 0 | 0 |

storage:
- missing record d5fe2a8d-71b7-4471-8b63-4135925f0169: missing (1 acknowledged changes)
- missing record bc372301-c8e8-4064-a172-9d8d60dc29af: missing (1 acknowledged changes)
- lost update d5fe2a8d-71b7-4471-8b63-4135925f0169 v1 Create by s1c1-registrant via ws1.example:8080 (stress-dc6a4c4bcb9d4322b5affb2edcde1f61): record missing
- lost update bc372301-c8e8-4064-a172-9d8d60dc29af v1 Create by s1c1-registrant via ws1.example:8080 (stress-624d62b167034d9ebc3de1fd4c0e3a8c): record missing

server ws1.example:8080:
- missing record d5fe2a8d-71b7-4471-8b63-4135925f0169: missing (1 acknowledged changes)
- missing record bc372301-c8e8-4064-a172-9d8d60dc29af: missing (1 acknowledged changes)
- lost update d5fe2a8d-71b7-4471-8b63-4135925f0169 v1 Create by s1c1-registrant via ws1.example:8080 (stress-dc6a4c4bcb9d4322b5affb2edcde1f61): record missing
- lost update bc372301-c8e8-4064-a172-9d8d60dc29af v1 Create by s1c1-registrant via ws1.example:8080 (stress-624d62b167034d9ebc3de1fd4c0e3a8c): record missing

server ws2.example:8080:
- missing record d5fe2a8d-71b7-4471-8b63-4135925f0169: missing (1 acknowledged changes)
- missing record bc372301-c8e8-4064-a172-9d8d60dc29af: missing (1 acknowledged changes)
- lost update d5fe2a8d-71b7-4471-8b63-4135925f0169 v1 Create by s1c1-registrant via ws1.example:8080 (stress-dc6a4c4bcb9d4322b5affb2edcde1f61): record missing
- lost update bc372301-c8e8-4064-a172-9d8d60dc29af v1 Create by s1c1-registrant via ws1.example:8080 (stress-624d62b167034d9ebc3de1fd4c0e3a8c): record missing

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 2 | 24 | 14.9 | 13 | 22 | 2 | 0 | 0 | 62.2 | no |
