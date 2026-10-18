namespace BlazorApp.Shared.Rules;

/// <summary>Which records a permission covers.</summary>
public enum PermissionScope
{
    /// <summary>Not allowed at all.</summary>
    None,

    /// <summary>Allowed only on records the user created.</summary>
    Own,

    /// <summary>Allowed on every record.</summary>
    Any,
}
