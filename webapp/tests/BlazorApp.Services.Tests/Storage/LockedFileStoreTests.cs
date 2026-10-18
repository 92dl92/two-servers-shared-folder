using System.Text.Json;
using BlazorApp.Services.Storage;
using BlazorApp.Services.Storage.LockedFile;
using BlazorApp.Shared.Dtos;

namespace BlazorApp.Services.Tests.Storage;

/// <summary>
/// Strategy B, <c>LockedFile</c>: one JSON file holding every record, each operation done while
/// holding an OS lock on a lock file. Runs the whole shared suite, plus checks of its own lock.
/// </summary>
/// <remarks>
/// The lock tests hold the lock through a second handle in this process, which <c>flock</c>
/// treats like another process; the shared multi-process test covers real separate processes.
/// </remarks>
public sealed class LockedFileStoreTests : PersistentRegistrationStoreContract
{
    private string LockFile => Path.Combine(Folder.Path, LockedFileRegistrationStore.LockFileName);

    private string DataFile => Path.Combine(Folder.Path, LockedFileRegistrationStore.DataFileName);

    /// <inheritdoc />
    protected override string Strategy => "LockedFile";

    [Fact]
    public async Task All_records_are_kept_in_one_file()
    {
        var store = OpenStore(Folder.Path);
        await store.TryCreateAsync(Sample());
        await store.TryCreateAsync(Sample());

        Assert.Equal(2, JsonDocument.Parse(await File.ReadAllBytesAsync(DataFile)).RootElement.GetArrayLength());
        Assert.Equal(
            [LockedFileRegistrationStore.DataFileName, LockedFileRegistrationStore.LockFileName],
            Directory.EnumerateFiles(Folder.Path).Select(Path.GetFileName).Order());
    }

    [Fact]
    public async Task A_write_waits_while_another_handle_holds_the_lock()
    {
        var store = OpenStore(Folder.Path);
        var record = Sample();
        await store.TryCreateAsync(record);

        Task<WriteResult> write;
        using (new FileStream(LockFile, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
        {
            write = store.TryReplaceAsync(Next(record, "waits"), 1);
            await Task.Delay(300);
            Assert.False(write.IsCompleted, "The write went ahead while the lock was held.");
        }

        Assert.Equal(WriteResult.Saved, await write.WaitAsync(TimeSpan.FromSeconds(10)));
        Assert.Equal(2, (await store.GetAsync(record.Id))!.Version);
    }

    [Fact]
    public async Task A_read_waits_while_another_handle_holds_the_lock()
    {
        var store = OpenStore(Folder.Path);

        Task<IReadOnlyList<RegistrationRecordDto>> list;
        using (new FileStream(LockFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
        {
            list = store.ListAsync();
            await Task.Delay(300);
            Assert.False(list.IsCompleted, "The read went ahead while the lock was held.");
        }

        Assert.Empty(await list.WaitAsync(TimeSpan.FromSeconds(10)));
    }

    [Fact]
    public async Task Waiting_for_the_lock_can_be_cancelled()
    {
        var store = OpenStore(Folder.Path);
        using var cancel = new CancellationTokenSource(TimeSpan.FromMilliseconds(200));

        using (new FileStream(LockFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
        {
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => store.TryCreateAsync(Sample(), cancel.Token));
        }

        Assert.Empty(await store.ListAsync());
    }

    [Theory]
    [InlineData("[ { \"id\": ")]
    [InlineData("null")]
    [InlineData("[null]")]
    public async Task A_broken_data_file_is_reported_and_never_overwritten(string broken)
    {
        var store = OpenStore(Folder.Path);
        await File.WriteAllTextAsync(DataFile, broken);

        await Assert.ThrowsAnyAsync<JsonException>(() => store.ListAsync());
        await Assert.ThrowsAnyAsync<JsonException>(() => store.GetAsync(Guid.NewGuid()));
        await Assert.ThrowsAnyAsync<JsonException>(() => store.TryCreateAsync(Sample()));
        Assert.Equal(broken, await File.ReadAllTextAsync(DataFile));
    }
}
