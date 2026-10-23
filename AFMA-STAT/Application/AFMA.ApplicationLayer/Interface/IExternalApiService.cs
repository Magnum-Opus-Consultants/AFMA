namespace AFMA.ApplicationLayer.Interface
{
  using AFMA.ApplicationLayer.Model.ExternalApi;
  using System.Threading.Tasks;

  public interface IExternalApiService
  {
    Task<FeedsalesApiDashboardModel> GetFeedsalesDashboardDataAsync();
    Task<FeedsalesApiDashboardModel> RefreshFeedsalesDashboardDataAsync(int[] feedYears);
  }
}
