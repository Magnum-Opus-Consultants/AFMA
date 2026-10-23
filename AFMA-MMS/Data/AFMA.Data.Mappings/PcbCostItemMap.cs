namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class PcbCostItemMap : ClassMap<PcbCostItem>
  {
    public PcbCostItemMap()
    {
      Table("PcbCostItem");
      Id(x => x.Id).GeneratedBy.Identity();
      References(x => x.PcbCostItemGroup, "ItemGroupId").Not.Nullable().Cascade.Delete();
      Map(x => x.ItemCode).Not.Nullable();
      Map(x => x.ItemDescription);
      Map(x => x.Value).Not.Nullable();
      Map(x => x.DisplayOrder).Not.Nullable();
      Map(x => x.Active).Not.Nullable();
      Map(x => x.Editable).Not.Nullable();
      Map(x => x.ExcelSheetName);
      Map(x => x.ExcelSheetId);
      Map(x => x.ExcelRow).Not.Nullable();
      Map(x => x.ExcelColumn).Not.Nullable();
      Map(x => x.AddToTotal).Not.Nullable();
      Map(x => x.ReadOnly).Not.Nullable();
    }
  }
}
