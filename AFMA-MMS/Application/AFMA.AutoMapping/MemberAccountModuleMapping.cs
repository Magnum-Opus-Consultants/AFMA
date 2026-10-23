namespace AFMA.AutoMapping
{
  using ApplicationLayer.Model;
  using AutoMapper;
  using Data.Entities;

  public class MemberAccountModuleMapping : Profile
  {
    public MemberAccountModuleMapping()
    {
      this.CreateMap<MemberAccountModule, MemberAccountModuleModel>()
        .ForMember(d => d.MemberAccountId, m => m.MapFrom(s => s.MemberAccount.Id))
        .ForMember(d => d.ModuleId, m => m.MapFrom(s => s.Module.Id))
        .ForMember(d => d.Module, m => m.MapFrom(s => s.Module.Description));

      this.CreateMap<MemberAccountModuleModel, MemberAccountModule>()
        .ForMember(d => d.MemberAccount, m => m.MapFrom(s => new MemberAccount { Id = s.MemberAccountId }))
        .ForMember(d => d.Module, m => m.MapFrom(s => new Module { Id = s.ModuleId }));
    }
  }
}