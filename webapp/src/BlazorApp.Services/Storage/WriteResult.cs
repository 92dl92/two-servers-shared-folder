namespace BlazorApp.Services.Storage;

/// <summary>Outcome of a conditional write to an <see cref="IRegistrationStore"/>.</summary>
public enum WriteResult
{
    /// <summary>The write happened.</summary>
    Saved,

    /// <summary>Refused: the record exists already (create) or has a different version (replace, delete).</summary>
    Conflict,

    /// <summary>Refused: the record does not exist.</summary>
    NotFound,
}
