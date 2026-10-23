namespace AFMA.Data.Mappings.Wims
{
  using AFMA.Data.Entities.Wims;
  using FluentNHibernate.Mapping;

  public class MemberUserDistributionGroupMap : ClassMap<MemberUserDistributionGroup>
  {
    public MemberUserDistributionGroupMap()
    {
      Table("MemberUserDistributionGroup");
      Id(x => x.Id).GeneratedBy.Identity();
      References(x => x.MemberUser, "MemberUserId").Not.Nullable().Cascade.None();
      References(x => x.DistributionGroup, "DistributionGroupId").Not.Nullable().Cascade.None();
      Map(x => x.FirstName).Not.Nullable();
      Map(x => x.LastName).Not.Nullable();
      Map(x => x.EmailAddress).Not.Nullable();
    }
  }
}
