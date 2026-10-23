namespace AFMA.Data.Mappings.Wims
{
  using AFMA.Data.Entities.Wims;
  using FluentNHibernate.Mapping;

  /// <summary>
  /// The Notification Contact mapping class.
  /// </summary>
  public class NotificationContactMap : ClassMap<NotificationContact>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="NotificationContactMap"/> class.
    /// </summary>
    public NotificationContactMap()
    {
      Table("NotificationContact");
      Id(x => x.Id).GeneratedBy.Identity();
      Map(x => x.FullName).Not.Nullable();
      Map(x => x.EmailAddress).Not.Nullable();
      Map(x => x.ContactTypeId).Not.Nullable();
      References(x => x.NotificationTemplate, "NotificationTemplateId").Not.Nullable().Cascade.None();
      References(x => x.MemberContactType, "MemberContactTypeId").Nullable().Cascade.None();
    }
  }
}
