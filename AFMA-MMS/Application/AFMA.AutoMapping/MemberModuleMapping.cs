namespace AFMA.AutoMapping
{
  using ApplicationLayer.Model;
  using AutoMapper;
  using Data.Entities;

  public class MemberModuleMapping : Profile
  {
    public MemberModuleMapping()
    {
      this.CreateMap<MemberModule, MemberModuleModel>()
        .ForMember(d => d.MemberId, m => m.MapFrom(s => s.Member!.Id))
        .ForMember(d => d.ModuleId, m => m.MapFrom(s => s.Module!.Id))
        .ForMember(d => d.Module, m => m.MapFrom(s => s.Module!.Description));

      this.CreateMap<MemberModuleModel, MemberModule>()
        .ForMember(d => d.Member, m => m.MapFrom(s => new MemberAccount { Id = s.MemberId }))
        .ForMember(d => d.Module, m => m.MapFrom(s => new Module { Id = s.ModuleId }));
    }
  }
}