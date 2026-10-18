# Stress run: NaiveFile, 2 server(s): FAILED

- Commit `c71df76c37c18ca38a44e27c02961e268fbb1c09`, 2026-10-18 11:29:16 UTC, 1.6 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 5 clients, 6 records, 40 changes per client, seed 1
- Clients by role: 4 Registrant, 4 Reviewer, 2 Admin
- Data folder `stress-runs/win-w0-light-NaiveFile-1/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-w0-light-NaiveFile-1`
- 2 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-w0-light-NaiveFile-1` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 307 | 57 | 34.7 | 39 | 43 | 0 | 10.2 | 79.7 | 130.2 |

Writes by outcome: conflict 39, http500 168, saved 57, wrongStatus 43. Saves by action: Create 3, Update 8, Submit 3, Approve 2, Reject 1, Comment 40. Reads: 320 (http500 19).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | FAIL | 6/3 | 0 | 8 | 0 | 0 | 0 |
| server ws1.example:42617 | FAIL | 6/3 | 0 | 8 | 0 | 0 | 0 |
| server ws1.example:39713 | FAIL | 6/3 | 0 | 8 | 0 | 0 | 0 |

storage:
- lost update 0ef73c78-0502-4aad-91fd-be1d6999fe96 v6 Comment by s1c2-reviewer 's1c2-reviewer:14' via ws1.example:42617 (stress-d880752e62584d98b46e36931e4d442e): version 6 holds Approve by s2c4-admin
- lost update 0ef73c78-0502-4aad-91fd-be1d6999fe96 v12 Comment by s1c4-admin 's1c4-admin:22' via ws1.example:42617 (stress-fbb74753ff254b34adcb2de276bd5fd3): version 12 holds Comment by s1c4-admin
- lost update 19d4dd34-8dcd-490c-ac1b-a44be627d9ff v5 Update by s2c1-registrant 's2c1-registrant:18' via ws1.example:39713 (stress-377a3a46a6b3477086d6c1d55eb52a3b): version 5 holds Update by s2c4-admin
- lost update 19d4dd34-8dcd-490c-ac1b-a44be627d9ff v13 Comment by s2c2-reviewer 's2c2-reviewer:30' via ws1.example:39713 (stress-1a0902487b4a4224941597698c55d480): version 13 holds Comment by s2c2-reviewer
- lost update 19d4dd34-8dcd-490c-ac1b-a44be627d9ff v4 Comment by s2c4-admin 's2c4-admin:8' via ws1.example:39713 (stress-b0854fad9aa449a586ee00535a5d3da1): version 4 holds Update by s1c4-admin
- lost update 3f468550-3fef-45f7-86f7-f23cf4745c97 v7 Comment by s1c5-reviewer 's1c5-reviewer:29' via ws1.example:42617 (stress-e27b9a4f190a4d5e89a4e12bcdd7d871): version 7 holds Comment by s2c2-reviewer
- lost update 3f468550-3fef-45f7-86f7-f23cf4745c97 v7 Comment by s2c4-admin 's2c4-admin:14' via ws1.example:39713 (stress-1b48f5b893cf4dd28df0f88729593404): version 7 holds Comment by s2c2-reviewer
- lost update 3f468550-3fef-45f7-86f7-f23cf4745c97 v12 Comment by s2c5-reviewer 's2c5-reviewer:39' via ws1.example:39713 (stress-334dfb7a6c3f42ef99c74c491c4f521f): no history entry at that version

server ws1.example:42617:
- lost update 0ef73c78-0502-4aad-91fd-be1d6999fe96 v6 Comment by s1c2-reviewer 's1c2-reviewer:14' via ws1.example:42617 (stress-d880752e62584d98b46e36931e4d442e): version 6 holds Approve by s2c4-admin
- lost update 0ef73c78-0502-4aad-91fd-be1d6999fe96 v12 Comment by s1c4-admin 's1c4-admin:22' via ws1.example:42617 (stress-fbb74753ff254b34adcb2de276bd5fd3): version 12 holds Comment by s1c4-admin
- lost update 19d4dd34-8dcd-490c-ac1b-a44be627d9ff v5 Update by s2c1-registrant 's2c1-registrant:18' via ws1.example:39713 (stress-377a3a46a6b3477086d6c1d55eb52a3b): version 5 holds Update by s2c4-admin
- lost update 19d4dd34-8dcd-490c-ac1b-a44be627d9ff v13 Comment by s2c2-reviewer 's2c2-reviewer:30' via ws1.example:39713 (stress-1a0902487b4a4224941597698c55d480): version 13 holds Comment by s2c2-reviewer
- lost update 19d4dd34-8dcd-490c-ac1b-a44be627d9ff v4 Comment by s2c4-admin 's2c4-admin:8' via ws1.example:39713 (stress-b0854fad9aa449a586ee00535a5d3da1): version 4 holds Update by s1c4-admin
- lost update 3f468550-3fef-45f7-86f7-f23cf4745c97 v7 Comment by s1c5-reviewer 's1c5-reviewer:29' via ws1.example:42617 (stress-e27b9a4f190a4d5e89a4e12bcdd7d871): version 7 holds Comment by s2c2-reviewer
- lost update 3f468550-3fef-45f7-86f7-f23cf4745c97 v7 Comment by s2c4-admin 's2c4-admin:14' via ws1.example:39713 (stress-1b48f5b893cf4dd28df0f88729593404): version 7 holds Comment by s2c2-reviewer
- lost update 3f468550-3fef-45f7-86f7-f23cf4745c97 v12 Comment by s2c5-reviewer 's2c5-reviewer:39' via ws1.example:39713 (stress-334dfb7a6c3f42ef99c74c491c4f521f): no history entry at that version

server ws1.example:39713:
- lost update 0ef73c78-0502-4aad-91fd-be1d6999fe96 v6 Comment by s1c2-reviewer 's1c2-reviewer:14' via ws1.example:42617 (stress-d880752e62584d98b46e36931e4d442e): version 6 holds Approve by s2c4-admin
- lost update 0ef73c78-0502-4aad-91fd-be1d6999fe96 v12 Comment by s1c4-admin 's1c4-admin:22' via ws1.example:42617 (stress-fbb74753ff254b34adcb2de276bd5fd3): version 12 holds Comment by s1c4-admin
- lost update 19d4dd34-8dcd-490c-ac1b-a44be627d9ff v5 Update by s2c1-registrant 's2c1-registrant:18' via ws1.example:39713 (stress-377a3a46a6b3477086d6c1d55eb52a3b): version 5 holds Update by s2c4-admin
- lost update 19d4dd34-8dcd-490c-ac1b-a44be627d9ff v13 Comment by s2c2-reviewer 's2c2-reviewer:30' via ws1.example:39713 (stress-1a0902487b4a4224941597698c55d480): version 13 holds Comment by s2c2-reviewer
- lost update 19d4dd34-8dcd-490c-ac1b-a44be627d9ff v4 Comment by s2c4-admin 's2c4-admin:8' via ws1.example:39713 (stress-b0854fad9aa449a586ee00535a5d3da1): version 4 holds Update by s1c4-admin
- lost update 3f468550-3fef-45f7-86f7-f23cf4745c97 v7 Comment by s1c5-reviewer 's1c5-reviewer:29' via ws1.example:42617 (stress-e27b9a4f190a4d5e89a4e12bcdd7d871): version 7 holds Comment by s2c2-reviewer
- lost update 3f468550-3fef-45f7-86f7-f23cf4745c97 v7 Comment by s2c4-admin 's2c4-admin:14' via ws1.example:39713 (stress-1b48f5b893cf4dd28df0f88729593404): version 7 holds Comment by s2c2-reviewer
- lost update 3f468550-3fef-45f7-86f7-f23cf4745c97 v12 Comment by s2c5-reviewer 's2c5-reviewer:39' via ws1.example:39713 (stress-334dfb7a6c3f42ef99c74c491c4f521f): no history entry at that version

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 2 | 57 | 34.7 | 39 | 43 | 8 | 0 | 0 | 79.7 | no |
