# Stress run: NaiveFile, 3 server(s): FAILED

- Commit `ee6af83bef739621009cd4be91192b48f8df58df`, 2026-10-17 19:08:37 UTC, 1.6 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 12 records, 200 changes per client, seed 2
- Data folder `stress-runs/heavy-NaiveFile-2/data`; run folder `stress-runs/heavy-NaiveFile-2`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 409 | 133 | 81.5 | 110 | 154 | 0 | 5.8 | 114.6 | 115.4 |

Writes by outcome: conflict 110, http500 12, saved 133, wrongStatus 154. Saves by action: Create 12, Update 11, Submit 1, Approve 1, Comment 108. Reads: 4800 (http500 134, notFound 4269).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | FAIL | 1/12 | 11 | 44 | 0 | 0 | 0 |
| server 39607 | FAIL | 1/12 | 11 | 44 | 0 | 0 | 0 |
| server 34047 | FAIL | 1/12 | 11 | 44 | 0 | 0 | 0 |
| server 46087 | FAIL | 1/12 | 11 | 44 | 0 | 0 | 0 |

storage:
- missing record 143f49f7-cf63-40eb-8408-7e2ec46f4dbb: missing (1 acknowledged changes)
- missing record 0a5fb55c-47fe-4ce9-bdd8-921bfe08f673: missing (1 acknowledged changes)
- missing record cb2332f2-1d3f-440a-b45d-3c973c0ea986: missing (1 acknowledged changes)
- missing record 394add29-b8eb-46fd-9f07-588f714a638a: missing (1 acknowledged changes)
- missing record c45609a5-1ada-4c96-b87a-ac2d7207076f: missing (1 acknowledged changes)
- missing record 8334b1d4-90a6-43ef-8aab-f2cdce59be91: missing (1 acknowledged changes)
- missing record dfde47ea-5c5d-4ff7-888d-b845357a375a: missing (1 acknowledged changes)
- missing record 9a83ec2e-f698-4d20-9033-cf25e9877757: missing (1 acknowledged changes)
- missing record f9814baa-6afc-4a1b-8d62-a50509c2c582: missing (1 acknowledged changes)
- missing record c640a809-494a-4691-9b36-441ee6e59e5a: missing (1 acknowledged changes)
- missing record e3b06731-f5df-426a-8b0b-75000ab213ac: missing (1 acknowledged changes)
- lost update 143f49f7-cf63-40eb-8408-7e2ec46f4dbb v1 Create by s1c1-registrant via 39607 (stress-5fab8ec010e54181bea777296b4e4e09): record missing
- lost update 0a5fb55c-47fe-4ce9-bdd8-921bfe08f673 v1 Create by s2c1-registrant via 34047 (stress-daec5088969445eb90d21f75a83c1127): record missing
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v2 Comment by s1c2-reviewer 's1c2-reviewer:3' via 39607 (stress-c86cfeb6e52b49f88c8217c9bc77ccb8): version 2 holds Update by s2c4-admin
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v41 Comment by s1c2-reviewer 's1c2-reviewer:75' via 39607 (stress-3a29027e0d734343bd33dff817b2eeeb): version 41 holds Comment by s1c7-reviewer
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v45 Comment by s1c2-reviewer 's1c2-reviewer:88' via 39607 (stress-d032fc9dfb7e4ccda187f02cebb40674): version 45 holds Comment by s3c5-reviewer
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v47 Comment by s1c2-reviewer 's1c2-reviewer:94' via 39607 (stress-36bf8953940942edaba1052949a4b9af): version 47 holds Comment by s3c4-admin
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v2 Update by s1c4-admin 's1c4-admin:24' via 39607 (stress-46e6cfc0036d4920a296ad2a7ad439c0): version 2 holds Update by s2c4-admin
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v46 Comment by s1c4-admin 's1c4-admin:112' via 39607 (stress-b7f1208525b249b794a755ac3000342a): version 46 holds Comment by s3c7-reviewer
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v60 Comment by s1c4-admin 's1c4-admin:142' via 39607 (stress-740599db9dd749c88d6b976f793640bf): version 60 holds Comment by s1c7-reviewer
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v62 Comment by s1c4-admin 's1c4-admin:145' via 39607 (stress-b418608563624ac496e2542505074e04): version 62 holds Comment by s2c5-reviewer
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v64 Comment by s1c5-reviewer 's1c5-reviewer:134' via 39607 (stress-d001873dd7af40e59b8741a3f490915f): version 64 holds Comment by s3c7-reviewer
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v88 Comment by s1c5-reviewer 's1c5-reviewer:195' via 39607 (stress-468ea82095f74af3b8b45180375a55f5): version 88 holds Comment by s2c5-reviewer
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v38 Comment by s1c7-reviewer 's1c7-reviewer:58' via 39607 (stress-586d4f7718ad43d2862a181fe3573515): version 38 holds Comment by s2c5-reviewer
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v44 Comment by s1c7-reviewer 's1c7-reviewer:69' via 39607 (stress-f4f2808114154d798ebbb6229f080b61): version 44 holds Comment by s1c5-reviewer
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v45 Comment by s1c7-reviewer 's1c7-reviewer:70' via 39607 (stress-4bf2cff62cf24d1f9c98ba965189592b): version 45 holds Comment by s3c5-reviewer
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v48 Comment by s1c7-reviewer 's1c7-reviewer:80' via 39607 (stress-31f774c19d3f4a1d82e82ea081a9e346): version 48 holds Comment by s2c7-reviewer
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v2 Comment by s2c2-reviewer 's2c2-reviewer:4' via 34047 (stress-3c5abca2c95840c586dac7a798b3a82b): version 2 holds Update by s2c4-admin
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v50 Comment by s2c2-reviewer 's2c2-reviewer:109' via 34047 (stress-06f44cf1e6a549228ff672ad2d49150d): version 50 holds Comment by s3c4-admin
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v70 Comment by s2c2-reviewer 's2c2-reviewer:159' via 34047 (stress-f74823a289844daca41591c28d04b9c6): version 70 holds Comment by s2c4-admin
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v33 Comment by s2c4-admin 's2c4-admin:69' via 34047 (stress-728eedb7abce4cc89e7cb1d7986b7ca8): version 33 holds Comment by s1c5-reviewer

server 39607:
- missing record 143f49f7-cf63-40eb-8408-7e2ec46f4dbb: missing (1 acknowledged changes)
- missing record 0a5fb55c-47fe-4ce9-bdd8-921bfe08f673: missing (1 acknowledged changes)
- missing record cb2332f2-1d3f-440a-b45d-3c973c0ea986: missing (1 acknowledged changes)
- missing record 394add29-b8eb-46fd-9f07-588f714a638a: missing (1 acknowledged changes)
- missing record c45609a5-1ada-4c96-b87a-ac2d7207076f: missing (1 acknowledged changes)
- missing record 8334b1d4-90a6-43ef-8aab-f2cdce59be91: missing (1 acknowledged changes)
- missing record dfde47ea-5c5d-4ff7-888d-b845357a375a: missing (1 acknowledged changes)
- missing record 9a83ec2e-f698-4d20-9033-cf25e9877757: missing (1 acknowledged changes)
- missing record f9814baa-6afc-4a1b-8d62-a50509c2c582: missing (1 acknowledged changes)
- missing record c640a809-494a-4691-9b36-441ee6e59e5a: missing (1 acknowledged changes)
- missing record e3b06731-f5df-426a-8b0b-75000ab213ac: missing (1 acknowledged changes)
- lost update 143f49f7-cf63-40eb-8408-7e2ec46f4dbb v1 Create by s1c1-registrant via 39607 (stress-5fab8ec010e54181bea777296b4e4e09): record missing
- lost update 0a5fb55c-47fe-4ce9-bdd8-921bfe08f673 v1 Create by s2c1-registrant via 34047 (stress-daec5088969445eb90d21f75a83c1127): record missing
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v2 Comment by s1c2-reviewer 's1c2-reviewer:3' via 39607 (stress-c86cfeb6e52b49f88c8217c9bc77ccb8): version 2 holds Update by s2c4-admin
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v41 Comment by s1c2-reviewer 's1c2-reviewer:75' via 39607 (stress-3a29027e0d734343bd33dff817b2eeeb): version 41 holds Comment by s1c7-reviewer
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v45 Comment by s1c2-reviewer 's1c2-reviewer:88' via 39607 (stress-d032fc9dfb7e4ccda187f02cebb40674): version 45 holds Comment by s3c5-reviewer
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v47 Comment by s1c2-reviewer 's1c2-reviewer:94' via 39607 (stress-36bf8953940942edaba1052949a4b9af): version 47 holds Comment by s3c4-admin
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v2 Update by s1c4-admin 's1c4-admin:24' via 39607 (stress-46e6cfc0036d4920a296ad2a7ad439c0): version 2 holds Update by s2c4-admin
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v46 Comment by s1c4-admin 's1c4-admin:112' via 39607 (stress-b7f1208525b249b794a755ac3000342a): version 46 holds Comment by s3c7-reviewer
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v60 Comment by s1c4-admin 's1c4-admin:142' via 39607 (stress-740599db9dd749c88d6b976f793640bf): version 60 holds Comment by s1c7-reviewer
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v62 Comment by s1c4-admin 's1c4-admin:145' via 39607 (stress-b418608563624ac496e2542505074e04): version 62 holds Comment by s2c5-reviewer
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v64 Comment by s1c5-reviewer 's1c5-reviewer:134' via 39607 (stress-d001873dd7af40e59b8741a3f490915f): version 64 holds Comment by s3c7-reviewer
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v88 Comment by s1c5-reviewer 's1c5-reviewer:195' via 39607 (stress-468ea82095f74af3b8b45180375a55f5): version 88 holds Comment by s2c5-reviewer
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v38 Comment by s1c7-reviewer 's1c7-reviewer:58' via 39607 (stress-586d4f7718ad43d2862a181fe3573515): version 38 holds Comment by s2c5-reviewer
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v44 Comment by s1c7-reviewer 's1c7-reviewer:69' via 39607 (stress-f4f2808114154d798ebbb6229f080b61): version 44 holds Comment by s1c5-reviewer
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v45 Comment by s1c7-reviewer 's1c7-reviewer:70' via 39607 (stress-4bf2cff62cf24d1f9c98ba965189592b): version 45 holds Comment by s3c5-reviewer
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v48 Comment by s1c7-reviewer 's1c7-reviewer:80' via 39607 (stress-31f774c19d3f4a1d82e82ea081a9e346): version 48 holds Comment by s2c7-reviewer
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v2 Comment by s2c2-reviewer 's2c2-reviewer:4' via 34047 (stress-3c5abca2c95840c586dac7a798b3a82b): version 2 holds Update by s2c4-admin
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v50 Comment by s2c2-reviewer 's2c2-reviewer:109' via 34047 (stress-06f44cf1e6a549228ff672ad2d49150d): version 50 holds Comment by s3c4-admin
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v70 Comment by s2c2-reviewer 's2c2-reviewer:159' via 34047 (stress-f74823a289844daca41591c28d04b9c6): version 70 holds Comment by s2c4-admin
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v33 Comment by s2c4-admin 's2c4-admin:69' via 34047 (stress-728eedb7abce4cc89e7cb1d7986b7ca8): version 33 holds Comment by s1c5-reviewer

server 34047:
- missing record 143f49f7-cf63-40eb-8408-7e2ec46f4dbb: missing (1 acknowledged changes)
- missing record 0a5fb55c-47fe-4ce9-bdd8-921bfe08f673: missing (1 acknowledged changes)
- missing record cb2332f2-1d3f-440a-b45d-3c973c0ea986: missing (1 acknowledged changes)
- missing record 394add29-b8eb-46fd-9f07-588f714a638a: missing (1 acknowledged changes)
- missing record c45609a5-1ada-4c96-b87a-ac2d7207076f: missing (1 acknowledged changes)
- missing record 8334b1d4-90a6-43ef-8aab-f2cdce59be91: missing (1 acknowledged changes)
- missing record dfde47ea-5c5d-4ff7-888d-b845357a375a: missing (1 acknowledged changes)
- missing record 9a83ec2e-f698-4d20-9033-cf25e9877757: missing (1 acknowledged changes)
- missing record f9814baa-6afc-4a1b-8d62-a50509c2c582: missing (1 acknowledged changes)
- missing record c640a809-494a-4691-9b36-441ee6e59e5a: missing (1 acknowledged changes)
- missing record e3b06731-f5df-426a-8b0b-75000ab213ac: missing (1 acknowledged changes)
- lost update 143f49f7-cf63-40eb-8408-7e2ec46f4dbb v1 Create by s1c1-registrant via 39607 (stress-5fab8ec010e54181bea777296b4e4e09): record missing
- lost update 0a5fb55c-47fe-4ce9-bdd8-921bfe08f673 v1 Create by s2c1-registrant via 34047 (stress-daec5088969445eb90d21f75a83c1127): record missing
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v2 Comment by s1c2-reviewer 's1c2-reviewer:3' via 39607 (stress-c86cfeb6e52b49f88c8217c9bc77ccb8): version 2 holds Update by s2c4-admin
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v41 Comment by s1c2-reviewer 's1c2-reviewer:75' via 39607 (stress-3a29027e0d734343bd33dff817b2eeeb): version 41 holds Comment by s1c7-reviewer
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v45 Comment by s1c2-reviewer 's1c2-reviewer:88' via 39607 (stress-d032fc9dfb7e4ccda187f02cebb40674): version 45 holds Comment by s3c5-reviewer
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v47 Comment by s1c2-reviewer 's1c2-reviewer:94' via 39607 (stress-36bf8953940942edaba1052949a4b9af): version 47 holds Comment by s3c4-admin
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v2 Update by s1c4-admin 's1c4-admin:24' via 39607 (stress-46e6cfc0036d4920a296ad2a7ad439c0): version 2 holds Update by s2c4-admin
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v46 Comment by s1c4-admin 's1c4-admin:112' via 39607 (stress-b7f1208525b249b794a755ac3000342a): version 46 holds Comment by s3c7-reviewer
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v60 Comment by s1c4-admin 's1c4-admin:142' via 39607 (stress-740599db9dd749c88d6b976f793640bf): version 60 holds Comment by s1c7-reviewer
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v62 Comment by s1c4-admin 's1c4-admin:145' via 39607 (stress-b418608563624ac496e2542505074e04): version 62 holds Comment by s2c5-reviewer
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v64 Comment by s1c5-reviewer 's1c5-reviewer:134' via 39607 (stress-d001873dd7af40e59b8741a3f490915f): version 64 holds Comment by s3c7-reviewer
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v88 Comment by s1c5-reviewer 's1c5-reviewer:195' via 39607 (stress-468ea82095f74af3b8b45180375a55f5): version 88 holds Comment by s2c5-reviewer
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v38 Comment by s1c7-reviewer 's1c7-reviewer:58' via 39607 (stress-586d4f7718ad43d2862a181fe3573515): version 38 holds Comment by s2c5-reviewer
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v44 Comment by s1c7-reviewer 's1c7-reviewer:69' via 39607 (stress-f4f2808114154d798ebbb6229f080b61): version 44 holds Comment by s1c5-reviewer
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v45 Comment by s1c7-reviewer 's1c7-reviewer:70' via 39607 (stress-4bf2cff62cf24d1f9c98ba965189592b): version 45 holds Comment by s3c5-reviewer
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v48 Comment by s1c7-reviewer 's1c7-reviewer:80' via 39607 (stress-31f774c19d3f4a1d82e82ea081a9e346): version 48 holds Comment by s2c7-reviewer
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v2 Comment by s2c2-reviewer 's2c2-reviewer:4' via 34047 (stress-3c5abca2c95840c586dac7a798b3a82b): version 2 holds Update by s2c4-admin
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v50 Comment by s2c2-reviewer 's2c2-reviewer:109' via 34047 (stress-06f44cf1e6a549228ff672ad2d49150d): version 50 holds Comment by s3c4-admin
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v70 Comment by s2c2-reviewer 's2c2-reviewer:159' via 34047 (stress-f74823a289844daca41591c28d04b9c6): version 70 holds Comment by s2c4-admin
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v33 Comment by s2c4-admin 's2c4-admin:69' via 34047 (stress-728eedb7abce4cc89e7cb1d7986b7ca8): version 33 holds Comment by s1c5-reviewer

server 46087:
- missing record 143f49f7-cf63-40eb-8408-7e2ec46f4dbb: missing (1 acknowledged changes)
- missing record 0a5fb55c-47fe-4ce9-bdd8-921bfe08f673: missing (1 acknowledged changes)
- missing record cb2332f2-1d3f-440a-b45d-3c973c0ea986: missing (1 acknowledged changes)
- missing record 394add29-b8eb-46fd-9f07-588f714a638a: missing (1 acknowledged changes)
- missing record c45609a5-1ada-4c96-b87a-ac2d7207076f: missing (1 acknowledged changes)
- missing record 8334b1d4-90a6-43ef-8aab-f2cdce59be91: missing (1 acknowledged changes)
- missing record dfde47ea-5c5d-4ff7-888d-b845357a375a: missing (1 acknowledged changes)
- missing record 9a83ec2e-f698-4d20-9033-cf25e9877757: missing (1 acknowledged changes)
- missing record f9814baa-6afc-4a1b-8d62-a50509c2c582: missing (1 acknowledged changes)
- missing record c640a809-494a-4691-9b36-441ee6e59e5a: missing (1 acknowledged changes)
- missing record e3b06731-f5df-426a-8b0b-75000ab213ac: missing (1 acknowledged changes)
- lost update 143f49f7-cf63-40eb-8408-7e2ec46f4dbb v1 Create by s1c1-registrant via 39607 (stress-5fab8ec010e54181bea777296b4e4e09): record missing
- lost update 0a5fb55c-47fe-4ce9-bdd8-921bfe08f673 v1 Create by s2c1-registrant via 34047 (stress-daec5088969445eb90d21f75a83c1127): record missing
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v2 Comment by s1c2-reviewer 's1c2-reviewer:3' via 39607 (stress-c86cfeb6e52b49f88c8217c9bc77ccb8): version 2 holds Update by s2c4-admin
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v41 Comment by s1c2-reviewer 's1c2-reviewer:75' via 39607 (stress-3a29027e0d734343bd33dff817b2eeeb): version 41 holds Comment by s1c7-reviewer
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v45 Comment by s1c2-reviewer 's1c2-reviewer:88' via 39607 (stress-d032fc9dfb7e4ccda187f02cebb40674): version 45 holds Comment by s3c5-reviewer
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v47 Comment by s1c2-reviewer 's1c2-reviewer:94' via 39607 (stress-36bf8953940942edaba1052949a4b9af): version 47 holds Comment by s3c4-admin
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v2 Update by s1c4-admin 's1c4-admin:24' via 39607 (stress-46e6cfc0036d4920a296ad2a7ad439c0): version 2 holds Update by s2c4-admin
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v46 Comment by s1c4-admin 's1c4-admin:112' via 39607 (stress-b7f1208525b249b794a755ac3000342a): version 46 holds Comment by s3c7-reviewer
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v60 Comment by s1c4-admin 's1c4-admin:142' via 39607 (stress-740599db9dd749c88d6b976f793640bf): version 60 holds Comment by s1c7-reviewer
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v62 Comment by s1c4-admin 's1c4-admin:145' via 39607 (stress-b418608563624ac496e2542505074e04): version 62 holds Comment by s2c5-reviewer
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v64 Comment by s1c5-reviewer 's1c5-reviewer:134' via 39607 (stress-d001873dd7af40e59b8741a3f490915f): version 64 holds Comment by s3c7-reviewer
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v88 Comment by s1c5-reviewer 's1c5-reviewer:195' via 39607 (stress-468ea82095f74af3b8b45180375a55f5): version 88 holds Comment by s2c5-reviewer
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v38 Comment by s1c7-reviewer 's1c7-reviewer:58' via 39607 (stress-586d4f7718ad43d2862a181fe3573515): version 38 holds Comment by s2c5-reviewer
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v44 Comment by s1c7-reviewer 's1c7-reviewer:69' via 39607 (stress-f4f2808114154d798ebbb6229f080b61): version 44 holds Comment by s1c5-reviewer
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v45 Comment by s1c7-reviewer 's1c7-reviewer:70' via 39607 (stress-4bf2cff62cf24d1f9c98ba965189592b): version 45 holds Comment by s3c5-reviewer
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v48 Comment by s1c7-reviewer 's1c7-reviewer:80' via 39607 (stress-31f774c19d3f4a1d82e82ea081a9e346): version 48 holds Comment by s2c7-reviewer
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v2 Comment by s2c2-reviewer 's2c2-reviewer:4' via 34047 (stress-3c5abca2c95840c586dac7a798b3a82b): version 2 holds Update by s2c4-admin
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v50 Comment by s2c2-reviewer 's2c2-reviewer:109' via 34047 (stress-06f44cf1e6a549228ff672ad2d49150d): version 50 holds Comment by s3c4-admin
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v70 Comment by s2c2-reviewer 's2c2-reviewer:159' via 34047 (stress-f74823a289844daca41591c28d04b9c6): version 70 holds Comment by s2c4-admin
- lost update a1d3e45c-96c7-449e-856b-b483d7b96408 v33 Comment by s2c4-admin 's2c4-admin:69' via 34047 (stress-728eedb7abce4cc89e7cb1d7986b7ca8): version 33 holds Comment by s1c5-reviewer

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 3 | 133 | 81.5 | 110 | 154 | 44 | 0 | 0 | 114.6 | no |
