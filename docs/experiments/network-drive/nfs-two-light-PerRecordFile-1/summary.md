# Stress run: PerRecordFile, 3 server(s): FAILED

- Commit `1b49659b6d0d95e1a10858057ef4082c27040edb`, 2026-10-17 22:19:19 UTC, 5.9 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 5 clients, 6 records, 40 changes per client, seed 1
- Clients by role: 6 Registrant, 6 Reviewer, 3 Admin
- Data folder `/mnt/share/runs/nfs-two-light-PerRecordFile-1/data`; run folder `stress-runs/nfs/nfs-two-light-PerRecordFile-1`
- 1 of the servers on `user@mac.example`, data folder `/Volumes/share/runs/nfs-two-light-PerRecordFile-1/data` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 595 | 255 | 43.2 | 196 | 130 | 0 | 24.6 | 184.1 | 388.6 |

Writes by outcome: conflict 196, http500 12, notFound 2, saved 255, wrongStatus 130. Saves by action: Create 6, Update 83, Submit 6, Approve 1, Reject 5, Comment 154. Reads: 600 (http500 11).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | FAIL | 6/6 | 0 | 8 | 0 | 0 | 0 |
| server 34861 | FAIL | 6/6 | 0 | 8 | 0 | 0 | 0 |
| server 41387 | FAIL | 6/6 | 0 | 8 | 0 | 0 | 0 |
| server mac-host.local:35109 | FAIL | 6/6 | 0 | 8 | 0 | 0 | 0 |

storage:
- lost update 3b8d8ecc-50ba-4799-9a4d-858fb6dd5072 v34 Comment by s3c2-reviewer 's3c2-reviewer:23' via mac-host.local:35109 (stress-49da40c27f8a44e0b575ba02dbf51f35): version 34 holds Comment by s3c4-admin
- lost update 3b8d8ecc-50ba-4799-9a4d-858fb6dd5072 v41 Comment by s3c2-reviewer 's3c2-reviewer:33' via mac-host.local:35109 (stress-50574345ead942738307ede8ee91b3ff): version 41 holds Comment by s3c4-admin
- lost update 3b8d8ecc-50ba-4799-9a4d-858fb6dd5072 v25 Comment by s3c4-admin 's3c4-admin:11' via mac-host.local:35109 (stress-f02623d600cb483197bfe4360c817868): version 25 holds Comment by s1c2-reviewer
- lost update 3b8d8ecc-50ba-4799-9a4d-858fb6dd5072 v36 Comment by s3c4-admin 's3c4-admin:24' via mac-host.local:35109 (stress-6d29394404e643218fd184cac8f8e307): version 36 holds Comment by s3c5-reviewer
- lost update 3b8d8ecc-50ba-4799-9a4d-858fb6dd5072 v41 Comment by s3c5-reviewer 's3c5-reviewer:36' via mac-host.local:35109 (stress-ecb1b79f779e4452a950d0055159bd35): version 41 holds Comment by s3c4-admin
- lost update c1391632-bf07-4eab-92a6-f1deee7fe1be v50 Comment by s3c5-reviewer 's3c5-reviewer:31' via mac-host.local:35109 (stress-58d51838572448e68896230f27e844d2): version 50 holds Comment by s3c2-reviewer
- lost update 7dfe47c6-6173-4ea1-a167-12f10c1bc523 v9 Update by s3c1-registrant 's3c1-registrant:5' via mac-host.local:35109 (stress-43ae81dd4549437e8ed485e18e564deb): version 9 holds Reject by s3c2-reviewer
- lost update 22cadb7d-9c51-4779-9107-04aa9a403375 v18 Comment by s3c5-reviewer 's3c5-reviewer:17' via mac-host.local:35109 (stress-6189ef3dddc84bcfba3da0266c382a4f): version 18 holds Comment by s1c4-admin

server 34861:
- lost update 3b8d8ecc-50ba-4799-9a4d-858fb6dd5072 v34 Comment by s3c2-reviewer 's3c2-reviewer:23' via mac-host.local:35109 (stress-49da40c27f8a44e0b575ba02dbf51f35): version 34 holds Comment by s3c4-admin
- lost update 3b8d8ecc-50ba-4799-9a4d-858fb6dd5072 v41 Comment by s3c2-reviewer 's3c2-reviewer:33' via mac-host.local:35109 (stress-50574345ead942738307ede8ee91b3ff): version 41 holds Comment by s3c4-admin
- lost update 3b8d8ecc-50ba-4799-9a4d-858fb6dd5072 v25 Comment by s3c4-admin 's3c4-admin:11' via mac-host.local:35109 (stress-f02623d600cb483197bfe4360c817868): version 25 holds Comment by s1c2-reviewer
- lost update 3b8d8ecc-50ba-4799-9a4d-858fb6dd5072 v36 Comment by s3c4-admin 's3c4-admin:24' via mac-host.local:35109 (stress-6d29394404e643218fd184cac8f8e307): version 36 holds Comment by s3c5-reviewer
- lost update 3b8d8ecc-50ba-4799-9a4d-858fb6dd5072 v41 Comment by s3c5-reviewer 's3c5-reviewer:36' via mac-host.local:35109 (stress-ecb1b79f779e4452a950d0055159bd35): version 41 holds Comment by s3c4-admin
- lost update c1391632-bf07-4eab-92a6-f1deee7fe1be v50 Comment by s3c5-reviewer 's3c5-reviewer:31' via mac-host.local:35109 (stress-58d51838572448e68896230f27e844d2): version 50 holds Comment by s3c2-reviewer
- lost update 7dfe47c6-6173-4ea1-a167-12f10c1bc523 v9 Update by s3c1-registrant 's3c1-registrant:5' via mac-host.local:35109 (stress-43ae81dd4549437e8ed485e18e564deb): version 9 holds Reject by s3c2-reviewer
- lost update 22cadb7d-9c51-4779-9107-04aa9a403375 v18 Comment by s3c5-reviewer 's3c5-reviewer:17' via mac-host.local:35109 (stress-6189ef3dddc84bcfba3da0266c382a4f): version 18 holds Comment by s1c4-admin

server 41387:
- lost update 3b8d8ecc-50ba-4799-9a4d-858fb6dd5072 v34 Comment by s3c2-reviewer 's3c2-reviewer:23' via mac-host.local:35109 (stress-49da40c27f8a44e0b575ba02dbf51f35): version 34 holds Comment by s3c4-admin
- lost update 3b8d8ecc-50ba-4799-9a4d-858fb6dd5072 v41 Comment by s3c2-reviewer 's3c2-reviewer:33' via mac-host.local:35109 (stress-50574345ead942738307ede8ee91b3ff): version 41 holds Comment by s3c4-admin
- lost update 3b8d8ecc-50ba-4799-9a4d-858fb6dd5072 v25 Comment by s3c4-admin 's3c4-admin:11' via mac-host.local:35109 (stress-f02623d600cb483197bfe4360c817868): version 25 holds Comment by s1c2-reviewer
- lost update 3b8d8ecc-50ba-4799-9a4d-858fb6dd5072 v36 Comment by s3c4-admin 's3c4-admin:24' via mac-host.local:35109 (stress-6d29394404e643218fd184cac8f8e307): version 36 holds Comment by s3c5-reviewer
- lost update 3b8d8ecc-50ba-4799-9a4d-858fb6dd5072 v41 Comment by s3c5-reviewer 's3c5-reviewer:36' via mac-host.local:35109 (stress-ecb1b79f779e4452a950d0055159bd35): version 41 holds Comment by s3c4-admin
- lost update c1391632-bf07-4eab-92a6-f1deee7fe1be v50 Comment by s3c5-reviewer 's3c5-reviewer:31' via mac-host.local:35109 (stress-58d51838572448e68896230f27e844d2): version 50 holds Comment by s3c2-reviewer
- lost update 7dfe47c6-6173-4ea1-a167-12f10c1bc523 v9 Update by s3c1-registrant 's3c1-registrant:5' via mac-host.local:35109 (stress-43ae81dd4549437e8ed485e18e564deb): version 9 holds Reject by s3c2-reviewer
- lost update 22cadb7d-9c51-4779-9107-04aa9a403375 v18 Comment by s3c5-reviewer 's3c5-reviewer:17' via mac-host.local:35109 (stress-6189ef3dddc84bcfba3da0266c382a4f): version 18 holds Comment by s1c4-admin

server mac-host.local:35109:
- lost update 3b8d8ecc-50ba-4799-9a4d-858fb6dd5072 v34 Comment by s3c2-reviewer 's3c2-reviewer:23' via mac-host.local:35109 (stress-49da40c27f8a44e0b575ba02dbf51f35): version 34 holds Comment by s3c4-admin
- lost update 3b8d8ecc-50ba-4799-9a4d-858fb6dd5072 v41 Comment by s3c2-reviewer 's3c2-reviewer:33' via mac-host.local:35109 (stress-50574345ead942738307ede8ee91b3ff): version 41 holds Comment by s3c4-admin
- lost update 3b8d8ecc-50ba-4799-9a4d-858fb6dd5072 v25 Comment by s3c4-admin 's3c4-admin:11' via mac-host.local:35109 (stress-f02623d600cb483197bfe4360c817868): version 25 holds Comment by s1c2-reviewer
- lost update 3b8d8ecc-50ba-4799-9a4d-858fb6dd5072 v36 Comment by s3c4-admin 's3c4-admin:24' via mac-host.local:35109 (stress-6d29394404e643218fd184cac8f8e307): version 36 holds Comment by s3c5-reviewer
- lost update 3b8d8ecc-50ba-4799-9a4d-858fb6dd5072 v41 Comment by s3c5-reviewer 's3c5-reviewer:36' via mac-host.local:35109 (stress-ecb1b79f779e4452a950d0055159bd35): version 41 holds Comment by s3c4-admin
- lost update c1391632-bf07-4eab-92a6-f1deee7fe1be v50 Comment by s3c5-reviewer 's3c5-reviewer:31' via mac-host.local:35109 (stress-58d51838572448e68896230f27e844d2): version 50 holds Comment by s3c2-reviewer
- lost update 7dfe47c6-6173-4ea1-a167-12f10c1bc523 v9 Update by s3c1-registrant 's3c1-registrant:5' via mac-host.local:35109 (stress-43ae81dd4549437e8ed485e18e564deb): version 9 holds Reject by s3c2-reviewer
- lost update 22cadb7d-9c51-4779-9107-04aa9a403375 v18 Comment by s3c5-reviewer 's3c5-reviewer:17' via mac-host.local:35109 (stress-6189ef3dddc84bcfba3da0266c382a4f): version 18 holds Comment by s1c4-admin

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 3 | 255 | 43.2 | 196 | 130 | 8 | 0 | 0 | 184.1 | no |
