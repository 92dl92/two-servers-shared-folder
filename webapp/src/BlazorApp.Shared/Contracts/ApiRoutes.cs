namespace BlazorApp.Shared.Contracts;

/// <summary>HTTP routes shared by the Server endpoints and the Client HTTP services.</summary>
public static class ApiRoutes
{
    /// <summary>Base route for registration endpoints.</summary>
    public const string Registrations = "api/registrations";

    /// <summary>Route of one record.</summary>
    /// <param name="id">Record id.</param>
    /// <returns>The relative URL.</returns>
    public static string Registration(Guid id) => $"{Registrations}/{id}";

    /// <summary>Route of a workflow action on a record.</summary>
    /// <param name="id">Record id.</param>
    /// <param name="action">One of <c>submit</c>, <c>approve</c>, <c>reject</c>, <c>comments</c>.</param>
    /// <returns>The relative URL.</returns>
    public static string RegistrationAction(Guid id, string action) => $"{Registrations}/{id}/{action}";

    /// <summary>Receives batches of client events (<c>POST</c>) for the server's log.</summary>
    public const string ClientLogs = "api/client-logs";

    /// <summary>Lists the users that can be picked in "Acting as".</summary>
    public const string Users = "api/users";

    /// <summary>Gets (GET) or sets (PUT) the acting user.</summary>
    public const string CurrentUser = "api/users/current";
}
