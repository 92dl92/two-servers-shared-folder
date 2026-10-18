using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Shared.Dtos;

/// <summary>
/// Data captured by the tabbed registration form. The same annotations drive
/// client-side validation (EditForm) and server-side validation (Services).
/// </summary>
/// <remarks>
/// A mutable class rather than a record, because Blazor input components bind
/// to settable properties.
/// </remarks>
public sealed class RegistrationDto
{
    // --- Personal tab ---------------------------------------------------

    /// <summary>Given name.</summary>
    [Required, StringLength(50)]
    [Display(Name = "First name")]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>Family name.</summary>
    [Required, StringLength(50)]
    [Display(Name = "Last name")]
    public string LastName { get; set; } = string.Empty;

    /// <summary>Email address.</summary>
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>Optional phone number.</summary>
    [Phone]
    public string? Phone { get; set; }

    // --- Address tab ----------------------------------------------------

    /// <summary>Street and house number.</summary>
    [Required, StringLength(100)]
    public string Street { get; set; } = string.Empty;

    /// <summary>City or town.</summary>
    [Required, StringLength(60)]
    public string City { get; set; } = string.Empty;

    /// <summary>Postal or ZIP code.</summary>
    [Required, StringLength(12)]
    [Display(Name = "Postal code")]
    public string PostalCode { get; set; } = string.Empty;

    /// <summary>Country name.</summary>
    [Required, StringLength(60)]
    public string Country { get; set; } = string.Empty;

    // --- Preferences tab ------------------------------------------------

    /// <summary>Preferred contact channel.</summary>
    [Display(Name = "Preferred contact")]
    public ContactMethod PreferredContact { get; set; } = ContactMethod.Email;

    /// <summary>Whether the registrant opted into the newsletter.</summary>
    public bool SubscribeToNewsletter { get; set; }

    /// <summary>Free-text notes.</summary>
    [StringLength(500)]
    public string? Notes { get; set; }
}
