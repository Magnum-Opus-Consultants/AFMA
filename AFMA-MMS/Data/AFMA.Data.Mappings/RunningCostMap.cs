namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class RunningCostMap : ClassMap<RunningCost>
  {
    public RunningCostMap()
    {
      Table("RunningCost");
      Id(x => x.Id).GeneratedBy.Identity();
      References(x => x.RunningCostGroup, "RunningCostGroupId").Not.Nullable().Cascade.Delete();
      Map(x => x.ItemCode).Not.Nullable();
      Map(x => x.ItemName).Not.Nullable();
      Map(x => x.Description);
      Map(x => x.TotalCost).Not.Nullable();
      Map(x => x.ProductionEnable).Not.Nullable();
      Map(x => x.SalesEnable).Not.Nullable();
      Map(x => x.AdministrationEnable).Not.Nullable();
      Map(x => x.RoadTransportEnable).Not.Nullable();
      Map(x => x.DisplayOrder).Not.Nullable();
      Map(x => x.ExcelRow).Not.Nullable();
      Map(x => x.ExcelSheetName);
      Map(x => x.ExcelSheetId);
      Map(x => x.Active).Not.Nullable();
      Map(x => x.ExcelColumnProduction).Not.Nullable();
      Map(x => x.ExcelColumnAdministration).Not.Nullable();
      Map(x => x.ExcelColumnSales).Not.Nullable();
      Map(x => x.ExcelColumnRoadTransport).Not.Nullable();
    }
  }
}
