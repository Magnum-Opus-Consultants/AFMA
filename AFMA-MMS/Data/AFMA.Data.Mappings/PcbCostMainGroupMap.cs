namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class PcbCostMainGroupMap : ClassMap<PcbCostMainGroup>
  {
    public PcbCostMainGroupMap()
    {
      Table("PcbCostMainGroup");
      Id(x => x.Id).GeneratedBy.Identity();
      Map(x => x.GroupCode).Not.Nullable();
      Map(x => x.GroupName).Not.Nullable();
      Map(x => x.Active).Not.Nullable();
      HasMany(x => x.SubGroups).KeyColumn("MainGroupId").Cascade.None();
    }
  }
}
