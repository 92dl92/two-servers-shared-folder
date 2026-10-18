using Microsoft.Extensions.Logging;

namespace BlazorApp.Services.Storage;

/// <summary>
/// Creates the store for one storage strategy. Each strategy provides one public
/// implementation with a parameterless constructor in its own folder
/// (<c>Storage/&lt;Strategy&gt;/</c>); <see cref="StoreStrategies"/> finds it by reflection, so
/// adding a strategy never means editing shared files.
/// </summary>
public interface IRegistrationStoreFactory
{
    /// <summary>The name used in settings, e.g. <c>--Storage:Strategy=PerRecordFile</c>. Compared case-insensitively.</summary>
    string Name { get; }

    /// <summary>Whether the store keeps data in the folder, so several processes can share it.</summary>
    bool UsesFolder { get; }

    /// <summary>Opens the store.</summary>
    /// <param name="path">The shared data folder (<c>Storage:Path</c>); null for strategies that don't use one.</param>
    /// <param name="loggerFactory">For the store's Debug logging of reads, writes and conflicts (<see cref="StoreLog"/>).</param>
    /// <returns>A store; safe to use from many threads at once.</returns>
    IRegistrationStore Create(string? path, ILoggerFactory loggerFactory);
}
