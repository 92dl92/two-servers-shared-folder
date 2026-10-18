using System.Net;
using System.Net.Http.Json;
using BlazorApp.Shared.Dtos;
using Microsoft.AspNetCore.Mvc.Testing;
using static BlazorApp.Shared.Dtos.RecordAction;

namespace BlazorApp.Server.Tests;

/// <summary>The record API: every user × action × status, version conflicts, and errors.</summary>
public sealed class RecordApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly RecordApi _api;

    /// <summary>Shares one app across the table rows; each row creates its own record.</summary>
    /// <param name="factory">The app under test.</param>
    public RecordApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _api = new RecordApi(factory);
    }

    /// <summary>
    /// Written out by hand. Records are created by alice. One letter per status (Draft,
    /// Submitted, Approved, Rejected): Y = success (200, or 204 for delete), F = 403, S = 422.
    /// alice is the registrant who owns the record, bob another registrant, rita a reviewer,
    /// ada an admin.
    /// </summary>
    private static readonly Dictionary<(string User, RecordAction Action), string> Expected = new()
    {
        [("alice", View)] = "YYYY",
        [("alice", Update)] = "YYSS",
        [("alice", Submit)] = "YSSS",
        [("alice", Approve)] = "FFFF",
        [("alice", Reject)] = "FFFF",
        [("alice", Comment)] = "FFFF",
        [("alice", Delete)] = "FFFF",

        [("bob", View)] = "FFFF",
        [("bob", Update)] = "FFFF",
        [("bob", Submit)] = "FFFF",
        [("bob", Approve)] = "FFFF",
        [("bob", Reject)] = "FFFF",
        [("bob", Comment)] = "FFFF",
        [("bob", Delete)] = "FFFF",

        [("rita", View)] = "YYYY",
        [("rita", Update)] = "FFFF",
        [("rita", Submit)] = "FFFF",
        [("rita", Approve)] = "SYSS",
        [("rita", Reject)] = "SYSS",
        [("rita", Comment)] = "YYYY",
        [("rita", Delete)] = "FFFF",

        [("ada", View)] = "YYYY",
        [("ada", Update)] = "YYSS",
        [("ada", Submit)] = "YSSS",
        [("ada", Approve)] = "SYSS",
        [("ada", Reject)] = "SYSS",
        [("ada", Comment)] = "YYYY",
        [("ada", Delete)] = "YYYY",
    };

    private static readonly RecordStatus[] StatusOrder =
        [RecordStatus.Draft, RecordStatus.Submitted, RecordStatus.Approved, RecordStatus.Rejected];

    private static readonly RecordAction[] RecordActions = [View, Update, Submit, Approve, Reject, Comment, Delete];

    /// <summary>Every user × action × status with the expected HTTP status.</summary>
    public static TheoryData<string, RecordAction, RecordStatus, HttpStatusCode> Table()
    {
        var data = new TheoryData<string, RecordAction, RecordStatus, HttpStatusCode>();
        foreach (var ((user, action), letters) in Expected)
        {
            for (var i = 0; i < StatusOrder.Length; i++)
            {
                data.Add(user, action, StatusOrder[i], letters[i] switch
                {
                    'Y' => action == Delete ? HttpStatusCode.NoContent : HttpStatusCode.OK,
                    'F' => HttpStatusCode.Forbidden,
                    'S' => HttpStatusCode.UnprocessableEntity,
                    _ => throw new InvalidOperationException(),
                });
            }
        }

        return data;
    }

    /// <summary>Every action on a record, for the 401/404/409 checks.</summary>
    public static TheoryData<RecordAction> AllRecordActions() => [.. RecordActions];

    /// <summary>Every action that changes a record.</summary>
    public static TheoryData<RecordAction> Changes() => [.. RecordActions.Where(a => a != View)];

    /// <summary>Every change except delete, which removes the record (see <see cref="Delete_removes_the_record"/>).</summary>
    public static TheoryData<RecordAction> ChangesThatKeepTheRecord() => [.. RecordActions.Where(a => a is not View and not Delete)];

    [Fact]
    public void Table_covers_every_user_and_action()
    {
        var expected = from user in new[] { "alice", "bob", "rita", "ada" } from action in RecordActions select (user, action);
        Assert.Equal(expected.Order(), Expected.Keys.Order());
    }

    [Theory]
    [MemberData(nameof(Table))]
    public async Task Each_user_action_and_status_gets_the_expected_response(
        string user, RecordAction action, RecordStatus status, HttpStatusCode expected)
    {
        var record = await _api.CreateInStatusAsync("alice", status);

        using var response = await _api.SendAsync(user, action, record.Id, record.Version);

        Assert.Equal(expected, response.StatusCode);

        // Anything refused leaves the record exactly as it was.
        if (!response.IsSuccessStatusCode)
        {
            var after = await _api.GetAsync("ada", record.Id);
            Assert.Equal((record.Version, record.Status, record.History.Count), (after.Version, after.Status, after.History.Count));
        }
    }

    [Theory]
    [MemberData(nameof(AllRecordActions))]
    public async Task Missing_or_unknown_user_gets_401(RecordAction action)
    {
        var record = await _api.CreateInStatusAsync("alice", RecordStatus.Submitted);

        using var none = await _api.SendAsync(null, action, record.Id, record.Version);
        using var unknown = await _api.SendAsync("mallory", action, record.Id, record.Version);

        Assert.Equal(HttpStatusCode.Unauthorized, none.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, unknown.StatusCode);
    }

    [Theory]
    [MemberData(nameof(AllRecordActions))]
    public async Task Unknown_record_gets_404(RecordAction action)
    {
        using var response = await _api.SendAsync("ada", action, Guid.NewGuid(), 1);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [MemberData(nameof(Changes))]
    public async Task Out_of_date_version_gets_409_and_changes_nothing(RecordAction action)
    {
        var status = action is Approve or Reject ? RecordStatus.Submitted : RecordStatus.Draft;
        var record = await _api.CreateInStatusAsync("alice", status);
        var current = await RecordApi.ReadAsync(await _api.SendAsync("ada", Comment, record.Id, record.Version, comment: "first"));

        using var response = await _api.SendAsync("ada", action, record.Id, record.Version);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Contains("has changed", await response.Content.ReadAsStringAsync());
        var after = await _api.GetAsync("ada", record.Id);
        Assert.Equal((current.Version, current.History.Count), (after.Version, after.History.Count));
    }

    [Theory]
    [MemberData(nameof(ChangesThatKeepTheRecord))]
    public async Task Successful_change_raises_the_version_by_one_and_adds_one_history_entry(RecordAction action)
    {
        var status = action is Approve or Reject ? RecordStatus.Submitted : RecordStatus.Draft;
        var record = await _api.CreateInStatusAsync("alice", status);

        var updated = await RecordApi.ReadAsync(await _api.SendAsync("ada", action, record.Id, record.Version));

        Assert.Equal(record.Version + 1, updated.Version);
        Assert.Equal(record.History.Count + 1, updated.History.Count);
        Assert.Equal((updated.Version, action, "ada"), (updated.History[^1].Version, updated.History[^1].Action, updated.History[^1].UserId));
    }

    [Fact]
    public async Task Delete_removes_the_record()
    {
        var record = await _api.CreateInStatusAsync("alice", RecordStatus.Approved);

        using var delete = await _api.SendAsync("ada", Delete, record.Id, record.Version);
        using var get = await _api.SendAsync("ada", View, record.Id, record.Version);

        Assert.Equal(HttpStatusCode.NoContent, delete.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
    }

    /// <summary>Found by the Claude review on PR #3: an update without form data was a 500.</summary>
    [Fact]
    public async Task Update_without_data_gets_400_and_changes_nothing()
    {
        var record = await _api.CreateInStatusAsync("alice", RecordStatus.Draft);

        using var response = await _factory.ClientAs("alice").PutAsJsonAsync(
            Shared.Contracts.ApiRoutes.Registration(record.Id), new { expectedVersion = record.Version });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblem>();
        Assert.Equal(["Data"], problem!.Errors.Keys);
        Assert.Equal(record.Version, (await _api.GetAsync("ada", record.Id)).Version);
    }

    [Fact]
    public async Task Invalid_update_gets_400_with_errors_by_field()
    {
        var record = await _api.CreateInStatusAsync("alice", RecordStatus.Draft);
        var data = RecordApi.Valid();
        data.Email = "nope";
        data.Country = "";

        using var response = await _api.SendAsync("alice", Update, record.Id, record.Version, data);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblem>();
        Assert.Equal(["Country", "Email"], problem!.Errors.Keys.Order());
    }

    [Fact]
    public async Task Empty_comment_gets_400()
    {
        var record = await _api.CreateInStatusAsync("alice", RecordStatus.Draft);

        using var response = await _api.SendAsync("rita", Comment, record.Id, record.Version, comment: " ");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Same_version_from_two_users_one_wins_one_conflicts()
    {
        var record = await _api.CreateInStatusAsync("alice", RecordStatus.Submitted);

        using var first = await _api.SendAsync("rita", Approve, record.Id, record.Version);
        using var second = await _api.SendAsync("alice", Update, record.Id, record.Version);

        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    /// <summary>The part of a validation problem-details body the tests read.</summary>
    /// <param name="Errors">Messages by field.</param>
    private sealed record ValidationProblem(Dictionary<string, string[]> Errors);
}
