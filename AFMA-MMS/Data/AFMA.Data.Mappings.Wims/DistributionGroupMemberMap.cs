namespace AFMA.Data.Mappings.Wims
{
  using AFMA.Data.Entities.Wims;
  using FluentNHibernate.Mapping;

  public class DistributionGroupMemberMap : ClassMap<DistributionGroupMember>
  {
    public DistributionGroupMemberMap()
    {
      Table("DistributionGroupMember");
      Id(x => x.Id).GeneratedBy.Identity();
      References(x => x.DistributionGroup, "DistributionGroupId").Cascade.None();
      References(x => x.MemberBranchContact, "MemberBranchContactId").Cascade.None();
      Map(x => x.ContactName);
      Map(x => x.EmailAddress);
      Map(x => x.ContactNumber);
    }
  }
}
