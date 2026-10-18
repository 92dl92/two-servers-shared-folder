# Stress run: NaiveFile, 2 server(s): FAILED

- Commit `c71df76c37c18ca38a44e27c02961e268fbb1c09`, 2026-10-18 12:19:10 UTC, 9.7 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 12 clients, 12 records, 200 changes per client, seed 3
- Clients by role: 10 Registrant, 10 Reviewer, 4 Admin
- Data folder `stress-runs/win-w2-heavy-NaiveFile-3/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-w2-heavy-NaiveFile-3`
- 1 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-w2-heavy-NaiveFile-3` there
- 1 of the servers on `Administrator@ws2.example`, data folder `\\ws1.example\share\runs\win-w2-heavy-NaiveFile-3` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 1816 | 160 | 16.5 | 166 | 407 | 0 | 30.6 | 152.2 | 340.0 |

Writes by outcome: conflict 166, http500 1083, saved 160, wrongStatus 407. Saves by action: Create 6, Update 12, Submit 4, Approve 3, Comment 135. Reads: 3800 (http500 140, notFound 1856).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | FAIL | 3/6 | 3 | 10 | 0 | 0 | 0 |
| server ws1.example:41187 | FAIL | 3/6 | 3 | 10 | 0 | 0 | 0 |
| server ws2.example:35465 | FAIL | 3/6 | 3 | 10 | 0 | 0 | 0 |

storage:
- missing record 26bd4cec-8622-4a2d-a8ad-68a80da797cd: missing (1 acknowledged changes)
- missing record 46da0d3b-312b-472e-9c51-2189b9ef3af6: missing (1 acknowledged changes)
- missing record 1a58c2d2-ba05-495f-9a2c-fc5417fafde3: missing (1 acknowledged changes)
- lost update 26bd4cec-8622-4a2d-a8ad-68a80da797cd v1 Create by s1c1-registrant via ws1.example:41187 (stress-9fa47b75cb4f4ff093bbffe3c2a000f7): record missing
- lost update ab4747e3-d061-4815-84c4-5fc70e0b41b0 v18 Comment by s1c4-admin 's1c4-admin:128' via ws1.example:41187 (stress-5928ca6b79524ebb9475ee51a3fc1bb2): version 18 holds Comment by s1c10-reviewer
- lost update ab4747e3-d061-4815-84c4-5fc70e0b41b0 v4 Update by s1c9-admin 's1c9-admin:17' via ws1.example:41187 (stress-ac7c7b1c25d04327a6d1a957de1d2581): version 4 holds Update by s1c1-registrant
- lost update a1a1024e-7386-494b-9d6f-896d499f3016 v3 Comment by s1c2-reviewer 's1c2-reviewer:5' via ws1.example:41187 (stress-6769c58192b843329dd95914e10eea5a): version 3 holds Comment by s1c12-reviewer
- lost update a1a1024e-7386-494b-9d6f-896d499f3016 v16 Comment by s1c5-reviewer 's1c5-reviewer:119' via ws1.example:41187 (stress-c4ad72f8b7d547cfb7e909c3e77f2acb): version 16 holds Comment by s1c2-reviewer
- lost update a1a1024e-7386-494b-9d6f-896d499f3016 v12 Comment by s1c12-reviewer 's1c12-reviewer:90' via ws1.example:41187 (stress-e947dd26b3114b97b32444fd86a5d611): version 12 holds Comment by s1c9-admin
- lost update 46da0d3b-312b-472e-9c51-2189b9ef3af6 v1 Create by s1c3-registrant via ws1.example:41187 (stress-b69a0b66ac454d229b34a621cc7c2c80): record missing
- lost update 1a58c2d2-ba05-495f-9a2c-fc5417fafde3 v1 Create by s1c8-registrant via ws1.example:41187 (stress-75d0c799d7594bbdab96dbfc2f648551): record missing
- lost update e3db112f-300f-48dc-8a6b-4f80401587d1 v12 Submit by s1c4-admin via ws1.example:41187 (stress-d49e219fa3c54bb4882eae02c1f52609): version 12 holds Comment by s2c7-reviewer
- lost update e3db112f-300f-48dc-8a6b-4f80401587d1 v5 Comment by s1c12-reviewer 's1c12-reviewer:44' via ws1.example:41187 (stress-e0fdd500051c45918f80d199602d95d0): version 5 holds Comment by s1c2-reviewer

server ws1.example:41187:
- missing record 26bd4cec-8622-4a2d-a8ad-68a80da797cd: missing (1 acknowledged changes)
- missing record 46da0d3b-312b-472e-9c51-2189b9ef3af6: missing (1 acknowledged changes)
- missing record 1a58c2d2-ba05-495f-9a2c-fc5417fafde3: missing (1 acknowledged changes)
- lost update 26bd4cec-8622-4a2d-a8ad-68a80da797cd v1 Create by s1c1-registrant via ws1.example:41187 (stress-9fa47b75cb4f4ff093bbffe3c2a000f7): record missing
- lost update ab4747e3-d061-4815-84c4-5fc70e0b41b0 v18 Comment by s1c4-admin 's1c4-admin:128' via ws1.example:41187 (stress-5928ca6b79524ebb9475ee51a3fc1bb2): version 18 holds Comment by s1c10-reviewer
- lost update ab4747e3-d061-4815-84c4-5fc70e0b41b0 v4 Update by s1c9-admin 's1c9-admin:17' via ws1.example:41187 (stress-ac7c7b1c25d04327a6d1a957de1d2581): version 4 holds Update by s1c1-registrant
- lost update a1a1024e-7386-494b-9d6f-896d499f3016 v3 Comment by s1c2-reviewer 's1c2-reviewer:5' via ws1.example:41187 (stress-6769c58192b843329dd95914e10eea5a): version 3 holds Comment by s1c12-reviewer
- lost update a1a1024e-7386-494b-9d6f-896d499f3016 v16 Comment by s1c5-reviewer 's1c5-reviewer:119' via ws1.example:41187 (stress-c4ad72f8b7d547cfb7e909c3e77f2acb): version 16 holds Comment by s1c2-reviewer
- lost update a1a1024e-7386-494b-9d6f-896d499f3016 v12 Comment by s1c12-reviewer 's1c12-reviewer:90' via ws1.example:41187 (stress-e947dd26b3114b97b32444fd86a5d611): version 12 holds Comment by s1c9-admin
- lost update 46da0d3b-312b-472e-9c51-2189b9ef3af6 v1 Create by s1c3-registrant via ws1.example:41187 (stress-b69a0b66ac454d229b34a621cc7c2c80): record missing
- lost update 1a58c2d2-ba05-495f-9a2c-fc5417fafde3 v1 Create by s1c8-registrant via ws1.example:41187 (stress-75d0c799d7594bbdab96dbfc2f648551): record missing
- lost update e3db112f-300f-48dc-8a6b-4f80401587d1 v12 Submit by s1c4-admin via ws1.example:41187 (stress-d49e219fa3c54bb4882eae02c1f52609): version 12 holds Comment by s2c7-reviewer
- lost update e3db112f-300f-48dc-8a6b-4f80401587d1 v5 Comment by s1c12-reviewer 's1c12-reviewer:44' via ws1.example:41187 (stress-e0fdd500051c45918f80d199602d95d0): version 5 holds Comment by s1c2-reviewer

server ws2.example:35465:
- missing record 26bd4cec-8622-4a2d-a8ad-68a80da797cd: missing (1 acknowledged changes)
- missing record 46da0d3b-312b-472e-9c51-2189b9ef3af6: missing (1 acknowledged changes)
- missing record 1a58c2d2-ba05-495f-9a2c-fc5417fafde3: missing (1 acknowledged changes)
- lost update 26bd4cec-8622-4a2d-a8ad-68a80da797cd v1 Create by s1c1-registrant via ws1.example:41187 (stress-9fa47b75cb4f4ff093bbffe3c2a000f7): record missing
- lost update ab4747e3-d061-4815-84c4-5fc70e0b41b0 v18 Comment by s1c4-admin 's1c4-admin:128' via ws1.example:41187 (stress-5928ca6b79524ebb9475ee51a3fc1bb2): version 18 holds Comment by s1c10-reviewer
- lost update ab4747e3-d061-4815-84c4-5fc70e0b41b0 v4 Update by s1c9-admin 's1c9-admin:17' via ws1.example:41187 (stress-ac7c7b1c25d04327a6d1a957de1d2581): version 4 holds Update by s1c1-registrant
- lost update a1a1024e-7386-494b-9d6f-896d499f3016 v3 Comment by s1c2-reviewer 's1c2-reviewer:5' via ws1.example:41187 (stress-6769c58192b843329dd95914e10eea5a): version 3 holds Comment by s1c12-reviewer
- lost update a1a1024e-7386-494b-9d6f-896d499f3016 v16 Comment by s1c5-reviewer 's1c5-reviewer:119' via ws1.example:41187 (stress-c4ad72f8b7d547cfb7e909c3e77f2acb): version 16 holds Comment by s1c2-reviewer
- lost update a1a1024e-7386-494b-9d6f-896d499f3016 v12 Comment by s1c12-reviewer 's1c12-reviewer:90' via ws1.example:41187 (stress-e947dd26b3114b97b32444fd86a5d611): version 12 holds Comment by s1c9-admin
- lost update 46da0d3b-312b-472e-9c51-2189b9ef3af6 v1 Create by s1c3-registrant via ws1.example:41187 (stress-b69a0b66ac454d229b34a621cc7c2c80): record missing
- lost update 1a58c2d2-ba05-495f-9a2c-fc5417fafde3 v1 Create by s1c8-registrant via ws1.example:41187 (stress-75d0c799d7594bbdab96dbfc2f648551): record missing
- lost update e3db112f-300f-48dc-8a6b-4f80401587d1 v12 Submit by s1c4-admin via ws1.example:41187 (stress-d49e219fa3c54bb4882eae02c1f52609): version 12 holds Comment by s2c7-reviewer
- lost update e3db112f-300f-48dc-8a6b-4f80401587d1 v5 Comment by s1c12-reviewer 's1c12-reviewer:44' via ws1.example:41187 (stress-e0fdd500051c45918f80d199602d95d0): version 5 holds Comment by s1c2-reviewer

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 2 | 160 | 16.5 | 166 | 407 | 10 | 0 | 0 | 152.2 | no |
