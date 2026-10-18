using BlazorApp.Services.Logging;
using BlazorApp.Services.Storage;
using BlazorApp.Services.Storage.InMemory;
using BlazorApp.Shared.Contracts;
using BlazorApp.Shared.Dtos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Microsoft.Extensions.Time.Testing;
using static BlazorApp.Services.Logging.RecordOutcome;
using static BlazorApp.Shared.Dtos.RecordAction;

namespace BlazorApp.Services.Tests;

/// <summary>
/// Every action × outcome writes exactly one record event with the expected id, level and
/// fields (plan section 8). The table is written out by hand, not derived from the code.
/// </summary>
public sealed class RecordEventTableTests
{
    /// <summary>Every reachable action × outcome: event id, event name and level.</summary>
    public static TheoryData<RecordAction, RecordOutcome, int, string, LogLevel> Table => new()
    {
        { Create, Saved, 1100, "RecordCreateSaved", LogLevel.Information },
        { Create, NoUser, 1101, "RecordCreateNoUser", LogLevel.Warning },
        { Create, Forbidden, 1102, "RecordCreateForbidden", LogLevel.Warning },
        { Create, Invalid, 1104, "RecordCreateInvalid", LogLevel.Warning },

        { Update, Saved, 1200, "RecordUpdateSaved", LogLevel.Information },
        { Update, NoUser, 1201, "RecordUpdateNoUser", LogLevel.Warning },
        { Update, Forbidden, 1202, "RecordUpdateForbidden", LogLevel.Warning },
        { Update, NotFound, 1203, "RecordUpdateNotFound", LogLevel.Warning },
        { Update, Invalid, 1204, "RecordUpdateInvalid", LogLevel.Warning },
        { Update, Conflict, 1205, "RecordUpdateConflict", LogLevel.Warning },
        { Update, WrongStatus, 1206, "RecordUpdateWrongStatus", LogLevel.Warning },

        { Submit, Saved, 1300, "RecordSubmitSaved", LogLevel.Information },
        { Submit, NoUser, 1301, "RecordSubmitNoUser", LogLevel.Warning },
        { Submit, Forbidden, 1302, "RecordSubmitForbidden", LogLevel.Warning },
        { Submit, NotFound, 1303, "RecordSubmitNotFound", LogLevel.Warning },
        { Submit, Conflict, 1305, "RecordSubmitConflict", LogLevel.Warning },
        { Submit, WrongStatus, 1306, "RecordSubmitWrongStatus", LogLevel.Warning },

        { Approve, Saved, 1400, "RecordApproveSaved", LogLevel.Information },
        { Approve, NoUser, 1401, "RecordApproveNoUser", LogLevel.Warning },
        { Approve, Forbidden, 1402, "RecordApproveForbidden", LogLevel.Warning },
        { Approve, NotFound, 1403, "RecordApproveNotFound", LogLevel.Warning },
        { Approve, Conflict, 1405, "RecordApproveConflict", LogLevel.Warning },
        { Approve, WrongStatus, 1406, "RecordApproveWrongStatus", LogLevel.Warning },

        { Reject, Saved, 1500, "RecordRejectSaved", LogLevel.Information },
        { Reject, NoUser, 1501, "RecordRejectNoUser", LogLevel.Warning },
        { Reject, Forbidden, 1502, "RecordRejectForbidden", LogLevel.Warning },
        { Reject, NotFound, 1503, "RecordRejectNotFound", LogLevel.Warning },
        { Reject, Conflict, 1505, "RecordRejectConflict", LogLevel.Warning },
        { Reject, WrongStatus, 1506, "RecordRejectWrongStatus", LogLevel.Warning },

        { Comment, Saved, 1600, "RecordCommentSaved", LogLevel.Information },
        { Comment, NoUser, 1601, "RecordCommentNoUser", LogLevel.Warning },
        { Comment, Forbidden, 1602, "RecordCommentForbidden", LogLevel.Warning },
        { Comment, NotFound, 1603, "RecordCommentNotFound", LogLevel.Warning },
        { Comment, Invalid, 1604, "RecordCommentInvalid", LogLevel.Warning },
        { Comment, Conflict, 1605, "RecordCommentConflict", LogLevel.Warning },

        { Delete, Saved, 1700, "RecordDeleteSaved", LogLevel.Information },
        { Delete, NoUser, 1701, "RecordDeleteNoUser", LogLevel.Warning },
        { Delete, Forbidden, 1702, "RecordDeleteForbidden", LogLevel.Warning },
        { Delete, NotFound, 1703, "RecordDeleteNotFound", LogLevel.Warning },
        { Delete, Conflict, 1705, "RecordDeleteConflict", LogLevel.Warning },

        { View, Read, 1807, "RecordViewRead", LogLevel.Debug },
        { View, NoUser, 1801, "RecordViewNoUser", LogLevel.Debug },
        { View, Forbidden, 1802, "RecordViewForbidden", LogLevel.Debug },
        { View, NotFound, 1803, "RecordViewNotFound", LogLevel.Debug },
    };

    private readonly FakeTimeProvider _clock = new(new DateTimeOffset(2026, 3, 1, 9, 0, 0, TimeSpan.Zero));
    private readonly SwitchableCurrentUser _user = new(TestUsers.Alice);
    private readonly InMemoryRegistrationStore _store = new();
    private readonly FakeLogger<RegistrationService> _log = new();
    private readonly RegistrationService _service;

    /// <summary>A service over an empty store with a fake logger.</summary>
    public RecordEventTableTests() => _service = new RegistrationService(_clock, _user, _store, _log);

    [Fact]
    public void Table_covers_every_reachable_action_and_outcome()
    {
        var rows = Table.Select(r => ((RecordAction)r[0], (RecordOutcome)r[1])).ToHashSet();

        // Every action has a success row and NoUser and Forbidden rows.
        foreach (var action in Enum.GetValues<RecordAction>())
        {
            Assert.Contains((action, action == View ? Read : Saved), rows);
            Assert.Contains((action, NoUser), rows);
            Assert.Contains((action, Forbidden), rows);
        }

        Assert.Equal(44, rows.Count);
    }

    [Theory]
    [MemberData(nameof(Table))]
    public async Task Each_action_and_outcome_writes_exactly_one_record_event(
        RecordAction action, RecordOutcome outcome, int eventId, string eventName, LogLevel level)
    {
        var (id, version) = await ArrangeAsync(action, outcome);
        _log.Collector.Clear();
        var targetId = outcome == NotFound ? Guid.NewGuid() : id;

        try
        {
            await ActAsync(action, outcome, targetId, version);
        }
        catch (Exception e) when (e is AccessDeniedException or RecordNotFoundException or RecordValidationException
                                      or VersionConflictException or WrongStatusException)
        {
            // Refusals are expected; the event is what's checked.
        }

        var entry = Assert.Single(_log.Collector.GetSnapshot(), r => r.Id.Id is >= 1100 and < 1900);
        Assert.Equal(eventId, entry.Id.Id);
        Assert.Equal(eventName, entry.Id.Name);
        Assert.Equal(level, entry.Level);
        Assert.Equal(action.ToString(), entry.GetStructuredStateValue("Action"));
        Assert.Equal(outcome.ToString(), entry.GetStructuredStateValue("Outcome"));
        Assert.Equal(_user.RequestedId, entry.GetStructuredStateValue("UserId"));
        Assert.NotNull(entry.GetStructuredStateValue("DurationMs"));
        if (action == Create)
        {
            // Create's id only exists once it is saved.
            Assert.Equal(outcome == Saved, entry.GetStructuredStateValue("RecordId") is not null);
        }
        else
        {
            Assert.Equal(targetId.ToString(), entry.GetStructuredStateValue("RecordId"));
        }

        if (outcome == Saved && action != Delete)
        {
            var expectedNew = action == Create ? 1 : version + 1;
            Assert.Equal(expectedNew.ToString(System.Globalization.CultureInfo.InvariantCulture), entry.GetStructuredStateValue("NewVersion"));
        }

        if (outcome == Invalid)
        {
            Assert.False(string.IsNullOrEmpty(entry.GetStructuredStateValue("InvalidFields")));
        }
    }

    [Fact]
    public async Task Saved_change_records_versions_and_status_change()
    {
        var created = await _service.CreateAsync(ValidRegistration.Create());
        _log.Collector.Clear();

        await _service.SubmitAsync(created.Id!.Value, 1);

        var entry = _log.Collector.LatestRecord;
        Assert.Equal("1", entry.GetStructuredStateValue("ExpectedVersion"));
        Assert.Equal("2", entry.GetStructuredStateValue("NewVersion"));
        Assert.Equal("Draft", entry.GetStructuredStateValue("OldStatus"));
        Assert.Equal("Submitted", entry.GetStructuredStateValue("NewStatus"));
    }

    [Fact]
    public async Task Duration_is_measured_with_the_clock()
    {
        var timed = new RegistrationService(_clock, _user, new SlowStore(_store, _clock), _log);

        await timed.CreateAsync(ValidRegistration.Create());

        Assert.Equal("250", _log.Collector.LatestRecord.GetStructuredStateValue("DurationMs"));
    }

    [Fact]
    public async Task Invalid_create_lists_the_invalid_field_names()
    {
        var data = ValidRegistration.Create();
        data.Email = "nope";
        data.City = "";

        await _service.CreateAsync(data);

        Assert.Equal("City, Email", _log.Collector.LatestRecord.GetStructuredStateValue("InvalidFields"));
    }

    [Fact]
    public async Task Listing_records_is_a_View_event_without_a_record_id()
    {
        await _service.GetAllAsync();

        var entry = _log.Collector.LatestRecord;
        Assert.Equal(1807, entry.Id.Id);
        Assert.Null(entry.GetStructuredStateValue("RecordId"));
    }

    /// <summary>Creates a record in the state the row needs and picks who acts.</summary>
    private async Task<(Guid Id, int Version)> ArrangeAsync(RecordAction action, RecordOutcome outcome)
    {
        _user.User = TestUsers.Alice;
        var id = (await _service.CreateAsync(ValidRegistration.Create())).Id!.Value;
        var version = 1;

        // Approve/Reject succeed only on a submitted record; Submit's WrongStatus needs one too.
        if (action is Approve or Reject && outcome != WrongStatus || action == Submit && outcome == WrongStatus)
        {
            version = (await _service.SubmitAsync(id, version)).Version;
        }

        // Update's WrongStatus needs an approved record.
        if (action == Update && outcome == WrongStatus)
        {
            version = (await _service.SubmitAsync(id, version)).Version;
            _user.User = TestUsers.Ada;
            version = (await _service.ApproveAsync(id, version)).Version;
        }

        _user.User = outcome switch
        {
            NoUser => null,
            Forbidden => action is Create or Update or Submit or Delete ? TestUsers.Rita : TestUsers.Bob,
            _ => TestUsers.Ada,
        };
        return (id, version);
    }

    private Task ActAsync(RecordAction action, RecordOutcome outcome, Guid id, int version)
    {
        if (outcome == Conflict)
        {
            version--;
        }

        var data = ValidRegistration.Create();
        if (outcome == Invalid)
        {
            data.Email = "nope";
        }

        return action switch
        {
            Create => _service.CreateAsync(data),
            Update => _service.UpdateAsync(id, version, data),
            Submit => _service.SubmitAsync(id, version),
            Approve => _service.ApproveAsync(id, version),
            Reject => _service.RejectAsync(id, version),
            Comment => _service.CommentAsync(id, version, outcome == Invalid ? " " : "Fine."),
            Delete => _service.DeleteAsync(id, version),
            View => _service.GetAsync(id),
            _ => throw new ArgumentOutOfRangeException(nameof(action)),
        };
    }

    [Fact]
    public async Task An_unexpected_failure_is_not_logged_as_saved()
    {
        var broken = new RegistrationService(_clock, _user, new SlowStore(_store, _clock, fail: true), _log);

        await Assert.ThrowsAsync<IOException>(() => broken.CreateAsync(ValidRegistration.Create()));

        Assert.DoesNotContain(_log.Collector.GetSnapshot(), r => r.Id.Id is >= 1100 and < 1900);
    }

    /// <summary>A store whose create takes 250 ms on the fake clock.</summary>
    private sealed class SlowStore(InMemoryRegistrationStore inner, FakeTimeProvider clock, bool fail = false) : IRegistrationStore
    {
        public Task<RegistrationRecordDto?> GetAsync(Guid id, CancellationToken cancellationToken = default) => inner.GetAsync(id, cancellationToken);

        public Task<IReadOnlyList<RegistrationRecordDto>> ListAsync(CancellationToken cancellationToken = default) => inner.ListAsync(cancellationToken);

        public Task<WriteResult> TryCreateAsync(RegistrationRecordDto record, CancellationToken cancellationToken = default)
        {
            clock.Advance(TimeSpan.FromMilliseconds(250));
            return fail ? throw new IOException("disk gone") : inner.TryCreateAsync(record, cancellationToken);
        }

        public Task<WriteResult> TryReplaceAsync(RegistrationRecordDto updated, int expectedVersion, CancellationToken cancellationToken = default) =>
            inner.TryReplaceAsync(updated, expectedVersion, cancellationToken);

        public Task<WriteResult> TryDeleteAsync(Guid id, int expectedVersion, CancellationToken cancellationToken = default) =>
            inner.TryDeleteAsync(id, expectedVersion, cancellationToken);
    }
}
