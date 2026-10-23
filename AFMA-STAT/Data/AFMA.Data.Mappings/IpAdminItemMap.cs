namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class IpAdminItemMap : ClassMap<IpAdminItem>
  {
    public IpAdminItemMap()
    {
      Table("IpAdminItem");
      Id(x => x.Id).GeneratedBy.Identity();
      References(x => x.IpAdminGroup, "IpAdminGroupId").Not.Nullable().Cascade.Delete();
      Map(x => x.CostInRand).Not.Nullable();
      Map(x => x.QuantityOrUnits).Not.Nullable();
      Map(x => x.ItemName).Not.Nullable();
      Map(x => x.Description);
      Map(x => x.DisplayOrder).Not.Nullable();
      Map(x => x.ExcelRow).Not.Nullable();
      Map(x => x.ExcelColumn).Not.Nullable();
      Map(x => x.ExcelSheetName);
      Map(x => x.ExcelSheetId);
      Map(x => x.Active).Not.Nullable();
    }
  }
}
