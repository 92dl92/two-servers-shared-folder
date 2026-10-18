# Stress run: LockedFile, 3 server(s): FAILED

- Commit `1b49659b6d0d95e1a10858057ef4082c27040edb`, 2026-10-17 22:21:57 UTC, 38.3 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 12 records, 200 changes per client, seed 3
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `/mnt/share/runs/nfs-two-heavy-LockedFile-3/data`; run folder `stress-runs/nfs/nfs-two-heavy-LockedFile-3`
- 1 of the servers on `user@mac.example`, data folder `/Volumes/share/runs/nfs-two-heavy-LockedFile-3/data` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 1302 | 378 | 9.9 | 412 | 468 | 0 | 228.2 | 568.5 | 2072.5 |

Writes by outcome: conflict 412, http500 34, notFound 10, saved 378, wrongStatus 468. Saves by action: Create 10, Update 47, Submit 7, Approve 5, Reject 4, Comment 305. Reads: 4400 (http500 23, notFound 3087).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | FAIL | 0/10 | 10 | 378 | 0 | 0 | 0 |
| server 42017 | FAIL | 0/10 | 10 | 378 | 0 | 0 | 0 |
| server 40333 | FAIL | 0/10 | 10 | 378 | 0 | 0 | 0 |
| server mac-host.local:44219 | FAIL | 0/10 | 10 | 378 | 0 | 0 | 0 |

storage:
- missing record 0a4ad5f2-7fe5-4237-8d92-e09608673b0e: missing (54 acknowledged changes)
- missing record 46967e68-43fe-4418-8c1c-c7a02917ceb7: missing (52 acknowledged changes)
- missing record 23404d23-147c-426b-ab51-62f6f6d98448: missing (50 acknowledged changes)
- missing record 2998f67a-b2f4-4b56-91b1-277cf6e4bdc1: missing (1 acknowledged changes)
- missing record 3d77a010-b700-449d-9f20-4f5ca97141f6: missing (50 acknowledged changes)
- missing record 843a0435-c9cd-422a-af02-706b5186f08f: missing (54 acknowledged changes)
- missing record d183a8a9-0876-4d1f-aeb9-8fcb0d18ef57: missing (1 acknowledged changes)
- missing record 8a6e4473-b1bd-4e89-9a07-bb83a3dad0c0: missing (58 acknowledged changes)
- missing record c6a1f9aa-263d-41aa-86e1-62c1c67e4945: missing (57 acknowledged changes)
- missing record 9f7e1772-5f60-42e7-a5e8-852937bda0d7: missing (1 acknowledged changes)
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v1 Create by s1c1-registrant via 42017 (stress-5139e85c0a1f4522b1e95bc014ce0c6d): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v2 Submit by s1c1-registrant via 42017 (stress-801e5092835b469ea5b6e609af605289): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v7 Update by s1c1-registrant 's1c1-registrant:10' via 42017 (stress-96d0c07a2914490c955383584bbdd4fd): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v8 Update by s1c1-registrant 's1c1-registrant:12' via 42017 (stress-d899c056dcad437f91cc4bbf77c66568): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v10 Update by s1c1-registrant 's1c1-registrant:14' via 42017 (stress-dba7c1f04f97456390ccf37245a70e67): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v11 Update by s1c1-registrant 's1c1-registrant:15' via 42017 (stress-834324d4a31144d48879bf3fb54218d2): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v24 Comment by s1c2-reviewer 's1c2-reviewer:38' via 42017 (stress-a733bbb7a1634ec5bdba1c8023733876): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v31 Comment by s1c2-reviewer 's1c2-reviewer:51' via 42017 (stress-cc05487557934efc91f871e3de75c633): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v35 Comment by s1c2-reviewer 's1c2-reviewer:60' via 42017 (stress-d1838bd2bf2b472c8ae28e0c506e3220): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v41 Comment by s1c2-reviewer 's1c2-reviewer:75' via 42017 (stress-e4eb50c9855b4d288f063a7a8d37f1fa): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v5 Update by s1c4-admin 's1c4-admin:8' via 42017 (stress-151e78e6d20348c89ba9ee68931a1660): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v6 Update by s1c4-admin 's1c4-admin:10' via 42017 (stress-fea2d5be43244ad5ab5498c80f7c72d0): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v9 Comment by s1c4-admin 's1c4-admin:18' via 42017 (stress-9196a19bab99450f81fbcf4333b2d272): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v21 Comment by s1c4-admin 's1c4-admin:34' via 42017 (stress-79f2ee956f6849cd8775b5d50b8e7fa0): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v29 Comment by s1c4-admin 's1c4-admin:55' via 42017 (stress-c5961620f4c24fb9a477775984145f56): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v42 Comment by s1c4-admin 's1c4-admin:81' via 42017 (stress-829d18357f2b42819c9720edebc96124): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v4 Reject by s1c5-reviewer via 42017 (stress-a2165387163d498faddaaca7faa3abbb): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v8 Comment by s1c5-reviewer 's1c5-reviewer:11' via 42017 (stress-63ce1af62fc04d25856e085f3a0f8342): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v13 Comment by s1c5-reviewer 's1c5-reviewer:17' via 42017 (stress-bcd36d9fc16f46a98a9808feb9d2deeb): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v18 Comment by s1c5-reviewer 's1c5-reviewer:29' via 42017 (stress-d0835560fa2d4affb971cf86cc4bf5ce): record missing

server 42017:
- missing record 0a4ad5f2-7fe5-4237-8d92-e09608673b0e: missing (54 acknowledged changes)
- missing record 46967e68-43fe-4418-8c1c-c7a02917ceb7: missing (52 acknowledged changes)
- missing record 23404d23-147c-426b-ab51-62f6f6d98448: missing (50 acknowledged changes)
- missing record 2998f67a-b2f4-4b56-91b1-277cf6e4bdc1: missing (1 acknowledged changes)
- missing record 3d77a010-b700-449d-9f20-4f5ca97141f6: missing (50 acknowledged changes)
- missing record 843a0435-c9cd-422a-af02-706b5186f08f: missing (54 acknowledged changes)
- missing record d183a8a9-0876-4d1f-aeb9-8fcb0d18ef57: missing (1 acknowledged changes)
- missing record 8a6e4473-b1bd-4e89-9a07-bb83a3dad0c0: missing (58 acknowledged changes)
- missing record c6a1f9aa-263d-41aa-86e1-62c1c67e4945: missing (57 acknowledged changes)
- missing record 9f7e1772-5f60-42e7-a5e8-852937bda0d7: missing (1 acknowledged changes)
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v1 Create by s1c1-registrant via 42017 (stress-5139e85c0a1f4522b1e95bc014ce0c6d): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v2 Submit by s1c1-registrant via 42017 (stress-801e5092835b469ea5b6e609af605289): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v7 Update by s1c1-registrant 's1c1-registrant:10' via 42017 (stress-96d0c07a2914490c955383584bbdd4fd): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v8 Update by s1c1-registrant 's1c1-registrant:12' via 42017 (stress-d899c056dcad437f91cc4bbf77c66568): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v10 Update by s1c1-registrant 's1c1-registrant:14' via 42017 (stress-dba7c1f04f97456390ccf37245a70e67): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v11 Update by s1c1-registrant 's1c1-registrant:15' via 42017 (stress-834324d4a31144d48879bf3fb54218d2): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v24 Comment by s1c2-reviewer 's1c2-reviewer:38' via 42017 (stress-a733bbb7a1634ec5bdba1c8023733876): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v31 Comment by s1c2-reviewer 's1c2-reviewer:51' via 42017 (stress-cc05487557934efc91f871e3de75c633): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v35 Comment by s1c2-reviewer 's1c2-reviewer:60' via 42017 (stress-d1838bd2bf2b472c8ae28e0c506e3220): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v41 Comment by s1c2-reviewer 's1c2-reviewer:75' via 42017 (stress-e4eb50c9855b4d288f063a7a8d37f1fa): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v5 Update by s1c4-admin 's1c4-admin:8' via 42017 (stress-151e78e6d20348c89ba9ee68931a1660): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v6 Update by s1c4-admin 's1c4-admin:10' via 42017 (stress-fea2d5be43244ad5ab5498c80f7c72d0): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v9 Comment by s1c4-admin 's1c4-admin:18' via 42017 (stress-9196a19bab99450f81fbcf4333b2d272): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v21 Comment by s1c4-admin 's1c4-admin:34' via 42017 (stress-79f2ee956f6849cd8775b5d50b8e7fa0): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v29 Comment by s1c4-admin 's1c4-admin:55' via 42017 (stress-c5961620f4c24fb9a477775984145f56): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v42 Comment by s1c4-admin 's1c4-admin:81' via 42017 (stress-829d18357f2b42819c9720edebc96124): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v4 Reject by s1c5-reviewer via 42017 (stress-a2165387163d498faddaaca7faa3abbb): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v8 Comment by s1c5-reviewer 's1c5-reviewer:11' via 42017 (stress-63ce1af62fc04d25856e085f3a0f8342): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v13 Comment by s1c5-reviewer 's1c5-reviewer:17' via 42017 (stress-bcd36d9fc16f46a98a9808feb9d2deeb): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v18 Comment by s1c5-reviewer 's1c5-reviewer:29' via 42017 (stress-d0835560fa2d4affb971cf86cc4bf5ce): record missing

server 40333:
- missing record 0a4ad5f2-7fe5-4237-8d92-e09608673b0e: missing (54 acknowledged changes)
- missing record 46967e68-43fe-4418-8c1c-c7a02917ceb7: missing (52 acknowledged changes)
- missing record 23404d23-147c-426b-ab51-62f6f6d98448: missing (50 acknowledged changes)
- missing record 2998f67a-b2f4-4b56-91b1-277cf6e4bdc1: missing (1 acknowledged changes)
- missing record 3d77a010-b700-449d-9f20-4f5ca97141f6: missing (50 acknowledged changes)
- missing record 843a0435-c9cd-422a-af02-706b5186f08f: missing (54 acknowledged changes)
- missing record d183a8a9-0876-4d1f-aeb9-8fcb0d18ef57: missing (1 acknowledged changes)
- missing record 8a6e4473-b1bd-4e89-9a07-bb83a3dad0c0: missing (58 acknowledged changes)
- missing record c6a1f9aa-263d-41aa-86e1-62c1c67e4945: missing (57 acknowledged changes)
- missing record 9f7e1772-5f60-42e7-a5e8-852937bda0d7: missing (1 acknowledged changes)
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v1 Create by s1c1-registrant via 42017 (stress-5139e85c0a1f4522b1e95bc014ce0c6d): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v2 Submit by s1c1-registrant via 42017 (stress-801e5092835b469ea5b6e609af605289): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v7 Update by s1c1-registrant 's1c1-registrant:10' via 42017 (stress-96d0c07a2914490c955383584bbdd4fd): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v8 Update by s1c1-registrant 's1c1-registrant:12' via 42017 (stress-d899c056dcad437f91cc4bbf77c66568): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v10 Update by s1c1-registrant 's1c1-registrant:14' via 42017 (stress-dba7c1f04f97456390ccf37245a70e67): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v11 Update by s1c1-registrant 's1c1-registrant:15' via 42017 (stress-834324d4a31144d48879bf3fb54218d2): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v24 Comment by s1c2-reviewer 's1c2-reviewer:38' via 42017 (stress-a733bbb7a1634ec5bdba1c8023733876): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v31 Comment by s1c2-reviewer 's1c2-reviewer:51' via 42017 (stress-cc05487557934efc91f871e3de75c633): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v35 Comment by s1c2-reviewer 's1c2-reviewer:60' via 42017 (stress-d1838bd2bf2b472c8ae28e0c506e3220): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v41 Comment by s1c2-reviewer 's1c2-reviewer:75' via 42017 (stress-e4eb50c9855b4d288f063a7a8d37f1fa): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v5 Update by s1c4-admin 's1c4-admin:8' via 42017 (stress-151e78e6d20348c89ba9ee68931a1660): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v6 Update by s1c4-admin 's1c4-admin:10' via 42017 (stress-fea2d5be43244ad5ab5498c80f7c72d0): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v9 Comment by s1c4-admin 's1c4-admin:18' via 42017 (stress-9196a19bab99450f81fbcf4333b2d272): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v21 Comment by s1c4-admin 's1c4-admin:34' via 42017 (stress-79f2ee956f6849cd8775b5d50b8e7fa0): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v29 Comment by s1c4-admin 's1c4-admin:55' via 42017 (stress-c5961620f4c24fb9a477775984145f56): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v42 Comment by s1c4-admin 's1c4-admin:81' via 42017 (stress-829d18357f2b42819c9720edebc96124): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v4 Reject by s1c5-reviewer via 42017 (stress-a2165387163d498faddaaca7faa3abbb): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v8 Comment by s1c5-reviewer 's1c5-reviewer:11' via 42017 (stress-63ce1af62fc04d25856e085f3a0f8342): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v13 Comment by s1c5-reviewer 's1c5-reviewer:17' via 42017 (stress-bcd36d9fc16f46a98a9808feb9d2deeb): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v18 Comment by s1c5-reviewer 's1c5-reviewer:29' via 42017 (stress-d0835560fa2d4affb971cf86cc4bf5ce): record missing

server mac-host.local:44219:
- missing record 0a4ad5f2-7fe5-4237-8d92-e09608673b0e: missing (54 acknowledged changes)
- missing record 46967e68-43fe-4418-8c1c-c7a02917ceb7: missing (52 acknowledged changes)
- missing record 23404d23-147c-426b-ab51-62f6f6d98448: missing (50 acknowledged changes)
- missing record 2998f67a-b2f4-4b56-91b1-277cf6e4bdc1: missing (1 acknowledged changes)
- missing record 3d77a010-b700-449d-9f20-4f5ca97141f6: missing (50 acknowledged changes)
- missing record 843a0435-c9cd-422a-af02-706b5186f08f: missing (54 acknowledged changes)
- missing record d183a8a9-0876-4d1f-aeb9-8fcb0d18ef57: missing (1 acknowledged changes)
- missing record 8a6e4473-b1bd-4e89-9a07-bb83a3dad0c0: missing (58 acknowledged changes)
- missing record c6a1f9aa-263d-41aa-86e1-62c1c67e4945: missing (57 acknowledged changes)
- missing record 9f7e1772-5f60-42e7-a5e8-852937bda0d7: missing (1 acknowledged changes)
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v1 Create by s1c1-registrant via 42017 (stress-5139e85c0a1f4522b1e95bc014ce0c6d): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v2 Submit by s1c1-registrant via 42017 (stress-801e5092835b469ea5b6e609af605289): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v7 Update by s1c1-registrant 's1c1-registrant:10' via 42017 (stress-96d0c07a2914490c955383584bbdd4fd): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v8 Update by s1c1-registrant 's1c1-registrant:12' via 42017 (stress-d899c056dcad437f91cc4bbf77c66568): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v10 Update by s1c1-registrant 's1c1-registrant:14' via 42017 (stress-dba7c1f04f97456390ccf37245a70e67): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v11 Update by s1c1-registrant 's1c1-registrant:15' via 42017 (stress-834324d4a31144d48879bf3fb54218d2): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v24 Comment by s1c2-reviewer 's1c2-reviewer:38' via 42017 (stress-a733bbb7a1634ec5bdba1c8023733876): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v31 Comment by s1c2-reviewer 's1c2-reviewer:51' via 42017 (stress-cc05487557934efc91f871e3de75c633): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v35 Comment by s1c2-reviewer 's1c2-reviewer:60' via 42017 (stress-d1838bd2bf2b472c8ae28e0c506e3220): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v41 Comment by s1c2-reviewer 's1c2-reviewer:75' via 42017 (stress-e4eb50c9855b4d288f063a7a8d37f1fa): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v5 Update by s1c4-admin 's1c4-admin:8' via 42017 (stress-151e78e6d20348c89ba9ee68931a1660): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v6 Update by s1c4-admin 's1c4-admin:10' via 42017 (stress-fea2d5be43244ad5ab5498c80f7c72d0): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v9 Comment by s1c4-admin 's1c4-admin:18' via 42017 (stress-9196a19bab99450f81fbcf4333b2d272): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v21 Comment by s1c4-admin 's1c4-admin:34' via 42017 (stress-79f2ee956f6849cd8775b5d50b8e7fa0): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v29 Comment by s1c4-admin 's1c4-admin:55' via 42017 (stress-c5961620f4c24fb9a477775984145f56): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v42 Comment by s1c4-admin 's1c4-admin:81' via 42017 (stress-829d18357f2b42819c9720edebc96124): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v4 Reject by s1c5-reviewer via 42017 (stress-a2165387163d498faddaaca7faa3abbb): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v8 Comment by s1c5-reviewer 's1c5-reviewer:11' via 42017 (stress-63ce1af62fc04d25856e085f3a0f8342): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v13 Comment by s1c5-reviewer 's1c5-reviewer:17' via 42017 (stress-bcd36d9fc16f46a98a9808feb9d2deeb): record missing
- lost update 0a4ad5f2-7fe5-4237-8d92-e09608673b0e v18 Comment by s1c5-reviewer 's1c5-reviewer:29' via 42017 (stress-d0835560fa2d4affb971cf86cc4bf5ce): record missing

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| LockedFile | 3 | 378 | 9.9 | 412 | 468 | 378 | 0 | 0 | 568.5 | no |
