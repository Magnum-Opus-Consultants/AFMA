namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class IpAdditionalGroupMap : ClassMap<IpAdditionalGroup>
  {
    public IpAdditionalGroupMap()
    {
      Table("IpAdditionalGroup");
      Id(x => x.Id).GeneratedBy.Identity();
      Map(x => x.GroupName).Not.Nullable();
      Map(x => x.GroupTotal).Not.Nullable();
      Map(x => x.GroupMeasure);
      Map(x => x.GroupCode);
      Map(x => x.DisplayOrder).Not.Nullable();
      Map(x => x.Active).Not.Nullable();
    }
  }
}
