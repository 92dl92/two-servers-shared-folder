# Stress run: Sqlite, 2 server(s): FAILED

- Commit `c71df76c37c18ca38a44e27c02961e268fbb1c09`, 2026-10-18 12:14:42 UTC, 1.2 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 5 clients, 6 records, 40 changes per client, seed 1
- Clients by role: 4 Registrant, 4 Reviewer, 2 Admin
- Data folder `stress-runs/win-w2-light-Sqlite-1/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-w2-light-Sqlite-1`
- 1 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-w2-light-Sqlite-1` there
- 1 of the servers on `Administrator@ws2.example`, data folder `\\ws1.example\share\runs\win-w2-light-Sqlite-1` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 192 | 117 | 98.9 | 58 | 16 | 0 | 10.8 | 125.8 | 156.3 |

Writes by outcome: conflict 58, notFound 1, saved 117, wrongStatus 16. Saves by action: Create 6, Update 58, Submit 5, Approve 1, Reject 2, Comment 45. Reads: 400 (notFound 214).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | FAIL | 3/6 | 3 | 34 | 0 | 0 | 0 |
| server ws1.example:42989 | FAIL | 3/6 | 3 | 83 | 0 | 0 | 0 |
| server ws2.example:34879 | FAIL | 3/6 | 3 | 34 | 0 | 0 | 0 |

storage:
- missing record b8acefee-6afa-4389-82b8-051ba3c93d51: missing (1 acknowledged changes)
- missing record ee19c64a-6207-4e57-92b4-9cdbb82aa0ed: missing (1 acknowledged changes)
- missing record 9fb92d06-af5b-4cbe-bf86-53519353f970: missing (1 acknowledged changes)
- lost update b8acefee-6afa-4389-82b8-051ba3c93d51 v1 Create by s1c1-registrant via ws1.example:42989 (stress-d25c72972c9d4e88927803542442b329): record missing
- lost update ee19c64a-6207-4e57-92b4-9cdbb82aa0ed v1 Create by s1c1-registrant via ws1.example:42989 (stress-b1e115aca13847838478fc200aa41bfa): record missing
- lost update d2d114a7-dfdb-412c-afd9-fe619235b272 v4 Comment by s1c2-reviewer 's1c2-reviewer:13' via ws1.example:42989 (stress-645bb3ac377b4892974e61712495a94d): version 4 holds Comment by s2c2-reviewer
- lost update d2d114a7-dfdb-412c-afd9-fe619235b272 v7 Comment by s1c2-reviewer 's1c2-reviewer:25' via ws1.example:42989 (stress-0d8f5fb3d9bf4c05a50ca7ae1a3107e5): version 7 holds Update by s2c1-registrant
- lost update d2d114a7-dfdb-412c-afd9-fe619235b272 v8 Comment by s1c2-reviewer 's1c2-reviewer:27' via ws1.example:42989 (stress-4f4f90c62d3c46268c3b67a19237cb75): version 8 holds Submit by s2c1-registrant
- lost update d2d114a7-dfdb-412c-afd9-fe619235b272 v2 Comment by s1c4-admin 's1c4-admin:5' via ws1.example:42989 (stress-138845706ac24ac6966589292de3f54f): version 2 holds Comment by s2c2-reviewer
- lost update d2d114a7-dfdb-412c-afd9-fe619235b272 v5 Update by s1c4-admin 's1c4-admin:13' via ws1.example:42989 (stress-2599fcaa12da47c89254ff9083135eb5): version 5 holds Comment by s2c2-reviewer
- lost update d2d114a7-dfdb-412c-afd9-fe619235b272 v6 Comment by s1c4-admin 's1c4-admin:14' via ws1.example:42989 (stress-e8aa57d181de4b3f8f216f1d584184e2): version 6 holds Update by s2c1-registrant
- lost update d2d114a7-dfdb-412c-afd9-fe619235b272 v3 Comment by s1c5-reviewer 's1c5-reviewer:8' via ws1.example:42989 (stress-66c1991ed46c4ac2885816a211cd52f5): version 3 holds Comment by s2c2-reviewer
- lost update d2d114a7-dfdb-412c-afd9-fe619235b272 v9 Comment by s1c5-reviewer 's1c5-reviewer:32' via ws1.example:42989 (stress-98a01422e5af492a8f9b478e9af25dcc): version 9 holds Update by s2c1-registrant
- lost update d2d114a7-dfdb-412c-afd9-fe619235b272 v10 Comment by s1c5-reviewer 's1c5-reviewer:35' via ws1.example:42989 (stress-ba2bc141f4274284a123ccff9eddcc9d): version 10 holds Update by s2c1-registrant
- lost update 8fb45117-dd5d-4135-a067-d42fc7ef9acf v3 Comment by s1c2-reviewer 's1c2-reviewer:7' via ws1.example:42989 (stress-6c43e1243a81442c8251760ae75f63a7): version 3 holds Update by s2c1-registrant
- lost update 8fb45117-dd5d-4135-a067-d42fc7ef9acf v7 Comment by s1c2-reviewer 's1c2-reviewer:28' via ws1.example:42989 (stress-893cb63304204929893c054027d1eaaf): version 7 holds Comment by s2c2-reviewer
- lost update 8fb45117-dd5d-4135-a067-d42fc7ef9acf v9 Comment by s1c2-reviewer 's1c2-reviewer:32' via ws1.example:42989 (stress-b7986360bd9244939435e499dde6e1d9): version 9 holds Comment by s2c4-admin
- lost update 8fb45117-dd5d-4135-a067-d42fc7ef9acf v10 Comment by s1c2-reviewer 's1c2-reviewer:35' via ws1.example:42989 (stress-a37f7f6b6eb3452eb7a5d90f89e4abcc): version 10 holds Comment by s2c4-admin
- lost update 8fb45117-dd5d-4135-a067-d42fc7ef9acf v11 Comment by s1c2-reviewer 's1c2-reviewer:36' via ws1.example:42989 (stress-e6445f917b9c4710ae41e68c26385670): version 11 holds Comment by s2c5-reviewer
- lost update 8fb45117-dd5d-4135-a067-d42fc7ef9acf v4 Comment by s1c4-admin 's1c4-admin:9' via ws1.example:42989 (stress-d4a38235f0ed48e190f8741f3041284c): version 4 holds Update by s2c1-registrant
- lost update 8fb45117-dd5d-4135-a067-d42fc7ef9acf v5 Submit by s1c4-admin via ws1.example:42989 (stress-bf95456e967d492699014c09ad01e55f): version 5 holds Reject by s2c5-reviewer
- lost update 8fb45117-dd5d-4135-a067-d42fc7ef9acf v8 Comment by s1c4-admin 's1c4-admin:27' via ws1.example:42989 (stress-ece8fe903c4b458dad5c56a724aa9ef7): version 8 holds Comment by s2c5-reviewer
- lost update 8fb45117-dd5d-4135-a067-d42fc7ef9acf v2 Comment by s1c5-reviewer 's1c5-reviewer:3' via ws1.example:42989 (stress-038070f9f2ad4070a824b74d5281ffa1): version 2 holds Submit by s2c1-registrant

server ws1.example:42989:
- missing record b8acefee-6afa-4389-82b8-051ba3c93d51: missing (1 acknowledged changes)
- missing record ee19c64a-6207-4e57-92b4-9cdbb82aa0ed: missing (1 acknowledged changes)
- missing record 9fb92d06-af5b-4cbe-bf86-53519353f970: missing (1 acknowledged changes)
- lost update b8acefee-6afa-4389-82b8-051ba3c93d51 v1 Create by s1c1-registrant via ws1.example:42989 (stress-d25c72972c9d4e88927803542442b329): record missing
- lost update ee19c64a-6207-4e57-92b4-9cdbb82aa0ed v1 Create by s1c1-registrant via ws1.example:42989 (stress-b1e115aca13847838478fc200aa41bfa): record missing
- lost update d2d114a7-dfdb-412c-afd9-fe619235b272 v6 Update by s2c1-registrant 's2c1-registrant:8' via ws2.example:34879 (stress-0188abe3770c4ce2bbd210534a01111e): version 6 holds Comment by s1c4-admin
- lost update d2d114a7-dfdb-412c-afd9-fe619235b272 v7 Update by s2c1-registrant 's2c1-registrant:9' via ws2.example:34879 (stress-c97df4fd8c7d490192a6dcb6c150c5de): version 7 holds Comment by s1c2-reviewer
- lost update d2d114a7-dfdb-412c-afd9-fe619235b272 v8 Submit by s2c1-registrant via ws2.example:34879 (stress-f9e321eb16204090aa72ce62388d25cb): version 8 holds Comment by s1c2-reviewer
- lost update d2d114a7-dfdb-412c-afd9-fe619235b272 v9 Update by s2c1-registrant 's2c1-registrant:12' via ws2.example:34879 (stress-092f2ed8b0be4a068eb7eb5e1fe26843): version 9 holds Comment by s1c5-reviewer
- lost update d2d114a7-dfdb-412c-afd9-fe619235b272 v10 Update by s2c1-registrant 's2c1-registrant:13' via ws2.example:34879 (stress-d509518de2184a3b955bb2dae9011807): version 10 holds Comment by s1c5-reviewer
- lost update d2d114a7-dfdb-412c-afd9-fe619235b272 v11 Update by s2c1-registrant 's2c1-registrant:14' via ws2.example:34879 (stress-a4fdf0bfee75495d81ca54c10095fecc): no history entry at that version
- lost update d2d114a7-dfdb-412c-afd9-fe619235b272 v12 Update by s2c1-registrant 's2c1-registrant:16' via ws2.example:34879 (stress-cc982bf52e0f4d2487e2c08043ee8e96): no history entry at that version
- lost update d2d114a7-dfdb-412c-afd9-fe619235b272 v13 Update by s2c1-registrant 's2c1-registrant:18' via ws2.example:34879 (stress-e25664c0c1f0414eba862318fafbe96a): no history entry at that version
- lost update d2d114a7-dfdb-412c-afd9-fe619235b272 v14 Update by s2c1-registrant 's2c1-registrant:20' via ws2.example:34879 (stress-d59747c3550c4130b9e1be82d7260db8): no history entry at that version
- lost update d2d114a7-dfdb-412c-afd9-fe619235b272 v15 Update by s2c1-registrant 's2c1-registrant:23' via ws2.example:34879 (stress-9aa6f9f558954d7888f99b9ba46a9f16): no history entry at that version
- lost update d2d114a7-dfdb-412c-afd9-fe619235b272 v16 Update by s2c1-registrant 's2c1-registrant:24' via ws2.example:34879 (stress-6ddd6093fec44a859b8f04a85f754b21): no history entry at that version
- lost update d2d114a7-dfdb-412c-afd9-fe619235b272 v17 Update by s2c1-registrant 's2c1-registrant:26' via ws2.example:34879 (stress-edda022865ae4ae3a4bf1804ccd4862b): no history entry at that version
- lost update d2d114a7-dfdb-412c-afd9-fe619235b272 v18 Update by s2c1-registrant 's2c1-registrant:27' via ws2.example:34879 (stress-5a4da662fa174979adce24a3c4122518): no history entry at that version
- lost update d2d114a7-dfdb-412c-afd9-fe619235b272 v19 Update by s2c1-registrant 's2c1-registrant:28' via ws2.example:34879 (stress-4c4a0b07af2d487ba617b4fa812ac057): no history entry at that version
- lost update d2d114a7-dfdb-412c-afd9-fe619235b272 v20 Update by s2c1-registrant 's2c1-registrant:29' via ws2.example:34879 (stress-c93fecf83b1f4273b236ada15b7ac935): no history entry at that version
- lost update d2d114a7-dfdb-412c-afd9-fe619235b272 v21 Update by s2c1-registrant 's2c1-registrant:31' via ws2.example:34879 (stress-51a2db52fcef4412b5a5455c6a930853): no history entry at that version
- lost update d2d114a7-dfdb-412c-afd9-fe619235b272 v22 Update by s2c1-registrant 's2c1-registrant:32' via ws2.example:34879 (stress-aea52830b4844c9d9cf6024af83f2dc8): no history entry at that version
- lost update d2d114a7-dfdb-412c-afd9-fe619235b272 v23 Update by s2c1-registrant 's2c1-registrant:33' via ws2.example:34879 (stress-a1d0b5df739a469bbade768500126af4): no history entry at that version

server ws2.example:34879:
- missing record b8acefee-6afa-4389-82b8-051ba3c93d51: missing (1 acknowledged changes)
- missing record ee19c64a-6207-4e57-92b4-9cdbb82aa0ed: missing (1 acknowledged changes)
- missing record 9fb92d06-af5b-4cbe-bf86-53519353f970: missing (1 acknowledged changes)
- lost update b8acefee-6afa-4389-82b8-051ba3c93d51 v1 Create by s1c1-registrant via ws1.example:42989 (stress-d25c72972c9d4e88927803542442b329): record missing
- lost update ee19c64a-6207-4e57-92b4-9cdbb82aa0ed v1 Create by s1c1-registrant via ws1.example:42989 (stress-b1e115aca13847838478fc200aa41bfa): record missing
- lost update d2d114a7-dfdb-412c-afd9-fe619235b272 v4 Comment by s1c2-reviewer 's1c2-reviewer:13' via ws1.example:42989 (stress-645bb3ac377b4892974e61712495a94d): version 4 holds Comment by s2c2-reviewer
- lost update d2d114a7-dfdb-412c-afd9-fe619235b272 v7 Comment by s1c2-reviewer 's1c2-reviewer:25' via ws1.example:42989 (stress-0d8f5fb3d9bf4c05a50ca7ae1a3107e5): version 7 holds Update by s2c1-registrant
- lost update d2d114a7-dfdb-412c-afd9-fe619235b272 v8 Comment by s1c2-reviewer 's1c2-reviewer:27' via ws1.example:42989 (stress-4f4f90c62d3c46268c3b67a19237cb75): version 8 holds Submit by s2c1-registrant
- lost update d2d114a7-dfdb-412c-afd9-fe619235b272 v2 Comment by s1c4-admin 's1c4-admin:5' via ws1.example:42989 (stress-138845706ac24ac6966589292de3f54f): version 2 holds Comment by s2c2-reviewer
- lost update d2d114a7-dfdb-412c-afd9-fe619235b272 v5 Update by s1c4-admin 's1c4-admin:13' via ws1.example:42989 (stress-2599fcaa12da47c89254ff9083135eb5): version 5 holds Comment by s2c2-reviewer
- lost update d2d114a7-dfdb-412c-afd9-fe619235b272 v6 Comment by s1c4-admin 's1c4-admin:14' via ws1.example:42989 (stress-e8aa57d181de4b3f8f216f1d584184e2): version 6 holds Update by s2c1-registrant
- lost update d2d114a7-dfdb-412c-afd9-fe619235b272 v3 Comment by s1c5-reviewer 's1c5-reviewer:8' via ws1.example:42989 (stress-66c1991ed46c4ac2885816a211cd52f5): version 3 holds Comment by s2c2-reviewer
- lost update d2d114a7-dfdb-412c-afd9-fe619235b272 v9 Comment by s1c5-reviewer 's1c5-reviewer:32' via ws1.example:42989 (stress-98a01422e5af492a8f9b478e9af25dcc): version 9 holds Update by s2c1-registrant
- lost update d2d114a7-dfdb-412c-afd9-fe619235b272 v10 Comment by s1c5-reviewer 's1c5-reviewer:35' via ws1.example:42989 (stress-ba2bc141f4274284a123ccff9eddcc9d): version 10 holds Update by s2c1-registrant
- lost update 8fb45117-dd5d-4135-a067-d42fc7ef9acf v3 Comment by s1c2-reviewer 's1c2-reviewer:7' via ws1.example:42989 (stress-6c43e1243a81442c8251760ae75f63a7): version 3 holds Update by s2c1-registrant
- lost update 8fb45117-dd5d-4135-a067-d42fc7ef9acf v7 Comment by s1c2-reviewer 's1c2-reviewer:28' via ws1.example:42989 (stress-893cb63304204929893c054027d1eaaf): version 7 holds Comment by s2c2-reviewer
- lost update 8fb45117-dd5d-4135-a067-d42fc7ef9acf v9 Comment by s1c2-reviewer 's1c2-reviewer:32' via ws1.example:42989 (stress-b7986360bd9244939435e499dde6e1d9): version 9 holds Comment by s2c4-admin
- lost update 8fb45117-dd5d-4135-a067-d42fc7ef9acf v10 Comment by s1c2-reviewer 's1c2-reviewer:35' via ws1.example:42989 (stress-a37f7f6b6eb3452eb7a5d90f89e4abcc): version 10 holds Comment by s2c4-admin
- lost update 8fb45117-dd5d-4135-a067-d42fc7ef9acf v11 Comment by s1c2-reviewer 's1c2-reviewer:36' via ws1.example:42989 (stress-e6445f917b9c4710ae41e68c26385670): version 11 holds Comment by s2c5-reviewer
- lost update 8fb45117-dd5d-4135-a067-d42fc7ef9acf v4 Comment by s1c4-admin 's1c4-admin:9' via ws1.example:42989 (stress-d4a38235f0ed48e190f8741f3041284c): version 4 holds Update by s2c1-registrant
- lost update 8fb45117-dd5d-4135-a067-d42fc7ef9acf v5 Submit by s1c4-admin via ws1.example:42989 (stress-bf95456e967d492699014c09ad01e55f): version 5 holds Reject by s2c5-reviewer
- lost update 8fb45117-dd5d-4135-a067-d42fc7ef9acf v8 Comment by s1c4-admin 's1c4-admin:27' via ws1.example:42989 (stress-ece8fe903c4b458dad5c56a724aa9ef7): version 8 holds Comment by s2c5-reviewer
- lost update 8fb45117-dd5d-4135-a067-d42fc7ef9acf v2 Comment by s1c5-reviewer 's1c5-reviewer:3' via ws1.example:42989 (stress-038070f9f2ad4070a824b74d5281ffa1): version 2 holds Submit by s2c1-registrant

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| Sqlite | 2 | 117 | 98.9 | 58 | 16 | 83 | 0 | 0 | 125.8 | no |
