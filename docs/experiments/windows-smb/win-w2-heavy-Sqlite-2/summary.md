# Stress run: Sqlite, 2 server(s): FAILED

- Commit `c71df76c37c18ca38a44e27c02961e268fbb1c09`, 2026-10-18 12:37:50 UTC, 7.7 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 12 clients, 12 records, 200 changes per client, seed 2
- Clients by role: 10 Registrant, 10 Reviewer, 4 Admin
- Data folder `stress-runs/win-w2-heavy-Sqlite-2/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-w2-heavy-Sqlite-2`
- 1 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-w2-heavy-Sqlite-2` there
- 1 of the servers on `Administrator@ws2.example`, data folder `\\ws1.example\share\runs\win-w2-heavy-Sqlite-2` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 2589 | 510 | 66.2 | 469 | 862 | 0 | 36.6 | 129.9 | 752.9 |

Writes by outcome: conflict 469, notFound 748, saved 510, wrongStatus 862. Saves by action: Create 12, Update 75, Submit 12, Approve 5, Reject 6, Comment 400. Reads: 4800 (notFound 2223).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | FAIL | 6/12 | 6 | 84 | 0 | 0 | 0 |
| server ws1.example:36393 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server ws2.example:44579 | FAIL | 6/12 | 6 | 84 | 0 | 0 | 0 |

storage:
- missing record 3e359c33-c767-4b56-b5db-7e1d0f6e05a2: missing (17 acknowledged changes)
- missing record 512f7c9e-d776-4ba4-81ed-771aa29d9c21: missing (13 acknowledged changes)
- missing record 5022cde7-0c57-4335-bc39-5e3124c3ac54: missing (13 acknowledged changes)
- missing record 128431e1-70da-4b16-9495-3f70eadfff3b: missing (10 acknowledged changes)
- missing record 478ec2c8-4430-40c3-94bc-25b35371dff6: missing (11 acknowledged changes)
- missing record f970579e-2b81-4b75-9078-fd6692c670b1: missing (20 acknowledged changes)
- lost update 3e359c33-c767-4b56-b5db-7e1d0f6e05a2 v1 Create by s1c1-registrant via ws1.example:36393 (stress-d3925b4997de4599bed48cf044e18951): record missing
- lost update 3e359c33-c767-4b56-b5db-7e1d0f6e05a2 v6 Submit by s1c1-registrant via ws1.example:36393 (stress-6cd6fb13eca94dbb9b9cc327ef85a9b7): record missing
- lost update 3e359c33-c767-4b56-b5db-7e1d0f6e05a2 v7 Reject by s1c2-reviewer via ws1.example:36393 (stress-1fb4879828b04c2c9c9a4af9df1c7356): record missing
- lost update 3e359c33-c767-4b56-b5db-7e1d0f6e05a2 v4 Comment by s1c5-reviewer 's1c5-reviewer:4' via ws1.example:36393 (stress-bf5404b9b8a0449883351c0acc8ca82a): record missing
- lost update 3e359c33-c767-4b56-b5db-7e1d0f6e05a2 v11 Comment by s1c5-reviewer 's1c5-reviewer:72' via ws1.example:36393 (stress-47d01a26612043758542d03c08543f52): record missing
- lost update 3e359c33-c767-4b56-b5db-7e1d0f6e05a2 v16 Comment by s1c7-reviewer 's1c7-reviewer:102' via ws1.example:36393 (stress-1018378f245b4879afa987741c34f850): record missing
- lost update 3e359c33-c767-4b56-b5db-7e1d0f6e05a2 v17 Comment by s1c7-reviewer 's1c7-reviewer:103' via ws1.example:36393 (stress-bf51a8ef63504faa9aa47d63e541937d): record missing
- lost update 3e359c33-c767-4b56-b5db-7e1d0f6e05a2 v2 Comment by s1c10-reviewer 's1c10-reviewer:1' via ws1.example:36393 (stress-ec7dcb69b63a4b8f9efae614cbe062d1): record missing
- lost update 3e359c33-c767-4b56-b5db-7e1d0f6e05a2 v5 Comment by s1c10-reviewer 's1c10-reviewer:9' via ws1.example:36393 (stress-6a15c16086044b968543e5227adebddf): record missing
- lost update 3e359c33-c767-4b56-b5db-7e1d0f6e05a2 v10 Comment by s1c10-reviewer 's1c10-reviewer:63' via ws1.example:36393 (stress-fc942cd867384b9c89b37c84c825842f): record missing
- lost update 3e359c33-c767-4b56-b5db-7e1d0f6e05a2 v12 Comment by s1c10-reviewer 's1c10-reviewer:80' via ws1.example:36393 (stress-7b88cb9c32d04d9f80a775e8912779e2): record missing
- lost update 3e359c33-c767-4b56-b5db-7e1d0f6e05a2 v13 Comment by s1c10-reviewer 's1c10-reviewer:85' via ws1.example:36393 (stress-680f3acaff8e45aeb179fe5a6f36afb6): record missing
- lost update 3e359c33-c767-4b56-b5db-7e1d0f6e05a2 v15 Comment by s1c10-reviewer 's1c10-reviewer:107' via ws1.example:36393 (stress-a5d88b21676a4622b6a25882ca53b4ec): record missing
- lost update 3e359c33-c767-4b56-b5db-7e1d0f6e05a2 v3 Comment by s1c12-reviewer 's1c12-reviewer:3' via ws1.example:36393 (stress-6d3f74467c4749f29e7816402742ce79): record missing
- lost update 3e359c33-c767-4b56-b5db-7e1d0f6e05a2 v8 Comment by s1c12-reviewer 's1c12-reviewer:31' via ws1.example:36393 (stress-029b1a6205ef44af8561eefb034d12e5): record missing
- lost update 3e359c33-c767-4b56-b5db-7e1d0f6e05a2 v9 Comment by s1c12-reviewer 's1c12-reviewer:34' via ws1.example:36393 (stress-c9cef41c73dc43858384c6c07a707453): record missing
- lost update 3e359c33-c767-4b56-b5db-7e1d0f6e05a2 v14 Comment by s1c12-reviewer 's1c12-reviewer:61' via ws1.example:36393 (stress-a9907eac194f4f5dac0e5cb459c598f0): record missing
- lost update 512f7c9e-d776-4ba4-81ed-771aa29d9c21 v1 Create by s1c1-registrant via ws1.example:36393 (stress-8a4f8d084e124c5ca594cd40a5e7e58c): record missing
- lost update 512f7c9e-d776-4ba4-81ed-771aa29d9c21 v2 Update by s1c1-registrant 's1c1-registrant:0' via ws1.example:36393 (stress-86498233254241fa9762dbe635a8c804): record missing
- lost update 512f7c9e-d776-4ba4-81ed-771aa29d9c21 v3 Update by s1c1-registrant 's1c1-registrant:1' via ws1.example:36393 (stress-8016686564004cd0b31a867c0e93ffc4): record missing

server ws1.example:36393:
- unreadable: reading 512f7c9e-d776-4ba4-81ed-771aa29d9c21 answered 404

server ws2.example:44579:
- missing record 3e359c33-c767-4b56-b5db-7e1d0f6e05a2: missing (17 acknowledged changes)
- missing record 512f7c9e-d776-4ba4-81ed-771aa29d9c21: missing (13 acknowledged changes)
- missing record 5022cde7-0c57-4335-bc39-5e3124c3ac54: missing (13 acknowledged changes)
- missing record 128431e1-70da-4b16-9495-3f70eadfff3b: missing (10 acknowledged changes)
- missing record 478ec2c8-4430-40c3-94bc-25b35371dff6: missing (11 acknowledged changes)
- missing record f970579e-2b81-4b75-9078-fd6692c670b1: missing (20 acknowledged changes)
- lost update 3e359c33-c767-4b56-b5db-7e1d0f6e05a2 v1 Create by s1c1-registrant via ws1.example:36393 (stress-d3925b4997de4599bed48cf044e18951): record missing
- lost update 3e359c33-c767-4b56-b5db-7e1d0f6e05a2 v6 Submit by s1c1-registrant via ws1.example:36393 (stress-6cd6fb13eca94dbb9b9cc327ef85a9b7): record missing
- lost update 3e359c33-c767-4b56-b5db-7e1d0f6e05a2 v7 Reject by s1c2-reviewer via ws1.example:36393 (stress-1fb4879828b04c2c9c9a4af9df1c7356): record missing
- lost update 3e359c33-c767-4b56-b5db-7e1d0f6e05a2 v4 Comment by s1c5-reviewer 's1c5-reviewer:4' via ws1.example:36393 (stress-bf5404b9b8a0449883351c0acc8ca82a): record missing
- lost update 3e359c33-c767-4b56-b5db-7e1d0f6e05a2 v11 Comment by s1c5-reviewer 's1c5-reviewer:72' via ws1.example:36393 (stress-47d01a26612043758542d03c08543f52): record missing
- lost update 3e359c33-c767-4b56-b5db-7e1d0f6e05a2 v16 Comment by s1c7-reviewer 's1c7-reviewer:102' via ws1.example:36393 (stress-1018378f245b4879afa987741c34f850): record missing
- lost update 3e359c33-c767-4b56-b5db-7e1d0f6e05a2 v17 Comment by s1c7-reviewer 's1c7-reviewer:103' via ws1.example:36393 (stress-bf51a8ef63504faa9aa47d63e541937d): record missing
- lost update 3e359c33-c767-4b56-b5db-7e1d0f6e05a2 v2 Comment by s1c10-reviewer 's1c10-reviewer:1' via ws1.example:36393 (stress-ec7dcb69b63a4b8f9efae614cbe062d1): record missing
- lost update 3e359c33-c767-4b56-b5db-7e1d0f6e05a2 v5 Comment by s1c10-reviewer 's1c10-reviewer:9' via ws1.example:36393 (stress-6a15c16086044b968543e5227adebddf): record missing
- lost update 3e359c33-c767-4b56-b5db-7e1d0f6e05a2 v10 Comment by s1c10-reviewer 's1c10-reviewer:63' via ws1.example:36393 (stress-fc942cd867384b9c89b37c84c825842f): record missing
- lost update 3e359c33-c767-4b56-b5db-7e1d0f6e05a2 v12 Comment by s1c10-reviewer 's1c10-reviewer:80' via ws1.example:36393 (stress-7b88cb9c32d04d9f80a775e8912779e2): record missing
- lost update 3e359c33-c767-4b56-b5db-7e1d0f6e05a2 v13 Comment by s1c10-reviewer 's1c10-reviewer:85' via ws1.example:36393 (stress-680f3acaff8e45aeb179fe5a6f36afb6): record missing
- lost update 3e359c33-c767-4b56-b5db-7e1d0f6e05a2 v15 Comment by s1c10-reviewer 's1c10-reviewer:107' via ws1.example:36393 (stress-a5d88b21676a4622b6a25882ca53b4ec): record missing
- lost update 3e359c33-c767-4b56-b5db-7e1d0f6e05a2 v3 Comment by s1c12-reviewer 's1c12-reviewer:3' via ws1.example:36393 (stress-6d3f74467c4749f29e7816402742ce79): record missing
- lost update 3e359c33-c767-4b56-b5db-7e1d0f6e05a2 v8 Comment by s1c12-reviewer 's1c12-reviewer:31' via ws1.example:36393 (stress-029b1a6205ef44af8561eefb034d12e5): record missing
- lost update 3e359c33-c767-4b56-b5db-7e1d0f6e05a2 v9 Comment by s1c12-reviewer 's1c12-reviewer:34' via ws1.example:36393 (stress-c9cef41c73dc43858384c6c07a707453): record missing
- lost update 3e359c33-c767-4b56-b5db-7e1d0f6e05a2 v14 Comment by s1c12-reviewer 's1c12-reviewer:61' via ws1.example:36393 (stress-a9907eac194f4f5dac0e5cb459c598f0): record missing
- lost update 512f7c9e-d776-4ba4-81ed-771aa29d9c21 v1 Create by s1c1-registrant via ws1.example:36393 (stress-8a4f8d084e124c5ca594cd40a5e7e58c): record missing
- lost update 512f7c9e-d776-4ba4-81ed-771aa29d9c21 v2 Update by s1c1-registrant 's1c1-registrant:0' via ws1.example:36393 (stress-86498233254241fa9762dbe635a8c804): record missing
- lost update 512f7c9e-d776-4ba4-81ed-771aa29d9c21 v3 Update by s1c1-registrant 's1c1-registrant:1' via ws1.example:36393 (stress-8016686564004cd0b31a867c0e93ffc4): record missing

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| Sqlite | 2 | 510 | 66.2 | 469 | 862 | 84 | 1 | 0 | 129.9 | no |
