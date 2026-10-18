namespace BlazorApp.Services.Tests.Storage;

/// <summary>
/// Strategy A, the naive single file (the control), against the whole shared suite. It is
/// expected to lose updates, so the concurrency tests are reversed: they pass only if updates
/// <em>are</em> lost, which shows the suite can detect lost updates at all (plan section 6, 4b).
/// </summary>
public sealed class NaiveFileStoreTests : PersistentRegistrationStoreContract
{
    /// <inheritdoc />
    protected override string Strategy => "NaiveFile";

    /// <inheritdoc />
    protected override bool ExpectsLostUpdates => true;
}
