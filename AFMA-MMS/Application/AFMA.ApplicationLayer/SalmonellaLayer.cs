namespace AFMA.ApplicationLayer
{
  using System.Drawing;
  using AFMA.ApplicationLayer.Expressions;
  using AFMA.ApplicationLayer.Interface;
  using AFMA.Data.Entities;
  using AFMA.Infrastructure.Options;
  using Alpha.Services.Models.Data.PageData;
  using Alpha.Services.Models.Repository;
  using Alpha.Services.NHibernate.Repository.Services;
  using Alpha.Services.Notifications.Smtp;
  using Alpha.Services.Notifications.Templates;
  using AutoMapper;
  using Helpers;
  using Infrastructure.Settings;
  using Microsoft.AspNetCore.Mvc.Rendering;
  using Microsoft.Extensions.Options;
  using NHibernate.Transform;
  using OfficeOpenXml;
  using OfficeOpenXml.Style;
  using Razor.Templating.Core;
  using Shared.Infrastructure.Settings;
  using Templating.Models;
  using ViewModels;
  using X.PagedList;
  using Color = System.Drawing.Color;
  using Month = Data.Entities.Month;

  public class SalmonellaLayer : ISalmonellaLayer
  {
    private readonly IApplicationRepository<SessionFactory> _applicationRepository;
    private readonly ApplicationSettings _applicationSettings;
    private readonly SmtpSettings _smtpSettings;
    private readonly IMapper _mapper;
    private readonly IIdentityLayer _identityLayer;
    private readonly IApplicationLayer _applicationLayer;
    private readonly IEmailClient _emailClient;
    private readonly IAlphaNotificationTemplateService _notificationTemplateService;
    private readonly IRazorTemplateEngine _razorTemplateEngine;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationLayer"/> class.
    /// </summary>
    /// <param name="applicationRepository">The application repository.</param>
    /// <param name="mapper">The Automapper interface..</param>
    public SalmonellaLayer(
      IApplicationRepository<SessionFactory> applicationRepository,
      IOptions<ApplicationSettings> applicationSettings,
      IOptions<SmtpSettings> smtpSettings,
      IMapper mapper,
      IIdentityLayer identityLayer,
      IApplicationLayer applicationLayer,
      IEmailClient emailClient,
      IAlphaNotificationTemplateService notificationTemplateService,
      IRazorTemplateEngine razorTemplateEngine)
    {
      _applicationRepository = applicationRepository;
      _applicationSettings = applicationSettings.Value;
      _smtpSettings = smtpSettings.Value;
      _mapper = mapper;
      _identityLayer = identityLayer;
      _applicationLayer = applicationLayer;
      _emailClient = emailClient;
      _notificationTemplateService = notificationTemplateService;
      _razorTemplateEngine = razorTemplateEngine;
    }

    public async Task<IEnumerable<SalmonellaSampleTypeModel>> GetSalmonellaSampleTypes()
    {
      var results = await _applicationRepository.QueryAsync<SalmonellaSampleType>(x => x.Active).ConfigureAwait(false);
      return _mapper.Map<IEnumerable<SalmonellaSampleType>, List<SalmonellaSampleTypeModel>>(results);
    }

    #region Select Lists

    public async Task<IEnumerable<SelectListItem>> GenerateSalmonellaTypeSelectList(int? selectedValue)
    {
      var results = await _applicationRepository.QueryAsync<SalmonellaType>();
      return results.Where(x => x.Active).OrderBy(n => n.Name).ToList().ToSelectList(x => x.Id, x => x.Name, selectedValue);
    }

    public async Task<IEnumerable<SelectListItem>> GenerateSalmonellaGroupSelectList(int? selectedValue)
    {
      var results = await _applicationRepository.QueryAsync<SalmonellaGroup>();
      return results.Where(x => x.Active).OrderBy(n => n.Name).ToList().ToSelectList(x => x.Id, x => x.Name, selectedValue);
    }

    public async Task<IEnumerable<SelectListItem>> GenerateProductGroupSelectList(int? selectedValue)
    {
      return await _applicationRepository.QuerySelectListAsync<ProductGroup>(
        q => q.Where(q => !q.HideOnSalmonella && q.VisibleToMembers).OrderBy(d => d.DisplayOrder).ThenBy(n => n.Name), x => x.Id, x => $"{x.Code} - {x.Name}", selectedValue);
    }

    public async Task<IEnumerable<SelectListItem>> GenerateMaterialSelectList(int? selectedValue)
    {
      return await _applicationRepository.QuerySelectListAsync<Material>(
        q => q.Where(q => !q.HideOnSalmonella && q.VisibleToMembers).OrderBy(d => d.DisplayOrder).ThenBy(n => n.Name), x => x.Id,
        x => $"{x.Code} - {x.Name}", selectedValue);
    }

    public async Task<IEnumerable<SelectListItem>> GenerateSalmonellaReportGroupSelectList(int? selectedValue)
    {
      return await _applicationRepository.QuerySelectListAsync<SalmonellaReportGroup>(
        q => q.Where(q => q.Active).OrderBy(d => d.Name), x => x.Id, x => x.Name, selectedValue);
    }

    public async Task<IEnumerable<SelectListItem>> GenerateSalmonellaProductGroupCategorySelectList(int? selectedValue)
    {
      return await _applicationRepository.QuerySelectListAsync<SalmonellaProductGroupCategory>(
        q => q.Where(q => q.Active).OrderBy(d => d.Name), x => x.Id, x => x.Name, selectedValue);
    }


    #endregion

    #region Stored Procedure and SQL Queries

    public T? ExecuteSqlQuery<T>(string sqlQuery)
    {
      try
      {
        if (string.IsNullOrEmpty(sqlQuery))
        {
          return default(T?);
        }

        var result = _applicationRepository.Session().CreateSQLQuery(sqlQuery).List<T>();
        return result[0];
      }
      catch
      {
        return default(T?);
      }
    }

    #endregion

    #region Finished Products

    /// <inheritdoc cref="GetFinishedProducts"/>
    public async Task<IEnumerable<ProductGroupModel>> GetFinishedProductGroups()
    {
      var result = await _applicationRepository.QueryAsync<ProductGroup>().ConfigureAwait(false);
      return _mapper.Map<IEnumerable<ProductGroup>, List<ProductGroupModel>>(result
        .Where(x => !x.HideOnSalmonella && x.VisibleToMembers).OrderBy(a => a.DisplayOrder).ThenBy(b => b.Name));
    }

    #endregion

    #region Salmonella Types

    /// <inheritdoc />
    public async Task<SalmonellaTypeModel> GetSalmonellaType(int typeId)
    {
      var result = await _applicationRepository.QuerySingleAsync<SalmonellaType>(x => x.Id == typeId).ConfigureAwait(false);
      return _mapper.Map<SalmonellaType, SalmonellaTypeModel>(result);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<SalmonellaTypeModel>> GetSalmonellaTypes()
    {
      var result = await _applicationRepository.QueryAsync<SalmonellaType>().ConfigureAwait(false);
      return _mapper.Map<IEnumerable<SalmonellaType>, List<SalmonellaTypeModel>>(result.OrderBy(o => o.DisplayOrder).ThenBy(n => n.Name));
    }

    /// <inheritdoc />
    public async Task<IEnumerable<SalmonellaTypeModel>> GetSalmonellaTypes(AlphaPageRequest pageRequest, int? salmonellaGroupId = null, string? searchText = null)
    {
      var result = await _applicationRepository.QueryPagedAsync(pageRequest, SearchExpression.FilterSalmonellaTypes(salmonellaGroupId, searchText ?? ""));
      var resultMapped = _mapper.Map<IEnumerable<SalmonellaType>, List<SalmonellaTypeModel>>(result.PageData);
      return new StaticPagedList<SalmonellaTypeModel>(resultMapped, pageRequest.PageNumber, (int)pageRequest.PageLength!, result.TotalResults);
    }

    /// <inheritdoc />
    public async Task<RepositoryResult> DeleteSalmonellaType(int typeId)
    {
      var result = await _applicationRepository.QuerySingleAsync<SalmonellaType>(x => x.Id == typeId).ConfigureAwait(false);
      return _applicationRepository.DeleteAndCommit(result);
    }

    #endregion

    #region Salmonella Groups

    /// <inheritdoc />
    public async Task<SalmonellaGroupModel> GetSalmonellaGroup(int groupId)
    {
      var result = await _applicationRepository.QuerySingleAsync<SalmonellaGroup>(x => x.Id == groupId).ConfigureAwait(false);
      return _mapper.Map<SalmonellaGroup, SalmonellaGroupModel>(result);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<SalmonellaGroupModel>> GetSalmonellaGroups()
    {
      var result = await _applicationRepository.QueryAsync<SalmonellaGroup>().ConfigureAwait(false);
      return _mapper.Map<IEnumerable<SalmonellaGroup>, List<SalmonellaGroupModel>>(result.OrderBy(o => o.DisplayOrder).ThenBy(n => n.Name));
    }

    /// <inheritdoc />
    public async Task<IEnumerable<SalmonellaGroupModel>> GetSalmonellaGroups(AlphaPageRequest pageRequest, string? searchText = null)
    {
      var result = await _applicationRepository.QueryPagedAsync(pageRequest, SearchExpression.FilterSalmonellaGroups(searchText ?? ""));
      var resultMapped = _mapper.Map<IEnumerable<SalmonellaGroup>, List<SalmonellaGroupModel>>(result.PageData);
      return new StaticPagedList<SalmonellaGroupModel>(resultMapped, pageRequest.PageNumber, (int)pageRequest.PageLength!, result.TotalResults);
    }

    /// <inheritdoc />
    public async Task<RepositoryResult> DeleteSalmonellaGroup(int groupId)
    {
      var result = await _applicationRepository.QuerySingleAsync<SalmonellaGroup>(x => x.Id == groupId).ConfigureAwait(false);
      return _applicationRepository.DeleteAndCommit(result);
    }

    #endregion

    #region Salmonella Admin Reports

    #region Months (Seasonal)

    public async Task<IEnumerable<MonthModel>> GetSeasonalMonths()
    {
      var result = await _applicationRepository.QueryAsync<Month>();
      return _mapper.Map<IEnumerable<Month>, List<MonthModel>>(result).OrderBy(x => x.MonthNumber);
    }

    #endregion

    /// <inheritdoc cref="GetSalmonellaReportsLookup"/>
    public async Task<IEnumerable<SalmonellaReportLookupModel>> GetSalmonellaReportsLookup()
    {
      var result = await _applicationRepository.QueryAsync<SalmonellaReportLookup>();
      return _mapper.Map<IEnumerable<SalmonellaReportLookup>, List<SalmonellaReportLookupModel>>(result);
    }

    /// <inheritdoc cref="GetSalmonellaReportsLookup"/>
    public async Task<IEnumerable<SalmonellaReportLookupModel>> GetSalmonellaReportsLookup(int year, int memberBranchId)
    {
      var result = await _applicationRepository.QueryAsync<SalmonellaReportLookup>(x => x.MemberBranch.Id == memberBranchId && x.ReportYear == year);
      return _mapper.Map<IEnumerable<SalmonellaReportLookup>, List<SalmonellaReportLookupModel>>(result);
    }

    /// <inheritdoc cref="GetSalmonellaReportsLookup"/>
    public async Task<IEnumerable<SalmonellaReportLookupModel>> GetSalmonellaReportsLookup(int? memberId, int? memberBranchId, int? memberAccountId, bool? isSubmitted, int? currentMonth = null, int? currentYear = null)
    {
      var result = await _applicationRepository.QueryAsync<SalmonellaReportLookup>();

      if (memberId != null)
      {
        result = result.Where(x => x.Member.Id == memberId);
      }

      if (memberBranchId != null)
      {
        result = result.Where(x => x.MemberBranch.Id == memberBranchId);
      }

      if (memberAccountId != null)
      {
        result = result.Where(x => x.MemberAccount.Id == memberAccountId);
      }

      if (currentMonth != null)
      {
        result = result.Where(x => x.ReportMonth == currentMonth);
      }

      if (currentYear != null)
      {
        result = result.Where(x => x.ReportYear == currentYear);
      }

      if (isSubmitted != null)
      {
        result = result.Where(x => x.IsSubmitted == isSubmitted);
      }

      return _mapper.Map<IEnumerable<SalmonellaReportLookup>, List<SalmonellaReportLookupModel>>(result);
    }

    /// <summary>
    /// Returns a filtered list of feedsales report for a specific member account.
    /// </summary>
    /// <param name="pageRequest">The pages request.</param>
    /// <param name="memberId">The member Id.</param>
    /// <param name="memberBranchId">The member branch Id.</param>
    /// <param name="memberAccountId">The member accunt Id.</param>
    /// <param name="isSubmitted">The submitted indicator.</param>
    /// <returns>A paged results with feedsales reports.</returns>
    public async Task<IEnumerable<SalmonellaReportLookupModel>> GetSalmonellaReportsLookup(AlphaPageRequest pageRequest, int memberId, int memberBranchId, int memberAccountId, bool? isSubmitted)
    {
      var result = await _applicationRepository.QueryPagedAsync(pageRequest,
        SearchExpression.FilterSalmonellaReports(memberId, memberBranchId, memberAccountId, isSubmitted));
      var resultMapped = _mapper.Map<IEnumerable<SalmonellaReportLookup>, List<SalmonellaReportLookupModel>>(result.PageData);
      return new StaticPagedList<SalmonellaReportLookupModel>(resultMapped, pageRequest.PageNumber, (int)pageRequest.PageLength!, result.TotalResults);
    }

    public async Task<IEnumerable<SalmonellaReportLookupModel>> GetSalmonellaReportsLookup(AlphaPageRequest pageRequest, int? memberId, int? memberBranchId, bool? isSubmitted)
    {
      var result = await _applicationRepository.QueryPagedAsync(pageRequest,
        SearchExpression.FilterSalmonellaReports(memberId, memberBranchId, isSubmitted));
      var resultMapped = _mapper.Map<IEnumerable<SalmonellaReportLookup>, List<SalmonellaReportLookupModel>>(result.PageData);
      return new StaticPagedList<SalmonellaReportLookupModel>(resultMapped, pageRequest.PageNumber, (int)pageRequest.PageLength!, result.TotalResults);
    }

    public async Task<SalmonellaReportModel> GetSalmonellaReport(int id)
    {
      var result = await _applicationRepository.QuerySingleAsync<SalmonellaReport>(x => x.Id == id);
      return _mapper.Map<SalmonellaReport, SalmonellaReportModel>(result);
    }
    public async Task<IEnumerable<SalmonellaSampleModel>> GetSalmonellaReportSamples(int reportYear, int? memberId, int? branchId)
    {
      var result = await _applicationRepository.QueryAsync<SalmonellaSample>(x
        => x.SalmonellaReport.ReportYear == reportYear
        && (memberId == null || x.SalmonellaReport.Member.Id == memberId) 
        && (branchId == null || x.SalmonellaReport.MemberBranch.Id == branchId))
        .ConfigureAwait(false);

      return _mapper.Map<IEnumerable<SalmonellaSample>, List<SalmonellaSampleModel>>(result);
    }

    public async Task<SalmonellaReportLookupModel> GetSalmonellaReportLookup(int id)
    {
      var result = await _applicationRepository.QuerySingleAsync<SalmonellaReportLookup>(x => x.Id == id);
      return _mapper.Map<SalmonellaReportLookup, SalmonellaReportLookupModel>(result);
    }

    public async Task<SalmonellaReportLookupModel> GetSalmonellaReportLookup(int id, int memberBranchId)
    {
      var result = await _applicationRepository.QuerySingleAsync<SalmonellaReportLookup>(x => x.Id == id && x.MemberBranch.Id == memberBranchId);
      return _mapper.Map<SalmonellaReportLookup, SalmonellaReportLookupModel>(result);
    }

    public async Task<SalmonellaReportModel> GetSalmonellaReport(int id, int memberBranchId)
    {
      var result = await _applicationRepository.QuerySingleAsync<SalmonellaReport>(x => x.Id == id && x.MemberBranch.Id == memberBranchId);
      return _mapper.Map<SalmonellaReport, SalmonellaReportModel>(result);
    }

    public async Task<bool> DeleteSalmonellaSample(int id)
    {
      var sampleItem = await _applicationRepository.QuerySingleAsync<SalmonellaSample>(x => x.Id == id);
      return _applicationRepository.DeleteAndCommit(sampleItem).Succeeded;
    }

    public async Task<SaveOrUpdateResult> UpdateSalmonellaReportStatus(int id, bool submitStatus)
    {
      var report = await _applicationRepository.QuerySingleAsync<SalmonellaReport>(x => x.Id == id);

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

    public async Task<RepositoryResult> DeleteSalmonellaReport(int id)
    {
      var report = await _applicationRepository.QuerySingleAsync<SalmonellaReport>(x => x.Id == id);
      return _applicationRepository.DeleteAndCommit(report);
    }

    //[Obsolete("Not used", true)]
    //public async Task<bool> SendSalmonellaSubmissionReminder(int salmonellaReportId)
    //{
    //  //TODO: Remove this!!
    //  try
    //  {
    //    var report = await _applicationRepository
    //      .QuerySingleAsync<SalmonellaReportLookup>(x => x.Id == salmonellaReportId)
    //      .ConfigureAwait(false);

    //    var submissionEmail = await _razorTemplateEngine.RenderAsync("/Templates/GenericEmail.cshtml",
    //      new GenericEmailModel
    //      {
    //        BaseUrl = _applicationSettings.BaseUrl,
    //        Body = Templating.EmailContent.Submissions.GenerateSalmonellaEmail((int)report.ReportMonth, (int)report.ReportYear),
    //      }).ConfigureAwait(false);

    //    var recipient = report.MemberAccount.ApplicationUser.Email;

    //    if (_smtpSettings.Debug && !string.IsNullOrEmpty(_smtpSettings.DebugEmail))
    //    {
    //      recipient = _smtpSettings.DebugEmail;
    //    }

    //    await _emailClient.SendAsync(recipient,
    //        $"AFMA SALMONELLA: Outstanding Report for {report.Member.Name} - {report.MemberBranch.Name}",
    //        submissionEmail, null, new List<string> { "operations@resolvetech.co.za" })
    //      .ConfigureAwait(false);

    //    return true;
    //  }
    //  catch
    //  {
    //    return false;
    //  }
    //}

    public async Task<IEnumerable<SalmonellaProductGroupCategoryModel>> GetSalmonellaProductGroupCategories()
    {
      var result = await _applicationRepository.QueryAsync<SalmonellaProductGroupCategory>().ConfigureAwait(false);
      return _mapper.Map<IEnumerable<SalmonellaProductGroupCategory>, List<SalmonellaProductGroupCategoryModel>>(result);
    }

    #region Salmonella Admin Reports Queries

    public async Task<IEnumerable<SalmonellaYearByMonthExcelReversedViewModel>> GetSalmonellaYearByMonthReversedExcel(DateTime startDate, DateTime endDate, int? memberBranchId)
    {
      //*** This report works!!!
      //***** MAIN REPORT FUNCTION

      var formattedEndDate = endDate.AddHours(23).AddMinutes(59);

      var sampleTypes = await _applicationRepository.QueryAsync<SalmonellaSampleType>().ConfigureAwait(false); //get the sample types, eg. RAW MATERIAL, FINISHED PRODUCTS, DUST SWABS
      var salmonellaReports = await _applicationRepository.QueryAsync<SalmonellaReport>(x => x.InfoFor >= startDate && x.InfoFor <= formattedEndDate).ConfigureAwait(false); //get all the submitted reports between the dates

      if (memberBranchId != null)
      {
        salmonellaReports = salmonellaReports.Where(a => a.MemberBranch.Id == memberBranchId);
      }

      if (!salmonellaReports.Any())
      {
        return new List<SalmonellaYearByMonthExcelReversedViewModel>();
      }

      var distinctReportYears = salmonellaReports.ToList().Select(x => x.InfoFor.Year).Distinct().OrderBy(m => m).ToList(); //get distinct years between report dates
      var months = await _applicationRepository.QueryAsync<Data.Entities.Month>().ConfigureAwait(false);

      //--*** The main view model that we will pass to the controller and the view
      var mainModel = new List<SalmonellaYearByMonthExcelReversedViewModel>();
      var yearMonthValues = new List<YearMonthValue>();
      //--***

      foreach (var sampleType in sampleTypes)
      {
        foreach (var year in distinctReportYears)
        {
          foreach (var month in months)
          {
            var y = new YearMonthValue
            {
              Year = year,
              SampleType = sampleType.Name,
              SampleTypeId = sampleType.Id,
              Month = month.MonthNumber,
            };

            var samplesTotal =
              (from report in salmonellaReports.Where(d =>
                  d.InfoFor.Year == year && d.InfoFor.Month == month.MonthNumber && d.IsSubmitted)
               from sample in report.ReportItems.Where(s => s.SalmonellaSampleType.Id == sampleType.Id)
               select new { report, sample }).ToLookup(k => k.sample, k => k.report);

            var samplesPositive = (from report in salmonellaReports.Where(d =>
                  d.InfoFor.Year == year && d.InfoFor.Month == month.MonthNumber && d.IsSubmitted)
                                   from sample in report.ReportItems.Where(s =>
                                     s.SalmonellaSampleType.Id == sampleType.Id && s.SalmonellaDetected)
                                   select new { report, sample })
              .ToLookup(k => k.sample, k => k.report);

            y.TotalTested = samplesTotal.Count();
            y.TotalPositive = samplesPositive.Count();
            yearMonthValues.Add(y);
          }
        }

        var xx = new SalmonellaYearByMonthExcelReversedViewModel
        {
          SampleType = sampleType.Name,
          SampleTypeId = sampleType.Id,
          Years = distinctReportYears.ToArray(),
          SamplesTestedPositive = yearMonthValues,
        };

        mainModel.Add(xx);
      }

      return mainModel;
    }

    public async Task<IEnumerable<SalmonellaSeasonalViewModel>> GetSalmonellaYearByMonthSeasonal(DateTime startDate, DateTime endDate, int? memberBranchId = null)
    {
      //*** This report works!!!
      //***** MAIN REPORT FUNCTION

      var formattedEndDate = endDate.AddHours(23).AddMinutes(59);

      var sampleTypes = await _applicationRepository.QueryAsync<SalmonellaSampleType>().ConfigureAwait(false); //get the sample types, eg. RAW MATERIAL, FINISHED PRODUCTS, DUST SWABS
      var salmonellaReports = await _applicationRepository.QueryAsync<SalmonellaReport>(x => x.InfoFor >= startDate && x.InfoFor <= formattedEndDate).ConfigureAwait(false); //get all the submitted reports between the dates

      if (memberBranchId != null)
      {
        salmonellaReports = salmonellaReports.Where(a => a.MemberBranch.Id == memberBranchId);
      }

      if (!salmonellaReports.Any())
      {
        return new List<SalmonellaSeasonalViewModel>();
      }

      var distinctReportYears = salmonellaReports.ToList().Select(x => x.InfoFor.Year).Distinct().OrderBy(m => m).ToList(); //get distinct years between report dates
      var months = await _applicationRepository.QueryAsync<Data.Entities.Month>().ConfigureAwait(false);
      var winterMonths = months.ToList().Where(d => d.Season == (int)Season.Winter).Select(s => s.MonthNumber).ToArray();

      //--*** The main view model that we will pass to the controller and the view
      var mainModel = new List<SalmonellaSeasonalViewModel>();
      var yearMonthValues = new List<SalmonellaSeasonalDetail>();
      //--***

      //winter
      yearMonthValues = new List<SalmonellaSeasonalDetail>();
      foreach (var sampleType in sampleTypes)
      {
        foreach (var year in distinctReportYears)
        {
          foreach (var month in months.ToList().Where(d => d.Season == (int)Season.Winter))
          {
            var y = new SalmonellaSeasonalDetail
            {
              Year = year,
              SampleType = sampleType.Name,
              Season = "Winter",
            };

            var samplesTotal = (from report in salmonellaReports.Where(d =>
                  d.InfoFor.Year == year && d.InfoFor.Month == month.MonthNumber && d.IsSubmitted)
                                from sample in report.ReportItems.Where(s => s.SalmonellaSampleType.Id == sampleType.Id)
                                select new { report, sample })
              .ToLookup(k => k.sample, k => k.report);

            var samplesPositive = (from report in salmonellaReports.Where(d =>
                  d.InfoFor.Year == year && d.InfoFor.Month == month.MonthNumber && d.IsSubmitted)
                                   from sample in report.ReportItems.Where(s =>
                                     s.SalmonellaSampleType.Id == sampleType.Id && s.SalmonellaDetected)
                                   select new { report, sample })
              .ToLookup(k => k.sample, k => k.report);

            y.TotalTested = samplesTotal.Count();
            y.TotalPositive = samplesPositive.Count();
            yearMonthValues.Add(y);
          }
        }

        var xx = new SalmonellaSeasonalViewModel
        {
          SampleType = sampleType.Name,
          Years = distinctReportYears.ToArray(),
          SamplesTestedPositive = yearMonthValues
        };
        mainModel.Add(xx);
      }

      //summer
      yearMonthValues = new List<SalmonellaSeasonalDetail>();
      foreach (var sampleType in sampleTypes)
      {
        foreach (var year in distinctReportYears)
        {
          foreach (var month in months.ToList().Where(d => d.Season != (int)Season.Winter))
          {
            var y = new SalmonellaSeasonalDetail
            {
              Year = year,
              SampleType = sampleType.Name,
              Season = "Summer",
            };
            var samplesTotal = (from report in salmonellaReports.Where(d => d.InfoFor.Year == year && d.InfoFor.Month == month.MonthNumber && d.IsSubmitted)
                                from sample in report.ReportItems.Where(s => s.SalmonellaSampleType.Id == sampleType.Id)
                                select new { report, sample })
                                .ToLookup(k => k.sample, k => k.report);

            var samplesPositive = (from report in salmonellaReports.Where(d => d.InfoFor.Year == year && d.InfoFor.Month == month.MonthNumber && d.IsSubmitted)
                                   from sample in report.ReportItems.Where(s => s.SalmonellaSampleType.Id == sampleType.Id && s.SalmonellaDetected == true)
                                   select new { report, sample })
                                  .ToLookup(k => k.sample, k => k.report);
            y.TotalTested = samplesTotal.Count();
            y.TotalPositive = samplesPositive.Count();
            yearMonthValues.Add(y);
          }
        }

        var xx = new SalmonellaSeasonalViewModel
        {
          SampleType = sampleType.Name,
          Years = distinctReportYears.ToArray(),
          SamplesTestedPositive = yearMonthValues,
        };
        mainModel.Add(xx);
      }

      return mainModel;
    }

    public async Task<IEnumerable<SalmonellaMonthlyExcelReversedViewModel>> GetSalmonellaMonthlyReversedExcel(DateTime startDate, DateTime endDate, int? branchId = null)
    {
      var formattedEndDate = endDate.AddHours(23).AddMinutes(59);

      var sampleTypes = await _applicationRepository.QueryAsync<SalmonellaSampleType>().ConfigureAwait(false); //get the sample types, eg. RAW MATERIAL, FINISHED PRODUCTS, DUST SWABS
      var salmonellaReports = await _applicationRepository.QueryAsync<SalmonellaReport>(x => x.InfoFor >= startDate && x.InfoFor <= formattedEndDate).ConfigureAwait(false); //get all the submitted reports between the dates

      if (branchId != null)
      {
        salmonellaReports = salmonellaReports.Where(x => x.MemberBranch.Id == (int)branchId);
      }

      var finishedProductGroups = await _applicationRepository.QueryAsync<ProductGroup>(x => x.Active && x.VisibleToMembers && !x.HideOnSalmonella).ConfigureAwait(false);
      var rawMaterials = await _applicationRepository.QueryAsync<Material>(x => x.Active && x.VisibleToMembers && !x.HideOnSalmonella).ConfigureAwait(false);
      var sampleTypeItems = await _applicationRepository.QueryAsync<SalmonellaSampleTypeItem>(x => x.Active && x.VisibleToMembers).ConfigureAwait(false);
      var reportGroups = await _applicationRepository.QueryAnyAsync<SalmonellaReportGroup>(x => x.Active).ConfigureAwait(false);

      //Int32 _dMonthCount = ((endDate.Year - startDate.Year) * 12) + endDate.Month - startDate.Month;
      var distinctiveMonthCount = Convert.ToInt32(formattedEndDate.Subtract(startDate).Days / (365.25 / 12));
      var returnList = new List<SalmonellaMonthlyExcelReversedViewModel>();

      foreach (var sampleType in sampleTypes)
      {
        var v = new SalmonellaMonthlyExcelReversedViewModel
        {
          SampleType = sampleType.Name,
          SampleTypeId = sampleType.Id,
          Months = distinctiveMonthCount,
          StartDate = startDate,
          SampleTypes = new List<MonthlyValues>(),
        };

        switch (sampleType.Id)
        {
          case 1: //raw material
            foreach (var rawMaterial in rawMaterials)
            {
              var monthlyValue = new MonthlyValues
              {
                SampleTypeId = sampleType.Id,
                SampleType = sampleType.Name,
                Code = rawMaterial.Code,
                Sample = rawMaterial.Name,
              };

              var valueTotals = new List<MonthlyValueTotals>();

              for (int i = 0; i < distinctiveMonthCount; i++)
              {
                var xx = new MonthlyValueTotals
                {
                  MonthNumber = startDate.AddMonths(i).Month,
                  Year = startDate.AddMonths(i).Year,
                };

                var samplesTotal = (from report in salmonellaReports.Where(d => d.InfoFor.Year == startDate.AddMonths(i).Year && d.InfoFor.Month == startDate.AddMonths(i).Month && d.IsSubmitted)
                                    from sample in report.ReportItems.Where(s => s.Material.Id == rawMaterial.Id && s.SalmonellaSampleType.Id == 1)
                                    select new { report, sample })
                                      .ToLookup(k => k.sample, k => k.report);

                var samplesPositive = (from report in salmonellaReports.Where(d => d.InfoFor.Year == startDate.AddMonths(i).Year && d.InfoFor.Month == startDate.AddMonths(i).Month && d.IsSubmitted)
                                       from sample in report.ReportItems.Where(s => s.Material.Id == rawMaterial.Id && s.SalmonellaSampleType.Id == 1 && s.SalmonellaDetected)
                                       select new { report, sample })
                                        .ToLookup(k => k.sample, k => k.report);

                xx.TotalTested = samplesTotal.Count();
                xx.TotalPositive = samplesPositive.Count();
                valueTotals.Add(xx);
              }

              monthlyValue.SamplesTestedPositive = valueTotals;
              v.SampleTypes.Add(monthlyValue);
            }

            returnList.Add(v);

            break;

          case 2: //finished products
            foreach (var finishedProductGroup in finishedProductGroups)
            {
              var monthlyValue = new MonthlyValues
              {
                SampleTypeId = sampleType.Id,
                SampleType = sampleType.Name,
                Code = finishedProductGroup.Code,
                Sample = finishedProductGroup.Name,
              };

              var valueTotals = new List<MonthlyValueTotals>();

              for (int i = 0; i < distinctiveMonthCount; i++)
              {
                var xx = new MonthlyValueTotals
                {
                  MonthNumber = startDate.AddMonths(i).Month,
                  Year = startDate.AddMonths(i).Year,
                };

                var samplesTotal = (from report in salmonellaReports.Where(d => d.InfoFor.Year == startDate.AddMonths(i).Year && d.InfoFor.Month == startDate.AddMonths(i).Month && d.IsSubmitted)
                                    from sample in report.ReportItems.Where(s => s.ProductGroup.Id == finishedProductGroup.Id && s.SalmonellaSampleType.Id == 2)
                                    select new { report, sample })
                                    .ToLookup(k => k.sample, k => k.report);

                var samplesPositive = (from report in salmonellaReports.Where(d => d.InfoFor.Year == startDate.AddMonths(i).Year && d.InfoFor.Month == startDate.AddMonths(i).Month && d.IsSubmitted)
                                       from sample in report.ReportItems.Where(s => s.ProductGroup.Id == finishedProductGroup.Id && s.SalmonellaSampleType.Id == 2 && s.SalmonellaDetected)
                                       select new { report, sample })
                                        .ToLookup(k => k.sample, k => k.report);

                xx.TotalTested = samplesTotal.Count();
                xx.TotalPositive = samplesPositive.Count();
                valueTotals.Add(xx);
              }

              monthlyValue.SamplesTestedPositive = valueTotals;
              v.SampleTypes.Add(monthlyValue);
            }

            returnList.Add(v);

            break;

          case 3: //dust swabs
            foreach (var sampleTypeItem in sampleTypeItems)
            {
              var monthlyValue = new MonthlyValues
              {
                SampleTypeId = sampleType.Id,
                SampleType = sampleType.Name,
                Code = string.Empty,
                Sample = sampleTypeItem.Name,
              };

              var valueTotals = new List<MonthlyValueTotals>();

              for (int i = 0; i < distinctiveMonthCount; i++)
              {
                var xx = new MonthlyValueTotals
                {
                  MonthNumber = startDate.AddMonths(i).Month,
                  Year = startDate.AddMonths(i).Year,
                };

                var samplesTotal = (from report in salmonellaReports.Where(d => d.InfoFor.Year == startDate.AddMonths(i).Year && d.InfoFor.Month == startDate.AddMonths(i).Month && d.IsSubmitted)
                                    from sample in report.ReportItems.Where(s => s.SalmonellaSampleTypeItem.Id == sampleTypeItem.Id && s.SalmonellaSampleType.Id == 3)
                                    select new { report, sample })
                                    .ToLookup(k => k.sample, k => k.report);

                var samplesPositive = (from report in salmonellaReports.Where(d => d.InfoFor.Year == startDate.AddMonths(i).Year && d.InfoFor.Month == startDate.AddMonths(i).Month && d.IsSubmitted)
                                       from sample in report.ReportItems.Where(s => s.SalmonellaSampleTypeItem.Id == sampleTypeItem.Id && s.SalmonellaSampleType.Id == 3 && s.SalmonellaDetected)
                                       select new { report, sample })
                                        .ToLookup(k => k.sample, k => k.report);

                xx.TotalTested = samplesTotal.Count();
                xx.TotalPositive = samplesPositive.Count();
                valueTotals.Add(xx);
              }

              monthlyValue.SamplesTestedPositive = valueTotals;
              v.SampleTypes.Add(monthlyValue);
            }

            returnList.Add(v);
            break;
        }
      }

      return returnList;
    }

    public async Task<IEnumerable<SalmonellaYearlyExcelReversedViewModel>> GetSalmonellaYearlyReportReversedExcel(DateTime startDate, DateTime endDate)
    {
      //*** This report works!!!
      //***** MAIN REPORT FUNCTION
      var formattedEndDate = endDate.AddHours(23).AddMinutes(59);

      var sampleTypes = await _applicationRepository.QueryAsync<SalmonellaSampleType>().ConfigureAwait(false); //get the sample types, eg. RAW MATERIAL, FINISHED PRODUCTS, DUST SWABS
      var salmonellaReports = await _applicationRepository.QueryAsync<SalmonellaReport>(x => x.InfoFor >= startDate && x.InfoFor <= formattedEndDate).ConfigureAwait(false); //get all the submitted reports between the dates
      var distinctReportYears = salmonellaReports.ToList().Select(x => x.InfoFor.Year).Distinct().OrderBy(m => m).ToList(); //get distinct years between report dates

      //Int32[] _t = new Int32[_dYears.Count]; //to store the total values for each year
      //Int32 _counter = 0; //used to store each year's total value in the array element

      var totalRMValues = new List<KeyValuePair<int, int>>();
      var positiveRMValues = new List<KeyValuePair<int, int>>();
      var percentageRMValues = new List<KeyValuePair<int, float>>();
      var totalFPValues = new List<KeyValuePair<int, int>>();
      var positiveFPValues = new List<KeyValuePair<int, int>>();
      var percentageFPValues = new List<KeyValuePair<int, float>>();
      var totalDSValues = new List<KeyValuePair<int, int>>();
      var positiveDSValues = new List<KeyValuePair<int, int>>();
      var percentageDSValues = new List<KeyValuePair<int, float>>();

      var finishedProductGroups = await _applicationRepository.QueryAsync<ProductGroup>(x => x.Active && x.VisibleToMembers && !x.HideOnSalmonella).ConfigureAwait(false);
      var rawMaterials = await _applicationRepository.QueryAsync<Material>(x => x.Active && x.VisibleToMembers && !x.HideOnSalmonella).ConfigureAwait(false);
      var sampleTypeItems = await _applicationRepository.QueryAsync<SalmonellaSampleTypeItem>(x => x.Active).ConfigureAwait(false);

      //--*** The main view model that we will pass to the controller and the view
      var mainModel = new List<SalmonellaYearlyExcelReversedViewModel>();
      //--***

      //---*** Since we know what the view needs, we can hardcode the Samples Tested, - Positive and Percentage positive loop.
      var samplesTested = new List<SalmonellaYearlySamplesTestedViewModel>();

      //*** START GETTING the total samples tested for the years specified
      foreach (var rawMaterial in rawMaterials)
      {
        var x = new SalmonellaYearlySamplesTestedViewModel
        {
          Code = rawMaterial.Code,
          Sample = rawMaterial.Name,
          ReportingGroupId = rawMaterial.SalmonellaReportGroup?.Id,
          ReportingGroupName = rawMaterial.SalmonellaReportGroup?.Name,
          SampleTypeId = 1,
          YearValues = new List<YearValue>(),
        };

        //loop deur jare en kry die aantal tested samples
        foreach (var year in distinctReportYears)
        {
          var samplesTotal = (from report in salmonellaReports.Where(d => d.InfoFor.Year == year && d.IsSubmitted)
                              from sample in report.ReportItems.Where(s => s.Material.Id == rawMaterial.Id && s.SalmonellaSampleType.Id == 1)
                              select new { report, sample })
                              .ToLookup(k => k.sample, k => k.report);

          var samplesPositive = (from report in salmonellaReports.Where(d => d.InfoFor.Year == year && d.IsSubmitted)
                                 from sample in report.ReportItems.Where(s => s.Material.Id == rawMaterial.Id && s.SalmonellaSampleType.Id == 1 && s.SalmonellaDetected)
                                 select new { report, sample })
                                .ToLookup(k => k.sample, k => k.report);

          var yearValue = new YearValue
          {
            Year = year,
            TotalTested = samplesTotal.Count(),
            TotalPositive = samplesPositive.Count(),
          };

          totalRMValues.Add(new KeyValuePair<int, int>(year, samplesTotal.Count));
          positiveRMValues.Add(new KeyValuePair<int, int>(year, samplesPositive.Count));
          try
          {
            percentageRMValues.Add(new KeyValuePair<int, float>(year, samplesPositive.Count / samplesTotal.Count * 100));
          }
          catch
          {
            percentageRMValues.Add(new KeyValuePair<int, float>(year, 0F));
          }

          x.YearValues.Add(yearValue);
        }

        samplesTested.Add(x);
      }

      foreach (var finishedProductGroup in finishedProductGroups)
      {
        var x = new SalmonellaYearlySamplesTestedViewModel
        {
          Code = finishedProductGroup.Code,
          Sample = finishedProductGroup.Name,
          SampleTypeId = 2,
          YearValues = new List<YearValue>(),
        };

        //loop deur jare en kry die aantal tested samples
        foreach (var year in distinctReportYears)
        {
          var samplesTotal = (from report in salmonellaReports.Where(d => d.InfoFor.Year == year && d.IsSubmitted)
                              from sample in report.ReportItems.Where(s => s.ProductGroup.Id == finishedProductGroup.Id && s.SalmonellaSampleType.Id == 2)
                              select new { report, sample })
                              .ToLookup(k => k.sample, k => k.report);

          var samplesPositive = (from report in salmonellaReports.Where(d => d.InfoFor.Year == year && d.IsSubmitted)
                                 from sample in report.ReportItems.Where(s => s.ProductGroup.Id == finishedProductGroup.Id && s.SalmonellaSampleType.Id == 2 && s.SalmonellaDetected)
                                 select new { report, sample })
                               .ToLookup(k => k.sample, k => k.report);

          var yearValue = new YearValue
          {
            Year = year,
            TotalTested = samplesTotal.Count(),
            TotalPositive = samplesPositive.Count(),
          };

          totalFPValues.Add(new KeyValuePair<int, int>(year, samplesTotal.Count));
          positiveFPValues.Add(new KeyValuePair<int, int>(year, samplesPositive.Count));
          x.YearValues.Add(yearValue);
        }

        samplesTested.Add(x);
      }

      foreach (var sampleTypeItem in sampleTypeItems)
      {
        var x = new SalmonellaYearlySamplesTestedViewModel
        {
          Code = string.Empty,
          Sample = sampleTypeItem.Name,
          SampleTypeId = 3,
          YearValues = new List<YearValue>(),
        };

        //loop deur jare en kry die aantal tested samples
        foreach (var year in distinctReportYears)
        {
          var samplesTotal = (from report in salmonellaReports.Where(d => d.InfoFor.Year == year && d.IsSubmitted)
                              from sample in report.ReportItems.Where(s => s.SalmonellaSampleTypeItem.Id == sampleTypeItem.Id && s.SalmonellaSampleType.Id == 3)
                              select new { report, sample })
                             .ToLookup(k => k.sample, k => k.report);

          var samplesPositive = (from report in salmonellaReports.Where(d => d.InfoFor.Year == year && d.IsSubmitted)
                                 from sample in report.ReportItems.Where(s => s.SalmonellaSampleTypeItem.Id == sampleTypeItem.Id && s.SalmonellaSampleType.Id == 3 && s.SalmonellaDetected)
                                 select new { report, sample })
                               .ToLookup(k => k.sample, k => k.report);

          var yearValue = new YearValue
          {
            Year = year,
            TotalTested = samplesTotal.Count(),
            TotalPositive = samplesPositive.Count(),
          };

          totalDSValues.Add(new KeyValuePair<int, int>(year, samplesTotal.Count));
          positiveDSValues.Add(new KeyValuePair<int, int>(year, samplesPositive.Count));
          x.YearValues.Add(yearValue);
        }

        samplesTested.Add(x);
      }

      //*** Creating and adding to Main Model to be passed to view
      var z = new SalmonellaYearlyExcelReversedViewModel
      {
        Description = "Samples Tested",
        Years = new int[distinctReportYears.Count()],
        SamplesTestedPositive = samplesTested,
        RMTotalTested = totalRMValues,
        RMTotalPositive = positiveRMValues,
        RMPositivePercent = percentageRMValues,
        FPTotalTested = totalFPValues,
        FPTotalPositive = positiveFPValues,
        DSTotalTested = totalDSValues,
        DSTotalPositive = positiveDSValues,
      };

      mainModel.Add(z);
      return mainModel;
    }

    public async Task<IEnumerable<SalmonellaRawMaterialGroupReportViewModel>> GetSalmonellaGroupStatistics(DateTime startDate, DateTime endDate)
    {
      var formattedEndDate = endDate.AddHours(23).AddMinutes(59);

      var salmonellaReports = await _applicationRepository.QueryAsync<SalmonellaReport>(x => x.InfoFor >= startDate && x.InfoFor <= formattedEndDate).ConfigureAwait(false); //get all the submitted reports between the dates
      var rawMaterials = await _applicationRepository.QueryAsync<Material>(x => x.Active).ConfigureAwait(false);
      var reportGroups = await _applicationRepository.QueryAsync<SalmonellaReportGroup>(x => x.Active).ConfigureAwait(false);
      var distinctReportYears = salmonellaReports.ToList().Select(x => x.InfoFor.Year).Distinct().OrderBy(m => m).ToList(); //get distinct years between report dates

      var totalRMValues = new List<KeyValuePair<int, int>>();
      var positiveRMValues = new List<KeyValuePair<int, int>>();
      var percentageRMValues = new List<KeyValuePair<int, decimal>>();

      var reportList = new List<SalmonellaRawMaterialGroupReportViewModel>();

      foreach (var reportGroup in reportGroups.OrderBy(x => x.Name))
      {
        var r = new SalmonellaRawMaterialGroupReportViewModel
        {
          GroupId = reportGroup.Id,
          GroupName = reportGroup.Name,
          TestedPositiveSamples = new List<SalmonellaRawMaterialGroupSamplesViewModel>(),
        };

        foreach (var year in distinctReportYears)
        {
          var samplesTotal = (from report in salmonellaReports.Where(d => d.InfoFor.Year == year && d.IsSubmitted)
                              from sample in report.ReportItems
                              where sample.SalmonellaSampleType.Id == 1 && sample.Material.SalmonellaReportGroup.Id == reportGroup.Id
                              select new { report, sample }).ToLookup(k => k.sample, k => k.report);

          var samplesPositive = (from report in salmonellaReports.Where(d => d.InfoFor.Year == year && d.IsSubmitted)
                                 from sample in report.ReportItems.Where(s => s.SalmonellaDetected && s.SalmonellaSampleType.Id == 1 && s.Material.SalmonellaReportGroup.Id == reportGroup.Id)
                                 select new { report, sample }).ToLookup(k => k.sample, k => k.report);

          var yearValue = new SalmonellaRawMaterialGroupSamplesViewModel
          {
            Year = year,
            TotalTested = samplesTotal.Count(),
            TotalPositive = samplesPositive.Count(),
          };
          //y.PercentagePositive = samplesPositive.Count() / samplesTotal.Count();
          totalRMValues.Add(new KeyValuePair<int, int>(year, samplesTotal.Count()));
          positiveRMValues.Add(new KeyValuePair<int, int>(year, samplesPositive.Count()));

          try
          {
            percentageRMValues.Add(new KeyValuePair<int, decimal>(year, yearValue.PercentagePositive));
          }
          catch
          {
            percentageRMValues.Add(new KeyValuePair<int, decimal>(year, 0M));
          }

          r.TestedPositiveSamples.Add(yearValue);
        }

        reportList.Add(r);
      }

      return reportList;
    }

    public int GetTotalSalmonellaByProductGroupByYear(DateTime startDate, DateTime endDate, int year, int productGroupId, int salmonellaDetected)
    {
      try
      {
        var result = _applicationRepository.Session().CreateSQLQuery(
            "DECLARE @return_value int;EXEC @return_value = [dbo].[sp_GetTotalSalmonellaByProductGroupByYear] @StartDate=:startDate, @EndDate=:endDate, @Year=:year, @ProductGroupId=:productGroupId, @SalmonellaDetected=:salmonellaDetected;SELECT 'Return Value' = @return_value")
          .SetDateTime("startDate", startDate)
          .SetDateTime("endDate", endDate)
          .SetInt32("year", year)
          .SetInt32("productGroupId", productGroupId)
          .SetInt32("salmonellaDetected", salmonellaDetected)
          .List<int>();
        return result[0];
      }
      catch
      {
        return 0;
      }
    }

    public int GetTotalSalmonellaTestedByYearByGroupCategory(DateTime startDate, DateTime endDate, int productGroupCategoryId, int year)
    {
      try
      {
        var result = _applicationRepository.Session().CreateSQLQuery(
            "DECLARE @return_value int;EXEC @return_value = [dbo].[sp_GetTotalSalmonellaTestedByYearByGroupCategory] @StartDate=:startDate, @EndDate=:endDate, @ProductGroupCategoryId=:productGroupCategoryId, @Year=:year;SELECT 'Return Value' = @return_value")
          .SetDateTime("startDate", startDate)
          .SetDateTime("endDate", endDate)
          .SetInt32("productGroupCategoryId", productGroupCategoryId)
          .SetInt32("year", year)
          .List<int>();
        return result[0];
      }
      catch
      {
        return 0;
      }
    }

    public int GetTotalSalmonellaPositiveByYearByGroupCategory(DateTime startDate, DateTime endDate, int productGroupCategoryId, int year)
    {
      try
      {
        var result = _applicationRepository.Session().CreateSQLQuery(
            "DECLARE @return_value int;EXEC @return_value = [dbo].[sp_GetTotalSalmonellaPositiveByYearByGroupCategory] @StartDate=:startDate, @EndDate=:endDate, @ProductGroupCategoryId=:productGroupCategoryId, @Year=:year;SELECT 'Return Value' = @return_value")
          .SetDateTime("startDate", startDate)
          .SetDateTime("endDate", endDate)
          .SetInt32("productGroupCategoryId", productGroupCategoryId)
          .SetInt32("year", year)
          .List<int>();
        return result[0];
      }
      catch
      {
        return 0;
      }
    }

    public IEnumerable<SalmonellaDetailSubmissionSummaryViewModel> GetSalmonellaDetailSubmissionSummary(DateTime startDate, DateTime endDate, int sampleTypeId, int sampleId)
    {
      try
      {
        var result = _applicationRepository.Session().CreateSQLQuery(
            "DECLARE @return_value int;EXEC @return_value = [dbo].[sp_GetDetailedSalmonellaSubmissions] @StartDate=:startDate, @EndDate=:endDate, @SampleTypeId=:sampleTypeId;SELECT 'Return Value' = @return_value")
          .SetResultTransformer(Transformers.AliasToBean(typeof(SalmonellaDetailSubmissionSummaryViewModel)))
          .SetDateTime("startDate", startDate)
          .SetDateTime("endDate", endDate)
          .SetInt32("sampleTypeId", sampleTypeId)
          .List<SalmonellaDetailSubmissionSummaryViewModel>();

        if (sampleId <= 0)
        {
          return result;
        }

        switch (sampleTypeId)
        {
          case 1:
            result = result.Where(x => x.MaterialId == sampleId).ToList();
            break;
          case 2:
            result = result.Where(x => x.ProductGroupId == sampleId).ToList();
            break;
        }

        return result;
      }
      catch
      {
        //return new List<SalmonellaDetailSubmissionSummaryViewModel>();
      }

      return new List<SalmonellaDetailSubmissionSummaryViewModel>();
    }

    #endregion

    #region Salmonella Excel Reports

    public async Task<byte[]> GenerateTotalSalmonellaExcelReport(ReportCriteriaModel reportCriteria)
    {
      System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();

      //YEAR BY MONTH FUNCTIONS
      var yearByMonthReport = await GetSalmonellaYearByMonthReversedExcel(reportCriteria.StartDate, reportCriteria.EndDate, reportCriteria.MemberBranchId).ConfigureAwait(false);
      var seasonalData = await GetSalmonellaYearByMonthSeasonal(reportCriteria.StartDate, reportCriteria.EndDate, reportCriteria.MemberBranchId).ConfigureAwait(false);

      var totalMonthsInReport = 12;
      var winterStartCol1 = 0;
      var winterEndCol1 = 0;

      var winterStartCol2 = 0;
      var winterEndCol2 = 0;

      var winterStartCol3 = 0;
      var winterEndCol3 = 0;

      //YEARLY REPORT FUNCTIONS
      var yearlyReport = await GetSalmonellaYearlyReportReversedExcel(reportCriteria.StartDate, reportCriteria.EndDate);
      var reportGroups = await GetSalmonellaGroupStatistics(reportCriteria.StartDate, reportCriteria.EndDate).ConfigureAwait(false);
      var totalYearsInReport = (reportCriteria.EndDate.Year - reportCriteria.StartDate.Year - 1) + (((reportCriteria.EndDate.Month > reportCriteria.StartDate.Month) || ((reportCriteria.EndDate.Month == reportCriteria.StartDate.Month) && (reportCriteria.EndDate.Day >= reportCriteria.StartDate.Day))) ? 1 : 0) + 1;

      //MONTHLY REPORT FUNCTIONS
      var monthlyReport = await GetSalmonellaMonthlyReversedExcel(reportCriteria.StartDate, reportCriteria.EndDate).ConfigureAwait(false);
      int totalMonthsInMonthlyReport = Convert.ToInt32(reportCriteria.EndDate.Subtract(reportCriteria.StartDate).Days / (365.25 / 12));
      var reportDates = new List<DateTime>();
      //DateTime _startdate = StartDate;
      for (int i = 0; i < totalMonthsInMonthlyReport; i++)
      {
        reportDates.Add(reportCriteria.StartDate.AddMonths(i));
      }

      //GLOBAL FUNCTIONS
      var months = await GetSeasonalMonths();
      int startYear = yearByMonthReport.First().Years[0];
      int yearCount = yearByMonthReport.First().Years.Length;
      //var db = new AFMA.Data.AFMAContext();

      //Int32 totalMonthsInReport = (EndDate.Year - StartDate.Year) + EndDate.Month - StartDate.Month;
      //Int32 totalMonthsInReport = Convert.ToInt32(EndDate.Subtract(StartDate).Days / (365.25 / 12));


      //columns start index
      int samplesTestedStart = 3;
      int samplesPositiveStart = samplesTestedStart + totalMonthsInReport + 2; //1 at the end is for total col, need to add another 1 for space. for 12 months, positive tested will start at 17 
      int percentagePositiveStart = samplesPositiveStart + totalMonthsInReport + 2; //1 at the end is for total, need to add another 1 for space. for 12 months = 31

      ///// Debug /////
      // if months = 12, samples tested will start at column 3, for 12 months, this positive will start at 3 + 12 + 2 = 17
      // if months = 12, samples positive will start at 15, for 10 months, thus percentage will start at 3 + 12 + 2 + 12 + 2 = 31
      // if months = 12, percentage positive will start at 27, for 10 months, it will go to 3 + 10 + 2 + 10 + 2 + 10 + 2

      //Int32 totalRows = materials.Count();

      using (ExcelPackage p = new ExcelPackage())
      {
        p.Workbook.Properties.Author = "AFMA";
        p.Workbook.Properties.Title = "AFMA Total Salmonella Report";
        p.Workbook.Properties.Company = "www.afma.co.za";

        #region YEAR BY MONTH REPORT

        p.Workbook.Worksheets.Add("Year By Month Report");
        ExcelWorksheet ws = p.Workbook.Worksheets[0];
        ws.Name = "Year By Month Report";

        //DEFUALT COLUMN WIDTHS
        ws.Column(1).Width = 17;
        ws.Row(1).Height = 28;
        ws.Row(2).Height = 25;

        //DEFAULT COLORS
        var maroonHeader = ColorTranslator.FromHtml("#c0504d");
        var blueHeader = ColorTranslator.FromHtml("#4f81bd");
        var silverHeader = ColorTranslator.FromHtml("#999999");
        var greenHeader = ColorTranslator.FromHtml("#82bc4f");
        int startRow = 0;
        int endRow = 0;

        //PLACEHOLDERS
        var _rmTotalRowNo = 0;
        var _foTotalRowNo = 0;
        var _dsTotalRowNo = 0;

        //header row - Samples Tested
        //Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#999999");
        var row_header2 = ws.Cells[1, samplesTestedStart];
        row_header2.Value = "Samples Tested";
        row_header2.Style.Font.Bold = true;
        row_header2.Style.Font.Color.SetColor(Color.White);
        row_header2.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        row_header2.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        row_header2.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        row_header2.Style.Fill.BackgroundColor.SetColor(silverHeader);
        ws.Cells[1, 3, 1, samplesPositiveStart - 2].Merge = true;
        ws.Cells[1, 3, 1, samplesPositiveStart - 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

        //header row - Samples Positive
        var row_header1 = ws.Cells[1, samplesTestedStart + totalMonthsInReport + 2];
        row_header1.Value = "Samples Positive";
        row_header1.Style.Font.Bold = true;
        row_header1.Style.Font.Color.SetColor(Color.White);
        row_header1.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        row_header1.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        row_header1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        row_header1.Style.Fill.BackgroundColor.SetColor(silverHeader);
        ws.Cells[1, samplesPositiveStart, 1, percentagePositiveStart - 2].Merge = true;
        ws.Cells[1, samplesPositiveStart, 1, percentagePositiveStart - 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

        //header row - Samples Positive
        var row_header3 = ws.Cells[1, (samplesPositiveStart + totalMonthsInReport + 2)];
        row_header3.Value = "Percentage Positive";
        row_header3.Style.Font.Bold = true;
        row_header3.Style.Font.Color.SetColor(Color.White);
        row_header3.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        row_header3.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        row_header3.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        row_header3.Style.Fill.BackgroundColor.SetColor(silverHeader);
        ws.Cells[1, percentagePositiveStart, 1, percentagePositiveStart + totalMonthsInReport + 1].Merge = true;
        ws.Cells[1, percentagePositiveStart, 1, percentagePositiveStart + totalMonthsInReport + 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

        //second row
        //colFromHex = System.Drawing.ColorTranslator.FromHtml("#c0504d");
        var code_row = ws.Cells[2, 1];
        code_row.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        code_row.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        code_row.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        code_row.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        code_row.Style.Font.Color.SetColor(Color.White);
        code_row.Value = "CODE";

        var ingredient_row = ws.Cells[2, 2];
        ingredient_row.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        ingredient_row.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        ingredient_row.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        ingredient_row.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        ingredient_row.Style.Font.Color.SetColor(Color.White);
        ingredient_row.Value = "YEAR";

        for (int i = 1; i < totalMonthsInReport + 1; i++)
        {
          var month_row1 = ws.Cells[2, i + 2];

          month_row1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          month_row1.Style.Fill.BackgroundColor.SetColor(maroonHeader);
          month_row1.Style.Font.Color.SetColor(Color.White);
          month_row1.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
          month_row1.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
          month_row1.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          month_row1.Value = mfi.GetMonthName(i).Substring(0, 3).ToUpper();
          var season = months.Where(f => f.MonthNumber == i).Single().Season;
          if (season == (int)Season.Winter)
          {
            month_row1.Style.Fill.BackgroundColor.SetColor(greenHeader);
          }

          var month_row2 = ws.Cells[2, i + totalMonthsInReport + 4];
          month_row2.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          month_row2.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          month_row2.Style.Fill.BackgroundColor.SetColor(maroonHeader);
          month_row2.Style.Font.Color.SetColor(Color.White);
          month_row2.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
          month_row2.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
          month_row2.Value = mfi.GetMonthName(i).Substring(0, 3).ToUpper();
          if (season == (int)Season.Winter)
          {
            month_row2.Style.Fill.BackgroundColor.SetColor(greenHeader);
          }


          var month_row3 = ws.Cells[2, i + totalMonthsInReport * 2 + 6];
          month_row3.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          month_row3.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          month_row3.Style.Fill.BackgroundColor.SetColor(maroonHeader);
          month_row3.Style.Font.Color.SetColor(Color.White);
          month_row3.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
          month_row3.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
          month_row3.Value = mfi.GetMonthName(i).Substring(0, 3).ToUpper();
          if (season == (int)Season.Winter)
          {
            month_row3.Style.Fill.BackgroundColor.SetColor(greenHeader);
          }
        }

        //total rows
        var total_cols = ws.Cells[2, totalMonthsInReport + 3];
        total_cols.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        total_cols.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        total_cols.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        total_cols.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        total_cols.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        total_cols.Style.Font.Color.SetColor(Color.White);
        total_cols.Value = "TOTAL";

        var total_positive_col = ws.Cells[2, totalMonthsInReport * 2 + 5];
        total_positive_col.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        total_positive_col.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        total_positive_col.Style.Font.Color.SetColor(Color.White);
        total_positive_col.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        total_positive_col.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        total_positive_col.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        total_positive_col.Value = "TOTAL";

        var total_percentage_col = ws.Cells[2, totalMonthsInReport * 3 + 7];
        total_percentage_col.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        total_percentage_col.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        total_percentage_col.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        total_percentage_col.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        total_percentage_col.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        total_percentage_col.Style.Font.Color.SetColor(Color.White);
        total_percentage_col.Value = "AVERAGE";

        Int32 rowIndex = 3;
        Int32 colIndex = 1;

        foreach (var item in yearByMonthReport.OrderBy(x => x.SampleTypeId))
        {
          //sample type header row
          //colFromHex = System.Drawing.ColorTranslator.FromHtml("#4f81bd");
          var code_row1 = ws.Cells[rowIndex, colIndex];
          code_row1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          code_row1.Style.Fill.BackgroundColor.SetColor(Color.DarkOliveGreen);
          colIndex++;

          ws.Cells[rowIndex, 2, rowIndex, totalMonthsInReport * 3 + 7].Merge = true;
          var sampletype_row = ws.Cells[rowIndex, colIndex];
          sampletype_row.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          sampletype_row.Style.Fill.BackgroundColor.SetColor(blueHeader);
          sampletype_row.Value = item.SampleType;
          sampletype_row.Style.Font.Color.SetColor(Color.White);

          rowIndex++;
          startRow = rowIndex; //used to see where the total columns will be located
          for (int y = startYear; y < (startYear + yearCount); y++)
          {
            colIndex = 1;
            var code_row2 = ws.Cells[rowIndex, colIndex];
            code_row2.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            code_row2.Style.Fill.BackgroundColor.SetColor(Color.DarkOliveGreen);
            colIndex++;

            var year_row = ws.Cells[rowIndex, colIndex];
            year_row.Value = y;
            year_row.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            colIndex++;
            foreach (var month in months)
            {
              var season = months.Where(f => f.MonthNumber == month.MonthNumber).Single().Season;

              foreach (var sample in item.SamplesTestedPositive.Where(d => d.Year == y && d.Month == month.MonthNumber && d.SampleType == item.SampleType))
              {
                var count_row = ws.Cells[rowIndex, colIndex];
                if (sample.TotalTested == 0)
                {
                  count_row.Value = string.Empty;
                }
                else
                {
                  count_row.Style.Numberformat.Format = "#,#";
                  count_row.Value = sample.TotalTested;
                }
                count_row.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                var positive_row = ws.Cells[rowIndex, colIndex + 14];

                if (sample.TotalPositive == 0)
                {
                  positive_row.Value = string.Empty;
                }
                else
                {
                  positive_row.Value = sample.TotalPositive;
                  positive_row.Style.Numberformat.Format = "#,#";
                }

                positive_row.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                var percentage_row = ws.Cells[rowIndex, colIndex + 28];

                if (sample.TotalPositive == 0)
                {
                  percentage_row.Value = string.Empty;
                }
                else
                {
                  percentage_row.Style.Numberformat.Format = "#,#";
                  percentage_row.Value = sample.PercentagePositive;
                }

                percentage_row.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                colIndex++;
              }


            }
            var total_tested_row = ws.Cells[rowIndex, colIndex];
            total_tested_row.Formula = $"SUM(C{rowIndex}:N{rowIndex})";
            total_tested_row.Style.Numberformat.Format = "#,#";
            total_tested_row.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            total_tested_row.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            total_tested_row.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);

            var total_positive_row = ws.Cells[rowIndex, colIndex + 14];
            total_positive_row.Formula = $"SUM(Q{rowIndex}:AB{rowIndex})";
            total_positive_row.Style.Numberformat.Format = "#,#";
            total_positive_row.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            total_positive_row.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            total_positive_row.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);

            var total_percentage_row = ws.Cells[rowIndex, colIndex + 28];
            //total_percentage_row.Formula = String.Format("SUM(AE{0}:AP{0})/12", rowIndex);
            total_percentage_row.Formula = $"({total_positive_row.Address}/{total_tested_row.Address})*100";
            total_percentage_row.Style.Numberformat.Format = "#,#";
            total_percentage_row.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            total_percentage_row.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            total_percentage_row.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);

            rowIndex++;
          }
          endRow = rowIndex - 1;

          //total rows
          if (_rmTotalRowNo == 0)
          {
            _rmTotalRowNo = rowIndex;
            _foTotalRowNo = _rmTotalRowNo + totalYearsInReport + 2;
            _dsTotalRowNo = _foTotalRowNo + totalYearsInReport + 2;
          }

          colIndex = 1;
          var total_row_ds = ws.Cells[rowIndex, colIndex];
          total_row_ds.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          total_row_ds.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
          total_row_ds.Style.Fill.BackgroundColor.SetColor(Color.DarkOliveGreen);
          total_row_ds.Style.Font.Color.SetColor(Color.White);
          total_row_ds.Value = "TOTAL";
          colIndex++;
          colIndex++;

          for (int i = 1; i < totalMonthsInReport + 2; i++) //added 2 here to count the last total as well
          {
            var total_row_ds_value = ws.Cells[rowIndex, colIndex];
            total_row_ds_value.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            total_row_ds_value.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
            total_row_ds_value.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            total_row_ds_value.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
            total_row_ds_value.Style.Font.Color.SetColor(Color.Black);
            total_row_ds_value.Style.Font.Bold = true;
            total_row_ds_value.Formula = $"=SUM({ws.Cells[startRow, colIndex].Address}:{ws.Cells[endRow, colIndex].Address})";
            colIndex++;
          }

          colIndex++;
          for (int i = 1; i < totalMonthsInReport + 2; i++) //added 2 here to count the last total as well
          {
            var total_row_ds_value = ws.Cells[rowIndex, colIndex];
            total_row_ds_value.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            total_row_ds_value.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
            total_row_ds_value.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            total_row_ds_value.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
            total_row_ds_value.Style.Font.Color.SetColor(Color.Black);
            total_row_ds_value.Style.Font.Bold = true;
            total_row_ds_value.Formula = $"=SUM({ws.Cells[startRow, colIndex].Address}:{ws.Cells[endRow, colIndex].Address})";
            total_row_ds_value.Style.Numberformat.Format = "#,#";
            colIndex++;
          }

          colIndex++;
          for (int i = 1; i < totalMonthsInReport + 2; i++) //added 2 here to count the last total as well
          {
            var total_row_ds_value = ws.Cells[rowIndex, colIndex];
            total_row_ds_value.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            total_row_ds_value.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
            total_row_ds_value.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            total_row_ds_value.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
            total_row_ds_value.Style.Font.Color.SetColor(Color.Black);
            total_row_ds_value.Style.Font.Bold = true;
            //total_row_ds_value.Formula = String.Format("({0}/{1})*100", total_positive_row.Address, total_tested_row.Address);
            total_row_ds_value.Formula = $"({ws.Cells[rowIndex, colIndex - 14].Address}/{ws.Cells[rowIndex, colIndex - 28].Address})*100";
            total_row_ds_value.Style.Numberformat.Format = "#,#";
            colIndex++;
          }

          rowIndex += 2;
          colIndex = 1;
        }

        rowIndex++;

        #region MONTHLY/YEARLY TOTALS

        colIndex = 2;
        var my_totals_header = ws.Cells[rowIndex + 1, colIndex - 1];
        my_totals_header.Value = "TOTAL SAMPLES1";
        my_totals_header.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        my_totals_header.Style.Fill.BackgroundColor.SetColor(Color.DarkOliveGreen);
        my_totals_header.Style.Font.Color.SetColor(Color.White);
        my_totals_header.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

        rowIndex++;

        for (int x = 0; x < totalYearsInReport; x++)
        {
          var my_year = ws.Cells[rowIndex, colIndex];
          my_year.Value = startYear + x;
          my_year.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

          colIndex++;

          foreach (var month in months)
          {
            var checkDate = new DateTime(startYear + x, month.MonthNumber, 1);

            if (reportDates.Contains(checkDate))
            {
              var my_row_tested = ws.Cells[rowIndex, colIndex];
              my_row_tested.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              my_row_tested.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
              my_row_tested.Style.Font.Bold = true;
              my_row_tested.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
              my_row_tested.Style.Numberformat.Format = "#,#";
              try
              {
                var sqlQuery = $"DECLARE @return_value int;SELECT COUNT(SalmonellaSample.Id) FROM SalmonellaReport LEFT JOIN SalmonellaSample ON SalmonellaReport.Id = SalmonellaSample.SalmonellaReportId WHERE YEAR(SalmonellaReport.InfoFor) = {startYear + x} AND MONTH(SalmonellaReport.InfoFor) = {month.MonthNumber} AND SalmonellaReport.IsSubmitted = 1;SELECT 'Return Value' = @return_value";
                var totalValue = ExecuteSqlQuery<int>(sqlQuery);
                //var totalValue = context.Database.SqlQuery<Int32>(sqlQuery).FirstOrDefault();
                my_row_tested.Value = totalValue;
              }
              catch
              {
                my_row_tested.Value = "";
              }

              var my_row_positive = ws.Cells[rowIndex, colIndex + 14];
              my_row_positive.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              my_row_positive.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
              my_row_positive.Style.Font.Bold = true;
              my_row_positive.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
              my_row_positive.Style.Numberformat.Format = "#,#";
              try
              {
                var sqlQuery = $"DECLARE @return_value int;SELECT COUNT(SalmonellaSample.Id) FROM SalmonellaReport LEFT JOIN SalmonellaSample ON SalmonellaReport.Id = SalmonellaSample.SalmonellaReportId WHERE YEAR(SalmonellaReport.InfoFor) = {startYear + x} AND MONTH(SalmonellaReport.InfoFor) = {month.MonthNumber} AND SalmonellaSample.SalmonellaDetected = 1 AND SalmonellaReport.IsSubmitted = 1;SELECT 'Return Value' = @return_value";
                var totalValue = ExecuteSqlQuery<int>(sqlQuery);
                //var totalValue = context.Database.SqlQuery<Int32>(sqlQuery).FirstOrDefault();
                my_row_positive.Value = totalValue;
              }
              catch
              {
                my_row_positive.Value = "";
              }

              var my_row_positive_percentage = ws.Cells[rowIndex, colIndex + 28];
              my_row_positive_percentage.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              my_row_positive_percentage.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
              my_row_positive_percentage.Style.Font.Bold = true;
              my_row_positive_percentage.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
              my_row_positive_percentage.Style.Numberformat.Format = "#,#";
              Int32 _startIndex = 3;
              Int32 _increment = 12;
              try
              {
                my_row_positive_percentage.Formula = $"=({ws.Cells[rowIndex, colIndex + 14].Address}/{ws.Cells[rowIndex, colIndex].Address})*100";
                //String sqlQuery = String.Format("DECLARE @return_value int;SELECT COUNT(SalmonellaSample.Id) FROM SalmonellaReport LEFT JOIN SalmonellaSample ON SalmonellaReport.Id = SalmonellaSample.ReportId WHERE YEAR(SalmonellaReport.CreatedDate) = {0} AND MONTH(SalmonellaReport.CreatedDate) = {1} AND SalmonellaSample.SalmonellaDetected = 1;SELECT 'Return Value' = @return_value", startYear + x, month.MonthNumber);
                //var totalValue = db.Database.SqlQuery<Int32>(sqlQuery).FirstOrDefault();
                //my_row_positive_percentage.Value = totalValue;
              }
              catch
              {
                my_row_positive_percentage.Value = "";
              }

              colIndex++;
            }
            else
            {
              var my_row_tested = ws.Cells[rowIndex, colIndex];
              my_row_tested.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              my_row_tested.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
              my_row_tested.Style.Font.Bold = true;
              my_row_tested.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
              my_row_tested.Style.Numberformat.Format = "#,#";
              try
              {
                //String sqlQuery = String.Format("DECLARE @return_value int;SELECT COUNT(SalmonellaSample.Id) FROM SalmonellaReport LEFT JOIN SalmonellaSample ON SalmonellaReport.Id = SalmonellaSample.ReportId WHERE YEAR(SalmonellaReport.InfoFor) = {0} AND MONTH(SalmonellaReport.InfoFor) = {1} AND SalmonellaReport.Submitted = 1;SELECT 'Return Value' = @return_value", startYear + x, month.MonthNumber);
                //var totalValue = context.Database.SqlQuery<Int32>(sqlQuery).FirstOrDefault();
                my_row_tested.Value = "";
              }
              catch
              {
                my_row_tested.Value = "";
              }

              var my_row_positive = ws.Cells[rowIndex, colIndex + 14];
              my_row_positive.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              my_row_positive.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
              my_row_positive.Style.Font.Bold = true;
              my_row_positive.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
              my_row_positive.Style.Numberformat.Format = "#,#";
              try
              {
                //String sqlQuery = String.Format("DECLARE @return_value int;SELECT COUNT(SalmonellaSample.Id) FROM SalmonellaReport LEFT JOIN SalmonellaSample ON SalmonellaReport.Id = SalmonellaSample.ReportId WHERE YEAR(SalmonellaReport.InfoFor) = {0} AND MONTH(SalmonellaReport.InfoFor) = {1} AND SalmonellaSample.SalmonellaDetected = 1 AND SalmonellaReport.Submitted = 1;SELECT 'Return Value' = @return_value", startYear + x, month.MonthNumber);
                //var totalValue = context.Database.SqlQuery<Int32>(sqlQuery).FirstOrDefault();
                my_row_positive.Value = ""; ;
              }
              catch
              {
                my_row_positive.Value = "";
              }

              var my_row_positive_percentage = ws.Cells[rowIndex, colIndex + 28];
              my_row_positive_percentage.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              my_row_positive_percentage.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
              my_row_positive_percentage.Style.Font.Bold = true;
              my_row_positive_percentage.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
              my_row_positive_percentage.Style.Numberformat.Format = "#,#";
              var _startIndex = 3;
              var _increment = 12;
              try
              {
                //my_row_positive_percentage.Formula = String.Format("=({0}/{1})*100", ws.Cells[rowIndex, colIndex + 14].Address, ws.Cells[rowIndex, colIndex].Address);
                //String sqlQuery = String.Format("DECLARE @return_value int;SELECT COUNT(SalmonellaSample.Id) FROM SalmonellaReport LEFT JOIN SalmonellaSample ON SalmonellaReport.Id = SalmonellaSample.ReportId WHERE YEAR(SalmonellaReport.CreatedDate) = {0} AND MONTH(SalmonellaReport.CreatedDate) = {1} AND SalmonellaSample.SalmonellaDetected = 1;SELECT 'Return Value' = @return_value", startYear + x, month.MonthNumber);
                //var totalValue = db.Database.SqlQuery<Int32>(sqlQuery).FirstOrDefault();
                //my_row_positive_percentage.Value = totalValue;
                my_row_positive_percentage.Value = "";
              }
              catch
              {
                my_row_positive_percentage.Value = "";
              }

              colIndex++;
            }
          }

          //totals for the row
          var my_total_tested_row = ws.Cells[rowIndex, colIndex];
          my_total_tested_row.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          my_total_tested_row.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          my_total_tested_row.Style.Font.Bold = true;
          my_total_tested_row.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
          my_total_tested_row.Formula = $"=SUM({ws.Cells[rowIndex, 3].Address}:{ws.Cells[rowIndex, 14].Address})";

          var my_total_positive_row = ws.Cells[rowIndex, colIndex + 14];
          my_total_positive_row.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          my_total_positive_row.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          my_total_positive_row.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
          my_total_positive_row.Style.Font.Bold = true;
          my_total_positive_row.Formula = $"=SUM({ws.Cells[rowIndex, 17].Address}:{ws.Cells[rowIndex, 28].Address})";

          var my_total_percentage_positive = ws.Cells[rowIndex, colIndex + 28];
          Int32 startIndex = 3;




          //bottom total rows
          rowIndex++;

          Int32 _testedStart = 0;
          Int32 _positveStart = 14;
          Int32 _percentageStart = 28;
          colIndex = 3;
          foreach (var month in months)
          {
            var year_total_row_bottom_tested = ws.Cells[rowIndex, colIndex];
            year_total_row_bottom_tested.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            year_total_row_bottom_tested.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            year_total_row_bottom_tested.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
            year_total_row_bottom_tested.Style.Font.Bold = true;
            year_total_row_bottom_tested.Formula = $"=SUM({ws.Cells[rowIndex - totalYearsInReport, colIndex].Address}:{ws.Cells[rowIndex - 1, colIndex].Address})";

            var year_total_row_bottom_positive = ws.Cells[rowIndex, colIndex + _positveStart];
            year_total_row_bottom_positive.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            year_total_row_bottom_positive.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            year_total_row_bottom_positive.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
            year_total_row_bottom_positive.Style.Font.Bold = true;
            year_total_row_bottom_positive.Formula = $"=SUM({ws.Cells[rowIndex - totalYearsInReport, colIndex + _positveStart].Address}:{ws.Cells[rowIndex - 1, colIndex + _positveStart].Address})";

            var year_total_row_bottom_percentage = ws.Cells[rowIndex, colIndex + _percentageStart];
            year_total_row_bottom_percentage.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            year_total_row_bottom_percentage.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            year_total_row_bottom_percentage.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
            year_total_row_bottom_percentage.Style.Font.Bold = true;
            //year_total_row_bottom_percentage.Formula = String.Format("({0}/{1})*100", ws.Cells[rowIndex - totalYearsInReport, colIndex + _percentageStart].Address, ws.Cells[rowIndex - 1, colIndex + _percentageStart].Address, totalYearsInReport);
            year_total_row_bottom_percentage.Formula = $"({ws.Cells[rowIndex, (colIndex + _percentageStart) - 14].Address}/{ws.Cells[rowIndex, (colIndex + _percentageStart) - 28].Address})*100";
            year_total_row_bottom_percentage.Style.Numberformat.Format = "#,#";

            colIndex++;
          }

          //colIndex++;

          var my_grand_total_tested_row = ws.Cells[rowIndex - 1, 15];
          my_total_tested_row.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          my_total_tested_row.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          my_total_tested_row.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
          my_total_tested_row.Style.Font.Bold = true;
          my_total_tested_row.Formula = $"=SUM({ws.Cells[rowIndex - 1, 3].Address}:{ws.Cells[rowIndex - 1, 14].Address})";

          var my_grand_total_positive_row = ws.Cells[rowIndex - 1, 29];
          my_total_positive_row.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          my_total_positive_row.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          my_total_positive_row.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
          my_total_positive_row.Style.Font.Bold = true;
          my_total_positive_row.Formula = $"=SUM({ws.Cells[rowIndex - 1, 17].Address}:{ws.Cells[rowIndex - 1, 28].Address})";

          var my_grand_total_percentage_positive = ws.Cells[rowIndex - 1, 43];
          my_grand_total_percentage_positive.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          my_grand_total_percentage_positive.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          my_grand_total_percentage_positive.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
          my_grand_total_percentage_positive.Style.Font.Bold = true;
          my_grand_total_percentage_positive.Formula = $"({ws.Cells[rowIndex - 1, 29].Address}/{ws.Cells[rowIndex - 1, 15].Address})*100";
          my_grand_total_percentage_positive.Style.Numberformat.Format = "#,#";

          colIndex = 2;
        }

        var grand_total_tested_cell = ws.Cells[rowIndex, 15];
        grand_total_tested_cell.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        grand_total_tested_cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        grand_total_tested_cell.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        grand_total_tested_cell.Style.Numberformat.Format = "#,#";
        grand_total_tested_cell.Style.Font.Bold = true;
        grand_total_tested_cell.Formula = $"=SUM({ws.Cells[rowIndex, 3].Address}:{ws.Cells[rowIndex, 14].Address})";


        var grand_total_positive_cell = ws.Cells[rowIndex, 29];
        grand_total_positive_cell.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        grand_total_positive_cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        grand_total_positive_cell.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        grand_total_positive_cell.Style.Numberformat.Format = "#,#";
        grand_total_positive_cell.Style.Font.Bold = true;
        grand_total_positive_cell.Formula = $"=SUM({ws.Cells[rowIndex, 17].Address}:{ws.Cells[rowIndex, 28].Address})";

        var grand_total_percentage_cell = ws.Cells[rowIndex, 43];
        grand_total_percentage_cell.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        grand_total_percentage_cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        grand_total_percentage_cell.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        grand_total_percentage_cell.Style.Numberformat.Format = "#,#";
        grand_total_percentage_cell.Style.Font.Bold = true;
        grand_total_percentage_cell.Formula = $"({ws.Cells[rowIndex, 29].Address}/{ws.Cells[rowIndex, 15].Address})*100";

        #endregion

        rowIndex++;
        rowIndex++;
        colIndex = 3;

        #region SEASONAL DATA
        //SEASONAL DATA

        //header row - Samples Tested
        ws.Cells[rowIndex, 1, rowIndex, 10].Merge = true;
        ws.Cells[rowIndex, 1, rowIndex, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        var header_row = ws.Cells[rowIndex, 1];
        header_row.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        header_row.Style.Font.Color.SetColor(Color.White);
        header_row.Style.Fill.BackgroundColor.SetColor(Color.DarkOliveGreen);
        header_row.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        header_row.Value = "SEASONAL DATA";
        rowIndex++;
        colIndex = 3;

        var row_header_s1 = ws.Cells[rowIndex, colIndex];
        ws.Row(rowIndex).Height = 22;
        row_header_s1.Value = "Samples Tested";
        row_header_s1.Style.Font.Bold = true;
        row_header_s1.Style.Font.Color.SetColor(Color.White);
        row_header_s1.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        row_header_s1.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        row_header_s1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        row_header_s1.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        row_header_s1.Style.Fill.BackgroundColor.SetColor(greenHeader);
        ws.Cells[rowIndex, colIndex, rowIndex, colIndex + 1].Merge = true;
        ws.Cells[rowIndex, colIndex, rowIndex, colIndex + 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

        colIndex += 3;

        //header row - Samples Positive
        var row_header_s2 = ws.Cells[rowIndex, colIndex];
        row_header_s2.Value = "Samples Positive";
        row_header_s2.Style.Font.Bold = true;
        row_header_s2.Style.Font.Color.SetColor(Color.White);
        row_header_s2.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        row_header_s2.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        row_header_s2.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        row_header_s2.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        row_header_s2.Style.Fill.BackgroundColor.SetColor(greenHeader);
        ws.Cells[rowIndex, colIndex, rowIndex, colIndex + 1].Merge = true;
        ws.Cells[rowIndex, colIndex, rowIndex, colIndex + 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

        colIndex += 3;

        //header row - Samples Positive
        var row_header_s3 = ws.Cells[rowIndex, colIndex];
        row_header_s3.Value = "Percentage Positive";
        row_header_s3.Style.Font.Bold = true;
        row_header_s3.Style.Font.Color.SetColor(Color.White);
        row_header_s3.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        row_header_s3.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        row_header_s3.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        row_header_s3.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        row_header_s3.Style.Fill.BackgroundColor.SetColor(greenHeader);
        ws.Cells[rowIndex, colIndex, rowIndex, colIndex + 1].Merge = true;
        ws.Cells[rowIndex, colIndex, rowIndex, colIndex + 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

        rowIndex++;

        colIndex = 1;

        //second row
        //colFromHex = System.Drawing.ColorTranslator.FromHtml("#c0504d");
        var code_row_s1 = ws.Cells[rowIndex, 1];
        ws.Row(rowIndex).Height = 22;
        code_row_s1.Value = "SAMPLE";
        code_row_s1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        code_row_s1.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        code_row_s1.Style.Fill.BackgroundColor.SetColor(silverHeader);
        code_row_s1.Style.Font.Color.SetColor(Color.White);

        var ingredient_row_s1 = ws.Cells[2, 2];
        ingredient_row_s1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        ingredient_row_s1.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        ingredient_row_s1.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        ingredient_row_s1.Style.Font.Color.SetColor(Color.White);
        ingredient_row_s1.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        ingredient_row_s1.Value = "YEAR";

        colIndex += 2;

        var winter_col1 = ws.Cells[rowIndex, colIndex];
        winter_col1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        winter_col1.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        winter_col1.Style.Font.Color.SetColor(Color.White);
        winter_col1.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        winter_col1.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        winter_col1.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        winter_col1.Value = "Winter";

        winter_col1 = ws.Cells[rowIndex, colIndex + 3];
        winter_col1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        winter_col1.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        winter_col1.Style.Font.Color.SetColor(Color.White);
        winter_col1.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        winter_col1.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        winter_col1.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        winter_col1.Value = "Winter";

        winter_col1 = ws.Cells[rowIndex, colIndex + 6];
        winter_col1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        winter_col1.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        winter_col1.Style.Font.Color.SetColor(Color.White);
        winter_col1.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        winter_col1.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        winter_col1.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        winter_col1.Value = "Winter";

        colIndex++;

        var winter_col2 = ws.Cells[rowIndex, colIndex];
        winter_col2.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        winter_col2.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        winter_col2.Style.Font.Color.SetColor(Color.White);
        winter_col2.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        winter_col2.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        winter_col2.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        winter_col2.Value = "Summer";

        winter_col2 = ws.Cells[rowIndex, colIndex + 3];
        winter_col2.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        winter_col2.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        winter_col2.Style.Font.Color.SetColor(Color.White);
        winter_col2.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        winter_col2.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        winter_col2.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        winter_col2.Value = "Summer";

        winter_col2 = ws.Cells[rowIndex, colIndex + 6];
        winter_col2.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        winter_col2.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        winter_col2.Style.Font.Color.SetColor(Color.White);
        winter_col2.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        winter_col2.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        winter_col2.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        winter_col2.Value = "Summer";

        colIndex = 1;
        rowIndex++;
        //loop thru FP and DS and RM

        //raw materials
        var rmData = seasonalData.Where(x => x.SampleType == "Raw Material").SelectMany(d => d.SamplesTestedPositive);

        var seasonalRowStart = rowIndex;

        for (var f = 0; f < totalYearsInReport; f++)
        {
          var season_rm = ws.Cells[rowIndex, colIndex];
          season_rm.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          season_rm.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          season_rm.Style.Fill.BackgroundColor.SetColor(blueHeader);
          season_rm.Style.Font.Color.SetColor(Color.White);
          season_rm.Value = "Raw Material";
          colIndex++;
          var season_rm2 = ws.Cells[rowIndex, colIndex];
          season_rm2.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          season_rm2.Value = startYear + f;

          colIndex++;
          var season_rm_winter_tested = ws.Cells[rowIndex, colIndex];
          season_rm_winter_tested.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          season_rm_winter_tested.Value = rmData.Where(x => x.Season == "Winter" && x.Year == startYear + f && x.SampleType == "Raw Material").Sum(s => s.TotalTested);
          if (season_rm_winter_tested.Value == "0") { season_rm_winter_tested.Value = ""; }
          season_rm_winter_tested.Style.Numberformat.Format = "#,#";

          colIndex++;
          var season_rm_summer_tested = ws.Cells[rowIndex, colIndex];
          season_rm_summer_tested.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          season_rm_summer_tested.Value = rmData.Where(x => x.Season == "Summer" && x.Year == startYear + f && x.SampleType == "Raw Material").Sum(s => s.TotalTested);
          if (season_rm_summer_tested.Value == "0") { season_rm_summer_tested.Value = ""; }
          season_rm_summer_tested.Style.Numberformat.Format = "#,#";

          colIndex += 2;
          var season_rm_winter_positive = ws.Cells[rowIndex, colIndex];
          season_rm_winter_positive.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          season_rm_winter_positive.Value = rmData.Where(x => x.Season == "Winter" && x.Year == startYear + f && x.SampleType == "Raw Material").Sum(s => s.TotalPositive);
          if (season_rm_winter_positive.Value == "0") { season_rm_winter_positive.Value = ""; }
          season_rm_winter_positive.Style.Numberformat.Format = "#,#";

          colIndex++;
          var season_rm_summer_positive = ws.Cells[rowIndex, colIndex];
          season_rm_summer_positive.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          season_rm_summer_positive.Value = rmData.Where(x => x.Season == "Summer" && x.Year == startYear + f && x.SampleType == "Raw Material").Sum(s => s.TotalPositive);
          if (season_rm_summer_positive.Value == "0") { season_rm_summer_positive.Value = ""; }
          season_rm_summer_positive.Style.Numberformat.Format = "#,#";

          colIndex += 2;
          var season_rm_winter_percentage = ws.Cells[rowIndex, colIndex];
          season_rm_winter_percentage.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          try
          {
            //season_rm_winter_percentage.Value = rmData.Where(x => x.Season == "Winter" && x.Year == startYear + f && x.SampleType == "Raw Material").Average(s => s.PercentagePositive);total_samples_percentage_winter.Formula = String.Format("({0}/{1})*100", ws.Cells[rowIndex, colIndex - 3].Address, ws.Cells[rowIndex, colIndex - 6].Address);
            season_rm_winter_percentage.Formula = $"({ws.Cells[rowIndex, colIndex - 3].Address}/{ws.Cells[rowIndex, colIndex - 6].Address})*100";
            season_rm_winter_percentage.Style.Numberformat.Format = "#,#";
          }
          catch
          {
            season_rm_winter_percentage.Value = "";
          }

          colIndex++;
          var season_rm_summer_percentage = ws.Cells[rowIndex, colIndex];
          season_rm_summer_percentage.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          try
          {
            //season_rm_summer_percentage.Value = Decimal.Round(rmData.Where(x => x.Season == "Summer" && x.Year == startYear + f && x.SampleType == "Raw Material").Average(s => s.PercentagePositive), 0);
            season_rm_summer_percentage.Formula = $"({ws.Cells[rowIndex, colIndex - 3].Address}/{ws.Cells[rowIndex, colIndex - 6].Address})*100";
            season_rm_summer_percentage.Style.Numberformat.Format = "#,#";
          }
          catch
          {
            season_rm_summer_percentage.Value = "";
          }

          colIndex = 1;
          rowIndex++;
        }

        Int32 seasonalRowEnd = rowIndex;

        //Seasonal RM totals
        var rm_seasonal_total_header = ws.Cells[rowIndex, colIndex];
        rm_seasonal_total_header.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        rm_seasonal_total_header.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        rm_seasonal_total_header.Style.Fill.BackgroundColor.SetColor(Color.DarkOliveGreen);
        rm_seasonal_total_header.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
        rm_seasonal_total_header.Style.Font.Color.SetColor(Color.White);
        rm_seasonal_total_header.Value = "TOTAL";

        colIndex++;
        colIndex++;
        var rm_seasonal_total_tested_winter = ws.Cells[rowIndex, colIndex];
        rm_seasonal_total_tested_winter.Formula = $"=SUM({ws.Cells[rowIndex - totalYearsInReport, colIndex].Address}:{ws.Cells[rowIndex - 1, colIndex].Address})";
        rm_seasonal_total_tested_winter.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        rm_seasonal_total_tested_winter.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        rm_seasonal_total_tested_winter.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        rm_seasonal_total_tested_winter.Style.Numberformat.Format = "#,#";

        colIndex++;
        var rm_seasonal_total_tested_summer = ws.Cells[rowIndex, colIndex];
        rm_seasonal_total_tested_summer.Formula = $"=SUM({ws.Cells[rowIndex - totalYearsInReport, colIndex].Address}:{ws.Cells[rowIndex - 1, colIndex].Address})";
        rm_seasonal_total_tested_summer.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        rm_seasonal_total_tested_summer.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        rm_seasonal_total_tested_summer.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        rm_seasonal_total_tested_summer.Style.Numberformat.Format = "#,#";


        colIndex++;
        colIndex++;
        var rm_seasonal_total_positive_winter = ws.Cells[rowIndex, colIndex];
        rm_seasonal_total_positive_winter.Formula = $"=SUM({ws.Cells[rowIndex - totalYearsInReport, colIndex].Address}:{ws.Cells[rowIndex - 1, colIndex].Address})";
        rm_seasonal_total_positive_winter.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        rm_seasonal_total_positive_winter.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        rm_seasonal_total_positive_winter.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        rm_seasonal_total_positive_winter.Style.Numberformat.Format = "#,#";
        colIndex++;
        var rm_seasonal_total_positive_summer = ws.Cells[rowIndex, colIndex];
        rm_seasonal_total_positive_summer.Formula = $"=SUM({ws.Cells[rowIndex - totalYearsInReport, colIndex].Address}:{ws.Cells[rowIndex - 1, colIndex].Address})";
        rm_seasonal_total_positive_summer.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        rm_seasonal_total_positive_summer.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        rm_seasonal_total_positive_summer.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        rm_seasonal_total_positive_summer.Style.Numberformat.Format = "#,#";

        colIndex++;
        colIndex++;

        var rm_seasonal_total_percentage_winter = ws.Cells[rowIndex, colIndex];
        rm_seasonal_total_percentage_winter.Formula = $"({ws.Cells[rowIndex, colIndex - 3].Address}/{ws.Cells[rowIndex, colIndex - 6].Address})*100";
        rm_seasonal_total_percentage_winter.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        rm_seasonal_total_percentage_winter.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        rm_seasonal_total_percentage_winter.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        rm_seasonal_total_percentage_winter.Style.Numberformat.Format = "#,#";
        colIndex++;
        var rm_seasonal_total_percentage_summer = ws.Cells[rowIndex, colIndex];
        rm_seasonal_total_percentage_summer.Formula = $"({ws.Cells[rowIndex, colIndex - 3].Address}/{ws.Cells[rowIndex, colIndex - 6].Address})*100";
        rm_seasonal_total_percentage_summer.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        rm_seasonal_total_percentage_summer.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        rm_seasonal_total_percentage_summer.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        rm_seasonal_total_percentage_summer.Style.Numberformat.Format = "#,#";

        rowIndex++;
        colIndex = 1;
        rowIndex++;

        //finished products
        var fpData = seasonalData.Where(x => x.SampleType == "Finished Product").SelectMany(d => d.SamplesTestedPositive);
        for (int f = 0; f < totalYearsInReport; f++)
        {
          var season1 = ws.Cells[rowIndex, colIndex];
          season1.Value = "Finished Product";
          season1.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          season1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          season1.Style.Fill.BackgroundColor.SetColor(blueHeader);
          season1.Style.Font.Color.SetColor(Color.White);

          colIndex++;
          var season2 = ws.Cells[rowIndex, colIndex];
          season2.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          season2.Value = startYear + f;

          colIndex++;
          var season_winter_tested = ws.Cells[rowIndex, colIndex];
          season_winter_tested.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          season_winter_tested.Value = fpData.Where(x => x.Season == "Winter" && x.Year == startYear + f && x.SampleType == "Finished Product").Sum(s => s.TotalTested);
          if (season_winter_tested.Value == "0") { season_winter_tested.Value = ""; }
          season_winter_tested.Style.Numberformat.Format = "#,#";

          colIndex++;
          var season_summer_tested = ws.Cells[rowIndex, colIndex];
          season_summer_tested.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          season_summer_tested.Value = fpData.Where(x => x.Season == "Summer" && x.Year == startYear + f && x.SampleType == "Finished Product").Sum(s => s.TotalTested);
          if (season_summer_tested.Value == "0") { season_summer_tested.Value = ""; }
          season_summer_tested.Style.Numberformat.Format = "#,#";

          colIndex += 2;
          var season_winter_positive = ws.Cells[rowIndex, colIndex];
          season_winter_positive.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          season_winter_positive.Value = fpData.Where(x => x.Season == "Winter" && x.Year == startYear + f && x.SampleType == "Finished Product").Sum(s => s.TotalPositive);
          if (season_winter_positive.Value == "0") { season_winter_positive.Value = ""; }
          season_winter_positive.Style.Numberformat.Format = "#,#";

          colIndex++;
          var season_summer_positive = ws.Cells[rowIndex, colIndex];
          season_summer_positive.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          season_summer_positive.Value = fpData.Where(x => x.Season == "Summer" && x.Year == startYear + f && x.SampleType == "Finished Product").Sum(s => s.TotalPositive);
          if (season_summer_positive.Value == "0") { season_summer_positive.Value = ""; }
          season_summer_positive.Style.Numberformat.Format = "#,#";

          colIndex += 2;
          var season_winter_percentage = ws.Cells[rowIndex, colIndex];
          season_winter_percentage.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          try
          {
            //season_winter_percentage.Value = fpData.Where(x => x.Season == "Winter" && x.Year == startYear + f && x.SampleType == "Finished Product").Average(s => s.PercentagePositive);
            season_winter_percentage.Formula = $"({ws.Cells[rowIndex, colIndex - 3].Address}/{ws.Cells[rowIndex, colIndex - 6].Address})*100";
            season_winter_percentage.Style.Numberformat.Format = "#,#";
          }
          catch
          {
            season_winter_percentage.Value = "";
          }

          colIndex++;

          var season_summer_percentage = ws.Cells[rowIndex, colIndex];
          season_summer_percentage.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          try
          {
            //season_summer_percentage.Value = Decimal.Round(fpData.Where(x => x.Season == "Summer" && x.Year == startYear + f && x.SampleType == "Finished Product").Average(s => s.PercentagePositive), 0);
            season_summer_percentage.Formula = $"({ws.Cells[rowIndex, colIndex - 3].Address}/{ws.Cells[rowIndex, colIndex - 6].Address})*100";
            season_summer_percentage.Style.Numberformat.Format = "#,#";
          }
          catch
          {
            season_summer_percentage.Value = "";
          }

          colIndex = 1;
          rowIndex++;
        }

        //Seasonal FP totals
        var fp_seasonal_total_header = ws.Cells[rowIndex, colIndex];
        fp_seasonal_total_header.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        fp_seasonal_total_header.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        fp_seasonal_total_header.Style.Fill.BackgroundColor.SetColor(Color.DarkOliveGreen);
        fp_seasonal_total_header.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
        fp_seasonal_total_header.Style.Font.Color.SetColor(Color.White);
        fp_seasonal_total_header.Value = "TOTAL";

        colIndex++;
        colIndex++;
        var fp_seasonal_total_tested_winter = ws.Cells[rowIndex, colIndex];
        fp_seasonal_total_tested_winter.Formula = $"=SUM({ws.Cells[rowIndex - totalYearsInReport, colIndex].Address}:{ws.Cells[rowIndex - 1, colIndex].Address})";
        fp_seasonal_total_tested_winter.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        fp_seasonal_total_tested_winter.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        fp_seasonal_total_tested_winter.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        fp_seasonal_total_tested_winter.Style.Numberformat.Format = "#,#";

        colIndex++;
        var fp_seasonal_total_tested_summer = ws.Cells[rowIndex, colIndex];
        fp_seasonal_total_tested_summer.Formula = $"=SUM({ws.Cells[rowIndex - totalYearsInReport, colIndex].Address}:{ws.Cells[rowIndex - 1, colIndex].Address})";
        fp_seasonal_total_tested_summer.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        fp_seasonal_total_tested_summer.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        fp_seasonal_total_tested_summer.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        fp_seasonal_total_tested_summer.Style.Numberformat.Format = "#,#";


        colIndex++;
        colIndex++;
        var fp_seasonal_total_positive_winter = ws.Cells[rowIndex, colIndex];
        fp_seasonal_total_positive_winter.Formula = String.Format("=SUM({0}:{1})", ws.Cells[rowIndex - totalYearsInReport, colIndex].Address, ws.Cells[rowIndex - 1, colIndex].Address);
        fp_seasonal_total_positive_winter.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        fp_seasonal_total_positive_winter.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        fp_seasonal_total_positive_winter.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        fp_seasonal_total_positive_winter.Style.Numberformat.Format = "#,#";
        colIndex++;
        var fp_seasonal_total_positive_summer = ws.Cells[rowIndex, colIndex];
        fp_seasonal_total_positive_summer.Formula = String.Format("=SUM({0}:{1})", ws.Cells[rowIndex - totalYearsInReport, colIndex].Address, ws.Cells[rowIndex - 1, colIndex].Address);
        fp_seasonal_total_positive_summer.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        fp_seasonal_total_positive_summer.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        fp_seasonal_total_positive_summer.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        fp_seasonal_total_positive_summer.Style.Numberformat.Format = "#,#";

        colIndex++;
        colIndex++;

        var fp_seasonal_total_percentage_winter = ws.Cells[rowIndex, colIndex];
        //fp_seasonal_total_percentage_winter.Formula = String.Format("=SUM({0}:{1})/{2}", ws.Cells[rowIndex - totalYearsInReport, colIndex].Address, ws.Cells[rowIndex - 1, colIndex].Address, totalYearsInReport);
        fp_seasonal_total_percentage_winter.Formula = String.Format("({0}/{1})*100", ws.Cells[rowIndex, colIndex - 3].Address, ws.Cells[rowIndex, colIndex - 6].Address);
        fp_seasonal_total_percentage_winter.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        fp_seasonal_total_percentage_winter.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        fp_seasonal_total_percentage_winter.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        fp_seasonal_total_percentage_winter.Style.Numberformat.Format = "#,#";
        colIndex++;

        var fp_seasonal_total_percentage_summer = ws.Cells[rowIndex, colIndex];
        fp_seasonal_total_percentage_summer.Formula = String.Format("({0}/{1})*100", ws.Cells[rowIndex, colIndex - 3].Address, ws.Cells[rowIndex, colIndex - 6].Address);
        //fp_seasonal_total_percentage_summer.Formula = String.Format("=SUM({0}:{1})/{2}", ws.Cells[rowIndex - totalYearsInReport, colIndex].Address, ws.Cells[rowIndex - 1, colIndex].Address, totalYearsInReport);
        fp_seasonal_total_percentage_summer.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        fp_seasonal_total_percentage_summer.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        fp_seasonal_total_percentage_summer.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        fp_seasonal_total_percentage_summer.Style.Numberformat.Format = "#,#";

        rowIndex++;
        colIndex = 1;
        rowIndex++;

        //dust swabs
        var dsData = seasonalData.Where(x => x.SampleType == "Dust Swab").SelectMany(d => d.SamplesTestedPositive);
        for (int f = 0; f < totalYearsInReport; f++)
        {
          var season1 = ws.Cells[rowIndex, colIndex];
          season1.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          season1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          season1.Style.Fill.BackgroundColor.SetColor(blueHeader);
          season1.Style.Font.Color.SetColor(Color.White);
          season1.Value = "Dust Swab";
          colIndex++;
          var season2 = ws.Cells[rowIndex, colIndex];
          season2.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          season2.Value = startYear + f;

          colIndex++;
          var season_winter_tested = ws.Cells[rowIndex, colIndex];
          season_winter_tested.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          season_winter_tested.Value = dsData.Where(x => x.Season == "Winter" && x.Year == startYear + f && x.SampleType == "Dust Swab").Sum(s => s.TotalTested);
          if (season_winter_tested.Value == "0") { season_winter_tested.Value = ""; }
          season_winter_tested.Style.Numberformat.Format = "#,#";

          colIndex++;
          var season_summer_tested = ws.Cells[rowIndex, colIndex];
          season_summer_tested.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          season_summer_tested.Value = dsData.Where(x => x.Season == "Summer" && x.Year == startYear + f && x.SampleType == "Dust Swab").Sum(s => s.TotalTested);
          if (season_summer_tested.Value == "0") { season_summer_tested.Value = ""; }
          season_summer_tested.Style.Numberformat.Format = "#,#";

          colIndex += 2;
          var season_winter_positive = ws.Cells[rowIndex, colIndex];
          season_winter_positive.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          season_winter_positive.Value = dsData.Where(x => x.Season == "Winter" && x.Year == startYear + f && x.SampleType == "Dust Swab").Sum(s => s.TotalPositive);
          if (season_winter_positive.Value == "0") { season_winter_positive.Value = ""; }
          season_winter_positive.Style.Numberformat.Format = "#,#";

          colIndex++;
          var season_summer_positive = ws.Cells[rowIndex, colIndex];
          season_summer_positive.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          season_summer_positive.Value = dsData.Where(x => x.Season == "Summer" && x.Year == startYear + f && x.SampleType == "Dust Swab").Sum(s => s.TotalPositive);
          if (season_summer_positive.Value == "0") { season_summer_positive.Value = ""; }
          season_summer_positive.Style.Numberformat.Format = "#,#";

          colIndex += 2;
          var season_winter_percentage = ws.Cells[rowIndex, colIndex];
          season_winter_percentage.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          try
          {
            //season_winter_percentage.Value = Decimal.Round(dsData.Where(x => x.Season == "Winter" && x.Year == startYear + f && x.SampleType == "Dust Swab").Average(s => s.PercentagePositive), 0);
            season_winter_percentage.Formula = String.Format("({0}/{1})*100", ws.Cells[rowIndex, colIndex - 3].Address, ws.Cells[rowIndex, colIndex - 6].Address);
            season_winter_percentage.Style.Numberformat.Format = "#,#";
          }
          catch
          {
            season_winter_percentage.Value = "";
          }

          colIndex++;
          var season_summer_percentage = ws.Cells[rowIndex, colIndex];
          season_summer_percentage.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          try
          {
            //season_summer_percentage.Value = Decimal.Round(dsData.Where(x => x.Season == "Summer" && x.Year == startYear + f && x.SampleType == "Dust Swab").Average(s => s.PercentagePositive), 0);
            season_summer_percentage.Formula = String.Format("({0}/{1})*100", ws.Cells[rowIndex, colIndex - 3].Address, ws.Cells[rowIndex, colIndex - 6].Address);
            season_summer_percentage.Style.Numberformat.Format = "#,#";
          }
          catch
          {
            season_summer_percentage.Value = "";
          }

          colIndex = 1;
          rowIndex++;
        }

        //Seasonal DS totals
        var ds_seasonal_total_header = ws.Cells[rowIndex, colIndex];
        ds_seasonal_total_header.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        ds_seasonal_total_header.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        ds_seasonal_total_header.Style.Fill.BackgroundColor.SetColor(Color.DarkOliveGreen);
        ds_seasonal_total_header.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
        ds_seasonal_total_header.Style.Font.Color.SetColor(Color.White);
        ds_seasonal_total_header.Value = "TOTAL";

        colIndex++;
        colIndex++;
        var ds_seasonal_total_tested_winter = ws.Cells[rowIndex, colIndex];
        ds_seasonal_total_tested_winter.Formula = String.Format("=SUM({0}:{1})", ws.Cells[rowIndex - totalYearsInReport, colIndex].Address, ws.Cells[rowIndex - 1, colIndex].Address);
        ds_seasonal_total_tested_winter.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        ds_seasonal_total_tested_winter.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        ds_seasonal_total_tested_winter.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        ds_seasonal_total_tested_winter.Style.Numberformat.Format = "#,#";

        colIndex++;
        var ds_seasonal_total_tested_summer = ws.Cells[rowIndex, colIndex];
        ds_seasonal_total_tested_summer.Formula = String.Format("=SUM({0}:{1})", ws.Cells[rowIndex - totalYearsInReport, colIndex].Address, ws.Cells[rowIndex - 1, colIndex].Address);
        ds_seasonal_total_tested_summer.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        ds_seasonal_total_tested_summer.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        ds_seasonal_total_tested_summer.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        ds_seasonal_total_tested_summer.Style.Numberformat.Format = "#,#";


        colIndex++;
        colIndex++;
        var ds_seasonal_total_positive_winter = ws.Cells[rowIndex, colIndex];
        ds_seasonal_total_positive_winter.Formula = String.Format("=SUM({0}:{1})", ws.Cells[rowIndex - totalYearsInReport, colIndex].Address, ws.Cells[rowIndex - 1, colIndex].Address);
        ds_seasonal_total_positive_winter.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        ds_seasonal_total_positive_winter.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        ds_seasonal_total_positive_winter.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        ds_seasonal_total_positive_winter.Style.Numberformat.Format = "#,#";
        colIndex++;
        var ds_seasonal_total_positive_summer = ws.Cells[rowIndex, colIndex];
        ds_seasonal_total_positive_summer.Formula = String.Format("=SUM({0}:{1})", ws.Cells[rowIndex - totalYearsInReport, colIndex].Address, ws.Cells[rowIndex - 1, colIndex].Address);
        ds_seasonal_total_positive_summer.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        ds_seasonal_total_positive_summer.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        ds_seasonal_total_positive_summer.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        ds_seasonal_total_positive_summer.Style.Numberformat.Format = "#,#";

        colIndex++;
        colIndex++;

        var ds_seasonal_total_percentage_winter = ws.Cells[rowIndex, colIndex];
        //ds_seasonal_total_percentage_winter.Formula = String.Format("=SUM({0}:{1})/{2}", ws.Cells[rowIndex - totalYearsInReport, colIndex].Address, ws.Cells[rowIndex - 1, colIndex].Address, totalYearsInReport);
        ds_seasonal_total_percentage_winter.Formula = String.Format("({0}/{1})*100", ws.Cells[rowIndex, colIndex - 3].Address, ws.Cells[rowIndex, colIndex - 6].Address);
        ds_seasonal_total_percentage_winter.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        ds_seasonal_total_percentage_winter.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        ds_seasonal_total_percentage_winter.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        ds_seasonal_total_percentage_winter.Style.Numberformat.Format = "#,#";
        colIndex++;
        var ds_seasonal_total_percentage_summer = ws.Cells[rowIndex, colIndex];
        //ds_seasonal_total_percentage_summer.Formula = String.Format("=SUM({0}:{1})/{2}", ws.Cells[rowIndex - totalYearsInReport, colIndex].Address, ws.Cells[rowIndex - 1, colIndex].Address, totalYearsInReport);
        ds_seasonal_total_percentage_summer.Formula = String.Format("({0}/{1})*100", ws.Cells[rowIndex, colIndex - 3].Address, ws.Cells[rowIndex, colIndex - 6].Address);
        ds_seasonal_total_percentage_summer.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        ds_seasonal_total_percentage_summer.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        ds_seasonal_total_percentage_summer.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        ds_seasonal_total_percentage_summer.Style.Numberformat.Format = "#,#";

        rowIndex++;
        rowIndex++;
        colIndex = 1;

        var seasonal_data_total_header = ws.Cells[rowIndex, colIndex];
        seasonal_data_total_header.Value = "TOTAL SAMPLES";
        seasonal_data_total_header.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        seasonal_data_total_header.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        seasonal_data_total_header.Style.Fill.BackgroundColor.SetColor(Color.DarkOliveGreen);
        seasonal_data_total_header.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
        seasonal_data_total_header.Style.Font.Color.SetColor(Color.White);

        colIndex++;
        colIndex++;

        var total_samples_tested_winter = ws.Cells[rowIndex, colIndex];
        total_samples_tested_winter.Formula = String.Format("={0}+{1}+{2}", rm_seasonal_total_tested_winter.Address, fp_seasonal_total_tested_winter.Address, ds_seasonal_total_tested_winter.Address);
        total_samples_tested_winter.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        total_samples_tested_winter.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        total_samples_tested_winter.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        total_samples_tested_winter.Style.Numberformat.Format = "#,#";

        colIndex++;
        var total_samples_tested_summer = ws.Cells[rowIndex, colIndex];
        total_samples_tested_summer.Formula = String.Format("={0}+{1}+{2}", rm_seasonal_total_tested_summer.Address, fp_seasonal_total_tested_summer.Address, ds_seasonal_total_tested_summer.Address);
        total_samples_tested_summer.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        total_samples_tested_summer.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        total_samples_tested_summer.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        total_samples_tested_summer.Style.Numberformat.Format = "#,#";

        colIndex++;
        colIndex++;

        var total_samples_positive_winter = ws.Cells[rowIndex, colIndex];
        total_samples_positive_winter.Formula = String.Format("={0}+{1}+{2}", rm_seasonal_total_positive_winter.Address, fp_seasonal_total_positive_winter.Address, ds_seasonal_total_positive_winter.Address);
        total_samples_positive_winter.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        total_samples_positive_winter.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        total_samples_positive_winter.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        total_samples_positive_winter.Style.Numberformat.Format = "#,#";

        colIndex++;
        var total_samples_positive_summer = ws.Cells[rowIndex, colIndex];
        total_samples_positive_summer.Formula = String.Format("={0}+{1}+{2}", rm_seasonal_total_positive_summer.Address, fp_seasonal_total_positive_summer.Address, ds_seasonal_total_positive_summer.Address);
        total_samples_positive_summer.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        total_samples_positive_summer.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        total_samples_positive_summer.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        total_samples_positive_summer.Style.Numberformat.Format = "#,#";

        colIndex++;
        colIndex++;

        var total_samples_percentage_winter = ws.Cells[rowIndex, colIndex];
        //total_samples_percentage_winter.Formula = String.Format("=({0}+{1}+{2})/3", rm_seasonal_total_percentage_winter.Address, fp_seasonal_total_percentage_winter.Address, ds_seasonal_total_percentage_winter.Address);
        total_samples_percentage_winter.Formula = String.Format("({0}/{1})*100", ws.Cells[rowIndex, colIndex - 3].Address, ws.Cells[rowIndex, colIndex - 6].Address);
        total_samples_percentage_winter.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        total_samples_percentage_winter.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        total_samples_percentage_winter.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        total_samples_percentage_winter.Style.Numberformat.Format = "#,#";

        colIndex++;
        var total_samples_percentage_summer = ws.Cells[rowIndex, colIndex];
        //total_samples_percentage_summer.Formula = String.Format("=({0}+{1}+{2})/3", rm_seasonal_total_percentage_summer.Address, fp_seasonal_total_percentage_summer.Address, ds_seasonal_total_percentage_summer.Address);
        total_samples_percentage_summer.Formula = String.Format("({0}/{1})*100", ws.Cells[rowIndex, colIndex - 3].Address, ws.Cells[rowIndex, colIndex - 6].Address);
        total_samples_percentage_summer.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        total_samples_percentage_summer.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        total_samples_percentage_summer.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        total_samples_percentage_summer.Style.Numberformat.Format = "#,#";

        #endregion

        #endregion

        #region YEARLY REPORT

        p.Workbook.Worksheets.Add("Yearly Report");
        ExcelWorksheet ws1 = p.Workbook.Worksheets[1];
        ws1.Name = "Yearly Report";

        //DEFAULT COLUMN WIDTH AND HEIGHT
        ws1.Column(2).Width = 35;
        ws1.Row(1).Height = 22;
        ws1.Row(2).Height = 22;

        //reinit header and variables
        samplesTestedStart = 3;
        samplesPositiveStart = samplesTestedStart + totalYearsInReport + 2; //1 at the end is for total col, need to add another 1 for space. for 12 months, positive tested will start at 17 
        percentagePositiveStart = samplesPositiveStart + totalYearsInReport + 2; //1 at the end is for total, need to add another 1 for space. for 12 months = 31


        //header row - Samples Tested
        ///Color colFromHex_yr1 = System.Drawing.ColorTranslator.FromHtml("#999999");
        var row_header_yr1 = ws1.Cells[1, samplesTestedStart];
        row_header_yr1.Value = "Samples Tested";
        row_header_yr1.Style.Font.Bold = true;
        row_header_yr1.Style.Font.Color.SetColor(Color.White);
        row_header_yr1.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        row_header_yr1.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        row_header_yr1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        row_header_yr1.Style.Fill.BackgroundColor.SetColor(silverHeader);
        ws1.Cells[1, 3, 1, samplesPositiveStart - 2].Merge = true;

        //header row - Samples Positive
        var row_header_yr2 = ws1.Cells[1, samplesTestedStart + totalYearsInReport + 2];
        row_header_yr2.Value = "Samples Positive";
        row_header_yr2.Style.Font.Bold = true;
        row_header_yr2.Style.Font.Color.SetColor(Color.White);
        row_header_yr2.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        row_header_yr2.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        row_header_yr2.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        row_header_yr2.Style.Fill.BackgroundColor.SetColor(silverHeader);
        ws1.Cells[1, samplesPositiveStart, 1, percentagePositiveStart - 2].Merge = true;

        //header row - Samples Positive
        var row_header_yr3 = ws1.Cells[1, (samplesPositiveStart + totalYearsInReport + 2)];
        row_header_yr3.Value = "Percentage Positive";
        row_header_yr3.Style.Font.Bold = true;
        row_header_yr3.Style.Font.Color.SetColor(Color.White);
        row_header_yr3.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        row_header_yr3.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        row_header_yr3.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        row_header_yr3.Style.Fill.BackgroundColor.SetColor(silverHeader);
        ws1.Cells[1, percentagePositiveStart, 1, percentagePositiveStart + totalYearsInReport].Merge = true;

        //second row
        //colFromHex_yr1 = System.Drawing.ColorTranslator.FromHtml("#c0504d");
        var code_row_yr = ws1.Cells[2, 1];
        code_row_yr.Value = "CODE";
        var ingredient_row_yr = ws1.Cells[2, 2];
        ingredient_row_yr.Style.Font.Color.SetColor(Color.White);
        ingredient_row_yr.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        ingredient_row_yr.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        ingredient_row_yr.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        ingredient_row_yr.Value = "FEED INGREDIENT";

        for (int k = 0; k < totalYearsInReport; k++)
        {
          var year_row1 = ws1.Cells[2, k + 3];
          year_row1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          year_row1.Style.Fill.BackgroundColor.SetColor(maroonHeader);
          year_row1.Style.Font.Color.SetColor(Color.White);
          year_row1.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
          year_row1.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
          year_row1.Value = startYear + k;

          var year_row2 = ws1.Cells[2, k + totalYearsInReport + 5];
          year_row2.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          year_row2.Style.Fill.BackgroundColor.SetColor(maroonHeader);
          year_row2.Style.Font.Color.SetColor(Color.White);
          year_row2.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
          year_row2.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
          year_row2.Value = startYear + k;

          var year_row3 = ws1.Cells[2, k + totalYearsInReport * 2 + 7];
          year_row3.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          year_row3.Style.Fill.BackgroundColor.SetColor(maroonHeader);
          year_row3.Style.Font.Color.SetColor(Color.White);
          year_row3.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
          year_row3.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
          year_row3.Value = startYear + k;
        }

        //total rows
        var total_cols_yr = ws1.Cells[2, totalYearsInReport + 3];
        total_cols_yr.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        total_cols_yr.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        total_cols_yr.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        total_cols_yr.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        total_cols_yr.Style.Font.Color.SetColor(Color.White);
        total_cols_yr.Value = "TOTAL";

        var total_positive_col_yr = ws1.Cells[2, totalYearsInReport * 2 + 5];
        total_positive_col_yr.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        total_positive_col_yr.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        total_positive_col_yr.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        total_positive_col_yr.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        total_positive_col_yr.Style.Font.Color.SetColor(Color.White);
        total_positive_col_yr.Value = "TOTAL";

        var total_percentage_col_yr = ws1.Cells[2, totalYearsInReport * 3 + 7];
        total_percentage_col_yr.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        total_percentage_col_yr.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        total_percentage_col_yr.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        total_percentage_col_yr.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        total_percentage_col_yr.Style.Font.Color.SetColor(Color.White);
        total_percentage_col_yr.Value = "AVERAGE";

        rowIndex = 3;
        colIndex = 1;
        //colFromHex_yr1 = System.Drawing.ColorTranslator.FromHtml("#4f81bd");

        //js test
        var yearStartAddress = "";
        var yearEndAddress = "";

        foreach (var item in yearlyReport)
        {
          //raw materials                
          ws1.Cells[rowIndex, 1, rowIndex, totalYearsInReport * 3 + 7].Merge = true;
          ws1.Cells[rowIndex, 1, rowIndex, totalYearsInReport * 3 + 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          var raw_materials_row = ws1.Cells[rowIndex, colIndex];
          raw_materials_row.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          raw_materials_row.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          raw_materials_row.Style.Font.Color.SetColor(Color.White);
          raw_materials_row.Style.Fill.BackgroundColor.SetColor(blueHeader);
          raw_materials_row.Value = "Raw Materials";
          rowIndex++;

          //get the starting row and column
          startRow = rowIndex;

          foreach (var sample in item.SamplesTestedPositive.Where(s => s.SampleTypeId == 1).OrderBy(x => x.Sample))
          {
            //colFromHex_yr1 = System.Drawing.ColorTranslator.FromHtml("#82bc4f");
            var code_row_yr1 = ws1.Cells[rowIndex, colIndex];
            code_row_yr1.Value = sample.Code;
            code_row_yr1.Style.Font.Color.SetColor(Color.White);
            code_row_yr1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            code_row_yr1.Style.Fill.BackgroundColor.SetColor(Color.DarkOliveGreen);
            code_row_yr1.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            colIndex++;
            var sample_row_yr1 = ws1.Cells[rowIndex, colIndex];
            sample_row_yr1.Value = sample.Sample;
            sample_row_yr1.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

            colIndex++;
            //js test
            String startAddress = "";
            String endAddress = "";
            yearStartAddress = ws1.Cells[rowIndex, colIndex + 1].Address;
            startAddress = ws1.Cells[rowIndex, colIndex].Address;
            foreach (var year in sample.YearValues)
            {
              var yr_test = ws1.Cells[rowIndex, colIndex];
              yr_test.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              if (year.TotalTested == 0)
              {
                yr_test.Value = "";
              }
              else
              {
                yr_test.Value = year.TotalTested;
              }
              colIndex++;
            }
            endAddress = ws1.Cells[rowIndex, colIndex - 1].Address;
            var tt = ws1.Cells[rowIndex, colIndex];
            tt.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            tt.Style.Font.Bold = true;
            tt.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            tt.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
            tt.Formula = String.Format("=SUM({0}:{1})", startAddress, endAddress);
            colIndex += 2;


            startAddress = ws1.Cells[rowIndex, colIndex].Address;
            foreach (var year in sample.YearValues)
            {
              var yr_pos = ws1.Cells[rowIndex, colIndex];
              yr_pos.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              if (year.TotalPositive == 0)
              {
                yr_pos.Value = "";
              }
              else
              {
                yr_pos.Value = year.TotalPositive;
              }
              colIndex++;
            }
            endAddress = ws1.Cells[rowIndex, colIndex - 1].Address;
            var tp = ws1.Cells[rowIndex, colIndex];
            tp.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            tp.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            tp.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
            tp.Style.Font.Bold = true;
            tp.Formula = String.Format("=SUM({0}:{1})", startAddress, endAddress);

            colIndex += 2;

            startAddress = ws1.Cells[rowIndex, colIndex].Address;
            foreach (var year in sample.YearValues)
            {
              var yr_per = ws1.Cells[rowIndex, colIndex];
              yr_per.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              if (year.PercentagePositive != 0)
              {
                yr_per.Value = decimal.Round(Convert.ToDecimal(year.PercentagePositive), 0);
              }
              else
              {
                yr_per.Value = "";
              }
              colIndex++;
            }
            endAddress = ws1.Cells[rowIndex, colIndex - 1].Address;

            var pp = ws1.Cells[rowIndex, colIndex];
            Int32 t1 = totalYearsInReport + 2;
            Int32 t2 = (totalYearsInReport + 2) * 2;
            pp.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            pp.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            pp.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
            pp.Style.Font.Bold = true;
            pp.Style.Numberformat.Format = "#,#";
            //pp.Formula = String.Format("=SUM({0}:{1})/{2}", startAddress, endAddress, totalYearsInReport);
            pp.Formula = String.Format("({0}/{1})*100", ws1.Cells[rowIndex, colIndex - t1].Address, ws1.Cells[rowIndex, colIndex - t2].Address);
            if (pp.Value == "0") { pp.Value = ""; }

            colIndex = 1;
            rowIndex++;
          }
          endRow = rowIndex;

          //botttom totals

          //js test
          //yearEndAddress = ws1.Cells[rowIndex, colIndex].Address;
          //rowIndex++;
          //var yearTotal = ws1.Cells[rowIndex, colIndex];
          //yearTotal.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          //yearTotal.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          //yearTotal.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
          //yearTotal.Style.Font.Bold = true;
          //yearTotal.Formula = String.Format("=SUM({0}:{1})", yearStartAddress, yearEndAddress);

          colIndex = 2;

          var rm_total_text = ws1.Cells[endRow, colIndex];
          rm_total_text.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          rm_total_text.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          rm_total_text.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
          rm_total_text.Style.Font.Bold = true;
          rm_total_text.Value = "TOTALS";
          colIndex++;

          for (int i = 0; i < totalYearsInReport; i++)
          {
            var total_year_value = ws1.Cells[endRow, colIndex + i];
            total_year_value.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            total_year_value.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            total_year_value.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
            total_year_value.Style.Font.Bold = true;
            total_year_value.Formula = String.Format("=SUM({0}:{1})", ws1.Cells[startRow, colIndex + i].Address, ws1.Cells[endRow - 1, colIndex + i].Address);
          }

          //grand total
          colIndex = colIndex + totalYearsInReport;
          var rm_grand_total = ws1.Cells[endRow, colIndex];
          rm_grand_total.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          rm_grand_total.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          rm_grand_total.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
          rm_grand_total.Style.Font.Bold = true;
          rm_grand_total.Formula = String.Format("=SUM({0}:{1})", ws1.Cells[rowIndex, colIndex - totalYearsInReport].Address, ws1.Cells[rowIndex, colIndex - 1].Address);
          //rowIndex++;

          colIndex += 2;

          for (int i = 0; i < totalYearsInReport; i++)
          {
            var total_year_value = ws1.Cells[endRow, colIndex + i];
            total_year_value.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            total_year_value.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            total_year_value.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
            total_year_value.Style.Font.Bold = true;
            total_year_value.Formula = String.Format("=SUM({0}:{1})", ws1.Cells[startRow, colIndex + i].Address, ws1.Cells[endRow - 1, colIndex + i].Address);
          }

          //grand total
          colIndex = colIndex + totalYearsInReport;
          var rm_grand_total_pos = ws1.Cells[endRow, colIndex];
          rm_grand_total_pos.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          rm_grand_total_pos.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          rm_grand_total_pos.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
          rm_grand_total_pos.Style.Font.Bold = true;
          rm_grand_total_pos.Formula = String.Format("=SUM({0}:{1})", ws1.Cells[rowIndex, colIndex - totalYearsInReport].Address, ws1.Cells[rowIndex, colIndex - 1].Address);

          colIndex += 2;

          for (int i = 0; i < totalYearsInReport; i++)
          {
            var total_year_value = ws1.Cells[endRow, colIndex + i];
            total_year_value.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            total_year_value.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            total_year_value.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
            total_year_value.Style.Font.Bold = true;
            total_year_value.Style.Numberformat.Format = "#,#";
            total_year_value.Formula = String.Format("=SUM({0}:{1})", ws1.Cells[startRow, colIndex + i].Address, ws1.Cells[endRow - 1, colIndex + i].Address, endRow - startRow);
          }

          //grand total
          colIndex = colIndex + totalYearsInReport;
          Int32 r1 = totalYearsInReport + 2;
          Int32 r2 = (totalYearsInReport + 2) * 2;
          var rm_grand_total_per = ws1.Cells[endRow, colIndex];
          rm_grand_total_per.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          rm_grand_total_per.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          rm_grand_total_per.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
          rm_grand_total_per.Style.Font.Bold = true;
          rm_grand_total_per.Style.Numberformat.Format = "#,#";
          //rm_grand_total_per.Formula = String.Format("=SUM({0}:{1}/{2})", ws1.Cells[rowIndex, colIndex - 1].Address, ws1.Cells[rowIndex, colIndex - 1].Address, totalYearsInReport);
          rm_grand_total_per.Formula = String.Format("({0}/{1})*100", ws1.Cells[rowIndex, colIndex - r1].Address, ws1.Cells[rowIndex, colIndex - r2].Address);

          rowIndex++;
          rowIndex++;

          colIndex = 1;



          //finished products
          //colFromHex_yr1 = System.Drawing.ColorTranslator.FromHtml("#4f81bd");
          ws1.Cells[rowIndex, 1, rowIndex, totalYearsInReport * 3 + 7].Merge = true;
          ws1.Cells[rowIndex, 1, rowIndex, totalYearsInReport * 3 + 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          var fin_products_row = ws1.Cells[rowIndex, colIndex];
          fin_products_row.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          fin_products_row.Style.Fill.BackgroundColor.SetColor(blueHeader);
          fin_products_row.Value = "Finished Product";
          fin_products_row.Style.Font.Color.SetColor(Color.White);
          rowIndex++;

          startRow = rowIndex;
          foreach (var sample in item.SamplesTestedPositive.Where(s => s.SampleTypeId == 2).OrderBy(x => x.Sample))
          {
            //colFromHex_yr1 = System.Drawing.ColorTranslator.FromHtml("#82bc4f");
            var code_row_yr1 = ws1.Cells[rowIndex, colIndex];
            code_row_yr1.Value = sample.Code;
            code_row_yr1.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            code_row_yr1.Style.Font.Color.SetColor(Color.White);
            code_row_yr1.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            code_row_yr1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            code_row_yr1.Style.Fill.BackgroundColor.SetColor(Color.DarkOliveGreen);

            colIndex++;
            var sample_row_yr1 = ws1.Cells[rowIndex, colIndex];
            sample_row_yr1.Value = sample.Sample;
            sample_row_yr1.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

            String startAddress = "";
            String endAddress = "";

            colIndex++;

            startAddress = ws1.Cells[rowIndex, colIndex].Address;
            foreach (var year in sample.YearValues)
            {
              var yr_test = ws1.Cells[rowIndex, colIndex];
              yr_test.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              if (year.TotalTested == 0)
              {
                yr_test.Value = "";
              }
              else
              {
                yr_test.Value = year.TotalTested;
              }
              colIndex++;
            }
            endAddress = ws1.Cells[rowIndex, colIndex - 1].Address;
            var tt = ws1.Cells[rowIndex, colIndex];
            tt.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            tt.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            tt.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
            tt.Style.Font.Bold = true;
            tt.Formula = String.Format("=SUM({0}:{1})", startAddress, endAddress);
            colIndex += 2;

            startAddress = ws1.Cells[rowIndex, colIndex].Address;
            foreach (var year in sample.YearValues)
            {
              var yr_pos = ws1.Cells[rowIndex, colIndex];
              yr_pos.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              if (year.TotalPositive == 0)
              {
                yr_pos.Value = "";
              }
              else
              {
                yr_pos.Value = year.TotalPositive;
              }
              colIndex++;
            }
            endAddress = ws1.Cells[rowIndex, colIndex - 1].Address;
            var tt1 = ws1.Cells[rowIndex, colIndex];
            tt1.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            tt1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            tt1.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
            tt1.Style.Font.Bold = true;
            tt1.Formula = String.Format("=SUM({0}:{1})", startAddress, endAddress);
            colIndex += 2;

            startAddress = ws1.Cells[rowIndex, colIndex].Address;
            foreach (var year in sample.YearValues)
            {
              var yr_per = ws1.Cells[rowIndex, colIndex];
              yr_per.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              yr_per.Value = Decimal.Round(Convert.ToDecimal(year.PercentagePositive), 0);
              colIndex++;
            }
            var tt2 = ws1.Cells[rowIndex, colIndex];
            tt2.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            tt2.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            tt2.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
            tt2.Style.Font.Bold = true;
            tt2.Style.Numberformat.Format = "#,#";
            //tt2.Formula = String.Format("=SUM({0}:{1})/{2}", startAddress, endAddress, totalYearsInReport);
            Int32 f1 = totalYearsInReport + 2;
            Int32 f2 = (totalYearsInReport + 2) * 2;
            tt2.Formula = String.Format("({0}/{1})*100", ws1.Cells[rowIndex, colIndex - r1].Address, ws1.Cells[rowIndex, colIndex - r2].Address);

            colIndex = 1;
            rowIndex++;
          }
          endRow = rowIndex;

          colIndex = 2;

          var fp_total_text = ws1.Cells[endRow, colIndex];
          fp_total_text.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          fp_total_text.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          fp_total_text.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
          fp_total_text.Style.Font.Bold = true;
          fp_total_text.Value = "TOTALS";
          colIndex++;

          for (int i = 0; i < totalYearsInReport; i++)
          {
            var total_year_value = ws1.Cells[endRow, colIndex + i];
            total_year_value.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            total_year_value.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            total_year_value.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
            total_year_value.Style.Font.Bold = true;
            total_year_value.Formula = String.Format("=SUM({0}:{1})", ws1.Cells[startRow, colIndex + i].Address, ws1.Cells[endRow - 1, colIndex + i].Address);
          }

          //grand total
          colIndex = colIndex + totalYearsInReport;
          var fp_grand_total = ws1.Cells[endRow, colIndex];
          fp_grand_total.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          fp_grand_total.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          fp_grand_total.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
          fp_grand_total.Style.Font.Bold = true;
          fp_grand_total.Formula = String.Format("=SUM({0}:{1})", ws1.Cells[rowIndex, colIndex - totalYearsInReport].Address, ws1.Cells[rowIndex, colIndex - 1].Address);
          //rowIndex++;

          colIndex += 2;

          for (int i = 0; i < totalYearsInReport; i++)
          {
            var total_year_value = ws1.Cells[endRow, colIndex + i];
            total_year_value.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            total_year_value.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            total_year_value.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
            total_year_value.Style.Font.Bold = true;
            total_year_value.Formula = String.Format("=SUM({0}:{1})", ws1.Cells[startRow, colIndex + i].Address, ws1.Cells[endRow - 1, colIndex + i].Address);
          }

          //grand total
          colIndex = colIndex + totalYearsInReport;
          var fp_grand_total_pos = ws1.Cells[endRow, colIndex];
          fp_grand_total_pos.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          fp_grand_total_pos.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          fp_grand_total_pos.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
          fp_grand_total_pos.Style.Font.Bold = true;
          fp_grand_total_pos.Formula = String.Format("=SUM({0}:{1})", ws1.Cells[rowIndex, colIndex - totalYearsInReport].Address, ws1.Cells[rowIndex, colIndex - 1].Address);

          colIndex += 2;

          for (int i = 0; i < totalYearsInReport; i++)
          {
            var total_year_value = ws1.Cells[endRow, colIndex + i];
            total_year_value.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            total_year_value.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            total_year_value.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
            total_year_value.Style.Font.Bold = true;
            total_year_value.Style.Numberformat.Format = "#,#";
            total_year_value.Formula = String.Format("=SUM({0}:{1})", ws1.Cells[startRow, colIndex + i].Address, ws1.Cells[endRow - 1, colIndex + i].Address, endRow - startRow);
          }

          //grand total
          colIndex = colIndex + totalYearsInReport;
          var fp_grand_total_per = ws1.Cells[endRow, colIndex];
          fp_grand_total_per.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          fp_grand_total_per.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          fp_grand_total_per.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
          fp_grand_total_per.Style.Font.Bold = true;
          fp_grand_total_per.Style.Numberformat.Format = "#,#";
          fp_grand_total_per.Formula = String.Format("({0}/{1})*100", ws1.Cells[rowIndex, colIndex - r1].Address, ws1.Cells[rowIndex, colIndex - r2].Address);
          //fp_grand_total_per.Formula = String.Format("=SUM({0}:{1}/{2})", ws1.Cells[rowIndex, colIndex - 1].Address, ws1.Cells[rowIndex, colIndex - 1].Address, totalYearsInReport);

          rowIndex++;
          rowIndex++;

          colIndex = 1;

          //dust swabs
          //colFromHex_yr1 = System.Drawing.ColorTranslator.FromHtml("#4f81bd");
          ws1.Cells[rowIndex, 1, rowIndex, totalYearsInReport * 3 + 7].Merge = true;
          ws1.Cells[rowIndex, 1, rowIndex, totalYearsInReport * 3 + 7].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          var dust_swab_row = ws1.Cells[rowIndex, colIndex];
          dust_swab_row.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          dust_swab_row.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          dust_swab_row.Style.Font.Color.SetColor(Color.White);
          dust_swab_row.Style.Fill.BackgroundColor.SetColor(blueHeader);
          dust_swab_row.Value = "Dust Swabs";
          dust_swab_row.Style.Font.Color.SetColor(Color.White);
          rowIndex++;

          startRow = rowIndex;
          foreach (var sample in item.SamplesTestedPositive.Where(s => s.SampleTypeId == 3).OrderBy(x => x.Sample))
          {
            //colFromHex_yr1 = System.Drawing.ColorTranslator.FromHtml("#82bc4f");
            var code_row_yr1 = ws1.Cells[rowIndex, colIndex];
            code_row_yr1.Value = sample.Code;
            code_row_yr1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            code_row_yr1.Style.Fill.BackgroundColor.SetColor(Color.DarkOliveGreen);

            colIndex++;
            var sample_row_yr1 = ws1.Cells[rowIndex, colIndex];
            sample_row_yr1.Value = sample.Sample;
            sample_row_yr1.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

            colIndex++;

            String startAddress = "";
            String endAddress = "";

            startAddress = ws1.Cells[rowIndex, colIndex].Address;
            foreach (var year in sample.YearValues)
            {
              var yr_test = ws1.Cells[rowIndex, colIndex];
              yr_test.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              yr_test.Style.Numberformat.Format = "#,#";
              if (year.TotalTested == 0)
              {
                yr_test.Value = "";
              }
              else
              {
                yr_test.Value = year.TotalTested;
              }
              colIndex++;
            }
            endAddress = ws1.Cells[rowIndex, colIndex - 1].Address;
            var tt = ws1.Cells[rowIndex, colIndex];
            tt.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            tt.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            tt.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
            tt.Style.Font.Bold = true;
            tt.Formula = String.Format("=SUM({0}:{1})", startAddress, endAddress);
            tt.Style.Numberformat.Format = "#,#";

            colIndex += 2;

            startAddress = ws1.Cells[rowIndex, colIndex].Address;
            foreach (var year in sample.YearValues)
            {
              var yr_pos = ws1.Cells[rowIndex, colIndex];
              yr_pos.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              if (year.TotalPositive == 0)
              {
                yr_pos.Value = "";
              }
              else
              {
                yr_pos.Style.Numberformat.Format = "#,#";
                yr_pos.Value = year.TotalPositive;
              }
              colIndex++;
            }
            endAddress = ws1.Cells[rowIndex, colIndex - 1].Address;
            var tt1 = ws1.Cells[rowIndex, colIndex];
            tt1.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            tt1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            tt1.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
            tt1.Style.Font.Bold = true;
            tt1.Style.Numberformat.Format = "#,#";
            tt1.Formula = String.Format("=SUM({0}:{1})", startAddress, endAddress);

            colIndex += 2;
            startAddress = ws1.Cells[rowIndex, colIndex].Address;
            foreach (var year in sample.YearValues)
            {
              var yr_per = ws1.Cells[rowIndex, colIndex];
              yr_per.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              if (year.PercentagePositive == 0)
              {
                yr_per.Value = "";
              }
              else
              {
                yr_per.Value = Decimal.Round(Convert.ToDecimal(year.PercentagePositive), 0);
              }
              colIndex++;
            }
            endAddress = ws1.Cells[rowIndex, colIndex - 1].Address;
            //dustswabs average percentage column
            var tt3 = ws1.Cells[rowIndex, colIndex];
            tt3.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            tt3.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            tt3.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
            tt3.Style.Font.Bold = true;
            tt3.Style.Numberformat.Format = "#,#";
            tt3.Formula = String.Format("({0}/{1})*100", ws1.Cells[rowIndex, colIndex - r1].Address, ws1.Cells[rowIndex, colIndex - r2].Address);
            //tt3.Formula = String.Format("=SUM({0}:{1})/{2}", startAddress, endAddress, totalYearsInReport);

            colIndex = 1;
            rowIndex++;
          }
          endRow = rowIndex;

          colIndex = 2;

          var ds_total_text = ws1.Cells[endRow, colIndex];
          ds_total_text.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          ds_total_text.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          ds_total_text.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
          ds_total_text.Style.Font.Bold = true;
          ds_total_text.Value = "TOTALS";
          colIndex++;

          for (int i = 0; i < totalYearsInReport; i++)
          {
            var total_year_value = ws1.Cells[endRow, colIndex + i];
            total_year_value.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            total_year_value.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            total_year_value.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
            total_year_value.Style.Font.Bold = true;
            total_year_value.Formula = String.Format("=SUM({0}:{1})", ws1.Cells[startRow, colIndex + i].Address, ws1.Cells[endRow - 1, colIndex + i].Address);
          }

          //grand total
          colIndex = colIndex + totalYearsInReport;
          var ds_grand_total = ws1.Cells[endRow, colIndex];
          ds_grand_total.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          ds_grand_total.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          ds_grand_total.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
          ds_grand_total.Style.Font.Bold = true;
          ds_grand_total.Formula = String.Format("=SUM({0}:{1})", ws1.Cells[rowIndex, colIndex - totalYearsInReport].Address, ws1.Cells[rowIndex, colIndex - 1].Address);
          //rowIndex++;

          colIndex += 2;

          for (int i = 0; i < totalYearsInReport; i++)
          {
            var total_year_value = ws1.Cells[endRow, colIndex + i];
            total_year_value.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            total_year_value.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            total_year_value.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
            total_year_value.Style.Font.Bold = true;
            total_year_value.Formula = String.Format("=SUM({0}:{1})", ws1.Cells[startRow, colIndex + i].Address, ws1.Cells[endRow - 1, colIndex + i].Address);
          }

          //grand total
          colIndex = colIndex + totalYearsInReport;
          var ds_grand_total_pos = ws1.Cells[endRow, colIndex];
          ds_grand_total_pos.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          ds_grand_total_pos.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          ds_grand_total_pos.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
          ds_grand_total_pos.Style.Font.Bold = true;
          ds_grand_total_pos.Formula = String.Format("=SUM({0}:{1})", ws1.Cells[rowIndex, colIndex - totalYearsInReport].Address, ws1.Cells[rowIndex, colIndex - 1].Address);

          colIndex += 2;

          for (int i = 0; i < totalYearsInReport; i++)
          {
            var ds_total_per = ws1.Cells[endRow, colIndex + i];
            ds_total_per.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            ds_total_per.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            ds_total_per.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
            ds_total_per.Style.Font.Bold = true;
            ds_total_per.Style.Numberformat.Format = "#,#";
            ds_total_per.Formula = String.Format("=SUM({0}:{1})", ws1.Cells[startRow, colIndex + i].Address, ws1.Cells[endRow - 1, colIndex + i].Address, endRow - startRow);
          }

          //grand total
          colIndex = colIndex + totalYearsInReport;
          var ds_grand_total_per = ws1.Cells[endRow, colIndex];
          ds_grand_total_per.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          ds_grand_total_per.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          ds_grand_total_per.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
          ds_grand_total_per.Style.Font.Bold = true;
          ds_grand_total_per.Style.Numberformat.Format = "#,#";
          ds_grand_total_per.Formula = String.Format("({0}/{1})*100", ws1.Cells[rowIndex, colIndex - r1].Address, ws1.Cells[rowIndex, colIndex - r2].Address);
          //ds_grand_total_per.Formula = String.Format("=SUM({0}:{1})/{2}", ws1.Cells[rowIndex, colIndex - 1].Address, ws1.Cells[rowIndex, colIndex - 1].Address, totalYearsInReport);

          rowIndex++;
          rowIndex++;

          colIndex = 1;

        }

        #endregion

        #region SALMONELLA CATEGORY GROUPS (Animal Proteins, Grain etc)

        //rowIndex = 4;
        colIndex = 1;
        rowIndex++;

        Int32 GroupStartCol = 1;
        Int32 YearStartCol = 3;
        Int32 testedStart = YearStartCol;
        Int32 positiveStart = YearStartCol + totalYearsInReport + 1; //1 added for the total col, and 1 for the space between sample types
        Int32 percentageStart = positiveStart + totalYearsInReport + 2;

        //header row - Samples Tested
        //Color colFromHex_grp1 = System.Drawing.ColorTranslator.FromHtml("#999999");

        var row_header_grp1 = ws1.Cells[rowIndex, testedStart];
        row_header_grp1.Value = "Samples Tested";
        row_header_grp1.Style.Font.Bold = true;
        row_header_grp1.Style.Font.Color.SetColor(Color.White);
        row_header_grp1.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        row_header_grp1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        row_header_grp1.Style.Fill.BackgroundColor.SetColor(silverHeader);
        ws1.Cells[rowIndex, YearStartCol, rowIndex, positiveStart - 1].Merge = true;
        ws1.Cells[rowIndex, YearStartCol, rowIndex, positiveStart - 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

        positiveStart++;

        //header row - Samples Positive
        var row_header_grp2 = ws1.Cells[rowIndex, positiveStart];
        row_header_grp2.Value = "Samples Positive";
        row_header_grp2.Style.Font.Bold = true;
        row_header_grp2.Style.Font.Color.SetColor(Color.White);
        row_header_grp2.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        row_header_grp2.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        row_header_grp2.Style.Fill.BackgroundColor.SetColor(silverHeader);
        ws1.Cells[rowIndex, positiveStart, rowIndex, percentageStart - 1].Merge = true;
        ws1.Cells[rowIndex, positiveStart, rowIndex, percentageStart - 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

        percentageStart++;

        //header row - Samples Positive
        var row_header_grp3 = ws1.Cells[rowIndex, percentageStart];
        row_header_grp3.Value = "Percentage Positive";
        row_header_grp3.Style.Font.Bold = true;
        row_header_grp3.Style.Font.Color.SetColor(Color.White);
        row_header_grp3.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        row_header_grp3.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        row_header_grp3.Style.Fill.BackgroundColor.SetColor(silverHeader);
        ws1.Cells[rowIndex, percentageStart, rowIndex, percentageStart + totalYearsInReport].Merge = true;
        ws1.Cells[rowIndex, percentageStart, rowIndex, percentageStart + totalYearsInReport].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

        rowIndex++;

        //colFromHex_yr1 = System.Drawing.ColorTranslator.FromHtml("#c0504d");
        var code_row_code = ws1.Cells[rowIndex, 1];
        code_row_code.Value = "CODE";
        code_row_code.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        code_row_code.Style.Fill.BackgroundColor.SetColor(silverHeader);
        code_row_code.Style.Font.Color.SetColor(Color.White);

        var name_row_grp = ws1.Cells[rowIndex, 2];
        name_row_grp.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        name_row_grp.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        name_row_grp.Value = "CATEGORY";
        name_row_grp.Style.Font.Color.SetColor(Color.White);

        for (int k = 0; k < totalYearsInReport; k++)
        {
          var grp_year_row1 = ws1.Cells[rowIndex, k + testedStart];
          grp_year_row1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          grp_year_row1.Style.Fill.BackgroundColor.SetColor(maroonHeader);
          grp_year_row1.Style.Font.Color.SetColor(Color.White);
          grp_year_row1.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
          grp_year_row1.Value = startYear + k;

          var grp_year_row2 = ws1.Cells[rowIndex, k + positiveStart];
          grp_year_row2.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          grp_year_row2.Style.Fill.BackgroundColor.SetColor(maroonHeader);
          grp_year_row2.Style.Font.Color.SetColor(Color.White);
          grp_year_row2.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
          grp_year_row2.Value = startYear + k;

          var grp_year_row3 = ws1.Cells[rowIndex, k + percentageStart];
          grp_year_row3.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          grp_year_row3.Style.Fill.BackgroundColor.SetColor(maroonHeader);
          grp_year_row3.Style.Font.Color.SetColor(Color.White);
          grp_year_row3.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
          grp_year_row3.Value = startYear + k;
        }

        //total rows
        var total_cols_grp1 = ws1.Cells[rowIndex, YearStartCol + totalYearsInReport];
        total_cols_grp1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        total_cols_grp1.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        total_cols_grp1.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        total_cols_grp1.Style.Font.Color.SetColor(Color.White);
        total_cols_grp1.Value = "TOTAL";

        var total_positive_col_grp = ws1.Cells[rowIndex, positiveStart + totalYearsInReport];
        total_positive_col_grp.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        total_positive_col_grp.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        total_positive_col_grp.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        total_positive_col_grp.Style.Font.Color.SetColor(Color.White);
        total_positive_col_grp.Value = "TOTAL";

        var total_percentage_col_grp = ws1.Cells[rowIndex, percentageStart + totalYearsInReport];
        total_percentage_col_grp.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        total_percentage_col_grp.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        total_percentage_col_grp.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        total_percentage_col_grp.Style.Font.Color.SetColor(Color.White);
        total_percentage_col_grp.Value = "AVERAGE";

        rowIndex++;

        colIndex = 1;

        foreach (var item in reportGroups)
        {
          //colFromHex_yr1 = System.Drawing.ColorTranslator.FromHtml("#82bc4f");
          var group_row_code = ws1.Cells[rowIndex, colIndex];
          group_row_code.Value = item.GroupId;
          group_row_code.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          group_row_code.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
          group_row_code.Style.Fill.BackgroundColor.SetColor(maroonHeader);
          group_row_code.Style.Font.Color.SetColor(Color.White);
          group_row_code.Style.Fill.BackgroundColor.SetColor(Color.DarkOliveGreen);

          colIndex++;
          var group_row_item = ws1.Cells[rowIndex, colIndex];
          group_row_item.Value = item.GroupName;
          group_row_item.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

          for (int k = 0; k < totalYearsInReport; k++)
          {
            var grp_year_row1 = ws1.Cells[rowIndex, k + testedStart];
            grp_year_row1.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            grp_year_row1.Style.Numberformat.Format = "#,#";
            Int32 totalTested = item.TestedPositiveSamples.Where(d => d.Year == startYear + k).Single().TotalTested;
            if (totalTested == 0)
            {
              grp_year_row1.Value = "";
            }
            else
            {
              grp_year_row1.Value = totalTested;
            }

            var grp_year_row2 = ws1.Cells[rowIndex, k + positiveStart];
            grp_year_row2.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            grp_year_row2.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            grp_year_row2.Style.Numberformat.Format = "#,#";
            var totalPositive = item.TestedPositiveSamples.Where(d => d.Year == startYear + k).Single().TotalPositive;
            if (totalPositive == 0)
            {
              grp_year_row2.Value = "";
            }
            else
            {
              grp_year_row2.Value = totalPositive;
            }

            var grp_year_row3 = ws1.Cells[rowIndex, k + percentageStart];
            grp_year_row3.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            //grp_year_row3.Style.Numberformat.Format = "#,#0.00%";
            var totalPercentagePositive = decimal.Round(item.TestedPositiveSamples.Where(d => d.Year == startYear + k).Single().PercentagePositive, 0);
            if (totalPercentagePositive == 0)
            {
              grp_year_row3.Value = "";
            }
            else
            {
              grp_year_row3.Value = totalPercentagePositive;
            }
          }

          //sum
          colIndex++;
          Int32 v1 = 0;
          Int32 v2 = 0;

          for (int k = 0; k < totalYearsInReport; k++)
          {
            var grp_total_col_s1 = ws1.Cells[rowIndex, YearStartCol].Address;
            var grp_total_col_e1 = ws1.Cells[rowIndex, YearStartCol + totalYearsInReport - 1].Address; //-1 to remove the total column from the count
            var grp_total_col_t1 = ws1.Cells[rowIndex, YearStartCol + totalYearsInReport];
            v1 = YearStartCol + totalYearsInReport;
            grp_total_col_t1.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            grp_total_col_t1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            grp_total_col_t1.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
            grp_total_col_t1.Style.Font.Bold = true;
            grp_total_col_t1.Style.Numberformat.Format = "#,#";
            grp_total_col_t1.Formula = String.Format("=SUM({0}:{1})", grp_total_col_s1, grp_total_col_e1);

            var grp_total_col_s2 = ws1.Cells[rowIndex, positiveStart].Address;
            var grp_total_col_e2 = ws1.Cells[rowIndex, positiveStart + totalYearsInReport - 1].Address; //-1 to remove the total column from the count
            var grp_total_col2_t = ws1.Cells[rowIndex, positiveStart + totalYearsInReport];
            v2 = positiveStart + totalYearsInReport;
            grp_total_col2_t.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            grp_total_col2_t.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            grp_total_col2_t.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
            grp_total_col2_t.Style.Font.Bold = true;
            grp_total_col2_t.Style.Numberformat.Format = "#,#";
            grp_total_col2_t.Formula = String.Format("=SUM({0}:{1})", grp_total_col_s2, grp_total_col_e2);

            var grp_total_col_s3 = ws1.Cells[rowIndex, percentageStart].Address;
            var grp_total_col_e3 = ws1.Cells[rowIndex, percentageStart + totalYearsInReport - 1].Address;
            var grp_total_col_t3 = ws1.Cells[rowIndex, percentageStart + totalYearsInReport];
            grp_total_col_t3.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            grp_total_col_t3.Style.Font.Bold = true;
            grp_total_col_t3.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            grp_total_col_t3.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
            grp_total_col_t3.Style.Numberformat.Format = "#,#";
            //grp_total_col_t3.Formula = String.Format("=SUM({0}:{1})/{2}", grp_total_col_s3, grp_total_col_e3, totalYearsInReport);
            grp_total_col_t3.Formula = String.Format("({0}/{1})*100", ws1.Cells[rowIndex, v2].Address, ws1.Cells[rowIndex, v1].Address);
          }

          rowIndex++;
          colIndex = 1;

        }

        rowIndex++;
        rowIndex++;
        colIndex = 1;

        #endregion

        #region SALMONELLA PRODUCT GROUPS, E.G Dairy, Beef and Sheep feed, Pig feed etc.

        Int32 productGroupStartCol = 1;
        Int32 productYearStartCol = 3;
        Int32 productTestedStart = productYearStartCol;
        Int32 productPositiveStart = productYearStartCol + totalYearsInReport + 1; //1 added for the total col, and 1 for the space between sample types
        Int32 productPercentageStart = productPositiveStart + totalYearsInReport + 2;

        //DATA FUNCTIONS
        var productGroups = await GetFinishedProductGroups().ConfigureAwait(false);
        var salmonellaProductGroupCategories = await GetSalmonellaProductGroupCategories();

        //header row - Samples Tested
        //Color colFromHex_grp1 = System.Drawing.ColorTranslator.FromHtml("#999999");

        var row_header_product_group = ws1.Cells[rowIndex, testedStart];
        row_header_product_group.Value = "Samples Tested";
        row_header_product_group.Style.Font.Bold = true;
        row_header_product_group.Style.Font.Color.SetColor(Color.White);
        row_header_product_group.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        row_header_product_group.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        row_header_product_group.Style.Fill.BackgroundColor.SetColor(silverHeader);
        ws1.Cells[rowIndex, productYearStartCol, rowIndex, productPositiveStart - 1].Merge = true;
        ws1.Cells[rowIndex, productYearStartCol, rowIndex, productPositiveStart - 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

        productPositiveStart++;

        //header row - Samples Positive
        var row_header_product_group2 = ws1.Cells[rowIndex, positiveStart];
        row_header_product_group2.Value = "Samples Positive";
        row_header_product_group2.Style.Font.Bold = true;
        row_header_product_group2.Style.Font.Color.SetColor(Color.White);
        row_header_product_group2.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        row_header_product_group2.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        row_header_product_group2.Style.Fill.BackgroundColor.SetColor(silverHeader);
        ws1.Cells[rowIndex, productPositiveStart, rowIndex, productPercentageStart - 1].Merge = true;
        ws1.Cells[rowIndex, productPositiveStart, rowIndex, productPercentageStart - 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

        productPercentageStart++;

        //header row - Samples Positive
        var row_header_product_group3 = ws1.Cells[rowIndex, productPercentageStart];
        row_header_product_group3.Value = "Percentage Positive";
        row_header_product_group3.Style.Font.Bold = true;
        row_header_product_group3.Style.Font.Color.SetColor(Color.White);
        row_header_product_group3.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        row_header_product_group3.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        row_header_product_group3.Style.Fill.BackgroundColor.SetColor(silverHeader);
        ws1.Cells[rowIndex, productPercentageStart, rowIndex, productPercentageStart + totalYearsInReport].Merge = true;
        ws1.Cells[rowIndex, productPercentageStart, rowIndex, productPercentageStart + totalYearsInReport].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

        rowIndex++;

        //colFromHex_yr1 = System.Drawing.ColorTranslator.FromHtml("#c0504d");

        var name_row_code = ws1.Cells[rowIndex, 1];
        name_row_code.Value = "CODE";
        name_row_code.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        name_row_code.Style.Fill.BackgroundColor.SetColor(silverHeader);
        name_row_code.Style.Font.Color.SetColor(Color.White);

        var name_row_product_group = ws1.Cells[rowIndex, 2];
        name_row_product_group.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        name_row_product_group.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        name_row_product_group.Value = "PRODUCT GROUP";
        name_row_product_group.Style.Font.Color.SetColor(Color.White);

        for (int k = 0; k < totalYearsInReport; k++)
        {
          var grp_year_row1 = ws1.Cells[rowIndex, k + productTestedStart];
          grp_year_row1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          grp_year_row1.Style.Fill.BackgroundColor.SetColor(maroonHeader);
          grp_year_row1.Style.Font.Color.SetColor(Color.White);
          grp_year_row1.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
          grp_year_row1.Value = startYear + k;

          var grp_year_row2 = ws1.Cells[rowIndex, k + productPositiveStart];
          grp_year_row2.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          grp_year_row2.Style.Fill.BackgroundColor.SetColor(maroonHeader);
          grp_year_row2.Style.Font.Color.SetColor(Color.White);
          grp_year_row2.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
          grp_year_row2.Value = startYear + k;

          var grp_year_row3 = ws1.Cells[rowIndex, k + productPercentageStart];
          grp_year_row3.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          grp_year_row3.Style.Fill.BackgroundColor.SetColor(maroonHeader);
          grp_year_row3.Style.Font.Color.SetColor(Color.White);
          grp_year_row3.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
          grp_year_row3.Value = startYear + k;
        }

        //total rows
        var total_cols_product_group = ws1.Cells[rowIndex, YearStartCol + totalYearsInReport];
        total_cols_product_group.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        total_cols_product_group.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        total_cols_product_group.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        total_cols_product_group.Style.Font.Color.SetColor(Color.White);
        total_cols_product_group.Value = "TOTAL";

        var total_cols_product_group_pos = ws1.Cells[rowIndex, positiveStart + totalYearsInReport];
        total_cols_product_group_pos.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        total_cols_product_group_pos.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        total_cols_product_group_pos.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        total_cols_product_group_pos.Style.Font.Color.SetColor(Color.White);
        total_cols_product_group_pos.Value = "TOTAL";

        var total_percentage_col_product_group = ws1.Cells[rowIndex, percentageStart + totalYearsInReport];
        total_percentage_col_product_group.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        total_percentage_col_product_group.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        total_percentage_col_product_group.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        total_percentage_col_product_group.Style.Font.Color.SetColor(Color.White);
        total_percentage_col_product_group.Value = "AVERAGE";

        rowIndex++;

        colIndex = 1;
        //colIndex++;

        foreach (var productGroup in productGroups.OrderBy(x => x.Code))
        {
          var group_row_code = ws1.Cells[rowIndex, colIndex];
          group_row_code.Value = productGroup.Code;
          group_row_code.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          group_row_code.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
          group_row_code.Style.Fill.BackgroundColor.SetColor(Color.DarkOliveGreen);
          group_row_code.Style.Font.Color.SetColor(Color.White);

          colIndex++;

          var group_row_text = ws1.Cells[rowIndex, colIndex];
          group_row_text.Value = productGroup.Name;
          group_row_text.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
          group_row_text.Style.Font.Color.SetColor(Color.Black);

          colIndex++;

          for (int k = 0; k < totalYearsInReport; k++)
          {
            var grp_year_row1 = ws1.Cells[rowIndex, k + productTestedStart];
            grp_year_row1.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            grp_year_row1.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            grp_year_row1.Style.Numberformat.Format = "#,#";
            try
            {
              var totalValue = GetTotalSalmonellaByProductGroupByYear(reportCriteria.StartDate, reportCriteria.EndDate, startYear + k, productGroup.Id, 0);
              grp_year_row1.Value = totalValue;
            }
            catch (Exception ex)
            {
              grp_year_row1.Value = "";
            }

            var grp_year_row2 = ws1.Cells[rowIndex, k + productPositiveStart];
            grp_year_row2.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            grp_year_row2.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            grp_year_row2.Style.Numberformat.Format = "#,#";
            try
            {
              //total positive
              var totalPositiveValue = GetTotalSalmonellaByProductGroupByYear(reportCriteria.StartDate, reportCriteria.EndDate, startYear + k, productGroup.Id, 1);
              grp_year_row2.Value = totalPositiveValue;

            }
            catch (Exception ex)
            {
              grp_year_row2.Value = "";
            }

            var grp_year_row3 = ws1.Cells[rowIndex, k + productPercentageStart];
            grp_year_row3.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            grp_year_row3.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            grp_year_row3.Formula = String.Format("={0}/{1}*100", grp_year_row2.Address, grp_year_row1.Address);
            grp_year_row3.Style.Numberformat.Format = "#,#";
            colIndex++;
          }

          //sum
          colIndex++;
          Int32 q1 = 0; // totalYearsInReport + 2;
          Int32 q2 = 0; // (totalYearsInReport + 2) * 2;

          for (int k = 0; k < totalYearsInReport; k++)
          {
            var grp_total_col_s1 = ws1.Cells[rowIndex, productYearStartCol].Address;
            var grp_total_col_e1 = ws1.Cells[rowIndex, productYearStartCol + totalYearsInReport - 1].Address; //-1 to remove the total column from the count
            var grp_total_col_t1 = ws1.Cells[rowIndex, productYearStartCol + totalYearsInReport];
            q1 = productYearStartCol + totalYearsInReport;
            grp_total_col_t1.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            grp_total_col_t1.Style.Font.Bold = true;
            grp_total_col_t1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            grp_total_col_t1.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
            grp_total_col_t1.Style.Numberformat.Format = "#,#";
            grp_total_col_t1.Formula = String.Format("=SUM({0}:{1})", grp_total_col_s1, grp_total_col_e1);

            var grp_total_col_s2 = ws1.Cells[rowIndex, productPositiveStart].Address;
            var grp_total_col_e2 = ws1.Cells[rowIndex, productPositiveStart + totalYearsInReport - 1].Address; //-1 to remove the total column from the count
            var grp_total_col2_t = ws1.Cells[rowIndex, productPositiveStart + totalYearsInReport];
            q2 = productPositiveStart + totalYearsInReport;
            grp_total_col2_t.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            grp_total_col2_t.Style.Font.Bold = true;
            grp_total_col2_t.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            grp_total_col2_t.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
            grp_total_col2_t.Style.Numberformat.Format = "#,#";
            grp_total_col2_t.Formula = String.Format("=SUM({0}:{1})", grp_total_col_s2, grp_total_col_e2);

            var grp_total_col_s3 = ws1.Cells[rowIndex, productPercentageStart].Address;
            var grp_total_col_e3 = ws1.Cells[rowIndex, productPercentageStart + totalYearsInReport - 1].Address;
            var grp_total_col_t3 = ws1.Cells[rowIndex, productPercentageStart + totalYearsInReport];
            grp_total_col_t3.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            grp_total_col_t3.Style.Font.Bold = true;
            grp_total_col_t3.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            grp_total_col_t3.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
            grp_total_col_t3.Formula = String.Format("({0}/{1})*100", ws1.Cells[rowIndex, q2].Address, ws1.Cells[rowIndex, q1].Address);
            //grp_total_col_t3.Formula = String.Format("=SUM({0}:{1})/{2}", grp_total_col_s3, grp_total_col_e3, totalYearsInReport);
            grp_total_col_t3.Style.Numberformat.Format = "#,#";
          }

          rowIndex++;
          colIndex = 1;
        }

        rowIndex++;

        //product groups categories e.g monogastric etc - IGNORE FOR NOW!!!
        foreach (var category in salmonellaProductGroupCategories)
        {
          var category_row_code = ws1.Cells[rowIndex, colIndex];
          category_row_code.Value = category.Id;
          category_row_code.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          category_row_code.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
          category_row_code.Style.Fill.BackgroundColor.SetColor(Color.DarkOliveGreen);
          category_row_code.Style.Font.Color.SetColor(Color.White);

          colIndex++;

          var category_row_text = ws1.Cells[rowIndex, colIndex];
          category_row_text.Value = category.Name;
          //category_row_text.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          //category_row_text.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
          category_row_text.Style.Font.Bold = true;
          //category_row_text.Style.Fill.BackgroundColor.SetColor(Color.White);
          category_row_text.Style.Font.Color.SetColor(Color.Black);

          colIndex++;

          for (int j = 0; j < totalYearsInReport; j++)
          {
            var cat_year_row1 = ws1.Cells[rowIndex, j + productTestedStart];
            cat_year_row1.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            cat_year_row1.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            cat_year_row1.Style.Numberformat.Format = "#,#";
            try
            {
              var totalValue = GetTotalSalmonellaTestedByYearByGroupCategory(reportCriteria.StartDate, reportCriteria.EndDate, category.Id, startYear + j);
              cat_year_row1.Value = totalValue;
            }
            catch
            {
              cat_year_row1.Value = "";
            }

            var cat_year_row2 = ws1.Cells[rowIndex, j + productPositiveStart];
            cat_year_row2.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            cat_year_row2.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            cat_year_row2.Style.Numberformat.Format = "#,#";
            try
            {
              var totalValue = GetTotalSalmonellaPositiveByYearByGroupCategory(reportCriteria.StartDate,
                  reportCriteria.EndDate, category.Id, startYear + j);
              cat_year_row2.Value = totalValue;
            }
            catch
            {
              cat_year_row2.Value = "";
            }

            var cat_year_row3 = ws1.Cells[rowIndex, j + productPercentageStart];
            cat_year_row3.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            cat_year_row3.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            cat_year_row3.Formula = String.Format("={0}/{1}*100", cat_year_row2.Address, cat_year_row1.Address);
            cat_year_row3.Style.Numberformat.Format = "#,#";
          }

          //totals
          colIndex++;
          Int32 s1 = 0; // totalYearsInReport + 2;
          Int32 s2 = 0; // (totalYearsInReport + 2) * 2;

          for (int l = 0; l < totalYearsInReport; l++)
          {
            var grp_total_col_s1 = ws1.Cells[rowIndex, productYearStartCol].Address;
            var grp_total_col_e1 = ws1.Cells[rowIndex, productYearStartCol + totalYearsInReport - 1].Address; //-1 to remove the total column from the count
            var grp_total_col_t1 = ws1.Cells[rowIndex, productYearStartCol + totalYearsInReport];
            s1 = productYearStartCol + totalYearsInReport; //will get col 8 which is the total tested total
            grp_total_col_t1.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            grp_total_col_t1.Style.Font.Bold = true;
            grp_total_col_t1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            grp_total_col_t1.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
            grp_total_col_t1.Style.Numberformat.Format = "#,#";
            grp_total_col_t1.Formula = String.Format("=SUM({0}:{1})", grp_total_col_s1, grp_total_col_e1);

            var grp_total_col_s2 = ws1.Cells[rowIndex, productPositiveStart].Address;
            var grp_total_col_e2 = ws1.Cells[rowIndex, productPositiveStart + totalYearsInReport - 1].Address; //-1 to remove the total column from the count
            var grp_total_col2_t = ws1.Cells[rowIndex, productPositiveStart + totalYearsInReport];
            s2 = productPositiveStart + totalYearsInReport; //wil get col 15 which is the total positive
            grp_total_col2_t.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            grp_total_col2_t.Style.Font.Bold = true;
            grp_total_col2_t.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            grp_total_col2_t.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
            grp_total_col2_t.Style.Numberformat.Format = "#,#";
            grp_total_col2_t.Formula = String.Format("=SUM({0}:{1})", grp_total_col_s2, grp_total_col_e2);

            var grp_total_col_s3 = ws1.Cells[rowIndex, productPercentageStart].Address;
            var grp_total_col_e3 = ws1.Cells[rowIndex, productPercentageStart + totalYearsInReport - 1].Address;
            var grp_total_col_t3 = ws1.Cells[rowIndex, productPercentageStart + totalYearsInReport];
            grp_total_col_t3.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            grp_total_col_t3.Style.Font.Bold = true;
            grp_total_col_t3.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            grp_total_col_t3.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
            grp_total_col_t3.Formula = String.Format("({0}/{1})*100", ws1.Cells[rowIndex, s2].Address, ws1.Cells[rowIndex, s1].Address);
            //grp_total_col_t3.Formula = String.Format("=SUM({0}:{1})/{2}", grp_total_col_s3, grp_total_col_e3, totalYearsInReport);
            grp_total_col_t3.Style.Numberformat.Format = "#,#";
          }

          rowIndex++;
          colIndex = 1;
        }

        #endregion

        #region MONTHLY REPORT

        p.Workbook.Worksheets.Add("Monthly Report");
        ExcelWorksheet ws3 = p.Workbook.Worksheets[2];
        ws3.Name = "Monthly Report";

        //reinit header and variables
        samplesTestedStart = 3;
        samplesPositiveStart = samplesTestedStart + totalMonthsInMonthlyReport + 2; //1 at the end is for total col, need to add another 1 for space. for 12 months, positive tested will start at 17 
        percentagePositiveStart = samplesPositiveStart + totalMonthsInMonthlyReport + 2; //1 at the end is for total, need to add another 1 for space. for 12 months = 31

        //DEFAULT COLUMN WIDTHS AND HEIGHTS
        ws3.Row(1).Height = 22;
        ws3.Row(1).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        ws3.Row(2).Style.TextRotation = 90;
        ws3.Row(2).Height = 65;
        ws3.Row(2).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        ws3.Column(2).Width = 35;

        //header row - Samples Tested
        //Color colFromHex_mnth1 = System.Drawing.ColorTranslator.FromHtml("#999999");
        var row_header_mnth1 = ws3.Cells[1, samplesTestedStart];
        row_header_mnth1.Value = "Samples Tested";
        row_header_mnth1.Style.Font.Bold = true;
        row_header_mnth1.Style.Font.Color.SetColor(Color.White);
        row_header_mnth1.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        row_header_mnth1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        row_header_mnth1.Style.Fill.BackgroundColor.SetColor(silverHeader);
        ws3.Cells[1, 3, 1, samplesPositiveStart - 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        ws3.Cells[1, 3, 1, samplesPositiveStart - 2].Merge = true;

        //header row - Samples Positive
        var row_header_mnth2 = ws3.Cells[1, samplesTestedStart + totalMonthsInMonthlyReport + 2];
        row_header_mnth2.Value = "Samples Positive";
        row_header_mnth2.Style.Font.Bold = true;
        row_header_mnth2.Style.Font.Color.SetColor(Color.White);
        row_header_mnth2.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        row_header_mnth2.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        row_header_mnth2.Style.Fill.BackgroundColor.SetColor(silverHeader);
        ws3.Cells[1, samplesPositiveStart, 1, percentagePositiveStart - 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        ws3.Cells[1, samplesPositiveStart, 1, percentagePositiveStart - 2].Merge = true;

        //header row - Samples Positive
        var row_header_mnth3 = ws3.Cells[1, (samplesPositiveStart + totalMonthsInMonthlyReport + 2)];
        row_header_mnth3.Value = "Percentage Positive";
        row_header_mnth3.Style.Font.Bold = true;
        row_header_mnth3.Style.Font.Color.SetColor(Color.White);
        row_header_mnth3.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        row_header_mnth3.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        row_header_mnth3.Style.Fill.BackgroundColor.SetColor(silverHeader);
        ws3.Cells[1, percentagePositiveStart, 1, percentagePositiveStart + totalMonthsInMonthlyReport].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        ws3.Cells[1, percentagePositiveStart, 1, percentagePositiveStart + totalMonthsInMonthlyReport].Merge = true;

        //second row
        //colFromHex_mnth1 = System.Drawing.ColorTranslator.FromHtml("#c0504d");
        var code_row_mnth = ws3.Cells[2, 1];
        code_row_mnth.Value = "CODE";
        code_row_mnth.Style.TextRotation = 0;
        code_row_mnth.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        code_row_mnth.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        code_row_mnth.Style.Fill.BackgroundColor.SetColor(silverHeader);
        code_row_mnth.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        code_row_mnth.Style.Font.Color.SetColor(Color.White);


        var ingredient_row_mnth = ws3.Cells[2, 2];
        ingredient_row_mnth.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        ingredient_row_mnth.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        ingredient_row_mnth.Style.Font.Color.SetColor(Color.White);
        ingredient_row_mnth.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        ingredient_row_mnth.Value = "FEED INGREDIENT";
        ingredient_row_mnth.Style.TextRotation = 0;

        for (int k = 0; k < totalMonthsInMonthlyReport; k++)
        {
          String _fullDate = String.Format("{0} {1}", System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(reportCriteria.StartDate.AddMonths(k).Month).Substring(0, 3).ToUpper(), reportCriteria.StartDate.AddMonths(k).Year);
          var year_mnth1 = ws3.Cells[2, k + 3];
          year_mnth1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          year_mnth1.Style.Fill.BackgroundColor.SetColor(maroonHeader);
          year_mnth1.Style.Font.Color.SetColor(Color.White);
          year_mnth1.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
          year_mnth1.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Bottom;
          year_mnth1.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          year_mnth1.Value = _fullDate;

          var year_mnth2 = ws3.Cells[2, k + totalMonthsInMonthlyReport + 5];
          year_mnth2.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          year_mnth2.Style.Fill.BackgroundColor.SetColor(maroonHeader);
          year_mnth2.Style.Font.Color.SetColor(Color.White);
          year_mnth2.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
          year_mnth2.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
          year_mnth2.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          year_mnth2.Value = _fullDate;

          var year_mnth3 = ws3.Cells[2, k + totalMonthsInMonthlyReport * 2 + 7];
          year_mnth3.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          year_mnth3.Style.Fill.BackgroundColor.SetColor(maroonHeader);
          year_mnth3.Style.Font.Color.SetColor(Color.White);
          year_mnth3.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
          year_mnth3.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
          year_mnth3.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          year_mnth3.Value = _fullDate;
        }

        //total rows
        var total_cols_mnth = ws3.Cells[2, totalMonthsInMonthlyReport + 3];
        total_cols_mnth.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        total_cols_mnth.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        total_cols_mnth.Style.Font.Color.SetColor(Color.White);
        total_cols_mnth.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        total_cols_mnth.Value = "TOTAL";

        var total_positive_col_mnth = ws3.Cells[2, totalMonthsInMonthlyReport * 2 + 5];
        total_positive_col_mnth.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        total_positive_col_mnth.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        total_positive_col_mnth.Style.Font.Color.SetColor(Color.White);
        total_positive_col_mnth.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        total_positive_col_mnth.Value = "TOTAL";

        var total_percentage_col_mnth = ws3.Cells[2, totalMonthsInMonthlyReport * 3 + 7];
        total_percentage_col_mnth.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        total_percentage_col_mnth.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        total_percentage_col_mnth.Style.Font.Color.SetColor(Color.White);
        total_percentage_col_mnth.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        total_percentage_col_mnth.Value = "AVERAGE";

        rowIndex = 3;
        colIndex = 1;

        foreach (var item in monthlyReport.OrderBy(x => x.SampleTypeId))
        {
          //colFromHex_mnth1 = System.Drawing.ColorTranslator.FromHtml("#4f81bd");
          ws3.Cells[rowIndex, 1, rowIndex, totalMonthsInMonthlyReport * 3 + 7].Merge = true;
          var sample_type_row_mnth = ws3.Cells[rowIndex, colIndex];
          sample_type_row_mnth.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          sample_type_row_mnth.Style.Fill.BackgroundColor.SetColor(blueHeader);
          sample_type_row_mnth.Value = item.SampleType;
          sample_type_row_mnth.Style.Font.Color.SetColor(Color.White);
          rowIndex++;

          String startAddress = "";
          String endAddress = "";

          startRow = rowIndex;
          foreach (var sample in item.SampleTypes.OrderBy(x => x.Sample))
          {
            //colFromHex_mnth1 = System.Drawing.ColorTranslator.FromHtml("#82bc4f");
            var code_row_mnth1 = ws3.Cells[rowIndex, colIndex];
            code_row_mnth1.Value = sample.Code;
            code_row_mnth1.Style.Font.Color.SetColor(Color.White);
            code_row_mnth1.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            code_row_mnth1.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            code_row_mnth1.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            code_row_mnth1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            code_row_mnth1.Style.Fill.BackgroundColor.SetColor(Color.DarkOliveGreen);

            colIndex++;

            var sample_row_mnth1 = ws3.Cells[rowIndex, colIndex];
            sample_row_mnth1.Value = sample.Sample;
            sample_row_mnth1.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            colIndex++;


            //js test 2
            startAddress = ws3.Cells[rowIndex, colIndex].Address;
            for (int k = 0; k < totalMonthsInMonthlyReport + 1; k++)
            {
              foreach (var s in sample.SamplesTestedPositive.Where(d => d.MonthNumber == reportCriteria.StartDate.AddMonths(k).Month && d.Year == reportCriteria.StartDate.AddMonths(k).Year))
              {
                var month_test = ws3.Cells[rowIndex, colIndex];
                month_test.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                if (s.TotalTested == 0)
                {
                  month_test.Value = "";
                }
                else
                {
                  month_test.Style.Numberformat.Format = "#,#";
                  month_test.Value = s.TotalTested;
                }
                colIndex++;
              }

            }
            endAddress = ws3.Cells[rowIndex, colIndex - 1].Address;
            var tt = ws3.Cells[rowIndex, colIndex];
            tt.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            tt.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
            tt.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            tt.Style.Font.Bold = true;
            tt.Style.Numberformat.Format = "#,#";
            tt.Formula = String.Format("=SUM({0}:{1})", startAddress, endAddress);

            colIndex += 2; ;
            //rowIndex++;

            startAddress = ws3.Cells[rowIndex, colIndex].Address;
            for (int k = 0; k < totalMonthsInMonthlyReport + 1; k++)
            {
              foreach (var s in sample.SamplesTestedPositive.Where(d => d.MonthNumber == reportCriteria.StartDate.AddMonths(k).Month && d.Year == reportCriteria.StartDate.AddMonths(k).Year))
              {
                var month_test = ws3.Cells[rowIndex, colIndex];
                month_test.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                if (s.TotalPositive == 0)
                {
                  month_test.Value = "";
                }
                else
                {
                  month_test.Value = s.TotalPositive;
                  month_test.Style.Numberformat.Format = "#,#";
                }
                colIndex++;
              }
            }
            endAddress = ws3.Cells[rowIndex, colIndex - 1].Address;
            var tt1 = ws3.Cells[rowIndex, colIndex];
            tt1.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            tt1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            tt1.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
            tt1.Style.Font.Bold = true;
            tt1.Style.Numberformat.Format = "#,#";
            tt1.Formula = String.Format("=SUM({0}:{1})", startAddress, endAddress);

            colIndex += 2;
            //rowIndex++;

            startAddress = ws3.Cells[rowIndex, colIndex].Address;
            Int32 y1 = totalMonthsInMonthlyReport + 2;
            Int32 y2 = (totalMonthsInMonthlyReport + 2) * 2;

            for (int k = 0; k < totalMonthsInMonthlyReport + 1; k++)
            {
              foreach (var s in sample.SamplesTestedPositive.Where(d => d.MonthNumber == reportCriteria.StartDate.AddMonths(k).Month && d.Year == reportCriteria.StartDate.AddMonths(k).Year))
              {
                var month_test = ws3.Cells[rowIndex, colIndex];
                month_test.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                if (s.PercentagePositive == 0)
                {
                  month_test.Value = ""; ;
                }
                else
                {
                  month_test.Value = Decimal.Round(s.PercentagePositive, 0);
                }
                //month_test.Style.Numberformat.Format = "#,#0.00%";
                colIndex++;
              }
            }
            endAddress = ws3.Cells[rowIndex, colIndex - 1].Address;
            var tt2 = ws3.Cells[rowIndex, colIndex];
            tt2.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            tt2.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            tt2.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
            tt2.Style.Font.Bold = true;
            tt2.Style.Numberformat.Format = "#,#";
            //tt2.Formula = String.Format("=SUM({0}:{1})/{2}", startAddress, endAddress, totalMonthsInMonthlyReport);
            tt2.Formula = String.Format("({0}/{1})*100", ws3.Cells[rowIndex, colIndex - y1].Address, ws3.Cells[rowIndex, colIndex - y2].Address);

            colIndex = 1;
            rowIndex++;
          }
          endRow = rowIndex;

          colIndex = 2;

          var rm_total_text = ws3.Cells[endRow, colIndex];
          rm_total_text.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          rm_total_text.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          rm_total_text.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
          rm_total_text.Style.Font.Bold = true;
          rm_total_text.Value = "TOTALS";
          colIndex++;

          for (int i = 0; i < totalMonthsInMonthlyReport; i++)
          {
            var total_year_value = ws3.Cells[endRow, colIndex + i];
            total_year_value.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            total_year_value.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            total_year_value.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
            total_year_value.Style.Font.Bold = true;
            total_year_value.Style.Numberformat.Format = "#,#";
            total_year_value.Formula = String.Format("=SUM({0}:{1})", ws3.Cells[startRow, colIndex + i].Address, ws3.Cells[endRow - 1, colIndex + i].Address);
          }

          //grand total
          colIndex = colIndex + totalMonthsInMonthlyReport;
          var rm_grand_total = ws3.Cells[endRow, colIndex];
          rm_grand_total.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          rm_grand_total.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          rm_grand_total.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
          rm_grand_total.Style.Font.Bold = true;
          rm_grand_total.Style.Numberformat.Format = "#,#";
          rm_grand_total.Formula = String.Format("=SUM({0}:{1})", ws3.Cells[rowIndex, colIndex - totalMonthsInMonthlyReport].Address, ws3.Cells[rowIndex, colIndex - 1].Address);
          //rowIndex++;

          colIndex += 2;

          for (int i = 0; i < totalMonthsInMonthlyReport; i++)
          {
            var total_year_value = ws3.Cells[endRow, colIndex + i];
            total_year_value.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            total_year_value.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            total_year_value.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
            total_year_value.Style.Font.Bold = true;
            total_year_value.Style.Numberformat.Format = "#,#";
            total_year_value.Formula = String.Format("=SUM({0}:{1})", ws3.Cells[startRow, colIndex + i].Address, ws3.Cells[endRow - 1, colIndex + i].Address);
          }

          //grand total
          colIndex = colIndex + totalMonthsInMonthlyReport;
          var rm_grand_total_pos = ws3.Cells[endRow, colIndex];
          rm_grand_total_pos.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          rm_grand_total_pos.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          rm_grand_total_pos.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
          rm_grand_total_pos.Style.Font.Bold = true;
          rm_grand_total_pos.Style.Numberformat.Format = "#,#";
          rm_grand_total_pos.Formula = String.Format("=SUM({0}:{1})", ws3.Cells[rowIndex, colIndex - totalMonthsInMonthlyReport].Address, ws3.Cells[rowIndex, colIndex - 1].Address);

          colIndex += 2;

          for (int i = 0; i < totalMonthsInMonthlyReport; i++)
          {
            var total_year_value = ws3.Cells[endRow, colIndex + i];
            total_year_value.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            total_year_value.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            total_year_value.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
            total_year_value.Style.Font.Bold = true;
            total_year_value.Style.Numberformat.Format = "#,#";
            total_year_value.Formula = String.Format("=SUM({0}:{1})/{2}", ws3.Cells[startRow, colIndex + i].Address, ws3.Cells[endRow - 1, colIndex + i].Address, endRow - startRow);
          }

          //grand total
          colIndex = colIndex + totalMonthsInMonthlyReport;
          Int32 z1 = (totalMonthsInMonthlyReport + 2) + 1;
          Int32 z2 = ((totalMonthsInMonthlyReport + 2) * 2) + 1;
          var rm_grand_total_per = ws3.Cells[endRow, colIndex];
          rm_grand_total_per.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          rm_grand_total_per.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          rm_grand_total_per.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
          rm_grand_total_per.Style.Font.Bold = true;
          rm_grand_total_per.Style.Numberformat.Format = "#,#";
          //rm_grand_total_per.Formula = String.Format("=SUM({0}:{1}/{2})", ws3.Cells[rowIndex, colIndex - 1].Address, ws3.Cells[rowIndex, colIndex - 1].Address, totalMonthsInMonthlyReport);
          rm_grand_total_per.Formula = String.Format("({0}/{1}*100)", ws3.Cells[rowIndex, z2].Address, ws3.Cells[rowIndex, z1].Address);

          rowIndex++;

          colIndex = 1;

          rowIndex++;

        }

        #endregion

        return p.GetAsByteArray();
      }
    }

    public async Task<byte[]> GenerateDetailSalmonellaExcelReport(ReportCriteriaModel reportCriteria)
    {
      var sampleTypes = await GetSalmonellaSampleTypes().ConfigureAwait(false);

      var sampleId = 0;

      switch (reportCriteria.SampleTypeId)
      {
        case 1:
          sampleId = (int)reportCriteria.MaterialId;
          break;

        case 2:
          sampleId = (int)reportCriteria.ProductId;
          break;

        default:
          sampleId = 0;
          break;
      }

      var results = GetSalmonellaDetailSubmissionSummary(reportCriteria.StartDate, reportCriteria.EndDate, (int)reportCriteria.SampleTypeId, sampleId).ToList();

      var sheetCounter = 0;

      using (ExcelPackage p = new ExcelPackage())
      {
        p.Workbook.Properties.Author = "AFMA";
        p.Workbook.Properties.Title = "AFMA Detail Salmonella Summary";
        p.Workbook.Properties.Company = "www.afma.co.za";

        //DEFAULT COLORS
        var maroonHeader = ColorTranslator.FromHtml("#c0504d");
        var blueHeader = ColorTranslator.FromHtml("#4f81bd");
        var silverHeader = ColorTranslator.FromHtml("#999999");
        var greenHeader = ColorTranslator.FromHtml("#82bc4f");

        var rowIndex = 1;
        var colIndex = 1;

        foreach (var sampleType in sampleTypes)
        {
          if (results.Any(x => x.SampleType == sampleType.Name))
          {
            p.Workbook.Worksheets.Add(sampleType.Name);
            ExcelWorksheet ws = p.Workbook.Worksheets[sheetCounter];
            ws.Name = sampleType.Name;

            ws.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            ws.Cells[rowIndex, colIndex].Style.WrapText = true;
            ws.Cells[rowIndex, colIndex].Style.Font.Bold = true;
            ws.Column(colIndex).Width = 10;
            ws.Cells[rowIndex, colIndex].Value = "Report Id";
            colIndex++;

            ws.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            ws.Cells[rowIndex, colIndex].Style.WrapText = true;
            ws.Cells[rowIndex, colIndex].Style.Font.Bold = true;
            ws.Cells[rowIndex, colIndex].Value = "Info For";
            ws.Column(colIndex).Width = 15;
            colIndex++;

            ws.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            ws.Cells[rowIndex, colIndex].Style.WrapText = true;
            ws.Cells[rowIndex, colIndex].Style.Font.Bold = true;
            ws.Cells[rowIndex, colIndex].Value = "SampleIdentifier";
            ws.Column(colIndex).Width = 18;
            colIndex++;

            ws.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            ws.Cells[rowIndex, colIndex].Style.WrapText = true;
            ws.Cells[rowIndex, colIndex].Style.Font.Bold = true;
            ws.Cells[rowIndex, colIndex].Value = "Company";
            ws.Column(colIndex).Width = 25;
            colIndex++;

            ws.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            ws.Cells[rowIndex, colIndex].Style.WrapText = true;
            ws.Cells[rowIndex, colIndex].Style.Font.Bold = true;
            ws.Cells[rowIndex, colIndex].Value = "Branch";
            ws.Column(colIndex).Width = 25;
            colIndex++;

            ws.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            ws.Cells[rowIndex, colIndex].Style.WrapText = true;
            ws.Cells[rowIndex, colIndex].Style.Font.Bold = true;
            ws.Cells[rowIndex, colIndex].Value = "Sample Type";
            ws.Column(colIndex).Width = 18;
            colIndex++;

            ws.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            ws.Cells[rowIndex, colIndex].Style.WrapText = true;
            ws.Cells[rowIndex, colIndex].Style.Font.Bold = true;
            ws.Cells[rowIndex, colIndex].Value = "Sample Id";
            ws.Column(colIndex).Width = 30;
            colIndex++;

            ws.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            ws.Cells[rowIndex, colIndex].Style.WrapText = true;
            ws.Cells[rowIndex, colIndex].Style.Font.Bold = true;
            ws.Cells[rowIndex, colIndex].Value = "Sample Detail";
            ws.Column(colIndex).Width = 30;
            colIndex++;

            ws.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            ws.Cells[rowIndex, colIndex].Style.WrapText = true;
            ws.Cells[rowIndex, colIndex].Style.Font.Bold = true;
            ws.Cells[rowIndex, colIndex].Value = "Product Group";
            ws.Column(colIndex).Width = 30;
            colIndex++;

            ws.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            ws.Cells[rowIndex, colIndex].Style.WrapText = true;
            ws.Cells[rowIndex, colIndex].Style.Font.Bold = true;
            ws.Cells[rowIndex, colIndex].Value = "Additional Info";
            ws.Column(colIndex).Width = 30;
            colIndex++;

            ws.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            ws.Cells[rowIndex, colIndex].Style.WrapText = true;
            ws.Cells[rowIndex, colIndex].Style.Font.Bold = true;
            ws.Cells[rowIndex, colIndex].Value = "Note";
            ws.Column(colIndex).Width = 40;
            colIndex++;

            ws.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            ws.Cells[rowIndex, colIndex].Style.WrapText = true;
            ws.Cells[rowIndex, colIndex].Style.Font.Bold = true;
            ws.Cells[rowIndex, colIndex].Value = "Salmonella Detected";
            ws.Column(colIndex).Width = 15;
            colIndex++;

            ws.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            ws.Cells[rowIndex, colIndex].Style.WrapText = true;
            ws.Cells[rowIndex, colIndex].Style.Font.Bold = true;
            ws.Cells[rowIndex, colIndex].Value = "Type Name";
            ws.Column(colIndex).Width = 30;
            colIndex++;

            ws.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            ws.Cells[rowIndex, colIndex].Style.WrapText = true;
            ws.Cells[rowIndex, colIndex].Style.Font.Bold = true;
            ws.Cells[rowIndex, colIndex].Value = "Group Name";
            ws.Column(colIndex).Width = 30;
            colIndex++;

            rowIndex++;
            colIndex = 1;

            foreach (var item in results.Where(x => x.SampleType == sampleType.Name))
            {
              ws.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              ws.Cells[rowIndex, colIndex].Style.WrapText = true;
              ws.Cells[rowIndex, colIndex].Style.Font.Bold = false;
              ws.Cells[rowIndex, colIndex].Value = item.ReportId;
              colIndex++;

              ws.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              ws.Cells[rowIndex, colIndex].Style.WrapText = true;
              ws.Cells[rowIndex, colIndex].Style.Font.Bold = false;
              ws.Cells[rowIndex, colIndex].Value = item.InfoFor.ToString("d");
              colIndex++;

              ws.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              ws.Cells[rowIndex, colIndex].Style.WrapText = true;
              ws.Cells[rowIndex, colIndex].Style.Font.Bold = false;
              ws.Cells[rowIndex, colIndex].Value = item.SampleIdentifier;
              colIndex++;

              ws.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              ws.Cells[rowIndex, colIndex].Style.WrapText = true;
              ws.Cells[rowIndex, colIndex].Style.Font.Bold = false;
              ws.Cells[rowIndex, colIndex].Value = item.Member;
              colIndex++;

              ws.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              ws.Cells[rowIndex, colIndex].Style.WrapText = true;
              ws.Cells[rowIndex, colIndex].Style.Font.Bold = false;
              ws.Cells[rowIndex, colIndex].Value = item.BranchName;
              colIndex++;

              ws.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              ws.Cells[rowIndex, colIndex].Style.WrapText = true;
              ws.Cells[rowIndex, colIndex].Style.Font.Bold = false;
              ws.Cells[rowIndex, colIndex].Value = item.SampleType;
              colIndex++;

              ws.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              ws.Cells[rowIndex, colIndex].Style.WrapText = true;
              ws.Cells[rowIndex, colIndex].Style.Font.Bold = false;
              ws.Cells[rowIndex, colIndex].Value = item.SampleId;
              colIndex++;

              ws.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              ws.Cells[rowIndex, colIndex].Style.WrapText = true;
              ws.Cells[rowIndex, colIndex].Style.Font.Bold = false;
              ws.Cells[rowIndex, colIndex].Value = item.SampleDetail;
              colIndex++;

              ws.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              ws.Cells[rowIndex, colIndex].Style.WrapText = true;
              ws.Cells[rowIndex, colIndex].Style.Font.Bold = false;
              ws.Cells[rowIndex, colIndex].Value = item.ProductGroup;
              colIndex++;

              ws.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              ws.Cells[rowIndex, colIndex].Style.WrapText = true;
              ws.Cells[rowIndex, colIndex].Style.Font.Bold = false;
              ws.Cells[rowIndex, colIndex].Value = item.AdditionalInfo;
              colIndex++;

              ws.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              ws.Cells[rowIndex, colIndex].Style.WrapText = true;
              ws.Cells[rowIndex, colIndex].Style.Font.Bold = false;
              ws.Cells[rowIndex, colIndex].Value = item.Note;
              colIndex++;

              if (item.SalmonellaDetected == null)
              {
                item.SalmonellaDetected = false;
              }

              ws.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              ws.Cells[rowIndex, colIndex].Style.WrapText = true;
              ws.Cells[rowIndex, colIndex].Style.Font.Bold = false;
              ws.Cells[rowIndex, colIndex].Value = item.SalmonellaDetected.ToBoolean() ? "Yes" : "No";
              colIndex++;

              ws.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              ws.Cells[rowIndex, colIndex].Style.WrapText = true;
              ws.Cells[rowIndex, colIndex].Style.Font.Bold = false;
              ws.Cells[rowIndex, colIndex].Value = item.SalmonellaDetected.ToBoolean() ? item.TypeName : "";
              colIndex++;

              ws.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              ws.Cells[rowIndex, colIndex].Style.WrapText = true;
              ws.Cells[rowIndex, colIndex].Style.Font.Bold = false;
              ws.Cells[rowIndex, colIndex].Value = item.SalmonellaDetected.ToBoolean() ? item.GroupName : "";
              colIndex++;

              rowIndex++;
              colIndex = 1;

            }

            rowIndex = 1;
            colIndex = 1;
            sheetCounter++;
          }
        }

        try
        {
          return p.GetAsByteArray();
        }
        catch
        {
          p.Workbook.Worksheets.Add("Sheet1");
          ExcelWorksheet ws = p.Workbook.Worksheets[1];
          ws.Name = "Sheet1";

          ws.Cells[1, 1].Value = "Your report criteria yielded no results!";

          return p.GetAsByteArray();
        }
      }
    }

    public async Task<byte[]> GenerateSalmonellaClientExcelReport(ReportCriteriaModel reportCriteria)
    {

      var yearByMonthReport =
        await GetSalmonellaYearByMonthReversedExcel(reportCriteria.StartDate, reportCriteria.EndDate,
          reportCriteria.MemberBranchId).ConfigureAwait(false);

      if (!yearByMonthReport.Any())
      {
        throw new Exception("No data found for report");
      }

      System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
      var totalMonthsInReport = 12;
      var reportMonths = GeneralHelpers.MonthsBetween(reportCriteria.StartDate, reportCriteria.EndDate).ToList();

      var monthCount = new int[reportMonths.Count()];

      for (int i = 0; i < reportMonths.Count(); i++)
      {
        monthCount[i] = reportMonths.ElementAt(i).Month;
      }

      //columns start index
      var samplesTestedStart = 3;
      var samplesPositiveStart = samplesTestedStart + totalMonthsInReport + 2; //1 at the end is for total col, need to add another 1 for space. for 12 months, positive tested will start at 17 
      var percentagePositiveStart = samplesPositiveStart + totalMonthsInReport + 2; //1 at the end is for total, need to add another 1 for space. for 12 months = 31

      var seasonalData =
        await GetSalmonellaYearByMonthSeasonal(reportCriteria.StartDate, reportCriteria.EndDate,
          reportCriteria.MemberBranchId).ConfigureAwait(false);

      var totalYearsInReport = (reportCriteria.EndDate.Year - reportCriteria.StartDate.Year - 1) +
                               (((reportCriteria.EndDate.Month > reportCriteria.StartDate.Month) ||
                                 ((reportCriteria.EndDate.Month == reportCriteria.StartDate.Month) &&
                                  (reportCriteria.EndDate.Day >= reportCriteria.StartDate.Day)))
                                 ? 1
                                 : 0) + 1;

      var months = await GetSeasonalMonths().ConfigureAwait(false);
      var startYear = yearByMonthReport.First().Years[0];
      var yearCount = yearByMonthReport.First().Years.Length;

      using (ExcelPackage p = new ExcelPackage())
      {
        p.Workbook.Properties.Author = "AFMA";
        p.Workbook.Properties.Title = "AFMA Total Salmonella Report";
        p.Workbook.Properties.Company = "www.afma.co.za";

        #region YEAR BY MONTH REPORT

        p.Workbook.Worksheets.Add("Year By Month Report");
        var ws = p.Workbook.Worksheets[0];
        ws.Name = "Year By Month Report";

        //DEFUALT COLUMN WIDTHS
        ws.Column(1).Width = 20;
        ws.Row(1).Height = 32;
        ws.Row(2).Height = 25;

        //DEFAULT COLORS
        var maroonHeader = ColorTranslator.FromHtml("#c0504d");
        var blueHeader = ColorTranslator.FromHtml("#4f81bd");
        var silverHeader = ColorTranslator.FromHtml("#999999");
        var greenHeader = ColorTranslator.FromHtml("#82bc4f");
        var startRow = 0;
        var endRow = 0;

        //PLACEHOLDERS
        var _rmTotalRowNo = 0;
        var _foTotalRowNo = 0;
        var _dsTotalRowNo = 0;

        //header row - Samples Tested
        var mainHeading = ws.Cells[1, 1];
        mainHeading.Style.Font.Bold = true;
        mainHeading.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        mainHeading.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
        mainHeading.Style.WrapText = true;
        mainHeading.Value =
          $"{_applicationLayer.GetMemberNameFromId((int)reportCriteria.MemberId!).Result.ToUpper()}\r\n{await _applicationLayer.GetMemberBranchNameFromId((int)reportCriteria.MemberBranchId!)}";

        //Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#999999");
        var row_header2 = ws.Cells[1, samplesTestedStart];
        row_header2.Value = "Samples Tested";
        row_header2.Style.Font.Bold = true;
        row_header2.Style.Font.Color.SetColor(Color.White);
        row_header2.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        row_header2.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        row_header2.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        row_header2.Style.Fill.BackgroundColor.SetColor(silverHeader);
        ws.Cells[1, 3, 1, samplesPositiveStart - 2].Merge = true;
        ws.Cells[1, 3, 1, samplesPositiveStart - 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

        //header row - Samples Positive
        var row_header1 = ws.Cells[1, samplesTestedStart + totalMonthsInReport + 2];
        row_header1.Value = "Samples Positive";
        row_header1.Style.Font.Bold = true;
        row_header1.Style.Font.Color.SetColor(Color.White);
        row_header1.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        row_header1.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        row_header1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        row_header1.Style.Fill.BackgroundColor.SetColor(silverHeader);
        ws.Cells[1, samplesPositiveStart, 1, percentagePositiveStart - 2].Merge = true;
        ws.Cells[1, samplesPositiveStart, 1, percentagePositiveStart - 2].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

        //header row - Samples Positive
        var row_header3 = ws.Cells[1, (samplesPositiveStart + totalMonthsInReport + 2)];
        row_header3.Value = "Percentage Positive";
        row_header3.Style.Font.Bold = true;
        row_header3.Style.Font.Color.SetColor(Color.White);
        row_header3.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        row_header3.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        row_header3.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        row_header3.Style.Fill.BackgroundColor.SetColor(silverHeader);
        ws.Cells[1, percentagePositiveStart, 1, percentagePositiveStart + totalMonthsInReport + 1].Merge = true;
        ws.Cells[1, percentagePositiveStart, 1, percentagePositiveStart + totalMonthsInReport + 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

        //second row
        //colFromHex = System.Drawing.ColorTranslator.FromHtml("#c0504d");
        var code_row = ws.Cells[2, 1];
        code_row.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        code_row.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        code_row.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        code_row.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        code_row.Style.Font.Color.SetColor(Color.White);
        code_row.Value = "CODE";

        var ingredient_row = ws.Cells[2, 2];
        ingredient_row.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        ingredient_row.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        ingredient_row.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        ingredient_row.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        ingredient_row.Style.Font.Color.SetColor(Color.White);
        ingredient_row.Value = "YEAR";

        for (int i = 1; i < totalMonthsInReport + 1; i++)
        {
          var month_row1 = ws.Cells[2, i + 2];

          month_row1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          month_row1.Style.Fill.BackgroundColor.SetColor(maroonHeader);
          month_row1.Style.Font.Color.SetColor(Color.White);
          month_row1.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
          month_row1.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
          month_row1.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          month_row1.Value = mfi.GetMonthName(i).Substring(0, 3).ToUpper();
          var season = months.Single(f => f.MonthNumber == i).Season;
          if (season == (int)Season.Winter)
          {
            month_row1.Style.Fill.BackgroundColor.SetColor(greenHeader);
          }

          var month_row2 = ws.Cells[2, i + totalMonthsInReport + 4];
          month_row2.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          month_row2.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          month_row2.Style.Fill.BackgroundColor.SetColor(maroonHeader);
          month_row2.Style.Font.Color.SetColor(Color.White);
          month_row2.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
          month_row2.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
          month_row2.Value = mfi.GetMonthName(i).Substring(0, 3).ToUpper();
          if (season == (int)Season.Winter)
          {
            month_row2.Style.Fill.BackgroundColor.SetColor(greenHeader);
          }


          var month_row3 = ws.Cells[2, i + totalMonthsInReport * 2 + 6];
          month_row3.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          month_row3.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          month_row3.Style.Fill.BackgroundColor.SetColor(maroonHeader);
          month_row3.Style.Font.Color.SetColor(Color.White);
          month_row3.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
          month_row3.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
          month_row3.Value = mfi.GetMonthName(i).Substring(0, 3).ToUpper();
          if (season == (int)Season.Winter)
          {
            month_row3.Style.Fill.BackgroundColor.SetColor(greenHeader);
          }
        }

        //total rows
        var total_cols = ws.Cells[2, totalMonthsInReport + 3];
        total_cols.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        total_cols.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        total_cols.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        total_cols.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        total_cols.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        total_cols.Style.Font.Color.SetColor(Color.White);
        total_cols.Value = "TOTAL";

        var total_positive_col = ws.Cells[2, totalMonthsInReport * 2 + 5];
        total_positive_col.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        total_positive_col.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        total_positive_col.Style.Font.Color.SetColor(Color.White);
        total_positive_col.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        total_positive_col.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        total_positive_col.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        total_positive_col.Value = "TOTAL";

        var total_percentage_col = ws.Cells[2, totalMonthsInReport * 3 + 7];
        total_percentage_col.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        total_percentage_col.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        total_percentage_col.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        total_percentage_col.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        total_percentage_col.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        total_percentage_col.Style.Font.Color.SetColor(Color.White);
        total_percentage_col.Value = "AVERAGE";

        Int32 rowIndex = 3;
        Int32 colIndex = 1;

        foreach (var item in yearByMonthReport.OrderBy(x => x.SampleTypeId))
        {
          //sample type header row
          //colFromHex = System.Drawing.ColorTranslator.FromHtml("#4f81bd");
          var code_row1 = ws.Cells[rowIndex, colIndex];
          code_row1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          code_row1.Style.Fill.BackgroundColor.SetColor(Color.DarkOliveGreen);
          colIndex++;

          ws.Cells[rowIndex, 2, rowIndex, totalMonthsInReport * 3 + 7].Merge = true;
          var sampletype_row = ws.Cells[rowIndex, colIndex];
          sampletype_row.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          sampletype_row.Style.Fill.BackgroundColor.SetColor(blueHeader);
          sampletype_row.Value = item.SampleType;
          sampletype_row.Style.Font.Color.SetColor(Color.White);

          rowIndex++;
          startRow = rowIndex; //used to see where the total columns will be located
          for (int y = startYear; y < (startYear + yearCount); y++)
          {
            colIndex = 1;
            var code_row2 = ws.Cells[rowIndex, colIndex];
            code_row2.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            code_row2.Style.Fill.BackgroundColor.SetColor(Color.DarkOliveGreen);
            colIndex++;

            var year_row = ws.Cells[rowIndex, colIndex];
            year_row.Value = y;
            year_row.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            colIndex++;
            foreach (var month in months)
            {
              var season = months.Where(f => f.MonthNumber == month.MonthNumber).Single().Season;

              foreach (var sample in item.SamplesTestedPositive.Where(d => d.Year == y && d.Month == month.MonthNumber && d.SampleType == item.SampleType))
              {
                var count_row = ws.Cells[rowIndex, colIndex];
                if (sample.TotalTested == 0)
                {
                  count_row.Value = String.Empty;
                }
                else
                {
                  count_row.Style.Numberformat.Format = "#,#";
                  count_row.Value = sample.TotalTested;
                }
                count_row.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                var positive_row = ws.Cells[rowIndex, colIndex + 14];

                if (sample.TotalPositive == 0)
                {
                  positive_row.Value = String.Empty;
                }
                else
                {
                  positive_row.Value = sample.TotalPositive;
                  positive_row.Style.Numberformat.Format = "#,#";
                }

                positive_row.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                var percentage_row = ws.Cells[rowIndex, colIndex + 28];

                if (sample.TotalPositive == 0)
                {
                  percentage_row.Value = String.Empty;
                }
                else
                {
                  percentage_row.Style.Numberformat.Format = "#,#";
                  percentage_row.Value = sample.PercentagePositive;
                }

                percentage_row.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                colIndex++;
              }


            }
            var total_tested_row = ws.Cells[rowIndex, colIndex];
            total_tested_row.Formula = String.Format("SUM(C{0}:N{0})", rowIndex);
            total_tested_row.Style.Numberformat.Format = "#,#";
            total_tested_row.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            total_tested_row.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            total_tested_row.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);

            var total_positive_row = ws.Cells[rowIndex, colIndex + 14];
            total_positive_row.Formula = String.Format("SUM(Q{0}:AB{0})", rowIndex);
            total_positive_row.Style.Numberformat.Format = "#,#";
            total_positive_row.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            total_positive_row.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            total_positive_row.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);

            var total_percentage_row = ws.Cells[rowIndex, colIndex + 28];
            //total_percentage_row.Formula = String.Format("SUM(AE{0}:AP{0})/12", rowIndex);
            total_percentage_row.Formula = String.Format("({0}/{1})*100", total_positive_row.Address, total_tested_row.Address);
            total_percentage_row.Style.Numberformat.Format = "#,#";
            total_percentage_row.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            total_percentage_row.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            total_percentage_row.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);

            rowIndex++;
          }
          endRow = rowIndex - 1;

          //total rows
          if (_rmTotalRowNo == 0)
          {
            _rmTotalRowNo = rowIndex;
            _foTotalRowNo = _rmTotalRowNo + totalYearsInReport + 2;
            _dsTotalRowNo = _foTotalRowNo + totalYearsInReport + 2;
          }

          colIndex = 1;
          var total_row_ds = ws.Cells[rowIndex, colIndex];
          total_row_ds.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          total_row_ds.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
          total_row_ds.Style.Fill.BackgroundColor.SetColor(Color.DarkOliveGreen);
          total_row_ds.Style.Font.Color.SetColor(Color.White);
          total_row_ds.Value = "TOTAL";
          colIndex++;
          colIndex++;

          for (int i = 1; i < totalMonthsInReport + 2; i++) //added 2 here to count the last total as well
          {
            var total_row_ds_value = ws.Cells[rowIndex, colIndex];
            total_row_ds_value.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            total_row_ds_value.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
            total_row_ds_value.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            total_row_ds_value.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
            total_row_ds_value.Style.Font.Color.SetColor(Color.Black);
            total_row_ds_value.Style.Font.Bold = true;
            total_row_ds_value.Formula = String.Format("=SUM({0}:{1})", ws.Cells[startRow, colIndex].Address, ws.Cells[endRow, colIndex].Address);
            colIndex++;
          }

          colIndex++;
          for (int i = 1; i < totalMonthsInReport + 2; i++) //added 2 here to count the last total as well
          {
            var total_row_ds_value = ws.Cells[rowIndex, colIndex];
            total_row_ds_value.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            total_row_ds_value.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
            total_row_ds_value.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            total_row_ds_value.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
            total_row_ds_value.Style.Font.Color.SetColor(Color.Black);
            total_row_ds_value.Style.Font.Bold = true;
            total_row_ds_value.Formula = String.Format("=SUM({0}:{1})", ws.Cells[startRow, colIndex].Address, ws.Cells[endRow, colIndex].Address);
            total_row_ds_value.Style.Numberformat.Format = "#,#";
            colIndex++;
          }

          colIndex++;
          for (int i = 1; i < totalMonthsInReport + 2; i++) //added 2 here to count the last total as well
          {
            var total_row_ds_value = ws.Cells[rowIndex, colIndex];
            total_row_ds_value.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            total_row_ds_value.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
            total_row_ds_value.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            total_row_ds_value.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
            total_row_ds_value.Style.Font.Color.SetColor(Color.Black);
            total_row_ds_value.Style.Font.Bold = true;
            //total_row_ds_value.Formula = String.Format("({0}/{1})*100", total_positive_row.Address, total_tested_row.Address);
            total_row_ds_value.Formula = String.Format("({0}/{1})*100", ws.Cells[rowIndex, colIndex - 14].Address, ws.Cells[rowIndex, colIndex - 28].Address);
            total_row_ds_value.Style.Numberformat.Format = "#,#";
            colIndex++;
          }

          rowIndex += 2;
          colIndex = 1;
        }

        rowIndex++;

        #region MONTHLY/YEARLY TOTALS

        colIndex = 2;
        var my_totals_header = ws.Cells[rowIndex + 1, colIndex - 1];
        my_totals_header.Value = "TOTAL SAMPLES";
        my_totals_header.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        my_totals_header.Style.Fill.BackgroundColor.SetColor(Color.DarkOliveGreen);
        my_totals_header.Style.Font.Color.SetColor(Color.White);
        my_totals_header.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

        rowIndex++;

        for (int x = 0; x < totalYearsInReport; x++)
        {
          var my_year = ws.Cells[rowIndex, colIndex];
          my_year.Value = startYear + x;
          my_year.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

          colIndex++;

          foreach (var month in months)
          {
            var my_row_tested = ws.Cells[rowIndex, colIndex];
            my_row_tested.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            my_row_tested.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            my_row_tested.Style.Font.Bold = true;
            my_row_tested.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
            my_row_tested.Style.Numberformat.Format = "#,#";
            try
            {
              if (monthCount.Contains(month.MonthNumber))
              {
                var sqlQuery =
                  $"DECLARE @return_value int;SELECT COUNT(SalmonellaSample.Id) FROM SalmonellaReport LEFT JOIN SalmonellaSample ON SalmonellaReport.Id = SalmonellaSample.SalmonellaReportId WHERE SalmonellaReport.ReportYear = {startYear + x} AND SalmonellaReport.ReportMonth = {month.MonthNumber} AND SalmonellaReport.MemberBranchId = {reportCriteria.MemberBranchId};SELECT 'Return Value' = @return_value";
                var totalValue = ExecuteSqlQuery<int>(sqlQuery);
                my_row_tested.Value = totalValue;
              }
              else
              {
                my_row_tested.Value = "";
              }
            }
            catch
            {
              my_row_tested.Value = "";
            }

            var my_row_positive = ws.Cells[rowIndex, colIndex + 14];
            my_row_positive.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            my_row_positive.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            my_row_positive.Style.Font.Bold = true;
            my_row_positive.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
            my_row_positive.Style.Numberformat.Format = "#,#";
            try
            {
              if (monthCount.Contains(month.MonthNumber))
              {
                var sqlQuery =
                  $"DECLARE @return_value int;SELECT COUNT(SalmonellaSample.Id) FROM SalmonellaReport LEFT JOIN SalmonellaSample ON SalmonellaReport.Id = SalmonellaSample.SalmonellaReportId WHERE SalmonellaReport.ReportYear = {startYear + x} AND SalmonellaReport.ReportMonth = {month.MonthNumber} AND SalmonellaSample.SalmonellaDetected = 1 AND SalmonellaReport.MemberBranchId = {reportCriteria.MemberBranchId};SELECT 'Return Value' = @return_value";
                var totalValue = ExecuteSqlQuery<int>(sqlQuery);
                my_row_positive.Value = totalValue;
              }
              else
              {
                my_row_positive.Value = 0;
              }
            }
            catch
            {
              my_row_positive.Value = "";
            }

            var my_row_positive_percentage = ws.Cells[rowIndex, colIndex + 28];
            my_row_positive_percentage.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            my_row_positive_percentage.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            my_row_positive_percentage.Style.Font.Bold = true;
            my_row_positive_percentage.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
            my_row_positive_percentage.Style.Numberformat.Format = "#,#";
            var _startIndex = 3;
            var _increment = 12;
            try
            {
              my_row_positive_percentage.Formula =
                $"=({ws.Cells[rowIndex, colIndex + 14].Address}/{ws.Cells[rowIndex, colIndex].Address})*100";
            }
            catch
            {
              my_row_positive_percentage.Value = "";
            }

            colIndex++;
          }

          //totals for the row
          var my_total_tested_row = ws.Cells[rowIndex, colIndex];
          my_total_tested_row.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          my_total_tested_row.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          my_total_tested_row.Style.Font.Bold = true;
          my_total_tested_row.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
          my_total_tested_row.Formula = $"=SUM({ws.Cells[rowIndex, 3].Address}:{ws.Cells[rowIndex, 14].Address})";

          var my_total_positive_row = ws.Cells[rowIndex, colIndex + 14];
          my_total_positive_row.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          my_total_positive_row.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          my_total_positive_row.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
          my_total_positive_row.Style.Font.Bold = true;
          my_total_positive_row.Formula = $"=SUM({ws.Cells[rowIndex, 17].Address}:{ws.Cells[rowIndex, 28].Address})";

          var my_total_percentage_positive = ws.Cells[rowIndex, colIndex + 28];
          var startIndex = 3;

          //bottom total rows
          rowIndex++;

          var _testedStart = 0;
          var _positveStart = 14;
          var _percentageStart = 28;
          colIndex = 3;
          foreach (var month in months)
          {
            var year_total_row_bottom_tested = ws.Cells[rowIndex, colIndex];
            year_total_row_bottom_tested.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            year_total_row_bottom_tested.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            year_total_row_bottom_tested.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
            year_total_row_bottom_tested.Style.Font.Bold = true;
            year_total_row_bottom_tested.Formula =
              $"=SUM({ws.Cells[rowIndex - totalYearsInReport, colIndex].Address}:{ws.Cells[rowIndex - 1, colIndex].Address})";

            var year_total_row_bottom_positive = ws.Cells[rowIndex, colIndex + _positveStart];
            year_total_row_bottom_positive.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            year_total_row_bottom_positive.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            year_total_row_bottom_positive.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
            year_total_row_bottom_positive.Style.Font.Bold = true;
            year_total_row_bottom_positive.Formula =
              $"=SUM({ws.Cells[rowIndex - totalYearsInReport, colIndex + _positveStart].Address}:{ws.Cells[rowIndex - 1, colIndex + _positveStart].Address})";

            var year_total_row_bottom_percentage = ws.Cells[rowIndex, colIndex + _percentageStart];
            year_total_row_bottom_percentage.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            year_total_row_bottom_percentage.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            year_total_row_bottom_percentage.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
            year_total_row_bottom_percentage.Style.Font.Bold = true;
            //year_total_row_bottom_percentage.Formula = String.Format("({0}/{1})*100", ws.Cells[rowIndex - totalYearsInReport, colIndex + _percentageStart].Address, ws.Cells[rowIndex - 1, colIndex + _percentageStart].Address, totalYearsInReport);
            year_total_row_bottom_percentage.Formula =
              $"({ws.Cells[rowIndex, (colIndex + _percentageStart) - 14].Address}/{ws.Cells[rowIndex, (colIndex + _percentageStart) - 28].Address})*100";
            year_total_row_bottom_percentage.Style.Numberformat.Format = "#,#";

            colIndex++;
          }

          //colIndex++;

          var my_grand_total_tested_row = ws.Cells[rowIndex - 1, 15];
          my_total_tested_row.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          my_total_tested_row.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          my_total_tested_row.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
          my_total_tested_row.Style.Font.Bold = true;
          my_total_tested_row.Formula =
            $"=SUM({ws.Cells[rowIndex - 1, 3].Address}:{ws.Cells[rowIndex - 1, 14].Address})";

          var my_grand_total_positive_row = ws.Cells[rowIndex - 1, 29];
          my_total_positive_row.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          my_total_positive_row.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          my_total_positive_row.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
          my_total_positive_row.Style.Font.Bold = true;
          my_total_positive_row.Formula =
            $"=SUM({ws.Cells[rowIndex - 1, 17].Address}:{ws.Cells[rowIndex - 1, 28].Address})";

          var my_grand_total_percentage_positive = ws.Cells[rowIndex - 1, 43];
          my_grand_total_percentage_positive.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          my_grand_total_percentage_positive.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          my_grand_total_percentage_positive.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
          my_grand_total_percentage_positive.Style.Font.Bold = true;
          my_grand_total_percentage_positive.Formula =
            $"({ws.Cells[rowIndex, 29].Address}/{ws.Cells[rowIndex, 15].Address})*100";
          my_grand_total_percentage_positive.Style.Numberformat.Format = "#,#";

          colIndex = 2;

        }

        var grand_total_tested_cell = ws.Cells[rowIndex, 15];
        grand_total_tested_cell.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        grand_total_tested_cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        grand_total_tested_cell.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        grand_total_tested_cell.Style.Numberformat.Format = "#,#";
        grand_total_tested_cell.Style.Font.Bold = true;
        grand_total_tested_cell.Formula = $"=SUM({ws.Cells[rowIndex, 3].Address}:{ws.Cells[rowIndex, 14].Address})";


        var grand_total_positive_cell = ws.Cells[rowIndex, 29];
        grand_total_positive_cell.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        grand_total_positive_cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        grand_total_positive_cell.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        grand_total_positive_cell.Style.Numberformat.Format = "#,#";
        grand_total_positive_cell.Style.Font.Bold = true;
        grand_total_positive_cell.Formula = $"=SUM({ws.Cells[rowIndex, 17].Address}:{ws.Cells[rowIndex, 28].Address})";

        var grand_total_percentage_cell = ws.Cells[rowIndex, 43];
        grand_total_percentage_cell.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        grand_total_percentage_cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        grand_total_percentage_cell.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        grand_total_percentage_cell.Style.Numberformat.Format = "#,#";
        grand_total_percentage_cell.Style.Font.Bold = true;
        grand_total_percentage_cell.Formula =
          $"({ws.Cells[rowIndex, 29].Address}/{ws.Cells[rowIndex, 15].Address})*100";

        #endregion

        rowIndex++;
        rowIndex++;
        colIndex = 3;

        #region SEASONAL DATA
        //SEASONAL DATA

        //header row - Samples Tested
        ws.Cells[rowIndex, 1, rowIndex, 10].Merge = true;
        ws.Cells[rowIndex, 1, rowIndex, 10].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        var header_row = ws.Cells[rowIndex, 1];
        header_row.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        header_row.Style.Font.Color.SetColor(Color.White);
        header_row.Style.Fill.BackgroundColor.SetColor(Color.DarkOliveGreen);
        header_row.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        header_row.Value = "SEASONAL DATA";
        rowIndex++;
        colIndex = 3;

        var row_header_s1 = ws.Cells[rowIndex, colIndex];
        ws.Row(rowIndex).Height = 22;
        row_header_s1.Value = "Samples Tested";
        row_header_s1.Style.Font.Bold = true;
        row_header_s1.Style.Font.Color.SetColor(Color.White);
        row_header_s1.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        row_header_s1.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        row_header_s1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        row_header_s1.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        row_header_s1.Style.Fill.BackgroundColor.SetColor(greenHeader);
        ws.Cells[rowIndex, colIndex, rowIndex, colIndex + 1].Merge = true;
        ws.Cells[rowIndex, colIndex, rowIndex, colIndex + 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

        colIndex += 3;

        //header row - Samples Positive
        var row_header_s2 = ws.Cells[rowIndex, colIndex];
        row_header_s2.Value = "Samples Positive";
        row_header_s2.Style.Font.Bold = true;
        row_header_s2.Style.Font.Color.SetColor(Color.White);
        row_header_s2.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        row_header_s2.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        row_header_s2.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        row_header_s2.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        row_header_s2.Style.Fill.BackgroundColor.SetColor(greenHeader);
        ws.Cells[rowIndex, colIndex, rowIndex, colIndex + 1].Merge = true;
        ws.Cells[rowIndex, colIndex, rowIndex, colIndex + 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

        colIndex += 3;

        //header row - Samples Positive
        var row_header_s3 = ws.Cells[rowIndex, colIndex];
        row_header_s3.Value = "Percentage Positive";
        row_header_s3.Style.Font.Bold = true;
        row_header_s3.Style.Font.Color.SetColor(Color.White);
        row_header_s3.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        row_header_s3.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        row_header_s3.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        row_header_s3.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        row_header_s3.Style.Fill.BackgroundColor.SetColor(greenHeader);
        ws.Cells[rowIndex, colIndex, rowIndex, colIndex + 1].Merge = true;
        ws.Cells[rowIndex, colIndex, rowIndex, colIndex + 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

        rowIndex++;

        colIndex = 1;

        //second row
        //colFromHex = System.Drawing.ColorTranslator.FromHtml("#c0504d");
        var code_row_s1 = ws.Cells[rowIndex, 1];
        ws.Row(rowIndex).Height = 22;
        code_row_s1.Value = "SAMPLE";
        code_row_s1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        code_row_s1.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        code_row_s1.Style.Fill.BackgroundColor.SetColor(silverHeader);
        code_row_s1.Style.Font.Color.SetColor(Color.White);

        var ingredient_row_s1 = ws.Cells[2, 2];
        ingredient_row_s1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        ingredient_row_s1.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        ingredient_row_s1.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        ingredient_row_s1.Style.Font.Color.SetColor(Color.White);
        ingredient_row_s1.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        ingredient_row_s1.Value = "YEAR";

        colIndex += 2;

        var winter_col1 = ws.Cells[rowIndex, colIndex];
        winter_col1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        winter_col1.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        winter_col1.Style.Font.Color.SetColor(Color.White);
        winter_col1.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        winter_col1.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        winter_col1.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        winter_col1.Value = "Winter";

        winter_col1 = ws.Cells[rowIndex, colIndex + 3];
        winter_col1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        winter_col1.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        winter_col1.Style.Font.Color.SetColor(Color.White);
        winter_col1.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        winter_col1.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        winter_col1.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        winter_col1.Value = "Winter";

        winter_col1 = ws.Cells[rowIndex, colIndex + 6];
        winter_col1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        winter_col1.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        winter_col1.Style.Font.Color.SetColor(Color.White);
        winter_col1.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        winter_col1.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        winter_col1.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        winter_col1.Value = "Winter";

        colIndex++;

        var winter_col2 = ws.Cells[rowIndex, colIndex];
        winter_col2.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        winter_col2.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        winter_col2.Style.Font.Color.SetColor(Color.White);
        winter_col2.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        winter_col2.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        winter_col2.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        winter_col2.Value = "Summer";

        winter_col2 = ws.Cells[rowIndex, colIndex + 3];
        winter_col2.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        winter_col2.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        winter_col2.Style.Font.Color.SetColor(Color.White);
        winter_col2.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        winter_col2.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        winter_col2.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        winter_col2.Value = "Summer";

        winter_col2 = ws.Cells[rowIndex, colIndex + 6];
        winter_col2.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        winter_col2.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        winter_col2.Style.Font.Color.SetColor(Color.White);
        winter_col2.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        winter_col2.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        winter_col2.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        winter_col2.Value = "Summer";

        colIndex = 1;
        rowIndex++;
        //loop thru FP and DS and RM

        //raw materials
        var rmData = seasonalData.Where(x => x.SampleType == "Raw Material").SelectMany(d => d.SamplesTestedPositive);

        Int32 _seasonalRowStart = rowIndex;

        for (int f = 0; f < totalYearsInReport; f++)
        {
          var season_rm = ws.Cells[rowIndex, colIndex];
          season_rm.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          season_rm.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          season_rm.Style.Fill.BackgroundColor.SetColor(blueHeader);
          season_rm.Style.Font.Color.SetColor(Color.White);
          season_rm.Value = "Raw Material";
          colIndex++;
          var season_rm2 = ws.Cells[rowIndex, colIndex];
          season_rm2.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          season_rm2.Value = startYear + f;

          colIndex++;
          var season_rm_winter_tested = ws.Cells[rowIndex, colIndex];
          season_rm_winter_tested.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          season_rm_winter_tested.Value = rmData.Where(x => x.Season == "Winter" && x.Year == startYear + f && x.SampleType == "Raw Material").Sum(s => s.TotalTested);
          if (season_rm_winter_tested.Value == "0") { season_rm_winter_tested.Value = ""; }
          season_rm_winter_tested.Style.Numberformat.Format = "#,#";

          colIndex++;
          var season_rm_summer_tested = ws.Cells[rowIndex, colIndex];
          season_rm_summer_tested.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          season_rm_summer_tested.Value = rmData.Where(x => x.Season == "Summer" && x.Year == startYear + f && x.SampleType == "Raw Material").Sum(s => s.TotalTested);
          if (season_rm_summer_tested.Value == "0") { season_rm_summer_tested.Value = ""; }
          season_rm_summer_tested.Style.Numberformat.Format = "#,#";

          colIndex += 2;
          var season_rm_winter_positive = ws.Cells[rowIndex, colIndex];
          season_rm_winter_positive.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          season_rm_winter_positive.Value = rmData.Where(x => x.Season == "Winter" && x.Year == startYear + f && x.SampleType == "Raw Material").Sum(s => s.TotalPositive);
          if (season_rm_winter_positive.Value == "0") { season_rm_winter_positive.Value = ""; }
          season_rm_winter_positive.Style.Numberformat.Format = "#,#";

          colIndex++;
          var season_rm_summer_positive = ws.Cells[rowIndex, colIndex];
          season_rm_summer_positive.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          season_rm_summer_positive.Value = rmData.Where(x => x.Season == "Summer" && x.Year == startYear + f && x.SampleType == "Raw Material").Sum(s => s.TotalPositive);
          if (season_rm_summer_positive.Value == "0") { season_rm_summer_positive.Value = ""; }
          season_rm_summer_positive.Style.Numberformat.Format = "#,#";

          colIndex += 2;
          var season_rm_winter_percentage = ws.Cells[rowIndex, colIndex];
          season_rm_winter_percentage.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          try
          {
            //js
            //season_rm_winter_percentage.Value = rmData.Where(x => x.Season == "Winter" && x.Year == startYear + f && x.SampleType == "Raw Material").Average(s => s.PercentagePositive);
            season_rm_winter_percentage.Formula =
              $"({ws.Cells[rowIndex, colIndex - 3].Address}/{ws.Cells[rowIndex, colIndex - 6].Address})*100";
            season_rm_winter_percentage.Style.Numberformat.Format = "#,#";
          }
          catch
          {
            season_rm_winter_percentage.Value = "";
          }

          colIndex++;
          var season_rm_summer_percentage = ws.Cells[rowIndex, colIndex];
          season_rm_summer_percentage.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          try
          {
            //season_rm_summer_percentage.Value = Decimal.Round(rmData.Where(x => x.Season == "Summer" && x.Year == startYear + f && x.SampleType == "Raw Material").Average(s => s.PercentagePositive), 0);
            season_rm_summer_percentage.Formula =
              $"({ws.Cells[rowIndex, colIndex - 3].Address}/{ws.Cells[rowIndex, colIndex - 6].Address})*100";
            season_rm_summer_percentage.Style.Numberformat.Format = "#,#";
          }
          catch
          {
            season_rm_summer_percentage.Value = "";
          }

          colIndex = 1;
          rowIndex++;
        }

        Int32 seasonalRowEnd = rowIndex;

        //Seasonal RM totals
        var rm_seasonal_total_header = ws.Cells[rowIndex, colIndex];
        rm_seasonal_total_header.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        rm_seasonal_total_header.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        rm_seasonal_total_header.Style.Fill.BackgroundColor.SetColor(Color.DarkOliveGreen);
        rm_seasonal_total_header.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
        rm_seasonal_total_header.Style.Font.Color.SetColor(Color.White);
        rm_seasonal_total_header.Value = "TOTAL";

        colIndex++;
        colIndex++;
        var rm_seasonal_total_tested_winter = ws.Cells[rowIndex, colIndex];
        rm_seasonal_total_tested_winter.Formula =
          $"=SUM({ws.Cells[rowIndex - totalYearsInReport, colIndex].Address}:{ws.Cells[rowIndex - 1, colIndex].Address})";
        rm_seasonal_total_tested_winter.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        rm_seasonal_total_tested_winter.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        rm_seasonal_total_tested_winter.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        rm_seasonal_total_tested_winter.Style.Numberformat.Format = "#,#";

        colIndex++;
        var rm_seasonal_total_tested_summer = ws.Cells[rowIndex, colIndex];
        rm_seasonal_total_tested_summer.Formula =
          $"=SUM({ws.Cells[rowIndex - totalYearsInReport, colIndex].Address}:{ws.Cells[rowIndex - 1, colIndex].Address})";
        rm_seasonal_total_tested_summer.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        rm_seasonal_total_tested_summer.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        rm_seasonal_total_tested_summer.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        rm_seasonal_total_tested_summer.Style.Numberformat.Format = "#,#";


        colIndex++;
        colIndex++;
        var rm_seasonal_total_positive_winter = ws.Cells[rowIndex, colIndex];
        rm_seasonal_total_positive_winter.Formula =
          $"=SUM({ws.Cells[rowIndex - totalYearsInReport, colIndex].Address}:{ws.Cells[rowIndex - 1, colIndex].Address})";
        rm_seasonal_total_positive_winter.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        rm_seasonal_total_positive_winter.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        rm_seasonal_total_positive_winter.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        rm_seasonal_total_positive_winter.Style.Numberformat.Format = "#,#";
        colIndex++;
        var rm_seasonal_total_positive_summer = ws.Cells[rowIndex, colIndex];
        rm_seasonal_total_positive_summer.Formula =
          $"=SUM({ws.Cells[rowIndex - totalYearsInReport, colIndex].Address}:{ws.Cells[rowIndex - 1, colIndex].Address})";
        rm_seasonal_total_positive_summer.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        rm_seasonal_total_positive_summer.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        rm_seasonal_total_positive_summer.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        rm_seasonal_total_positive_summer.Style.Numberformat.Format = "#,#";

        colIndex++;
        colIndex++;

        var rm_seasonal_total_percentage_winter = ws.Cells[rowIndex, colIndex];
        rm_seasonal_total_percentage_winter.Formula =
          $"({ws.Cells[rowIndex, colIndex - 3].Address}/{ws.Cells[rowIndex, colIndex - 6].Address})*100";
        rm_seasonal_total_percentage_winter.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        rm_seasonal_total_percentage_winter.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        rm_seasonal_total_percentage_winter.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        rm_seasonal_total_percentage_winter.Style.Numberformat.Format = "#,#";
        colIndex++;
        var rm_seasonal_total_percentage_summer = ws.Cells[rowIndex, colIndex];
        rm_seasonal_total_percentage_summer.Formula =
          $"({ws.Cells[rowIndex, colIndex - 3].Address}/{ws.Cells[rowIndex, colIndex - 6].Address})*100";
        rm_seasonal_total_percentage_summer.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        rm_seasonal_total_percentage_summer.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        rm_seasonal_total_percentage_summer.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        rm_seasonal_total_percentage_summer.Style.Numberformat.Format = "#,#";

        rowIndex++;
        colIndex = 1;
        rowIndex++;

        //Finished products
        var fpData = seasonalData.Where(x => x.SampleType == "Finished Product").SelectMany(d => d.SamplesTestedPositive);
        for (int f = 0; f < totalYearsInReport; f++)
        {
          var season1 = ws.Cells[rowIndex, colIndex];
          season1.Value = "Finished Product";
          season1.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          season1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          season1.Style.Fill.BackgroundColor.SetColor(blueHeader);
          season1.Style.Font.Color.SetColor(Color.White);

          colIndex++;
          var season2 = ws.Cells[rowIndex, colIndex];
          season2.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          season2.Value = startYear + f;

          colIndex++;
          var season_winter_tested = ws.Cells[rowIndex, colIndex];
          season_winter_tested.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          season_winter_tested.Value = fpData.Where(x => x.Season == "Winter" && x.Year == startYear + f && x.SampleType == "Finished Product").Sum(s => s.TotalTested);
          if (season_winter_tested.Value == "0") { season_winter_tested.Value = ""; }
          season_winter_tested.Style.Numberformat.Format = "#,#";

          colIndex++;
          var season_summer_tested = ws.Cells[rowIndex, colIndex];
          season_summer_tested.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          season_summer_tested.Value = fpData.Where(x => x.Season == "Summer" && x.Year == startYear + f && x.SampleType == "Finished Product").Sum(s => s.TotalTested);
          if (season_summer_tested.Value == "0") { season_summer_tested.Value = ""; }
          season_summer_tested.Style.Numberformat.Format = "#,#";

          colIndex += 2;
          var season_winter_positive = ws.Cells[rowIndex, colIndex];
          season_winter_positive.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          season_winter_positive.Value = fpData.Where(x => x.Season == "Winter" && x.Year == startYear + f && x.SampleType == "Finished Product").Sum(s => s.TotalPositive);
          if (season_winter_positive.Value == "0") { season_winter_positive.Value = ""; }
          season_winter_positive.Style.Numberformat.Format = "#,#";

          colIndex++;
          var season_summer_positive = ws.Cells[rowIndex, colIndex];
          season_summer_positive.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          season_summer_positive.Value = fpData.Where(x => x.Season == "Summer" && x.Year == startYear + f && x.SampleType == "Finished Product").Sum(s => s.TotalPositive);
          if (season_summer_positive.Value == "0") { season_summer_positive.Value = ""; }
          season_summer_positive.Style.Numberformat.Format = "#,#";

          colIndex += 2;
          var season_winter_percentage = ws.Cells[rowIndex, colIndex];
          season_winter_percentage.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          try
          {
            //season_winter_percentage.Value = fpData.Where(x => x.Season == "Winter" && x.Year == startYear + f && x.SampleType == "Finished Product").Average(s => s.PercentagePositive);
            season_winter_percentage.Formula =
              $"({ws.Cells[rowIndex, colIndex - 3].Address}/{ws.Cells[rowIndex, colIndex - 6].Address})*100";
            season_winter_percentage.Style.Numberformat.Format = "#,#";
          }
          catch
          {
            season_winter_percentage.Value = "";
          }

          colIndex++;

          var season_summer_percentage = ws.Cells[rowIndex, colIndex];
          season_summer_percentage.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          try
          {
            //season_summer_percentage.Value = Decimal.Round(fpData.Where(x => x.Season == "Summer" && x.Year == startYear + f && x.SampleType == "Finished Product").Average(s => s.PercentagePositive), 0);
            season_summer_percentage.Formula =
              $"({ws.Cells[rowIndex, colIndex - 3].Address}/{ws.Cells[rowIndex, colIndex - 6].Address})*100";
            season_summer_percentage.Style.Numberformat.Format = "#,#";
          }
          catch
          {
            season_summer_percentage.Value = "";
          }

          colIndex = 1;
          rowIndex++;
        }

        //Seasonal FP totals
        var fp_seasonal_total_header = ws.Cells[rowIndex, colIndex];
        fp_seasonal_total_header.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        fp_seasonal_total_header.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        fp_seasonal_total_header.Style.Fill.BackgroundColor.SetColor(Color.DarkOliveGreen);
        fp_seasonal_total_header.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
        fp_seasonal_total_header.Style.Font.Color.SetColor(Color.White);
        fp_seasonal_total_header.Value = "TOTAL";

        colIndex++;
        colIndex++;
        var fp_seasonal_total_tested_winter = ws.Cells[rowIndex, colIndex];
        fp_seasonal_total_tested_winter.Formula =
          $"=SUM({ws.Cells[rowIndex - totalYearsInReport, colIndex].Address}:{ws.Cells[rowIndex - 1, colIndex].Address})";
        fp_seasonal_total_tested_winter.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        fp_seasonal_total_tested_winter.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        fp_seasonal_total_tested_winter.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        fp_seasonal_total_tested_winter.Style.Numberformat.Format = "#,#";

        colIndex++;
        var fp_seasonal_total_tested_summer = ws.Cells[rowIndex, colIndex];
        fp_seasonal_total_tested_summer.Formula =
          $"=SUM({ws.Cells[rowIndex - totalYearsInReport, colIndex].Address}:{ws.Cells[rowIndex - 1, colIndex].Address})";
        fp_seasonal_total_tested_summer.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        fp_seasonal_total_tested_summer.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        fp_seasonal_total_tested_summer.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        fp_seasonal_total_tested_summer.Style.Numberformat.Format = "#,#";


        colIndex++;
        colIndex++;
        var fp_seasonal_total_positive_winter = ws.Cells[rowIndex, colIndex];
        fp_seasonal_total_positive_winter.Formula =
          $"=SUM({ws.Cells[rowIndex - totalYearsInReport, colIndex].Address}:{ws.Cells[rowIndex - 1, colIndex].Address})";
        fp_seasonal_total_positive_winter.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        fp_seasonal_total_positive_winter.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        fp_seasonal_total_positive_winter.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        fp_seasonal_total_positive_winter.Style.Numberformat.Format = "#,#";
        colIndex++;
        var fp_seasonal_total_positive_summer = ws.Cells[rowIndex, colIndex];
        fp_seasonal_total_positive_summer.Formula =
          $"=SUM({ws.Cells[rowIndex - totalYearsInReport, colIndex].Address}:{ws.Cells[rowIndex - 1, colIndex].Address})";
        fp_seasonal_total_positive_summer.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        fp_seasonal_total_positive_summer.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        fp_seasonal_total_positive_summer.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        fp_seasonal_total_positive_summer.Style.Numberformat.Format = "#,#";

        colIndex++;
        colIndex++;

        var fp_seasonal_total_percentage_winter = ws.Cells[rowIndex, colIndex];
        //fp_seasonal_total_percentage_winter.Formula = String.Format("=SUM({0}:{1})/{2}", ws.Cells[rowIndex - totalYearsInReport, colIndex].Address, ws.Cells[rowIndex - 1, colIndex].Address, totalYearsInReport);
        fp_seasonal_total_percentage_winter.Formula =
          $"({ws.Cells[rowIndex, colIndex - 3].Address}/{ws.Cells[rowIndex, colIndex - 6].Address})*100";
        fp_seasonal_total_percentage_winter.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        fp_seasonal_total_percentage_winter.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        fp_seasonal_total_percentage_winter.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        fp_seasonal_total_percentage_winter.Style.Numberformat.Format = "#,#";
        colIndex++;

        var fp_seasonal_total_percentage_summer = ws.Cells[rowIndex, colIndex];
        fp_seasonal_total_percentage_summer.Formula =
          $"({ws.Cells[rowIndex, colIndex - 3].Address}/{ws.Cells[rowIndex, colIndex - 6].Address})*100";
        //fp_seasonal_total_percentage_summer.Formula = String.Format("=SUM({0}:{1})/{2}", ws.Cells[rowIndex - totalYearsInReport, colIndex].Address, ws.Cells[rowIndex - 1, colIndex].Address, totalYearsInReport);
        fp_seasonal_total_percentage_summer.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        fp_seasonal_total_percentage_summer.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        fp_seasonal_total_percentage_summer.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        fp_seasonal_total_percentage_summer.Style.Numberformat.Format = "#,#";

        rowIndex++;
        colIndex = 1;
        rowIndex++;

        //dust swabs
        var dsData = seasonalData.Where(x => x.SampleType == "Dust Swab").SelectMany(d => d.SamplesTestedPositive);
        for (int f = 0; f < totalYearsInReport; f++)
        {
          var season1 = ws.Cells[rowIndex, colIndex];
          season1.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          season1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          season1.Style.Fill.BackgroundColor.SetColor(blueHeader);
          season1.Style.Font.Color.SetColor(Color.White);
          season1.Value = "Dust Swab";
          colIndex++;
          var season2 = ws.Cells[rowIndex, colIndex];
          season2.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          season2.Value = startYear + f;

          colIndex++;
          var season_winter_tested = ws.Cells[rowIndex, colIndex];
          season_winter_tested.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          season_winter_tested.Value = dsData.Where(x => x.Season == "Winter" && x.Year == startYear + f && x.SampleType == "Dust Swab").Sum(s => s.TotalTested);
          if (season_winter_tested.Value == "0") { season_winter_tested.Value = ""; }
          season_winter_tested.Style.Numberformat.Format = "#,#";

          colIndex++;
          var season_summer_tested = ws.Cells[rowIndex, colIndex];
          season_summer_tested.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          season_summer_tested.Value = dsData.Where(x => x.Season == "Summer" && x.Year == startYear + f && x.SampleType == "Dust Swab").Sum(s => s.TotalTested);
          if (season_summer_tested.Value == "0") { season_summer_tested.Value = ""; }
          season_summer_tested.Style.Numberformat.Format = "#,#";

          colIndex += 2;
          var season_winter_positive = ws.Cells[rowIndex, colIndex];
          season_winter_positive.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          season_winter_positive.Value = dsData.Where(x => x.Season == "Winter" && x.Year == startYear + f && x.SampleType == "Dust Swab").Sum(s => s.TotalPositive);
          if (season_winter_positive.Value == "0") { season_winter_positive.Value = ""; }
          season_winter_positive.Style.Numberformat.Format = "#,#";

          colIndex++;
          var season_summer_positive = ws.Cells[rowIndex, colIndex];
          season_summer_positive.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          season_summer_positive.Value = dsData.Where(x => x.Season == "Summer" && x.Year == startYear + f && x.SampleType == "Dust Swab").Sum(s => s.TotalPositive);
          if (season_summer_positive.Value == "0") { season_summer_positive.Value = ""; }
          season_summer_positive.Style.Numberformat.Format = "#,#";

          colIndex += 2;
          var season_winter_percentage = ws.Cells[rowIndex, colIndex];
          season_winter_percentage.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          try
          {
            //season_winter_percentage.Value = Decimal.Round(dsData.Where(x => x.Season == "Winter" && x.Year == startYear + f && x.SampleType == "Dust Swab").Average(s => s.PercentagePositive), 0);
            season_winter_percentage.Formula =
              $"({ws.Cells[rowIndex, colIndex - 3].Address}/{ws.Cells[rowIndex, colIndex - 6].Address})*100";
            season_winter_percentage.Style.Numberformat.Format = "#,#";
          }
          catch
          {
            season_winter_percentage.Value = "";
          }

          colIndex++;
          var season_summer_percentage = ws.Cells[rowIndex, colIndex];
          season_summer_percentage.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          try
          {
            //season_summer_percentage.Value = Decimal.Round(dsData.Where(x => x.Season == "Summer" && x.Year == startYear + f && x.SampleType == "Dust Swab").Average(s => s.PercentagePositive), 0);
            season_summer_percentage.Formula =
              $"({ws.Cells[rowIndex, colIndex - 3].Address}/{ws.Cells[rowIndex, colIndex - 6].Address})*100";
            season_summer_percentage.Style.Numberformat.Format = "#,#";
          }
          catch
          {
            season_summer_percentage.Value = "";
          }

          colIndex = 1;
          rowIndex++;
        }

        //Seasonal DS totals
        var ds_seasonal_total_header = ws.Cells[rowIndex, colIndex];
        ds_seasonal_total_header.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        ds_seasonal_total_header.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        ds_seasonal_total_header.Style.Fill.BackgroundColor.SetColor(Color.DarkOliveGreen);
        ds_seasonal_total_header.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
        ds_seasonal_total_header.Style.Font.Color.SetColor(Color.White);
        ds_seasonal_total_header.Value = "TOTAL";

        colIndex++;
        colIndex++;
        var ds_seasonal_total_tested_winter = ws.Cells[rowIndex, colIndex];
        ds_seasonal_total_tested_winter.Formula =
          $"=SUM({ws.Cells[rowIndex - totalYearsInReport, colIndex].Address}:{ws.Cells[rowIndex - 1, colIndex].Address})";
        ds_seasonal_total_tested_winter.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        ds_seasonal_total_tested_winter.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        ds_seasonal_total_tested_winter.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        ds_seasonal_total_tested_winter.Style.Numberformat.Format = "#,#";

        colIndex++;
        var ds_seasonal_total_tested_summer = ws.Cells[rowIndex, colIndex];
        ds_seasonal_total_tested_summer.Formula =
          $"=SUM({ws.Cells[rowIndex - totalYearsInReport, colIndex].Address}:{ws.Cells[rowIndex - 1, colIndex].Address})";
        ds_seasonal_total_tested_summer.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        ds_seasonal_total_tested_summer.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        ds_seasonal_total_tested_summer.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        ds_seasonal_total_tested_summer.Style.Numberformat.Format = "#,#";


        colIndex++;
        colIndex++;
        var ds_seasonal_total_positive_winter = ws.Cells[rowIndex, colIndex];
        ds_seasonal_total_positive_winter.Formula =
          $"=SUM({ws.Cells[rowIndex - totalYearsInReport, colIndex].Address}:{ws.Cells[rowIndex - 1, colIndex].Address})";
        ds_seasonal_total_positive_winter.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        ds_seasonal_total_positive_winter.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        ds_seasonal_total_positive_winter.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        ds_seasonal_total_positive_winter.Style.Numberformat.Format = "#,#";
        colIndex++;
        var ds_seasonal_total_positive_summer = ws.Cells[rowIndex, colIndex];
        ds_seasonal_total_positive_summer.Formula =
          $"=SUM({ws.Cells[rowIndex - totalYearsInReport, colIndex].Address}:{ws.Cells[rowIndex - 1, colIndex].Address})";
        ds_seasonal_total_positive_summer.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        ds_seasonal_total_positive_summer.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        ds_seasonal_total_positive_summer.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        ds_seasonal_total_positive_summer.Style.Numberformat.Format = "#,#";

        colIndex++;
        colIndex++;

        var ds_seasonal_total_percentage_winter = ws.Cells[rowIndex, colIndex];
        //ds_seasonal_total_percentage_winter.Formula = String.Format("=SUM({0}:{1})/{2}", ws.Cells[rowIndex - totalYearsInReport, colIndex].Address, ws.Cells[rowIndex - 1, colIndex].Address, totalYearsInReport);
        ds_seasonal_total_percentage_winter.Formula =
          $"({ws.Cells[rowIndex, colIndex - 3].Address}/{ws.Cells[rowIndex, colIndex - 6].Address})*100";
        ds_seasonal_total_percentage_winter.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        ds_seasonal_total_percentage_winter.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        ds_seasonal_total_percentage_winter.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        ds_seasonal_total_percentage_winter.Style.Numberformat.Format = "#,#";
        colIndex++;
        var ds_seasonal_total_percentage_summer = ws.Cells[rowIndex, colIndex];
        //ds_seasonal_total_percentage_summer.Formula = String.Format("=SUM({0}:{1})/{2}", ws.Cells[rowIndex - totalYearsInReport, colIndex].Address, ws.Cells[rowIndex - 1, colIndex].Address, totalYearsInReport);
        ds_seasonal_total_percentage_summer.Formula =
          $"({ws.Cells[rowIndex, colIndex - 3].Address}/{ws.Cells[rowIndex, colIndex - 6].Address})*100";
        ds_seasonal_total_percentage_summer.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        ds_seasonal_total_percentage_summer.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        ds_seasonal_total_percentage_summer.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        ds_seasonal_total_percentage_summer.Style.Numberformat.Format = "#,#";

        rowIndex++;
        rowIndex++;
        colIndex = 1;

        var seasonal_data_total_header = ws.Cells[rowIndex, colIndex];
        seasonal_data_total_header.Value = "TOTAL SAMPLES";
        seasonal_data_total_header.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        seasonal_data_total_header.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        seasonal_data_total_header.Style.Fill.BackgroundColor.SetColor(Color.DarkOliveGreen);
        seasonal_data_total_header.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
        seasonal_data_total_header.Style.Font.Color.SetColor(Color.White);

        colIndex++;
        colIndex++;

        var total_samples_tested_winter = ws.Cells[rowIndex, colIndex];
        total_samples_tested_winter.Formula =
          $"={rm_seasonal_total_tested_winter.Address}+{fp_seasonal_total_tested_winter.Address}+{ds_seasonal_total_tested_winter.Address}";
        total_samples_tested_winter.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        total_samples_tested_winter.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        total_samples_tested_winter.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        total_samples_tested_winter.Style.Numberformat.Format = "#,#";

        colIndex++;
        var total_samples_tested_summer = ws.Cells[rowIndex, colIndex];
        total_samples_tested_summer.Formula =
          $"={rm_seasonal_total_tested_summer.Address}+{fp_seasonal_total_tested_summer.Address}+{ds_seasonal_total_tested_summer.Address}";
        total_samples_tested_summer.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        total_samples_tested_summer.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        total_samples_tested_summer.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        total_samples_tested_summer.Style.Numberformat.Format = "#,#";

        colIndex++;
        colIndex++;

        var total_samples_positive_winter = ws.Cells[rowIndex, colIndex];
        total_samples_positive_winter.Formula =
          $"={rm_seasonal_total_positive_winter.Address}+{fp_seasonal_total_positive_winter.Address}+{ds_seasonal_total_positive_winter.Address}";
        total_samples_positive_winter.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        total_samples_positive_winter.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        total_samples_positive_winter.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        total_samples_positive_winter.Style.Numberformat.Format = "#,#";

        colIndex++;
        var total_samples_positive_summer = ws.Cells[rowIndex, colIndex];
        total_samples_positive_summer.Formula =
          $"={rm_seasonal_total_positive_summer.Address}+{fp_seasonal_total_positive_summer.Address}+{ds_seasonal_total_positive_summer.Address}";
        total_samples_positive_summer.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        total_samples_positive_summer.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        total_samples_positive_summer.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        total_samples_positive_summer.Style.Numberformat.Format = "#,#";

        colIndex++;
        colIndex++;

        var total_samples_percentage_winter = ws.Cells[rowIndex, colIndex];
        //total_samples_percentage_winter.Formula = String.Format("=({0}+{1}+{2})/3", rm_seasonal_total_percentage_winter.Address, fp_seasonal_total_percentage_winter.Address, ds_seasonal_total_percentage_winter.Address);
        total_samples_percentage_winter.Formula =
          $"({ws.Cells[rowIndex, colIndex - 3].Address}/{ws.Cells[rowIndex, colIndex - 6].Address})*100";
        total_samples_percentage_winter.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        total_samples_percentage_winter.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        total_samples_percentage_winter.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        total_samples_percentage_winter.Style.Numberformat.Format = "#,#";

        colIndex++;
        var total_samples_percentage_summer = ws.Cells[rowIndex, colIndex];
        //total_samples_percentage_summer.Formula = String.Format("=({0}+{1}+{2})/3", rm_seasonal_total_percentage_summer.Address, fp_seasonal_total_percentage_summer.Address, ds_seasonal_total_percentage_summer.Address);
        total_samples_percentage_summer.Formula =
          $"({ws.Cells[rowIndex, colIndex - 3].Address}/{ws.Cells[rowIndex, colIndex - 6].Address})*100";
        total_samples_percentage_summer.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        total_samples_percentage_summer.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        total_samples_percentage_summer.Style.Fill.BackgroundColor.SetColor(Color.WhiteSmoke);
        total_samples_percentage_summer.Style.Numberformat.Format = "#,#";

        #endregion

        #endregion

        return p.GetAsByteArray();
      }
    }

    #endregion

    #endregion

    #region Branch Summary Reports

    public async Task<BranchSummaryReportViewModel> GetReportBranchSummaryReport(ModuleEnum module, int year)
    {
      BranchSummaryReportViewModel report = new BranchSummaryReportViewModel
      {
        Module = module.ToString(),
        ReportName = "Branch Summary Report",
        Year = year,
      };

      var branches = await _applicationLayer.GetActiveMemberBranchesSummaryByModule((int)module).ConfigureAwait(false);

      if (branches != null)
      {
        foreach (var branch in branches)
        {
          MemberBranchViewModel memberBranch = new()
          {
            BranchId = branch.Id,
            BranchName = branch.Name,
            MemberName = branch.Member,
          };

          var salmonellaReports = await GetSalmonellaReportsLookup(year, branch.Id).ConfigureAwait(false);

          if (salmonellaReports != null)
          {
            for (int i = 1; i < 13; i++)
            {
              if (salmonellaReports.Any(x => x.ReportMonth == i))
              {
                var currentMonth = salmonellaReports.Single(x => x.ReportMonth == i);
                memberBranch.Reports.Add(new ReportViewModel
                {
                  ReportExists = true,
                  ReportId = currentMonth.Id,
                  ReportMonth = (int)currentMonth.ReportMonth!,
                  ReportYear = (int)currentMonth.ReportYear!,
                  Submitted = currentMonth.IsSubmitted,
                  SubmittedDate = currentMonth.SubmittedDate,
                });
              }
              else
              {
                memberBranch.Reports.Add(new ReportViewModel
                {
                  ReportExists = false,
                  ReportId = 0,
                  ReportMonth = i,
                  ReportYear = year,
                  Submitted = false,
                  SubmittedDate = null,
                });
              }
            }
          }

          report.MemberBranches.Add(memberBranch);
        }
      }

      return report;
    }

    #endregion

  }
}
