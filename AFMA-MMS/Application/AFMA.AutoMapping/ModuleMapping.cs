namespace AFMA.AutoMapping
{
  using ApplicationLayer.Model;
  using AutoMapper;
  using Data.Entities;

  public class ModuleMapping : Profile
  {
    public ModuleMapping()
    {
      this.CreateMap<Module, ModuleModel>().ReverseMap();
    }
  }
}
