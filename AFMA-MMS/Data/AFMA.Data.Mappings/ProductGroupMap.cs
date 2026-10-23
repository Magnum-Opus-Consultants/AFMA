namespace AFMA.Data.Mappings
{
  using Entities;
  using FluentNHibernate.Mapping;

  public class ProductGroupMap : ClassMap<ProductGroup>
  {
    public ProductGroupMap()
    {
      Table("ProductGroup");
      Id(x => x.Id).GeneratedBy.Identity();
      Map(x => x.Name).Not.Nullable();
      Map(x => x.Code);
      Map(x => x.DisplayOrder).Not.Nullable();
      Map(x => x.Active).Not.Nullable();
      Map(x => x.HideOnFeedsales).Not.Nullable();
      Map(x => x.HideOnSalmonella).Not.Nullable();
      Map(x => x.VisibleToMembers).Not.Nullable();
      References(x => x.SalmonellaProductGroupCategory, "SalmonellaProductGroupCategoryId").Cascade.None();
      HasMany(x => x.Products).KeyColumn("ProductGroupId").Cascade.None().Inverse();
    }
  }
}
