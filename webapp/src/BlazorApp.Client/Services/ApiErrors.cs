using System.Net;
using System.Net.Http.Json;
using BlazorApp.Shared.Contracts;
using BlazorApp.Shared.Dtos;

namespace BlazorApp.Client.Services;

/// <summary>Turns API error responses back into the exceptions the Services layer throws.</summary>
internal static class ApiErrors
{
    /// <summary>Throws <see cref="AccessDeniedException"/> for 401 and 403 responses.</summary>
    /// <param name="response">The API response.</param>
    /// <param name="cancellationToken">Cancels reading the body.</param>
    /// <returns>A task that completes if the response is not an access error.</returns>
    public static async Task ThrowIfAccessDeniedAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var reason = response.StatusCode switch
        {
            HttpStatusCode.Unauthorized => AccessDenial.NoUser,
            HttpStatusCode.Forbidden => AccessDenial.Forbidden,
            _ => (AccessDenial?)null,
        };
        if (reason is { } denial)
        {
            throw new AccessDeniedException(denial, (await ReadProblemAsync(response, cancellationToken)).Detail);
        }
    }

    /// <summary>
    /// Throws the matching exception for any error response about a record, or does nothing on success.
    /// </summary>
    /// <param name="response">The API response.</param>
    /// <param name="id">The record the request was about.</param>
    /// <param name="expectedVersion">The version the request sent, if any.</param>
    /// <param name="action">The action the request performed.</param>
    /// <param name="cancellationToken">Cancels reading the body.</param>
    /// <returns>A task that completes if the response was successful.</returns>
    public static async Task ThrowIfRecordErrorAsync(
        HttpResponseMessage response, Guid id, int? expectedVersion, RecordAction action, CancellationToken cancellationToken)
    {
        await ThrowIfAccessDeniedAsync(response, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var problem = await ReadProblemAsync(response, cancellationToken);
        throw response.StatusCode switch
        {
            HttpStatusCode.NotFound => new RecordNotFoundException(id, problem.Detail),
            HttpStatusCode.Conflict => new VersionConflictException(id, expectedVersion ?? 0, null, problem.Detail),
            HttpStatusCode.UnprocessableEntity => new WrongStatusException(action, null, problem.Detail),
            HttpStatusCode.BadRequest when problem.Errors is { Count: > 0 } errors => new RecordValidationException(errors),
            _ => new HttpRequestException(problem.Detail, null, response.StatusCode),
        };
    }

    /// <summary>Reads a problem-details body, falling back to the status text.</summary>
    private static async Task<Problem> ReadProblemAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var fallback = $"{(int)response.StatusCode} {response.ReasonPhrase}";
        try
        {
            var problem = await response.Content.ReadFromJsonAsync<Problem>(cancellationToken);
            return problem is null ? new Problem(fallback, null)
                : string.IsNullOrWhiteSpace(problem.Detail) ? problem with { Detail = fallback }
                : problem;
        }
        catch (System.Text.Json.JsonException)
        {
            return new Problem(fallback, null);
        }
    }

    /// <summary>The parts of an RFC 7807 problem-details body the client uses.</summary>
    /// <param name="Detail">Human-readable explanation.</param>
    /// <param name="Errors">Validation errors by field, for 400 responses.</param>
    private sealed record Problem(string Detail, Dictionary<string, string[]>? Errors);
}
