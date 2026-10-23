namespace AFMA.ApplicationLayer.Interface
{
  using AFMA.ApplicationLayer.Helpers;
  using AFMA.ApplicationLayer.ViewModels;
  using Alpha.Services.Models.Data.PageData;
  using Alpha.Services.Models.Repository;
  using Microsoft.AspNetCore.Mvc.Rendering;

  /// <summary>
  /// The SalmonellaLayer interface.
  /// </summary>
  public interface ISalmonellaLayer
  {
    /// <summary>
    /// Retrieves a list of salmonella sample types.
    /// </summary>
    /// <returns>The list of salmonella types</returns>
    Task<IEnumerable<SalmonellaSampleTypeModel>> GetSalmonellaSampleTypes();

    #region Select Lists

    /// <summary>
    /// Generates the salmonella sample type select list.
    /// </summary>
    /// <param name="selectedValue">The selected value.</param>
    /// <returns>The salmonella sample types select list.</returns>
    Task<IEnumerable<SelectListItem>> GenerateSalmonellaTypeSelectList(int? selectedValue);

    /// <summary>
    /// Generated the salmonella group select list.
    /// </summary>
    /// <param name="selectedValue">The selected value.</param>
    /// <returns>The salmonella group list.</returns>
    Task<IEnumerable<SelectListItem>> GenerateSalmonellaGroupSelectList(int? selectedValue);

    /// <summary>
    /// Generates a product group select list for salmonella.
    /// </summary>
    /// <param name="selectedValue">The selected value.</param>
    /// <returns>The product group select list.</returns>
    Task<IEnumerable<SelectListItem>> GenerateProductGroupSelectList(int? selectedValue);

    /// <summary>
    /// Generates a raw material select list for salmonella.
    /// </summary>
    /// <param name="selectedValue">The selected value.</param>
    /// <returns>The material select list.</returns>
    Task<IEnumerable<SelectListItem>> GenerateMaterialSelectList(int? selectedValue);

    /// <summary>
    /// Generates the salmonella report groups select list.
    /// </summary>
    /// <param name="selectedValue">The selected value.</param>
    /// <returns>the report group select list.</returns>
    Task<IEnumerable<SelectListItem>> GenerateSalmonellaReportGroupSelectList(int? selectedValue);

    /// <summary>
    /// Generates the salmonella product group category select list.
    /// </summary>
    /// <param name="selectedValue">The selected value.</param>
    /// <returns>The select list.</returns>
    Task<IEnumerable<SelectListItem>> GenerateSalmonellaProductGroupCategorySelectList(int? selectedValue);

    #endregion

    #region Finished Products

    Task<IEnumerable<ProductGroupModel>> GetFinishedProductGroups();

    #endregion

    #region Salmonella Types

    /// <summary>
    /// Retrieves a single salmonella type.
    /// </summary>
    /// <param name="typeId">The salmonella type ID</param>
    /// <returns>The salmonella type.</returns>
    Task<SalmonellaTypeModel> GetSalmonellaType(int typeId);

    /// <summary>
    /// Retrieved a list of salmonella types.
    /// </summary>
    /// <returns>The list.</returns>
    Task<IEnumerable<SalmonellaTypeModel>> GetSalmonellaTypes();

    /// <summary>
    /// Retrieves a filtered and paged list of salmonella types.
    /// </summary>
    /// <param name="pageRequest">The page request data.</param>
    /// <param name="salmonellaGroupId">The salmonella group ID.</param>
    /// <param name="searchText">The search text.</param>
    /// <returns>The paged and filtered list.</returns>
    Task<IEnumerable<SalmonellaTypeModel>> GetSalmonellaTypes(AlphaPageRequest pageRequest, int? salmonellaGroupId = null, string? searchText = null);

    /// <summary>
    /// Deletes a salmonella type.
    /// </summary>
    /// <param name="typeId">The type ID.</param>
    /// <returns>The result.</returns>
    Task<RepositoryResult> DeleteSalmonellaType(int typeId);

    #endregion

    #region Months (Seasonal)

    Task<IEnumerable<MonthModel>> GetSeasonalMonths();

    #endregion

    #region Salmonella Groups

    /// <summary>
    /// Retrieves a single salmonella group.
    /// </summary>
    /// <param name="groupId">The salmonella group ID</param>
    /// <returns>The salmonella group.</returns>
    Task<SalmonellaGroupModel> GetSalmonellaGroup(int groupId);

    /// <summary>
    /// Retrieves a list of salmonella groups.
    /// </summary>
    /// <returns>The list.</returns>
    Task<IEnumerable<SalmonellaGroupModel>> GetSalmonellaGroups();

    /// <summary>
    /// Retrieves a filtered and paged list of salmonella groups.
    /// </summary>
    /// <param name="pageRequest">The page request data.</param>
    /// <param name="searchText">The search text.</param>
    /// <returns>The paged and filtered list.</returns>
    Task<IEnumerable<SalmonellaGroupModel>> GetSalmonellaGroups(AlphaPageRequest pageRequest, string? searchText = null);

    /// <summary>
    /// Deletes a salmonella group.
    /// </summary>
    /// <param name="groupId">The group ID.</param>
    /// <returns>The result.</returns>
    Task<RepositoryResult> DeleteSalmonellaGroup(int groupId);

    #endregion

    #region Salmonella Admin Reports

    Task<IEnumerable<SalmonellaReportLookupModel>> GetSalmonellaReportsLookup();
    Task<IEnumerable<SalmonellaSampleModel>> GetSalmonellaReportSamples(int reportYear, int? memberId, int? branchId);
    Task<SalmonellaReportModel> GetSalmonellaReport(int id);
    Task<SalmonellaReportLookupModel> GetSalmonellaReportLookup(int id);
    Task<SalmonellaReportModel> GetSalmonellaReport(int id, int memberBranchId);
    Task<SalmonellaReportLookupModel> GetSalmonellaReportLookup(int id, int memberBranchId);

    Task<IEnumerable<SalmonellaReportLookupModel>> GetSalmonellaReportsLookup(int? memberId, int? memberBranchId,
      int? memberAccountId, bool? isSubmitted, int? currentMonth = null, int? currentYear = null);

    Task<IEnumerable<SalmonellaReportLookupModel>> GetSalmonellaReportsLookup(AlphaPageRequest pageRequest,
      int memberId, int memberBranchId, int memberAccountId, bool? isSubmitted);

    Task<IEnumerable<SalmonellaReportLookupModel>> GetSalmonellaReportsLookup(AlphaPageRequest pageRequest,
      int? memberId, int? memberBranchId, bool? isSubmitted);
    Task<bool> DeleteSalmonellaSample(int id);
    Task<SaveOrUpdateResult> UpdateSalmonellaReportStatus(int id, bool submitStatus);
    Task<RepositoryResult> DeleteSalmonellaReport(int id);
    //Task<bool> SendSalmonellaSubmissionReminder(int salmonellaReportId);
    Task<IEnumerable<SalmonellaProductGroupCategoryModel>> GetSalmonellaProductGroupCategories();

    Task<IEnumerable<SalmonellaYearByMonthExcelReversedViewModel>> GetSalmonellaYearByMonthReversedExcel(
      DateTime startDate, DateTime endDate, int? memberBranchId);

    Task<IEnumerable<SalmonellaSeasonalViewModel>> GetSalmonellaYearByMonthSeasonal(DateTime startDate,
      DateTime endDate, int? memberBranchId = null);

    Task<IEnumerable<SalmonellaMonthlyExcelReversedViewModel>> GetSalmonellaMonthlyReversedExcel(DateTime startDate,
      DateTime endDate, int? branchId = null);

    Task<IEnumerable<SalmonellaYearlyExcelReversedViewModel>> GetSalmonellaYearlyReportReversedExcel(DateTime startDate,
      DateTime endDate);

    Task<IEnumerable<SalmonellaRawMaterialGroupReportViewModel>> GetSalmonellaGroupStatistics(DateTime startDate,
      DateTime endDate);

    int GetTotalSalmonellaByProductGroupByYear(DateTime startDate, DateTime endDate, int year, int productGroupId,
      int salmonellaDetected);

    int GetTotalSalmonellaTestedByYearByGroupCategory(DateTime startDate, DateTime endDate,
      int productGroupCategoryId, int year);

    int GetTotalSalmonellaPositiveByYearByGroupCategory(DateTime startDate, DateTime endDate,
      int productGroupCategoryId, int year);

    IEnumerable<SalmonellaDetailSubmissionSummaryViewModel> GetSalmonellaDetailSubmissionSummary(DateTime startDate,
      DateTime endDate, int sampleTypeId, int sampleId);

    Task<byte[]> GenerateTotalSalmonellaExcelReport(ReportCriteriaModel reportCriteria);
    Task<byte[]> GenerateDetailSalmonellaExcelReport(ReportCriteriaModel reportCriteria);
    Task<byte[]> GenerateSalmonellaClientExcelReport(ReportCriteriaModel reportCriteria);

    #endregion

    Task<IEnumerable<SalmonellaReportLookupModel>> GetSalmonellaReportsLookup(int year, int memberBranchId);
    Task<BranchSummaryReportViewModel> GetReportBranchSummaryReport(ModuleEnum module, int year);
  }
}
