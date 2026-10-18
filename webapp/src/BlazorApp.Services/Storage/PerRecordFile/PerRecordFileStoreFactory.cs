using Microsoft.Extensions.Logging;

namespace BlazorApp.Services.Storage.PerRecordFile;

/// <summary>Registers the <c>PerRecordFile</c> strategy (C): <see cref="PerRecordFileStore"/>, one file per record in the shared folder.</summary>
public sealed class PerRecordFileStoreFactory : IRegistrationStoreFactory
{
    /// <inheritdoc />
    public string Name => "PerRecordFile";

    /// <inheritdoc />
    public bool UsesFolder => true;

    /// <inheritdoc />
    public IRegistrationStore Create(string? path, ILoggerFactory loggerFactory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(loggerFactory);
        return new PerRecordFileStore(path, loggerFactory.CreateLogger<PerRecordFileStore>());
    }
}
