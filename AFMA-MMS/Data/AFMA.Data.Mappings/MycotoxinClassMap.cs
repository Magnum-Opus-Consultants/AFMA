namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class MycotoxinClassMap : ClassMap<MycotoxinClass>
  {
    public MycotoxinClassMap()
    {
      Table("MycotoxinClass");
      Id(x => x.Id).GeneratedBy.Identity();
      Map(x => x.Name).Not.Nullable();
      Map(x => x.Active).Not.Nullable();
    }
  }
}
