# Stress run: NaiveFile, 3 server(s): FAILED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:44:05 UTC, 1.6 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 24 records, 200 changes per client, seed 2
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r24-NaiveFile-2/data`; run folder `stress-runs/sweep/sweep-3x8-r24-NaiveFile-2`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 407 | 150 | 96.8 | 67 | 173 | 0 | 4.7 | 116.2 | 117.9 |

Writes by outcome: conflict 67, http500 17, saved 150, wrongStatus 173. Saves by action: Create 24, Update 2, Submit 3, Approve 2, Comment 119. Reads: 4800 (http500 167, notFound 4250).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | FAIL | 2/24 | 22 | 43 | 0 | 0 | 0 |
| server 35667 | FAIL | 2/24 | 22 | 43 | 0 | 0 | 0 |
| server 44905 | FAIL | 2/24 | 22 | 43 | 0 | 0 | 0 |
| server 33415 | FAIL | 2/24 | 22 | 43 | 0 | 0 | 0 |

storage:
- missing record 09506c1f-3963-40ff-bccf-03b21237fcf7: missing (1 acknowledged changes)
- missing record 0c50d59f-e3a4-4300-a2fb-de6a1e03e10c: missing (1 acknowledged changes)
- missing record b06c83e2-71a8-4c6f-9c05-d0646e80b43e: missing (1 acknowledged changes)
- missing record 73f319b3-c092-4af8-ae48-b5ce73bf9758: missing (1 acknowledged changes)
- missing record ca37e0be-5915-49c4-8c4d-0bbab0b67d40: missing (1 acknowledged changes)
- missing record 29be01d8-8d95-48cf-95fe-ed8fdbc81be2: missing (1 acknowledged changes)
- missing record 4261fd67-84cb-4c60-9ce2-f252797f2e76: missing (1 acknowledged changes)
- missing record 0da0131a-ccfd-4baa-a422-1207779e5125: missing (1 acknowledged changes)
- missing record 4857a9ab-c16e-4065-bf24-1e403b5e184a: missing (1 acknowledged changes)
- missing record f31e3b6c-94bc-4746-9262-8773546daf8b: missing (1 acknowledged changes)
- missing record 287e5192-4b7a-440f-8709-2c970552ae20: missing (1 acknowledged changes)
- missing record f3bd6f4d-b628-4b8e-b775-89719032f025: missing (1 acknowledged changes)
- missing record ddfe45f2-d138-4a3f-b219-533017c808bc: missing (1 acknowledged changes)
- missing record 88c0b39a-78fa-49f3-81a7-d004fc3f3655: missing (1 acknowledged changes)
- missing record 7008e484-c7e5-4c4b-8bf0-d12f1800de8b: missing (1 acknowledged changes)
- missing record df17a221-1893-46d1-b961-fe74369cc2f0: missing (1 acknowledged changes)
- missing record d84a0207-ba47-495c-82da-82edee25a28a: missing (1 acknowledged changes)
- missing record 1934222e-4bc6-4e9f-9047-be094fd23b6e: missing (1 acknowledged changes)
- missing record 749b81eb-a8df-41b9-a4fc-836ec8d6f5ea: missing (1 acknowledged changes)
- missing record 39905936-a044-440a-a7cc-f2d921c4ebb1: missing (1 acknowledged changes)
- lost update 09506c1f-3963-40ff-bccf-03b21237fcf7 v1 Create by s1c1-registrant via 35667 (stress-a9ccd7cc1ad24619b2de14e5962de4d4): record missing
- lost update 0c50d59f-e3a4-4300-a2fb-de6a1e03e10c v1 Create by s1c1-registrant via 35667 (stress-cba0f26286e64a0aa9e3deef06cd5920): record missing
- lost update b06c83e2-71a8-4c6f-9c05-d0646e80b43e v1 Create by s2c1-registrant via 44905 (stress-77067f7cb0a94e268f2a19e598536345): record missing
- lost update 73f319b3-c092-4af8-ae48-b5ce73bf9758 v1 Create by s2c1-registrant via 44905 (stress-07c9e5edbfaa45f480791108fcd5beb8): record missing
- lost update ca37e0be-5915-49c4-8c4d-0bbab0b67d40 v1 Create by s3c1-registrant via 33415 (stress-f79e028e56894f3b9c10a1673dd7e4d2): record missing
- lost update 29be01d8-8d95-48cf-95fe-ed8fdbc81be2 v1 Create by s3c1-registrant via 33415 (stress-3de8032af0a14230ad6463aa8b933754): record missing
- lost update 4261fd67-84cb-4c60-9ce2-f252797f2e76 v1 Create by s1c3-registrant via 35667 (stress-dce638dc98b84b349cc481f95a435fc4): record missing
- lost update 0da0131a-ccfd-4baa-a422-1207779e5125 v1 Create by s1c3-registrant via 35667 (stress-69e1cc4d8d6949b48eacd97103e4ee77): record missing
- lost update 4857a9ab-c16e-4065-bf24-1e403b5e184a v1 Create by s2c3-registrant via 44905 (stress-49ba27e110d54c1b9903b9e8bb2d86f3): record missing
- lost update f31e3b6c-94bc-4746-9262-8773546daf8b v1 Create by s2c3-registrant via 44905 (stress-c685c8d386da48abb1f2227c834971dd): record missing
- lost update 287e5192-4b7a-440f-8709-2c970552ae20 v1 Create by s3c3-registrant via 33415 (stress-c8458418bd634f66a5aad26e5db10046): record missing
- lost update f3bd6f4d-b628-4b8e-b775-89719032f025 v1 Create by s3c3-registrant via 33415 (stress-863e700b2e5f445aa5f91a38e7a15122): record missing
- lost update ddfe45f2-d138-4a3f-b219-533017c808bc v1 Create by s1c6-registrant via 35667 (stress-079b2e0e9eeb40d9a61b5a129550e26a): record missing
- lost update 88c0b39a-78fa-49f3-81a7-d004fc3f3655 v1 Create by s1c6-registrant via 35667 (stress-8152e6a57f54414d83c4bac31a1884ab): record missing
- lost update faaf6ccc-5ce8-4677-b65e-fc6ed7de895c v2 Comment by s1c2-reviewer 's1c2-reviewer:21' via 35667 (stress-696e6297e3bd413db1ce0df55b7be904): version 2 holds Comment by s2c2-reviewer
- lost update faaf6ccc-5ce8-4677-b65e-fc6ed7de895c v9 Comment by s1c2-reviewer 's1c2-reviewer:43' via 35667 (stress-27a1ea2289c14bf0ae2ea51f32f62e72): version 9 holds Comment by s2c2-reviewer
- lost update faaf6ccc-5ce8-4677-b65e-fc6ed7de895c v14 Comment by s1c2-reviewer 's1c2-reviewer:66' via 35667 (stress-45655e0945c749129ccc6a1e8ab83a2d): version 14 holds Comment by s3c2-reviewer
- lost update faaf6ccc-5ce8-4677-b65e-fc6ed7de895c v2 Comment by s1c5-reviewer 's1c5-reviewer:6' via 35667 (stress-137db98f3cbb42be835d7b3a7ba01592): version 2 holds Comment by s2c2-reviewer
- lost update faaf6ccc-5ce8-4677-b65e-fc6ed7de895c v50 Comment by s1c5-reviewer 's1c5-reviewer:176' via 35667 (stress-4a4425f355e540628af5875b399e306d): version 50 holds Comment by s1c4-admin
- lost update faaf6ccc-5ce8-4677-b65e-fc6ed7de895c v3 Comment by s2c7-reviewer 's2c7-reviewer:19' via 44905 (stress-b14d47d8e5bf4d60a03d7226a02678b9): version 3 holds Submit by s1c4-admin

server 35667:
- missing record 09506c1f-3963-40ff-bccf-03b21237fcf7: missing (1 acknowledged changes)
- missing record 0c50d59f-e3a4-4300-a2fb-de6a1e03e10c: missing (1 acknowledged changes)
- missing record b06c83e2-71a8-4c6f-9c05-d0646e80b43e: missing (1 acknowledged changes)
- missing record 73f319b3-c092-4af8-ae48-b5ce73bf9758: missing (1 acknowledged changes)
- missing record ca37e0be-5915-49c4-8c4d-0bbab0b67d40: missing (1 acknowledged changes)
- missing record 29be01d8-8d95-48cf-95fe-ed8fdbc81be2: missing (1 acknowledged changes)
- missing record 4261fd67-84cb-4c60-9ce2-f252797f2e76: missing (1 acknowledged changes)
- missing record 0da0131a-ccfd-4baa-a422-1207779e5125: missing (1 acknowledged changes)
- missing record 4857a9ab-c16e-4065-bf24-1e403b5e184a: missing (1 acknowledged changes)
- missing record f31e3b6c-94bc-4746-9262-8773546daf8b: missing (1 acknowledged changes)
- missing record 287e5192-4b7a-440f-8709-2c970552ae20: missing (1 acknowledged changes)
- missing record f3bd6f4d-b628-4b8e-b775-89719032f025: missing (1 acknowledged changes)
- missing record ddfe45f2-d138-4a3f-b219-533017c808bc: missing (1 acknowledged changes)
- missing record 88c0b39a-78fa-49f3-81a7-d004fc3f3655: missing (1 acknowledged changes)
- missing record 7008e484-c7e5-4c4b-8bf0-d12f1800de8b: missing (1 acknowledged changes)
- missing record df17a221-1893-46d1-b961-fe74369cc2f0: missing (1 acknowledged changes)
- missing record d84a0207-ba47-495c-82da-82edee25a28a: missing (1 acknowledged changes)
- missing record 1934222e-4bc6-4e9f-9047-be094fd23b6e: missing (1 acknowledged changes)
- missing record 749b81eb-a8df-41b9-a4fc-836ec8d6f5ea: missing (1 acknowledged changes)
- missing record 39905936-a044-440a-a7cc-f2d921c4ebb1: missing (1 acknowledged changes)
- lost update 09506c1f-3963-40ff-bccf-03b21237fcf7 v1 Create by s1c1-registrant via 35667 (stress-a9ccd7cc1ad24619b2de14e5962de4d4): record missing
- lost update 0c50d59f-e3a4-4300-a2fb-de6a1e03e10c v1 Create by s1c1-registrant via 35667 (stress-cba0f26286e64a0aa9e3deef06cd5920): record missing
- lost update b06c83e2-71a8-4c6f-9c05-d0646e80b43e v1 Create by s2c1-registrant via 44905 (stress-77067f7cb0a94e268f2a19e598536345): record missing
- lost update 73f319b3-c092-4af8-ae48-b5ce73bf9758 v1 Create by s2c1-registrant via 44905 (stress-07c9e5edbfaa45f480791108fcd5beb8): record missing
- lost update ca37e0be-5915-49c4-8c4d-0bbab0b67d40 v1 Create by s3c1-registrant via 33415 (stress-f79e028e56894f3b9c10a1673dd7e4d2): record missing
- lost update 29be01d8-8d95-48cf-95fe-ed8fdbc81be2 v1 Create by s3c1-registrant via 33415 (stress-3de8032af0a14230ad6463aa8b933754): record missing
- lost update 4261fd67-84cb-4c60-9ce2-f252797f2e76 v1 Create by s1c3-registrant via 35667 (stress-dce638dc98b84b349cc481f95a435fc4): record missing
- lost update 0da0131a-ccfd-4baa-a422-1207779e5125 v1 Create by s1c3-registrant via 35667 (stress-69e1cc4d8d6949b48eacd97103e4ee77): record missing
- lost update 4857a9ab-c16e-4065-bf24-1e403b5e184a v1 Create by s2c3-registrant via 44905 (stress-49ba27e110d54c1b9903b9e8bb2d86f3): record missing
- lost update f31e3b6c-94bc-4746-9262-8773546daf8b v1 Create by s2c3-registrant via 44905 (stress-c685c8d386da48abb1f2227c834971dd): record missing
- lost update 287e5192-4b7a-440f-8709-2c970552ae20 v1 Create by s3c3-registrant via 33415 (stress-c8458418bd634f66a5aad26e5db10046): record missing
- lost update f3bd6f4d-b628-4b8e-b775-89719032f025 v1 Create by s3c3-registrant via 33415 (stress-863e700b2e5f445aa5f91a38e7a15122): record missing
- lost update ddfe45f2-d138-4a3f-b219-533017c808bc v1 Create by s1c6-registrant via 35667 (stress-079b2e0e9eeb40d9a61b5a129550e26a): record missing
- lost update 88c0b39a-78fa-49f3-81a7-d004fc3f3655 v1 Create by s1c6-registrant via 35667 (stress-8152e6a57f54414d83c4bac31a1884ab): record missing
- lost update faaf6ccc-5ce8-4677-b65e-fc6ed7de895c v2 Comment by s1c2-reviewer 's1c2-reviewer:21' via 35667 (stress-696e6297e3bd413db1ce0df55b7be904): version 2 holds Comment by s2c2-reviewer
- lost update faaf6ccc-5ce8-4677-b65e-fc6ed7de895c v9 Comment by s1c2-reviewer 's1c2-reviewer:43' via 35667 (stress-27a1ea2289c14bf0ae2ea51f32f62e72): version 9 holds Comment by s2c2-reviewer
- lost update faaf6ccc-5ce8-4677-b65e-fc6ed7de895c v14 Comment by s1c2-reviewer 's1c2-reviewer:66' via 35667 (stress-45655e0945c749129ccc6a1e8ab83a2d): version 14 holds Comment by s3c2-reviewer
- lost update faaf6ccc-5ce8-4677-b65e-fc6ed7de895c v2 Comment by s1c5-reviewer 's1c5-reviewer:6' via 35667 (stress-137db98f3cbb42be835d7b3a7ba01592): version 2 holds Comment by s2c2-reviewer
- lost update faaf6ccc-5ce8-4677-b65e-fc6ed7de895c v50 Comment by s1c5-reviewer 's1c5-reviewer:176' via 35667 (stress-4a4425f355e540628af5875b399e306d): version 50 holds Comment by s1c4-admin
- lost update faaf6ccc-5ce8-4677-b65e-fc6ed7de895c v3 Comment by s2c7-reviewer 's2c7-reviewer:19' via 44905 (stress-b14d47d8e5bf4d60a03d7226a02678b9): version 3 holds Submit by s1c4-admin

server 44905:
- missing record 09506c1f-3963-40ff-bccf-03b21237fcf7: missing (1 acknowledged changes)
- missing record 0c50d59f-e3a4-4300-a2fb-de6a1e03e10c: missing (1 acknowledged changes)
- missing record b06c83e2-71a8-4c6f-9c05-d0646e80b43e: missing (1 acknowledged changes)
- missing record 73f319b3-c092-4af8-ae48-b5ce73bf9758: missing (1 acknowledged changes)
- missing record ca37e0be-5915-49c4-8c4d-0bbab0b67d40: missing (1 acknowledged changes)
- missing record 29be01d8-8d95-48cf-95fe-ed8fdbc81be2: missing (1 acknowledged changes)
- missing record 4261fd67-84cb-4c60-9ce2-f252797f2e76: missing (1 acknowledged changes)
- missing record 0da0131a-ccfd-4baa-a422-1207779e5125: missing (1 acknowledged changes)
- missing record 4857a9ab-c16e-4065-bf24-1e403b5e184a: missing (1 acknowledged changes)
- missing record f31e3b6c-94bc-4746-9262-8773546daf8b: missing (1 acknowledged changes)
- missing record 287e5192-4b7a-440f-8709-2c970552ae20: missing (1 acknowledged changes)
- missing record f3bd6f4d-b628-4b8e-b775-89719032f025: missing (1 acknowledged changes)
- missing record ddfe45f2-d138-4a3f-b219-533017c808bc: missing (1 acknowledged changes)
- missing record 88c0b39a-78fa-49f3-81a7-d004fc3f3655: missing (1 acknowledged changes)
- missing record 7008e484-c7e5-4c4b-8bf0-d12f1800de8b: missing (1 acknowledged changes)
- missing record df17a221-1893-46d1-b961-fe74369cc2f0: missing (1 acknowledged changes)
- missing record d84a0207-ba47-495c-82da-82edee25a28a: missing (1 acknowledged changes)
- missing record 1934222e-4bc6-4e9f-9047-be094fd23b6e: missing (1 acknowledged changes)
- missing record 749b81eb-a8df-41b9-a4fc-836ec8d6f5ea: missing (1 acknowledged changes)
- missing record 39905936-a044-440a-a7cc-f2d921c4ebb1: missing (1 acknowledged changes)
- lost update 09506c1f-3963-40ff-bccf-03b21237fcf7 v1 Create by s1c1-registrant via 35667 (stress-a9ccd7cc1ad24619b2de14e5962de4d4): record missing
- lost update 0c50d59f-e3a4-4300-a2fb-de6a1e03e10c v1 Create by s1c1-registrant via 35667 (stress-cba0f26286e64a0aa9e3deef06cd5920): record missing
- lost update b06c83e2-71a8-4c6f-9c05-d0646e80b43e v1 Create by s2c1-registrant via 44905 (stress-77067f7cb0a94e268f2a19e598536345): record missing
- lost update 73f319b3-c092-4af8-ae48-b5ce73bf9758 v1 Create by s2c1-registrant via 44905 (stress-07c9e5edbfaa45f480791108fcd5beb8): record missing
- lost update ca37e0be-5915-49c4-8c4d-0bbab0b67d40 v1 Create by s3c1-registrant via 33415 (stress-f79e028e56894f3b9c10a1673dd7e4d2): record missing
- lost update 29be01d8-8d95-48cf-95fe-ed8fdbc81be2 v1 Create by s3c1-registrant via 33415 (stress-3de8032af0a14230ad6463aa8b933754): record missing
- lost update 4261fd67-84cb-4c60-9ce2-f252797f2e76 v1 Create by s1c3-registrant via 35667 (stress-dce638dc98b84b349cc481f95a435fc4): record missing
- lost update 0da0131a-ccfd-4baa-a422-1207779e5125 v1 Create by s1c3-registrant via 35667 (stress-69e1cc4d8d6949b48eacd97103e4ee77): record missing
- lost update 4857a9ab-c16e-4065-bf24-1e403b5e184a v1 Create by s2c3-registrant via 44905 (stress-49ba27e110d54c1b9903b9e8bb2d86f3): record missing
- lost update f31e3b6c-94bc-4746-9262-8773546daf8b v1 Create by s2c3-registrant via 44905 (stress-c685c8d386da48abb1f2227c834971dd): record missing
- lost update 287e5192-4b7a-440f-8709-2c970552ae20 v1 Create by s3c3-registrant via 33415 (stress-c8458418bd634f66a5aad26e5db10046): record missing
- lost update f3bd6f4d-b628-4b8e-b775-89719032f025 v1 Create by s3c3-registrant via 33415 (stress-863e700b2e5f445aa5f91a38e7a15122): record missing
- lost update ddfe45f2-d138-4a3f-b219-533017c808bc v1 Create by s1c6-registrant via 35667 (stress-079b2e0e9eeb40d9a61b5a129550e26a): record missing
- lost update 88c0b39a-78fa-49f3-81a7-d004fc3f3655 v1 Create by s1c6-registrant via 35667 (stress-8152e6a57f54414d83c4bac31a1884ab): record missing
- lost update faaf6ccc-5ce8-4677-b65e-fc6ed7de895c v2 Comment by s1c2-reviewer 's1c2-reviewer:21' via 35667 (stress-696e6297e3bd413db1ce0df55b7be904): version 2 holds Comment by s2c2-reviewer
- lost update faaf6ccc-5ce8-4677-b65e-fc6ed7de895c v9 Comment by s1c2-reviewer 's1c2-reviewer:43' via 35667 (stress-27a1ea2289c14bf0ae2ea51f32f62e72): version 9 holds Comment by s2c2-reviewer
- lost update faaf6ccc-5ce8-4677-b65e-fc6ed7de895c v14 Comment by s1c2-reviewer 's1c2-reviewer:66' via 35667 (stress-45655e0945c749129ccc6a1e8ab83a2d): version 14 holds Comment by s3c2-reviewer
- lost update faaf6ccc-5ce8-4677-b65e-fc6ed7de895c v2 Comment by s1c5-reviewer 's1c5-reviewer:6' via 35667 (stress-137db98f3cbb42be835d7b3a7ba01592): version 2 holds Comment by s2c2-reviewer
- lost update faaf6ccc-5ce8-4677-b65e-fc6ed7de895c v50 Comment by s1c5-reviewer 's1c5-reviewer:176' via 35667 (stress-4a4425f355e540628af5875b399e306d): version 50 holds Comment by s1c4-admin
- lost update faaf6ccc-5ce8-4677-b65e-fc6ed7de895c v3 Comment by s2c7-reviewer 's2c7-reviewer:19' via 44905 (stress-b14d47d8e5bf4d60a03d7226a02678b9): version 3 holds Submit by s1c4-admin

server 33415:
- missing record 09506c1f-3963-40ff-bccf-03b21237fcf7: missing (1 acknowledged changes)
- missing record 0c50d59f-e3a4-4300-a2fb-de6a1e03e10c: missing (1 acknowledged changes)
- missing record b06c83e2-71a8-4c6f-9c05-d0646e80b43e: missing (1 acknowledged changes)
- missing record 73f319b3-c092-4af8-ae48-b5ce73bf9758: missing (1 acknowledged changes)
- missing record ca37e0be-5915-49c4-8c4d-0bbab0b67d40: missing (1 acknowledged changes)
- missing record 29be01d8-8d95-48cf-95fe-ed8fdbc81be2: missing (1 acknowledged changes)
- missing record 4261fd67-84cb-4c60-9ce2-f252797f2e76: missing (1 acknowledged changes)
- missing record 0da0131a-ccfd-4baa-a422-1207779e5125: missing (1 acknowledged changes)
- missing record 4857a9ab-c16e-4065-bf24-1e403b5e184a: missing (1 acknowledged changes)
- missing record f31e3b6c-94bc-4746-9262-8773546daf8b: missing (1 acknowledged changes)
- missing record 287e5192-4b7a-440f-8709-2c970552ae20: missing (1 acknowledged changes)
- missing record f3bd6f4d-b628-4b8e-b775-89719032f025: missing (1 acknowledged changes)
- missing record ddfe45f2-d138-4a3f-b219-533017c808bc: missing (1 acknowledged changes)
- missing record 88c0b39a-78fa-49f3-81a7-d004fc3f3655: missing (1 acknowledged changes)
- missing record 7008e484-c7e5-4c4b-8bf0-d12f1800de8b: missing (1 acknowledged changes)
- missing record df17a221-1893-46d1-b961-fe74369cc2f0: missing (1 acknowledged changes)
- missing record d84a0207-ba47-495c-82da-82edee25a28a: missing (1 acknowledged changes)
- missing record 1934222e-4bc6-4e9f-9047-be094fd23b6e: missing (1 acknowledged changes)
- missing record 749b81eb-a8df-41b9-a4fc-836ec8d6f5ea: missing (1 acknowledged changes)
- missing record 39905936-a044-440a-a7cc-f2d921c4ebb1: missing (1 acknowledged changes)
- lost update 09506c1f-3963-40ff-bccf-03b21237fcf7 v1 Create by s1c1-registrant via 35667 (stress-a9ccd7cc1ad24619b2de14e5962de4d4): record missing
- lost update 0c50d59f-e3a4-4300-a2fb-de6a1e03e10c v1 Create by s1c1-registrant via 35667 (stress-cba0f26286e64a0aa9e3deef06cd5920): record missing
- lost update b06c83e2-71a8-4c6f-9c05-d0646e80b43e v1 Create by s2c1-registrant via 44905 (stress-77067f7cb0a94e268f2a19e598536345): record missing
- lost update 73f319b3-c092-4af8-ae48-b5ce73bf9758 v1 Create by s2c1-registrant via 44905 (stress-07c9e5edbfaa45f480791108fcd5beb8): record missing
- lost update ca37e0be-5915-49c4-8c4d-0bbab0b67d40 v1 Create by s3c1-registrant via 33415 (stress-f79e028e56894f3b9c10a1673dd7e4d2): record missing
- lost update 29be01d8-8d95-48cf-95fe-ed8fdbc81be2 v1 Create by s3c1-registrant via 33415 (stress-3de8032af0a14230ad6463aa8b933754): record missing
- lost update 4261fd67-84cb-4c60-9ce2-f252797f2e76 v1 Create by s1c3-registrant via 35667 (stress-dce638dc98b84b349cc481f95a435fc4): record missing
- lost update 0da0131a-ccfd-4baa-a422-1207779e5125 v1 Create by s1c3-registrant via 35667 (stress-69e1cc4d8d6949b48eacd97103e4ee77): record missing
- lost update 4857a9ab-c16e-4065-bf24-1e403b5e184a v1 Create by s2c3-registrant via 44905 (stress-49ba27e110d54c1b9903b9e8bb2d86f3): record missing
- lost update f31e3b6c-94bc-4746-9262-8773546daf8b v1 Create by s2c3-registrant via 44905 (stress-c685c8d386da48abb1f2227c834971dd): record missing
- lost update 287e5192-4b7a-440f-8709-2c970552ae20 v1 Create by s3c3-registrant via 33415 (stress-c8458418bd634f66a5aad26e5db10046): record missing
- lost update f3bd6f4d-b628-4b8e-b775-89719032f025 v1 Create by s3c3-registrant via 33415 (stress-863e700b2e5f445aa5f91a38e7a15122): record missing
- lost update ddfe45f2-d138-4a3f-b219-533017c808bc v1 Create by s1c6-registrant via 35667 (stress-079b2e0e9eeb40d9a61b5a129550e26a): record missing
- lost update 88c0b39a-78fa-49f3-81a7-d004fc3f3655 v1 Create by s1c6-registrant via 35667 (stress-8152e6a57f54414d83c4bac31a1884ab): record missing
- lost update faaf6ccc-5ce8-4677-b65e-fc6ed7de895c v2 Comment by s1c2-reviewer 's1c2-reviewer:21' via 35667 (stress-696e6297e3bd413db1ce0df55b7be904): version 2 holds Comment by s2c2-reviewer
- lost update faaf6ccc-5ce8-4677-b65e-fc6ed7de895c v9 Comment by s1c2-reviewer 's1c2-reviewer:43' via 35667 (stress-27a1ea2289c14bf0ae2ea51f32f62e72): version 9 holds Comment by s2c2-reviewer
- lost update faaf6ccc-5ce8-4677-b65e-fc6ed7de895c v14 Comment by s1c2-reviewer 's1c2-reviewer:66' via 35667 (stress-45655e0945c749129ccc6a1e8ab83a2d): version 14 holds Comment by s3c2-reviewer
- lost update faaf6ccc-5ce8-4677-b65e-fc6ed7de895c v2 Comment by s1c5-reviewer 's1c5-reviewer:6' via 35667 (stress-137db98f3cbb42be835d7b3a7ba01592): version 2 holds Comment by s2c2-reviewer
- lost update faaf6ccc-5ce8-4677-b65e-fc6ed7de895c v50 Comment by s1c5-reviewer 's1c5-reviewer:176' via 35667 (stress-4a4425f355e540628af5875b399e306d): version 50 holds Comment by s1c4-admin
- lost update faaf6ccc-5ce8-4677-b65e-fc6ed7de895c v3 Comment by s2c7-reviewer 's2c7-reviewer:19' via 44905 (stress-b14d47d8e5bf4d60a03d7226a02678b9): version 3 holds Submit by s1c4-admin

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 3 | 150 | 96.8 | 67 | 173 | 43 | 0 | 0 | 116.2 | no |
