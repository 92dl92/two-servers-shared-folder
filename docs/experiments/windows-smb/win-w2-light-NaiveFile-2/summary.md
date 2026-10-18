# Stress run: NaiveFile, 2 server(s): FAILED

- Commit `c71df76c37c18ca38a44e27c02961e268fbb1c09`, 2026-10-18 12:04:34 UTC, 2.7 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 5 clients, 6 records, 40 changes per client, seed 2
- Clients by role: 4 Registrant, 4 Reviewer, 2 Admin
- Data folder `stress-runs/win-w2-light-NaiveFile-2/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-w2-light-NaiveFile-2`
- 1 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-w2-light-NaiveFile-2` there
- 1 of the servers on `Administrator@ws2.example`, data folder `\\ws1.example\share\runs\win-w2-light-NaiveFile-2` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 243 | 53 | 19.4 | 17 | 23 | 0 | 40.3 | 183.2 | 285.2 |

Writes by outcome: conflict 17, http500 150, saved 53, wrongStatus 23. Saves by action: Create 4, Update 27, Submit 2, Approve 1, Comment 19. Reads: 360 (http500 53, notFound 70).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | FAIL | 3/4 | 1 | 3 | 0 | 0 | 0 |
| server ws1.example:43981 | FAIL | 3/4 | 1 | 3 | 0 | 0 | 0 |
| server ws2.example:38561 | FAIL | 3/4 | 1 | 3 | 0 | 0 | 0 |

storage:
- missing record bd04cd35-289f-492e-86bb-0bdc531acb64: missing (1 acknowledged changes)
- lost update bd04cd35-289f-492e-86bb-0bdc531acb64 v1 Create by s1c1-registrant via ws1.example:43981 (stress-4935290e00fd4563add04fde61465dfa): record missing
- lost update 86025902-098d-4566-a59c-5eeaddbba86a v2 Update by s1c4-admin 's1c4-admin:0' via ws1.example:43981 (stress-bfc8091b13e441d3bba2986a4aa30a9e): version 2 holds Comment by s2c5-reviewer
- lost update 242ab118-a743-4116-83f2-993e3eab897f v3 Comment by s1c5-reviewer 's1c5-reviewer:34' via ws1.example:43981 (stress-739437f0389b4723a8b1d3642fa0caed): version 3 holds Comment by s2c5-reviewer

server ws1.example:43981:
- missing record bd04cd35-289f-492e-86bb-0bdc531acb64: missing (1 acknowledged changes)
- lost update bd04cd35-289f-492e-86bb-0bdc531acb64 v1 Create by s1c1-registrant via ws1.example:43981 (stress-4935290e00fd4563add04fde61465dfa): record missing
- lost update 86025902-098d-4566-a59c-5eeaddbba86a v2 Update by s1c4-admin 's1c4-admin:0' via ws1.example:43981 (stress-bfc8091b13e441d3bba2986a4aa30a9e): version 2 holds Comment by s2c5-reviewer
- lost update 242ab118-a743-4116-83f2-993e3eab897f v3 Comment by s1c5-reviewer 's1c5-reviewer:34' via ws1.example:43981 (stress-739437f0389b4723a8b1d3642fa0caed): version 3 holds Comment by s2c5-reviewer

server ws2.example:38561:
- missing record bd04cd35-289f-492e-86bb-0bdc531acb64: missing (1 acknowledged changes)
- lost update bd04cd35-289f-492e-86bb-0bdc531acb64 v1 Create by s1c1-registrant via ws1.example:43981 (stress-4935290e00fd4563add04fde61465dfa): record missing
- lost update 86025902-098d-4566-a59c-5eeaddbba86a v2 Update by s1c4-admin 's1c4-admin:0' via ws1.example:43981 (stress-bfc8091b13e441d3bba2986a4aa30a9e): version 2 holds Comment by s2c5-reviewer
- lost update 242ab118-a743-4116-83f2-993e3eab897f v3 Comment by s1c5-reviewer 's1c5-reviewer:34' via ws1.example:43981 (stress-739437f0389b4723a8b1d3642fa0caed): version 3 holds Comment by s2c5-reviewer

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 2 | 53 | 19.4 | 17 | 23 | 3 | 0 | 0 | 183.2 | no |
