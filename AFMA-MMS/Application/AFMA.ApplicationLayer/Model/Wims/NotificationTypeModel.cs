namespace AFMA.ApplicationLayer.Model.Wims
{
  using AFMA.Data.Entities.Wims;

  /// <summary>
  /// The Notification Type model class.
  /// </summary>
  [MapTo(typeof(NotificationType))]
  public class NotificationTypeModel
  {
    /// <summary>
    /// Gets or sets the unique identifier for the Notification Type.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the Notification Type.
    /// </summary>
    public string Description { get; set; } = string.Empty;
  }
}
