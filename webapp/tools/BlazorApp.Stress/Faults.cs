using System.Globalization;

namespace BlazorApp.Stress;

/// <summary>
/// A failure the run causes on purpose at a set time into the workload (phase 6b's failure
/// tests): <c>kill:&lt;n&gt;@&lt;seconds&gt;</c> kills server <c>n</c> (1-based, in start order)
/// abruptly; <c>run:&lt;command&gt;@&lt;seconds&gt;</c> runs a shell command here (e.g. one that cuts
/// a machine's network, waits and restores it) while the workload carries on.
/// </summary>
/// <param name="Kind"><see cref="Kill"/> or <see cref="Run"/>.</param>
/// <param name="Server">For a kill, the server's number (1-based); otherwise 0.</param>
/// <param name="Command">For a run, the shell command; otherwise null.</param>
/// <param name="AtSeconds">When, in seconds from the start of the workload.</param>
public sealed record Fault(string Kind, int Server, string? Command, double AtSeconds)
{
    /// <summary>The kind that kills a server.</summary>
    public const string Kill = "kill";

    /// <summary>The kind that runs a command.</summary>
    public const string Run = "run";

    /// <summary>The latest a fault can be: a day into the workload.</summary>
    public const double MaxSeconds = 86_400;

    /// <summary>Reads a <c>--fault</c> value.</summary>
    /// <param name="value">E.g. <c>kill:2@40</c> or <c>run:./cut.sh ws2 10@40</c> (the last <c>@</c> starts the time).</param>
    /// <param name="servers">How many servers the run starts.</param>
    /// <returns>The fault.</returns>
    /// <exception cref="ArgumentException">The value isn't one of the two forms.</exception>
    public static Fault Parse(string value, int servers)
    {
        ArgumentNullException.ThrowIfNull(value);
        var colon = value.IndexOf(':', StringComparison.Ordinal);
        var at = value.LastIndexOf('@');
        if (colon < 1 || at < colon
            || !double.TryParse(value[(at + 1)..], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var seconds))
        {
            throw new ArgumentException($"--fault {value}: expected kill:<server>@<seconds> or run:<command>@<seconds>.");
        }

        if (seconds > MaxSeconds)
        {
            throw new ArgumentException($"--fault {value}: at most {MaxSeconds} s into the workload.");
        }

        var what = value[(colon + 1)..at];
        return value[..colon] switch
        {
            Kill when int.TryParse(what, NumberStyles.None, CultureInfo.InvariantCulture, out var n) && n >= 1 && n <= servers
                => new Fault(Kill, n, null, seconds),
            Kill => throw new ArgumentException($"--fault {value}: the server is a number from 1 to {servers}."),
            Run when what.Trim().Length > 0 => new Fault(Run, 0, what, seconds),
            Run => throw new ArgumentException($"--fault {value}: the command is missing."),
            var kind => throw new ArgumentException($"--fault {value}: unknown kind {kind}; expected {Kill} or {Run}."),
        };
    }

    /// <inheritdoc/>
    public override string ToString() =>
        Kind == Kill
            ? $"kill server {Server} at {AtSeconds.ToString(CultureInfo.InvariantCulture)} s"
            : $"run `{Command}` at {AtSeconds.ToString(CultureInfo.InvariantCulture)} s";
}

/// <summary>What happened to a fault.</summary>
/// <param name="Fault">The fault, as text.</param>
/// <param name="AtSeconds">When it actually began, in seconds from the start of the workload; null if the workload ended first.</param>
/// <param name="Result">What it did: e.g. which server was killed, or the command's exit code and output.</param>
/// <param name="Happened">Whether it did what it was meant to: reached in time, the kill sent, the command exited 0.</param>
public sealed record FaultOutcome(string Fault, double? AtSeconds, string Result, bool Happened = true);
