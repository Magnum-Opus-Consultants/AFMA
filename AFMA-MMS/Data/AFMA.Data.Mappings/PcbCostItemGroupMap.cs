namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class PcbCostItemGroupMap : ClassMap<PcbCostItemGroup>
  {
    public PcbCostItemGroupMap()
    {
      Table("PcbCostItemGroup");
      Id(x => x.Id).GeneratedBy.Identity();
      References(x => x.PcbCostSubGroup, "SubGroupId").Not.Nullable().Cascade.Delete();
      Map(x => x.ItemGroupName).Not.Nullable();
      Map(x => x.ItemGroupCode).Not.Nullable();
      Map(x => x.HeaderName);
      Map(x => x.ItemGroupHelpText);
      Map(x => x.Active).Not.Nullable();
      Map(x => x.DisplayOrder).Not.Nullable();
      HasMany(x => x.PcbCostItems).KeyColumn("ItemGroupId").Cascade.Delete().Inverse();
    }
  }
}
