# Stress run: NaiveFile, 3 server(s): FAILED

- Commit `ee6af83bef739621009cd4be91192b48f8df58df`, 2026-10-17 19:08:42 UTC, 1.5 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 3 servers × 8 clients, 12 records, 200 changes per client, seed 3
- Data folder `stress-runs/heavy-NaiveFile-3/data`; run folder `stress-runs/heavy-NaiveFile-3`

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 403 | 124 | 82.8 | 111 | 159 | 0 | 5.2 | 109.5 | 110.1 |

Writes by outcome: conflict 111, http500 9, saved 124, wrongStatus 159. Saves by action: Create 12, Update 6, Submit 1, Reject 1, Comment 104. Reads: 4800 (http500 118, notFound 4291).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | FAIL | 1/12 | 11 | 39 | 0 | 0 | 0 |
| server 38269 | FAIL | 1/12 | 11 | 39 | 0 | 0 | 0 |
| server 35875 | FAIL | 1/12 | 11 | 39 | 0 | 0 | 0 |
| server 34419 | FAIL | 1/12 | 11 | 39 | 0 | 0 | 0 |

storage:
- missing record d9c40e0c-ae6e-4203-b4f5-29ed3602e0c1: missing (1 acknowledged changes)
- missing record 5029a2cd-97c0-4283-be98-05eb45385245: missing (1 acknowledged changes)
- missing record 13f11278-c29f-47bd-a08a-e9d9b8c462d0: missing (1 acknowledged changes)
- missing record 9876ec6c-3895-41db-9f79-58e0b6f1c0e7: missing (1 acknowledged changes)
- missing record 784ccc05-4d2a-43c8-a782-abd0f12a6137: missing (1 acknowledged changes)
- missing record 0ca7b4a2-b061-4b2c-99cd-aa34ad4458b6: missing (1 acknowledged changes)
- missing record d62b778e-0496-4361-af36-1e891446fb87: missing (1 acknowledged changes)
- missing record 5effea12-f574-460b-94da-38aa01292f9a: missing (1 acknowledged changes)
- missing record 930c12a1-3af3-436e-b67e-ceb63712eee4: missing (1 acknowledged changes)
- missing record 84db9155-7b82-46df-8240-a7012177cde3: missing (1 acknowledged changes)
- missing record f78bedb4-071f-4f75-9180-4ac5c676f48e: missing (1 acknowledged changes)
- lost update d9c40e0c-ae6e-4203-b4f5-29ed3602e0c1 v1 Create by s1c1-registrant via 38269 (stress-8d4617f59e4d445fbd1d8549ca311a55): record missing
- lost update 5029a2cd-97c0-4283-be98-05eb45385245 v1 Create by s2c1-registrant via 35875 (stress-7737d18abb014945be20108fd144b7be): record missing
- lost update 13f11278-c29f-47bd-a08a-e9d9b8c462d0 v1 Create by s3c1-registrant via 34419 (stress-6e5e85d57d144bbdaf4406907edc7c12): record missing
- lost update 9876ec6c-3895-41db-9f79-58e0b6f1c0e7 v1 Create by s1c3-registrant via 38269 (stress-428d1f3605b7440bad812185edfc5aac): record missing
- lost update 784ccc05-4d2a-43c8-a782-abd0f12a6137 v1 Create by s2c3-registrant via 35875 (stress-a11b823401da45ed8aecfb0a480748e0): record missing
- lost update 0ca7b4a2-b061-4b2c-99cd-aa34ad4458b6 v1 Create by s3c3-registrant via 34419 (stress-898f1e00bc60487c8186190041a6db7d): record missing
- lost update d62b778e-0496-4361-af36-1e891446fb87 v1 Create by s1c6-registrant via 38269 (stress-4c3a7cadb7b449bc9c4e5a793dce952e): record missing
- lost update 5effea12-f574-460b-94da-38aa01292f9a v1 Create by s2c6-registrant via 35875 (stress-0feb1020ea964143b20c0998b939abd5): record missing
- lost update 930c12a1-3af3-436e-b67e-ceb63712eee4 v1 Create by s3c6-registrant via 34419 (stress-6aee5873658d4d3d929c599032ad9f53): record missing
- lost update 845fe67a-ea22-4c91-aef1-fad26d959550 v7 Comment by s1c2-reviewer 's1c2-reviewer:12' via 38269 (stress-73aadc83e57f45e8b4eb8ca4fccfb769): version 7 holds Comment by s1c5-reviewer
- lost update 845fe67a-ea22-4c91-aef1-fad26d959550 v51 Comment by s1c2-reviewer 's1c2-reviewer:104' via 38269 (stress-2a61d78ed712441ba34da8b03b51c203): version 51 holds Comment by s1c5-reviewer
- lost update 845fe67a-ea22-4c91-aef1-fad26d959550 v63 Comment by s1c2-reviewer 's1c2-reviewer:143' via 38269 (stress-cc1a6dd4a0a84a99826e7675448d4fce): version 63 holds Comment by s2c5-reviewer
- lost update 845fe67a-ea22-4c91-aef1-fad26d959550 v76 Comment by s1c2-reviewer 's1c2-reviewer:193' via 38269 (stress-a97f56b121734974b989c126c2f6d9ca): version 76 holds Comment by s1c4-admin
- lost update 845fe67a-ea22-4c91-aef1-fad26d959550 v49 Comment by s1c4-admin 's1c4-admin:108' via 38269 (stress-6550b2f5cc994159b75e0def1a065e3d): version 49 holds Comment by s2c4-admin
- lost update 845fe67a-ea22-4c91-aef1-fad26d959550 v54 Comment by s1c4-admin 's1c4-admin:120' via 38269 (stress-5822f1123285499e804cff3048807e75): version 54 holds Comment by s3c5-reviewer
- lost update 845fe67a-ea22-4c91-aef1-fad26d959550 v64 Comment by s1c4-admin 's1c4-admin:151' via 38269 (stress-5796dd7d110e417ba5f3c85b4f727be7): version 64 holds Comment by s3c2-reviewer
- lost update 845fe67a-ea22-4c91-aef1-fad26d959550 v20 Comment by s1c7-reviewer 's1c7-reviewer:43' via 38269 (stress-2730805756814686805d93a6a659e27f): version 20 holds Comment by s3c5-reviewer
- lost update 845fe67a-ea22-4c91-aef1-fad26d959550 v31 Comment by s1c7-reviewer 's1c7-reviewer:61' via 38269 (stress-555e97971596456ab8634dc513ec60b4): version 31 holds Comment by s3c4-admin
- lost update 845fe67a-ea22-4c91-aef1-fad26d959550 v40 Comment by s1c7-reviewer 's1c7-reviewer:85' via 38269 (stress-9f277e2a30db46e88787a9c30e89bdda): version 40 holds Comment by s2c7-reviewer
- lost update 845fe67a-ea22-4c91-aef1-fad26d959550 v5 Update by s1c8-registrant 's1c8-registrant:2' via 38269 (stress-d3e3b7bd3a4c48dca48ec5e94f74c689): version 5 holds Comment by s1c4-admin

server 38269:
- missing record d9c40e0c-ae6e-4203-b4f5-29ed3602e0c1: missing (1 acknowledged changes)
- missing record 5029a2cd-97c0-4283-be98-05eb45385245: missing (1 acknowledged changes)
- missing record 13f11278-c29f-47bd-a08a-e9d9b8c462d0: missing (1 acknowledged changes)
- missing record 9876ec6c-3895-41db-9f79-58e0b6f1c0e7: missing (1 acknowledged changes)
- missing record 784ccc05-4d2a-43c8-a782-abd0f12a6137: missing (1 acknowledged changes)
- missing record 0ca7b4a2-b061-4b2c-99cd-aa34ad4458b6: missing (1 acknowledged changes)
- missing record d62b778e-0496-4361-af36-1e891446fb87: missing (1 acknowledged changes)
- missing record 5effea12-f574-460b-94da-38aa01292f9a: missing (1 acknowledged changes)
- missing record 930c12a1-3af3-436e-b67e-ceb63712eee4: missing (1 acknowledged changes)
- missing record 84db9155-7b82-46df-8240-a7012177cde3: missing (1 acknowledged changes)
- missing record f78bedb4-071f-4f75-9180-4ac5c676f48e: missing (1 acknowledged changes)
- lost update d9c40e0c-ae6e-4203-b4f5-29ed3602e0c1 v1 Create by s1c1-registrant via 38269 (stress-8d4617f59e4d445fbd1d8549ca311a55): record missing
- lost update 5029a2cd-97c0-4283-be98-05eb45385245 v1 Create by s2c1-registrant via 35875 (stress-7737d18abb014945be20108fd144b7be): record missing
- lost update 13f11278-c29f-47bd-a08a-e9d9b8c462d0 v1 Create by s3c1-registrant via 34419 (stress-6e5e85d57d144bbdaf4406907edc7c12): record missing
- lost update 9876ec6c-3895-41db-9f79-58e0b6f1c0e7 v1 Create by s1c3-registrant via 38269 (stress-428d1f3605b7440bad812185edfc5aac): record missing
- lost update 784ccc05-4d2a-43c8-a782-abd0f12a6137 v1 Create by s2c3-registrant via 35875 (stress-a11b823401da45ed8aecfb0a480748e0): record missing
- lost update 0ca7b4a2-b061-4b2c-99cd-aa34ad4458b6 v1 Create by s3c3-registrant via 34419 (stress-898f1e00bc60487c8186190041a6db7d): record missing
- lost update d62b778e-0496-4361-af36-1e891446fb87 v1 Create by s1c6-registrant via 38269 (stress-4c3a7cadb7b449bc9c4e5a793dce952e): record missing
- lost update 5effea12-f574-460b-94da-38aa01292f9a v1 Create by s2c6-registrant via 35875 (stress-0feb1020ea964143b20c0998b939abd5): record missing
- lost update 930c12a1-3af3-436e-b67e-ceb63712eee4 v1 Create by s3c6-registrant via 34419 (stress-6aee5873658d4d3d929c599032ad9f53): record missing
- lost update 845fe67a-ea22-4c91-aef1-fad26d959550 v7 Comment by s1c2-reviewer 's1c2-reviewer:12' via 38269 (stress-73aadc83e57f45e8b4eb8ca4fccfb769): version 7 holds Comment by s1c5-reviewer
- lost update 845fe67a-ea22-4c91-aef1-fad26d959550 v51 Comment by s1c2-reviewer 's1c2-reviewer:104' via 38269 (stress-2a61d78ed712441ba34da8b03b51c203): version 51 holds Comment by s1c5-reviewer
- lost update 845fe67a-ea22-4c91-aef1-fad26d959550 v63 Comment by s1c2-reviewer 's1c2-reviewer:143' via 38269 (stress-cc1a6dd4a0a84a99826e7675448d4fce): version 63 holds Comment by s2c5-reviewer
- lost update 845fe67a-ea22-4c91-aef1-fad26d959550 v76 Comment by s1c2-reviewer 's1c2-reviewer:193' via 38269 (stress-a97f56b121734974b989c126c2f6d9ca): version 76 holds Comment by s1c4-admin
- lost update 845fe67a-ea22-4c91-aef1-fad26d959550 v49 Comment by s1c4-admin 's1c4-admin:108' via 38269 (stress-6550b2f5cc994159b75e0def1a065e3d): version 49 holds Comment by s2c4-admin
- lost update 845fe67a-ea22-4c91-aef1-fad26d959550 v54 Comment by s1c4-admin 's1c4-admin:120' via 38269 (stress-5822f1123285499e804cff3048807e75): version 54 holds Comment by s3c5-reviewer
- lost update 845fe67a-ea22-4c91-aef1-fad26d959550 v64 Comment by s1c4-admin 's1c4-admin:151' via 38269 (stress-5796dd7d110e417ba5f3c85b4f727be7): version 64 holds Comment by s3c2-reviewer
- lost update 845fe67a-ea22-4c91-aef1-fad26d959550 v20 Comment by s1c7-reviewer 's1c7-reviewer:43' via 38269 (stress-2730805756814686805d93a6a659e27f): version 20 holds Comment by s3c5-reviewer
- lost update 845fe67a-ea22-4c91-aef1-fad26d959550 v31 Comment by s1c7-reviewer 's1c7-reviewer:61' via 38269 (stress-555e97971596456ab8634dc513ec60b4): version 31 holds Comment by s3c4-admin
- lost update 845fe67a-ea22-4c91-aef1-fad26d959550 v40 Comment by s1c7-reviewer 's1c7-reviewer:85' via 38269 (stress-9f277e2a30db46e88787a9c30e89bdda): version 40 holds Comment by s2c7-reviewer
- lost update 845fe67a-ea22-4c91-aef1-fad26d959550 v5 Update by s1c8-registrant 's1c8-registrant:2' via 38269 (stress-d3e3b7bd3a4c48dca48ec5e94f74c689): version 5 holds Comment by s1c4-admin

server 35875:
- missing record d9c40e0c-ae6e-4203-b4f5-29ed3602e0c1: missing (1 acknowledged changes)
- missing record 5029a2cd-97c0-4283-be98-05eb45385245: missing (1 acknowledged changes)
- missing record 13f11278-c29f-47bd-a08a-e9d9b8c462d0: missing (1 acknowledged changes)
- missing record 9876ec6c-3895-41db-9f79-58e0b6f1c0e7: missing (1 acknowledged changes)
- missing record 784ccc05-4d2a-43c8-a782-abd0f12a6137: missing (1 acknowledged changes)
- missing record 0ca7b4a2-b061-4b2c-99cd-aa34ad4458b6: missing (1 acknowledged changes)
- missing record d62b778e-0496-4361-af36-1e891446fb87: missing (1 acknowledged changes)
- missing record 5effea12-f574-460b-94da-38aa01292f9a: missing (1 acknowledged changes)
- missing record 930c12a1-3af3-436e-b67e-ceb63712eee4: missing (1 acknowledged changes)
- missing record 84db9155-7b82-46df-8240-a7012177cde3: missing (1 acknowledged changes)
- missing record f78bedb4-071f-4f75-9180-4ac5c676f48e: missing (1 acknowledged changes)
- lost update d9c40e0c-ae6e-4203-b4f5-29ed3602e0c1 v1 Create by s1c1-registrant via 38269 (stress-8d4617f59e4d445fbd1d8549ca311a55): record missing
- lost update 5029a2cd-97c0-4283-be98-05eb45385245 v1 Create by s2c1-registrant via 35875 (stress-7737d18abb014945be20108fd144b7be): record missing
- lost update 13f11278-c29f-47bd-a08a-e9d9b8c462d0 v1 Create by s3c1-registrant via 34419 (stress-6e5e85d57d144bbdaf4406907edc7c12): record missing
- lost update 9876ec6c-3895-41db-9f79-58e0b6f1c0e7 v1 Create by s1c3-registrant via 38269 (stress-428d1f3605b7440bad812185edfc5aac): record missing
- lost update 784ccc05-4d2a-43c8-a782-abd0f12a6137 v1 Create by s2c3-registrant via 35875 (stress-a11b823401da45ed8aecfb0a480748e0): record missing
- lost update 0ca7b4a2-b061-4b2c-99cd-aa34ad4458b6 v1 Create by s3c3-registrant via 34419 (stress-898f1e00bc60487c8186190041a6db7d): record missing
- lost update d62b778e-0496-4361-af36-1e891446fb87 v1 Create by s1c6-registrant via 38269 (stress-4c3a7cadb7b449bc9c4e5a793dce952e): record missing
- lost update 5effea12-f574-460b-94da-38aa01292f9a v1 Create by s2c6-registrant via 35875 (stress-0feb1020ea964143b20c0998b939abd5): record missing
- lost update 930c12a1-3af3-436e-b67e-ceb63712eee4 v1 Create by s3c6-registrant via 34419 (stress-6aee5873658d4d3d929c599032ad9f53): record missing
- lost update 845fe67a-ea22-4c91-aef1-fad26d959550 v7 Comment by s1c2-reviewer 's1c2-reviewer:12' via 38269 (stress-73aadc83e57f45e8b4eb8ca4fccfb769): version 7 holds Comment by s1c5-reviewer
- lost update 845fe67a-ea22-4c91-aef1-fad26d959550 v51 Comment by s1c2-reviewer 's1c2-reviewer:104' via 38269 (stress-2a61d78ed712441ba34da8b03b51c203): version 51 holds Comment by s1c5-reviewer
- lost update 845fe67a-ea22-4c91-aef1-fad26d959550 v63 Comment by s1c2-reviewer 's1c2-reviewer:143' via 38269 (stress-cc1a6dd4a0a84a99826e7675448d4fce): version 63 holds Comment by s2c5-reviewer
- lost update 845fe67a-ea22-4c91-aef1-fad26d959550 v76 Comment by s1c2-reviewer 's1c2-reviewer:193' via 38269 (stress-a97f56b121734974b989c126c2f6d9ca): version 76 holds Comment by s1c4-admin
- lost update 845fe67a-ea22-4c91-aef1-fad26d959550 v49 Comment by s1c4-admin 's1c4-admin:108' via 38269 (stress-6550b2f5cc994159b75e0def1a065e3d): version 49 holds Comment by s2c4-admin
- lost update 845fe67a-ea22-4c91-aef1-fad26d959550 v54 Comment by s1c4-admin 's1c4-admin:120' via 38269 (stress-5822f1123285499e804cff3048807e75): version 54 holds Comment by s3c5-reviewer
- lost update 845fe67a-ea22-4c91-aef1-fad26d959550 v64 Comment by s1c4-admin 's1c4-admin:151' via 38269 (stress-5796dd7d110e417ba5f3c85b4f727be7): version 64 holds Comment by s3c2-reviewer
- lost update 845fe67a-ea22-4c91-aef1-fad26d959550 v20 Comment by s1c7-reviewer 's1c7-reviewer:43' via 38269 (stress-2730805756814686805d93a6a659e27f): version 20 holds Comment by s3c5-reviewer
- lost update 845fe67a-ea22-4c91-aef1-fad26d959550 v31 Comment by s1c7-reviewer 's1c7-reviewer:61' via 38269 (stress-555e97971596456ab8634dc513ec60b4): version 31 holds Comment by s3c4-admin
- lost update 845fe67a-ea22-4c91-aef1-fad26d959550 v40 Comment by s1c7-reviewer 's1c7-reviewer:85' via 38269 (stress-9f277e2a30db46e88787a9c30e89bdda): version 40 holds Comment by s2c7-reviewer
- lost update 845fe67a-ea22-4c91-aef1-fad26d959550 v5 Update by s1c8-registrant 's1c8-registrant:2' via 38269 (stress-d3e3b7bd3a4c48dca48ec5e94f74c689): version 5 holds Comment by s1c4-admin

server 34419:
- missing record d9c40e0c-ae6e-4203-b4f5-29ed3602e0c1: missing (1 acknowledged changes)
- missing record 5029a2cd-97c0-4283-be98-05eb45385245: missing (1 acknowledged changes)
- missing record 13f11278-c29f-47bd-a08a-e9d9b8c462d0: missing (1 acknowledged changes)
- missing record 9876ec6c-3895-41db-9f79-58e0b6f1c0e7: missing (1 acknowledged changes)
- missing record 784ccc05-4d2a-43c8-a782-abd0f12a6137: missing (1 acknowledged changes)
- missing record 0ca7b4a2-b061-4b2c-99cd-aa34ad4458b6: missing (1 acknowledged changes)
- missing record d62b778e-0496-4361-af36-1e891446fb87: missing (1 acknowledged changes)
- missing record 5effea12-f574-460b-94da-38aa01292f9a: missing (1 acknowledged changes)
- missing record 930c12a1-3af3-436e-b67e-ceb63712eee4: missing (1 acknowledged changes)
- missing record 84db9155-7b82-46df-8240-a7012177cde3: missing (1 acknowledged changes)
- missing record f78bedb4-071f-4f75-9180-4ac5c676f48e: missing (1 acknowledged changes)
- lost update d9c40e0c-ae6e-4203-b4f5-29ed3602e0c1 v1 Create by s1c1-registrant via 38269 (stress-8d4617f59e4d445fbd1d8549ca311a55): record missing
- lost update 5029a2cd-97c0-4283-be98-05eb45385245 v1 Create by s2c1-registrant via 35875 (stress-7737d18abb014945be20108fd144b7be): record missing
- lost update 13f11278-c29f-47bd-a08a-e9d9b8c462d0 v1 Create by s3c1-registrant via 34419 (stress-6e5e85d57d144bbdaf4406907edc7c12): record missing
- lost update 9876ec6c-3895-41db-9f79-58e0b6f1c0e7 v1 Create by s1c3-registrant via 38269 (stress-428d1f3605b7440bad812185edfc5aac): record missing
- lost update 784ccc05-4d2a-43c8-a782-abd0f12a6137 v1 Create by s2c3-registrant via 35875 (stress-a11b823401da45ed8aecfb0a480748e0): record missing
- lost update 0ca7b4a2-b061-4b2c-99cd-aa34ad4458b6 v1 Create by s3c3-registrant via 34419 (stress-898f1e00bc60487c8186190041a6db7d): record missing
- lost update d62b778e-0496-4361-af36-1e891446fb87 v1 Create by s1c6-registrant via 38269 (stress-4c3a7cadb7b449bc9c4e5a793dce952e): record missing
- lost update 5effea12-f574-460b-94da-38aa01292f9a v1 Create by s2c6-registrant via 35875 (stress-0feb1020ea964143b20c0998b939abd5): record missing
- lost update 930c12a1-3af3-436e-b67e-ceb63712eee4 v1 Create by s3c6-registrant via 34419 (stress-6aee5873658d4d3d929c599032ad9f53): record missing
- lost update 845fe67a-ea22-4c91-aef1-fad26d959550 v7 Comment by s1c2-reviewer 's1c2-reviewer:12' via 38269 (stress-73aadc83e57f45e8b4eb8ca4fccfb769): version 7 holds Comment by s1c5-reviewer
- lost update 845fe67a-ea22-4c91-aef1-fad26d959550 v51 Comment by s1c2-reviewer 's1c2-reviewer:104' via 38269 (stress-2a61d78ed712441ba34da8b03b51c203): version 51 holds Comment by s1c5-reviewer
- lost update 845fe67a-ea22-4c91-aef1-fad26d959550 v63 Comment by s1c2-reviewer 's1c2-reviewer:143' via 38269 (stress-cc1a6dd4a0a84a99826e7675448d4fce): version 63 holds Comment by s2c5-reviewer
- lost update 845fe67a-ea22-4c91-aef1-fad26d959550 v76 Comment by s1c2-reviewer 's1c2-reviewer:193' via 38269 (stress-a97f56b121734974b989c126c2f6d9ca): version 76 holds Comment by s1c4-admin
- lost update 845fe67a-ea22-4c91-aef1-fad26d959550 v49 Comment by s1c4-admin 's1c4-admin:108' via 38269 (stress-6550b2f5cc994159b75e0def1a065e3d): version 49 holds Comment by s2c4-admin
- lost update 845fe67a-ea22-4c91-aef1-fad26d959550 v54 Comment by s1c4-admin 's1c4-admin:120' via 38269 (stress-5822f1123285499e804cff3048807e75): version 54 holds Comment by s3c5-reviewer
- lost update 845fe67a-ea22-4c91-aef1-fad26d959550 v64 Comment by s1c4-admin 's1c4-admin:151' via 38269 (stress-5796dd7d110e417ba5f3c85b4f727be7): version 64 holds Comment by s3c2-reviewer
- lost update 845fe67a-ea22-4c91-aef1-fad26d959550 v20 Comment by s1c7-reviewer 's1c7-reviewer:43' via 38269 (stress-2730805756814686805d93a6a659e27f): version 20 holds Comment by s3c5-reviewer
- lost update 845fe67a-ea22-4c91-aef1-fad26d959550 v31 Comment by s1c7-reviewer 's1c7-reviewer:61' via 38269 (stress-555e97971596456ab8634dc513ec60b4): version 31 holds Comment by s3c4-admin
- lost update 845fe67a-ea22-4c91-aef1-fad26d959550 v40 Comment by s1c7-reviewer 's1c7-reviewer:85' via 38269 (stress-9f277e2a30db46e88787a9c30e89bdda): version 40 holds Comment by s2c7-reviewer
- lost update 845fe67a-ea22-4c91-aef1-fad26d959550 v5 Update by s1c8-registrant 's1c8-registrant:2' via 38269 (stress-d3e3b7bd3a4c48dca48ec5e94f74c689): version 5 holds Comment by s1c4-admin

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 3 | 124 | 82.8 | 111 | 159 | 39 | 0 | 0 | 109.5 | no |
