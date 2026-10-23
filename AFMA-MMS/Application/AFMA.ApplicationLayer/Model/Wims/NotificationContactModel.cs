namespace AFMA.ApplicationLayer.Model.Wims
{
  using AFMA.ApplicationLayer.Helpers;
  using AFMA.Data.Entities.Wims;

  /// <summary>
  /// The Notification Contact model class.
  /// </summary>
  [MapTo(typeof(NotificationContact))]
  public class NotificationContactModel
  {
    /// <summary>
    /// Gets or sets the unique identifier for the Notification Contact.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the full name of the Notification Contact.
    /// </summary>
    [Display(Name = "Full Name")]
    [StringLength(100, ErrorMessage = "Full Name must 100 characters or less")]
    [Required(ErrorMessage = "Full Name is required")]
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email address of the Notification Contact.
    /// </summary>
    [Display(Name = "Email Address")]
    [StringLength(100, ErrorMessage = "Email Address must 100 characters or less")]
    [Required(ErrorMessage = "Email Address is required")]
    [RegularExpression("[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}", ErrorMessage = "Please enter a valid email address")]
    public string EmailAddress { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Notification Template foreign key.
    /// </summary>
    public int NotificationTemplateId { get; set; }

    /// <summary>
    /// Gets or sets the Notification Template foreign key.
    /// </summary>
    public string NotificationTemplate { get; set; } = string.Empty;

    [Display(Name = "Contact Type")]
    public ContactType ContactTypeId { get; set; }

    /// <summary>
    /// Gets or sets the Member Contact Type
    /// </summary>
    [Display(Name = "Member Contact Type")]
    public int? MemberContactTypeId { get; set; }

    /// <summary>
    /// Gets or sets the Member Contact Type
    /// </summary>
    [Display(Name = "Member Contact Type")]
    public string MemberContactType { get; set; } = string.Empty;
  }
}
