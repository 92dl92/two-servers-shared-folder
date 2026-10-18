# Experiment results: local disk (phase 5)

Every storage strategy run by the stress tool (`webapp/tools/BlazorApp.Stress`) with three
real server processes sharing one data folder on local disk. The method and every check
are in `docs/strategies/StressTool.md`. Each run's `result.json` and `summary.md` are in
`local-disk/<size>-<strategy>-<seed>/`, with paths made relative to the repository. The
runs' data and log folders (`stress-runs/`) were not kept.

- **Commit:** `ee6af83` (clean tree), the phase 5 branch with `run-cluster.sh` added. The
  app and the strategies are as merged at the end of phase 4b.
- **Machine:** `linux-host`, Debian GNU/Linux 12, 16 CPUs, .NET 10.0.12, local ext4 disk. All
  runs were on 2026-10-17.
- **Settings:**
  - **light:** `--servers 3 --clients 5 --records 6 --ops 40`
  - **heavy:** `--servers 3 --clients 8 --records 12 --ops 200`

  Both were run with seeds 1, 2 and 3. A seed fixes each client's choices; the
  interleaving of requests (and so the numbers) still varies between runs.
- **Columns:**
  - **Saves:** acknowledged changes, including creates.
  - **409 / 422:** refusals because of an out-of-date version, or a status that doesn't
    allow the change.
  - **Lost:** acknowledged changes not found at their version.
  - **Unreadable sources:** of the four read-backs (the folder through the strategy, and
    each server's API), how many couldn't be read at all.
  - **p95:** save latency.

## Known-answer check (plan section 6, phase 5)

The stress tool has to tell a broken strategy from a correct one before its other results
count:

- **A `NaiveFile` (the control):** failed **6 of 6** runs, in both of the ways plan
  section 3 predicts.
  - **Lost updates (4 runs):** 14, 16, 44 and 39 acknowledged changes lost. Whole
    records vanished: in a light run, 5 of 6 acknowledged creates were overwritten by a
    concurrent whole-file write.
  - **A permanently broken file (2 runs):** `records.json` was left with the tail of a
    longer write after its closing `]` ("']' is invalid after a single JSON value"). All
    four read-backs were unreadable, and in heavy seed 1, 4,316 of 4,800 reads answered 500.
- **E `Sqlite` (the known good):** passed **6 of 6**, with nothing lost and nothing broken.

So the tool tells them apart. The same rule as the shared storage suite applies (decision
in #18): for the control, **lost updates or a broken file** is the expected failure.

## Results

### light

| Strategy | Seed | Saves | Saves/s | 409 | 422 | Lost | Unreadable sources | Rules broken | p95 ms | Passed |
|---|---|---|---|---|---|---|---|---|---|---|
| A NaiveFile | 1 | 39 | 87.2 | 27 | 26 | 14 | 0 | 0 | 117.6 | no |
| A NaiveFile | 2 | 37 | 78.7 | 19 | 12 | 0 | 4 | 0 | 109.1 | no |
| A NaiveFile | 3 | 39 | 93.4 | 25 | 26 | 16 | 0 | 0 | 102.1 | no |
| B LockedFile | 1 | 275 | 76.4 | 150 | 181 | 0 | 0 | 0 | 76.7 | yes |
| B LockedFile | 2 | 279 | 72.8 | 171 | 156 | 0 | 0 | 0 | 96.0 | yes |
| B LockedFile | 3 | 279 | 72.5 | 158 | 169 | 0 | 0 | 0 | 115.3 | yes |
| C PerRecordFile | 1 | 225 | 276.8 | 244 | 137 | 0 | 0 | 0 | 22.9 | yes |
| C PerRecordFile | 2 | 242 | 288.1 | 242 | 122 | 0 | 0 | 0 | 22.2 | yes |
| C PerRecordFile | 3 | 237 | 279.7 | 245 | 124 | 0 | 0 | 0 | 19.0 | yes |
| D AppendLog | 1 | 226 | 332.5 | 235 | 145 | 0 | 0 | 0 | 22.8 | yes |
| D AppendLog | 2 | 232 | 322.5 | 215 | 159 | 0 | 0 | 0 | 15.8 | yes |
| D AppendLog | 3 | 232 | 330.4 | 203 | 171 | 0 | 0 | 0 | 17.1 | yes |
| E Sqlite | 1 | 308 | 170.5 | 156 | 142 | 0 | 0 | 0 | 24.8 | yes |
| E Sqlite | 2 | 300 | 158.8 | 131 | 175 | 0 | 0 | 0 | 31.2 | yes |
| E Sqlite | 3 | 354 | 160.7 | 158 | 94 | 0 | 0 | 0 | 28.5 | yes |

### heavy

| Strategy | Seed | Saves | Saves/s | 409 | 422 | Lost | Unreadable sources | Rules broken | p95 ms | Passed |
|---|---|---|---|---|---|---|---|---|---|---|
| A NaiveFile | 1 | 23 | 8.1 | 4 | 1 | 0 | 4 | 0 | 116.3 | no |
| A NaiveFile | 2 | 133 | 81.5 | 110 | 154 | 44 | 0 | 0 | 114.6 | no |
| A NaiveFile | 3 | 124 | 82.8 | 111 | 159 | 39 | 0 | 0 | 109.5 | no |
| B LockedFile | 1 | 1847 | 50.4 | 963 | 2002 | 0 | 0 | 0 | 196.5 | yes |
| B LockedFile | 2 | 1895 | 56.7 | 920 | 1997 | 0 | 0 | 0 | 179.4 | yes |
| B LockedFile | 3 | 1873 | 55.8 | 965 | 1974 | 0 | 0 | 0 | 175.5 | yes |
| C PerRecordFile | 1 | 1438 | 429.8 | 1440 | 1934 | 0 | 0 | 0 | 19.4 | yes |
| C PerRecordFile | 2 | 1430 | 419.2 | 1409 | 1973 | 0 | 0 | 0 | 21.5 | yes |
| C PerRecordFile | 3 | 1390 | 406.2 | 1381 | 2041 | 0 | 0 | 0 | 20.3 | yes |
| D AppendLog | 1 | 1467 | 292.4 | 1200 | 2145 | 0 | 0 | 0 | 38.9 | yes |
| D AppendLog | 2 | 1494 | 278.4 | 1217 | 2101 | 0 | 0 | 0 | 42.8 | yes |
| D AppendLog | 3 | 1493 | 231.1 | 1283 | 2036 | 0 | 0 | 0 | 47.8 | yes |
| E Sqlite | 1 | 2009 | 224.8 | 691 | 2112 | 0 | 0 | 0 | 28.6 | yes |
| E Sqlite | 2 | 2106 | 218.3 | 708 | 1998 | 0 | 0 | 0 | 28.5 | yes |
| E Sqlite | 3 | 2061 | 222.9 | 689 | 2062 | 0 | 0 | 0 | 27.6 | yes |

### Per strategy (median of 3 runs)

| Strategy | Size | Passed | Median saves/s | Median p95 ms | Median 409 | Lost (per run) | Unreadable (per run) |
|---|---|---|---|---|---|---|---|
| A NaiveFile | light | 0/3 | 87.2 | 109.1 | 25 | 14, 0, 16 | 0, 4, 0 |
| A NaiveFile | heavy | 0/3 | 81.5 | 114.6 | 110 | 0, 44, 39 | 4, 0, 0 |
| B LockedFile | light | 3/3 | 72.8 | 96 | 158 | 0, 0, 0 | 0, 0, 0 |
| B LockedFile | heavy | 3/3 | 55.8 | 179.4 | 963 | 0, 0, 0 | 0, 0, 0 |
| C PerRecordFile | light | 3/3 | 279.7 | 22.2 | 244 | 0, 0, 0 | 0, 0, 0 |
| C PerRecordFile | heavy | 3/3 | 419.2 | 20.3 | 1409 | 0, 0, 0 | 0, 0, 0 |
| D AppendLog | light | 3/3 | 330.4 | 17.1 | 215 | 0, 0, 0 | 0, 0, 0 |
| D AppendLog | heavy | 3/3 | 278.4 | 42.8 | 1217 | 0, 0, 0 | 0, 0, 0 |
| E Sqlite | light | 3/3 | 160.7 | 28.5 | 156 | 0, 0, 0 | 0, 0, 0 |
| E Sqlite | heavy | 3/3 | 222.9 | 28.5 | 691 | 0, 0, 0 | 0, 0, 0 |

`Saves/s` and `p95` for A describe a store that broke partway through: A made far fewer
writes than the other strategies, because its records vanished or became unreadable. Don't
compare them with B–E.

## Hypotheses (plan section 3) against what was seen

| Strategy | Predicted | Seen on local disk |
|---|---|---|
| A NaiveFile | One of two concurrent saves is lost; a crash mid-write can leave a half-written file | ✅ Both, without any crash: lost updates in 4 runs and a permanently unreadable file in 2. Concurrent in-place writes are enough to break it. |
| B LockedFile | Nothing lost, but every save waits its turn | ✅ Nothing lost. Slowest of B–E, and it degrades with load: median 73 → 56 saves/s, p95 96 → 179 ms from light to heavy, because every save in every server queues for one lock. |
| C PerRecordFile | No half-written files; servers wait for each other only when they touch the same record | ✅ Nothing lost or broken. Fastest under heavy load (median 419 saves/s), and p95 stays about 20 ms in both sizes. It scales with the number of records. |
| D AppendLog | Do appends from several processes ever interleave? Reads get slower as the log grows | ✅ No interleaved or lost appends. Reads grow with the log: the heavy runs have a lower throughput than the light (median 330 → 278 saves/s) and a higher p95 (17 → 43 ms). |
| E Sqlite | The known good on local disk | ✅ Nothing lost or broken, with a steady p95 of about 28 ms. Fewer 409s than C or D at the same settings: probably because SQLite's busy waits serialise writers more, so fewer requests start from a version that has just changed (not measured). |

## Contention sweep

Phase 5's two sizes had about the same **contention** (clients per record: 2.5 and 2).
The sweep keeps the load fixed (3 servers × 8 clients, all writing) and changes only the
number of records, from 1 (every client on one record) to 96. One extra point adds servers:
5 × 8 clients on 20 records.

- **Commit:** `f161ef4` (clean tree). This commit has the stress tool change that allows
  fewer records than registrants. With fewer records, the surplus registrants run as
  reviewers (`StressTool.md`; plan decision "Contention sweep"). From 12 records up,
  the users are the same as in the phase 5 runs.
- **Machine and date:** as above, 2026-10-17.
- **Settings:** `--servers 3 --clients 8 --ops 200` with `--records 1, 3, 12, 24, 48, 96`,
  plus `--servers 5 --clients 8 --records 20 --ops 200`, seeds 1–3, all five strategies:
  105 runs. Results in `local-disk/sweep-<servers>x<clients>-r<records>-<strategy>-<seed>/`.
- **Role mix:** registrants / reviewers / admins are 1/20/3 at 1 record, 3/18/3 at 3, and
  12/9/3 from 12 up; 20/15/5 on 5 × 8. With few records, the records reach Approved or
  Rejected early, so most saves are comments (at 1 record, 298 of E's 301 saves in seed 1).
  A comment is a full read-modify-write of the record in every strategy, so it still
  collides like any other change.

### Correctness

| Strategy | r1 | r3 | r12 | r24 | r48 | r96 | 5×8 r20 |
|---|---|---|---|---|---|---|---|
| A NaiveFile | 0/3 | 0/3 | 0/3 | 0/3 | 0/3 | 0/3 | 0/3 |
| B LockedFile | 3/3 | 3/3 | 3/3 | 3/3 | 3/3 | 3/3 | 3/3 |
| C PerRecordFile | 3/3 | 3/3 | 3/3 | 3/3 | 3/3 | 3/3 | 3/3 |
| D AppendLog | 3/3 | 3/3 | 3/3 | 3/3 | 3/3 | 3/3 | 3/3 |
| E Sqlite | 3/3 | 3/3 | 3/3 | 3/3 | 3/3 | 3/3 | 3/3 |

- **B–E passed all 84 runs**, including the worst case: 24 clients on one record, with
  medians of 3,400–4,300 409s per run. Nothing was lost or broken, and no rule was broken.
  Each 409 is a race the version check caught.
- **A failed all 21 runs.** 18 left `records.json` unreadable. 3 lost acknowledged changes
  (43, 225 and 35). The broken file is A's usual failure once 24 or more clients write.

### Throughput and latency (median of 3 runs)

| Strategy | r1 | r3 | r12 | r24 | r48 | r96 | 5×8 r20 |
|---|---|---|---|---|---|---|---|
| **saves/s** | | | | | | | |
| B LockedFile | 36 | 51 | 58 | 59 | 54 | 52 | 41 |
| C PerRecordFile | 43 | 146 | 434 | 582 | 756 | 914 | 338 |
| D AppendLog | 37 | 135 | 296 | 459 | 622 | 759 | 286 |
| E Sqlite | 54 | 156 | 225 | 233 | 240 | 245 | 204 |
| **p95 ms** | | | | | | | |
| B LockedFile | 38 | 68 | 174 | 276 | 399 | 522 | 427 |
| C PerRecordFile | 32 | 23 | 20 | 21 | 20 | 20 | 52 |
| D AppendLog | 44 | 25 | 40 | 38 | 42 | 50 | 80 |
| E Sqlite | 27 | 23 | 28 | 32 | 35 | 38 | 43 |
| **409s** | | | | | | | |
| B LockedFile | 3434 | 2374 | 963 | 657 | 392 | 225 | 1699 |
| C PerRecordFile | 3948 | 3489 | 1425 | 900 | 579 | 346 | 2412 |
| D AppendLog | 4179 | 3161 | 1195 | 787 | 509 | 328 | 2058 |
| E Sqlite | 4280 | 3042 | 683 | 382 | 265 | 230 | 946 |

A is left out: its numbers describe a store that broke partway through (per-run tables
below).

### What the sweep shows

- **B is capped by its single lock.** It stays at 36–59 saves/s however many records
  there are. More records only mean more saves get through the version check and queue
  for the lock, so its p95 grows from 38 to 522 ms.
- **C's advantage really is per-record locking** (plan section 3). On one record, C does
  43 saves/s, about the same as B's 36. It scales with the number of records, to 914
  saves/s at 96, while its p95 stays about 20 ms.
- **D scales nearly as well as C**, to 759 saves/s at 96 records. Appending to one log
  doesn't serialise it the way B's lock does. Its p95 grows with the log, as in phase 5.
- **E levels off at about 240 saves/s** from 12 records up, because SQLite lets one writer
  in at a time. With few records it is the fastest: 54 saves/s on one record, and
  156 on three. On one record, its busy wait queues writers instead of letting them
  race and fail. It also has the fewest 409s from 12 records up, as phase 5 noted.
- **409s follow contention for every strategy:** about 4,000 per run on one record, down
  to 230–350 on 96. They are refused by the app's version rule, not by the storage.
- **More servers cost every strategy.** 5 × 8 on 20 records has more clients per record
  than 3 × 8 on 24 (2 against 1). Still, the drop is larger than contention alone
  explains: B 59 → 41, C 582 → 338, D 459 → 286 saves/s, and C's p95 is 21 → 52 ms. With
  40 clients and 5 servers on 16 CPUs, the machine itself becomes part of the
  bottleneck. This point doesn't separate the two causes.
- **Reproducible:** the r12 point repeats phase 5's heavy settings on a later commit.
  The medians agree within about 6%: B 58 against 56, C 434 against 419, D 296 against
  278, and E 225 against 223 saves/s.

<details>
<summary>Every sweep run (105)</summary>


#### 3 × 8 on 1 record(s); registrants/reviewers/admins 1/20/3

| Strategy | Seed | Saves | Saves/s | 409 | 422 | Lost | Unreadable sources | Rules broken | p95 ms | Passed |
|---|---|---|---|---|---|---|---|---|---|---|
| A NaiveFile | 1 | 270 | 58.43 | 904 | 27 | 0 | 4 | 0 | 32.5 | no |
| A NaiveFile | 2 | 70 | 19.83 | 180 | 4 | 0 | 4 | 0 | 22.03 | no |
| A NaiveFile | 3 | 11 | 3.86 | 0 | 0 | 0 | 4 | 0 | 88.54 | no |
| B LockedFile | 1 | 1308 | 37.52 | 3306 | 187 | 0 | 0 | 0 | 34.75 | yes |
| B LockedFile | 2 | 1147 | 35.84 | 3476 | 178 | 0 | 0 | 0 | 38.07 | yes |
| B LockedFile | 3 | 1185 | 36.44 | 3434 | 182 | 0 | 0 | 0 | 37.79 | yes |
| C PerRecordFile | 1 | 590 | 42.7 | 3948 | 263 | 0 | 0 | 0 | 31.75 | yes |
| C PerRecordFile | 2 | 472 | 37.06 | 4078 | 251 | 0 | 0 | 0 | 35.16 | yes |
| C PerRecordFile | 3 | 759 | 53.69 | 3744 | 298 | 0 | 0 | 0 | 24.01 | yes |
| D AppendLog | 1 | 419 | 40.92 | 4145 | 237 | 0 | 0 | 0 | 43.56 | yes |
| D AppendLog | 2 | 400 | 36.5 | 4179 | 222 | 0 | 0 | 0 | 42.42 | yes |
| D AppendLog | 3 | 382 | 32.64 | 4208 | 211 | 0 | 0 | 0 | 51.81 | yes |
| E Sqlite | 1 | 301 | 51.08 | 4280 | 220 | 0 | 0 | 0 | 28.89 | yes |
| E Sqlite | 2 | 309 | 58.65 | 4260 | 232 | 0 | 0 | 0 | 23.59 | yes |
| E Sqlite | 3 | 321 | 54.37 | 4284 | 196 | 0 | 0 | 0 | 26.66 | yes |

#### 3 × 8 on 3 record(s); registrants/reviewers/admins 3/18/3

| Strategy | Seed | Saves | Saves/s | 409 | 422 | Lost | Unreadable sources | Rules broken | p95 ms | Passed |
|---|---|---|---|---|---|---|---|---|---|---|
| A NaiveFile | 1 | 49 | 17.7 | 46 | 2 | 0 | 4 | 0 | 106.78 | no |
| A NaiveFile | 2 | 17 | 6.26 | 17 | 0 | 0 | 4 | 0 | 105.08 | no |
| A NaiveFile | 3 | 156 | 61.53 | 196 | 27 | 0 | 4 | 0 | 18.34 | no |
| B LockedFile | 1 | 1928 | 51.48 | 2374 | 501 | 0 | 0 | 0 | 68.31 | yes |
| B LockedFile | 2 | 1851 | 51.37 | 2456 | 496 | 0 | 0 | 0 | 71.52 | yes |
| B LockedFile | 3 | 1944 | 52.65 | 2341 | 518 | 0 | 0 | 0 | 67.01 | yes |
| C PerRecordFile | 1 | 784 | 151.37 | 3464 | 555 | 0 | 0 | 0 | 21.99 | yes |
| C PerRecordFile | 2 | 765 | 146.13 | 3489 | 549 | 0 | 0 | 0 | 24.58 | yes |
| C PerRecordFile | 3 | 765 | 143.23 | 3515 | 523 | 0 | 0 | 0 | 23.37 | yes |
| D AppendLog | 1 | 1044 | 135.2 | 3173 | 586 | 0 | 0 | 0 | 25.41 | yes |
| D AppendLog | 2 | 1069 | 138.93 | 3139 | 595 | 0 | 0 | 0 | 25.18 | yes |
| D AppendLog | 3 | 1050 | 135.18 | 3161 | 592 | 0 | 0 | 0 | 26.54 | yes |
| E Sqlite | 1 | 1221 | 158.86 | 2955 | 627 | 0 | 0 | 0 | 22.64 | yes |
| E Sqlite | 2 | 1055 | 147.35 | 3108 | 640 | 0 | 0 | 0 | 23.8 | yes |
| E Sqlite | 3 | 1151 | 156.1 | 3042 | 610 | 0 | 0 | 0 | 22.05 | yes |

#### 3 × 8 on 12 record(s); registrants/reviewers/admins 12/9/3

| Strategy | Seed | Saves | Saves/s | 409 | 422 | Lost | Unreadable sources | Rules broken | p95 ms | Passed |
|---|---|---|---|---|---|---|---|---|---|---|
| A NaiveFile | 1 | 110 | 71.95 | 110 | 47 | 0 | 4 | 0 | 110.95 | no |
| A NaiveFile | 2 | 12 | 4.65 | 0 | 0 | 0 | 4 | 0 | 113.55 | no |
| A NaiveFile | 3 | 12 | 4.9 | 0 | 0 | 0 | 4 | 0 | 111.16 | no |
| B LockedFile | 1 | 1857 | 56.46 | 974 | 1981 | 0 | 0 | 0 | 174.1 | yes |
| B LockedFile | 2 | 1857 | 58.39 | 931 | 2024 | 0 | 0 | 0 | 177.49 | yes |
| B LockedFile | 3 | 1828 | 57.81 | 963 | 2021 | 0 | 0 | 0 | 158.96 | yes |
| C PerRecordFile | 1 | 1441 | 431.35 | 1455 | 1916 | 0 | 0 | 0 | 19.65 | yes |
| C PerRecordFile | 2 | 1431 | 434.4 | 1405 | 1976 | 0 | 0 | 0 | 19.57 | yes |
| C PerRecordFile | 3 | 1388 | 437.19 | 1425 | 1999 | 0 | 0 | 0 | 19.68 | yes |
| D AppendLog | 1 | 1506 | 299.37 | 1140 | 2166 | 0 | 0 | 0 | 39.37 | yes |
| D AppendLog | 2 | 1498 | 294.76 | 1212 | 2102 | 0 | 0 | 0 | 40.47 | yes |
| D AppendLog | 3 | 1518 | 296.45 | 1195 | 2099 | 0 | 0 | 0 | 39.52 | yes |
| E Sqlite | 1 | 1919 | 221.05 | 646 | 2247 | 0 | 0 | 0 | 25.07 | yes |
| E Sqlite | 2 | 2232 | 224.79 | 695 | 1885 | 0 | 0 | 0 | 28.56 | yes |
| E Sqlite | 3 | 2174 | 229.5 | 683 | 1955 | 0 | 0 | 0 | 28.1 | yes |

#### 3 × 8 on 24 record(s); registrants/reviewers/admins 12/9/3

| Strategy | Seed | Saves | Saves/s | 409 | 422 | Lost | Unreadable sources | Rules broken | p95 ms | Passed |
|---|---|---|---|---|---|---|---|---|---|---|
| A NaiveFile | 1 | 38 | 15.59 | 7 | 1 | 0 | 4 | 0 | 107.13 | no |
| A NaiveFile | 2 | 150 | 96.78 | 67 | 173 | 43 | 0 | 0 | 116.18 | no |
| A NaiveFile | 3 | 12 | 5.39 | 0 | 0 | 0 | 4 | 0 | 116.24 | no |
| B LockedFile | 1 | 2104 | 58.86 | 657 | 2063 | 0 | 0 | 0 | 276.14 | yes |
| B LockedFile | 2 | 2124 | 57.19 | 692 | 2008 | 0 | 0 | 0 | 259.05 | yes |
| B LockedFile | 3 | 2165 | 58.55 | 612 | 2047 | 0 | 0 | 0 | 279.1 | yes |
| C PerRecordFile | 1 | 1874 | 582.39 | 905 | 2045 | 0 | 0 | 0 | 21.09 | yes |
| C PerRecordFile | 2 | 1897 | 587.97 | 900 | 2027 | 0 | 0 | 0 | 21.78 | yes |
| C PerRecordFile | 3 | 1870 | 561.71 | 875 | 2079 | 0 | 0 | 0 | 20.01 | yes |
| D AppendLog | 1 | 1891 | 440.14 | 776 | 2157 | 0 | 0 | 0 | 38.95 | yes |
| D AppendLog | 2 | 1925 | 461.49 | 793 | 2106 | 0 | 0 | 0 | 37.79 | yes |
| D AppendLog | 3 | 1914 | 459.31 | 787 | 2123 | 0 | 0 | 0 | 35.15 | yes |
| E Sqlite | 1 | 2459 | 232.81 | 382 | 1983 | 0 | 0 | 0 | 35.21 | yes |
| E Sqlite | 2 | 2359 | 237.72 | 358 | 2107 | 0 | 0 | 0 | 32.03 | yes |
| E Sqlite | 3 | 2372 | 231.73 | 447 | 2005 | 0 | 0 | 0 | 30.2 | yes |

#### 3 × 8 on 48 record(s); registrants/reviewers/admins 12/9/3

| Strategy | Seed | Saves | Saves/s | 409 | 422 | Lost | Unreadable sources | Rules broken | p95 ms | Passed |
|---|---|---|---|---|---|---|---|---|---|---|
| A NaiveFile | 1 | 12 | 4.85 | 0 | 0 | 0 | 4 | 0 | 115.2 | no |
| A NaiveFile | 2 | 475 | 181.99 | 93 | 385 | 225 | 0 | 0 | 29.88 | no |
| A NaiveFile | 3 | 35 | 14.9 | 0 | 0 | 0 | 4 | 0 | 107.81 | no |
| B LockedFile | 1 | 2398 | 53.44 | 390 | 2060 | 0 | 0 | 0 | 411.88 | yes |
| B LockedFile | 2 | 2448 | 53.7 | 392 | 2008 | 0 | 0 | 0 | 386.2 | yes |
| B LockedFile | 3 | 2423 | 55.62 | 392 | 2033 | 0 | 0 | 0 | 399.17 | yes |
| C PerRecordFile | 1 | 2273 | 760 | 570 | 2005 | 0 | 0 | 0 | 18.75 | yes |
| C PerRecordFile | 2 | 2340 | 737.48 | 580 | 1928 | 0 | 0 | 0 | 19.71 | yes |
| C PerRecordFile | 3 | 2320 | 755.71 | 579 | 1949 | 0 | 0 | 0 | 20.56 | yes |
| D AppendLog | 1 | 2270 | 613.36 | 509 | 2069 | 0 | 0 | 0 | 41.71 | yes |
| D AppendLog | 2 | 2365 | 640.39 | 458 | 2025 | 0 | 0 | 0 | 39.55 | yes |
| D AppendLog | 3 | 2355 | 622.01 | 542 | 1951 | 0 | 0 | 0 | 44.37 | yes |
| E Sqlite | 1 | 2918 | 240.21 | 312 | 1618 | 0 | 0 | 0 | 35.37 | yes |
| E Sqlite | 2 | 2982 | 240.87 | 256 | 1610 | 0 | 0 | 0 | 35.74 | yes |
| E Sqlite | 3 | 2741 | 240.26 | 265 | 1842 | 0 | 0 | 0 | 34.08 | yes |

#### 3 × 8 on 96 record(s); registrants/reviewers/admins 12/9/3

| Strategy | Seed | Saves | Saves/s | 409 | 422 | Lost | Unreadable sources | Rules broken | p95 ms | Passed |
|---|---|---|---|---|---|---|---|---|---|---|
| A NaiveFile | 1 | 42 | 17.31 | 0 | 0 | 0 | 4 | 0 | 109.41 | no |
| A NaiveFile | 2 | 12 | 5.59 | 0 | 0 | 0 | 4 | 0 | 111.4 | no |
| A NaiveFile | 3 | 118 | 43.72 | 1 | 2 | 0 | 4 | 0 | 105.69 | no |
| B LockedFile | 1 | 2825 | 51.87 | 225 | 1846 | 0 | 0 | 0 | 521.75 | yes |
| B LockedFile | 2 | 2915 | 53.55 | 219 | 1762 | 0 | 0 | 0 | 513.48 | yes |
| B LockedFile | 3 | 2868 | 49.1 | 228 | 1800 | 0 | 0 | 0 | 579.28 | yes |
| C PerRecordFile | 1 | 2863 | 958.68 | 357 | 1676 | 0 | 0 | 0 | 18.94 | yes |
| C PerRecordFile | 2 | 2864 | 914.37 | 343 | 1689 | 0 | 0 | 0 | 20.11 | yes |
| C PerRecordFile | 3 | 2850 | 894.01 | 346 | 1700 | 0 | 0 | 0 | 19.9 | yes |
| D AppendLog | 1 | 2895 | 759.28 | 328 | 1673 | 0 | 0 | 0 | 49.54 | yes |
| D AppendLog | 2 | 2923 | 760.69 | 339 | 1634 | 0 | 0 | 0 | 51.09 | yes |
| D AppendLog | 3 | 2881 | 737.79 | 327 | 1688 | 0 | 0 | 0 | 48.95 | yes |
| E Sqlite | 1 | 3309 | 244.67 | 178 | 1409 | 0 | 0 | 0 | 37.3 | yes |
| E Sqlite | 2 | 3288 | 245.41 | 230 | 1378 | 0 | 0 | 0 | 38.23 | yes |
| E Sqlite | 3 | 3229 | 246.57 | 234 | 1433 | 0 | 0 | 0 | 40.12 | yes |

#### 5 × 8 on 20 record(s); registrants/reviewers/admins 20/15/5

| Strategy | Seed | Saves | Saves/s | 409 | 422 | Lost | Unreadable sources | Rules broken | p95 ms | Passed |
|---|---|---|---|---|---|---|---|---|---|---|
| A NaiveFile | 1 | 121 | 37.28 | 106 | 163 | 35 | 0 | 0 | 132.88 | no |
| A NaiveFile | 2 | 42 | 9.93 | 33 | 4 | 0 | 6 | 0 | 129.74 | no |
| A NaiveFile | 3 | 20 | 4.28 | 0 | 0 | 0 | 6 | 0 | 141.6 | no |
| B LockedFile | 1 | 2996 | 41.48 | 1750 | 3274 | 0 | 0 | 0 | 427.48 | yes |
| B LockedFile | 2 | 3099 | 40.1 | 1684 | 3237 | 0 | 0 | 0 | 428.52 | yes |
| B LockedFile | 3 | 3030 | 43.48 | 1699 | 3291 | 0 | 0 | 0 | 378.14 | yes |
| C PerRecordFile | 1 | 2381 | 405.59 | 2412 | 3227 | 0 | 0 | 0 | 40.4 | yes |
| C PerRecordFile | 2 | 2427 | 338.03 | 2381 | 3212 | 0 | 0 | 0 | 51.56 | yes |
| C PerRecordFile | 3 | 2440 | 301.32 | 2412 | 3168 | 0 | 0 | 0 | 60.01 | yes |
| D AppendLog | 1 | 2381 | 251.56 | 2129 | 3510 | 0 | 0 | 0 | 83.59 | yes |
| D AppendLog | 2 | 2525 | 292.74 | 1967 | 3528 | 0 | 0 | 0 | 72.44 | yes |
| D AppendLog | 3 | 2461 | 285.56 | 2058 | 3501 | 0 | 0 | 0 | 79.91 | yes |
| E Sqlite | 1 | 3544 | 213.71 | 819 | 3657 | 0 | 0 | 0 | 33.89 | yes |
| E Sqlite | 2 | 3260 | 203.93 | 946 | 3814 | 0 | 0 | 0 | 42.59 | yes |
| E Sqlite | 3 | 3343 | 199.23 | 959 | 3718 | 0 | 0 | 0 | 45.72 | yes |


</details>

## Notes

- **Every run still collided:** 409 counts are in the hundreds to over a thousand for B–E.
  In heavy runs, 422s are about as many as saves or more (B, C, D: more; E: within 5%), because most records reach Approved or Rejected
  early and later changes are refused by the workflow. More `--records` would keep more
  of them in play.
- **Latency includes the HTTP round trip** through the real servers, not just the store.
- **Phase 6** repeats this with the data folder on a network drive: results, and a
  comparison with these, in `network-drive.md`.
