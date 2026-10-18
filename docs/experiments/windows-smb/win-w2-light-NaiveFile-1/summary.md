# Stress run: NaiveFile, 2 server(s): FAILED

- Commit `c71df76c37c18ca38a44e27c02961e268fbb1c09`, 2026-10-18 12:03:38 UTC, 2.8 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 5 clients, 6 records, 40 changes per client, seed 1
- Clients by role: 4 Registrant, 4 Reviewer, 2 Admin
- Data folder `stress-runs/win-w2-light-NaiveFile-1/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-w2-light-NaiveFile-1`
- 1 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-w2-light-NaiveFile-1` there
- 1 of the servers on `Administrator@ws2.example`, data folder `\\ws1.example\share\runs\win-w2-light-NaiveFile-1` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 263 | 55 | 19.5 | 37 | 33 | 0 | 36.8 | 161.8 | 217.4 |

Writes by outcome: conflict 37, http500 138, saved 55, wrongStatus 33. Saves by action: Create 4, Update 18, Submit 2, Reject 2, Comment 29. Reads: 360 (http500 30, notFound 73).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | FAIL | 3/4 | 1 | 3 | 0 | 0 | 0 |
| server ws1.example:44733 | FAIL | 3/4 | 1 | 3 | 0 | 0 | 0 |
| server ws2.example:38527 | FAIL | 3/4 | 1 | 3 | 0 | 0 | 0 |

storage:
- missing record 127ef120-8c8e-4cd2-83d4-6904849cc800: missing (1 acknowledged changes)
- lost update 127ef120-8c8e-4cd2-83d4-6904849cc800 v1 Create by s1c1-registrant via ws1.example:44733 (stress-cc2597e43e8f410f8d69eed65ca633f1): record missing
- lost update e14bad6d-cb5b-4d6d-ae05-53f389dd37a2 v9 Comment by s1c2-reviewer 's1c2-reviewer:38' via ws1.example:44733 (stress-6a1188b865324309a48906d32b9b2ed0): version 9 holds Comment by s2c5-reviewer
- lost update a5aa80eb-29b0-42bb-a6f9-020099f46206 v2 Comment by s1c5-reviewer 's1c5-reviewer:3' via ws1.example:44733 (stress-d983ea60fe944108a1b31e42f3f152ea): version 2 holds Update by s2c3-registrant

server ws1.example:44733:
- missing record 127ef120-8c8e-4cd2-83d4-6904849cc800: missing (1 acknowledged changes)
- lost update 127ef120-8c8e-4cd2-83d4-6904849cc800 v1 Create by s1c1-registrant via ws1.example:44733 (stress-cc2597e43e8f410f8d69eed65ca633f1): record missing
- lost update e14bad6d-cb5b-4d6d-ae05-53f389dd37a2 v9 Comment by s1c2-reviewer 's1c2-reviewer:38' via ws1.example:44733 (stress-6a1188b865324309a48906d32b9b2ed0): version 9 holds Comment by s2c5-reviewer
- lost update a5aa80eb-29b0-42bb-a6f9-020099f46206 v2 Comment by s1c5-reviewer 's1c5-reviewer:3' via ws1.example:44733 (stress-d983ea60fe944108a1b31e42f3f152ea): version 2 holds Update by s2c3-registrant

server ws2.example:38527:
- missing record 127ef120-8c8e-4cd2-83d4-6904849cc800: missing (1 acknowledged changes)
- lost update 127ef120-8c8e-4cd2-83d4-6904849cc800 v1 Create by s1c1-registrant via ws1.example:44733 (stress-cc2597e43e8f410f8d69eed65ca633f1): record missing
- lost update e14bad6d-cb5b-4d6d-ae05-53f389dd37a2 v9 Comment by s1c2-reviewer 's1c2-reviewer:38' via ws1.example:44733 (stress-6a1188b865324309a48906d32b9b2ed0): version 9 holds Comment by s2c5-reviewer
- lost update a5aa80eb-29b0-42bb-a6f9-020099f46206 v2 Comment by s1c5-reviewer 's1c5-reviewer:3' via ws1.example:44733 (stress-d983ea60fe944108a1b31e42f3f152ea): version 2 holds Update by s2c3-registrant

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 2 | 55 | 19.5 | 37 | 33 | 3 | 0 | 0 | 161.8 | no |
