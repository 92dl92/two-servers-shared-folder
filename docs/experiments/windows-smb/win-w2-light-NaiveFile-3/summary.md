# Stress run: NaiveFile, 2 server(s): FAILED

- Commit `c71df76c37c18ca38a44e27c02961e268fbb1c09`, 2026-10-18 12:05:28 UTC, 3.1 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 5 clients, 6 records, 40 changes per client, seed 3
- Clients by role: 4 Registrant, 4 Reviewer, 2 Admin
- Data folder `stress-runs/win-w2-light-NaiveFile-3/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-w2-light-NaiveFile-3`
- 1 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-w2-light-NaiveFile-3` there
- 1 of the servers on `Administrator@ws2.example`, data folder `\\ws1.example\share\runs\win-w2-light-NaiveFile-3` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 335 | 72 | 23.3 | 48 | 20 | 0 | 28.1 | 159.3 | 286.1 |

Writes by outcome: conflict 48, http500 195, saved 72, wrongStatus 20. Saves by action: Create 3, Update 23, Submit 3, Approve 2, Reject 1, Comment 40. Reads: 360 (http500 31).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | FAIL | 3/3 | 0 | 3 | 0 | 0 | 0 |
| server ws1.example:35971 | FAIL | 3/3 | 0 | 3 | 0 | 0 | 0 |
| server ws2.example:40735 | FAIL | 3/3 | 0 | 3 | 0 | 0 | 0 |

storage:
- lost update 336ff103-51eb-45a1-a3f1-464d65604224 v19 Comment by s1c5-reviewer 's1c5-reviewer:37' via ws1.example:35971 (stress-3f425047e7ef4873920cf671aa96f8e3): version 19 holds Submit by s2c4-admin
- lost update db6f4f38-caed-4b6d-aa80-7c8ddcefa5d8 v14 Update by s2c3-registrant 's2c3-registrant:28' via ws2.example:40735 (stress-925840276a604e60ad0d485b526d47f6): version 14 holds Update by s2c3-registrant
- lost update db6f4f38-caed-4b6d-aa80-7c8ddcefa5d8 v14 Update by s2c3-registrant 's2c3-registrant:19' via ws2.example:40735 (stress-62e9a6720fc44d8fbb7fc95ba09a447e): the form data is from another update (notes 's2c3-registrant:28')

server ws1.example:35971:
- lost update 336ff103-51eb-45a1-a3f1-464d65604224 v19 Comment by s1c5-reviewer 's1c5-reviewer:37' via ws1.example:35971 (stress-3f425047e7ef4873920cf671aa96f8e3): version 19 holds Submit by s2c4-admin
- lost update db6f4f38-caed-4b6d-aa80-7c8ddcefa5d8 v14 Update by s2c3-registrant 's2c3-registrant:28' via ws2.example:40735 (stress-925840276a604e60ad0d485b526d47f6): version 14 holds Update by s2c3-registrant
- lost update db6f4f38-caed-4b6d-aa80-7c8ddcefa5d8 v14 Update by s2c3-registrant 's2c3-registrant:19' via ws2.example:40735 (stress-62e9a6720fc44d8fbb7fc95ba09a447e): the form data is from another update (notes 's2c3-registrant:28')

server ws2.example:40735:
- lost update 336ff103-51eb-45a1-a3f1-464d65604224 v19 Comment by s1c5-reviewer 's1c5-reviewer:37' via ws1.example:35971 (stress-3f425047e7ef4873920cf671aa96f8e3): version 19 holds Submit by s2c4-admin
- lost update db6f4f38-caed-4b6d-aa80-7c8ddcefa5d8 v14 Update by s2c3-registrant 's2c3-registrant:28' via ws2.example:40735 (stress-925840276a604e60ad0d485b526d47f6): version 14 holds Update by s2c3-registrant
- lost update db6f4f38-caed-4b6d-aa80-7c8ddcefa5d8 v14 Update by s2c3-registrant 's2c3-registrant:19' via ws2.example:40735 (stress-62e9a6720fc44d8fbb7fc95ba09a447e): the form data is from another update (notes 's2c3-registrant:28')

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 2 | 72 | 23.3 | 48 | 20 | 3 | 0 | 0 | 159.3 | no |
