using System.Net;
using System.Net.Http.Json;
using BlazorApp.Services.Storage;
using BlazorApp.Shared.Contracts;
using BlazorApp.Shared.Dtos;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorApp.Server.Tests;

/// <summary>Logging when things break: unexpected errors, a store that can't open, bad client batches.</summary>
public sealed class LoggingFailureTests
{
    [Fact]
    public async Task An_unhandled_error_is_logged_as_500_not_200()
    {
        using var factory = new WebApplicationFactory<Program>().WithFakeLogging()
            .WithWebHostBuilder(b => b.ConfigureTestServices(s => s.AddSingleton<IRegistrationStore>(new BrokenStore())));

        using var response = await factory.ClientAs("ada").GetAsync(ApiRoutes.Registrations);

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal("500", Assert.Single(factory.Logs(), r => r.Id.Id == 3001).GetStructuredStateValue("StatusCode"));
    }

    [Fact]
    public void A_store_that_cannot_open_stops_the_server_at_startup()
    {
        using var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(b => b.ConfigureTestServices(s => s.AddSingleton<IRegistrationStore>(_ => throw new IOException("cannot open the data folder"))));

        var e = Assert.ThrowsAny<Exception>(() => factory.Services);

        Assert.Contains("cannot open the data folder", e.ToString(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task A_client_batch_with_a_null_entry_is_a_400()
    {
        using var factory = new WebApplicationFactory<Program>().WithFakeLogging();

        using var response = await factory.CreateClient().PostAsync(
            ApiRoutes.ClientLogs, new StringContent("[null]", System.Text.Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Control_characters_from_the_client_cannot_forge_log_lines()
    {
        using var factory = new WebApplicationFactory<Program>().WithFakeLogging();
        ClientLogEntryDto[] batch = [new(DateTimeOffset.UtcNow, "PageLoaded\r\nfail: forged", "records\nfail: BlazorApp[1205] forged", "a\tb", "c\u001bd")];

        using var response = await factory.CreateClient().PostAsJsonAsync(ApiRoutes.ClientLogs, batch);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var line = Assert.Single(factory.Logs(), r => r.Id.Id == 4001);
        Assert.DoesNotContain(line.Message, char.IsControl);
        Assert.Equal("records fail: BlazorApp[1205] forged", line.GetStructuredStateValue("Page"));
    }

    /// <summary>A store that fails every call, like a broken data folder.</summary>
    private sealed class BrokenStore : IRegistrationStore
    {
        public Task<RegistrationRecordDto?> GetAsync(Guid id, CancellationToken cancellationToken = default) => throw new IOException("disk gone");

        public Task<IReadOnlyList<RegistrationRecordDto>> ListAsync(CancellationToken cancellationToken = default) => throw new IOException("disk gone");

        public Task<WriteResult> TryCreateAsync(RegistrationRecordDto record, CancellationToken cancellationToken = default) => throw new IOException("disk gone");

        public Task<WriteResult> TryReplaceAsync(RegistrationRecordDto updated, int expectedVersion, CancellationToken cancellationToken = default) =>
            throw new IOException("disk gone");

        public Task<WriteResult> TryDeleteAsync(Guid id, int expectedVersion, CancellationToken cancellationToken = default) => throw new IOException("disk gone");
    }
}
