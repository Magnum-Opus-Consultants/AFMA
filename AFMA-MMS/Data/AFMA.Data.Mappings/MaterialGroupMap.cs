namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class MaterialGroupMap : ClassMap<MaterialGroup>
  {
    public MaterialGroupMap()
    {
      Table("MaterialGroup");
      Id(x => x.Id).GeneratedBy.Identity();
      Map(x => x.Name).Not.Nullable();
      Map(x => x.Code);
      Map(x => x.Active).Not.Nullable();
    }
  }
}
