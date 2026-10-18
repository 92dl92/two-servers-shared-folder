using Microsoft.Extensions.Logging;

namespace BlazorApp.Services.Storage.InMemory;

/// <summary>Registers the <c>InMemory</c> strategy: <see cref="InMemoryRegistrationStore"/>, no folder.</summary>
public sealed class InMemoryStoreFactory : IRegistrationStoreFactory
{
    /// <inheritdoc />
    public string Name => StoreStrategies.Default;

    /// <inheritdoc />
    public bool UsesFolder => false;

    /// <inheritdoc />
    public IRegistrationStore Create(string? path, ILoggerFactory loggerFactory)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);
        return new InMemoryRegistrationStore(loggerFactory.CreateLogger<InMemoryRegistrationStore>());
    }
}
