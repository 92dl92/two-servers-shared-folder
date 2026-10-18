using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Json;
using BlazorApp.Shared.Contracts;
using BlazorApp.Shared.Dtos;

namespace BlazorApp.Stress.Tests;

/// <summary>The workload against a fake API, for cases a real server can't be made to produce.</summary>
public sealed class WorkloadTests
{
    private const int Ops = 5;

    [Fact]
    public async Task A_registrant_whose_create_failed_sits_out()
    {
        // "b" can't create its record and may not view anyone else's, so anything it sent after
        // the failed create would only be refused.
        var api = new FakeApi(failCreateFor: "b");
        using var http = new HttpClient(api) { BaseAddress = new Uri("http://fake/") };
        StressClient[] clients =
        [
            new(new ApiClient(http, "fake", "a"), Role.Registrant),
            new(new ApiClient(http, "fake", "b"), Role.Registrant),
        ];

        var outcome = await Workload.RunAsync(clients, records: 2, Ops, seed: 1, CancellationToken.None);

        Assert.Single(outcome.Created);
        Assert.Equal(1, api.Requests["b"]);
        Assert.Equal(1 + (2 * Ops), api.Requests["a"]);
        Assert.Empty(outcome.Numbers.ReadFailures);
    }

    [Fact]
    public async Task A_server_that_shows_an_old_version_gives_stale_reads()
    {
        // The negative control for the stale-read check: this API saves every change but always
        // shows version 1, as a server reading a stale cache would.
        var api = new StaleApi();
        using var one = new HttpClient(api) { BaseAddress = new Uri("http://one/") };
        using var two = new HttpClient(api) { BaseAddress = new Uri("http://two/") };
        StressClient[] clients =
        [
            new(new ApiClient(one, "one", "a"), Role.Registrant),
            new(new ApiClient(two, "two", "r"), Role.Reviewer),
        ];

        var outcome = await Workload.RunPipelineAsync(clients, Ops, TimeSpan.Zero, seed: 1, CancellationToken.None);

        var stale = outcome.Numbers.StaleReads!;
        Assert.True(stale.Count > 0);
        Assert.All(stale.Samples, s => Assert.True(s.VersionRead < s.VersionAcknowledged));
        Assert.Contains(stale.Samples, s => s.Server == "one" && s.AcknowledgedBy == "one");
    }

    [Fact]
    public void The_longest_stall_is_the_longest_time_without_an_acknowledged_save()
    {
        // Saves (in seconds from the start, in any order, as clients finish): a 35 s gap after 6.5 s,
        // as phase 6b saw when an SMB lease break timed out.
        long[] savedAt = [Ticks(2), Ticks(1), Ticks(6.5), Ticks(41.5), Ticks(42)];

        var stall = StressNumbers.FindLongestStall(savedAt, started: Ticks(0), ended: Ticks(43));

        Assert.Equal(35, stall.Seconds);
        Assert.Equal(6.5, stall.AfterSeconds);
    }

    [Fact]
    public void A_freeze_the_run_never_recovered_from_is_a_stall_to_the_end()
    {
        // Every request after 3 s timed out: no save from then on.
        var stall = StressNumbers.FindLongestStall([Ticks(1), Ticks(3)], started: 0, ended: Ticks(63));

        Assert.Equal(new Stall(60, 3), stall);
    }

    [Fact]
    public void A_run_with_no_saves_is_one_stall() =>
        Assert.Equal(new Stall(5, 0), StressNumbers.FindLongestStall([], started: 0, ended: Ticks(5)));

    [Fact]
    public void A_read_is_stale_only_against_saves_acknowledged_before_it_was_sent()
    {
        var check = new StaleReadCheck();
        var id = Guid.NewGuid();

        check.Acknowledged(id, 2, "one", answeredAt: 100);
        check.Read(id, 1, "two", sentAt: 50);   // sent before the save was acknowledged: fine
        check.Read(id, 2, "two", sentAt: 150);  // up to date, after another server's save
        check.Read(id, 1, "two", sentAt: 200);  // stale
        check.Read(id, 1, "one", sentAt: 300);  // stale, through the server that saved it

        var report = check.Report();
        Assert.Equal(2, report.Count);
        Assert.Equal(2, report.ReadsAfterAnotherServer);
        Assert.Equal(["two", "one"], report.Samples.Select(s => s.Server));
        Assert.All(report.Samples, s => Assert.Equal((1, 2, "one"), (s.VersionRead, s.VersionAcknowledged, s.AcknowledgedBy)));
    }

    [Fact]
    public void Not_finding_a_record_whose_create_was_acknowledged_is_a_stale_read()
    {
        // The SMB client caches "not found" for a few seconds: a record created through one
        // server can be missing on the other.
        var check = new StaleReadCheck();
        var id = Guid.NewGuid();
        check.Acknowledged(id, 1, "one", answeredAt: 100);

        check.NotFound(id, "two", sentAt: 50);   // sent before the create was acknowledged: fine
        check.NotFound(id, "two", sentAt: 200);  // stale
        check.NotFound(Guid.NewGuid(), "two", sentAt: 300); // never acknowledged: not this check's business

        var report = check.Report();
        Assert.Equal(1, report.Count);
        var sample = Assert.Single(report.Samples);
        Assert.Equal((0, 1, "two"), (sample.VersionRead, sample.VersionAcknowledged, sample.Server));
    }

    [Fact]
    public async Task A_server_that_cannot_find_another_servers_records_gives_stale_reads()
    {
        // Two separate in-memory APIs: each knows only the records created through it, so a
        // responder on the other one gets 404 for every handoff.
        var oneApi = new MemoryApi();
        var twoApi = new MemoryApi();
        using var one = new HttpClient(oneApi) { BaseAddress = new Uri("http://one/") };
        using var two = new HttpClient(twoApi) { BaseAddress = new Uri("http://two/") };
        StressClient[] clients =
        [
            new(new ApiClient(one, "one", "a"), Role.Registrant),
            new(new ApiClient(two, "two", "r"), Role.Reviewer),
        ];

        var outcome = await Workload.RunPipelineAsync(clients, Ops, TimeSpan.Zero, seed: 1, CancellationToken.None);

        var stale = outcome.Numbers.StaleReads!;
        Assert.True(stale.Count > 0);
        Assert.All(stale.Samples, s => Assert.Equal((0, "two"), (s.VersionRead, s.Server)));
    }

    private static long Ticks(double seconds) => (long)(seconds * System.Diagnostics.Stopwatch.Frequency);

    [Fact]
    public async Task The_lock_check_changes_every_record_through_every_server()
    {
        var api = new MemoryApi();
        using var one = new HttpClient(api) { BaseAddress = new Uri("http://one/") };
        using var two = new HttpClient(api) { BaseAddress = new Uri("http://two/") };
        var ids = await CreateAsync(one, 3);

        var (report, acks, unknown) = await LockCheck.RunAsync(
            [new ApiClient(one, "one", StressRun.ReaderId), new ApiClient(two, "two", StressRun.ReaderId)], ids, TimeSpan.FromSeconds(5), CancellationToken.None);

        Assert.Equal(new LockReport(6, 6, 0, 0, false, []), report with { Examples = [] });
        Assert.True(report.Passed);
        Assert.Equal(0, unknown);
        Assert.Equal(6, acks.Count);
        Assert.All(acks, a => Assert.Equal((RecordAction.Comment, StressRun.ReaderId), (a.Action, a.UserId)));
        Assert.Equal(6, acks.Select(a => a.Marker).Distinct().Count());
    }

    [Fact]
    public async Task A_record_whose_change_never_finishes_is_stuck()
    {
        // The negative control: changes to one record hang, as behind a lock nobody releases.
        var api = new MemoryApi();
        using var http = new HttpClient(api) { BaseAddress = new Uri("http://one/") };
        var ids = await CreateAsync(http, 2);
        api.HangOn = ids[1];

        var (report, acks, unknown) = await LockCheck.RunAsync(
            [new ApiClient(http, "one", StressRun.ReaderId)], ids, TimeSpan.FromMilliseconds(300), CancellationToken.None);

        Assert.Equal((2, 1, 1), (report.Changes, report.Changed, report.Stuck));
        Assert.False(report.Passed);
        Assert.Contains(ids[1].ToString(), Assert.Single(report.Examples), StringComparison.Ordinal);
        Assert.Single(acks);
        Assert.Equal(1, unknown);
    }

    [Fact]
    public async Task A_record_that_cannot_be_read_is_unreadable_not_stuck()
    {
        // Missing data is the data check's business; nothing was sent, so nothing is locked.
        var api = new MemoryApi();
        using var http = new HttpClient(api) { BaseAddress = new Uri("http://one/") };
        var ids = await CreateAsync(http, 1);

        var (report, _, unknown) = await LockCheck.RunAsync(
            [new ApiClient(http, "one", StressRun.ReaderId)], [ids[0], Guid.NewGuid()], TimeSpan.FromSeconds(5), CancellationToken.None);

        Assert.Equal((2, 1, 0, 1), (report.Changes, report.Changed, report.Stuck, report.Unreadable));
        Assert.True(report.Passed);
        Assert.Equal(0, unknown);
    }

    [Fact]
    public async Task The_lock_check_stops_after_enough_stuck_records()
    {
        // A share that's gone for good would otherwise cost the timeout for every record.
        var api = new MemoryApi();
        using var http = new HttpClient(api) { BaseAddress = new Uri("http://one/") };
        var ids = await CreateAsync(http, 60);
        api.HangOnEverything = true;

        var (report, _, _) = await LockCheck.RunAsync(
            [new ApiClient(http, "one", StressRun.ReaderId)], ids, TimeSpan.FromMilliseconds(200), CancellationToken.None);

        Assert.True(report.StoppedEarly);
        Assert.InRange(report.Stuck, LockCheck.StopAfterStuck, LockCheck.StopAfterStuck + LockCheck.Parallel);
        Assert.True(report.Changes < 60);
        Assert.False(report.Passed);
    }

    [Fact]
    public async Task A_server_that_refuses_connections_is_counted_as_refused_not_unknown()
    {
        // After a kill, nothing reaches the dead server: its refusals prove nothing was saved, so
        // they mustn't add to the unknown outcomes every check allows for.
        var live = new MemoryApi();
        using var one = new HttpClient(live) { BaseAddress = new Uri("http://one/") };
        using var dead = new HttpClient(new RefusingApi()) { BaseAddress = new Uri("http://two/") };
        StressClient[] clients =
        [
            new(new ApiClient(one, "one", "a"), Role.Registrant),
            new(new ApiClient(dead, "two", "b"), Role.Registrant),
        ];

        var outcome = await Workload.RunPipelineAsync(clients, Ops, TimeSpan.Zero, seed: 1, CancellationToken.None);

        Assert.Equal(0, outcome.Numbers.NoResponse);
        Assert.True(outcome.Numbers.WritesByOutcome[StressNumbers.RefusedName] > 0);
    }

    [Fact]
    public async Task With_the_list_responders_open_it_before_each_pick_and_it_is_timed()
    {
        var api = new MemoryApi();
        using var http = new HttpClient(api) { BaseAddress = new Uri("http://one/") };
        StressClient[] clients = [new(new ApiClient(http, "one", "a"), Role.Registrant), new(new ApiClient(http, "one", "r"), Role.Reviewer)];

        var outcome = await Workload.RunPipelineAsync(clients, Ops, TimeSpan.Zero, seed: 1, CancellationToken.None, list: true);

        Assert.True(outcome.Numbers.Lists > 0);
        Assert.NotNull(outcome.Numbers.ListLatencyMs);
        Assert.True(outcome.Numbers.ReadLatencyMs!.Max > 0);

        var without = await Workload.RunPipelineAsync(clients, Ops, TimeSpan.Zero, seed: 1, CancellationToken.None);
        Assert.Equal(0, without.Numbers.Lists);
        Assert.Null(without.Numbers.ListLatencyMs);
    }

    /// <summary>Refuses every connection, as a killed server's port does.</summary>
    private sealed class RefusingApi : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            throw new HttpRequestException(HttpRequestError.ConnectionError, "Connection refused");
    }

    private static async Task<List<Guid>> CreateAsync(HttpClient http, int count)
    {
        var api = new ApiClient(http, "setup", "a");
        var ids = new List<Guid>();
        for (var i = 0; i < count; i++)
        {
            ids.Add((await api.CreateAsync(new RegistrationDto(), CancellationToken.None)).Body!.Id!.Value);
        }

        return ids;
    }

    /// <summary>Keeps records in memory, with versions and statuses, as one server would.</summary>
    private sealed class MemoryApi : HttpMessageHandler
    {
        private readonly ConcurrentDictionary<Guid, (int Version, RecordStatus Status)> records = new();

        /// <summary>A record whose changes never get an answer.</summary>
        public Guid? HangOn { get; set; }

        /// <summary>Whether no change gets an answer.</summary>
        public bool HangOnEverything { get; set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Method != HttpMethod.Get && request.RequestUri!.AbsolutePath != "/" + ApiRoutes.Registrations
                && (HangOnEverything || (HangOn is { } hang && request.RequestUri.AbsolutePath.Contains(hang.ToString(), StringComparison.Ordinal))))
            {
                await Task.Delay(Timeout.Infinite, cancellationToken);
            }

            return await Respond(request);
        }

        private Task<HttpResponseMessage> Respond(HttpRequestMessage request)
        {
            var user = request.Headers.GetValues("Cookie").Single().Split('=')[1];
            var path = request.RequestUri!.AbsolutePath.TrimStart('/');
            if (request.Method == HttpMethod.Post && path == ApiRoutes.Registrations)
            {
                var created = Guid.NewGuid();
                records[created] = (1, RecordStatus.Draft);
                return Answer(HttpStatusCode.Created, SubmissionResultDto.Ok(created));
            }

            if (request.Method == HttpMethod.Get && path == ApiRoutes.Registrations)
            {
                return Answer(HttpStatusCode.OK, new List<RegistrationSummaryDto>());
            }

            var id = path.Split('/').Select(p => Guid.TryParse(p, out var g) ? g : (Guid?)null).OfType<Guid>().First();
            if (!records.TryGetValue(id, out var current))
            {
                return Answer(HttpStatusCode.NotFound);
            }

            if (request.Method == HttpMethod.Get)
            {
                return Answer(HttpStatusCode.OK, Record(id, current.Version, current.Status, user));
            }

            var status = path.EndsWith("/submit", StringComparison.Ordinal) ? RecordStatus.Submitted : current.Status;
            records[id] = (current.Version + 1, status);
            return Answer(HttpStatusCode.OK, Record(id, current.Version + 1, status, user));
        }
    }

    /// <summary>Saves everything, but every read shows version 1 as a draft: a stale cache.</summary>
    private sealed class StaleApi : HttpMessageHandler
    {
        private readonly ConcurrentDictionary<Guid, int> versions = new();

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var user = request.Headers.GetValues("Cookie").Single().Split('=')[1];
            var path = request.RequestUri!.AbsolutePath.TrimStart('/');
            if (request.Method == HttpMethod.Post && path == ApiRoutes.Registrations)
            {
                var id = Guid.NewGuid();
                versions[id] = 1;
                return Answer(HttpStatusCode.Created, SubmissionResultDto.Ok(id));
            }

            var recordId = path.Split('/').Select(p => Guid.TryParse(p, out var g) ? g : (Guid?)null).OfType<Guid>().First();
            if (request.Method == HttpMethod.Get)
            {
                return Answer(HttpStatusCode.OK, Record(recordId, 1, user));
            }

            return Answer(HttpStatusCode.OK, Record(recordId, versions.AddOrUpdate(recordId, 2, (_, v) => v + 1), user));
        }

        private static RegistrationRecordDto Record(Guid id, int version, string user) =>
            WorkloadTests.Record(id, version, RecordStatus.Draft, user);
    }

    private static RegistrationRecordDto Record(Guid id, int version, RecordStatus status, string user) =>
        new(id, version, status, user, DateTimeOffset.UnixEpoch, user, DateTimeOffset.UnixEpoch, new RegistrationDto(), []);

    private static Task<HttpResponseMessage> Answer(HttpStatusCode status, object? body = null) =>
        Task.FromResult(new HttpResponseMessage(status) { Content = body is null ? null : JsonContent.Create(body, body.GetType()) });

    /// <summary>Creates for everyone but one user, shows a record only to its owner, and refuses every change as a conflict.</summary>
    private sealed class FakeApi(string failCreateFor) : HttpMessageHandler
    {
        private readonly ConcurrentDictionary<Guid, string> owners = new();

        public ConcurrentDictionary<string, int> Requests { get; } = new();

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var user = request.Headers.GetValues("Cookie").Single().Split('=')[1];
            Requests.AddOrUpdate(user, 1, (_, n) => n + 1);
            var path = request.RequestUri!.AbsolutePath.TrimStart('/');

            if (request.Method == HttpMethod.Post && path == ApiRoutes.Registrations)
            {
                if (user == failCreateFor)
                {
                    return Answer(HttpStatusCode.InternalServerError);
                }

                var id = Guid.NewGuid();
                owners[id] = user;
                return Answer(HttpStatusCode.Created, SubmissionResultDto.Ok(id));
            }

            if (request.Method == HttpMethod.Get && Guid.TryParse(path.Split('/').Last(), out var recordId))
            {
                return owners.GetValueOrDefault(recordId) == user
                    ? Answer(HttpStatusCode.OK, Record(recordId, user))
                    : Answer(HttpStatusCode.Forbidden);
            }

            return Answer(HttpStatusCode.Conflict);
        }

        private static RegistrationRecordDto Record(Guid id, string owner) =>
            new(id, 1, RecordStatus.Draft, owner, DateTimeOffset.UnixEpoch, owner, DateTimeOffset.UnixEpoch, new RegistrationDto(), []);

        private static Task<HttpResponseMessage> Answer(HttpStatusCode status, object? body = null) =>
            Task.FromResult(new HttpResponseMessage(status) { Content = body is null ? null : JsonContent.Create(body, body.GetType()) });
    }
}
