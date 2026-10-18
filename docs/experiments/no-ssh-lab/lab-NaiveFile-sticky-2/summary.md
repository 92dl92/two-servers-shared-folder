# Stress run: NaiveFile, 2 server(s): FAILED

- Commit `5d54eb86e993b9c29b4e326d9494a1811ac767b9 (from the build)`, 2026-10-18 22:20:02 UTC, 3.2 s
- Machine WS1: Microsoft Windows 10.0.26100, 2 CPUs, .NET 10.0.12
- Settings: 2 servers × 5 clients, 6 records, 40 changes per client, seed 2
- Clients by role: 4 Registrant, 4 Reviewer, 2 Admin
- Data folder `C:\share\6c\lab-NaiveFile-sticky-2\data`; run folder `C:\Users\Administrator\claudetest-6c\runs\lab-NaiveFile-sticky-2`
- Servers started by hand: `http://ws1.example:8080/`, `http://ws2.example:8080/`
- Routing sticky. Requests by server: ws2.example:8080 330, ws1.example:8080 221

| Writes | Saves | Saves/s | 409 conflicts | 422 wrong status | No response | Save p50 ms | p95 ms | max ms |
|---|---|---|---|---|---|---|---|---|
| 191 | 44 | 13.6 | 25 | 27 | 0 | 54.0 | 106.5 | 163.7 |

Read p50/p95/max ms: 20.6 / 67.9 / 130.9.

Writes by outcome: conflict 25, http500 95, saved 44, wrongStatus 27. Saves by action: Create 4, Update 23, Submit 2, Approve 1, Comment 14. Reads: 360 (http500 15, notFound 160).

Longest stall: 0.3 s with no save acknowledged, from 2.1 s into the run.

Stale reads: 160 (older than a version already acknowledged); 74 reads followed a save through another server.
- record bae05974-830d-460b-ab09-d1a6f0d80d70 through ws1.example:8080: version 0, 98.3 ms after version 1 was acknowledged by ws1.example:8080
- record bae05974-830d-460b-ab09-d1a6f0d80d70 through ws1.example:8080: version 0, 100.7 ms after version 1 was acknowledged by ws1.example:8080
- record bae05974-830d-460b-ab09-d1a6f0d80d70 through ws1.example:8080: version 0, 120.8 ms after version 1 was acknowledged by ws1.example:8080
- record bae05974-830d-460b-ab09-d1a6f0d80d70 through ws1.example:8080: version 0, 97.2 ms after version 1 was acknowledged by ws1.example:8080
- record bae05974-830d-460b-ab09-d1a6f0d80d70 through ws1.example:8080: version 0, 123.3 ms after version 1 was acknowledged by ws1.example:8080
- record 89663f01-18b5-41a2-accf-87debfcdb9c0 through ws1.example:8080: version 0, 135.5 ms after version 1 was acknowledged by ws1.example:8080
- record 89663f01-18b5-41a2-accf-87debfcdb9c0 through ws1.example:8080: version 0, 138.9 ms after version 1 was acknowledged by ws1.example:8080
- record bae05974-830d-460b-ab09-d1a6f0d80d70 through ws1.example:8080: version 0, 150.8 ms after version 1 was acknowledged by ws1.example:8080
- record 89663f01-18b5-41a2-accf-87debfcdb9c0 through ws1.example:8080: version 0, 173.5 ms after version 1 was acknowledged by ws1.example:8080
- record 89663f01-18b5-41a2-accf-87debfcdb9c0 through ws1.example:8080: version 0, 173.6 ms after version 1 was acknowledged by ws1.example:8080
- record 89663f01-18b5-41a2-accf-87debfcdb9c0 through ws1.example:8080: version 0, 176.0 ms after version 1 was acknowledged by ws1.example:8080
- record bae05974-830d-460b-ab09-d1a6f0d80d70 through ws1.example:8080: version 0, 169.4 ms after version 1 was acknowledged by ws1.example:8080
- record 89663f01-18b5-41a2-accf-87debfcdb9c0 through ws1.example:8080: version 0, 183.2 ms after version 1 was acknowledged by ws1.example:8080
- record 89663f01-18b5-41a2-accf-87debfcdb9c0 through ws1.example:8080: version 0, 192.7 ms after version 1 was acknowledged by ws1.example:8080
- record 89663f01-18b5-41a2-accf-87debfcdb9c0 through ws1.example:8080: version 0, 195.5 ms after version 1 was acknowledged by ws1.example:8080
- record 89663f01-18b5-41a2-accf-87debfcdb9c0 through ws1.example:8080: version 0, 213.7 ms after version 1 was acknowledged by ws1.example:8080
- record 89663f01-18b5-41a2-accf-87debfcdb9c0 through ws1.example:8080: version 0, 213.8 ms after version 1 was acknowledged by ws1.example:8080
- record bae05974-830d-460b-ab09-d1a6f0d80d70 through ws1.example:8080: version 0, 202.5 ms after version 1 was acknowledged by ws1.example:8080
- record bae05974-830d-460b-ab09-d1a6f0d80d70 through ws1.example:8080: version 0, 202.6 ms after version 1 was acknowledged by ws1.example:8080
- record bae05974-830d-460b-ab09-d1a6f0d80d70 through ws1.example:8080: version 0, 205.4 ms after version 1 was acknowledged by ws1.example:8080

| Read back from | Result | Records found/expected | Missing records | Lost updates | Wrong version | Unacknowledged | Rules broken |
|---|---|---|---|---|---|---|---|
| storage | FAIL | 2/4 | 2 | 2 | 0 | 0 | 0 |
| server ws1.example:8080 | FAIL | 2/4 | 2 | 2 | 0 | 0 | 0 |
| server ws2.example:8080 | FAIL | 2/4 | 2 | 2 | 0 | 0 | 0 |

storage:
- missing record 89663f01-18b5-41a2-accf-87debfcdb9c0: missing (1 acknowledged changes)
- missing record bae05974-830d-460b-ab09-d1a6f0d80d70: missing (1 acknowledged changes)
- lost update 89663f01-18b5-41a2-accf-87debfcdb9c0 v1 Create by s1c1-registrant via ws1.example:8080 (stress-3c505bba412c45bb8de8e866fa677e74): record missing
- lost update bae05974-830d-460b-ab09-d1a6f0d80d70 v1 Create by s1c1-registrant via ws1.example:8080 (stress-fbf4a3106ef34ba9a23c0a070954fefe): record missing

server ws1.example:8080:
- missing record 89663f01-18b5-41a2-accf-87debfcdb9c0: missing (1 acknowledged changes)
- missing record bae05974-830d-460b-ab09-d1a6f0d80d70: missing (1 acknowledged changes)
- lost update 89663f01-18b5-41a2-accf-87debfcdb9c0 v1 Create by s1c1-registrant via ws1.example:8080 (stress-3c505bba412c45bb8de8e866fa677e74): record missing
- lost update bae05974-830d-460b-ab09-d1a6f0d80d70 v1 Create by s1c1-registrant via ws1.example:8080 (stress-fbf4a3106ef34ba9a23c0a070954fefe): record missing

server ws2.example:8080:
- missing record 89663f01-18b5-41a2-accf-87debfcdb9c0: missing (1 acknowledged changes)
- missing record bae05974-830d-460b-ab09-d1a6f0d80d70: missing (1 acknowledged changes)
- lost update 89663f01-18b5-41a2-accf-87debfcdb9c0 v1 Create by s1c1-registrant via ws1.example:8080 (stress-3c505bba412c45bb8de8e866fa677e74): record missing
- lost update bae05974-830d-460b-ab09-d1a6f0d80d70 v1 Create by s1c1-registrant via ws1.example:8080 (stress-fbf4a3106ef34ba9a23c0a070954fefe): record missing

Results row (strategy | servers | saves | saves/s | 409 | 422 | lost | unreadable sources | rules broken | p95 ms | passed):

| NaiveFile | 2 | 44 | 13.6 | 25 | 27 | 2 | 0 | 0 | 106.5 | no |
