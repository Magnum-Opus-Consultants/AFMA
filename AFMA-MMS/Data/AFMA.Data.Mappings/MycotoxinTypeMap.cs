namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class MycotoxinTypeMap : ClassMap<MycotoxinType>
  {
    public MycotoxinTypeMap()
    {
      Table("MycotoxinType");
      Id(x => x.Id).GeneratedBy.Identity();
      References(x => x.MycotoxinClass, "MycotoxinClassId").Not.Nullable().Cascade.None();
      Map(x => x.Name).Not.Nullable();
      Map(x => x.Active).Not.Nullable();
    }
  }
}
