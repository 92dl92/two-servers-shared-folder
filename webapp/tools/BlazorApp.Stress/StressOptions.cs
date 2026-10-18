using System.Globalization;

namespace BlazorApp.Stress;

/// <summary>The settings of one stress run, from the command line.</summary>
public sealed record StressOptions
{
    /// <summary>Command-line help.</summary>
    public const string Usage = """
        Usage: dotnet BlazorApp.Stress.dll [options]

          --strategy <name>        storage strategy every server uses (default InMemory)
          --servers <n>            server processes sharing the data folder (default 3)
          --clients <n>            clients per server, each acting as its own user (default 5)
          --records <n>            records created and then changed by every client (default 6)
          --ops <n>                changes each client attempts (default 40)
          --seed <n>               seed for the clients' choices (default: random, recorded)
          --workload <name>        contention (default): every client changes the same records
                                   as fast as it can; pipeline: registrants create, edit and
                                   submit records, reviewers and admins decide them, with a
                                   think time before each step (--records is not used)
          --think-ms <n>           pipeline: mean pause before each step (default 1000)
          --list                   pipeline: reviewers and admins open the record list before
                                   each pick, as the Records page would; list times are reported
          --prefill <n>            fill the folder with n finished records before the run
                                   (created here through the app's rules, then copied over)
          --out <folder>           run folder for data, logs and results
                                   (default stress-runs/<utc time>-<strategy>)
          --data <folder>          shared data folder (default <out>/data)
          --server-dll <path>      the built BlazorApp.Server.dll (default: the one built with this tool)
          --server-arg <arg>       extra argument for every server, repeatable,
                                   e.g. --server-arg --Logging:LogLevel:BlazorApp=Debug
          --fault <fault>          a failure caused on purpose, repeatable: kill:<n>@<s> kills
                                   server n (1-based) abruptly s seconds into the workload;
                                   run:<command>@<s> runs a shell command then (e.g. one that
                                   cuts a machine's network and restores it). With a fault, every
                                   record must still be changeable afterwards (the lock check)
          --remote <user@host>     run some servers on another machine over ssh (key login);
                                   it needs the .NET 10 runtime and the shared folder mounted.
                                   Repeatable, each with its own --remote-data; Linux, macOS
                                   or Windows (PowerShell as the ssh shell)
          --remote-servers <n>     how many of --servers run on each remote
          --remote-data <folder>   the shared folder as that remote sees it, one per --remote.
                                   With every server remote, leave out --data: the folder is
                                   copied back from the first remote to be checked
          --routing <name>         sticky (default): each client sends every request to its own
                                   server; random: each request goes to a server picked at
                                   random, like a load balancer without session affinity
          --external <url>,<url>   use servers started by hand (ClaudeTestServer.ps1) instead of
                                   starting any; their count is the number of addresses, each
                                   with its port. Needs --data (the shared folder as this machine
                                   sees it) and --out. After the read-back through the servers,
                                   the tool waits for them to be stopped, then checks the folder
          --prepare                with --external: write the run's users file (to --users-out,
                                   for the servers' -Users) and any --prefill records, then exit;
                                   run the same command without --prepare once the servers are up
          --users-out <file>       with --prepare: where the servers will read the users file

        Exit code: 0 when every check passed, 1 when a check failed, 2 when the run couldn't be done.
        """;

    /// <summary>Storage strategy name, passed to every server as <c>--Storage:Strategy</c>.</summary>
    public string Strategy { get; init; } = "InMemory";

    /// <summary>How many server processes to start.</summary>
    public int Servers { get; init; } = 3;

    /// <summary>Clients per server; each acts as its own user.</summary>
    public int ClientsPerServer { get; init; } = 5;

    /// <summary>Records created at the start, which every client then changes.</summary>
    public int Records { get; init; } = 6;

    /// <summary>Changes each client attempts after the records exist.</summary>
    public int OpsPerClient { get; init; } = 40;

    /// <summary>The workload: <see cref="Contention"/> or <see cref="Pipeline"/>.</summary>
    public string Workload { get; init; } = Contention;

    /// <summary>Pipeline: whether responders open the record list before each pick.</summary>
    public bool List { get; init; }

    /// <summary>Finished records put in the folder before the run (the volume test).</summary>
    public int Prefill { get; init; }

    /// <summary>Pipeline: the mean pause before each step, in milliseconds.</summary>
    public int ThinkMilliseconds { get; init; } = 1000;

    /// <summary>The workload name of <see cref="BlazorApp.Stress.Workload.RunAsync"/>.</summary>
    public const string Contention = "contention";

    /// <summary>The workload name of <see cref="BlazorApp.Stress.Workload.RunPipelineAsync"/>.</summary>
    public const string Pipeline = "pipeline";

    /// <summary>Seed for the clients' random choices.</summary>
    public int Seed { get; init; } = System.Random.Shared.Next();

    /// <summary>The run folder; null for <c>stress-runs/&lt;utc time&gt;-&lt;strategy&gt;</c>.</summary>
    public string? OutputDirectory { get; init; }

    /// <summary>The shared data folder; null for <c>&lt;run folder&gt;/data</c>.</summary>
    public string? DataPath { get; init; }

    /// <summary>The Server to run; null for the one built with this tool.</summary>
    public string? ServerDll { get; init; }

    /// <summary>Extra arguments for every server.</summary>
    public IReadOnlyList<string> ServerArgs { get; init; } = [];

    /// <summary>Failures caused on purpose during the workload (<see cref="Fault"/>).</summary>
    public IReadOnlyList<Fault> Faults { get; init; } = [];

    /// <summary>The machines that run some of the servers, in order; empty for all local.</summary>
    public IReadOnlyList<RemoteTarget> Remotes { get; init; } = [];

    /// <summary>How many of the <see cref="Servers"/> run on each of the <see cref="Remotes"/>.</summary>
    public int RemoteServers { get; init; }

    /// <summary>Servers started by hand, by address; empty when the tool starts its own.</summary>
    public IReadOnlyList<Uri> External { get; init; } = [];

    /// <summary>How clients pick a server for each request: <see cref="Sticky"/> or <see cref="Random"/>.</summary>
    public string Routing { get; init; } = Sticky;

    /// <summary>The routing name for each client keeping to its own server.</summary>
    public const string Sticky = "sticky";

    /// <summary>The routing name for each request going to a random server.</summary>
    public const string Random = "random";

    /// <summary>With <see cref="External"/>: write the users file and any prefill, then stop.</summary>
    public bool Prepare { get; init; }

    /// <summary>With <see cref="Prepare"/>: where the users file goes, for the servers to read.</summary>
    public string? UsersOut { get; init; }

    /// <summary>Whether every server runs on a remote, so none runs here.</summary>
    public bool AllRemote => Remotes.Count > 0 && RemoteServers * Remotes.Count == Servers;

    /// <summary>Reads options from the command line.</summary>
    /// <param name="args">The arguments.</param>
    /// <returns>The options.</returns>
    /// <exception cref="ArgumentException">An unknown option, a missing value or a number out of range.</exception>
    public static StressOptions Parse(IReadOnlyList<string> args)
    {
        ArgumentNullException.ThrowIfNull(args);
        var options = new StressOptions();
        var serverArgs = new List<string>();
        var remotes = new List<string>();
        var faults = new List<string>();
        var recordsGiven = false;
        var thinkGiven = false;
        var remoteData = new List<string>();
        var serversGiven = false;
        for (var i = 0; i < args.Count; i++)
        {
            var name = args[i];
            string Value() => i + 1 < args.Count ? args[++i] : throw new ArgumentException($"{name} needs a value.");
            int Count(int min) =>
                int.TryParse(Value(), NumberStyles.None, CultureInfo.InvariantCulture, out var n) && n >= min
                    ? n
                    : throw new ArgumentException($"{name} needs a whole number of at least {min}.");

            options = name switch
            {
                "--strategy" => options with { Strategy = Value() },
                "--servers" => Given(ref serversGiven, options with { Servers = Count(1) }),
                "--clients" => options with { ClientsPerServer = Count(1) },
                "--records" => Given(ref recordsGiven, options with { Records = Count(1) }),
                "--ops" => options with { OpsPerClient = Count(0) },
                "--seed" => options with { Seed = Count(0) },
                "--workload" => options with { Workload = WorkloadName(Value()) },
                "--think-ms" => Given(ref thinkGiven, options with { ThinkMilliseconds = Count(0) }),
                "--list" => options with { List = true },
                "--prefill" => options with { Prefill = Count(0) },
                "--out" => options with { OutputDirectory = Value() },
                "--data" => options with { DataPath = Value() },
                "--server-dll" => options with { ServerDll = Value() },
                "--server-arg" => AddServerArg(options, Value()),
                "--remote" => Add(remotes, Value(), options),
                "--fault" => Add(faults, Value(), options),
                "--remote-servers" => options with { RemoteServers = Count(0) },
                "--remote-data" => Add(remoteData, Value(), options),
                "--routing" => options with { Routing = RoutingName(Value()) },
                "--external" => options with { External = Addresses(Value()) },
                "--prepare" => options with { Prepare = true },
                "--users-out" => options with { UsersOut = Value() },
                _ => throw new ArgumentException($"Unknown option {name}."),
            };
        }

        if (options.External.Count > 0)
        {
            if (serversGiven || options.ServerDll is not null || options.ServerArgs.Count > 0 || remotes.Count > 0)
            {
                throw new ArgumentException("--external servers are started by hand: leave out --servers, --server-dll, --server-arg and --remote.");
            }

            if (options.DataPath is null || options.OutputDirectory is null)
            {
                throw new ArgumentException("--external needs --data (the shared folder as this machine sees it) and --out (where --prepare left the run's files).");
            }

            options = options with { Servers = options.External.Count };
        }

        if (options.Prepare != (options.UsersOut is not null) || (options.Prepare && options.External.Count == 0))
        {
            throw new ArgumentException("--prepare and --users-out go together, with --external.");
        }

        if (options.Routing == Random && options.Servers < 2)
        {
            throw new ArgumentException("--routing random needs at least two servers to choose from.");
        }

        if (faults.Count > 0)
        {
            options = options with { Faults = [.. faults.Select(f => Fault.Parse(f, options.Servers))] };
            if (options.Faults.Where(f => f.Kind == Fault.Kill).Select(f => f.Server).Distinct().Count() >= options.Servers)
            {
                throw new ArgumentException("--fault can't kill every server: the lock check and the read-backs need one left.");
            }

            if (options.External.Count > 0 && options.Faults.Any(f => f.Kind == Fault.Kill))
            {
                throw new ArgumentException("--fault kill needs servers the tool started; with --external, stop a server by hand instead.");
            }
        }
        if (options.Workload == Pipeline ? recordsGiven || options.OpsPerClient == 0 : thinkGiven || options.List)
        {
            throw new ArgumentException(options.Workload == Pipeline
                ? "The pipeline creates its own records and needs --ops of at least 1: leave out --records."
                : "--think-ms and --list are for --workload pipeline.");
        }

        if (remotes.Count == 0)
        {
            if (options.RemoteServers != 0 || remoteData.Count != 0)
            {
                throw new ArgumentException("--remote-servers and --remote-data need --remote.");
            }

            return options;
        }

        if (remoteData.Count != remotes.Count)
        {
            throw new ArgumentException("Each --remote needs its own --remote-data, the shared folder as that machine sees it.");
        }

        options = options with { Remotes = [.. remotes.Zip(remoteData, (target, data) => new RemoteTarget(target, data))] };
        if (options.RemoteServers < 1 || options.RemoteServers > options.Servers / remotes.Count)
        {
            throw new ArgumentException("--remote needs --remote-servers of at least 1, and no more servers on the remotes than --servers.");
        }

        if (options.AllRemote ? options.DataPath is not null : options.DataPath is null)
        {
            throw new ArgumentException(options.AllRemote
                ? "With every server remote, the folder is copied back from the first remote: leave out --data."
                : "The servers here need the shared folder as this machine sees it: --data.");
        }

        return options;

        static StressOptions Given(ref bool given, StressOptions current)
        {
            given = true;
            return current;
        }

        static string RoutingName(string value) =>
            value is Sticky or Random ? value : throw new ArgumentException($"--routing is {Sticky} or {Random}.");

        // Each server once, as http://host:port with the port the owner opened for it written out.
        static IReadOnlyList<Uri> Addresses(string value)
        {
            List<Uri> addresses =
            [
                .. value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(a =>
                    System.Text.RegularExpressions.Regex.IsMatch(a, @"^http://[A-Za-z0-9.\-]+:[0-9]{1,5}/?$") && Uri.TryCreate(a, UriKind.Absolute, out var uri)
                        ? new Uri(uri.GetLeftPart(UriPartial.Authority) + "/")
                        : throw new ArgumentException($"--external needs addresses like http://server1:8080 (host and port only); got {a}.")),
            ];
            return addresses.Distinct().Count() == addresses.Count
                ? addresses
                : throw new ArgumentException("--external lists a server more than once.");
        }

        static string WorkloadName(string value) =>
            value is Contention or Pipeline ? value : throw new ArgumentException($"--workload is {Contention} or {Pipeline}.");

        static StressOptions Add(List<string> values, string value, StressOptions current)
        {
            values.Add(value);
            return current;
        }

        StressOptions AddServerArg(StressOptions current, string arg)
        {
            serverArgs.Add(arg);
            return current with { ServerArgs = [.. serverArgs] };
        }
    }
}

/// <summary>A machine that runs some of the servers.</summary>
/// <param name="Target">The ssh target, e.g. <c>user@mac.example</c>; its host must resolve here, as the servers are reached on it over HTTP.</param>
/// <param name="DataPath">The shared data folder as that machine sees it.</param>
public sealed record RemoteTarget(string Target, string DataPath);
