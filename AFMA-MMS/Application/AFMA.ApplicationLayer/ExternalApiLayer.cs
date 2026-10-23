namespace AFMA.ApplicationLayer
{
  using AFMA.ApplicationLayer.Helpers;
  using AFMA.ApplicationLayer.Interface;
  using AFMA.ApplicationLayer.Model.ExternalApi;

  public class ExternalApiLayer : IExternalApiLayer
  {
    private readonly IApplicationLayer _applicationLayer;

    public ExternalApiLayer(IApplicationLayer applicationLayer)
    {
      _applicationLayer = applicationLayer;
    }

    public async Task<FeedsalesApiDashboardModel> GetFeedsalesReportDataAsync()
    {
      var reportItems = await _applicationLayer.GetFeedsalesReportItems().ConfigureAwait(false);

      var feedsalesApiDashboardModel = new FeedsalesApiDashboardModel
      {
        Data = reportItems.Select(report => new FeedsalesApiModel
        {
          SubmittedValue = report.Value,
          ReportYear = report.ReportYear,
          ReportMonth = report.ReportMonth,
          IsSubmitted = report.IsSubmitted,
          ProductGroupId = report.ProductGroupId,
          ProductGroupName = report.ProductGroup,
          ProductMaterial = report.ProductId == null ? ProductMaterial.RawMaterial : ProductMaterial.FinishedProduct,
        }).ToList()
      };

      return feedsalesApiDashboardModel;
    }
  }
}
