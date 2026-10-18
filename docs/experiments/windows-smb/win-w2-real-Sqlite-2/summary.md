# Stress run: Sqlite, 2 server(s): FAILED

- Commit `fe53fea387646cb0ee65f1cccd5a309abb2291d2`, 2026-10-18 13:26:28 UTC, 127.2 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 5 clients, pipeline with 1000 ms think time, 120 changes per client, seed 2
- Clients by role: 4 Registrant, 4 Reviewer, 2 Admin
- Data folder `stress-runs/win-w2-real-Sqlite-2/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-w2-real-Sqlite-2`
- 1 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-w2-real-Sqlite-2` there
- 1 of the servers on `Administrator@ws2.example`, data folder `\\ws1.example\share\runs\win-w2-real-Sqlite-2` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 734 | 725 | 5.7 | 7 | 0 | 0 | 7.1 | 48.4 | 162.9 |

Writes by outcome: conflict 7, http500 2, saved 725. Saves by action: Create 158, Update 165, Submit 156, Approve 120, Reject 36, Comment 90. Reads: 809 (http500 14, notFound 221).

Stale reads: 221 (older than a version already acknowledged); 220 reads followed a save through another server.
- record 6c2fd68f-17da-4084-a1f0-8006fa5bb34b through ws1.example:35687: version 0, 34.6 ms after version 5 was acknowledged by ws2.example:41333
- record 6c2fd68f-17da-4084-a1f0-8006fa5bb34b through ws1.example:35687: version 0, 110.8 ms after version 6 was acknowledged by ws2.example:41333
- record 6c2fd68f-17da-4084-a1f0-8006fa5bb34b through ws1.example:35687: version 0, 82.6 ms after version 7 was acknowledged by ws2.example:41333
- record 6c2fd68f-17da-4084-a1f0-8006fa5bb34b through ws1.example:35687: version 0, 98.8 ms after version 7 was acknowledged by ws2.example:41333
- record 5faf0a53-7b7c-44be-bd5f-9df16d6906c3 through ws1.example:35687: version 0, 78.2 ms after version 3 was acknowledged by ws2.example:41333
- record 1cc151e7-41a8-48e5-8cc6-969920256823 through ws1.example:35687: version 0, 105.4 ms after version 4 was acknowledged by ws2.example:41333
- record 1cc151e7-41a8-48e5-8cc6-969920256823 through ws1.example:35687: version 0, 25.3 ms after version 5 was acknowledged by ws2.example:41333
- record f1a7390e-9507-4556-a890-b849cf0ff1c4 through ws2.example:41333: version 0, 21.4 ms after version 2 was acknowledged by ws1.example:35687
- record 018998d5-25c1-45e4-ab6c-4c3e489d78de through ws1.example:35687: version 0, 290.0 ms after version 4 was acknowledged by ws2.example:41333
- record 018998d5-25c1-45e4-ab6c-4c3e489d78de through ws1.example:35687: version 0, 294.3 ms after version 4 was acknowledged by ws2.example:41333
- record ea6dc67d-8fc5-4cd2-9b3c-827080f74bbb through ws1.example:35687: version 0, 29.1 ms after version 2 was acknowledged by ws2.example:41333
- record 72e01eaf-33b8-4e8b-9808-72e50c25be42 through ws2.example:41333: version 0, 427.1 ms after version 4 was acknowledged by ws1.example:35687
- record e3c50421-70ec-448a-9878-7367b9404b2d through ws2.example:41333: version 0, 51.5 ms after version 4 was acknowledged by ws1.example:35687
- record df9d94b6-4057-47be-bf06-74fa1fbe30f0 through ws1.example:35687: version 0, 36.6 ms after version 3 was acknowledged by ws2.example:41333
- record a7067697-29f3-4738-91c8-95cd530b6946 through ws1.example:35687: version 0, 332.2 ms after version 3 was acknowledged by ws2.example:41333
- record a7067697-29f3-4738-91c8-95cd530b6946 through ws1.example:35687: version 0, 362.7 ms after version 3 was acknowledged by ws2.example:41333
- record a7067697-29f3-4738-91c8-95cd530b6946 through ws1.example:35687: version 0, 123.9 ms after version 5 was acknowledged by ws2.example:41333
- record a7067697-29f3-4738-91c8-95cd530b6946 through ws1.example:35687: version 0, 144.6 ms after version 5 was acknowledged by ws2.example:41333
- record a7067697-29f3-4738-91c8-95cd530b6946 through ws1.example:35687: version 0, 372.4 ms after version 6 was acknowledged by ws2.example:41333
- record fcc41f93-95c7-48c9-a134-7adb9d8503d3 through ws1.example:35687: version 0, 27.4 ms after version 2 was acknowledged by ws2.example:41333

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | FAIL | 3/158 | 155 | 715 | 0 | 0 | 0 |
| server ws1.example:35687 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |
| server ws2.example:41333 | UNREADABLE | 0/0 | 0 | 0 | 0 | 0 | 0 |

storage:
- missing record 9044230d-e7d0-431d-8b46-d864506ae15b: missing (5 acknowledged changes)
- missing record 4cbd2d14-7b59-42b5-86fe-27cbe5f8cfd4: missing (5 acknowledged changes)
- missing record b43819a1-845e-4992-a838-7b18b6e4d569: missing (6 acknowledged changes)
- missing record 5eadc34f-2151-4f55-be7e-12fa963d2ef1: missing (4 acknowledged changes)
- missing record cb7fac11-8bdd-4789-ac65-98bada963824: missing (4 acknowledged changes)
- missing record 3c5ae9d5-7c59-469c-8a62-10207931bb72: missing (7 acknowledged changes)
- missing record fd039a59-69af-4ab5-9732-8950cad15c00: missing (4 acknowledged changes)
- missing record 7eb13fdd-1a1b-4235-b4ea-c4470a73c18f: missing (4 acknowledged changes)
- missing record 5fd935a6-1ecc-4b5a-b187-fd059a659542: missing (5 acknowledged changes)
- missing record 47327794-c6f6-4ae9-bae7-c0fa176c3c11: missing (5 acknowledged changes)
- missing record 0dd8d176-1f84-44f1-afac-8c68cee41683: missing (6 acknowledged changes)
- missing record 4c1bd8ee-c0b3-4dda-80bc-01fe193ae3a6: missing (5 acknowledged changes)
- missing record c6fe0a18-1ff8-4adc-8e50-f94680242aba: missing (3 acknowledged changes)
- missing record 5564262b-f0a3-424a-bf24-8cbb1bd626ba: missing (7 acknowledged changes)
- missing record a5d8ada7-5c9c-4c34-9a65-212d6b498112: missing (4 acknowledged changes)
- missing record c7544692-efeb-4429-ba34-96bbde617dc5: missing (4 acknowledged changes)
- missing record 796e06d4-0bf8-48c3-ab93-d39972293bf5: missing (5 acknowledged changes)
- missing record efd594c7-9999-4ec8-bdbb-8812e1618b8d: missing (4 acknowledged changes)
- missing record 6e2beff3-91f2-4d15-8691-5bc2246095ad: missing (3 acknowledged changes)
- missing record bfe10585-2534-4af7-8d92-af259f72e59b: missing (6 acknowledged changes)
- lost update 9044230d-e7d0-431d-8b46-d864506ae15b v1 Create by s2c1-registrant via ws2.example:41333 (stress-84d1eb2098c945a184a3c507bf59182b): record missing
- lost update 9044230d-e7d0-431d-8b46-d864506ae15b v2 Update by s2c1-registrant 's2c1-registrant:74' via ws2.example:41333 (stress-f95cf397496240c5b00d13e5602fe18a): record missing
- lost update 9044230d-e7d0-431d-8b46-d864506ae15b v3 Update by s2c1-registrant 's2c1-registrant:75' via ws2.example:41333 (stress-b01b204b3a3d434d98d6c7a8f0c3dd2d): record missing
- lost update 9044230d-e7d0-431d-8b46-d864506ae15b v4 Submit by s2c1-registrant via ws2.example:41333 (stress-674ce78625984bfa9953b33cb2fe87a5): record missing
- lost update 9044230d-e7d0-431d-8b46-d864506ae15b v5 Reject by s2c4-admin via ws2.example:41333 (stress-6077bf552d3d44f9b88dc0a1b9a8f5a2): record missing
- lost update 4cbd2d14-7b59-42b5-86fe-27cbe5f8cfd4 v5 Approve by s2c2-reviewer via ws2.example:41333 (stress-3d00327ec3da40769083f26d457fdea0): record missing
- lost update 4cbd2d14-7b59-42b5-86fe-27cbe5f8cfd4 v1 Create by s2c3-registrant via ws2.example:41333 (stress-3d46bea49dba43248df2e44335fac5ff): record missing
- lost update 4cbd2d14-7b59-42b5-86fe-27cbe5f8cfd4 v2 Update by s2c3-registrant 's2c3-registrant:71' via ws2.example:41333 (stress-a9130918befd46eca6cf3f8aba1b4451): record missing
- lost update 4cbd2d14-7b59-42b5-86fe-27cbe5f8cfd4 v3 Update by s2c3-registrant 's2c3-registrant:72' via ws2.example:41333 (stress-ce19df127a564fc28f829414fd53971f): record missing
- lost update 4cbd2d14-7b59-42b5-86fe-27cbe5f8cfd4 v4 Submit by s2c3-registrant via ws2.example:41333 (stress-b8b602b978624748997fadfd3d79e860): record missing
- lost update b43819a1-845e-4992-a838-7b18b6e4d569 v1 Create by s1c1-registrant via ws1.example:35687 (stress-06350d33a2d94c1683088229a5361731): record missing
- lost update b43819a1-845e-4992-a838-7b18b6e4d569 v2 Update by s1c1-registrant 's1c1-registrant:118' via ws1.example:35687 (stress-0979be5e098a41219255a3a1d0158284): record missing
- lost update b43819a1-845e-4992-a838-7b18b6e4d569 v3 Update by s1c1-registrant 's1c1-registrant:119' via ws1.example:35687 (stress-66355128a4aa470ba697c0432ee2b7b7): record missing
- lost update b43819a1-845e-4992-a838-7b18b6e4d569 v4 Submit by s1c1-registrant via ws1.example:35687 (stress-b2f0871aa11e4e4485f647d1459fabe7): record missing
- lost update b43819a1-845e-4992-a838-7b18b6e4d569 v6 Approve by s1c4-admin via ws1.example:35687 (stress-efe58910e0694c3597b0eb5053519104): record missing
- lost update b43819a1-845e-4992-a838-7b18b6e4d569 v5 Comment by s1c5-reviewer 's1c5-reviewer:79' via ws1.example:35687 (stress-351a12c6d92f4c2ebf823221c5fdefba): record missing
- lost update 5eadc34f-2151-4f55-be7e-12fa963d2ef1 v4 Approve by s2c2-reviewer via ws2.example:41333 (stress-ae8f5959497841109e8515693b4937bf): record missing
- lost update 5eadc34f-2151-4f55-be7e-12fa963d2ef1 v1 Create by s2c3-registrant via ws2.example:41333 (stress-c5340348b3c64021b35d2298834b9bc4): record missing
- lost update 5eadc34f-2151-4f55-be7e-12fa963d2ef1 v2 Update by s2c3-registrant 's2c3-registrant:113' via ws2.example:41333 (stress-7b039cff0ada44a5b2be10b4a66d3cd2): record missing
- lost update 5eadc34f-2151-4f55-be7e-12fa963d2ef1 v3 Submit by s2c3-registrant via ws2.example:41333 (stress-d34f997a2f6e4eeca7757d3335d7ea3d): record missing

server ws1.example:35687:
- unreadable: listing records answered 500

server ws2.example:41333:
- unreadable: reading 25f8ce61-f9cd-462c-9a30-46f1cd7ec942 answered 404

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| Sqlite | 2 | 725 | 5.7 | 7 | 0 | 715 | 2 | 0 | 48.4 | no |
