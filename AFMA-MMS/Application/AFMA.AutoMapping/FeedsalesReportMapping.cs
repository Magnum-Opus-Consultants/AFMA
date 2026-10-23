namespace AFMA.AutoMapping
{
  using Alpha.Services.Models.Data.PageData;
  using ApplicationLayer.Model;
  using AutoMapper;
  using Data.Entities;

  public class FeedsalesReportMapping : Profile
  {
    public FeedsalesReportMapping()
    {
      this.CreateMap<FeedsalesReport, FeedsalesReportModel>()
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


      this.CreateMap<FeedsalesReportModel, FeedsalesReport>()
        .ForMember(d => d.Member, m => m.MapFrom(s => new Member { Id = s.MemberId }))
        .ForMember(d => d.MemberBranch, m => m.MapFrom(s => new MemberBranch { Id = s.MemberBranchId }))
        .ForMember(d => d.MemberAccount, m => m.MapFrom(s => s.MemberAccountId == null ? null : new MemberAccount { Id = (int)s.MemberAccountId }));

      this.CreateMap<FeedsalesReportLookup, FeedsalesReportLookupModel>()
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

      this.CreateMap<FeedsalesReportItem, FeedsalesReportItemModel>()
        .ForMember(d => d.FeedsalesReportId, m => m.MapFrom(s => s.FeedsalesReport.Id))
        .ForMember(d => d.ProductId, m => m.MapFrom(s => s.Product.Id))
        .ForMember(d => d.Product, m => m.MapFrom(s => s.Product.Name))
        .ForMember(d => d.ProductCode, m => m.MapFrom(s => s.Product.Code))
        .ForMember(d => d.ProductGroupId, m => m.MapFrom(s => s.Product.ProductGroup.Id))
        .ForMember(d => d.MaterialId, m => m.MapFrom(s => s.Material.Id))
        .ForMember(d => d.Material, m => m.MapFrom(s => s.Material.Name))
        .ForMember(d => d.MaterialCode, m => m.MapFrom(s => s.Material.Code))
        .ForMember(d => d.MemberId, m => m.MapFrom(s => s.Member.Id))
        .ForMember(d => d.Member, m => m.MapFrom(s => s.Member.Name))
        .ForMember(d => d.MemberBranchId, m => m.MapFrom(s => s.MemberBranch.Id))
        .ForMember(d => d.MemberBranch, m => m.MapFrom(s => s.MemberBranch.Name))
        .ForMember(d => d.MemberAccountId, m => m.MapFrom(s => s.MemberAccount.Id))
        .ForMember(d => d.MemberAccount, m => m.MapFrom(s => s.MemberAccount.Id));

      this.CreateMap<FeedsalesReportItem, FeedsalesReportItemLookupModel>()
        .ForMember(d => d.Product, m => m.MapFrom(s => s.Product.Name))
        .ForMember(d => d.ProductGroupId, m => m.MapFrom(s => s.Product.ProductGroup.Id))
        .ForMember(d => d.ProductGroup, m => m.MapFrom(s => s.Product.ProductGroup.Name))
        .ForMember(d => d.MaterialId, m => m.MapFrom(s => s.Material.Id))
        .ForMember(d => d.Material, m => m.MapFrom(s => s.Material.Name))
        .ForMember(d => d.ProductCode, m => m.MapFrom(s => s.Product.Code))
        .ForMember(d => d.MaterialCode, m => m.MapFrom(s => s.Material.Code));

      this.CreateMap<FeedsalesReportItemModel, FeedsalesReportItem>()
        .ForMember(d => d.FeedsalesReport, m => m.MapFrom(s => new FeedsalesReport { Id = s.FeedsalesReportId }))
        .ForMember(d => d.Product,
          m => m.MapFrom(s => s.ProductId == null ? null : new Product { Id = (int)s.ProductId }))
        .ForMember(d => d.Material,
          m => m.MapFrom(s => s.MaterialId == null ? null : new Material { Id = (int)s.MaterialId }))
        .ForMember(d => d.Member, m => m.MapFrom(s => new Member { Id = s.MemberId }))
        .ForMember(d => d.MemberBranch, m => m.MapFrom(s => new MemberBranch { Id = s.MemberBranchId }))
        .ForMember(d => d.MemberAccount, m => m.MapFrom(s => s.MemberAccountId == null ? null : new MemberAccount { Id = (int)s.MemberAccountId }));

      this.CreateMap<AlphaPageResult<FeedsalesReport>, AlphaPageResult<FeedsalesReportLookupModel>>();

      this.CreateMap<FeedsalesReport, FeedsalesReportLookupModel>()
        .ForMember(d => d.MemberId, m => m.MapFrom(s => s.Member.Id))
        .ForMember(d => d.Member, m => m.MapFrom(s => s.Member.Name))
        .ForMember(d => d.MemberBranchId, m => m.MapFrom(s => s.MemberBranch.Id))
        .ForMember(d => d.MemberBranch, m => m.MapFrom(s => s.MemberBranch.Name))
        .ForMember(d => d.MemberAccountId, m => m.MapFrom(s => s.MemberAccount.Id))
        .ForMember(d => d.MemberAccount, m => m.Ignore());

      this.CreateMap<FeedsalesReportItemLookup, FeedsalesReportItemLookupModel>()
        .ForMember(d => d.FeedsalesReportId, m => m.MapFrom(s => s.FeedsalesReport.Id))
        .ForMember(d => d.ProductId, m => m.MapFrom(s => s.Product.Id))
        .ForMember(d => d.Product, m => m.MapFrom(s => s.Product.Name))
        .ForMember(d => d.ProductCode, m => m.MapFrom(s => s.Product.Code))
        .ForMember(d => d.ProductGroupId, m => m.MapFrom(s => s.Product.ProductGroup.Id))
        .ForMember(d => d.MaterialId, m => m.MapFrom(s => s.Material.Id))
        .ForMember(d => d.Material, m => m.MapFrom(s => s.Material.Name))
        .ForMember(d => d.MaterialCode, m => m.MapFrom(s => s.Material.Code))
        .ForMember(d => d.ReportYear, m => m.MapFrom(s => s.FeedsalesReport.ReportYear))
        .ForMember(d => d.ReportMonth, m => m.MapFrom(s => s.FeedsalesReport.ReportMonth))
        .ForMember(d => d.IsSubmitted, m => m.MapFrom(s => s.FeedsalesReport.IsSubmitted));

      this.CreateMap<FeedsalesReportWithItemsLookup, FeedsalesReportWithItemsLookupModel>()
        .ForMember(d => d.MemberId, m => m.MapFrom(s => s.Member.Id))
        .ForMember(d => d.Member, m => m.MapFrom(s => s.Member.Name))
        .ForMember(d => d.MemberBranchId, m => m.MapFrom(s => s.MemberBranch.Id))
        .ForMember(d => d.MemberBranch, m => m.MapFrom(s => s.MemberBranch.Name))
        .ForMember(d => d.MemberAccountId, m => m.MapFrom(s => s.MemberAccount.Id))
        .ForMember(d => d.MemberAccount, m => m.Ignore());
    }
  }
}
