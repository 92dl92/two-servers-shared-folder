using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace BlazorApp.Services.Storage;

/// <summary>Finds storage strategies (<see cref="IRegistrationStoreFactory"/> implementations) and opens stores by name.</summary>
public static class StoreStrategies
{
    /// <summary>The strategy used when none is configured.</summary>
    public const string Default = "InMemory";

    /// <summary>Finds every strategy in this assembly and in <paramref name="extraAssemblies"/>.</summary>
    /// <param name="extraAssemblies">More assemblies to search, e.g. test-only stores.</param>
    /// <returns>Factories keyed by name (case-insensitive).</returns>
    /// <exception cref="InvalidOperationException">Two strategies share a name.</exception>
    public static IReadOnlyDictionary<string, IRegistrationStoreFactory> Discover(params Assembly[] extraAssemblies)
    {
        var factories = new Dictionary<string, IRegistrationStoreFactory>(StringComparer.OrdinalIgnoreCase);
        foreach (var assembly in extraAssemblies.Prepend(typeof(StoreStrategies).Assembly).Distinct())
        {
            var types = assembly.GetTypes().Where(t =>
                t is { IsClass: true, IsAbstract: false, IsPublic: true }
                && typeof(IRegistrationStoreFactory).IsAssignableFrom(t)
                && t.GetConstructor(Type.EmptyTypes) is not null);

            foreach (var type in types)
            {
                var factory = (IRegistrationStoreFactory)Activator.CreateInstance(type)!;
                if (!factories.TryAdd(factory.Name, factory))
                {
                    throw new InvalidOperationException(
                        $"Two storage strategies are called '{factory.Name}': {factories[factory.Name].GetType()} and {type}.");
                }
            }
        }

        return factories;
    }

    /// <summary>Opens a store by strategy name, without logging.</summary>
    /// <param name="name">Strategy name; case-insensitive.</param>
    /// <param name="path">The shared data folder; required by strategies that use one.</param>
    /// <param name="extraAssemblies">More assemblies to search for strategies.</param>
    /// <returns>The open store.</returns>
    /// <exception cref="ArgumentException">Unknown strategy, or a folder-based strategy without a path.</exception>
    public static IRegistrationStore Create(string name, string? path, params Assembly[] extraAssemblies) =>
        Create(name, path, NullLoggerFactory.Instance, extraAssemblies);

    /// <summary>Opens a store by strategy name.</summary>
    /// <param name="name">Strategy name; case-insensitive.</param>
    /// <param name="path">The shared data folder; required by strategies that use one.</param>
    /// <param name="loggerFactory">Passed to the store for its Debug logging.</param>
    /// <param name="extraAssemblies">More assemblies to search for strategies.</param>
    /// <returns>The open store.</returns>
    /// <exception cref="ArgumentException">Unknown strategy, or a folder-based strategy without a path.</exception>
    public static IRegistrationStore Create(string name, string? path, ILoggerFactory loggerFactory, params Assembly[] extraAssemblies) =>
        Resolve(name, path, extraAssemblies).Create(path, loggerFactory);

    /// <summary>Finds a strategy by name and checks its settings, without opening it.</summary>
    /// <param name="name">Strategy name; case-insensitive.</param>
    /// <param name="path">The shared data folder; required by strategies that use one.</param>
    /// <param name="extraAssemblies">More assemblies to search for strategies.</param>
    /// <returns>The strategy's factory.</returns>
    /// <exception cref="ArgumentException">Unknown strategy, or a folder-based strategy without a path.</exception>
    public static IRegistrationStoreFactory Resolve(string name, string? path, params Assembly[] extraAssemblies)
    {
        var all = Discover(extraAssemblies);
        if (!all.TryGetValue(name, out var factory))
        {
            throw new ArgumentException(
                $"Unknown storage strategy '{name}'. Known: {string.Join(", ", all.Keys.Order(StringComparer.OrdinalIgnoreCase))}.",
                nameof(name));
        }

        if (factory.UsesFolder && string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException($"Storage strategy '{factory.Name}' needs a folder (Storage:Path).", nameof(path));
        }

        return factory;
    }
}
