namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class RunningCostGroupMap : ClassMap<RunningCostGroup>
  {
    public RunningCostGroupMap()
    {
      Table("RunningCostGroup");
      Id(x => x.Id).GeneratedBy.Identity();
      Map(x => x.GroupCode).Not.Nullable();
      Map(x => x.GroupName).Not.Nullable();
      Map(x => x.GroupTotal).Not.Nullable();
      Map(x => x.TotalProduction).Not.Nullable();
      Map(x => x.TotalSales).Not.Nullable();
      Map(x => x.TotalAdministration).Not.Nullable();
      Map(x => x.TotalRoadTransport).Not.Nullable();
      Map(x => x.DisplayOrder).Not.Nullable();
      Map(x => x.Active).Not.Nullable();
      Map(x => x.Note);
      Map(x => x.ProductionEnabled).Not.Nullable();
      Map(x => x.SalesEnabled).Not.Nullable();
      Map(x => x.AdministrationEnabled).Not.Nullable();
      Map(x => x.RoadTransportEnabled).Not.Nullable();
      Map(x => x.GroupHasSubItems).Not.Nullable();
      Map(x => x.ExcelColumnProduction).Not.Nullable();
      Map(x => x.ExcelColumnAdministration).Not.Nullable();
      Map(x => x.ExcelColumnSales).Not.Nullable();
      Map(x => x.ExcelColumnRoadTransport).Not.Nullable();
      Map(x => x.ExcelSheetName);
      Map(x => x.ExcelSheetId);
      Map(x => x.ExcelRow).Not.Nullable();
      Map(x => x.CalculationType).Not.Nullable();
      HasMany(x => x.RunningCosts).KeyColumn("RunningCostGroupId").Cascade.None().Inverse();
    }
  }
}
