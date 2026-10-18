using System.Globalization;

namespace BlazorApp.Server.Logging;

/// <summary>
/// Which server wrote a log line: its port and process id, so the logs of several servers
/// sharing one data folder can be merged and still told apart (plan section 8).
/// </summary>
/// <param name="Port">The first port in the configured URLs; null when none is configured (e.g. in-memory test servers).</param>
/// <param name="ProcessId">This process's id.</param>
public sealed record ServerIdentity(int? Port, int ProcessId)
{
    /// <summary>The id written on every line, e.g. <c>5081-12345</c>.</summary>
    public string ServerId => $"{Port?.ToString(CultureInfo.InvariantCulture) ?? "noport"}-{ProcessId.ToString(CultureInfo.InvariantCulture)}";

    /// <summary>
    /// The log file's name, <c>server-&lt;port&gt;.jsonl</c>; null without a port. Port 0 (let the
    /// OS choose) adds the process id, because several such servers would otherwise share one
    /// file, and .NET doesn't append atomically across processes on Unix.
    /// </summary>
    public string? LogFileName => Port switch
    {
        null => null,
        0 => $"server-0-{ProcessId.ToString(CultureInfo.InvariantCulture)}.jsonl",
        { } port => $"server-{port.ToString(CultureInfo.InvariantCulture)}.jsonl",
    };

    /// <summary>Works out the identity from the <c>urls</c> setting (<c>--urls</c> or <c>ASPNETCORE_URLS</c>).</summary>
    /// <param name="urls">The setting, e.g. <c>http://localhost:5081;https://localhost:7081</c>.</param>
    /// <returns>The identity of this process.</returns>
    public static ServerIdentity From(string? urls)
    {
        int? port = null;
        var first = urls?.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).FirstOrDefault();

        // "http://*:5080" and "http://+:5080" aren't valid URIs, so read the port after the last ':'.
        if (first is not null
            && int.TryParse(first.TrimEnd('/')[(first.TrimEnd('/').LastIndexOf(':') + 1)..], NumberStyles.None, CultureInfo.InvariantCulture, out var parsed))
        {
            port = parsed;
        }

        return new ServerIdentity(port, Environment.ProcessId);
    }
}
