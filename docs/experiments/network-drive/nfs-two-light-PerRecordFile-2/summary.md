# Stress run: PerRecordFile, 3 server(s): FAILED

- Commit `1b49659b6d0d95e1a10858057ef4082c27040edb`, 2026-10-17 22:19:30 UTC, 5.3 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 5 clients, 6 records, 40 changes per client, seed 2
- Clients by role: 6 Registrant, 6 Reviewer, 3 Admin
- Data folder `/mnt/share/runs/nfs-two-light-PerRecordFile-2/data`; run folder `stress-runs/nfs/nfs-two-light-PerRecordFile-2`
- 1 of the servers on `user@mac.example`, data folder `/Volumes/share/runs/nfs-two-light-PerRecordFile-2/data` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 594 | 245 | 45.8 | 201 | 119 | 0 | 22.8 | 192.4 | 312.7 |

Writes by outcome: conflict 201, http500 24, notFound 5, saved 245, wrongStatus 119. Saves by action: Create 6, Update 97, Submit 6, Approve 4, Reject 2, Comment 130. Reads: 600 (http500 10, notFound 2).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | FAIL | 6/6 | 0 | 9 | 0 | 0 | 0 |
| server 42529 | FAIL | 6/6 | 0 | 9 | 0 | 0 | 0 |
| server 34643 | FAIL | 6/6 | 0 | 9 | 0 | 0 | 0 |
| server mac-host.local:43681 | FAIL | 6/6 | 0 | 9 | 0 | 0 | 0 |

storage:
- lost update 63b6871c-a340-41fb-b885-5f01b42e5d34 v16 Comment by s3c4-admin 's3c4-admin:8' via mac-host.local:43681 (stress-57dbad6633f342c8b66661643e767380): version 16 holds Comment by s3c5-reviewer
- lost update 63b6871c-a340-41fb-b885-5f01b42e5d34 v34 Comment by s3c4-admin 's3c4-admin:39' via mac-host.local:43681 (stress-662961957db14d8b855fadcd8098dfc1): version 34 holds Comment by s3c5-reviewer
- lost update 63b6871c-a340-41fb-b885-5f01b42e5d34 v27 Comment by s3c5-reviewer 's3c5-reviewer:17' via mac-host.local:43681 (stress-21ce0bcbaf024de4aee9c6feb9626538): version 27 holds Comment by s3c4-admin
- lost update ee54a616-7831-4cd9-93bd-dfd13c9c0b85 v45 Comment by s3c5-reviewer 's3c5-reviewer:20' via mac-host.local:43681 (stress-60155f99c13144f19ed65281d71bee63): version 45 holds Comment by s3c2-reviewer
- lost update b82f1bf2-b34d-4f8a-aab7-f1b447c005e9 v17 Comment by s3c2-reviewer 's3c2-reviewer:15' via mac-host.local:43681 (stress-a3da51b12c634de48f4345e0e86e7005): version 17 holds Submit by s3c1-registrant
- lost update aaaacdde-0f90-4898-94cd-4ceb4320d3c9 v45 Comment by s3c5-reviewer 's3c5-reviewer:16' via mac-host.local:43681 (stress-4ca22cbc225c4a4ca3e14039d3408aa4): version 45 holds Comment by s2c4-admin
- lost update 0f83319f-e0e4-44fe-8170-00c7588c09f1 v16 Update by s3c3-registrant 's3c3-registrant:8' via mac-host.local:43681 (stress-40d07cbc4d6749f7a08e34740edd3968): version 16 holds Update by s3c4-admin
- lost update 0f83319f-e0e4-44fe-8170-00c7588c09f1 v11 Comment by s3c4-admin 's3c4-admin:4' via mac-host.local:43681 (stress-4b21162b1bdf4f84b52bfb8ad8f6a92b): version 11 holds Submit by s3c3-registrant
- lost update 0f83319f-e0e4-44fe-8170-00c7588c09f1 v11 Comment by s3c5-reviewer 's3c5-reviewer:5' via mac-host.local:43681 (stress-8e4d052ddd974e088099634177189de7): version 11 holds Submit by s3c3-registrant

server 42529:
- lost update 63b6871c-a340-41fb-b885-5f01b42e5d34 v16 Comment by s3c4-admin 's3c4-admin:8' via mac-host.local:43681 (stress-57dbad6633f342c8b66661643e767380): version 16 holds Comment by s3c5-reviewer
- lost update 63b6871c-a340-41fb-b885-5f01b42e5d34 v34 Comment by s3c4-admin 's3c4-admin:39' via mac-host.local:43681 (stress-662961957db14d8b855fadcd8098dfc1): version 34 holds Comment by s3c5-reviewer
- lost update 63b6871c-a340-41fb-b885-5f01b42e5d34 v27 Comment by s3c5-reviewer 's3c5-reviewer:17' via mac-host.local:43681 (stress-21ce0bcbaf024de4aee9c6feb9626538): version 27 holds Comment by s3c4-admin
- lost update ee54a616-7831-4cd9-93bd-dfd13c9c0b85 v45 Comment by s3c5-reviewer 's3c5-reviewer:20' via mac-host.local:43681 (stress-60155f99c13144f19ed65281d71bee63): version 45 holds Comment by s3c2-reviewer
- lost update b82f1bf2-b34d-4f8a-aab7-f1b447c005e9 v17 Comment by s3c2-reviewer 's3c2-reviewer:15' via mac-host.local:43681 (stress-a3da51b12c634de48f4345e0e86e7005): version 17 holds Submit by s3c1-registrant
- lost update aaaacdde-0f90-4898-94cd-4ceb4320d3c9 v45 Comment by s3c5-reviewer 's3c5-reviewer:16' via mac-host.local:43681 (stress-4ca22cbc225c4a4ca3e14039d3408aa4): version 45 holds Comment by s2c4-admin
- lost update 0f83319f-e0e4-44fe-8170-00c7588c09f1 v16 Update by s3c3-registrant 's3c3-registrant:8' via mac-host.local:43681 (stress-40d07cbc4d6749f7a08e34740edd3968): version 16 holds Update by s3c4-admin
- lost update 0f83319f-e0e4-44fe-8170-00c7588c09f1 v11 Comment by s3c4-admin 's3c4-admin:4' via mac-host.local:43681 (stress-4b21162b1bdf4f84b52bfb8ad8f6a92b): version 11 holds Submit by s3c3-registrant
- lost update 0f83319f-e0e4-44fe-8170-00c7588c09f1 v11 Comment by s3c5-reviewer 's3c5-reviewer:5' via mac-host.local:43681 (stress-8e4d052ddd974e088099634177189de7): version 11 holds Submit by s3c3-registrant

server 34643:
- lost update 63b6871c-a340-41fb-b885-5f01b42e5d34 v16 Comment by s3c4-admin 's3c4-admin:8' via mac-host.local:43681 (stress-57dbad6633f342c8b66661643e767380): version 16 holds Comment by s3c5-reviewer
- lost update 63b6871c-a340-41fb-b885-5f01b42e5d34 v34 Comment by s3c4-admin 's3c4-admin:39' via mac-host.local:43681 (stress-662961957db14d8b855fadcd8098dfc1): version 34 holds Comment by s3c5-reviewer
- lost update 63b6871c-a340-41fb-b885-5f01b42e5d34 v27 Comment by s3c5-reviewer 's3c5-reviewer:17' via mac-host.local:43681 (stress-21ce0bcbaf024de4aee9c6feb9626538): version 27 holds Comment by s3c4-admin
- lost update ee54a616-7831-4cd9-93bd-dfd13c9c0b85 v45 Comment by s3c5-reviewer 's3c5-reviewer:20' via mac-host.local:43681 (stress-60155f99c13144f19ed65281d71bee63): version 45 holds Comment by s3c2-reviewer
- lost update b82f1bf2-b34d-4f8a-aab7-f1b447c005e9 v17 Comment by s3c2-reviewer 's3c2-reviewer:15' via mac-host.local:43681 (stress-a3da51b12c634de48f4345e0e86e7005): version 17 holds Submit by s3c1-registrant
- lost update aaaacdde-0f90-4898-94cd-4ceb4320d3c9 v45 Comment by s3c5-reviewer 's3c5-reviewer:16' via mac-host.local:43681 (stress-4ca22cbc225c4a4ca3e14039d3408aa4): version 45 holds Comment by s2c4-admin
- lost update 0f83319f-e0e4-44fe-8170-00c7588c09f1 v16 Update by s3c3-registrant 's3c3-registrant:8' via mac-host.local:43681 (stress-40d07cbc4d6749f7a08e34740edd3968): version 16 holds Update by s3c4-admin
- lost update 0f83319f-e0e4-44fe-8170-00c7588c09f1 v11 Comment by s3c4-admin 's3c4-admin:4' via mac-host.local:43681 (stress-4b21162b1bdf4f84b52bfb8ad8f6a92b): version 11 holds Submit by s3c3-registrant
- lost update 0f83319f-e0e4-44fe-8170-00c7588c09f1 v11 Comment by s3c5-reviewer 's3c5-reviewer:5' via mac-host.local:43681 (stress-8e4d052ddd974e088099634177189de7): version 11 holds Submit by s3c3-registrant

server mac-host.local:43681:
- lost update 63b6871c-a340-41fb-b885-5f01b42e5d34 v16 Comment by s3c4-admin 's3c4-admin:8' via mac-host.local:43681 (stress-57dbad6633f342c8b66661643e767380): version 16 holds Comment by s3c5-reviewer
- lost update 63b6871c-a340-41fb-b885-5f01b42e5d34 v34 Comment by s3c4-admin 's3c4-admin:39' via mac-host.local:43681 (stress-662961957db14d8b855fadcd8098dfc1): version 34 holds Comment by s3c5-reviewer
- lost update 63b6871c-a340-41fb-b885-5f01b42e5d34 v27 Comment by s3c5-reviewer 's3c5-reviewer:17' via mac-host.local:43681 (stress-21ce0bcbaf024de4aee9c6feb9626538): version 27 holds Comment by s3c4-admin
- lost update ee54a616-7831-4cd9-93bd-dfd13c9c0b85 v45 Comment by s3c5-reviewer 's3c5-reviewer:20' via mac-host.local:43681 (stress-60155f99c13144f19ed65281d71bee63): version 45 holds Comment by s3c2-reviewer
- lost update b82f1bf2-b34d-4f8a-aab7-f1b447c005e9 v17 Comment by s3c2-reviewer 's3c2-reviewer:15' via mac-host.local:43681 (stress-a3da51b12c634de48f4345e0e86e7005): version 17 holds Submit by s3c1-registrant
- lost update aaaacdde-0f90-4898-94cd-4ceb4320d3c9 v45 Comment by s3c5-reviewer 's3c5-reviewer:16' via mac-host.local:43681 (stress-4ca22cbc225c4a4ca3e14039d3408aa4): version 45 holds Comment by s2c4-admin
- lost update 0f83319f-e0e4-44fe-8170-00c7588c09f1 v16 Update by s3c3-registrant 's3c3-registrant:8' via mac-host.local:43681 (stress-40d07cbc4d6749f7a08e34740edd3968): version 16 holds Update by s3c4-admin
- lost update 0f83319f-e0e4-44fe-8170-00c7588c09f1 v11 Comment by s3c4-admin 's3c4-admin:4' via mac-host.local:43681 (stress-4b21162b1bdf4f84b52bfb8ad8f6a92b): version 11 holds Submit by s3c3-registrant
- lost update 0f83319f-e0e4-44fe-8170-00c7588c09f1 v11 Comment by s3c5-reviewer 's3c5-reviewer:5' via mac-host.local:43681 (stress-8e4d052ddd974e088099634177189de7): version 11 holds Submit by s3c3-registrant

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 3 | 245 | 45.8 | 201 | 119 | 9 | 0 | 0 | 192.4 | no |
