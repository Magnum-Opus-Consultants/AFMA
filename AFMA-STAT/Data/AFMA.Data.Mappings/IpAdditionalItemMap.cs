namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class IpAdditionalItemMap : ClassMap<IpAdditionalItem>
  {
    public IpAdditionalItemMap()
    {
      Table("IpAdditionalItem");
      Id(x => x.Id).GeneratedBy.Identity();
      References(x => x.IpAdditionalGroup, "IpAdditionalGroupId").Not.Nullable().Cascade.Delete();
      Map(x => x.ReplacementValue).Not.Nullable();
      Map(x => x.Quantity).Not.Nullable();
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
