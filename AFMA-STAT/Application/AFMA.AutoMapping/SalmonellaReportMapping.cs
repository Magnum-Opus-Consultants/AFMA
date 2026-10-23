namespace AFMA.AutoMapping
{
  using Alpha.Services.Models.Data.PageData;
  using ApplicationLayer.Model;
  using AutoMapper;
  using Data.Entities;

  public class SalmonellaReportMapping : Profile
  {
    public SalmonellaReportMapping()
    {
      this.CreateMap<SalmonellaReport, SalmonellaReportModel>()
        .ForMember(d => d.LaboratoryId, m => m.MapFrom(s => s.Laboratory.Id))
        .ForMember(d => d.Laboratory, m => m.MapFrom(s => s.Laboratory.Name))
        .ForMember(d => d.MemberId, m => m.MapFrom(s => s.Member.Id))
        .ForMember(d => d.Member, m => m.MapFrom(s => s.Member.Name))
        .ForMember(d => d.MemberBranchId, m => m.MapFrom(s => s.MemberBranch.Id))
        .ForMember(d => d.MemberBranch, m => m.MapFrom(s => s.MemberBranch.Name))
        .ForMember(d => d.MemberAccountId, m => m.MapFrom(s => s.MemberAccount.Id))
        .ForMember(d => d.MemberAccount, m => m.MapFrom(s => s.MemberAccount.ApplicationUser.FullName));

      this.CreateMap<SalmonellaReportModel, SalmonellaReport>()
        .ForMember(d => d.Laboratory, m => m.MapFrom(s => s.LaboratoryId == null ? null : new Laboratory { Id = (int)s.LaboratoryId }))
        .ForMember(d => d.Member, m => m.MapFrom(s => new Member { Id = s.MemberId }))
        .ForMember(d => d.MemberBranch, m => m.MapFrom(s => new MemberBranch { Id = s.MemberBranchId }))
        .ForMember(d => d.MemberAccount, m => m.MapFrom(s => s.MemberAccountId == null ? null : new MemberAccount { Id = (int)s.MemberAccountId }));

      this.CreateMap<SalmonellaReportLookup, SalmonellaReportLookupModel>()
        .ForMember(d => d.LaboratoryId, m => m.MapFrom(s => s.Laboratory.Id))
        .ForMember(d => d.Laboratory, m => m.MapFrom(s => s.Laboratory.Name))
        .ForMember(d => d.MemberId, m => m.MapFrom(s => s.Member.Id))
        .ForMember(d => d.Member, m => m.MapFrom(s => s.Member.Name))
        .ForMember(d => d.MemberBranchId, m => m.MapFrom(s => s.MemberBranch.Id))
        .ForMember(d => d.MemberBranch, m => m.MapFrom(s => s.MemberBranch.Name))
        .ForMember(d => d.MemberAccountId, m => m.MapFrom(s => s.MemberAccount.Id))
        .ForMember(d => d.MemberAccount, m => m.MapFrom(s => s.MemberAccount.ApplicationUser.FullName))
        .ForMember(d => d.MemberAccountEmail,
          m => m.MapFrom(s =>
            s.MemberAccount != null && s.MemberAccount.ApplicationUser != null
              ? s.MemberAccount.ApplicationUser.Email
              : string.Empty));

      this.CreateMap<SalmonellaReport, SalmonellaReportLookupModel>()
        .ForMember(d => d.LaboratoryId, m => m.MapFrom(s => s.Laboratory.Id))
        .ForMember(d => d.Laboratory, m => m.MapFrom(s => s.Laboratory.Name))
        .ForMember(d => d.MemberId, m => m.MapFrom(s => s.Member.Id))
        .ForMember(d => d.Member, m => m.MapFrom(s => s.Member.Name))
        .ForMember(d => d.MemberBranchId, m => m.MapFrom(s => s.MemberBranch.Id))
        .ForMember(d => d.MemberBranch, m => m.MapFrom(s => s.MemberBranch.Name))
        .ForMember(d => d.MemberAccountId, m => m.MapFrom(s => s.MemberAccount.Id))
        .ForMember(d => d.MemberAccount, m => m.MapFrom(s => s.MemberAccount.ApplicationUser.FullName))
        .ForMember(d => d.MemberAccountEmail,
          m => m.MapFrom(s =>
            s.MemberAccount != null && s.MemberAccount.ApplicationUser != null
              ? s.MemberAccount.ApplicationUser.Email
              : string.Empty));


      this.CreateMap<AlphaPageResult<SalmonellaReport>, AlphaPageResult<SalmonellaReportLookupModel>>();

      this.CreateMap<SalmonellaSample, SalmonellaSampleModel>()
        .ForMember(d => d.SalmonellaReportId, m => m.MapFrom(s => s.SalmonellaReport.Id))
        .ForMember(d => d.ProductId, m => m.MapFrom(s => s.Product.Id))
        .ForMember(d => d.Product, m => m.MapFrom(s => s.Product.Name))
        .ForMember(d => d.MaterialId, m => m.MapFrom(s => s.Material.Id))
        .ForMember(d => d.Material, m => m.MapFrom(s => s.Material.Name))
        .ForMember(d => d.SalmonellaSampleTypeId, m => m.MapFrom(s => s.SalmonellaSampleType.Id))
        .ForMember(d => d.SalmonellaSampleType, m => m.MapFrom(s => s.SalmonellaSampleType.Name))
        .ForMember(d => d.SalmonellaSampleTypeItemId, m => m.MapFrom(s => s.SalmonellaSampleTypeItem.Id))
        .ForMember(d => d.SalmonellaSampleTypeItem, m => m.MapFrom(s => s.SalmonellaSampleTypeItem.Name))
        .ForMember(d => d.SalmonellaSampleTypeShortCode, m => m.MapFrom(s => s.SalmonellaSampleType.ShortCode))
        .ForMember(d => d.SalmonellaSampleTypeSubItemId, m => m.MapFrom(s => s.SalmonellaSampleTypeSubItem.Id))
        .ForMember(d => d.SalmonellaSampleTypeSubItem, m => m.MapFrom(s => s.SalmonellaSampleTypeSubItem.Name))
        .ForMember(d => d.SalmonellaGroupId, m => m.MapFrom(s => s.SalmonellaGroup.Id))
        .ForMember(d => d.SalmonellaGroup, m => m.MapFrom(s => s.SalmonellaGroup.Name))
        .ForMember(d => d.SalmonellaTypeId, m => m.MapFrom(s => s.SalmonellaType.Id))
        .ForMember(d => d.SalmonellaType, m => m.MapFrom(s => s.SalmonellaType.Name))
        .ForMember(d => d.ProductGroupId, m => m.MapFrom(s => s.ProductGroup.Id))
        .ForMember(d => d.ProductGroup, m => m.MapFrom(s => s.ProductGroup.Name))
        .ForMember(d => d.IsSubmitted, m => m.MapFrom(s => s.SalmonellaReport.IsSubmitted));

      this.CreateMap<SalmonellaSample, SalmonellaSampleLookupModel>()
        .ForMember(d => d.SalmonellaSampleType, m => m.MapFrom(s => s.SalmonellaSampleType.Name))
        .ForMember(d => d.SalmonellaSampleTypeItem, m => m.MapFrom(s => s.SalmonellaSampleTypeItem.Name))
        .ForMember(d => d.SalmonellaSampleTypeSubItem, m => m.MapFrom(s => s.SalmonellaSampleTypeSubItem.Name))
        .ForMember(d => d.SalmonellaGroup, m => m.MapFrom(s => s.SalmonellaGroup.Name))
        .ForMember(d => d.SalmonellaType, m => m.MapFrom(s => s.SalmonellaType.Name));

      this.CreateMap<SalmonellaSampleModel, SalmonellaSample>()
        .ForMember(d => d.SalmonellaReport, m => m.MapFrom(s => new SalmonellaReport { Id = s.SalmonellaReportId }))
        .ForMember(d => d.SalmonellaSampleType,
          m => m.MapFrom(s => new SalmonellaSampleType { Id = s.SalmonellaSampleTypeId }))
        .ForMember(d => d.Product,
          m => m.MapFrom(s => s.ProductId == null ? null : new Product { Id = (int)s.ProductId }))
        .ForMember(d => d.Material,
          m => m.MapFrom(s => s.MaterialId == null ? null : new Material { Id = (int)s.MaterialId }))
        .ForMember(d => d.SalmonellaSampleTypeItem,
          m => m.MapFrom(s =>
            s.SalmonellaSampleTypeItemId == null
              ? null
              : new SalmonellaSampleTypeItem { Id = (int)s.SalmonellaSampleTypeItemId }))
        .ForMember(d => d.SalmonellaSampleTypeSubItem,
          m => m.MapFrom(s =>
            s.SalmonellaSampleTypeSubItemId == null
              ? null
              : new SalmonellaSampleTypeSubItem { Id = (int)s.SalmonellaSampleTypeSubItemId }))
        .ForMember(d => d.SalmonellaGroup,
          m => m.MapFrom(
            s => s.SalmonellaGroupId == null ? null : new SalmonellaGroup { Id = (int)s.SalmonellaGroupId }))
        .ForMember(d => d.SalmonellaType,
          m => m.MapFrom(s => s.SalmonellaTypeId == null ? null : new SalmonellaType { Id = (int)s.SalmonellaTypeId }))
        .ForMember(d => d.ProductGroup,
          m => m.MapFrom(s => s.ProductGroupId == null ? null : new ProductGroup { Id = (int)s.ProductGroupId }));
    }
  }
}
