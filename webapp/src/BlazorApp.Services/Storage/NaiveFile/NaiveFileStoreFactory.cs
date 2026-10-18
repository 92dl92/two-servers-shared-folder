using Microsoft.Extensions.Logging;

namespace BlazorApp.Services.Storage.NaiveFile;

/// <summary>Registers the <c>NaiveFile</c> strategy (A, the control): <see cref="NaiveFileRegistrationStore"/> in the data folder.</summary>
public sealed class NaiveFileStoreFactory : IRegistrationStoreFactory
{
    /// <inheritdoc />
    public string Name => "NaiveFile";

    /// <inheritdoc />
    public bool UsesFolder => true;

    /// <inheritdoc />
    public IRegistrationStore Create(string? path, ILoggerFactory loggerFactory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(loggerFactory);
        return new NaiveFileRegistrationStore(path, loggerFactory.CreateLogger<NaiveFileRegistrationStore>());
    }
}
