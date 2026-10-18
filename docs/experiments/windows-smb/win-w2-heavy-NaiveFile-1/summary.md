# Stress run: NaiveFile, 2 server(s): FAILED

- Commit `c71df76c37c18ca38a44e27c02961e268fbb1c09`, 2026-10-18 12:17:08 UTC, 11.3 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 12 clients, 12 records, 200 changes per client, seed 1
- Clients by role: 10 Registrant, 10 Reviewer, 4 Admin
- Data folder `stress-runs/win-w2-heavy-NaiveFile-1/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-w2-heavy-NaiveFile-1`
- 1 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-w2-heavy-NaiveFile-1` there
- 1 of the servers on `Administrator@ws2.example`, data folder `\\ws1.example\share\runs\win-w2-heavy-NaiveFile-1` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 1892 | 168 | 14.8 | 157 | 484 | 0 | 40.5 | 125.4 | 314.8 |

Writes by outcome: conflict 157, http500 1083, saved 168, wrongStatus 484. Saves by action: Create 6, Update 17, Submit 3, Approve 1, Reject 2, Comment 139. Reads: 4000 (http500 190, notFound 1930).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | FAIL | 3/6 | 3 | 6 | 0 | 0 | 0 |
| server ws1.example:38909 | FAIL | 3/6 | 3 | 6 | 0 | 0 | 0 |
| server ws2.example:36907 | FAIL | 3/6 | 3 | 6 | 0 | 0 | 0 |

storage:
- missing record 950647d7-6c80-4f75-8b94-3e58d011f612: missing (1 acknowledged changes)
- missing record 146df4ea-22af-4e12-85f3-fc859d4d3b88: missing (1 acknowledged changes)
- missing record aa8b83a3-4460-4c88-92a6-8191d8e8265a: missing (1 acknowledged changes)
- lost update 950647d7-6c80-4f75-8b94-3e58d011f612 v1 Create by s1c1-registrant via ws1.example:38909 (stress-c21d4a3befbf4af099d2513ffda9abba): record missing
- lost update f5041e1f-c2a8-4191-86c4-486c34e403a4 v7 Update by s1c4-admin 's1c4-admin:30' via ws1.example:38909 (stress-2339d61f2f284f01b6848d444eee9f17): version 7 holds Comment by s1c7-reviewer
- lost update 146df4ea-22af-4e12-85f3-fc859d4d3b88 v1 Create by s1c6-registrant via ws1.example:38909 (stress-23d3486ef75c4bbaa966d20c6bfea595): record missing
- lost update 26fb02a5-fee2-44ba-bbf3-59ea190b3025 v9 Comment by s1c12-reviewer 's1c12-reviewer:35' via ws1.example:38909 (stress-abf592f6a2204a21abda17fb19aec18a): version 9 holds Comment by s1c2-reviewer
- lost update 26fb02a5-fee2-44ba-bbf3-59ea190b3025 v45 Comment by s2c7-reviewer 's2c7-reviewer:114' via ws2.example:36907 (stress-2ada7e95b0aa4e569bd25b2812187774): version 45 holds Comment by s2c12-reviewer
- lost update aa8b83a3-4460-4c88-92a6-8191d8e8265a v1 Create by s1c11-registrant via ws1.example:38909 (stress-240abc44e68047c38756079af05911b2): record missing

server ws1.example:38909:
- missing record 950647d7-6c80-4f75-8b94-3e58d011f612: missing (1 acknowledged changes)
- missing record 146df4ea-22af-4e12-85f3-fc859d4d3b88: missing (1 acknowledged changes)
- missing record aa8b83a3-4460-4c88-92a6-8191d8e8265a: missing (1 acknowledged changes)
- lost update 950647d7-6c80-4f75-8b94-3e58d011f612 v1 Create by s1c1-registrant via ws1.example:38909 (stress-c21d4a3befbf4af099d2513ffda9abba): record missing
- lost update f5041e1f-c2a8-4191-86c4-486c34e403a4 v7 Update by s1c4-admin 's1c4-admin:30' via ws1.example:38909 (stress-2339d61f2f284f01b6848d444eee9f17): version 7 holds Comment by s1c7-reviewer
- lost update 146df4ea-22af-4e12-85f3-fc859d4d3b88 v1 Create by s1c6-registrant via ws1.example:38909 (stress-23d3486ef75c4bbaa966d20c6bfea595): record missing
- lost update 26fb02a5-fee2-44ba-bbf3-59ea190b3025 v9 Comment by s1c12-reviewer 's1c12-reviewer:35' via ws1.example:38909 (stress-abf592f6a2204a21abda17fb19aec18a): version 9 holds Comment by s1c2-reviewer
- lost update 26fb02a5-fee2-44ba-bbf3-59ea190b3025 v45 Comment by s2c7-reviewer 's2c7-reviewer:114' via ws2.example:36907 (stress-2ada7e95b0aa4e569bd25b2812187774): version 45 holds Comment by s2c12-reviewer
- lost update aa8b83a3-4460-4c88-92a6-8191d8e8265a v1 Create by s1c11-registrant via ws1.example:38909 (stress-240abc44e68047c38756079af05911b2): record missing

server ws2.example:36907:
- missing record 950647d7-6c80-4f75-8b94-3e58d011f612: missing (1 acknowledged changes)
- missing record 146df4ea-22af-4e12-85f3-fc859d4d3b88: missing (1 acknowledged changes)
- missing record aa8b83a3-4460-4c88-92a6-8191d8e8265a: missing (1 acknowledged changes)
- lost update 950647d7-6c80-4f75-8b94-3e58d011f612 v1 Create by s1c1-registrant via ws1.example:38909 (stress-c21d4a3befbf4af099d2513ffda9abba): record missing
- lost update f5041e1f-c2a8-4191-86c4-486c34e403a4 v7 Update by s1c4-admin 's1c4-admin:30' via ws1.example:38909 (stress-2339d61f2f284f01b6848d444eee9f17): version 7 holds Comment by s1c7-reviewer
- lost update 146df4ea-22af-4e12-85f3-fc859d4d3b88 v1 Create by s1c6-registrant via ws1.example:38909 (stress-23d3486ef75c4bbaa966d20c6bfea595): record missing
- lost update 26fb02a5-fee2-44ba-bbf3-59ea190b3025 v9 Comment by s1c12-reviewer 's1c12-reviewer:35' via ws1.example:38909 (stress-abf592f6a2204a21abda17fb19aec18a): version 9 holds Comment by s1c2-reviewer
- lost update 26fb02a5-fee2-44ba-bbf3-59ea190b3025 v45 Comment by s2c7-reviewer 's2c7-reviewer:114' via ws2.example:36907 (stress-2ada7e95b0aa4e569bd25b2812187774): version 45 holds Comment by s2c12-reviewer
- lost update aa8b83a3-4460-4c88-92a6-8191d8e8265a v1 Create by s1c11-registrant via ws1.example:38909 (stress-240abc44e68047c38756079af05911b2): record missing

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 2 | 168 | 14.8 | 157 | 484 | 6 | 0 | 0 | 125.4 | no |
