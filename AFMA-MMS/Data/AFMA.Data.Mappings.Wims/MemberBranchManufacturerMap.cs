namespace AFMA.Data.Mappings.Wims
{
  using AFMA.Data.Entities.Wims;
  using FluentNHibernate.Mapping;

  public class MemberBranchManufacturerMap : ClassMap<MemberBranchManufacturer>
  {
    public MemberBranchManufacturerMap()
    {
      Table("MemberBranchManufacturer");
      Id(x => x.Id).GeneratedBy.Identity();
      References(x => x.Manufacturer, "ManufacturerId").Not.Nullable().Cascade.None();
      References(x => x.MemberBranch, "MemberBranchId").Not.Nullable().Cascade.None();
    }
  }
}
