using Microsoft.Extensions.Logging;

namespace BlazorApp.Server.Logging;

/// <summary>
/// Fixed event ids for the server's own log lines (record events are 1100–1899 in
/// <c>RecordEvents</c>, store events 2001–2004 in <c>StoreLog</c>).
/// </summary>
public static class LogEvents
{
    /// <summary>One line per API request.</summary>
    public static readonly EventId HttpRequest = new(3001, "HttpRequest");

    /// <summary>The "Acting as" user was changed.</summary>
    public static readonly EventId ActingAsChanged = new(3101, "ActingAsChanged");

    /// <summary>A request named a user id that isn't in users.json.</summary>
    public static readonly EventId UnknownUser = new(3102, "UnknownUser");

    /// <summary>The server started.</summary>
    public static readonly EventId ServerStarted = new(3201, "ServerStarted");

    /// <summary>An event sent by the browser.</summary>
    public static readonly EventId ClientEvent = new(4001, "ClientEvent");
}
