using System.Diagnostics;
using System.Net.Sockets;

namespace BlazorApp.Stress;

/// <summary>A server in a run: one the tool started (<see cref="ServerProcess"/>) or one started by hand (<see cref="ExternalServer"/>).</summary>
public interface IStressServer : IAsyncDisposable
{
    /// <summary>Its name in acks and reports.</summary>
    string Name { get; }

    /// <summary>Its port.</summary>
    int Port { get; }

    /// <summary>The machine it runs on; null for this one.</summary>
    string? Host { get; }

    /// <summary>Where its API is.</summary>
    Uri BaseAddress { get; }

    /// <summary>Whether its process has ended (known only for a server the tool started).</summary>
    bool HasExited { get; }

    /// <summary>Whether a fault killed it.</summary>
    bool Killed { get; }

    /// <summary>What it printed (known only for a server the tool started).</summary>
    string Output { get; }

    /// <summary>Kills it abruptly, as a fault.</summary>
    /// <returns>When it has been killed.</returns>
    Task KillAsync();
}

/// <summary>
/// A server started by hand on another machine (<c>ClaudeTestServer.ps1</c>, phase 6c), reached
/// at the address and port the owner gave. The tool can't start, kill or stop it: when the run
/// is done with it, <see cref="WaitUntilStoppedAsync"/> waits until a person has stopped it, so
/// the folder is read only once nothing writes to it.
/// </summary>
/// <param name="baseAddress">Its address, with the port.</param>
/// <param name="stopTimeout">How long to wait for it to be stopped.</param>
public sealed class ExternalServer(Uri baseAddress, TimeSpan stopTimeout) : IStressServer
{
    /// <inheritdoc />
    public string Name { get; } = $"{baseAddress.Host}:{baseAddress.Port}";

    /// <inheritdoc />
    public int Port { get; } = baseAddress.Port;

    /// <inheritdoc />
    public string? Host { get; } = baseAddress.Host;

    /// <inheritdoc />
    public Uri BaseAddress { get; } = baseAddress;

    /// <inheritdoc />
    public bool HasExited => false;

    /// <inheritdoc />
    public bool Killed => false;

    /// <inheritdoc />
    public string Output => "";

    /// <summary>Whether it was seen stopped (a refused connection) within the wait.</summary>
    public bool Stopped { get; private set; }

    /// <inheritdoc />
    /// <exception cref="InvalidOperationException">Always: only a person can stop it.</exception>
    public Task KillAsync() => throw new InvalidOperationException($"{Name} was started by hand; the tool can't kill it.");

    /// <summary>
    /// Waits until a person has stopped it: until a connection to its port is refused, or the
    /// wait runs out. A connection attempt that times out proves nothing (a dropped packet, a
    /// busy network), so only a refusal counts as stopped.
    /// </summary>
    /// <param name="cancellationToken">Ends the wait (Ctrl+C).</param>
    /// <returns>When it has stopped, or the wait ran out (<see cref="Stopped"/> says which).</returns>
    /// <exception cref="OperationCanceledException">The wait was cancelled.</exception>
    public async Task WaitUntilStoppedAsync(CancellationToken cancellationToken)
    {
        var waited = Stopwatch.StartNew();
        while (waited.Elapsed < stopTimeout)
        {
            if (await RefusesConnectionsAsync(cancellationToken))
            {
                Stopped = true;
                return;
            }

            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        }
    }

    /// <summary>Nothing to release: the server isn't the tool's to stop.</summary>
    /// <returns>A completed task.</returns>
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    private async Task<bool> RefusesConnectionsAsync(CancellationToken cancellationToken)
    {
        using var client = new TcpClient();
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(5));
        try
        {
            await client.ConnectAsync(BaseAddress.Host, BaseAddress.Port, timeout.Token);
            return false;
        }
        catch (SocketException e) when (e.SocketErrorCode == SocketError.ConnectionRefused)
        {
            return true;
        }
        catch (Exception e) when ((e is SocketException || e is OperationCanceledException) && !cancellationToken.IsCancellationRequested)
        {
            return false; // timed out or another network error: not known to be stopped
        }
    }
}
