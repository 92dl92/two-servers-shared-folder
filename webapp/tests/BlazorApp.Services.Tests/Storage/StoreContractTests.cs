using System.Reflection;
using BlazorApp.Services.Storage;
using BlazorApp.Services.Storage.InMemory;
using Microsoft.Extensions.Logging;

namespace BlazorApp.Services.Tests.Storage;

/// <summary>
/// The in-memory store against every part of the shared suite that applies to it. It keeps
/// nothing in a folder, so restarting and several processes don't apply.
/// </summary>
public sealed class InMemoryStoreContractTests : RegistrationStoreContract
{
    private InMemoryRegistrationStore? _store;

    /// <inheritdoc />
    /// <remarks>One instance per test, made on first use; the folder is ignored.</remarks>
    protected override IRegistrationStore OpenStore(string folder, ILoggerFactory loggerFactory) =>
        _store ??= (InMemoryRegistrationStore)new InMemoryStoreFactory().Create(null, loggerFactory);
}

/// <summary>
/// Positive control: the test-only locked-folder store passes the whole suite, including
/// several processes, so the suite and the multi-process helper work when a store is right.
/// </summary>
public sealed class LockedFolderControlTests : PersistentRegistrationStoreContract
{
    /// <inheritdoc />
    protected override string Strategy => "TestLockedFolder";

    /// <inheritdoc />
    protected override IEnumerable<Assembly> PluginAssemblies => [typeof(LockedFolderTestStore).Assembly];
}
