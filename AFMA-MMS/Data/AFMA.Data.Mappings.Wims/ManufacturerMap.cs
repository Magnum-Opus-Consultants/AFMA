namespace AFMA.Data.Mappings.Wims
{
  using AFMA.Data.Entities.Wims;
  using FluentNHibernate.Mapping;

  public class ManufacturerMap : ClassMap<Manufacturer>
  {
    public ManufacturerMap()
    {
      Table("Manufacturer");
      Id(x => x.Id).GeneratedBy.Identity();
      Map(x => x.Description).Not.Nullable();
    }
  }
}
