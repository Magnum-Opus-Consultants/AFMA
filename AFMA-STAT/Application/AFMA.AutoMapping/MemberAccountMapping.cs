namespace AFMA.AutoMapping
{
  using ApplicationLayer.Model;
  using AutoMapper;
  using Data.Entities;

  public class MemberAccountMapping : Profile
  {
    public MemberAccountMapping()
    {
      this.CreateMap<MemberAccount, MemberAccountModel>()
        .ForMember(d => d.MemberId, m => m.MapFrom(s => s.Member.Id))
        .ForMember(d => d.Member, m => m.MapFrom(s => s.Member.Name))
        .ForMember(d => d.MemberBranchId, m => m.MapFrom(s => s.MemberBranch.Id))
        .ForMember(d => d.MemberBranch, m => m.MapFrom(s => s.MemberBranch.Name))
        .ForMember(d => d.Email, m => m.MapFrom(s => s.ApplicationUser.Email))
        .ForMember(d => d.FirstName, m => m.MapFrom(s => s.ApplicationUser.FirstName))
        .ForMember(d => d.LastName, m => m.MapFrom(s => s.ApplicationUser.LastName))
        .ForMember(d => d.UserId, m => m.MapFrom(s => s.ApplicationUser.Id))
        .ForMember(d => d.UserName, m => m.MapFrom(s => s.ApplicationUser.UserName))
        .ForMember(d => d.FullName, m => m.MapFrom(s => $"{s.ApplicationUser.FirstName} {s.ApplicationUser.LastName}"))
        .ForMember(d => d.Registered, m => m.MapFrom(s => s.ApplicationUser.CreatedDate))
        .ForMember(d => d.LastLogin, m => m.MapFrom(s => s.ApplicationUser.LastLoginDate));

      this.CreateMap<MemberAccountModel, MemberAccount>()
        .ForMember(d => d.Member, m => m.MapFrom(s => new Member { Id = s.MemberId }))
        .ForMember(d => d.MemberBranch, m => m.MapFrom(s => new MemberBranch { Id = s.MemberBranchId }))
        .ForMember(d => d.ApplicationUser, m => m.MapFrom(s => new ApplicationUser { Id = s.UserId }));
    }
  }
}
