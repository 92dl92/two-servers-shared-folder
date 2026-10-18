# Stress run: Sqlite, 2 server(s): FAILED

- Commit `fe53fea387646cb0ee65f1cccd5a309abb2291d2`, 2026-10-18 13:29:09 UTC, 126.0 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 5 clients, pipeline with 1000 ms think time, 120 changes per client, seed 3
- Clients by role: 4 Registrant, 4 Reviewer, 2 Admin
- Data folder `stress-runs/win-w2-real-Sqlite-3/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-w2-real-Sqlite-3`
- 1 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-w2-real-Sqlite-3` there
- 1 of the servers on `Administrator@ws2.example`, data folder `\\ws1.example\share\runs\win-w2-real-Sqlite-3` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 700 | 620 | 4.9 | 8 | 0 | 0 | 6.7 | 40.4 | 184.1 |

Writes by outcome: conflict 8, http500 72, saved 620. Saves by action: Create 142, Update 123, Submit 138, Approve 99, Reject 39, Comment 79. Reads: 675 (http500 34, notFound 154).

Stale reads: 154 (older than a version already acknowledged); 154 reads followed a save through another server.
- record 6faaf589-69cd-40b9-a3dd-f77418d7c37b through ws2.example:37861: version 0, 127.2 ms after version 2 was acknowledged by ws1.example:33759
- record 6faaf589-69cd-40b9-a3dd-f77418d7c37b through ws2.example:37861: version 0, 56.4 ms after version 3 was acknowledged by ws1.example:33759
- record d058190d-0958-4014-86cb-3487dd1bcdab through ws1.example:33759: version 0, 11.5 ms after version 2 was acknowledged by ws2.example:37861
- record d058190d-0958-4014-86cb-3487dd1bcdab through ws1.example:33759: version 0, 223.3 ms after version 3 was acknowledged by ws2.example:37861
- record 6faaf589-69cd-40b9-a3dd-f77418d7c37b through ws2.example:37861: version 0, 74.7 ms after version 4 was acknowledged by ws1.example:33759
- record acd646f7-fe63-4af5-8709-5f0ef2f149bb through ws2.example:37861: version 0, 19.2 ms after version 4 was acknowledged by ws1.example:33759
- record a41cb461-949a-4371-a36d-e9f4780310c2 through ws2.example:37861: version 0, 439.1 ms after version 3 was acknowledged by ws1.example:33759
- record e65513e8-6968-466d-a695-a9292b181166 through ws1.example:33759: version 0, 383.5 ms after version 4 was acknowledged by ws2.example:37861
- record 3c3cadb7-a48b-4320-aecf-e26e110538fa through ws2.example:37861: version 0, 691.5 ms after version 2 was acknowledged by ws1.example:33759
- record c6ed53ba-de46-4b70-8439-1531ed5c1258 through ws1.example:33759: version 0, 187.1 ms after version 3 was acknowledged by ws2.example:37861
- record d2c31a37-ec4f-4443-8c3e-7862cf7a4749 through ws1.example:33759: version 0, 126.0 ms after version 2 was acknowledged by ws2.example:37861
- record 2408661d-3b18-472a-8339-3c60ad1ed190 through ws2.example:37861: version 0, 58.3 ms after version 3 was acknowledged by ws1.example:33759
- record ec6d4c3e-5ecd-4218-9cc1-13ac2256a54c through ws2.example:37861: version 0, 278.5 ms after version 3 was acknowledged by ws1.example:33759
- record c566c2ae-7048-4979-9ebc-4c3627913b50 through ws2.example:37861: version 0, 221.6 ms after version 2 was acknowledged by ws1.example:33759
- record c566c2ae-7048-4979-9ebc-4c3627913b50 through ws2.example:37861: version 0, 573.6 ms after version 2 was acknowledged by ws1.example:33759
- record c566c2ae-7048-4979-9ebc-4c3627913b50 through ws2.example:37861: version 0, 756.1 ms after version 2 was acknowledged by ws1.example:33759
- record b696391e-61e4-4007-ba8c-9b8c325129b1 through ws1.example:33759: version 0, 24.2 ms after version 2 was acknowledged by ws2.example:37861
- record b696391e-61e4-4007-ba8c-9b8c325129b1 through ws1.example:33759: version 0, 32.5 ms after version 3 was acknowledged by ws2.example:37861
- record b696391e-61e4-4007-ba8c-9b8c325129b1 through ws1.example:33759: version 0, 502.0 ms after version 3 was acknowledged by ws2.example:37861
- record 0f28d8b5-daa7-492f-96bd-d3e1d8b85f14 through ws2.example:37861: version 0, 797.1 ms after version 2 was acknowledged by ws1.example:33759

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | FAIL | 2/142 | 140 | 615 | 0 | 0 | 0 |
| server ws1.example:33759 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server ws2.example:37861 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |

storage:
- missing record b9610a3e-b6ca-41ad-b6de-8e8f17dbabcd: missing (5 acknowledged changes)
- missing record c2a61a9a-820a-4606-958d-d00df9d52d1e: missing (7 acknowledged changes)
- missing record 133b9a9f-12d7-417c-86b4-f06a9f372f5d: missing (3 acknowledged changes)
- missing record 30aef63a-622c-47de-af26-dcc9d86884ac: missing (5 acknowledged changes)
- missing record bbfb76b7-1941-4e92-98e8-074e6f2d37ec: missing (4 acknowledged changes)
- missing record 5b54cbb0-e0d9-4959-aa0e-f13cbade7061: missing (9 acknowledged changes)
- missing record f99aa81e-27c5-403d-bad6-01e1bcc193e4: missing (3 acknowledged changes)
- missing record 733abcbd-0145-4f18-b7a8-71e27713e8b6: missing (5 acknowledged changes)
- missing record 80c66500-8623-4bcb-9b68-2195806bcb7a: missing (4 acknowledged changes)
- missing record 9e27ab95-505e-41c0-80a1-c7a629935b4b: missing (3 acknowledged changes)
- missing record e0408e00-2e46-4756-95bb-c2013cc7c8af: missing (3 acknowledged changes)
- missing record 1a38bc49-3112-4c2e-b9e2-b25315b0b9d2: missing (3 acknowledged changes)
- missing record 90fcb7d7-dde9-44e0-ae6c-b8bc6fc77e1e: missing (5 acknowledged changes)
- missing record a862ee4c-8086-434f-b48d-b7e140f76a24: missing (5 acknowledged changes)
- missing record 3aee12e3-efcd-4771-9add-3e3cee8d28b5: missing (4 acknowledged changes)
- missing record 77a5388f-de32-4fd7-ac65-e642d78aaf13: missing (6 acknowledged changes)
- missing record df47568b-ae9d-40af-835e-b0de40ea609b: missing (4 acknowledged changes)
- missing record f98e20a1-012c-4c39-b8c0-510026e56b09: missing (6 acknowledged changes)
- missing record e115b53d-4064-4936-9a46-1114c0fee666: missing (3 acknowledged changes)
- missing record b62055d8-3bb5-4898-a4be-c4386d814781: missing (6 acknowledged changes)
- lost update b9610a3e-b6ca-41ad-b6de-8e8f17dbabcd v1 Create by s1c1-registrant via ws1.example:33759 (stress-795d205e55324b32aa59940dab91b21c): record missing
- lost update b9610a3e-b6ca-41ad-b6de-8e8f17dbabcd v2 Update by s1c1-registrant 's1c1-registrant:118' via ws1.example:33759 (stress-ba2760f8f90f42c79a19f8ca410fb953): record missing
- lost update b9610a3e-b6ca-41ad-b6de-8e8f17dbabcd v3 Update by s1c1-registrant 's1c1-registrant:119' via ws1.example:33759 (stress-1b2ad0e3ad98407c914b30ad92a422b3): record missing
- lost update b9610a3e-b6ca-41ad-b6de-8e8f17dbabcd v4 Submit by s1c1-registrant via ws1.example:33759 (stress-99243cb54e1640fc8ddb2a1b9025c448): record missing
- lost update b9610a3e-b6ca-41ad-b6de-8e8f17dbabcd v5 Approve by s1c4-admin via ws1.example:33759 (stress-43a9c17bdc814e67891299edd57f31eb): record missing
- lost update c2a61a9a-820a-4606-958d-d00df9d52d1e v5 Comment by s1c2-reviewer 's1c2-reviewer:70' via ws1.example:33759 (stress-4fbf2c31cb734519abb1319481e2d1a1): record missing
- lost update c2a61a9a-820a-4606-958d-d00df9d52d1e v7 Approve by s1c2-reviewer via ws1.example:33759 (stress-e63a41a171064ae6a5c3c4bb17d9807e): record missing
- lost update c2a61a9a-820a-4606-958d-d00df9d52d1e v1 Create by s1c3-registrant via ws1.example:33759 (stress-b8abaf8a7e9d4ea5bc468a6933e0bbe8): record missing
- lost update c2a61a9a-820a-4606-958d-d00df9d52d1e v2 Update by s1c3-registrant 's1c3-registrant:107' via ws1.example:33759 (stress-383e3d30ccbe4ad8b4aeb80967ffbc16): record missing
- lost update c2a61a9a-820a-4606-958d-d00df9d52d1e v3 Update by s1c3-registrant 's1c3-registrant:108' via ws1.example:33759 (stress-0e43b70c8d3447ee969fd9078e2c74d0): record missing
- lost update c2a61a9a-820a-4606-958d-d00df9d52d1e v4 Submit by s1c3-registrant via ws1.example:33759 (stress-2a6b8736acab4ba4abdfd737db68c531): record missing
- lost update c2a61a9a-820a-4606-958d-d00df9d52d1e v6 Comment by s1c4-admin 's1c4-admin:64' via ws1.example:33759 (stress-1e17dade44b74f41b6f2ab04daadc280): record missing
- lost update 133b9a9f-12d7-417c-86b4-f06a9f372f5d v3 Approve by s1c2-reviewer via ws1.example:33759 (stress-db3395b72eaf47c2bf5f0b8eee42667a): record missing
- lost update 133b9a9f-12d7-417c-86b4-f06a9f372f5d v1 Create by s1c3-registrant via ws1.example:33759 (stress-b5a9febacdcb4b3788d33361637f28eb): record missing
- lost update 133b9a9f-12d7-417c-86b4-f06a9f372f5d v2 Submit by s1c3-registrant via ws1.example:33759 (stress-b40ab75757cf49aea262ede95353df6e): record missing
- lost update 30aef63a-622c-47de-af26-dcc9d86884ac v1 Create by s1c1-registrant via ws1.example:33759 (stress-ee192d2907ba44f9bbbeaff218647779): record missing
- lost update 30aef63a-622c-47de-af26-dcc9d86884ac v2 Update by s1c1-registrant 's1c1-registrant:101' via ws1.example:33759 (stress-ec5bb31a102e4d53861c09e220c90022): record missing
- lost update 30aef63a-622c-47de-af26-dcc9d86884ac v3 Update by s1c1-registrant 's1c1-registrant:102' via ws1.example:33759 (stress-dcc44a4aa2b749e8b9040196e68d16d7): record missing
- lost update 30aef63a-622c-47de-af26-dcc9d86884ac v4 Submit by s1c1-registrant via ws1.example:33759 (stress-a46992d2da8b4909b9f6760de7d7316d): record missing
- lost update 30aef63a-622c-47de-af26-dcc9d86884ac v5 Reject by s1c2-reviewer via ws1.example:33759 (stress-ff9b0ca9f6cf4afe844232145001b9ef): record missing

server ws1.example:33759:
- unreadable: listing records answered 500

server ws2.example:37861:
- unreadable: listing records answered 500

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| Sqlite | 2 | 620 | 4.9 | 8 | 0 | 615 | 2 | 0 | 40.4 | no |
