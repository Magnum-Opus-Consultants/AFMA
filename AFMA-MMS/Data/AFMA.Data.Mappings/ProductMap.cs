namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class ProductMap : ClassMap<Product>
  {
    public ProductMap()
    {
      Table("Product");
      Id(x => x.Id).GeneratedBy.Identity();
      Map(x => x.Name).Not.Nullable();
      References(x => x.ProductGroup, "ProductGroupId").Cascade.None();
      Map(x => x.Code);
      Map(x => x.DisplayOrder).Not.Nullable();
      Map(x => x.Active).Not.Nullable();
      Map(x => x.VisibleToMembers).Not.Nullable();
      Map(x => x.Other).Not.Nullable();
      Map(x => x.AllowDuplicates).Not.Nullable();
    }
  }
}
