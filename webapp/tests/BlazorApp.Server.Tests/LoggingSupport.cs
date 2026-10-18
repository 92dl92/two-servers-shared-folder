using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;

namespace BlazorApp.Server.Tests;

/// <summary>Captures everything the in-process server logs, down to Trace, with <see cref="FakeLogger"/>.</summary>
public static class LoggingSupport
{
    /// <summary>A factory whose logs, at every level, go to a <see cref="FakeLogCollector"/>.</summary>
    /// <param name="factory">The app under test.</param>
    /// <returns>A derived factory; read its logs with <see cref="Logs"/>.</returns>
    public static WebApplicationFactory<Program> WithFakeLogging(this WebApplicationFactory<Program> factory) =>
        factory.WithWebHostBuilder(b =>
        {
            b.UseSetting("Logging:LogLevel:Default", "Trace");
            b.UseSetting("Logging:LogLevel:Microsoft.AspNetCore", "Trace");
            b.UseSetting("Logging:LogLevel:BlazorApp", "Trace");
            b.ConfigureLogging(l => l.AddFakeLogging());
        });

    /// <summary>Everything logged so far.</summary>
    /// <param name="factory">A factory made by <see cref="WithFakeLogging"/>.</param>
    /// <returns>The captured records, oldest first.</returns>
    public static IReadOnlyList<FakeLogRecord> Logs(this WebApplicationFactory<Program> factory) =>
        factory.Services.GetFakeLogCollector().GetSnapshot();

    /// <summary>The innermost scope value with this key, as text.</summary>
    /// <param name="record">A captured record.</param>
    /// <param name="key">Scope key, e.g. <c>CorrelationId</c>.</param>
    /// <returns>The value, or null when no scope has it.</returns>
    public static string? ScopeValue(this FakeLogRecord record, string key)
    {
        ArgumentNullException.ThrowIfNull(record);
        string? found = null;
        foreach (var scope in record.Scopes)
        {
            if (scope is IEnumerable<KeyValuePair<string, object?>> pairs)
            {
                foreach (var (k, v) in pairs)
                {
                    if (k == key)
                    {
                        found = v?.ToString();
                    }
                }
            }
        }

        return found;
    }

    /// <summary>All the text in a record: message, fields, scopes and exception.</summary>
    /// <param name="record">A captured record.</param>
    /// <returns>Everything that could reach a log file.</returns>
    public static string AllText(this FakeLogRecord record)
    {
        ArgumentNullException.ThrowIfNull(record);
        var parts = new List<string?> { record.Message, record.Exception?.ToString() };
        parts.AddRange(record.StructuredState?.Select(kv => kv.Value) ?? []);
        foreach (var scope in record.Scopes)
        {
            parts.Add(scope?.ToString());
            if (scope is IEnumerable<KeyValuePair<string, object?>> pairs)
            {
                parts.AddRange(pairs.Select(p => p.Value?.ToString()));
            }
        }

        return string.Join("\n", parts.Where(p => p is not null));
    }
}
