# Stress run: NaiveFile, 3 server(s): FAILED

- Commit `ee6af83bef739621009cd4be91192b48f8df58df`, 2026-10-17 19:07:33 UTC, 0.4 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 5 clients, 6 records, 40 changes per client, seed 3
- Data folder `stress-runs/light-NaiveFile-3/data`; run folder `stress-runs/light-NaiveFile-3`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 93 | 39 | 93.4 | 25 | 26 | 0 | 2.7 | 102.1 | 102.3 |

Writes by outcome: conflict 25, http500 3, saved 39, wrongStatus 26. Saves by action: Create 6, Update 4, Submit 2, Reject 1, Comment 26. Reads: 600 (http500 4, notFound 509).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | FAIL | 1/6 | 5 | 16 | 0 | 0 | 0 |
| server 39295 | FAIL | 1/6 | 5 | 16 | 0 | 0 | 0 |
| server 45253 | FAIL | 1/6 | 5 | 16 | 0 | 0 | 0 |
| server 41195 | FAIL | 1/6 | 5 | 16 | 0 | 0 | 0 |

storage:
- missing record e098fe7a-635a-4753-b31b-d1238605543a: missing (1 acknowledged changes)
- missing record 474d9f24-8be5-4f82-a76c-b534e8d66bca: missing (1 acknowledged changes)
- missing record 1cf8a242-05df-49de-b81a-da2d89acd7a6: missing (1 acknowledged changes)
- missing record 06e19c9b-e73c-4e37-a037-e0a167fecdf6: missing (1 acknowledged changes)
- missing record b8671b10-276d-4b1f-96be-cb9a55fc059d: missing (1 acknowledged changes)
- lost update e098fe7a-635a-4753-b31b-d1238605543a v1 Create by s1c1-registrant via 39295 (stress-2d6e1c59abe54f95abd844d2ade0087b): record missing
- lost update 8bfcbfe7-ddfd-4295-b176-a23221df44b6 v2 Comment by s1c2-reviewer 's1c2-reviewer:10' via 39295 (stress-57bbcc8b951546a98f337b6c8001b351): version 2 holds Update by s2c1-registrant
- lost update 8bfcbfe7-ddfd-4295-b176-a23221df44b6 v2 Update by s1c4-admin 's1c4-admin:14' via 39295 (stress-46645aea3f4442faa1fe2c87d8f3852f): version 2 holds Update by s2c1-registrant
- lost update 8bfcbfe7-ddfd-4295-b176-a23221df44b6 v2 Comment by s1c5-reviewer 's1c5-reviewer:22' via 39295 (stress-0b6ad08a6a2546499d6f03777fae59ab): version 2 holds Update by s2c1-registrant
- lost update 8bfcbfe7-ddfd-4295-b176-a23221df44b6 v2 Comment by s2c2-reviewer 's2c2-reviewer:4' via 45253 (stress-1731429f09424256ac4891d53d4fe2ff): version 2 holds Update by s2c1-registrant
- lost update 8bfcbfe7-ddfd-4295-b176-a23221df44b6 v9 Comment by s2c2-reviewer 's2c2-reviewer:19' via 45253 (stress-44fad814d0ff43ea98cd330f5bc8d169): version 9 holds Comment by s2c5-reviewer
- lost update 8bfcbfe7-ddfd-4295-b176-a23221df44b6 v13 Comment by s2c2-reviewer 's2c2-reviewer:25' via 45253 (stress-d85e7961b19f43c6952319d2a8923432): version 13 holds Comment by s2c4-admin
- lost update 8bfcbfe7-ddfd-4295-b176-a23221df44b6 v2 Submit by s2c4-admin via 45253 (stress-8fe3366d5e96430aa8962ffaba2a915d): version 2 holds Update by s2c1-registrant
- lost update 8bfcbfe7-ddfd-4295-b176-a23221df44b6 v12 Comment by s2c4-admin 's2c4-admin:22' via 45253 (stress-6b44129a946846c79a6de0e1ce87d871): version 12 holds Comment by s3c4-admin
- lost update 8bfcbfe7-ddfd-4295-b176-a23221df44b6 v2 Comment by s2c5-reviewer 's2c5-reviewer:6' via 45253 (stress-6284fa6cc2194bed9e2af3e31ba1d80e): version 2 holds Update by s2c1-registrant
- lost update 8bfcbfe7-ddfd-4295-b176-a23221df44b6 v13 Comment by s2c5-reviewer 's2c5-reviewer:27' via 45253 (stress-0d9651f841884373afcca8df660ebe8c): version 13 holds Comment by s2c4-admin
- lost update 8bfcbfe7-ddfd-4295-b176-a23221df44b6 v4 Comment by s3c2-reviewer 's3c2-reviewer:10' via 41195 (stress-e1d486cdb6f04ca49af7a6773f34c9d7): version 4 holds Update by s2c4-admin
- lost update 474d9f24-8be5-4f82-a76c-b534e8d66bca v1 Create by s3c1-registrant via 41195 (stress-afefd8864ef24f8c8a2309622a770b07): record missing
- lost update 1cf8a242-05df-49de-b81a-da2d89acd7a6 v1 Create by s1c3-registrant via 39295 (stress-36ea5c834c7b4f8996d29c3f8644621a): record missing
- lost update 06e19c9b-e73c-4e37-a037-e0a167fecdf6 v1 Create by s2c3-registrant via 45253 (stress-b9f0dbce017348a3918f592391481c97): record missing
- lost update b8671b10-276d-4b1f-96be-cb9a55fc059d v1 Create by s3c3-registrant via 41195 (stress-dd0ec0b79c7b4c41bb9ee56f89dae14e): record missing

server 39295:
- missing record e098fe7a-635a-4753-b31b-d1238605543a: missing (1 acknowledged changes)
- missing record 474d9f24-8be5-4f82-a76c-b534e8d66bca: missing (1 acknowledged changes)
- missing record 1cf8a242-05df-49de-b81a-da2d89acd7a6: missing (1 acknowledged changes)
- missing record 06e19c9b-e73c-4e37-a037-e0a167fecdf6: missing (1 acknowledged changes)
- missing record b8671b10-276d-4b1f-96be-cb9a55fc059d: missing (1 acknowledged changes)
- lost update e098fe7a-635a-4753-b31b-d1238605543a v1 Create by s1c1-registrant via 39295 (stress-2d6e1c59abe54f95abd844d2ade0087b): record missing
- lost update 8bfcbfe7-ddfd-4295-b176-a23221df44b6 v2 Comment by s1c2-reviewer 's1c2-reviewer:10' via 39295 (stress-57bbcc8b951546a98f337b6c8001b351): version 2 holds Update by s2c1-registrant
- lost update 8bfcbfe7-ddfd-4295-b176-a23221df44b6 v2 Update by s1c4-admin 's1c4-admin:14' via 39295 (stress-46645aea3f4442faa1fe2c87d8f3852f): version 2 holds Update by s2c1-registrant
- lost update 8bfcbfe7-ddfd-4295-b176-a23221df44b6 v2 Comment by s1c5-reviewer 's1c5-reviewer:22' via 39295 (stress-0b6ad08a6a2546499d6f03777fae59ab): version 2 holds Update by s2c1-registrant
- lost update 8bfcbfe7-ddfd-4295-b176-a23221df44b6 v2 Comment by s2c2-reviewer 's2c2-reviewer:4' via 45253 (stress-1731429f09424256ac4891d53d4fe2ff): version 2 holds Update by s2c1-registrant
- lost update 8bfcbfe7-ddfd-4295-b176-a23221df44b6 v9 Comment by s2c2-reviewer 's2c2-reviewer:19' via 45253 (stress-44fad814d0ff43ea98cd330f5bc8d169): version 9 holds Comment by s2c5-reviewer
- lost update 8bfcbfe7-ddfd-4295-b176-a23221df44b6 v13 Comment by s2c2-reviewer 's2c2-reviewer:25' via 45253 (stress-d85e7961b19f43c6952319d2a8923432): version 13 holds Comment by s2c4-admin
- lost update 8bfcbfe7-ddfd-4295-b176-a23221df44b6 v2 Submit by s2c4-admin via 45253 (stress-8fe3366d5e96430aa8962ffaba2a915d): version 2 holds Update by s2c1-registrant
- lost update 8bfcbfe7-ddfd-4295-b176-a23221df44b6 v12 Comment by s2c4-admin 's2c4-admin:22' via 45253 (stress-6b44129a946846c79a6de0e1ce87d871): version 12 holds Comment by s3c4-admin
- lost update 8bfcbfe7-ddfd-4295-b176-a23221df44b6 v2 Comment by s2c5-reviewer 's2c5-reviewer:6' via 45253 (stress-6284fa6cc2194bed9e2af3e31ba1d80e): version 2 holds Update by s2c1-registrant
- lost update 8bfcbfe7-ddfd-4295-b176-a23221df44b6 v13 Comment by s2c5-reviewer 's2c5-reviewer:27' via 45253 (stress-0d9651f841884373afcca8df660ebe8c): version 13 holds Comment by s2c4-admin
- lost update 8bfcbfe7-ddfd-4295-b176-a23221df44b6 v4 Comment by s3c2-reviewer 's3c2-reviewer:10' via 41195 (stress-e1d486cdb6f04ca49af7a6773f34c9d7): version 4 holds Update by s2c4-admin
- lost update 474d9f24-8be5-4f82-a76c-b534e8d66bca v1 Create by s3c1-registrant via 41195 (stress-afefd8864ef24f8c8a2309622a770b07): record missing
- lost update 1cf8a242-05df-49de-b81a-da2d89acd7a6 v1 Create by s1c3-registrant via 39295 (stress-36ea5c834c7b4f8996d29c3f8644621a): record missing
- lost update 06e19c9b-e73c-4e37-a037-e0a167fecdf6 v1 Create by s2c3-registrant via 45253 (stress-b9f0dbce017348a3918f592391481c97): record missing
- lost update b8671b10-276d-4b1f-96be-cb9a55fc059d v1 Create by s3c3-registrant via 41195 (stress-dd0ec0b79c7b4c41bb9ee56f89dae14e): record missing

server 45253:
- missing record e098fe7a-635a-4753-b31b-d1238605543a: missing (1 acknowledged changes)
- missing record 474d9f24-8be5-4f82-a76c-b534e8d66bca: missing (1 acknowledged changes)
- missing record 1cf8a242-05df-49de-b81a-da2d89acd7a6: missing (1 acknowledged changes)
- missing record 06e19c9b-e73c-4e37-a037-e0a167fecdf6: missing (1 acknowledged changes)
- missing record b8671b10-276d-4b1f-96be-cb9a55fc059d: missing (1 acknowledged changes)
- lost update e098fe7a-635a-4753-b31b-d1238605543a v1 Create by s1c1-registrant via 39295 (stress-2d6e1c59abe54f95abd844d2ade0087b): record missing
- lost update 8bfcbfe7-ddfd-4295-b176-a23221df44b6 v2 Comment by s1c2-reviewer 's1c2-reviewer:10' via 39295 (stress-57bbcc8b951546a98f337b6c8001b351): version 2 holds Update by s2c1-registrant
- lost update 8bfcbfe7-ddfd-4295-b176-a23221df44b6 v2 Update by s1c4-admin 's1c4-admin:14' via 39295 (stress-46645aea3f4442faa1fe2c87d8f3852f): version 2 holds Update by s2c1-registrant
- lost update 8bfcbfe7-ddfd-4295-b176-a23221df44b6 v2 Comment by s1c5-reviewer 's1c5-reviewer:22' via 39295 (stress-0b6ad08a6a2546499d6f03777fae59ab): version 2 holds Update by s2c1-registrant
- lost update 8bfcbfe7-ddfd-4295-b176-a23221df44b6 v2 Comment by s2c2-reviewer 's2c2-reviewer:4' via 45253 (stress-1731429f09424256ac4891d53d4fe2ff): version 2 holds Update by s2c1-registrant
- lost update 8bfcbfe7-ddfd-4295-b176-a23221df44b6 v9 Comment by s2c2-reviewer 's2c2-reviewer:19' via 45253 (stress-44fad814d0ff43ea98cd330f5bc8d169): version 9 holds Comment by s2c5-reviewer
- lost update 8bfcbfe7-ddfd-4295-b176-a23221df44b6 v13 Comment by s2c2-reviewer 's2c2-reviewer:25' via 45253 (stress-d85e7961b19f43c6952319d2a8923432): version 13 holds Comment by s2c4-admin
- lost update 8bfcbfe7-ddfd-4295-b176-a23221df44b6 v2 Submit by s2c4-admin via 45253 (stress-8fe3366d5e96430aa8962ffaba2a915d): version 2 holds Update by s2c1-registrant
- lost update 8bfcbfe7-ddfd-4295-b176-a23221df44b6 v12 Comment by s2c4-admin 's2c4-admin:22' via 45253 (stress-6b44129a946846c79a6de0e1ce87d871): version 12 holds Comment by s3c4-admin
- lost update 8bfcbfe7-ddfd-4295-b176-a23221df44b6 v2 Comment by s2c5-reviewer 's2c5-reviewer:6' via 45253 (stress-6284fa6cc2194bed9e2af3e31ba1d80e): version 2 holds Update by s2c1-registrant
- lost update 8bfcbfe7-ddfd-4295-b176-a23221df44b6 v13 Comment by s2c5-reviewer 's2c5-reviewer:27' via 45253 (stress-0d9651f841884373afcca8df660ebe8c): version 13 holds Comment by s2c4-admin
- lost update 8bfcbfe7-ddfd-4295-b176-a23221df44b6 v4 Comment by s3c2-reviewer 's3c2-reviewer:10' via 41195 (stress-e1d486cdb6f04ca49af7a6773f34c9d7): version 4 holds Update by s2c4-admin
- lost update 474d9f24-8be5-4f82-a76c-b534e8d66bca v1 Create by s3c1-registrant via 41195 (stress-afefd8864ef24f8c8a2309622a770b07): record missing
- lost update 1cf8a242-05df-49de-b81a-da2d89acd7a6 v1 Create by s1c3-registrant via 39295 (stress-36ea5c834c7b4f8996d29c3f8644621a): record missing
- lost update 06e19c9b-e73c-4e37-a037-e0a167fecdf6 v1 Create by s2c3-registrant via 45253 (stress-b9f0dbce017348a3918f592391481c97): record missing
- lost update b8671b10-276d-4b1f-96be-cb9a55fc059d v1 Create by s3c3-registrant via 41195 (stress-dd0ec0b79c7b4c41bb9ee56f89dae14e): record missing

server 41195:
- missing record e098fe7a-635a-4753-b31b-d1238605543a: missing (1 acknowledged changes)
- missing record 474d9f24-8be5-4f82-a76c-b534e8d66bca: missing (1 acknowledged changes)
- missing record 1cf8a242-05df-49de-b81a-da2d89acd7a6: missing (1 acknowledged changes)
- missing record 06e19c9b-e73c-4e37-a037-e0a167fecdf6: missing (1 acknowledged changes)
- missing record b8671b10-276d-4b1f-96be-cb9a55fc059d: missing (1 acknowledged changes)
- lost update e098fe7a-635a-4753-b31b-d1238605543a v1 Create by s1c1-registrant via 39295 (stress-2d6e1c59abe54f95abd844d2ade0087b): record missing
- lost update 8bfcbfe7-ddfd-4295-b176-a23221df44b6 v2 Comment by s1c2-reviewer 's1c2-reviewer:10' via 39295 (stress-57bbcc8b951546a98f337b6c8001b351): version 2 holds Update by s2c1-registrant
- lost update 8bfcbfe7-ddfd-4295-b176-a23221df44b6 v2 Update by s1c4-admin 's1c4-admin:14' via 39295 (stress-46645aea3f4442faa1fe2c87d8f3852f): version 2 holds Update by s2c1-registrant
- lost update 8bfcbfe7-ddfd-4295-b176-a23221df44b6 v2 Comment by s1c5-reviewer 's1c5-reviewer:22' via 39295 (stress-0b6ad08a6a2546499d6f03777fae59ab): version 2 holds Update by s2c1-registrant
- lost update 8bfcbfe7-ddfd-4295-b176-a23221df44b6 v2 Comment by s2c2-reviewer 's2c2-reviewer:4' via 45253 (stress-1731429f09424256ac4891d53d4fe2ff): version 2 holds Update by s2c1-registrant
- lost update 8bfcbfe7-ddfd-4295-b176-a23221df44b6 v9 Comment by s2c2-reviewer 's2c2-reviewer:19' via 45253 (stress-44fad814d0ff43ea98cd330f5bc8d169): version 9 holds Comment by s2c5-reviewer
- lost update 8bfcbfe7-ddfd-4295-b176-a23221df44b6 v13 Comment by s2c2-reviewer 's2c2-reviewer:25' via 45253 (stress-d85e7961b19f43c6952319d2a8923432): version 13 holds Comment by s2c4-admin
- lost update 8bfcbfe7-ddfd-4295-b176-a23221df44b6 v2 Submit by s2c4-admin via 45253 (stress-8fe3366d5e96430aa8962ffaba2a915d): version 2 holds Update by s2c1-registrant
- lost update 8bfcbfe7-ddfd-4295-b176-a23221df44b6 v12 Comment by s2c4-admin 's2c4-admin:22' via 45253 (stress-6b44129a946846c79a6de0e1ce87d871): version 12 holds Comment by s3c4-admin
- lost update 8bfcbfe7-ddfd-4295-b176-a23221df44b6 v2 Comment by s2c5-reviewer 's2c5-reviewer:6' via 45253 (stress-6284fa6cc2194bed9e2af3e31ba1d80e): version 2 holds Update by s2c1-registrant
- lost update 8bfcbfe7-ddfd-4295-b176-a23221df44b6 v13 Comment by s2c5-reviewer 's2c5-reviewer:27' via 45253 (stress-0d9651f841884373afcca8df660ebe8c): version 13 holds Comment by s2c4-admin
- lost update 8bfcbfe7-ddfd-4295-b176-a23221df44b6 v4 Comment by s3c2-reviewer 's3c2-reviewer:10' via 41195 (stress-e1d486cdb6f04ca49af7a6773f34c9d7): version 4 holds Update by s2c4-admin
- lost update 474d9f24-8be5-4f82-a76c-b534e8d66bca v1 Create by s3c1-registrant via 41195 (stress-afefd8864ef24f8c8a2309622a770b07): record missing
- lost update 1cf8a242-05df-49de-b81a-da2d89acd7a6 v1 Create by s1c3-registrant via 39295 (stress-36ea5c834c7b4f8996d29c3f8644621a): record missing
- lost update 06e19c9b-e73c-4e37-a037-e0a167fecdf6 v1 Create by s2c3-registrant via 45253 (stress-b9f0dbce017348a3918f592391481c97): record missing
- lost update b8671b10-276d-4b1f-96be-cb9a55fc059d v1 Create by s3c3-registrant via 41195 (stress-dd0ec0b79c7b4c41bb9ee56f89dae14e): record missing

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 3 | 39 | 93.4 | 25 | 26 | 16 | 0 | 0 | 102.1 | no |
