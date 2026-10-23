namespace AFMA.AutoMapping
{
  using AFMA.ApplicationLayer.Model;
  using AFMA.Data.Entities;
  using AutoMapper;

  public class MycotoxinMapping : Profile
  {
    public MycotoxinMapping()
    {
      this.CreateMap<MycotoxinClass, MycotoxinClassModel>().ReverseMap();

      this.CreateMap<MycotoxinType, MycotoxinTypeModel>()
        .ForMember(d => d.MycotoxinClassId, m => m.MapFrom(s => s.MycotoxinClass.Id))
        .ForMember(d => d.MycotoxinClass, m => m.MapFrom(s => s.MycotoxinClass.Name));

      this.CreateMap<MycotoxinTypeModel, MycotoxinType>()
        .ForMember(d => d.MycotoxinClass, m => m.MapFrom(s => new MycotoxinClass { Id = s.MycotoxinClassId }));

      this.CreateMap<MycotoxinAnalysis, MycotoxinAnalysisModel>().ReverseMap();

      this.CreateMap<MycotoxinSampleType, MycotoxinSampleTypeModel>().ReverseMap();
    }
  }
}
