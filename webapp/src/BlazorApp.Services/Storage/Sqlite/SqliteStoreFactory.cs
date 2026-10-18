using Microsoft.Extensions.Logging;

namespace BlazorApp.Services.Storage.Sqlite;

/// <summary>Registers the <c>Sqlite</c> strategy (E): <see cref="SqliteRegistrationStore"/>, one database file in the folder.</summary>
public sealed class SqliteStoreFactory : IRegistrationStoreFactory
{
    /// <inheritdoc />
    public string Name => "Sqlite";

    /// <inheritdoc />
    public bool UsesFolder => true;

    /// <inheritdoc />
    public IRegistrationStore Create(string? path, ILoggerFactory loggerFactory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(loggerFactory);
        return new SqliteRegistrationStore(path, loggerFactory.CreateLogger<SqliteRegistrationStore>());
    }
}
