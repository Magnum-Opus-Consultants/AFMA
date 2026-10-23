namespace AFMA.Data.Mappings.Wims
{
  using AFMA.Data.Entities.Wims;
  using FluentNHibernate.Mapping;

  public class MemberUserBranchMap : ClassMap<MemberUserBranch>
  {
    public MemberUserBranchMap()
    {
      Table("MemberUserBranch");
      Id(x => x.Id).GeneratedBy.Identity();
      References(x => x.MemberUser, "MemberUserId").Not.Nullable().Cascade.None();
      References(x => x.MemberBranch, "MemberBranchId").Not.Nullable().Cascade.None();
      Map(x => x.CanEdit).Not.Nullable();
    }
  }
}
