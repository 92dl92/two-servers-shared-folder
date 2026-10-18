using System.ComponentModel.DataAnnotations;
using BlazorApp.Services.Logging;
using BlazorApp.Services.Storage;
using BlazorApp.Services.Users;
using BlazorApp.Shared.Contracts;
using BlazorApp.Shared.Dtos;
using BlazorApp.Shared.Rules;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace BlazorApp.Services;

/// <summary>
/// <see cref="IRegistrationService"/> over an <see cref="IRegistrationStore"/>. Checks the acting
/// user's permissions, the workflow status and the expected version on every change, then
/// leaves it to the store to write only if nobody else changed the record in between.
/// </summary>
/// <param name="timeProvider">Clock used to stamp changes (injectable for tests).</param>
/// <param name="currentUser">The acting user, whose permissions are checked on every call.</param>
/// <param name="store">Where records are kept; chosen by the <c>Storage:Strategy</c> setting.</param>
/// <param name="logger">Receives one record event per command (see <see cref="RecordEvents"/>); none when null.</param>
public sealed class RegistrationService(
    TimeProvider timeProvider,
    ICurrentUser currentUser,
    IRegistrationStore store,
    ILogger<RegistrationService>? logger = null) : IRegistrationService
{
    private readonly ILogger _logger = logger ?? NullLogger<RegistrationService>.Instance;

    /// <summary>Longest comment accepted.</summary>
    public const int MaxCommentLength = 1000;

    /// <inheritdoc />
    public Task<SubmissionResultDto> CreateAsync(
        RegistrationDto registration,
        CancellationToken cancellationToken = default) =>
        LoggedAsync(new RecordEventInfo(RecordAction.Create, null, null), info => CreateCoreAsync(registration, info, cancellationToken));

    private async Task<SubmissionResultDto> CreateCoreAsync(
        RegistrationDto registration, RecordEventInfo info, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(registration);
        var (user, _) = Access.Require(currentUser, RecordAction.Create);

        // Never trust the client: re-run the DTO's data annotations on the server.
        var errors = Validate(registration);
        if (errors.Count > 0)
        {
            info.Outcome = RecordOutcome.Invalid;
            info.InvalidFields = [.. errors.Keys];
            return SubmissionResultDto.Failed(errors);
        }

        var now = timeProvider.GetUtcNow();
        var record = new RegistrationRecordDto(
            Guid.NewGuid(),
            Version: 1,
            RecordStatus.Draft,
            user.Id,
            now,
            user.Id,
            now,
            registration,
            [new HistoryEntryDto(1, RecordAction.Create, user.Id, now)]);

        if (await store.TryCreateAsync(record, cancellationToken) != WriteResult.Saved)
        {
            throw new InvalidOperationException($"Record id {record.Id} already exists.");
        }

        info.RecordId = record.Id;
        info.NewVersion = record.Version;
        info.NewStatus = record.Status;
        return SubmissionResultDto.Ok(record.Id);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<RegistrationSummaryDto>> GetAllAsync(
        CancellationToken cancellationToken = default) =>
        LoggedAsync<IReadOnlyList<RegistrationSummaryDto>>(new RecordEventInfo(RecordAction.View, null, null), async _ =>
        {
            var (user, scope) = Access.Require(currentUser, RecordAction.View);

            return
            [
                .. (await store.ListAsync(cancellationToken))
                    .Where(r => scope == PermissionScope.Any || r.CreatedBy == user.Id)
                    .OrderByDescending(r => r.CreatedAt)
                    .Select(RegistrationSummaryDto.From),
            ];
        });

    /// <inheritdoc />
    public Task<RegistrationRecordDto> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
        LoggedAsync(new RecordEventInfo(RecordAction.View, id, null), async info =>
        {
            var (_, record) = await LoadAsync(id, RecordAction.View, info, cancellationToken);
            return record;
        });

    /// <inheritdoc />
    public Task<RegistrationRecordDto> UpdateAsync(
        Guid id, int expectedVersion, RegistrationDto data, CancellationToken cancellationToken = default)
    {
        // Over HTTP a body can leave out the data even though the parameter isn't nullable;
        // that is invalid input (400), checked after access like any other validation error.
        return ChangeAsync(
            id,
            expectedVersion,
            RecordAction.Update,
            cancellationToken,
            data,
            validate: () => data is null ? new() { ["Data"] = ["The form data is required."] } : Validate(data));
    }

    /// <inheritdoc />
    public Task<RegistrationRecordDto> SubmitAsync(Guid id, int expectedVersion, CancellationToken cancellationToken = default) =>
        ChangeAsync(id, expectedVersion, RecordAction.Submit, cancellationToken);

    /// <inheritdoc />
    public Task<RegistrationRecordDto> ApproveAsync(Guid id, int expectedVersion, CancellationToken cancellationToken = default) =>
        ChangeAsync(id, expectedVersion, RecordAction.Approve, cancellationToken);

    /// <inheritdoc />
    public Task<RegistrationRecordDto> RejectAsync(Guid id, int expectedVersion, CancellationToken cancellationToken = default) =>
        ChangeAsync(id, expectedVersion, RecordAction.Reject, cancellationToken);

    /// <inheritdoc />
    public Task<RegistrationRecordDto> CommentAsync(
        Guid id, int expectedVersion, string text, CancellationToken cancellationToken = default) =>
        ChangeAsync(id, expectedVersion, RecordAction.Comment, cancellationToken, comment: text?.Trim(), validate: () => ValidateComment(text));

    /// <inheritdoc />
    public Task DeleteAsync(Guid id, int expectedVersion, CancellationToken cancellationToken = default) =>
        LoggedAsync(new RecordEventInfo(RecordAction.Delete, id, expectedVersion), async info =>
        {
            var (_, record) = await LoadAsync(id, RecordAction.Delete, info, cancellationToken);
            CheckVersionAndStatus(record, expectedVersion, RecordAction.Delete);

            return await store.TryDeleteAsync(id, expectedVersion, cancellationToken) switch
            {
                WriteResult.Saved => true,
                WriteResult.NotFound => throw new RecordNotFoundException(id),
                _ => throw new VersionConflictException(id, expectedVersion, actualVersion: null),
            };
        });

    /// <summary>Loads a record and checks the acting user may perform <paramref name="action"/> on it.</summary>
    private async Task<(UserDto User, RegistrationRecordDto Record)> LoadAsync(
        Guid id, RecordAction action, RecordEventInfo info, CancellationToken cancellationToken)
    {
        var user = Access.RequireUser(currentUser);
        var record = await store.GetAsync(id, cancellationToken) ?? throw new RecordNotFoundException(id);
        info.OldStatus = record.Status;
        if (RecordRules.CheckPermission(user, action, record.CreatedBy) != RuleOutcome.Allowed)
        {
            throw new AccessDeniedException(
                AccessDenial.Forbidden,
                $"{user.Name} may not {action.ToString().ToLowerInvariant()} this record.");
        }

        return (user, record);
    }

    /// <summary>
    /// The shared path for every change: permission, validation, version, status, then a write
    /// that only succeeds if nobody else changed the record in between.
    /// </summary>
    private Task<RegistrationRecordDto> ChangeAsync(
        Guid id,
        int expectedVersion,
        RecordAction action,
        CancellationToken cancellationToken,
        RegistrationDto? data = null,
        string? comment = null,
        Func<Dictionary<string, string[]>>? validate = null) =>
        LoggedAsync(
            new RecordEventInfo(action, id, expectedVersion),
            info => ChangeCoreAsync(id, expectedVersion, action, info, cancellationToken, data, comment, validate));

    private async Task<RegistrationRecordDto> ChangeCoreAsync(
        Guid id,
        int expectedVersion,
        RecordAction action,
        RecordEventInfo info,
        CancellationToken cancellationToken,
        RegistrationDto? data,
        string? comment,
        Func<Dictionary<string, string[]>>? validate)
    {
        var (user, record) = await LoadAsync(id, action, info, cancellationToken);

        if (validate?.Invoke() is { Count: > 0 } errors)
        {
            throw new RecordValidationException(errors);
        }

        CheckVersionAndStatus(record, expectedVersion, action);

        var now = timeProvider.GetUtcNow();
        var version = record.Version + 1;
        var updated = record with
        {
            Version = version,
            Status = RecordRules.StatusAfter(action, record.Status),
            UpdatedBy = user.Id,
            UpdatedAt = now,
            Data = data ?? record.Data,
            History = [.. record.History, new HistoryEntryDto(version, action, user.Id, now, comment)],
        };

        info.NewVersion = updated.Version;
        info.NewStatus = updated.Status;
        return await store.TryReplaceAsync(updated, expectedVersion, cancellationToken) switch
        {
            WriteResult.Saved => updated,
            WriteResult.NotFound => throw new RecordNotFoundException(id),

            // Someone else saved between our read and our write.
            _ => throw new VersionConflictException(id, expectedVersion, actualVersion: null),
        };
    }

    /// <summary>
    /// Runs one command and writes its record event, whatever the outcome: the single place
    /// record events come from, so API calls and prerendered pages both produce them.
    /// </summary>
    /// <param name="info">The event's fields; the command fills in what it learns.</param>
    /// <param name="command">The command.</param>
    /// <returns>The command's result; refusals are rethrown after logging.</returns>
    private async Task<T> LoggedAsync<T>(RecordEventInfo info, Func<RecordEventInfo, Task<T>> command)
    {
        var started = timeProvider.GetTimestamp();
        info.UserId = currentUser.RequestedId;
        info.Outcome = info.Action == RecordAction.View ? RecordOutcome.Read : RecordOutcome.Saved;

        // An unexpected exception (a bug, a broken store) has no outcome; ASP.NET logs it as a 500.
        var known = true;
        try
        {
            var result = await command(info);
            if (info.Outcome != RecordOutcome.Saved)
            {
                // Read, or an invalid Create (which returns errors instead of throwing).
                info.NewVersion = null;
                info.NewStatus = null;
            }

            return result;
        }
        catch (Exception e) when (OutcomeOf(e) is { } outcome)
        {
            info.Outcome = outcome;
            info.NewVersion = null;
            info.NewStatus = null;
            if (e is RecordValidationException invalid)
            {
                info.InvalidFields = [.. invalid.Errors.Keys];
            }

            throw;
        }
        catch
        {
            known = false;
            throw;
        }
        finally
        {
            if (known)
            {
                info.Duration = timeProvider.GetElapsedTime(started);
                RecordEvents.Log(_logger, info);
            }
        }
    }

    /// <summary>The outcome a refusal exception stands for; null for unexpected errors, which get no record event.</summary>
    private static RecordOutcome? OutcomeOf(Exception e) => e switch
    {
        AccessDeniedException { Reason: AccessDenial.NoUser } => RecordOutcome.NoUser,
        AccessDeniedException => RecordOutcome.Forbidden,
        RecordNotFoundException => RecordOutcome.NotFound,
        RecordValidationException => RecordOutcome.Invalid,
        VersionConflictException => RecordOutcome.Conflict,
        WrongStatusException => RecordOutcome.WrongStatus,
        _ => null,
    };

    /// <summary>Refuses an out-of-date version (409) before a disallowed status (422).</summary>
    private static void CheckVersionAndStatus(RegistrationRecordDto record, int expectedVersion, RecordAction action)
    {
        if (record.Version != expectedVersion)
        {
            throw new VersionConflictException(record.Id, expectedVersion, record.Version);
        }

        if (!RecordRules.StatusAllows(action, record.Status))
        {
            throw new WrongStatusException(action, record.Status);
        }
    }

    /// <summary>Runs data-annotation validation and groups failures by member name.</summary>
    /// <param name="registration">The DTO to validate.</param>
    /// <returns>Errors keyed by property name; empty when valid.</returns>
    private static Dictionary<string, string[]> Validate(RegistrationDto registration)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(
            registration, new ValidationContext(registration), results, validateAllProperties: true);

        return results
            .SelectMany(r => (r.MemberNames.Any() ? r.MemberNames : [string.Empty])
                .Select(member => (member, message: r.ErrorMessage ?? "Invalid value.")))
            .GroupBy(e => e.member)
            .ToDictionary(g => g.Key, g => g.Select(e => e.message).ToArray());
    }

    /// <summary>A comment must have text and be at most <see cref="MaxCommentLength"/> characters.</summary>
    private static Dictionary<string, string[]> ValidateComment(string? text) =>
        string.IsNullOrWhiteSpace(text) ? new() { ["Text"] = ["A comment needs some text."] }
        : text.Trim().Length > MaxCommentLength ? new() { ["Text"] = [$"A comment can be at most {MaxCommentLength} characters."] }
        : [];
}
