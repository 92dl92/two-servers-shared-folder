using BlazorApp.Services;
using BlazorApp.Services.Storage;
using BlazorApp.Services.Users;
using BlazorApp.Shared.Dtos;

namespace BlazorApp.Stress;

/// <summary>
/// Fills a store with finished records before a run (phase 6b's volume test: about 6,000
/// records, three years of production). Each goes through the app's own rules
/// (<see cref="RegistrationService"/>): created by a registrant, updated 3 to 8 times, submitted,
/// then approved or rejected by a reviewer, so 6 to 11 changes, as production records have.
/// The run's checks expect each still to be there at the version it was left at.
/// </summary>
/// <remarks>One record at a time: some strategies (A) aren't safe for several writers even in
/// one process, and the prefill must not be the thing that fails.</remarks>
public static class Prefill
{
    /// <summary>Creates the records.</summary>
    /// <param name="store">The store, on a local folder (the tool copies it to the share).</param>
    /// <param name="users">The run's users; registrants own the records, reviewers decide them.</param>
    /// <param name="count">How many records.</param>
    /// <param name="seed">Seed for the number of updates and the decisions.</param>
    /// <param name="cancellationToken">Stops it.</param>
    /// <returns>Each record's id and the version it was left at.</returns>
    /// <exception cref="InvalidOperationException">The users have no registrant or no reviewer.</exception>
    public static async Task<IReadOnlyDictionary<Guid, int>> CreateAsync(
        IRegistrationStore store, IReadOnlyList<UserDto> users, int count, int seed, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(users);
        var registrants = users.Where(u => u.Roles.Contains(Role.Registrant)).ToList();
        var reviewers = users.Where(u => u.Roles.Contains(Role.Reviewer)).ToList();
        if (registrants.Count == 0 || reviewers.Count == 0)
        {
            throw new InvalidOperationException("Prefilling needs a registrant and a reviewer among the users.");
        }

        var actingAs = new ActingAs();
        var service = new RegistrationService(TimeProvider.System, actingAs, store);
        var made = new Dictionary<Guid, int>();
        for (var i = 0; i < count; i++)
        {
            var random = new Random(seed + i);
            actingAs.User = registrants[i % registrants.Count];
            var id = (await service.CreateAsync(Data(i, null), cancellationToken)).Id
                ?? throw new InvalidOperationException($"Prefill record {i} was refused.");
            var record = await service.GetAsync(id, cancellationToken);
            for (var u = random.Next(3, 9); u > 0; u--)
            {
                record = await service.UpdateAsync(id, record.Version, Data(i, $"prefill update {record.Version}"), cancellationToken);
            }

            record = await service.SubmitAsync(id, record.Version, cancellationToken);
            actingAs.User = reviewers[i % reviewers.Count];
            record = random.Next(4) == 0
                ? await service.RejectAsync(id, record.Version, cancellationToken)
                : await service.ApproveAsync(id, record.Version, cancellationToken);
            made[id] = record.Version;
        }

        return made;
    }

    private static RegistrationDto Data(int index, string? notes) => new()
    {
        FirstName = "Prefill",
        LastName = $"Record {index + 1}",
        Email = $"prefill{index + 1}@example.com",
        Street = "1 Archive Street",
        City = "Testville",
        PostalCode = "12345",
        Country = "Testland",
        Notes = notes,
    };

    /// <summary>The user a worker's service acts as, switched per step.</summary>
    private sealed class ActingAs : ICurrentUser
    {
        public UserDto? User { get; set; }

        public string? RequestedId => User?.Id;
    }
}
