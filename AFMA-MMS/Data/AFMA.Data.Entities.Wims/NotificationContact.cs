namespace AFMA.Data.Entities.Wims
{
  /// <summary>
  /// The Notification Contact class.
  /// </summary>
  public class NotificationContact
  {
    /// <summary>
    /// Gets or sets the unique identifier for the Notification Contact.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the full name of the Notification Contact.
    /// </summary>
    public virtual string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email address of the Notification Contact.
    /// </summary>
    public virtual string EmailAddress { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Notification Template foreign key.
    /// </summary>
    public virtual NotificationTemplate NotificationTemplate { get; set; } = new();

    /// <summary>
    /// Gets or sets the Contact Type
    /// </summary>
    public virtual int ContactTypeId { get; set; }

    /// <summary>
    /// Gets or sets the Member Contact Type
    /// </summary>
    public virtual MemberContactType MemberContactType { get; set; } = new();
  }
}
