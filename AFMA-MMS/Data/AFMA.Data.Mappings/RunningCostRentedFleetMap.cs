namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class RunningCostRentedFleetMap : ClassMap<RunningCostRentedFleet>
  {
    public RunningCostRentedFleetMap()
    {
      Table("RunningCostRentedFleet");
      Id(x => x.Id).GeneratedBy.Identity();
      Map(x => x.ItemCode).Not.Nullable();
      Map(x => x.ItemName).Not.Nullable();
      Map(x => x.Note);
      Map(x => x.DisplayOrder).Not.Nullable();
      Map(x => x.ExcelSheetName);
      Map(x => x.ExcelSheetId).Not.Nullable();
      Map(x => x.ExcelRow).Not.Nullable();
      Map(x => x.ExcelColumn).Not.Nullable();
      Map(x => x.Active).Not.Nullable();
    }
  }
}
