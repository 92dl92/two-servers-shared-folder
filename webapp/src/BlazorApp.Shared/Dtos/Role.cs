namespace BlazorApp.Shared.Dtos;

/// <summary>What a user is allowed to do. A user can hold several roles.</summary>
public enum Role
{
    /// <summary>Creates records and edits their own while they are drafts or submitted.</summary>
    Registrant,

    /// <summary>Views all records, approves or rejects them and adds comments.</summary>
    Reviewer,

    /// <summary>Can do everything, including delete.</summary>
    Admin,
}
