using System.Text.Json;
using BlazorApp.Server.Logging;
using Microsoft.Extensions.Logging;

namespace BlazorApp.Server.Tests;

/// <summary>The per-server JSON-lines log file and the server id written on each line.</summary>
public sealed class JsonLinesLoggerTests : IDisposable
{
    private readonly string _folder = Path.Combine(Path.GetTempPath(), $"jsonl-{Guid.NewGuid():N}");

    /// <inheritdoc />
    public void Dispose()
    {
        if (Directory.Exists(_folder))
        {
            Directory.Delete(_folder, recursive: true);
        }
    }

    [Fact]
    public void Each_entry_is_one_json_line_with_fields_scope_and_ids()
    {
        var path = Path.Combine(_folder, "logs", "server-5081.jsonl");
        using (var factory = LoggerFactory.Create(b => b.SetMinimumLevel(LogLevel.Trace).AddProvider(new JsonLinesLoggerProvider(path, "5081-42"))))
        {
            var logger = factory.CreateLogger("BlazorApp.Test");
            using (logger.BeginScope(new Dictionary<string, object?> { ["CorrelationId"] = "abc", ["RequestPath"] = "/api/x" }))
            {
                logger.LogWarning(new EventId(1205, "RecordUpdateConflict"), "Record {Action} v{NewVersion} ok {Ok}", "Update", 7, true);

                using (logger.BeginScope(new Dictionary<string, object?> { ["Source"] = "client", ["CorrelationId"] = "def" }))
                {
                    logger.LogInformation(new EventId(4001, "ClientEvent"), "Client {Event}", "ButtonPressed");
                }
            }

            logger.LogError(new EventId(9, "Boom"), new InvalidOperationException("bad"), "Failed");
        }

        var lines = File.ReadAllLines(path).Select(l => JsonDocument.Parse(l).RootElement).ToList();
        Assert.Equal(3, lines.Count);

        var server = lines[0];
        Assert.Equal("Warning", server.GetProperty("Level").GetString());
        Assert.Equal("BlazorApp.Test", server.GetProperty("Category").GetString());
        Assert.Equal(1205, server.GetProperty("EventId").GetInt32());
        Assert.Equal("RecordUpdateConflict", server.GetProperty("EventName").GetString());
        Assert.Equal("server", server.GetProperty("Source").GetString());
        Assert.Equal("5081-42", server.GetProperty("ServerId").GetString());
        Assert.Equal("abc", server.GetProperty("CorrelationId").GetString());
        Assert.Equal("Record Update v7 ok True", server.GetProperty("Message").GetString());
        Assert.Equal(7, server.GetProperty("Fields").GetProperty("NewVersion").GetInt32());
        Assert.True(server.GetProperty("Fields").GetProperty("Ok").GetBoolean());
        Assert.False(server.GetProperty("Fields").TryGetProperty("{OriginalFormat}", out _));
        Assert.Equal("/api/x", server.GetProperty("Scope").GetProperty("RequestPath").GetString());
        Assert.True(server.GetProperty("Timestamp").TryGetDateTimeOffset(out _));

        Assert.Equal("client", lines[1].GetProperty("Source").GetString());
        Assert.Equal("def", lines[1].GetProperty("CorrelationId").GetString());

        Assert.Equal(JsonValueKind.Null, lines[2].GetProperty("CorrelationId").ValueKind);
        Assert.Contains("bad", lines[2].GetProperty("Exception").GetString(), StringComparison.Ordinal);
    }

    [Fact]
    public void The_file_can_be_read_while_the_server_writes_and_is_appended_to_after_a_restart()
    {
        var path = Path.Combine(_folder, "server-5082.jsonl");
        using (var first = new JsonLinesLoggerProvider(path, "5082-1"))
        {
            first.CreateLogger("A").LogInformation("one");
            Assert.Single(ReadShared(path));
        }

        using (var second = new JsonLinesLoggerProvider(path, "5082-2"))
        {
            second.CreateLogger("A").LogInformation("two");
        }

        Assert.Equal(["5082-1", "5082-2"], ReadShared(path).Select(l => JsonDocument.Parse(l).RootElement.GetProperty("ServerId").GetString()));
    }

    [Theory]
    [InlineData("http://127.0.0.1:5081", 5081)]
    [InlineData("http://localhost:5080;https://localhost:7080", 5080)]
    [InlineData("http://*:5082", 5082)]
    [InlineData("http://+:5083/", 5083)]
    [InlineData(null, null)]
    [InlineData("", null)]
    public void Server_id_is_the_first_port_and_the_process_id(string? urls, int? port)
    {
        var identity = ServerIdentity.From(urls);

        Assert.Equal(port, identity.Port);
        Assert.Equal(Environment.ProcessId, identity.ProcessId);
        Assert.Equal($"{port?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? "noport"}-{Environment.ProcessId}", identity.ServerId);
    }

    [Fact]
    public void A_log_file_that_cannot_be_written_never_fails_the_caller()
    {
        // Logs are diagnostics only (decision L4): a disk error, or a line logged while the
        // server shuts down, must not turn into a failed request.
        var path = Path.Combine(_folder, "server-5084.jsonl");
        var provider = new JsonLinesLoggerProvider(path, "5084-1");
        using var factory = LoggerFactory.Create(b => b.AddProvider(provider));
        var logger = factory.CreateLogger("A");
        provider.Dispose(); // the file is closed, as at shutdown

        logger.LogInformation("too late"); // must not throw
    }

    [Theory]
    [InlineData(5081, "server-5081.jsonl")]
    [InlineData(null, null)]
    public void Log_file_is_named_after_the_port(int? port, string? expected)
    {
        Assert.Equal(expected, new ServerIdentity(port, 42).LogFileName);
    }

    [Fact]
    public void Port_0_adds_the_process_id_so_two_servers_never_share_a_file()
    {
        // Several servers started with port 0 all report port 0; .NET doesn't append atomically
        // on Unix, so a shared file would lose lines.
        Assert.Equal("server-0-42.jsonl", new ServerIdentity(0, 42).LogFileName);
        Assert.NotEqual(new ServerIdentity(0, 42).LogFileName, new ServerIdentity(0, 43).LogFileName);
    }

    private static List<string> ReadShared(string path)
    {
        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var reader = new StreamReader(stream);
        var lines = new List<string>();
        while (reader.ReadLine() is { } line)
        {
            lines.Add(line);
        }

        return lines;
    }
}
