using System.Net.Http.Json;
using BlazorApp.Shared.Contracts;
using BlazorApp.Shared.Dtos;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BlazorApp.Server.Tests;

/// <summary>
/// No log line, at any level, contains a value from the form data or a comment (plan
/// section 8, Privacy). The form is filled with recognisable values and every captured line
/// is searched for them.
/// </summary>
public sealed class LogPrivacyTests : IDisposable
{
    private static readonly string[] Secrets =
    [
        "Zebediah", "Quimbleton", "zq.secret@example.org", "+44 7700 900123", "77 Hidden Lane",
        "Mysteryville", "ZZ9 9ZZ", "Atlantis-Land", "notes-not-for-logs", "Ploughman", "comment-not-for-logs",
    ];

    private readonly WebApplicationFactory<Program> _factory = new WebApplicationFactory<Program>().WithFakeLogging();

    /// <inheritdoc />
    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task No_log_line_contains_form_values_or_comment_text()
    {
        var api = new RecordApi(_factory);
        var data = new RegistrationDto
        {
            FirstName = "Zebediah",
            LastName = "Quimbleton",
            Email = "zq.secret@example.org",
            Phone = "+44 7700 900123",
            Street = "77 Hidden Lane",
            City = "Mysteryville",
            PostalCode = "ZZ9 9ZZ",
            Country = "Atlantis-Land",
            Notes = "notes-not-for-logs",
        };

        // Every action, successful and refused, so every code path that logs is exercised.
        using var created = await _factory.ClientAs("alice").PostAsJsonAsync(ApiRoutes.Registrations, data);
        var id = (await created.Content.ReadFromJsonAsync<SubmissionResultDto>())!.Id!.Value;
        data.PostalCode = "ZZ9 9ZZ is far too long";
        using (var refused = await _factory.ClientAs("alice").PostAsJsonAsync(ApiRoutes.Registrations, data))
        {
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, refused.StatusCode);
        }

        data.PostalCode = "ZZ9 9ZZ";
        data.LastName = "Ploughman";
        (await api.SendAsync("alice", RecordAction.Update, id, 1, data)).Dispose();
        (await api.SendAsync("alice", RecordAction.Update, id, 1, data)).Dispose(); // conflict
        (await api.SendAsync("alice", RecordAction.Submit, id, 2)).Dispose();
        (await api.SendAsync("rita", RecordAction.Comment, id, 3, comment: "comment-not-for-logs")).Dispose();
        (await api.SendAsync("rita", RecordAction.Update, id, 4, data)).Dispose(); // forbidden
        (await api.SendAsync("rita", RecordAction.Approve, id, 4)).Dispose();
        (await api.SendAsync("ada", RecordAction.View, id, 5)).Dispose();
        (await _factory.ClientAs("ada").GetAsync(ApiRoutes.Registrations)).Dispose();
        (await api.SendAsync("ada", RecordAction.Delete, id, 5)).Dispose();

        var lines = _factory.Logs();
        // The paths that carry data really were logged: invalid create, update, comment.
        Assert.Contains(lines, r => r.Id.Id == 1104);
        Assert.Contains(lines, r => r.Id.Id == 1200);
        Assert.Contains(lines, r => r.Id.Id == 1600);
        foreach (var line in lines)
        {
            var text = line.AllText();
            foreach (var secret in Secrets)
            {
                Assert.False(
                    text.Contains(secret, StringComparison.OrdinalIgnoreCase),
                    $"'{secret}' appears in log event {line.Id} ({line.Category}): {text}");
            }
        }
    }
}
