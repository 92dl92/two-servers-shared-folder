using BlazorApp.Services.Storage.InMemory;
using BlazorApp.Shared.Contracts;
using BlazorApp.Shared.Dtos;
using Microsoft.Extensions.Time.Testing;

namespace BlazorApp.Services.Tests;

/// <summary>Versions, history, the version check and the workflow, on the in-memory service.</summary>
public sealed class RecordWorkflowTests
{
    private readonly FakeTimeProvider _clock = new(new DateTimeOffset(2026, 3, 1, 9, 0, 0, TimeSpan.Zero));
    private readonly SwitchableCurrentUser _user = new(TestUsers.Alice);
    private readonly InMemoryRegistrationStore _store = new();
    private readonly RegistrationService _service;

    /// <summary>Creates a service over an empty store, acting as Alice.</summary>
    public RecordWorkflowTests() => _service = new RegistrationService(_clock, _user, _store);

    [Fact]
    public async Task Create_makes_a_draft_at_version_1_with_one_history_entry()
    {
        var record = await CreateAsync();

        Assert.Equal(1, record.Version);
        Assert.Equal(RecordStatus.Draft, record.Status);
        Assert.Equal("alice", record.CreatedBy);
        Assert.Equal("alice", record.UpdatedBy);
        Assert.Equal(_clock.GetUtcNow(), record.CreatedAt);
        var entry = Assert.Single(record.History);
        Assert.Equal(new HistoryEntryDto(1, RecordAction.Create, "alice", _clock.GetUtcNow()), entry);
    }

    [Fact]
    public async Task Every_successful_change_raises_the_version_by_one_and_adds_one_history_entry()
    {
        var record = await CreateAsync();
        var steps = new (UserDto User, RecordAction Action, Func<RegistrationRecordDto, Task<RegistrationRecordDto>> Run)[]
        {
            (TestUsers.Alice, RecordAction.Update, r => _service.UpdateAsync(r.Id, r.Version, Changed(r.Data, "Paris"))),
            (TestUsers.Alice, RecordAction.Submit, r => _service.SubmitAsync(r.Id, r.Version)),
            (TestUsers.Rita, RecordAction.Comment, r => _service.CommentAsync(r.Id, r.Version, "Looks fine.")),
            (TestUsers.Alice, RecordAction.Update, r => _service.UpdateAsync(r.Id, r.Version, Changed(r.Data, "Lyon"))),
            (TestUsers.Rita, RecordAction.Approve, r => _service.ApproveAsync(r.Id, r.Version)),
            (TestUsers.Ada, RecordAction.Comment, r => _service.CommentAsync(r.Id, r.Version, "Archived.")),
        };

        foreach (var (user, action, run) in steps)
        {
            _clock.Advance(TimeSpan.FromMinutes(1));
            _user.User = user;

            var updated = await run(record);

            Assert.Equal(record.Version + 1, updated.Version);
            Assert.Equal(record.History.Count + 1, updated.History.Count);
            Assert.Equal(record.History, updated.History.Take(record.History.Count));
            var entry = updated.History[^1];
            Assert.Equal((updated.Version, action, user.Id, _clock.GetUtcNow()), (entry.Version, entry.Action, entry.UserId, entry.At));
            Assert.Equal(user.Id, updated.UpdatedBy);
            Assert.Equal(_clock.GetUtcNow(), updated.UpdatedAt);
            Assert.Equal(updated, await _service.GetAsync(record.Id), RecordComparer.Instance);
            record = updated;
        }

        Assert.Equal(7, record.Version);
        Assert.Equal(RecordStatus.Approved, record.Status);
        Assert.Equal("Lyon", record.Data.City);
        Assert.Equal("alice", record.CreatedBy);
        Assert.Equal(["Looks fine.", "Archived."], record.History.Where(h => h.Comment is not null).Select(h => h.Comment));
    }

    [Fact]
    public async Task Reject_moves_a_submitted_record_to_rejected()
    {
        var record = await CreateAsync();
        record = await _service.SubmitAsync(record.Id, record.Version);
        _user.User = TestUsers.Rita;

        record = await _service.RejectAsync(record.Id, record.Version);

        Assert.Equal(RecordStatus.Rejected, record.Status);
        Assert.Equal(3, record.Version);
    }

    /// <summary>Every change, attempted with an out-of-date version by a user who may otherwise do it.</summary>
    public static TheoryData<string> Changes => ["Update", "Submit", "Approve", "Reject", "Comment", "Delete"];

    [Theory]
    [MemberData(nameof(Changes))]
    public async Task Out_of_date_version_is_refused_with_a_conflict_and_changes_nothing(string change)
    {
        var record = await CreateAsync();
        _user.User = TestUsers.Ada;
        if (change is "Approve" or "Reject")
        {
            record = await _service.SubmitAsync(record.Id, record.Version);
        }

        var current = await _service.CommentAsync(record.Id, record.Version, "Someone else got here first.");
        var stale = record.Version;

        var e = await Assert.ThrowsAsync<VersionConflictException>(() => Run(change, record.Id, stale));

        Assert.Equal(stale, e.ExpectedVersion);
        Assert.Equal(current.Version, e.ActualVersion);
        Assert.Equal(current, await _service.GetAsync(record.Id), RecordComparer.Instance);
    }

    [Fact]
    public async Task Version_is_checked_before_status()
    {
        var record = await CreateAsync();
        await _service.SubmitAsync(record.Id, record.Version);

        // Stale version, and Submit is no longer allowed either: the conflict is what the user needs to know.
        await Assert.ThrowsAsync<VersionConflictException>(() => _service.SubmitAsync(record.Id, record.Version));
    }

    [Fact]
    public async Task Wrong_status_is_refused_and_changes_nothing()
    {
        var record = await CreateAsync();
        _user.User = TestUsers.Rita;

        var e = await Assert.ThrowsAsync<WrongStatusException>(() => _service.ApproveAsync(record.Id, record.Version));

        Assert.Equal(RecordStatus.Draft, e.Status);
        Assert.Equal(record, await _service.GetAsync(record.Id), RecordComparer.Instance);
    }

    [Fact]
    public async Task Another_registrants_record_is_forbidden()
    {
        var record = await CreateAsync();
        _user.User = TestUsers.Bob;

        var view = await Assert.ThrowsAsync<AccessDeniedException>(() => _service.GetAsync(record.Id));
        var update = await Assert.ThrowsAsync<AccessDeniedException>(
            () => _service.UpdateAsync(record.Id, record.Version, ValidRegistration.Create()));

        Assert.Equal(AccessDenial.Forbidden, view.Reason);
        Assert.Equal(AccessDenial.Forbidden, update.Reason);
    }

    [Fact]
    public async Task Missing_record_is_not_found()
    {
        _user.User = TestUsers.Ada;
        var id = Guid.NewGuid();

        await Assert.ThrowsAsync<RecordNotFoundException>(() => _service.GetAsync(id));
        await Assert.ThrowsAsync<RecordNotFoundException>(() => _service.SubmitAsync(id, 1));
        await Assert.ThrowsAsync<RecordNotFoundException>(() => _service.DeleteAsync(id, 1));
    }

    [Fact]
    public async Task Invalid_update_is_refused_with_errors_by_field_and_changes_nothing()
    {
        var record = await CreateAsync();
        var data = ValidRegistration.Create();
        data.Email = "nope";

        var e = await Assert.ThrowsAsync<RecordValidationException>(() => _service.UpdateAsync(record.Id, record.Version, data));

        Assert.Equal([nameof(RegistrationDto.Email)], e.Errors.Keys);
        Assert.Equal(record, await _service.GetAsync(record.Id), RecordComparer.Instance);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Empty_comment_is_refused(string text)
    {
        var record = await CreateAsync();
        _user.User = TestUsers.Rita;

        var e = await Assert.ThrowsAsync<RecordValidationException>(() => _service.CommentAsync(record.Id, record.Version, text));

        Assert.Equal(["Text"], e.Errors.Keys);
    }

    [Fact]
    public async Task Overlong_comment_is_refused_and_the_maximum_is_accepted()
    {
        var record = await CreateAsync();
        _user.User = TestUsers.Rita;

        await Assert.ThrowsAsync<RecordValidationException>(
            () => _service.CommentAsync(record.Id, record.Version, new string('a', RegistrationService.MaxCommentLength + 1)));
        var ok = await _service.CommentAsync(record.Id, record.Version, new string('a', RegistrationService.MaxCommentLength));

        Assert.Equal(2, ok.Version);
    }

    [Fact]
    public async Task Delete_removes_the_record()
    {
        var record = await CreateAsync();
        _user.User = TestUsers.Ada;

        await _service.DeleteAsync(record.Id, record.Version);

        await Assert.ThrowsAsync<RecordNotFoundException>(() => _service.GetAsync(record.Id));
        Assert.Empty(await _service.GetAllAsync());
    }

    [Fact]
    public async Task Returned_records_are_copies()
    {
        var record = await CreateAsync();

        record.Data.City = "Changed by the caller";

        Assert.Equal("London", (await _service.GetAsync(record.Id)).Data.City);
    }

    [Fact]
    public async Task List_shows_status_and_version()
    {
        var record = await CreateAsync();
        await _service.SubmitAsync(record.Id, record.Version);

        var summary = Assert.Single(await _service.GetAllAsync());

        Assert.Equal((RecordStatus.Submitted, 2), (summary.Status, summary.Version));
    }

    /// <summary>Many threads, one record, all starting from the same version: exactly one may win.</summary>
    [Theory]
    [InlineData("Update")]
    [InlineData("Comment")]
    [InlineData("Mixed")]
    public async Task Many_threads_from_the_same_version_exactly_one_succeeds(string kind)
    {
        const int threads = 64;
        var record = await CreateAsync();
        _user.User = TestUsers.Ada;
        using var start = new Barrier(threads);

        // Dedicated threads (LongRunning): blocking pool threads on the barrier would starve the pool.
        var outcomes = await Task.WhenAll(Enumerable.Range(0, threads).Select(i => Task.Factory.StartNew(async () =>
        {
            start.SignalAndWait();
            var change = kind == "Mixed" ? MixedChanges[i % MixedChanges.Length] : kind;
            try
            {
                await Run(change, record.Id, record.Version, marker: $"thread {i}");
                return "ok";
            }
            catch (VersionConflictException)
            {
                return "409";
            }
        }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap()));

        Assert.Equal(1, outcomes.Count(o => o == "ok"));
        Assert.Equal(threads - 1, outcomes.Count(o => o == "409"));
        var final = await _service.GetAsync(record.Id);
        Assert.Equal(2, final.Version);
        Assert.Equal(2, final.History.Count);
    }

    /// <summary>Changes that are all allowed on a draft, for the mixed race.</summary>
    private static readonly string[] MixedChanges = ["Update", "Comment", "Submit"];

    private async Task<RegistrationRecordDto> CreateAsync()
    {
        var result = await _service.CreateAsync(ValidRegistration.Create());
        return await _service.GetAsync(result.Id!.Value);
    }

    private Task Run(string change, Guid id, int version, string marker = "x") => change switch
    {
        "Update" => _service.UpdateAsync(id, version, Changed(ValidRegistration.Create(), marker)),
        "Submit" => _service.SubmitAsync(id, version),
        "Approve" => _service.ApproveAsync(id, version),
        "Reject" => _service.RejectAsync(id, version),
        "Comment" => _service.CommentAsync(id, version, marker),
        "Delete" => _service.DeleteAsync(id, version),
        _ => throw new ArgumentOutOfRangeException(nameof(change)),
    };

    private static RegistrationDto Changed(RegistrationDto data, string city)
    {
        var copy = ValidRegistration.Create();
        copy.FirstName = data.FirstName;
        copy.City = city;
        return copy;
    }
}
