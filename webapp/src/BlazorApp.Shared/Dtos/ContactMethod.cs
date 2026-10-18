namespace BlazorApp.Shared.Dtos;

/// <summary>How a registrant prefers to be contacted.</summary>
public enum ContactMethod
{
    /// <summary>Contact by email.</summary>
    Email,

    /// <summary>Contact by phone call.</summary>
    Phone,

    /// <summary>Contact by text message.</summary>
    Sms,
}
