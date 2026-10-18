# Stress run: Sqlite, 2 server(s): FAILED

- Commit `c71df76c37c18ca38a44e27c02961e268fbb1c09`, 2026-10-18 12:16:20 UTC, 1.3 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 5 clients, 6 records, 40 changes per client, seed 3
- Clients by role: 4 Registrant, 4 Reviewer, 2 Admin
- Data folder `stress-runs/win-w2-light-Sqlite-3/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-w2-light-Sqlite-3`
- 1 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-w2-light-Sqlite-3` there
- 1 of the servers on `Administrator@ws2.example`, data folder `\\ws1.example\share\runs\win-w2-light-Sqlite-3` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 210 | 127 | 98.2 | 64 | 16 | 0 | 11.7 | 122.7 | 170.3 |

Writes by outcome: conflict 64, notFound 3, saved 127, wrongStatus 16. Saves by action: Create 6, Update 54, Submit 5, Approve 2, Reject 1, Comment 59. Reads: 400 (notFound 196).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | FAIL | 3/6 | 3 | 44 | 0 | 0 | 0 |
| server ws1.example:36651 | FAIL | 3/6 | 3 | 83 | 0 | 0 | 0 |
| server ws2.example:43667 | FAIL | 3/6 | 3 | 44 | 0 | 0 | 0 |

storage:
- missing record 4e3e46eb-a2e5-410f-a04d-230274982088: missing (1 acknowledged changes)
- missing record fdd4bb26-af69-426c-b54e-f420500922c9: missing (1 acknowledged changes)
- missing record dad45341-b178-4879-86f5-86ea391d1d24: missing (1 acknowledged changes)
- lost update 4e3e46eb-a2e5-410f-a04d-230274982088 v1 Create by s1c1-registrant via ws1.example:36651 (stress-c2a605f63fd34eabb69f9d20b9ba63cd): record missing
- lost update fdd4bb26-af69-426c-b54e-f420500922c9 v1 Create by s1c1-registrant via ws1.example:36651 (stress-5dffeb3897b3431384babdd691aee9e6): record missing
- lost update 5fe79d88-dffd-4d99-b37b-da585b3ea31b v2 Comment by s1c2-reviewer 's1c2-reviewer:1' via ws1.example:36651 (stress-e1babdb6fda947eaaca4197201c48428): version 2 holds Comment by s2c4-admin
- lost update 5fe79d88-dffd-4d99-b37b-da585b3ea31b v3 Comment by s1c2-reviewer 's1c2-reviewer:3' via ws1.example:36651 (stress-97d495fdf7e144999df30e508c91e0ee): version 3 holds Submit by s2c1-registrant
- lost update 5fe79d88-dffd-4d99-b37b-da585b3ea31b v6 Comment by s1c2-reviewer 's1c2-reviewer:11' via ws1.example:36651 (stress-f80a9fd19e6141b1a9a1421d9bbf7c9a): version 6 holds Update by s2c1-registrant
- lost update 5fe79d88-dffd-4d99-b37b-da585b3ea31b v4 Update by s1c4-admin 's1c4-admin:3' via ws1.example:36651 (stress-60fb07829cad4fcb9fbb03105c55c365): version 4 holds Update by s2c1-registrant
- lost update 5fe79d88-dffd-4d99-b37b-da585b3ea31b v7 Comment by s1c4-admin 's1c4-admin:11' via ws1.example:36651 (stress-075da9d12da44f23ae9fbfc9b0e4b998): version 7 holds Reject by s2c5-reviewer
- lost update 5fe79d88-dffd-4d99-b37b-da585b3ea31b v9 Update by s1c4-admin 's1c4-admin:19' via ws1.example:36651 (stress-45024fe846e9426b8f5c4824cc18aecd): version 9 holds Comment by s2c4-admin
- lost update 5fe79d88-dffd-4d99-b37b-da585b3ea31b v10 Submit by s1c4-admin via ws1.example:36651 (stress-75ad7b5e90b840c3b49341aab3fe7bff): version 10 holds Comment by s2c4-admin
- lost update 5fe79d88-dffd-4d99-b37b-da585b3ea31b v11 Comment by s1c4-admin 's1c4-admin:24' via ws1.example:36651 (stress-3f73f6eb8b194328980c3060f45817ba): version 11 holds Comment by s2c2-reviewer
- lost update 5fe79d88-dffd-4d99-b37b-da585b3ea31b v12 Comment by s1c4-admin 's1c4-admin:27' via ws1.example:36651 (stress-7d832e5491ee4856b0e50351405997be): version 12 holds Comment by s2c2-reviewer
- lost update 5fe79d88-dffd-4d99-b37b-da585b3ea31b v13 Comment by s1c4-admin 's1c4-admin:28' via ws1.example:36651 (stress-11db369ad9d343958e2564b2a4f45c62): version 13 holds Comment by s2c5-reviewer
- lost update 5fe79d88-dffd-4d99-b37b-da585b3ea31b v14 Approve by s1c4-admin via ws1.example:36651 (stress-2ed8ce4bf57549c9b77f040b296fda81): version 14 holds Comment by s2c4-admin
- lost update 5fe79d88-dffd-4d99-b37b-da585b3ea31b v5 Comment by s1c5-reviewer 's1c5-reviewer:4' via ws1.example:36651 (stress-40807f2bd3ef45c3a0190c66eb4772dd): version 5 holds Update by s2c1-registrant
- lost update 5fe79d88-dffd-4d99-b37b-da585b3ea31b v8 Comment by s1c5-reviewer 's1c5-reviewer:12' via ws1.example:36651 (stress-bd44caabecf14e648be6775dd6ab81be): version 8 holds Comment by s2c5-reviewer
- lost update 5fe79d88-dffd-4d99-b37b-da585b3ea31b v15 Comment by s1c5-reviewer 's1c5-reviewer:30' via ws1.example:36651 (stress-3169d86bc3784256b25fbc3fbe89a40e): version 15 holds Comment by s2c4-admin
- lost update 5fe79d88-dffd-4d99-b37b-da585b3ea31b v16 Comment by s1c5-reviewer 's1c5-reviewer:38' via ws1.example:36651 (stress-9807d5536586406294b344bd69203f0c): version 16 holds Comment by s2c4-admin
- lost update 5fe79d88-dffd-4d99-b37b-da585b3ea31b v17 Comment by s1c5-reviewer 's1c5-reviewer:39' via ws1.example:36651 (stress-6a1c92128edb4e8584657a03436dd35a): version 17 holds Comment by s2c5-reviewer
- lost update 96dffd40-adaa-4c6b-8d9b-b0e315ffa93f v3 Comment by s1c2-reviewer 's1c2-reviewer:5' via ws1.example:36651 (stress-5167d46558824446bd5b13a92cb00baf): version 3 holds Comment by s2c5-reviewer
- lost update 96dffd40-adaa-4c6b-8d9b-b0e315ffa93f v4 Comment by s1c2-reviewer 's1c2-reviewer:7' via ws1.example:36651 (stress-c3c78c6dbc7f4c7babf621e5759ed2f8): version 4 holds Comment by s2c2-reviewer

server ws1.example:36651:
- missing record 4e3e46eb-a2e5-410f-a04d-230274982088: missing (1 acknowledged changes)
- missing record fdd4bb26-af69-426c-b54e-f420500922c9: missing (1 acknowledged changes)
- missing record dad45341-b178-4879-86f5-86ea391d1d24: missing (1 acknowledged changes)
- lost update 4e3e46eb-a2e5-410f-a04d-230274982088 v1 Create by s1c1-registrant via ws1.example:36651 (stress-c2a605f63fd34eabb69f9d20b9ba63cd): record missing
- lost update fdd4bb26-af69-426c-b54e-f420500922c9 v1 Create by s1c1-registrant via ws1.example:36651 (stress-5dffeb3897b3431384babdd691aee9e6): record missing
- lost update 5fe79d88-dffd-4d99-b37b-da585b3ea31b v3 Submit by s2c1-registrant via ws2.example:43667 (stress-761a56399c65440d89bc94d3c79cab27): version 3 holds Comment by s1c2-reviewer
- lost update 5fe79d88-dffd-4d99-b37b-da585b3ea31b v4 Update by s2c1-registrant 's2c1-registrant:3' via ws2.example:43667 (stress-9636243ee4a443a68192ebb15147c314): version 4 holds Update by s1c4-admin
- lost update 5fe79d88-dffd-4d99-b37b-da585b3ea31b v5 Update by s2c1-registrant 's2c1-registrant:4' via ws2.example:43667 (stress-93a6aabef7b44b01bbe0d71f14ba9ba6): version 5 holds Comment by s1c5-reviewer
- lost update 5fe79d88-dffd-4d99-b37b-da585b3ea31b v6 Update by s2c1-registrant 's2c1-registrant:5' via ws2.example:43667 (stress-d2460a196cfd4893801c067c81c9a8fa): version 6 holds Comment by s1c2-reviewer
- lost update 5fe79d88-dffd-4d99-b37b-da585b3ea31b v11 Comment by s2c2-reviewer 's2c2-reviewer:20' via ws2.example:43667 (stress-f070e833f43c43029af57299e37f455e): version 11 holds Comment by s1c4-admin
- lost update 5fe79d88-dffd-4d99-b37b-da585b3ea31b v12 Comment by s2c2-reviewer 's2c2-reviewer:22' via ws2.example:43667 (stress-9c197b173a874e4a9c97ca73da281634): version 12 holds Comment by s1c4-admin
- lost update 5fe79d88-dffd-4d99-b37b-da585b3ea31b v2 Comment by s2c4-admin 's2c4-admin:0' via ws2.example:43667 (stress-50d66cef25ba4f499654d9467a11e12d): version 2 holds Comment by s1c2-reviewer
- lost update 5fe79d88-dffd-4d99-b37b-da585b3ea31b v9 Comment by s2c4-admin 's2c4-admin:13' via ws2.example:43667 (stress-b1b8c6b7eeb84cb6aff2234a8462b195): version 9 holds Update by s1c4-admin
- lost update 5fe79d88-dffd-4d99-b37b-da585b3ea31b v10 Comment by s2c4-admin 's2c4-admin:14' via ws2.example:43667 (stress-ecbaeaf222c448eda384937ba47d8288): version 10 holds Submit by s1c4-admin
- lost update 5fe79d88-dffd-4d99-b37b-da585b3ea31b v14 Comment by s2c4-admin 's2c4-admin:26' via ws2.example:43667 (stress-be2ad950b4f74108bf7a7aeb706635fa): version 14 holds Approve by s1c4-admin
- lost update 5fe79d88-dffd-4d99-b37b-da585b3ea31b v15 Comment by s2c4-admin 's2c4-admin:28' via ws2.example:43667 (stress-21d34536689549e99fdaef82e98bf607): version 15 holds Comment by s1c5-reviewer
- lost update 5fe79d88-dffd-4d99-b37b-da585b3ea31b v16 Comment by s2c4-admin 's2c4-admin:29' via ws2.example:43667 (stress-4cd1bd7d2b454aacbca4cced69a14df8): version 16 holds Comment by s1c5-reviewer
- lost update 5fe79d88-dffd-4d99-b37b-da585b3ea31b v7 Reject by s2c5-reviewer via ws2.example:43667 (stress-d96b121add8d4481b09e6b9864347662): version 7 holds Comment by s1c4-admin
- lost update 5fe79d88-dffd-4d99-b37b-da585b3ea31b v8 Comment by s2c5-reviewer 's2c5-reviewer:8' via ws2.example:43667 (stress-258a8a2d34dd4bb9bb4f34363b33e06f): version 8 holds Comment by s1c5-reviewer
- lost update 5fe79d88-dffd-4d99-b37b-da585b3ea31b v13 Comment by s2c5-reviewer 's2c5-reviewer:26' via ws2.example:43667 (stress-68b7bb4795db4b879a517985d39bff24): version 13 holds Comment by s1c4-admin
- lost update 5fe79d88-dffd-4d99-b37b-da585b3ea31b v17 Comment by s2c5-reviewer 's2c5-reviewer:35' via ws2.example:43667 (stress-2e492159969440c1bc295900302b9f85): version 17 holds Comment by s1c5-reviewer
- lost update 5fe79d88-dffd-4d99-b37b-da585b3ea31b v18 Comment by s2c5-reviewer 's2c5-reviewer:37' via ws2.example:43667 (stress-05c2a0f50bbf4ad0ad8c6ecaad30a3fd): no history entry at that version
- lost update 96dffd40-adaa-4c6b-8d9b-b0e315ffa93f v2 Update by s2c1-registrant 's2c1-registrant:0' via ws2.example:43667 (stress-ad4e929be4924865acb534cdff5b06c6): version 2 holds Comment by s1c4-admin

server ws2.example:43667:
- missing record 4e3e46eb-a2e5-410f-a04d-230274982088: missing (1 acknowledged changes)
- missing record fdd4bb26-af69-426c-b54e-f420500922c9: missing (1 acknowledged changes)
- missing record dad45341-b178-4879-86f5-86ea391d1d24: missing (1 acknowledged changes)
- lost update 4e3e46eb-a2e5-410f-a04d-230274982088 v1 Create by s1c1-registrant via ws1.example:36651 (stress-c2a605f63fd34eabb69f9d20b9ba63cd): record missing
- lost update fdd4bb26-af69-426c-b54e-f420500922c9 v1 Create by s1c1-registrant via ws1.example:36651 (stress-5dffeb3897b3431384babdd691aee9e6): record missing
- lost update 5fe79d88-dffd-4d99-b37b-da585b3ea31b v2 Comment by s1c2-reviewer 's1c2-reviewer:1' via ws1.example:36651 (stress-e1babdb6fda947eaaca4197201c48428): version 2 holds Comment by s2c4-admin
- lost update 5fe79d88-dffd-4d99-b37b-da585b3ea31b v3 Comment by s1c2-reviewer 's1c2-reviewer:3' via ws1.example:36651 (stress-97d495fdf7e144999df30e508c91e0ee): version 3 holds Submit by s2c1-registrant
- lost update 5fe79d88-dffd-4d99-b37b-da585b3ea31b v6 Comment by s1c2-reviewer 's1c2-reviewer:11' via ws1.example:36651 (stress-f80a9fd19e6141b1a9a1421d9bbf7c9a): version 6 holds Update by s2c1-registrant
- lost update 5fe79d88-dffd-4d99-b37b-da585b3ea31b v4 Update by s1c4-admin 's1c4-admin:3' via ws1.example:36651 (stress-60fb07829cad4fcb9fbb03105c55c365): version 4 holds Update by s2c1-registrant
- lost update 5fe79d88-dffd-4d99-b37b-da585b3ea31b v7 Comment by s1c4-admin 's1c4-admin:11' via ws1.example:36651 (stress-075da9d12da44f23ae9fbfc9b0e4b998): version 7 holds Reject by s2c5-reviewer
- lost update 5fe79d88-dffd-4d99-b37b-da585b3ea31b v9 Update by s1c4-admin 's1c4-admin:19' via ws1.example:36651 (stress-45024fe846e9426b8f5c4824cc18aecd): version 9 holds Comment by s2c4-admin
- lost update 5fe79d88-dffd-4d99-b37b-da585b3ea31b v10 Submit by s1c4-admin via ws1.example:36651 (stress-75ad7b5e90b840c3b49341aab3fe7bff): version 10 holds Comment by s2c4-admin
- lost update 5fe79d88-dffd-4d99-b37b-da585b3ea31b v11 Comment by s1c4-admin 's1c4-admin:24' via ws1.example:36651 (stress-3f73f6eb8b194328980c3060f45817ba): version 11 holds Comment by s2c2-reviewer
- lost update 5fe79d88-dffd-4d99-b37b-da585b3ea31b v12 Comment by s1c4-admin 's1c4-admin:27' via ws1.example:36651 (stress-7d832e5491ee4856b0e50351405997be): version 12 holds Comment by s2c2-reviewer
- lost update 5fe79d88-dffd-4d99-b37b-da585b3ea31b v13 Comment by s1c4-admin 's1c4-admin:28' via ws1.example:36651 (stress-11db369ad9d343958e2564b2a4f45c62): version 13 holds Comment by s2c5-reviewer
- lost update 5fe79d88-dffd-4d99-b37b-da585b3ea31b v14 Approve by s1c4-admin via ws1.example:36651 (stress-2ed8ce4bf57549c9b77f040b296fda81): version 14 holds Comment by s2c4-admin
- lost update 5fe79d88-dffd-4d99-b37b-da585b3ea31b v5 Comment by s1c5-reviewer 's1c5-reviewer:4' via ws1.example:36651 (stress-40807f2bd3ef45c3a0190c66eb4772dd): version 5 holds Update by s2c1-registrant
- lost update 5fe79d88-dffd-4d99-b37b-da585b3ea31b v8 Comment by s1c5-reviewer 's1c5-reviewer:12' via ws1.example:36651 (stress-bd44caabecf14e648be6775dd6ab81be): version 8 holds Comment by s2c5-reviewer
- lost update 5fe79d88-dffd-4d99-b37b-da585b3ea31b v15 Comment by s1c5-reviewer 's1c5-reviewer:30' via ws1.example:36651 (stress-3169d86bc3784256b25fbc3fbe89a40e): version 15 holds Comment by s2c4-admin
- lost update 5fe79d88-dffd-4d99-b37b-da585b3ea31b v16 Comment by s1c5-reviewer 's1c5-reviewer:38' via ws1.example:36651 (stress-9807d5536586406294b344bd69203f0c): version 16 holds Comment by s2c4-admin
- lost update 5fe79d88-dffd-4d99-b37b-da585b3ea31b v17 Comment by s1c5-reviewer 's1c5-reviewer:39' via ws1.example:36651 (stress-6a1c92128edb4e8584657a03436dd35a): version 17 holds Comment by s2c5-reviewer
- lost update 96dffd40-adaa-4c6b-8d9b-b0e315ffa93f v3 Comment by s1c2-reviewer 's1c2-reviewer:5' via ws1.example:36651 (stress-5167d46558824446bd5b13a92cb00baf): version 3 holds Comment by s2c5-reviewer
- lost update 96dffd40-adaa-4c6b-8d9b-b0e315ffa93f v4 Comment by s1c2-reviewer 's1c2-reviewer:7' via ws1.example:36651 (stress-c3c78c6dbc7f4c7babf621e5759ed2f8): version 4 holds Comment by s2c2-reviewer

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| Sqlite | 2 | 127 | 98.2 | 64 | 16 | 83 | 0 | 0 | 122.7 | no |
