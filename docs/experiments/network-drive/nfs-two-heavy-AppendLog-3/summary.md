# Stress run: AppendLog, 3 server(s): FAILED

- Commit `1b49659b6d0d95e1a10858057ef4082c27040edb`, 2026-10-17 22:38:01 UTC, 54.8 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 12 records, 200 changes per client, seed 3
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `/mnt/share/runs/nfs-two-heavy-AppendLog-3/data`; run folder `stress-runs/nfs/nfs-two-heavy-AppendLog-3`
- 1 of the servers on `user@mac.example`, data folder `/Volumes/share/runs/nfs-two-heavy-AppendLog-3/data` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4613 | 2219 | 40.5 | 870 | 1524 | 0 | 176.9 | 515.2 | 940.8 |

Writes by outcome: conflict 870, saved 2219, wrongStatus 1524. Saves by action: Create 12, Update 918, Submit 18, Approve 13, Reject 9, Comment 1249. Reads: 4800 (notFound 199).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | FAIL | 11/12 | 1 | 1476 | 0 | 0 | 0 |
| server 35451 | FAIL | 11/12 | 1 | 1141 | 0 | 0 | 0 |
| server 37083 | FAIL | 11/12 | 1 | 1179 | 0 | 0 | 0 |
| server mac-host.local:45359 | FAIL | 11/12 | 1 | 1476 | 0 | 0 | 0 |

storage:
- missing record 0fe38969-ca08-4c97-9dad-afaa938fb61c: missing (195 acknowledged changes)
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v1 Create by s1c1-registrant via 35451 (stress-c754acf791af4cff8b78f571d26af42d): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v2 Submit by s1c1-registrant via 35451 (stress-182908025fa24d8fadc703931c496d75): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v3 Update by s1c1-registrant 's1c1-registrant:1' via 35451 (stress-a19a461612454a70b7ff158d2b878c2c): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v4 Update by s1c1-registrant 's1c1-registrant:2' via 35451 (stress-a69df3df4f8c4bad9698ee1337619558): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v5 Update by s1c1-registrant 's1c1-registrant:3' via 35451 (stress-3f14b405f5ab4158b639e54886286b38): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v6 Update by s1c1-registrant 's1c1-registrant:4' via 35451 (stress-a783f5e18e88410e973aff8ce367e8b1): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v7 Update by s1c1-registrant 's1c1-registrant:5' via 35451 (stress-c0efe686f5364f6790c7a6d337e4bf5b): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v8 Update by s1c1-registrant 's1c1-registrant:6' via 35451 (stress-caf1de49545d417e943d17b3824434e1): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v9 Update by s1c1-registrant 's1c1-registrant:7' via 35451 (stress-40b58a03cb3e4a918b223e7c8fc1eff1): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v10 Update by s1c1-registrant 's1c1-registrant:8' via 35451 (stress-4ab35ac15f074185b20e5448b7b19dac): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v11 Update by s1c1-registrant 's1c1-registrant:9' via 35451 (stress-b99cb11480f247fab81ea06f617a940a): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v12 Update by s1c1-registrant 's1c1-registrant:10' via 35451 (stress-444b867c7837493183c6292c4a9749d7): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v13 Update by s1c1-registrant 's1c1-registrant:11' via 35451 (stress-d012ac539a8e4ad193ae24e2ea202a79): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v14 Update by s1c1-registrant 's1c1-registrant:12' via 35451 (stress-f3cd7bbbb2d34455b1754479de94ce9a): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v15 Update by s1c1-registrant 's1c1-registrant:13' via 35451 (stress-fc078465e5074aa985c61e0818c4d63c): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v16 Update by s1c1-registrant 's1c1-registrant:14' via 35451 (stress-d3459da07e7f4c3dae0e51b5b22f968b): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v17 Update by s1c1-registrant 's1c1-registrant:15' via 35451 (stress-a6e3c85f25504bf08b0511e9a75e8b81): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v18 Update by s1c1-registrant 's1c1-registrant:16' via 35451 (stress-631a2db83d784cb1a15ac11fde1681cf): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v19 Update by s1c1-registrant 's1c1-registrant:17' via 35451 (stress-293127e5ec95479495a4a5d67172e649): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v20 Update by s1c1-registrant 's1c1-registrant:18' via 35451 (stress-d2d4beccafd742d4a05e6994afd92a42): record missing

server 35451:
- missing record 76e4949d-94a2-4229-acde-fce7a4246ed4: missing (165 acknowledged changes)
- lost update 77b071a6-139b-4280-beaf-404b868c4a4c v8 Comment by s1c2-reviewer 's1c2-reviewer:9' via 35451 (stress-ac297500208b4abb9bd370fbb922aa6a): version 8 holds Update by s2c1-registrant
- lost update 77b071a6-139b-4280-beaf-404b868c4a4c v11 Update by s2c1-registrant 's2c1-registrant:9' via 37083 (stress-6164f8abdabe4493b6101a2ad040cfc8): version 11 holds Reject by s1c5-reviewer
- lost update 77b071a6-139b-4280-beaf-404b868c4a4c v12 Update by s2c1-registrant 's2c1-registrant:10' via 37083 (stress-cfeeb7d6fa23427db8a22921ef872c51): version 12 holds Comment by s1c5-reviewer
- lost update 77b071a6-139b-4280-beaf-404b868c4a4c v13 Update by s2c1-registrant 's2c1-registrant:11' via 37083 (stress-c23df2f3461241a7955b8ff77f858dd4): version 13 holds Comment by s1c7-reviewer
- lost update 77b071a6-139b-4280-beaf-404b868c4a4c v14 Update by s2c1-registrant 's2c1-registrant:12' via 37083 (stress-046175fa68f6463d89fff369150b37c0): version 14 holds Comment by s1c5-reviewer
- lost update 77b071a6-139b-4280-beaf-404b868c4a4c v15 Update by s2c1-registrant 's2c1-registrant:13' via 37083 (stress-b194a82fbf7f40fda900a6ce4eb5228f): version 15 holds Comment by s1c2-reviewer
- lost update 77b071a6-139b-4280-beaf-404b868c4a4c v16 Update by s2c1-registrant 's2c1-registrant:14' via 37083 (stress-4926236917854b7c9cfb4d8a84619af4): version 16 holds Comment by s1c7-reviewer
- lost update 77b071a6-139b-4280-beaf-404b868c4a4c v17 Update by s2c1-registrant 's2c1-registrant:15' via 37083 (stress-278c0614ce5f4c81bb901e8ad94e9796): version 17 holds Comment by s1c4-admin
- lost update 77b071a6-139b-4280-beaf-404b868c4a4c v18 Update by s2c1-registrant 's2c1-registrant:16' via 37083 (stress-2fb76dc76ed645f2a1c91229c8b558eb): version 18 holds Comment by s1c5-reviewer
- lost update 77b071a6-139b-4280-beaf-404b868c4a4c v19 Update by s2c1-registrant 's2c1-registrant:17' via 37083 (stress-712c3a3ab20c4fe59b5e76d40425c169): version 19 holds Comment by s1c5-reviewer
- lost update 77b071a6-139b-4280-beaf-404b868c4a4c v20 Update by s2c1-registrant 's2c1-registrant:18' via 37083 (stress-42db4605c2d14c1e8712c4d50955b678): version 20 holds Comment by s1c7-reviewer
- lost update 77b071a6-139b-4280-beaf-404b868c4a4c v21 Update by s2c1-registrant 's2c1-registrant:19' via 37083 (stress-947d457566094a8c866c2793e1c4d203): version 21 holds Comment by s1c5-reviewer
- lost update 77b071a6-139b-4280-beaf-404b868c4a4c v22 Update by s2c1-registrant 's2c1-registrant:20' via 37083 (stress-f6c503b14c2c4a6fb9c14a970aa9270a): version 22 holds Comment by s1c7-reviewer
- lost update 77b071a6-139b-4280-beaf-404b868c4a4c v23 Update by s2c1-registrant 's2c1-registrant:21' via 37083 (stress-6f5ef552046e4893a2a06451fe1304b8): version 23 holds Comment by s1c2-reviewer
- lost update 77b071a6-139b-4280-beaf-404b868c4a4c v24 Update by s2c1-registrant 's2c1-registrant:22' via 37083 (stress-3a7880296af4452b8440a8e25665a34d): version 24 holds Comment by s1c2-reviewer
- lost update 77b071a6-139b-4280-beaf-404b868c4a4c v25 Update by s2c1-registrant 's2c1-registrant:23' via 37083 (stress-fbffb6e0a5eb43a5a1a65d6001c4f538): version 25 holds Comment by s1c4-admin
- lost update 77b071a6-139b-4280-beaf-404b868c4a4c v26 Update by s2c1-registrant 's2c1-registrant:24' via 37083 (stress-fcf66b071ad242dd80a04203b8727d62): version 26 holds Comment by s1c5-reviewer
- lost update 77b071a6-139b-4280-beaf-404b868c4a4c v27 Update by s2c1-registrant 's2c1-registrant:25' via 37083 (stress-a603cfcba8d14514bb7ef113ec8c7bb1): version 27 holds Comment by s1c2-reviewer
- lost update 77b071a6-139b-4280-beaf-404b868c4a4c v28 Update by s2c1-registrant 's2c1-registrant:26' via 37083 (stress-473acaad5f004e328fb0d116e6b5336f): version 28 holds Comment by s1c7-reviewer
- lost update 77b071a6-139b-4280-beaf-404b868c4a4c v29 Update by s2c1-registrant 's2c1-registrant:27' via 37083 (stress-7b8b8fb9ecfb4ec8a28a66cab8c369d0): version 29 holds Comment by s1c5-reviewer

server 37083:
- missing record 0fe38969-ca08-4c97-9dad-afaa938fb61c: missing (195 acknowledged changes)
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v1 Create by s1c1-registrant via 35451 (stress-c754acf791af4cff8b78f571d26af42d): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v2 Submit by s1c1-registrant via 35451 (stress-182908025fa24d8fadc703931c496d75): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v3 Update by s1c1-registrant 's1c1-registrant:1' via 35451 (stress-a19a461612454a70b7ff158d2b878c2c): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v4 Update by s1c1-registrant 's1c1-registrant:2' via 35451 (stress-a69df3df4f8c4bad9698ee1337619558): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v5 Update by s1c1-registrant 's1c1-registrant:3' via 35451 (stress-3f14b405f5ab4158b639e54886286b38): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v6 Update by s1c1-registrant 's1c1-registrant:4' via 35451 (stress-a783f5e18e88410e973aff8ce367e8b1): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v7 Update by s1c1-registrant 's1c1-registrant:5' via 35451 (stress-c0efe686f5364f6790c7a6d337e4bf5b): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v8 Update by s1c1-registrant 's1c1-registrant:6' via 35451 (stress-caf1de49545d417e943d17b3824434e1): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v9 Update by s1c1-registrant 's1c1-registrant:7' via 35451 (stress-40b58a03cb3e4a918b223e7c8fc1eff1): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v10 Update by s1c1-registrant 's1c1-registrant:8' via 35451 (stress-4ab35ac15f074185b20e5448b7b19dac): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v11 Update by s1c1-registrant 's1c1-registrant:9' via 35451 (stress-b99cb11480f247fab81ea06f617a940a): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v12 Update by s1c1-registrant 's1c1-registrant:10' via 35451 (stress-444b867c7837493183c6292c4a9749d7): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v13 Update by s1c1-registrant 's1c1-registrant:11' via 35451 (stress-d012ac539a8e4ad193ae24e2ea202a79): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v14 Update by s1c1-registrant 's1c1-registrant:12' via 35451 (stress-f3cd7bbbb2d34455b1754479de94ce9a): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v15 Update by s1c1-registrant 's1c1-registrant:13' via 35451 (stress-fc078465e5074aa985c61e0818c4d63c): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v16 Update by s1c1-registrant 's1c1-registrant:14' via 35451 (stress-d3459da07e7f4c3dae0e51b5b22f968b): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v17 Update by s1c1-registrant 's1c1-registrant:15' via 35451 (stress-a6e3c85f25504bf08b0511e9a75e8b81): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v18 Update by s1c1-registrant 's1c1-registrant:16' via 35451 (stress-631a2db83d784cb1a15ac11fde1681cf): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v19 Update by s1c1-registrant 's1c1-registrant:17' via 35451 (stress-293127e5ec95479495a4a5d67172e649): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v20 Update by s1c1-registrant 's1c1-registrant:18' via 35451 (stress-d2d4beccafd742d4a05e6994afd92a42): record missing

server mac-host.local:45359:
- missing record 0fe38969-ca08-4c97-9dad-afaa938fb61c: missing (195 acknowledged changes)
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v1 Create by s1c1-registrant via 35451 (stress-c754acf791af4cff8b78f571d26af42d): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v2 Submit by s1c1-registrant via 35451 (stress-182908025fa24d8fadc703931c496d75): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v3 Update by s1c1-registrant 's1c1-registrant:1' via 35451 (stress-a19a461612454a70b7ff158d2b878c2c): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v4 Update by s1c1-registrant 's1c1-registrant:2' via 35451 (stress-a69df3df4f8c4bad9698ee1337619558): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v5 Update by s1c1-registrant 's1c1-registrant:3' via 35451 (stress-3f14b405f5ab4158b639e54886286b38): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v6 Update by s1c1-registrant 's1c1-registrant:4' via 35451 (stress-a783f5e18e88410e973aff8ce367e8b1): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v7 Update by s1c1-registrant 's1c1-registrant:5' via 35451 (stress-c0efe686f5364f6790c7a6d337e4bf5b): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v8 Update by s1c1-registrant 's1c1-registrant:6' via 35451 (stress-caf1de49545d417e943d17b3824434e1): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v9 Update by s1c1-registrant 's1c1-registrant:7' via 35451 (stress-40b58a03cb3e4a918b223e7c8fc1eff1): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v10 Update by s1c1-registrant 's1c1-registrant:8' via 35451 (stress-4ab35ac15f074185b20e5448b7b19dac): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v11 Update by s1c1-registrant 's1c1-registrant:9' via 35451 (stress-b99cb11480f247fab81ea06f617a940a): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v12 Update by s1c1-registrant 's1c1-registrant:10' via 35451 (stress-444b867c7837493183c6292c4a9749d7): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v13 Update by s1c1-registrant 's1c1-registrant:11' via 35451 (stress-d012ac539a8e4ad193ae24e2ea202a79): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v14 Update by s1c1-registrant 's1c1-registrant:12' via 35451 (stress-f3cd7bbbb2d34455b1754479de94ce9a): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v15 Update by s1c1-registrant 's1c1-registrant:13' via 35451 (stress-fc078465e5074aa985c61e0818c4d63c): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v16 Update by s1c1-registrant 's1c1-registrant:14' via 35451 (stress-d3459da07e7f4c3dae0e51b5b22f968b): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v17 Update by s1c1-registrant 's1c1-registrant:15' via 35451 (stress-a6e3c85f25504bf08b0511e9a75e8b81): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v18 Update by s1c1-registrant 's1c1-registrant:16' via 35451 (stress-631a2db83d784cb1a15ac11fde1681cf): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v19 Update by s1c1-registrant 's1c1-registrant:17' via 35451 (stress-293127e5ec95479495a4a5d67172e649): record missing
- lost update 0fe38969-ca08-4c97-9dad-afaa938fb61c v20 Update by s1c1-registrant 's1c1-registrant:18' via 35451 (stress-d2d4beccafd742d4a05e6994afd92a42): record missing

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| AppendLog | 3 | 2219 | 40.5 | 870 | 1524 | 1476 | 0 | 0 | 515.2 | no |
