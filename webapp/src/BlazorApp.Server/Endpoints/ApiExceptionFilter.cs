using BlazorApp.Shared.Contracts;

namespace BlazorApp.Server.Endpoints;

/// <summary>
/// Turns the exceptions the Services layer throws into HTTP responses: 401/403 for
/// <see cref="AccessDeniedException"/>, 404 not found, 400 invalid data, 409 out-of-date
/// version, 422 wrong status. Each is a problem-details body whose <c>detail</c> explains it.
/// </summary>
public sealed class ApiExceptionFilter : IEndpointFilter
{
    /// <inheritdoc />
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        try
        {
            return await next(context);
        }
        catch (AccessDeniedException e)
        {
            return TypedResults.Problem(
                e.Message,
                statusCode: e.Reason == AccessDenial.NoUser
                    ? StatusCodes.Status401Unauthorized
                    : StatusCodes.Status403Forbidden);
        }
        catch (RecordNotFoundException e)
        {
            return TypedResults.Problem(e.Message, statusCode: StatusCodes.Status404NotFound);
        }
        catch (RecordValidationException e)
        {
            return TypedResults.ValidationProblem(e.Errors.ToDictionary(), detail: e.Message);
        }
        catch (VersionConflictException e)
        {
            return TypedResults.Problem(e.Message, statusCode: StatusCodes.Status409Conflict);
        }
        catch (WrongStatusException e)
        {
            return TypedResults.Problem(e.Message, statusCode: StatusCodes.Status422UnprocessableEntity);
        }
    }
}
