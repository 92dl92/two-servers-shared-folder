# Stress tool without ssh: servers started by hand, clients from a PC (phase 6c)

How to run the stress test where the servers can't be reached over ssh: an enterprise Windows
environment with 2 Windows servers, a Windows SMB share, **no ssh**, no PowerShell remoting,
and firewalls closed except for ports the owner names. Built in phase 6c (plan, Decisions "No-ssh
test mode"); checked in the lab (below).

## Setup

```text
  your Windows PC                            server 1 (Windows)          server 2 (Windows)
  ┌──────────────────────────┐   port P1    ┌──────────────────┐        ┌──────────────────┐
  │ stress tool              │ ───────────▶ │ test server      │        │ test server      │
  │  simulated users         │   port P2    │ (started by hand │        │ (started by hand │
  │  each request → server 1 │ ───────────────────────────────────────▶ │  with the script)│
  │  or server 2 (random or  │              └────────┬─────────┘        └────────┬─────────┘
  │  sticky, like a load     │                       │  SMB                      │  SMB
  │  balancer)               │                       ▼                           ▼
  │ checks: API + folder     │ ── SMB (read) ──▶ \\fileserver\share\stress\<run>\  (the folder under test)
  └──────────────────────────┘
```

- **One test server per server machine**, started by hand with `ClaudeTestServer.ps1`, each
  on a port the owner supplies. The servers never talk to each other; they share only the
  SMB share.
- **The stress tool runs on the owner's Windows PC.** Its simulated users are tasks inside the
  tool, sending HTTP requests as browsers would. It starts and stops no servers.
- **Ports:** one per server, from the PC to that server. Production users' PCs need the same
  access to the real application, so this is the access most likely to be granted.
- **The PC reads the share** for the strongest check: after the servers stop, every file in
  the folder must load through the strategy.

| Constraint | How it is met |
|---|---|
| No ssh, no remoting | A person starts and stops each server with the script on that machine; the tool uses only HTTP and the share |
| Files can be uploaded | `webapp/scripts/windows/build-packages.sh` builds the packages to upload |
| .NET 8 on the servers (the app is .NET 10) | Both packages are **self-contained** (`win-x64`, runtime included): they run whatever .NET is installed, and install nothing |
| Firewalls closed | The script never changes firewall rules. **Every port is a parameter the owner supplies.** `Start` refuses a port already in use |

## The packages

```bash
webapp/scripts/windows/build-packages.sh [output folder]     # default dist/ (git-ignored)
```

| File | Where it goes |
|---|---|
| `claudetest-server-win-x64.zip` (about 65 MB) | Each server, with the script |
| `ClaudeTestServer.ps1` | Each server |
| `claudetest-stress-win-x64.zip` (about 41 MB) | The PC: unzip anywhere, run `BlazorApp.Stress.exe` |

Each zip holds `commit.txt`. The tool records that commit in every result (from the build,
since the PC has no git).

## The server script: `webapp/scripts/windows/ClaudeTestServer.ps1`

Run locally on each server, in Windows PowerShell 5.1. The server runs as the user who starts
it, so it reaches the share with that user's access.

| Action | Does |
|---|---|
| `Install -Package <zip> [-Root C:\ClaudeTest]` | Unpacks the server into `<Root>\server`; no firewall changes, no services |
| `Start -Strategy <name> -Data <folder> -Users <file> -Port <n> [-Name <run>]` | Starts one server in the background, as the tool starts its own (Development, listening on every interface at `<n>`, logs in `<Root>\runs\<run>\logs`), saves its process id, waits until `GET api/users` answers, prints the address. Refuses a port in use, a missing users file, or a run that already has a server; if the server doesn't answer within 60 s, stops it and shows its last output |
| `Stop [-Name <run>]` | Stops the server that run started, and nothing else |
| `Status` | Which test servers run, on which port and data folder, and the package's commit |
| `CollectLogs -Name <run> -To <folder>` | Zips a run's folder (logs and console output) to `<folder>`, e.g. on the share |
| `Uninstall` | Stops anything still running and removes `<Root>` |

Before the first run, check from the PC that each port is open, with the server started on
it: `Test-NetConnection server1 -Port <n>` (it only succeeds while something listens).

## Stress tool options

| Option | Does |
|---|---|
| `--external <url>,<url>` | Uses these running servers (each `http://host:port`); their count is the number of addresses. Needs `--data` (the share as the PC sees it) and `--out`. Not with `--servers`, `--server-dll`, `--server-arg`, `--remote` or `--fault kill` |
| `--prepare --users-out <file>` | Same command line as the run, plus these: writes the run's users file to `<file>` (for the servers' `-Users`) and the run folder, puts any `--prefill` records into the folder, and exits |
| `--routing random` | Each request goes to a server picked at random, like a load balancer without session affinity: a user can read through one server and save through the other a moment later |
| `--routing sticky` (default) | Each simulated user stays on one server, like a load balancer with session affinity. Handoffs between users still cross servers |

**A run with `--external`:**

1. reads the users (and any prefilled records) `--prepare` left in `--out`, and refuses to run
   without them;
2. checks that each server knows this run's users, i.e. was started with that users file;
3. runs the workload, then reads everything back through each server;
4. prints **"Stop the test servers now"** and waits (up to 60 min) until neither answers;
5. reads the folder through the strategy, runs every check, writes `result.json` and
   `summary.md`.

If the run stops early, it says to stop the servers and checks nothing. Ctrl+C ends the wait
for the servers. The result records the routing, the server addresses, and **requests by
server**. The run refuses settings that differ from those `--prepare` used (strategy, servers,
clients, records, workload, prefill, data folder).

**Not checked:** that each server was started with the same `-Strategy` and `-Data` as the
tool's `--strategy` and `--data` (the servers don't report them). A mismatch shows as a failed
run (records missing from the folder or a server), not a pass, but is reported against the
tool's strategy: start the servers from the same values, and check `Status` on each.

### One run

```powershell
# On the PC (same options both times; --prepare first):
.\BlazorApp.Stress.exe --strategy PerRecordFile --external http://server1:8080,http://server2:8080 `
    --data \\fileserver\share\stress\run1\data --out C:\stress-runs\run1 `
    --clients 5 --workload pipeline --think-ms 1000 --ops 120 --routing random `
    --prepare --users-out \\fileserver\share\stress\run1-users.json

# On each server:
.\ClaudeTestServer.ps1 Start -Strategy PerRecordFile -Data \\fileserver\share\stress\run1\data `
    -Users \\fileserver\share\stress\run1-users.json -Port 8080 -Name run1

# On the PC: the same command without --prepare and --users-out. When it prints
# "Stop the test servers now", on each server:
.\ClaudeTestServer.ps1 Stop -Name run1
```

Each run needs a new data folder, `--out` folder and run name, and **ends with `Stop` on both
servers**: the tool checks the folder only once both have stopped answering, so servers can't
be kept running from one run to the next. A round (strategies × profiles × routings × seeds)
is therefore a start and stop on each server per run. The one exception is a prefilled
folder: seeds can share it (same `--out` and `--data`, no new `--prepare`, results copied out
after each run), since the seed isn't part of the prepared settings; earlier runs' records
then count as "unexpected" and are checked for the rules only.

## Lab check (phase 6c)

Run on 2026-10-18, commit `5d54eb8`, packages from `build-packages.sh`. The phase 6b VMs:
`ws1` and `ws2`, Windows Server 2025 Core, 2 vCPUs each, the share on ws1's own disk
(`C:\share`, leasing off) and reached by ws2 as `\\ws1\share`. **The stress tool ran on ws1**,
standing in for the owner's PC (a Windows machine, self-contained build, no git), against
`http://ws1:8080` and `http://ws2:8080`, each server started with `ClaudeTestServer.ps1
Start`. ssh was only how the commands were typed on each VM; the tool itself used only HTTP and
the share. Light load (2 servers × 5 clients, 6 records, 40 changes each), seeds 1–3.
Write-up: `docs/experiments/no-ssh-lab.md`; every run's `result.json` and `summary.md`:
`docs/experiments/no-ssh-lab/`.

| Strategy | Routing | Result | Lost per run | Stale reads | Requests by server (ws1 / ws2) |
|---|---|---|---|---|---|
| A NaiveFile | sticky | **failed 3/3** | 1–2 | 111–195 | 205/234, 221/330, 242/280 |
| A NaiveFile | random | **failed 3/3** | 2–3 | 0–100 | 327/281, 318/302, 283/321 |
| C PerRecordFile | sticky | **passed 3/3** | 0 | 0 | 403/403 each time |
| C PerRecordFile | random | **passed 3/3** | 0 | 0 | 423/383, 411/395, 381/425 |

- **The known answer holds** through servers started by hand: A fails, C passes, with both
  routings.
- **Routing works as described:** sticky splits requests exactly by client (403/403 every
  time for C); random splits them unevenly, request by request.
- **The tool runs on Windows:** prepared, used the servers, waited for them to be stopped,
  read the share and recorded the build's commit (`5d54eb8 (from the build)`).
- **Found and fixed on the way:** the script didn't parse at first (`$Users:` in a string,
  read as a drive-qualified variable); both Windows scripts now pass PowerShell's parser.
- **Lab-only effect:** a server started through an ssh session stopped when that session
  ended (Windows' ssh server ends every process a session started), so the lab kept those
  sessions open; the tool reported the missing server cleanly (exit 2, nothing checked).

## Fallback: one tool per server, local traffic only

Only if **no port from the PC to the servers** can be opened. Each server runs its own copy
of the tool against its own server on `localhost`; both copies start together on a go-file on
the share; responders find submitted records by listing through their own server, so handoffs
cross between servers only through the share; afterwards one `--check` step merges both
copies' acknowledgements and reads and runs every check. Stale reads then rely on the two
servers' clocks, so the tool measures the difference through the share and reports only
stale reads beyond it. **Designed, not built**: build it only if needed.

## Open

- Which of the ports the owner named is really open from the PC to each server: confirm with
  the server started on it and `Test-NetConnection` from the PC.
- Whether a server started with `Start-Process` from a console keeps running after that
  console window is closed (expected on Windows; not yet seen on a real server). In the lab,
  ssh sessions end every process they started, so the lab kept those sessions open.
