namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class MemberBranchMap : ClassMap<MemberBranch>
  {
    public MemberBranchMap()
    {
      Table("MemberBranch");
      Id(x => x.Id).GeneratedBy.Identity();
      References(x => x.Member, "MemberId").Cascade.None().Not.Nullable();
      References(x => x.Province, "ProvinceId").Cascade.None().Not.Nullable();
      Map(x => x.Code);
      Map(x => x.Name).Not.Nullable();
      Map(x => x.Active).Not.Nullable();
      Map(x => x.CreatedDate).Not.Nullable();
    }
  }

  public class MemberBranchSummaryMap : ClassMap<MemberBranchSummary>
  {
    public MemberBranchSummaryMap()
    {
      Table("MemberBranch");
      Id(x => x.Id).GeneratedBy.Identity();
      References(x => x.Member, "MemberId").Cascade.None().Not.Nullable();
      References(x => x.Province, "ProvinceId").Cascade.None().Not.Nullable();
      Map(x => x.Code);
      Map(x => x.Name).Not.Nullable();
      Map(x => x.Active).Not.Nullable();
    }
  }
}
