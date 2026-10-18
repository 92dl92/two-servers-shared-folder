using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using BlazorApp.Shared.Contracts;
using BlazorApp.Shared.Dtos;

namespace BlazorApp.Client.Services;

/// <summary>Browser-side <see cref="IRegistrationService"/> that calls the Server API.</summary>
/// <remarks>Error responses are turned back into the same exceptions the Services layer throws.</remarks>
/// <param name="http">HTTP client whose base address is the Server origin.</param>
public sealed class HttpRegistrationService(HttpClient http) : IRegistrationService
{
    /// <inheritdoc />
    public async Task<SubmissionResultDto> CreateAsync(
        RegistrationDto registration,
        CancellationToken cancellationToken = default)
    {
        using var response = await http.PostAsJsonAsync(ApiRoutes.Registrations, registration, cancellationToken);
        await ApiErrors.ThrowIfAccessDeniedAsync(response, cancellationToken);

        // 400 carries a SubmissionResultDto with validation errors; anything else unexpected throws.
        if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.BadRequest)
        {
            var result = await response.Content.ReadFromJsonAsync<SubmissionResultDto>(cancellationToken);
            return result ?? throw new InvalidOperationException("Server returned an empty response.");
        }

        response.EnsureSuccessStatusCode();
        throw new UnreachableException();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<RegistrationSummaryDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        using var response = await http.GetAsync(ApiRoutes.Registrations, cancellationToken);
        await ApiErrors.ThrowIfAccessDeniedAsync(response, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<RegistrationSummaryDto>>(cancellationToken) ?? [];
    }

    /// <inheritdoc />
    public async Task<RegistrationRecordDto> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var response = await http.GetAsync(ApiRoutes.Registration(id), cancellationToken);
        return await ReadRecordAsync(response, id, null, RecordAction.View, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<RegistrationRecordDto> UpdateAsync(
        Guid id, int expectedVersion, RegistrationDto data, CancellationToken cancellationToken = default)
    {
        using var response = await http.PutAsJsonAsync(
            ApiRoutes.Registration(id), new UpdateRecordDto(expectedVersion, data), cancellationToken);
        return await ReadRecordAsync(response, id, expectedVersion, RecordAction.Update, cancellationToken);
    }

    /// <inheritdoc />
    public Task<RegistrationRecordDto> SubmitAsync(Guid id, int expectedVersion, CancellationToken cancellationToken = default) =>
        PostActionAsync(id, "submit", new ExpectedVersionDto(expectedVersion), expectedVersion, RecordAction.Submit, cancellationToken);

    /// <inheritdoc />
    public Task<RegistrationRecordDto> ApproveAsync(Guid id, int expectedVersion, CancellationToken cancellationToken = default) =>
        PostActionAsync(id, "approve", new ExpectedVersionDto(expectedVersion), expectedVersion, RecordAction.Approve, cancellationToken);

    /// <inheritdoc />
    public Task<RegistrationRecordDto> RejectAsync(Guid id, int expectedVersion, CancellationToken cancellationToken = default) =>
        PostActionAsync(id, "reject", new ExpectedVersionDto(expectedVersion), expectedVersion, RecordAction.Reject, cancellationToken);

    /// <inheritdoc />
    public Task<RegistrationRecordDto> CommentAsync(
        Guid id, int expectedVersion, string text, CancellationToken cancellationToken = default) =>
        PostActionAsync(id, "comments", new CommentRequestDto(expectedVersion, text), expectedVersion, RecordAction.Comment, cancellationToken);

    /// <inheritdoc />
    public async Task DeleteAsync(Guid id, int expectedVersion, CancellationToken cancellationToken = default)
    {
        using var response = await http.DeleteAsync(
            $"{ApiRoutes.Registration(id)}?expectedVersion={expectedVersion}", cancellationToken);
        await ApiErrors.ThrowIfRecordErrorAsync(response, id, expectedVersion, RecordAction.Delete, cancellationToken);
    }

    private async Task<RegistrationRecordDto> PostActionAsync<TBody>(
        Guid id, string route, TBody body, int expectedVersion, RecordAction action, CancellationToken cancellationToken)
    {
        using var response = await http.PostAsJsonAsync(ApiRoutes.RegistrationAction(id, route), body, cancellationToken);
        return await ReadRecordAsync(response, id, expectedVersion, action, cancellationToken);
    }

    private static async Task<RegistrationRecordDto> ReadRecordAsync(
        HttpResponseMessage response, Guid id, int? expectedVersion, RecordAction action, CancellationToken cancellationToken)
    {
        await ApiErrors.ThrowIfRecordErrorAsync(response, id, expectedVersion, action, cancellationToken);
        return await response.Content.ReadFromJsonAsync<RegistrationRecordDto>(cancellationToken)
            ?? throw new InvalidOperationException("Server returned an empty response.");
    }
}
