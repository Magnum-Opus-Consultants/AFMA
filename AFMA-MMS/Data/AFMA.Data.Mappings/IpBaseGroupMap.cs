namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class IpBaseGroupMap : ClassMap<IpBaseGroup>
  {
    public IpBaseGroupMap()
    {
      Table("IpBaseGroup");
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