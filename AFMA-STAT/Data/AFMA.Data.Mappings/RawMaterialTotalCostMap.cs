namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class RawMaterialTotalCostMap : ClassMap<RawMaterialTotalCost>
  {
    public RawMaterialTotalCostMap()
    {
      Table("RawMaterialTotalCost");
      Id(x => x.Id).GeneratedBy.Identity();
      Map(x => x.ItemName).Not.Nullable();
      Map(x => x.Active).Not.Nullable();
      Map(x => x.DisplayOrder).Not.Nullable();
      Map(x => x.ExcelRow).Not.Nullable();
      Map(x => x.ExcelColumn).Not.Nullable();
      Map(x => x.ExcelSheetId);
      Map(x => x.ExcelSheetName);
    }
  }
}
