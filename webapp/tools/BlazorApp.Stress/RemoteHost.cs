using System.Diagnostics;
using System.Text;

namespace BlazorApp.Stress;

/// <summary>
/// A second machine, reached over ssh with a key, that runs some of the servers on the same
/// shared folder (plan section 4: per-machine caching is where network drives tend to break).
/// Each run copies the built Server there (to <c>~/claudetest-stress/server</c>, or on Windows into
/// the run's folder), so both machines run the same commit, and gives the run its own folder for
/// the users file and logs.
/// </summary>
/// <remarks>
/// <para>Linux and macOS are reached through their POSIX shell. Windows (phase 6b) needs
/// PowerShell as the ssh server's shell, and differs in these ways: commands go as encoded
/// PowerShell (<see cref="EncodedPowerShell"/>); files go with <c>scp -O</c>, as there's no
/// rsync (its default SFTP mode stops silently at 200 KiB when copying from OpenSSH for Windows
/// 9.5, so the legacy protocol is used); each run gets its own copy of the Server, so a window
/// open in an old one can't block the next run;
/// a server keeps running when its ssh session ends, so it is stopped by its process tree; and an
/// ssh key logon has no network credentials, so every command that touches the share first runs
/// <c>~/claudetest-stress/connect.ps1</c> if the machine has one (e.g. a <c>net use</c> with the
/// share's user, kept on the machine and out of the repo).</para>
/// </remarks>
public sealed class RemoteHost
{
    /// <summary>The folder under the remote home that holds the server copy and the runs.</summary>
    public const string Folder = "claudetest-stress";

    /// <summary>
    /// The PowerShell that runs a Windows machine's share connection script, if it has one, before
    /// a command that touches the share.
    /// </summary>
    public const string WindowsConnect =
        "$connect = Join-Path $HOME 'claudetest-stress\\connect.ps1'; if (Test-Path $connect) { . $connect | Out-Null }";

    private RemoteHost(string target, string home, string runFolder, string dataPath)
    {
        Target = target;
        Home = home;
        RunFolder = runFolder;
        DataPath = dataPath;
        IsWindows = IsWindowsHome(home);
    }

    /// <summary>The ssh target, e.g. <c>user@mac.example</c>.</summary>
    public string Target { get; }

    /// <summary>The host name the servers are reached on over HTTP, so it must resolve here (not an ssh config alias).</summary>
    public string Host => Target[(Target.IndexOf('@', StringComparison.Ordinal) + 1)..];

    /// <summary>The remote home folder.</summary>
    public string Home { get; }

    /// <summary>Whether the remote runs Windows (see the remarks on the class).</summary>
    public bool IsWindows { get; }

    /// <summary>This run's folder on the remote machine.</summary>
    public string RunFolder { get; }

    /// <summary>The shared data folder as the remote machine sees it.</summary>
    public string DataPath { get; }

    /// <summary>The users file there.</summary>
    public string UsersFile => Combine(RunFolder, "users.json");

    /// <summary>The servers' log folder there.</summary>
    public string LogDirectory => Combine(RunFolder, "logs");

    /// <summary>The Server copy there: one per run on Windows, where a folder in use (a window
    /// open in it) can't be deleted; one shared copy, kept in step by rsync, elsewhere.</summary>
    private string ServerFolder => IsWindows ? Combine(RunFolder, "server") : Combine(Home, Folder, "server");

    /// <summary>scp's options for this remote (see the remarks on the class for <c>-O</c>).</summary>
    private string[] ScpOptions => IsWindows ? ["-O", "-q", .. RsyncSshOptions] : ["-q", .. RsyncSshOptions];

    /// <summary>Whether a home folder is a Windows one, which starts with a drive letter.</summary>
    /// <param name="home">The home folder the remote reported.</param>
    /// <returns>True for Windows.</returns>
    public static bool IsWindowsHome(string home) =>
        home.Length >= 3 && char.IsAsciiLetter(home[0]) && home[1] == ':' && home[2] == '\\';

    /// <summary>Copies the Server and the users file to the remote machine.</summary>
    /// <param name="target">The ssh target.</param>
    /// <param name="serverDirectory">The folder of the built <c>BlazorApp.Server.dll</c>.</param>
    /// <param name="usersFile">The run's users file.</param>
    /// <param name="runName">The run's name, for its remote folder.</param>
    /// <param name="dataPath">The shared data folder as the remote machine sees it.</param>
    /// <param name="cancellationToken">Stops the copy.</param>
    /// <returns>The prepared host.</returns>
    /// <exception cref="InvalidOperationException">ssh, rsync or scp failed.</exception>
    public static async Task<RemoteHost> PrepareAsync(
        string target, string serverDirectory, string usersFile, string runName, string dataPath, CancellationToken cancellationToken)
    {
        // `echo $HOME` answers in both a POSIX shell and PowerShell.
        var home = (await RunAsync("ssh", [.. SshOptions, target, "echo $HOME"], cancellationToken)).Trim();
        var host = new RemoteHost(target, home, Combine(IsWindowsHome(home), home, Folder, "runs", runName), dataPath);
        if (host.IsWindows)
        {
            await host.PrepareWindowsAsync(serverDirectory, usersFile, cancellationToken);
            return host;
        }

        var server = host.ServerFolder;

        // A server left running by an earlier run (e.g. one stopped by force) would still use the
        // shared folder; stop every stress server there first. The data folder is made from that
        // side too: a new folder made here can stay invisible there for seconds (NFS caches
        // directory lookups), and SQLite won't create a missing folder.
        await host.SshAsync($"pkill -f {Quote(ServerPattern)}; mkdir -p {Quote(server)} {Quote(host.LogDirectory)} {Quote(dataPath)}", cancellationToken);
        await RunAsync("rsync", ["-a", "--delete", "-e", $"ssh {string.Join(" ", RsyncSshOptions)}", $"{serverDirectory.TrimEnd('/')}/", $"{target}:{server}/"], cancellationToken);

        // The build's static web assets manifest names folders on this machine, which a server
        // started in Development would fail on; the stress run uses only the API. (find, not a
        // glob: zsh, macOS's shell, fails on a glob that matches nothing.)
        await host.SshAsync($"find {Quote(server)} -maxdepth 1 -name '*.staticwebassets.runtime.json' -delete", cancellationToken);
        await RunAsync("scp", ["-q", .. RsyncSshOptions, usersFile, $"{target}:{host.UsersFile}"], cancellationToken);
        return host;
    }

    /// <summary>The ssh arguments that start one server, for <see cref="ServerProcess"/>.</summary>
    /// <param name="port">The port.</param>
    /// <param name="args">Its settings, with remote paths.</param>
    /// <returns>The arguments for <c>ssh</c>.</returns>
    /// <remarks>On Linux and macOS a forced terminal (<c>-tt</c>) makes the server stop when its
    /// ssh session ends, even if the kill over ssh doesn't arrive. Windows' console swallows the
    /// server's output under <c>-tt</c>, and doesn't stop the server either way.</remarks>
    public IReadOnlyList<string> StartArguments(int port, IEnumerable<string> args) =>
        IsWindows
            ? [.. SshOptions, Target, EncodedPowerShell(WindowsServerScript(ServerFolder, port, LogDirectory, args))]
            : ["-tt", .. SshOptions, Target, ServerCommand(Home, port, LogDirectory, args)];

    /// <summary>
    /// The shell command that starts a server: it prints its process id first (so it can be
    /// stopped), then becomes the server, listening on every interface.
    /// </summary>
    /// <param name="home">The remote home folder.</param>
    /// <param name="port">The port.</param>
    /// <param name="logDirectory">The server's log folder there.</param>
    /// <param name="args">Its settings.</param>
    /// <returns>The command.</returns>
    public static string ServerCommand(string home, int port, string logDirectory, IEnumerable<string> args)
    {
        string[] all = ["--urls", $"http://0.0.0.0:{port}", "--Logging:File:Directory", logDirectory, .. args];
        return $"echo PID $$ && cd {Quote($"{home}/{Folder}/server")} && d=\"$HOME/.dotnet/dotnet\"; [ -x \"$d\" ] || d=dotnet; "
            + $"ASPNETCORE_ENVIRONMENT=Development exec \"$d\" BlazorApp.Server.dll {string.Join(" ", all.Select(Quote))}";
    }

    /// <summary>
    /// The PowerShell that starts a server on Windows: it prints its process id first (the
    /// PowerShell's, whose process tree is stopped), connects to the share, then runs the server,
    /// listening on every interface.
    /// </summary>
    /// <param name="serverFolder">The folder of the Server copy there.</param>
    /// <param name="port">The port.</param>
    /// <param name="logDirectory">The server's log folder there.</param>
    /// <param name="args">Its settings.</param>
    /// <returns>The script.</returns>
    /// <remarks>Windows PowerShell 5.1 quotes arguments to native programs badly: one ending in a
    /// backslash inside quotes, or an empty one, arrives mangled. The paths used here have
    /// neither.</remarks>
    public static string WindowsServerScript(string serverFolder, int port, string logDirectory, IEnumerable<string> args)
    {
        string[] all = ["--urls", $"http://0.0.0.0:{port}", "--Logging:File:Directory", logDirectory, .. args];
        return $"Write-Output \"PID $PID\"; {WindowsConnect}; Set-Location {QuotePowerShell(serverFolder)}; "
            + $"$env:ASPNETCORE_ENVIRONMENT = 'Development'; & dotnet BlazorApp.Server.dll {string.Join(" ", all.Select(QuotePowerShell))}";
    }

    /// <summary>
    /// A command for the Windows ssh server that runs <paramref name="script"/> in PowerShell. The
    /// ssh server hands a command to its shell through a command line that loses nested quotes,
    /// so the script goes as base64 of its UTF-16 text.
    /// </summary>
    /// <param name="script">The PowerShell script.</param>
    /// <returns>The command.</returns>
    public static string EncodedPowerShell(string script) =>
        $"powershell -NoProfile -NonInteractive -EncodedCommand {Convert.ToBase64String(Encoding.Unicode.GetBytes(script))}";

    /// <summary>Kills a remote server abruptly, as a crash would (a failure test): <c>kill -9</c>, or <c>taskkill /F</c> on Windows.</summary>
    /// <param name="pid">The process id it printed.</param>
    /// <returns>A task that completes when the kill was sent.</returns>
    /// <exception cref="InvalidOperationException">ssh failed.</exception>
    public Task KillAsync(int pid) =>
        SshAsync(IsWindows ? $"taskkill /PID {pid} /T /F 2>&1 | Out-Null; exit 0" : $"kill -9 {pid} 2>/dev/null; true", CancellationToken.None);

    /// <summary>Stops a remote server; a server that already stopped is fine. On Windows,
    /// <see cref="EnsureStoppedAsync"/> then checks nothing is left.</summary>
    /// <param name="pid">Its process id.</param>
    /// <returns>A task that completes when the kill was sent.</returns>
    public async Task StopAsync(int pid)
    {
        try
        {
            await SshAsync(
                IsWindows ? $"taskkill /PID {pid} /T /F 2>&1 | Out-Null; exit 0" : $"kill {pid} 2>/dev/null; true",
                CancellationToken.None);
        }
        catch (InvalidOperationException)
        {
            // Unreachable now: on Linux and macOS the server's ssh session (with a terminal)
            // ending stops it too; on Windows EnsureStoppedAsync fails the run.
        }
    }

    /// <summary>Copies the remote servers' logs into the local log folder.</summary>
    /// <param name="logDirectory">The local log folder.</param>
    /// <param name="cancellationToken">Stops the copy.</param>
    /// <returns>A task that completes when they're copied.</returns>
    public Task FetchLogsAsync(string logDirectory, CancellationToken cancellationToken) =>
        IsWindows
            ? RunAsync("scp", [.. ScpOptions, $"{Target}:{ScpPath(LogDirectory)}/*", $"{logDirectory.TrimEnd('/')}/"], cancellationToken)
            : RunAsync("rsync", ["-a", "-e", $"ssh {string.Join(" ", RsyncSshOptions)}", $"{Target}:{LogDirectory}/", $"{logDirectory.TrimEnd('/')}/"], cancellationToken);

    /// <summary>
    /// Copies the shared folder, as this remote sees it, into a local folder, for runs where no
    /// server ran here: archived there with <c>tar</c> (built into Windows too), copied, unpacked.
    /// </summary>
    /// <param name="localPath">The empty local folder to unpack into.</param>
    /// <param name="cancellationToken">Stops the copy.</param>
    /// <returns>A task that completes when it's copied.</returns>
    /// <exception cref="InvalidOperationException">ssh, scp or tar failed, or the copy came out a different size.</exception>
    public async Task FetchDataAsync(string localPath, CancellationToken cancellationToken)
    {
        var archive = Combine(RunFolder, "data.tar");
        var local = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(localPath))!, "data.tar");
        try
        {
            // The archive's size comes back with it: a copy that stops short without an error
            // (as scp's SFTP mode did from Windows) must stop the run, not lose records.
            var size = LastNumber(await SshAsync(
                IsWindows
                    ? $"{WindowsConnect}; tar -cf {QuotePowerShell(archive)} -C {QuotePowerShell(DataPath)} .; if ($LASTEXITCODE) {{ exit $LASTEXITCODE }}; (Get-Item {QuotePowerShell(archive)}).Length"
                    : $"tar -cf {Quote(archive)} -C {Quote(DataPath)} . && wc -c < {Quote(archive)}",
                cancellationToken));
            await RunAsync("scp", [.. ScpOptions, $"{Target}:{ScpPath(archive)}", local], cancellationToken);
            var copied = new FileInfo(local).Length;
            if (copied != size)
            {
                throw new InvalidOperationException($"The data archive from {Host} is {size} bytes there but {copied} bytes here.");
            }

            await RunAsync("tar", ["-xf", local, "-C", localPath], cancellationToken);
        }
        finally
        {
            File.Delete(local);
            await SshAsync(IsWindows ? $"Remove-Item -Force -ErrorAction SilentlyContinue {QuotePowerShell(archive)}" : $"rm -f {Quote(archive)}", CancellationToken.None);
        }
    }

    /// <summary>The number on the last line a remote printed, e.g. a file size.</summary>
    /// <param name="output">What it printed; a connect script or shell profile may print lines before it.</param>
    /// <returns>The number.</returns>
    /// <exception cref="InvalidOperationException">The last line isn't a whole number.</exception>
    public static long LastNumber(string output)
    {
        ArgumentNullException.ThrowIfNull(output);
        var last = output.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).LastOrDefault();
        return long.TryParse(last, System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out var number)
            ? number
            : throw new InvalidOperationException($"Expected a number from the remote, got: {output}");
    }

    /// <summary>
    /// Unpacks a local archive into the shared folder as this remote sees it (the volume test's
    /// prefilled records): copied over, size compared, unpacked with <c>tar</c>.
    /// </summary>
    /// <param name="archive">The local tar archive.</param>
    /// <param name="cancellationToken">Stops it.</param>
    /// <returns>A task that completes when it's unpacked.</returns>
    /// <exception cref="InvalidOperationException">ssh, scp or tar failed, or the copy came out a different size.</exception>
    public async Task UploadDataAsync(string archive, CancellationToken cancellationToken)
    {
        var remote = Combine(RunFolder, "prefill.tar");
        try
        {
            await RunAsync("scp", [.. ScpOptions, archive, $"{Target}:{ScpPath(remote)}"], cancellationToken);
            var size = LastNumber(await SshAsync(
                IsWindows ? $"(Get-Item {QuotePowerShell(remote)}).Length" : $"wc -c < {Quote(remote)}", cancellationToken));
            if (size != new FileInfo(archive).Length)
            {
                throw new InvalidOperationException($"The prefill archive is {new FileInfo(archive).Length} bytes here but {size} bytes on {Host}.");
            }

            await SshAsync(
                IsWindows
                    ? $"{WindowsConnect}; tar -xf {QuotePowerShell(remote)} -C {QuotePowerShell(DataPath)}; exit $LASTEXITCODE"
                    : $"tar -xf {Quote(remote)} -C {Quote(DataPath)}",
                cancellationToken);
        }
        finally
        {
            await SshAsync(IsWindows ? $"Remove-Item -Force -ErrorAction SilentlyContinue {QuotePowerShell(remote)}" : $"rm -f {Quote(remote)}", CancellationToken.None);
        }
    }

    /// <summary>
    /// On Windows, stops any stress server still running there and fails if that can't be
    /// confirmed: a server's ssh session ending doesn't stop it, so one whose stop didn't arrive
    /// could still be writing while the data is checked. Linux and macOS need nothing (see
    /// <see cref="StartArguments"/>).
    /// </summary>
    /// <param name="cancellationToken">Stops the check.</param>
    /// <returns>A task that completes when no server runs there.</returns>
    /// <exception cref="InvalidOperationException">ssh failed, or a server didn't stop.</exception>
    public Task EnsureStoppedAsync(CancellationToken cancellationToken) =>
        IsWindows ? SshAsync(StopWindowsServers, cancellationToken) : Task.CompletedTask;

    /// <summary>
    /// PowerShell that stops every stress server (PowerShell hands dotnet its arguments
    /// unquoted, so <see cref="ServerPattern"/> fits) and waits until they have exited, so their
    /// files are released; it fails if one is still running after 10 seconds.
    /// </summary>
    private const string StopWindowsServers =
        "$s = @(Get-CimInstance Win32_Process | Where-Object CommandLine -match '" + ServerPattern + "'); "
        + "$s | ForEach-Object { Stop-Process -Id $_.ProcessId -Force -ErrorAction SilentlyContinue }; "
        + "$s | ForEach-Object { Wait-Process -Id $_.ProcessId -Timeout 10 -ErrorAction SilentlyContinue }; "
        + "if (Get-CimInstance Win32_Process | Where-Object CommandLine -match '" + ServerPattern + "') { throw 'A stress server is still running.' }";

    private async Task PrepareWindowsAsync(string serverDirectory, string usersFile, CancellationToken cancellationToken)
    {
        // As on Linux and macOS: stop leftover servers, then this run's Server folder, the data
        // folder made from this side (after connecting to the share). Earlier runs' Server
        // copies are removed where nothing holds them. The run's own folder is new, unless a
        // run name is used again: a delete that fails then stops the run, or scp would copy into
        // a folder inside the old build.
        var runs = Combine(Home, Folder, "runs");
        await SshAsync(
            $"{StopWindowsServers}; "
                + $"Get-ChildItem -Directory {QuotePowerShell(Combine(runs, "*", "server"))} -ErrorAction SilentlyContinue "
                + "| Remove-Item -Recurse -Force -ErrorAction SilentlyContinue; $ErrorActionPreference = 'Stop'; "
                + $"if (Test-Path {QuotePowerShell(ServerFolder)}) {{ Remove-Item -Recurse -Force {QuotePowerShell(ServerFolder)} }}; {WindowsConnect}; "
                + $"New-Item -ItemType Directory -Force {QuotePowerShell(LogDirectory)}, {QuotePowerShell(DataPath)} | Out-Null",
            cancellationToken);

        // scp -r makes the missing server folder with the build's content.
        await RunAsync("scp", [.. ScpOptions, "-r", serverDirectory.TrimEnd('/'), $"{Target}:{ScpPath(ServerFolder)}"], cancellationToken);
        await SshAsync($"Remove-Item {QuotePowerShell(Combine(ServerFolder, "*.staticwebassets.runtime.json"))}", cancellationToken);
        await RunAsync("scp", [.. ScpOptions, usersFile, $"{Target}:{ScpPath(UsersFile)}"], cancellationToken);
    }

    /// <summary>
    /// A regular expression for the command line of every server started by <see cref="ServerCommand"/>.
    /// The brackets keep it from matching the shell that runs <c>pkill</c> with it.
    /// </summary>
    private const string ServerPattern = "BlazorApp[.]Server[.]dll --urls http://0[.]0[.]0[.]0:";

    /// <summary>No password prompts; for rsync and scp, which need ssh's stdin for themselves.</summary>
    private static readonly string[] RsyncSshOptions = ["-o", "BatchMode=yes"];

    /// <summary>Also <c>-n</c>: ssh mustn't read the tool's stdin (a script feeding it runs, say).</summary>
    private static readonly string[] SshOptions = ["-n", .. RsyncSshOptions];

    /// <summary>Runs a command there: a POSIX shell command, or a PowerShell script on Windows.</summary>
    private Task<string> SshAsync(string command, CancellationToken cancellationToken) =>
        RunAsync("ssh", [.. SshOptions, Target, IsWindows ? EncodedPowerShell(command) : command], cancellationToken);

    /// <summary>Joins path parts with the remote's separator.</summary>
    private string Combine(params string[] parts) => Combine(IsWindows, parts);

    private static string Combine(bool windows, params string[] parts) => string.Join(windows ? "\\" : "/", parts);

    /// <summary>A remote path as scp takes it: Windows' with forward slashes (<c>C:/Users/...</c>).</summary>
    private string ScpPath(string path) => IsWindows ? path.Replace('\\', '/') : path;

    /// <summary>Quotes a value for a POSIX shell.</summary>
    private static string Quote(string value) => $"'{value.Replace("'", "'\\''", StringComparison.Ordinal)}'";

    /// <summary>Quotes a value for PowerShell: single quotes, with a quote inside doubled.</summary>
    private static string QuotePowerShell(string value) => $"'{value.Replace("'", "''", StringComparison.Ordinal)}'";

    /// <summary>Runs a local program with its stdin closed; returns its output, or throws with its error if it fails.</summary>
    internal static async Task<string> RunAsync(string program, IEnumerable<string> args, CancellationToken cancellationToken)
    {
        var start = new ProcessStartInfo(program) { RedirectStandardInput = true, RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false };
        foreach (var arg in args)
        {
            start.ArgumentList.Add(arg);
        }

        using var process = Process.Start(start) ?? throw new InvalidOperationException($"{program} didn't start.");
        process.StandardInput.Close();
        var output = process.StandardOutput.ReadToEndAsync(cancellationToken);
        var error = process.StandardError.ReadToEndAsync(cancellationToken);
        await process.WaitForExitAsync(cancellationToken);
        return process.ExitCode == 0
            ? await output
            : throw new InvalidOperationException($"{program} {string.Join(" ", start.ArgumentList)} exited with {process.ExitCode}: {await error}");
    }
}
