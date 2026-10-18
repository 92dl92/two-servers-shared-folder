using BlazorApp.Shared.Dtos;

namespace BlazorApp.Stress;

/// <summary>What the lock check found.</summary>
/// <param name="Changes">Record and server pairs tried.</param>
/// <param name="Changed">Pairs where a comment was saved.</param>
/// <param name="Stuck">Pairs where the record could be read but no comment saved in time: it looks locked for good.</param>
/// <param name="Unreadable">Pairs where the record couldn't be read, so nothing was sent; a missing record is the data check's business.</param>
/// <param name="StoppedEarly">Whether the check stopped after <see cref="LockCheck.StopAfterStuck"/> stuck pairs, leaving the rest untried.</param>
/// <param name="Examples">The first stuck or unreadable pairs.</param>
public sealed record LockReport(int Changes, int Changed, int Stuck, int Unreadable, bool StoppedEarly, IReadOnlyList<string> Examples)
{
    /// <summary>Gets a value indicating whether every record that could be read could still be changed.</summary>
    public bool Passed => Stuck == 0;
}

/// <summary>
/// The lock check (phase 6b's failure tests): after a fault, every record must still be
/// changeable through every surviving server. A server killed or cut off while holding a
/// record's lock could leave it locked for ever. The reader comments on each record through
/// each server, with a time limit per request; the comments are acknowledged saves, checked
/// like any other.
/// </summary>
public static class LockCheck
{
    /// <summary>How many times a refused comment (409, 5xx) is tried again.</summary>
    public const int Attempts = 3;

    /// <summary>How many pairs are tried at once.</summary>
    public const int Parallel = 8;

    /// <summary>After this many stuck pairs the check stops: a share that's gone would otherwise cost the timeout per record.</summary>
    public const int StopAfterStuck = 20;

    /// <summary>Runs the check.</summary>
    /// <param name="through">The reader's client for each surviving server.</param>
    /// <param name="records">The records to change.</param>
    /// <param name="timeout">How long each read or comment may take before the record counts as stuck.</param>
    /// <param name="cancellationToken">Stops the check.</param>
    /// <returns>The report, the comments acknowledged, and how many comments got no answer (their outcome is unknown).</returns>
    public static async Task<(LockReport Report, IReadOnlyList<Ack> Acks, int Unknown)> RunAsync(
        IReadOnlyList<ApiClient> through, IReadOnlyList<Guid> records, TimeSpan timeout, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(through);
        ArgumentNullException.ThrowIfNull(records);
        var pairs = through.SelectMany(api => records.Select(id => (Api: api, Id: id))).ToList();
        var results = new List<Attempt>();
        var gate = new Lock();
        var stuck = 0;
        var stoppedEarly = false;
        using var stop = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        try
        {
            await System.Threading.Tasks.Parallel.ForEachAsync(
                pairs, new ParallelOptions { MaxDegreeOfParallelism = Parallel, CancellationToken = stop.Token }, async (pair, token) =>
                {
                    var attempt = await TryCommentAsync(pair.Api, pair.Id, timeout, token);
                    lock (gate)
                    {
                        results.Add(attempt);
                        if (attempt.Kind == AttemptKind.Stuck && ++stuck >= StopAfterStuck && !stop.IsCancellationRequested)
                        {
                            stoppedEarly = true;
                            stop.Cancel();
                        }
                    }
                });
        }
        catch (OperationCanceledException) when (stoppedEarly && !cancellationToken.IsCancellationRequested)
        {
            // Stopped on purpose; the pairs left are untried.
        }

        var acks = results.Where(r => r.Ack is not null).Select(r => r.Ack!).ToList();
        var report = new LockReport(
            results.Count,
            acks.Count,
            results.Count(r => r.Kind == AttemptKind.Stuck),
            results.Count(r => r.Kind == AttemptKind.Unreadable),
            stoppedEarly,
            [.. results.Where(r => r.Kind != AttemptKind.Changed).Take(20).Select(r => r.Detail)]);
        return (report, acks, results.Count(r => r.CommentUnanswered));
    }

    /// <summary>Reads the record and comments on it from the version read, trying again after a refusal.</summary>
    private static async Task<Attempt> TryCommentAsync(ApiClient api, Guid id, TimeSpan timeout, CancellationToken cancellationToken)
    {
        var where = $"record {id} through {api.Server}";
        var last = new Attempt(AttemptKind.Unreadable, null, $"{where}: not tried", false);
        for (var attempt = 1; attempt <= Attempts; attempt++)
        {
            var read = await WithTimeoutAsync(t => api.GetAsync(id, t), timeout, cancellationToken);
            if (read?.Body is not { } record)
            {
                // Can't read it: nothing was sent. A timeout here is the share hanging, so stuck.
                last = read is null
                    ? new Attempt(AttemptKind.Stuck, null, $"{where}: read: no answer within {timeout.TotalSeconds:0.#} s", false)
                    : new Attempt(AttemptKind.Unreadable, null, $"{where}: read {(read.Status is { } status ? (int)status : "no answer")}", false);
                continue;
            }

            var marker = $"lock-check:{api.Server}:{id}";
            var response = await WithTimeoutAsync(t => api.ChangeAsync(RecordAction.Comment, id, record.Version, null, marker, t), timeout, cancellationToken);
            if (response?.Body is { } saved)
            {
                return new Attempt(AttemptKind.Changed, new Ack(saved.Id, saved.Version, RecordAction.Comment, api.UserId, marker, api.Server, response.CorrelationId), "", false);
            }

            if (response?.Status is null)
            {
                // Sent, but no answer (a timeout or a lost connection): it may have been saved.
                return new Attempt(AttemptKind.Stuck, null, $"{where}: comment: no answer within {timeout.TotalSeconds:0.#} s", !(response?.Refused ?? false));
            }

            last = new Attempt(AttemptKind.Stuck, null, $"{where}: comment {(int)response.Status}", false);
        }

        return last;
    }

    /// <summary>Runs one request with its own time limit; null if it ran out.</summary>
    private static async Task<ApiResponse<T>?> WithTimeoutAsync<T>(
        Func<CancellationToken, Task<ApiResponse<T>>> request, TimeSpan timeout, CancellationToken cancellationToken)
    {
        using var limit = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        limit.CancelAfter(timeout);
        try
        {
            return await request(limit.Token);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return null;
        }
    }

    private enum AttemptKind
    {
        Changed,
        Stuck,
        Unreadable,
    }

    /// <summary>How one record and server pair went.</summary>
    private sealed record Attempt(AttemptKind Kind, Ack? Ack, string Detail, bool CommentUnanswered);
}
