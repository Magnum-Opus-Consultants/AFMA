namespace AFMA.Data.Entities.Wims
{
  /// <summary>
  /// The Notification Type class.
  /// </summary>
  public class NotificationType
  {
    /// <summary>
    /// Gets or sets the unique identifier for the Notification Type.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the Notification Type.
    /// </summary>
    public virtual string Description { get; set; } = string.Empty;
  }
}
