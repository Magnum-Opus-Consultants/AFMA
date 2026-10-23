namespace AFMA.AutoMapping.Wims
{
  using AFMA.ApplicationLayer.Model.Wims;
  using AFMA.Data.Entities.Wims;
  using AutoMapper;

  public class DistributionGroupMapping : Profile
  {
    public DistributionGroupMapping()
    {
      this.CreateMap<DistributionGroup, DistributionGroupModel>().ReverseMap();

      this.CreateMap<DistributionGroupMember, DistributionGroupMemberModel>()
        .ForMember(d => d.DistributionGroupId, m => m.MapFrom(s => s.DistributionGroup.Id))
        .ForMember(d => d.DistributionGroup, m => m.MapFrom(s => s.DistributionGroup.Name))
        .ForMember(d => d.MemberBranchContactId, m => m.MapFrom(s => s.MemberBranchContact.Id))
        .ForMember(d => d.MemberBranchContact,
          m => m.MapFrom(s => $"{s.MemberBranchContact.FirstName} {s.MemberBranchContact.Surname}"));

      this.CreateMap<DistributionGroupMemberModel, DistributionGroupMember>()
        .ForMember(d => d.DistributionGroup, m => m.MapFrom(s => new DistributionGroup { Id = s.DistributionGroupId }))
        .ForMember(d => d.MemberBranchContact, m => m.MapFrom(s => s.MemberBranchContactId != null ? new MemberBranchContact { Id = s.MemberBranchContactId.Value } : null));
    }
  }
}
