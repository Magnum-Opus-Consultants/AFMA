namespace AFMA.Data.Mappings.Wims
{
  using AFMA.Data.Entities.Wims;
  using FluentNHibernate.Mapping;

  public class MemberBranchServiceMap : ClassMap<MemberBranchService>
  {
    public MemberBranchServiceMap()
    {
      Table("MemberBranchService ");
      Id(x => x.Id).GeneratedBy.Identity();
      References(x => x.Service, "ServiceId").Not.Nullable().Cascade.None();
      References(x => x.MemberBranch, "MemberBranchId").Not.Nullable().Cascade.None();
    }
  }
}
