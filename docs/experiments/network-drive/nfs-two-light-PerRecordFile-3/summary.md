# Stress run: PerRecordFile, 3 server(s): FAILED

- Commit `1b49659b6d0d95e1a10858057ef4082c27040edb`, 2026-10-17 22:19:39 UTC, 6.3 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 5 clients, 6 records, 40 changes per client, seed 3
- Clients by role: 6 Registrant, 6 Reviewer, 3 Admin
- Data folder `/mnt/share/runs/nfs-two-light-PerRecordFile-3/data`; run folder `stress-runs/nfs/nfs-two-light-PerRecordFile-3`
- 1 of the servers on `user@mac.example`, data folder `/Volumes/share/runs/nfs-two-light-PerRecordFile-3/data` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 600 | 264 | 42.2 | 198 | 115 | 0 | 23.5 | 187.9 | 340.3 |

Writes by outcome: conflict 198, http500 23, saved 264, wrongStatus 115. Saves by action: Create 6, Update 104, Submit 8, Approve 4, Reject 3, Comment 139. Reads: 600 (http500 5, notFound 1).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | FAIL | 6/6 | 0 | 23 | 0 | 0 | 0 |
| server 44603 | FAIL | 6/6 | 0 | 23 | 0 | 0 | 0 |
| server 34409 | FAIL | 6/6 | 0 | 23 | 0 | 0 | 0 |
| server mac-host.local:34911 | FAIL | 6/6 | 0 | 23 | 0 | 0 | 0 |

storage:
- lost update e28a6d97-cf9d-4667-b29f-607a571eab7c v2 Submit by s1c1-registrant via 44603 (stress-5cd9802670f24b6fae5af16772129c7c): version 2 holds Submit by s3c4-admin
- lost update e28a6d97-cf9d-4667-b29f-607a571eab7c v3 Update by s1c1-registrant 's1c1-registrant:4' via 44603 (stress-27d0343b9bbc4017bd1b8c65d5db6489): version 3 holds Update by s1c1-registrant
- lost update e28a6d97-cf9d-4667-b29f-607a571eab7c v4 Update by s1c1-registrant 's1c1-registrant:5' via 44603 (stress-6e82af9e1b5541f0a765aa10a2a7593f): version 4 holds Update by s1c1-registrant
- lost update e28a6d97-cf9d-4667-b29f-607a571eab7c v27 Comment by s3c4-admin 's3c4-admin:36' via mac-host.local:34911 (stress-4ddb5966095f4286a81540fed5ffa016): version 27 holds Comment by s3c5-reviewer
- lost update e28a6d97-cf9d-4667-b29f-607a571eab7c v4 Update by s1c1-registrant 's1c1-registrant:2' via 44603 (stress-f1a4779b103d489092c250068eaf7cfb): the form data is from another update (notes 's1c1-registrant:5')
- lost update eb77e3db-6a42-44e1-8057-665c254ca881 v34 Comment by s3c4-admin 's3c4-admin:24' via mac-host.local:34911 (stress-08c0d66288df4f26b61d3a7fa058aaa7): version 34 holds Comment by s3c2-reviewer
- lost update 0c8fc448-3d90-48a8-9479-b941a5e56758 v3 Comment by s1c2-reviewer 's1c2-reviewer:1' via 44603 (stress-4bb6d4cf5a0b478fad56dc80f4d4493d): version 3 holds Comment by s1c5-reviewer
- lost update 0c8fc448-3d90-48a8-9479-b941a5e56758 v2 Comment by s2c4-admin 's2c4-admin:0' via 34409 (stress-53598af05f7b45faacd1252d5729f2ca): version 2 holds Update by s3c1-registrant
- lost update 0c8fc448-3d90-48a8-9479-b941a5e56758 v7 Update by s2c4-admin 's2c4-admin:18' via 34409 (stress-aa9926f50dfc4100a647ceda96c7e422): version 7 holds Update by s3c1-registrant
- lost update 0c8fc448-3d90-48a8-9479-b941a5e56758 v31 Comment by s3c2-reviewer 's3c2-reviewer:32' via mac-host.local:34911 (stress-216dae1139234bac92dbddf21b74bcff): version 31 holds Comment by s3c4-admin
- lost update 0c8fc448-3d90-48a8-9479-b941a5e56758 v26 Comment by s3c5-reviewer 's3c5-reviewer:17' via mac-host.local:34911 (stress-659095bec43d4982a88b07446403945f): version 26 holds Comment by s3c2-reviewer
- lost update 917d4623-71c0-43ba-ab59-dfc345ab2552 v48 Comment by s3c5-reviewer 's3c5-reviewer:14' via mac-host.local:34911 (stress-44524a18bab74ae4a1e9635760476ba2): version 48 holds Comment by s3c2-reviewer
- lost update 917d4623-71c0-43ba-ab59-dfc345ab2552 v52 Comment by s3c5-reviewer 's3c5-reviewer:25' via mac-host.local:34911 (stress-6d12ff9d389b4cbdb54898753a6b1e51): version 52 holds Comment by s3c4-admin
- lost update 917d4623-71c0-43ba-ab59-dfc345ab2552 v53 Comment by s3c5-reviewer 's3c5-reviewer:27' via mac-host.local:34911 (stress-505ab145f9f648a584f23f572048383d): version 53 holds Comment by s3c4-admin
- lost update 7ce4f500-791e-4f3e-a29d-1f5650219218 v20 Approve by s1c2-reviewer via 44603 (stress-2a6c7c034e53493f83d0c5330bbde96a): version 20 holds Update by s2c3-registrant
- lost update 7ce4f500-791e-4f3e-a29d-1f5650219218 v19 Comment by s3c2-reviewer 's3c2-reviewer:7' via mac-host.local:34911 (stress-9be9789ab7204fefb215cef3f301d975): version 19 holds Update by s2c3-registrant
- lost update 7ce4f500-791e-4f3e-a29d-1f5650219218 v45 Comment by s3c2-reviewer 's3c2-reviewer:25' via mac-host.local:34911 (stress-54ddadc1b0964fc59f8fce4106f6d6c3): version 45 holds Update by s3c4-admin
- lost update 7ce4f500-791e-4f3e-a29d-1f5650219218 v19 Comment by s3c5-reviewer 's3c5-reviewer:4' via mac-host.local:34911 (stress-d35cd5beb5e54256990f4cd4653b1e60): version 19 holds Update by s2c3-registrant
- lost update 5af0475c-fdca-425d-8b70-feedf81cf28a v24 Comment by s3c2-reviewer 's3c2-reviewer:21' via mac-host.local:34911 (stress-79f06d5364644ca0a3a84fb53a7082b4): version 24 holds Comment by s3c4-admin
- lost update 5af0475c-fdca-425d-8b70-feedf81cf28a v10 Submit by s3c3-registrant via mac-host.local:34911 (stress-ee0d91198bf34ec29f199f02ad7de6d8): version 10 holds Comment by s1c5-reviewer

server 44603:
- lost update e28a6d97-cf9d-4667-b29f-607a571eab7c v2 Submit by s1c1-registrant via 44603 (stress-5cd9802670f24b6fae5af16772129c7c): version 2 holds Submit by s3c4-admin
- lost update e28a6d97-cf9d-4667-b29f-607a571eab7c v3 Update by s1c1-registrant 's1c1-registrant:4' via 44603 (stress-27d0343b9bbc4017bd1b8c65d5db6489): version 3 holds Update by s1c1-registrant
- lost update e28a6d97-cf9d-4667-b29f-607a571eab7c v4 Update by s1c1-registrant 's1c1-registrant:5' via 44603 (stress-6e82af9e1b5541f0a765aa10a2a7593f): version 4 holds Update by s1c1-registrant
- lost update e28a6d97-cf9d-4667-b29f-607a571eab7c v27 Comment by s3c4-admin 's3c4-admin:36' via mac-host.local:34911 (stress-4ddb5966095f4286a81540fed5ffa016): version 27 holds Comment by s3c5-reviewer
- lost update e28a6d97-cf9d-4667-b29f-607a571eab7c v4 Update by s1c1-registrant 's1c1-registrant:2' via 44603 (stress-f1a4779b103d489092c250068eaf7cfb): the form data is from another update (notes 's1c1-registrant:5')
- lost update eb77e3db-6a42-44e1-8057-665c254ca881 v34 Comment by s3c4-admin 's3c4-admin:24' via mac-host.local:34911 (stress-08c0d66288df4f26b61d3a7fa058aaa7): version 34 holds Comment by s3c2-reviewer
- lost update 0c8fc448-3d90-48a8-9479-b941a5e56758 v3 Comment by s1c2-reviewer 's1c2-reviewer:1' via 44603 (stress-4bb6d4cf5a0b478fad56dc80f4d4493d): version 3 holds Comment by s1c5-reviewer
- lost update 0c8fc448-3d90-48a8-9479-b941a5e56758 v2 Comment by s2c4-admin 's2c4-admin:0' via 34409 (stress-53598af05f7b45faacd1252d5729f2ca): version 2 holds Update by s3c1-registrant
- lost update 0c8fc448-3d90-48a8-9479-b941a5e56758 v7 Update by s2c4-admin 's2c4-admin:18' via 34409 (stress-aa9926f50dfc4100a647ceda96c7e422): version 7 holds Update by s3c1-registrant
- lost update 0c8fc448-3d90-48a8-9479-b941a5e56758 v31 Comment by s3c2-reviewer 's3c2-reviewer:32' via mac-host.local:34911 (stress-216dae1139234bac92dbddf21b74bcff): version 31 holds Comment by s3c4-admin
- lost update 0c8fc448-3d90-48a8-9479-b941a5e56758 v26 Comment by s3c5-reviewer 's3c5-reviewer:17' via mac-host.local:34911 (stress-659095bec43d4982a88b07446403945f): version 26 holds Comment by s3c2-reviewer
- lost update 917d4623-71c0-43ba-ab59-dfc345ab2552 v48 Comment by s3c5-reviewer 's3c5-reviewer:14' via mac-host.local:34911 (stress-44524a18bab74ae4a1e9635760476ba2): version 48 holds Comment by s3c2-reviewer
- lost update 917d4623-71c0-43ba-ab59-dfc345ab2552 v52 Comment by s3c5-reviewer 's3c5-reviewer:25' via mac-host.local:34911 (stress-6d12ff9d389b4cbdb54898753a6b1e51): version 52 holds Comment by s3c4-admin
- lost update 917d4623-71c0-43ba-ab59-dfc345ab2552 v53 Comment by s3c5-reviewer 's3c5-reviewer:27' via mac-host.local:34911 (stress-505ab145f9f648a584f23f572048383d): version 53 holds Comment by s3c4-admin
- lost update 7ce4f500-791e-4f3e-a29d-1f5650219218 v20 Approve by s1c2-reviewer via 44603 (stress-2a6c7c034e53493f83d0c5330bbde96a): version 20 holds Update by s2c3-registrant
- lost update 7ce4f500-791e-4f3e-a29d-1f5650219218 v19 Comment by s3c2-reviewer 's3c2-reviewer:7' via mac-host.local:34911 (stress-9be9789ab7204fefb215cef3f301d975): version 19 holds Update by s2c3-registrant
- lost update 7ce4f500-791e-4f3e-a29d-1f5650219218 v45 Comment by s3c2-reviewer 's3c2-reviewer:25' via mac-host.local:34911 (stress-54ddadc1b0964fc59f8fce4106f6d6c3): version 45 holds Update by s3c4-admin
- lost update 7ce4f500-791e-4f3e-a29d-1f5650219218 v19 Comment by s3c5-reviewer 's3c5-reviewer:4' via mac-host.local:34911 (stress-d35cd5beb5e54256990f4cd4653b1e60): version 19 holds Update by s2c3-registrant
- lost update 5af0475c-fdca-425d-8b70-feedf81cf28a v24 Comment by s3c2-reviewer 's3c2-reviewer:21' via mac-host.local:34911 (stress-79f06d5364644ca0a3a84fb53a7082b4): version 24 holds Comment by s3c4-admin
- lost update 5af0475c-fdca-425d-8b70-feedf81cf28a v10 Submit by s3c3-registrant via mac-host.local:34911 (stress-ee0d91198bf34ec29f199f02ad7de6d8): version 10 holds Comment by s1c5-reviewer

server 34409:
- lost update e28a6d97-cf9d-4667-b29f-607a571eab7c v2 Submit by s1c1-registrant via 44603 (stress-5cd9802670f24b6fae5af16772129c7c): version 2 holds Submit by s3c4-admin
- lost update e28a6d97-cf9d-4667-b29f-607a571eab7c v3 Update by s1c1-registrant 's1c1-registrant:4' via 44603 (stress-27d0343b9bbc4017bd1b8c65d5db6489): version 3 holds Update by s1c1-registrant
- lost update e28a6d97-cf9d-4667-b29f-607a571eab7c v4 Update by s1c1-registrant 's1c1-registrant:5' via 44603 (stress-6e82af9e1b5541f0a765aa10a2a7593f): version 4 holds Update by s1c1-registrant
- lost update e28a6d97-cf9d-4667-b29f-607a571eab7c v27 Comment by s3c4-admin 's3c4-admin:36' via mac-host.local:34911 (stress-4ddb5966095f4286a81540fed5ffa016): version 27 holds Comment by s3c5-reviewer
- lost update e28a6d97-cf9d-4667-b29f-607a571eab7c v4 Update by s1c1-registrant 's1c1-registrant:2' via 44603 (stress-f1a4779b103d489092c250068eaf7cfb): the form data is from another update (notes 's1c1-registrant:5')
- lost update eb77e3db-6a42-44e1-8057-665c254ca881 v34 Comment by s3c4-admin 's3c4-admin:24' via mac-host.local:34911 (stress-08c0d66288df4f26b61d3a7fa058aaa7): version 34 holds Comment by s3c2-reviewer
- lost update 0c8fc448-3d90-48a8-9479-b941a5e56758 v3 Comment by s1c2-reviewer 's1c2-reviewer:1' via 44603 (stress-4bb6d4cf5a0b478fad56dc80f4d4493d): version 3 holds Comment by s1c5-reviewer
- lost update 0c8fc448-3d90-48a8-9479-b941a5e56758 v2 Comment by s2c4-admin 's2c4-admin:0' via 34409 (stress-53598af05f7b45faacd1252d5729f2ca): version 2 holds Update by s3c1-registrant
- lost update 0c8fc448-3d90-48a8-9479-b941a5e56758 v7 Update by s2c4-admin 's2c4-admin:18' via 34409 (stress-aa9926f50dfc4100a647ceda96c7e422): version 7 holds Update by s3c1-registrant
- lost update 0c8fc448-3d90-48a8-9479-b941a5e56758 v31 Comment by s3c2-reviewer 's3c2-reviewer:32' via mac-host.local:34911 (stress-216dae1139234bac92dbddf21b74bcff): version 31 holds Comment by s3c4-admin
- lost update 0c8fc448-3d90-48a8-9479-b941a5e56758 v26 Comment by s3c5-reviewer 's3c5-reviewer:17' via mac-host.local:34911 (stress-659095bec43d4982a88b07446403945f): version 26 holds Comment by s3c2-reviewer
- lost update 917d4623-71c0-43ba-ab59-dfc345ab2552 v48 Comment by s3c5-reviewer 's3c5-reviewer:14' via mac-host.local:34911 (stress-44524a18bab74ae4a1e9635760476ba2): version 48 holds Comment by s3c2-reviewer
- lost update 917d4623-71c0-43ba-ab59-dfc345ab2552 v52 Comment by s3c5-reviewer 's3c5-reviewer:25' via mac-host.local:34911 (stress-6d12ff9d389b4cbdb54898753a6b1e51): version 52 holds Comment by s3c4-admin
- lost update 917d4623-71c0-43ba-ab59-dfc345ab2552 v53 Comment by s3c5-reviewer 's3c5-reviewer:27' via mac-host.local:34911 (stress-505ab145f9f648a584f23f572048383d): version 53 holds Comment by s3c4-admin
- lost update 7ce4f500-791e-4f3e-a29d-1f5650219218 v20 Approve by s1c2-reviewer via 44603 (stress-2a6c7c034e53493f83d0c5330bbde96a): version 20 holds Update by s2c3-registrant
- lost update 7ce4f500-791e-4f3e-a29d-1f5650219218 v19 Comment by s3c2-reviewer 's3c2-reviewer:7' via mac-host.local:34911 (stress-9be9789ab7204fefb215cef3f301d975): version 19 holds Update by s2c3-registrant
- lost update 7ce4f500-791e-4f3e-a29d-1f5650219218 v45 Comment by s3c2-reviewer 's3c2-reviewer:25' via mac-host.local:34911 (stress-54ddadc1b0964fc59f8fce4106f6d6c3): version 45 holds Update by s3c4-admin
- lost update 7ce4f500-791e-4f3e-a29d-1f5650219218 v19 Comment by s3c5-reviewer 's3c5-reviewer:4' via mac-host.local:34911 (stress-d35cd5beb5e54256990f4cd4653b1e60): version 19 holds Update by s2c3-registrant
- lost update 5af0475c-fdca-425d-8b70-feedf81cf28a v24 Comment by s3c2-reviewer 's3c2-reviewer:21' via mac-host.local:34911 (stress-79f06d5364644ca0a3a84fb53a7082b4): version 24 holds Comment by s3c4-admin
- lost update 5af0475c-fdca-425d-8b70-feedf81cf28a v10 Submit by s3c3-registrant via mac-host.local:34911 (stress-ee0d91198bf34ec29f199f02ad7de6d8): version 10 holds Comment by s1c5-reviewer

server mac-host.local:34911:
- lost update e28a6d97-cf9d-4667-b29f-607a571eab7c v2 Submit by s1c1-registrant via 44603 (stress-5cd9802670f24b6fae5af16772129c7c): version 2 holds Submit by s3c4-admin
- lost update e28a6d97-cf9d-4667-b29f-607a571eab7c v3 Update by s1c1-registrant 's1c1-registrant:4' via 44603 (stress-27d0343b9bbc4017bd1b8c65d5db6489): version 3 holds Update by s1c1-registrant
- lost update e28a6d97-cf9d-4667-b29f-607a571eab7c v4 Update by s1c1-registrant 's1c1-registrant:5' via 44603 (stress-6e82af9e1b5541f0a765aa10a2a7593f): version 4 holds Update by s1c1-registrant
- lost update e28a6d97-cf9d-4667-b29f-607a571eab7c v27 Comment by s3c4-admin 's3c4-admin:36' via mac-host.local:34911 (stress-4ddb5966095f4286a81540fed5ffa016): version 27 holds Comment by s3c5-reviewer
- lost update e28a6d97-cf9d-4667-b29f-607a571eab7c v4 Update by s1c1-registrant 's1c1-registrant:2' via 44603 (stress-f1a4779b103d489092c250068eaf7cfb): the form data is from another update (notes 's1c1-registrant:5')
- lost update eb77e3db-6a42-44e1-8057-665c254ca881 v34 Comment by s3c4-admin 's3c4-admin:24' via mac-host.local:34911 (stress-08c0d66288df4f26b61d3a7fa058aaa7): version 34 holds Comment by s3c2-reviewer
- lost update 0c8fc448-3d90-48a8-9479-b941a5e56758 v3 Comment by s1c2-reviewer 's1c2-reviewer:1' via 44603 (stress-4bb6d4cf5a0b478fad56dc80f4d4493d): version 3 holds Comment by s1c5-reviewer
- lost update 0c8fc448-3d90-48a8-9479-b941a5e56758 v2 Comment by s2c4-admin 's2c4-admin:0' via 34409 (stress-53598af05f7b45faacd1252d5729f2ca): version 2 holds Update by s3c1-registrant
- lost update 0c8fc448-3d90-48a8-9479-b941a5e56758 v7 Update by s2c4-admin 's2c4-admin:18' via 34409 (stress-aa9926f50dfc4100a647ceda96c7e422): version 7 holds Update by s3c1-registrant
- lost update 0c8fc448-3d90-48a8-9479-b941a5e56758 v31 Comment by s3c2-reviewer 's3c2-reviewer:32' via mac-host.local:34911 (stress-216dae1139234bac92dbddf21b74bcff): version 31 holds Comment by s3c4-admin
- lost update 0c8fc448-3d90-48a8-9479-b941a5e56758 v26 Comment by s3c5-reviewer 's3c5-reviewer:17' via mac-host.local:34911 (stress-659095bec43d4982a88b07446403945f): version 26 holds Comment by s3c2-reviewer
- lost update 917d4623-71c0-43ba-ab59-dfc345ab2552 v48 Comment by s3c5-reviewer 's3c5-reviewer:14' via mac-host.local:34911 (stress-44524a18bab74ae4a1e9635760476ba2): version 48 holds Comment by s3c2-reviewer
- lost update 917d4623-71c0-43ba-ab59-dfc345ab2552 v52 Comment by s3c5-reviewer 's3c5-reviewer:25' via mac-host.local:34911 (stress-6d12ff9d389b4cbdb54898753a6b1e51): version 52 holds Comment by s3c4-admin
- lost update 917d4623-71c0-43ba-ab59-dfc345ab2552 v53 Comment by s3c5-reviewer 's3c5-reviewer:27' via mac-host.local:34911 (stress-505ab145f9f648a584f23f572048383d): version 53 holds Comment by s3c4-admin
- lost update 7ce4f500-791e-4f3e-a29d-1f5650219218 v20 Approve by s1c2-reviewer via 44603 (stress-2a6c7c034e53493f83d0c5330bbde96a): version 20 holds Update by s2c3-registrant
- lost update 7ce4f500-791e-4f3e-a29d-1f5650219218 v19 Comment by s3c2-reviewer 's3c2-reviewer:7' via mac-host.local:34911 (stress-9be9789ab7204fefb215cef3f301d975): version 19 holds Update by s2c3-registrant
- lost update 7ce4f500-791e-4f3e-a29d-1f5650219218 v45 Comment by s3c2-reviewer 's3c2-reviewer:25' via mac-host.local:34911 (stress-54ddadc1b0964fc59f8fce4106f6d6c3): version 45 holds Update by s3c4-admin
- lost update 7ce4f500-791e-4f3e-a29d-1f5650219218 v19 Comment by s3c5-reviewer 's3c5-reviewer:4' via mac-host.local:34911 (stress-d35cd5beb5e54256990f4cd4653b1e60): version 19 holds Update by s2c3-registrant
- lost update 5af0475c-fdca-425d-8b70-feedf81cf28a v24 Comment by s3c2-reviewer 's3c2-reviewer:21' via mac-host.local:34911 (stress-79f06d5364644ca0a3a84fb53a7082b4): version 24 holds Comment by s3c4-admin
- lost update 5af0475c-fdca-425d-8b70-feedf81cf28a v10 Submit by s3c3-registrant via mac-host.local:34911 (stress-ee0d91198bf34ec29f199f02ad7de6d8): version 10 holds Comment by s1c5-reviewer

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| PerRecordFile | 3 | 264 | 42.2 | 198 | 115 | 23 | 0 | 0 | 187.9 | no |
