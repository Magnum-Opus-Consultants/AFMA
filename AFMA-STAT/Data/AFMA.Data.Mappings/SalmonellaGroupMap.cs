namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class SalmonellaGroupMap : ClassMap<SalmonellaGroup>
  {
    public SalmonellaGroupMap()
    {
      Table("SalmonellaGroup");
      Id(x => x.Id).GeneratedBy.Identity();
      Map(x => x.Name).Not.Nullable();
      Map(x => x.DisplayOrder).Not.Nullable();
      Map(x => x.Active).Not.Nullable();
    }
  }
}
