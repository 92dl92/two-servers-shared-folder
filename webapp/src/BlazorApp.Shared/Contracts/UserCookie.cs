namespace BlazorApp.Shared.Contracts;

/// <summary>The cookie that remembers the "Acting as" user.</summary>
public static class UserCookie
{
    /// <summary>Cookie name. The value is a user id.</summary>
    public const string Name = "acting-as";
}
