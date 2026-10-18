# Stress run: AppendLog, 3 server(s): FAILED

- Commit `1b49659b6d0d95e1a10858057ef4082c27040edb`, 2026-10-17 22:37:10 UTC, 45.3 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 12 records, 200 changes per client, seed 2
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `/mnt/share/runs/nfs-two-heavy-AppendLog-2/data`; run folder `stress-runs/nfs/nfs-two-heavy-AppendLog-2`
- 1 of the servers on `user@mac.example`, data folder `/Volumes/share/runs/nfs-two-heavy-AppendLog-2/data` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4624 | 1769 | 39.1 | 768 | 2084 | 0 | 143.7 | 416.9 | 829.1 |

Writes by outcome: conflict 768, http500 3, saved 1769, wrongStatus 2084. Saves by action: Create 12, Update 360, Submit 20, Approve 8, Reject 11, Comment 1358. Reads: 4800 (notFound 188).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | FAIL | 11/12 | 1 | 1074 | 0 | 0 | 0 |
| server 36917 | FAIL | 11/12 | 1 | 952 | 0 | 0 | 0 |
| server 34913 | FAIL | 11/12 | 1 | 740 | 0 | 0 | 0 |
| server mac-host.local:34289 | FAIL | 11/12 | 1 | 1074 | 0 | 0 | 0 |

storage:
- missing record 4b4616dc-4231-4405-b284-46e2bd8eaa4e: missing (67 acknowledged changes)
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v20 Comment by s1c2-reviewer 's1c2-reviewer:38' via 36917 (stress-a5a8dfa7e3db469b96d93ac181ced10f): version 20 holds Comment by s3c7-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v40 Comment by s1c2-reviewer 's1c2-reviewer:97' via 36917 (stress-6473de4ebca447058a4ba20eebdf5872): version 40 holds Comment by s3c2-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v48 Comment by s1c2-reviewer 's1c2-reviewer:123' via 36917 (stress-cb3eeda8eb0f410b819dcf5f74a36f7f): version 48 holds Comment by s3c5-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v49 Comment by s1c2-reviewer 's1c2-reviewer:127' via 36917 (stress-1e3f94db5d02411eb933fe50f8702f66): version 49 holds Comment by s3c4-admin
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v50 Comment by s1c2-reviewer 's1c2-reviewer:128' via 36917 (stress-ecd29f9c7b4b4d08bec8badad123e7a7): version 50 holds Comment by s3c5-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v61 Comment by s1c2-reviewer 's1c2-reviewer:161' via 36917 (stress-8a307eb2f8ab4272b1562d6801f12b57): version 61 holds Comment by s3c5-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v62 Comment by s1c2-reviewer 's1c2-reviewer:164' via 36917 (stress-323ee9dca7f7466981c1a639d97148bf): version 62 holds Comment by s3c2-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v18 Comment by s1c4-admin 's1c4-admin:34' via 36917 (stress-a50049479afc40ee9470cff76b2eec19): version 18 holds Comment by s3c7-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v22 Comment by s1c4-admin 's1c4-admin:43' via 36917 (stress-e8c18895a8264baeae9aec3241f712ba): version 22 holds Comment by s3c2-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v23 Comment by s1c4-admin 's1c4-admin:44' via 36917 (stress-dcbef294c1f345298248293a277a04d9): version 23 holds Comment by s3c7-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v24 Comment by s1c4-admin 's1c4-admin:49' via 36917 (stress-0493be198e7f42308604252e06ccefe7): version 24 holds Comment by s3c4-admin
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v31 Comment by s1c4-admin 's1c4-admin:62' via 36917 (stress-15b29eb3c363480fa4861409886e5322): version 31 holds Comment by s3c5-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v51 Comment by s1c4-admin 's1c4-admin:134' via 36917 (stress-8f8119b742cf4ab682505d39d6791bb3): version 51 holds Comment by s3c5-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v57 Comment by s1c4-admin 's1c4-admin:151' via 36917 (stress-c092e3ad682d481893bea891818bb8a6): version 57 holds Comment by s3c5-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v60 Comment by s1c4-admin 's1c4-admin:158' via 36917 (stress-5ff36ac82f644c2382412619ec96e76a): version 60 holds Comment by s3c4-admin
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v63 Comment by s1c4-admin 's1c4-admin:170' via 36917 (stress-72c04390e312453189265c580335f34f): version 63 holds Comment by s3c4-admin
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v68 Comment by s1c4-admin 's1c4-admin:190' via 36917 (stress-5ad79f679a414c15bdcb27a1188df2c1): version 68 holds Comment by s3c5-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v69 Comment by s1c4-admin 's1c4-admin:195' via 36917 (stress-af5e4d3040f94e89b5eaca1fdff4f90f): version 69 holds Comment by s3c7-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v29 Comment by s1c5-reviewer 's1c5-reviewer:53' via 36917 (stress-facf9828edb6499fb490724533ae93a4): version 29 holds Comment by s3c2-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v30 Comment by s1c5-reviewer 's1c5-reviewer:55' via 36917 (stress-e57fa10986554e98a25c26f66449856e): version 30 holds Comment by s3c2-reviewer

server 36917:
- missing record 4b4616dc-4231-4405-b284-46e2bd8eaa4e: missing (67 acknowledged changes)
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v29 Comment by s2c2-reviewer 's2c2-reviewer:39' via 34913 (stress-adf3e82d3bd3493c9d1bdb855aa73d8a): version 29 holds Comment by s1c5-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v28 Comment by s2c4-admin 's2c4-admin:40' via 34913 (stress-b7cd60be5d7f4dba8a116d0858f16fe6): version 28 holds Comment by s1c7-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v78 Comment by s2c4-admin 's2c4-admin:189' via 34913 (stress-ca18c9b4e05840afb9fcdc00763f628e): no history entry at that version
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v79 Comment by s2c4-admin 's2c4-admin:199' via 34913 (stress-837745a586f74374b5e881f890fa2810): no history entry at that version
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v2 Comment by s2c5-reviewer 's2c5-reviewer:0' via 34913 (stress-af371df3efda4f739e1b2ad810a4f0ef): version 2 holds Update by s1c1-registrant
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v30 Comment by s2c5-reviewer 's2c5-reviewer:40' via 34913 (stress-e874292a39834a2ab5b89b1f2170fa0c): version 30 holds Comment by s1c5-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v40 Comment by s2c5-reviewer 's2c5-reviewer:68' via 34913 (stress-ce0b5670db95405490dd91f90780cfac): version 40 holds Comment by s1c2-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v80 Comment by s2c5-reviewer 's2c5-reviewer:193' via 34913 (stress-ba5dcac9b625474fa3d9a716ec70b357): no history entry at that version
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v81 Comment by s2c5-reviewer 's2c5-reviewer:195' via 34913 (stress-06ee868d963f49e3a28f77f2c0ceda73): no history entry at that version
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v4 Comment by s2c7-reviewer 's2c7-reviewer:2' via 34913 (stress-bca860df4b0a4c73831f6350565470df): version 4 holds Update by s1c1-registrant
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v12 Comment by s2c7-reviewer 's2c7-reviewer:13' via 34913 (stress-4ef6395d7a4142f7b5ac70a6c61a973a): version 12 holds Comment by s1c5-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v77 Comment by s2c7-reviewer 's2c7-reviewer:179' via 34913 (stress-10a510e6c18746a3977a6c5917840531): no history entry at that version
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v19 Comment by s3c2-reviewer 's3c2-reviewer:16' via mac-host.local:34289 (stress-7532820a3b8c4ff38f83a7643c1d57d5): version 19 holds Comment by s1c7-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v22 Comment by s3c2-reviewer 's3c2-reviewer:27' via mac-host.local:34289 (stress-43dde85999894d2393a6c0c79d5b8f96): version 22 holds Comment by s1c4-admin
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v29 Comment by s3c2-reviewer 's3c2-reviewer:74' via mac-host.local:34289 (stress-b5316ff925f44ab18e3e839f92979201): version 29 holds Comment by s1c5-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v30 Comment by s3c2-reviewer 's3c2-reviewer:81' via mac-host.local:34289 (stress-709fa9043b624d6d892e61776d04f77e): version 30 holds Comment by s1c5-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v33 Comment by s3c2-reviewer 's3c2-reviewer:96' via mac-host.local:34289 (stress-83024095cf00476c8584a0c7e3216bf9): version 33 holds Comment by s2c7-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v39 Comment by s3c2-reviewer 's3c2-reviewer:109' via mac-host.local:34289 (stress-053cc9e6a66d4819a03f59bf0c699f19): version 39 holds Comment by s1c5-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v40 Comment by s3c2-reviewer 's3c2-reviewer:121' via mac-host.local:34289 (stress-f8c554eeba2645afa6e6210a667d75bc): version 40 holds Comment by s1c2-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v41 Comment by s3c2-reviewer 's3c2-reviewer:124' via mac-host.local:34289 (stress-5fd9319ab0eb4863a2124660b18b8865): version 41 holds Comment by s1c5-reviewer

server 34913:
- missing record fb260806-f9ce-4484-ad4f-52752609b831: missing (92 acknowledged changes)
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v29 Comment by s2c2-reviewer 's2c2-reviewer:39' via 34913 (stress-adf3e82d3bd3493c9d1bdb855aa73d8a): version 29 holds Comment by s1c5-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v28 Comment by s2c4-admin 's2c4-admin:40' via 34913 (stress-b7cd60be5d7f4dba8a116d0858f16fe6): version 28 holds Comment by s1c7-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v2 Comment by s2c5-reviewer 's2c5-reviewer:0' via 34913 (stress-af371df3efda4f739e1b2ad810a4f0ef): version 2 holds Update by s1c1-registrant
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v30 Comment by s2c5-reviewer 's2c5-reviewer:40' via 34913 (stress-e874292a39834a2ab5b89b1f2170fa0c): version 30 holds Comment by s1c5-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v40 Comment by s2c5-reviewer 's2c5-reviewer:68' via 34913 (stress-ce0b5670db95405490dd91f90780cfac): version 40 holds Comment by s1c2-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v4 Comment by s2c7-reviewer 's2c7-reviewer:2' via 34913 (stress-bca860df4b0a4c73831f6350565470df): version 4 holds Update by s1c1-registrant
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v12 Comment by s2c7-reviewer 's2c7-reviewer:13' via 34913 (stress-4ef6395d7a4142f7b5ac70a6c61a973a): version 12 holds Comment by s1c5-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v19 Comment by s3c2-reviewer 's3c2-reviewer:16' via mac-host.local:34289 (stress-7532820a3b8c4ff38f83a7643c1d57d5): version 19 holds Comment by s1c7-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v22 Comment by s3c2-reviewer 's3c2-reviewer:27' via mac-host.local:34289 (stress-43dde85999894d2393a6c0c79d5b8f96): version 22 holds Comment by s1c4-admin
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v29 Comment by s3c2-reviewer 's3c2-reviewer:74' via mac-host.local:34289 (stress-b5316ff925f44ab18e3e839f92979201): version 29 holds Comment by s1c5-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v30 Comment by s3c2-reviewer 's3c2-reviewer:81' via mac-host.local:34289 (stress-709fa9043b624d6d892e61776d04f77e): version 30 holds Comment by s1c5-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v33 Comment by s3c2-reviewer 's3c2-reviewer:96' via mac-host.local:34289 (stress-83024095cf00476c8584a0c7e3216bf9): version 33 holds Comment by s2c7-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v39 Comment by s3c2-reviewer 's3c2-reviewer:109' via mac-host.local:34289 (stress-053cc9e6a66d4819a03f59bf0c699f19): version 39 holds Comment by s1c5-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v40 Comment by s3c2-reviewer 's3c2-reviewer:121' via mac-host.local:34289 (stress-f8c554eeba2645afa6e6210a667d75bc): version 40 holds Comment by s1c2-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v41 Comment by s3c2-reviewer 's3c2-reviewer:124' via mac-host.local:34289 (stress-5fd9319ab0eb4863a2124660b18b8865): version 41 holds Comment by s1c5-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v62 Comment by s3c2-reviewer 's3c2-reviewer:190' via mac-host.local:34289 (stress-31a778bf6d4e432393fbef99a166bf10): version 62 holds Comment by s1c2-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v67 Comment by s3c2-reviewer 's3c2-reviewer:199' via mac-host.local:34289 (stress-bc8216dd2a1344f5a7aae171745fe8d7): version 67 holds Comment by s1c5-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v21 Comment by s3c4-admin 's3c4-admin:22' via mac-host.local:34289 (stress-88699ffdb0a2440893250eb94de612d0): version 21 holds Comment by s2c5-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v24 Comment by s3c4-admin 's3c4-admin:50' via mac-host.local:34289 (stress-b6523f8660b34fa5907a60268b284f6c): version 24 holds Comment by s1c4-admin
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v36 Comment by s3c4-admin 's3c4-admin:100' via mac-host.local:34289 (stress-c3a37ba7e8fa4ff88fbf3a4269d96f58): version 36 holds Comment by s2c5-reviewer

server mac-host.local:34289:
- missing record 4b4616dc-4231-4405-b284-46e2bd8eaa4e: missing (67 acknowledged changes)
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v20 Comment by s1c2-reviewer 's1c2-reviewer:38' via 36917 (stress-a5a8dfa7e3db469b96d93ac181ced10f): version 20 holds Comment by s3c7-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v40 Comment by s1c2-reviewer 's1c2-reviewer:97' via 36917 (stress-6473de4ebca447058a4ba20eebdf5872): version 40 holds Comment by s3c2-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v48 Comment by s1c2-reviewer 's1c2-reviewer:123' via 36917 (stress-cb3eeda8eb0f410b819dcf5f74a36f7f): version 48 holds Comment by s3c5-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v49 Comment by s1c2-reviewer 's1c2-reviewer:127' via 36917 (stress-1e3f94db5d02411eb933fe50f8702f66): version 49 holds Comment by s3c4-admin
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v50 Comment by s1c2-reviewer 's1c2-reviewer:128' via 36917 (stress-ecd29f9c7b4b4d08bec8badad123e7a7): version 50 holds Comment by s3c5-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v61 Comment by s1c2-reviewer 's1c2-reviewer:161' via 36917 (stress-8a307eb2f8ab4272b1562d6801f12b57): version 61 holds Comment by s3c5-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v62 Comment by s1c2-reviewer 's1c2-reviewer:164' via 36917 (stress-323ee9dca7f7466981c1a639d97148bf): version 62 holds Comment by s3c2-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v18 Comment by s1c4-admin 's1c4-admin:34' via 36917 (stress-a50049479afc40ee9470cff76b2eec19): version 18 holds Comment by s3c7-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v22 Comment by s1c4-admin 's1c4-admin:43' via 36917 (stress-e8c18895a8264baeae9aec3241f712ba): version 22 holds Comment by s3c2-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v23 Comment by s1c4-admin 's1c4-admin:44' via 36917 (stress-dcbef294c1f345298248293a277a04d9): version 23 holds Comment by s3c7-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v24 Comment by s1c4-admin 's1c4-admin:49' via 36917 (stress-0493be198e7f42308604252e06ccefe7): version 24 holds Comment by s3c4-admin
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v31 Comment by s1c4-admin 's1c4-admin:62' via 36917 (stress-15b29eb3c363480fa4861409886e5322): version 31 holds Comment by s3c5-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v51 Comment by s1c4-admin 's1c4-admin:134' via 36917 (stress-8f8119b742cf4ab682505d39d6791bb3): version 51 holds Comment by s3c5-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v57 Comment by s1c4-admin 's1c4-admin:151' via 36917 (stress-c092e3ad682d481893bea891818bb8a6): version 57 holds Comment by s3c5-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v60 Comment by s1c4-admin 's1c4-admin:158' via 36917 (stress-5ff36ac82f644c2382412619ec96e76a): version 60 holds Comment by s3c4-admin
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v63 Comment by s1c4-admin 's1c4-admin:170' via 36917 (stress-72c04390e312453189265c580335f34f): version 63 holds Comment by s3c4-admin
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v68 Comment by s1c4-admin 's1c4-admin:190' via 36917 (stress-5ad79f679a414c15bdcb27a1188df2c1): version 68 holds Comment by s3c5-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v69 Comment by s1c4-admin 's1c4-admin:195' via 36917 (stress-af5e4d3040f94e89b5eaca1fdff4f90f): version 69 holds Comment by s3c7-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v29 Comment by s1c5-reviewer 's1c5-reviewer:53' via 36917 (stress-facf9828edb6499fb490724533ae93a4): version 29 holds Comment by s3c2-reviewer
- lost update c7611f6f-9b27-4ca4-915b-8f5a5d3855e8 v30 Comment by s1c5-reviewer 's1c5-reviewer:55' via 36917 (stress-e57fa10986554e98a25c26f66449856e): version 30 holds Comment by s3c2-reviewer

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| AppendLog | 3 | 1769 | 39.1 | 768 | 2084 | 1074 | 0 | 0 | 416.9 | no |
