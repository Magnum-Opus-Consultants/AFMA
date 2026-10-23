namespace AFMA.AutoMapping
{
  using ApplicationLayer.Model;
  using AutoMapper;
  using Data.Entities;

  public class ProvinceMapping : Profile
  {
    public ProvinceMapping()
    {
      this.CreateMap<Province, ProvinceModel>().ReverseMap();
    }
  }
}
