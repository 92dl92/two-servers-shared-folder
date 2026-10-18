using Microsoft.Extensions.Logging;

namespace BlazorApp.Services.Storage;

/// <summary>Which write a <see cref="StoreLog"/> entry is about.</summary>
public enum StoreOperation
{
    /// <summary><see cref="IRegistrationStore.TryCreateAsync"/>.</summary>
    Create,

    /// <summary><see cref="IRegistrationStore.TryReplaceAsync"/>.</summary>
    Replace,

    /// <summary><see cref="IRegistrationStore.TryDeleteAsync"/>.</summary>
    Delete,
}

/// <summary>
/// The Debug-level events every store writes, with fixed ids shared by all strategies, so the
/// experiment can count reads, writes and conflicts per server and strategy (plan section 8).
/// Each takes a free-text <c>detail</c> for the store's own information: file name, lock wait
/// time, number of retries. Ids and versions only, never form values.
/// </summary>
public static class StoreLog
{
    /// <summary>A record was read (or found missing).</summary>
    public static readonly EventId Read = new(2001, "StoreRead");

    /// <summary>All records were listed.</summary>
    public static readonly EventId List = new(2002, "StoreList");

    /// <summary>A create, replace or delete finished with Saved or NotFound.</summary>
    public static readonly EventId Write = new(2003, "StoreWrite");

    /// <summary>A create, replace or delete was refused as a conflict.</summary>
    public static readonly EventId Conflict = new(2004, "StoreConflict");

    /// <summary>Replacing a file had to be tried again because it was in use (<see cref="FileReplace"/>).</summary>
    public static readonly EventId ReplaceRetried = new(2005, "StoreReplaceRetried");

    private static readonly Action<ILogger, Guid, bool, string?, Exception?> ReadMessage =
        LoggerMessage.Define<Guid, bool, string?>(LogLevel.Debug, Read, "Store read {RecordId}: found {Found} ({Detail})");

    private static readonly Action<ILogger, int, string?, Exception?> ListMessage =
        LoggerMessage.Define<int, string?>(LogLevel.Debug, List, "Store list: {Count} records ({Detail})");

    private static readonly Action<ILogger, StoreOperation, Guid, int?, WriteResult, string?, Exception?> WriteMessage =
        LoggerMessage.Define<StoreOperation, Guid, int?, WriteResult, string?>(
            LogLevel.Debug, Write, "Store {Operation} {RecordId} expecting version {ExpectedVersion}: {Result} ({Detail})");

    private static readonly Action<ILogger, StoreOperation, Guid, int?, WriteResult, string?, Exception?> ConflictMessage =
        LoggerMessage.Define<StoreOperation, Guid, int?, WriteResult, string?>(
            LogLevel.Debug, Conflict, "Store {Operation} {RecordId} expecting version {ExpectedVersion}: {Result} ({Detail})");

    private static readonly Action<ILogger, string, string, int, long, Exception?> ReplaceRetriedMessage =
        LoggerMessage.Define<string, string, int, long>(
            LogLevel.Warning, ReplaceRetried, "Store replace of {File} {Outcome} after {Retries} retries, {WaitedMs} ms: the file was in use");

    /// <summary>Logs a replace that needed retries; a warning, so it shows in every run's log.</summary>
    /// <param name="logger">The store's logger.</param>
    /// <param name="file">The file replaced (its name only).</param>
    /// <param name="outcome"><c>succeeded</c>, or <c>failed</c> when it gave up.</param>
    /// <param name="retries">How many times it was tried again.</param>
    /// <param name="waitedMs">How long that took.</param>
    public static void LogReplaceRetried(ILogger logger, string file, string outcome, int retries, long waitedMs) =>
        ReplaceRetriedMessage(logger, file, outcome, retries, waitedMs, null);

    /// <summary>Logs a read.</summary>
    /// <param name="logger">The store's logger.</param>
    /// <param name="recordId">The record.</param>
    /// <param name="found">Whether it existed.</param>
    /// <param name="detail">Store-specific detail, e.g. the file read.</param>
    public static void LogRead(ILogger logger, Guid recordId, bool found, string? detail = null) =>
        ReadMessage(logger, recordId, found, detail, null);

    /// <summary>Logs a list.</summary>
    /// <param name="logger">The store's logger.</param>
    /// <param name="count">How many records were returned.</param>
    /// <param name="detail">Store-specific detail.</param>
    public static void LogList(ILogger logger, int count, string? detail = null) =>
        ListMessage(logger, count, detail, null);

    /// <summary>Logs a write; a <see cref="WriteResult.Conflict"/> is logged as <see cref="Conflict"/>.</summary>
    /// <param name="logger">The store's logger.</param>
    /// <param name="operation">Which write.</param>
    /// <param name="recordId">The record.</param>
    /// <param name="expectedVersion">The version the caller expected; null for a create.</param>
    /// <param name="result">How it ended.</param>
    /// <param name="detail">Store-specific detail, e.g. lock wait time or retries.</param>
    public static void LogWrite(
        ILogger logger, StoreOperation operation, Guid recordId, int? expectedVersion, WriteResult result, string? detail = null) =>
        (result == WriteResult.Conflict ? ConflictMessage : WriteMessage)(logger, operation, recordId, expectedVersion, result, detail, null);
}
