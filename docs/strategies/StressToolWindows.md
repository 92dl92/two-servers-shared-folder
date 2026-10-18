# Stress tool without ssh: scripted Windows servers, clients from a PC (planned)

Design for running the stress test in an enterprise Windows environment: 2 Windows servers,
a Windows SMB share, **no ssh**, PowerShell remoting not assumed, firewalls closed except
for ports the user names. Status: **planned**, to be built after phase 6b merges (the 6b
session owns the stress tool until then). Decided 2026-10-18.

## Setup

```text
  your Windows PC                            server 1 (Windows)          server 2 (Windows)
  ┌──────────────────────────┐   port P1    ┌──────────────────┐        ┌──────────────────┐
  │ stress tool              │ ───────────▶ │ test server      │        │ test server      │
  │  10 simulated users      │   port P2    │ (started by hand │        │ (started by hand │
  │  each request → server 1 │ ───────────────────────────────────────▶ │  with the script)│
  │  or server 2 (random or  │              └────────┬─────────┘        └────────┬─────────┘
  │  sticky, like a load     │                       │  SMB                      │  SMB
  │  balancer)               │                       ▼                           ▼
  │ checks: API + folder     │ ── SMB (read) ──▶ \\fileserver\share\stress\<run>\  (the folder under test)
  └──────────────────────────┘
```

- **One test server per server machine**, started by hand with a script, each on a port the
  user supplies. The servers never talk to each other; they share only the SMB share.
- **The stress tool runs on the user's Windows PC.** Its simulated users are tasks inside the
  tool, sending HTTP requests as browsers would. It starts no servers.
- **Ports:** one per server, from the PC to that server. Production users' PCs need the same
  access to the real application, so this is the access most likely to be granted.
- **The PC reads the share** for the strongest check: after the servers stop, every file in
  the folder must load through the strategy.

| Constraint | Consequence |
|---|---|
| No ssh, no remoting | A person starts and stops each server with a script on that machine |
| Files can be uploaded | A package: the published Server and the scripts (the tool is published for the PC) |
| .NET 8 on the servers (maybe 10) | The app targets .NET 10, so the server is published **self-contained** (`win-x64`, runtime included, about 100 MB): it runs whatever .NET is installed, and installs nothing |
| Firewalls closed | The scripts never change firewall rules. **Every port is a parameter the user supplies.** A `Test` action checks a port before a run |
| A Windows PC as the tool's machine | Untested so far (only Linux has run the tool): first run it from a Windows machine in the lab |

## The server script: `webapp/scripts/windows/ClaudeTestServer.ps1`

Run locally on each server, in Windows PowerShell 5.1.

| Action | Does |
|---|---|
| `Install -Package <zip> [-Root C:\ClaudeTest]` | Unpacks the package; checks the runtime; no firewall changes, no services |
| `Start -Strategy <name> -Data <folder> -Users <file> -Port <n> [-Name <run>]` | Starts one server in the background (`--urls http://*:<n>`, logs under `<Root>\runs\<run>\logs`), saves its process id, waits until `GET api/users` answers locally, prints the URL |
| `Stop [-Name <run>]` | Stops the server it started (by saved process id), and nothing else |
| `Status` | Which test servers run, on which port and data folder, and the package's commit |
| `CollectLogs -Name <run> -To <folder>` | Zips a run's logs, for example onto the share for the PC to pick up |
| `Uninstall` | Stops anything still running and removes `<Root>` |

From the PC, a matching check before every round: `Test-NetConnection server1 -Port P1`
(and the tool's own first request) shows a blocked port before anything runs.

## Stress tool changes

New options (names to be settled when built):

| Option | Does |
|---|---|
| `--prepare --users-out <file>` | Writes the run's users file (on the share, so both servers read the same one) and exits |
| `--external <url>,<url>` | Uses these running servers; starts and stops none. Requires `--data` (the share path as the PC sees it) |
| `--routing random` | Each request goes to a server picked at random, like a load balancer without session affinity. The strictest case: a user can read through one server and save through the other a second later |
| `--routing sticky` | Each simulated user stays on one server (today's behaviour), like a load balancer with session affinity. Handoffs between users still cross servers |

**Read-back order:** the API read-back through both servers while they run; then the tool
asks for the servers to be stopped (it waits until neither answers, or for
`--confirm-stopped`), then reads the folder through the strategy and runs every check.

**Results** record the routing, the server URLs and that the servers were external, so the
round's write-up can say how they were started.

### One run

1. On the PC: `stress --prepare … --users-out \\fileserver\share\stress\<run>\users.json`
2. On each server: `ClaudeTestServer.ps1 Start -Strategy PerRecordFile -Data \\fileserver\share\stress\<run>\data -Users \\fileserver\share\stress\<run>\users.json -Port <its port>`
3. On the PC: `stress --external http://server1:P1,http://server2:P2 --data \\fileserver\share\stress\<run>\data --routing random --workload pipeline …`
4. When the tool asks: `ClaudeTestServer.ps1 Stop` on each server.
5. The tool reads the folder, checks, and writes `result.json` and `summary.md`.

A round (strategies × profiles × routing × seeds 1–3) is many runs with two manual steps on
each server per run; a single long-running server per strategy, with a fresh data folder
per strategy rather than per run, is a possible shortcut to weigh when building.

## Fallback: one tool per server, local traffic only

Only if **no port from the PC to the servers** can be opened. Each server runs its own copy
of the tool against its own server on `localhost`; both copies start together on a go-file on
the share; responders find submitted records by listing through their own server, so handoffs
cross between servers only through the share; afterwards one `--check` step merges both
copies' acknowledgements and reads and runs every check. Stale reads then rely on the two
servers' clocks, so the tool measures the difference through the share and reports only
stale reads beyond it. Designed, not built unless needed.

## Tests to write first (seen failing before the code)

- Option parsing and refused combinations (`--external` without `--data`, a URL without a
  port, `--routing` with an unknown value).
- `--external` against servers the test starts itself: the existing known answers must hold
  (in-memory: one server passes, two servers fail; A fails, C passes), with both routings.
- `--routing random` really spreads one user's requests over both servers (count per server
  in the result), and `sticky` doesn't.
- The script: `Start`/`Status`/`Stop` round trip on a Windows test VM; `Start` on a port in
  use fails clearly. (No Windows in CI: by hand, recorded in the commit.)
- The tool from a Windows machine in the lab (one light run of A and C) before the
  enterprise round.

## Open

- Which of the ports the owner named (believed open from the user's PC to either
  server), really is open: confirm with the server started on it and `Test-NetConnection
  <server> -Port <n>` from the PC (it only succeeds when something is listening).

Settled: .NET 8 on the servers (so self-contained); the user can clean the share's test
folder between runs.
