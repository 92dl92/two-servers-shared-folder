# Stress run: NaiveFile, 2 server(s): FAILED

- Commit `fe53fea387646cb0ee65f1cccd5a309abb2291d2`, 2026-10-18 12:56:33 UTC, 127.5 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 5 clients, pipeline with 1000 ms think time, 120 changes per client, seed 3
- Clients by role: 4 Registrant, 4 Reviewer, 2 Admin
- Data folder `stress-runs/win-w2-real-NaiveFile-3/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-w2-real-NaiveFile-3`
- 1 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-w2-real-NaiveFile-3` there
- 1 of the servers on `Administrator@ws2.example`, data folder `\\ws1.example\share\runs\win-w2-real-NaiveFile-3` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 737 | 692 | 5.4 | 8 | 0 | 0 | 16.9 | 53.8 | 220.6 |

Writes by outcome: conflict 8, http500 37, saved 692. Saves by action: Create 161, Update 143, Submit 158, Approve 129, Reject 30, Comment 71. Reads: 586 (http500 5, notFound 3).

Stale reads: 4 (older than a version already acknowledged); 131 reads followed a save through another server.
- record d77b322a-f58a-4c9a-9646-36b204931ce5 through ws1.example:36427: version 0, 1370.9 ms after version 1 was acknowledged by ws1.example:36427
- record d77b322a-f58a-4c9a-9646-36b204931ce5 through ws1.example:36427: version 0, 2101.4 ms after version 1 was acknowledged by ws1.example:36427
- record d77b322a-f58a-4c9a-9646-36b204931ce5 through ws1.example:36427: version 0, 3162.7 ms after version 1 was acknowledged by ws1.example:36427
- record 9c4f6b05-da0e-43eb-86ce-2a72594f69dd through ws1.example:36427: version 2, 662.3 ms after version 3 was acknowledged by ws1.example:36427

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | FAIL | 160/161 | 1 | 4 | 0 | 0 | 0 |
| server ws1.example:36427 | FAIL | 160/161 | 1 | 4 | 0 | 0 | 0 |
| server ws2.example:35665 | FAIL | 160/161 | 1 | 4 | 0 | 0 | 0 |

storage:
- missing record d77b322a-f58a-4c9a-9646-36b204931ce5: missing (1 acknowledged changes)
- lost update a778c5eb-9d47-4b18-acdd-17fca4869542 v5 Comment by s1c4-admin 's1c4-admin:31' via ws1.example:36427 (stress-328ecf5be1984d78980e04dbb6c0d66b): version 5 holds Approve by s2c2-reviewer
- lost update 9c4f6b05-da0e-43eb-86ce-2a72594f69dd v3 Update by s1c3-registrant 's1c3-registrant:120' via ws1.example:36427 (stress-59f8594752074d92bfffff33e2c5fd0f): version 3 holds Submit by s1c3-registrant
- lost update d77b322a-f58a-4c9a-9646-36b204931ce5 v1 Create by s1c1-registrant via ws1.example:36427 (stress-0631fcafdee04d32b93afb0382349b12): record missing
- lost update cc769b66-170c-49f9-84de-55ac3214d430 v6 Approve by s1c2-reviewer via ws1.example:36427 (stress-3ff87bd3559b4ba98270eff5f21ad170): version 6 holds Approve by s2c2-reviewer

server ws1.example:36427:
- missing record d77b322a-f58a-4c9a-9646-36b204931ce5: missing (1 acknowledged changes)
- lost update a778c5eb-9d47-4b18-acdd-17fca4869542 v5 Comment by s1c4-admin 's1c4-admin:31' via ws1.example:36427 (stress-328ecf5be1984d78980e04dbb6c0d66b): version 5 holds Approve by s2c2-reviewer
- lost update 9c4f6b05-da0e-43eb-86ce-2a72594f69dd v3 Update by s1c3-registrant 's1c3-registrant:120' via ws1.example:36427 (stress-59f8594752074d92bfffff33e2c5fd0f): version 3 holds Submit by s1c3-registrant
- lost update d77b322a-f58a-4c9a-9646-36b204931ce5 v1 Create by s1c1-registrant via ws1.example:36427 (stress-0631fcafdee04d32b93afb0382349b12): record missing
- lost update cc769b66-170c-49f9-84de-55ac3214d430 v6 Approve by s1c2-reviewer via ws1.example:36427 (stress-3ff87bd3559b4ba98270eff5f21ad170): version 6 holds Approve by s2c2-reviewer

server ws2.example:35665:
- missing record d77b322a-f58a-4c9a-9646-36b204931ce5: missing (1 acknowledged changes)
- lost update a778c5eb-9d47-4b18-acdd-17fca4869542 v5 Comment by s1c4-admin 's1c4-admin:31' via ws1.example:36427 (stress-328ecf5be1984d78980e04dbb6c0d66b): version 5 holds Approve by s2c2-reviewer
- lost update 9c4f6b05-da0e-43eb-86ce-2a72594f69dd v3 Update by s1c3-registrant 's1c3-registrant:120' via ws1.example:36427 (stress-59f8594752074d92bfffff33e2c5fd0f): version 3 holds Submit by s1c3-registrant
- lost update d77b322a-f58a-4c9a-9646-36b204931ce5 v1 Create by s1c1-registrant via ws1.example:36427 (stress-0631fcafdee04d32b93afb0382349b12): record missing
- lost update cc769b66-170c-49f9-84de-55ac3214d430 v6 Approve by s1c2-reviewer via ws1.example:36427 (stress-3ff87bd3559b4ba98270eff5f21ad170): version 6 holds Approve by s2c2-reviewer

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 2 | 692 | 5.4 | 8 | 0 | 4 | 0 | 0 | 53.8 | no |
