namespace AFMA.Stats.Admin.Controllers
{
  using AFMA.ApplicationLayer.Interface;
  using AFMA.ApplicationLayer.Model;
  using Alpha.Services.Models.Data.PageData;
  using ApplicationLayer;
  using AspNetCoreHero.ToastNotification.Abstractions;
  using Infrastructure.Settings;
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.Extensions.Options;
  using NHibernate.Engine.Query;

  [Authorize(Roles = "Admin")]
  public class MycotoxinReportController : Controller
  {
    private readonly ILogger<MycotoxinReportController> _logger;
    private readonly IApplicationLayer _applicationLayer;
    private readonly IMycotoxinLayer _mycotoxinLayer;
    private readonly INotyfService _notyf;
    private readonly IOptions<ApplicationSettings> _applicationSettings;

    public MycotoxinReportController(ILogger<MycotoxinReportController> logger, IApplicationLayer applicationLayer, IMycotoxinLayer mycotoxinLayer, INotyfService notyf, IOptions<ApplicationSettings> applicationSettiongs)
    {
      _logger = logger;
      _applicationLayer = applicationLayer;
      _mycotoxinLayer = mycotoxinLayer;
      _notyf = notyf;
      _applicationSettings = applicationSettiongs;
    }

    public async Task<IActionResult> Dashboard()
    {
      return View();
    }

    public async Task<IActionResult> Guideline()
    {
      return View();
    }

    public async Task<IActionResult> Index(int? page, int? filterMemberId, int? filterMemberBranchId, bool? filterIsSubmitted)
    {
      var pageNo = page ?? 1;

      var results = await _mycotoxinLayer.GetMycotoxinReports(
        new AlphaPageRequest(pageNo, _applicationSettings.Value.PageSize, "InfoFor", true), filterMemberId,
        filterMemberBranchId, filterIsSubmitted);
      return View(results.Where(x => x.InfoFor <= DateTime.Now).OrderByDescending(a => a.Id).Take(1000));
    }

    public async Task<IActionResult> Details(int id)
    {
      var result = await _mycotoxinLayer.GetMycotoxinReport(id).ConfigureAwait(false);
      return View(result);
    }

    public async Task<IActionResult> Edit(int id)
    {
      var result = await _mycotoxinLayer.GetMycotoxinReport(id).ConfigureAwait(false);
      return View(result);
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

    [HttpPost]
    public async Task<IActionResult> DeleteMycotoxinReport(int mycotoxinReportId)
    {
      var result = await _mycotoxinLayer.DeleteMycotoxinReport(mycotoxinReportId);

      if (result.Succeeded)
      {
        _notyf.Success("Mycotoxin Report deleted successfully");
      }
      else
      {
        _notyf.Error($"Mycotoxin Report could not be deleted : {result.ExceptionMessage}");
      }

      return RedirectToAction("Index", "MycotoxinReport");
    }

    public async Task<IActionResult> CreateSample(int mycotoxinReportId)
    {
      return View(new MycotoxinSampleModel { MycotoxinReportId = mycotoxinReportId });
    }

    [HttpPost]
    public async Task<IActionResult> CreateSample(MycotoxinSampleModel mycotoxinSampleModel)
    {
      var result = await _mycotoxinLayer.SaveOrUpdate(mycotoxinSampleModel).ConfigureAwait(false);
      return RedirectToAction("ViewSample", "MycotoxinReport", new { id = result.SavedObjectId, mycotoxinReportId = mycotoxinSampleModel.MycotoxinReportId });
    }

    public async Task<IActionResult> SampleDetails(int id, int reportId)
    {
      var result = await _mycotoxinLayer.GetMycotoxinSample(id, reportId);
      return View(result);
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
      return RedirectToAction("ViewSample", "MycotoxinReport", new { id = sampleId, mycotoxinReportId = reportId });
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
      return RedirectToAction("ViewSample", "MycotoxinReport", new { id = sampleId, mycotoxinReportId = reportId });
    }

    public async Task<IActionResult> ViewSample(int id, int mycotoxinReportId)
    {
      var result = await _mycotoxinLayer.GetMycotoxinSample(id, mycotoxinReportId).ConfigureAwait(false);
      return View(result);
    }

    public async Task<IActionResult> EditSample(int id, int reportId)
    {
      var result = await _mycotoxinLayer.GetMycotoxinSample(id,reportId).ConfigureAwait(false);
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

      return RedirectToAction("ViewSample", "MycotoxinReport", new { id = result.SavedObjectId, mycotoxinReportId = mycotoxinSampleModel.MycotoxinReportId });
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
      return RedirectToAction("Details", "MycotoxinReport", new { id = reportId });
    }

    public async Task<IActionResult> Submissions(int? filterYear, int? filterMonth)
    {
      var result =
        await _mycotoxinLayer.GetMycotoxinSubmissions(new DateTime(filterYear ?? DateTime.Now.Year, filterMonth ?? DateTime.Now.Month, 1));
      return View(result);
    }

    public async Task<IActionResult> SendReminder(int id)
    {
      var result = await _mycotoxinLayer.SendMycotoxinSubmissionReminder(id).ConfigureAwait(false);

      if (result)
      {
        _notyf.Success("Mycotoxin submission reminder sent successfully");
      }
      else
      {
        _notyf.Error("Mycotoxin submission reminder could not be sent");
      }

      return RedirectToAction("Submissions", "MycotoxinReport");
    }

    public async Task<IActionResult> Submit(int mycotoxinReportId)
    {
      var result = await _mycotoxinLayer.UpdateMycotoxinReportStatus(mycotoxinReportId, true);

      if (result.Success)
      {
        _notyf.Success("Feedsales Report submitted successfully");
      }
      else
      {
        _notyf.Error($"Feedsales Report not submitted: {result.ErrorMessage}");
      }

      return RedirectToAction("Details", "MycotoxinReport", new { id = mycotoxinReportId });
    }

    public async Task<IActionResult> Unsubmit(int mycotoxinReportId)
    {
      var result = await _mycotoxinLayer.UpdateMycotoxinReportStatus(mycotoxinReportId, false);

      if (result.Success)
      {
        _notyf.Success("Mycotoxin Report unsubmitted successfully");
      }
      else
      {
        _notyf.Error($"Mycotoxin Report not unsubmitted: {result.ErrorMessage}");
      }

      return RedirectToAction("Details", "MycotoxinReport", new { id = mycotoxinReportId });
    }


    [HttpPost]
    public async Task<IActionResult> Submissions(DateTime? period)
    {
      var result = await _mycotoxinLayer.GetMycotoxinSubmissions(period ?? new DateTime(2022, 1, 1));
      return View(result);
    }

    public async Task<IActionResult> GenerateMycotoxinTypeSelectList(int mycotoxinClassId)
    {
      var result = await _mycotoxinLayer.GetMycotoxinTypes(mycotoxinClassId).ConfigureAwait(false);
      var r = result.OrderBy(o => o.Name).Select(x => new
      {
        x.Id,
        x.Name,
      }).ToList();

      return Json(r);
    }
  }
}
