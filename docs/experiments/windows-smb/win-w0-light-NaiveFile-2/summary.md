# Stress run: NaiveFile, 2 server(s): FAILED

- Commit `c71df76c37c18ca38a44e27c02961e268fbb1c09`, 2026-10-18 11:30:11 UTC, 1.6 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 5 clients, 6 records, 40 changes per client, seed 2
- Clients by role: 4 Registrant, 4 Reviewer, 2 Admin
- Data folder `stress-runs/win-w0-light-NaiveFile-2/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-w0-light-NaiveFile-2`
- 2 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-w0-light-NaiveFile-2` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 268 | 48 | 30.9 | 20 | 36 | 0 | 23.9 | 120.9 | 142.2 |

Writes by outcome: conflict 20, http500 164, saved 48, wrongStatus 36. Saves by action: Create 6, Update 13, Submit 4, Approve 1, Reject 3, Comment 21. Reads: 400 (http500 3, notFound 135).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | FAIL | 7/6 | 2 | 3 | 0 | 0 | 0 |
| server ws1.example:35811 | FAIL | 7/6 | 2 | 3 | 0 | 0 | 0 |
| server ws1.example:42589 | FAIL | 7/6 | 2 | 3 | 0 | 0 | 0 |

storage:
- missing record c6cd97e2-999e-43f4-a1a5-015e4078af31: missing (1 acknowledged changes)
- missing record 94deb5eb-0475-4c76-a60a-3ae3d5206749: missing (1 acknowledged changes)
- lost update c6cd97e2-999e-43f4-a1a5-015e4078af31 v1 Create by s1c1-registrant via ws1.example:35811 (stress-5bce3b777c634b8fa1e87ea573dc23f8): record missing
- lost update 991f07c0-e5e5-43a1-819c-22cbb88c5753 v6 Reject by s1c4-admin via ws1.example:35811 (stress-b0607d9061a74b14a360da0a3b91ba81): version 6 holds Reject by s1c5-reviewer
- lost update 94deb5eb-0475-4c76-a60a-3ae3d5206749 v1 Create by s2c3-registrant via ws1.example:42589 (stress-8b61470cf9fa40e4aa2f092fc2e0dcd3): record missing

server ws1.example:35811:
- missing record c6cd97e2-999e-43f4-a1a5-015e4078af31: missing (1 acknowledged changes)
- missing record 94deb5eb-0475-4c76-a60a-3ae3d5206749: missing (1 acknowledged changes)
- lost update c6cd97e2-999e-43f4-a1a5-015e4078af31 v1 Create by s1c1-registrant via ws1.example:35811 (stress-5bce3b777c634b8fa1e87ea573dc23f8): record missing
- lost update 991f07c0-e5e5-43a1-819c-22cbb88c5753 v6 Reject by s1c4-admin via ws1.example:35811 (stress-b0607d9061a74b14a360da0a3b91ba81): version 6 holds Reject by s1c5-reviewer
- lost update 94deb5eb-0475-4c76-a60a-3ae3d5206749 v1 Create by s2c3-registrant via ws1.example:42589 (stress-8b61470cf9fa40e4aa2f092fc2e0dcd3): record missing

server ws1.example:42589:
- missing record c6cd97e2-999e-43f4-a1a5-015e4078af31: missing (1 acknowledged changes)
- missing record 94deb5eb-0475-4c76-a60a-3ae3d5206749: missing (1 acknowledged changes)
- lost update c6cd97e2-999e-43f4-a1a5-015e4078af31 v1 Create by s1c1-registrant via ws1.example:35811 (stress-5bce3b777c634b8fa1e87ea573dc23f8): record missing
- lost update 991f07c0-e5e5-43a1-819c-22cbb88c5753 v6 Reject by s1c4-admin via ws1.example:35811 (stress-b0607d9061a74b14a360da0a3b91ba81): version 6 holds Reject by s1c5-reviewer
- lost update 94deb5eb-0475-4c76-a60a-3ae3d5206749 v1 Create by s2c3-registrant via ws1.example:42589 (stress-8b61470cf9fa40e4aa2f092fc2e0dcd3): record missing

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 2 | 48 | 30.9 | 20 | 36 | 3 | 0 | 0 | 120.9 | no |
