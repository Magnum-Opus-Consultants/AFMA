namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  /// <summary>
  /// The member mapping class.
  /// </summary>
  public class MemberAccountMap : ClassMap<MemberAccount>
  {
    public MemberAccountMap()
    {
      Table("MemberAccount");
      Id(x => x.Id).GeneratedBy.Identity();
      References(x => x.Member, "MemberId").Cascade.None();
      References(x => x.MemberBranch, "MemberBranchId").Not.Nullable().Cascade.None();
      References(x => x.ApplicationUser, "UserId").Cascade.None();
      Map(x => x.Active).Not.Nullable();
      HasMany(x => x.Modules).KeyColumn("MemberAccountId").Cascade.None().Inverse();
    }
  }
}
