namespace AFMA.AutoMapping
{
  using ApplicationLayer.Model;
  using AutoMapper;
  using Data.Entities;

  public class LaboratoryMapping : Profile
  {
    public LaboratoryMapping()
    {
      this.CreateMap<Laboratory, LaboratoryModel>().ReverseMap();
      this.CreateMap<Laboratory, LaboratorySelectLookupModel>();
    }
  }
}
