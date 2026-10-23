namespace AFMA.ApplicationLayer.Services
{
  using AFMA.ApplicationLayer.Helpers;
  using AFMA.ApplicationLayer.Interface;
  using AFMA.ApplicationLayer.Model.ExternalApi;
  using Microsoft.Extensions.Caching.Memory;
  using Microsoft.Extensions.Options;
  using System.Threading.Tasks;

  public class ExternalApiService(
    IMemoryCache cache,
    IOptions<MemoryCacheEntryOptions> cacheOptions,
    IExternalApiLayer externalApiLayer) : IExternalApiService
  {
    private readonly MemoryCacheEntryOptions _cacheEntryOptions = cacheOptions.Value;
    private readonly string cacheKey = "feedsales-reports";

    public async Task<FeedsalesApiDashboardModel> GetFeedsalesDashboardDataAsync()
    {
      if (!cache.TryGetValue(cacheKey, out FeedsalesApiDashboardModel dashboardModel))
      {
        dashboardModel = await externalApiLayer.GetFeedsalesReportDataAsync();
        dashboardModel.Years = dashboardModel.Data.Select(year => year.ReportYear).Distinct().OrderBy(y => y).ToList();

        foreach (var year in dashboardModel.Years)
        {
          dashboardModel.YearlyData.Add(new FeedsalesApiYearlyModel
          {
            ReportYear = year,
            TotalSubmitted = dashboardModel.Data.Where(x => x.ReportYear == year && x is { IsSubmitted: true, ProductMaterial: ProductMaterial.FinishedProduct }).Sum(s => s.SubmittedValue),
          });

          dashboardModel.RmYearlyData.Add(new FeedsalesApiYearlyModel
          {
            ReportYear = year,
            TotalSubmitted = dashboardModel.Data.Where(x => x.ReportYear == year && x is { IsSubmitted: true, ProductMaterial: ProductMaterial.RawMaterial }).Sum(s => s.SubmittedValue),
          });
        }

        cache.Set(cacheKey, dashboardModel, _cacheEntryOptions);
      }

      return dashboardModel;
    }

    public async Task<FeedsalesApiDashboardModel> RefreshFeedsalesDashboardDataAsync(int[] feedYears)
    {
      if (!cache.TryGetValue(cacheKey, out FeedsalesApiDashboardModel dashboardModel))
      {
        var result = await externalApiLayer.GetFeedsalesReportDataAsync().ConfigureAwait(false);

        dashboardModel.Data = result.Data.Where(x => feedYears.Contains(x.ReportYear)).ToList();

        dashboardModel.Years = dashboardModel.Data.Select(year => year.ReportYear).Distinct().OrderBy(y => y).ToList();

        foreach (var year in dashboardModel.Years)
        {
          dashboardModel.YearlyData.Add(new FeedsalesApiYearlyModel
          {
            ReportYear = year,
            TotalSubmitted = dashboardModel.Data.Where(x => x.ReportYear == year && x is { IsSubmitted: true, ProductMaterial: ProductMaterial.FinishedProduct }).Sum(s => s.SubmittedValue),
          });

          dashboardModel.RmYearlyData.Add(new FeedsalesApiYearlyModel
          {
            ReportYear = year,
            TotalSubmitted = dashboardModel.Data.Where(x => x.ReportYear == year && x is { IsSubmitted: true, ProductMaterial: ProductMaterial.RawMaterial }).Sum(s => s.SubmittedValue),
          });
        }

        cache.Set(cacheKey, dashboardModel, _cacheEntryOptions);
      }

      return dashboardModel;
    }
  }
}
