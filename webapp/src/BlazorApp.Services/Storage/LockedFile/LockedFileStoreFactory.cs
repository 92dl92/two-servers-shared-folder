using Microsoft.Extensions.Logging;

namespace BlazorApp.Services.Storage.LockedFile;

/// <summary>Registers the <c>LockedFile</c> strategy (B): <see cref="LockedFileRegistrationStore"/> in the data folder.</summary>
public sealed class LockedFileStoreFactory : IRegistrationStoreFactory
{
    /// <inheritdoc />
    public string Name => "LockedFile";

    /// <inheritdoc />
    public bool UsesFolder => true;

    /// <inheritdoc />
    public IRegistrationStore Create(string? path, ILoggerFactory loggerFactory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(loggerFactory);
        return new LockedFileRegistrationStore(path, loggerFactory.CreateLogger<LockedFileRegistrationStore>());
    }
}
