namespace AFMA.Stats.Client.Controllers
{
  using AFMA.ApplicationLayer;
  using AFMA.ApplicationLayer.Attributes;
  using AFMA.ApplicationLayer.Helpers;
  using AFMA.ApplicationLayer.Interface;
  using AFMA.ApplicationLayer.Model;
  using AFMA.ApplicationLayer.ViewModels;
  using AFMA.Infrastructure.Settings;
  using AFMA.Shared.Services.Maintenance;
  using Alpha.Services.Models.Data.PageData;
  using AspNetCoreHero.ToastNotification.Abstractions;
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Http;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.AspNetCore.Mvc.Filters;
  using Microsoft.Extensions.Options;

  [Authorize(Roles = "User")]
  [TypeFilter(typeof(ModulePermissionAttribute), Arguments = new object[] { ModuleEnum.Salmonella })]
  [ServiceFilter(typeof(SalmonellaMaintenanceFilter))]
  public class SalmonellaController : Controller
  {
    private readonly ILogger<SalmonellaController> _logger;
    private readonly IApplicationLayer _applicationLayer;
    private readonly ISalmonellaLayer _salmonellaLayer;
    private readonly IOptions<ApplicationSettings> _applicationSettings;
    private readonly INotyfService _notyf;
    private MemberAccessViewModel memberAccessViewModel;

    public SalmonellaController(
      ILogger<SalmonellaController> logger,
      IApplicationLayer applicationLayer,
      ISalmonellaLayer salmonellalayer,
      IOptions<ApplicationSettings> applicationSettings,
      INotyfService notyfService)
    {
      _applicationLayer = applicationLayer;
      _logger = logger;
      _salmonellaLayer = salmonellalayer;
      _notyf = notyfService;
      _applicationSettings = applicationSettings;
    }

    // Populate member access asynchronously per-request (avoids blocking a thread-pool thread
    // with .Result in the constructor, which starves the pool under concurrency).
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
      memberAccessViewModel = await _applicationLayer.GetCurrentMemberAccess().ConfigureAwait(false);
      await next().ConfigureAwait(false);
    }

    public async Task<IActionResult> Guideline()
    {
      return View();
    }

    /// <summary>
    /// Salmonella-module maintenance page. Shown by <see cref="SalmonellaMaintenanceFilter"/>
    /// while <c>SalmonellaMaintenanceSettings:Enabled</c> is true; visitors with the access code
    /// receive a bypass cookie and can continue to the module.
    /// (Recovered from the production build of 2026-07-21; this code was never committed.)
    /// </summary>
    [HttpGet]
    public IActionResult Maintenance([FromServices] IOptionsMonitor<SalmonellaMaintenanceSettings> maintenanceSettings)
    {
      var settings = maintenanceSettings.CurrentValue;
      ViewBag.MaintenanceHeading = settings.Heading;
      ViewBag.MaintenanceMessage = settings.Message;
      ViewBag.ShowBypassForm = !string.IsNullOrEmpty(settings.BypassToken);
      return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Maintenance([FromServices] IOptionsMonitor<SalmonellaMaintenanceSettings> maintenanceSettings, string accessCode)
    {
      var settings = maintenanceSettings.CurrentValue;

      if (!string.IsNullOrEmpty(settings.BypassToken) && string.Equals(accessCode, settings.BypassToken, StringComparison.Ordinal))
      {
        Response.Cookies.Append(settings.BypassCookieName, settings.BypassToken, new CookieOptions
        {
          HttpOnly = true,
          Secure = Request.IsHttps,
          SameSite = SameSiteMode.Lax,
          Expires = settings.BypassCookieDays > 0 ? DateTimeOffset.UtcNow.AddDays(settings.BypassCookieDays) : null,
        });

        return RedirectToAction("Index");
      }

      ViewBag.MaintenanceHeading = settings.Heading;
      ViewBag.MaintenanceMessage = settings.Message;
      ViewBag.ShowBypassForm = !string.IsNullOrEmpty(settings.BypassToken);
      ViewBag.InvalidCode = true;
      return View();
    }

    public async Task<IActionResult> Index(int? page, bool? filterIsSubmitted)
    {
      var pageNo = page ?? 1;
      var result = await _salmonellaLayer.GetSalmonellaReportsLookup(
        new AlphaPageRequest(pageNo, _applicationSettings.Value.PageSize, "DueDate", true),
        memberAccessViewModel.MemberId, memberAccessViewModel.MemberBranchId, filterIsSubmitted);

      return View(result);
    }

    public async Task<IActionResult> Details(int id)
    {
      try
      {
        var result = await _salmonellaLayer.GetSalmonellaReport(id, memberAccessViewModel.MemberBranchId);
        return View(result);
      }
      catch (Exception ex)
      {
        return RedirectToAction("ReportNotFound", "Account");
      }
    }

    public async Task<IActionResult> Edit(int id)
    {
      try
      {
        var result = await _salmonellaLayer.GetSalmonellaReport(id, memberAccessViewModel.MemberBranchId);
        return View(result);
      }
      catch (Exception ex)
      {
        return RedirectToAction("ReportNotFound", "Account");
      }
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

      return RedirectToAction("Manage", new { id = salmonellaReportModel.Id });
    }

    public async Task<IActionResult> Manage(int id)
    {
      try
      {
        var result = await _salmonellaLayer.GetSalmonellaReport(id, memberAccessViewModel.MemberBranchId);
        return View(result);
      }
      catch (Exception ex)
      {
        return RedirectToAction("ReportNotFound", "Account");
      }
    }

    public async Task<IActionResult> CreateSample(int salmonellaReportId)
    {
      try
      {
        var currentUser = await _applicationLayer.GetCurrentMemberAccess();
        var reportLookup =
          await _salmonellaLayer.GetSalmonellaReportLookup(salmonellaReportId, currentUser.MemberBranchId);
        var result = new SalmonellaSampleModel
        {
          SalmonellaReportId = salmonellaReportId,
          IsSubmitted = reportLookup.IsSubmitted,
          Member = reportLookup.Member,
          MemberBranch = reportLookup.MemberBranch,
        };

        return View(result);
      }
      catch
      {
        return RedirectToAction("ReportNotFound", "Account");
      }
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

      return RedirectToAction("Manage", new { id = salmonellaSampleModel.SalmonellaReportId });
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

      return RedirectToAction("Manage", new { id = salmonellaReportId });
    }

    public async Task<IActionResult> Report()
    {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> SalmonellaDetailReport(ReportCriteriaModel reportCriteria)
    {
      reportCriteria.MemberId = memberAccessViewModel.MemberId;
      reportCriteria.MemberBranchId = memberAccessViewModel.MemberBranchId;
      if (!await Infrastructure.ReportThrottle.WaitAsync())
      {
        _notyf.Warning("The system is busy generating reports. Please try again in a moment.");
        return RedirectToAction(nameof(Report));
      }
      try
      {
        var report = await _salmonellaLayer.GenerateSalmonellaClientExcelReport(reportCriteria).ConfigureAwait(false);
        return File(report, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DetailSalmonella.xlsx");
      }
      catch (Exception ex)
      {
        _logger.LogWarning(ex, "Salmonella report generation failed for the selected criteria.");
        _notyf.Warning("No data found for the selected period, or the report could not be generated.");
        return RedirectToAction(nameof(Report));
      }
      finally
      {
        Infrastructure.ReportThrottle.Release();
      }
    }
  }
}
