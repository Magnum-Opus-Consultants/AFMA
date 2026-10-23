namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class MaterialCategoryMap : ClassMap<MaterialCategory>
  {
    public MaterialCategoryMap()
    {
      Table("MaterialCategory");
      Id(x => x.Id).GeneratedBy.Identity();
      Map(x => x.Name).Not.Nullable();
      References(x => x.MaterialGroup, "MaterialGroupId").Cascade.None().Not.Nullable();
      Map(x => x.Code);
      Map(x => x.Active).Not.Nullable();
    }
  }
}
