namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class IndustryCostGroupMap : ClassMap<IndustryCostGroup>
  {
    public IndustryCostGroupMap()
    {
      Table("IndustryCostGroup");
      Id(x => x.Id).GeneratedBy.Identity();
      Map(x => x.GroupName).Not.Nullable();
      Map(x => x.FinancialYear);
      Map(x => x.LinkedTable);
      Map(x => x.LinkedId);
      Map(x => x.Active).Not.Nullable();
      HasMany(x => x.Items).KeyColumn("IndustryCostGroupId").Cascade.Delete().Inverse();
    }
  }
}
