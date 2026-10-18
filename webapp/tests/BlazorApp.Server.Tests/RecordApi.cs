using System.Net;
using System.Net.Http.Json;
using BlazorApp.Shared.Contracts;
using BlazorApp.Shared.Dtos;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BlazorApp.Server.Tests;

/// <summary>Calls the record API the way the browser does, as a given user.</summary>
/// <param name="factory">The app under test.</param>
public sealed class RecordApi(WebApplicationFactory<Program> factory)
{
    /// <summary>A valid registration.</summary>
    /// <returns>New form data.</returns>
    public static RegistrationDto Valid() => new()
    {
        FirstName = "Ada",
        LastName = "Lovelace",
        Email = "ada@example.com",
        Street = "12 Analytical Row",
        City = "London",
        PostalCode = "N1 9GU",
        Country = "United Kingdom",
    };

    /// <summary>Creates a record as <paramref name="owner"/> and moves it to <paramref name="status"/> (submitting as the owner, deciding as Ada).</summary>
    /// <param name="owner">User id that creates the record.</param>
    /// <param name="status">Status to leave it in.</param>
    /// <returns>The record.</returns>
    public async Task<RegistrationRecordDto> CreateInStatusAsync(string owner, RecordStatus status)
    {
        using var created = await factory.ClientAs(owner).PostAsJsonAsync(ApiRoutes.Registrations, Valid());
        created.EnsureSuccessStatusCode();
        var id = (await created.Content.ReadFromJsonAsync<SubmissionResultDto>())!.Id!.Value;
        var record = await GetAsync("ada", id);

        if (status != RecordStatus.Draft)
        {
            record = await ReadAsync(await SendAsync(owner, RecordAction.Submit, id, record.Version));
        }

        if (status is RecordStatus.Approved or RecordStatus.Rejected)
        {
            var decision = status == RecordStatus.Approved ? RecordAction.Approve : RecordAction.Reject;
            record = await ReadAsync(await SendAsync("ada", decision, id, record.Version));
        }

        Assert.Equal(status, record.Status);
        return record;
    }

    /// <summary>Gets a record as a user.</summary>
    /// <param name="user">User id.</param>
    /// <param name="id">Record id.</param>
    /// <returns>The record.</returns>
    public async Task<RegistrationRecordDto> GetAsync(string user, Guid id) =>
        (await factory.ClientAs(user).GetFromJsonAsync<RegistrationRecordDto>(ApiRoutes.Registration(id)))!;

    /// <summary>Sends one action as a user (null for no cookie).</summary>
    /// <param name="user">User id, or null.</param>
    /// <param name="action">The action; <see cref="RecordAction.Create"/> is not supported here.</param>
    /// <param name="id">Record id.</param>
    /// <param name="version">Expected version to send.</param>
    /// <param name="data">Form data for an update; a valid change when null.</param>
    /// <param name="comment">Comment text for a comment.</param>
    /// <returns>The response.</returns>
    public Task<HttpResponseMessage> SendAsync(
        string? user, RecordAction action, Guid id, int version, RegistrationDto? data = null, string comment = "Noted.")
    {
        var client = factory.ClientAs(user);
        return action switch
        {
            RecordAction.View => client.GetAsync(ApiRoutes.Registration(id)),
            RecordAction.Update => client.PutAsJsonAsync(ApiRoutes.Registration(id), new UpdateRecordDto(version, data ?? Changed())),
            RecordAction.Submit => client.PostAsJsonAsync(ApiRoutes.RegistrationAction(id, "submit"), new ExpectedVersionDto(version)),
            RecordAction.Approve => client.PostAsJsonAsync(ApiRoutes.RegistrationAction(id, "approve"), new ExpectedVersionDto(version)),
            RecordAction.Reject => client.PostAsJsonAsync(ApiRoutes.RegistrationAction(id, "reject"), new ExpectedVersionDto(version)),
            RecordAction.Comment => client.PostAsJsonAsync(ApiRoutes.RegistrationAction(id, "comments"), new CommentRequestDto(version, comment)),
            RecordAction.Delete => client.DeleteAsync($"{ApiRoutes.Registration(id)}?expectedVersion={version}"),
            _ => throw new ArgumentOutOfRangeException(nameof(action)),
        };
    }

    /// <summary>Reads a record from a successful response.</summary>
    /// <param name="response">The response; disposed afterwards.</param>
    /// <returns>The record in the body.</returns>
    public static async Task<RegistrationRecordDto> ReadAsync(HttpResponseMessage response)
    {
        using (response)
        {
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            return (await response.Content.ReadFromJsonAsync<RegistrationRecordDto>())!;
        }
    }

    private static RegistrationDto Changed()
    {
        var data = Valid();
        data.City = "Paris";
        return data;
    }
}
