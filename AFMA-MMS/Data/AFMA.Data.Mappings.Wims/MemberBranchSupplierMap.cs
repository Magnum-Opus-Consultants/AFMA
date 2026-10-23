namespace AFMA.Data.Mappings.Wims
{
  using AFMA.Data.Entities.Wims;
  using FluentNHibernate.Mapping;

  public class MemberBranchSupplierMap : ClassMap<MemberBranchSupplier>
  {
    public MemberBranchSupplierMap()
    {
      Table("MemberBranchSupplier");
      Id(x => x.Id).GeneratedBy.Identity();
      References(x => x.Supplier, "SupplierId").Not.Nullable().Cascade.None();
      References(x => x.MemberBranch, "MemberBranchId").Not.Nullable().Cascade.None();
    }
  }
}
