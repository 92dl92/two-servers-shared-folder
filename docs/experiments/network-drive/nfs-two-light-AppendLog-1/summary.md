# Stress run: AppendLog, 3 server(s): FAILED

- Commit `1b49659b6d0d95e1a10858057ef4082c27040edb`, 2026-10-17 22:19:50 UTC, 8.4 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 5 clients, 6 records, 40 changes per client, seed 1
- Clients by role: 6 Registrant, 6 Reviewer, 3 Admin
- Data folder `/mnt/share/runs/nfs-two-light-AppendLog-1/data`; run folder `stress-runs/nfs/nfs-two-light-AppendLog-1`
- 1 of the servers on `user@mac.example`, data folder `/Volumes/share/runs/nfs-two-light-AppendLog-1/data` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 539 | 260 | 30.9 | 158 | 120 | 0 | 124.4 | 328.3 | 469.5 |

Writes by outcome: conflict 158, http500 1, saved 260, wrongStatus 120. Saves by action: Create 6, Update 108, Submit 8, Approve 6, Reject 2, Comment 130. Reads: 600 (notFound 67).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | FAIL | 5/6 | 1 | 138 | 0 | 0 | 0 |
| server 36441 | FAIL | 5/6 | 1 | 107 | 0 | 0 | 0 |
| server 38679 | FAIL | 5/6 | 1 | 100 | 0 | 0 | 0 |
| server mac-host.local:43243 | FAIL | 5/6 | 1 | 138 | 0 | 0 | 0 |

storage:
- missing record e508f0c6-b982-46fe-873a-eb0306c19d92: missing (32 acknowledged changes)
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v12 Update by s1c1-registrant 's1c1-registrant:10' via 36441 (stress-69b1a4254577491091e7f5ed537d6ff2): version 12 holds Comment by s3c5-reviewer
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v13 Update by s1c1-registrant 's1c1-registrant:11' via 36441 (stress-a97d2ba797b5493486b41b5b7bd1056a): version 13 holds Comment by s3c5-reviewer
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v14 Update by s1c1-registrant 's1c1-registrant:12' via 36441 (stress-435dacc7ba2d4e7498cbd6f37936ccd0): version 14 holds Comment by s3c2-reviewer
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v15 Update by s1c1-registrant 's1c1-registrant:13' via 36441 (stress-21e00fb9d9b24f6eaa3f2ca74826aec5): version 15 holds Update by s3c4-admin
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v16 Update by s1c1-registrant 's1c1-registrant:14' via 36441 (stress-52de1e22dfac468b9480343d605edd7c): version 16 holds Comment by s3c4-admin
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v17 Update by s1c1-registrant 's1c1-registrant:15' via 36441 (stress-46f4ca58c0ae4317a6b602d347c52abc): version 17 holds Update by s3c4-admin
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v18 Update by s1c1-registrant 's1c1-registrant:16' via 36441 (stress-a45aa470546844c9ad8c153aef39682d): version 18 holds Approve by s3c2-reviewer
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v19 Update by s1c1-registrant 's1c1-registrant:17' via 36441 (stress-10587253979a4379b943c5c0cf3a285e): version 19 holds Comment by s3c2-reviewer
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v20 Update by s1c1-registrant 's1c1-registrant:18' via 36441 (stress-156f6ab093154027a9c4d207e18bc5b2): version 20 holds Comment by s3c5-reviewer
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v22 Update by s1c1-registrant 's1c1-registrant:20' via 36441 (stress-dc18213592c14ff78b3d6b35eb597227): version 22 holds Comment by s3c5-reviewer
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v23 Approve by s1c2-reviewer via 36441 (stress-9cd3f8d90c2348e297803a24597f8405): version 23 holds Comment by s3c5-reviewer
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v29 Comment by s1c4-admin 's1c4-admin:35' via 36441 (stress-1e4de432812749f190d40b3150c09a28): no history entry at that version
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v30 Comment by s1c4-admin 's1c4-admin:36' via 36441 (stress-d50d4e34ccf44cbc94eef521e0c5364f): no history entry at that version
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v31 Comment by s1c4-admin 's1c4-admin:38' via 36441 (stress-382bde83cf7249f488e67ad5446b5885): no history entry at that version
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v25 Comment by s1c5-reviewer 's1c5-reviewer:28' via 36441 (stress-a79c944a1a9b40fa86b2e19d2bbe1252): version 25 holds Comment by s3c2-reviewer
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v26 Comment by s1c5-reviewer 's1c5-reviewer:30' via 36441 (stress-9d6731725909442093fc81c4276f2b43): version 26 holds Comment by s3c5-reviewer
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v27 Comment by s1c5-reviewer 's1c5-reviewer:31' via 36441 (stress-516fc2ff9aaa4ae1b6ab6310a884b016): no history entry at that version
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v2 Comment by s2c2-reviewer 's2c2-reviewer:1' via 38679 (stress-f91b3cb380ad49afafe9cece0593ba00): version 2 holds Comment by s3c4-admin
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v3 Comment by s2c2-reviewer 's2c2-reviewer:2' via 38679 (stress-eb7ccb93853543f39ddd355a5942899c): version 3 holds Submit by s1c4-admin
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v11 Comment by s2c2-reviewer 's2c2-reviewer:9' via 38679 (stress-ce3989b3ff9f446cbfccb0d81d8de02c): version 11 holds Comment by s3c4-admin

server 36441:
- missing record e508f0c6-b982-46fe-873a-eb0306c19d92: missing (32 acknowledged changes)
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v2 Comment by s2c2-reviewer 's2c2-reviewer:1' via 38679 (stress-f91b3cb380ad49afafe9cece0593ba00): version 2 holds Comment by s3c4-admin
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v3 Comment by s2c2-reviewer 's2c2-reviewer:2' via 38679 (stress-eb7ccb93853543f39ddd355a5942899c): version 3 holds Submit by s1c4-admin
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v14 Comment by s3c2-reviewer 's3c2-reviewer:17' via mac-host.local:43243 (stress-f2d256779db742668b99dd4ea8a18756): version 14 holds Update by s1c1-registrant
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v18 Approve by s3c2-reviewer via mac-host.local:43243 (stress-d75a9db8f9ed48e1a26dbffbd6c97957): version 18 holds Update by s1c1-registrant
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v19 Comment by s3c2-reviewer 's3c2-reviewer:26' via mac-host.local:43243 (stress-8048b7c06700455984db8ddfeb700343): version 19 holds Update by s1c1-registrant
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v25 Comment by s3c2-reviewer 's3c2-reviewer:37' via mac-host.local:43243 (stress-6e40a5b23ebf48dc987a7ea2e9e09195): version 25 holds Comment by s1c5-reviewer
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v11 Comment by s3c4-admin 's3c4-admin:8' via mac-host.local:43243 (stress-f22fdd115902448bb7f383cbf8fbab40): version 11 holds Comment by s2c2-reviewer
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v15 Update by s3c4-admin 's3c4-admin:16' via mac-host.local:43243 (stress-e00959dee1b1469687e0f969774d879e): version 15 holds Update by s1c1-registrant
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v16 Comment by s3c4-admin 's3c4-admin:17' via mac-host.local:43243 (stress-f279b90d1ac1463c9558e355d0bc78b9): version 16 holds Update by s1c1-registrant
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v17 Update by s3c4-admin 's3c4-admin:19' via mac-host.local:43243 (stress-a39d86a4451d4b07bf3924206e1df2dc): version 17 holds Update by s1c1-registrant
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v21 Comment by s3c4-admin 's3c4-admin:29' via mac-host.local:43243 (stress-57b16ab95ae94a8e82d9bbb837912219): version 21 holds Comment by s2c4-admin
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v12 Comment by s3c5-reviewer 's3c5-reviewer:7' via mac-host.local:43243 (stress-063db9b7ee7b47e7a4edc515d0c101fa): version 12 holds Update by s1c1-registrant
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v13 Comment by s3c5-reviewer 's3c5-reviewer:11' via mac-host.local:43243 (stress-6880a4ea3fb64eb6a1a8f8400a59fc00): version 13 holds Update by s1c1-registrant
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v20 Comment by s3c5-reviewer 's3c5-reviewer:24' via mac-host.local:43243 (stress-8e174092ab12483a8d906c66b0223c70): version 20 holds Update by s1c1-registrant
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v22 Comment by s3c5-reviewer 's3c5-reviewer:31' via mac-host.local:43243 (stress-80c3dd92e8874ec1b9b0ba2e7b0db620): version 22 holds Update by s1c1-registrant
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v23 Comment by s3c5-reviewer 's3c5-reviewer:32' via mac-host.local:43243 (stress-507ff4a7b3ce4ba7a54ab3d55cbcd886): version 23 holds Approve by s1c2-reviewer
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v24 Comment by s3c5-reviewer 's3c5-reviewer:34' via mac-host.local:43243 (stress-ec6f7f2bd2b94420a4cb18ac02acaf13): version 24 holds Comment by s2c5-reviewer
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v26 Comment by s3c5-reviewer 's3c5-reviewer:38' via mac-host.local:43243 (stress-17aed001e7e346d396342196d691f4ab): version 26 holds Comment by s1c5-reviewer
- lost update 63bcb9ab-c82b-4d71-8395-931c3e1a8da5 v5 Update by s1c4-admin 's1c4-admin:4' via 36441 (stress-25aeadb9e71945589ac165102eec6d88): version 5 holds Update by s2c1-registrant
- lost update 63bcb9ab-c82b-4d71-8395-931c3e1a8da5 v16 Comment by s2c2-reviewer 's2c2-reviewer:18' via 38679 (stress-15fc87a1c31a4989840373646bcabbf3): version 16 holds Comment by s1c4-admin

server 38679:
- missing record 68ce63b6-2eec-490f-8d04-0dce9f76f0d2: missing (31 acknowledged changes)
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v2 Comment by s2c2-reviewer 's2c2-reviewer:1' via 38679 (stress-f91b3cb380ad49afafe9cece0593ba00): version 2 holds Comment by s3c4-admin
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v3 Comment by s2c2-reviewer 's2c2-reviewer:2' via 38679 (stress-eb7ccb93853543f39ddd355a5942899c): version 3 holds Submit by s1c4-admin
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v14 Comment by s3c2-reviewer 's3c2-reviewer:17' via mac-host.local:43243 (stress-f2d256779db742668b99dd4ea8a18756): version 14 holds Update by s1c1-registrant
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v18 Approve by s3c2-reviewer via mac-host.local:43243 (stress-d75a9db8f9ed48e1a26dbffbd6c97957): version 18 holds Update by s1c1-registrant
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v19 Comment by s3c2-reviewer 's3c2-reviewer:26' via mac-host.local:43243 (stress-8048b7c06700455984db8ddfeb700343): version 19 holds Update by s1c1-registrant
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v25 Comment by s3c2-reviewer 's3c2-reviewer:37' via mac-host.local:43243 (stress-6e40a5b23ebf48dc987a7ea2e9e09195): version 25 holds Comment by s1c5-reviewer
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v11 Comment by s3c4-admin 's3c4-admin:8' via mac-host.local:43243 (stress-f22fdd115902448bb7f383cbf8fbab40): version 11 holds Comment by s2c2-reviewer
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v15 Update by s3c4-admin 's3c4-admin:16' via mac-host.local:43243 (stress-e00959dee1b1469687e0f969774d879e): version 15 holds Update by s1c1-registrant
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v16 Comment by s3c4-admin 's3c4-admin:17' via mac-host.local:43243 (stress-f279b90d1ac1463c9558e355d0bc78b9): version 16 holds Update by s1c1-registrant
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v17 Update by s3c4-admin 's3c4-admin:19' via mac-host.local:43243 (stress-a39d86a4451d4b07bf3924206e1df2dc): version 17 holds Update by s1c1-registrant
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v21 Comment by s3c4-admin 's3c4-admin:29' via mac-host.local:43243 (stress-57b16ab95ae94a8e82d9bbb837912219): version 21 holds Comment by s2c4-admin
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v12 Comment by s3c5-reviewer 's3c5-reviewer:7' via mac-host.local:43243 (stress-063db9b7ee7b47e7a4edc515d0c101fa): version 12 holds Update by s1c1-registrant
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v13 Comment by s3c5-reviewer 's3c5-reviewer:11' via mac-host.local:43243 (stress-6880a4ea3fb64eb6a1a8f8400a59fc00): version 13 holds Update by s1c1-registrant
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v20 Comment by s3c5-reviewer 's3c5-reviewer:24' via mac-host.local:43243 (stress-8e174092ab12483a8d906c66b0223c70): version 20 holds Update by s1c1-registrant
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v22 Comment by s3c5-reviewer 's3c5-reviewer:31' via mac-host.local:43243 (stress-80c3dd92e8874ec1b9b0ba2e7b0db620): version 22 holds Update by s1c1-registrant
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v23 Comment by s3c5-reviewer 's3c5-reviewer:32' via mac-host.local:43243 (stress-507ff4a7b3ce4ba7a54ab3d55cbcd886): version 23 holds Approve by s1c2-reviewer
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v24 Comment by s3c5-reviewer 's3c5-reviewer:34' via mac-host.local:43243 (stress-ec6f7f2bd2b94420a4cb18ac02acaf13): version 24 holds Comment by s2c5-reviewer
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v26 Comment by s3c5-reviewer 's3c5-reviewer:38' via mac-host.local:43243 (stress-17aed001e7e346d396342196d691f4ab): version 26 holds Comment by s1c5-reviewer
- lost update 63bcb9ab-c82b-4d71-8395-931c3e1a8da5 v5 Update by s1c4-admin 's1c4-admin:4' via 36441 (stress-25aeadb9e71945589ac165102eec6d88): version 5 holds Update by s2c1-registrant
- lost update 63bcb9ab-c82b-4d71-8395-931c3e1a8da5 v16 Comment by s2c2-reviewer 's2c2-reviewer:18' via 38679 (stress-15fc87a1c31a4989840373646bcabbf3): version 16 holds Comment by s1c4-admin

server mac-host.local:43243:
- missing record e508f0c6-b982-46fe-873a-eb0306c19d92: missing (32 acknowledged changes)
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v12 Update by s1c1-registrant 's1c1-registrant:10' via 36441 (stress-69b1a4254577491091e7f5ed537d6ff2): version 12 holds Comment by s3c5-reviewer
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v13 Update by s1c1-registrant 's1c1-registrant:11' via 36441 (stress-a97d2ba797b5493486b41b5b7bd1056a): version 13 holds Comment by s3c5-reviewer
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v14 Update by s1c1-registrant 's1c1-registrant:12' via 36441 (stress-435dacc7ba2d4e7498cbd6f37936ccd0): version 14 holds Comment by s3c2-reviewer
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v15 Update by s1c1-registrant 's1c1-registrant:13' via 36441 (stress-21e00fb9d9b24f6eaa3f2ca74826aec5): version 15 holds Update by s3c4-admin
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v16 Update by s1c1-registrant 's1c1-registrant:14' via 36441 (stress-52de1e22dfac468b9480343d605edd7c): version 16 holds Comment by s3c4-admin
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v17 Update by s1c1-registrant 's1c1-registrant:15' via 36441 (stress-46f4ca58c0ae4317a6b602d347c52abc): version 17 holds Update by s3c4-admin
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v18 Update by s1c1-registrant 's1c1-registrant:16' via 36441 (stress-a45aa470546844c9ad8c153aef39682d): version 18 holds Approve by s3c2-reviewer
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v19 Update by s1c1-registrant 's1c1-registrant:17' via 36441 (stress-10587253979a4379b943c5c0cf3a285e): version 19 holds Comment by s3c2-reviewer
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v20 Update by s1c1-registrant 's1c1-registrant:18' via 36441 (stress-156f6ab093154027a9c4d207e18bc5b2): version 20 holds Comment by s3c5-reviewer
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v22 Update by s1c1-registrant 's1c1-registrant:20' via 36441 (stress-dc18213592c14ff78b3d6b35eb597227): version 22 holds Comment by s3c5-reviewer
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v23 Approve by s1c2-reviewer via 36441 (stress-9cd3f8d90c2348e297803a24597f8405): version 23 holds Comment by s3c5-reviewer
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v29 Comment by s1c4-admin 's1c4-admin:35' via 36441 (stress-1e4de432812749f190d40b3150c09a28): no history entry at that version
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v30 Comment by s1c4-admin 's1c4-admin:36' via 36441 (stress-d50d4e34ccf44cbc94eef521e0c5364f): no history entry at that version
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v31 Comment by s1c4-admin 's1c4-admin:38' via 36441 (stress-382bde83cf7249f488e67ad5446b5885): no history entry at that version
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v25 Comment by s1c5-reviewer 's1c5-reviewer:28' via 36441 (stress-a79c944a1a9b40fa86b2e19d2bbe1252): version 25 holds Comment by s3c2-reviewer
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v26 Comment by s1c5-reviewer 's1c5-reviewer:30' via 36441 (stress-9d6731725909442093fc81c4276f2b43): version 26 holds Comment by s3c5-reviewer
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v27 Comment by s1c5-reviewer 's1c5-reviewer:31' via 36441 (stress-516fc2ff9aaa4ae1b6ab6310a884b016): no history entry at that version
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v2 Comment by s2c2-reviewer 's2c2-reviewer:1' via 38679 (stress-f91b3cb380ad49afafe9cece0593ba00): version 2 holds Comment by s3c4-admin
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v3 Comment by s2c2-reviewer 's2c2-reviewer:2' via 38679 (stress-eb7ccb93853543f39ddd355a5942899c): version 3 holds Submit by s1c4-admin
- lost update 513381a4-efd0-4ec2-b831-edb5a1e4e1b6 v11 Comment by s2c2-reviewer 's2c2-reviewer:9' via 38679 (stress-ce3989b3ff9f446cbfccb0d81d8de02c): version 11 holds Comment by s3c4-admin

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| AppendLog | 3 | 260 | 30.9 | 158 | 120 | 138 | 0 | 0 | 328.3 | no |
