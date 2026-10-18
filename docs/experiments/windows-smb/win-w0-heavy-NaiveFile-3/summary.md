# Stress run: NaiveFile, 2 server(s): FAILED

- Commit `c71df76c37c18ca38a44e27c02961e268fbb1c09`, 2026-10-18 11:45:24 UTC, 8.2 s
- Machine linux-host: Debian GNU/Linux 12 (bookworm), 16 CPUs, .NET 10.0.12
- Settings: 2 servers × 12 clients, 12 records, 200 changes per client, seed 3
- Clients by role: 10 Registrant, 10 Reviewer, 4 Admin
- Data folder `stress-runs/win-w0-heavy-NaiveFile-3/data` (a copy of the shared folder, made on `Administrator@ws1.example` after the servers stopped); run folder `stress-runs/win-w0-heavy-NaiveFile-3`
- 2 of the servers on `Administrator@ws1.example`, data folder `C:\share\runs\win-w0-heavy-NaiveFile-3` there

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 2201 | 232 | 28.5 | 531 | 337 | 0 | 17.4 | 57.0 | 226.8 |

Writes by outcome: conflict 531, http500 1101, saved 232, wrongStatus 337. Saves by action: Create 3, Update 3, Submit 2, Reject 2, Comment 222. Reads: 3400 (http500 118, notFound 1093).

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | FAIL | 2/3 | 1 | 28 | 0 | 0 | 0 |
| server ws1.example:41135 | FAIL | 2/3 | 1 | 28 | 0 | 0 | 0 |
| server ws1.example:33537 | FAIL | 2/3 | 1 | 28 | 0 | 0 | 0 |

storage:
- missing record aeb0d708-5d58-4fa8-a9a7-6b749d1f208b: missing (1 acknowledged changes)
- lost update aeb0d708-5d58-4fa8-a9a7-6b749d1f208b v1 Create by s1c1-registrant via ws1.example:41135 (stress-68f5345dcb0a498e96b6a5ebf8151aa9): record missing
- lost update 915b4288-196f-4188-ba6d-2bcda50ae295 v105 Comment by s1c2-reviewer 's1c2-reviewer:198' via ws1.example:41135 (stress-8ab31d8ba86a411a8450a8458a0a377d): version 105 holds Comment by s1c7-reviewer
- lost update 915b4288-196f-4188-ba6d-2bcda50ae295 v17 Comment by s1c5-reviewer 's1c5-reviewer:24' via ws1.example:41135 (stress-8f4567085bbd47a4855702171df9a6cb): version 17 holds Comment by s1c5-reviewer
- lost update 915b4288-196f-4188-ba6d-2bcda50ae295 v37 Comment by s1c7-reviewer 's1c7-reviewer:57' via ws1.example:41135 (stress-923a99755fc5420799bc8ff269581033): version 37 holds Comment by s1c5-reviewer
- lost update 915b4288-196f-4188-ba6d-2bcda50ae295 v70 Comment by s1c9-admin 's1c9-admin:122' via ws1.example:41135 (stress-c723b77641af4aa9b46bca1c6e8a73ad): version 70 holds Comment by s1c7-reviewer
- lost update 915b4288-196f-4188-ba6d-2bcda50ae295 v82 Comment by s1c9-admin 's1c9-admin:150' via ws1.example:41135 (stress-9f6cdaeda1804680a229b4160464bfc8): version 82 holds Comment by s2c2-reviewer
- lost update 915b4288-196f-4188-ba6d-2bcda50ae295 v41 Comment by s1c12-reviewer 's1c12-reviewer:83' via ws1.example:41135 (stress-e68da712680c46d08d05fb917227749e): version 41 holds Comment by s1c12-reviewer
- lost update 915b4288-196f-4188-ba6d-2bcda50ae295 v74 Comment by s2c7-reviewer 's2c7-reviewer:142' via ws1.example:33537 (stress-df2217a238fd49aa86263ea67db2d22e): version 74 holds Comment by s2c9-admin
- lost update 915b4288-196f-4188-ba6d-2bcda50ae295 v92 Comment by s2c7-reviewer 's2c7-reviewer:175' via ws1.example:33537 (stress-a3548f5205fc4174902f20973f9fa3b6): version 92 holds Comment by s1c7-reviewer
- lost update 915b4288-196f-4188-ba6d-2bcda50ae295 v40 Comment by s2c12-reviewer 's2c12-reviewer:68' via ws1.example:33537 (stress-019a2b3a700f463ca49ad380dd4b7926): version 40 holds Comment by s2c10-reviewer
- lost update 44d47101-ffcb-476f-8b3a-5f93aea71b68 v23 Comment by s1c4-admin 's1c4-admin:48' via ws1.example:41135 (stress-2aeafc46a14d41a78467df321de335a5): version 23 holds Comment by s2c9-admin
- lost update 44d47101-ffcb-476f-8b3a-5f93aea71b68 v63 Comment by s1c5-reviewer 's1c5-reviewer:154' via ws1.example:41135 (stress-fc6d017878624b128bf3752db5f91796): version 63 holds Comment by s2c12-reviewer
- lost update 44d47101-ffcb-476f-8b3a-5f93aea71b68 v70 Comment by s1c5-reviewer 's1c5-reviewer:172' via ws1.example:41135 (stress-0575f794b1d14e9c82f3765567040b53): version 70 holds Comment by s2c4-admin
- lost update 44d47101-ffcb-476f-8b3a-5f93aea71b68 v33 Comment by s1c7-reviewer 's1c7-reviewer:60' via ws1.example:41135 (stress-f39ae3e9ede643eb88c7f23e79746a05): version 33 holds Comment by s2c12-reviewer
- lost update 44d47101-ffcb-476f-8b3a-5f93aea71b68 v47 Comment by s1c7-reviewer 's1c7-reviewer:88' via ws1.example:41135 (stress-ef72ef20933f46d4bf5438a5ca406992): version 47 holds Comment by s1c5-reviewer
- lost update 44d47101-ffcb-476f-8b3a-5f93aea71b68 v56 Comment by s1c7-reviewer 's1c7-reviewer:112' via ws1.example:41135 (stress-a3e1230f515f486c829fdb0e249ad00e): version 56 holds Comment by s1c2-reviewer
- lost update 44d47101-ffcb-476f-8b3a-5f93aea71b68 v29 Comment by s1c9-admin 's1c9-admin:62' via ws1.example:41135 (stress-1610ca5b7fb84bb19b54872cea416fe2): version 29 holds Comment by s1c2-reviewer
- lost update 44d47101-ffcb-476f-8b3a-5f93aea71b68 v50 Comment by s1c9-admin 's1c9-admin:100' via ws1.example:41135 (stress-2453d0b359b04ba2897feda68b65e7c2): version 50 holds Comment by s2c4-admin
- lost update 44d47101-ffcb-476f-8b3a-5f93aea71b68 v59 Comment by s1c9-admin 's1c9-admin:118' via ws1.example:41135 (stress-2cd273e3e2d64c738bdc90a786d7b2b2): version 59 holds Comment by s2c9-admin
- lost update 44d47101-ffcb-476f-8b3a-5f93aea71b68 v43 Comment by s1c10-reviewer 's1c10-reviewer:98' via ws1.example:41135 (stress-9607356f4db5461094ee8fe9665bc412): version 43 holds Comment by s1c9-admin

server ws1.example:41135:
- missing record aeb0d708-5d58-4fa8-a9a7-6b749d1f208b: missing (1 acknowledged changes)
- lost update aeb0d708-5d58-4fa8-a9a7-6b749d1f208b v1 Create by s1c1-registrant via ws1.example:41135 (stress-68f5345dcb0a498e96b6a5ebf8151aa9): record missing
- lost update 915b4288-196f-4188-ba6d-2bcda50ae295 v105 Comment by s1c2-reviewer 's1c2-reviewer:198' via ws1.example:41135 (stress-8ab31d8ba86a411a8450a8458a0a377d): version 105 holds Comment by s1c7-reviewer
- lost update 915b4288-196f-4188-ba6d-2bcda50ae295 v17 Comment by s1c5-reviewer 's1c5-reviewer:24' via ws1.example:41135 (stress-8f4567085bbd47a4855702171df9a6cb): version 17 holds Comment by s1c5-reviewer
- lost update 915b4288-196f-4188-ba6d-2bcda50ae295 v37 Comment by s1c7-reviewer 's1c7-reviewer:57' via ws1.example:41135 (stress-923a99755fc5420799bc8ff269581033): version 37 holds Comment by s1c5-reviewer
- lost update 915b4288-196f-4188-ba6d-2bcda50ae295 v70 Comment by s1c9-admin 's1c9-admin:122' via ws1.example:41135 (stress-c723b77641af4aa9b46bca1c6e8a73ad): version 70 holds Comment by s1c7-reviewer
- lost update 915b4288-196f-4188-ba6d-2bcda50ae295 v82 Comment by s1c9-admin 's1c9-admin:150' via ws1.example:41135 (stress-9f6cdaeda1804680a229b4160464bfc8): version 82 holds Comment by s2c2-reviewer
- lost update 915b4288-196f-4188-ba6d-2bcda50ae295 v41 Comment by s1c12-reviewer 's1c12-reviewer:83' via ws1.example:41135 (stress-e68da712680c46d08d05fb917227749e): version 41 holds Comment by s1c12-reviewer
- lost update 915b4288-196f-4188-ba6d-2bcda50ae295 v74 Comment by s2c7-reviewer 's2c7-reviewer:142' via ws1.example:33537 (stress-df2217a238fd49aa86263ea67db2d22e): version 74 holds Comment by s2c9-admin
- lost update 915b4288-196f-4188-ba6d-2bcda50ae295 v92 Comment by s2c7-reviewer 's2c7-reviewer:175' via ws1.example:33537 (stress-a3548f5205fc4174902f20973f9fa3b6): version 92 holds Comment by s1c7-reviewer
- lost update 915b4288-196f-4188-ba6d-2bcda50ae295 v40 Comment by s2c12-reviewer 's2c12-reviewer:68' via ws1.example:33537 (stress-019a2b3a700f463ca49ad380dd4b7926): version 40 holds Comment by s2c10-reviewer
- lost update 44d47101-ffcb-476f-8b3a-5f93aea71b68 v23 Comment by s1c4-admin 's1c4-admin:48' via ws1.example:41135 (stress-2aeafc46a14d41a78467df321de335a5): version 23 holds Comment by s2c9-admin
- lost update 44d47101-ffcb-476f-8b3a-5f93aea71b68 v63 Comment by s1c5-reviewer 's1c5-reviewer:154' via ws1.example:41135 (stress-fc6d017878624b128bf3752db5f91796): version 63 holds Comment by s2c12-reviewer
- lost update 44d47101-ffcb-476f-8b3a-5f93aea71b68 v70 Comment by s1c5-reviewer 's1c5-reviewer:172' via ws1.example:41135 (stress-0575f794b1d14e9c82f3765567040b53): version 70 holds Comment by s2c4-admin
- lost update 44d47101-ffcb-476f-8b3a-5f93aea71b68 v33 Comment by s1c7-reviewer 's1c7-reviewer:60' via ws1.example:41135 (stress-f39ae3e9ede643eb88c7f23e79746a05): version 33 holds Comment by s2c12-reviewer
- lost update 44d47101-ffcb-476f-8b3a-5f93aea71b68 v47 Comment by s1c7-reviewer 's1c7-reviewer:88' via ws1.example:41135 (stress-ef72ef20933f46d4bf5438a5ca406992): version 47 holds Comment by s1c5-reviewer
- lost update 44d47101-ffcb-476f-8b3a-5f93aea71b68 v56 Comment by s1c7-reviewer 's1c7-reviewer:112' via ws1.example:41135 (stress-a3e1230f515f486c829fdb0e249ad00e): version 56 holds Comment by s1c2-reviewer
- lost update 44d47101-ffcb-476f-8b3a-5f93aea71b68 v29 Comment by s1c9-admin 's1c9-admin:62' via ws1.example:41135 (stress-1610ca5b7fb84bb19b54872cea416fe2): version 29 holds Comment by s1c2-reviewer
- lost update 44d47101-ffcb-476f-8b3a-5f93aea71b68 v50 Comment by s1c9-admin 's1c9-admin:100' via ws1.example:41135 (stress-2453d0b359b04ba2897feda68b65e7c2): version 50 holds Comment by s2c4-admin
- lost update 44d47101-ffcb-476f-8b3a-5f93aea71b68 v59 Comment by s1c9-admin 's1c9-admin:118' via ws1.example:41135 (stress-2cd273e3e2d64c738bdc90a786d7b2b2): version 59 holds Comment by s2c9-admin
- lost update 44d47101-ffcb-476f-8b3a-5f93aea71b68 v43 Comment by s1c10-reviewer 's1c10-reviewer:98' via ws1.example:41135 (stress-9607356f4db5461094ee8fe9665bc412): version 43 holds Comment by s1c9-admin

server ws1.example:33537:
- missing record aeb0d708-5d58-4fa8-a9a7-6b749d1f208b: missing (1 acknowledged changes)
- lost update aeb0d708-5d58-4fa8-a9a7-6b749d1f208b v1 Create by s1c1-registrant via ws1.example:41135 (stress-68f5345dcb0a498e96b6a5ebf8151aa9): record missing
- lost update 915b4288-196f-4188-ba6d-2bcda50ae295 v105 Comment by s1c2-reviewer 's1c2-reviewer:198' via ws1.example:41135 (stress-8ab31d8ba86a411a8450a8458a0a377d): version 105 holds Comment by s1c7-reviewer
- lost update 915b4288-196f-4188-ba6d-2bcda50ae295 v17 Comment by s1c5-reviewer 's1c5-reviewer:24' via ws1.example:41135 (stress-8f4567085bbd47a4855702171df9a6cb): version 17 holds Comment by s1c5-reviewer
- lost update 915b4288-196f-4188-ba6d-2bcda50ae295 v37 Comment by s1c7-reviewer 's1c7-reviewer:57' via ws1.example:41135 (stress-923a99755fc5420799bc8ff269581033): version 37 holds Comment by s1c5-reviewer
- lost update 915b4288-196f-4188-ba6d-2bcda50ae295 v70 Comment by s1c9-admin 's1c9-admin:122' via ws1.example:41135 (stress-c723b77641af4aa9b46bca1c6e8a73ad): version 70 holds Comment by s1c7-reviewer
- lost update 915b4288-196f-4188-ba6d-2bcda50ae295 v82 Comment by s1c9-admin 's1c9-admin:150' via ws1.example:41135 (stress-9f6cdaeda1804680a229b4160464bfc8): version 82 holds Comment by s2c2-reviewer
- lost update 915b4288-196f-4188-ba6d-2bcda50ae295 v41 Comment by s1c12-reviewer 's1c12-reviewer:83' via ws1.example:41135 (stress-e68da712680c46d08d05fb917227749e): version 41 holds Comment by s1c12-reviewer
- lost update 915b4288-196f-4188-ba6d-2bcda50ae295 v74 Comment by s2c7-reviewer 's2c7-reviewer:142' via ws1.example:33537 (stress-df2217a238fd49aa86263ea67db2d22e): version 74 holds Comment by s2c9-admin
- lost update 915b4288-196f-4188-ba6d-2bcda50ae295 v92 Comment by s2c7-reviewer 's2c7-reviewer:175' via ws1.example:33537 (stress-a3548f5205fc4174902f20973f9fa3b6): version 92 holds Comment by s1c7-reviewer
- lost update 915b4288-196f-4188-ba6d-2bcda50ae295 v40 Comment by s2c12-reviewer 's2c12-reviewer:68' via ws1.example:33537 (stress-019a2b3a700f463ca49ad380dd4b7926): version 40 holds Comment by s2c10-reviewer
- lost update 44d47101-ffcb-476f-8b3a-5f93aea71b68 v23 Comment by s1c4-admin 's1c4-admin:48' via ws1.example:41135 (stress-2aeafc46a14d41a78467df321de335a5): version 23 holds Comment by s2c9-admin
- lost update 44d47101-ffcb-476f-8b3a-5f93aea71b68 v63 Comment by s1c5-reviewer 's1c5-reviewer:154' via ws1.example:41135 (stress-fc6d017878624b128bf3752db5f91796): version 63 holds Comment by s2c12-reviewer
- lost update 44d47101-ffcb-476f-8b3a-5f93aea71b68 v70 Comment by s1c5-reviewer 's1c5-reviewer:172' via ws1.example:41135 (stress-0575f794b1d14e9c82f3765567040b53): version 70 holds Comment by s2c4-admin
- lost update 44d47101-ffcb-476f-8b3a-5f93aea71b68 v33 Comment by s1c7-reviewer 's1c7-reviewer:60' via ws1.example:41135 (stress-f39ae3e9ede643eb88c7f23e79746a05): version 33 holds Comment by s2c12-reviewer
- lost update 44d47101-ffcb-476f-8b3a-5f93aea71b68 v47 Comment by s1c7-reviewer 's1c7-reviewer:88' via ws1.example:41135 (stress-ef72ef20933f46d4bf5438a5ca406992): version 47 holds Comment by s1c5-reviewer
- lost update 44d47101-ffcb-476f-8b3a-5f93aea71b68 v56 Comment by s1c7-reviewer 's1c7-reviewer:112' via ws1.example:41135 (stress-a3e1230f515f486c829fdb0e249ad00e): version 56 holds Comment by s1c2-reviewer
- lost update 44d47101-ffcb-476f-8b3a-5f93aea71b68 v29 Comment by s1c9-admin 's1c9-admin:62' via ws1.example:41135 (stress-1610ca5b7fb84bb19b54872cea416fe2): version 29 holds Comment by s1c2-reviewer
- lost update 44d47101-ffcb-476f-8b3a-5f93aea71b68 v50 Comment by s1c9-admin 's1c9-admin:100' via ws1.example:41135 (stress-2453d0b359b04ba2897feda68b65e7c2): version 50 holds Comment by s2c4-admin
- lost update 44d47101-ffcb-476f-8b3a-5f93aea71b68 v59 Comment by s1c9-admin 's1c9-admin:118' via ws1.example:41135 (stress-2cd273e3e2d64c738bdc90a786d7b2b2): version 59 holds Comment by s2c9-admin
- lost update 44d47101-ffcb-476f-8b3a-5f93aea71b68 v43 Comment by s1c10-reviewer 's1c10-reviewer:98' via ws1.example:41135 (stress-9607356f4db5461094ee8fe9665bc412): version 43 holds Comment by s1c9-admin

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 2 | 232 | 28.5 | 531 | 337 | 28 | 0 | 0 | 57.0 | no |
