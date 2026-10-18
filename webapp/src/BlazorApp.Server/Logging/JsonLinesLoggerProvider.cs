using System.Globalization;
using System.Text.Json;

namespace BlazorApp.Server.Logging;

/// <summary>
/// Writes every log entry as one JSON object per line to <c>server-&lt;port&gt;.jsonl</c>
/// (plan section 8, decision L1: built-in logging plus this small writer, no Serilog).
/// </summary>
/// <remarks>
/// <para>Each line has <c>Timestamp</c>, <c>Level</c>, <c>Category</c>, <c>EventId</c>,
/// <c>EventName</c>, <c>Source</c> (<c>server</c>, or <c>client</c> for events sent by the
/// browser), <c>ServerId</c>, <c>CorrelationId</c>, <c>Message</c>, the entry's own fields under
/// <c>Fields</c>, other scope values under <c>Scope</c>, and <c>Exception</c> when there is one.</para>
/// <para><c>Source</c> and <c>CorrelationId</c> come from logging scopes with those keys.
/// Lines are written under a lock and flushed at once, so the file can be read while the
/// server runs; this is research code, so simplicity wins over throughput.</para>
/// </remarks>
public sealed class JsonLinesLoggerProvider : ILoggerProvider, ISupportExternalScope
{
    private static readonly byte[] NewLine = "\n"u8.ToArray();

    private readonly FileStream _file;
    private readonly Lock _gate = new();
    private readonly string _serverId;
    private IExternalScopeProvider _scopes = new LoggerExternalScopeProvider();

    /// <summary>Opens (or appends to) the log file.</summary>
    /// <param name="path">Full path of the <c>.jsonl</c> file; its folder is created if needed.</param>
    /// <param name="serverId">Written on every line.</param>
    public JsonLinesLoggerProvider(string path, string serverId)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path))!);
        LogPath = path;
        _serverId = serverId;
        _file = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
    }

    /// <summary>The log file's path.</summary>
    public string LogPath { get; }

    /// <inheritdoc />
    public ILogger CreateLogger(string categoryName) => new JsonLinesLogger(this, categoryName);

    /// <inheritdoc />
    public void SetScopeProvider(IExternalScopeProvider scopeProvider) => _scopes = scopeProvider;

    /// <inheritdoc />
    public void Dispose()
    {
        lock (_gate)
        {
            _file.Dispose();
        }
    }

    private void Write<TState>(string category, LogLevel level, EventId eventId, TState state, Exception? exception, string message)
    {
        using var buffer = new MemoryStream();
        using (var json = new Utf8JsonWriter(buffer))
        {
            var scope = new Dictionary<string, object?>(StringComparer.Ordinal);
            _scopes.ForEachScope(
                (value, values) =>
                {
                    if (value is IEnumerable<KeyValuePair<string, object?>> pairs)
                    {
                        foreach (var (key, item) in pairs)
                        {
                            values[key] = item;
                        }
                    }
                },
                scope);

            json.WriteStartObject();
            json.WriteString("Timestamp", DateTimeOffset.UtcNow);
            json.WriteString("Level", level.ToString());
            json.WriteString("Category", category);
            json.WriteNumber("EventId", eventId.Id);
            json.WriteString("EventName", eventId.Name);
            json.WriteString("Source", scope.Remove("Source", out var source) ? Convert.ToString(source, CultureInfo.InvariantCulture) : "server");
            json.WriteString("ServerId", _serverId);
            json.WriteString("CorrelationId", scope.Remove("CorrelationId", out var correlation) ? Convert.ToString(correlation, CultureInfo.InvariantCulture) : null);
            json.WriteString("Message", message);

            json.WriteStartObject("Fields");
            if (state is IEnumerable<KeyValuePair<string, object?>> fields)
            {
                foreach (var (key, value) in fields.Where(f => f.Key != "{OriginalFormat}"))
                {
                    WriteValue(json, key, value);
                }
            }

            json.WriteEndObject();

            json.WriteStartObject("Scope");
            foreach (var (key, value) in scope.Where(s => s.Key != "{OriginalFormat}"))
            {
                WriteValue(json, key, value);
            }

            json.WriteEndObject();

            if (exception is not null)
            {
                json.WriteString("Exception", exception.ToString());
            }

            json.WriteEndObject();
        }

        // The JSON is already UTF-8: write the bytes and a newline straight to the file.
        // A write that fails (disk full, file gone, or closed at shutdown) drops the line:
        // logs are diagnostics only and must never fail the request that logged (decision L4).
        lock (_gate)
        {
            try
            {
                _file.Write(buffer.GetBuffer().AsSpan(0, (int)buffer.Length));
                _file.Write(NewLine);
                _file.Flush();
            }
            catch (Exception e) when (e is IOException or ObjectDisposedException)
            {
                Console.Error.WriteLine($"Log line dropped, {LogPath} could not be written: {e.Message}");
            }
        }
    }

    /// <summary>Numbers and booleans stay typed; everything else is written as text.</summary>
    private static void WriteValue(Utf8JsonWriter json, string key, object? value)
    {
        switch (value)
        {
            case null:
                json.WriteNull(key);
                break;
            case bool b:
                json.WriteBoolean(key, b);
                break;
            case int or long or short or byte:
                json.WriteNumber(key, Convert.ToInt64(value, CultureInfo.InvariantCulture));
                break;
            case double or float or decimal:
                json.WriteNumber(key, Convert.ToDouble(value, CultureInfo.InvariantCulture));
                break;
            default:
                json.WriteString(key, Convert.ToString(value, CultureInfo.InvariantCulture));
                break;
        }
    }

    /// <summary>A logger for one category; all of them write through the provider.</summary>
    private sealed class JsonLinesLogger(JsonLinesLoggerProvider provider, string category) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull => provider._scopes.Push(state);

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (IsEnabled(logLevel))
            {
                provider.Write(category, logLevel, eventId, state, exception, formatter(state, exception));
            }
        }
    }
}
