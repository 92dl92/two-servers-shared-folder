using Microsoft.Extensions.Logging;

namespace BlazorApp.Services.Storage.AppendLog;

/// <summary>Registers the <c>AppendLog</c> strategy (D): <see cref="AppendLogRegistrationStore"/> in the data folder.</summary>
public sealed class AppendLogStoreFactory : IRegistrationStoreFactory
{
    /// <inheritdoc />
    public string Name => "AppendLog";

    /// <inheritdoc />
    public bool UsesFolder => true;

    /// <inheritdoc />
    public IRegistrationStore Create(string? path, ILoggerFactory loggerFactory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(loggerFactory);
        return new AppendLogRegistrationStore(path, loggerFactory.CreateLogger<AppendLogRegistrationStore>());
    }
}
