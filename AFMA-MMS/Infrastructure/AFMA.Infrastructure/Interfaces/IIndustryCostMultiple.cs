namespace AFMA.Infrastructure.Interfaces
{
  public interface IIndustryCostMultiple : IIndustryCostBase  
  {
    decimal ProductionValue { get; set; }
    decimal SalesValue { get; set; }
    decimal AdministrationValue { get; set; }
    decimal RoadTransportValue { get; set; }
  }
}
