namespace AFMA.Data.Mappings.Wims
{
  using AFMA.Data.Entities.Wims;
  using FluentNHibernate.Mapping;

  public class DistributionGroupMap : ClassMap<DistributionGroup>
  {
    public DistributionGroupMap()
    {
      Table("DistributionGroup");
      Id(x => x.Id).GeneratedBy.Identity();
      Map(x => x.Name).Not.Nullable();
      Map(x => x.Description);
      Map(x => x.Owner).Not.Nullable();
      Map(x => x.FromAddress).Not.Nullable();
      Map(x => x.Private).Not.Nullable();
      HasMany(x => x.DistributionGroupMembers).KeyColumn("DistributionGroupId").Cascade.Delete().Inverse();
    }
  }
}
