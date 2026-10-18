namespace BlazorApp.Stress;

/// <summary>The command line around <see cref="StressRun"/>: options in, summary and exit code out.</summary>
public static class StressProgram
{
    /// <summary>Exit code when every check passed.</summary>
    public const int Passed = 0;

    /// <summary>Exit code when a check failed.</summary>
    public const int ChecksFailed = 1;

    /// <summary>Exit code when the run couldn't be done (bad options, a server that didn't start).</summary>
    public const int CouldNotRun = 2;

    /// <summary>Runs the tool.</summary>
    /// <param name="args">Command-line arguments; see <see cref="StressOptions.Usage"/>.</param>
    /// <param name="output">Receives progress and the summary.</param>
    /// <param name="error">Receives errors.</param>
    /// <param name="cancellationToken">Stops the run.</param>
    /// <returns>The exit code.</returns>
    public static async Task<int> MainAsync(string[] args, TextWriter output, TextWriter error, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(args);
        ArgumentNullException.ThrowIfNull(output);
        ArgumentNullException.ThrowIfNull(error);

        if (args is ["--help" or "-h"])
        {
            output.WriteLine(StressOptions.Usage);
            return Passed;
        }

        StressOptions options;
        try
        {
            options = StressOptions.Parse(args);
        }
        catch (ArgumentException e)
        {
            error.WriteLine(e.Message);
            error.WriteLine(StressOptions.Usage);
            return CouldNotRun;
        }

        try
        {
            if (options.Prepare)
            {
                await StressRun.PrepareAsync(options, output, cancellationToken);
                return Passed;
            }

            var result = await StressRun.RunAsync(options, output, cancellationToken);
            output.WriteLine();
            output.WriteLine(result.ToSummary());
            output.WriteLine($"Results: {Path.Combine(result.RunFolder, "result.json")}");
            return result.Passed ? Passed : ChecksFailed;
        }
        catch (Exception e)
        {
            // Anything that stops the run (bad strategy, a server that didn't start, no dotnet
            // on the path, a folder that can't be written) is "couldn't run", never a pass or fail.
            error.WriteLine($"The stress run couldn't be done: {e.GetType().Name}: {e.Message}");
            return CouldNotRun;
        }
    }
}
