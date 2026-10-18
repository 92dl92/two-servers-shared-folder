using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace BlazorApp.Services.Storage;

/// <summary>
/// Replaces a file with another by renaming over it, as strategies B and C save. Windows refuses
/// the rename while any other handle has the old file open (another server reading it, an SMB
/// client keeping its handle for caching, an antivirus scan): phase 6b saw it as save errors. Those
/// handles close within milliseconds, so the rename is tried again for up to <see cref="MaxWait"/>
/// (3 s: with the share's leasing off, phase 6b's heavy runs still had a few renames refused for
/// longer than the first limit of 1 s).
/// Linux and macOS allow the rename regardless, so it succeeds the first time there.
/// </summary>
public static class FileReplace
{
    /// <summary>How long a refused rename is tried again before the error is thrown.</summary>
    public static readonly TimeSpan MaxWait = TimeSpan.FromSeconds(3);

    /// <summary>Renames <paramref name="source"/> over <paramref name="destination"/>, trying again while the destination is in use.</summary>
    /// <param name="source">The new file.</param>
    /// <param name="destination">The file it replaces.</param>
    /// <param name="logger">Gets a warning when a retry was needed.</param>
    /// <param name="move">The rename; a stand-in for tests, <see cref="File.Move(string, string, bool)"/> otherwise.</param>
    /// <param name="windows">Whether to treat access denied as "in use"; for tests, the running system otherwise.</param>
    /// <returns>How many times the rename was tried again.</returns>
    /// <remarks>Logs one <see cref="StoreLog.ReplaceRetried"/> warning per replace that needed
    /// retries, whether it then succeeded or gave up, so the two can be told apart.</remarks>
    public static int Move(string source, string destination, ILogger logger, Action<string, string>? move = null, bool? windows = null)
    {
        ArgumentNullException.ThrowIfNull(logger);
        move ??= (s, d) => File.Move(s, d, overwrite: true);
        var onWindows = windows ?? OperatingSystem.IsWindows();
        var waited = Stopwatch.StartNew();
        for (var retries = 0; ; retries++)
        {
            try
            {
                move(source, destination);
                if (retries > 0)
                {
                    StoreLog.LogReplaceRetried(logger, Path.GetFileName(destination), "succeeded", retries, waited.ElapsedMilliseconds);
                }

                return retries;
            }
            catch (Exception e) when (IsInUse(e, onWindows))
            {
                if (waited.Elapsed >= MaxWait)
                {
                    StoreLog.LogReplaceRetried(logger, Path.GetFileName(destination), "failed", retries, waited.ElapsedMilliseconds);
                    throw;
                }

                // Called while holding the store's lock, which is held for the wait too.
                Thread.Sleep(Math.Min(5 * (retries + 1), 50));
            }
        }
    }

    /// <summary>
    /// Whether an error means the destination is in use: on Windows, access denied (how .NET
    /// reports a rename over an open file) or a sharing or lock violation. Elsewhere a rename
    /// isn't refused for that, so access denied is a real permission problem.
    /// </summary>
    private static bool IsInUse(Exception e, bool windows) =>
        windows
        && (e is UnauthorizedAccessException
            || (e.GetType() == typeof(IOException) && e.HResult is unchecked((int)0x80070020) or unchecked((int)0x80070021)));
}
