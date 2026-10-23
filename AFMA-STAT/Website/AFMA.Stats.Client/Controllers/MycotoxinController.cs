namespace AFMA.Stats.Client.Controllers
{
  using AFMA.ApplicationLayer;
  using AFMA.ApplicationLayer.Attributes;
  using AFMA.ApplicationLayer.Helpers;
  using AFMA.ApplicationLayer.Interface;
  using AFMA.ApplicationLayer.Model;
  using AFMA.ApplicationLayer.ViewModels;
  using AFMA.Infrastructure.Settings;
  using Alpha.Services.Models.Data.PageData;
  using AspNetCoreHero.ToastNotification.Abstractions;
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.AspNetCore.Mvc.Filters;
  using Microsoft.Extensions.Options;

  [Authorize(Roles = "User")]
  [TypeFilter(typeof(ModulePermissionAttribute), Arguments = new object[] { ModuleEnum.Mycotoxin })]
  public class MycotoxinController : Controller
  {
    private readonly ILogger<MycotoxinController> _logger;
    private readonly IApplicationLayer _applicationLayer;
    private readonly IMycotoxinLayer _mycotoxinLayer;
    private readonly IOptions<ApplicationSettings> _applicationSettings;
    private readonly INotyfService _notyf;
    private MemberAccessViewModel _memberAccessViewModel;

    public MycotoxinController(
      ILogger<MycotoxinController> logger,
      IApplicationLayer applicationLayer,
      IMycotoxinLayer mycotoxinLayer,
      IOptions<ApplicationSettings> applicationSettings,
      INotyfService notyfService)
    {
      _applicationLayer = applicationLayer;
      _logger = logger;
      _mycotoxinLayer = mycotoxinLayer;
      _notyf = notyfService;
      _applicationSettings = applicationSettings;
    }

    // Populate member access asynchronously per-request (avoids blocking a thread-pool thread
    // with .Result in the constructor, which starves the pool under concurrency).
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
      _memberAccessViewModel = await _applicationLayer.GetCurrentMemberAccess().ConfigureAwait(false);
      await next().ConfigureAwait(false);
    }

    public IActionResult Guideline()
    {
      return View();
    }

    public async Task<IActionResult> Index(int? page, bool? filterIsSubmitted = false)
    {
      var pageNo = page ?? 1;
      var result = await _mycotoxinLayer.GetMycotoxinReports(new AlphaPageRequest(pageNo, _applicationSettings.Value.PageSize, "DueDate", false), _memberAccessViewModel.MemberId,
        _memberAccessViewModel.MemberBranchId, _memberAccessViewModel.MemberAccountId, filterIsSubmitted);
      return View(result);
    }

    public async Task<IActionResult> Details(int id)
    {
      try
      {
        var result = await _mycotoxinLayer.GetMycotoxinReport(id, _memberAccessViewModel.MemberAccountId);
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
        var result = await _mycotoxinLayer.GetMycotoxinReport(id, _memberAccessViewModel.MemberAccountId);
        return View(result);
      }
      catch (Exception ex)
      {
        return RedirectToAction("ReportNotFound", "Account");
      }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(MycotoxinReportModel mycotoxinReportModel)
    {
      var result = await _applicationLayer.SaveOrUpdate(mycotoxinReportModel);

      if (result.Success)
      {
        _notyf.Success("Mycotoxin Report updated successfully");
      }
      else
      {
        _notyf.Error($"Mycotoxin Report not updated: {result.ErrorMessage}");
      }

      return RedirectToAction("Details", new { id = mycotoxinReportModel.Id });
    }

    public async Task<IActionResult> Manage(int id)
    {
      try
      {
        var result = await _mycotoxinLayer.GetMycotoxinReport(id, _memberAccessViewModel.MemberAccountId);
        return View(result);
      }
      catch (Exception ex)
      {
        return RedirectToAction("ReportNotFound", "Account");
      }
    }

    public async Task<IActionResult> CreateSample(int mycotoxinReportId)
    {
      var report = await _mycotoxinLayer.GetMycotoxinReportLookup(mycotoxinReportId);
      return View(new MycotoxinSampleModel
      {
        MycotoxinReportId = mycotoxinReportId,
        Member = report.Member,
        MemberBranch = report.MemberBranch,
        IsSubmitted = report.IsSubmitted,
      });
    }

    [HttpPost]
    public async Task<IActionResult> CreateSample(MycotoxinSampleModel mycotoxinSampleModel)
    {
      var result = await _mycotoxinLayer.SaveOrUpdate(mycotoxinSampleModel).ConfigureAwait(false);

      if (result.Success)
      {
        _notyf.Success("Mycotoxin Sample created successfully");
      }
      else
      {
        _notyf.Error($"Mycotoxin Sample not created: {result.ErrorMessage}");
      }

      return RedirectToAction("ViewSample", "Mycotoxin", new { id = result.SavedObjectId, mycotoxinReportId = mycotoxinSampleModel.MycotoxinReportId });
    }

    public async Task<IActionResult> ViewSample(int id, int mycotoxinReportId)
    {
      var result = await _mycotoxinLayer.GetMycotoxinSample(id, mycotoxinReportId).ConfigureAwait(false);
      return View(result);
    }

    public async Task<IActionResult> EditSample(int id, int reportId)
    {
      var result = await _mycotoxinLayer.GetMycotoxinSample(id, reportId).ConfigureAwait(false);
      return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> EditSample(MycotoxinSampleModel mycotoxinSampleModel)
    {
      var result = await _mycotoxinLayer.SaveOrUpdateSample(mycotoxinSampleModel).ConfigureAwait(false);
      if (result.Succeeded)
      {
        _notyf.Success("Sample updated successfully.");
      }
      else
      {
        _notyf.Error($"Sample could not be updated: {result.ExceptionMessage}");
      }

      return RedirectToAction("ViewSample", "Mycotoxin", new { id = result.SavedObjectId, mycotoxinReportId = mycotoxinSampleModel.MycotoxinReportId });
    }

    public async Task<IActionResult> Submit(int mycotoxinReportId)
    {
      var result = await _mycotoxinLayer.UpdateMycotoxinReportStatus(mycotoxinReportId, true);

      if (result.Success)
      {
        _notyf.Success("Mycotoxin Report submitted successfully");
      }
      else
      {
        _notyf.Error($"Mycotoxin Report not submitted: {result.ErrorMessage}");
      }

      return RedirectToAction("Details", new { id = mycotoxinReportId });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteSample(int reportId, int sampleId)
    {
      var result = await _mycotoxinLayer.DeleteSample(reportId, sampleId).ConfigureAwait(false);
      if (result.Succeeded)
      {
        _notyf.Success("Sample deleted successfully.");
      }
      else
      {
        _notyf.Error($"Sample could not be deleted: {result.ExceptionMessage}");
      }
      return RedirectToAction("Details", "Mycotoxin", new { id = reportId });
    }

    [HttpPost]
    public async Task<IActionResult> DeleteSampleDetection(int reportId, int sampleId, int sampleDetectionId)
    {
      var result = await _mycotoxinLayer.DeleteSampleDetection(reportId, sampleId, sampleDetectionId).ConfigureAwait(false);
      if (result.Succeeded)
      {
        _notyf.Success("Detection removed successfully.");
      }
      else
      {
        _notyf.Error($"Detection could not be removed: {result.ExceptionMessage}");
      }
      return RedirectToAction("ViewSample", "Mycotoxin", new { id = sampleId, mycotoxinReportId = reportId });
    }

    [HttpPost]
    public async Task<IActionResult> CreateDetection(int reportId, int sampleId, IFormCollection collection)
    {
      var sampleDetectionModel = new MycotoxinSampleDetectionModel
      {
        MycotoxinClassId = int.Parse(collection["MycotoxinClassId"]!),
        MycotoxinTypeId = int.Parse(collection["MycotoxinTypeId"]!),
        Concentration = collection["Concentration"],
        Lod = collection["Lod"]!,
        MycotoxinSampleId = sampleId,
        MycotoxinReportId = reportId,
      };

      var result = await _mycotoxinLayer.SaveOrUpdate(sampleDetectionModel);
      if (result.Success)
      {
        _notyf.Success("Mycotoxin detection added successfully.");
      }
      else
      {
        _notyf.Error($"Mycotoxin detection could not be added: {result.ErrorMessage}");
      }
      return RedirectToAction("ViewSample", "Mycotoxin", new { id = sampleId, mycotoxinReportId = reportId });
    }
  }
}
