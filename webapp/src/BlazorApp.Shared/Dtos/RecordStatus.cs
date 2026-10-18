namespace BlazorApp.Shared.Dtos;

/// <summary>Where a record is in the workflow: Draft → Submitted → Approved or Rejected.</summary>
public enum RecordStatus
{
    /// <summary>Created, not yet sent for review.</summary>
    Draft,

    /// <summary>Waiting for a reviewer.</summary>
    Submitted,

    /// <summary>Accepted by a reviewer.</summary>
    Approved,

    /// <summary>Turned down by a reviewer.</summary>
    Rejected,
}
