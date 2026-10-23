namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class SalmonellaProductGroupCategoryMap : ClassMap<SalmonellaProductGroupCategory>
  {
    public SalmonellaProductGroupCategoryMap()
    {
      Table("SalmonellaProductGroupCategory");
      Id(x => x.Id).GeneratedBy.Identity();
      Map(x => x.Name).Not.Nullable();
      Map(x => x.Active).Not.Nullable();
    }
  }
}
