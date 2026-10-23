namespace AFMA.ApplicationLayer
{
  using AFMA.ApplicationLayer.Expressions;
  using AFMA.ApplicationLayer.Templating.Models;
  using AFMA.ApplicationLayer.ViewModels;
  using AFMA.Infrastructure.Options;
  using AFMA.Shared.Infrastructure.Settings;
  using Alpha.Services.Extensions.Attributes;
  using Alpha.Services.Models.Data.PageData;
  using Alpha.Services.Models.Repository;
  using Alpha.Services.NHibernate.Repository.Services;
  using Alpha.Services.Notifications.Smtp;
  using AutoMapper;
  using Helpers;
  using Infrastructure.Settings;
  using Interface;
  using Microsoft.AspNetCore.Mvc.Rendering;
  using Microsoft.Extensions.Options;
  using Razor.Templating.Core;
  using X.PagedList;

  /// <summary>
  /// The MycotoxinLayer class.
  /// </summary>
  public class MycotoxinLayer : IMycotoxinLayer
  {
    private readonly IApplicationRepository<SessionFactory> _applicationRepository;
    private readonly ApplicationSettings _applicationSettings;
    private readonly SmtpSettings _smtpSettings;
    private readonly IMapper _mapper;
    private readonly IIdentityLayer _identityLayer;
    private readonly IApplicationLayer _applicationLayer;
    private readonly IRazorTemplateEngine _razorTemplateEngine;
    private readonly IEmailClient _emailClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="MycotoxinLayer"/> class.
    /// </summary>
    /// <param name="applicationSettings">The Application Settings.</param>
    /// <param name="mapper">The Auto Mapper.</param>
    /// <param name="identityLayer">The Identity Layer.</param>
    /// <param name="smtpSettings">The SMTP Settings</param>
    public MycotoxinLayer(
      IApplicationRepository<SessionFactory> applicationRepository,
      IOptions<ApplicationSettings> applicationSettings,
      IMapper mapper,
      IIdentityLayer identityLayer,
      IOptions<SmtpSettings> smtpSettings,
      IApplicationLayer applicationLayer,
      IRazorTemplateEngine razorTemplateEngine,
      IEmailClient emailClient)
    {
      this._applicationRepository = applicationRepository;
      this._applicationSettings = applicationSettings.Value;
      this._mapper = mapper;
      this._identityLayer = identityLayer;
      this._smtpSettings = smtpSettings.Value;
      this._applicationLayer = applicationLayer;
      this._razorTemplateEngine = razorTemplateEngine;
      this._emailClient = emailClient;
    }

    #region Select Lists

    public async Task<IEnumerable<SelectListItem>> GenerateMycotoxinAnalysisSelectList(int? selectedValue)
    {
      var results = await _applicationRepository.QueryAsync<MycotoxinAnalysis>();
      return results.OrderBy(o => o.Name).ToList().ToSelectList(x => x.Id, x => x.Name, selectedValue);
    }

    public async Task<IEnumerable<SelectListItem>> GenerateMycotoxinSampleTypeSelectList(int? selectedValue)
    {
      var results = await _applicationRepository.QueryAsync<MycotoxinSampleType>();
      return results.OrderBy(o => o.Name).ToList().ToSelectList(x => x.Id, x => x.Name, selectedValue);
    }

    public async Task<IEnumerable<SelectListItem>> GenerateMycotoxinClassSelectList(int? selectedValue)
    {
      var result = await _applicationRepository.QueryAsync<MycotoxinClass>(x => x.Active).ConfigureAwait(false);
      return result.ToList().ToSelectList(x => x.Id, x => x.Name, selectedValue);
    }

    public async Task<IEnumerable<SelectListItem>> GenerateMycotoxinTypeSelectList(int mycotoxinClassId, int? selectedValue)
    {
      var result =
        await _applicationRepository.QueryAsync<MycotoxinType>(x =>
          x.MycotoxinClass.Id == mycotoxinClassId && x.Active).ConfigureAwait(false);

      return result.OrderBy(n => n.Name).ToList().ToSelectList(x => x.Id, x => x.Name, selectedValue);
    }

    public async Task<IEnumerable<SelectListItem>> GenerateMycotoxinTypeSelectList(int? selectedValue)
    {
      var result = await _applicationRepository.QueryAsync<MycotoxinType>(x => x.Active).ConfigureAwait(false);
      return result.OrderBy(n => n.Name).ToList().ToSelectList(x => x.Id, x => x.Name, selectedValue);
    }

    #endregion

    #region Reports

    public async Task<IEnumerable<MycotoxinReportLookupModel>> GetMycotoxinReports(AlphaPageRequest pageRequest, int? memberId, int? memberBranchId, bool? isSubmitted)
    {
      var result = await _applicationRepository.QueryPagedAsync(pageRequest,
        SearchExpression.FilterMycotoxinReports(memberId, memberBranchId, isSubmitted));
      var resultMapped = _mapper.Map<IEnumerable<MycotoxinReportLookup>, List<MycotoxinReportLookupModel>>(result.PageData);
      return new StaticPagedList<MycotoxinReportLookupModel>(resultMapped, pageRequest.PageNumber, (int)pageRequest.PageLength!, result.TotalResults);
    }

    /// <summary>
    /// Returns a filtered list of mycotoxin reports for a specific member account.
    /// </summary>
    /// <param name="pageRequest">The pages request.</param>
    /// <param name="memberId">The member Id.</param>
    /// <param name="memberBranchId">The member branch Id.</param>
    /// <param name="memberAccountId">The member account Id.</param>
    /// <param name="isSubmitted">The submitted indicator.</param>
    /// <returns>A paged results with feedsales reports.</returns>
    public async Task<IEnumerable<MycotoxinReportModel>> GetMycotoxinReports(AlphaPageRequest pageRequest, int memberId, int memberBranchId, int memberAccountId, bool? isSubmitted)
    {
      var result = await _applicationRepository.QueryPagedAsync(pageRequest, SearchExpression.FilterMycotoxinReports(memberId, memberBranchId, memberAccountId, isSubmitted));
      var resultMapped = _mapper.Map<IEnumerable<MycotoxinReport>, List<MycotoxinReportModel>>(result.PageData);
      return new StaticPagedList<MycotoxinReportModel>(resultMapped, pageRequest.PageNumber, (int)pageRequest.PageLength!, result.TotalResults);
    }

    public async Task<MycotoxinReportModel> GetMycotoxinReport(int id)
    {
      var result = await _applicationRepository.QuerySingleAsync<MycotoxinReport>(x => x.Id == id);
      return _mapper.Map<MycotoxinReport, MycotoxinReportModel>(result);
    }

    public async Task<MycotoxinReportLookupModel> GetMycotoxinReportLookup(int id)
    {
      var result = await _applicationRepository.QuerySingleAsync<MycotoxinReportLookup>(x => x.Id == id);
      return _mapper.Map<MycotoxinReportLookup, MycotoxinReportLookupModel>(result);
    }

    public async Task<MycotoxinReportModel> GetMycotoxinReport(int id, int memberAccountId)
    {
      var result = await _applicationRepository.QuerySingleAsync<MycotoxinReport>(x => x.Id == id && x.MemberAccount.Id == memberAccountId);
      return _mapper.Map<MycotoxinReport, MycotoxinReportModel>(result);
    }

    public async Task<RepositoryResult> DeleteMycotoxinReport(int id)
    {
      var report = await _applicationRepository.QuerySingleAsync<MycotoxinReport>(x => x.Id == id)
        .ConfigureAwait(false);
      return await _applicationRepository.DeleteAndCommitAsync(report).ConfigureAwait(false);
    }

    public async Task<MycotoxinSampleModel> GetMycotoxinSample(int sampleId, int reportId)
    {
      var result =
        await _applicationRepository.QuerySingleAsync<MycotoxinSample>(x =>
          x.Id == sampleId && x.MycotoxinReport.Id == reportId).ConfigureAwait(false);
      return _mapper.Map<MycotoxinSample, MycotoxinSampleModel>(result);
    }

    public async Task<RepositoryResult> SaveOrUpdateSample(MycotoxinSampleModel mycotoxinSampleModel)
    {
      if (mycotoxinSampleModel.MaterialOriginId == (int)MaterialOriginEnum.Local)
      {
        mycotoxinSampleModel.MaterialCountryOfOrigin = null;
      }

      if (mycotoxinSampleModel.MaterialOriginId == (int)MaterialOriginEnum.Imported)
      {
        mycotoxinSampleModel.ProvinceId = null;
      }

      if (!mycotoxinSampleModel.MycotoxinDetected)
      {
        await DeleteDetectionsForSample(mycotoxinSampleModel.Id).ConfigureAwait(false);
      }

      var result = await _applicationRepository
        .SaveOrUpdateAndCommitAsync(_mapper.Map<MycotoxinSampleModel, MycotoxinSample>(mycotoxinSampleModel))
        .ConfigureAwait(false);

      return result;
    }

    public async Task<SaveOrUpdateResult> UpdateMycotoxinReportStatus(int id, bool submitStatus)
    {
      var report = await _applicationRepository.QuerySingleAsync<MycotoxinReport>(x => x.Id == id);

      if (submitStatus)
      {
        report.IsSubmitted = true;
        report.SubmittedDate = DateTime.Now;
        report.MemberAccount ??= new MemberAccount { Id = await _applicationLayer.GetCurrentMemberAccountId().ConfigureAwait(false) };
      }
      else
      {
        report.IsSubmitted = false;
        report.SubmittedDate = null;
        report.MemberAccount = null;
      }

      var result = await _applicationRepository.SaveOrUpdateAndCommitAsync(report);

      return new SaveOrUpdateResult
      {
        Success = result.Succeeded,
        ErrorMessage = result.ExceptionMessage,
        SavedObjectId = result.SavedObjectId,
      };
    }

    #endregion

    #region Classes

    public async Task<IEnumerable<MycotoxinClassModel>> GetMycotoxinClasses()
    {
      var result = await _applicationRepository.QueryAsync<MycotoxinClass>();
      return _mapper.Map<IEnumerable<MycotoxinClass>, List<MycotoxinClassModel>>(result);
    }

    public async Task<IEnumerable<MycotoxinClassModel>> GetMycotoxinClasses(AlphaPageRequest pageRequest, string? searchText = null)
    {
      var result = await _applicationRepository.QueryPagedAsync(pageRequest, SearchExpression.FilterMycotoxinClasses(searchText ?? ""));
      var resultMapped = _mapper.Map<IEnumerable<MycotoxinClass>, List<MycotoxinClassModel>>(result.PageData);
      return new StaticPagedList<MycotoxinClassModel>(resultMapped, pageRequest.PageNumber, (int)pageRequest.PageLength!, result.TotalResults);
    }

    public async Task<MycotoxinClassModel> GetMycotoxinClass(int mycotoxinClassId)
    {
      var result = await _applicationRepository.QuerySingleAsync<MycotoxinClass>(x => x.Id == mycotoxinClassId);
      return _mapper.Map<MycotoxinClass, MycotoxinClassModel>(result);
    }

    public async Task<RepositoryResult> DeleteMycotoxinClass(int mycotoxinClassId)
    {
      var mycotoxinClass = await _applicationRepository.QuerySingleAsync<MycotoxinClass>(x => x.Id == mycotoxinClassId).ConfigureAwait(false);
      var result = await _applicationRepository.DeleteAndCommitAsync(mycotoxinClass);
      return result;
    }

    #endregion

    #region Types

    public async Task<IEnumerable<MycotoxinTypeModel>> GetMycotoxinTypes()
    {
      var result = await _applicationRepository.QueryAsync<MycotoxinType>();
      return _mapper.Map<IEnumerable<MycotoxinType>, List<MycotoxinTypeModel>>(result);
    }

    public async Task<IEnumerable<MycotoxinTypeModel>> GetMycotoxinTypes(AlphaPageRequest pageRequest, int? filterMycotoxinClassId, string? searchText = null)
    {
      var result = await _applicationRepository.QueryPagedAsync(pageRequest, SearchExpression.FilterMycotoxinTypes(filterMycotoxinClassId, searchText ?? ""));
      var resultMapped = _mapper.Map<IEnumerable<MycotoxinType>, List<MycotoxinTypeModel>>(result.PageData);
      return new StaticPagedList<MycotoxinTypeModel>(resultMapped, pageRequest.PageNumber, (int)pageRequest.PageLength!, result.TotalResults);
    }

    public async Task<IEnumerable<MycotoxinTypeModel>> GetMycotoxinTypes(int mycotoxinClassId)
    {
      var result = await _applicationRepository.QueryAsync<MycotoxinType>(x => x.MycotoxinClass.Id == mycotoxinClassId).ConfigureAwait(false);
      return _mapper.Map<IEnumerable<MycotoxinType>, List<MycotoxinTypeModel>>(result);
    }

    public async Task<MycotoxinTypeModel> GetMycotoxinType(int mycotoxinTypeId)
    {
      var result = await _applicationRepository.QuerySingleAsync<MycotoxinType>(x => x.Id == mycotoxinTypeId);
      return _mapper.Map<MycotoxinType, MycotoxinTypeModel>(result);
    }

    public async Task<RepositoryResult> DeleteMycotoxinType(int mycotoxinTypeId)
    {
      var mycotoxinType = await _applicationRepository.QuerySingleAsync<MycotoxinType>(x => x.Id == mycotoxinTypeId).ConfigureAwait(false);
      var result = await _applicationRepository.DeleteAndCommitAsync(mycotoxinType);
      return result;
    }

    #endregion

    #region Detections

    public async Task<IEnumerable<MycotoxinSampleDetectionModel>> GetMycotoxinSampleDetections(int sampleId)
    {
      var result =
        await _applicationRepository.QueryAsync<MycotoxinSampleDetection>(x => x.MycotoxinSample.Id == sampleId).ConfigureAwait(false);
      return _mapper.Map<IEnumerable<MycotoxinSampleDetection>, List<MycotoxinSampleDetectionModel>>(result);
    }

    public async Task<RepositoryResult> DeleteSampleDetection(int reportId, int sampleId, int sampleDetectionId)
    {
      var sampleDetection = await _applicationRepository
        .QuerySingleAsync<MycotoxinSampleDetection>(x => x.Id == sampleDetectionId).ConfigureAwait(false);
      var result = await _applicationRepository.DeleteAndCommitAsync(sampleDetection);
      return result;
    }

    public async Task<MycotoxinSampleDetectionModel> GetMycotoxinSampleDetection(int detectionSampleId)
    {
      var result = await _applicationRepository.QuerySingleAsync<MycotoxinSampleDetection>(x => x.Id == detectionSampleId).ConfigureAwait(false);
      return _mapper.Map<MycotoxinSampleDetection, MycotoxinSampleDetectionModel>(result);
    }

    #endregion

    #region Samples

    public async Task<RepositoryResult> DeleteSample(int reportId, int sampleId)
    {
      var sample = await _applicationRepository.QuerySingleAsync<MycotoxinSample>(x => x.Id == sampleId).ConfigureAwait(false);
      var result = await _applicationRepository.DeleteAndCommitAsync(sample);
      return result;
    }

    public async Task DeleteDetectionsForSample(int sampleId)
    {
      var sampleDetections = await _applicationRepository.QueryAsync<MycotoxinSampleDetection>(x => x.MycotoxinSample.Id == sampleId).ConfigureAwait(false);
      foreach (var detection in sampleDetections)
      {
        await _applicationRepository.DeleteAndCommitAsync(detection).ConfigureAwait(false);
      }
    }

    #endregion

    #region Analysis Methods

    public async Task<MycotoxinAnalysisModel> GetMycotoxinAnalysisMethod(int mycotoxinAnalysisMethodId)
    {
      var result = await _applicationRepository.QuerySingleAsync<MycotoxinAnalysis>(x => x.Id == mycotoxinAnalysisMethodId);
      return _mapper.Map<MycotoxinAnalysis, MycotoxinAnalysisModel>(result);
    }

    #endregion

    #region Submission Stats

    public async Task<IEnumerable<SubmissionStatsViewModel>> GetMycotoxinSubmissions(DateTime period)
    {
      var submissionStatsList = new List<SubmissionStatsViewModel>();
      var branches = await _applicationLayer.GetActiveMemberBranchesSummaryByModule((int)ModuleEnum.Mycotoxin).ConfigureAwait(false);

      foreach (var branch in branches.OrderBy(a => a.Member).ThenBy(a => a.Name))
      {
        var s = new SubmissionStatsViewModel
        {
          BranchName = branch.Name!,
          MemberName = branch.Member!,
          Submitted = false
        };

        try
        {
          var report = await GetMycotoxinReportForPeriod(period, branch.Id);
          s.DueDate = report.DueDate;
          s.InfoFor = $"{report.ReportYear}/{report.ReportMonth.ToString().PadLeft(2, '0')} ({report.InfoFor.ToString("MMMM yyyy")})";
          s.ReportId = report.Id;
          s.Submitted = report.IsSubmitted;
          s.DateSubmitted = report.SubmittedDate;
          s.Comment = report.Comment;
          s.Status = report.IsSubmitted ? "<span class='badge bg-success'>Submitted</span>" : "<span class='badge bg-warning'>Outstanding</span>";
        }
        catch (Exception ex)
        {
          s.Status = "<span class='badge bg-danger' title='A report for this month was not found.'>REPORT NOT FOUND</span>";
        }

        submissionStatsList.Add(s);
      }

      return submissionStatsList;
    }

    public async Task<MycotoxinReportModel> GetMycotoxinReportForPeriod(DateTime period, int memberBranchId)
    {
      try
      {
        var results = await _applicationRepository.QuerySingleAsync<MycotoxinReport>(x =>
          x.ReportMonth == period.Month && x.ReportYear == period.Year && x.MemberBranch.Id == memberBranchId);
        return _mapper.Map<MycotoxinReport, MycotoxinReportModel>(results);
      }
      catch
      {
        return null;
      }
    }

    public async Task<bool> SendMycotoxinSubmissionReminder(int mycotoxinReportId)
    {
      try
      {
        var report = await _applicationRepository
          .QuerySingleAsync<MycotoxinReportLookup>(x => x.Id == mycotoxinReportId)
          .ConfigureAwait(false);

        var recipient = report.MemberAccount.ApplicationUser.Email;

        var submissionEmail = await this._razorTemplateEngine.RenderAsync("/Templates/GenericEmail.cshtml",
          new GenericEmailModel
          {
            BaseUrl = this._applicationSettings.BaseUrl,
            Body = Templating.EmailContent.Submissions.GenerateMycotoxinEmail(report.ReportMonth, report.ReportYear, recipient, report.Member.Name),
          }).ConfigureAwait(false);

        if (this._smtpSettings.Debug && !string.IsNullOrEmpty(this._smtpSettings.DebugEmail))
        {
          recipient = this._smtpSettings.DebugEmail;
        }

        await _emailClient.SendAsync(recipient,
            $"AFMA MYCOTOXINS: Outstanding Report for {report.Member.Name} - {report.MemberBranch.Name}",
            submissionEmail, null, new List<string> { "operations@resolvetech.co.za" })
          .ConfigureAwait(false);

        return true;
      }
      catch
      {
        return false;
      }
    }

    #endregion

    #region CRUD Methods

    ///<inheritdoc/>
    public async Task<SaveOrUpdateResult> SaveOrUpdate<TSource>(TSource source)
    {
      var mapped = _mapper.Map(source, source!.GetType(), source!.GetAttribute<MapToAttribute>()!.EntityType);
      var result = await _applicationRepository.SaveOrUpdateAndCommitAsync(mapped);

      return new SaveOrUpdateResult
      {
        Success = result.Succeeded,
        ErrorMessage = result.ExceptionMessage,
        SavedObjectId = result.SavedObjectId,
      };
    }

    ///<inheritdoc/>
    public async Task<SaveOrUpdateResult> Delete<TSource>(TSource source)
    {
      var result = _applicationRepository.DeleteAndCommit(_mapper.Map(source, source!.GetType(), source!.GetAttribute<MapToAttribute>()!.EntityType));

      return await Task.FromResult(new SaveOrUpdateResult
      {
        Success = result.Succeeded,
        ErrorMessage = result.ExceptionMessage,
      });
    }

    #endregion
  }
}
