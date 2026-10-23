namespace AFMA.AutoMapping
{
  using ApplicationLayer.Model;
  using AutoMapper;
  using Data.Entities;

  public class MemberBranchMapping : Profile
  {
    public MemberBranchMapping()
    {
      this.CreateMap<MemberBranch, MemberBranchModel>()
        .ForMember(d => d.MemberId, m => m.MapFrom(s => s.Member.Id))
        .ForMember(d => d.Member, m => m.MapFrom(s => s.Member.Name))
        .ForMember(d => d.ProvinceId, m => m.MapFrom(s => s.Province.Id))
        .ForMember(d => d.Province, m => m.MapFrom(s => s.Province.Description));


      this.CreateMap<MemberBranchModel, MemberBranch>()
        .ForMember(d => d.Member, m => m.MapFrom(s => new Member { Id = s.MemberId }))
        .ForMember(d => d.Province, m => m.MapFrom(s => new Province { Id = s.ProvinceId }));

      this.CreateMap<MemberBranchSummary, MemberBranchSummaryModel>()
        .ForMember(d => d.MemberId, m => m.MapFrom(s => s.Member.Id))
        .ForMember(d => d.Member, m => m.MapFrom(s => s.Member.Name))
        .ForMember(d => d.ProvinceId, m => m.MapFrom(s => s.Province.Id))
        .ForMember(d => d.Province, m => m.MapFrom(s => s.Province.Description));
    }
  }
}
