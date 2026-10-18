# Stress run: NaiveFile, 2 server(s): FAILED

- Commit `c71df76c37c18ca38a44e27c02961e268fbb1c09`, 2026-10-18 12:18:08 UTC, 14.5 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 12 clients, 12 records, 200 changes per client, seed 2
- Clients by role: 10 Registrant, 10 Reviewer, 4 Admin
- Data folder `stress-runs/win-w2-heavy-NaiveFile-2/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-w2-heavy-NaiveFile-2`
- 1 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-w2-heavy-NaiveFile-2` there
- 1 of the servers on `Administrator@ws2.example`, data folder `\\ws1.example\share\runs\win-w2-heavy-NaiveFile-2` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 3487 | 200 | 13.8 | 177 | 847 | 0 | 33.4 | 157.2 | 575.6 |

Writes by outcome: conflict 177, http500 2263, saved 200, wrongStatus 847. Saves by action: Create 5, Update 15, Submit 6, Approve 4, Reject 1, Comment 169. Reads: 3800 (http500 325).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | FAIL | 5/5 | 0 | 11 | 0 | 0 | 0 |
| server ws1.example:39783 | FAIL | 5/5 | 0 | 11 | 0 | 0 | 0 |
| server ws2.example:42907 | FAIL | 5/5 | 0 | 11 | 0 | 0 | 0 |

storage:
- lost update 203fac02-1104-4118-b9ad-03429227d801 v26 Comment by s1c4-admin 's1c4-admin:178' via ws1.example:39783 (stress-5cdd11460c4d4479ad32284c6fbc7039): version 26 holds Comment by s1c7-reviewer
- lost update 203fac02-1104-4118-b9ad-03429227d801 v20 Comment by s1c10-reviewer 's1c10-reviewer:99' via ws1.example:39783 (stress-ea54b95944ca400591ab648c0725c112): version 20 holds Comment by s1c12-reviewer
- lost update 203fac02-1104-4118-b9ad-03429227d801 v36 Comment by s2c7-reviewer 's2c7-reviewer:137' via ws2.example:42907 (stress-ea18ccc014e340769c185e30c3a078dd): version 36 holds Comment by s2c7-reviewer
- lost update 6b87e893-e4e6-4d36-b09b-abc5f7b94c2d v15 Comment by s1c10-reviewer 's1c10-reviewer:157' via ws1.example:39783 (stress-e48076ce96304b2cb558d42d6692a0e5): version 15 holds Comment by s2c2-reviewer
- lost update 6b87e893-e4e6-4d36-b09b-abc5f7b94c2d v13 Comment by s1c12-reviewer 's1c12-reviewer:149' via ws1.example:39783 (stress-5161ae153e414a96ad253257c030e4d5): version 13 holds Comment by s1c4-admin
- lost update 12f0994e-189d-49bf-8de9-631112cb1d0b v7 Comment by s1c10-reviewer 's1c10-reviewer:50' via ws1.example:39783 (stress-db48acc9599d47cb9dce196e12f9534c): version 7 holds Comment by s1c9-admin
- lost update 12f0994e-189d-49bf-8de9-631112cb1d0b v5 Comment by s1c12-reviewer 's1c12-reviewer:17' via ws1.example:39783 (stress-e00bbad07c2c4221939817abc47ada40): version 5 holds Comment by s1c12-reviewer
- lost update 40bbd324-7166-4e78-a9d7-66759fbe3b86 v10 Comment by s1c7-reviewer 's1c7-reviewer:65' via ws1.example:39783 (stress-a61594e7c81646cf8e38dfb2aeafd552): version 10 holds Comment by s1c10-reviewer
- lost update b0ae1d33-98eb-4811-b03e-b5d5bd06f705 v20 Comment by s1c5-reviewer 's1c5-reviewer:195' via ws1.example:39783 (stress-eb5cc4d750d8471190d5d7a748e8fa49): version 20 holds Comment by s2c5-reviewer
- lost update b0ae1d33-98eb-4811-b03e-b5d5bd06f705 v10 Submit by s1c8-registrant via ws1.example:39783 (stress-553402f9174a4c76a95ef1c413ac8363): version 10 holds Submit by s1c8-registrant
- lost update b0ae1d33-98eb-4811-b03e-b5d5bd06f705 v19 Comment by s1c10-reviewer 's1c10-reviewer:155' via ws1.example:39783 (stress-599832bb5f5043d2b54272ca584a7d2e): version 19 holds Comment by s1c10-reviewer

server ws1.example:39783:
- lost update 203fac02-1104-4118-b9ad-03429227d801 v26 Comment by s1c4-admin 's1c4-admin:178' via ws1.example:39783 (stress-5cdd11460c4d4479ad32284c6fbc7039): version 26 holds Comment by s1c7-reviewer
- lost update 203fac02-1104-4118-b9ad-03429227d801 v20 Comment by s1c10-reviewer 's1c10-reviewer:99' via ws1.example:39783 (stress-ea54b95944ca400591ab648c0725c112): version 20 holds Comment by s1c12-reviewer
- lost update 203fac02-1104-4118-b9ad-03429227d801 v36 Comment by s2c7-reviewer 's2c7-reviewer:137' via ws2.example:42907 (stress-ea18ccc014e340769c185e30c3a078dd): version 36 holds Comment by s2c7-reviewer
- lost update 6b87e893-e4e6-4d36-b09b-abc5f7b94c2d v15 Comment by s1c10-reviewer 's1c10-reviewer:157' via ws1.example:39783 (stress-e48076ce96304b2cb558d42d6692a0e5): version 15 holds Comment by s2c2-reviewer
- lost update 6b87e893-e4e6-4d36-b09b-abc5f7b94c2d v13 Comment by s1c12-reviewer 's1c12-reviewer:149' via ws1.example:39783 (stress-5161ae153e414a96ad253257c030e4d5): version 13 holds Comment by s1c4-admin
- lost update 12f0994e-189d-49bf-8de9-631112cb1d0b v7 Comment by s1c10-reviewer 's1c10-reviewer:50' via ws1.example:39783 (stress-db48acc9599d47cb9dce196e12f9534c): version 7 holds Comment by s1c9-admin
- lost update 12f0994e-189d-49bf-8de9-631112cb1d0b v5 Comment by s1c12-reviewer 's1c12-reviewer:17' via ws1.example:39783 (stress-e00bbad07c2c4221939817abc47ada40): version 5 holds Comment by s1c12-reviewer
- lost update 40bbd324-7166-4e78-a9d7-66759fbe3b86 v10 Comment by s1c7-reviewer 's1c7-reviewer:65' via ws1.example:39783 (stress-a61594e7c81646cf8e38dfb2aeafd552): version 10 holds Comment by s1c10-reviewer
- lost update b0ae1d33-98eb-4811-b03e-b5d5bd06f705 v20 Comment by s1c5-reviewer 's1c5-reviewer:195' via ws1.example:39783 (stress-eb5cc4d750d8471190d5d7a748e8fa49): version 20 holds Comment by s2c5-reviewer
- lost update b0ae1d33-98eb-4811-b03e-b5d5bd06f705 v10 Submit by s1c8-registrant via ws1.example:39783 (stress-553402f9174a4c76a95ef1c413ac8363): version 10 holds Submit by s1c8-registrant
- lost update b0ae1d33-98eb-4811-b03e-b5d5bd06f705 v19 Comment by s1c10-reviewer 's1c10-reviewer:155' via ws1.example:39783 (stress-599832bb5f5043d2b54272ca584a7d2e): version 19 holds Comment by s1c10-reviewer

server ws2.example:42907:
- lost update 203fac02-1104-4118-b9ad-03429227d801 v26 Comment by s1c4-admin 's1c4-admin:178' via ws1.example:39783 (stress-5cdd11460c4d4479ad32284c6fbc7039): version 26 holds Comment by s1c7-reviewer
- lost update 203fac02-1104-4118-b9ad-03429227d801 v20 Comment by s1c10-reviewer 's1c10-reviewer:99' via ws1.example:39783 (stress-ea54b95944ca400591ab648c0725c112): version 20 holds Comment by s1c12-reviewer
- lost update 203fac02-1104-4118-b9ad-03429227d801 v36 Comment by s2c7-reviewer 's2c7-reviewer:137' via ws2.example:42907 (stress-ea18ccc014e340769c185e30c3a078dd): version 36 holds Comment by s2c7-reviewer
- lost update 6b87e893-e4e6-4d36-b09b-abc5f7b94c2d v15 Comment by s1c10-reviewer 's1c10-reviewer:157' via ws1.example:39783 (stress-e48076ce96304b2cb558d42d6692a0e5): version 15 holds Comment by s2c2-reviewer
- lost update 6b87e893-e4e6-4d36-b09b-abc5f7b94c2d v13 Comment by s1c12-reviewer 's1c12-reviewer:149' via ws1.example:39783 (stress-5161ae153e414a96ad253257c030e4d5): version 13 holds Comment by s1c4-admin
- lost update 12f0994e-189d-49bf-8de9-631112cb1d0b v7 Comment by s1c10-reviewer 's1c10-reviewer:50' via ws1.example:39783 (stress-db48acc9599d47cb9dce196e12f9534c): version 7 holds Comment by s1c9-admin
- lost update 12f0994e-189d-49bf-8de9-631112cb1d0b v5 Comment by s1c12-reviewer 's1c12-reviewer:17' via ws1.example:39783 (stress-e00bbad07c2c4221939817abc47ada40): version 5 holds Comment by s1c12-reviewer
- lost update 40bbd324-7166-4e78-a9d7-66759fbe3b86 v10 Comment by s1c7-reviewer 's1c7-reviewer:65' via ws1.example:39783 (stress-a61594e7c81646cf8e38dfb2aeafd552): version 10 holds Comment by s1c10-reviewer
- lost update b0ae1d33-98eb-4811-b03e-b5d5bd06f705 v20 Comment by s1c5-reviewer 's1c5-reviewer:195' via ws1.example:39783 (stress-eb5cc4d750d8471190d5d7a748e8fa49): version 20 holds Comment by s2c5-reviewer
- lost update b0ae1d33-98eb-4811-b03e-b5d5bd06f705 v10 Submit by s1c8-registrant via ws1.example:39783 (stress-553402f9174a4c76a95ef1c413ac8363): version 10 holds Submit by s1c8-registrant
- lost update b0ae1d33-98eb-4811-b03e-b5d5bd06f705 v19 Comment by s1c10-reviewer 's1c10-reviewer:155' via ws1.example:39783 (stress-599832bb5f5043d2b54272ca584a7d2e): version 19 holds Comment by s1c10-reviewer

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 2 | 200 | 13.8 | 177 | 847 | 11 | 0 | 0 | 157.2 | no |
