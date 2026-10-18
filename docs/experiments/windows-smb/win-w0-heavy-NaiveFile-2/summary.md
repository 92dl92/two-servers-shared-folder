# Stress run: NaiveFile, 2 server(s): FAILED

- Commit `c71df76c37c18ca38a44e27c02961e268fbb1c09`, 2026-10-18 11:44:24 UTC, 8.8 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 12 clients, 12 records, 200 changes per client, seed 2
- Clients by role: 10 Registrant, 10 Reviewer, 4 Admin
- Data folder `stress-runs/win-w0-heavy-NaiveFile-2/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-w0-heavy-NaiveFile-2`
- 2 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-w0-heavy-NaiveFile-2` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 2044 | 207 | 23.5 | 266 | 540 | 0 | 23.2 | 120.4 | 180.6 |

Writes by outcome: conflict 266, http500 1031, saved 207, wrongStatus 540. Saves by action: Create 8, Update 19, Submit 5, Approve 3, Reject 1, Comment 171. Reads: 4200 (http500 88, notFound 2080).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | FAIL | 4/8 | 4 | 20 | 0 | 0 | 0 |
| server ws1.example:39571 | FAIL | 4/8 | 4 | 20 | 0 | 0 | 0 |
| server ws1.example:36421 | FAIL | 4/8 | 4 | 20 | 0 | 0 | 0 |

storage:
- missing record 05a8c9ea-0baa-43eb-910b-d49aefebb789: missing (1 acknowledged changes)
- missing record d09223ef-5e3d-421c-9aa4-cdee8b88c425: missing (1 acknowledged changes)
- missing record f28363cc-c867-4b13-b334-83420fa1e691: missing (1 acknowledged changes)
- missing record 73f59e09-b686-4327-a1f8-8583f1bf9fc2: missing (1 acknowledged changes)
- lost update 05a8c9ea-0baa-43eb-910b-d49aefebb789 v1 Create by s1c1-registrant via ws1.example:39571 (stress-045e9ffb889f46dd9e818ae4fae017b3): record missing
- lost update 33414359-47da-49aa-a942-e89bf97da433 v27 Comment by s1c5-reviewer 's1c5-reviewer:128' via ws1.example:39571 (stress-168e28d69aa44e44bc0a8060cde7f694): version 27 holds Comment by s2c2-reviewer
- lost update 33414359-47da-49aa-a942-e89bf97da433 v26 Comment by s1c9-admin 's1c9-admin:117' via ws1.example:39571 (stress-2239e68ea04443fb866b882d9f5380fc): version 26 holds Comment by s1c4-admin
- lost update 33414359-47da-49aa-a942-e89bf97da433 v18 Comment by s2c9-admin 's2c9-admin:81' via ws1.example:36421 (stress-6b31db2f6ea34d1a8974cb284c6932a0): version 18 holds Comment by s1c4-admin
- lost update eb2cc965-99e8-4ef8-9158-d5588ecd7467 v43 Comment by s1c4-admin 's1c4-admin:189' via ws1.example:39571 (stress-fe0180ab5f52417888b94c9f1b06662e): version 43 holds Comment by s1c2-reviewer
- lost update eb2cc965-99e8-4ef8-9158-d5588ecd7467 v38 Comment by s1c9-admin 's1c9-admin:162' via ws1.example:39571 (stress-2d574e1215e442bbaa85649e309ebb69): version 38 holds Comment by s1c12-reviewer
- lost update eb2cc965-99e8-4ef8-9158-d5588ecd7467 v32 Comment by s2c4-admin 's2c4-admin:156' via ws1.example:36421 (stress-22524b7d74a744438ba083a1357f6f84): version 32 holds Comment by s2c4-admin
- lost update eb2cc965-99e8-4ef8-9158-d5588ecd7467 v20 Comment by s2c12-reviewer 's2c12-reviewer:78' via ws1.example:36421 (stress-6a31acc9ccc54ff5acac49ba42d7fb6c): version 20 holds Comment by s1c12-reviewer
- lost update d09223ef-5e3d-421c-9aa4-cdee8b88c425 v1 Create by s2c3-registrant via ws1.example:36421 (stress-5fe80e446fc7451f94a57367828e06a4): record missing
- lost update f28363cc-c867-4b13-b334-83420fa1e691 v1 Create by s1c6-registrant via ws1.example:39571 (stress-1f7ce7a0cf1642069a829c71f41cefdc): record missing
- lost update 05511324-820f-4d1c-832e-757f8b7ee519 v19 Comment by s1c12-reviewer 's1c12-reviewer:54' via ws1.example:39571 (stress-5f8144e0446243328e8af772cf633dd4): version 19 holds Comment by s2c2-reviewer
- lost update 05511324-820f-4d1c-832e-757f8b7ee519 v53 Comment by s2c5-reviewer 's2c5-reviewer:187' via ws1.example:36421 (stress-7e15093f1d3f482193ff04934e8dc5c2): version 53 holds Comment by s1c7-reviewer
- lost update 05511324-820f-4d1c-832e-757f8b7ee519 v51 Comment by s2c7-reviewer 's2c7-reviewer:173' via ws1.example:36421 (stress-6c22958de06548f8909012f14bd129af): version 51 holds Comment by s1c7-reviewer
- lost update 4eaff9cd-b670-42ce-a2de-6979b0579af6 v4 Submit by s1c8-registrant via ws1.example:39571 (stress-9e41367f4e7946eca8b0ce3b3f1e48cb): version 4 holds Comment by s2c5-reviewer
- lost update 4eaff9cd-b670-42ce-a2de-6979b0579af6 v17 Update by s1c8-registrant 's1c8-registrant:50' via ws1.example:39571 (stress-7d2f41e31495434cab5c5f208ad4b0d0): version 17 holds Update by s1c8-registrant
- lost update 4eaff9cd-b670-42ce-a2de-6979b0579af6 v23 Comment by s1c10-reviewer 's1c10-reviewer:116' via ws1.example:39571 (stress-8540d27e8b19472f8944d35b3e31300b): version 23 holds Comment by s2c10-reviewer
- lost update 4eaff9cd-b670-42ce-a2de-6979b0579af6 v36 Comment by s2c2-reviewer 's2c2-reviewer:183' via ws1.example:36421 (stress-49cf1f62e8424780a764cb3e4df6815a): version 36 holds Comment by s1c2-reviewer
- lost update 4eaff9cd-b670-42ce-a2de-6979b0579af6 v18 Comment by s2c7-reviewer 's2c7-reviewer:76' via ws1.example:36421 (stress-b24e706cbe8a4f6fa79a9fbcb1a8800b): version 18 holds Approve by s2c2-reviewer
- lost update 4eaff9cd-b670-42ce-a2de-6979b0579af6 v17 Update by s1c8-registrant 's1c8-registrant:49' via ws1.example:39571 (stress-a5a70a87ff7c4d2d89d5ea6f26343e7e): the form data is from another update (notes 's1c8-registrant:50')
- lost update 73f59e09-b686-4327-a1f8-8583f1bf9fc2 v1 Create by s2c8-registrant via ws1.example:36421 (stress-60f45aac12cb425c89da663aede48ba1): record missing

server ws1.example:39571:
- missing record 05a8c9ea-0baa-43eb-910b-d49aefebb789: missing (1 acknowledged changes)
- missing record d09223ef-5e3d-421c-9aa4-cdee8b88c425: missing (1 acknowledged changes)
- missing record f28363cc-c867-4b13-b334-83420fa1e691: missing (1 acknowledged changes)
- missing record 73f59e09-b686-4327-a1f8-8583f1bf9fc2: missing (1 acknowledged changes)
- lost update 05a8c9ea-0baa-43eb-910b-d49aefebb789 v1 Create by s1c1-registrant via ws1.example:39571 (stress-045e9ffb889f46dd9e818ae4fae017b3): record missing
- lost update 33414359-47da-49aa-a942-e89bf97da433 v27 Comment by s1c5-reviewer 's1c5-reviewer:128' via ws1.example:39571 (stress-168e28d69aa44e44bc0a8060cde7f694): version 27 holds Comment by s2c2-reviewer
- lost update 33414359-47da-49aa-a942-e89bf97da433 v26 Comment by s1c9-admin 's1c9-admin:117' via ws1.example:39571 (stress-2239e68ea04443fb866b882d9f5380fc): version 26 holds Comment by s1c4-admin
- lost update 33414359-47da-49aa-a942-e89bf97da433 v18 Comment by s2c9-admin 's2c9-admin:81' via ws1.example:36421 (stress-6b31db2f6ea34d1a8974cb284c6932a0): version 18 holds Comment by s1c4-admin
- lost update eb2cc965-99e8-4ef8-9158-d5588ecd7467 v43 Comment by s1c4-admin 's1c4-admin:189' via ws1.example:39571 (stress-fe0180ab5f52417888b94c9f1b06662e): version 43 holds Comment by s1c2-reviewer
- lost update eb2cc965-99e8-4ef8-9158-d5588ecd7467 v38 Comment by s1c9-admin 's1c9-admin:162' via ws1.example:39571 (stress-2d574e1215e442bbaa85649e309ebb69): version 38 holds Comment by s1c12-reviewer
- lost update eb2cc965-99e8-4ef8-9158-d5588ecd7467 v32 Comment by s2c4-admin 's2c4-admin:156' via ws1.example:36421 (stress-22524b7d74a744438ba083a1357f6f84): version 32 holds Comment by s2c4-admin
- lost update eb2cc965-99e8-4ef8-9158-d5588ecd7467 v20 Comment by s2c12-reviewer 's2c12-reviewer:78' via ws1.example:36421 (stress-6a31acc9ccc54ff5acac49ba42d7fb6c): version 20 holds Comment by s1c12-reviewer
- lost update d09223ef-5e3d-421c-9aa4-cdee8b88c425 v1 Create by s2c3-registrant via ws1.example:36421 (stress-5fe80e446fc7451f94a57367828e06a4): record missing
- lost update f28363cc-c867-4b13-b334-83420fa1e691 v1 Create by s1c6-registrant via ws1.example:39571 (stress-1f7ce7a0cf1642069a829c71f41cefdc): record missing
- lost update 05511324-820f-4d1c-832e-757f8b7ee519 v19 Comment by s1c12-reviewer 's1c12-reviewer:54' via ws1.example:39571 (stress-5f8144e0446243328e8af772cf633dd4): version 19 holds Comment by s2c2-reviewer
- lost update 05511324-820f-4d1c-832e-757f8b7ee519 v53 Comment by s2c5-reviewer 's2c5-reviewer:187' via ws1.example:36421 (stress-7e15093f1d3f482193ff04934e8dc5c2): version 53 holds Comment by s1c7-reviewer
- lost update 05511324-820f-4d1c-832e-757f8b7ee519 v51 Comment by s2c7-reviewer 's2c7-reviewer:173' via ws1.example:36421 (stress-6c22958de06548f8909012f14bd129af): version 51 holds Comment by s1c7-reviewer
- lost update 4eaff9cd-b670-42ce-a2de-6979b0579af6 v4 Submit by s1c8-registrant via ws1.example:39571 (stress-9e41367f4e7946eca8b0ce3b3f1e48cb): version 4 holds Comment by s2c5-reviewer
- lost update 4eaff9cd-b670-42ce-a2de-6979b0579af6 v17 Update by s1c8-registrant 's1c8-registrant:50' via ws1.example:39571 (stress-7d2f41e31495434cab5c5f208ad4b0d0): version 17 holds Update by s1c8-registrant
- lost update 4eaff9cd-b670-42ce-a2de-6979b0579af6 v23 Comment by s1c10-reviewer 's1c10-reviewer:116' via ws1.example:39571 (stress-8540d27e8b19472f8944d35b3e31300b): version 23 holds Comment by s2c10-reviewer
- lost update 4eaff9cd-b670-42ce-a2de-6979b0579af6 v36 Comment by s2c2-reviewer 's2c2-reviewer:183' via ws1.example:36421 (stress-49cf1f62e8424780a764cb3e4df6815a): version 36 holds Comment by s1c2-reviewer
- lost update 4eaff9cd-b670-42ce-a2de-6979b0579af6 v18 Comment by s2c7-reviewer 's2c7-reviewer:76' via ws1.example:36421 (stress-b24e706cbe8a4f6fa79a9fbcb1a8800b): version 18 holds Approve by s2c2-reviewer
- lost update 4eaff9cd-b670-42ce-a2de-6979b0579af6 v17 Update by s1c8-registrant 's1c8-registrant:49' via ws1.example:39571 (stress-a5a70a87ff7c4d2d89d5ea6f26343e7e): the form data is from another update (notes 's1c8-registrant:50')
- lost update 73f59e09-b686-4327-a1f8-8583f1bf9fc2 v1 Create by s2c8-registrant via ws1.example:36421 (stress-60f45aac12cb425c89da663aede48ba1): record missing

server ws1.example:36421:
- missing record 05a8c9ea-0baa-43eb-910b-d49aefebb789: missing (1 acknowledged changes)
- missing record d09223ef-5e3d-421c-9aa4-cdee8b88c425: missing (1 acknowledged changes)
- missing record f28363cc-c867-4b13-b334-83420fa1e691: missing (1 acknowledged changes)
- missing record 73f59e09-b686-4327-a1f8-8583f1bf9fc2: missing (1 acknowledged changes)
- lost update 05a8c9ea-0baa-43eb-910b-d49aefebb789 v1 Create by s1c1-registrant via ws1.example:39571 (stress-045e9ffb889f46dd9e818ae4fae017b3): record missing
- lost update 33414359-47da-49aa-a942-e89bf97da433 v27 Comment by s1c5-reviewer 's1c5-reviewer:128' via ws1.example:39571 (stress-168e28d69aa44e44bc0a8060cde7f694): version 27 holds Comment by s2c2-reviewer
- lost update 33414359-47da-49aa-a942-e89bf97da433 v26 Comment by s1c9-admin 's1c9-admin:117' via ws1.example:39571 (stress-2239e68ea04443fb866b882d9f5380fc): version 26 holds Comment by s1c4-admin
- lost update 33414359-47da-49aa-a942-e89bf97da433 v18 Comment by s2c9-admin 's2c9-admin:81' via ws1.example:36421 (stress-6b31db2f6ea34d1a8974cb284c6932a0): version 18 holds Comment by s1c4-admin
- lost update eb2cc965-99e8-4ef8-9158-d5588ecd7467 v43 Comment by s1c4-admin 's1c4-admin:189' via ws1.example:39571 (stress-fe0180ab5f52417888b94c9f1b06662e): version 43 holds Comment by s1c2-reviewer
- lost update eb2cc965-99e8-4ef8-9158-d5588ecd7467 v38 Comment by s1c9-admin 's1c9-admin:162' via ws1.example:39571 (stress-2d574e1215e442bbaa85649e309ebb69): version 38 holds Comment by s1c12-reviewer
- lost update eb2cc965-99e8-4ef8-9158-d5588ecd7467 v32 Comment by s2c4-admin 's2c4-admin:156' via ws1.example:36421 (stress-22524b7d74a744438ba083a1357f6f84): version 32 holds Comment by s2c4-admin
- lost update eb2cc965-99e8-4ef8-9158-d5588ecd7467 v20 Comment by s2c12-reviewer 's2c12-reviewer:78' via ws1.example:36421 (stress-6a31acc9ccc54ff5acac49ba42d7fb6c): version 20 holds Comment by s1c12-reviewer
- lost update d09223ef-5e3d-421c-9aa4-cdee8b88c425 v1 Create by s2c3-registrant via ws1.example:36421 (stress-5fe80e446fc7451f94a57367828e06a4): record missing
- lost update f28363cc-c867-4b13-b334-83420fa1e691 v1 Create by s1c6-registrant via ws1.example:39571 (stress-1f7ce7a0cf1642069a829c71f41cefdc): record missing
- lost update 05511324-820f-4d1c-832e-757f8b7ee519 v19 Comment by s1c12-reviewer 's1c12-reviewer:54' via ws1.example:39571 (stress-5f8144e0446243328e8af772cf633dd4): version 19 holds Comment by s2c2-reviewer
- lost update 05511324-820f-4d1c-832e-757f8b7ee519 v53 Comment by s2c5-reviewer 's2c5-reviewer:187' via ws1.example:36421 (stress-7e15093f1d3f482193ff04934e8dc5c2): version 53 holds Comment by s1c7-reviewer
- lost update 05511324-820f-4d1c-832e-757f8b7ee519 v51 Comment by s2c7-reviewer 's2c7-reviewer:173' via ws1.example:36421 (stress-6c22958de06548f8909012f14bd129af): version 51 holds Comment by s1c7-reviewer
- lost update 4eaff9cd-b670-42ce-a2de-6979b0579af6 v4 Submit by s1c8-registrant via ws1.example:39571 (stress-9e41367f4e7946eca8b0ce3b3f1e48cb): version 4 holds Comment by s2c5-reviewer
- lost update 4eaff9cd-b670-42ce-a2de-6979b0579af6 v17 Update by s1c8-registrant 's1c8-registrant:50' via ws1.example:39571 (stress-7d2f41e31495434cab5c5f208ad4b0d0): version 17 holds Update by s1c8-registrant
- lost update 4eaff9cd-b670-42ce-a2de-6979b0579af6 v23 Comment by s1c10-reviewer 's1c10-reviewer:116' via ws1.example:39571 (stress-8540d27e8b19472f8944d35b3e31300b): version 23 holds Comment by s2c10-reviewer
- lost update 4eaff9cd-b670-42ce-a2de-6979b0579af6 v36 Comment by s2c2-reviewer 's2c2-reviewer:183' via ws1.example:36421 (stress-49cf1f62e8424780a764cb3e4df6815a): version 36 holds Comment by s1c2-reviewer
- lost update 4eaff9cd-b670-42ce-a2de-6979b0579af6 v18 Comment by s2c7-reviewer 's2c7-reviewer:76' via ws1.example:36421 (stress-b24e706cbe8a4f6fa79a9fbcb1a8800b): version 18 holds Approve by s2c2-reviewer
- lost update 4eaff9cd-b670-42ce-a2de-6979b0579af6 v17 Update by s1c8-registrant 's1c8-registrant:49' via ws1.example:39571 (stress-a5a70a87ff7c4d2d89d5ea6f26343e7e): the form data is from another update (notes 's1c8-registrant:50')
- lost update 73f59e09-b686-4327-a1f8-8583f1bf9fc2 v1 Create by s2c8-registrant via ws1.example:36421 (stress-60f45aac12cb425c89da663aede48ba1): record missing

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 2 | 207 | 23.5 | 266 | 540 | 20 | 0 | 0 | 120.4 | no |
