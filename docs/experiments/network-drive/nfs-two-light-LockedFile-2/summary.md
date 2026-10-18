# Stress run: LockedFile, 3 server(s): FAILED

- Commit `1b49659b6d0d95e1a10858057ef4082c27040edb`, 2026-10-17 22:18:56 UTC, 8.6 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 5 clients, 6 records, 40 changes per client, seed 2
- Clients by role: 6 Registrant, 6 Reviewer, 3 Admin
- Data folder `/mnt/share/runs/nfs-two-light-LockedFile-2/data`; run folder `stress-runs/nfs/nfs-two-light-LockedFile-2`
- 1 of the servers on `user@mac.example`, data folder `/Volumes/share/runs/nfs-two-light-LockedFile-2/data` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 371 | 140 | 16.3 | 131 | 80 | 0 | 118.2 | 302.9 | 712.3 |

Writes by outcome: conflict 131, http500 12, notFound 8, saved 140, wrongStatus 80. Saves by action: Create 6, Update 27, Submit 5, Approve 4, Reject 1, Comment 97. Reads: 600 (http500 1, notFound 234).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | FAIL | 5/6 | 1 | 28 | 0 | 6 | 0 |
| server 36005 | FAIL | 0/6 | 6 | 140 | 0 | 0 | 0 |
| server 39229 | FAIL | 0/6 | 6 | 140 | 0 | 0 | 0 |
| server mac-host.local:37043 | FAIL | 5/6 | 1 | 28 | 0 | 6 | 0 |

storage:
- missing record e31e185c-4669-4157-abeb-7c110f97d324: missing (1 acknowledged changes)
- lost update 3bf0f31e-667e-4669-ae9e-557785faec20 v2 Update by s1c1-registrant 's1c1-registrant:0' via 36005 (stress-91574162cd6d48608b54e2b4bec69b29): version 2 holds Comment by s3c5-reviewer
- lost update 3bf0f31e-667e-4669-ae9e-557785faec20 v26 Comment by s1c2-reviewer 's1c2-reviewer:38' via 36005 (stress-79413984e8014010863f2500d82ee0ab): no history entry at that version
- lost update 3bf0f31e-667e-4669-ae9e-557785faec20 v25 Comment by s2c4-admin 's2c4-admin:33' via 39229 (stress-71846752fc8f4abeb851b98c3c2af989): version 25 holds Comment by s3c5-reviewer
- lost update 3bf0f31e-667e-4669-ae9e-557785faec20 v26 Comment by s3c2-reviewer 's3c2-reviewer:39' via mac-host.local:37043 (stress-24b5449bceb84e9f91e29c60910f9ae5): no history entry at that version
- lost update 3bf0f31e-667e-4669-ae9e-557785faec20 v26 Comment by s3c4-admin 's3c4-admin:36' via mac-host.local:37043 (stress-5e058c7c797a41218e5a62436e835d2e): no history entry at that version
- lost update 27af98f9-5971-4352-a86c-084f6074cec0 v2 Comment by s1c2-reviewer 's1c2-reviewer:0' via 36005 (stress-23c9b838b9ef4df488988c8789fea0a3): version 2 holds Submit by s2c1-registrant
- lost update 27af98f9-5971-4352-a86c-084f6074cec0 v2 Comment by s1c4-admin 's1c4-admin:1' via 36005 (stress-9b723c49d3f8428597d23a6ac1371b94): version 2 holds Submit by s2c1-registrant
- lost update 27af98f9-5971-4352-a86c-084f6074cec0 v24 Comment by s2c2-reviewer 's2c2-reviewer:36' via 39229 (stress-10734114f7544f73b917435faa098b25): version 24 holds Comment by s3c5-reviewer
- lost update 27af98f9-5971-4352-a86c-084f6074cec0 v24 Comment by s3c5-reviewer 's3c5-reviewer:29' via mac-host.local:37043 (stress-4540ae8295a74770947cceb85629dcca): version 24 holds Comment by s3c5-reviewer
- lost update e31e185c-4669-4157-abeb-7c110f97d324 v1 Create by s3c1-registrant via mac-host.local:37043 (stress-d8c725e3c8304f7b8150d7453a1d7b9c): record missing
- lost update 8f28c976-2974-4961-998c-34d181d3c18a v22 Comment by s2c5-reviewer 's2c5-reviewer:33' via 39229 (stress-bdc2a483df154ab499b0b39adf9060c9): version 22 holds Comment by s3c5-reviewer
- lost update 8f28c976-2974-4961-998c-34d181d3c18a v23 Comment by s2c5-reviewer 's2c5-reviewer:35' via 39229 (stress-33421ef1fe274bc0b8b139ccb172dbae): version 23 holds Comment by s3c5-reviewer
- lost update 8f28c976-2974-4961-998c-34d181d3c18a v24 Comment by s2c5-reviewer 's2c5-reviewer:37' via 39229 (stress-62a43014532845688a701284af6e6a0f): no history entry at that version
- lost update 8f28c976-2974-4961-998c-34d181d3c18a v22 Comment by s3c2-reviewer 's3c2-reviewer:37' via mac-host.local:37043 (stress-4a40401c9ac1432cb5091363a40b9bd9): version 22 holds Comment by s3c5-reviewer
- lost update 8f28c976-2974-4961-998c-34d181d3c18a v5 Comment by s3c4-admin 's3c4-admin:2' via mac-host.local:37043 (stress-a47bbb582da44c759a2d27e4e58832ca): version 5 holds Comment by s1c5-reviewer
- lost update 2b0be573-aa03-42cd-9089-9f57f71537d0 v25 Comment by s1c4-admin 's1c4-admin:30' via 36005 (stress-8772f91e26ad42bdab921db2e70926ef): version 25 holds Comment by s3c5-reviewer
- lost update 2b0be573-aa03-42cd-9089-9f57f71537d0 v2 Update by s2c3-registrant 's2c3-registrant:2' via 39229 (stress-33744df9db2f40e7a4851a7ec08844d4): version 2 holds Update by s2c3-registrant
- lost update 2b0be573-aa03-42cd-9089-9f57f71537d0 v3 Update by s2c4-admin 's2c4-admin:1' via 39229 (stress-86cd103bc7f542febd09ceaa5d8b7b38): version 3 holds Comment by s2c2-reviewer
- lost update 2b0be573-aa03-42cd-9089-9f57f71537d0 v25 Comment by s3c2-reviewer 's3c2-reviewer:36' via mac-host.local:37043 (stress-b8043ccfdaaa4795b77540cd9d95d3bc): version 25 holds Comment by s3c5-reviewer
- lost update 2b0be573-aa03-42cd-9089-9f57f71537d0 v4 Comment by s3c4-admin 's3c4-admin:1' via mac-host.local:37043 (stress-be9d2dbbdd8e45e19f0e52269c59e2b7): version 4 holds Update by s2c3-registrant
- unacknowledged 3bf0f31e-667e-4669-ae9e-557785faec20 v25: Comment by s3c5-reviewer
- unacknowledged 27af98f9-5971-4352-a86c-084f6074cec0 v24: Comment by s3c5-reviewer
- unacknowledged 8f28c976-2974-4961-998c-34d181d3c18a v2: Comment by s3c2-reviewer
- unacknowledged 2b0be573-aa03-42cd-9089-9f57f71537d0 v26: Comment by s3c5-reviewer
- unacknowledged 2b0be573-aa03-42cd-9089-9f57f71537d0 v27: Comment by s3c5-reviewer
- unacknowledged 505c4cd3-02a8-423a-a3ca-a6ac4f75163c v18: Comment by s3c4-admin

server 36005:
- missing record 3bf0f31e-667e-4669-ae9e-557785faec20: missing (29 acknowledged changes)
- missing record 27af98f9-5971-4352-a86c-084f6074cec0: missing (27 acknowledged changes)
- missing record e31e185c-4669-4157-abeb-7c110f97d324: missing (1 acknowledged changes)
- missing record 8f28c976-2974-4961-998c-34d181d3c18a: missing (27 acknowledged changes)
- missing record 2b0be573-aa03-42cd-9089-9f57f71537d0: missing (34 acknowledged changes)
- missing record 505c4cd3-02a8-423a-a3ca-a6ac4f75163c: missing (22 acknowledged changes)
- lost update 3bf0f31e-667e-4669-ae9e-557785faec20 v1 Create by s1c1-registrant via 36005 (stress-af9ad17a1d7a418abb8711ab4a23c845): record missing
- lost update 3bf0f31e-667e-4669-ae9e-557785faec20 v2 Update by s1c1-registrant 's1c1-registrant:0' via 36005 (stress-91574162cd6d48608b54e2b4bec69b29): record missing
- lost update 3bf0f31e-667e-4669-ae9e-557785faec20 v3 Update by s1c1-registrant 's1c1-registrant:2' via 36005 (stress-a35ea350ad35475ebdbb7ae16a63f156): record missing
- lost update 3bf0f31e-667e-4669-ae9e-557785faec20 v6 Update by s1c1-registrant 's1c1-registrant:4' via 36005 (stress-f337dc760f93472084451bbc2ac16825): record missing
- lost update 3bf0f31e-667e-4669-ae9e-557785faec20 v7 Update by s1c1-registrant 's1c1-registrant:5' via 36005 (stress-80e7d8e9da2541b3a65d299f147e0166): record missing
- lost update 3bf0f31e-667e-4669-ae9e-557785faec20 v9 Submit by s1c1-registrant via 36005 (stress-cb92f64d20264b7faff2e4a2fdb651d7): record missing
- lost update 3bf0f31e-667e-4669-ae9e-557785faec20 v10 Update by s1c1-registrant 's1c1-registrant:8' via 36005 (stress-0e53ec2183fb49938f97a6f78706abf8): record missing
- lost update 3bf0f31e-667e-4669-ae9e-557785faec20 v12 Update by s1c1-registrant 's1c1-registrant:11' via 36005 (stress-95233121914e4b9f8a55330561d7096f): record missing
- lost update 3bf0f31e-667e-4669-ae9e-557785faec20 v13 Update by s1c1-registrant 's1c1-registrant:12' via 36005 (stress-c540c26456fb4ded9e3bbd52ba699454): record missing
- lost update 3bf0f31e-667e-4669-ae9e-557785faec20 v14 Update by s1c1-registrant 's1c1-registrant:13' via 36005 (stress-151dabf550d148abae91332c13daf579): record missing
- lost update 3bf0f31e-667e-4669-ae9e-557785faec20 v15 Update by s1c1-registrant 's1c1-registrant:14' via 36005 (stress-e13f6e07af47475ebf9d5853d8cb5de0): record missing
- lost update 3bf0f31e-667e-4669-ae9e-557785faec20 v8 Comment by s1c2-reviewer 's1c2-reviewer:6' via 36005 (stress-0fc50bf327dc42268d44abaa93b08ee1): record missing
- lost update 3bf0f31e-667e-4669-ae9e-557785faec20 v23 Comment by s1c2-reviewer 's1c2-reviewer:31' via 36005 (stress-9be1671e2a6041a38786e12087aa344b): record missing
- lost update 3bf0f31e-667e-4669-ae9e-557785faec20 v26 Comment by s1c2-reviewer 's1c2-reviewer:38' via 36005 (stress-79413984e8014010863f2500d82ee0ab): record missing
- lost update 3bf0f31e-667e-4669-ae9e-557785faec20 v5 Comment by s1c4-admin 's1c4-admin:6' via 36005 (stress-8975bea70c114e199f7e04f4ee36e6e1): record missing
- lost update 3bf0f31e-667e-4669-ae9e-557785faec20 v16 Reject by s1c4-admin via 36005 (stress-9a03d8a10f5e4edbabffb10d3c692143): record missing
- lost update 3bf0f31e-667e-4669-ae9e-557785faec20 v20 Comment by s1c5-reviewer 's1c5-reviewer:16' via 36005 (stress-c744ea1a649c4f2193e3125a8091bd09): record missing
- lost update 3bf0f31e-667e-4669-ae9e-557785faec20 v21 Comment by s1c5-reviewer 's1c5-reviewer:20' via 36005 (stress-617b395316f54e46a9809f72f3f5a437): record missing
- lost update 3bf0f31e-667e-4669-ae9e-557785faec20 v17 Comment by s2c2-reviewer 's2c2-reviewer:16' via 39229 (stress-bc0a970fdcf34644af28aed031a09c9f): record missing
- lost update 3bf0f31e-667e-4669-ae9e-557785faec20 v11 Update by s2c4-admin 's2c4-admin:11' via 39229 (stress-67a352b0fd9c4617a3899b5f86b39fbb): record missing

server 39229:
- missing record 3bf0f31e-667e-4669-ae9e-557785faec20: missing (29 acknowledged changes)
- missing record 27af98f9-5971-4352-a86c-084f6074cec0: missing (27 acknowledged changes)
- missing record e31e185c-4669-4157-abeb-7c110f97d324: missing (1 acknowledged changes)
- missing record 8f28c976-2974-4961-998c-34d181d3c18a: missing (27 acknowledged changes)
- missing record 2b0be573-aa03-42cd-9089-9f57f71537d0: missing (34 acknowledged changes)
- missing record 505c4cd3-02a8-423a-a3ca-a6ac4f75163c: missing (22 acknowledged changes)
- lost update 3bf0f31e-667e-4669-ae9e-557785faec20 v1 Create by s1c1-registrant via 36005 (stress-af9ad17a1d7a418abb8711ab4a23c845): record missing
- lost update 3bf0f31e-667e-4669-ae9e-557785faec20 v2 Update by s1c1-registrant 's1c1-registrant:0' via 36005 (stress-91574162cd6d48608b54e2b4bec69b29): record missing
- lost update 3bf0f31e-667e-4669-ae9e-557785faec20 v3 Update by s1c1-registrant 's1c1-registrant:2' via 36005 (stress-a35ea350ad35475ebdbb7ae16a63f156): record missing
- lost update 3bf0f31e-667e-4669-ae9e-557785faec20 v6 Update by s1c1-registrant 's1c1-registrant:4' via 36005 (stress-f337dc760f93472084451bbc2ac16825): record missing
- lost update 3bf0f31e-667e-4669-ae9e-557785faec20 v7 Update by s1c1-registrant 's1c1-registrant:5' via 36005 (stress-80e7d8e9da2541b3a65d299f147e0166): record missing
- lost update 3bf0f31e-667e-4669-ae9e-557785faec20 v9 Submit by s1c1-registrant via 36005 (stress-cb92f64d20264b7faff2e4a2fdb651d7): record missing
- lost update 3bf0f31e-667e-4669-ae9e-557785faec20 v10 Update by s1c1-registrant 's1c1-registrant:8' via 36005 (stress-0e53ec2183fb49938f97a6f78706abf8): record missing
- lost update 3bf0f31e-667e-4669-ae9e-557785faec20 v12 Update by s1c1-registrant 's1c1-registrant:11' via 36005 (stress-95233121914e4b9f8a55330561d7096f): record missing
- lost update 3bf0f31e-667e-4669-ae9e-557785faec20 v13 Update by s1c1-registrant 's1c1-registrant:12' via 36005 (stress-c540c26456fb4ded9e3bbd52ba699454): record missing
- lost update 3bf0f31e-667e-4669-ae9e-557785faec20 v14 Update by s1c1-registrant 's1c1-registrant:13' via 36005 (stress-151dabf550d148abae91332c13daf579): record missing
- lost update 3bf0f31e-667e-4669-ae9e-557785faec20 v15 Update by s1c1-registrant 's1c1-registrant:14' via 36005 (stress-e13f6e07af47475ebf9d5853d8cb5de0): record missing
- lost update 3bf0f31e-667e-4669-ae9e-557785faec20 v8 Comment by s1c2-reviewer 's1c2-reviewer:6' via 36005 (stress-0fc50bf327dc42268d44abaa93b08ee1): record missing
- lost update 3bf0f31e-667e-4669-ae9e-557785faec20 v23 Comment by s1c2-reviewer 's1c2-reviewer:31' via 36005 (stress-9be1671e2a6041a38786e12087aa344b): record missing
- lost update 3bf0f31e-667e-4669-ae9e-557785faec20 v26 Comment by s1c2-reviewer 's1c2-reviewer:38' via 36005 (stress-79413984e8014010863f2500d82ee0ab): record missing
- lost update 3bf0f31e-667e-4669-ae9e-557785faec20 v5 Comment by s1c4-admin 's1c4-admin:6' via 36005 (stress-8975bea70c114e199f7e04f4ee36e6e1): record missing
- lost update 3bf0f31e-667e-4669-ae9e-557785faec20 v16 Reject by s1c4-admin via 36005 (stress-9a03d8a10f5e4edbabffb10d3c692143): record missing
- lost update 3bf0f31e-667e-4669-ae9e-557785faec20 v20 Comment by s1c5-reviewer 's1c5-reviewer:16' via 36005 (stress-c744ea1a649c4f2193e3125a8091bd09): record missing
- lost update 3bf0f31e-667e-4669-ae9e-557785faec20 v21 Comment by s1c5-reviewer 's1c5-reviewer:20' via 36005 (stress-617b395316f54e46a9809f72f3f5a437): record missing
- lost update 3bf0f31e-667e-4669-ae9e-557785faec20 v17 Comment by s2c2-reviewer 's2c2-reviewer:16' via 39229 (stress-bc0a970fdcf34644af28aed031a09c9f): record missing
- lost update 3bf0f31e-667e-4669-ae9e-557785faec20 v11 Update by s2c4-admin 's2c4-admin:11' via 39229 (stress-67a352b0fd9c4617a3899b5f86b39fbb): record missing

server mac-host.local:37043:
- missing record e31e185c-4669-4157-abeb-7c110f97d324: missing (1 acknowledged changes)
- lost update 3bf0f31e-667e-4669-ae9e-557785faec20 v2 Update by s1c1-registrant 's1c1-registrant:0' via 36005 (stress-91574162cd6d48608b54e2b4bec69b29): version 2 holds Comment by s3c5-reviewer
- lost update 3bf0f31e-667e-4669-ae9e-557785faec20 v26 Comment by s1c2-reviewer 's1c2-reviewer:38' via 36005 (stress-79413984e8014010863f2500d82ee0ab): no history entry at that version
- lost update 3bf0f31e-667e-4669-ae9e-557785faec20 v25 Comment by s2c4-admin 's2c4-admin:33' via 39229 (stress-71846752fc8f4abeb851b98c3c2af989): version 25 holds Comment by s3c5-reviewer
- lost update 3bf0f31e-667e-4669-ae9e-557785faec20 v26 Comment by s3c2-reviewer 's3c2-reviewer:39' via mac-host.local:37043 (stress-24b5449bceb84e9f91e29c60910f9ae5): no history entry at that version
- lost update 3bf0f31e-667e-4669-ae9e-557785faec20 v26 Comment by s3c4-admin 's3c4-admin:36' via mac-host.local:37043 (stress-5e058c7c797a41218e5a62436e835d2e): no history entry at that version
- lost update 27af98f9-5971-4352-a86c-084f6074cec0 v2 Comment by s1c2-reviewer 's1c2-reviewer:0' via 36005 (stress-23c9b838b9ef4df488988c8789fea0a3): version 2 holds Submit by s2c1-registrant
- lost update 27af98f9-5971-4352-a86c-084f6074cec0 v2 Comment by s1c4-admin 's1c4-admin:1' via 36005 (stress-9b723c49d3f8428597d23a6ac1371b94): version 2 holds Submit by s2c1-registrant
- lost update 27af98f9-5971-4352-a86c-084f6074cec0 v24 Comment by s2c2-reviewer 's2c2-reviewer:36' via 39229 (stress-10734114f7544f73b917435faa098b25): version 24 holds Comment by s3c5-reviewer
- lost update 27af98f9-5971-4352-a86c-084f6074cec0 v24 Comment by s3c5-reviewer 's3c5-reviewer:29' via mac-host.local:37043 (stress-4540ae8295a74770947cceb85629dcca): version 24 holds Comment by s3c5-reviewer
- lost update e31e185c-4669-4157-abeb-7c110f97d324 v1 Create by s3c1-registrant via mac-host.local:37043 (stress-d8c725e3c8304f7b8150d7453a1d7b9c): record missing
- lost update 8f28c976-2974-4961-998c-34d181d3c18a v22 Comment by s2c5-reviewer 's2c5-reviewer:33' via 39229 (stress-bdc2a483df154ab499b0b39adf9060c9): version 22 holds Comment by s3c5-reviewer
- lost update 8f28c976-2974-4961-998c-34d181d3c18a v23 Comment by s2c5-reviewer 's2c5-reviewer:35' via 39229 (stress-33421ef1fe274bc0b8b139ccb172dbae): version 23 holds Comment by s3c5-reviewer
- lost update 8f28c976-2974-4961-998c-34d181d3c18a v24 Comment by s2c5-reviewer 's2c5-reviewer:37' via 39229 (stress-62a43014532845688a701284af6e6a0f): no history entry at that version
- lost update 8f28c976-2974-4961-998c-34d181d3c18a v22 Comment by s3c2-reviewer 's3c2-reviewer:37' via mac-host.local:37043 (stress-4a40401c9ac1432cb5091363a40b9bd9): version 22 holds Comment by s3c5-reviewer
- lost update 8f28c976-2974-4961-998c-34d181d3c18a v5 Comment by s3c4-admin 's3c4-admin:2' via mac-host.local:37043 (stress-a47bbb582da44c759a2d27e4e58832ca): version 5 holds Comment by s1c5-reviewer
- lost update 2b0be573-aa03-42cd-9089-9f57f71537d0 v25 Comment by s1c4-admin 's1c4-admin:30' via 36005 (stress-8772f91e26ad42bdab921db2e70926ef): version 25 holds Comment by s3c5-reviewer
- lost update 2b0be573-aa03-42cd-9089-9f57f71537d0 v2 Update by s2c3-registrant 's2c3-registrant:2' via 39229 (stress-33744df9db2f40e7a4851a7ec08844d4): version 2 holds Update by s2c3-registrant
- lost update 2b0be573-aa03-42cd-9089-9f57f71537d0 v3 Update by s2c4-admin 's2c4-admin:1' via 39229 (stress-86cd103bc7f542febd09ceaa5d8b7b38): version 3 holds Comment by s2c2-reviewer
- lost update 2b0be573-aa03-42cd-9089-9f57f71537d0 v25 Comment by s3c2-reviewer 's3c2-reviewer:36' via mac-host.local:37043 (stress-b8043ccfdaaa4795b77540cd9d95d3bc): version 25 holds Comment by s3c5-reviewer
- lost update 2b0be573-aa03-42cd-9089-9f57f71537d0 v4 Comment by s3c4-admin 's3c4-admin:1' via mac-host.local:37043 (stress-be9d2dbbdd8e45e19f0e52269c59e2b7): version 4 holds Update by s2c3-registrant
- unacknowledged 3bf0f31e-667e-4669-ae9e-557785faec20 v25: Comment by s3c5-reviewer
- unacknowledged 27af98f9-5971-4352-a86c-084f6074cec0 v24: Comment by s3c5-reviewer
- unacknowledged 8f28c976-2974-4961-998c-34d181d3c18a v2: Comment by s3c2-reviewer
- unacknowledged 2b0be573-aa03-42cd-9089-9f57f71537d0 v26: Comment by s3c5-reviewer
- unacknowledged 2b0be573-aa03-42cd-9089-9f57f71537d0 v27: Comment by s3c5-reviewer
- unacknowledged 505c4cd3-02a8-423a-a3ca-a6ac4f75163c v18: Comment by s3c4-admin

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| LockedFile | 3 | 140 | 16.3 | 131 | 80 | 140 | 0 | 0 | 302.9 | no |
