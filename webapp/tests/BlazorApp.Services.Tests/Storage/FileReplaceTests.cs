using System.Diagnostics;
using BlazorApp.Services.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Testing;

namespace BlazorApp.Services.Tests.Storage;

/// <summary>
/// The rename B and C save with (phase 6b). Windows' "file in use" can't be produced on Linux,
/// so the rename is a stand-in that refuses a set number of times.
/// </summary>
public sealed class FileReplaceTests
{
    public static TheoryData<Exception> InUse => new()
    {
        new UnauthorizedAccessException("Access to the path is denied."),
        new IOException("sharing violation") { HResult = unchecked((int)0x80070020) },
        new IOException("lock violation") { HResult = unchecked((int)0x80070021) },
    };

    [Theory]
    [MemberData(nameof(InUse))]
    public void A_rename_refused_because_the_file_is_in_use_is_tried_again(Exception inUse)
    {
        var calls = 0;

        var log = new FakeLogger();

        var retries = FileReplace.Move(
            "a.tmp",
            Path.Combine("data", "records", "a.json"),
            log,
            (_, _) =>
            {
                if (++calls <= 2)
                {
                    throw inUse;
                }
            },
            windows: true);

        Assert.Equal(2, retries);
        Assert.Equal(3, calls);

        // One warning per replace that needed retries, naming the file only (no folders).
        var entry = Assert.Single(log.Collector.GetSnapshot());
        Assert.Equal(StoreLog.ReplaceRetried, entry.Id);
        Assert.Equal(LogLevel.Warning, entry.Level);
        Assert.Contains("a.json succeeded after 2 retries", entry.Message, StringComparison.Ordinal);
        Assert.DoesNotContain("records", entry.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void A_rename_that_succeeds_at_once_logs_nothing()
    {
        var log = new FakeLogger();

        Assert.Equal(0, FileReplace.Move("a.tmp", "a.json", log, (_, _) => { }, windows: true));

        Assert.Empty(log.Collector.GetSnapshot());
    }

    [Fact]
    public void A_rename_still_refused_after_the_wait_throws_the_error_and_says_so()
    {
        var log = new FakeLogger();
        var waited = Stopwatch.StartNew();

        Assert.Throws<UnauthorizedAccessException>(() =>
            FileReplace.Move("a.tmp", "a.json", log, (_, _) => throw new UnauthorizedAccessException(), windows: true));

        // Generous on top: a busy test machine may oversleep.
        Assert.InRange(waited.Elapsed, FileReplace.MaxWait, FileReplace.MaxWait * 5);
        var entry = Assert.Single(log.Collector.GetSnapshot());
        Assert.Equal(StoreLog.ReplaceRetried, entry.Id);
        Assert.Contains("a.json failed after", entry.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Access_denied_is_not_retried_outside_windows()
    {
        // Linux and macOS never refuse a rename because the file is open, so access denied
        // there is a real permission problem, which waiting wouldn't fix.
        var calls = 0;

        Assert.Throws<UnauthorizedAccessException>(() =>
            FileReplace.Move("a.tmp", "a.json", NullLogger.Instance, (_, _) =>
            {
                calls++;
                throw new UnauthorizedAccessException();
            }, windows: false));

        Assert.Equal(1, calls);
    }

    [Fact]
    public void Any_other_error_is_thrown_at_once()
    {
        var calls = 0;

        Assert.Throws<FileNotFoundException>(() =>
            FileReplace.Move("a.tmp", "a.json", NullLogger.Instance, (_, _) =>
            {
                calls++;
                throw new FileNotFoundException();
            }));

        Assert.Equal(1, calls);
    }

    [Fact]
    public void A_real_rename_replaces_the_file()
    {
        using var folder = new TempFolder();
        var source = Path.Combine(folder.Path, "new.tmp");
        var destination = Path.Combine(folder.Path, "record.json");
        File.WriteAllText(source, "new");
        File.WriteAllText(destination, "old");

        Assert.Equal(0, FileReplace.Move(source, destination, NullLogger.Instance));

        Assert.Equal("new", File.ReadAllText(destination));
        Assert.False(File.Exists(source));
    }
}
