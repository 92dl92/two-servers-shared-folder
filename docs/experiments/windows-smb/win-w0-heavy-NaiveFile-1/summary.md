# Stress run: NaiveFile, 2 server(s): FAILED

- Commit `c71df76c37c18ca38a44e27c02961e268fbb1c09`, 2026-10-18 11:43:22 UTC, 8.8 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 12 clients, 12 records, 200 changes per client, seed 1
- Clients by role: 10 Registrant, 10 Reviewer, 4 Admin
- Data folder `stress-runs/win-w0-heavy-NaiveFile-1/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-w0-heavy-NaiveFile-1`
- 2 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-w0-heavy-NaiveFile-1` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 1902 | 243 | 27.6 | 340 | 361 | 0 | 23.9 | 72.1 | 172.2 |

Writes by outcome: conflict 340, http500 958, saved 243, wrongStatus 361. Saves by action: Create 6, Update 17, Submit 3, Reject 3, Comment 214. Reads: 3800 (http500 59, notFound 1851).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | FAIL | 3/6 | 3 | 22 | 0 | 0 | 0 |
| server ws1.example:42483 | FAIL | 3/6 | 3 | 22 | 0 | 0 | 0 |
| server ws1.example:41633 | FAIL | 3/6 | 3 | 22 | 0 | 0 | 0 |

storage:
- missing record a2a15ee9-ef8d-4240-95a5-34c28063a190: missing (1 acknowledged changes)
- missing record 9f6d934a-2f9c-4f32-afad-63fa003201ae: missing (1 acknowledged changes)
- missing record 2331b1b2-828e-4bc6-a696-8721826e1889: missing (1 acknowledged changes)
- lost update 45f4fd6b-1ea4-419a-96e7-0d884fe8c9d5 v59 Comment by s2c9-admin 's2c9-admin:179' via ws1.example:41633 (stress-237b63347a5d4f19b5df96f485b87b72): version 59 holds Comment by s2c12-reviewer
- lost update 45f4fd6b-1ea4-419a-96e7-0d884fe8c9d5 v29 Comment by s2c10-reviewer 's2c10-reviewer:76' via ws1.example:41633 (stress-97d6e16aa8dc48949b3904a6cd366891): version 29 holds Comment by s1c9-admin
- lost update 45f4fd6b-1ea4-419a-96e7-0d884fe8c9d5 v43 Comment by s2c10-reviewer 's2c10-reviewer:122' via ws1.example:41633 (stress-bdc94732af554ea59c9de9d4aa90e8bf): version 43 holds Comment by s2c2-reviewer
- lost update a2a15ee9-ef8d-4240-95a5-34c28063a190 v1 Create by s2c1-registrant via ws1.example:41633 (stress-c5abcdf122824a2ab536cd4bf2bbddfc): record missing
- lost update 9f6d934a-2f9c-4f32-afad-63fa003201ae v1 Create by s2c3-registrant via ws1.example:41633 (stress-3182c1fe30d74e1382a245f145efae8a): record missing
- lost update 2331b1b2-828e-4bc6-a696-8721826e1889 v1 Create by s1c6-registrant via ws1.example:42483 (stress-7b3fa6eec2ac4047830ba21ee29be52c): record missing
- lost update c2aebee2-a402-4c73-b9ad-e8b2c443699e v40 Comment by s1c5-reviewer 's1c5-reviewer:169' via ws1.example:42483 (stress-5d7d2c8f28b149da862d10610036250b): version 40 holds Comment by s2c10-reviewer
- lost update c2aebee2-a402-4c73-b9ad-e8b2c443699e v37 Comment by s1c7-reviewer 's1c7-reviewer:186' via ws1.example:42483 (stress-705be59be6e6411d80badf2b3e97b309): version 37 holds Comment by s2c7-reviewer
- lost update c2aebee2-a402-4c73-b9ad-e8b2c443699e v9 Comment by s1c10-reviewer 's1c10-reviewer:29' via ws1.example:42483 (stress-7d5c0e229ef646e2bfd7e40503084e80): version 9 holds Update by s2c6-registrant
- lost update c2aebee2-a402-4c73-b9ad-e8b2c443699e v7 Comment by s2c2-reviewer 's2c2-reviewer:7' via ws1.example:41633 (stress-a993b226e64b478883a0400815494df1): version 7 holds Comment by s2c7-reviewer
- lost update c2aebee2-a402-4c73-b9ad-e8b2c443699e v52 Comment by s2c2-reviewer 's2c2-reviewer:153' via ws1.example:41633 (stress-a09118aef688417f9f312a3d4fec75a5): version 52 holds Comment by s2c9-admin
- lost update c2aebee2-a402-4c73-b9ad-e8b2c443699e v13 Update by s2c6-registrant 's2c6-registrant:16' via ws1.example:41633 (stress-24db7cd1b36e4febb83c2b02d77bbe55): version 13 holds Update by s1c9-admin
- lost update c2aebee2-a402-4c73-b9ad-e8b2c443699e v66 Comment by s2c7-reviewer 's2c7-reviewer:185' via ws1.example:41633 (stress-99966ef82f784970910eb475a516696a): version 66 holds Comment by s2c4-admin
- lost update c2aebee2-a402-4c73-b9ad-e8b2c443699e v36 Comment by s2c10-reviewer 's2c10-reviewer:95' via ws1.example:41633 (stress-abcd9905787f4066bda0a4832eec5353): version 36 holds Comment by s2c4-admin
- lost update 6fced058-3d5d-40e4-81ef-931acb2da5ab v45 Comment by s1c2-reviewer 's1c2-reviewer:172' via ws1.example:42483 (stress-f34d34fd7b1743d48b20b7f8fbc20f68): version 45 holds Comment by s1c4-admin
- lost update 6fced058-3d5d-40e4-81ef-931acb2da5ab v18 Comment by s1c4-admin 's1c4-admin:63' via ws1.example:42483 (stress-9c83585f69014fb8ac4a14c75eb08272): version 18 holds Comment by s1c5-reviewer
- lost update 6fced058-3d5d-40e4-81ef-931acb2da5ab v29 Comment by s1c9-admin 's1c9-admin:88' via ws1.example:42483 (stress-8582e2e462b1435eab7a7cae38536b4f): version 29 holds Comment by s1c2-reviewer
- lost update 6fced058-3d5d-40e4-81ef-931acb2da5ab v38 Comment by s2c4-admin 's2c4-admin:86' via ws1.example:41633 (stress-33901dc7828942fdb227044de7057685): version 38 holds Comment by s1c12-reviewer
- lost update 6fced058-3d5d-40e4-81ef-931acb2da5ab v36 Comment by s2c5-reviewer 's2c5-reviewer:87' via ws1.example:41633 (stress-beda659fa0a349dc9f0d245b51974200): version 36 holds Comment by s2c10-reviewer
- lost update 6fced058-3d5d-40e4-81ef-931acb2da5ab v79 Comment by s2c7-reviewer 's2c7-reviewer:194' via ws1.example:41633 (stress-34eeef6895e24575868cac6495203185): version 79 holds Comment by s2c4-admin

server ws1.example:42483:
- missing record a2a15ee9-ef8d-4240-95a5-34c28063a190: missing (1 acknowledged changes)
- missing record 9f6d934a-2f9c-4f32-afad-63fa003201ae: missing (1 acknowledged changes)
- missing record 2331b1b2-828e-4bc6-a696-8721826e1889: missing (1 acknowledged changes)
- lost update 45f4fd6b-1ea4-419a-96e7-0d884fe8c9d5 v59 Comment by s2c9-admin 's2c9-admin:179' via ws1.example:41633 (stress-237b63347a5d4f19b5df96f485b87b72): version 59 holds Comment by s2c12-reviewer
- lost update 45f4fd6b-1ea4-419a-96e7-0d884fe8c9d5 v29 Comment by s2c10-reviewer 's2c10-reviewer:76' via ws1.example:41633 (stress-97d6e16aa8dc48949b3904a6cd366891): version 29 holds Comment by s1c9-admin
- lost update 45f4fd6b-1ea4-419a-96e7-0d884fe8c9d5 v43 Comment by s2c10-reviewer 's2c10-reviewer:122' via ws1.example:41633 (stress-bdc94732af554ea59c9de9d4aa90e8bf): version 43 holds Comment by s2c2-reviewer
- lost update a2a15ee9-ef8d-4240-95a5-34c28063a190 v1 Create by s2c1-registrant via ws1.example:41633 (stress-c5abcdf122824a2ab536cd4bf2bbddfc): record missing
- lost update 9f6d934a-2f9c-4f32-afad-63fa003201ae v1 Create by s2c3-registrant via ws1.example:41633 (stress-3182c1fe30d74e1382a245f145efae8a): record missing
- lost update 2331b1b2-828e-4bc6-a696-8721826e1889 v1 Create by s1c6-registrant via ws1.example:42483 (stress-7b3fa6eec2ac4047830ba21ee29be52c): record missing
- lost update c2aebee2-a402-4c73-b9ad-e8b2c443699e v40 Comment by s1c5-reviewer 's1c5-reviewer:169' via ws1.example:42483 (stress-5d7d2c8f28b149da862d10610036250b): version 40 holds Comment by s2c10-reviewer
- lost update c2aebee2-a402-4c73-b9ad-e8b2c443699e v37 Comment by s1c7-reviewer 's1c7-reviewer:186' via ws1.example:42483 (stress-705be59be6e6411d80badf2b3e97b309): version 37 holds Comment by s2c7-reviewer
- lost update c2aebee2-a402-4c73-b9ad-e8b2c443699e v9 Comment by s1c10-reviewer 's1c10-reviewer:29' via ws1.example:42483 (stress-7d5c0e229ef646e2bfd7e40503084e80): version 9 holds Update by s2c6-registrant
- lost update c2aebee2-a402-4c73-b9ad-e8b2c443699e v7 Comment by s2c2-reviewer 's2c2-reviewer:7' via ws1.example:41633 (stress-a993b226e64b478883a0400815494df1): version 7 holds Comment by s2c7-reviewer
- lost update c2aebee2-a402-4c73-b9ad-e8b2c443699e v52 Comment by s2c2-reviewer 's2c2-reviewer:153' via ws1.example:41633 (stress-a09118aef688417f9f312a3d4fec75a5): version 52 holds Comment by s2c9-admin
- lost update c2aebee2-a402-4c73-b9ad-e8b2c443699e v13 Update by s2c6-registrant 's2c6-registrant:16' via ws1.example:41633 (stress-24db7cd1b36e4febb83c2b02d77bbe55): version 13 holds Update by s1c9-admin
- lost update c2aebee2-a402-4c73-b9ad-e8b2c443699e v66 Comment by s2c7-reviewer 's2c7-reviewer:185' via ws1.example:41633 (stress-99966ef82f784970910eb475a516696a): version 66 holds Comment by s2c4-admin
- lost update c2aebee2-a402-4c73-b9ad-e8b2c443699e v36 Comment by s2c10-reviewer 's2c10-reviewer:95' via ws1.example:41633 (stress-abcd9905787f4066bda0a4832eec5353): version 36 holds Comment by s2c4-admin
- lost update 6fced058-3d5d-40e4-81ef-931acb2da5ab v45 Comment by s1c2-reviewer 's1c2-reviewer:172' via ws1.example:42483 (stress-f34d34fd7b1743d48b20b7f8fbc20f68): version 45 holds Comment by s1c4-admin
- lost update 6fced058-3d5d-40e4-81ef-931acb2da5ab v18 Comment by s1c4-admin 's1c4-admin:63' via ws1.example:42483 (stress-9c83585f69014fb8ac4a14c75eb08272): version 18 holds Comment by s1c5-reviewer
- lost update 6fced058-3d5d-40e4-81ef-931acb2da5ab v29 Comment by s1c9-admin 's1c9-admin:88' via ws1.example:42483 (stress-8582e2e462b1435eab7a7cae38536b4f): version 29 holds Comment by s1c2-reviewer
- lost update 6fced058-3d5d-40e4-81ef-931acb2da5ab v38 Comment by s2c4-admin 's2c4-admin:86' via ws1.example:41633 (stress-33901dc7828942fdb227044de7057685): version 38 holds Comment by s1c12-reviewer
- lost update 6fced058-3d5d-40e4-81ef-931acb2da5ab v36 Comment by s2c5-reviewer 's2c5-reviewer:87' via ws1.example:41633 (stress-beda659fa0a349dc9f0d245b51974200): version 36 holds Comment by s2c10-reviewer
- lost update 6fced058-3d5d-40e4-81ef-931acb2da5ab v79 Comment by s2c7-reviewer 's2c7-reviewer:194' via ws1.example:41633 (stress-34eeef6895e24575868cac6495203185): version 79 holds Comment by s2c4-admin

server ws1.example:41633:
- missing record a2a15ee9-ef8d-4240-95a5-34c28063a190: missing (1 acknowledged changes)
- missing record 9f6d934a-2f9c-4f32-afad-63fa003201ae: missing (1 acknowledged changes)
- missing record 2331b1b2-828e-4bc6-a696-8721826e1889: missing (1 acknowledged changes)
- lost update 45f4fd6b-1ea4-419a-96e7-0d884fe8c9d5 v59 Comment by s2c9-admin 's2c9-admin:179' via ws1.example:41633 (stress-237b63347a5d4f19b5df96f485b87b72): version 59 holds Comment by s2c12-reviewer
- lost update 45f4fd6b-1ea4-419a-96e7-0d884fe8c9d5 v29 Comment by s2c10-reviewer 's2c10-reviewer:76' via ws1.example:41633 (stress-97d6e16aa8dc48949b3904a6cd366891): version 29 holds Comment by s1c9-admin
- lost update 45f4fd6b-1ea4-419a-96e7-0d884fe8c9d5 v43 Comment by s2c10-reviewer 's2c10-reviewer:122' via ws1.example:41633 (stress-bdc94732af554ea59c9de9d4aa90e8bf): version 43 holds Comment by s2c2-reviewer
- lost update a2a15ee9-ef8d-4240-95a5-34c28063a190 v1 Create by s2c1-registrant via ws1.example:41633 (stress-c5abcdf122824a2ab536cd4bf2bbddfc): record missing
- lost update 9f6d934a-2f9c-4f32-afad-63fa003201ae v1 Create by s2c3-registrant via ws1.example:41633 (stress-3182c1fe30d74e1382a245f145efae8a): record missing
- lost update 2331b1b2-828e-4bc6-a696-8721826e1889 v1 Create by s1c6-registrant via ws1.example:42483 (stress-7b3fa6eec2ac4047830ba21ee29be52c): record missing
- lost update c2aebee2-a402-4c73-b9ad-e8b2c443699e v40 Comment by s1c5-reviewer 's1c5-reviewer:169' via ws1.example:42483 (stress-5d7d2c8f28b149da862d10610036250b): version 40 holds Comment by s2c10-reviewer
- lost update c2aebee2-a402-4c73-b9ad-e8b2c443699e v37 Comment by s1c7-reviewer 's1c7-reviewer:186' via ws1.example:42483 (stress-705be59be6e6411d80badf2b3e97b309): version 37 holds Comment by s2c7-reviewer
- lost update c2aebee2-a402-4c73-b9ad-e8b2c443699e v9 Comment by s1c10-reviewer 's1c10-reviewer:29' via ws1.example:42483 (stress-7d5c0e229ef646e2bfd7e40503084e80): version 9 holds Update by s2c6-registrant
- lost update c2aebee2-a402-4c73-b9ad-e8b2c443699e v7 Comment by s2c2-reviewer 's2c2-reviewer:7' via ws1.example:41633 (stress-a993b226e64b478883a0400815494df1): version 7 holds Comment by s2c7-reviewer
- lost update c2aebee2-a402-4c73-b9ad-e8b2c443699e v52 Comment by s2c2-reviewer 's2c2-reviewer:153' via ws1.example:41633 (stress-a09118aef688417f9f312a3d4fec75a5): version 52 holds Comment by s2c9-admin
- lost update c2aebee2-a402-4c73-b9ad-e8b2c443699e v13 Update by s2c6-registrant 's2c6-registrant:16' via ws1.example:41633 (stress-24db7cd1b36e4febb83c2b02d77bbe55): version 13 holds Update by s1c9-admin
- lost update c2aebee2-a402-4c73-b9ad-e8b2c443699e v66 Comment by s2c7-reviewer 's2c7-reviewer:185' via ws1.example:41633 (stress-99966ef82f784970910eb475a516696a): version 66 holds Comment by s2c4-admin
- lost update c2aebee2-a402-4c73-b9ad-e8b2c443699e v36 Comment by s2c10-reviewer 's2c10-reviewer:95' via ws1.example:41633 (stress-abcd9905787f4066bda0a4832eec5353): version 36 holds Comment by s2c4-admin
- lost update 6fced058-3d5d-40e4-81ef-931acb2da5ab v45 Comment by s1c2-reviewer 's1c2-reviewer:172' via ws1.example:42483 (stress-f34d34fd7b1743d48b20b7f8fbc20f68): version 45 holds Comment by s1c4-admin
- lost update 6fced058-3d5d-40e4-81ef-931acb2da5ab v18 Comment by s1c4-admin 's1c4-admin:63' via ws1.example:42483 (stress-9c83585f69014fb8ac4a14c75eb08272): version 18 holds Comment by s1c5-reviewer
- lost update 6fced058-3d5d-40e4-81ef-931acb2da5ab v29 Comment by s1c9-admin 's1c9-admin:88' via ws1.example:42483 (stress-8582e2e462b1435eab7a7cae38536b4f): version 29 holds Comment by s1c2-reviewer
- lost update 6fced058-3d5d-40e4-81ef-931acb2da5ab v38 Comment by s2c4-admin 's2c4-admin:86' via ws1.example:41633 (stress-33901dc7828942fdb227044de7057685): version 38 holds Comment by s1c12-reviewer
- lost update 6fced058-3d5d-40e4-81ef-931acb2da5ab v36 Comment by s2c5-reviewer 's2c5-reviewer:87' via ws1.example:41633 (stress-beda659fa0a349dc9f0d245b51974200): version 36 holds Comment by s2c10-reviewer
- lost update 6fced058-3d5d-40e4-81ef-931acb2da5ab v79 Comment by s2c7-reviewer 's2c7-reviewer:194' via ws1.example:41633 (stress-34eeef6895e24575868cac6495203185): version 79 holds Comment by s2c4-admin

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 2 | 243 | 27.6 | 340 | 361 | 22 | 0 | 0 | 72.1 | no |
