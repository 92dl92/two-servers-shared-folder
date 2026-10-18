using BlazorApp.Services.Storage;
using BlazorApp.Shared.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace BlazorApp.Services;

/// <summary>Registers the Services layer with the dependency-injection container.</summary>
public static class ServiceCollectionExtensions
{
    /// <summary>Adds the server-side implementations of the Shared contracts and the configured store.</summary>
    /// <remarks>The host must also register <see cref="Users.IUserDirectory"/> and <see cref="Users.ICurrentUser"/>.</remarks>
    /// <param name="services">The service collection to add to.</param>
    /// <param name="strategy">Storage strategy name (<c>Storage:Strategy</c>); see <see cref="StoreStrategies"/>.</param>
    /// <param name="path">Shared data folder (<c>Storage:Path</c>), for strategies that use one.</param>
    /// <returns>The same collection, for chaining.</returns>
    public static IServiceCollection AddAppServices(
        this IServiceCollection services,
        string strategy = StoreStrategies.Default,
        string? path = null)
    {
        services.AddSingleton(TimeProvider.System);

        // One store per process. The strategy is checked now so a bad setting fails at startup;
        // the store is opened once the container exists, so it can get the logger factory.
        // The service is per request because it acts as that request's user (ICurrentUser,
        // registered by the host).
        var factory = StoreStrategies.Resolve(strategy, path);
        services.AddSingleton(sp => factory.Create(path, sp.GetService<ILoggerFactory>() ?? NullLoggerFactory.Instance));
        services.AddScoped<IRegistrationService, RegistrationService>();

        return services;
    }
}
