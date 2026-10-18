namespace BlazorApp.Shared.Dtos;

/// <summary>Things a user can do to registration records.</summary>
public enum RecordAction
{
    /// <summary>See a record (or the list of records).</summary>
    View,

    /// <summary>Create a new record.</summary>
    Create,

    /// <summary>Change a record's form data.</summary>
    Update,

    /// <summary>Send a draft for review.</summary>
    Submit,

    /// <summary>Accept a submitted record.</summary>
    Approve,

    /// <summary>Turn down a submitted record.</summary>
    Reject,

    /// <summary>Add a comment to a record.</summary>
    Comment,

    /// <summary>Remove a record.</summary>
    Delete,
}
