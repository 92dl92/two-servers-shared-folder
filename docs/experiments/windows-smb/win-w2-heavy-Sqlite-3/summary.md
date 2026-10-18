# Stress run: Sqlite, 2 server(s): FAILED

- Commit `c71df76c37c18ca38a44e27c02961e268fbb1c09`, 2026-10-18 12:38:47 UTC, 11.0 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 12 clients, 12 records, 200 changes per client, seed 3
- Clients by role: 10 Registrant, 10 Reviewer, 4 Admin
- Data folder `stress-runs/win-w2-heavy-Sqlite-3/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-w2-heavy-Sqlite-3`
- 1 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-w2-heavy-Sqlite-3` there
- 1 of the servers on `Administrator@ws2.example`, data folder `\\ws1.example\share\runs\win-w2-heavy-Sqlite-3` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 2231 | 504 | 45.9 | 510 | 877 | 0 | 52.8 | 379.4 | 616.1 |

Writes by outcome: conflict 510, http500 9, notFound 331, saved 504, wrongStatus 877. Saves by action: Create 12, Update 171, Submit 11, Approve 2, Reject 6, Comment 302. Reads: 4800 (http500 1170, notFound 1411).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | FAIL | 6/12 | 6 | 67 | 0 | 0 | 0 |
| server ws1.example:43625 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server ws2.example:38979 | FAIL | 6/12 | 6 | 67 | 0 | 0 | 0 |

storage:
- missing record 62300266-87bd-4209-a8e2-14dfb98341f8: missing (13 acknowledged changes)
- missing record 8604931e-c844-43f7-8f43-744746f6597e: missing (9 acknowledged changes)
- missing record db24bce2-180c-4128-af7e-2809650e8429: missing (10 acknowledged changes)
- missing record 29c4091a-0466-491d-ad6e-64d04f303c9e: missing (10 acknowledged changes)
- missing record c2f425e3-3e85-4ad4-97d0-eda8db3e53ee: missing (12 acknowledged changes)
- missing record 451132e0-9b26-47dd-8154-ae8e760848c2: missing (13 acknowledged changes)
- lost update 62300266-87bd-4209-a8e2-14dfb98341f8 v1 Create by s1c1-registrant via ws1.example:43625 (stress-897b569b11284c8fbaba2e351e7142f9): record missing
- lost update 62300266-87bd-4209-a8e2-14dfb98341f8 v3 Update by s1c1-registrant 's1c1-registrant:6' via ws1.example:43625 (stress-53bc69b8d7fa4716bc11d5d6009b456a): record missing
- lost update 62300266-87bd-4209-a8e2-14dfb98341f8 v4 Submit by s1c1-registrant via ws1.example:43625 (stress-e3627ebe9fd44b3382cecc7ea48e0882): record missing
- lost update 62300266-87bd-4209-a8e2-14dfb98341f8 v11 Comment by s1c4-admin 's1c4-admin:64' via ws1.example:43625 (stress-54dcabbd58c64d8081eceb4d8c37790a): record missing
- lost update 62300266-87bd-4209-a8e2-14dfb98341f8 v13 Comment by s1c4-admin 's1c4-admin:81' via ws1.example:43625 (stress-fdba13f2d2e24f9699f9a249e335372c): record missing
- lost update 62300266-87bd-4209-a8e2-14dfb98341f8 v6 Comment by s1c7-reviewer 's1c7-reviewer:15' via ws1.example:43625 (stress-61725d4638e144f1a2153aa360a7996e): record missing
- lost update 62300266-87bd-4209-a8e2-14dfb98341f8 v8 Comment by s1c10-reviewer 's1c10-reviewer:38' via ws1.example:43625 (stress-2258a2a0b50740bf84b0412c11845cf0): record missing
- lost update 62300266-87bd-4209-a8e2-14dfb98341f8 v2 Comment by s1c12-reviewer 's1c12-reviewer:0' via ws1.example:43625 (stress-8c4480fdd0914399b210b0b94284bdbc): record missing
- lost update 62300266-87bd-4209-a8e2-14dfb98341f8 v5 Approve by s1c12-reviewer via ws1.example:43625 (stress-a1139434c908453f90618d3f98f24d74): record missing
- lost update 62300266-87bd-4209-a8e2-14dfb98341f8 v7 Comment by s1c12-reviewer 's1c12-reviewer:29' via ws1.example:43625 (stress-ce61b33032e74543a4ffbba9bc730202): record missing
- lost update 62300266-87bd-4209-a8e2-14dfb98341f8 v9 Comment by s1c12-reviewer 's1c12-reviewer:44' via ws1.example:43625 (stress-6ad565e34cfd4c908ea2091ce65a0e6d): record missing
- lost update 62300266-87bd-4209-a8e2-14dfb98341f8 v10 Comment by s1c12-reviewer 's1c12-reviewer:47' via ws1.example:43625 (stress-ba456c173c6e4dabb86aed590377e265): record missing
- lost update 62300266-87bd-4209-a8e2-14dfb98341f8 v12 Comment by s1c12-reviewer 's1c12-reviewer:62' via ws1.example:43625 (stress-f9a6fa69bbbc4b5bbb280c480e3f4cd6): record missing
- lost update 8604931e-c844-43f7-8f43-744746f6597e v1 Create by s1c1-registrant via ws1.example:43625 (stress-cae0e3840c174e3d877cfa7e54695598): record missing
- lost update 8604931e-c844-43f7-8f43-744746f6597e v6 Submit by s1c1-registrant via ws1.example:43625 (stress-f6c4ffad1f0847849998e5996541c18b): record missing
- lost update 8604931e-c844-43f7-8f43-744746f6597e v8 Update by s1c1-registrant 's1c1-registrant:97' via ws1.example:43625 (stress-21082af891544504afbb11eb841a992c): record missing
- lost update 8604931e-c844-43f7-8f43-744746f6597e v9 Update by s1c1-registrant 's1c1-registrant:103' via ws1.example:43625 (stress-d6c3fe1e70b54e2293a38c597fe0d959): record missing
- lost update 8604931e-c844-43f7-8f43-744746f6597e v4 Update by s1c4-admin 's1c4-admin:21' via ws1.example:43625 (stress-41fdd14a26d64b398f5c1e55894037c7): record missing
- lost update 8604931e-c844-43f7-8f43-744746f6597e v5 Comment by s1c7-reviewer 's1c7-reviewer:25' via ws1.example:43625 (stress-ba245dafb854472b87ba2483e86725a7): record missing
- lost update 8604931e-c844-43f7-8f43-744746f6597e v7 Comment by s1c7-reviewer 's1c7-reviewer:46' via ws1.example:43625 (stress-dddcbd5632004106b31a0e6c6d109ab1): record missing

server ws1.example:43625:
- unreadable: listing records answered 500

server ws2.example:38979:
- missing record 62300266-87bd-4209-a8e2-14dfb98341f8: missing (13 acknowledged changes)
- missing record 8604931e-c844-43f7-8f43-744746f6597e: missing (9 acknowledged changes)
- missing record db24bce2-180c-4128-af7e-2809650e8429: missing (10 acknowledged changes)
- missing record 29c4091a-0466-491d-ad6e-64d04f303c9e: missing (10 acknowledged changes)
- missing record c2f425e3-3e85-4ad4-97d0-eda8db3e53ee: missing (12 acknowledged changes)
- missing record 451132e0-9b26-47dd-8154-ae8e760848c2: missing (13 acknowledged changes)
- lost update 62300266-87bd-4209-a8e2-14dfb98341f8 v1 Create by s1c1-registrant via ws1.example:43625 (stress-897b569b11284c8fbaba2e351e7142f9): record missing
- lost update 62300266-87bd-4209-a8e2-14dfb98341f8 v3 Update by s1c1-registrant 's1c1-registrant:6' via ws1.example:43625 (stress-53bc69b8d7fa4716bc11d5d6009b456a): record missing
- lost update 62300266-87bd-4209-a8e2-14dfb98341f8 v4 Submit by s1c1-registrant via ws1.example:43625 (stress-e3627ebe9fd44b3382cecc7ea48e0882): record missing
- lost update 62300266-87bd-4209-a8e2-14dfb98341f8 v11 Comment by s1c4-admin 's1c4-admin:64' via ws1.example:43625 (stress-54dcabbd58c64d8081eceb4d8c37790a): record missing
- lost update 62300266-87bd-4209-a8e2-14dfb98341f8 v13 Comment by s1c4-admin 's1c4-admin:81' via ws1.example:43625 (stress-fdba13f2d2e24f9699f9a249e335372c): record missing
- lost update 62300266-87bd-4209-a8e2-14dfb98341f8 v6 Comment by s1c7-reviewer 's1c7-reviewer:15' via ws1.example:43625 (stress-61725d4638e144f1a2153aa360a7996e): record missing
- lost update 62300266-87bd-4209-a8e2-14dfb98341f8 v8 Comment by s1c10-reviewer 's1c10-reviewer:38' via ws1.example:43625 (stress-2258a2a0b50740bf84b0412c11845cf0): record missing
- lost update 62300266-87bd-4209-a8e2-14dfb98341f8 v2 Comment by s1c12-reviewer 's1c12-reviewer:0' via ws1.example:43625 (stress-8c4480fdd0914399b210b0b94284bdbc): record missing
- lost update 62300266-87bd-4209-a8e2-14dfb98341f8 v5 Approve by s1c12-reviewer via ws1.example:43625 (stress-a1139434c908453f90618d3f98f24d74): record missing
- lost update 62300266-87bd-4209-a8e2-14dfb98341f8 v7 Comment by s1c12-reviewer 's1c12-reviewer:29' via ws1.example:43625 (stress-ce61b33032e74543a4ffbba9bc730202): record missing
- lost update 62300266-87bd-4209-a8e2-14dfb98341f8 v9 Comment by s1c12-reviewer 's1c12-reviewer:44' via ws1.example:43625 (stress-6ad565e34cfd4c908ea2091ce65a0e6d): record missing
- lost update 62300266-87bd-4209-a8e2-14dfb98341f8 v10 Comment by s1c12-reviewer 's1c12-reviewer:47' via ws1.example:43625 (stress-ba456c173c6e4dabb86aed590377e265): record missing
- lost update 62300266-87bd-4209-a8e2-14dfb98341f8 v12 Comment by s1c12-reviewer 's1c12-reviewer:62' via ws1.example:43625 (stress-f9a6fa69bbbc4b5bbb280c480e3f4cd6): record missing
- lost update 8604931e-c844-43f7-8f43-744746f6597e v1 Create by s1c1-registrant via ws1.example:43625 (stress-cae0e3840c174e3d877cfa7e54695598): record missing
- lost update 8604931e-c844-43f7-8f43-744746f6597e v6 Submit by s1c1-registrant via ws1.example:43625 (stress-f6c4ffad1f0847849998e5996541c18b): record missing
- lost update 8604931e-c844-43f7-8f43-744746f6597e v8 Update by s1c1-registrant 's1c1-registrant:97' via ws1.example:43625 (stress-21082af891544504afbb11eb841a992c): record missing
- lost update 8604931e-c844-43f7-8f43-744746f6597e v9 Update by s1c1-registrant 's1c1-registrant:103' via ws1.example:43625 (stress-d6c3fe1e70b54e2293a38c597fe0d959): record missing
- lost update 8604931e-c844-43f7-8f43-744746f6597e v4 Update by s1c4-admin 's1c4-admin:21' via ws1.example:43625 (stress-41fdd14a26d64b398f5c1e55894037c7): record missing
- lost update 8604931e-c844-43f7-8f43-744746f6597e v5 Comment by s1c7-reviewer 's1c7-reviewer:25' via ws1.example:43625 (stress-ba245dafb854472b87ba2483e86725a7): record missing
- lost update 8604931e-c844-43f7-8f43-744746f6597e v7 Comment by s1c7-reviewer 's1c7-reviewer:46' via ws1.example:43625 (stress-dddcbd5632004106b31a0e6c6d109ab1): record missing

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| Sqlite | 2 | 504 | 45.9 | 510 | 877 | 67 | 1 | 0 | 379.4 | no |
