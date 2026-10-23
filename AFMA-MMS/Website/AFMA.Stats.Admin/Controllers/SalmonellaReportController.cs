namespace AFMA.Stats.Admin.Controllers
{
  using Alpha.Services.Models.Data.PageData;
  using ApplicationLayer.Helpers;
  using ApplicationLayer.Interface;
  using ApplicationLayer.Model;
  using AspNetCoreHero.ToastNotification.Abstractions;
  using Infrastructure.Settings;
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.Extensions.Options;

  [Authorize(Roles = "Admin")]
  public class SalmonellaReportController : Controller
  {
    private readonly ILogger<SalmonellaReportController> _logger;
    private readonly IApplicationLayer _applicationLayer;
    private readonly ISalmonellaLayer _salmonellaLayer;
    private readonly INotyfService _notyf;
    private readonly INotificationService _notificationService;
    private readonly IOptions<ApplicationSettings> _applicationSettings;

    public SalmonellaReportController(
      ILogger<SalmonellaReportController> logger,
      IApplicationLayer applicationLayer,
      ISalmonellaLayer salmonellaLayer,
      INotyfService notyf,
      IOptions<ApplicationSettings> applicationSettings,
      INotificationService notificationService)
    {
      _logger = logger;
      _applicationLayer = applicationLayer;
      _salmonellaLayer = salmonellaLayer;
      _notyf = notyf;
      _notificationService = notificationService;
      _applicationSettings = applicationSettings;
    }

    public async Task<IActionResult> Dashboard()
    {
      return View();
    }

    public async Task<IActionResult> Guideline()
    {
      return View();
    }

    public async Task<IActionResult> GenerateDashboard()
    {
      var results = await _applicationLayer.GenerateSalmonellaDashboard().ConfigureAwait(false);
      return Json(results);
    }

    public async Task<IActionResult> Index(int? page, int? filterMemberId, int? filterMemberBranchId, bool? filterIsSubmitted)
    {
      var pageNo = page ?? 1;
      var results = await _salmonellaLayer.GetSalmonellaReportsLookup(
        new AlphaPageRequest(pageNo, _applicationSettings.Value.PageSize, "Id", true), filterMemberId,
        filterMemberBranchId, filterIsSubmitted);
      return View(results);
    }

    public async Task<IActionResult> Details(int id)
    {
      var result = await _salmonellaLayer.GetSalmonellaReport(id);
      return View(result);
    }

    public async Task<IActionResult> CreateSample(int salmonellaReportId)
    {
      var result = new SalmonellaSampleModel { SalmonellaReportId = salmonellaReportId };
      return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateSample(SalmonellaSampleModel salmonellaSampleModel)
    {
      var result = await _applicationLayer.SaveOrUpdate(salmonellaSampleModel);

      if (result.Success)
      {
        _notyf.Success("Salmonella Sample created successfully");
      }
      else
      {
        _notyf.Error($"Salmonella Sample not created: {result.ErrorMessage}");
      }

      return RedirectToAction("Details", new { id = salmonellaSampleModel.SalmonellaReportId });
    }

    public async Task<IActionResult> DeleteSample(int salmonellaSampleId, int salmonellaReportId)
    {
      var result = await _salmonellaLayer.DeleteSalmonellaSample(salmonellaSampleId);

      if (result)
      {
        _notyf.Success("Salmonella Sample deleted successfully");
      }
      else
      {
        _notyf.Error("Salmonella Sample could not be deleted");
      }

      return RedirectToAction("Details", new { id = salmonellaReportId });
    }

    public async Task<IActionResult> Edit(int id)
    {
      var result = await _salmonellaLayer.GetSalmonellaReport(id);
      return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(SalmonellaReportModel salmonellaReportModel)
    {
      var result = await _applicationLayer.SaveOrUpdate(salmonellaReportModel);

      if (result.Success)
      {
        _notyf.Success("Salmonella Report updated successfully");
      }
      else
      {
        _notyf.Error($"Salmonella Report not updated: {result.ErrorMessage}");
      }

      return RedirectToAction("Details", new { id = salmonellaReportModel.Id });
    }

    public async Task<IActionResult> Submit(int salmonellaReportId)
    {
      var result = await _salmonellaLayer.UpdateSalmonellaReportStatus(salmonellaReportId, true);

      if (result.Success)
      {
        _notyf.Success("Salmonella Report submitted successfully");
      }
      else
      {
        _notyf.Error($"Salmonella Report not submitted: {result.ErrorMessage}");
      }

      return RedirectToAction("Details", new { id = salmonellaReportId });
    }

    public async Task<IActionResult> Unsubmit(int salmonellaReportId)
    {
      var result = await _salmonellaLayer.UpdateSalmonellaReportStatus(salmonellaReportId, false);

      if (result.Success)
      {
        _notyf.Success("Salmonella Report unsubmitted successfully");
      }
      else
      {
        _notyf.Error($"Salmonella Report not unsubmitted: {result.ErrorMessage}");
      }

      return RedirectToAction("Details", new { id = salmonellaReportId });
    }

    public async Task<IActionResult> DeleteSalmonellaReport(int salmonellaReportId)
    {
      var result = await _salmonellaLayer.DeleteSalmonellaReport(salmonellaReportId);

      if (result.Succeeded)
      {
        _notyf.Success("Salmonella Report deleted successfully");
      }
      else
      {
        _notyf.Error($"Salmonella Report could not be deleted : {result.ExceptionMessage}");
      }

      return RedirectToAction("Index", "SalmonellaReport");
    }

    public async Task<IActionResult> Submissions(int? filterYear, int? filterMonth)
    {
      var result =
        await _applicationLayer.GetSalmonellaSubmissions(new DateTime(filterYear ?? DateTime.Now.Year, filterMonth ?? DateTime.Now.Month, 1));
      return View(result);
    }

    public async Task<IActionResult> SendReminder(int id, int reportMonth, int reportYear)
    {
      var result = await _notificationService.SendSalmonellaSubmissionReminder(id).ConfigureAwait(false);

      if (result)
      {
        _notyf.Success("Salmonella submission reminder sent successfully");
      }
      else
      {
        _notyf.Error("Salmonella submission reminder could not be sent");
      }

      return RedirectToAction("Submissions", "SalmonellaReport",
        new { filterYear = reportYear, filterMonth = reportMonth });
    }

    [HttpPost]
    public async Task<IActionResult> Submissions(DateTime? period)
    {
      var result = await _applicationLayer.GetSalmonellaSubmissions(period ?? new DateTime(2022, 1, 1));
      return View(result);
    }

    public async Task<IActionResult> DetailSalmonellaAdminReport()
    {
      return View();
    }

    public async Task<IActionResult> TotalSalmonellaAdminReport()
    {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> TotalSalmonellaAdminReport(ReportCriteriaModel reportCriteria)
    {
      var report = await _salmonellaLayer.GenerateTotalSalmonellaExcelReport(reportCriteria).ConfigureAwait(false);
      return File(report, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TotalSalmonella.xlsx");
    }

    [HttpPost]
    public async Task<IActionResult> DetailSalmonellaAdminReport(ReportCriteriaModel reportCriteria)
    {
      var report = await _salmonellaLayer.GenerateDetailSalmonellaExcelReport(reportCriteria).ConfigureAwait(false);
      return File(report, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DetailSalmonella.xlsx");
    }

    public async Task<IActionResult> BranchReportSummary()
    {
      var result = await _salmonellaLayer.GetReportBranchSummaryReport(ModuleEnum.Salmonella, DateTime.Now.Year);
      return View(result);
    }
  }
}
