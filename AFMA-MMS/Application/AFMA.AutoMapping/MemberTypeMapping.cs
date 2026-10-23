namespace AFMA.AutoMapping
{
  using AFMA.ApplicationLayer.Model;
  using AutoMapper;
  using Data.Entities;

  public class MemberTypeMapping : Profile
  {
    public MemberTypeMapping()
    {
      this.CreateMap<MemberType, MemberTypeModel>().ReverseMap();
    }
  }
}
