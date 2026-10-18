# Stress run: NaiveFile, 3 server(s): FAILED

- Commit `ee6af83bef739621009cd4be91192b48f8df58df`, 2026-10-17 19:07:27 UTC, 0.4 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 5 clients, 6 records, 40 changes per client, seed 1
- Data folder `stress-runs/light-NaiveFile-1/data`; run folder `stress-runs/light-NaiveFile-1`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 98 | 39 | 87.2 | 27 | 26 | 0 | 2.8 | 117.6 | 118.2 |

Writes by outcome: conflict 27, http500 6, saved 39, wrongStatus 26. Saves by action: Create 6, Update 4, Submit 3, Reject 1, Comment 25. Reads: 600 (http500 13, notFound 495).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | FAIL | 1/6 | 5 | 14 | 0 | 0 | 0 |
| server 35137 | FAIL | 1/6 | 5 | 14 | 0 | 0 | 0 |
| server 37761 | FAIL | 1/6 | 5 | 14 | 0 | 0 | 0 |
| server 34761 | FAIL | 1/6 | 5 | 14 | 0 | 0 | 0 |

storage:
- missing record 56dcb958-bd16-48bc-8b40-ba6a75fcd46b: missing (1 acknowledged changes)
- missing record 58ec1146-87d3-4747-8062-65523b75462a: missing (1 acknowledged changes)
- missing record 53e2d4b4-5b6a-403f-a43e-725b13dec5f4: missing (1 acknowledged changes)
- missing record 432fd708-b0ca-4f65-a54a-3f7aedcd7ca3: missing (1 acknowledged changes)
- missing record 8233e3a9-b70f-4d4c-a49e-33bf2bda6c8c: missing (1 acknowledged changes)
- lost update 56dcb958-bd16-48bc-8b40-ba6a75fcd46b v1 Create by s1c1-registrant via 35137 (stress-94092dc8c8ad48c5bb085176a90e10fb): record missing
- lost update b46f6860-d4f4-423b-850e-2cd5b551a48d v23 Comment by s1c5-reviewer 's1c5-reviewer:35' via 35137 (stress-f17d4f351cbe4aeebeb5c90c6165b635): version 23 holds Comment by s2c4-admin
- lost update b46f6860-d4f4-423b-850e-2cd5b551a48d v2 Submit by s2c1-registrant via 37761 (stress-2ed0918981624b97bf6671cf522ca010): version 2 holds Comment by s3c5-reviewer
- lost update b46f6860-d4f4-423b-850e-2cd5b551a48d v2 Comment by s2c2-reviewer 's2c2-reviewer:22' via 37761 (stress-3c60c7c9f470400b9461e49754a33737): version 2 holds Comment by s3c5-reviewer
- lost update b46f6860-d4f4-423b-850e-2cd5b551a48d v2 Submit by s2c4-admin via 37761 (stress-b80cfe1ff59c4c6090303b3f23202a0e): version 2 holds Comment by s3c5-reviewer
- lost update b46f6860-d4f4-423b-850e-2cd5b551a48d v21 Comment by s2c4-admin 's2c4-admin:25' via 37761 (stress-31a522a044854925bfcae5933c506169): version 21 holds Comment by s1c5-reviewer
- lost update b46f6860-d4f4-423b-850e-2cd5b551a48d v2 Comment by s2c5-reviewer 's2c5-reviewer:5' via 37761 (stress-d6120bc6b5ed4cda916bed401b1e470e): version 2 holds Comment by s3c5-reviewer
- lost update b46f6860-d4f4-423b-850e-2cd5b551a48d v16 Comment by s2c5-reviewer 's2c5-reviewer:20' via 37761 (stress-ab03aa3ca9bc4b3f9bd9042bbb0bdd60): version 16 holds Comment by s1c5-reviewer
- lost update b46f6860-d4f4-423b-850e-2cd5b551a48d v2 Comment by s3c2-reviewer 's3c2-reviewer:6' via 34761 (stress-006e9e5200ef496aaf8eeab8b9c7af68): version 2 holds Comment by s3c5-reviewer
- lost update b46f6860-d4f4-423b-850e-2cd5b551a48d v2 Comment by s3c4-admin 's3c4-admin:6' via 34761 (stress-63a432e342b740fab59c8c820c5b0a88): version 2 holds Comment by s3c5-reviewer
- lost update 58ec1146-87d3-4747-8062-65523b75462a v1 Create by s3c1-registrant via 34761 (stress-3a586e68836249f8b45ce7d5586ddcf5): record missing
- lost update 53e2d4b4-5b6a-403f-a43e-725b13dec5f4 v1 Create by s1c3-registrant via 35137 (stress-d7c90dbabc4442b093392bddd27f571e): record missing
- lost update 432fd708-b0ca-4f65-a54a-3f7aedcd7ca3 v1 Create by s2c3-registrant via 37761 (stress-cdcc44c71b6e444db73a1ae8856a881d): record missing
- lost update 8233e3a9-b70f-4d4c-a49e-33bf2bda6c8c v1 Create by s3c3-registrant via 34761 (stress-8c3a8bd1fcec49168e2bca1ae4d8bbf3): record missing

server 35137:
- missing record 56dcb958-bd16-48bc-8b40-ba6a75fcd46b: missing (1 acknowledged changes)
- missing record 58ec1146-87d3-4747-8062-65523b75462a: missing (1 acknowledged changes)
- missing record 53e2d4b4-5b6a-403f-a43e-725b13dec5f4: missing (1 acknowledged changes)
- missing record 432fd708-b0ca-4f65-a54a-3f7aedcd7ca3: missing (1 acknowledged changes)
- missing record 8233e3a9-b70f-4d4c-a49e-33bf2bda6c8c: missing (1 acknowledged changes)
- lost update 56dcb958-bd16-48bc-8b40-ba6a75fcd46b v1 Create by s1c1-registrant via 35137 (stress-94092dc8c8ad48c5bb085176a90e10fb): record missing
- lost update b46f6860-d4f4-423b-850e-2cd5b551a48d v23 Comment by s1c5-reviewer 's1c5-reviewer:35' via 35137 (stress-f17d4f351cbe4aeebeb5c90c6165b635): version 23 holds Comment by s2c4-admin
- lost update b46f6860-d4f4-423b-850e-2cd5b551a48d v2 Submit by s2c1-registrant via 37761 (stress-2ed0918981624b97bf6671cf522ca010): version 2 holds Comment by s3c5-reviewer
- lost update b46f6860-d4f4-423b-850e-2cd5b551a48d v2 Comment by s2c2-reviewer 's2c2-reviewer:22' via 37761 (stress-3c60c7c9f470400b9461e49754a33737): version 2 holds Comment by s3c5-reviewer
- lost update b46f6860-d4f4-423b-850e-2cd5b551a48d v2 Submit by s2c4-admin via 37761 (stress-b80cfe1ff59c4c6090303b3f23202a0e): version 2 holds Comment by s3c5-reviewer
- lost update b46f6860-d4f4-423b-850e-2cd5b551a48d v21 Comment by s2c4-admin 's2c4-admin:25' via 37761 (stress-31a522a044854925bfcae5933c506169): version 21 holds Comment by s1c5-reviewer
- lost update b46f6860-d4f4-423b-850e-2cd5b551a48d v2 Comment by s2c5-reviewer 's2c5-reviewer:5' via 37761 (stress-d6120bc6b5ed4cda916bed401b1e470e): version 2 holds Comment by s3c5-reviewer
- lost update b46f6860-d4f4-423b-850e-2cd5b551a48d v16 Comment by s2c5-reviewer 's2c5-reviewer:20' via 37761 (stress-ab03aa3ca9bc4b3f9bd9042bbb0bdd60): version 16 holds Comment by s1c5-reviewer
- lost update b46f6860-d4f4-423b-850e-2cd5b551a48d v2 Comment by s3c2-reviewer 's3c2-reviewer:6' via 34761 (stress-006e9e5200ef496aaf8eeab8b9c7af68): version 2 holds Comment by s3c5-reviewer
- lost update b46f6860-d4f4-423b-850e-2cd5b551a48d v2 Comment by s3c4-admin 's3c4-admin:6' via 34761 (stress-63a432e342b740fab59c8c820c5b0a88): version 2 holds Comment by s3c5-reviewer
- lost update 58ec1146-87d3-4747-8062-65523b75462a v1 Create by s3c1-registrant via 34761 (stress-3a586e68836249f8b45ce7d5586ddcf5): record missing
- lost update 53e2d4b4-5b6a-403f-a43e-725b13dec5f4 v1 Create by s1c3-registrant via 35137 (stress-d7c90dbabc4442b093392bddd27f571e): record missing
- lost update 432fd708-b0ca-4f65-a54a-3f7aedcd7ca3 v1 Create by s2c3-registrant via 37761 (stress-cdcc44c71b6e444db73a1ae8856a881d): record missing
- lost update 8233e3a9-b70f-4d4c-a49e-33bf2bda6c8c v1 Create by s3c3-registrant via 34761 (stress-8c3a8bd1fcec49168e2bca1ae4d8bbf3): record missing

server 37761:
- missing record 56dcb958-bd16-48bc-8b40-ba6a75fcd46b: missing (1 acknowledged changes)
- missing record 58ec1146-87d3-4747-8062-65523b75462a: missing (1 acknowledged changes)
- missing record 53e2d4b4-5b6a-403f-a43e-725b13dec5f4: missing (1 acknowledged changes)
- missing record 432fd708-b0ca-4f65-a54a-3f7aedcd7ca3: missing (1 acknowledged changes)
- missing record 8233e3a9-b70f-4d4c-a49e-33bf2bda6c8c: missing (1 acknowledged changes)
- lost update 56dcb958-bd16-48bc-8b40-ba6a75fcd46b v1 Create by s1c1-registrant via 35137 (stress-94092dc8c8ad48c5bb085176a90e10fb): record missing
- lost update b46f6860-d4f4-423b-850e-2cd5b551a48d v23 Comment by s1c5-reviewer 's1c5-reviewer:35' via 35137 (stress-f17d4f351cbe4aeebeb5c90c6165b635): version 23 holds Comment by s2c4-admin
- lost update b46f6860-d4f4-423b-850e-2cd5b551a48d v2 Submit by s2c1-registrant via 37761 (stress-2ed0918981624b97bf6671cf522ca010): version 2 holds Comment by s3c5-reviewer
- lost update b46f6860-d4f4-423b-850e-2cd5b551a48d v2 Comment by s2c2-reviewer 's2c2-reviewer:22' via 37761 (stress-3c60c7c9f470400b9461e49754a33737): version 2 holds Comment by s3c5-reviewer
- lost update b46f6860-d4f4-423b-850e-2cd5b551a48d v2 Submit by s2c4-admin via 37761 (stress-b80cfe1ff59c4c6090303b3f23202a0e): version 2 holds Comment by s3c5-reviewer
- lost update b46f6860-d4f4-423b-850e-2cd5b551a48d v21 Comment by s2c4-admin 's2c4-admin:25' via 37761 (stress-31a522a044854925bfcae5933c506169): version 21 holds Comment by s1c5-reviewer
- lost update b46f6860-d4f4-423b-850e-2cd5b551a48d v2 Comment by s2c5-reviewer 's2c5-reviewer:5' via 37761 (stress-d6120bc6b5ed4cda916bed401b1e470e): version 2 holds Comment by s3c5-reviewer
- lost update b46f6860-d4f4-423b-850e-2cd5b551a48d v16 Comment by s2c5-reviewer 's2c5-reviewer:20' via 37761 (stress-ab03aa3ca9bc4b3f9bd9042bbb0bdd60): version 16 holds Comment by s1c5-reviewer
- lost update b46f6860-d4f4-423b-850e-2cd5b551a48d v2 Comment by s3c2-reviewer 's3c2-reviewer:6' via 34761 (stress-006e9e5200ef496aaf8eeab8b9c7af68): version 2 holds Comment by s3c5-reviewer
- lost update b46f6860-d4f4-423b-850e-2cd5b551a48d v2 Comment by s3c4-admin 's3c4-admin:6' via 34761 (stress-63a432e342b740fab59c8c820c5b0a88): version 2 holds Comment by s3c5-reviewer
- lost update 58ec1146-87d3-4747-8062-65523b75462a v1 Create by s3c1-registrant via 34761 (stress-3a586e68836249f8b45ce7d5586ddcf5): record missing
- lost update 53e2d4b4-5b6a-403f-a43e-725b13dec5f4 v1 Create by s1c3-registrant via 35137 (stress-d7c90dbabc4442b093392bddd27f571e): record missing
- lost update 432fd708-b0ca-4f65-a54a-3f7aedcd7ca3 v1 Create by s2c3-registrant via 37761 (stress-cdcc44c71b6e444db73a1ae8856a881d): record missing
- lost update 8233e3a9-b70f-4d4c-a49e-33bf2bda6c8c v1 Create by s3c3-registrant via 34761 (stress-8c3a8bd1fcec49168e2bca1ae4d8bbf3): record missing

server 34761:
- missing record 56dcb958-bd16-48bc-8b40-ba6a75fcd46b: missing (1 acknowledged changes)
- missing record 58ec1146-87d3-4747-8062-65523b75462a: missing (1 acknowledged changes)
- missing record 53e2d4b4-5b6a-403f-a43e-725b13dec5f4: missing (1 acknowledged changes)
- missing record 432fd708-b0ca-4f65-a54a-3f7aedcd7ca3: missing (1 acknowledged changes)
- missing record 8233e3a9-b70f-4d4c-a49e-33bf2bda6c8c: missing (1 acknowledged changes)
- lost update 56dcb958-bd16-48bc-8b40-ba6a75fcd46b v1 Create by s1c1-registrant via 35137 (stress-94092dc8c8ad48c5bb085176a90e10fb): record missing
- lost update b46f6860-d4f4-423b-850e-2cd5b551a48d v23 Comment by s1c5-reviewer 's1c5-reviewer:35' via 35137 (stress-f17d4f351cbe4aeebeb5c90c6165b635): version 23 holds Comment by s2c4-admin
- lost update b46f6860-d4f4-423b-850e-2cd5b551a48d v2 Submit by s2c1-registrant via 37761 (stress-2ed0918981624b97bf6671cf522ca010): version 2 holds Comment by s3c5-reviewer
- lost update b46f6860-d4f4-423b-850e-2cd5b551a48d v2 Comment by s2c2-reviewer 's2c2-reviewer:22' via 37761 (stress-3c60c7c9f470400b9461e49754a33737): version 2 holds Comment by s3c5-reviewer
- lost update b46f6860-d4f4-423b-850e-2cd5b551a48d v2 Submit by s2c4-admin via 37761 (stress-b80cfe1ff59c4c6090303b3f23202a0e): version 2 holds Comment by s3c5-reviewer
- lost update b46f6860-d4f4-423b-850e-2cd5b551a48d v21 Comment by s2c4-admin 's2c4-admin:25' via 37761 (stress-31a522a044854925bfcae5933c506169): version 21 holds Comment by s1c5-reviewer
- lost update b46f6860-d4f4-423b-850e-2cd5b551a48d v2 Comment by s2c5-reviewer 's2c5-reviewer:5' via 37761 (stress-d6120bc6b5ed4cda916bed401b1e470e): version 2 holds Comment by s3c5-reviewer
- lost update b46f6860-d4f4-423b-850e-2cd5b551a48d v16 Comment by s2c5-reviewer 's2c5-reviewer:20' via 37761 (stress-ab03aa3ca9bc4b3f9bd9042bbb0bdd60): version 16 holds Comment by s1c5-reviewer
- lost update b46f6860-d4f4-423b-850e-2cd5b551a48d v2 Comment by s3c2-reviewer 's3c2-reviewer:6' via 34761 (stress-006e9e5200ef496aaf8eeab8b9c7af68): version 2 holds Comment by s3c5-reviewer
- lost update b46f6860-d4f4-423b-850e-2cd5b551a48d v2 Comment by s3c4-admin 's3c4-admin:6' via 34761 (stress-63a432e342b740fab59c8c820c5b0a88): version 2 holds Comment by s3c5-reviewer
- lost update 58ec1146-87d3-4747-8062-65523b75462a v1 Create by s3c1-registrant via 34761 (stress-3a586e68836249f8b45ce7d5586ddcf5): record missing
- lost update 53e2d4b4-5b6a-403f-a43e-725b13dec5f4 v1 Create by s1c3-registrant via 35137 (stress-d7c90dbabc4442b093392bddd27f571e): record missing
- lost update 432fd708-b0ca-4f65-a54a-3f7aedcd7ca3 v1 Create by s2c3-registrant via 37761 (stress-cdcc44c71b6e444db73a1ae8856a881d): record missing
- lost update 8233e3a9-b70f-4d4c-a49e-33bf2bda6c8c v1 Create by s3c3-registrant via 34761 (stress-8c3a8bd1fcec49168e2bca1ae4d8bbf3): record missing

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 3 | 39 | 87.2 | 27 | 26 | 14 | 0 | 0 | 117.6 | no |
