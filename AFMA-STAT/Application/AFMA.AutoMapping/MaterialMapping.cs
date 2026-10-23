namespace AFMA.AutoMapping
{
  using ApplicationLayer.Model;
  using AutoMapper;
  using Data.Entities;

  public class MaterialMapping : Profile
  {
    public MaterialMapping()
    {
      this.CreateMap<Material, MaterialSelectLookupModel>();
      
      this.CreateMap<MaterialOrigin, MaterialOriginModel>().ReverseMap();
    }
  }
}
