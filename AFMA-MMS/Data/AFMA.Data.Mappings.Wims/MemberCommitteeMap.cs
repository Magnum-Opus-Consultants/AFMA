namespace AFMA.Data.Mappings.Wims
{
  using AFMA.Data.Entities.Wims;
  using FluentNHibernate.Mapping;

  /// <summary>
  /// The committee member mapping class.
  /// </summary>
  public class MemberCommitteeMap : ClassMap<MemberCommittee>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="MemberCommitteeMap"/> class.
    /// </summary>
    public MemberCommitteeMap()
    {
      Table("MemberCommittee");
      Id(x => x.Id).GeneratedBy.Identity().Not.Nullable();
      References(x => x.Committee, "CommitteeId").Not.Nullable().Cascade.None();
      References(x => x.Member, "MemberId").Nullable().Cascade.None();
      References(x => x.MemberBranchContact, "MemberBranchContactId").Nullable().Cascade.None();
      Map(x => x.Name).Nullable();
      Map(x => x.MemberName).Nullable();
      Map(x => x.EmailAddress).Nullable();
    }
  }
}
