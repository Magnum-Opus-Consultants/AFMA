namespace AFMA.Data.Mappings.Wims
{
  using AFMA.Data.Entities.Wims;
  using FluentNHibernate.Mapping;

  public class MemberBranchServiceProviderMap : ClassMap<MemberBranchServiceProvider>
  {
    public MemberBranchServiceProviderMap()
    {
      Table("MemberBranchServiceProvider");
      Id(x => x.Id).GeneratedBy.Identity();
      References(x => x.ServiceProvider, "ServiceProviderId").Not.Nullable().Cascade.None();
      References(x => x.MemberBranch, "MemberBranchId").Not.Nullable().Cascade.None();
    }
  }
}
