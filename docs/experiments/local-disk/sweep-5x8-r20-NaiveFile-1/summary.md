# Stress run: NaiveFile, 5 server(s): FAILED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:55:49 UTC, 3.2 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 5 servers × 8 clients, 20 records, 200 changes per client, seed 1
- Clients by role: 20 Registrant, 15 Reviewer, 5 Admin
- Data folder `stress-runs/sweep/sweep-5x8-r20-NaiveFile-1/data`; run folder `stress-runs/sweep/sweep-5x8-r20-NaiveFile-1`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 401 | 121 | 37.3 | 106 | 163 | 0 | 13.1 | 132.9 | 137.8 |

Writes by outcome: conflict 106, http500 11, saved 121, wrongStatus 163. Saves by action: Create 20, Update 1, Submit 1, Approve 1, Comment 98. Reads: 8000 (http500 115, notFound 7504).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | FAIL | 1/20 | 19 | 35 | 0 | 0 | 0 |
| server 38591 | FAIL | 1/20 | 19 | 35 | 0 | 0 | 0 |
| server 37293 | FAIL | 1/20 | 19 | 35 | 0 | 0 | 0 |
| server 46253 | FAIL | 1/20 | 19 | 35 | 0 | 0 | 0 |
| server 41317 | FAIL | 1/20 | 19 | 35 | 0 | 0 | 0 |
| server 44933 | FAIL | 1/20 | 19 | 35 | 0 | 0 | 0 |

storage:
- missing record ad7a9e49-02ec-4d7c-b269-0f0b4b867e11: missing (1 acknowledged changes)
- missing record 1b7bc6d9-ac10-469e-a159-4d1a58220c74: missing (1 acknowledged changes)
- missing record d2b48121-116e-4cb5-a1ab-1ac1385d918e: missing (1 acknowledged changes)
- missing record c8af5921-1af8-4bbb-9c4f-63d3d7207eff: missing (1 acknowledged changes)
- missing record 04f96ee4-e781-49a2-a881-fcd7d6c2bba4: missing (1 acknowledged changes)
- missing record 08a83d89-a4d5-4004-b250-e59008277a8c: missing (1 acknowledged changes)
- missing record ecd4317d-11a2-48e2-8d71-f1deafbbb631: missing (1 acknowledged changes)
- missing record bf63ae72-623a-41b4-8eef-7dd967e88060: missing (1 acknowledged changes)
- missing record 46f1b05f-ce13-4fbf-8544-22b067b2942b: missing (1 acknowledged changes)
- missing record b6f3966d-1cf4-4d1d-9c14-3076d3bd6e33: missing (1 acknowledged changes)
- missing record 5117474f-a03e-45bf-80aa-e9f1b936d58b: missing (1 acknowledged changes)
- missing record ef324af8-f60b-40ca-be55-b75bc1a89ad2: missing (1 acknowledged changes)
- missing record 0a193d63-d22a-4b30-940e-4d2da80a33fb: missing (1 acknowledged changes)
- missing record 0a6f7ca2-a77c-46e2-8e78-ad611c7143f0: missing (1 acknowledged changes)
- missing record 2c6f1965-3a4e-41f9-8279-eccd9a62b17b: missing (1 acknowledged changes)
- missing record ce21a387-ce39-46e6-a2b9-3542715e2aeb: missing (1 acknowledged changes)
- missing record 03f4c05b-85c6-4040-bdd4-ddfd2b2830a9: missing (1 acknowledged changes)
- missing record 17998c4d-7caa-4eef-9a93-e87f927df14f: missing (1 acknowledged changes)
- missing record a2e016e2-a99c-4289-a5ce-b69630bb8402: missing (1 acknowledged changes)
- lost update ad7a9e49-02ec-4d7c-b269-0f0b4b867e11 v1 Create by s1c1-registrant via 38591 (stress-5fab8c584a22418398aa27e4cd4f21e1): record missing
- lost update 1b7bc6d9-ac10-469e-a159-4d1a58220c74 v1 Create by s2c1-registrant via 37293 (stress-9d3cf918f208484798bdd44488a88450): record missing
- lost update d2b48121-116e-4cb5-a1ab-1ac1385d918e v1 Create by s3c1-registrant via 46253 (stress-94e0601294844fad93d0d0d5faef11de): record missing
- lost update c8af5921-1af8-4bbb-9c4f-63d3d7207eff v1 Create by s4c1-registrant via 41317 (stress-4af043a7d6ed4166b7444859c22a7b6c): record missing
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v56 Comment by s1c2-reviewer 's1c2-reviewer:150' via 38591 (stress-537fae6048b5466ba18a76dcd0cf6ad8): version 56 holds Comment by s3c7-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v64 Comment by s1c2-reviewer 's1c2-reviewer:172' via 38591 (stress-ffdf3bbfb07046728b1e8ff11a1d718f): version 64 holds Comment by s1c5-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v28 Comment by s1c4-admin 's1c4-admin:91' via 38591 (stress-620db14a86934cffac990a86359d9733): version 28 holds Comment by s1c2-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v37 Comment by s1c4-admin 's1c4-admin:119' via 38591 (stress-31a3f529e5af4e3282bf7c478537b63d): version 37 holds Comment by s2c4-admin
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v48 Comment by s1c7-reviewer 's1c7-reviewer:141' via 38591 (stress-794e3067877d440fb617eee724657809): version 48 holds Comment by s3c4-admin
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v60 Comment by s1c7-reviewer 's1c7-reviewer:176' via 38591 (stress-20f59bbd3efd4223b1600f5b569416ba): version 60 holds Comment by s3c4-admin
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v38 Comment by s2c2-reviewer 's2c2-reviewer:72' via 37293 (stress-8c95b374d5ec4447ab977c83c6a61c0f): version 38 holds Comment by s1c2-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v74 Comment by s2c2-reviewer 's2c2-reviewer:144' via 37293 (stress-2eddfbe841ca4b9691cc7866a2a893d1): version 74 holds Comment by s1c2-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v76 Comment by s2c7-reviewer 's2c7-reviewer:170' via 37293 (stress-85d9823c7c104f06965d374c858be452): version 76 holds Comment by s3c7-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v39 Comment by s3c4-admin 's3c4-admin:73' via 46253 (stress-a1f3d1755c594979a45e628cea59ed07): version 39 holds Comment by s5c2-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v62 Comment by s3c5-reviewer 's3c5-reviewer:141' via 46253 (stress-3d9c126496ca42f5a1b33ca3a347bf95): version 62 holds Comment by s1c2-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v78 Comment by s3c7-reviewer 's3c7-reviewer:163' via 46253 (stress-397ae6a458d34804b28ee7a2329b7d0e): version 78 holds Comment by s3c2-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v47 Comment by s4c2-reviewer 's4c2-reviewer:136' via 41317 (stress-85d8e460e8f145ea9f0f33d0796f5a46): version 47 holds Comment by s5c7-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v8 Update by s4c4-admin 's4c4-admin:36' via 41317 (stress-090598be7bf840569fba817c0112e01c): version 8 holds Comment by s1c2-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v10 Comment by s4c5-reviewer 's4c5-reviewer:46' via 41317 (stress-667f0e8336a44190b6b0f1bcc15e4b17): version 10 holds Comment by s4c2-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v44 Comment by s5c5-reviewer 's5c5-reviewer:99' via 44933 (stress-34b542982a734f7a907428f5795d8823): version 44 holds Comment by s1c5-reviewer

server 38591:
- missing record ad7a9e49-02ec-4d7c-b269-0f0b4b867e11: missing (1 acknowledged changes)
- missing record 1b7bc6d9-ac10-469e-a159-4d1a58220c74: missing (1 acknowledged changes)
- missing record d2b48121-116e-4cb5-a1ab-1ac1385d918e: missing (1 acknowledged changes)
- missing record c8af5921-1af8-4bbb-9c4f-63d3d7207eff: missing (1 acknowledged changes)
- missing record 04f96ee4-e781-49a2-a881-fcd7d6c2bba4: missing (1 acknowledged changes)
- missing record 08a83d89-a4d5-4004-b250-e59008277a8c: missing (1 acknowledged changes)
- missing record ecd4317d-11a2-48e2-8d71-f1deafbbb631: missing (1 acknowledged changes)
- missing record bf63ae72-623a-41b4-8eef-7dd967e88060: missing (1 acknowledged changes)
- missing record 46f1b05f-ce13-4fbf-8544-22b067b2942b: missing (1 acknowledged changes)
- missing record b6f3966d-1cf4-4d1d-9c14-3076d3bd6e33: missing (1 acknowledged changes)
- missing record 5117474f-a03e-45bf-80aa-e9f1b936d58b: missing (1 acknowledged changes)
- missing record ef324af8-f60b-40ca-be55-b75bc1a89ad2: missing (1 acknowledged changes)
- missing record 0a193d63-d22a-4b30-940e-4d2da80a33fb: missing (1 acknowledged changes)
- missing record 0a6f7ca2-a77c-46e2-8e78-ad611c7143f0: missing (1 acknowledged changes)
- missing record 2c6f1965-3a4e-41f9-8279-eccd9a62b17b: missing (1 acknowledged changes)
- missing record ce21a387-ce39-46e6-a2b9-3542715e2aeb: missing (1 acknowledged changes)
- missing record 03f4c05b-85c6-4040-bdd4-ddfd2b2830a9: missing (1 acknowledged changes)
- missing record 17998c4d-7caa-4eef-9a93-e87f927df14f: missing (1 acknowledged changes)
- missing record a2e016e2-a99c-4289-a5ce-b69630bb8402: missing (1 acknowledged changes)
- lost update ad7a9e49-02ec-4d7c-b269-0f0b4b867e11 v1 Create by s1c1-registrant via 38591 (stress-5fab8c584a22418398aa27e4cd4f21e1): record missing
- lost update 1b7bc6d9-ac10-469e-a159-4d1a58220c74 v1 Create by s2c1-registrant via 37293 (stress-9d3cf918f208484798bdd44488a88450): record missing
- lost update d2b48121-116e-4cb5-a1ab-1ac1385d918e v1 Create by s3c1-registrant via 46253 (stress-94e0601294844fad93d0d0d5faef11de): record missing
- lost update c8af5921-1af8-4bbb-9c4f-63d3d7207eff v1 Create by s4c1-registrant via 41317 (stress-4af043a7d6ed4166b7444859c22a7b6c): record missing
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v56 Comment by s1c2-reviewer 's1c2-reviewer:150' via 38591 (stress-537fae6048b5466ba18a76dcd0cf6ad8): version 56 holds Comment by s3c7-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v64 Comment by s1c2-reviewer 's1c2-reviewer:172' via 38591 (stress-ffdf3bbfb07046728b1e8ff11a1d718f): version 64 holds Comment by s1c5-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v28 Comment by s1c4-admin 's1c4-admin:91' via 38591 (stress-620db14a86934cffac990a86359d9733): version 28 holds Comment by s1c2-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v37 Comment by s1c4-admin 's1c4-admin:119' via 38591 (stress-31a3f529e5af4e3282bf7c478537b63d): version 37 holds Comment by s2c4-admin
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v48 Comment by s1c7-reviewer 's1c7-reviewer:141' via 38591 (stress-794e3067877d440fb617eee724657809): version 48 holds Comment by s3c4-admin
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v60 Comment by s1c7-reviewer 's1c7-reviewer:176' via 38591 (stress-20f59bbd3efd4223b1600f5b569416ba): version 60 holds Comment by s3c4-admin
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v38 Comment by s2c2-reviewer 's2c2-reviewer:72' via 37293 (stress-8c95b374d5ec4447ab977c83c6a61c0f): version 38 holds Comment by s1c2-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v74 Comment by s2c2-reviewer 's2c2-reviewer:144' via 37293 (stress-2eddfbe841ca4b9691cc7866a2a893d1): version 74 holds Comment by s1c2-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v76 Comment by s2c7-reviewer 's2c7-reviewer:170' via 37293 (stress-85d9823c7c104f06965d374c858be452): version 76 holds Comment by s3c7-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v39 Comment by s3c4-admin 's3c4-admin:73' via 46253 (stress-a1f3d1755c594979a45e628cea59ed07): version 39 holds Comment by s5c2-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v62 Comment by s3c5-reviewer 's3c5-reviewer:141' via 46253 (stress-3d9c126496ca42f5a1b33ca3a347bf95): version 62 holds Comment by s1c2-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v78 Comment by s3c7-reviewer 's3c7-reviewer:163' via 46253 (stress-397ae6a458d34804b28ee7a2329b7d0e): version 78 holds Comment by s3c2-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v47 Comment by s4c2-reviewer 's4c2-reviewer:136' via 41317 (stress-85d8e460e8f145ea9f0f33d0796f5a46): version 47 holds Comment by s5c7-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v8 Update by s4c4-admin 's4c4-admin:36' via 41317 (stress-090598be7bf840569fba817c0112e01c): version 8 holds Comment by s1c2-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v10 Comment by s4c5-reviewer 's4c5-reviewer:46' via 41317 (stress-667f0e8336a44190b6b0f1bcc15e4b17): version 10 holds Comment by s4c2-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v44 Comment by s5c5-reviewer 's5c5-reviewer:99' via 44933 (stress-34b542982a734f7a907428f5795d8823): version 44 holds Comment by s1c5-reviewer

server 37293:
- missing record ad7a9e49-02ec-4d7c-b269-0f0b4b867e11: missing (1 acknowledged changes)
- missing record 1b7bc6d9-ac10-469e-a159-4d1a58220c74: missing (1 acknowledged changes)
- missing record d2b48121-116e-4cb5-a1ab-1ac1385d918e: missing (1 acknowledged changes)
- missing record c8af5921-1af8-4bbb-9c4f-63d3d7207eff: missing (1 acknowledged changes)
- missing record 04f96ee4-e781-49a2-a881-fcd7d6c2bba4: missing (1 acknowledged changes)
- missing record 08a83d89-a4d5-4004-b250-e59008277a8c: missing (1 acknowledged changes)
- missing record ecd4317d-11a2-48e2-8d71-f1deafbbb631: missing (1 acknowledged changes)
- missing record bf63ae72-623a-41b4-8eef-7dd967e88060: missing (1 acknowledged changes)
- missing record 46f1b05f-ce13-4fbf-8544-22b067b2942b: missing (1 acknowledged changes)
- missing record b6f3966d-1cf4-4d1d-9c14-3076d3bd6e33: missing (1 acknowledged changes)
- missing record 5117474f-a03e-45bf-80aa-e9f1b936d58b: missing (1 acknowledged changes)
- missing record ef324af8-f60b-40ca-be55-b75bc1a89ad2: missing (1 acknowledged changes)
- missing record 0a193d63-d22a-4b30-940e-4d2da80a33fb: missing (1 acknowledged changes)
- missing record 0a6f7ca2-a77c-46e2-8e78-ad611c7143f0: missing (1 acknowledged changes)
- missing record 2c6f1965-3a4e-41f9-8279-eccd9a62b17b: missing (1 acknowledged changes)
- missing record ce21a387-ce39-46e6-a2b9-3542715e2aeb: missing (1 acknowledged changes)
- missing record 03f4c05b-85c6-4040-bdd4-ddfd2b2830a9: missing (1 acknowledged changes)
- missing record 17998c4d-7caa-4eef-9a93-e87f927df14f: missing (1 acknowledged changes)
- missing record a2e016e2-a99c-4289-a5ce-b69630bb8402: missing (1 acknowledged changes)
- lost update ad7a9e49-02ec-4d7c-b269-0f0b4b867e11 v1 Create by s1c1-registrant via 38591 (stress-5fab8c584a22418398aa27e4cd4f21e1): record missing
- lost update 1b7bc6d9-ac10-469e-a159-4d1a58220c74 v1 Create by s2c1-registrant via 37293 (stress-9d3cf918f208484798bdd44488a88450): record missing
- lost update d2b48121-116e-4cb5-a1ab-1ac1385d918e v1 Create by s3c1-registrant via 46253 (stress-94e0601294844fad93d0d0d5faef11de): record missing
- lost update c8af5921-1af8-4bbb-9c4f-63d3d7207eff v1 Create by s4c1-registrant via 41317 (stress-4af043a7d6ed4166b7444859c22a7b6c): record missing
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v56 Comment by s1c2-reviewer 's1c2-reviewer:150' via 38591 (stress-537fae6048b5466ba18a76dcd0cf6ad8): version 56 holds Comment by s3c7-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v64 Comment by s1c2-reviewer 's1c2-reviewer:172' via 38591 (stress-ffdf3bbfb07046728b1e8ff11a1d718f): version 64 holds Comment by s1c5-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v28 Comment by s1c4-admin 's1c4-admin:91' via 38591 (stress-620db14a86934cffac990a86359d9733): version 28 holds Comment by s1c2-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v37 Comment by s1c4-admin 's1c4-admin:119' via 38591 (stress-31a3f529e5af4e3282bf7c478537b63d): version 37 holds Comment by s2c4-admin
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v48 Comment by s1c7-reviewer 's1c7-reviewer:141' via 38591 (stress-794e3067877d440fb617eee724657809): version 48 holds Comment by s3c4-admin
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v60 Comment by s1c7-reviewer 's1c7-reviewer:176' via 38591 (stress-20f59bbd3efd4223b1600f5b569416ba): version 60 holds Comment by s3c4-admin
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v38 Comment by s2c2-reviewer 's2c2-reviewer:72' via 37293 (stress-8c95b374d5ec4447ab977c83c6a61c0f): version 38 holds Comment by s1c2-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v74 Comment by s2c2-reviewer 's2c2-reviewer:144' via 37293 (stress-2eddfbe841ca4b9691cc7866a2a893d1): version 74 holds Comment by s1c2-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v76 Comment by s2c7-reviewer 's2c7-reviewer:170' via 37293 (stress-85d9823c7c104f06965d374c858be452): version 76 holds Comment by s3c7-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v39 Comment by s3c4-admin 's3c4-admin:73' via 46253 (stress-a1f3d1755c594979a45e628cea59ed07): version 39 holds Comment by s5c2-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v62 Comment by s3c5-reviewer 's3c5-reviewer:141' via 46253 (stress-3d9c126496ca42f5a1b33ca3a347bf95): version 62 holds Comment by s1c2-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v78 Comment by s3c7-reviewer 's3c7-reviewer:163' via 46253 (stress-397ae6a458d34804b28ee7a2329b7d0e): version 78 holds Comment by s3c2-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v47 Comment by s4c2-reviewer 's4c2-reviewer:136' via 41317 (stress-85d8e460e8f145ea9f0f33d0796f5a46): version 47 holds Comment by s5c7-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v8 Update by s4c4-admin 's4c4-admin:36' via 41317 (stress-090598be7bf840569fba817c0112e01c): version 8 holds Comment by s1c2-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v10 Comment by s4c5-reviewer 's4c5-reviewer:46' via 41317 (stress-667f0e8336a44190b6b0f1bcc15e4b17): version 10 holds Comment by s4c2-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v44 Comment by s5c5-reviewer 's5c5-reviewer:99' via 44933 (stress-34b542982a734f7a907428f5795d8823): version 44 holds Comment by s1c5-reviewer

server 46253:
- missing record ad7a9e49-02ec-4d7c-b269-0f0b4b867e11: missing (1 acknowledged changes)
- missing record 1b7bc6d9-ac10-469e-a159-4d1a58220c74: missing (1 acknowledged changes)
- missing record d2b48121-116e-4cb5-a1ab-1ac1385d918e: missing (1 acknowledged changes)
- missing record c8af5921-1af8-4bbb-9c4f-63d3d7207eff: missing (1 acknowledged changes)
- missing record 04f96ee4-e781-49a2-a881-fcd7d6c2bba4: missing (1 acknowledged changes)
- missing record 08a83d89-a4d5-4004-b250-e59008277a8c: missing (1 acknowledged changes)
- missing record ecd4317d-11a2-48e2-8d71-f1deafbbb631: missing (1 acknowledged changes)
- missing record bf63ae72-623a-41b4-8eef-7dd967e88060: missing (1 acknowledged changes)
- missing record 46f1b05f-ce13-4fbf-8544-22b067b2942b: missing (1 acknowledged changes)
- missing record b6f3966d-1cf4-4d1d-9c14-3076d3bd6e33: missing (1 acknowledged changes)
- missing record 5117474f-a03e-45bf-80aa-e9f1b936d58b: missing (1 acknowledged changes)
- missing record ef324af8-f60b-40ca-be55-b75bc1a89ad2: missing (1 acknowledged changes)
- missing record 0a193d63-d22a-4b30-940e-4d2da80a33fb: missing (1 acknowledged changes)
- missing record 0a6f7ca2-a77c-46e2-8e78-ad611c7143f0: missing (1 acknowledged changes)
- missing record 2c6f1965-3a4e-41f9-8279-eccd9a62b17b: missing (1 acknowledged changes)
- missing record ce21a387-ce39-46e6-a2b9-3542715e2aeb: missing (1 acknowledged changes)
- missing record 03f4c05b-85c6-4040-bdd4-ddfd2b2830a9: missing (1 acknowledged changes)
- missing record 17998c4d-7caa-4eef-9a93-e87f927df14f: missing (1 acknowledged changes)
- missing record a2e016e2-a99c-4289-a5ce-b69630bb8402: missing (1 acknowledged changes)
- lost update ad7a9e49-02ec-4d7c-b269-0f0b4b867e11 v1 Create by s1c1-registrant via 38591 (stress-5fab8c584a22418398aa27e4cd4f21e1): record missing
- lost update 1b7bc6d9-ac10-469e-a159-4d1a58220c74 v1 Create by s2c1-registrant via 37293 (stress-9d3cf918f208484798bdd44488a88450): record missing
- lost update d2b48121-116e-4cb5-a1ab-1ac1385d918e v1 Create by s3c1-registrant via 46253 (stress-94e0601294844fad93d0d0d5faef11de): record missing
- lost update c8af5921-1af8-4bbb-9c4f-63d3d7207eff v1 Create by s4c1-registrant via 41317 (stress-4af043a7d6ed4166b7444859c22a7b6c): record missing
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v56 Comment by s1c2-reviewer 's1c2-reviewer:150' via 38591 (stress-537fae6048b5466ba18a76dcd0cf6ad8): version 56 holds Comment by s3c7-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v64 Comment by s1c2-reviewer 's1c2-reviewer:172' via 38591 (stress-ffdf3bbfb07046728b1e8ff11a1d718f): version 64 holds Comment by s1c5-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v28 Comment by s1c4-admin 's1c4-admin:91' via 38591 (stress-620db14a86934cffac990a86359d9733): version 28 holds Comment by s1c2-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v37 Comment by s1c4-admin 's1c4-admin:119' via 38591 (stress-31a3f529e5af4e3282bf7c478537b63d): version 37 holds Comment by s2c4-admin
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v48 Comment by s1c7-reviewer 's1c7-reviewer:141' via 38591 (stress-794e3067877d440fb617eee724657809): version 48 holds Comment by s3c4-admin
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v60 Comment by s1c7-reviewer 's1c7-reviewer:176' via 38591 (stress-20f59bbd3efd4223b1600f5b569416ba): version 60 holds Comment by s3c4-admin
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v38 Comment by s2c2-reviewer 's2c2-reviewer:72' via 37293 (stress-8c95b374d5ec4447ab977c83c6a61c0f): version 38 holds Comment by s1c2-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v74 Comment by s2c2-reviewer 's2c2-reviewer:144' via 37293 (stress-2eddfbe841ca4b9691cc7866a2a893d1): version 74 holds Comment by s1c2-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v76 Comment by s2c7-reviewer 's2c7-reviewer:170' via 37293 (stress-85d9823c7c104f06965d374c858be452): version 76 holds Comment by s3c7-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v39 Comment by s3c4-admin 's3c4-admin:73' via 46253 (stress-a1f3d1755c594979a45e628cea59ed07): version 39 holds Comment by s5c2-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v62 Comment by s3c5-reviewer 's3c5-reviewer:141' via 46253 (stress-3d9c126496ca42f5a1b33ca3a347bf95): version 62 holds Comment by s1c2-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v78 Comment by s3c7-reviewer 's3c7-reviewer:163' via 46253 (stress-397ae6a458d34804b28ee7a2329b7d0e): version 78 holds Comment by s3c2-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v47 Comment by s4c2-reviewer 's4c2-reviewer:136' via 41317 (stress-85d8e460e8f145ea9f0f33d0796f5a46): version 47 holds Comment by s5c7-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v8 Update by s4c4-admin 's4c4-admin:36' via 41317 (stress-090598be7bf840569fba817c0112e01c): version 8 holds Comment by s1c2-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v10 Comment by s4c5-reviewer 's4c5-reviewer:46' via 41317 (stress-667f0e8336a44190b6b0f1bcc15e4b17): version 10 holds Comment by s4c2-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v44 Comment by s5c5-reviewer 's5c5-reviewer:99' via 44933 (stress-34b542982a734f7a907428f5795d8823): version 44 holds Comment by s1c5-reviewer

server 41317:
- missing record ad7a9e49-02ec-4d7c-b269-0f0b4b867e11: missing (1 acknowledged changes)
- missing record 1b7bc6d9-ac10-469e-a159-4d1a58220c74: missing (1 acknowledged changes)
- missing record d2b48121-116e-4cb5-a1ab-1ac1385d918e: missing (1 acknowledged changes)
- missing record c8af5921-1af8-4bbb-9c4f-63d3d7207eff: missing (1 acknowledged changes)
- missing record 04f96ee4-e781-49a2-a881-fcd7d6c2bba4: missing (1 acknowledged changes)
- missing record 08a83d89-a4d5-4004-b250-e59008277a8c: missing (1 acknowledged changes)
- missing record ecd4317d-11a2-48e2-8d71-f1deafbbb631: missing (1 acknowledged changes)
- missing record bf63ae72-623a-41b4-8eef-7dd967e88060: missing (1 acknowledged changes)
- missing record 46f1b05f-ce13-4fbf-8544-22b067b2942b: missing (1 acknowledged changes)
- missing record b6f3966d-1cf4-4d1d-9c14-3076d3bd6e33: missing (1 acknowledged changes)
- missing record 5117474f-a03e-45bf-80aa-e9f1b936d58b: missing (1 acknowledged changes)
- missing record ef324af8-f60b-40ca-be55-b75bc1a89ad2: missing (1 acknowledged changes)
- missing record 0a193d63-d22a-4b30-940e-4d2da80a33fb: missing (1 acknowledged changes)
- missing record 0a6f7ca2-a77c-46e2-8e78-ad611c7143f0: missing (1 acknowledged changes)
- missing record 2c6f1965-3a4e-41f9-8279-eccd9a62b17b: missing (1 acknowledged changes)
- missing record ce21a387-ce39-46e6-a2b9-3542715e2aeb: missing (1 acknowledged changes)
- missing record 03f4c05b-85c6-4040-bdd4-ddfd2b2830a9: missing (1 acknowledged changes)
- missing record 17998c4d-7caa-4eef-9a93-e87f927df14f: missing (1 acknowledged changes)
- missing record a2e016e2-a99c-4289-a5ce-b69630bb8402: missing (1 acknowledged changes)
- lost update ad7a9e49-02ec-4d7c-b269-0f0b4b867e11 v1 Create by s1c1-registrant via 38591 (stress-5fab8c584a22418398aa27e4cd4f21e1): record missing
- lost update 1b7bc6d9-ac10-469e-a159-4d1a58220c74 v1 Create by s2c1-registrant via 37293 (stress-9d3cf918f208484798bdd44488a88450): record missing
- lost update d2b48121-116e-4cb5-a1ab-1ac1385d918e v1 Create by s3c1-registrant via 46253 (stress-94e0601294844fad93d0d0d5faef11de): record missing
- lost update c8af5921-1af8-4bbb-9c4f-63d3d7207eff v1 Create by s4c1-registrant via 41317 (stress-4af043a7d6ed4166b7444859c22a7b6c): record missing
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v56 Comment by s1c2-reviewer 's1c2-reviewer:150' via 38591 (stress-537fae6048b5466ba18a76dcd0cf6ad8): version 56 holds Comment by s3c7-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v64 Comment by s1c2-reviewer 's1c2-reviewer:172' via 38591 (stress-ffdf3bbfb07046728b1e8ff11a1d718f): version 64 holds Comment by s1c5-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v28 Comment by s1c4-admin 's1c4-admin:91' via 38591 (stress-620db14a86934cffac990a86359d9733): version 28 holds Comment by s1c2-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v37 Comment by s1c4-admin 's1c4-admin:119' via 38591 (stress-31a3f529e5af4e3282bf7c478537b63d): version 37 holds Comment by s2c4-admin
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v48 Comment by s1c7-reviewer 's1c7-reviewer:141' via 38591 (stress-794e3067877d440fb617eee724657809): version 48 holds Comment by s3c4-admin
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v60 Comment by s1c7-reviewer 's1c7-reviewer:176' via 38591 (stress-20f59bbd3efd4223b1600f5b569416ba): version 60 holds Comment by s3c4-admin
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v38 Comment by s2c2-reviewer 's2c2-reviewer:72' via 37293 (stress-8c95b374d5ec4447ab977c83c6a61c0f): version 38 holds Comment by s1c2-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v74 Comment by s2c2-reviewer 's2c2-reviewer:144' via 37293 (stress-2eddfbe841ca4b9691cc7866a2a893d1): version 74 holds Comment by s1c2-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v76 Comment by s2c7-reviewer 's2c7-reviewer:170' via 37293 (stress-85d9823c7c104f06965d374c858be452): version 76 holds Comment by s3c7-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v39 Comment by s3c4-admin 's3c4-admin:73' via 46253 (stress-a1f3d1755c594979a45e628cea59ed07): version 39 holds Comment by s5c2-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v62 Comment by s3c5-reviewer 's3c5-reviewer:141' via 46253 (stress-3d9c126496ca42f5a1b33ca3a347bf95): version 62 holds Comment by s1c2-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v78 Comment by s3c7-reviewer 's3c7-reviewer:163' via 46253 (stress-397ae6a458d34804b28ee7a2329b7d0e): version 78 holds Comment by s3c2-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v47 Comment by s4c2-reviewer 's4c2-reviewer:136' via 41317 (stress-85d8e460e8f145ea9f0f33d0796f5a46): version 47 holds Comment by s5c7-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v8 Update by s4c4-admin 's4c4-admin:36' via 41317 (stress-090598be7bf840569fba817c0112e01c): version 8 holds Comment by s1c2-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v10 Comment by s4c5-reviewer 's4c5-reviewer:46' via 41317 (stress-667f0e8336a44190b6b0f1bcc15e4b17): version 10 holds Comment by s4c2-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v44 Comment by s5c5-reviewer 's5c5-reviewer:99' via 44933 (stress-34b542982a734f7a907428f5795d8823): version 44 holds Comment by s1c5-reviewer

server 44933:
- missing record ad7a9e49-02ec-4d7c-b269-0f0b4b867e11: missing (1 acknowledged changes)
- missing record 1b7bc6d9-ac10-469e-a159-4d1a58220c74: missing (1 acknowledged changes)
- missing record d2b48121-116e-4cb5-a1ab-1ac1385d918e: missing (1 acknowledged changes)
- missing record c8af5921-1af8-4bbb-9c4f-63d3d7207eff: missing (1 acknowledged changes)
- missing record 04f96ee4-e781-49a2-a881-fcd7d6c2bba4: missing (1 acknowledged changes)
- missing record 08a83d89-a4d5-4004-b250-e59008277a8c: missing (1 acknowledged changes)
- missing record ecd4317d-11a2-48e2-8d71-f1deafbbb631: missing (1 acknowledged changes)
- missing record bf63ae72-623a-41b4-8eef-7dd967e88060: missing (1 acknowledged changes)
- missing record 46f1b05f-ce13-4fbf-8544-22b067b2942b: missing (1 acknowledged changes)
- missing record b6f3966d-1cf4-4d1d-9c14-3076d3bd6e33: missing (1 acknowledged changes)
- missing record 5117474f-a03e-45bf-80aa-e9f1b936d58b: missing (1 acknowledged changes)
- missing record ef324af8-f60b-40ca-be55-b75bc1a89ad2: missing (1 acknowledged changes)
- missing record 0a193d63-d22a-4b30-940e-4d2da80a33fb: missing (1 acknowledged changes)
- missing record 0a6f7ca2-a77c-46e2-8e78-ad611c7143f0: missing (1 acknowledged changes)
- missing record 2c6f1965-3a4e-41f9-8279-eccd9a62b17b: missing (1 acknowledged changes)
- missing record ce21a387-ce39-46e6-a2b9-3542715e2aeb: missing (1 acknowledged changes)
- missing record 03f4c05b-85c6-4040-bdd4-ddfd2b2830a9: missing (1 acknowledged changes)
- missing record 17998c4d-7caa-4eef-9a93-e87f927df14f: missing (1 acknowledged changes)
- missing record a2e016e2-a99c-4289-a5ce-b69630bb8402: missing (1 acknowledged changes)
- lost update ad7a9e49-02ec-4d7c-b269-0f0b4b867e11 v1 Create by s1c1-registrant via 38591 (stress-5fab8c584a22418398aa27e4cd4f21e1): record missing
- lost update 1b7bc6d9-ac10-469e-a159-4d1a58220c74 v1 Create by s2c1-registrant via 37293 (stress-9d3cf918f208484798bdd44488a88450): record missing
- lost update d2b48121-116e-4cb5-a1ab-1ac1385d918e v1 Create by s3c1-registrant via 46253 (stress-94e0601294844fad93d0d0d5faef11de): record missing
- lost update c8af5921-1af8-4bbb-9c4f-63d3d7207eff v1 Create by s4c1-registrant via 41317 (stress-4af043a7d6ed4166b7444859c22a7b6c): record missing
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v56 Comment by s1c2-reviewer 's1c2-reviewer:150' via 38591 (stress-537fae6048b5466ba18a76dcd0cf6ad8): version 56 holds Comment by s3c7-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v64 Comment by s1c2-reviewer 's1c2-reviewer:172' via 38591 (stress-ffdf3bbfb07046728b1e8ff11a1d718f): version 64 holds Comment by s1c5-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v28 Comment by s1c4-admin 's1c4-admin:91' via 38591 (stress-620db14a86934cffac990a86359d9733): version 28 holds Comment by s1c2-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v37 Comment by s1c4-admin 's1c4-admin:119' via 38591 (stress-31a3f529e5af4e3282bf7c478537b63d): version 37 holds Comment by s2c4-admin
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v48 Comment by s1c7-reviewer 's1c7-reviewer:141' via 38591 (stress-794e3067877d440fb617eee724657809): version 48 holds Comment by s3c4-admin
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v60 Comment by s1c7-reviewer 's1c7-reviewer:176' via 38591 (stress-20f59bbd3efd4223b1600f5b569416ba): version 60 holds Comment by s3c4-admin
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v38 Comment by s2c2-reviewer 's2c2-reviewer:72' via 37293 (stress-8c95b374d5ec4447ab977c83c6a61c0f): version 38 holds Comment by s1c2-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v74 Comment by s2c2-reviewer 's2c2-reviewer:144' via 37293 (stress-2eddfbe841ca4b9691cc7866a2a893d1): version 74 holds Comment by s1c2-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v76 Comment by s2c7-reviewer 's2c7-reviewer:170' via 37293 (stress-85d9823c7c104f06965d374c858be452): version 76 holds Comment by s3c7-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v39 Comment by s3c4-admin 's3c4-admin:73' via 46253 (stress-a1f3d1755c594979a45e628cea59ed07): version 39 holds Comment by s5c2-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v62 Comment by s3c5-reviewer 's3c5-reviewer:141' via 46253 (stress-3d9c126496ca42f5a1b33ca3a347bf95): version 62 holds Comment by s1c2-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v78 Comment by s3c7-reviewer 's3c7-reviewer:163' via 46253 (stress-397ae6a458d34804b28ee7a2329b7d0e): version 78 holds Comment by s3c2-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v47 Comment by s4c2-reviewer 's4c2-reviewer:136' via 41317 (stress-85d8e460e8f145ea9f0f33d0796f5a46): version 47 holds Comment by s5c7-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v8 Update by s4c4-admin 's4c4-admin:36' via 41317 (stress-090598be7bf840569fba817c0112e01c): version 8 holds Comment by s1c2-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v10 Comment by s4c5-reviewer 's4c5-reviewer:46' via 41317 (stress-667f0e8336a44190b6b0f1bcc15e4b17): version 10 holds Comment by s4c2-reviewer
- lost update 2cfc5a03-f8ae-4881-8fc3-1bfe8a728182 v44 Comment by s5c5-reviewer 's5c5-reviewer:99' via 44933 (stress-34b542982a734f7a907428f5795d8823): version 44 holds Comment by s1c5-reviewer

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 5 | 121 | 37.3 | 106 | 163 | 35 | 0 | 0 | 132.9 | no |
