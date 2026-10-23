namespace AFMA.Data.Mappings.Wims
{
  using AFMA.Data.Entities.Wims;
  using FluentNHibernate.Mapping;

  /// <summary>
  /// The Notification Type mapping class.
  /// </summary>
  public class NotificationTypeMap : ClassMap<NotificationType>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationTypeMap"/> class.
    /// </summary>
    public NotificationTypeMap()
    {
      Table("NotificationType");
      Id(x => x.Id).GeneratedBy.Identity();
      Map(x => x.Description).Not.Nullable();
    }
  }
}
