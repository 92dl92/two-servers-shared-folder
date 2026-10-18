# Stress run: NaiveFile, 2 server(s): FAILED

- Commit `c71df76c37c18ca38a44e27c02961e268fbb1c09`, 2026-10-18 11:31:06 UTC, 1.2 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 5 clients, 6 records, 40 changes per client, seed 3
- Clients by role: 4 Registrant, 4 Reviewer, 2 Admin
- Data folder `stress-runs/win-w0-light-NaiveFile-3/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-w0-light-NaiveFile-3`
- 2 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-w0-light-NaiveFile-3` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 197 | 37 | 30.5 | 27 | 50 | 0 | 15.1 | 114.4 | 114.9 |

Writes by outcome: conflict 27, http500 83, saved 37, wrongStatus 50. Saves by action: Create 6, Update 7, Submit 3, Approve 3, Comment 18. Reads: 400 (http500 12, notFound 197).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | FAIL | 5/6 | 3 | 3 | 0 | 0 | 0 |
| server ws1.example:35979 | FAIL | 5/6 | 3 | 3 | 0 | 0 | 0 |
| server ws1.example:33679 | FAIL | 5/6 | 3 | 3 | 0 | 0 | 0 |

storage:
- missing record 8598d8d1-ab88-454f-abd0-a61cc419d9db: missing (1 acknowledged changes)
- missing record 679d4569-af77-4447-9510-3fb9f70994eb: missing (1 acknowledged changes)
- missing record e3bb24bf-6454-443b-895f-e76591aceb33: missing (1 acknowledged changes)
- lost update 8598d8d1-ab88-454f-abd0-a61cc419d9db v1 Create by s1c1-registrant via ws1.example:35979 (stress-a745bc135fbb4a0badfce07ad85b1cc3): record missing
- lost update 679d4569-af77-4447-9510-3fb9f70994eb v1 Create by s2c1-registrant via ws1.example:33679 (stress-ec7d8476b3cc46bdbbd63f0a88c7f792): record missing
- lost update e3bb24bf-6454-443b-895f-e76591aceb33 v1 Create by s1c3-registrant via ws1.example:35979 (stress-1c0ce6c050664e40a80aeb195d4a6acc): record missing

server ws1.example:35979:
- missing record 8598d8d1-ab88-454f-abd0-a61cc419d9db: missing (1 acknowledged changes)
- missing record 679d4569-af77-4447-9510-3fb9f70994eb: missing (1 acknowledged changes)
- missing record e3bb24bf-6454-443b-895f-e76591aceb33: missing (1 acknowledged changes)
- lost update 8598d8d1-ab88-454f-abd0-a61cc419d9db v1 Create by s1c1-registrant via ws1.example:35979 (stress-a745bc135fbb4a0badfce07ad85b1cc3): record missing
- lost update 679d4569-af77-4447-9510-3fb9f70994eb v1 Create by s2c1-registrant via ws1.example:33679 (stress-ec7d8476b3cc46bdbbd63f0a88c7f792): record missing
- lost update e3bb24bf-6454-443b-895f-e76591aceb33 v1 Create by s1c3-registrant via ws1.example:35979 (stress-1c0ce6c050664e40a80aeb195d4a6acc): record missing

server ws1.example:33679:
- missing record 8598d8d1-ab88-454f-abd0-a61cc419d9db: missing (1 acknowledged changes)
- missing record 679d4569-af77-4447-9510-3fb9f70994eb: missing (1 acknowledged changes)
- missing record e3bb24bf-6454-443b-895f-e76591aceb33: missing (1 acknowledged changes)
- lost update 8598d8d1-ab88-454f-abd0-a61cc419d9db v1 Create by s1c1-registrant via ws1.example:35979 (stress-a745bc135fbb4a0badfce07ad85b1cc3): record missing
- lost update 679d4569-af77-4447-9510-3fb9f70994eb v1 Create by s2c1-registrant via ws1.example:33679 (stress-ec7d8476b3cc46bdbbd63f0a88c7f792): record missing
- lost update e3bb24bf-6454-443b-895f-e76591aceb33 v1 Create by s1c3-registrant via ws1.example:35979 (stress-1c0ce6c050664e40a80aeb195d4a6acc): record missing

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 2 | 37 | 30.5 | 27 | 50 | 3 | 0 | 0 | 114.4 | no |
