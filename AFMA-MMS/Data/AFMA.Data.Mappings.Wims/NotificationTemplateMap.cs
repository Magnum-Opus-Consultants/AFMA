namespace AFMA.Data.Mappings.Wims
{
  using AFMA.Data.Entities.Wims;
  using FluentNHibernate.Mapping;

  /// <summary>
  /// The Notification Template mapping class.
  /// </summary>
  public class NotificationTemplateMap : ClassMap<NotificationTemplate>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationTemplateMap"/> class.
    /// </summary>
    public NotificationTemplateMap()
    {
      Table("NotificationTemplate");
      Id(x => x.Id).GeneratedBy.Identity();
      Map(x => x.Description).Not.Nullable();
      References(x => x.NotificationType, "NotificationTypeId").Not.Nullable().Cascade.None();
      Map(x => x.Subject).Not.Nullable();
      Map(x => x.Body).Not.Nullable();
      HasMany(x => x.NotificationContacts).KeyColumn("NotificationTemplateId").Cascade.Delete().Inverse();
    }
  }
}
