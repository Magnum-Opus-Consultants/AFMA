namespace AFMA.AutoMapping
{
  using ApplicationLayer.Model;
  using AutoMapper;
  using Data.Entities;

  public class FeedsalesMapping : Profile
  {
    public FeedsalesMapping()
    {
      this.CreateMap<FeedsalesReportGroupCategory, FeedsalesReportGroupCategoryModel>().ReverseMap();

      this.CreateMap<FeedsalesReportGroup, FeedsalesReportGroupModel>()
        .ForMember(d => d.FeedsalesReportGroupCategoryId, m => m.MapFrom(s => s.FeedsalesReportGroupCategory.Id))
        .ForMember(d => d.FeedsalesReportGroupCategory, m => m.MapFrom(s => s.FeedsalesReportGroupCategory.Name));

      this.CreateMap<FeedsalesReportGroupModel, FeedsalesReportGroup>()
        .ForMember(d => d.FeedsalesReportGroupCategory,
          m => m.MapFrom(s =>
            s.FeedsalesReportGroupCategoryId == null ? null : new FeedsalesReportGroupCategory { Id = (int)s.FeedsalesReportGroupCategoryId }));
    }
  }
}
