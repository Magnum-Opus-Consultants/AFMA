namespace AFMA.ApplicationLayer.Helpers
{
  public enum IncreaseDecrease
  {
    Increase,
    Decrease,
  }

  public enum ModuleEnum
  {
    Feedsales = 1,
    Salmonella = 2,
    IndustryProfile = 3,
    Mycotoxin = 1002
  }

  public enum CalculationType
  {
    Value,
    Formula
  }

  public enum IndustryProfileDisplayType
  {
    Dashboard,
    Other
  }

  public enum CostingGroup
  {
    ProductionValue,
    SalesValue,
    AdministrationValue,
    RoadTransportValue,
  }

  public enum PcbItem
  {
    All = 0,
    Pellet = 1,
    Crumbling = 2,
    Bagging = 3
  }

  public enum RunningCostType
  {
    Production,
    Marketing,
    Administration,
    RoadTransport,
    Other
  }

  public enum MycotoxinSampleTypeEnum
  {
    RawMaterial = 1,
    FinishedProduct = 2
  }

  public enum MaterialOriginEnum
  {
    Local = 1,
    Imported = 2
  }

  public enum MycotoxinAnalysisEnum
  {
    Other = 5,
  }

  public enum Season
  {
    Summer,
    Fall,
    Winter,
    Spring
  }

  public enum IndustryProfileSection
  {
    IndustryProfile,
    IndustryCost,
    RunningCost,
    RunningCostSundry,
    Pelleting,
    Crumbling,
    Bagging
  }

  public enum CurrentPageAction
  {
    View,
    Manage
  }

  public enum DisplayValue
  {
    Ton,
    Value
  }

  public enum ProductMaterial
  {
    FinishedProduct,
    RawMaterial
  }

  public enum BatchStatus
  {
    Pending,
    Submitted,
    Error
  }
}