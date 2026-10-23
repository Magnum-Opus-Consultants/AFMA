namespace AFMA.ApplicationLayer.Model.Wims
{
  using AFMA.Data.Entities.Wims;
  using System.Collections.Generic;

  /// <summary>
  /// The Notification Template model class.
  /// </summary>
  [MapTo(typeof(NotificationTemplate))]
  public class NotificationTemplateModel
  {
    /// <summary>
    /// Gets or sets the unique identifier for the Notification Template.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the description of the Notification Template.
    /// </summary>
    [Display(Name = "Description")]
    [StringLength(100, ErrorMessage = "Description must 100 characters or less")]
    [Required(ErrorMessage = "Description is required")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Notification Type foreign key.
    /// </summary>
    [Display(Name = "Notification Type")]
    [Required(ErrorMessage = "Notification Type is required")]
    public int NotificationTypeId { get; set; }

    /// <summary>
    /// Gets or sets the Notification Type foreign key.
    /// </summary>
    [Display(Name = "Notification Type")]
    public string NotificationType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the subject of the Notification Template.
    /// </summary>
    [Display(Name = "Subject")]
    [StringLength(250, ErrorMessage = "Subject must 250 characters or less")]
    [Required(ErrorMessage = "Subject is required")]
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the body of the Notification Template.
    /// </summary>
    [Display(Name = "Body")]
    [Required(ErrorMessage = "Body is required")]
    public string Body { get; set; } = string.Empty;

    public ICollection<NotificationContactModel> NotificationContacts { get; set; } = new List<NotificationContactModel>();
  }
}
