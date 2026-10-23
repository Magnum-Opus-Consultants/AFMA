namespace AFMA.AutoMapping
{
  using ApplicationLayer.Model;
  using Data.Entities;
  using AutoMapper;

  public class MemberMapping : Profile
  {
    public MemberMapping()
    {
      this.CreateMap<Member, MemberModel>()
        .ForMember(d => d.MemberTypeId, m => m.MapFrom(s => s.MemberType.Id))
        .ForMember(d => d.MemberType, m => m.MapFrom(s => s.MemberType.Description));

      this.CreateMap<MemberModel, Member>()
        .ForMember(d => d.MemberType, m => m.MapFrom(s => new MemberType { Id = s.MemberTypeId }));

      this.CreateMap<MemberSummary, MemberSummaryModel>()
        .ForMember(d => d.MemberTypeId, m => m.MapFrom(s => s.MemberType.Id))
        .ForMember(d => d.MemberType, m => m.MapFrom(s => s.MemberType.Description));
    }
  }
}
