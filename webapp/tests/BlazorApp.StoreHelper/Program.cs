// Store helper: opens one store and runs StoreWorkload against it, so tests can run several
// copies at once as separate operating-system processes sharing one data folder.
//
//   dotnet BlazorApp.StoreHelper.dll --strategy PerRecordFile --path /data
//       --records <guid>,<guid> --writer p1 --iterations 50 --go-file /tmp/go
//       [--plugin Extra.dll ...]
//
// Output, one line each, flushed immediately:
//   READY                         store open; waiting for --go-file to appear
//   BEGIN <utc ticks>             started
//   ACK <record> <version> <marker>   a save the store reported as successful
//   END <utc ticks> <acks> <conflicts> <broken reads>
// Exit code 0 on success; 1 with a message on stderr otherwise.

using System.Reflection;
using BlazorApp.Services.Storage;
using BlazorApp.StoreHelper;

try
{
    var options = Options.Parse(args);
    var plugins = options.Plugins.Select(Assembly.LoadFrom).ToArray();
    var store = StoreStrategies.Create(options.Strategy, options.Path, plugins);

    Console.WriteLine("READY");
    while (!File.Exists(options.GoFile))
    {
        await Task.Delay(5);
    }

    Console.WriteLine($"BEGIN {DateTime.UtcNow.Ticks}");
    var result = await StoreWorkload.RunAsync(
        store,
        options.Records,
        options.Writer,
        options.Iterations,
        ack => Console.WriteLine($"ACK {ack.RecordId} {ack.Version} {ack.Marker}"));
    Console.WriteLine($"END {DateTime.UtcNow.Ticks} {result.Acks} {result.Conflicts} {result.BrokenReads}");
    return 0;
}
catch (Exception e)
{
    Console.Error.WriteLine(e);
    return 1;
}

/// <summary>Command-line options.</summary>
/// <param name="Strategy">Storage strategy name.</param>
/// <param name="Path">Shared data folder.</param>
/// <param name="Plugins">Extra assemblies to search for strategies.</param>
/// <param name="Records">Record ids to write to.</param>
/// <param name="Writer">This writer's name.</param>
/// <param name="Iterations">Number of changes to make.</param>
/// <param name="GoFile">Start once this file exists, so all copies start together.</param>
internal sealed record Options(
    string Strategy, string? Path, IReadOnlyList<string> Plugins, IReadOnlyList<Guid> Records, string Writer, int Iterations, string GoFile)
{
    /// <summary>Parses <c>--name value</c> pairs; <c>--plugin</c> may repeat.</summary>
    /// <param name="args">The command-line arguments.</param>
    /// <returns>The options.</returns>
    public static Options Parse(string[] args)
    {
        var values = new Dictionary<string, List<string>>(StringComparer.Ordinal);
        for (var i = 0; i < args.Length; i += 2)
        {
            if (!args[i].StartsWith("--", StringComparison.Ordinal) || i + 1 >= args.Length)
            {
                throw new ArgumentException($"Expected '--name value' at '{args[i]}'.");
            }

            var key = args[i][2..];
            if (!values.TryGetValue(key, out var list))
            {
                values[key] = list = [];
            }

            list.Add(args[i + 1]);
        }

        string One(string key) => values.TryGetValue(key, out var v) ? v[^1] : throw new ArgumentException($"Missing --{key}.");

        return new Options(
            One("strategy"),
            values.TryGetValue("path", out var path) ? path[^1] : null,
            values.TryGetValue("plugin", out var plugins) ? plugins : [],
            [.. One("records").Split(',', StringSplitOptions.RemoveEmptyEntries).Select(Guid.Parse)],
            One("writer"),
            int.Parse(One("iterations"), System.Globalization.CultureInfo.InvariantCulture),
            One("go-file"));
    }
}
