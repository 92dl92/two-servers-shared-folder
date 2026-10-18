# Experiment results: network drive (phase 6)

The phase 5 runs repeated with the shared data folder on a NAS over NFS: first with all three
servers on one machine, then with one of the three on a second machine. The method and every
check are in `docs/strategies/StressTool.md` (servers on a second machine: its "Servers on a
second machine" section). Each run's `result.json` and `summary.md` are in
`network-drive/nfs-<one|two>-<size>-<strategy>-<seed>/`, with paths made relative to the
repository. The data and log folders were not kept.

- **Commit:** `1b49659` (clean tree), with `--remote` added to the stress tool. The app and
  the strategies are as merged at the end of phase 5.
- **Share:** Synology NAS nas.example, `/volume1/share`, NFSv3 over TCP,
  `hard` mounts, locks sent to the NAS (NLM; Linux `local_lock=none`, macOS `locks`), all
  users mapped to one NAS user, NAS-side "asynchronous" writes on (its default).
- **Machines:**
  - `linux-host`: Debian 12, 16 CPUs, .NET 10.0.12. Mount defaults: attribute cache 3–60 s.
  - `mac-host`: macOS 26.6.2, Apple silicon, 10 CPUs, .NET 10.0.12 runtime in `~/.dotnet`.
    Mount defaults: attribute cache 5–60 s (`acregmin=5, acregmax=60`), negative name
    cache on.
- **Settings:** phase 5's sizes, seeds 1–3, all five strategies:
  - **light:** `--servers 3 --clients 5 --records 6 --ops 40`
  - **heavy:** `--servers 3 --clients 8 --records 12 --ops 200`
  - **one machine:** all 3 servers on `linux-host`, `--data /mnt/share/runs/<run>/data`
  - **two machines:** as above plus `--remote user@mac.example --remote-servers 1 --remote-data
    /Volumes/share/runs/<run>/data`: 2 servers on `linux-host`, 1 on `mac-host`. The
    clients and every check run on `linux-host`; users `s3c…` are the Mac's server's clients.
  - All runs on 2026-10-17. 30-minute limit per run; the last three (D, two machines, heavy)
    had 10 minutes.
- **Not run:** C two machines heavy seeds 2–3 (seed 1 hung; see below). E's two-machine runs
  never started a workload, so they have no `result.json`.

## Summary

| Strategy | Local disk (phase 5) | NFS, one machine | NFS, two machines (Linux + macOS) |
|---|---|---|---|
| A NaiveFile | failed 6/6 | failed 6/6 | failed 6/6 |
| B LockedFile | passed 6/6 | passed 6/6 | **failed 6/6**: unreadable file (4 runs), lost updates (2 runs) |
| C PerRecordFile | passed 6/6 | passed 6/6 | **failed 3/3** light (8–23 lost); heavy **hung** (Linux servers locked out) |
| D AppendLog | passed 6/6 | passed 6/6 | **failed 6/6**: 130–1,476 lost per run |
| E Sqlite | passed 6/6 | passed 6/6 | **couldn't run 6/6**: the Mac's server can't open the database |

**Known answer:** on the share from one machine, A failed 6/6 (an unreadable
`records.json` every time) and E passed 6/6, so the tool tells them apart there too.

**Speed, one machine (median saves/s, p95 ms):**

| Strategy | Local light | NFS light | Local heavy | NFS heavy |
|---|---|---|---|---|
| B LockedFile | 72.8 / 96 | 17.2 / 333 | 55.8 / 179 | 5.8 / 1,843 |
| C PerRecordFile | 279.7 / 22 | 114.8 / 60 | 419.2 / 20 | 96.0 / 86 |
| D AppendLog | 330.4 / 17 | 74.7 / 103 | 278.4 / 43 | 71.0 / 173 |
| E Sqlite | 160.7 / 29 | 23.4 / 245 | 222.9 / 29 | 21.0 / 458 |

Every strategy is 2.4–11× slower on the share; B falls furthest (one file rewritten whole
under one lock, now over the network), and C, with small per-record files, keeps most of
its lead. Two-machine speeds aren't compared: none of those runs passed.

## Why the two-machine runs fail: three macOS NFS client behaviours

Each was confirmed with a small Python test against the same share (below), separately
from the stress runs.

1. **Stale reads under a lock.** Linux appended a line under an exclusive lock, flushed and
   released it. The Mac then opened the file fresh, took the lock, and still saw the old
   size and contents, for three appends in a row (12 bytes when the NAS had 24). Taking a
   lock doesn't make the macOS client revalidate its cached attributes or data; Linux's
   does (which is why every strategy passed with all servers on Linux).
2. **Exclusive locks aren't exclusive within one process.** On the share, two handles in the
   same Mac process both got `flock(LOCK_EX)` on one file; on the Mac's local disk the
   second was refused (errno 35). macOS implements `flock` on NFS with locks held at the
   NAS per process, so threads of one server don't exclude each other. Between machines the
   locks do work: while Linux held either lock type, the Mac was refused, and the other way
   round.
3. **A busy lock has a different error code.** macOS reports "would block" as errno 35;
   Linux as 11. B, C and D retry only on 11 (`IsLockHeld`), so on the Mac a busy lock is an
   error (an HTTP 500), not a wait.

### What each strategy did with that

- **D AppendLog: the Mac overwrites Linux's lines.** Under the lock the Mac reads a stale,
  shorter log, so its version check passes on old state, and it appends at the old end of
  the file, on top of lines Linux had just written. Both servers acknowledged the same
  version. In the listed examples, most lost changes were acknowledged by Linux and their
  slot now holds a Mac change; the rest are records whose create line was overwritten
  (`record missing`).
- **C PerRecordFile: the Mac's threads race each other.** Lost changes in the light runs are
  mostly a Mac change replaced by another Mac change (finding 2), and the Mac answered 500
  for "file not found" and "Stale NFS file handle" on record files another machine had just
  renamed into place (finding 1). In the heavy run the Mac's server always had some thread
  holding every record's lock (44 lock handles open at the end, for 8 clients), so the NAS
  never granted them to Linux. C waits for a lock without a time limit, and the two Linux
  servers stopped answering after 15 s; the run was stopped after 13 minutes. Its logs were
  checked but not kept.
- **B LockedFile: a broken file.** In 4 of 6 runs `records.json` ended with bytes after the
  last JSON value (`'}' is invalid after a single JSON value`), A's failure: a shorter file
  written over a longer one without cutting the end off, consistent with the Mac using a
  stale file length. The other two lost 140 and 378 acknowledged changes.
- **E Sqlite: doesn't start.** The Mac's server exits with `SQLite Error 14: 'unable to open
  database file'` on the WAL database the Linux servers created. WAL keeps its index in
  shared memory (`-shm`) and requires every process to be on one machine, as `Sqlite.md`
  says; SQLite's documentation warns against network file systems. Refusing to start is the
  safe outcome: nothing is silently lost.

### Lost changes by machine

From the examples each read-back lists (at most 20 per run), classified by the server that
acknowledged the lost change and the server whose change now holds its version:

| Run | Lost | Linux→Mac | Linux→Linux | Mac→Mac | Mac→Linux | Record missing |
|---|---|---|---|---|---|---|
| C light 1 | 8 | 0 | 0 | 6 | 2 | 0 |
| C light 2 | 9 | 0 | 0 | 8 | 1 | 0 |
| C light 3 | 23 | 3 | 4 | 9 | 3 | 1 |
| D light 1 | 138 | 15 | 1 | 0 | 0 | 4 |
| D light 2 | 130 | 15 | 0 | 0 | 0 | 5 |
| D light 3 | 133 | 14 | 0 | 0 | 1 | 5 |
| D heavy 1 | 1,332 | 12 | 8 | 0 | 0 | 0 |
| D heavy 2 | 1,074 | 20 | 0 | 0 | 0 | 0 |
| D heavy 3 | 1,476 | 0 | 0 | 0 | 0 | 20 |
| B light 2 | 140 | 6 | 4 | 3 | 2 | 5 |
| B heavy 3 | 378 | 0 | 0 | 0 | 0 | 20 |

D's losses are the Mac writing over Linux; C's are mostly the Mac racing itself.
Linux→Linux losses appear only alongside Mac writes in the same run, so they are
consistent with the Mac damaging state that Linux then built on, and were never seen with
all servers on Linux.

## What this means

- **One NFS client is not the experiment.** With all servers on one machine, B–E were as
  safe as on local disk, just slower. Every failure needed a second client.
- **The strategies' safety depends on the NFS client, not just the share.** Linux's client
  revalidates when a lock is taken and its `flock` excludes other handles; macOS's does
  neither, and B, C and D all depend on both.
- **The failures are silent.** Apart from E (refuses to start) and the 500s, every lost
  change was acknowledged to its client as saved.
- **Not tested here:** two Linux machines (the likely production setup), NFSv4, SMB, and
  the Mac with attribute caching off (`noac`). These would separate "macOS's client" from
  "any second machine".

## The client tests

Run on `linux-host` and, over ssh, on `mac-host`, against the share, while no stress run was using
those files:

```python
# 1. Exclusion between machines: Linux holds the lock, the Mac tries (both flock and lockf).
fcntl.flock(f, fcntl.LOCK_EX)            # Linux, held for 12 s
fcntl.flock(g, fcntl.LOCK_EX | fcntl.LOCK_NB)   # Mac: refused, errno 35 (also with lockf)

# 2. Exclusion within one Mac process: two handles on one file.
fcntl.flock(a, fcntl.LOCK_EX | fcntl.LOCK_NB)   # Mac, share: granted
fcntl.flock(b, fcntl.LOCK_EX | fcntl.LOCK_NB)   # Mac, share: ALSO granted (local disk: errno 35)

# 3. Stale reads: Linux appends under the lock (flush, fsync, unlock); the Mac then opens,
#    locks and reads.            Linux size   Mac sees (fstat, read)
#    after line2:                12           12, line1+line2
#    after line3:                18           12, line1+line2
#    after line4:                24           12, line1+line2
```

## Every run
#### One machine, light

| Strategy | Seed | Result | Saves | Saves/s | 409 | 422 | Lost | Missing records | Unreadable sources | 500s | 404 reads | p95 ms |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| A NaiveFile | 1 | FAILED | 31 | 42.06 | 13 | 0 | 0 | 0 | 4 | 150 | 412 | 116.2 |
| A NaiveFile | 2 | FAILED | 13 | 17.31 | 1 | 0 | 0 | 0 | 4 | 508 | 84 | 120.55 |
| A NaiveFile | 3 | FAILED | 27 | 39.13 | 12 | 1 | 0 | 0 | 4 | 255 | 311 | 111.19 |
| B LockedFile | 1 | passed | 211 | 17.16 | 235 | 160 | 0 | 0 | 0 | 0 | 0 | 323.02 |
| B LockedFile | 2 | passed | 212 | 17.03 | 246 | 148 | 0 | 0 | 0 | 0 | 0 | 337.01 |
| B LockedFile | 3 | passed | 239 | 18.32 | 256 | 111 | 0 | 0 | 0 | 0 | 0 | 333.41 |
| C PerRecordFile | 1 | passed | 199 | 113.7 | 261 | 146 | 0 | 0 | 0 | 0 | 0 | 55.57 |
| C PerRecordFile | 2 | passed | 183 | 114.8 | 258 | 165 | 0 | 0 | 0 | 0 | 0 | 59.68 |
| C PerRecordFile | 3 | passed | 206 | 115.87 | 272 | 128 | 0 | 0 | 0 | 0 | 0 | 65.36 |
| D AppendLog | 1 | passed | 222 | 74.68 | 187 | 197 | 0 | 0 | 0 | 0 | 0 | 102.71 |
| D AppendLog | 2 | passed | 207 | 71.08 | 196 | 203 | 0 | 0 | 0 | 0 | 0 | 98.73 |
| D AppendLog | 3 | passed | 222 | 76.82 | 181 | 203 | 0 | 0 | 0 | 0 | 0 | 109.47 |
| E Sqlite | 1 | passed | 204 | 23.41 | 205 | 197 | 0 | 0 | 0 | 0 | 0 | 263.56 |
| E Sqlite | 2 | passed | 206 | 7.55 | 217 | 183 | 0 | 0 | 0 | 0 | 0 | 245.46 |
| E Sqlite | 3 | passed | 204 | 23.72 | 210 | 192 | 0 | 0 | 0 | 0 | 0 | 239.44 |

#### One machine, heavy

| Strategy | Seed | Result | Saves | Saves/s | 409 | 422 | Lost | Missing records | Unreadable sources | 500s | 404 reads | p95 ms |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| A NaiveFile | 1 | FAILED | 15 | 5.29 | 0 | 0 | 0 | 0 | 4 | 4739 | 58 | 124.12 |
| A NaiveFile | 2 | FAILED | 25 | 8.72 | 12 | 0 | 0 | 0 | 4 | 4303 | 472 | 113.44 |
| A NaiveFile | 3 | FAILED | 12 | 4.15 | 0 | 0 | 0 | 0 | 4 | 4800 | 0 | 117.91 |
| B LockedFile | 1 | passed | 1532 | 5.87 | 1376 | 1904 | 0 | 0 | 0 | 0 | 0 | 1761.14 |
| B LockedFile | 2 | passed | 1592 | 5.75 | 1408 | 1812 | 0 | 0 | 0 | 0 | 0 | 1849.1 |
| B LockedFile | 3 | passed | 1565 | 5.79 | 1349 | 1898 | 0 | 0 | 0 | 0 | 0 | 1842.56 |
| C PerRecordFile | 1 | passed | 1368 | 96.57 | 1415 | 2029 | 0 | 0 | 0 | 0 | 0 | 86.45 |
| C PerRecordFile | 2 | passed | 1527 | 94.1 | 1476 | 1809 | 0 | 0 | 0 | 0 | 0 | 93.81 |
| C PerRecordFile | 3 | passed | 1387 | 95.95 | 1392 | 2033 | 0 | 0 | 0 | 0 | 0 | 85.8 |
| D AppendLog | 1 | passed | 1460 | 70.98 | 1159 | 2193 | 0 | 0 | 0 | 0 | 0 | 172.56 |
| D AppendLog | 2 | passed | 1409 | 71.38 | 1118 | 2285 | 0 | 0 | 0 | 0 | 0 | 163.57 |
| D AppendLog | 3 | passed | 1417 | 70.45 | 1136 | 2259 | 0 | 0 | 0 | 0 | 0 | 172.64 |
| E Sqlite | 1 | passed | 1425 | 23.22 | 1150 | 2237 | 0 | 0 | 0 | 0 | 0 | 443.23 |
| E Sqlite | 2 | passed | 1462 | 17.48 | 1157 | 2193 | 0 | 0 | 0 | 0 | 0 | 514.5 |
| E Sqlite | 3 | passed | 1455 | 21.01 | 1148 | 2209 | 0 | 0 | 0 | 0 | 0 | 458.37 |

#### Two machines, light

| Strategy | Seed | Result | Saves | Saves/s | 409 | 422 | Lost | Missing records | Unreadable sources | 500s | 404 reads | p95 ms |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| A NaiveFile | 1 | FAILED | 33 | 20.15 | 6 | 1 | 0 | 0 | 4 | 362 | 204 | 289.68 |
| A NaiveFile | 2 | FAILED | 39 | 21.68 | 7 | 1 | 0 | 0 | 4 | 222 | 337 | 147.16 |
| A NaiveFile | 3 | FAILED | 36 | 22.61 | 7 | 6 | 0 | 0 | 4 | 380 | 177 | 196.64 |
| B LockedFile | 1 | FAILED | 23 | 5.79 | 19 | 4 | 0 | 0 | 4 | 503 | 17 | 341.15 |
| B LockedFile | 2 | FAILED | 140 | 16.3 | 131 | 80 | 140 | 6 | 0 | 13 | 242 | 302.86 |
| B LockedFile | 3 | FAILED | 68 | 12.26 | 58 | 18 | 0 | 0 | 4 | 435 | 27 | 356.92 |
| C PerRecordFile | 1 | FAILED | 255 | 43.17 | 196 | 130 | 8 | 0 | 0 | 23 | 2 | 184.14 |
| C PerRecordFile | 2 | FAILED | 245 | 45.84 | 201 | 119 | 9 | 0 | 0 | 34 | 7 | 192.38 |
| C PerRecordFile | 3 | FAILED | 264 | 42.22 | 198 | 115 | 23 | 0 | 0 | 28 | 1 | 187.89 |
| D AppendLog | 1 | FAILED | 260 | 30.94 | 158 | 120 | 138 | 1 | 0 | 1 | 67 | 328.29 |
| D AppendLog | 2 | FAILED | 289 | 29.47 | 201 | 113 | 130 | 0 | 0 | 3 | 0 | 367.05 |
| D AppendLog | 3 | FAILED | 256 | 35.66 | 160 | 131 | 133 | 1 | 0 | 4 | 55 | 239 |
| E Sqlite | 1 | couldn't run | – | – | – | – | – | – | – | – | – | – |
| E Sqlite | 2 | couldn't run | – | – | – | – | – | – | – | – | – | – |
| E Sqlite | 3 | couldn't run | – | – | – | – | – | – | – | – | – | – |

#### Two machines, heavy

| Strategy | Seed | Result | Saves | Saves/s | 409 | 422 | Lost | Missing records | Unreadable sources | 500s | 404 reads | p95 ms |
|---|---|---|---|---|---|---|---|---|---|---|---|---|
| A NaiveFile | 1 | FAILED | 35 | 4.53 | 8 | 0 | 0 | 0 | 4 | 3926 | 643 | 125.48 |
| A NaiveFile | 2 | FAILED | 8 | 1.74 | 0 | 0 | 0 | 0 | 4 | 4004 | 0 | 124.03 |
| A NaiveFile | 3 | FAILED | 8 | 1.75 | 0 | 0 | 0 | 0 | 4 | 4004 | 0 | 128.38 |
| B LockedFile | 1 | FAILED | 5 | 0.32 | 0 | 0 | 0 | 0 | 4 | 3407 | 0 | 251.36 |
| B LockedFile | 2 | FAILED | 70 | 2.94 | 55 | 12 | 0 | 0 | 4 | 4230 | 45 | 568.45 |
| B LockedFile | 3 | FAILED | 378 | 9.88 | 412 | 468 | 378 | 10 | 0 | 57 | 3097 | 568.49 |
| C PerRecordFile | 1 | stopped-hung | – | – | – | – | – | – | – | – | – | – |
| D AppendLog | 1 | FAILED | 2037 | 39.47 | 792 | 1581 | 1332 | 2 | 0 | 2 | 400 | 490.22 |
| D AppendLog | 2 | FAILED | 1769 | 39.05 | 768 | 2084 | 1074 | 1 | 0 | 3 | 188 | 416.87 |
| D AppendLog | 3 | FAILED | 2219 | 40.48 | 870 | 1524 | 1476 | 1 | 0 | 0 | 199 | 515.21 |
| E Sqlite | 1 | couldn't run | – | – | – | – | – | – | – | – | – | – |
| E Sqlite | 2 | couldn't run | – | – | – | – | – | – | – | – | – | – |
| E Sqlite | 3 | couldn't run | – | – | – | – | – | – | – | – | – | – |

