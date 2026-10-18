# Stress run: NaiveFile, 2 server(s): FAILED

- Commit `5d54eb86e993b9c29b4e326d9494a1811ac767b9 (from the build)`, 2026-10-18 22:20:48 UTC, 1.7 s
- Machine WS1: Microsoft Windows 10.0.26100, 2 CPUs, .NET 10.0.12
- Settings: 2 servers × 5 clients, 6 records, 40 changes per client, seed 1
- Clients by role: 4 Registrant, 4 Reviewer, 2 Admin
- Data folder `C:\share\6c\lab-NaiveFile-random-1\data`; run folder `C:\Users\Administrator\claudetest-6c\runs\lab-NaiveFile-random-1`
- Servers started by hand: `http://ws1.example:8080/`, `http://ws2.example:8080/`
- Routing random. Requests by server: ws2.example:8080 281, ws1.example:8080 327

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 288 | 62 | 35.6 | 75 | 37 | 0 | 20.2 | 62.0 | 124.0 |

Read p50/p95/max ms: 12.6 / 47.9 / 94.2.

Writes by outcome: conflict 75, http500 114, saved 62, wrongStatus 37. Saves by action: Create 2, Update 4, Submit 2, Reject 2, Comment 52. Reads: 320 (http500 38).

Longest stall: 0.2 s with no save acknowledged, from 0.0 s into the run.

Stale reads: 0 (older than a version already acknowledged); 137 reads followed a save through another server.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | FAIL | 2/2 | 0 | 2 | 0 | 0 | 0 |
| server ws1.example:8080 | FAIL | 2/2 | 0 | 2 | 0 | 0 | 0 |
| server ws2.example:8080 | FAIL | 2/2 | 0 | 2 | 0 | 0 | 0 |

storage:
- lost update e090c2e5-beba-499b-b06a-b428d4f0f6a6 v24 Comment by s1c4-admin 's1c4-admin:34' via ws1.example:8080 (stress-cf6b66fbf25b47bfa6d9385efc3caf15): version 24 holds Comment by s1c2-reviewer
- lost update e090c2e5-beba-499b-b06a-b428d4f0f6a6 v12 Comment by s2c5-reviewer 's2c5-reviewer:17' via ws1.example:8080 (stress-ee0d5540a66c42469d1c2cd130cb478a): version 12 holds Submit by s1c3-registrant

server ws1.example:8080:
- lost update e090c2e5-beba-499b-b06a-b428d4f0f6a6 v24 Comment by s1c4-admin 's1c4-admin:34' via ws1.example:8080 (stress-cf6b66fbf25b47bfa6d9385efc3caf15): version 24 holds Comment by s1c2-reviewer
- lost update e090c2e5-beba-499b-b06a-b428d4f0f6a6 v12 Comment by s2c5-reviewer 's2c5-reviewer:17' via ws1.example:8080 (stress-ee0d5540a66c42469d1c2cd130cb478a): version 12 holds Submit by s1c3-registrant

server ws2.example:8080:
- lost update e090c2e5-beba-499b-b06a-b428d4f0f6a6 v24 Comment by s1c4-admin 's1c4-admin:34' via ws1.example:8080 (stress-cf6b66fbf25b47bfa6d9385efc3caf15): version 24 holds Comment by s1c2-reviewer
- lost update e090c2e5-beba-499b-b06a-b428d4f0f6a6 v12 Comment by s2c5-reviewer 's2c5-reviewer:17' via ws1.example:8080 (stress-ee0d5540a66c42469d1c2cd130cb478a): version 12 holds Submit by s1c3-registrant

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 2 | 62 | 35.6 | 75 | 37 | 2 | 0 | 0 | 62.0 | no |
