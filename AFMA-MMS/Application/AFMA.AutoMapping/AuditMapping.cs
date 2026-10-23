namespace AFMA.AutoMapping
{
  using ApplicationLayer.Model;
  using AutoMapper;
  using Data.Entities;

  public class AuditMapping : Profile 
  {
    public AuditMapping()
    {
      this.CreateMap<LoginHistory, LoginHistoryModel>().ReverseMap();
    }
  }
}
