using BlazorApp.Shared.Dtos;

namespace BlazorApp.Services.Storage;

/// <summary>
/// Where registration records are kept. Each storage strategy (in memory, files, SQLite, …)
/// implements this, and every implementation must pass the shared test suite in
/// <c>BlazorApp.Services.Tests/Storage</c>.
/// </summary>
/// <remarks>
/// <para>The contract, which several servers sharing one folder rely on:</para>
/// <list type="bullet">
/// <item>Reads return the latest saved state. A store must not serve a copy kept in memory that
/// another process may have changed since.</item>
/// <item>Writes are conditional and atomic: <see cref="TryReplaceAsync"/> and
/// <see cref="TryDeleteAsync"/> succeed only if the stored version still equals the expected
/// version at the moment of writing, even when other threads or processes write at the same
/// time. Two writers starting from the same version can never both succeed.</item>
/// <item>A write reported as <see cref="WriteResult.Saved"/> is never lost later.</item>
/// <item>Records going in and out are copies: changing a returned record never changes stored data.</item>
/// </list>
/// </remarks>
public interface IRegistrationStore
{
    /// <summary>Gets a record.</summary>
    /// <param name="id">Record id.</param>
    /// <param name="cancellationToken">Cancels the operation.</param>
    /// <returns>The record, or null if it does not exist.</returns>
    Task<RegistrationRecordDto?> GetAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Lists every record, in no particular order.</summary>
    /// <param name="cancellationToken">Cancels the operation.</param>
    /// <returns>All records.</returns>
    Task<IReadOnlyList<RegistrationRecordDto>> ListAsync(CancellationToken cancellationToken = default);

    /// <summary>Adds a new record.</summary>
    /// <param name="record">The record; its id must not exist yet.</param>
    /// <param name="cancellationToken">Cancels the operation.</param>
    /// <returns><see cref="WriteResult.Saved"/>, or <see cref="WriteResult.Conflict"/> if the id exists.</returns>
    Task<WriteResult> TryCreateAsync(RegistrationRecordDto record, CancellationToken cancellationToken = default);

    /// <summary>Replaces a record if its stored version is still <paramref name="expectedVersion"/>.</summary>
    /// <param name="updated">The new record: same id, version <paramref name="expectedVersion"/> + 1.</param>
    /// <param name="expectedVersion">The version the caller read.</param>
    /// <param name="cancellationToken">Cancels the operation.</param>
    /// <returns>Whether the replacement happened, and if not, why.</returns>
    /// <exception cref="ArgumentException"><paramref name="updated"/> does not have version <paramref name="expectedVersion"/> + 1.</exception>
    Task<WriteResult> TryReplaceAsync(RegistrationRecordDto updated, int expectedVersion, CancellationToken cancellationToken = default);

    /// <summary>Deletes a record if its stored version is still <paramref name="expectedVersion"/>.</summary>
    /// <param name="id">Record id.</param>
    /// <param name="expectedVersion">The version the caller read.</param>
    /// <param name="cancellationToken">Cancels the operation.</param>
    /// <returns>Whether the deletion happened, and if not, why.</returns>
    Task<WriteResult> TryDeleteAsync(Guid id, int expectedVersion, CancellationToken cancellationToken = default);
}
