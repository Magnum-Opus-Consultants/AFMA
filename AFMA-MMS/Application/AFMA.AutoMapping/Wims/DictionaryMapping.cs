namespace AFMA.AutoMapping.Wims
{
  using AFMA.ApplicationLayer.Model.Wims;
  using AFMA.Data.Entities.Wims;
  using AutoMapper;

  public class DictionaryMapping : Profile
  {
    public DictionaryMapping()
    {
      this.CreateMap<Province, ProvinceModel>().ReverseMap();
      this.CreateMap<MemberType, MemberTypeModel>().ReverseMap();
      this.CreateMap<Service, ServiceModel>().ReverseMap();
      this.CreateMap<ServiceProvider, ServiceProviderModel>().ReverseMap();
      this.CreateMap<MemberContactType, MemberContactTypeModel>().ReverseMap();
      this.CreateMap<AnimalFeedProduct, AnimalFeedProductModel>().ReverseMap();
      this.CreateMap<NotificationType, NotificationTypeModel>().ReverseMap();
      this.CreateMap<Service, ServiceModel>().ReverseMap();
      this.CreateMap<Supplier, SupplierModel>().ReverseMap();
      this.CreateMap<Manufacturer, ManufacturerModel>().ReverseMap();
    }
  }
}
