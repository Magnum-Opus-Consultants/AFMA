namespace AFMA.Data.Entities.Wims
{
  /// <summary>
  /// The Notification Template class.
  /// </summary>
  public class NotificationTemplate
  {
    /// <summary>
    /// Gets or sets the unique identifier for the Notification Template.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the description of the Notification Template.
    /// </summary>
    public virtual string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Notification Type foreign key.
    /// </summary>
    public virtual NotificationType NotificationType { get; set; } = new();

    /// <summary>
    /// Gets or sets the subject of the Notification Template.
    /// </summary>
    public virtual string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the body of the Notification Template.
    /// </summary>
    public virtual string Body { get; set; } = string.Empty;

    public virtual ICollection<NotificationContact> NotificationContacts { get; set; } = new List<NotificationContact>();
  }
}
