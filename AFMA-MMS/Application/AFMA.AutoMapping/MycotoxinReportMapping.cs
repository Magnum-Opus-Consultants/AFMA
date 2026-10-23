namespace AFMA.AutoMapping
{
  using System.Xml.Schema;
  using AFMA.ApplicationLayer.Model;
  using AFMA.Data.Entities;
  using AutoMapper;

  public class MycotoxinReportMapping : Profile
  {
    public MycotoxinReportMapping()
    {
      this.CreateMap<MycotoxinReport, MycotoxinReportModel>()
        .ForMember(d => d.MemberId, m => m.MapFrom(s => s.Member.Id))
        .ForMember(d => d.Member, m => m.MapFrom(s => s.Member.Name))
        .ForMember(d => d.MemberBranchId, m => m.MapFrom(s => s.MemberBranch.Id))
        .ForMember(d => d.MemberBranch, m => m.MapFrom(s => s.MemberBranch.Name))
        .ForMember(d => d.MemberAccountId, m => m.MapFrom(s => s.MemberAccount.Id))
        .ForMember(d => d.MemberAccount, m => m.MapFrom(s => $"{s.MemberAccount.ApplicationUser.FirstName} {s.MemberAccount.ApplicationUser.LastName}"))
        .ForMember(d => d.LaboratoryId, m => m.MapFrom(s => s.Laboratory.Id))
        .ForMember(d => d.Laboratory, m => m.MapFrom(s => s.Laboratory.Name));


      this.CreateMap<MycotoxinReportModel, MycotoxinReport>()
        .ForMember(d => d.Member, m => m.MapFrom(s => new Member { Id = s.MemberId }))
        .ForMember(d => d.MemberBranch, m => m.MapFrom(s => new MemberBranch { Id = s.MemberBranchId }))
        .ForMember(d => d.MemberAccount, m => m.MapFrom(s => s.MemberAccountId == null ? null : new MemberAccount { Id = (int)s.MemberAccountId }))
        .ForMember(d => d.Laboratory,
          m => m.MapFrom(s => s.LaboratoryId == null ? null : new Laboratory { Id = (int)s.LaboratoryId }));


      this.CreateMap<MycotoxinReportLookup, MycotoxinReportLookupModel>()
        .ForMember(d => d.MemberId, m => m.MapFrom(s => s.Member.Id))
        .ForMember(d => d.Member, m => m.MapFrom(s => s.Member.Name))
        .ForMember(d => d.MemberBranchId, m => m.MapFrom(s => s.MemberBranch.Id))
        .ForMember(d => d.MemberBranch, m => m.MapFrom(s => s.MemberBranch.Name))
        .ForMember(d => d.MemberAccountId, m => m.MapFrom(s => s.MemberAccount.Id))
        .ForMember(d => d.MemberAccount,
          m => m.MapFrom(s =>
            $"{s.MemberAccount.ApplicationUser.FirstName} {s.MemberAccount.ApplicationUser.LastName}"));

      this.CreateMap<MycotoxinSample, MycotoxinSampleModel>()
        .ForMember(d => d.MycotoxinReportId, m => m.MapFrom(s => s.MycotoxinReport.Id))
        .ForMember(d => d.MycotoxinSampleTypeId, m => m.MapFrom(s => s.MycotoxinSampleType.Id))
        .ForMember(d => d.MycotoxinSampleType, m => m.MapFrom(s => s.MycotoxinSampleType.Name))
        .ForMember(d => d.ProductId, m => m.MapFrom(s => s.Product.Id))
        .ForMember(d => d.Product, m => m.MapFrom(s => s.Product.Name))
        .ForMember(d => d.MaterialId, m => m.MapFrom(s => s.Material.Id))
        .ForMember(d => d.Material, m => m.MapFrom(s => s.Material.Name))
        .ForMember(d => d.MaterialOriginId, m => m.MapFrom(s => s.MaterialOrigin.Id))
        .ForMember(d => d.MaterialOrigin, m => m.MapFrom(s => s.MaterialOrigin.Name))
        .ForMember(d => d.ProvinceId, m => m.MapFrom(s => s.Province.Id))
        .ForMember(d => d.Province, m => m.MapFrom(s => s.Province.Description))
        .ForMember(d => d.MycotoxinAnalysisId, m => m.MapFrom(s => s.MycotoxinAnalysis.Id))
        .ForMember(d => d.MycotoxinAnalysis, m => m.MapFrom(s => s.MycotoxinAnalysis.Name));

      this.CreateMap<MycotoxinSampleModel, MycotoxinSample>()
        .ForMember(d => d.MycotoxinReport, m => m.MapFrom(s => new MycotoxinReport { Id = s.MycotoxinReportId }))
        .ForMember(d => d.MycotoxinSampleType,
          m => m.MapFrom(s => new MycotoxinSampleType { Id = s.MycotoxinSampleTypeId }))
        .ForMember(d => d.Product,
          m => m.MapFrom(s => s.ProductId == null ? null : new Product { Id = s.ProductId.Value }))
        .ForMember(d => d.Material,
          m => m.MapFrom(s => s.MaterialId == null ? null : new Material { Id = s.MaterialId.Value }))
        .ForMember(d => d.MaterialOrigin,
          m => m.MapFrom(s => s.MaterialOriginId == null ? null : new MaterialOrigin { Id = s.MaterialOriginId.Value }))
        .ForMember(d => d.Province,
          m => m.MapFrom(s => s.ProvinceId == null ? null : new Province { Id = s.ProvinceId.Value }))
        .ForMember(d => d.MycotoxinAnalysis, m => m.MapFrom(s => new MycotoxinAnalysis { Id = s.MycotoxinAnalysisId }));

      this.CreateMap<MycotoxinSampleDetection, MycotoxinSampleDetectionModel>()
        .ForMember(d => d.MycotoxinSampleId, m => m.MapFrom(s => s.MycotoxinSample.Id))
        .ForMember(d => d.MycotoxinClassId, m => m.MapFrom(s => s.MycotoxinClass.Id))
        .ForMember(d => d.MycotoxinClass, m => m.MapFrom(s => s.MycotoxinClass.Name))
        .ForMember(d => d.MycotoxinTypeId, m => m.MapFrom(s => s.MycotoxinType.Id))
        .ForMember(d => d.MycotoxinType, m => m.MapFrom(s => s.MycotoxinType.Name));

      this.CreateMap<MycotoxinSampleDetectionModel, MycotoxinSampleDetection>()
        .ForMember(d => d.MycotoxinSample, m => m.MapFrom(s => new MycotoxinSample { Id = s.MycotoxinSampleId }))
        .ForMember(d => d.MycotoxinClass, m => m.MapFrom(s => new MycotoxinClass { Id = s.MycotoxinClassId }))
        .ForMember(d => d.MycotoxinType, m => m.MapFrom(s => new MycotoxinType { Id = s.MycotoxinTypeId }));
    }
  }
}
