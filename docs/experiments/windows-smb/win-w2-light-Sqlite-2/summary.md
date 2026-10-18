# Stress run: Sqlite, 2 server(s): FAILED

- Commit `c71df76c37c18ca38a44e27c02961e268fbb1c09`, 2026-10-18 12:15:31 UTC, 1.4 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 5 clients, 6 records, 40 changes per client, seed 2
- Clients by role: 4 Registrant, 4 Reviewer, 2 Admin
- Data folder `stress-runs/win-w2-light-Sqlite-2/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-w2-light-Sqlite-2`
- 1 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-w2-light-Sqlite-2` there
- 1 of the servers on `Administrator@ws2.example`, data folder `\\ws1.example\share\runs\win-w2-light-Sqlite-2` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 202 | 111 | 77.0 | 57 | 33 | 0 | 14.4 | 86.4 | 171.7 |

Writes by outcome: conflict 57, notFound 1, saved 111, wrongStatus 33. Saves by action: Create 6, Update 39, Submit 4, Approve 2, Reject 1, Comment 59. Reads: 400 (notFound 204).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | FAIL | 3/6 | 3 | 36 | 0 | 0 | 0 |
| server ws1.example:34309 | FAIL | 3/6 | 3 | 75 | 0 | 0 | 0 |
| server ws2.example:41715 | FAIL | 3/6 | 3 | 36 | 0 | 0 | 0 |

storage:
- missing record 5b65ab82-23e9-43ce-8234-e561e9e6e11d: missing (1 acknowledged changes)
- missing record e33f8c04-30d9-4ed7-b9ca-54205c0058fc: missing (1 acknowledged changes)
- missing record fb42fb1d-d12b-4a69-a2fd-c428bf813357: missing (1 acknowledged changes)
- lost update 5b65ab82-23e9-43ce-8234-e561e9e6e11d v1 Create by s1c1-registrant via ws1.example:34309 (stress-55ce931ce0de4d0c8a9fcd982f97eaf0): record missing
- lost update e33f8c04-30d9-4ed7-b9ca-54205c0058fc v1 Create by s1c1-registrant via ws1.example:34309 (stress-99b4a1dba808415ab4fa4f4b3cabe1cb): record missing
- lost update 4fd1d1fe-30c8-4402-bdb4-1227b1f31543 v4 Comment by s1c2-reviewer 's1c2-reviewer:9' via ws1.example:34309 (stress-9c475d01ab58424f808e74b38c7f26be): version 4 holds Update by s2c1-registrant
- lost update 4fd1d1fe-30c8-4402-bdb4-1227b1f31543 v6 Comment by s1c2-reviewer 's1c2-reviewer:11' via ws1.example:34309 (stress-a00146baf3694fe2a75094965d055c83): version 6 holds Update by s2c1-registrant
- lost update 4fd1d1fe-30c8-4402-bdb4-1227b1f31543 v7 Comment by s1c2-reviewer 's1c2-reviewer:12' via ws1.example:34309 (stress-6fe7aa6ee07947dc8d02a43dff01c4ec): version 7 holds Approve by s2c2-reviewer
- lost update 4fd1d1fe-30c8-4402-bdb4-1227b1f31543 v10 Comment by s1c2-reviewer 's1c2-reviewer:18' via ws1.example:34309 (stress-07971ce7bcd1458c8258646000261d25): version 10 holds Comment by s2c2-reviewer
- lost update 4fd1d1fe-30c8-4402-bdb4-1227b1f31543 v13 Comment by s1c2-reviewer 's1c2-reviewer:36' via ws1.example:34309 (stress-d8e11ba3a7094ea3881c5c5477fc831b): version 13 holds Comment by s2c5-reviewer
- lost update 4fd1d1fe-30c8-4402-bdb4-1227b1f31543 v2 Comment by s1c5-reviewer 's1c5-reviewer:3' via ws1.example:34309 (stress-e21d1626e0bf4ce19ee89f4a53e8fcb1): version 2 holds Comment by s2c5-reviewer
- lost update 4fd1d1fe-30c8-4402-bdb4-1227b1f31543 v3 Comment by s1c5-reviewer 's1c5-reviewer:6' via ws1.example:34309 (stress-283c095a8e84496da8cb1a93ba8fc63c): version 3 holds Submit by s2c1-registrant
- lost update 4fd1d1fe-30c8-4402-bdb4-1227b1f31543 v5 Comment by s1c5-reviewer 's1c5-reviewer:11' via ws1.example:34309 (stress-d047419257e94424bd310f05155f43c7): version 5 holds Update by s2c1-registrant
- lost update 4fd1d1fe-30c8-4402-bdb4-1227b1f31543 v8 Comment by s1c5-reviewer 's1c5-reviewer:19' via ws1.example:34309 (stress-85ee8c6c549449adbd4ed8da22f403b8): version 8 holds Comment by s2c5-reviewer
- lost update 4fd1d1fe-30c8-4402-bdb4-1227b1f31543 v9 Comment by s1c5-reviewer 's1c5-reviewer:21' via ws1.example:34309 (stress-afeb9b8946ee45d685469f8257393380): version 9 holds Comment by s2c5-reviewer
- lost update 4fd1d1fe-30c8-4402-bdb4-1227b1f31543 v11 Comment by s1c5-reviewer 's1c5-reviewer:27' via ws1.example:34309 (stress-2f65b58f5bf84146b6a27cf2616c8e02): version 11 holds Comment by s2c2-reviewer
- lost update 4fd1d1fe-30c8-4402-bdb4-1227b1f31543 v12 Comment by s1c5-reviewer 's1c5-reviewer:28' via ws1.example:34309 (stress-0efafe79f44741d39e5ceb09d0b175ac): version 12 holds Comment by s2c2-reviewer
- lost update 4321421a-5fbc-4e68-b489-3e6c3fcefb63 v3 Comment by s1c2-reviewer 's1c2-reviewer:2' via ws1.example:34309 (stress-f1955782fdf849f9a5ec73f33792005c): version 3 holds Submit by s2c1-registrant
- lost update 4321421a-5fbc-4e68-b489-3e6c3fcefb63 v5 Comment by s1c2-reviewer 's1c2-reviewer:4' via ws1.example:34309 (stress-9853c9d783ce41a88a21f03870dced9a): version 5 holds Comment by s2c5-reviewer
- lost update 4321421a-5fbc-4e68-b489-3e6c3fcefb63 v9 Comment by s1c2-reviewer 's1c2-reviewer:32' via ws1.example:34309 (stress-9d8aca98a6a444379e29edfa8ec55101): version 9 holds Comment by s2c4-admin
- lost update 4321421a-5fbc-4e68-b489-3e6c3fcefb63 v11 Comment by s1c2-reviewer 's1c2-reviewer:39' via ws1.example:34309 (stress-98dd16a39af14198bd8600ab5ed706b0): version 11 holds Comment by s2c4-admin
- lost update 4321421a-5fbc-4e68-b489-3e6c3fcefb63 v2 Submit by s1c4-admin via ws1.example:34309 (stress-c73bd74478c5416ebfc06967e48ecd6e): version 2 holds Comment by s2c2-reviewer
- lost update 4321421a-5fbc-4e68-b489-3e6c3fcefb63 v7 Comment by s1c4-admin 's1c4-admin:29' via ws1.example:34309 (stress-c6dd22131b5a48138055b0d72a4e21e3): version 7 holds Comment by s2c2-reviewer

server ws1.example:34309:
- missing record 5b65ab82-23e9-43ce-8234-e561e9e6e11d: missing (1 acknowledged changes)
- missing record e33f8c04-30d9-4ed7-b9ca-54205c0058fc: missing (1 acknowledged changes)
- missing record fb42fb1d-d12b-4a69-a2fd-c428bf813357: missing (1 acknowledged changes)
- lost update 5b65ab82-23e9-43ce-8234-e561e9e6e11d v1 Create by s1c1-registrant via ws1.example:34309 (stress-55ce931ce0de4d0c8a9fcd982f97eaf0): record missing
- lost update e33f8c04-30d9-4ed7-b9ca-54205c0058fc v1 Create by s1c1-registrant via ws1.example:34309 (stress-99b4a1dba808415ab4fa4f4b3cabe1cb): record missing
- lost update 4fd1d1fe-30c8-4402-bdb4-1227b1f31543 v3 Submit by s2c1-registrant via ws2.example:41715 (stress-7213fe7be0924c11b71b26f0fc6f4649): version 3 holds Comment by s1c5-reviewer
- lost update 4fd1d1fe-30c8-4402-bdb4-1227b1f31543 v4 Update by s2c1-registrant 's2c1-registrant:2' via ws2.example:41715 (stress-c735035a33034203b104ae3077e45996): version 4 holds Comment by s1c2-reviewer
- lost update 4fd1d1fe-30c8-4402-bdb4-1227b1f31543 v5 Update by s2c1-registrant 's2c1-registrant:4' via ws2.example:41715 (stress-80510efa1cf8455599477bce4f2f027f): version 5 holds Comment by s1c5-reviewer
- lost update 4fd1d1fe-30c8-4402-bdb4-1227b1f31543 v6 Update by s2c1-registrant 's2c1-registrant:5' via ws2.example:41715 (stress-a7c2405dd56d48b4a3b5380370c5ead3): version 6 holds Comment by s1c2-reviewer
- lost update 4fd1d1fe-30c8-4402-bdb4-1227b1f31543 v7 Approve by s2c2-reviewer via ws2.example:41715 (stress-564acb639756491d879705585ab3bf13): version 7 holds Comment by s1c2-reviewer
- lost update 4fd1d1fe-30c8-4402-bdb4-1227b1f31543 v10 Comment by s2c2-reviewer 's2c2-reviewer:16' via ws2.example:41715 (stress-24797f1eb69d4cf9b2dcd5127a941b56): version 10 holds Comment by s1c2-reviewer
- lost update 4fd1d1fe-30c8-4402-bdb4-1227b1f31543 v11 Comment by s2c2-reviewer 's2c2-reviewer:19' via ws2.example:41715 (stress-78c45f40f5824650a8613c57da7d5581): version 11 holds Comment by s1c5-reviewer
- lost update 4fd1d1fe-30c8-4402-bdb4-1227b1f31543 v12 Comment by s2c2-reviewer 's2c2-reviewer:26' via ws2.example:41715 (stress-79ff77e888d84726ac81e4f05fe7e43a): version 12 holds Comment by s1c5-reviewer
- lost update 4fd1d1fe-30c8-4402-bdb4-1227b1f31543 v16 Comment by s2c2-reviewer 's2c2-reviewer:37' via ws2.example:41715 (stress-7e3c3f7756ab4326a89375f07222d766): no history entry at that version
- lost update 4fd1d1fe-30c8-4402-bdb4-1227b1f31543 v2 Comment by s2c5-reviewer 's2c5-reviewer:0' via ws2.example:41715 (stress-ee1cb436c6454fd787aa9bee6b0f718f): version 2 holds Comment by s1c5-reviewer
- lost update 4fd1d1fe-30c8-4402-bdb4-1227b1f31543 v8 Comment by s2c5-reviewer 's2c5-reviewer:13' via ws2.example:41715 (stress-85811c551f794cd5883c86a2ae24bd85): version 8 holds Comment by s1c5-reviewer
- lost update 4fd1d1fe-30c8-4402-bdb4-1227b1f31543 v9 Comment by s2c5-reviewer 's2c5-reviewer:14' via ws2.example:41715 (stress-88462426682e4a5e8b44b79ddc32d04d): version 9 holds Comment by s1c5-reviewer
- lost update 4fd1d1fe-30c8-4402-bdb4-1227b1f31543 v13 Comment by s2c5-reviewer 's2c5-reviewer:26' via ws2.example:41715 (stress-16401f1d208641b992797e3547eb6223): version 13 holds Comment by s1c2-reviewer
- lost update 4fd1d1fe-30c8-4402-bdb4-1227b1f31543 v14 Comment by s2c5-reviewer 's2c5-reviewer:28' via ws2.example:41715 (stress-e00ccd28e8c24f2290b25128954fe62a): no history entry at that version
- lost update 4fd1d1fe-30c8-4402-bdb4-1227b1f31543 v15 Comment by s2c5-reviewer 's2c5-reviewer:29' via ws2.example:41715 (stress-b841c72d29f8472e864c2f0d2d020122): no history entry at that version
- lost update 4321421a-5fbc-4e68-b489-3e6c3fcefb63 v3 Submit by s2c1-registrant via ws2.example:41715 (stress-3a575329bec44c5abd3f1f8f938b2711): version 3 holds Comment by s1c2-reviewer
- lost update 4321421a-5fbc-4e68-b489-3e6c3fcefb63 v2 Comment by s2c2-reviewer 's2c2-reviewer:3' via ws2.example:41715 (stress-b11bc5ee33b645b8a04dbffb81ca52a0): version 2 holds Submit by s1c4-admin
- lost update 4321421a-5fbc-4e68-b489-3e6c3fcefb63 v7 Comment by s2c2-reviewer 's2c2-reviewer:15' via ws2.example:41715 (stress-f8a1bfd52d1c44ee8ef099cd8a0742c5): version 7 holds Comment by s1c4-admin

server ws2.example:41715:
- missing record 5b65ab82-23e9-43ce-8234-e561e9e6e11d: missing (1 acknowledged changes)
- missing record e33f8c04-30d9-4ed7-b9ca-54205c0058fc: missing (1 acknowledged changes)
- missing record fb42fb1d-d12b-4a69-a2fd-c428bf813357: missing (1 acknowledged changes)
- lost update 5b65ab82-23e9-43ce-8234-e561e9e6e11d v1 Create by s1c1-registrant via ws1.example:34309 (stress-55ce931ce0de4d0c8a9fcd982f97eaf0): record missing
- lost update e33f8c04-30d9-4ed7-b9ca-54205c0058fc v1 Create by s1c1-registrant via ws1.example:34309 (stress-99b4a1dba808415ab4fa4f4b3cabe1cb): record missing
- lost update 4fd1d1fe-30c8-4402-bdb4-1227b1f31543 v4 Comment by s1c2-reviewer 's1c2-reviewer:9' via ws1.example:34309 (stress-9c475d01ab58424f808e74b38c7f26be): version 4 holds Update by s2c1-registrant
- lost update 4fd1d1fe-30c8-4402-bdb4-1227b1f31543 v6 Comment by s1c2-reviewer 's1c2-reviewer:11' via ws1.example:34309 (stress-a00146baf3694fe2a75094965d055c83): version 6 holds Update by s2c1-registrant
- lost update 4fd1d1fe-30c8-4402-bdb4-1227b1f31543 v7 Comment by s1c2-reviewer 's1c2-reviewer:12' via ws1.example:34309 (stress-6fe7aa6ee07947dc8d02a43dff01c4ec): version 7 holds Approve by s2c2-reviewer
- lost update 4fd1d1fe-30c8-4402-bdb4-1227b1f31543 v10 Comment by s1c2-reviewer 's1c2-reviewer:18' via ws1.example:34309 (stress-07971ce7bcd1458c8258646000261d25): version 10 holds Comment by s2c2-reviewer
- lost update 4fd1d1fe-30c8-4402-bdb4-1227b1f31543 v13 Comment by s1c2-reviewer 's1c2-reviewer:36' via ws1.example:34309 (stress-d8e11ba3a7094ea3881c5c5477fc831b): version 13 holds Comment by s2c5-reviewer
- lost update 4fd1d1fe-30c8-4402-bdb4-1227b1f31543 v2 Comment by s1c5-reviewer 's1c5-reviewer:3' via ws1.example:34309 (stress-e21d1626e0bf4ce19ee89f4a53e8fcb1): version 2 holds Comment by s2c5-reviewer
- lost update 4fd1d1fe-30c8-4402-bdb4-1227b1f31543 v3 Comment by s1c5-reviewer 's1c5-reviewer:6' via ws1.example:34309 (stress-283c095a8e84496da8cb1a93ba8fc63c): version 3 holds Submit by s2c1-registrant
- lost update 4fd1d1fe-30c8-4402-bdb4-1227b1f31543 v5 Comment by s1c5-reviewer 's1c5-reviewer:11' via ws1.example:34309 (stress-d047419257e94424bd310f05155f43c7): version 5 holds Update by s2c1-registrant
- lost update 4fd1d1fe-30c8-4402-bdb4-1227b1f31543 v8 Comment by s1c5-reviewer 's1c5-reviewer:19' via ws1.example:34309 (stress-85ee8c6c549449adbd4ed8da22f403b8): version 8 holds Comment by s2c5-reviewer
- lost update 4fd1d1fe-30c8-4402-bdb4-1227b1f31543 v9 Comment by s1c5-reviewer 's1c5-reviewer:21' via ws1.example:34309 (stress-afeb9b8946ee45d685469f8257393380): version 9 holds Comment by s2c5-reviewer
- lost update 4fd1d1fe-30c8-4402-bdb4-1227b1f31543 v11 Comment by s1c5-reviewer 's1c5-reviewer:27' via ws1.example:34309 (stress-2f65b58f5bf84146b6a27cf2616c8e02): version 11 holds Comment by s2c2-reviewer
- lost update 4fd1d1fe-30c8-4402-bdb4-1227b1f31543 v12 Comment by s1c5-reviewer 's1c5-reviewer:28' via ws1.example:34309 (stress-0efafe79f44741d39e5ceb09d0b175ac): version 12 holds Comment by s2c2-reviewer
- lost update 4321421a-5fbc-4e68-b489-3e6c3fcefb63 v3 Comment by s1c2-reviewer 's1c2-reviewer:2' via ws1.example:34309 (stress-f1955782fdf849f9a5ec73f33792005c): version 3 holds Submit by s2c1-registrant
- lost update 4321421a-5fbc-4e68-b489-3e6c3fcefb63 v5 Comment by s1c2-reviewer 's1c2-reviewer:4' via ws1.example:34309 (stress-9853c9d783ce41a88a21f03870dced9a): version 5 holds Comment by s2c5-reviewer
- lost update 4321421a-5fbc-4e68-b489-3e6c3fcefb63 v9 Comment by s1c2-reviewer 's1c2-reviewer:32' via ws1.example:34309 (stress-9d8aca98a6a444379e29edfa8ec55101): version 9 holds Comment by s2c4-admin
- lost update 4321421a-5fbc-4e68-b489-3e6c3fcefb63 v11 Comment by s1c2-reviewer 's1c2-reviewer:39' via ws1.example:34309 (stress-98dd16a39af14198bd8600ab5ed706b0): version 11 holds Comment by s2c4-admin
- lost update 4321421a-5fbc-4e68-b489-3e6c3fcefb63 v2 Submit by s1c4-admin via ws1.example:34309 (stress-c73bd74478c5416ebfc06967e48ecd6e): version 2 holds Comment by s2c2-reviewer
- lost update 4321421a-5fbc-4e68-b489-3e6c3fcefb63 v7 Comment by s1c4-admin 's1c4-admin:29' via ws1.example:34309 (stress-c6dd22131b5a48138055b0d72a4e21e3): version 7 holds Comment by s2c2-reviewer

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| Sqlite | 2 | 111 | 77.0 | 57 | 33 | 75 | 0 | 0 | 86.4 | no |
