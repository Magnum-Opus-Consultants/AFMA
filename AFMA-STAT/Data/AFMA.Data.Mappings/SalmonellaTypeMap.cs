namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class SalmonellaTypeMap : ClassMap<SalmonellaType>
  {
    public SalmonellaTypeMap()
    {
      Table("SalmonellaType");
      Id(x => x.Id).GeneratedBy.Identity();
      Map(x => x.Name).Not.Nullable();
      Map(x => x.DisplayOrder).Not.Nullable();
      Map(x => x.Active).Not.Nullable();
      References(x => x.SalmonellaGroup, "SalmonellaGroupId").Cascade.None();
    }
  }
}
