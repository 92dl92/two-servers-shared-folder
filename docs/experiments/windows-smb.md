# Experiment results: Windows servers on an SMB share (phase 6b)

Phase 6 showed that servers on two machines sharing an NFS folder lost data, with the macOS
client to blame. Production would be different: **two Windows Server machines**, at most
**10 clients**, working as a **pipeline** (registrants create and submit, reviewers decide), and
about **2,000 records a year, kept 3 years, each updated 5–10 times**. The owner also requires
**one file (or more) per record** on the share. Phase 6b asks whether a shared folder is safe
there, and under what conditions.

The method and every check are in `docs/strategies/StressTool.md`. Each run's `result.json`
and `summary.md` are in `windows-smb/<run>/`, with paths made relative to the repository; the
data and log folders were not kept. Decisions taken along the way are in `docs/plan.md`
(Decisions, rows marked "phase 6b").

## Summary

- **Strategy C (one file per record) on a Windows SMB share passed every test**: two servers
  colliding as hard as they can, the realistic pipeline, a server killed on either machine,
  ws2's network cut for 10 s and 60 s, 6,000 records on the share, and an hour of continuous
  use. No acknowledged save was lost, no file broken, no record left locked, and no read ever
  showed an older version than one already saved (0 stale reads in over 3,600 handoffs
  between the servers in the soak alone).
- **It needed three changes to get there**, each found by a test and each now in the code or
  the setup:
  1. **Retry the rename.** Windows refuses to replace a file while any other handle has it
     open, which gave users "save failed" errors (up to 141 per heavy run). B and C now retry
     for up to 3 s: errors 0.
  2. **Share leasing off** (`Set-SmbShare -LeasingMode None`). With it on, the whole system
     froze for about 35 s now and then (the SMB lease-break timeout), in 2 of 6 heavy runs.
     Off, no freeze; slower under heavy load, no difference at the realistic load.
  3. **A list cache in C.** With 6,000 records, the Records list read every file: about 0.6 s
     on ws1 and more than a minute (every time) on ws2 over SMB. Now ~0.1 s on both. The first
     list after a server starts still reads every file (~50 s on ws2), which the owner accepted.
- **Ruled out:** E (SQLite) on a share **loses data without any error** (each server sees only
  its own writes); A (no lock) loses data **even at the realistic, low load**. B and D work but
  don't meet the one-file-per-record requirement.
- **Not tested** (see the end): a real network between real machines, the NAS (Samba) as the
  share host, a dedicated Windows file server, a load balancer, more than 2 processors per
  server, and production's antivirus and backup software.

## Setup

- **Machines:** two Windows Server 2025 Standard Evaluation VMs (Server Core, build 26100),
  `ws1` ws1.example and `ws2` ws2.example, each 2 vCPUs and 4 GB, on `linux-host` (Debian 12,
  16 CPUs) under KVM/libvirt with the default NAT network. .NET 10.0.13 runtime on the VMs;
  the stress tool and every client run on `linux-host` (.NET 10.0.12).
- **Share:** ws1's `C:\share`, shared as `\\ws1\share` (SMB 3.1.1). ws2 reaches it with
  `net use` and ws1's Administrator account, run by `connect.ps1` before each command, since an
  ssh key logon has no network credentials.
- **Layouts:**
  - **W0** (baseline): both servers on ws1, data on its own disk. Tells Windows apart from SMB.
  - **W2** (as production): server 1 on ws1 (`C:\share\runs\<run>`), server 2 on ws2
    (`\\ws1.example\share\runs\<run>`).
- **Loads** (2 servers each):
  - **light:** 5 clients per server, 6 records, 40 changes per client;
  - **heavy:** 12 clients per server, 12 records, 200 changes per client;
  - **realistic:** the pipeline, 5 clients per server (2 registrants, 2 reviewers, 1 admin),
    1 s think time (0.5–1.5 s), 120 changes per registrant. With `--list`, reviewers open the
    Records list before each pick.
- **Checks** (every run): every acknowledged save is in the data at its version; no broken
  file; no rule broken; no history entry nobody acknowledged (beyond requests that got no
  answer); stale reads, longest stall, server errors and latencies reported. Failure tests
  add the lock check (every record still changeable afterwards); volume runs add that every
  prefilled record is still there, unchanged.
- **Known answer** (as in phases 5 and 6): A must fail and E pass with both servers on one
  machine (W0). It did: A failed 6 of 6, E passed 6 of 6.

## Results by round

### 1. W0 and W2 under contention (commit `c71df76`, 60 runs)

| Strategy | W0 (one machine) | **W2 (two machines, SMB)** | Saves/s heavy, W0 → W2 | Save p95 heavy, W0 → W2 |
|---|---|---|---|---|
| A NaiveFile (control) | fails 6/6 | fails 6/6 | 26 → 15 | ~85 → ~145 ms |
| B LockedFile | passes 6/6 | **passes 6/6** | 31 → 19 | ~290 → ~495 ms |
| C PerRecordFile | passes 6/6 | **passes 6/6** | 85 → 68 | ~105 → ~165 ms |
| D AppendLog | passes 6/6 | **passes 6/6** | 80 → 40 | ~150 → ~220 ms |
| E Sqlite | passes 6/6 | **loses data 6/6** | 192 → 73 | ~47 → ~195 ms |

Windows on its own disk behaves like Linux did in phase 5. Over SMB, B, C and D lost nothing
with up to ~1,200 refused collisions per run, where on NFS with the Mac every strategy failed.
**E fails silently:** SQLite's WAL mode keeps its index in memory shared only between
processes on one machine, so each server saw only its own records (up to 386 acknowledged
saves lost per run, and one server unable to read the database at all). On NFS it refused to
start; on SMB it starts and loses data.

**Server errors the checks didn't count.** A refused save (HTTP 500) loses nothing, so the
checks pass, but a user sees it. C had 3–5 per light and 45–68 per heavy run on W0, about three
times as many on W2; B had one in all of W2; D none. Each was Windows refusing
`File.Move(..., overwrite: true)` ("access denied") while another handle had the old file open:
another server reading it, the SMB client keeping its handle, perhaps the antivirus. Linux
allows that rename, so phase 5 never saw it. See round 3.

### 2. The realistic pipeline (commit `fe53fea`, 15 runs)

About 160 records per run went through create, edit, submit and decide, with a 1 s think
time; about half the handoffs crossed from one server to the other.

| Strategy | Result | Lost | Server errors | Stale reads / cross-server reads |
|---|---|---|---|---|
| A | fails 3/3 | 4 updates and 1 record per run | 26–39 | 2–4 / ~140 |
| B, C, D | pass 9/9 | 0 | 0 (B, D), 1–4 (C) | 0 / ~140–165 |
| E | fails 3/3 | 376–715 saves, 85–155 records | up to 223 | every cross-server read |

**Rare collisions don't make the naive approach safe:** A lost data in every run at this load.

### 3. The rename retry (commit `1e1cb64`, 12 runs)

The owner chose to retry the rename while Windows reports the file in use
(`Storage/FileReplace.cs`, logged as `StoreReplaceRetried`): first 1 s, later 3 s.

| C | Server errors before | After | Saves/s before → after |
|---|---|---|---|
| W0 light | 3–5 | 0 | ~79 → ~87 |
| W0 heavy | 45–68 | 0 | ~85 → ~94 |
| W2 light | 9–19 | 0 | ~54 → ~64 |
| W2 heavy | 111–141 | 0, 0, 1 | ~68 → ~83 |

A retried save waited 35 ms on average (at most 5 retries, 145 ms). The one remaining error
was not a slow reader: **the rename waited 35.4 s** and no server saved anything meanwhile.

### 4. Share leasing on or off (commit `a8d2366`, 18 runs)

35 s is the Windows SMB **lease-break timeout**: the file server asks the client holding a
cached copy (a lease) to give it up before the file can be replaced, and waits up to 35 s for
an answer. With leasing off for the share there is nothing to break.

| C on W2 | Runs | 35 s freeze | Server errors | Retries per run | Saves/s | Save p95 | Slowest save |
|---|---|---|---|---|---|---|---|
| leasing on, heavy | 6 | **2 of 6** | 0, or 3–7 when frozen | ~140 | ~80 (20–30 when frozen) | ~140 ms | 35.6 s |
| leasing off, heavy | 6 | 0 | 0–10 (gave up after 1 s) | ~470 | ~42 | ~290 ms | 1.9 s |
| leasing on, realistic | 3 | 0 | 0 | 0–7 | 5.7 | ~48 ms | 0.2 s |
| leasing off, realistic | 3 | 0 | 0 | 11–16 | 5.7 | ~64 ms | 0.3 s |

At the realistic load both are clean. Under heavy load, leasing on is faster but occasionally
freezes everyone for 35 s; off is predictable but slower. **The owner chose leasing off**, and
the retry limit was raised from 1 s to 3 s for the few renames still refused for longer.

### 5. Failure tests (commit `22febeb`, 12 runs)

C on W2, leasing off, the realistic load, the fault 40 s into each run, seeds 1–3. After each
run the lock check commented on every record through every surviving server.

| Test | Lost | Broken files | Stuck records | Stale reads | What users saw |
|---|---|---|---|---|---|
| **F1** ws2's server killed | 0 | 0 | 0 of ~110 | 0 | ws2's users refused (~160 requests); ws1 carried on, longest pause 3.8 s |
| **F2** ws1's server killed | 0 | 0 | 0 of ~110 | 0 | the same the other way round, longest pause 1.3 s |
| **F3** ws2's network cut 10 s | 0 | 0 | 0 of ~330 | 0 | ws2's requests waited and completed (one save took 15 s); 1 save in 3 runs failed ("network error") |
| **F4** ws2's network cut 60 s | 0 | 0 | 0 of ~325 | 0 | ws2 unreachable for the minute; for 2–3 s after, 2–6 saves failed ("network path not found") while it reconnected to the share |

**The dangerous case didn't happen:** after a 60 s cut, ws2 never wrote over what ws1 saved
meanwhile (no "split brain"), and no lock was left held. What users notice is availability:
errors or waits on the failed server, a handful of clean "save failed" around reconnection.
Whether ws2 held a record's lock at the exact moment of a cut is down to chance; the 15 s saves
in F3 show it happened at least sometimes and completed correctly.

### 6. Volume: 6,000 records (commits `90481b1`, `b3d3ed7`, 3 runs + trials)

The share was filled with 6,000 finished records (6–11 changes each, made through the app's
rules) before the realistic load with the Records list.

| C on W2, 6,000 records | List median / p95 | First list after start | Save median / p95 | Lost |
|---|---|---|---|---|
| without the cache (trial) | ws1 0.6 s; **ws2 > 60 s, every list failed** | (every list) | 17 ms / 220 ms | 0 |
| **with the list cache** (3 runs) | **64–99 / ~240 ms** | ~46–53 s | 15–24 / 71–154 ms | 0 |

C built the list by reading every record file; over SMB with leasing off each file is a
network round trip. The owner chose a **list cache per server** over an archive folder or an
index file (one shared file again): the store remembers each record with its file's creation
time, write time and size (all from one directory listing), re-reads only changed files, and
re-reads any file written in the last 3 s, since a change can leave all three values the same.
A record's page and every save still read the file. **Accepted limitation:** the first list
after a server starts reads every file (~21 s on ws1, ~50 s on ws2 on these 2-processor VMs)
and slows other requests meanwhile (saves waited up to 15–25 s then).

### 7. Soak: one hour (commit `7d9ce59`, code as `b3d3ed7`)

C on W2, leasing off, 6,000 prefilled records, the realistic load with the list for 59
minutes: **19,828 saves, 4,401 new records, all 10,401 records present, 0 lost, 0 server
errors, 0 stale reads** in 3,619 cross-server reads, longest pause 6.4 s (at the start).

| Minutes | Saves | Save median / p95 | List median / p95 | Read p95 |
|---|---|---|---|---|
| 0–10 | 3,333 | 16 / 63 ms | 60 / 236 ms | 24 ms |
| 10–20 | 3,583 | 17 / 63 ms | 90 / 236 ms | 23 ms |
| 20–30 | 3,582 | 16 / 59 ms | 89 / 244 ms | 20 ms |
| 30–40 | 3,547 | 15 / 57 ms | 92 / 268 ms | 24 ms |
| 40–50 | 3,590 | 16 / 60 ms | 103 / 298 ms | 23 ms |
| 50–60 | 2,686 | 25 / 57 ms | 106 / 287 ms | 14 ms |

Saves and reads stay flat; the list grows with the number of records (each list still looks
at every file name). Each server stayed at about 210–240 MB and 510–560 handles, and no temp
file was left on the share (sampled every 5 minutes).

## What this means for production

If the production setup matches what was tested, C on a Windows SMB share is a workable design
for this load. These conditions come from the tests:

1. **Strategy C with the rename retry and the list cache** (as now in the code). Never A, never
   SQLite on the share.
2. **The share on Windows, leasing off** for that share (`Set-SmbShare -Name <share>
   -LeasingMode None`).
3. **The two servers' clocks in sync** to well within 3 s (normal for domain members): the list
   cache relies on it.
4. **Expect, and tell users about:** a "save failed, try again" now and then after a network
   problem; a slow Records list for up to about a minute after a server restarts; users on a
   server that's down get errors until they use the other one (a load balancer would fix that).
5. **Watch the logs** for `StoreReplaceRetried` warnings with outcome `failed`, and for 500s.
6. **Ask IT** about antivirus exclusions and backup software on the share; neither was tested.

## Not tested

- **A real network** between real machines: both VMs ran on one host, so SMB never saw real
  latency, packet loss or a switch.
- **Other share hosts:** the NAS (Samba), a dedicated Windows file server, a clustered
  ("continuously available") share.
- **More processors:** the VMs had 2; the cold first list and its slowdown are likely shorter
  with more.
- **Production software:** antivirus configuration, backup or snapshot software touching the
  share, Windows updates restarting a server.
- **Deleting records** under load (no workload deletes; C's delete has no retry on Windows).
- **A load balancer** in front of the two servers.
- **Faults beyond F1–F4:** the share host (ws1) itself rebooting, a full disk, clock skew.
- The flaky known-answer test `NaiveFileStoreTests.Many_threads_from_the_same_version_exactly_one_wins`
  (A, under load) is unchanged, waiting for the owner.

## Every run
#### W0 light (commit c71df76)

| Run | Result | Saves | Saves/s | 409 | 500 | Lost | Missing | Unreadable | Save p95 ms |
|---|---|---|---|---|---|---|---|---|---|
| win-w0-light-AppendLog-1 | pass | 175 | 74.0 | 123 | 0 | 0 | 0 | 0 | 89.3 |
| win-w0-light-AppendLog-2 | pass | 188 | 78.1 | 115 | 0 | 0 | 0 | 0 | 61.2 |
| win-w0-light-AppendLog-3 | pass | 187 | 73.7 | 105 | 0 | 0 | 0 | 0 | 72.2 |
| win-w0-light-LockedFile-1 | pass | 239 | 34.9 | 69 | 0 | 0 | 0 | 0 | 164.8 |
| win-w0-light-LockedFile-2 | pass | 213 | 31.7 | 78 | 0 | 0 | 0 | 0 | 147.4 |
| win-w0-light-LockedFile-3 | pass | 224 | 33.8 | 75 | 0 | 0 | 0 | 0 | 150.1 |
| win-w0-light-NaiveFile-1 | **FAIL** | 57 | 34.7 | 39 | 168 | 8 | 0 | 0 | 79.7 |
| win-w0-light-NaiveFile-2 | **FAIL** | 48 | 30.9 | 20 | 164 | 3 | 2 | 0 | 120.8 |
| win-w0-light-NaiveFile-3 | **FAIL** | 37 | 30.5 | 27 | 83 | 3 | 3 | 0 | 114.4 |
| win-w0-light-PerRecordFile-1 | pass | 178 | 80.2 | 131 | 4 | 0 | 0 | 0 | 66.2 |
| win-w0-light-PerRecordFile-2 | pass | 171 | 79.7 | 134 | 5 | 0 | 0 | 0 | 52.5 |
| win-w0-light-PerRecordFile-3 | pass | 167 | 75.6 | 123 | 3 | 0 | 0 | 0 | 59.9 |
| win-w0-light-Sqlite-1 | pass | 195 | 136.3 | 92 | 0 | 0 | 0 | 0 | 27.0 |
| win-w0-light-Sqlite-2 | pass | 222 | 132.5 | 110 | 0 | 0 | 0 | 0 | 27.4 |
| win-w0-light-Sqlite-3 | pass | 234 | 137.2 | 104 | 0 | 0 | 0 | 0 | 31.5 |

#### W0 heavy (commit c71df76)

| Run | Result | Saves | Saves/s | 409 | 500 | Lost | Missing | Unreadable | Save p95 ms |
|---|---|---|---|---|---|---|---|---|---|
| win-w0-heavy-AppendLog-1 | pass | 1750 | 78.0 | 1299 | 0 | 0 | 0 | 0 | 156.1 |
| win-w0-heavy-AppendLog-2 | pass | 1783 | 80.5 | 1329 | 0 | 0 | 0 | 0 | 142.0 |
| win-w0-heavy-AppendLog-3 | pass | 1770 | 81.5 | 1343 | 0 | 0 | 0 | 0 | 149.7 |
| win-w0-heavy-LockedFile-1 | pass | 2144 | 32.1 | 1029 | 0 | 0 | 0 | 0 | 266.1 |
| win-w0-heavy-LockedFile-2 | pass | 2121 | 31.5 | 1020 | 0 | 0 | 0 | 0 | 309.4 |
| win-w0-heavy-LockedFile-3 | pass | 2099 | 30.8 | 1055 | 0 | 0 | 0 | 0 | 300.5 |
| win-w0-heavy-NaiveFile-1 | **FAIL** | 243 | 27.6 | 340 | 958 | 22 | 3 | 0 | 72.1 |
| win-w0-heavy-NaiveFile-2 | **FAIL** | 207 | 23.5 | 266 | 1031 | 20 | 4 | 0 | 120.4 |
| win-w0-heavy-NaiveFile-3 | **FAIL** | 232 | 28.4 | 531 | 1101 | 28 | 1 | 0 | 57.0 |
| win-w0-heavy-PerRecordFile-1 | pass | 1710 | 87.3 | 1662 | 45 | 0 | 0 | 0 | 96.9 |
| win-w0-heavy-PerRecordFile-2 | pass | 1730 | 83.1 | 1584 | 68 | 0 | 0 | 0 | 111.0 |
| win-w0-heavy-PerRecordFile-3 | pass | 1728 | 84.2 | 1642 | 66 | 0 | 0 | 0 | 102.3 |
| win-w0-heavy-Sqlite-1 | pass | 1991 | 186.6 | 1132 | 0 | 0 | 0 | 0 | 45.5 |
| win-w0-heavy-Sqlite-2 | pass | 1987 | 201.7 | 1081 | 0 | 0 | 0 | 0 | 41.6 |
| win-w0-heavy-Sqlite-3 | pass | 1895 | 187.5 | 1194 | 0 | 0 | 0 | 0 | 53.4 |

#### W2 light (commit c71df76)

| Run | Result | Saves | Saves/s | 409 | 500 | Lost | Missing | Unreadable | Save p95 ms |
|---|---|---|---|---|---|---|---|---|---|
| win-w2-light-AppendLog-1 | pass | 197 | 46.4 | 102 | 0 | 0 | 0 | 0 | 124.2 |
| win-w2-light-AppendLog-2 | pass | 227 | 45.9 | 106 | 0 | 0 | 0 | 0 | 112.3 |
| win-w2-light-AppendLog-3 | pass | 212 | 13.4 | 97 | 0 | 0 | 0 | 0 | 130.6 |
| win-w2-light-LockedFile-1 | pass | 223 | 25.9 | 76 | 0 | 0 | 0 | 0 | 245.9 |
| win-w2-light-LockedFile-2 | pass | 233 | 25.7 | 72 | 0 | 0 | 0 | 0 | 181.6 |
| win-w2-light-LockedFile-3 | pass | 215 | 28.7 | 73 | 0 | 0 | 0 | 0 | 178.9 |
| win-w2-light-NaiveFile-1 | **FAIL** | 55 | 19.5 | 37 | 138 | 3 | 1 | 0 | 161.8 |
| win-w2-light-NaiveFile-2 | **FAIL** | 53 | 19.4 | 17 | 150 | 3 | 1 | 0 | 183.2 |
| win-w2-light-NaiveFile-3 | **FAIL** | 72 | 23.3 | 48 | 195 | 3 | 0 | 0 | 159.3 |
| win-w2-light-PerRecordFile-1 | pass | 173 | 48.4 | 108 | 9 | 0 | 0 | 0 | 112.8 |
| win-w2-light-PerRecordFile-2 | pass | 206 | 55.5 | 122 | 19 | 0 | 0 | 0 | 117.1 |
| win-w2-light-PerRecordFile-3 | pass | 181 | 58.6 | 115 | 11 | 0 | 0 | 0 | 102.1 |
| win-w2-light-Sqlite-1 | **FAIL** | 117 | 98.9 | 58 | 0 | 83 | 3 | 0 | 125.8 |
| win-w2-light-Sqlite-2 | **FAIL** | 111 | 77.0 | 57 | 0 | 75 | 3 | 0 | 86.4 |
| win-w2-light-Sqlite-3 | **FAIL** | 127 | 98.2 | 64 | 0 | 83 | 3 | 0 | 122.7 |

#### W2 heavy (commit c71df76)

| Run | Result | Saves | Saves/s | 409 | 500 | Lost | Missing | Unreadable | Save p95 ms |
|---|---|---|---|---|---|---|---|---|---|
| win-w2-heavy-AppendLog-1 | pass | 1978 | 44.2 | 1171 | 0 | 0 | 0 | 0 | 209.3 |
| win-w2-heavy-AppendLog-2 | pass | 1983 | 44.2 | 1148 | 0 | 0 | 0 | 0 | 209.3 |
| win-w2-heavy-AppendLog-3 | pass | 1953 | 33.0 | 979 | 0 | 0 | 0 | 0 | 237.2 |
| win-w2-heavy-LockedFile-1 | pass | 2151 | 18.5 | 979 | 0 | 0 | 0 | 0 | 538.8 |
| win-w2-heavy-LockedFile-2 | pass | 2156 | 20.3 | 992 | 0 | 0 | 0 | 0 | 504.1 |
| win-w2-heavy-LockedFile-3 | pass | 2221 | 18.3 | 943 | 1 | 0 | 0 | 0 | 438.4 |
| win-w2-heavy-NaiveFile-1 | **FAIL** | 168 | 14.8 | 157 | 1083 | 6 | 3 | 0 | 125.4 |
| win-w2-heavy-NaiveFile-2 | **FAIL** | 200 | 13.8 | 177 | 2263 | 11 | 0 | 0 | 157.2 |
| win-w2-heavy-NaiveFile-3 | **FAIL** | 160 | 16.5 | 166 | 1083 | 10 | 3 | 0 | 152.2 |
| win-w2-heavy-PerRecordFile-1 | pass | 2362 | 65.2 | 1119 | 136 | 0 | 0 | 0 | 152.2 |
| win-w2-heavy-PerRecordFile-2 | pass | 1716 | 67.6 | 1207 | 111 | 0 | 0 | 0 | 175.9 |
| win-w2-heavy-PerRecordFile-3 | pass | 1743 | 71.3 | 1197 | 141 | 0 | 0 | 0 | 171.1 |
| win-w2-heavy-Sqlite-1 | **FAIL** | 813 | 108.5 | 771 | 0 | 386 | 6 | 1 | 79.8 |
| win-w2-heavy-Sqlite-2 | **FAIL** | 510 | 66.2 | 469 | 0 | 84 | 6 | 1 | 129.9 |
| win-w2-heavy-Sqlite-3 | **FAIL** | 504 | 45.9 | 510 | 9 | 67 | 6 | 1 | 379.4 |

#### W2 realistic pipeline (commit fe53fea)

| Run | Result | Saves | Saves/s | 409 | 500 | Lost | Missing | Unreadable | Save p95 ms | Stale reads | Longest stall s |
|---|---|---|---|---|---|---|---|---|---|---|---|
| win-w2-real-AppendLog-1 | pass | 747 | 5.8 | 19 | 0 | 0 | 0 | 0 | 80.7 | 0 |  |
| win-w2-real-AppendLog-2 | pass | 718 | 5.5 | 21 | 0 | 0 | 0 | 0 | 84.2 | 0 |  |
| win-w2-real-AppendLog-3 | pass | 732 | 5.7 | 16 | 0 | 0 | 0 | 0 | 69.4 | 0 |  |
| win-w2-real-LockedFile-1 | pass | 733 | 5.5 | 6 | 0 | 0 | 0 | 0 | 122.5 | 0 |  |
| win-w2-real-LockedFile-2 | pass | 729 | 5.4 | 11 | 0 | 0 | 0 | 0 | 124.7 | 0 |  |
| win-w2-real-LockedFile-3 | pass | 755 | 5.8 | 13 | 0 | 0 | 0 | 0 | 120.6 | 0 |  |
| win-w2-real-NaiveFile-1 | **FAIL** | 706 | 5.5 | 4 | 26 | 4 | 1 | 0 | 40.5 | 2 |  |
| win-w2-real-NaiveFile-2 | **FAIL** | 701 | 5.4 | 4 | 39 | 4 | 1 | 0 | 44.1 | 4 |  |
| win-w2-real-NaiveFile-3 | **FAIL** | 692 | 5.4 | 8 | 37 | 4 | 1 | 0 | 53.8 | 4 |  |
| win-w2-real-PerRecordFile-1 | pass | 728 | 5.7 | 36 | 2 | 0 | 0 | 0 | 73.0 | 0 |  |
| win-w2-real-PerRecordFile-2 | pass | 731 | 5.6 | 38 | 4 | 0 | 0 | 0 | 75.6 | 0 |  |
| win-w2-real-PerRecordFile-3 | pass | 744 | 5.8 | 19 | 1 | 0 | 0 | 0 | 66.1 | 0 |  |
| win-w2-real-Sqlite-1 | **FAIL** | 381 | 2.9 | 0 | 223 | 376 | 85 | 1 | 81.9 | 250 |  |
| win-w2-real-Sqlite-2 | **FAIL** | 725 | 5.7 | 7 | 2 | 715 | 155 | 2 | 48.4 | 221 |  |
| win-w2-real-Sqlite-3 | **FAIL** | 620 | 4.9 | 8 | 72 | 615 | 140 | 2 | 40.4 | 154 |  |

#### C with the rename retry, W0 and W2 (commit 1e1cb64)

| Run | Result | Saves | Saves/s | 409 | 500 | Lost | Missing | Unreadable | Save p95 ms |
|---|---|---|---|---|---|---|---|---|---|
| win-retry-w0-heavy-PerRecordFile-1 | pass | 1671 | 101.8 | 1664 | 0 | 0 | 0 | 0 | 92.8 |
| win-retry-w0-heavy-PerRecordFile-2 | pass | 1747 | 87.7 | 1680 | 0 | 0 | 0 | 0 | 98.7 |
| win-retry-w0-heavy-PerRecordFile-3 | pass | 1729 | 94.1 | 1696 | 0 | 0 | 0 | 0 | 95.4 |
| win-retry-w0-light-PerRecordFile-1 | pass | 169 | 87.4 | 128 | 0 | 0 | 0 | 0 | 50.4 |
| win-retry-w0-light-PerRecordFile-2 | pass | 174 | 88.9 | 119 | 0 | 0 | 0 | 0 | 51.3 |
| win-retry-w0-light-PerRecordFile-3 | pass | 179 | 85.6 | 131 | 0 | 0 | 0 | 0 | 55.3 |
| win-retry-w2-heavy-PerRecordFile-1 | pass | 1751 | 31.8 | 1203 | 1 | 0 | 0 | 0 | 145.4 |
| win-retry-w2-heavy-PerRecordFile-2 | pass | 1797 | 84.8 | 1393 | 0 | 0 | 0 | 0 | 128.5 |
| win-retry-w2-heavy-PerRecordFile-3 | pass | 1745 | 81.9 | 1279 | 0 | 0 | 0 | 0 | 148.7 |
| win-retry-w2-light-PerRecordFile-1 | pass | 185 | 66.0 | 122 | 0 | 0 | 0 | 0 | 94.6 |
| win-retry-w2-light-PerRecordFile-2 | pass | 196 | 63.0 | 129 | 0 | 0 | 0 | 0 | 81.9 |
| win-retry-w2-light-PerRecordFile-3 | pass | 192 | 61.9 | 106 | 0 | 0 | 0 | 0 | 91.0 |

#### C, share leasing on (commit a8d2366)

| Run | Result | Saves | Saves/s | 409 | 500 | Lost | Missing | Unreadable | Save p95 ms | Save max ms | Longest stall s | Stale reads |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| win-lease-full-w2-heavy-PerRecordFile-1 | pass | 1800 | 80.5 | 1383 | 0 | 0 | 0 | 0 | 145.0 | 412.4 | 0.2 | 0 |
| win-lease-full-w2-heavy-PerRecordFile-2 | pass | 1780 | 31.4 | 1274 | 3 | 0 | 0 | 0 | 131.1 | 35617.5 | 24.1 | 0 |
| win-lease-full-w2-heavy-PerRecordFile-3 | pass | 1798 | 79.6 | 1390 | 0 | 0 | 0 | 0 | 142.5 | 363.8 | 0.2 | 0 |
| win-lease-full-w2-heavy-PerRecordFile-4 | pass | 1781 | 61.6 | 1313 | 0 | 0 | 0 | 0 | 203.4 | 501.7 | 0.2 | 0 |
| win-lease-full-w2-heavy-PerRecordFile-5 | pass | 1930 | 20.4 | 1107 | 7 | 0 | 0 | 0 | 134.8 | 35643.2 | 34.4 | 0 |
| win-lease-full-w2-heavy-PerRecordFile-6 | pass | 1786 | 82.2 | 1288 | 0 | 0 | 0 | 0 | 145.6 | 461.6 | 0.2 | 0 |
| win-lease-full-w2-real-PerRecordFile-1 | pass | 712 | 5.6 | 27 | 0 | 0 | 0 | 0 | 48.6 | 195.4 | 1.0 | 0 |
| win-lease-full-w2-real-PerRecordFile-2 | pass | 734 | 5.7 | 43 | 0 | 0 | 0 | 0 | 45.5 | 185.2 | 1.4 | 0 |
| win-lease-full-w2-real-PerRecordFile-3 | pass | 742 | 5.9 | 34 | 0 | 0 | 0 | 0 | 51.1 | 128.5 | 1 | 0 |

#### C, share leasing off (commit a8d2366)

| Run | Result | Saves | Saves/s | 409 | 500 | Lost | Missing | Unreadable | Save p95 ms | Save max ms | Longest stall s | Stale reads |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| win-lease-none-w2-heavy-PerRecordFile-1 | pass | 1797 | 42.0 | 1223 | 1 | 0 | 0 | 0 | 303.7 | 1289.2 | 0.2 | 0 |
| win-lease-none-w2-heavy-PerRecordFile-2 | pass | 1843 | 44.9 | 1276 | 1 | 0 | 0 | 0 | 264.8 | 1053.0 | 0.3 | 0 |
| win-lease-none-w2-heavy-PerRecordFile-3 | pass | 1817 | 45.1 | 1242 | 2 | 0 | 0 | 0 | 262 | 1066.3 | 0.4 | 0 |
| win-lease-none-w2-heavy-PerRecordFile-4 | pass | 1836 | 41.6 | 1265 | 0 | 0 | 0 | 0 | 298.1 | 963.3 | 0.2 | 0 |
| win-lease-none-w2-heavy-PerRecordFile-5 | pass | 1838 | 38.9 | 1267 | 6 | 0 | 0 | 0 | 307.7 | 1910.4 | 0.3 | 0 |
| win-lease-none-w2-heavy-PerRecordFile-6 | pass | 1788 | 40.0 | 1236 | 10 | 0 | 0 | 0 | 290.4 | 1666.7 | 0.2 | 0 |
| win-lease-none-w2-real-PerRecordFile-1 | pass | 729 | 5.7 | 32 | 0 | 0 | 0 | 0 | 63.8 | 158.5 | 1.0 | 0 |
| win-lease-none-w2-real-PerRecordFile-2 | pass | 732 | 5.5 | 45 | 0 | 0 | 0 | 0 | 68.5 | 257.6 | 1.4 | 0 |
| win-lease-none-w2-real-PerRecordFile-3 | pass | 739 | 5.8 | 42 | 0 | 0 | 0 | 0 | 61.0 | 164.6 | 1.4 | 0 |

#### F1: ws2's server killed at 40 s (commit 22febeb)

| Run | Result | Records | Saves | 500 | Refused | No answer | Lost | Missing | Lock check | Stale reads | Longest stall s | Save max ms |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| win-f1-w2-real-PerRecordFile-1 | pass | 110 | 496 | 0 | 159 | 2 | 0 | 0 | 110/110 | 0 | 1.3 | 297.5 |
| win-f1-w2-real-PerRecordFile-2 | pass | 107 | 487 | 0 | 161 | 1 | 0 | 0 | 107/107 | 0 | 3.8 | 171.5 |
| win-f1-w2-real-PerRecordFile-3 | pass | 113 | 479 | 0 | 159 | 1 | 0 | 0 | 113/113 | 0 | 1.4 | 304.4 |

#### F2: ws1's server killed at 40 s (commit 22febeb)

| Run | Result | Records | Saves | 500 | Refused | No answer | Lost | Missing | Lock check | Stale reads | Longest stall s | Save max ms |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| win-f2-w2-real-PerRecordFile-1 | pass | 108 | 494 | 0 | 160 | 0 | 0 | 0 | 108/108 | 0 | 1.3 | 335.2 |
| win-f2-w2-real-PerRecordFile-2 | pass | 110 | 494 | 0 | 151 | 0 | 0 | 0 | 110/110 | 0 | 1.3 | 519.9 |
| win-f2-w2-real-PerRecordFile-3 | pass | 113 | 497 | 0 | 158 | 0 | 0 | 0 | 113/113 | 0 | 1.2 | 529.9 |

#### F3: ws2's network cut for 10 s at 40 s (commit 22febeb)

| Run | Result | Records | Saves | 500 | Refused | No answer | Lost | Missing | Lock check | Stale reads | Longest stall s | Save max ms |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| win-f3-w2-real-PerRecordFile-1 | pass | 164 | 749 | 0 | 0 | 0 | 0 | 0 | 328/328 | 0 | 1.3 | 14238.3 |
| win-f3-w2-real-PerRecordFile-2 | pass | 159 | 728 | 1 | 0 | 0 | 0 | 0 | 318/318 | 0 | 1.3 | 284.5 |
| win-f3-w2-real-PerRecordFile-3 | pass | 168 | 729 | 0 | 0 | 0 | 0 | 0 | 336/336 | 0 | 1.3 | 15511.4 |

#### F4: ws2's network cut for 60 s at 40 s (commit 22febeb)

| Run | Result | Records | Saves | 500 | Refused | No answer | Lost | Missing | Lock check | Stale reads | Longest stall s | Save max ms |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| win-f4-w2-real-PerRecordFile-1 | pass | 163 | 722 | 4 | 0 | 1 | 0 | 0 | 326/326 | 0 | 1.2 | 167.2 |
| win-f4-w2-real-PerRecordFile-2 | pass | 158 | 715 | 2 | 0 | 1 | 0 | 0 | 316/316 | 0 | 1.3 | 183.3 |
| win-f4-w2-real-PerRecordFile-3 | pass | 167 | 741 | 6 | 0 | 0 | 0 | 0 | 334/334 | 0 | 1.3 | 152.0 |

#### Volume: 6,000 prefilled records, list timed (commit b3d3ed7)

| Run | Result | Records | Saves | 500 | Lost | Missing | List p50 ms | List p95 ms | Save p95 ms | Save max ms | Stale reads | Longest stall s |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| win-vol-w2-real-PerRecordFile-1 | pass | 6164 | 726 | 0 | 0 | 0 | 99.3 | 244.6 | 74.4 | 25567.5 | 0 | 7.6 |
| win-vol-w2-real-PerRecordFile-2 | pass | 6159 | 738 | 0 | 0 | 0 | 64.1 | 235.8 | 153.8 | 15312.4 | 0 | 9.1 |
| win-vol-w2-real-PerRecordFile-3 | pass | 6168 | 737 | 0 | 0 | 0 | 64.1 | 239.3 | 71.3 | 19949.3 | 0 | 6.7 |

#### Soak: one hour, 6,000 prefilled records (commit 7d9ce59, code as b3d3ed7)

| Run | Result | Records | Saves | 500 | Lost | Missing | List p50 ms | List p95 ms | Save p95 ms | Save max ms | Stale reads | Longest stall s |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| win-soak-w2-PerRecordFile-1 | pass | 10401 | 19828 | 0 | 0 | 0 | 123.9 | 303.8 | 63.0 | 27121.3 | 0 | 6.4 |

