namespace AFMA.AutoMapping
{
  using ApplicationLayer.Model;
  using AutoMapper;
  using Data.Entities;

  public class SalmonellaMapping : Profile
  {
    public SalmonellaMapping()
    {
      this.CreateMap<SalmonellaProductGroupCategory, SalmonellaProductGroupCategoryModel>().ReverseMap();

      this.CreateMap<SalmonellaReportGroup, SalmonellaReportGroupModel>().ReverseMap();

      this.CreateMap<SalmonellaGroup, SalmonellaGroupModel>().ReverseMap();

      this.CreateMap<SalmonellaSampleType, SalmonellaSampleTypeModel>().ReverseMap();

      this.CreateMap<SalmonellaSampleType, SalmonellaSampleTypeSelectLookupModel>();
      
      this.CreateMap<Month, MonthModel>().ReverseMap();

      this.CreateMap<SalmonellaType, SalmonellaTypeModel>()
        .ForMember(d => d.SalmonellaGroupId, m => m.MapFrom(s => s.SalmonellaGroup.Id))
        .ForMember(d => d.SalmonellaGroup, m => m.MapFrom(s => s.SalmonellaGroup.Name));

      this.CreateMap<SalmonellaTypeModel, SalmonellaType>()
        .ForMember(d => d.SalmonellaGroup, m => m.MapFrom(s => s.SalmonellaGroupId == null ? null : new SalmonellaGroup { Id = (int)s.SalmonellaGroupId }));

      this.CreateMap<SalmonellaSampleTypeItem, SalmonellaSampleTypeItemModel>()
        .ForMember(d => d.SalmonellaSampleTypeId, m => m.MapFrom(s => s.SalmonellaSampleType.Id))
        .ForMember(d => d.SalmonellaSampleType, m => m.MapFrom(s => s.SalmonellaSampleType.Name));

      this.CreateMap<SalmonellaSampleTypeItemModel, SalmonellaSampleTypeItem>()
        .ForMember(d => d.SalmonellaSampleType, m => m.MapFrom(s => new SalmonellaSampleType { Id = s.SalmonellaSampleTypeId }));

      this.CreateMap<SalmonellaSampleTypeSubItem, SalmonellaSampleTypeSubItemModel>()
        .ForMember(d => d.SalmonellaSampleTypeId, m => m.MapFrom(s => s.SalmonellaSampleType.Id))
        .ForMember(d => d.SalmonellaSampleType, m => m.MapFrom(s => s.SalmonellaSampleType.Name));

      this.CreateMap<SalmonellaSampleTypeSubItemModel, SalmonellaSampleTypeSubItem>()
        .ForMember(d => d.SalmonellaSampleType, m => m.MapFrom(s => new SalmonellaSampleType { Id = s.SalmonellaSampleTypeId }));

      this.CreateMap<SalmonellaSampleTypeItem, SalmonellaSampleTypeItemSelectLookupModel>();

      this.CreateMap<SalmonellaSampleTypeSubItem, SalmonellaSampleTypeSubItemSelectLookupModel>();

      this.CreateMap<SalmonellaType, SalmonellaTypeSelectLookupModel>();
    }
  }
}
