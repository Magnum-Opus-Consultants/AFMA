namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class IndustryGroupItemMap : ClassMap<IndustryGroupItem>
  {
    public IndustryGroupItemMap()
    {
      Table("IndustryGroupItem");
      Id(x => x.Id).GeneratedBy.Identity();
      Map(x => x.ItemName).Not.Nullable();
      Map(x => x.LinkedTable);
      Map(x => x.LinkedId1).Not.Nullable();
      Map(x => x.LinkedId2).Not.Nullable();
      Map(x => x.Active).Not.Nullable();
      References(x => x.IndustryCostGroup, "IndustryCostGroupId").Cascade.None();
      Map(x => x.RunningCostType);
      Map(x => x.RowOnRunningCostSheet);
      Map(x => x.LinkedIndustryCostGroupId1).Not.Nullable();
      Map(x => x.LinkedIndustryCostGroupId2).Not.Nullable();
    }
  }
}
