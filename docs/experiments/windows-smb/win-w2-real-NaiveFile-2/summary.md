# Stress run: NaiveFile, 2 server(s): FAILED

- Commit `fe53fea387646cb0ee65f1cccd5a309abb2291d2`, 2026-10-18 12:53:49 UTC, 130.5 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 5 clients, pipeline with 1000 ms think time, 120 changes per client, seed 2
- Clients by role: 4 Registrant, 4 Reviewer, 2 Admin
- Data folder `stress-runs/win-w2-real-NaiveFile-2/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-w2-real-NaiveFile-2`
- 1 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-w2-real-NaiveFile-2` there
- 1 of the servers on `Administrator@ws2.example`, data folder `\\ws1.example\share\runs\win-w2-real-NaiveFile-2` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 744 | 701 | 5.4 | 4 | 0 | 0 | 14.6 | 44.1 | 195.3 |

Writes by outcome: conflict 4, http500 39, saved 701. Saves by action: Create 156, Update 161, Submit 147, Approve 102, Reject 45, Comment 90. Reads: 607 (http500 8, notFound 3).

Stale reads: 4 (older than a version already acknowledged); 158 reads followed a save through another server.
- record 3dc4b667-e44f-4a7a-ac59-d64d78242ab0 through ws1.example:37779: version 0, 1065.4 ms after version 1 was acknowledged by ws1.example:37779
- record 3dc4b667-e44f-4a7a-ac59-d64d78242ab0 through ws1.example:37779: version 0, 1992.9 ms after version 1 was acknowledged by ws1.example:37779
- record 3dc4b667-e44f-4a7a-ac59-d64d78242ab0 through ws1.example:37779: version 0, 2508.4 ms after version 1 was acknowledged by ws1.example:37779
- record 4b6e0c0e-ba8f-4481-bfa2-01dd720e5652 through ws1.example:37779: version 1, 1347.3 ms after version 2 was acknowledged by ws1.example:37779

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | FAIL | 155/156 | 1 | 4 | 0 | 0 | 0 |
| server ws1.example:37779 | FAIL | 155/156 | 1 | 4 | 0 | 0 | 0 |
| server ws2.example:41777 | FAIL | 155/156 | 1 | 4 | 0 | 0 | 0 |

storage:
- missing record 3dc4b667-e44f-4a7a-ac59-d64d78242ab0: missing (1 acknowledged changes)
- lost update 4b6e0c0e-ba8f-4481-bfa2-01dd720e5652 v2 Update by s1c1-registrant 's1c1-registrant:105' via ws1.example:37779 (stress-1a6d87a1bfb84f66ba254ddcf5e6dd55): version 2 holds Update by s1c1-registrant
- lost update 4b6e0c0e-ba8f-4481-bfa2-01dd720e5652 v2 Update by s1c1-registrant 's1c1-registrant:104' via ws1.example:37779 (stress-f9d05bcf32c84e6f96591dcf40c62aac): the form data is from another update (notes 's1c1-registrant:105')
- lost update 3dc4b667-e44f-4a7a-ac59-d64d78242ab0 v1 Create by s1c3-registrant via ws1.example:37779 (stress-2db597d1e68c48ea8364b32766487c12): record missing
- lost update 9e1d65e9-ce2e-4692-b576-69dd98f6eab7 v5 Comment by s1c5-reviewer 's1c5-reviewer:36' via ws1.example:37779 (stress-4492881c11364fd7957a76e02634bbe1): version 5 holds Reject by s1c2-reviewer

server ws1.example:37779:
- missing record 3dc4b667-e44f-4a7a-ac59-d64d78242ab0: missing (1 acknowledged changes)
- lost update 4b6e0c0e-ba8f-4481-bfa2-01dd720e5652 v2 Update by s1c1-registrant 's1c1-registrant:105' via ws1.example:37779 (stress-1a6d87a1bfb84f66ba254ddcf5e6dd55): version 2 holds Update by s1c1-registrant
- lost update 4b6e0c0e-ba8f-4481-bfa2-01dd720e5652 v2 Update by s1c1-registrant 's1c1-registrant:104' via ws1.example:37779 (stress-f9d05bcf32c84e6f96591dcf40c62aac): the form data is from another update (notes 's1c1-registrant:105')
- lost update 3dc4b667-e44f-4a7a-ac59-d64d78242ab0 v1 Create by s1c3-registrant via ws1.example:37779 (stress-2db597d1e68c48ea8364b32766487c12): record missing
- lost update 9e1d65e9-ce2e-4692-b576-69dd98f6eab7 v5 Comment by s1c5-reviewer 's1c5-reviewer:36' via ws1.example:37779 (stress-4492881c11364fd7957a76e02634bbe1): version 5 holds Reject by s1c2-reviewer

server ws2.example:41777:
- missing record 3dc4b667-e44f-4a7a-ac59-d64d78242ab0: missing (1 acknowledged changes)
- lost update 4b6e0c0e-ba8f-4481-bfa2-01dd720e5652 v2 Update by s1c1-registrant 's1c1-registrant:105' via ws1.example:37779 (stress-1a6d87a1bfb84f66ba254ddcf5e6dd55): version 2 holds Update by s1c1-registrant
- lost update 4b6e0c0e-ba8f-4481-bfa2-01dd720e5652 v2 Update by s1c1-registrant 's1c1-registrant:104' via ws1.example:37779 (stress-f9d05bcf32c84e6f96591dcf40c62aac): the form data is from another update (notes 's1c1-registrant:105')
- lost update 3dc4b667-e44f-4a7a-ac59-d64d78242ab0 v1 Create by s1c3-registrant via ws1.example:37779 (stress-2db597d1e68c48ea8364b32766487c12): record missing
- lost update 9e1d65e9-ce2e-4692-b576-69dd98f6eab7 v5 Comment by s1c5-reviewer 's1c5-reviewer:36' via ws1.example:37779 (stress-4492881c11364fd7957a76e02634bbe1): version 5 holds Reject by s1c2-reviewer

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 2 | 701 | 5.4 | 4 | 0 | 4 | 0 | 0 | 44.1 | no |
