namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class RunningCostSundryExpenseMap : ClassMap<RunningCostSundryExpense>
  {
    public RunningCostSundryExpenseMap()
    {
      Table("RunningCostSundryExpense");
      Id(x => x.Id).GeneratedBy.Identity();
      Map(x => x.ItemName).Not.Nullable();
      Map(x => x.ProductionEnabled).Not.Nullable();
      Map(x => x.MarketingEnabled).Not.Nullable();
      Map(x => x.AdministrationEnabled).Not.Nullable();
      Map(x => x.RoadTransportEnabled).Not.Nullable();
      Map(x => x.Active).Not.Nullable();
      Map(x => x.DisplayOrder).Not.Nullable();
      Map(x => x.ExcelRow).Not.Nullable();
      Map(x => x.ExcelColumnProduction).Not.Nullable();
      Map(x => x.ExcelColumnAdministration).Not.Nullable();
      Map(x => x.ExcelColumnSales).Not.Nullable();
      Map(x => x.ExcelColumnRoadTransport).Not.Nullable();
      Map(x => x.ExcelSheetName);
      Map(x => x.ExcelSheetId);
    }
  }
}
