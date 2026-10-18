using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Text.Json;
using BlazorApp.Services.Storage;
using BlazorApp.Shared.Dtos;
using Microsoft.Extensions.Logging.Abstractions;

namespace BlazorApp.Stress;

/// <summary>
/// One stress run (plan section 4): start the servers on one data folder, run the workload
/// through all of them at once, then read the data back through every server and through the
/// strategy itself and check it. Nothing here depends on which strategy is used.
/// </summary>
public static class StressRun
{
    /// <summary>The user every server's data is read back as; an admin sees every record.</summary>
    public const string ReaderId = "stress-reader";

    /// <summary>Roles of a server's clients, in turn: two registrants, two reviewers and an admin per five.</summary>
    private static readonly Role[] RoleCycle = [Role.Registrant, Role.Reviewer, Role.Registrant, Role.Admin, Role.Reviewer];

    /// <summary>Runs the experiment.</summary>
    /// <param name="options">The settings.</param>
    /// <param name="log">Progress messages.</param>
    /// <param name="cancellationToken">Stops the run; the servers are stopped either way.</param>
    /// <returns>The result, also written to <c>result.json</c> and <c>summary.md</c> in the run folder.</returns>
    /// <exception cref="ArgumentException">The tool doesn't know the strategy, or it needs a folder.</exception>
    /// <exception cref="InvalidOperationException">A server didn't start, or no record could be created.</exception>
    public static async Task<StressResult> RunAsync(StressOptions options, TextWriter log, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(log);
        var startedAt = DateTimeOffset.UtcNow;
        var commit = Commit(); // before this run writes any files

        // A short random suffix keeps runs started in the same second apart.
        var runFolder = Path.GetFullPath(options.OutputDirectory
            ?? Path.Combine("stress-runs", $"{startedAt:yyyyMMdd-HHmmss}-{options.Strategy}-{Guid.NewGuid().ToString("N")[..6]}"));
        var dataPath = Path.GetFullPath(options.DataPath ?? Path.Combine(runFolder, "data"));
        var logDirectory = Path.Combine(runFolder, "logs");
        Directory.CreateDirectory(dataPath);
        Directory.CreateDirectory(logDirectory);

        // The read-back below uses the same strategy, so an unknown name fails before anything starts.
        var factory = StoreStrategies.Resolve(options.Strategy, dataPath);

        // The pipeline creates as many records as its registrants get through, so keeps every one.
        var pipeline = options.Workload == StressOptions.Pipeline;
        var users = MakeUsers(options.Servers, options.ClientsPerServer, pipeline ? int.MaxValue : options.Records);
        var usersFile = Path.Combine(runFolder, "users.json");
        await File.WriteAllTextAsync(usersFile, JsonSerializer.Serialize(users, StressResult.JsonOptions), cancellationToken);

        var serverDll = Path.GetFullPath(options.ServerDll ?? ServerProcess.DefaultServerDll);
        string[] ServerArgs(string data, string users) =>
            ["--Storage:Strategy", options.Strategy, "--Storage:Path", data, "--Users:Path", users, .. options.ServerArgs];

        // The last --remote-servers servers on each remote, in order, each on its path to the same folder.
        // Prepared at once: each only touches its own machine (and makes the same data folder).
        var runName = Path.GetFileName(Path.TrimEndingDirectorySeparator(runFolder));
        log.WriteLine($"Copying the Server to {string.Join(", ", options.Remotes.Select(r => r.Target))}");
        RemoteHost[] remotes = await Task.WhenAll(options.Remotes.Select(target =>
            RemoteHost.PrepareAsync(target.Target, Path.GetDirectoryName(serverDll)!, usersFile, runName, target.DataPath, cancellationToken)));

        IReadOnlyDictionary<Guid, int> prefilled = options.Prefill > 0
            ? await PrefillAsync(options, factory, users, dataPath, runFolder, remotes, log, cancellationToken)
            : new Dictionary<Guid, int>();

        var localServers = options.Servers - (options.RemoteServers * remotes.Length);
        log.WriteLine($"Starting {options.Servers} {options.Strategy} server(s)"
            + (localServers == 0 ? "" : $", {localServers} here on {dataPath}")
            + string.Concat(options.Remotes.Select(r => $", {options.RemoteServers} on {r.Target} ({r.DataPath})")));
        var starting = Enumerable.Range(0, options.Servers)
            .Select(i =>
            {
                if (i < localServers)
                {
                    return ServerProcess.StartAsync(serverDll, logDirectory, ServerArgs(dataPath, usersFile));
                }

                var r = (i - localServers) / options.RemoteServers;
                return ServerProcess.StartAsync(serverDll, logDirectory, ServerArgs(remotes[r].DataPath, remotes[r].UsersFile), remotes[r]);
            })
            .ToList();
        var servers = new List<ServerProcess>();
        WorkloadOutcome outcome;
        List<SourceCheck> checks = [];
        FaultOutcome[] faults = [];
        LockReport? lockCheck = null;
        var unknown = 0;
        var byId = users.ToDictionary(u => u.Id);
        try
        {
            try
            {
                await Task.WhenAll(starting);
            }
            finally
            {
                servers.AddRange(starting.Where(t => t.IsCompletedSuccessfully).Select(t => t.Result));
            }

            using var handler = new SocketsHttpHandler { UseCookies = false, MaxConnectionsPerServer = 256 };
            var http = servers.Select(s => new HttpClient(handler, disposeHandler: false) { BaseAddress = s.BaseAddress, Timeout = TimeSpan.FromSeconds(60) }).ToList();
            try
            {
                var clients = servers.SelectMany((server, s) => Enumerable.Range(0, options.ClientsPerServer).Select(c =>
                {
                    var user = users[(s * options.ClientsPerServer) + c];
                    return new StressClient(new ApiClient(http[s], server.Name, user.Id), user.Roles[0]);
                })).ToList();

                // Faults wait for their time from the start of the workload; any not reached when it
                // ends are skipped, and a command already running is let finish (e.g. restoring a network).
                using var faultsEnd = new CancellationTokenSource();
                var workloadStarted = Stopwatch.GetTimestamp();
                var faulting = options.Faults.Select(f => RunFaultAsync(f, servers, workloadStarted, log, faultsEnd.Token)).ToList();
                try
                {
                    if (pipeline)
                    {
                        log.WriteLine($"Running {clients.Count} clients as a pipeline: {options.OpsPerClient} changes each, {options.ThinkMilliseconds} ms think time");
                        outcome = await Workload.RunPipelineAsync(clients, options.OpsPerClient, TimeSpan.FromMilliseconds(options.ThinkMilliseconds), options.Seed, cancellationToken, options.List);
                    }
                    else
                    {
                        log.WriteLine($"Running {clients.Count} clients: {options.Records} records, {options.OpsPerClient} changes each");
                        outcome = await Workload.RunAsync(clients, options.Records, options.OpsPerClient, options.Seed, cancellationToken);
                    }
                }
                finally
                {
                    await faultsEnd.CancelAsync();
                    faults = await Task.WhenAll(faulting);
                }

                // A failure test whose failure didn't happen tests nothing.
                if (faults.FirstOrDefault(f => !f.Happened) is { } missed)
                {
                    throw new InvalidOperationException($"Fault {missed.Fault}: {missed.Result}.");
                }

                // A server that died (e.g. it lost its port to another process) would make the run mean less than it says.
                if (servers.FirstOrDefault(s => s.HasExited && !s.Killed) is { } died)
                {
                    throw new InvalidOperationException($"Server {died.Name} exited during the run:\n{died.Output}");
                }

                log.WriteLine($"{outcome.Numbers.Saves} saves, {outcome.Numbers.Conflicts} conflicts in {outcome.Numbers.DurationSeconds:0.0} s; reading back");

                // After a fault, every record must still be changeable through every surviving
                // server; the comments it saves are acknowledged and checked with the rest.
                var surviving = Enumerable.Range(0, servers.Count).Where(s => !servers[s].Killed).ToList();
                if (options.Faults.Count > 0)
                {
                    var (report, lockAcks, lockUnknown) = await LockCheck.RunAsync(
                        [.. surviving.Select(s => new ApiClient(http[s], servers[s].Name, ReaderId))], outcome.Created, LockTimeout, cancellationToken);
                    lockCheck = report;
                    unknown += lockUnknown;
                    outcome = outcome with { Acks = [.. outcome.Acks, .. lockAcks] };
                    log.WriteLine($"Lock check: {report.Changed} of {report.Changes} changed, {report.Stuck} stuck, {report.Unreadable} unreadable{(report.StoppedEarly ? ", stopped early" : "")}");
                }

                unknown += outcome.Numbers.NoResponse;
                for (var s = 0; s < servers.Count; s++)
                {
                    if (servers[s].Killed)
                    {
                        checks.Add(new SourceCheck(true, new SourceReport { Source = $"server {servers[s].Name}", Skipped = "killed by --fault" }));
                        continue;
                    }

                    var report = await ReadBackAsync(new ApiClient(http[s], servers[s].Name, ReaderId), outcome, byId, prefilled, cancellationToken);
                    checks.Add(new SourceCheck(report.Passed(unknown), report));
                }
            }
            finally
            {
                http.ForEach(h => h.Dispose());
            }
        }
        finally
        {
            // Stop every server before reading the folder, so nothing is still writing.
            await Task.WhenAll(servers.Select(s => s.DisposeAsync().AsTask()));
            await Task.WhenAll(remotes.Select(r => r.EnsureStoppedAsync(CancellationToken.None)));
            foreach (var remote in remotes)
            {
                try
                {
                    await remote.FetchLogsAsync(logDirectory, CancellationToken.None);
                }
                catch (InvalidOperationException e)
                {
                    // The logs are diagnostics; losing them mustn't lose the run or hide why it failed.
                    log.WriteLine($"Couldn't copy the logs from {remote.Host}: {e.Message}");
                }
            }
        }

        // With no server here, this machine may not see the folder at all (a share on a remote's
        // own disk), or only through its own cache: check a copy made from the first remote.
        if (options.AllRemote)
        {
            log.WriteLine($"Copying the data folder from {remotes[0].Host}");
            await remotes[0].FetchDataAsync(dataPath, cancellationToken);
        }

        var storage = await ReadStorageAsync(factory, dataPath, outcome, byId, prefilled, cancellationToken);
        checks.Insert(0, new SourceCheck(storage.Passed(unknown), storage));

        var result = new StressResult(
            checks.All(c => c.Passed) && lockCheck?.Passed != false,
            commit,
            startedAt,
            DateTimeOffset.UtcNow,
            MachineInfo.Current,
            new StressSettings(options.Strategy, dataPath, options.Servers, options.ClientsPerServer, ClientsByRole(users), pipeline ? outcome.Created.Count : options.Records, options.OpsPerClient, options.Seed, options.ServerArgs, options.Remotes.Count == 0 ? null : options.Remotes, options.RemoteServers, options.AllRemote, options.Workload, pipeline ? options.ThinkMilliseconds : null, options.Prefill),
            runFolder,
            [.. servers.Select(s => new ServerInfo(s.Port, Path.Combine(logDirectory, $"server-{s.Port}.jsonl"), s.Host))],
            outcome.Numbers,
            checks,
            options.Faults.Count > 0 ? faults : null,
            lockCheck);

        await File.WriteAllTextAsync(Path.Combine(runFolder, "result.json"), result.ToJson(), cancellationToken);
        await File.WriteAllTextAsync(Path.Combine(runFolder, "summary.md"), result.ToSummary(), cancellationToken);
        return result;
    }

    /// <summary>
    /// One user per client, named after its server and place (e.g. <c>s2c3-registrant</c>), so a
    /// history entry tells which client wrote it; plus <see cref="ReaderId"/>, an admin for reading back.
    /// </summary>
    /// <remarks>
    /// A registrant may view and change only its own records. With fewer records than registrants,
    /// the registrants that would get one stay (one per server in turn, in the order
    /// <see cref="Workload.RunAsync"/> hands records out, which this must match) and the rest
    /// become reviewers, so every client still writes. The role mix, and so the mix of actions,
    /// changes with it.
    /// </remarks>
    /// <param name="servers">Number of servers.</param>
    /// <param name="clientsPerServer">Clients per server.</param>
    /// <param name="records">Records the registrants create.</param>
    /// <returns>The users, server by server, then the reader.</returns>
    public static IReadOnlyList<UserDto> MakeUsers(int servers, int clientsPerServer, int records) =>
    [
        .. Enumerable.Range(1, servers).SelectMany(s => Enumerable.Range(1, clientsPerServer).Select(c =>
        {
            var role = RoleCycle[(c - 1) % RoleCycle.Length];
            var place = Enumerable.Range(0, c - 1).Count(k => RoleCycle[k % RoleCycle.Length] == Role.Registrant);
            if (role == Role.Registrant && (place * servers) + (s - 1) >= records)
            {
                role = Role.Reviewer;
            }

            var id = string.Create(CultureInfo.InvariantCulture, $"s{s}c{c}-{role.ToString().ToLowerInvariant()}");
            return new UserDto(id, id, [role]);
        })),
        new UserDto(ReaderId, "Stress reader", [Role.Admin]),
    ];

    /// <summary>How many clients have each role; the reader isn't a client.</summary>
    private static SortedDictionary<Role, int> ClientsByRole(IReadOnlyList<UserDto> users) =>
        new(users.Where(u => u.Id != ReaderId).GroupBy(u => u.Roles[0]).ToDictionary(g => g.Key, g => g.Count()));

    /// <summary>
    /// Fills the folder with finished records before the servers start (the volume test). They are
    /// made here through the app's rules, straight into the folder when this machine has it,
    /// otherwise into a local folder that's archived and unpacked on the first remote.
    /// </summary>
    /// <returns>Each record's id and the version it was left at.</returns>
    /// <exception cref="ArgumentException">The strategy keeps no data in the folder, so the servers would never see the records.</exception>
    private static async Task<IReadOnlyDictionary<Guid, int>> PrefillAsync(
        StressOptions options, IRegistrationStoreFactory factory, IReadOnlyList<UserDto> users, string dataPath, string runFolder, RemoteHost[] remotes,
        TextWriter log, CancellationToken cancellationToken)
    {
        if (!factory.UsesFolder)
        {
            throw new ArgumentException($"--prefill needs a strategy that keeps its data in the folder; {factory.Name} doesn't.");
        }

        var started = Stopwatch.GetTimestamp();
        var folder = options.AllRemote ? Path.Combine(runFolder, "prefill") : dataPath;
        var archive = Path.Combine(runFolder, "prefill.tar");
        Directory.CreateDirectory(folder);
        try
        {
            log.WriteLine($"Prefilling {options.Prefill} records");
            var store = factory.Create(folder, NullLoggerFactory.Instance);
            IReadOnlyDictionary<Guid, int> made;
            try
            {
                made = await Prefill.CreateAsync(store, users, options.Prefill, options.Seed, cancellationToken);
            }
            finally
            {
                // Release the store (e.g. pooled SQLite connections) before anything else opens the folder.
                if (store is IAsyncDisposable asyncDisposable)
                {
                    await asyncDisposable.DisposeAsync();
                }
                else if (store is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

            if (options.AllRemote)
            {
                await RemoteHost.RunAsync("tar", ["-cf", archive, "-C", folder, "."], cancellationToken);
                log.WriteLine($"Copying them to {remotes[0].Host}");
                await remotes[0].UploadDataAsync(archive, cancellationToken);
            }

            log.WriteLine($"Prefilled in {Stopwatch.GetElapsedTime(started).TotalSeconds:0} s");
            return made;
        }
        finally
        {
            if (options.AllRemote)
            {
                File.Delete(archive);
                Directory.Delete(folder, recursive: true);
            }
        }
    }

    /// <summary>How long each read or comment of the lock check may take before the record counts as stuck.</summary>
    private static readonly TimeSpan LockTimeout = TimeSpan.FromSeconds(30);

    /// <summary>Waits for a fault's time, then causes it; reports what it did.</summary>
    private static async Task<FaultOutcome> RunFaultAsync(
        Fault fault, IReadOnlyList<ServerProcess> servers, long workloadStarted, TextWriter log, CancellationToken workloadEnded)
    {
        try
        {
            var wait = TimeSpan.FromSeconds(fault.AtSeconds) - Stopwatch.GetElapsedTime(workloadStarted);
            if (wait > TimeSpan.Zero)
            {
                await Task.Delay(wait, workloadEnded);
            }
        }
        catch (OperationCanceledException)
        {
            return new FaultOutcome(fault.ToString(), null, "not reached: the workload ended first", Happened: false);
        }

        var at = Math.Round(Stopwatch.GetElapsedTime(workloadStarted).TotalSeconds, 1);
        log.WriteLine($"Fault at {at} s: {fault}");
        try
        {
            if (fault.Kind == Fault.Kill)
            {
                var server = servers[fault.Server - 1];
                await server.KillAsync();
                return new FaultOutcome(fault.ToString(), at, $"server {server.Name} killed");
            }

            // Its own stdin, closed: the command mustn't read the terminal running the tool.
            var start = new ProcessStartInfo("/bin/sh") { RedirectStandardInput = true, RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false };
            start.ArgumentList.Add("-c");
            start.ArgumentList.Add(fault.Command!);
            using var process = Process.Start(start) ?? throw new InvalidOperationException("/bin/sh didn't start.");
            process.StandardInput.Close();
            var output = process.StandardOutput.ReadToEndAsync(CancellationToken.None);
            var error = process.StandardError.ReadToEndAsync(CancellationToken.None);
            await process.WaitForExitAsync(CancellationToken.None);
            var text = $"{await output}{await error}".Trim().ReplaceLineEndings(" ");
            return new FaultOutcome(
                fault.ToString(), at, $"exit {process.ExitCode}{(text.Length > 0 ? $": {text[..Math.Min(200, text.Length)]}" : "")}", Happened: process.ExitCode == 0);
        }
        catch (Exception e) when (e is InvalidOperationException or System.ComponentModel.Win32Exception or OperationCanceledException)
        {
            return new FaultOutcome(fault.ToString(), at, $"failed: {e.Message}", Happened: false);
        }
    }

    /// <summary>Reads every record back through one server's API and checks it.</summary>
    private static async Task<SourceReport> ReadBackAsync(
        ApiClient reader, WorkloadOutcome outcome, IReadOnlyDictionary<string, UserDto> users, IReadOnlyDictionary<Guid, int> prefilled,
        CancellationToken cancellationToken)
    {
        var source = $"server {reader.Server}";
        var list = await reader.ListAsync(cancellationToken);
        if (list.Body is null)
        {
            return new SourceReport { Source = source, Error = $"listing records answered {Status(list.Status)}" };
        }

        // Read in full only what the run created or changed (and anything unknown); prefilled
        // records are checked by their listed version, and fully (rules included) in the folder read-back.
        var changed = outcome.Created.Concat(outcome.Acks.Select(a => a.RecordId)).ToHashSet();
        var records = new List<RegistrationRecordDto>();
        var listedOnly = new Dictionary<Guid, int>();
        foreach (var summary in list.Body)
        {
            if (!changed.Contains(summary.Id) && prefilled.ContainsKey(summary.Id))
            {
                listedOnly[summary.Id] = summary.Version;
                continue;
            }

            var read = await reader.GetAsync(summary.Id, cancellationToken);
            if (read.Body is null)
            {
                return new SourceReport { Source = source, Error = $"reading {summary.Id} answered {Status(read.Status)}" };
            }

            records.Add(read.Body);
        }

        return DataCheck.Check(source, outcome.Acks, outcome.Created, records, users, prefilled, listedOnly);
    }

    /// <summary>
    /// Reads the folder back through the strategy itself, so every stored record must
    /// deserialise. Skipped for strategies that don't keep data in the folder.
    /// </summary>
    private static async Task<SourceReport> ReadStorageAsync(
        IRegistrationStoreFactory factory,
        string dataPath,
        WorkloadOutcome outcome,
        IReadOnlyDictionary<string, UserDto> users,
        IReadOnlyDictionary<Guid, int> prefilled,
        CancellationToken cancellationToken)
    {
        const string source = "storage";
        if (!factory.UsesFolder)
        {
            return new SourceReport { Source = source, Skipped = $"{factory.Name} keeps no data in the folder" };
        }

        IReadOnlyList<RegistrationRecordDto> stored;
        try
        {
            stored = await factory.Create(dataPath, NullLoggerFactory.Instance).ListAsync(cancellationToken);
        }
        catch (Exception e) when (e is not OperationCanceledException)
        {
            return new SourceReport { Source = source, Error = $"{e.GetType().Name}: {e.Message}" };
        }

        return DataCheck.Check(source, outcome.Acks, outcome.Created, stored, users, prefilled);
    }

    private static string Status(HttpStatusCode? status) => status is { } code ? $"{(int)code}" : "nothing (no response)";

    /// <summary>The commit of the working tree the tool runs in, marked <c>-dirty</c> with uncommitted changes.</summary>
    private static string Commit()
    {
        var head = Git("rev-parse HEAD");
        return head is null ? "unknown" : string.IsNullOrEmpty(Git("status --porcelain")) ? head : $"{head}-dirty";
    }

    private static string? Git(string arguments)
    {
        try
        {
            var start = new ProcessStartInfo("git", arguments)
            {
                WorkingDirectory = AppContext.BaseDirectory,
                RedirectStandardOutput = true,
                UseShellExecute = false,
            };
            using var git = Process.Start(start)!;
            var output = git.StandardOutput.ReadToEnd().Trim();
            git.WaitForExit();
            return git.ExitCode == 0 ? output : null;
        }
        catch (System.ComponentModel.Win32Exception)
        {
            return null; // git isn't installed
        }
    }
}
