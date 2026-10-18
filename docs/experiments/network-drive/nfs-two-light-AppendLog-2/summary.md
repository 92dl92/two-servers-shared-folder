# Stress run: AppendLog, 3 server(s): FAILED

- Commit `1b49659b6d0d95e1a10858057ef4082c27040edb`, 2026-10-17 22:20:03 UTC, 9.8 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 5 clients, 6 records, 40 changes per client, seed 2
- Clients by role: 6 Registrant, 6 Reviewer, 3 Admin
- Data folder `/mnt/share/runs/nfs-two-light-AppendLog-2/data`; run folder `stress-runs/nfs/nfs-two-light-AppendLog-2`
- 1 of the servers on `user@mac.example`, data folder `/Volumes/share/runs/nfs-two-light-AppendLog-2/data` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 606 | 289 | 29.5 | 201 | 113 | 0 | 126.3 | 367.1 | 458.8 |

Writes by outcome: conflict 201, http500 3, saved 289, wrongStatus 113. Saves by action: Create 6, Update 115, Submit 9, Approve 4, Reject 4, Comment 151. Reads: 600.

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | FAIL | 6/6 | 0 | 130 | 0 | 0 | 0 |
| server 36515 | FAIL | 6/6 | 0 | 103 | 0 | 0 | 0 |
| server 43027 | FAIL | 6/6 | 0 | 81 | 0 | 0 | 0 |
| server mac-host.local:44403 | FAIL | 6/6 | 0 | 130 | 0 | 0 | 0 |

storage:
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v3 Update by s1c1-registrant 's1c1-registrant:1' via 36515 (stress-7159d674aabe4a16af7eac51f9776107): version 3 holds Comment by s3c5-reviewer
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v4 Update by s1c1-registrant 's1c1-registrant:2' via 36515 (stress-971ac2c56fdd45db934e537c916733b0): version 4 holds Comment by s3c5-reviewer
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v5 Update by s1c1-registrant 's1c1-registrant:3' via 36515 (stress-189343e7e6f943eab57c4b1c18dcdbba): version 5 holds Comment by s3c5-reviewer
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v6 Update by s1c1-registrant 's1c1-registrant:4' via 36515 (stress-4486c354f2f74f978ced9d5b2a2bfa60): version 6 holds Comment by s3c2-reviewer
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v7 Update by s1c1-registrant 's1c1-registrant:5' via 36515 (stress-2fc7ee0169114a80a42bc564a5db69a5): version 7 holds Comment by s3c5-reviewer
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v8 Update by s1c1-registrant 's1c1-registrant:6' via 36515 (stress-9789fbbd491f49a5b735e897e2d21281): version 8 holds Comment by s3c5-reviewer
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v9 Submit by s1c1-registrant via 36515 (stress-105373a6bdb24f489ff0fcab1e4d03d3): version 9 holds Comment by s3c4-admin
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v10 Update by s1c1-registrant 's1c1-registrant:8' via 36515 (stress-d49f54d8a09a461aaec7dbdf4a48bc0c): version 10 holds Comment by s3c2-reviewer
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v11 Update by s1c1-registrant 's1c1-registrant:10' via 36515 (stress-b42ea10f7ec74ce4978bf2319a677636): version 11 holds Comment by s3c4-admin
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v12 Update by s1c1-registrant 's1c1-registrant:11' via 36515 (stress-33e84bf24f6545e5b73d62bf249e19ef): version 12 holds Comment by s3c4-admin
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v13 Update by s1c1-registrant 's1c1-registrant:12' via 36515 (stress-155e5efd8e8d418081dd4d8b5a25898b): version 13 holds Submit by s3c4-admin
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v14 Update by s1c1-registrant 's1c1-registrant:13' via 36515 (stress-ce073a78c417466f87033dfd782747ea): version 14 holds Update by s3c4-admin
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v15 Update by s1c1-registrant 's1c1-registrant:14' via 36515 (stress-880330b4e822494993561b6726a87de9): version 15 holds Comment by s3c5-reviewer
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v16 Update by s1c1-registrant 's1c1-registrant:15' via 36515 (stress-336033cdb062414b8d95baa962cc252a): version 16 holds Approve by s3c5-reviewer
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v17 Update by s1c1-registrant 's1c1-registrant:16' via 36515 (stress-510a2352998048eaa1cbc366b7fecb6f): version 17 holds Comment by s3c2-reviewer
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v18 Update by s1c1-registrant 's1c1-registrant:18' via 36515 (stress-dbc52ed993be4856b55ccee458971f46): no history entry at that version
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v19 Update by s1c1-registrant 's1c1-registrant:19' via 36515 (stress-647745e9a851442bbf51166b3fe63a23): no history entry at that version
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v20 Update by s1c1-registrant 's1c1-registrant:20' via 36515 (stress-59eeadda007e4b2d9f02b47ba46ee436): no history entry at that version
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v22 Update by s1c1-registrant 's1c1-registrant:23' via 36515 (stress-51d6c357623c4b8aa3782b1d2065fa64): no history entry at that version
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v28 Comment by s1c2-reviewer 's1c2-reviewer:34' via 36515 (stress-6d3d36de17c74a80bfc5d651890eaccc): no history entry at that version

server 36515:
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v31 Comment by s2c4-admin 's2c4-admin:34' via 43027 (stress-6f45de110ae948549beffb1639232647): no history entry at that version
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v32 Comment by s2c5-reviewer 's2c5-reviewer:36' via 43027 (stress-7a50791d0120423d970c5a4df152d642): no history entry at that version
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v33 Comment by s2c5-reviewer 's2c5-reviewer:38' via 43027 (stress-fb2b4befb2ae4163813309c8e9cb8aae): no history entry at that version
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v6 Comment by s3c2-reviewer 's3c2-reviewer:7' via mac-host.local:44403 (stress-14ececc569864961bbe3efed1fa01523): version 6 holds Update by s1c1-registrant
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v10 Comment by s3c2-reviewer 's3c2-reviewer:22' via mac-host.local:44403 (stress-a0ceb703f9a74cbc8159c3bc6672ee7c): version 10 holds Update by s1c1-registrant
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v17 Comment by s3c2-reviewer 's3c2-reviewer:38' via mac-host.local:44403 (stress-7db8d772c1ed467c88114671c1061fdb): version 17 holds Update by s1c1-registrant
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v9 Comment by s3c4-admin 's3c4-admin:15' via mac-host.local:44403 (stress-3f9a9cdf04924772ae465e335a0ed56b): version 9 holds Submit by s1c1-registrant
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v11 Comment by s3c4-admin 's3c4-admin:23' via mac-host.local:44403 (stress-75a4a10bf4304367a607d9991aef01e0): version 11 holds Update by s1c1-registrant
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v12 Comment by s3c4-admin 's3c4-admin:27' via mac-host.local:44403 (stress-8628e6cc8e734738a8f2e7bc153a27c8): version 12 holds Update by s1c1-registrant
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v13 Submit by s3c4-admin via mac-host.local:44403 (stress-914af2eba4904873b1ea4030965d9642): version 13 holds Update by s1c1-registrant
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v14 Update by s3c4-admin 's3c4-admin:31' via mac-host.local:44403 (stress-09ca0f3940bc4be3b9f990dc52b1129f): version 14 holds Update by s1c1-registrant
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v3 Comment by s3c5-reviewer 's3c5-reviewer:2' via mac-host.local:44403 (stress-ee6b353bb4d94871a6557af90ae90bfb): version 3 holds Update by s1c1-registrant
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v4 Comment by s3c5-reviewer 's3c5-reviewer:5' via mac-host.local:44403 (stress-fcec48486cc1444090c5a8df0c7d48f2): version 4 holds Update by s1c1-registrant
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v5 Comment by s3c5-reviewer 's3c5-reviewer:6' via mac-host.local:44403 (stress-011874612b2a4d85a3daa05c50902b73): version 5 holds Update by s1c1-registrant
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v7 Comment by s3c5-reviewer 's3c5-reviewer:10' via mac-host.local:44403 (stress-0c9b0d556dad4509ba21156fbd25ff59): version 7 holds Update by s1c1-registrant
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v8 Comment by s3c5-reviewer 's3c5-reviewer:13' via mac-host.local:44403 (stress-9fb487c0bc6844b18e3927dd5758098f): version 8 holds Update by s1c1-registrant
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v15 Comment by s3c5-reviewer 's3c5-reviewer:34' via mac-host.local:44403 (stress-36b7a5f437ad46e599a3e0c0662182bb): version 15 holds Update by s1c1-registrant
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v16 Approve by s3c5-reviewer via mac-host.local:44403 (stress-295509c8ac9c46dfa6712f06dd0738f9): version 16 holds Update by s1c1-registrant
- lost update a1138d9b-a8fb-473f-8ddb-cd01aefb015c v8 Comment by s3c2-reviewer 's3c2-reviewer:11' via mac-host.local:44403 (stress-4256e123e44a484fb5b4b6d5b615560d): version 8 holds Comment by s1c2-reviewer
- lost update a1138d9b-a8fb-473f-8ddb-cd01aefb015c v10 Comment by s3c2-reviewer 's3c2-reviewer:15' via mac-host.local:44403 (stress-b3216bc534e34353bb3b30ded0c133b2): version 10 holds Submit by s2c1-registrant

server 43027:
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v6 Comment by s3c2-reviewer 's3c2-reviewer:7' via mac-host.local:44403 (stress-14ececc569864961bbe3efed1fa01523): version 6 holds Update by s1c1-registrant
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v10 Comment by s3c2-reviewer 's3c2-reviewer:22' via mac-host.local:44403 (stress-a0ceb703f9a74cbc8159c3bc6672ee7c): version 10 holds Update by s1c1-registrant
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v17 Comment by s3c2-reviewer 's3c2-reviewer:38' via mac-host.local:44403 (stress-7db8d772c1ed467c88114671c1061fdb): version 17 holds Update by s1c1-registrant
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v9 Comment by s3c4-admin 's3c4-admin:15' via mac-host.local:44403 (stress-3f9a9cdf04924772ae465e335a0ed56b): version 9 holds Submit by s1c1-registrant
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v11 Comment by s3c4-admin 's3c4-admin:23' via mac-host.local:44403 (stress-75a4a10bf4304367a607d9991aef01e0): version 11 holds Update by s1c1-registrant
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v12 Comment by s3c4-admin 's3c4-admin:27' via mac-host.local:44403 (stress-8628e6cc8e734738a8f2e7bc153a27c8): version 12 holds Update by s1c1-registrant
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v13 Submit by s3c4-admin via mac-host.local:44403 (stress-914af2eba4904873b1ea4030965d9642): version 13 holds Update by s1c1-registrant
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v14 Update by s3c4-admin 's3c4-admin:31' via mac-host.local:44403 (stress-09ca0f3940bc4be3b9f990dc52b1129f): version 14 holds Update by s1c1-registrant
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v3 Comment by s3c5-reviewer 's3c5-reviewer:2' via mac-host.local:44403 (stress-ee6b353bb4d94871a6557af90ae90bfb): version 3 holds Update by s1c1-registrant
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v4 Comment by s3c5-reviewer 's3c5-reviewer:5' via mac-host.local:44403 (stress-fcec48486cc1444090c5a8df0c7d48f2): version 4 holds Update by s1c1-registrant
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v5 Comment by s3c5-reviewer 's3c5-reviewer:6' via mac-host.local:44403 (stress-011874612b2a4d85a3daa05c50902b73): version 5 holds Update by s1c1-registrant
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v7 Comment by s3c5-reviewer 's3c5-reviewer:10' via mac-host.local:44403 (stress-0c9b0d556dad4509ba21156fbd25ff59): version 7 holds Update by s1c1-registrant
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v8 Comment by s3c5-reviewer 's3c5-reviewer:13' via mac-host.local:44403 (stress-9fb487c0bc6844b18e3927dd5758098f): version 8 holds Update by s1c1-registrant
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v15 Comment by s3c5-reviewer 's3c5-reviewer:34' via mac-host.local:44403 (stress-36b7a5f437ad46e599a3e0c0662182bb): version 15 holds Update by s1c1-registrant
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v16 Approve by s3c5-reviewer via mac-host.local:44403 (stress-295509c8ac9c46dfa6712f06dd0738f9): version 16 holds Update by s1c1-registrant
- lost update a1138d9b-a8fb-473f-8ddb-cd01aefb015c v8 Comment by s3c2-reviewer 's3c2-reviewer:11' via mac-host.local:44403 (stress-4256e123e44a484fb5b4b6d5b615560d): version 8 holds Comment by s1c2-reviewer
- lost update a1138d9b-a8fb-473f-8ddb-cd01aefb015c v10 Comment by s3c2-reviewer 's3c2-reviewer:15' via mac-host.local:44403 (stress-b3216bc534e34353bb3b30ded0c133b2): version 10 holds Submit by s2c1-registrant
- lost update a1138d9b-a8fb-473f-8ddb-cd01aefb015c v13 Comment by s3c2-reviewer 's3c2-reviewer:25' via mac-host.local:44403 (stress-19a705e5b43640e4bdb0fdbfe7fb62ac): version 13 holds Comment by s1c4-admin
- lost update a1138d9b-a8fb-473f-8ddb-cd01aefb015c v16 Comment by s3c2-reviewer 's3c2-reviewer:36' via mac-host.local:44403 (stress-b62878dd9f234cb5b5673bdef59bc0a7): version 16 holds Comment by s1c2-reviewer
- lost update a1138d9b-a8fb-473f-8ddb-cd01aefb015c v15 Update by s3c4-admin 's3c4-admin:34' via mac-host.local:44403 (stress-971995dd6ad94ff184d271b05b29d2a7): version 15 holds Comment by s2c5-reviewer

server mac-host.local:44403:
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v3 Update by s1c1-registrant 's1c1-registrant:1' via 36515 (stress-7159d674aabe4a16af7eac51f9776107): version 3 holds Comment by s3c5-reviewer
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v4 Update by s1c1-registrant 's1c1-registrant:2' via 36515 (stress-971ac2c56fdd45db934e537c916733b0): version 4 holds Comment by s3c5-reviewer
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v5 Update by s1c1-registrant 's1c1-registrant:3' via 36515 (stress-189343e7e6f943eab57c4b1c18dcdbba): version 5 holds Comment by s3c5-reviewer
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v6 Update by s1c1-registrant 's1c1-registrant:4' via 36515 (stress-4486c354f2f74f978ced9d5b2a2bfa60): version 6 holds Comment by s3c2-reviewer
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v7 Update by s1c1-registrant 's1c1-registrant:5' via 36515 (stress-2fc7ee0169114a80a42bc564a5db69a5): version 7 holds Comment by s3c5-reviewer
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v8 Update by s1c1-registrant 's1c1-registrant:6' via 36515 (stress-9789fbbd491f49a5b735e897e2d21281): version 8 holds Comment by s3c5-reviewer
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v9 Submit by s1c1-registrant via 36515 (stress-105373a6bdb24f489ff0fcab1e4d03d3): version 9 holds Comment by s3c4-admin
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v10 Update by s1c1-registrant 's1c1-registrant:8' via 36515 (stress-d49f54d8a09a461aaec7dbdf4a48bc0c): version 10 holds Comment by s3c2-reviewer
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v11 Update by s1c1-registrant 's1c1-registrant:10' via 36515 (stress-b42ea10f7ec74ce4978bf2319a677636): version 11 holds Comment by s3c4-admin
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v12 Update by s1c1-registrant 's1c1-registrant:11' via 36515 (stress-33e84bf24f6545e5b73d62bf249e19ef): version 12 holds Comment by s3c4-admin
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v13 Update by s1c1-registrant 's1c1-registrant:12' via 36515 (stress-155e5efd8e8d418081dd4d8b5a25898b): version 13 holds Submit by s3c4-admin
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v14 Update by s1c1-registrant 's1c1-registrant:13' via 36515 (stress-ce073a78c417466f87033dfd782747ea): version 14 holds Update by s3c4-admin
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v15 Update by s1c1-registrant 's1c1-registrant:14' via 36515 (stress-880330b4e822494993561b6726a87de9): version 15 holds Comment by s3c5-reviewer
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v16 Update by s1c1-registrant 's1c1-registrant:15' via 36515 (stress-336033cdb062414b8d95baa962cc252a): version 16 holds Approve by s3c5-reviewer
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v17 Update by s1c1-registrant 's1c1-registrant:16' via 36515 (stress-510a2352998048eaa1cbc366b7fecb6f): version 17 holds Comment by s3c2-reviewer
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v18 Update by s1c1-registrant 's1c1-registrant:18' via 36515 (stress-dbc52ed993be4856b55ccee458971f46): no history entry at that version
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v19 Update by s1c1-registrant 's1c1-registrant:19' via 36515 (stress-647745e9a851442bbf51166b3fe63a23): no history entry at that version
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v20 Update by s1c1-registrant 's1c1-registrant:20' via 36515 (stress-59eeadda007e4b2d9f02b47ba46ee436): no history entry at that version
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v22 Update by s1c1-registrant 's1c1-registrant:23' via 36515 (stress-51d6c357623c4b8aa3782b1d2065fa64): no history entry at that version
- lost update 51f7d5b9-f761-4462-84c6-baeba949d7da v28 Comment by s1c2-reviewer 's1c2-reviewer:34' via 36515 (stress-6d3d36de17c74a80bfc5d651890eaccc): no history entry at that version

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| AppendLog | 3 | 289 | 29.5 | 201 | 113 | 130 | 0 | 0 | 367.1 | no |
