# Stress tool (`BlazorApp.Stress`), phase 4b

Design notes for the stress tool in plan section 4. The tool lives in
`webapp/tools/BlazorApp.Stress/`, its tests in `webapp/tests/BlazorApp.Stress.Tests/`.

## What a run does

1. **Start the servers.** N copies of the built `BlazorApp.Server.dll`, each a separate
   `dotnet` process on a free port (as the E2E `ServerProcess` does), all with
   `--Storage:Strategy <name> --Storage:Path <data folder>`, a generated users file
   (`--Users:Path`) and their logs in the run folder (`--Logging:File:Directory`). A server is
   ready when `GET api/users` answers. The strategy name is only passed on; nothing in the
   tool depends on which strategy runs. The tool resolves the name with `StoreStrategies`
   first, so an unknown name stops the run (exit code 2) before any server starts.
2. **Create records.** Registrant clients create `--records` drafts, each through its own
   server, spread over the servers. A registrant may only view and change its own
   records, so with fewer records than registrants, the registrants that would own none
   are made reviewers instead (one registrant per server is kept first, in turn). Every
   client still writes, so `--records 1` puts all of them on one record; the mix of
   actions shifts with the roles, which the result records (`ClientsByRole`). A
   registrant whose create failed sits out.
3. **Collide.** Every client (`--clients` per server) runs `--ops` iterations at the same
   time: read a random record (`GET`), pick a change, send it with the version just read.
   Registrants update and submit their own records; reviewers comment, approve and reject;
   admins do all of these. Nine changes in ten are ones the status read allows, so a refusal
   means someone else got there first (409); one in ten is any change the role may make, as a
   button on a stale page would be, which gives the 422 refusals. Every request carries its
   own `X-Correlation-Id` (`stress-<guid>`), so it can be found in the server logs.
4. **Read back** through every server's API (as an extra admin user, `stress-reader`), then
   stop the servers and read the folder through the strategy itself
   (`StoreStrategies.Create(strategy, path).ListAsync()`), so every stored record must
   deserialise. A strategy that keeps no data in the folder (`UsesFolder` false, i.e.
   `InMemory`) has its folder read-back marked skipped.
5. **Check** each read-back separately and write `result.json` and `summary.md`.

## How a saved change is identified afterwards

Each client acts as **its own user** (`s2c3-registrant`: server 2, client 3), from a users
file the tool writes into the run folder. A client sends one change at a time, so "the
history entry at version V of record R is action A by user U" can only have been written by
one request. Comments also carry a unique marker (`<user>:<iteration>`) as their text, and
updates write it into the form data's `Notes`, so the data can be checked to be the last
acknowledged update's (a history entry alone doesn't show that the data was written). An acknowledged change is any 2xx answer; the tool records the record id, the
version the response returned, the action, the user, the marker, the server and the
correlation id.

This avoids comparing timestamps, which a strategy might store with less precision.

## Servers on a second machine (phase 6)

`--remote user@mac.example --remote-servers 1 --remote-data /Users/user/share/run/data` runs the
last `--remote-servers` of the `--servers` on a second machine, on that machine's path to
the same shared folder as `--data`. Each run:

- copies the built Server to `~/claudetest-stress/server/` there with `rsync`, without the
  static web assets manifest (it names folders on this machine and would stop a server in
  Development; the run uses only the API), so both machines run the same commit;
- copies the users file to `~/claudetest-stress/runs/<run>/`, where the servers also log;
- starts each server with `ssh` (it prints its process id, then becomes the server,
  listening on every interface) and stops it with `kill` over ssh;
- copies the logs back into the local `logs/` folder after the servers stop.

The clients, the read-backs and every check stay on this machine. Server names in the
results are `host:port` for remote servers; the host in `--remote` must resolve for HTTP as
well (a name or address, not an ssh config alias). Each run first stops any stress server
left running there, and the remote servers run with a terminal (`ssh -tt`), so one whose
kill doesn't arrive still stops when its ssh session ends. The second machine needs key login over ssh
(`BatchMode`), the .NET 10 runtime (in `~/.dotnet` or on the path), the share mounted, and
its firewall open to the ports picked here. CI has no second machine, so the ssh path is
tested by hand; the option checks and the start command are unit-tested.

### Several remotes, and Windows (phase 6b)

`--remote` can be given more than once, each with its own `--remote-data` (in the same
order); `--remote-servers` servers run on each. When every server is remote, leave out
`--data`: after the servers stop, the tool archives the folder on the first remote with
`tar`, copies it back into `<out>/data` and checks that copy, since this machine may not see
the share at all. For example, phase 6b's layout 2 (the share on ws1's own disk):

```bash
dotnet run --project webapp/tools/BlazorApp.Stress -- --strategy PerRecordFile --servers 2 \
  --remote Administrator@ws1.example --remote-data 'C:\share\run1' \
  --remote Administrator@ws2.example --remote-data '\\ws1.example\share\run1' \
  --remote-servers 1
```

A remote whose home starts with a drive letter is treated as Windows, which needs
PowerShell as the ssh server's default shell. There:

- every command goes as `powershell -EncodedCommand` (the ssh server passes a command to its
  shell through a command line that loses nested quotes);
- files go with `scp -O` (no `rsync`), logs come back with an `scp` glob. `-O` (the legacy
  protocol) matters: in its default SFTP mode, `scp` from OpenSSH for Windows 9.5 stopped
  at exactly 200 KiB without an error. The data archive's size is also compared after the
  copy, so a short copy stops the run (exit 2) instead of showing up as lost records. `-O`
  needs OpenSSH 9.0 or later on the machine running the tool;
- each run gets its own copy of the Server, in `runs\<run>\server` (earlier runs' copies
  are removed when nothing holds them): Windows can't delete a folder that a window has as
  its current folder, which stopped runs while someone looked around on the VM;
- servers run without `-tt` (under it the console swallows their output), and a server
  keeps running when its ssh session ends, so it is stopped with `taskkill /T` on the
  PowerShell that started it, and any leftover is stopped by the next run;
- an ssh key logon has no network credentials, so before every command that touches the
  share, and before each server starts, the tool runs `~/claudetest-stress/connect.ps1` if
  that machine has one (e.g. `net use \\host\share /user:... '...'`). It is kept on the
  machine, never in the repo.

## Servers started by hand, and routing (phase 6c)

`--external http://server1:<port>,http://server2:<port>` uses servers the tool didn't start
(started with `webapp/scripts/windows/ClaudeTestServer.ps1`, for environments without ssh):
the same command run first with `--prepare --users-out <file>` writes the users file the
servers read (and any `--prefill` records), and the run then checks each server knows those
users, reads back through the servers, prints "Stop the test servers now", waits until they
stop answering, and checks the folder. `--routing random` sends each request to a server
picked at random (`sticky`, the default, keeps each client on its own server); the result
records requests per server. Details and the lab check: `StressToolWindows.md`.

## The pipeline workload (phase 6b)

`--workload pipeline` replaces steps 2 and 3 with the way the app would be used in
production (2 servers, about 10 clients, true collisions rare). `--records` is refused:
every registrant is kept, records are created as the registrants go, and the result
records how many were. `--think-ms` is refused without the pipeline.

- **Registrants** repeat: create a record, update it 0 to 2 times, submit it. Each update
  and the submit read the record first (as opening its page would) and send the version
  read. `--ops` is how many changes each registrant sends; it finishes the record it's on.
- **Reviewers and admins** take any submitted record from a shared list, read it, and, if
  it still shows as submitted, either comment (one time in three) or decide (approve three
  times in four, else reject).
  Two may pick the same record, which gives the occasional real 409. A record leaves the
  list once it is approved or rejected, by this responder or another; a read showing an
  earlier status leaves it waiting. They stop when the registrants are done and nothing is
  waiting, or after picking `--ops` records (whether or not a change was sent, so a run
  whose reads stay stale still ends).
- **Think time**: before each step every client pauses `--think-ms` (default 1000) times a
  random factor from 0.5 to 1.5.

Records are created through each registrant's own server and decided through any server,
so with two servers about half the handoffs cross from one to the other: the moment a
server's own view of a share could be out of date.

## Volume (phase 6b)

`--prefill <n>` fills the folder with `n` finished records before the servers start, as
three years of production would (about 6,000). They are made here, one at a time, through
the app's own rules (`RegistrationService`): created by a registrant, updated 3 to 8 times,
submitted, then approved or rejected by a reviewer, so 6 to 11 changes each. With a local
`--data` they go straight into it; with every server remote they're made in a local folder,
archived and unpacked on the first remote (size checked). Every check then expects each
prefilled record to still be there at the version it was left at (nothing in the workloads
changes them): one missing counts as a missing record, one at another version as a lost
update. A server's read-back lists every record but reads in full only those the run created
or changed. Refused for a strategy that keeps no data in the folder.

`--list` (pipeline): reviewers and admins open the record list before each pick, as the
Records page would. Read and list latencies (p50/p95/max) are reported, with failed lists by
outcome.

## Stale reads (phase 6b, both workloads)

Every acknowledged save is noted with the time its answer arrived here, and every read is
compared with the newest version acknowledged **before the read was sent**. A read that
returns an older version is stale, and so is one that finds no record (404) after its
create was acknowledged (the Windows SMB client caches "not found" for a few seconds); its
version read is shown as 0. Nothing is lost by that alone (a change sent from the
old version gets a 409), so stale reads are reported, not counted as a failure:
`summary.md` gives their number, how many reads followed a save through another server
(the handoffs that could be stale), and up to 20 examples with how long after the newer
version's acknowledgement they were read. Known answers: a fake API that saves changes but
always shows version 1 must be reported as stale; a pipeline through one in-memory server
must report none.

## Failure tests (phase 6b)

`--fault` causes a failure on purpose, at a time in seconds from the start of the workload,
while the clients carry on. Repeatable.

- `kill:<n>@<s>` kills server `n` (1-based, in start order) abruptly, as a crash would:
  `kill -9`, or `taskkill /F` on Windows. Its clients' requests are then refused (counted as
  `refused`, not as no response: nothing was sent, so they don't widen what the checks
  allow), and its read-back is skipped (`killed by --fault`). A server that had already died
  by itself is a missed fault; killing every server is refused.
- `run:<command>@<s>` runs a shell command here, e.g. cutting a VM's network for 10 s with
  `webapp/scripts/vm-link.sh ws2 10` (libvirt: `virsh domif-setlink` down, wait, up). A command
  already running when the workload ends is let finish before the read-back.

A fault that didn't happen (the workload ended first, the kill failed, the command exited
non-zero) means the run couldn't be done (exit 2): a failure test without its failure tests
nothing.

**Lock check.** With any fault, after the workload the reader comments once on every record
through every surviving server, 8 at a time, each read and each comment limited to 30 s and a
refusal tried up to 3 times. A record that can be read but not changed counts as **stuck**
and fails the run: a server killed or cut off while holding a record's lock could otherwise
leave it locked for ever. One that can't be read is counted as unreadable instead (nothing was
sent; missing data is the data check's business). After 20 stuck pairs the check stops, so a
share that's gone doesn't cost 30 s per record. The comments are acknowledged saves, checked
with the rest; one that got no answer adds to the unknown outcomes.

## Checks (per read-back)

| Check | Fails when |
|---|---|
| Nothing lost | A created record is missing; an acknowledged change isn't the history entry at its version (overwritten or truncated); a comment marker is found at another version than acknowledged; the form data isn't from the last update in the history |
| No broken files | The strategy can't list the folder (an exception, e.g. invalid JSON); a server can't list or return a record |
| Rules respected | History isn't versions 1..N with N the record's version; it doesn't start with Create by the owner; replaying it through `RecordRules` finds a change the status didn't allow (e.g. approve of a draft) or the user's role didn't; the final status isn't where the history leads |
| Unacknowledged entries | More history entries no ack accounts for (a write applied twice included) than requests that got no answer (a request that timed out may still have been saved) |

Records found that the workload didn't create (a reused data folder) are counted and
checked for rules only. Each problem list keeps 20 examples in the report; counts are
complete.

## Numbers

Writes, saves, 409 conflicts, 422 wrong status and no-response counted separately, plus
every write outcome (404, 403, …), saves per action, reads and failed reads, saves per
second over the whole workload (creates included), and save latency p50/p95/max/mean
(nearest-rank) in milliseconds, and the **longest stall** (phase 6b): the longest time no
save was acknowledged by any client (counting from the start of the workload to its end, so a
freeze the run never recovered from counts too), and how far into the run it began. A share
that freezes shows there (phase 6b saw 35 s, the SMB lease-break timeout), where averages
would hide it. In the pipeline it includes think time; the summary says how long a normal
pause can be.
`result.json` also records the commit (`-dirty` with
uncommitted changes), machine, start/finish times and every setting, including the seed.
`summary.md` ends with one table row for the results table in `docs/experiments/`.

The seed fixes each client's choices, but the interleaving of requests (and so the
numbers) still varies between runs; that is the point of the experiment.

## Exit codes

0 every check passed; 1 a check failed; 2 the run couldn't be done (bad options, unknown
strategy, a server that didn't start or died during the run, no record created, any other
error).

## Known answers checked in 4b

`KnownAnswerTests` start real servers with the in-memory store:

- **One server:** passes, nothing lost, every saved change found.
- **Three servers:** each server has its own memory, so every server's read-back is missing
  the four records created through the others, with their changes reported as lost, and the
  run exits with 1.

Spot-check: ignoring missing records in the lost-update check makes the three-server test
fail (exit code 0 instead of 1).

Phase 5 adds the plan's main known-answer check once strategies A and E are merged: lost
updates found with A, none with E.

## Choices

- **Records reach a final status.** Once approved or rejected, a record only takes
  comments, so long runs drift towards comments and 422s from registrants. More `--records`
  keeps more records in play.
- **No deletes.** A delete would make every later acknowledged change on that record look
  lost; the plan's workflow list for the stress tool is create, update, comment, submit,
  approve and reject.
- **Default run folder** `stress-runs/<utc time>-<strategy>-<random>/` under the current
  directory, ignored by git. The commit is read before the run writes anything, so a run
  folder elsewhere in the repository doesn't mark it `-dirty`. `--data` can point the servers at another folder (e.g. a network share in
  phase 6).
- **Servers run in Development**, like the E2E tests, and from the Server's project folder
  when the default build is used.
