using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using BlazorApp.Shared.Dtos;
using BlazorApp.Shared.Rules;

namespace BlazorApp.Stress;

/// <summary>One client: a user acting through one server.</summary>
/// <param name="Api">Calls its server as its user.</param>
/// <param name="Role">The user's role, which decides what it tries to do.</param>
public sealed record StressClient(ApiClient Api, Role Role);

/// <summary>What the workload sent and what the servers answered.</summary>
/// <param name="Created">Ids of the records created.</param>
/// <param name="Acks">Every change reported as saved, creates included.</param>
/// <param name="Numbers">The counts and timings, with the stale-read check.</param>
public sealed record WorkloadOutcome(IReadOnlyList<Guid> Created, IReadOnlyList<Ack> Acks, StressNumbers Numbers);

/// <summary>
/// The workload: registrants create records (each through their own server), then every
/// client repeatedly reads a record and sends a change from the version it read, so changes
/// from different users and servers collide on the same records.
/// </summary>
/// <remarks>
/// Registrants update and submit their own records, reviewers comment, approve and reject,
/// admins do all of these. Nine changes in ten are ones the status read allows (a 409 then
/// means someone else got in first); one in ten is any change the role may make, as a user
/// pressing a button on a stale page would, which gives the 422 refusals.
/// </remarks>
public static class Workload
{
    /// <summary>How often a change is any the role may make rather than one the status allows.</summary>
    public const double StaleButtonChance = 0.1;

    private static readonly RecordAction[] Weighted =
    [
        RecordAction.Update, RecordAction.Update, RecordAction.Update, RecordAction.Update,
        RecordAction.Comment, RecordAction.Comment, RecordAction.Comment, RecordAction.Comment,
        RecordAction.Submit, RecordAction.Submit,
        RecordAction.Approve,
        RecordAction.Reject,
    ];

    /// <summary>Runs the workload.</summary>
    /// <param name="clients">The clients, all running at once.</param>
    /// <param name="records">How many records the registrants create.</param>
    /// <param name="opsPerClient">Changes each client attempts after that.</param>
    /// <param name="seed">Seed for the clients' choices.</param>
    /// <param name="cancellationToken">Stops the workload.</param>
    /// <returns>What was acknowledged, and the numbers.</returns>
    /// <exception cref="InvalidOperationException">No record could be created.</exception>
    public static async Task<WorkloadOutcome> RunAsync(
        IReadOnlyList<StressClient> clients, int records, int opsPerClient, int seed, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(clients);
        var tallies = clients.Select(_ => new Tally()).ToList();
        var stale = new StaleReadCheck();
        var started = Stopwatch.GetTimestamp();

        // Records go to the registrants in turn, taking one registrant from each server before
        // the next, so the records are spread over the servers. StressRun.MakeUsers keeps exactly
        // the registrants this order reaches with fewer records, so the two must match.
        var registrants = clients.Select((c, i) => (Client: c, Tally: tallies[i]))
            .Where(c => c.Client.Role == Role.Registrant)
            .GroupBy(c => c.Client.Api.Home)
            .SelectMany(g => g.Select((c, place) => (c, place)))
            .OrderBy(x => x.place)
            .Select(x => x.c)
            .ToList();
        if (registrants.Count == 0)
        {
            throw new InvalidOperationException("The workload needs at least one registrant to create records.");
        }

        // Each registrant creates its share in turn (its tally isn't shared), all registrants at once.
        var creates = await Task.WhenAll(registrants.Select(async (registrant, k) =>
        {
            var mine = new List<Pool?>();
            for (var r = k; r < records; r += registrants.Count)
            {
                mine.Add(await CreateAsync(registrant.Client, registrant.Tally, stale, r, cancellationToken));
            }

            return mine;
        }));
        var created = creates.SelectMany(c => c).OfType<Pool>().ToList();
        if (created.Count == 0)
        {
            throw new InvalidOperationException(
                $"No record could be created. Answers: {string.Join(", ", tallies.SelectMany(t => t.Outcomes).Select(o => $"{o.Key} {o.Value}"))}.");
        }

        await Task.WhenAll(clients.Select((client, i) =>
            RunClientAsync(client, tallies[i], stale, created, opsPerClient, new Random(seed + i), cancellationToken)));

        return new WorkloadOutcome(
            [.. created.Select(p => p.Id)],
            [.. tallies.SelectMany(t => t.Acks)],
            StressNumbers.From(tallies, started, stale.Report()));
    }

    /// <summary>
    /// The pipeline workload (plan phase 6b), close to how the app would be used: registrants
    /// create a record, update it a few times and submit it; reviewers and admins take
    /// submitted records, sometimes comment, then approve or reject. Every step reads the record
    /// first, as opening its page would, after a think time. True collisions are rare; the
    /// risky moment is the handoff, when a responder on one server reads a record just
    /// submitted through another.
    /// </summary>
    /// <param name="clients">The clients, all running at once; there must be registrants.</param>
    /// <param name="opsPerClient">Changes each registrant sends, creates included (it finishes the record it's on); the most records each responder picks.</param>
    /// <param name="think">The mean pause before each step; each pause is 0.5 to 1.5 times it.</param>
    /// <param name="seed">Seed for the clients' choices.</param>
    /// <param name="cancellationToken">Stops the workload.</param>
    /// <param name="list">Whether responders open the record list before each pick, as the Records page would.</param>
    /// <returns>What was acknowledged, and the numbers.</returns>
    /// <exception cref="InvalidOperationException">There are no registrants, or no record could be created.</exception>
    public static async Task<WorkloadOutcome> RunPipelineAsync(
        IReadOnlyList<StressClient> clients, int opsPerClient, TimeSpan think, int seed, CancellationToken cancellationToken, bool list = false)
    {
        ArgumentNullException.ThrowIfNull(clients);
        if (!clients.Any(c => c.Role == Role.Registrant))
        {
            throw new InvalidOperationException("The pipeline needs at least one registrant to create records.");
        }

        var tallies = clients.Select(_ => new Tally()).ToList();
        var stale = new StaleReadCheck();
        var queue = new SubmittedQueue();
        var created = new ConcurrentBag<Guid>();
        var registrantsLeft = clients.Count(c => c.Role == Role.Registrant);
        var started = Stopwatch.GetTimestamp();

        await Task.WhenAll(clients.Select(async (client, i) =>
        {
            var random = new Random(seed + i);
            Task Think() => Task.Delay(think * (0.5 + random.NextDouble()), cancellationToken);
            if (client.Role == Role.Registrant)
            {
                try
                {
                    await CreatorAsync(client, tallies[i], stale, queue, created, opsPerClient, random, Think, cancellationToken);
                }
                finally
                {
                    Interlocked.Decrement(ref registrantsLeft);
                }
            }
            else
            {
                await ResponderAsync(client, tallies[i], stale, queue, () => Volatile.Read(ref registrantsLeft) == 0, opsPerClient, random, Think, list, cancellationToken);
            }
        }));

        if (created.IsEmpty)
        {
            throw new InvalidOperationException(
                $"No record could be created. Answers: {string.Join(", ", tallies.SelectMany(t => t.Outcomes).Select(o => $"{o.Key} {o.Value}"))}.");
        }

        return new WorkloadOutcome([.. created], [.. tallies.SelectMany(t => t.Acks)], StressNumbers.From(tallies, started, stale.Report()));
    }

    private static async Task CreatorAsync(
        StressClient client, Tally tally, StaleReadCheck stale, SubmittedQueue queue, ConcurrentBag<Guid> created,
        int ops, Random random, Func<Task> think, CancellationToken cancellationToken)
    {
        var sent = 0;
        for (var n = 0; sent < ops; n++)
        {
            await think();
            sent++;
            var pool = await CreateAsync(client, tally, stale, n, cancellationToken);
            if (pool is null)
            {
                continue;
            }

            created.Add(pool.Id);

            // Draft edits, then submit; each from the version just read.
            var steps = Enumerable.Repeat(RecordAction.Update, random.Next(3)).Append(RecordAction.Submit);
            foreach (var action in steps)
            {
                await think();
                sent++;
                var (saved, _) = await ReadAndChangeAsync(client, tally, stale, pool.Id, action, $"{client.Api.UserId}:{sent}", cancellationToken);
                if (saved && action == RecordAction.Submit)
                {
                    queue.Add(pool.Id);
                }
            }
        }
    }

    private static async Task ResponderAsync(
        StressClient client, Tally tally, StaleReadCheck stale, SubmittedQueue queue, Func<bool> creatorsDone,
        int ops, Random random, Func<Task> think, bool list, CancellationToken cancellationToken)
    {
        var sent = 0;
        while (sent < ops)
        {
            await think();

            // The list is opened before a pick, as on the Records page, not while nothing waits.
            if (list && !queue.IsEmpty)
            {
                tally.Listed(await client.Api.ListAsync(cancellationToken));
            }

            if (queue.Pick(random) is not { } id)
            {
                if (creatorsDone() && queue.IsEmpty)
                {
                    return;
                }

                // Nothing waiting yet; don't spin when the think time is zero.
                await Task.Delay(TimeSpan.FromMilliseconds(50), cancellationToken);
                continue;
            }

            // A comment one time in three, then a decision; another responder may get there first.
            var action = random.Next(3) == 0 ? RecordAction.Comment : random.Next(4) == 0 ? RecordAction.Reject : RecordAction.Approve;
            sent++;
            var (saved, status) = await ReadAndChangeAsync(
                client, tally, stale, id, action, $"{client.Api.UserId}:{sent}", cancellationToken, onlyIf: RecordStatus.Submitted);
            // Done once decided, by this responder or another; a stale read showing an earlier
            // status leaves it waiting.
            if ((saved && action != RecordAction.Comment) || status is RecordStatus.Approved or RecordStatus.Rejected)
            {
                queue.Remove(id);
            }
        }
    }

    /// <summary>
    /// Reads a record and, if the status allows it (and is <paramref name="onlyIf"/>, when given),
    /// sends one change from the version read.
    /// </summary>
    /// <returns>Whether the change was saved, and the status read (null when the read failed).</returns>
    private static async Task<(bool Saved, RecordStatus? Status)> ReadAndChangeAsync(
        StressClient client, Tally tally, StaleReadCheck stale, Guid id, RecordAction action, string marker, CancellationToken cancellationToken,
        RecordStatus? onlyIf = null)
    {
        var sentAt = Stopwatch.GetTimestamp();
        var read = await client.Api.GetAsync(id, cancellationToken);
        tally.Read(read);
        if (read.Body is not { } record)
        {
            if (read.Status == HttpStatusCode.NotFound)
            {
                stale.NotFound(id, client.Api.Server, sentAt);
            }

            return (false, null);
        }

        stale.Read(id, record.Version, client.Api.Server, sentAt);
        if (!RecordRules.StatusAllows(action, record.Status) || (onlyIf is { } status && record.Status != status))
        {
            // Someone else decided it already (or this server shows an old status): nothing to send.
            return (false, record.Status);
        }

        var response = await client.Api.ChangeAsync(
            action, id, record.Version,
            action == RecordAction.Update ? CopyWithNotes(record.Data, marker) : null,
            action == RecordAction.Comment ? marker : null,
            cancellationToken);
        if (tally.Count(response) && response.Body is { } saved)
        {
            var acked = action is RecordAction.Comment or RecordAction.Update ? marker : null;
            var answeredAt = Stopwatch.GetTimestamp();
            tally.Saved(new Ack(saved.Id, saved.Version, action, client.Api.UserId, acked, client.Api.Server, response.CorrelationId), response.Elapsed, answeredAt);
            stale.Acknowledged(saved.Id, saved.Version, client.Api.Server, answeredAt);
            return (true, saved.Status);
        }

        return (false, record.Status);
    }

    private static async Task<Pool?> CreateAsync(StressClient client, Tally tally, StaleReadCheck stale, int index, CancellationToken cancellationToken)
    {
        var response = await client.Api.CreateAsync(NewRegistration(index), cancellationToken);
        if (tally.Count(response) && response.Body?.Id is { } id)
        {
            var answeredAt = Stopwatch.GetTimestamp();
            tally.Saved(new Ack(id, 1, RecordAction.Create, client.Api.UserId, null, client.Api.Server, response.CorrelationId), response.Elapsed, answeredAt);
            stale.Acknowledged(id, 1, client.Api.Server, answeredAt);
            return new Pool(id, client.Api.UserId);
        }

        return null;
    }

    private static async Task RunClientAsync(
        StressClient client, Tally tally, StaleReadCheck stale, IReadOnlyList<Pool> pool, int ops, Random random, CancellationToken cancellationToken)
    {
        // A registrant may view and change only its own records; one whose create failed owns
        // none and sits out (its create shows among the outcomes) rather than being refused.
        var own = pool.Where(p => p.Owner == client.Api.UserId).ToList();
        if (client.Role == Role.Registrant && own.Count == 0)
        {
            return;
        }

        var choices = client.Role == Role.Registrant ? own : pool;
        var allowed = Weighted.Where(a => Permissions.For(client.Role, a) != PermissionScope.None).ToArray();

        for (var i = 0; i < ops; i++)
        {
            var target = choices[random.Next(choices.Count)];
            var sentAt = Stopwatch.GetTimestamp();
            var read = await client.Api.GetAsync(target.Id, cancellationToken);
            tally.Read(read);
            if (read.Body is not { } record)
            {
                if (read.Status == HttpStatusCode.NotFound)
                {
                    stale.NotFound(target.Id, client.Api.Server, sentAt);
                }

                continue;
            }

            stale.Read(record.Id, record.Version, client.Api.Server, sentAt);

            var fitting = allowed.Where(a => RecordRules.StatusAllows(a, record.Status)).ToArray();
            var options = fitting.Length > 0 && random.NextDouble() >= StaleButtonChance ? fitting : allowed;
            var action = options[random.Next(options.Length)];

            // Unique per client and attempt, so an acknowledged comment can be found again.
            var marker = $"{client.Api.UserId}:{i}";
            var data = action == RecordAction.Update ? CopyWithNotes(record.Data, marker) : null;
            var comment = action == RecordAction.Comment ? marker : null;

            var response = await client.Api.ChangeAsync(action, record.Id, record.Version, data, comment, cancellationToken);
            if (tally.Count(response) && response.Body is { } saved)
            {
                var acked = action is RecordAction.Comment or RecordAction.Update ? marker : null;
                var answeredAt = Stopwatch.GetTimestamp();
                tally.Saved(new Ack(saved.Id, saved.Version, action, client.Api.UserId, acked, client.Api.Server, response.CorrelationId), response.Elapsed, answeredAt);
                stale.Acknowledged(saved.Id, saved.Version, client.Api.Server, answeredAt);
            }
        }
    }

    private static RegistrationDto NewRegistration(int index) => new()
    {
        FirstName = "Stress",
        LastName = $"Record {index + 1}",
        Email = $"stress{index + 1}@example.com",
        Street = "1 Load Street",
        City = "Testville",
        PostalCode = "12345",
        Country = "Testland",
    };

    private static RegistrationDto CopyWithNotes(RegistrationDto data, string notes) => new()
    {
        FirstName = data.FirstName,
        LastName = data.LastName,
        Email = data.Email,
        Phone = data.Phone,
        Street = data.Street,
        City = data.City,
        PostalCode = data.PostalCode,
        Country = data.Country,
        PreferredContact = data.PreferredContact,
        SubscribeToNewsletter = data.SubscribeToNewsletter,
        Notes = notes,
    };

    /// <summary>A created record and who owns it.</summary>
    private sealed record Pool(Guid Id, string Owner);

    /// <summary>The submitted records waiting for a decision, shared by the responders.</summary>
    private sealed class SubmittedQueue
    {
        private readonly List<Guid> _ids = [];
        private readonly Lock _lock = new();

        /// <summary>Gets a value indicating whether nothing is waiting.</summary>
        public bool IsEmpty
        {
            get
            {
                lock (_lock)
                {
                    return _ids.Count == 0;
                }
            }
        }

        /// <summary>Adds a submitted record.</summary>
        public void Add(Guid id)
        {
            lock (_lock)
            {
                _ids.Add(id);
            }
        }

        /// <summary>Any waiting record, left in the queue: two responders may pick the same one.</summary>
        public Guid? Pick(Random random)
        {
            lock (_lock)
            {
                return _ids.Count == 0 ? null : _ids[random.Next(_ids.Count)];
            }
        }

        /// <summary>Removes a decided record.</summary>
        public void Remove(Guid id)
        {
            lock (_lock)
            {
                _ids.Remove(id);
            }
        }
    }
}

/// <summary>One client's counts; each client has its own, so no locking is needed.</summary>
internal sealed class Tally
{
    /// <summary>Writes by outcome: <c>saved</c>, <c>conflict</c>, <c>wrongStatus</c>, <c>notFound</c>, …</summary>
    public Dictionary<string, int> Outcomes { get; } = [];

    public Dictionary<RecordAction, int> SavesByAction { get; } = [];

    public List<Ack> Acks { get; } = [];

    public List<double> SaveMilliseconds { get; } = [];

    /// <summary>How long successful reads took.</summary>
    public List<double> ReadMilliseconds { get; } = [];

    /// <summary>How long successful lists took.</summary>
    public List<double> ListMilliseconds { get; } = [];

    /// <summary>Lists sent.</summary>
    public int Lists { get; private set; }

    /// <summary>Lists that didn't return the records, by outcome.</summary>
    public Dictionary<string, int> ListFailures { get; } = [];

    /// <summary>Counts a list of the records.</summary>
    public void Listed<T>(ApiResponse<T> response)
    {
        Lists++;
        if (response.Status == HttpStatusCode.OK)
        {
            ListMilliseconds.Add(response.Elapsed.TotalMilliseconds);
        }
        else
        {
            var outcome = response.Refused ? StressNumbers.RefusedName : OutcomeName(response.Status);
            ListFailures[outcome] = ListFailures.GetValueOrDefault(outcome) + 1;
        }
    }

    /// <summary>When each save's answer arrived (<see cref="Stopwatch.GetTimestamp"/>), for the longest stall.</summary>
    public List<long> SavedAt { get; } = [];

    public int Reads { get; private set; }

    public Dictionary<string, int> ReadFailures { get; } = [];

    /// <summary>Counts a write's answer.</summary>
    /// <returns>True if it was saved.</returns>
    public bool Count<T>(ApiResponse<T> response)
    {
        var outcome = response.Refused ? StressNumbers.RefusedName : OutcomeName(response.Status);
        Outcomes[outcome] = Outcomes.GetValueOrDefault(outcome) + 1;
        return response.Succeeded;
    }

    public void Saved(Ack ack, TimeSpan elapsed, long answeredAt)
    {
        Acks.Add(ack);
        SaveMilliseconds.Add(elapsed.TotalMilliseconds);
        SavedAt.Add(answeredAt);
        SavesByAction[ack.Action] = SavesByAction.GetValueOrDefault(ack.Action) + 1;
    }

    public void Read<T>(ApiResponse<T> response)
    {
        Reads++;
        if (response.Status == HttpStatusCode.OK)
        {
            ReadMilliseconds.Add(response.Elapsed.TotalMilliseconds);
        }

        if (response.Status != HttpStatusCode.OK)
        {
            var outcome = response.Refused ? StressNumbers.RefusedName : OutcomeName(response.Status);
            ReadFailures[outcome] = ReadFailures.GetValueOrDefault(outcome) + 1;
        }
    }

    /// <summary>The name an HTTP status is counted under.</summary>
    public static string OutcomeName(HttpStatusCode? status) => status switch
    {
        null => StressNumbers.NoResponseName,
        HttpStatusCode.OK or HttpStatusCode.Created => StressNumbers.Saved,
        HttpStatusCode.Conflict => StressNumbers.Conflict,
        HttpStatusCode.UnprocessableEntity => StressNumbers.WrongStatusName,
        HttpStatusCode.NotFound => "notFound",
        HttpStatusCode.Forbidden => "forbidden",
        HttpStatusCode.Unauthorized => "noUser",
        HttpStatusCode.BadRequest => "invalid",
        { } other => $"http{(int)other}",
    };
}
