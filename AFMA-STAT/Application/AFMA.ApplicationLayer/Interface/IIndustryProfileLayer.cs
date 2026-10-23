namespace AFMA.ApplicationLayer.Interface
{
  using AFMA.ApplicationLayer.ViewModels;
  using Alpha.Services.Models.Repository;
  using Helpers;
  using Microsoft.AspNetCore.Mvc.Rendering;

  public interface IIndustryProfileLayer
  {
    Task<IEnumerable<SelectListItem>> GenerateReportYearSelectList(int? selectedValue);

    Task<IEnumerable<IndustryProfileReportModel>> GetIndustryProfileReports(int memberId, int memberBranchId,
      int startYear, int endYear, int memberTypeId);

    Task<IEnumerable<IndustryProfileReportModel>> GetIndustryProfileReports(int reportYear);

    Task<CompanyIndustryProfileViewModel> GetAllDashboardByYear(int memberId, int memberBranchId, int reportYear,
      int memberTypeId);

    Task<SaveOrUpdateResult> CreateReport(int memberId, int memberBranchId, int memberAccountId, int reportYear, int memberTypeId,
      string userId, string fullName);

    Task<RepositoryResult> DeleteReport(int reportId);

    Task<SaveOrUpdateResult> UpdateReportStatus(int id, bool submitStatus);

    Task<IEnumerable<PcbCostMainGroupModel>> GetPcbMainGroups(bool showAll = false);
    Task<IEnumerable<PcbCostSubGroupModel>> GetPcbSubGroups();

    Task<int> GetAnnualProductionAverage();
    Task<IEnumerable<int>> GetReportYears(int startYear, int endYear, int memberId, int memberBranchId);

    Task<IEnumerable<IndustryProfileTotalCalculationViewModel>> PopulateDefaultTotalValues(
      IndustryProfileDisplayType displayType, int memberTypeId, int reportId = 0, int branchId = 0, int reportYear = 0);

    Task<decimal> GetValueFromMemberIpBaseTable(int reportId, int pcbMainGroupItemId, int[] years, int year = 0);

    #region Running Costs Sundry Expenses

    Task<IEnumerable<RunningCostSundryExpenseModel>> GetRunningCostSundryExpense(bool showAll = true);

    #endregion

    #region Pellet Crumbling and Bagging

    Task<IEnumerable<PcbCostSubGroupModel>> GetPcbCostSubGroups(bool showAll = true);


    #endregion

    #region Running Costs

    Task<IEnumerable<RunningCostGroupModel>> GetRunningCostsGroups(bool showAll = true);

    #endregion

    #region Excel report export

    Task<byte[]> GenerateIndustryProfileExcelReport(ReportCriteriaModel reportCriteria);

    #endregion
  }
}
