namespace AFMA.AutoMapping.Wims
{
  using AFMA.ApplicationLayer.Model.Wims;
  using AFMA.Data.Entities.Wims;
  using AutoMapper;

  public class WimsMemberMapping : Profile
  {
    public WimsMemberMapping()
    {
      this.CreateMap<Member, MemberModel>()
        .ForMember(d => d.MemberTypeId, m => m.MapFrom(s => s.MemberType.Id))
        .ForMember(d => d.MemberType, m => m.MapFrom(s => s.MemberType.Description))
        .ForMember(d => d.ProvinceId, m => m.MapFrom(s => s.Province.Id))
        .ForMember(d => d.Province, m => m.MapFrom(s => s.Province.Description));

      this.CreateMap<MemberModel, Member>()
        .ForMember(d => d.MemberType, m => m.MapFrom(s => new MemberType { Id = s.MemberTypeId }))
        .ForMember(d => d.Province, m => m.MapFrom(s => new Province { Id = s.ProvinceId }));

      this.CreateMap<MemberBranch, MemberBranchModel>()
        .ForMember(d => d.MemberId, m => m.MapFrom(s => s.Member.Id))
        .ForMember(d => d.Member, m => m.MapFrom(s => s.Member.MemberTradeName))
        .ForMember(d => d.ProvinceId, m => m.MapFrom(s => s.Province.Id))
        .ForMember(d => d.Province, m => m.MapFrom(s => s.Province.Description));

      this.CreateMap<MemberBranchModel, MemberBranch>()
        .ForMember(d => d.Member, m => m.MapFrom(s => new Member { Id = s.MemberId }))
        .ForMember(d => d.Province, m => m.MapFrom(s => new Province { Id = s.ProvinceId }));

      this.CreateMap<MemberBranchContact, MemberBranchContactModel>()
        .ForMember(d => d.MemberId, m => m.MapFrom(s => s.Member.Id))
        .ForMember(d => d.Member, m => m.MapFrom(s => s.Member.MemberTradeName))
        .ForMember(d => d.MemberBranchId, m => m.MapFrom(s => s.MemberBranch.Id))
        .ForMember(d => d.MemberBranch, m => m.MapFrom(s => s.MemberBranch.BranchName))
        .ForMember(d => d.MemberContactTypeId, m => m.MapFrom(s => s.MemberContactType.Id))
        .ForMember(d => d.MemberContactType, m => m.MapFrom(s => s.MemberContactType.Description));

      this.CreateMap<MemberBranchContactModel, MemberBranchContact>()
        .ForMember(d => d.Member, m => m.MapFrom(s => new Member { Id = s.MemberId }))
        .ForMember(d => d.MemberBranch, m => m.MapFrom(s => new MemberBranch { Id = s.MemberBranchId }))
        .ForMember(d => d.MemberContactType, m => m.MapFrom(s => new MemberContactType { Id = s.MemberContactTypeId }));

      this.CreateMap<MemberBranchAnimalFeedProduct, MemberBranchAnimalFeedProductModel>()
        .ForMember(d => d.AnimalFeedProductId, m => m.MapFrom(s => s.AnimalFeedProduct.Id))
        .ForMember(d => d.AnimalFeedProduct, m => m.MapFrom(s => s.AnimalFeedProduct.Description))
        .ForMember(d => d.MemberBranchId, m => m.MapFrom(s => s.MemberBranch.Id))
        .ForMember(d => d.MemberBranch, m => m.MapFrom(s => s.MemberBranch.BranchName));

      this.CreateMap<MemberBranchAnimalFeedProductModel, MemberBranchAnimalFeedProduct>()
        .ForMember(d => d.AnimalFeedProduct, m => m.MapFrom(s => new AnimalFeedProduct { Id = s.AnimalFeedProductId }))
        .ForMember(d => d.MemberBranch, m => m.MapFrom(s => new MemberBranch { Id = s.MemberBranchId }));

      this.CreateMap<MemberBranchServiceProvider, MemberBranchServiceProviderModel>()
        .ForMember(d => d.ServiceProviderId, m => m.MapFrom(s => s.ServiceProvider.Id))
        .ForMember(d => d.ServiceProvider, m => m.MapFrom(s => s.ServiceProvider.Description))
        .ForMember(d => d.MemberBranchId, m => m.MapFrom(s => s.MemberBranch.Id))
        .ForMember(d => d.MemberBranch, m => m.MapFrom(s => s.MemberBranch.BranchName));

      this.CreateMap<MemberBranchServiceProviderModel, MemberBranchServiceProvider>()
        .ForMember(d => d.ServiceProvider, m => m.MapFrom(s => new ServiceProvider { Id = s.ServiceProviderId }))
        .ForMember(d => d.MemberBranch, m => m.MapFrom(s => new MemberBranch { Id = s.MemberBranchId }));

      //this.CreateMap<MemberSummary, MemberSummaryModel>()
      //  .ForMember(d => d.MemberTypeId, m => m.MapFrom(s => s.MemberType.Id))
      //  .ForMember(d => d.MemberType, m => m.MapFrom(s => s.MemberType.Description));
    }
  }
}
