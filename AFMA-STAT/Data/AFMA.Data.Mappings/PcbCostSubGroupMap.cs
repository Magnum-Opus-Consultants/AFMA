namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class PcbCostSubGroupMap : ClassMap<PcbCostSubGroup>
  {
    public PcbCostSubGroupMap()
    {
      Table("PcbCostSubGroup");
      Id(x => x.Id).GeneratedBy.Identity();
      References(x => x.PcbCostMainGroup, "MainGroupId").Not.Nullable().Cascade.None();
      Map(x => x.SubGroupCode).Not.Nullable();
      Map(x => x.SubGroupName).Not.Nullable();
      Map(x => x.HeaderName);
      Map(x => x.DisplayOrder).Not.Nullable();
      Map(x => x.Active).Not.Nullable();
      HasMany(x => x.PcbCostItemGroups).KeyColumn("SubGroupId").Cascade.Delete().Inverse();
    }
  }
}
