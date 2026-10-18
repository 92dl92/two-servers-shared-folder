# Stress run: NaiveFile, 3 server(s): FAILED

- Commit `f161ef446061acf35ef35e3eaac8fd7168560700`, 2026-10-17 20:47:27 UTC, 2.6 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 48 records, 200 changes per client, seed 2
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `stress-runs/sweep/sweep-3x8-r48-NaiveFile-2/data`; run folder `stress-runs/sweep/sweep-3x8-r48-NaiveFile-2`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 1036 | 475 | 182.0 | 93 | 385 | 0 | 8.6 | 29.9 | 112.3 |

Writes by outcome: conflict 93, http500 83, saved 475, wrongStatus 385. Saves by action: Create 44, Update 73, Submit 16, Approve 9, Reject 8, Comment 325. Reads: 4800 (http500 366, notFound 3446).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | FAIL | 10/44 | 34 | 225 | 0 | 0 | 0 |
| server 39715 | FAIL | 10/44 | 34 | 225 | 0 | 0 | 0 |
| server 39531 | FAIL | 10/44 | 34 | 225 | 0 | 0 | 0 |
| server 35789 | FAIL | 10/44 | 34 | 225 | 0 | 0 | 0 |

storage:
- missing record 6f4c012d-8fdf-4c3a-8f76-f0d5d18254a2: missing (1 acknowledged changes)
- missing record 9bf446e4-6b99-4754-9ab5-1067201f9ca2: missing (1 acknowledged changes)
- missing record 7c8bcd96-0c88-4abf-b788-7ea100ec573c: missing (1 acknowledged changes)
- missing record dd5ea6bd-8587-4e2b-819f-604924ba76be: missing (1 acknowledged changes)
- missing record e796005a-2ca3-4014-9bf5-da04899e9956: missing (1 acknowledged changes)
- missing record 4aa86ea4-f8fc-498f-8979-9c5b3d7e9e8c: missing (1 acknowledged changes)
- missing record 9ed9bb36-56de-4aa4-b27c-d292268fb878: missing (1 acknowledged changes)
- missing record f896579a-6bc2-4b52-a550-287a72fa28b7: missing (1 acknowledged changes)
- missing record e4b47a74-f791-4a86-ab2a-81cc13bbe192: missing (1 acknowledged changes)
- missing record 424c6cf7-07f7-4ef4-a852-3b915fa69ecf: missing (1 acknowledged changes)
- missing record af852f26-0bde-494f-b91c-c09cca26f7d9: missing (1 acknowledged changes)
- missing record bc391423-47df-407b-a495-9fdbc5a60fed: missing (1 acknowledged changes)
- missing record 7d9ef809-9982-4e94-a6e3-1841c4bbcf35: missing (1 acknowledged changes)
- missing record eae957e1-befd-42d6-baa7-c474baabac7b: missing (1 acknowledged changes)
- missing record 28db6884-f144-4579-b66d-eac338b51d21: missing (1 acknowledged changes)
- missing record ccc81a55-92a7-41c8-be9d-c7ec4b3688c7: missing (1 acknowledged changes)
- missing record 8e7363d3-1c7c-4f92-98c5-76e8592e62e4: missing (1 acknowledged changes)
- missing record 70b12058-fbec-43af-98da-421d3531e20f: missing (1 acknowledged changes)
- missing record 7f5f422f-224f-4094-b879-e74d95789f8b: missing (1 acknowledged changes)
- missing record c4b63778-0f51-496a-acf2-4c40a9c35c1a: missing (1 acknowledged changes)
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v3 Update by s1c1-registrant 's1c1-registrant:8' via 39715 (stress-5e7d23a54fc3495693414315583ee8dd): version 3 holds Comment by s1c5-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v5 Update by s1c1-registrant 's1c1-registrant:15' via 39715 (stress-89aea6d436694bf4a0d6300e0474b019): version 5 holds Reject by s3c4-admin
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v11 Comment by s1c2-reviewer 's1c2-reviewer:96' via 39715 (stress-a772b50b7e3c4378bf0cd733fb279aeb): version 11 holds Comment by s1c5-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v5 Update by s1c4-admin 's1c4-admin:46' via 39715 (stress-595f0d33a2294aacbef55075cfd340cd): version 5 holds Reject by s3c4-admin
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v6 Comment by s1c4-admin 's1c4-admin:51' via 39715 (stress-3e118aeb31a44e5f88841cdd5f9585f7): version 6 holds Comment by s3c5-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v8 Comment by s1c4-admin 's1c4-admin:82' via 39715 (stress-c416f96021c344afb38310ddc1233234): version 8 holds Comment by s1c2-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v15 Comment by s1c4-admin 's1c4-admin:164' via 39715 (stress-cf8032ab456d40b59bdab0b1fe7dedbb): version 15 holds Comment by s1c4-admin
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v11 Comment by s1c7-reviewer 's1c7-reviewer:109' via 39715 (stress-bc59bd0b765e41be82ca4103a056fd89): version 11 holds Comment by s1c5-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v12 Comment by s1c7-reviewer 's1c7-reviewer:123' via 39715 (stress-bb8431534c504ccdb2e0a2ed670af5ed): version 12 holds Comment by s3c5-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v11 Comment by s2c2-reviewer 's2c2-reviewer:97' via 39531 (stress-0f61fe8ad096455f9d5623db7cd33dc4): version 11 holds Comment by s1c5-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v16 Comment by s2c2-reviewer 's2c2-reviewer:174' via 39531 (stress-fa54180fe9924e78a00a6ca8b7679e09): version 16 holds Comment by s2c5-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v14 Comment by s2c7-reviewer 's2c7-reviewer:144' via 39531 (stress-c04c7fd39dc041bfb120655b54dd0e49): version 14 holds Comment by s2c2-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v11 Comment by s3c2-reviewer 's3c2-reviewer:144' via 35789 (stress-dee398f404a2424a94e753b419621709): version 11 holds Comment by s1c5-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v3 Comment by s3c4-admin 's3c4-admin:9' via 35789 (stress-d76045d2f50f4a2b956944fda3a2123d): version 3 holds Comment by s1c5-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v6 Comment by s3c4-admin 's3c4-admin:61' via 35789 (stress-1abb95a2a7f549e088338db95726ee08): version 6 holds Comment by s3c5-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v8 Comment by s3c4-admin 's3c4-admin:107' via 35789 (stress-268e0de3ec944bf489e306e001982a9f): version 8 holds Comment by s1c2-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v14 Comment by s3c5-reviewer 's3c5-reviewer:188' via 35789 (stress-e4c00c20f4cc41b487bb9c793fbd397a): version 14 holds Comment by s2c2-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v2 Comment by s3c7-reviewer 's3c7-reviewer:11' via 35789 (stress-d58467f8c8d34875aabc5171af8ae06e): version 2 holds Comment by s2c7-reviewer
- lost update 4d1739a3-99c0-42d8-a58b-9d5078d1f54e v5 Update by s1c1-registrant 's1c1-registrant:17' via 39715 (stress-b1e769c1a1a540b3ad9b9aeb2aae123b): version 5 holds Update by s1c1-registrant
- lost update 4d1739a3-99c0-42d8-a58b-9d5078d1f54e v6 Update by s1c1-registrant 's1c1-registrant:21' via 39715 (stress-5fe8cbefd7cd4ea9821e4dbe5a997a24): version 6 holds Update by s1c1-registrant

server 39715:
- missing record 6f4c012d-8fdf-4c3a-8f76-f0d5d18254a2: missing (1 acknowledged changes)
- missing record 9bf446e4-6b99-4754-9ab5-1067201f9ca2: missing (1 acknowledged changes)
- missing record 7c8bcd96-0c88-4abf-b788-7ea100ec573c: missing (1 acknowledged changes)
- missing record dd5ea6bd-8587-4e2b-819f-604924ba76be: missing (1 acknowledged changes)
- missing record e796005a-2ca3-4014-9bf5-da04899e9956: missing (1 acknowledged changes)
- missing record 4aa86ea4-f8fc-498f-8979-9c5b3d7e9e8c: missing (1 acknowledged changes)
- missing record 9ed9bb36-56de-4aa4-b27c-d292268fb878: missing (1 acknowledged changes)
- missing record f896579a-6bc2-4b52-a550-287a72fa28b7: missing (1 acknowledged changes)
- missing record e4b47a74-f791-4a86-ab2a-81cc13bbe192: missing (1 acknowledged changes)
- missing record 424c6cf7-07f7-4ef4-a852-3b915fa69ecf: missing (1 acknowledged changes)
- missing record af852f26-0bde-494f-b91c-c09cca26f7d9: missing (1 acknowledged changes)
- missing record bc391423-47df-407b-a495-9fdbc5a60fed: missing (1 acknowledged changes)
- missing record 7d9ef809-9982-4e94-a6e3-1841c4bbcf35: missing (1 acknowledged changes)
- missing record eae957e1-befd-42d6-baa7-c474baabac7b: missing (1 acknowledged changes)
- missing record 28db6884-f144-4579-b66d-eac338b51d21: missing (1 acknowledged changes)
- missing record ccc81a55-92a7-41c8-be9d-c7ec4b3688c7: missing (1 acknowledged changes)
- missing record 8e7363d3-1c7c-4f92-98c5-76e8592e62e4: missing (1 acknowledged changes)
- missing record 70b12058-fbec-43af-98da-421d3531e20f: missing (1 acknowledged changes)
- missing record 7f5f422f-224f-4094-b879-e74d95789f8b: missing (1 acknowledged changes)
- missing record c4b63778-0f51-496a-acf2-4c40a9c35c1a: missing (1 acknowledged changes)
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v3 Update by s1c1-registrant 's1c1-registrant:8' via 39715 (stress-5e7d23a54fc3495693414315583ee8dd): version 3 holds Comment by s1c5-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v5 Update by s1c1-registrant 's1c1-registrant:15' via 39715 (stress-89aea6d436694bf4a0d6300e0474b019): version 5 holds Reject by s3c4-admin
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v11 Comment by s1c2-reviewer 's1c2-reviewer:96' via 39715 (stress-a772b50b7e3c4378bf0cd733fb279aeb): version 11 holds Comment by s1c5-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v5 Update by s1c4-admin 's1c4-admin:46' via 39715 (stress-595f0d33a2294aacbef55075cfd340cd): version 5 holds Reject by s3c4-admin
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v6 Comment by s1c4-admin 's1c4-admin:51' via 39715 (stress-3e118aeb31a44e5f88841cdd5f9585f7): version 6 holds Comment by s3c5-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v8 Comment by s1c4-admin 's1c4-admin:82' via 39715 (stress-c416f96021c344afb38310ddc1233234): version 8 holds Comment by s1c2-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v15 Comment by s1c4-admin 's1c4-admin:164' via 39715 (stress-cf8032ab456d40b59bdab0b1fe7dedbb): version 15 holds Comment by s1c4-admin
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v11 Comment by s1c7-reviewer 's1c7-reviewer:109' via 39715 (stress-bc59bd0b765e41be82ca4103a056fd89): version 11 holds Comment by s1c5-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v12 Comment by s1c7-reviewer 's1c7-reviewer:123' via 39715 (stress-bb8431534c504ccdb2e0a2ed670af5ed): version 12 holds Comment by s3c5-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v11 Comment by s2c2-reviewer 's2c2-reviewer:97' via 39531 (stress-0f61fe8ad096455f9d5623db7cd33dc4): version 11 holds Comment by s1c5-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v16 Comment by s2c2-reviewer 's2c2-reviewer:174' via 39531 (stress-fa54180fe9924e78a00a6ca8b7679e09): version 16 holds Comment by s2c5-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v14 Comment by s2c7-reviewer 's2c7-reviewer:144' via 39531 (stress-c04c7fd39dc041bfb120655b54dd0e49): version 14 holds Comment by s2c2-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v11 Comment by s3c2-reviewer 's3c2-reviewer:144' via 35789 (stress-dee398f404a2424a94e753b419621709): version 11 holds Comment by s1c5-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v3 Comment by s3c4-admin 's3c4-admin:9' via 35789 (stress-d76045d2f50f4a2b956944fda3a2123d): version 3 holds Comment by s1c5-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v6 Comment by s3c4-admin 's3c4-admin:61' via 35789 (stress-1abb95a2a7f549e088338db95726ee08): version 6 holds Comment by s3c5-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v8 Comment by s3c4-admin 's3c4-admin:107' via 35789 (stress-268e0de3ec944bf489e306e001982a9f): version 8 holds Comment by s1c2-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v14 Comment by s3c5-reviewer 's3c5-reviewer:188' via 35789 (stress-e4c00c20f4cc41b487bb9c793fbd397a): version 14 holds Comment by s2c2-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v2 Comment by s3c7-reviewer 's3c7-reviewer:11' via 35789 (stress-d58467f8c8d34875aabc5171af8ae06e): version 2 holds Comment by s2c7-reviewer
- lost update 4d1739a3-99c0-42d8-a58b-9d5078d1f54e v5 Update by s1c1-registrant 's1c1-registrant:17' via 39715 (stress-b1e769c1a1a540b3ad9b9aeb2aae123b): version 5 holds Update by s1c1-registrant
- lost update 4d1739a3-99c0-42d8-a58b-9d5078d1f54e v6 Update by s1c1-registrant 's1c1-registrant:21' via 39715 (stress-5fe8cbefd7cd4ea9821e4dbe5a997a24): version 6 holds Update by s1c1-registrant

server 39531:
- missing record 6f4c012d-8fdf-4c3a-8f76-f0d5d18254a2: missing (1 acknowledged changes)
- missing record 9bf446e4-6b99-4754-9ab5-1067201f9ca2: missing (1 acknowledged changes)
- missing record 7c8bcd96-0c88-4abf-b788-7ea100ec573c: missing (1 acknowledged changes)
- missing record dd5ea6bd-8587-4e2b-819f-604924ba76be: missing (1 acknowledged changes)
- missing record e796005a-2ca3-4014-9bf5-da04899e9956: missing (1 acknowledged changes)
- missing record 4aa86ea4-f8fc-498f-8979-9c5b3d7e9e8c: missing (1 acknowledged changes)
- missing record 9ed9bb36-56de-4aa4-b27c-d292268fb878: missing (1 acknowledged changes)
- missing record f896579a-6bc2-4b52-a550-287a72fa28b7: missing (1 acknowledged changes)
- missing record e4b47a74-f791-4a86-ab2a-81cc13bbe192: missing (1 acknowledged changes)
- missing record 424c6cf7-07f7-4ef4-a852-3b915fa69ecf: missing (1 acknowledged changes)
- missing record af852f26-0bde-494f-b91c-c09cca26f7d9: missing (1 acknowledged changes)
- missing record bc391423-47df-407b-a495-9fdbc5a60fed: missing (1 acknowledged changes)
- missing record 7d9ef809-9982-4e94-a6e3-1841c4bbcf35: missing (1 acknowledged changes)
- missing record eae957e1-befd-42d6-baa7-c474baabac7b: missing (1 acknowledged changes)
- missing record 28db6884-f144-4579-b66d-eac338b51d21: missing (1 acknowledged changes)
- missing record ccc81a55-92a7-41c8-be9d-c7ec4b3688c7: missing (1 acknowledged changes)
- missing record 8e7363d3-1c7c-4f92-98c5-76e8592e62e4: missing (1 acknowledged changes)
- missing record 70b12058-fbec-43af-98da-421d3531e20f: missing (1 acknowledged changes)
- missing record 7f5f422f-224f-4094-b879-e74d95789f8b: missing (1 acknowledged changes)
- missing record c4b63778-0f51-496a-acf2-4c40a9c35c1a: missing (1 acknowledged changes)
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v3 Update by s1c1-registrant 's1c1-registrant:8' via 39715 (stress-5e7d23a54fc3495693414315583ee8dd): version 3 holds Comment by s1c5-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v5 Update by s1c1-registrant 's1c1-registrant:15' via 39715 (stress-89aea6d436694bf4a0d6300e0474b019): version 5 holds Reject by s3c4-admin
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v11 Comment by s1c2-reviewer 's1c2-reviewer:96' via 39715 (stress-a772b50b7e3c4378bf0cd733fb279aeb): version 11 holds Comment by s1c5-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v5 Update by s1c4-admin 's1c4-admin:46' via 39715 (stress-595f0d33a2294aacbef55075cfd340cd): version 5 holds Reject by s3c4-admin
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v6 Comment by s1c4-admin 's1c4-admin:51' via 39715 (stress-3e118aeb31a44e5f88841cdd5f9585f7): version 6 holds Comment by s3c5-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v8 Comment by s1c4-admin 's1c4-admin:82' via 39715 (stress-c416f96021c344afb38310ddc1233234): version 8 holds Comment by s1c2-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v15 Comment by s1c4-admin 's1c4-admin:164' via 39715 (stress-cf8032ab456d40b59bdab0b1fe7dedbb): version 15 holds Comment by s1c4-admin
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v11 Comment by s1c7-reviewer 's1c7-reviewer:109' via 39715 (stress-bc59bd0b765e41be82ca4103a056fd89): version 11 holds Comment by s1c5-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v12 Comment by s1c7-reviewer 's1c7-reviewer:123' via 39715 (stress-bb8431534c504ccdb2e0a2ed670af5ed): version 12 holds Comment by s3c5-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v11 Comment by s2c2-reviewer 's2c2-reviewer:97' via 39531 (stress-0f61fe8ad096455f9d5623db7cd33dc4): version 11 holds Comment by s1c5-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v16 Comment by s2c2-reviewer 's2c2-reviewer:174' via 39531 (stress-fa54180fe9924e78a00a6ca8b7679e09): version 16 holds Comment by s2c5-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v14 Comment by s2c7-reviewer 's2c7-reviewer:144' via 39531 (stress-c04c7fd39dc041bfb120655b54dd0e49): version 14 holds Comment by s2c2-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v11 Comment by s3c2-reviewer 's3c2-reviewer:144' via 35789 (stress-dee398f404a2424a94e753b419621709): version 11 holds Comment by s1c5-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v3 Comment by s3c4-admin 's3c4-admin:9' via 35789 (stress-d76045d2f50f4a2b956944fda3a2123d): version 3 holds Comment by s1c5-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v6 Comment by s3c4-admin 's3c4-admin:61' via 35789 (stress-1abb95a2a7f549e088338db95726ee08): version 6 holds Comment by s3c5-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v8 Comment by s3c4-admin 's3c4-admin:107' via 35789 (stress-268e0de3ec944bf489e306e001982a9f): version 8 holds Comment by s1c2-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v14 Comment by s3c5-reviewer 's3c5-reviewer:188' via 35789 (stress-e4c00c20f4cc41b487bb9c793fbd397a): version 14 holds Comment by s2c2-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v2 Comment by s3c7-reviewer 's3c7-reviewer:11' via 35789 (stress-d58467f8c8d34875aabc5171af8ae06e): version 2 holds Comment by s2c7-reviewer
- lost update 4d1739a3-99c0-42d8-a58b-9d5078d1f54e v5 Update by s1c1-registrant 's1c1-registrant:17' via 39715 (stress-b1e769c1a1a540b3ad9b9aeb2aae123b): version 5 holds Update by s1c1-registrant
- lost update 4d1739a3-99c0-42d8-a58b-9d5078d1f54e v6 Update by s1c1-registrant 's1c1-registrant:21' via 39715 (stress-5fe8cbefd7cd4ea9821e4dbe5a997a24): version 6 holds Update by s1c1-registrant

server 35789:
- missing record 6f4c012d-8fdf-4c3a-8f76-f0d5d18254a2: missing (1 acknowledged changes)
- missing record 9bf446e4-6b99-4754-9ab5-1067201f9ca2: missing (1 acknowledged changes)
- missing record 7c8bcd96-0c88-4abf-b788-7ea100ec573c: missing (1 acknowledged changes)
- missing record dd5ea6bd-8587-4e2b-819f-604924ba76be: missing (1 acknowledged changes)
- missing record e796005a-2ca3-4014-9bf5-da04899e9956: missing (1 acknowledged changes)
- missing record 4aa86ea4-f8fc-498f-8979-9c5b3d7e9e8c: missing (1 acknowledged changes)
- missing record 9ed9bb36-56de-4aa4-b27c-d292268fb878: missing (1 acknowledged changes)
- missing record f896579a-6bc2-4b52-a550-287a72fa28b7: missing (1 acknowledged changes)
- missing record e4b47a74-f791-4a86-ab2a-81cc13bbe192: missing (1 acknowledged changes)
- missing record 424c6cf7-07f7-4ef4-a852-3b915fa69ecf: missing (1 acknowledged changes)
- missing record af852f26-0bde-494f-b91c-c09cca26f7d9: missing (1 acknowledged changes)
- missing record bc391423-47df-407b-a495-9fdbc5a60fed: missing (1 acknowledged changes)
- missing record 7d9ef809-9982-4e94-a6e3-1841c4bbcf35: missing (1 acknowledged changes)
- missing record eae957e1-befd-42d6-baa7-c474baabac7b: missing (1 acknowledged changes)
- missing record 28db6884-f144-4579-b66d-eac338b51d21: missing (1 acknowledged changes)
- missing record ccc81a55-92a7-41c8-be9d-c7ec4b3688c7: missing (1 acknowledged changes)
- missing record 8e7363d3-1c7c-4f92-98c5-76e8592e62e4: missing (1 acknowledged changes)
- missing record 70b12058-fbec-43af-98da-421d3531e20f: missing (1 acknowledged changes)
- missing record 7f5f422f-224f-4094-b879-e74d95789f8b: missing (1 acknowledged changes)
- missing record c4b63778-0f51-496a-acf2-4c40a9c35c1a: missing (1 acknowledged changes)
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v3 Update by s1c1-registrant 's1c1-registrant:8' via 39715 (stress-5e7d23a54fc3495693414315583ee8dd): version 3 holds Comment by s1c5-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v5 Update by s1c1-registrant 's1c1-registrant:15' via 39715 (stress-89aea6d436694bf4a0d6300e0474b019): version 5 holds Reject by s3c4-admin
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v11 Comment by s1c2-reviewer 's1c2-reviewer:96' via 39715 (stress-a772b50b7e3c4378bf0cd733fb279aeb): version 11 holds Comment by s1c5-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v5 Update by s1c4-admin 's1c4-admin:46' via 39715 (stress-595f0d33a2294aacbef55075cfd340cd): version 5 holds Reject by s3c4-admin
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v6 Comment by s1c4-admin 's1c4-admin:51' via 39715 (stress-3e118aeb31a44e5f88841cdd5f9585f7): version 6 holds Comment by s3c5-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v8 Comment by s1c4-admin 's1c4-admin:82' via 39715 (stress-c416f96021c344afb38310ddc1233234): version 8 holds Comment by s1c2-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v15 Comment by s1c4-admin 's1c4-admin:164' via 39715 (stress-cf8032ab456d40b59bdab0b1fe7dedbb): version 15 holds Comment by s1c4-admin
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v11 Comment by s1c7-reviewer 's1c7-reviewer:109' via 39715 (stress-bc59bd0b765e41be82ca4103a056fd89): version 11 holds Comment by s1c5-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v12 Comment by s1c7-reviewer 's1c7-reviewer:123' via 39715 (stress-bb8431534c504ccdb2e0a2ed670af5ed): version 12 holds Comment by s3c5-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v11 Comment by s2c2-reviewer 's2c2-reviewer:97' via 39531 (stress-0f61fe8ad096455f9d5623db7cd33dc4): version 11 holds Comment by s1c5-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v16 Comment by s2c2-reviewer 's2c2-reviewer:174' via 39531 (stress-fa54180fe9924e78a00a6ca8b7679e09): version 16 holds Comment by s2c5-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v14 Comment by s2c7-reviewer 's2c7-reviewer:144' via 39531 (stress-c04c7fd39dc041bfb120655b54dd0e49): version 14 holds Comment by s2c2-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v11 Comment by s3c2-reviewer 's3c2-reviewer:144' via 35789 (stress-dee398f404a2424a94e753b419621709): version 11 holds Comment by s1c5-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v3 Comment by s3c4-admin 's3c4-admin:9' via 35789 (stress-d76045d2f50f4a2b956944fda3a2123d): version 3 holds Comment by s1c5-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v6 Comment by s3c4-admin 's3c4-admin:61' via 35789 (stress-1abb95a2a7f549e088338db95726ee08): version 6 holds Comment by s3c5-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v8 Comment by s3c4-admin 's3c4-admin:107' via 35789 (stress-268e0de3ec944bf489e306e001982a9f): version 8 holds Comment by s1c2-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v14 Comment by s3c5-reviewer 's3c5-reviewer:188' via 35789 (stress-e4c00c20f4cc41b487bb9c793fbd397a): version 14 holds Comment by s2c2-reviewer
- lost update 394ca794-a670-4719-a6d4-348ae645ebe6 v2 Comment by s3c7-reviewer 's3c7-reviewer:11' via 35789 (stress-d58467f8c8d34875aabc5171af8ae06e): version 2 holds Comment by s2c7-reviewer
- lost update 4d1739a3-99c0-42d8-a58b-9d5078d1f54e v5 Update by s1c1-registrant 's1c1-registrant:17' via 39715 (stress-b1e769c1a1a540b3ad9b9aeb2aae123b): version 5 holds Update by s1c1-registrant
- lost update 4d1739a3-99c0-42d8-a58b-9d5078d1f54e v6 Update by s1c1-registrant 's1c1-registrant:21' via 39715 (stress-5fe8cbefd7cd4ea9821e4dbe5a997a24): version 6 holds Update by s1c1-registrant

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 3 | 475 | 182.0 | 93 | 385 | 225 | 0 | 0 | 29.9 | no |
