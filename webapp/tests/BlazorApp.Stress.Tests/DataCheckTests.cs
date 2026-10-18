using BlazorApp.Shared.Dtos;

namespace BlazorApp.Stress.Tests;

/// <summary>
/// The checks the stress tool runs after a workload, on hand-made records: each problem the
/// plan names (lost update, broken history, a status change the rules forbid) must be found,
/// and clean data must pass.
/// </summary>
public sealed class DataCheckTests
{
    private static readonly DateTimeOffset At = new(2026, 10, 1, 12, 0, 0, TimeSpan.Zero);

    private static readonly Dictionary<string, UserDto> Users = new[]
    {
        new UserDto("reg", "Registrant", [Role.Registrant]),
        new UserDto("rev", "Reviewer", [Role.Reviewer]),
        new UserDto("adm", "Admin", [Role.Admin]),
    }.ToDictionary(u => u.Id);

    private static readonly Guid Id = Guid.Parse("11111111-1111-1111-1111-111111111111");

    private static readonly Guid Old = Guid.Parse("22222222-2222-2222-2222-222222222222");

    [Fact]
    public void A_prefilled_record_left_as_it_was_passes_and_is_expected()
    {
        var (record, acks) = Build((RecordAction.Create, "reg", null));
        var (old, _) = Build((RecordAction.Create, "reg", null), (RecordAction.Submit, "reg", null));

        var report = DataCheck.Check("test", acks, [Id], [record, old with { Id = Old }], Users, prefilled: new Dictionary<Guid, int> { [Old] = 2 });

        Assert.True(report.Passed(0), string.Join("\n", report.Examples));
        Assert.Equal((2, 2, 0), (report.RecordsExpected, report.RecordsFound, report.RecordsUnexpected));
    }

    [Fact]
    public void A_missing_prefilled_record_is_reported()
    {
        var (record, acks) = Build((RecordAction.Create, "reg", null));

        var report = DataCheck.Check("test", acks, [Id], [record], Users, prefilled: new Dictionary<Guid, int> { [Old] = 2 });

        Assert.False(report.Passed(0));
        Assert.Equal(1, report.MissingRecords);
        Assert.Contains(report.Examples, e => e.Contains(Old.ToString(), StringComparison.Ordinal) && e.Contains("prefilled", StringComparison.Ordinal));
    }

    [Fact]
    public void A_prefilled_record_at_another_version_is_a_lost_update()
    {
        // Nothing in the workload changes prefilled records: a different version means one was rolled back or overwritten.
        var (record, acks) = Build((RecordAction.Create, "reg", null));
        var (old, _) = Build((RecordAction.Create, "reg", null));

        var report = DataCheck.Check("test", acks, [Id], [record, old with { Id = Old }], Users, prefilled: new Dictionary<Guid, int> { [Old] = 2 });

        Assert.False(report.Passed(0));
        Assert.Equal(1, report.LostUpdates);
    }

    [Fact]
    public void Records_only_listed_count_as_found_at_their_listed_version()
    {
        // A server's read-back lists every record but reads in full only those the run changed.
        var (record, acks) = Build((RecordAction.Create, "reg", null));

        var report = DataCheck.Check(
            "test", acks, [Id], [record], Users, prefilled: new Dictionary<Guid, int> { [Old] = 2 }, listedOnly: new Dictionary<Guid, int> { [Old] = 2 });

        Assert.True(report.Passed(0), string.Join("\n", report.Examples));
        Assert.Equal((2, 2), (report.RecordsExpected, report.RecordsFound));
    }

    [Fact]
    public void Clean_data_passes()
    {
        var (record, acks) = Build(
            (RecordAction.Create, "reg", null),
            (RecordAction.Update, "reg", null),
            (RecordAction.Comment, "rev", "rev:1"),
            (RecordAction.Submit, "reg", null),
            (RecordAction.Approve, "rev", null));

        var report = DataCheck.Check("test", acks, [Id], [record], Users);

        Assert.True(report.Passed(unknownOutcomes: 0), string.Join("\n", report.Examples));
        Assert.Equal(1, report.RecordsFound);
        Assert.Equal(0, report.LostUpdates + report.MissingRecords + report.RuleViolations + report.Unacknowledged);
    }

    [Fact]
    public void An_overwritten_save_is_a_lost_update()
    {
        // Two clients both saved version 2; the data holds only the reviewer's comment.
        var (record, acks) = Build(
            (RecordAction.Create, "reg", null),
            (RecordAction.Comment, "rev", "rev:1"));
        acks.Add(new Ack(Id, 2, RecordAction.Comment, "adm", "adm:1", "2", "c"));

        var report = DataCheck.Check("test", acks, [Id], [record], Users);

        Assert.Equal(1, report.LostUpdates);
        Assert.False(report.Passed(0));
        Assert.Contains(report.Examples, e => e.Contains("adm:1", StringComparison.Ordinal));
    }

    [Fact]
    public void A_save_missing_from_the_end_of_the_history_is_a_lost_update()
    {
        var (record, acks) = Build(
            (RecordAction.Create, "reg", null),
            (RecordAction.Update, "reg", null));
        acks.Add(new Ack(Id, 3, RecordAction.Update, "adm", null, "1", "c"));

        var report = DataCheck.Check("test", acks, [Id], [record], Users);

        Assert.Equal(1, report.LostUpdates);
        Assert.False(report.Passed(0));
    }

    [Fact]
    public void A_comment_found_at_another_version_is_reported()
    {
        var (record, acks) = Build(
            (RecordAction.Create, "reg", null),
            (RecordAction.Comment, "rev", "rev:1"),
            (RecordAction.Comment, "rev", "rev:2"));
        acks[2] = acks[2] with { Version = 2 };
        acks[1] = acks[1] with { Version = 3 };

        var report = DataCheck.Check("test", acks, [Id], [record], Users);

        Assert.Equal(2, report.WrongVersion);
        Assert.Equal(0, report.LostUpdates);
        Assert.False(report.Passed(0));
    }

    [Fact]
    public void A_missing_record_counts_its_saves_as_lost()
    {
        var (_, acks) = Build(
            (RecordAction.Create, "reg", null),
            (RecordAction.Update, "reg", null));

        var report = DataCheck.Check("test", acks, [Id], [], Users);

        Assert.Equal(1, report.MissingRecords);
        Assert.Equal(2, report.LostUpdates);
        Assert.False(report.Passed(0));
    }

    [Fact]
    public void A_gap_in_the_history_breaks_the_rules()
    {
        var (record, acks) = Build(
            (RecordAction.Create, "reg", null),
            (RecordAction.Update, "reg", null),
            (RecordAction.Update, "reg", null));
        record = record with { History = [record.History[0], record.History[2]] };
        acks.RemoveAt(1);

        var report = DataCheck.Check("test", acks, [Id], [record], Users);

        Assert.True(report.RuleViolations > 0);
        Assert.False(report.Passed(0));
    }

    [Fact]
    public void A_version_that_does_not_match_the_history_breaks_the_rules()
    {
        var (record, acks) = Build(
            (RecordAction.Create, "reg", null),
            (RecordAction.Update, "reg", null));
        record = record with { Version = 3 };

        var report = DataCheck.Check("test", acks, [Id], [record], Users);

        Assert.True(report.RuleViolations > 0);
    }

    [Fact]
    public void Approving_a_draft_breaks_the_rules()
    {
        var (record, acks) = Build(
            (RecordAction.Create, "reg", null),
            (RecordAction.Approve, "rev", null));

        var report = DataCheck.Check("test", acks, [Id], [record], Users);

        Assert.Equal(1, report.RuleViolations);
        Assert.Contains(report.Examples, e => e.Contains("Approve", StringComparison.Ordinal) && e.Contains("Draft", StringComparison.Ordinal));
        Assert.False(report.Passed(0));
    }

    [Fact]
    public void A_change_by_a_role_that_may_not_do_it_breaks_the_rules()
    {
        var (record, acks) = Build(
            (RecordAction.Create, "reg", null),
            (RecordAction.Update, "rev", null));

        var report = DataCheck.Check("test", acks, [Id], [record], Users);

        Assert.Equal(1, report.RuleViolations);
    }

    [Fact]
    public void A_final_status_that_does_not_follow_from_the_history_breaks_the_rules()
    {
        var (record, acks) = Build(
            (RecordAction.Create, "reg", null),
            (RecordAction.Submit, "reg", null));
        record = record with { Status = RecordStatus.Approved };

        var report = DataCheck.Check("test", acks, [Id], [record], Users);

        Assert.Equal(1, report.RuleViolations);
    }

    [Fact]
    public void Unacknowledged_entries_pass_only_up_to_the_requests_whose_outcome_is_unknown()
    {
        var (record, acks) = Build(
            (RecordAction.Create, "reg", null),
            (RecordAction.Comment, "rev", "rev:1"),
            (RecordAction.Comment, "rev", "rev:2"));
        acks.RemoveRange(1, 2);

        var report = DataCheck.Check("test", acks, [Id], [record], Users);

        Assert.Equal(2, report.Unacknowledged);
        Assert.False(report.Passed(unknownOutcomes: 1));
        Assert.True(report.Passed(unknownOutcomes: 2));
    }

    [Fact]
    public void Records_the_workload_did_not_create_are_counted_and_checked_for_rules()
    {
        var (record, acks) = Build((RecordAction.Create, "reg", null));
        var other = record with
        {
            Id = Guid.NewGuid(),
            Version = 2,
            Status = RecordStatus.Approved,
            History = [.. record.History, new HistoryEntryDto(2, RecordAction.Approve, "rev", At)],
        };

        var report = DataCheck.Check("test", acks, [Id], [record, other], Users);

        Assert.Equal(1, report.RecordsUnexpected);
        Assert.Equal(1, report.RuleViolations);
        Assert.Equal(0, report.Unacknowledged);
    }

    [Fact]
    public void A_comment_saved_twice_is_unacknowledged()
    {
        // The same write applied twice: versions are still 1..N, but v3 was never acknowledged.
        var (record, acks) = Build(
            (RecordAction.Create, "reg", null),
            (RecordAction.Comment, "rev", "rev:1"),
            (RecordAction.Comment, "rev", "rev:1"));
        acks.RemoveAt(2);

        var report = DataCheck.Check("test", acks, [Id], [record], Users);

        Assert.Equal(1, report.Unacknowledged);
        Assert.False(report.Passed(0));
    }

    [Fact]
    public void The_data_must_be_from_the_last_acknowledged_update()
    {
        // History says the second update happened, but the form data is still the first one's.
        var (record, acks) = Build(
            (RecordAction.Create, "reg", null),
            (RecordAction.Update, "reg", null),
            (RecordAction.Update, "adm", null));
        acks[1] = acks[1] with { Marker = "reg:1" };
        acks[2] = acks[2] with { Marker = "adm:1" };
        record = record with { Data = new RegistrationDto { Notes = "reg:1" } };

        var report = DataCheck.Check("test", acks, [Id], [record], Users);

        Assert.Equal(1, report.LostUpdates);
        Assert.Contains(report.Examples, e => e.Contains("adm:1", StringComparison.Ordinal));
        Assert.False(report.Passed(0));

        record = record with { Data = new RegistrationDto { Notes = "adm:1" } };
        Assert.True(DataCheck.Check("test", acks, [Id], [record], Users).Passed(0));
    }

    [Fact]
    public void A_skipped_source_passes_and_an_unreadable_one_fails()
    {
        Assert.True(new SourceReport { Source = "storage", Skipped = "no folder" }.Passed(0));
        Assert.False(new SourceReport { Source = "storage", Error = "bad json" }.Passed(0));
    }

    /// <summary>A record whose history is the given changes, in order, and an ack for each.</summary>
    private static (RegistrationRecordDto Record, List<Ack> Acks) Build(params (RecordAction Action, string User, string? Comment)[] changes)
    {
        var history = changes.Select((c, i) => new HistoryEntryDto(i + 1, c.Action, c.User, At.AddSeconds(i), c.Comment)).ToList();
        var status = RecordStatus.Draft;
        foreach (var change in changes.Skip(1))
        {
            status = change.Action switch
            {
                RecordAction.Submit => RecordStatus.Submitted,
                RecordAction.Approve => RecordStatus.Approved,
                RecordAction.Reject => RecordStatus.Rejected,
                _ => status,
            };
        }

        var record = new RegistrationRecordDto(
            Id, history.Count, status, changes[0].User, At, changes[^1].User, At.AddSeconds(history.Count - 1), new RegistrationDto(), history);
        var acks = history.Select(h => new Ack(Id, h.Version, h.Action, h.UserId, h.Comment, "1", $"c{h.Version}")).ToList();
        return (record, acks);
    }
}
