using BlazorApp.Services.Storage.InMemory;
using BlazorApp.Shared.Dtos;

namespace BlazorApp.Stress.Tests;

/// <summary>Filling the share before the volume test (phase 6b).</summary>
public sealed class PrefillTests
{
    [Fact]
    public async Task Each_record_is_finished_with_6_to_11_changes_by_the_right_roles()
    {
        var store = new InMemoryRegistrationStore();
        var users = StressRun.MakeUsers(servers: 2, clientsPerServer: 5, records: int.MaxValue);

        await Prefill.CreateAsync(store, users, count: 40, seed: 1, CancellationToken.None);

        var records = await store.ListAsync(CancellationToken.None);
        Assert.Equal(40, records.Count);
        var byId = users.ToDictionary(u => u.Id);
        Assert.All(records, r =>
        {
            Assert.InRange(r.History.Count, 6, 11);
            Assert.Equal(r.History.Count, r.Version);
            Assert.Contains(r.Status, new[] { RecordStatus.Approved, RecordStatus.Rejected });
            Assert.Equal(RecordAction.Create, r.History[0].Action);
            Assert.Contains(Role.Registrant, byId[r.History[0].UserId].Roles);
            Assert.Contains(Role.Reviewer, byId[r.History[^1].UserId].Roles);
        });
        Assert.True(records.Select(r => r.History.Count).Distinct().Count() > 1);
        Assert.Contains(records, r => r.Status == RecordStatus.Rejected);
    }
}
