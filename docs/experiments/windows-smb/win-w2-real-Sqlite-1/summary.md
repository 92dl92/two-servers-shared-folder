# Stress run: Sqlite, 2 server(s): FAILED

- Commit `fe53fea387646cb0ee65f1cccd5a309abb2291d2`, 2026-10-18 13:23:43 UTC, 129.9 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 5 clients, pipeline with 1000 ms think time, 120 changes per client, seed 1
- Clients by role: 4 Registrant, 4 Reviewer, 2 Admin
- Data folder `stress-runs/win-w2-real-Sqlite-1/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-w2-real-Sqlite-1`
- 1 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-w2-real-Sqlite-1` there
- 1 of the servers on `Administrator@ws2.example`, data folder `\\ws1.example\share\runs\win-w2-real-Sqlite-1` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 604 | 381 | 2.9 | 0 | 0 | 0 | 11.7 | 81.9 | 163.9 |

Writes by outcome: http500 223, saved 381. Saves by action: Create 87, Update 86, Submit 85, Approve 62, Reject 19, Comment 42. Reads: 893 (http500 347, notFound 250).

Stale reads: 250 (older than a version already acknowledged); 250 reads followed a save through another server.
- record edf9d17f-a5a9-4292-b71f-bcb24b139d1f through ws1.example:39163: version 0, 137.5 ms after version 2 was acknowledged by ws2.example:37941
- record c8cef7ca-d80c-47c4-b584-541a4273a9d9 through ws2.example:37941: version 0, 62.1 ms after version 4 was acknowledged by ws1.example:39163
- record c8cef7ca-d80c-47c4-b584-541a4273a9d9 through ws2.example:37941: version 0, 120.5 ms after version 5 was acknowledged by ws1.example:39163
- record 6df2bf49-4d8d-4f39-8bc2-acf13393a118 through ws1.example:39163: version 0, 341.3 ms after version 3 was acknowledged by ws2.example:37941
- record 6df2bf49-4d8d-4f39-8bc2-acf13393a118 through ws1.example:39163: version 0, 613.1 ms after version 3 was acknowledged by ws2.example:37941
- record 6df2bf49-4d8d-4f39-8bc2-acf13393a118 through ws1.example:39163: version 0, 443.0 ms after version 4 was acknowledged by ws2.example:37941
- record cf5efb00-deef-46f9-9f55-81e6260aea3a through ws2.example:37941: version 0, 604.4 ms after version 3 was acknowledged by ws1.example:39163
- record cf5efb00-deef-46f9-9f55-81e6260aea3a through ws2.example:37941: version 0, 1050.2 ms after version 3 was acknowledged by ws1.example:39163
- record 6df2bf49-4d8d-4f39-8bc2-acf13393a118 through ws1.example:39163: version 0, 81.4 ms after version 5 was acknowledged by ws2.example:37941
- record 6df2bf49-4d8d-4f39-8bc2-acf13393a118 through ws1.example:39163: version 0, 99.5 ms after version 5 was acknowledged by ws2.example:37941
- record 6df2bf49-4d8d-4f39-8bc2-acf13393a118 through ws1.example:39163: version 0, 589.5 ms after version 5 was acknowledged by ws2.example:37941
- record 35f3f336-cff0-4087-8b94-5271640faf17 through ws1.example:39163: version 0, 693.1 ms after version 4 was acknowledged by ws2.example:37941
- record cf5efb00-deef-46f9-9f55-81e6260aea3a through ws2.example:37941: version 0, 364.4 ms after version 4 was acknowledged by ws1.example:39163
- record c8cef7ca-d80c-47c4-b584-541a4273a9d9 through ws2.example:37941: version 0, 2691.6 ms after version 6 was acknowledged by ws1.example:39163
- record 35bf320a-45d0-4583-be09-d9f5e79d799e through ws2.example:37941: version 0, 182.8 ms after version 2 was acknowledged by ws1.example:39163
- record c8cef7ca-d80c-47c4-b584-541a4273a9d9 through ws2.example:37941: version 0, 3342.9 ms after version 6 was acknowledged by ws1.example:39163
- record cf5efb00-deef-46f9-9f55-81e6260aea3a through ws2.example:37941: version 0, 1135.3 ms after version 5 was acknowledged by ws1.example:39163
- record cf5efb00-deef-46f9-9f55-81e6260aea3a through ws2.example:37941: version 0, 1223.5 ms after version 5 was acknowledged by ws1.example:39163
- record cf5efb00-deef-46f9-9f55-81e6260aea3a through ws2.example:37941: version 0, 2251.7 ms after version 5 was acknowledged by ws1.example:39163
- record c8cef7ca-d80c-47c4-b584-541a4273a9d9 through ws2.example:37941: version 0, 4900.2 ms after version 6 was acknowledged by ws1.example:39163

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | FAIL | 2/87 | 85 | 376 | 0 | 0 | 0 |
| server ws1.example:39163 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server ws2.example:37941 | FAIL | 80/87 | 7 | 22 | 0 | 0 | 0 |

storage:
- missing record 11a50ab8-9ae0-4f16-b762-5a9c5c713f83: missing (4 acknowledged changes)
- missing record e5ee4a07-0583-4618-9f7a-b32ced6c9ab2: missing (6 acknowledged changes)
- missing record 912cfc80-b900-4a92-bd67-49841d613e54: missing (7 acknowledged changes)
- missing record 2afc4c12-7c4e-4def-aed1-705475b405a1: missing (5 acknowledged changes)
- missing record 8e30fcd7-948e-4f76-b5e2-0d6d8231679b: missing (4 acknowledged changes)
- missing record ede76efb-8ed5-45a7-90b8-2de8234d57f6: missing (5 acknowledged changes)
- missing record 44fdc775-5af2-4640-9ec3-f34590a6ff21: missing (7 acknowledged changes)
- missing record 80571125-1000-417b-8f2f-d20a9e5a4e80: missing (5 acknowledged changes)
- missing record ec089bc8-dd0c-44a8-bd56-7e718b7ba9c2: missing (5 acknowledged changes)
- missing record e1cf7af7-6fe6-4384-929a-268019e70c57: missing (4 acknowledged changes)
- missing record fcc537e4-2b62-4903-824f-bea5d4e5957f: missing (4 acknowledged changes)
- missing record 6a9ba3c3-d35c-46f5-87a5-c77184132775: missing (5 acknowledged changes)
- missing record d92e4517-6844-4b73-820c-76369f37262d: missing (5 acknowledged changes)
- missing record bad37004-7e9d-4a99-90df-3c026da4c14b: missing (4 acknowledged changes)
- missing record 544ff0ce-3915-45f9-a4af-8fdc15c2509a: missing (3 acknowledged changes)
- missing record 47a7fdc2-14e1-4070-a59b-765746fe66e0: missing (3 acknowledged changes)
- missing record b4b0de4f-4026-4209-a734-da44b917e949: missing (4 acknowledged changes)
- missing record f68c0407-7aa5-4daa-99e9-8929b016c203: missing (5 acknowledged changes)
- missing record 4616ba5b-fcd3-4e09-abae-3b359935639b: missing (5 acknowledged changes)
- missing record 5dcda361-1edf-4936-85d9-a6e65936e7ea: missing (6 acknowledged changes)
- lost update 11a50ab8-9ae0-4f16-b762-5a9c5c713f83 v1 Create by s2c3-registrant via ws2.example:37941 (stress-6baf425c0b684fdab0572055deaa935b): record missing
- lost update 11a50ab8-9ae0-4f16-b762-5a9c5c713f83 v2 Update by s2c3-registrant 's2c3-registrant:118' via ws2.example:37941 (stress-5d89b4f047ad453391a20d0a601d6a40): record missing
- lost update 11a50ab8-9ae0-4f16-b762-5a9c5c713f83 v3 Submit by s2c3-registrant via ws2.example:37941 (stress-e83e88a7ae81455194d79781b14b1bd0): record missing
- lost update 11a50ab8-9ae0-4f16-b762-5a9c5c713f83 v4 Approve by s2c4-admin via ws2.example:37941 (stress-d63ea285e5074e408fbab3226f283e97): record missing
- lost update e5ee4a07-0583-4618-9f7a-b32ced6c9ab2 v5 Comment by s2c2-reviewer 's2c2-reviewer:74' via ws2.example:37941 (stress-552a20dc9d7b4f608c330b146b8183f7): record missing
- lost update e5ee4a07-0583-4618-9f7a-b32ced6c9ab2 v1 Create by s2c3-registrant via ws2.example:37941 (stress-2c76d8ee3c7d43bf85521b73d93522a6): record missing
- lost update e5ee4a07-0583-4618-9f7a-b32ced6c9ab2 v2 Update by s2c3-registrant 's2c3-registrant:71' via ws2.example:37941 (stress-375ae2d009164a37b4e256c047360084): record missing
- lost update e5ee4a07-0583-4618-9f7a-b32ced6c9ab2 v3 Update by s2c3-registrant 's2c3-registrant:72' via ws2.example:37941 (stress-63f9f6e95db6470f9797fa2d94f472c9): record missing
- lost update e5ee4a07-0583-4618-9f7a-b32ced6c9ab2 v4 Submit by s2c3-registrant via ws2.example:37941 (stress-32188345cf5d497eb0881a8503c06974): record missing
- lost update e5ee4a07-0583-4618-9f7a-b32ced6c9ab2 v6 Approve by s2c4-admin via ws2.example:37941 (stress-e1feb70be5f94adeab89a6303b4141f0): record missing
- lost update 912cfc80-b900-4a92-bd67-49841d613e54 v1 Create by s2c1-registrant via ws2.example:37941 (stress-4a6921a9a557470cbd2605090dba4943): record missing
- lost update 912cfc80-b900-4a92-bd67-49841d613e54 v2 Update by s2c1-registrant 's2c1-registrant:107' via ws2.example:37941 (stress-d8e02eace3c647f2969ab3e6db756f02): record missing
- lost update 912cfc80-b900-4a92-bd67-49841d613e54 v3 Update by s2c1-registrant 's2c1-registrant:108' via ws2.example:37941 (stress-76c00b7146ed431e9190f59ddadce7d6): record missing
- lost update 912cfc80-b900-4a92-bd67-49841d613e54 v4 Submit by s2c1-registrant via ws2.example:37941 (stress-08b8a30c197a409f87e5525bb7276137): record missing
- lost update 912cfc80-b900-4a92-bd67-49841d613e54 v5 Comment by s2c2-reviewer 's2c2-reviewer:108' via ws2.example:37941 (stress-00f4402807c24536b52a1e4c589aa219): record missing
- lost update 912cfc80-b900-4a92-bd67-49841d613e54 v7 Reject by s2c2-reviewer via ws2.example:37941 (stress-9503eb8a2db54e8fb1f9dc08b4a15ca1): record missing
- lost update 912cfc80-b900-4a92-bd67-49841d613e54 v6 Comment by s2c4-admin 's2c4-admin:101' via ws2.example:37941 (stress-412b295172134a88b48c6ee8368d6d38): record missing
- lost update 2afc4c12-7c4e-4def-aed1-705475b405a1 v1 Create by s2c3-registrant via ws2.example:37941 (stress-6ea2245f26904419951bff97b5ab84bd): record missing
- lost update 2afc4c12-7c4e-4def-aed1-705475b405a1 v2 Update by s2c3-registrant 's2c3-registrant:98' via ws2.example:37941 (stress-ab5573a84ae94e41966952b09574a0da): record missing
- lost update 2afc4c12-7c4e-4def-aed1-705475b405a1 v3 Update by s2c3-registrant 's2c3-registrant:99' via ws2.example:37941 (stress-7a8624fe784c469c93153b60d2795709): record missing

server ws1.example:39163:
- unreadable: listing records answered 500

server ws2.example:37941:
- missing record 5f890a8e-f106-4327-9798-155014247665: missing (1 acknowledged changes)
- missing record e10b366d-89f0-4b95-891a-2cb7c218878c: missing (3 acknowledged changes)
- missing record 35bf320a-45d0-4583-be09-d9f5e79d799e: missing (2 acknowledged changes)
- missing record c8cef7ca-d80c-47c4-b584-541a4273a9d9: missing (6 acknowledged changes)
- missing record 2c6c1d26-c951-484f-abc2-99fba48a6f84: missing (2 acknowledged changes)
- missing record cf5efb00-deef-46f9-9f55-81e6260aea3a: missing (5 acknowledged changes)
- missing record 3d5cb261-fb68-448d-9182-e909217b1b7e: missing (3 acknowledged changes)
- lost update 5f890a8e-f106-4327-9798-155014247665 v1 Create by s1c1-registrant via ws1.example:39163 (stress-51f3b43c4850470bb69a884f94166d16): record missing
- lost update e10b366d-89f0-4b95-891a-2cb7c218878c v1 Create by s1c3-registrant via ws1.example:39163 (stress-8283d1c554964c7193809ed29cbb67bc): record missing
- lost update e10b366d-89f0-4b95-891a-2cb7c218878c v2 Submit by s1c3-registrant via ws1.example:39163 (stress-24a52842f9134c71ab596f9d161a60dc): record missing
- lost update e10b366d-89f0-4b95-891a-2cb7c218878c v3 Approve by s1c4-admin via ws1.example:39163 (stress-90c3e788c37b4c10afa1a484220bc2ac): record missing
- lost update 35bf320a-45d0-4583-be09-d9f5e79d799e v1 Create by s1c1-registrant via ws1.example:39163 (stress-2967f52053604f95bf2b08b753051bff): record missing
- lost update 35bf320a-45d0-4583-be09-d9f5e79d799e v2 Submit by s1c1-registrant via ws1.example:39163 (stress-1487e3adf021435f91b09e8653e51a20): record missing
- lost update c8cef7ca-d80c-47c4-b584-541a4273a9d9 v5 Comment by s1c2-reviewer 's1c2-reviewer:2' via ws1.example:39163 (stress-e9a272f8ee954caeb8b304b08ba18321): record missing
- lost update c8cef7ca-d80c-47c4-b584-541a4273a9d9 v1 Create by s1c3-registrant via ws1.example:39163 (stress-e65ba70b9cde49e49bfe316b0fa9b760): record missing
- lost update c8cef7ca-d80c-47c4-b584-541a4273a9d9 v2 Update by s1c3-registrant 's1c3-registrant:2' via ws1.example:39163 (stress-bfa6d8ae4cdf4a5a9ceab29b71167fed): record missing
- lost update c8cef7ca-d80c-47c4-b584-541a4273a9d9 v3 Update by s1c3-registrant 's1c3-registrant:3' via ws1.example:39163 (stress-10b821e267e84183a0e474627e06784d): record missing
- lost update c8cef7ca-d80c-47c4-b584-541a4273a9d9 v4 Submit by s1c3-registrant via ws1.example:39163 (stress-13827240d21944999ed44621555e2523): record missing
- lost update c8cef7ca-d80c-47c4-b584-541a4273a9d9 v6 Comment by s1c4-admin 's1c4-admin:1' via ws1.example:39163 (stress-c6c6495927e843c28a89d0d41079638e): record missing
- lost update 2c6c1d26-c951-484f-abc2-99fba48a6f84 v1 Create by s1c3-registrant via ws1.example:39163 (stress-a637e1cdf1b9406f9ac201b2246ac5d5): record missing
- lost update 2c6c1d26-c951-484f-abc2-99fba48a6f84 v2 Update by s1c3-registrant 's1c3-registrant:8' via ws1.example:39163 (stress-4e034005d8044f68a5e8a1ccc36f03d1): record missing
- lost update cf5efb00-deef-46f9-9f55-81e6260aea3a v1 Create by s1c1-registrant via ws1.example:39163 (stress-e8bed47ef8de4239887250f4ea40f3a2): record missing
- lost update cf5efb00-deef-46f9-9f55-81e6260aea3a v2 Update by s1c1-registrant 's1c1-registrant:4' via ws1.example:39163 (stress-61f24f2ac8704f2abdb26c5a27e14dc8): record missing
- lost update cf5efb00-deef-46f9-9f55-81e6260aea3a v3 Submit by s1c1-registrant via ws1.example:39163 (stress-6c76e7f8cfd3415391a297074ddd95a3): record missing
- lost update cf5efb00-deef-46f9-9f55-81e6260aea3a v5 Comment by s1c2-reviewer 's1c2-reviewer:6' via ws1.example:39163 (stress-14b282bf1aea42aca6a607ee040bbd34): record missing
- lost update cf5efb00-deef-46f9-9f55-81e6260aea3a v4 Comment by s1c5-reviewer 's1c5-reviewer:4' via ws1.example:39163 (stress-cca3ba87a67f48d48cc77e8090ceb172): record missing
- lost update 3d5cb261-fb68-448d-9182-e909217b1b7e v1 Create by s1c1-registrant via ws1.example:39163 (stress-b304dd4458394da7a8ca93f2a8c57a0e): record missing

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| Sqlite | 2 | 381 | 2.9 | 0 | 0 | 376 | 1 | 0 | 81.9 | no |
