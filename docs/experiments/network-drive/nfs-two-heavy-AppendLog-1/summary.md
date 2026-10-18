# Stress run: AppendLog, 3 server(s): FAILED

- Commit `1b49659b6d0d95e1a10858057ef4082c27040edb`, 2026-10-17 22:36:12 UTC, 51.6 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 12 records, 200 changes per client, seed 1
- Clients by role: 12 Registrant, 9 Reviewer, 3 Admin
- Data folder `/mnt/share/runs/nfs-two-heavy-AppendLog-1/data`; run folder `stress-runs/nfs/nfs-two-heavy-AppendLog-1`
- 1 of the servers on `user@mac.example`, data folder `/Volumes/share/runs/nfs-two-heavy-AppendLog-1/data` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 4412 | 2037 | 39.5 | 792 | 1581 | 0 | 183.2 | 490.2 | 1009.3 |

Writes by outcome: conflict 792, http500 2, saved 2037, wrongStatus 1581. Saves by action: Create 12, Update 835, Submit 20, Approve 6, Reject 12, Comment 1152. Reads: 4800 (notFound 400).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | FAIL | 10/12 | 2 | 1332 | 0 | 0 | 0 |
| server 41289 | FAIL | 10/12 | 2 | 1164 | 0 | 0 | 0 |
| server 45715 | FAIL | 10/12 | 2 | 1083 | 0 | 0 | 0 |
| server mac-host.local:45521 | FAIL | 10/12 | 2 | 1332 | 0 | 0 | 0 |

storage:
- missing record de07f5d8-840f-44c4-b808-9874fd9d0caa: missing (197 acknowledged changes)
- missing record 28c905d6-ddb3-4b1f-bf97-ee9d0b3efb20: missing (79 acknowledged changes)
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v8 Update by s1c1-registrant 's1c1-registrant:6' via 41289 (stress-b35c8f02e3aa43b6920b9f9185aef853): version 8 holds Approve by s3c5-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v9 Update by s1c1-registrant 's1c1-registrant:7' via 41289 (stress-4a49e3d1964d4d8e88a8cc60ae0c05fa): version 9 holds Comment by s3c2-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v11 Update by s1c1-registrant 's1c1-registrant:9' via 41289 (stress-3ab23c96d67b4cb3a4ba0d404925549a): version 11 holds Comment by s2c7-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v12 Update by s1c1-registrant 's1c1-registrant:10' via 41289 (stress-549297fe3aea45eaaadd5ec08a93fab3): version 12 holds Comment by s2c2-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v13 Update by s1c1-registrant 's1c1-registrant:11' via 41289 (stress-b509b0cb13254358aa481257dd16e470): version 13 holds Comment by s2c7-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v14 Update by s1c1-registrant 's1c1-registrant:12' via 41289 (stress-73c73ed580604a0a95a62ebe6272cbc3): version 14 holds Comment by s3c7-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v15 Update by s1c1-registrant 's1c1-registrant:13' via 41289 (stress-c1ffa70b75974744a3c0ad7cc21d0ef8): version 15 holds Comment by s2c2-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v16 Update by s1c1-registrant 's1c1-registrant:14' via 41289 (stress-779e236fdb804295923b36dad7aa0c41): version 16 holds Comment by s2c7-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v17 Update by s1c1-registrant 's1c1-registrant:15' via 41289 (stress-eab8053e008b494c8072655d1e97cb19): version 17 holds Comment by s2c7-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v18 Update by s1c1-registrant 's1c1-registrant:16' via 41289 (stress-a01e4b804b8346078bcc980bfaa77bf0): version 18 holds Comment by s2c2-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v19 Update by s1c1-registrant 's1c1-registrant:17' via 41289 (stress-fffb91f47b5745eebe9da1a87b78b24b): version 19 holds Comment by s2c4-admin
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v20 Update by s1c1-registrant 's1c1-registrant:18' via 41289 (stress-99ad366b637a47b0af3104a454c03094): version 20 holds Comment by s3c7-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v21 Update by s1c1-registrant 's1c1-registrant:19' via 41289 (stress-7a0aae05aa87405ca84e17fa7c615f4b): version 21 holds Comment by s3c7-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v22 Update by s1c1-registrant 's1c1-registrant:20' via 41289 (stress-a016665e57cf4cf39a34748c02f185ab): version 22 holds Comment by s3c2-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v23 Update by s1c1-registrant 's1c1-registrant:21' via 41289 (stress-90cc196a6d294d0582dba26938182e06): version 23 holds Comment by s3c7-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v24 Update by s1c1-registrant 's1c1-registrant:22' via 41289 (stress-995015761d6a45b7bd21e53340f599ec): version 24 holds Comment by s3c7-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v25 Update by s1c1-registrant 's1c1-registrant:23' via 41289 (stress-4d112e34caa34dcab9d3f33f16bdbd40): version 25 holds Comment by s3c7-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v26 Update by s1c1-registrant 's1c1-registrant:24' via 41289 (stress-35e8381480cd4258bc16046ee8cea312): version 26 holds Comment by s3c7-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v27 Update by s1c1-registrant 's1c1-registrant:25' via 41289 (stress-8037a69c188e4bee8fadba2ead0a08fd): version 27 holds Comment by s3c7-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v28 Update by s1c1-registrant 's1c1-registrant:26' via 41289 (stress-9c30042c018e4a0d89d9b0e47da2ad39): version 28 holds Comment by s3c2-reviewer

server 41289:
- missing record de07f5d8-840f-44c4-b808-9874fd9d0caa: missing (197 acknowledged changes)
- missing record 28c905d6-ddb3-4b1f-bf97-ee9d0b3efb20: missing (79 acknowledged changes)
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v10 Comment by s2c2-reviewer 's2c2-reviewer:22' via 45715 (stress-7a764ecd23b3460f8fd90b7df59eb68a): version 10 holds Comment by s1c5-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v12 Comment by s2c2-reviewer 's2c2-reviewer:28' via 45715 (stress-0fda32585f244083844c468a66a1aef9): version 12 holds Update by s1c1-registrant
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v15 Comment by s2c2-reviewer 's2c2-reviewer:38' via 45715 (stress-cd265cfe527a4e46a16346cb0e360c86): version 15 holds Update by s1c1-registrant
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v18 Comment by s2c2-reviewer 's2c2-reviewer:60' via 45715 (stress-a4eaec8e7dc64b7a8092bac81a2d0ca6): version 18 holds Update by s1c1-registrant
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v22 Comment by s2c2-reviewer 's2c2-reviewer:73' via 45715 (stress-47aed22e805e4356b6ea419087e81ee6): version 22 holds Update by s1c1-registrant
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v23 Comment by s2c2-reviewer 's2c2-reviewer:76' via 45715 (stress-8bef72c4f83c457088ca7fe514f26008): version 23 holds Update by s1c1-registrant
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v34 Comment by s2c2-reviewer 's2c2-reviewer:112' via 45715 (stress-8cf1fa5164de41b8bf26592472203a80): version 34 holds Update by s1c1-registrant
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v38 Comment by s2c2-reviewer 's2c2-reviewer:128' via 45715 (stress-4d6b032d4e63463da35779174271f5ef): version 38 holds Update by s1c1-registrant
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v42 Comment by s2c2-reviewer 's2c2-reviewer:140' via 45715 (stress-0742ed4f294e4ab899ec611a48e72bc8): version 42 holds Update by s1c1-registrant
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v45 Comment by s2c2-reviewer 's2c2-reviewer:150' via 45715 (stress-9e4d30afaee44069a530f7735f8e5a27): version 45 holds Comment by s1c7-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v49 Comment by s2c2-reviewer 's2c2-reviewer:161' via 45715 (stress-3d70f66fe1a548b89c6e726f8a24fce2): version 49 holds Update by s1c1-registrant
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v54 Comment by s2c2-reviewer 's2c2-reviewer:182' via 45715 (stress-7cc05818c2744433a39706c9912ac918): version 54 holds Update by s1c1-registrant
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v55 Comment by s2c2-reviewer 's2c2-reviewer:184' via 45715 (stress-2415234c1ed54006a722ca4c7cef4cca): version 55 holds Update by s1c1-registrant
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v56 Comment by s2c2-reviewer 's2c2-reviewer:185' via 45715 (stress-2e92f1d05cbb4a3eb4ce23de5a268e33): version 56 holds Update by s1c1-registrant
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v8 Comment by s2c4-admin 's2c4-admin:17' via 45715 (stress-b2f2b54998724d1baf2ce848d55c26ec): version 8 holds Update by s1c1-registrant
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v19 Comment by s2c4-admin 's2c4-admin:66' via 45715 (stress-d6c791b66e604e9da4d6539d73bfc2ac): version 19 holds Update by s1c1-registrant
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v21 Comment by s2c4-admin 's2c4-admin:72' via 45715 (stress-dd183796e55e4277bae7505b63294cda): version 21 holds Update by s1c1-registrant
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v35 Comment by s2c4-admin 's2c4-admin:130' via 45715 (stress-12905c9368024f2c9100c1f9c7fbf421): version 35 holds Update by s1c1-registrant
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v37 Comment by s2c4-admin 's2c4-admin:145' via 45715 (stress-29837b99cb4942b98601efc2ddc09b85): version 37 holds Update by s1c1-registrant
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v39 Comment by s2c4-admin 's2c4-admin:150' via 45715 (stress-a6c37d62bc9d484a87c34654cd43acc0): version 39 holds Update by s1c1-registrant

server 45715:
- missing record 1be16bdf-346a-4544-836d-8edd66893db5: missing (113 acknowledged changes)
- missing record ac439c99-0626-45d8-ab2d-41c2b4591f20: missing (187 acknowledged changes)
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v8 Update by s1c1-registrant 's1c1-registrant:6' via 41289 (stress-b35c8f02e3aa43b6920b9f9185aef853): version 8 holds Approve by s3c5-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v9 Update by s1c1-registrant 's1c1-registrant:7' via 41289 (stress-4a49e3d1964d4d8e88a8cc60ae0c05fa): version 9 holds Comment by s3c2-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v11 Update by s1c1-registrant 's1c1-registrant:9' via 41289 (stress-3ab23c96d67b4cb3a4ba0d404925549a): version 11 holds Comment by s2c7-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v12 Update by s1c1-registrant 's1c1-registrant:10' via 41289 (stress-549297fe3aea45eaaadd5ec08a93fab3): version 12 holds Comment by s2c2-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v13 Update by s1c1-registrant 's1c1-registrant:11' via 41289 (stress-b509b0cb13254358aa481257dd16e470): version 13 holds Comment by s2c7-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v14 Update by s1c1-registrant 's1c1-registrant:12' via 41289 (stress-73c73ed580604a0a95a62ebe6272cbc3): version 14 holds Comment by s3c7-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v15 Update by s1c1-registrant 's1c1-registrant:13' via 41289 (stress-c1ffa70b75974744a3c0ad7cc21d0ef8): version 15 holds Comment by s2c2-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v16 Update by s1c1-registrant 's1c1-registrant:14' via 41289 (stress-779e236fdb804295923b36dad7aa0c41): version 16 holds Comment by s2c7-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v17 Update by s1c1-registrant 's1c1-registrant:15' via 41289 (stress-eab8053e008b494c8072655d1e97cb19): version 17 holds Comment by s2c7-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v18 Update by s1c1-registrant 's1c1-registrant:16' via 41289 (stress-a01e4b804b8346078bcc980bfaa77bf0): version 18 holds Comment by s2c2-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v19 Update by s1c1-registrant 's1c1-registrant:17' via 41289 (stress-fffb91f47b5745eebe9da1a87b78b24b): version 19 holds Comment by s2c4-admin
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v20 Update by s1c1-registrant 's1c1-registrant:18' via 41289 (stress-99ad366b637a47b0af3104a454c03094): version 20 holds Comment by s3c7-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v21 Update by s1c1-registrant 's1c1-registrant:19' via 41289 (stress-7a0aae05aa87405ca84e17fa7c615f4b): version 21 holds Comment by s3c7-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v22 Update by s1c1-registrant 's1c1-registrant:20' via 41289 (stress-a016665e57cf4cf39a34748c02f185ab): version 22 holds Comment by s3c2-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v23 Update by s1c1-registrant 's1c1-registrant:21' via 41289 (stress-90cc196a6d294d0582dba26938182e06): version 23 holds Comment by s3c7-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v24 Update by s1c1-registrant 's1c1-registrant:22' via 41289 (stress-995015761d6a45b7bd21e53340f599ec): version 24 holds Comment by s3c7-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v25 Update by s1c1-registrant 's1c1-registrant:23' via 41289 (stress-4d112e34caa34dcab9d3f33f16bdbd40): version 25 holds Comment by s3c7-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v26 Update by s1c1-registrant 's1c1-registrant:24' via 41289 (stress-35e8381480cd4258bc16046ee8cea312): version 26 holds Comment by s3c7-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v27 Update by s1c1-registrant 's1c1-registrant:25' via 41289 (stress-8037a69c188e4bee8fadba2ead0a08fd): version 27 holds Comment by s3c7-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v28 Update by s1c1-registrant 's1c1-registrant:26' via 41289 (stress-9c30042c018e4a0d89d9b0e47da2ad39): version 28 holds Comment by s3c2-reviewer

server mac-host.local:45521:
- missing record de07f5d8-840f-44c4-b808-9874fd9d0caa: missing (197 acknowledged changes)
- missing record 28c905d6-ddb3-4b1f-bf97-ee9d0b3efb20: missing (79 acknowledged changes)
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v8 Update by s1c1-registrant 's1c1-registrant:6' via 41289 (stress-b35c8f02e3aa43b6920b9f9185aef853): version 8 holds Approve by s3c5-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v9 Update by s1c1-registrant 's1c1-registrant:7' via 41289 (stress-4a49e3d1964d4d8e88a8cc60ae0c05fa): version 9 holds Comment by s3c2-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v11 Update by s1c1-registrant 's1c1-registrant:9' via 41289 (stress-3ab23c96d67b4cb3a4ba0d404925549a): version 11 holds Comment by s2c7-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v12 Update by s1c1-registrant 's1c1-registrant:10' via 41289 (stress-549297fe3aea45eaaadd5ec08a93fab3): version 12 holds Comment by s2c2-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v13 Update by s1c1-registrant 's1c1-registrant:11' via 41289 (stress-b509b0cb13254358aa481257dd16e470): version 13 holds Comment by s2c7-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v14 Update by s1c1-registrant 's1c1-registrant:12' via 41289 (stress-73c73ed580604a0a95a62ebe6272cbc3): version 14 holds Comment by s3c7-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v15 Update by s1c1-registrant 's1c1-registrant:13' via 41289 (stress-c1ffa70b75974744a3c0ad7cc21d0ef8): version 15 holds Comment by s2c2-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v16 Update by s1c1-registrant 's1c1-registrant:14' via 41289 (stress-779e236fdb804295923b36dad7aa0c41): version 16 holds Comment by s2c7-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v17 Update by s1c1-registrant 's1c1-registrant:15' via 41289 (stress-eab8053e008b494c8072655d1e97cb19): version 17 holds Comment by s2c7-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v18 Update by s1c1-registrant 's1c1-registrant:16' via 41289 (stress-a01e4b804b8346078bcc980bfaa77bf0): version 18 holds Comment by s2c2-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v19 Update by s1c1-registrant 's1c1-registrant:17' via 41289 (stress-fffb91f47b5745eebe9da1a87b78b24b): version 19 holds Comment by s2c4-admin
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v20 Update by s1c1-registrant 's1c1-registrant:18' via 41289 (stress-99ad366b637a47b0af3104a454c03094): version 20 holds Comment by s3c7-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v21 Update by s1c1-registrant 's1c1-registrant:19' via 41289 (stress-7a0aae05aa87405ca84e17fa7c615f4b): version 21 holds Comment by s3c7-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v22 Update by s1c1-registrant 's1c1-registrant:20' via 41289 (stress-a016665e57cf4cf39a34748c02f185ab): version 22 holds Comment by s3c2-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v23 Update by s1c1-registrant 's1c1-registrant:21' via 41289 (stress-90cc196a6d294d0582dba26938182e06): version 23 holds Comment by s3c7-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v24 Update by s1c1-registrant 's1c1-registrant:22' via 41289 (stress-995015761d6a45b7bd21e53340f599ec): version 24 holds Comment by s3c7-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v25 Update by s1c1-registrant 's1c1-registrant:23' via 41289 (stress-4d112e34caa34dcab9d3f33f16bdbd40): version 25 holds Comment by s3c7-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v26 Update by s1c1-registrant 's1c1-registrant:24' via 41289 (stress-35e8381480cd4258bc16046ee8cea312): version 26 holds Comment by s3c7-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v27 Update by s1c1-registrant 's1c1-registrant:25' via 41289 (stress-8037a69c188e4bee8fadba2ead0a08fd): version 27 holds Comment by s3c7-reviewer
- lost update 1fe9fc04-cd5f-4a70-86bf-7f094731cb7e v28 Update by s1c1-registrant 's1c1-registrant:26' via 41289 (stress-9c30042c018e4a0d89d9b0e47da2ad39): version 28 holds Comment by s3c2-reviewer

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| AppendLog | 3 | 2037 | 39.5 | 792 | 1581 | 1332 | 0 | 0 | 490.2 | no |
