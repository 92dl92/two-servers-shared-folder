using BlazorApp.Shared.Contracts;
using BlazorApp.Shared.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BlazorApp.Server.Endpoints;

/// <summary>HTTP API for registration records, consumed by the Client's <c>HttpRegistrationService</c>.</summary>
/// <remarks>Failures thrown by the service become 400/401/403/404/409/422 via <see cref="ApiExceptionFilter"/>.</remarks>
public static class RegistrationEndpoints
{
    /// <summary>Maps the record routes under <see cref="ApiRoutes.Registrations"/>.</summary>
    /// <param name="app">The endpoint route builder.</param>
    /// <returns>The route group, for further configuration.</returns>
    public static RouteGroupBuilder MapRegistrationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(ApiRoutes.Registrations)
            .WithTags("Registrations")
            .AddEndpointFilter<ApiExceptionFilter>();

        group.MapGet("/", GetAllAsync);
        group.MapPost("/", CreateAsync);
        group.MapGet("/{id:guid}", GetAsync);
        group.MapPut("/{id:guid}", UpdateAsync);
        group.MapPost("/{id:guid}/submit", (Guid id, ExpectedVersionDto body, IRegistrationService service, CancellationToken ct) =>
            OkAsync(service.SubmitAsync(id, body.ExpectedVersion, ct)));
        group.MapPost("/{id:guid}/approve", (Guid id, ExpectedVersionDto body, IRegistrationService service, CancellationToken ct) =>
            OkAsync(service.ApproveAsync(id, body.ExpectedVersion, ct)));
        group.MapPost("/{id:guid}/reject", (Guid id, ExpectedVersionDto body, IRegistrationService service, CancellationToken ct) =>
            OkAsync(service.RejectAsync(id, body.ExpectedVersion, ct)));
        group.MapPost("/{id:guid}/comments", (Guid id, CommentRequestDto body, IRegistrationService service, CancellationToken ct) =>
            OkAsync(service.CommentAsync(id, body.ExpectedVersion, body.Text, ct)));
        group.MapDelete("/{id:guid}", DeleteAsync);

        return group;
    }

    /// <summary>Returns the records the acting user may see, newest first.</summary>
    /// <param name="service">Registration service from the Services layer.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <returns>200 with the list of summaries.</returns>
    private static async Task<Ok<IReadOnlyList<RegistrationSummaryDto>>> GetAllAsync(
        IRegistrationService service,
        CancellationToken cancellationToken) =>
        TypedResults.Ok(await service.GetAllAsync(cancellationToken));

    /// <summary>Validates and stores a new draft record.</summary>
    /// <param name="registration">Form data from the request body.</param>
    /// <param name="service">Registration service from the Services layer.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <returns>201 with the result on success; 400 with the result (and its errors) when invalid.</returns>
    private static async Task<Results<Created<SubmissionResultDto>, BadRequest<SubmissionResultDto>>> CreateAsync(
        RegistrationDto registration,
        IRegistrationService service,
        CancellationToken cancellationToken)
    {
        var result = await service.CreateAsync(registration, cancellationToken);
        return result.Success
            ? TypedResults.Created($"/{ApiRoutes.Registration(result.Id!.Value)}", result)
            : TypedResults.BadRequest(result);
    }

    /// <summary>Returns one record.</summary>
    /// <param name="id">Record id.</param>
    /// <param name="service">Registration service from the Services layer.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <returns>200 with the record.</returns>
    private static Task<Ok<RegistrationRecordDto>> GetAsync(
        Guid id,
        IRegistrationService service,
        CancellationToken cancellationToken) =>
        OkAsync(service.GetAsync(id, cancellationToken));

    /// <summary>Replaces a record's form data.</summary>
    /// <param name="id">Record id.</param>
    /// <param name="body">Expected version and new data.</param>
    /// <param name="service">Registration service from the Services layer.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <returns>200 with the updated record.</returns>
    private static Task<Ok<RegistrationRecordDto>> UpdateAsync(
        Guid id,
        UpdateRecordDto body,
        IRegistrationService service,
        CancellationToken cancellationToken) =>
        OkAsync(service.UpdateAsync(id, body.ExpectedVersion, body.Data, cancellationToken));

    /// <summary>Deletes a record.</summary>
    /// <param name="id">Record id.</param>
    /// <param name="expectedVersion">The version the user was looking at.</param>
    /// <param name="service">Registration service from the Services layer.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <returns>204 when deleted.</returns>
    private static async Task<NoContent> DeleteAsync(
        Guid id,
        [FromQuery] int expectedVersion,
        IRegistrationService service,
        CancellationToken cancellationToken)
    {
        await service.DeleteAsync(id, expectedVersion, cancellationToken);
        return TypedResults.NoContent();
    }

    /// <summary>Wraps a service call that returns a record in a 200 response.</summary>
    /// <param name="call">The pending service call.</param>
    /// <returns>200 with the record.</returns>
    private static async Task<Ok<RegistrationRecordDto>> OkAsync(Task<RegistrationRecordDto> call) =>
        TypedResults.Ok(await call);
}
