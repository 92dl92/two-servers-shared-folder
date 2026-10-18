# Stress run: NaiveFile, 2 server(s): FAILED

- Commit `5d54eb86e993b9c29b4e326d9494a1811ac767b9 (from the build)`, 2026-10-18 22:21:10 UTC, 3.9 s
- Machine WS1: Microsoft Windows 10.0.26100, 2 CPUs, .NET 10.0.12
- Settings: 2 servers × 5 clients, 6 records, 40 changes per client, seed 2
- Clients by role: 4 Registrant, 4 Reviewer, 2 Admin
- Data folder `C:\share\6c\lab-NaiveFile-random-2\data`; run folder `C:\Users\Administrator\claudetest-6c\runs\lab-NaiveFile-random-2`
- Servers started by hand: `http://ws1.example:8080/`, `http://ws2.example:8080/`
- Routing random. Requests by server: ws2.example:8080 302, ws1.example:8080 318

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 300 | 40 | 10.4 | 22 | 20 | 0 | 50.4 | 141.6 | 150.0 |

Read p50/p95/max ms: 12.7 / 100.4 / 1034.5.

Writes by outcome: conflict 22, http500 218, saved 40, wrongStatus 20. Saves by action: Create 3, Update 10, Submit 2, Approve 1, Reject 1, Comment 23. Reads: 320 (http500 26).

Longest stall: 1.3 s with no save acknowledged, from 1.5 s into the run.

Stale reads: 6 (older than a version already acknowledged); 153 reads followed a save through another server.
- record 62a1faf2-84ea-4a46-b97d-2562fdea1eba through ws2.example:8080: version 3, 16.0 ms after version 4 was acknowledged by ws1.example:8080
- record 62a1faf2-84ea-4a46-b97d-2562fdea1eba through ws1.example:8080: version 3, 106.9 ms after version 4 was acknowledged by ws1.example:8080
- record 62a1faf2-84ea-4a46-b97d-2562fdea1eba through ws1.example:8080: version 3, 107.1 ms after version 4 was acknowledged by ws1.example:8080
- record 62a1faf2-84ea-4a46-b97d-2562fdea1eba through ws1.example:8080: version 3, 111.9 ms after version 4 was acknowledged by ws1.example:8080
- record 62a1faf2-84ea-4a46-b97d-2562fdea1eba through ws2.example:8080: version 3, 118.7 ms after version 4 was acknowledged by ws1.example:8080
- record c8fa8d3b-2562-4f0a-8a66-6a3eb748fb0e through ws1.example:8080: version 9, 19.2 ms after version 10 was acknowledged by ws1.example:8080

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | FAIL | 3/3 | 0 | 3 | 0 | 0 | 0 |
| server ws1.example:8080 | FAIL | 3/3 | 0 | 3 | 0 | 0 | 0 |
| server ws2.example:8080 | FAIL | 3/3 | 0 | 3 | 0 | 0 | 0 |

storage:
- lost update c8fa8d3b-2562-4f0a-8a66-6a3eb748fb0e v10 Comment by s2c2-reviewer 's2c2-reviewer:19' via ws1.example:8080 (stress-c55e3c163c264ab7bf89a16db8a868f1): version 10 holds Submit by s1c1-registrant
- lost update 58cd3c80-cda6-4fa1-9c9c-71d00ff5f412 v5 Comment by s2c5-reviewer 's2c5-reviewer:4' via ws1.example:8080 (stress-009925c6a34b49039a3a951da2efbab3): version 5 holds Approve by s1c2-reviewer
- lost update 62a1faf2-84ea-4a46-b97d-2562fdea1eba v4 Update by s2c1-registrant 's2c1-registrant:4' via ws1.example:8080 (stress-d2b9552949c848fe824c42d3d27d1dc2): version 4 holds Update by s1c4-admin

server ws1.example:8080:
- lost update c8fa8d3b-2562-4f0a-8a66-6a3eb748fb0e v10 Comment by s2c2-reviewer 's2c2-reviewer:19' via ws1.example:8080 (stress-c55e3c163c264ab7bf89a16db8a868f1): version 10 holds Submit by s1c1-registrant
- lost update 58cd3c80-cda6-4fa1-9c9c-71d00ff5f412 v5 Comment by s2c5-reviewer 's2c5-reviewer:4' via ws1.example:8080 (stress-009925c6a34b49039a3a951da2efbab3): version 5 holds Approve by s1c2-reviewer
- lost update 62a1faf2-84ea-4a46-b97d-2562fdea1eba v4 Update by s2c1-registrant 's2c1-registrant:4' via ws1.example:8080 (stress-d2b9552949c848fe824c42d3d27d1dc2): version 4 holds Update by s1c4-admin

server ws2.example:8080:
- lost update c8fa8d3b-2562-4f0a-8a66-6a3eb748fb0e v10 Comment by s2c2-reviewer 's2c2-reviewer:19' via ws1.example:8080 (stress-c55e3c163c264ab7bf89a16db8a868f1): version 10 holds Submit by s1c1-registrant
- lost update 58cd3c80-cda6-4fa1-9c9c-71d00ff5f412 v5 Comment by s2c5-reviewer 's2c5-reviewer:4' via ws1.example:8080 (stress-009925c6a34b49039a3a951da2efbab3): version 5 holds Approve by s1c2-reviewer
- lost update 62a1faf2-84ea-4a46-b97d-2562fdea1eba v4 Update by s2c1-registrant 's2c1-registrant:4' via ws1.example:8080 (stress-d2b9552949c848fe824c42d3d27d1dc2): version 4 holds Update by s1c4-admin

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 2 | 40 | 10.4 | 22 | 20 | 3 | 0 | 0 | 141.6 | no |
