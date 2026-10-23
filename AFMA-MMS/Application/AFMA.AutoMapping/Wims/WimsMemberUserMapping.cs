namespace AFMA.AutoMapping.Wims
{
  using AFMA.ApplicationLayer.Model.Wims;
  using AFMA.Data.Entities.Wims;
  using AutoMapper;

  public class WimsMemberUserMapping : Profile
  {
    public WimsMemberUserMapping()
    {
      this.CreateMap<MemberUser, MemberUserModel>()
        .ForMember(d => d.MemberId, m => m.MapFrom(s => s.Member.Id))
        .ForMember(d => d.Member, m => m.MapFrom(s => s.Member.MemberLegalName))
        .ForMember(d => d.UserId, m => m.MapFrom(s => s.User.Id))
        .ForMember(d => d.Email, m => m.MapFrom(s => s.User.UserName))
        .ForMember(d => d.LastLogin, m => m.MapFrom(s => s.User.LastLoginDate))
        .ForMember(d => d.IsActive, m => m.MapFrom(s => s.User.IsActive))
        .ForMember(d => d.MemberBranches, m => m.Ignore());

      this.CreateMap<MemberUserModel, MemberUser>()
        .ForMember(d => d.Member, m => m.MapFrom(s => new Member { Id = s.MemberId }))
        .ForMember(d => d.User, m => m.MapFrom(s => new WimsApplicationUser { Id = s.UserId }));

      this.CreateMap<MemberUserBranch, MemberUserBranchModel>()
        .ForMember(d => d.MemberUserId, m => m.MapFrom(s => s.MemberUser.Id))
        .ForMember(d => d.MemberBranch, m => m.MapFrom(s => s.MemberBranch.BranchName));

      this.CreateMap<MemberUserBranchModel, MemberUserBranch>()
        .ForMember(d => d.MemberUser, m => m.MapFrom(s => new MemberUser { Id = s.MemberUserId }))
        .ForMember(d => d.MemberBranch, m => m.MapFrom(s => new MemberBranch { Id = s.MemberBranchId }));

      this.CreateMap<MemberUserDistributionGroup, MemberUserDistributionGroupModel>()
        .ForMember(d => d.MemberUserId, m => m.MapFrom(s => s.MemberUser.Id))
        .ForMember(d => d.MemberUser, m => m.MapFrom(s => s.MemberUser.FirstName))
        .ForMember(d => d.DistributionGroupId, m => m.MapFrom(s => s.DistributionGroup.Id))
        .ForMember(d => d.DistributionGroup, m => m.MapFrom(s => s.DistributionGroup.Name));

      this.CreateMap<MemberUserDistributionGroupModel, MemberUserDistributionGroup>()
        .ForMember(d => d.MemberUser, m => m.MapFrom(s => new MemberUser { Id = s.MemberUserId }))
        .ForMember(d => d.DistributionGroup, m => m.MapFrom(s => new DistributionGroup { Id = s.DistributionGroupId }));
    }
  }
}
