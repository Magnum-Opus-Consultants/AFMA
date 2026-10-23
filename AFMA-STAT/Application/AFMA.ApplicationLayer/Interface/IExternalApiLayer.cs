namespace AFMA.ApplicationLayer.Interface
{
  using AFMA.ApplicationLayer.Model.ExternalApi;

  public interface IExternalApiLayer
  {
    Task<FeedsalesApiDashboardModel> GetFeedsalesReportDataAsync();
  }
}
