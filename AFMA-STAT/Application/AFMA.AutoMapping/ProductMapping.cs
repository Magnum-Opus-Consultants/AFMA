namespace AFMA.AutoMapping
{
  using ApplicationLayer.Model;
  using AutoMapper;
  using Data.Entities;

  public class ProductMapping : Profile
  {
    public ProductMapping()
    {
      this.CreateMap<Product, ProductModel>()
        .ForMember(d => d.ProductGroupId, m => m.MapFrom(s => s.ProductGroup.Id))
        .ForMember(d => d.ProductGroup, m => m.MapFrom(s => s.ProductGroup.Name));

      this.CreateMap<Product, ProductSelectLookupModel>();

      this.CreateMap<ProductModel, Product>()
        .ForMember(d => d.ProductGroup,
          m => m.MapFrom(s => s.ProductGroupId == null ? null : new ProductGroup { Id = (int)s.ProductGroupId }));

      this.CreateMap<ProductGroup, ProductGroupModel>()
        .ForMember(d => d.SalmonellaProductGroupCategoryId, m => m.MapFrom(s => s.SalmonellaProductGroupCategory.Id))
        .ForMember(d => d.SalmonellaProductGroupCategory, m => m.MapFrom(s => s.SalmonellaProductGroupCategory.Name));

      this.CreateMap<ProductGroup, ProductGroupSelectLookupModel>();

      this.CreateMap<ProductGroup, ProductGroupLookupModel>();

      this.CreateMap<ProductGroupModel, ProductGroup>()
        .ForMember(d => d.SalmonellaProductGroupCategory,
          m => m.MapFrom(s =>
            s.SalmonellaProductGroupCategoryId == null
              ? null
              : new SalmonellaProductGroupCategory { Id = (int)s.SalmonellaProductGroupCategoryId }));
    }
  }
}
