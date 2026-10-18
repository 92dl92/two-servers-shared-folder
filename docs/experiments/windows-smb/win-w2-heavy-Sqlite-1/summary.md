# Stress run: Sqlite, 2 server(s): FAILED

- Commit `c71df76c37c18ca38a44e27c02961e268fbb1c09`, 2026-10-18 12:36:55 UTC, 7.5 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 12 clients, 12 records, 200 changes per client, seed 1
- Clients by role: 10 Registrant, 10 Reviewer, 4 Admin
- Data folder `stress-runs/win-w2-heavy-Sqlite-1/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-w2-heavy-Sqlite-1`
- 1 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-w2-heavy-Sqlite-1` there
- 1 of the servers on `Administrator@ws2.example`, data folder `\\ws1.example\share\runs\win-w2-heavy-Sqlite-1` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 2439 | 813 | 108.5 | 771 | 837 | 0 | 24.8 | 79.8 | 1680.0 |

Writes by outcome: conflict 771, notFound 18, saved 813, wrongStatus 837. Saves by action: Create 12, Update 118, Submit 12, Approve 8, Reject 3, Comment 660. Reads: 4800 (notFound 2373).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | FAIL | 6/12 | 6 | 386 | 0 | 0 | 0 |
| server ws1.example:34087 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server ws2.example:33279 | FAIL | 6/12 | 6 | 386 | 0 | 0 | 0 |

storage:
- missing record 5b2fd6ee-7348-42d4-8174-f3fd90590065: missing (2 acknowledged changes)
- missing record bf8488f3-cfbb-452b-9d0d-a093e9ec3789: missing (1 acknowledged changes)
- missing record 69c6c4ae-d6f9-4ae7-9f38-2e4336e73043: missing (1 acknowledged changes)
- missing record 46defe79-0194-48da-8519-d62e06ecd7f1: missing (1 acknowledged changes)
- missing record a913b934-f3c5-4041-9621-b87c1f976ab5: missing (1 acknowledged changes)
- missing record dbacb622-5f5f-4c64-8bcc-45c1ac87dbbd: missing (1 acknowledged changes)
- lost update 5b2fd6ee-7348-42d4-8174-f3fd90590065 v1 Create by s1c1-registrant via ws1.example:34087 (stress-466fb6eecb61412eb2c2078e4197d529): record missing
- lost update 5b2fd6ee-7348-42d4-8174-f3fd90590065 v2 Submit by s1c1-registrant via ws1.example:34087 (stress-2fcdd8cff69647cdbb550b437b2028d1): record missing
- lost update bf8488f3-cfbb-452b-9d0d-a093e9ec3789 v1 Create by s1c1-registrant via ws1.example:34087 (stress-5fbcc1aef8d6405aa7f2eae41818d45e): record missing
- lost update 2f81f90f-ecc5-4fb3-988b-56b23c8e7a2f v15 Comment by s1c2-reviewer 's1c2-reviewer:36' via ws1.example:34087 (stress-f05791709cc445c0b44b5c7405b15f41): version 15 holds Comment by s2c2-reviewer
- lost update 2f81f90f-ecc5-4fb3-988b-56b23c8e7a2f v33 Comment by s1c2-reviewer 's1c2-reviewer:82' via ws1.example:34087 (stress-ed25a691846642f3914786bc336550fb): version 33 holds Comment by s2c4-admin
- lost update 2f81f90f-ecc5-4fb3-988b-56b23c8e7a2f v37 Comment by s1c2-reviewer 's1c2-reviewer:99' via ws1.example:34087 (stress-9ada515d89e9402a91678e3f15b2cc53): version 37 holds Comment by s2c10-reviewer
- lost update 2f81f90f-ecc5-4fb3-988b-56b23c8e7a2f v42 Comment by s1c2-reviewer 's1c2-reviewer:119' via ws1.example:34087 (stress-05df112a40884d999664db0d891bdffc): version 42 holds Comment by s2c9-admin
- lost update 2f81f90f-ecc5-4fb3-988b-56b23c8e7a2f v43 Comment by s1c2-reviewer 's1c2-reviewer:121' via ws1.example:34087 (stress-ae308b28a9b24b558a3e8c92a67168e9): version 43 holds Comment by s2c4-admin
- lost update 2f81f90f-ecc5-4fb3-988b-56b23c8e7a2f v46 Comment by s1c2-reviewer 's1c2-reviewer:131' via ws1.example:34087 (stress-7a1b9ce37c534f5ea1b5cd1da45e10b1): version 46 holds Comment by s2c10-reviewer
- lost update 2f81f90f-ecc5-4fb3-988b-56b23c8e7a2f v50 Comment by s1c2-reviewer 's1c2-reviewer:139' via ws1.example:34087 (stress-63379e7abd5c41c2a0a035c4cfb79fff): version 50 holds Comment by s2c2-reviewer
- lost update 2f81f90f-ecc5-4fb3-988b-56b23c8e7a2f v63 Comment by s1c2-reviewer 's1c2-reviewer:180' via ws1.example:34087 (stress-e597c73ba613499283d6906b69141c13): version 63 holds Comment by s2c2-reviewer
- lost update 2f81f90f-ecc5-4fb3-988b-56b23c8e7a2f v66 Comment by s1c2-reviewer 's1c2-reviewer:186' via ws1.example:34087 (stress-2bd7a04743ec4794abb30668455d8f0b): no history entry at that version
- lost update 2f81f90f-ecc5-4fb3-988b-56b23c8e7a2f v68 Comment by s1c2-reviewer 's1c2-reviewer:193' via ws1.example:34087 (stress-e180bc9fab86410dbb02bcb9d53274b4): no history entry at that version
- lost update 2f81f90f-ecc5-4fb3-988b-56b23c8e7a2f v2 Update by s1c4-admin 's1c4-admin:4' via ws1.example:34087 (stress-cecd6b2fef864adb9a635a5dec887bad): version 2 holds Comment by s2c10-reviewer
- lost update 2f81f90f-ecc5-4fb3-988b-56b23c8e7a2f v7 Submit by s1c4-admin via ws1.example:34087 (stress-af73e3b9ba30473cb3134fb575534336): version 7 holds Update by s2c1-registrant
- lost update 2f81f90f-ecc5-4fb3-988b-56b23c8e7a2f v19 Comment by s1c4-admin 's1c4-admin:55' via ws1.example:34087 (stress-33a3370036b54263838ab8839f167f58): version 19 holds Update by s2c1-registrant
- lost update 2f81f90f-ecc5-4fb3-988b-56b23c8e7a2f v24 Comment by s1c4-admin 's1c4-admin:68' via ws1.example:34087 (stress-9928d352d1e942a59bb8c3e594a456b9): version 24 holds Comment by s2c10-reviewer
- lost update 2f81f90f-ecc5-4fb3-988b-56b23c8e7a2f v25 Comment by s1c4-admin 's1c4-admin:71' via ws1.example:34087 (stress-55f3f43a1d9441718a8dc5c1d613bba7): version 25 holds Comment by s2c2-reviewer
- lost update 2f81f90f-ecc5-4fb3-988b-56b23c8e7a2f v28 Comment by s1c4-admin 's1c4-admin:77' via ws1.example:34087 (stress-211f74ac41994c499288f292a69b3175): version 28 holds Comment by s2c2-reviewer
- lost update 2f81f90f-ecc5-4fb3-988b-56b23c8e7a2f v35 Comment by s1c4-admin 's1c4-admin:98' via ws1.example:34087 (stress-975bdbb143074a6088489495dd3a9329): version 35 holds Comment by s2c9-admin

server ws1.example:34087:
- unreadable: listing records answered 500

server ws2.example:33279:
- missing record 5b2fd6ee-7348-42d4-8174-f3fd90590065: missing (2 acknowledged changes)
- missing record bf8488f3-cfbb-452b-9d0d-a093e9ec3789: missing (1 acknowledged changes)
- missing record 69c6c4ae-d6f9-4ae7-9f38-2e4336e73043: missing (1 acknowledged changes)
- missing record 46defe79-0194-48da-8519-d62e06ecd7f1: missing (1 acknowledged changes)
- missing record a913b934-f3c5-4041-9621-b87c1f976ab5: missing (1 acknowledged changes)
- missing record dbacb622-5f5f-4c64-8bcc-45c1ac87dbbd: missing (1 acknowledged changes)
- lost update 5b2fd6ee-7348-42d4-8174-f3fd90590065 v1 Create by s1c1-registrant via ws1.example:34087 (stress-466fb6eecb61412eb2c2078e4197d529): record missing
- lost update 5b2fd6ee-7348-42d4-8174-f3fd90590065 v2 Submit by s1c1-registrant via ws1.example:34087 (stress-2fcdd8cff69647cdbb550b437b2028d1): record missing
- lost update bf8488f3-cfbb-452b-9d0d-a093e9ec3789 v1 Create by s1c1-registrant via ws1.example:34087 (stress-5fbcc1aef8d6405aa7f2eae41818d45e): record missing
- lost update 2f81f90f-ecc5-4fb3-988b-56b23c8e7a2f v15 Comment by s1c2-reviewer 's1c2-reviewer:36' via ws1.example:34087 (stress-f05791709cc445c0b44b5c7405b15f41): version 15 holds Comment by s2c2-reviewer
- lost update 2f81f90f-ecc5-4fb3-988b-56b23c8e7a2f v33 Comment by s1c2-reviewer 's1c2-reviewer:82' via ws1.example:34087 (stress-ed25a691846642f3914786bc336550fb): version 33 holds Comment by s2c4-admin
- lost update 2f81f90f-ecc5-4fb3-988b-56b23c8e7a2f v37 Comment by s1c2-reviewer 's1c2-reviewer:99' via ws1.example:34087 (stress-9ada515d89e9402a91678e3f15b2cc53): version 37 holds Comment by s2c10-reviewer
- lost update 2f81f90f-ecc5-4fb3-988b-56b23c8e7a2f v42 Comment by s1c2-reviewer 's1c2-reviewer:119' via ws1.example:34087 (stress-05df112a40884d999664db0d891bdffc): version 42 holds Comment by s2c9-admin
- lost update 2f81f90f-ecc5-4fb3-988b-56b23c8e7a2f v43 Comment by s1c2-reviewer 's1c2-reviewer:121' via ws1.example:34087 (stress-ae308b28a9b24b558a3e8c92a67168e9): version 43 holds Comment by s2c4-admin
- lost update 2f81f90f-ecc5-4fb3-988b-56b23c8e7a2f v46 Comment by s1c2-reviewer 's1c2-reviewer:131' via ws1.example:34087 (stress-7a1b9ce37c534f5ea1b5cd1da45e10b1): version 46 holds Comment by s2c10-reviewer
- lost update 2f81f90f-ecc5-4fb3-988b-56b23c8e7a2f v50 Comment by s1c2-reviewer 's1c2-reviewer:139' via ws1.example:34087 (stress-63379e7abd5c41c2a0a035c4cfb79fff): version 50 holds Comment by s2c2-reviewer
- lost update 2f81f90f-ecc5-4fb3-988b-56b23c8e7a2f v63 Comment by s1c2-reviewer 's1c2-reviewer:180' via ws1.example:34087 (stress-e597c73ba613499283d6906b69141c13): version 63 holds Comment by s2c2-reviewer
- lost update 2f81f90f-ecc5-4fb3-988b-56b23c8e7a2f v66 Comment by s1c2-reviewer 's1c2-reviewer:186' via ws1.example:34087 (stress-2bd7a04743ec4794abb30668455d8f0b): no history entry at that version
- lost update 2f81f90f-ecc5-4fb3-988b-56b23c8e7a2f v68 Comment by s1c2-reviewer 's1c2-reviewer:193' via ws1.example:34087 (stress-e180bc9fab86410dbb02bcb9d53274b4): no history entry at that version
- lost update 2f81f90f-ecc5-4fb3-988b-56b23c8e7a2f v2 Update by s1c4-admin 's1c4-admin:4' via ws1.example:34087 (stress-cecd6b2fef864adb9a635a5dec887bad): version 2 holds Comment by s2c10-reviewer
- lost update 2f81f90f-ecc5-4fb3-988b-56b23c8e7a2f v7 Submit by s1c4-admin via ws1.example:34087 (stress-af73e3b9ba30473cb3134fb575534336): version 7 holds Update by s2c1-registrant
- lost update 2f81f90f-ecc5-4fb3-988b-56b23c8e7a2f v19 Comment by s1c4-admin 's1c4-admin:55' via ws1.example:34087 (stress-33a3370036b54263838ab8839f167f58): version 19 holds Update by s2c1-registrant
- lost update 2f81f90f-ecc5-4fb3-988b-56b23c8e7a2f v24 Comment by s1c4-admin 's1c4-admin:68' via ws1.example:34087 (stress-9928d352d1e942a59bb8c3e594a456b9): version 24 holds Comment by s2c10-reviewer
- lost update 2f81f90f-ecc5-4fb3-988b-56b23c8e7a2f v25 Comment by s1c4-admin 's1c4-admin:71' via ws1.example:34087 (stress-55f3f43a1d9441718a8dc5c1d613bba7): version 25 holds Comment by s2c2-reviewer
- lost update 2f81f90f-ecc5-4fb3-988b-56b23c8e7a2f v28 Comment by s1c4-admin 's1c4-admin:77' via ws1.example:34087 (stress-211f74ac41994c499288f292a69b3175): version 28 holds Comment by s2c2-reviewer
- lost update 2f81f90f-ecc5-4fb3-988b-56b23c8e7a2f v35 Comment by s1c4-admin 's1c4-admin:98' via ws1.example:34087 (stress-975bdbb143074a6088489495dd3a9329): version 35 holds Comment by s2c9-admin

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| Sqlite | 2 | 813 | 108.5 | 771 | 837 | 386 | 1 | 0 | 79.8 | no |
