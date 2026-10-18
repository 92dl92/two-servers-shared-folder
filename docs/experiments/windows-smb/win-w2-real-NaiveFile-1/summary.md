# Stress run: NaiveFile, 2 server(s): FAILED

- Commit `fe53fea387646cb0ee65f1cccd5a309abb2291d2`, 2026-10-18 12:51:10 UTC, 127.2 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 5 clients, pipeline with 1000 ms think time, 120 changes per client, seed 1
- Clients by role: 4 Registrant, 4 Reviewer, 2 Admin
- Data folder `stress-runs/win-w2-real-NaiveFile-1/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-w2-real-NaiveFile-1`
- 1 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-w2-real-NaiveFile-1` there
- 1 of the servers on `Administrator@ws2.example`, data folder `\\ws1.example\share\runs\win-w2-real-NaiveFile-1` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 736 | 706 | 5.6 | 4 | 0 | 0 | 13.3 | 40.5 | 175.6 |

Writes by outcome: conflict 4, http500 26, saved 706. Saves by action: Create 163, Update 148, Submit 157, Approve 119, Reject 38, Comment 81. Reads: 581 (http500 5, notFound 1).

Stale reads: 2 (older than a version already acknowledged); 140 reads followed a save through another server.
- record 45a6d5ea-1a07-4e83-87f3-f0a46e63ed3f through ws1.example:36813: version 0, 971.2 ms after version 1 was acknowledged by ws1.example:36813
- record 3cd3b9bd-086c-4f6e-9371-c6d3e32384b1 through ws2.example:39169: version 5, 64.0 ms after version 6 was acknowledged by ws1.example:36813

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | FAIL | 162/163 | 1 | 4 | 0 | 0 | 0 |
| server ws1.example:36813 | FAIL | 162/163 | 1 | 4 | 0 | 0 | 0 |
| server ws2.example:39169 | FAIL | 162/163 | 1 | 4 | 0 | 0 | 0 |

storage:
- missing record 45a6d5ea-1a07-4e83-87f3-f0a46e63ed3f: missing (1 acknowledged changes)
- lost update 3cd3b9bd-086c-4f6e-9371-c6d3e32384b1 v6 Comment by s1c5-reviewer 's1c5-reviewer:42' via ws1.example:36813 (stress-497bb88600694e0684f78bc6a368afd5): version 6 holds Comment by s2c4-admin
- lost update d972088c-18a6-41a3-828d-cbae38186e52 v4 Comment by s1c5-reviewer 's1c5-reviewer:27' via ws1.example:36813 (stress-253db4ae4b1242c8839e9d0a02a8d626): version 4 holds Approve by s2c4-admin
- lost update 47b23532-2e26-4136-8620-4c071eb3b37f v4 Reject by s1c5-reviewer via ws1.example:36813 (stress-9f2e77427a5e40a0b13793e07063321a): no history entry at that version
- lost update 45a6d5ea-1a07-4e83-87f3-f0a46e63ed3f v1 Create by s1c1-registrant via ws1.example:36813 (stress-8077bc92791840f3a957810ddb84537a): record missing

server ws1.example:36813:
- missing record 45a6d5ea-1a07-4e83-87f3-f0a46e63ed3f: missing (1 acknowledged changes)
- lost update 3cd3b9bd-086c-4f6e-9371-c6d3e32384b1 v6 Comment by s1c5-reviewer 's1c5-reviewer:42' via ws1.example:36813 (stress-497bb88600694e0684f78bc6a368afd5): version 6 holds Comment by s2c4-admin
- lost update d972088c-18a6-41a3-828d-cbae38186e52 v4 Comment by s1c5-reviewer 's1c5-reviewer:27' via ws1.example:36813 (stress-253db4ae4b1242c8839e9d0a02a8d626): version 4 holds Approve by s2c4-admin
- lost update 47b23532-2e26-4136-8620-4c071eb3b37f v4 Reject by s1c5-reviewer via ws1.example:36813 (stress-9f2e77427a5e40a0b13793e07063321a): no history entry at that version
- lost update 45a6d5ea-1a07-4e83-87f3-f0a46e63ed3f v1 Create by s1c1-registrant via ws1.example:36813 (stress-8077bc92791840f3a957810ddb84537a): record missing

server ws2.example:39169:
- missing record 45a6d5ea-1a07-4e83-87f3-f0a46e63ed3f: missing (1 acknowledged changes)
- lost update 3cd3b9bd-086c-4f6e-9371-c6d3e32384b1 v6 Comment by s1c5-reviewer 's1c5-reviewer:42' via ws1.example:36813 (stress-497bb88600694e0684f78bc6a368afd5): version 6 holds Comment by s2c4-admin
- lost update d972088c-18a6-41a3-828d-cbae38186e52 v4 Comment by s1c5-reviewer 's1c5-reviewer:27' via ws1.example:36813 (stress-253db4ae4b1242c8839e9d0a02a8d626): version 4 holds Approve by s2c4-admin
- lost update 47b23532-2e26-4136-8620-4c071eb3b37f v4 Reject by s1c5-reviewer via ws1.example:36813 (stress-9f2e77427a5e40a0b13793e07063321a): no history entry at that version
- lost update 45a6d5ea-1a07-4e83-87f3-f0a46e63ed3f v1 Create by s1c1-registrant via ws1.example:36813 (stress-8077bc92791840f3a957810ddb84537a): record missing

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 2 | 706 | 5.6 | 4 | 0 | 4 | 0 | 0 | 40.5 | no |
