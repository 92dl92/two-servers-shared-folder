# Experiment results: servers started by hand, tool on Windows (phase 6c lab check)

Phase 6c built a way to run the stress test where the servers can't be reached over ssh
(`docs/strategies/StressToolWindows.md`): test servers started by hand with
`webapp/scripts/windows/ClaudeTestServer.ps1`, and the stress tool on a Windows PC using them
(`--external`, `--prepare`, `--routing`). This round checks, before it is used on the real
servers, that the known answer still holds that way and that the tool works on Windows. Each
run's `result.json` and `summary.md` are in `no-ssh-lab/<run>/`.

- **Commit:** `5d54eb8` (clean tree), packages from `webapp/scripts/windows/build-packages.sh`
  (self-contained `win-x64`). The `/code-review high` fixes that followed (PR #29's last
  commit) were checked by the tests and on ws1 by hand, not by a second round.
- **Machines:** the phase 6b VMs on one Linux host (KVM, default NAT network):
  - `ws1`: Windows Server 2025 Standard Evaluation, Server Core (build 26100), 2 vCPUs, 4 GB.
    Runs test server 1 **and the stress tool**, standing in for the owner's PC (no git, no
    .NET SDK: the self-contained tool).
  - `ws2`: the same; runs test server 2.
- **Share:** ws1's `C:\share`, shared as `\\ws1\share` (SMB 3.1.1), **leasing off**. Server 1
  uses `C:\share\6c\<run>\data`, server 2 `\\ws1\share\6c\<run>\data`; the users file is
  `C:\share\6c\<run>-users.json` (ws2: through the share).
- **Ports:** 8080 on both servers. The VMs' firewalls are off (lab only), so this round
  doesn't test firewalls.
- **How the servers were started:** `ClaudeTestServer.ps1 Start -Strategy … -Data … -Users …
  -Port 8080 -Name <run>` on each VM and `Stop -Name <run>` when the tool printed "Stop the
  test servers now". The commands were typed on the VMs over ssh, the only way into them
  here; **the tool itself used only HTTP and the share**. ws2 ran `connect.ps1` (`net use`
  with ws1's Administrator account) first, since an ssh key logon has no network
  credentials.
- **Settings:** light: `--clients 5 --records 6 --ops 40` per server, 2 servers; A
  (`NaiveFile`) and C (`PerRecordFile`); `--routing sticky` and `random`; seeds 1–3. Each run
  was prepared first with the same options plus `--prepare --users-out`. 12 runs, all on
  2026-10-18.

## Summary

| Strategy | sticky | random |
|---|---|---|
| A NaiveFile (control) | **failed 3/3** (1–2 lost per run) | **failed 3/3** (2–3 lost per run) |
| C PerRecordFile | **passed 3/3** | **passed 3/3** |

- **The known answer holds** through servers started by hand, with both routings.
- **C:** nothing lost, no broken file, no rule broken, 0 stale reads, in all 6 runs; 111–131
  collisions per run refused with 409.
- **Routing:** with sticky, requests split exactly by client (403/403 every time for C); with
  random, unevenly, request by request (e.g. 423/383).
- **The tool on Windows:** prepared, checked each server knew the run's users, ran, waited for
  the servers to be stopped, read the share and recorded the build's commit
  (`5d54eb8… (from the build)`, as there is no git on ws1).

## Found along the way

- **The script didn't parse at first:** `"… $Users: …"` in a string is read as a
  drive-qualified variable. Fixed (`${Users}:`); both Windows scripts now pass PowerShell's
  parser on ws1.
- **Lab only:** a server started through an ssh session stopped when that session ended
  (Windows' ssh server ends every process a session started). The first attempt therefore
  found server 1 gone; the tool reported it as "couldn't be done" (exit 2) and checked
  nothing. The runs above kept those sessions open, as a person leaves a server running. On
  the real servers nobody uses ssh, so this doesn't arise; whether a server keeps running
  after the console window that started it is closed is still to be seen there.

## Every run

| Strategy | Routing | Seed | Saves | Saves/s | 409 | 422 | Lost | Unreadable sources | Stale reads | p95 ms | Passed | Requests by server |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| NaiveFile | sticky | 1 | 24 | 14.9 | 13 | 22 | 2 | 0 | 195 | 62.2 | no | ws2 234, ws1 205 |
| NaiveFile | sticky | 2 | 44 | 13.6 | 25 | 27 | 2 | 0 | 160 | 106.5 | no | ws2 330, ws1 221 |
| NaiveFile | sticky | 3 | 41 | 21.6 | 19 | 18 | 1 | 0 | 111 | 71.3 | no | ws2 280, ws1 242 |
| NaiveFile | random | 1 | 62 | 35.6 | 75 | 37 | 2 | 0 | 0 | 62.0 | no | ws2 281, ws1 327 |
| NaiveFile | random | 2 | 40 | 10.4 | 22 | 20 | 3 | 0 | 6 | 141.6 | no | ws2 302, ws1 318 |
| NaiveFile | random | 3 | 36 | 14.5 | 24 | 45 | 3 | 0 | 100 | 130.3 | no | ws2 321, ws1 283 |
| PerRecordFile | sticky | 1 | 187 | 48.8 | 131 | 88 | 0 | 0 | 0 | 101.4 | yes | ws2 403, ws1 403 |
| PerRecordFile | sticky | 2 | 186 | 42.5 | 111 | 109 | 0 | 0 | 0 | 91.8 | yes | ws2 403, ws1 403 |
| PerRecordFile | sticky | 3 | 182 | 35.1 | 117 | 107 | 0 | 0 | 0 | 136.7 | yes | ws2 403, ws1 403 |
| PerRecordFile | random | 1 | 189 | 47.5 | 129 | 88 | 0 | 0 | 0 | 114.5 | yes | ws2 383, ws1 423 |
| PerRecordFile | random | 2 | 180 | 48.0 | 127 | 99 | 0 | 0 | 0 | 117.5 | yes | ws2 395, ws1 411 |
| PerRecordFile | random | 3 | 168 | 47.1 | 114 | 124 | 0 | 0 | 0 | 119.7 | yes | ws2 425, ws1 381 |

`Lost` counts acknowledged changes not found at their version in the folder read-back. A's
saves per second describe a store that broke partway through; don't compare them with C's.

## What this does and doesn't show

- **Shows:** the harness works without ssh and from Windows, and still tells a broken store
  from a correct one, with either routing.
- **Doesn't show:** anything new about C's safety beyond phase 6b (same VMs, same share, light
  load only), firewalls (off here), a real network, or the real servers. Those are what the
  round on the production-like servers is for (`docs/handover-solution-c.md`, section 10).
