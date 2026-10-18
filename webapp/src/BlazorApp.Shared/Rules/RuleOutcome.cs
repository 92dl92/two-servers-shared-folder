namespace BlazorApp.Shared.Rules;

/// <summary>Result of checking whether a user may perform an action on a record.</summary>
public enum RuleOutcome
{
    /// <summary>The action may go ahead.</summary>
    Allowed,

    /// <summary>Nobody (or an unknown user) is acting. HTTP 401.</summary>
    NoUser,

    /// <summary>The user's roles, or not owning the record, rule it out. HTTP 403.</summary>
    Forbidden,

    /// <summary>The user may do this in general, but not while the record has its current status. HTTP 422.</summary>
    WrongStatus,
}
