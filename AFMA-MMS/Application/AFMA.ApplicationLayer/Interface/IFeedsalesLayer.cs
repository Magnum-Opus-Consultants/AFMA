namespace AFMA.ApplicationLayer.Interface
{
  using AFMA.ApplicationLayer.Helpers;

  using AFMA.ApplicationLayer.ViewModels;

  /// <summary>
  /// The IFeedsales interface.
  /// </summary>
  public interface IFeedsalesLayer
  {
    /// <summary>
    /// Retrieves a list of feedsales product groups.
    /// </summary>
    /// <returns>The list of groups.</returns>
    Task<IEnumerable<ProductGroupModel>> GetFinishedProductGroups();

    /// <summary>
    /// Retrieves a single report for a specific member branch.
    /// </summary>
    /// <param name="id">The report Id.</param>
    /// <param name="memberBranchId">The member account Id.</param>
    /// <returns>The feedsales report.</returns>
    Task<FeedsalesReportModel> GetFeedsalesReport(int id, int memberBranchId);

    /// <summary>
    /// Retrieves a single report for a specific member branch for the previous month.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="memberBranchId"></param>
    /// <returns></returns>
    Task<FeedsalesReportModel> GetPreviousMonthFeedsalesReport(int id, int memberBranchId);

    /// <summary>
    /// The total feedsales export report.
    /// </summary>
    /// <param name="reportCriteria">The report criteria</param>
    /// <returns>The report byte array object</returns>
    Task<byte[]> GenerateTotalFeedsalesReport(ReportCriteriaModel reportCriteria);

    /// <summary>
    /// The raw material usage report
    /// </summary>
    /// <param name="rawMaterialId">The Raw Material ID</param>
    /// <param name="startDate">The report start date</param>
    /// <param name="endDate">The report end date</param>
    /// <returns></returns>
    Task<byte[]> GetRawMaterialUsageReport(int rawMaterialId, DateTime startDate, DateTime endDate);

    /// <summary>
    /// Gets feedales report summaries per member per year.
    /// </summary>
    /// <param name="year">The report year</param>
    /// <param name="memberBranchId">The member branch ID</param>
    /// <returns></returns>
    Task<IEnumerable<FeedsalesReportLookupModel>> GetFeedsalesReports(int year, int memberBranchId);

    /// <summary>
    /// Gets the branch summary report.
    /// </summary>
    /// <param name="module"></param>
    /// <param name="year"></param>
    /// <returns></returns>
    Task<BranchSummaryReportViewModel> GetReportBranchSummaryReport(ModuleEnum module, int year);

    /// <summary>
    /// Gets feedales report summaries per member per year per month
    /// </summary>
    /// <param name="year"></param>
    /// <param name="month"></param>
    /// <param name="memberBranchId"></param>
    /// <returns></returns>
    Task<IEnumerable<FeedsalesReportLookupModel>> GetFeedsalesReports(int year, int month, int memberBranchId);

    #region RAW MATERIAL AND FINISHED PRODUCT USAGE REPORTS

    /// <summary>
    /// Get the total raw material usage by material by start and end date
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <param name="materialId"></param>
    /// <returns></returns>
    Task<byte[]> GenerateRawMaterialUsageReport(DateTime startDate, DateTime endDate, int[] materialId, int[] memberId);

    /// <summary>
    /// Get the total finished product usage by material by start and end date
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <param name="productGroupId"></param>
    /// <param name="memberId"></param>
    /// <returns></returns>
    Task<byte[]> GenerateFinishedProductUsageReport(DateTime startDate, DateTime endDate, int[] productGroupId, int[] memberId);

    #endregion
  }
}
