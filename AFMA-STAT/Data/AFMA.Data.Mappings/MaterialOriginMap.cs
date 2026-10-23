namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class MaterialOriginMap : ClassMap<MaterialOrigin>
  {
    public MaterialOriginMap()
    {
      Table("MaterialOrigin");
      Id(x => x.Id).GeneratedBy.Identity();
      Map(x => x.Name).Not.Nullable();
    }
  }
}
