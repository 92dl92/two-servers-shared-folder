# Stress run: AppendLog, 3 server(s): FAILED

- Commit `1b49659b6d0d95e1a10858057ef4082c27040edb`, 2026-10-17 22:20:17 UTC, 7.2 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 5 clients, 6 records, 40 changes per client, seed 3
- Clients by role: 6 Registrant, 6 Reviewer, 3 Admin
- Data folder `/mnt/share/runs/nfs-two-light-AppendLog-3/data`; run folder `stress-runs/nfs/nfs-two-light-AppendLog-3`
- 1 of the servers on `user@mac.example`, data folder `/Volumes/share/runs/nfs-two-light-AppendLog-3/data` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 551 | 256 | 35.7 | 160 | 131 | 0 | 85.2 | 239.0 | 418.2 |

Writes by outcome: conflict 160, http500 4, saved 256, wrongStatus 131. Saves by action: Create 6, Update 88, Submit 8, Approve 3, Reject 3, Comment 148. Reads: 600 (notFound 55).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | FAIL | 5/6 | 1 | 133 | 0 | 0 | 0 |
| server 39195 | FAIL | 5/6 | 1 | 118 | 0 | 0 | 0 |
| server 39527 | FAIL | 5/6 | 1 | 81 | 0 | 0 | 0 |
| server mac-host.local:33777 | FAIL | 5/6 | 1 | 133 | 0 | 0 | 0 |

storage:
- missing record 918ec255-ee0c-4797-a8d2-800e829a7691: missing (40 acknowledged changes)
- lost update a8821437-7daa-441d-b155-407d600d75f1 v19 Comment by s1c2-reviewer 's1c2-reviewer:33' via 39195 (stress-a4078e7420964cc384bcde9ab04dc683): version 19 holds Comment by s3c4-admin
- lost update a8821437-7daa-441d-b155-407d600d75f1 v20 Comment by s1c2-reviewer 's1c2-reviewer:34' via 39195 (stress-cc599f0572bf4ca1976f31f080d46610): version 20 holds Comment by s3c2-reviewer
- lost update a8821437-7daa-441d-b155-407d600d75f1 v21 Comment by s1c2-reviewer 's1c2-reviewer:38' via 39195 (stress-69b7225a81f34ad9a8b589ae2709d900): version 21 holds Comment by s3c5-reviewer
- lost update a8821437-7daa-441d-b155-407d600d75f1 v17 Comment by s1c4-admin 's1c4-admin:25' via 39195 (stress-d26608e282eb4da2be34d23cb6bc225a): version 17 holds Comment by s3c4-admin
- lost update a8821437-7daa-441d-b155-407d600d75f1 v18 Comment by s1c4-admin 's1c4-admin:30' via 39195 (stress-591a703dd4764e6f8d4a58b0fce14906): version 18 holds Comment by s3c5-reviewer
- lost update a8821437-7daa-441d-b155-407d600d75f1 v13 Comment by s1c5-reviewer 's1c5-reviewer:21' via 39195 (stress-6d1e59a6a3484be99dbcb8a431bf172e): version 13 holds Comment by s3c5-reviewer
- lost update a8821437-7daa-441d-b155-407d600d75f1 v14 Comment by s1c5-reviewer 's1c5-reviewer:22' via 39195 (stress-00ff9189fceb43f9b7447e4a327c1e15): version 14 holds Comment by s3c4-admin
- lost update a8821437-7daa-441d-b155-407d600d75f1 v15 Comment by s1c5-reviewer 's1c5-reviewer:24' via 39195 (stress-e094a85cd18143af918fbbf97d589dc5): version 15 holds Comment by s3c2-reviewer
- lost update a8821437-7daa-441d-b155-407d600d75f1 v16 Comment by s1c5-reviewer 's1c5-reviewer:26' via 39195 (stress-1fab14eb3cfa433888168fcc9a894623): version 16 holds Comment by s3c2-reviewer
- lost update a8821437-7daa-441d-b155-407d600d75f1 v21 Comment by s2c2-reviewer 's2c2-reviewer:31' via 39527 (stress-0c0b960cde0644e4aa2f5e125feb9c97): version 21 holds Comment by s3c5-reviewer
- lost update a8821437-7daa-441d-b155-407d600d75f1 v16 Comment by s2c4-admin 's2c4-admin:22' via 39527 (stress-5c7b6518526848338486121b78a6c6d0): version 16 holds Comment by s3c2-reviewer
- lost update a8821437-7daa-441d-b155-407d600d75f1 v17 Comment by s2c4-admin 's2c4-admin:24' via 39527 (stress-062feb4c556346f48a031363dfcc2f0b): version 17 holds Comment by s3c4-admin
- lost update a8821437-7daa-441d-b155-407d600d75f1 v20 Comment by s2c4-admin 's2c4-admin:28' via 39527 (stress-a588fe919b2e430b8558029e3ee192cc): version 20 holds Comment by s3c2-reviewer
- lost update a8821437-7daa-441d-b155-407d600d75f1 v18 Comment by s2c5-reviewer 's2c5-reviewer:26' via 39527 (stress-213dc88eb34545e39813f9170338eb94): version 18 holds Comment by s3c5-reviewer
- lost update a8821437-7daa-441d-b155-407d600d75f1 v22 Comment by s2c5-reviewer 's2c5-reviewer:39' via 39527 (stress-22069b8b5eac4bc688ecb62fa7bce8d9): no history entry at that version
- lost update a8821437-7daa-441d-b155-407d600d75f1 v2 Submit by s3c4-admin via mac-host.local:33777 (stress-6a5451d755cf47b5a4f0af7eb917aa5d): version 2 holds Submit by s2c4-admin
- lost update 918ec255-ee0c-4797-a8d2-800e829a7691 v1 Create by s2c1-registrant via 39527 (stress-af853b6c300543a88f50616866ec6c61): record missing
- lost update 918ec255-ee0c-4797-a8d2-800e829a7691 v2 Update by s2c1-registrant 's2c1-registrant:0' via 39527 (stress-1d131671d7c444c38da710fb7bad85f3): record missing
- lost update 918ec255-ee0c-4797-a8d2-800e829a7691 v3 Update by s2c1-registrant 's2c1-registrant:1' via 39527 (stress-99754c42a8fc4d7e9b88366e87aba0cb): record missing
- lost update 918ec255-ee0c-4797-a8d2-800e829a7691 v4 Submit by s2c1-registrant via 39527 (stress-c30b4187efd14642ad05a27c52335657): record missing

server 39195:
- missing record 918ec255-ee0c-4797-a8d2-800e829a7691: missing (40 acknowledged changes)
- lost update a8821437-7daa-441d-b155-407d600d75f1 v20 Comment by s1c2-reviewer 's1c2-reviewer:34' via 39195 (stress-cc599f0572bf4ca1976f31f080d46610): version 20 holds Comment by s2c4-admin
- lost update a8821437-7daa-441d-b155-407d600d75f1 v21 Comment by s1c2-reviewer 's1c2-reviewer:38' via 39195 (stress-69b7225a81f34ad9a8b589ae2709d900): version 21 holds Comment by s2c2-reviewer
- lost update a8821437-7daa-441d-b155-407d600d75f1 v16 Comment by s2c4-admin 's2c4-admin:22' via 39527 (stress-5c7b6518526848338486121b78a6c6d0): version 16 holds Comment by s1c5-reviewer
- lost update a8821437-7daa-441d-b155-407d600d75f1 v17 Comment by s2c4-admin 's2c4-admin:24' via 39527 (stress-062feb4c556346f48a031363dfcc2f0b): version 17 holds Comment by s1c4-admin
- lost update a8821437-7daa-441d-b155-407d600d75f1 v18 Comment by s2c5-reviewer 's2c5-reviewer:26' via 39527 (stress-213dc88eb34545e39813f9170338eb94): version 18 holds Comment by s1c4-admin
- lost update a8821437-7daa-441d-b155-407d600d75f1 v15 Comment by s3c2-reviewer 's3c2-reviewer:17' via mac-host.local:33777 (stress-0944f368dd23438799f24da10458dd8c): version 15 holds Comment by s1c5-reviewer
- lost update a8821437-7daa-441d-b155-407d600d75f1 v16 Comment by s3c2-reviewer 's3c2-reviewer:19' via mac-host.local:33777 (stress-f54ce5fe3ca64d51a8069f90b88fb4fa): version 16 holds Comment by s1c5-reviewer
- lost update a8821437-7daa-441d-b155-407d600d75f1 v20 Comment by s3c2-reviewer 's3c2-reviewer:27' via mac-host.local:33777 (stress-649426aaa162407390ce4eb96fee7f7d): version 20 holds Comment by s2c4-admin
- lost update a8821437-7daa-441d-b155-407d600d75f1 v2 Submit by s3c4-admin via mac-host.local:33777 (stress-6a5451d755cf47b5a4f0af7eb917aa5d): version 2 holds Submit by s2c4-admin
- lost update a8821437-7daa-441d-b155-407d600d75f1 v14 Comment by s3c4-admin 's3c4-admin:15' via mac-host.local:33777 (stress-1acf44e935404092b467feb99690f707): version 14 holds Comment by s1c5-reviewer
- lost update a8821437-7daa-441d-b155-407d600d75f1 v17 Comment by s3c4-admin 's3c4-admin:24' via mac-host.local:33777 (stress-f76fee86b11343f293296765a388deb7): version 17 holds Comment by s1c4-admin
- lost update a8821437-7daa-441d-b155-407d600d75f1 v19 Comment by s3c4-admin 's3c4-admin:33' via mac-host.local:33777 (stress-464e85983a524560b01d60a55405fdfd): version 19 holds Comment by s1c2-reviewer
- lost update a8821437-7daa-441d-b155-407d600d75f1 v13 Comment by s3c5-reviewer 's3c5-reviewer:12' via mac-host.local:33777 (stress-66e9b9beedcc43ba8fbdce85b95129a7): version 13 holds Comment by s1c5-reviewer
- lost update a8821437-7daa-441d-b155-407d600d75f1 v18 Comment by s3c5-reviewer 's3c5-reviewer:24' via mac-host.local:33777 (stress-1315309a4fd64287bbb1e9dc3dca6711): version 18 holds Comment by s1c4-admin
- lost update a8821437-7daa-441d-b155-407d600d75f1 v21 Comment by s3c5-reviewer 's3c5-reviewer:34' via mac-host.local:33777 (stress-3db929fc92384a0e87756b35162d64e4): version 21 holds Comment by s2c2-reviewer
- lost update 918ec255-ee0c-4797-a8d2-800e829a7691 v1 Create by s2c1-registrant via 39527 (stress-af853b6c300543a88f50616866ec6c61): record missing
- lost update 918ec255-ee0c-4797-a8d2-800e829a7691 v2 Update by s2c1-registrant 's2c1-registrant:0' via 39527 (stress-1d131671d7c444c38da710fb7bad85f3): record missing
- lost update 918ec255-ee0c-4797-a8d2-800e829a7691 v3 Update by s2c1-registrant 's2c1-registrant:1' via 39527 (stress-99754c42a8fc4d7e9b88366e87aba0cb): record missing
- lost update 918ec255-ee0c-4797-a8d2-800e829a7691 v4 Submit by s2c1-registrant via 39527 (stress-c30b4187efd14642ad05a27c52335657): record missing
- lost update 918ec255-ee0c-4797-a8d2-800e829a7691 v5 Update by s2c1-registrant 's2c1-registrant:3' via 39527 (stress-f535a69366e14b99bbc1b04bed9e847d): record missing

server 39527:
- missing record 22c08914-d98a-4560-b8ba-62f7b25090e5: missing (32 acknowledged changes)
- lost update a8821437-7daa-441d-b155-407d600d75f1 v20 Comment by s1c2-reviewer 's1c2-reviewer:34' via 39195 (stress-cc599f0572bf4ca1976f31f080d46610): version 20 holds Comment by s2c4-admin
- lost update a8821437-7daa-441d-b155-407d600d75f1 v21 Comment by s1c2-reviewer 's1c2-reviewer:38' via 39195 (stress-69b7225a81f34ad9a8b589ae2709d900): version 21 holds Comment by s2c2-reviewer
- lost update a8821437-7daa-441d-b155-407d600d75f1 v16 Comment by s2c4-admin 's2c4-admin:22' via 39527 (stress-5c7b6518526848338486121b78a6c6d0): version 16 holds Comment by s1c5-reviewer
- lost update a8821437-7daa-441d-b155-407d600d75f1 v17 Comment by s2c4-admin 's2c4-admin:24' via 39527 (stress-062feb4c556346f48a031363dfcc2f0b): version 17 holds Comment by s1c4-admin
- lost update a8821437-7daa-441d-b155-407d600d75f1 v18 Comment by s2c5-reviewer 's2c5-reviewer:26' via 39527 (stress-213dc88eb34545e39813f9170338eb94): version 18 holds Comment by s1c4-admin
- lost update a8821437-7daa-441d-b155-407d600d75f1 v15 Comment by s3c2-reviewer 's3c2-reviewer:17' via mac-host.local:33777 (stress-0944f368dd23438799f24da10458dd8c): version 15 holds Comment by s1c5-reviewer
- lost update a8821437-7daa-441d-b155-407d600d75f1 v16 Comment by s3c2-reviewer 's3c2-reviewer:19' via mac-host.local:33777 (stress-f54ce5fe3ca64d51a8069f90b88fb4fa): version 16 holds Comment by s1c5-reviewer
- lost update a8821437-7daa-441d-b155-407d600d75f1 v20 Comment by s3c2-reviewer 's3c2-reviewer:27' via mac-host.local:33777 (stress-649426aaa162407390ce4eb96fee7f7d): version 20 holds Comment by s2c4-admin
- lost update a8821437-7daa-441d-b155-407d600d75f1 v2 Submit by s3c4-admin via mac-host.local:33777 (stress-6a5451d755cf47b5a4f0af7eb917aa5d): version 2 holds Submit by s2c4-admin
- lost update a8821437-7daa-441d-b155-407d600d75f1 v14 Comment by s3c4-admin 's3c4-admin:15' via mac-host.local:33777 (stress-1acf44e935404092b467feb99690f707): version 14 holds Comment by s1c5-reviewer
- lost update a8821437-7daa-441d-b155-407d600d75f1 v17 Comment by s3c4-admin 's3c4-admin:24' via mac-host.local:33777 (stress-f76fee86b11343f293296765a388deb7): version 17 holds Comment by s1c4-admin
- lost update a8821437-7daa-441d-b155-407d600d75f1 v19 Comment by s3c4-admin 's3c4-admin:33' via mac-host.local:33777 (stress-464e85983a524560b01d60a55405fdfd): version 19 holds Comment by s1c2-reviewer
- lost update a8821437-7daa-441d-b155-407d600d75f1 v13 Comment by s3c5-reviewer 's3c5-reviewer:12' via mac-host.local:33777 (stress-66e9b9beedcc43ba8fbdce85b95129a7): version 13 holds Comment by s1c5-reviewer
- lost update a8821437-7daa-441d-b155-407d600d75f1 v18 Comment by s3c5-reviewer 's3c5-reviewer:24' via mac-host.local:33777 (stress-1315309a4fd64287bbb1e9dc3dca6711): version 18 holds Comment by s1c4-admin
- lost update a8821437-7daa-441d-b155-407d600d75f1 v21 Comment by s3c5-reviewer 's3c5-reviewer:34' via mac-host.local:33777 (stress-3db929fc92384a0e87756b35162d64e4): version 21 holds Comment by s2c2-reviewer
- lost update 22c08914-d98a-4560-b8ba-62f7b25090e5 v4 Comment by s1c2-reviewer 's1c2-reviewer:4' via 39195 (stress-cff057906bf04f05bc69d1bd7cce0536): record missing
- lost update 22c08914-d98a-4560-b8ba-62f7b25090e5 v7 Comment by s1c2-reviewer 's1c2-reviewer:12' via 39195 (stress-4a1402a39d3c47b789ec0dfe88e5809d): record missing
- lost update 22c08914-d98a-4560-b8ba-62f7b25090e5 v13 Reject by s1c2-reviewer via 39195 (stress-eda610cb44e245fbac6b2d71918507b0): record missing
- lost update 22c08914-d98a-4560-b8ba-62f7b25090e5 v17 Comment by s1c2-reviewer 's1c2-reviewer:39' via 39195 (stress-27bbb1860a684a9790e0b867e6d0d970): record missing
- lost update 22c08914-d98a-4560-b8ba-62f7b25090e5 v3 Update by s1c4-admin 's1c4-admin:3' via 39195 (stress-fc7b2f09bdfc4a4c8ce1cc7a5ba6db40): record missing

server mac-host.local:33777:
- missing record 918ec255-ee0c-4797-a8d2-800e829a7691: missing (40 acknowledged changes)
- lost update a8821437-7daa-441d-b155-407d600d75f1 v19 Comment by s1c2-reviewer 's1c2-reviewer:33' via 39195 (stress-a4078e7420964cc384bcde9ab04dc683): version 19 holds Comment by s3c4-admin
- lost update a8821437-7daa-441d-b155-407d600d75f1 v20 Comment by s1c2-reviewer 's1c2-reviewer:34' via 39195 (stress-cc599f0572bf4ca1976f31f080d46610): version 20 holds Comment by s3c2-reviewer
- lost update a8821437-7daa-441d-b155-407d600d75f1 v21 Comment by s1c2-reviewer 's1c2-reviewer:38' via 39195 (stress-69b7225a81f34ad9a8b589ae2709d900): version 21 holds Comment by s3c5-reviewer
- lost update a8821437-7daa-441d-b155-407d600d75f1 v17 Comment by s1c4-admin 's1c4-admin:25' via 39195 (stress-d26608e282eb4da2be34d23cb6bc225a): version 17 holds Comment by s3c4-admin
- lost update a8821437-7daa-441d-b155-407d600d75f1 v18 Comment by s1c4-admin 's1c4-admin:30' via 39195 (stress-591a703dd4764e6f8d4a58b0fce14906): version 18 holds Comment by s3c5-reviewer
- lost update a8821437-7daa-441d-b155-407d600d75f1 v13 Comment by s1c5-reviewer 's1c5-reviewer:21' via 39195 (stress-6d1e59a6a3484be99dbcb8a431bf172e): version 13 holds Comment by s3c5-reviewer
- lost update a8821437-7daa-441d-b155-407d600d75f1 v14 Comment by s1c5-reviewer 's1c5-reviewer:22' via 39195 (stress-00ff9189fceb43f9b7447e4a327c1e15): version 14 holds Comment by s3c4-admin
- lost update a8821437-7daa-441d-b155-407d600d75f1 v15 Comment by s1c5-reviewer 's1c5-reviewer:24' via 39195 (stress-e094a85cd18143af918fbbf97d589dc5): version 15 holds Comment by s3c2-reviewer
- lost update a8821437-7daa-441d-b155-407d600d75f1 v16 Comment by s1c5-reviewer 's1c5-reviewer:26' via 39195 (stress-1fab14eb3cfa433888168fcc9a894623): version 16 holds Comment by s3c2-reviewer
- lost update a8821437-7daa-441d-b155-407d600d75f1 v21 Comment by s2c2-reviewer 's2c2-reviewer:31' via 39527 (stress-0c0b960cde0644e4aa2f5e125feb9c97): version 21 holds Comment by s3c5-reviewer
- lost update a8821437-7daa-441d-b155-407d600d75f1 v16 Comment by s2c4-admin 's2c4-admin:22' via 39527 (stress-5c7b6518526848338486121b78a6c6d0): version 16 holds Comment by s3c2-reviewer
- lost update a8821437-7daa-441d-b155-407d600d75f1 v17 Comment by s2c4-admin 's2c4-admin:24' via 39527 (stress-062feb4c556346f48a031363dfcc2f0b): version 17 holds Comment by s3c4-admin
- lost update a8821437-7daa-441d-b155-407d600d75f1 v20 Comment by s2c4-admin 's2c4-admin:28' via 39527 (stress-a588fe919b2e430b8558029e3ee192cc): version 20 holds Comment by s3c2-reviewer
- lost update a8821437-7daa-441d-b155-407d600d75f1 v18 Comment by s2c5-reviewer 's2c5-reviewer:26' via 39527 (stress-213dc88eb34545e39813f9170338eb94): version 18 holds Comment by s3c5-reviewer
- lost update a8821437-7daa-441d-b155-407d600d75f1 v22 Comment by s2c5-reviewer 's2c5-reviewer:39' via 39527 (stress-22069b8b5eac4bc688ecb62fa7bce8d9): no history entry at that version
- lost update a8821437-7daa-441d-b155-407d600d75f1 v2 Submit by s3c4-admin via mac-host.local:33777 (stress-6a5451d755cf47b5a4f0af7eb917aa5d): version 2 holds Submit by s2c4-admin
- lost update 918ec255-ee0c-4797-a8d2-800e829a7691 v1 Create by s2c1-registrant via 39527 (stress-af853b6c300543a88f50616866ec6c61): record missing
- lost update 918ec255-ee0c-4797-a8d2-800e829a7691 v2 Update by s2c1-registrant 's2c1-registrant:0' via 39527 (stress-1d131671d7c444c38da710fb7bad85f3): record missing
- lost update 918ec255-ee0c-4797-a8d2-800e829a7691 v3 Update by s2c1-registrant 's2c1-registrant:1' via 39527 (stress-99754c42a8fc4d7e9b88366e87aba0cb): record missing
- lost update 918ec255-ee0c-4797-a8d2-800e829a7691 v4 Submit by s2c1-registrant via 39527 (stress-c30b4187efd14642ad05a27c52335657): record missing

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| AppendLog | 3 | 256 | 35.7 | 160 | 131 | 133 | 0 | 0 | 239.0 | no |
