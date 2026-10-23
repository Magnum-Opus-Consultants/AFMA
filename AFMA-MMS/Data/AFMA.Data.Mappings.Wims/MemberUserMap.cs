namespace AFMA.Data.Mappings.Wims
{
  using AFMA.Data.Entities.Wims;
  using FluentNHibernate.Mapping;

  public class MemberUserMap : ClassMap<MemberUser>
  {
    public MemberUserMap()
    {
      Table("MemberUser");
      Id(x => x.Id).GeneratedBy.Identity();
      References(x => x.Member, "MemberId").Not.Nullable().Cascade.None();
      References(x => x.User, "UserId").Not.Nullable().Cascade.None();
      Map(x => x.CanEdit).Not.Nullable();
      Map(x => x.FirstName).Nullable();
      Map(x => x.LastName).Nullable();
      HasMany(x => x.Branches).KeyColumn("MemberUserId").Cascade.Delete().Inverse();
    }
  }
}
