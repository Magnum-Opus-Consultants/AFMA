namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  /// <summary>
  /// The member mapping class.
  /// </summary>
  public class MemberMap : ClassMap<Member>
  {
    public MemberMap()
    {
      Table("Member");
      Id(x => x.Id).GeneratedBy.Identity();
      Map(x => x.Name).Not.Nullable();
      Map(x => x.MemberId);
      Map(x => x.Comment);
      Map(x => x.Active).Not.Nullable();
      Map(x => x.CreatedDate).Not.Nullable();
      Map(x => x.UpdatedDate);
      References(x => x.UpdatedUser, "UpdatedUserId").Cascade.None();
      References(x => x.MemberType, "MemberTypeId").Not.Nullable().Cascade.None();
      HasMany(x => x.Branches).KeyColumn("MemberId").Cascade.None().Inverse();
      HasMany(x => x.Accounts).KeyColumn("MemberId").Cascade.None().Inverse();
      HasMany(x => x.Modules).KeyColumn("MemberId").Cascade.None().Inverse();
    }
  }

  /// <summary>
  /// The member summary mapping class.
  /// </summary>
  public class MemberSummaryMap : ClassMap<MemberSummary>
  {
    public MemberSummaryMap()
    {
      Table("Member");
      Id(x => x.Id).GeneratedBy.Identity();
      Map(x => x.Name).Not.Nullable();
      Map(x => x.MemberId);
      Map(x => x.Active).Not.Nullable();
      References(x => x.MemberType, "MemberTypeId").Not.Nullable().Cascade.None();
      HasMany(x => x.Modules).KeyColumn("MemberId").Cascade.None().Inverse();
      HasMany(x => x.Branches).KeyColumn("MemberId").Cascade.None().Inverse();
    }
  }
}
