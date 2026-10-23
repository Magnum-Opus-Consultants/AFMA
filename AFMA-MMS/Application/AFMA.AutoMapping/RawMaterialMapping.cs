namespace AFMA.AutoMapping
{
  using ApplicationLayer.Model;
  using AutoMapper;
  using Data.Entities;

  public class RawMaterialMapping : Profile
  {
    public RawMaterialMapping()
    {
      this.CreateMap<Material, MaterialModel>()
        .ForMember(d => d.MaterialCategoryId, m => m.MapFrom(s => s.MaterialCategory.Id))
        .ForMember(d => d.MaterialCategory, m => m.MapFrom(s => s.MaterialCategory.Name))
        .ForMember(d => d.SalmonellaSampleTypeId, m => m.MapFrom(s => s.SalmonellaSampleType.Id))
        .ForMember(d => d.SalmonellaSampleType, m => m.MapFrom(s => s.SalmonellaSampleType.Name))
        .ForMember(d => d.SalmonellaReportGroupId, m => m.MapFrom(s => s.SalmonellaReportGroup.Id))
        .ForMember(d => d.SalmonellaReportGroup, m => m.MapFrom(s => s.SalmonellaReportGroup.Name))
        .ForMember(d => d.FeedsalesReportGroupId, m => m.MapFrom(s => s.FeedsalesReportGroup.Id))
        .ForMember(d => d.FeedsalesReportGroup, m => m.MapFrom(s => s.FeedsalesReportGroup.Name));

      this.CreateMap<MaterialModel, Material>()
        .ForMember(d => d.MaterialCategory,
          m => m.MapFrom(s =>
            s.MaterialCategoryId == null ? null : new MaterialCategory { Id = (int)s.MaterialCategoryId }))
        .ForMember(d => d.SalmonellaSampleType,
          m => m.MapFrom(s =>
            s.SalmonellaSampleTypeId == null ? null : new SalmonellaSampleType { Id = (int)s.SalmonellaSampleTypeId }))
        .ForMember(d => d.SalmonellaReportGroup,
          m => m.MapFrom(s =>
            s.SalmonellaReportGroupId == null
              ? null
              : new SalmonellaReportGroup { Id = (int)s.SalmonellaReportGroupId }))
        .ForMember(d => d.FeedsalesReportGroup,
          m => m.MapFrom(s =>
            s.FeedsalesReportGroupId == null ? null : new FeedsalesReportGroup { Id = (int)s.FeedsalesReportGroupId }));
    }
  }
}
