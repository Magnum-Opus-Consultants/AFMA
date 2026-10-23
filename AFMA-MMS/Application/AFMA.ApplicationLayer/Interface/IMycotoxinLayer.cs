namespace AFMA.ApplicationLayer.Interface
{
  using AFMA.ApplicationLayer.ViewModels;
  using Alpha.Services.Models.Data.PageData;
  using Alpha.Services.Models.Repository;
  using Microsoft.AspNetCore.Mvc.Rendering;

  public interface IMycotoxinLayer
  {
    #region Select Lists

    Task<IEnumerable<SelectListItem>> GenerateMycotoxinAnalysisSelectList(int? selectedValue);
    Task<IEnumerable<SelectListItem>> GenerateMycotoxinSampleTypeSelectList(int? selectedValue);
    Task<IEnumerable<SelectListItem>> GenerateMycotoxinClassSelectList(int? selectedValue);
    Task<IEnumerable<SelectListItem>> GenerateMycotoxinTypeSelectList(int mycotoxinClassId, int? selectedValue);
    Task<IEnumerable<SelectListItem>> GenerateMycotoxinTypeSelectList(int? selectedValue);

    #endregion

    #region Reports

    Task<IEnumerable<MycotoxinReportLookupModel>> GetMycotoxinReports(AlphaPageRequest pageRequest, int? memberId,
      int? memberBranchId, bool? isSubmitted);

    Task<IEnumerable<MycotoxinReportModel>> GetMycotoxinReports(AlphaPageRequest pageRequest, int memberId,
      int memberBranchId, int memberAccountId, bool? isSubmitted);

    Task<MycotoxinReportModel> GetMycotoxinReport(int id);
    Task<MycotoxinReportLookupModel> GetMycotoxinReportLookup(int id);
    Task<MycotoxinReportModel> GetMycotoxinReport(int id, int memberAccountId);
    Task<MycotoxinSampleModel> GetMycotoxinSample(int sampleId, int reportId);
    Task<RepositoryResult> DeleteMycotoxinReport(int id);
    Task<SaveOrUpdateResult> UpdateMycotoxinReportStatus(int id, bool submitStatus);

    #endregion

    #region Classes
    Task<IEnumerable<MycotoxinClassModel>> GetMycotoxinClasses();

    Task<IEnumerable<MycotoxinClassModel>> GetMycotoxinClasses(AlphaPageRequest pageRequest,
      string? searchText = null);
    Task<MycotoxinClassModel> GetMycotoxinClass(int mycotoxinClassId);
    Task<RepositoryResult> DeleteMycotoxinClass(int mycotoxinClassId);

    #endregion

    #region Types

    Task<IEnumerable<MycotoxinTypeModel>> GetMycotoxinTypes();

    Task<IEnumerable<MycotoxinTypeModel>> GetMycotoxinTypes(AlphaPageRequest pageRequest, int? filterMycotoxinClassId,
      string? searchText = null);
    Task<IEnumerable<MycotoxinTypeModel>> GetMycotoxinTypes(int mycotoxinClassId);
    Task<MycotoxinTypeModel> GetMycotoxinType(int mycotoxinTypeId);
    Task<RepositoryResult> DeleteMycotoxinType(int mycotoxinTypeId);

    #endregion

    #region Analysis Methods

    Task<MycotoxinAnalysisModel> GetMycotoxinAnalysisMethod(int mycotoxinAnalysisMethodId);

    #endregion

    #region Submission Stats

    Task<IEnumerable<SubmissionStatsViewModel>> GetMycotoxinSubmissions(DateTime period);
    Task<MycotoxinReportModel> GetMycotoxinReportForPeriod(DateTime period, int memberBranchId);
    Task<bool> SendMycotoxinSubmissionReminder(int mycotoxinReportId);

    #endregion

    #region Detections

    Task<IEnumerable<MycotoxinSampleDetectionModel>> GetMycotoxinSampleDetections(int sampleId);
    Task<MycotoxinSampleDetectionModel> GetMycotoxinSampleDetection(int detectionSampleId);
    Task<RepositoryResult> DeleteSampleDetection(int reportId, int sampleId, int sampleDetectionId);

    #endregion

    #region Samples

    Task<RepositoryResult> DeleteSample(int reportId, int sampleId);
    Task<RepositoryResult> SaveOrUpdateSample(MycotoxinSampleModel mycotoxinSampleModel);
    Task DeleteDetectionsForSample(int sampleId);

    #endregion

    #region CRUD Methods

    Task<SaveOrUpdateResult> SaveOrUpdate<TSource>(TSource source);
    Task<SaveOrUpdateResult> Delete<TSource>(TSource source);

    #endregion
  }
}
