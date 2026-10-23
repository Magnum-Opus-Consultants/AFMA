namespace AFMA.Stats.Admin.Areas.General.Controllers
{
  using AFMA.ApplicationLayer.Interface;
  using AspNetCoreHero.ToastNotification.Abstractions;
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Mvc;

  [Area("General")]
  [Authorize(Roles = "Admin")]
  public class ReportAdminController : Controller
  {
    private readonly ILogger<ReportAdminController> _logger;
    private readonly IApplicationLayer _applicationLayer;
    private readonly IIndustryProfileLayer _industryProfileLayer;
    private readonly IIdentityLayer _identityLayer;
    private readonly INotyfService _notyf;

    public ReportAdminController(ILogger<ReportAdminController> logger, IApplicationLayer applicationLayer,
      IIndustryProfileLayer industryProfileLayer,
      IIdentityLayer identityLayer,
      INotyfService notyf)
    {
      _logger = logger;
      _applicationLayer = applicationLayer;
      _notyf = notyf;
      _industryProfileLayer = industryProfileLayer;
      _identityLayer = identityLayer;
    }

    public async Task<IActionResult> PopulateFeedsales()
    {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> PopulateFeedsales(int memberId, int memberBranchId, int memberAccountId, DateTime startDate, DateTime endDate)
    {
      var result = await _applicationLayer.PopulateFeedsalesReports(memberId, memberBranchId, memberAccountId, startDate, endDate);

      if (result)
      {
        _notyf.Success("Feedsales Reports populated successfully");
      }
      else
      {
        _notyf.Error("Feedsales Reports could not be populated");
      }

      return RedirectToAction("PopulateFeedsales", new { status = "success" });
    }

    public async Task<IActionResult> PopulateSalmonella()
    {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> PopulateSalmonella(int memberId, int memberBranchId, int memberAccountId, DateTime startDate, DateTime endDate)
    {
      var result = await _applicationLayer.PopulateSalmonellaReports(memberId, memberBranchId, memberAccountId, startDate, endDate);

      if (result)
      {
        _notyf.Success("Salmonella Reports populated successfully");
      }
      else
      {
        _notyf.Error("Salmonella Reports could not be populated");
      }

      return RedirectToAction("PopulateSalmonella", new { status = "success" });
    }

    public async Task<IActionResult> PopulateMycotoxin()
    {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> PopulateMycotoxin(int memberId, int memberBranchId, int memberAccountId, DateTime startDate, DateTime endDate)
    {
      var result = await _applicationLayer.PopulateMycotoxinReports(memberId, memberBranchId, memberAccountId, startDate, endDate);

      if (result)
      {
        _notyf.Success("Mycotoxin Reports populated successfully");
      }
      else
      {
        _notyf.Error("Mycotoxin Reports could not be populated");
      }

      return RedirectToAction("PopulateMycotoxin", new { status = "success" });
    }

    public async Task<IActionResult> PopulateIndustryProfile()
    {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> PopulateIndustryProfile(int memberId, int memberBranchId, int memberAccountId, int reportYear, int memberTypeId)
    {
      var currentAccountId = await _applicationLayer.GetCurrentMemberAccountId().ConfigureAwait(false);
      var user = await _applicationLayer.GetMemberAccount(currentAccountId).ConfigureAwait(false);
      var result = await _industryProfileLayer.CreateReport(memberId, memberBranchId, currentAccountId, reportYear,
        1, user.UserId, user.FullName);

      if (result.Success)
      {
        _notyf.Success("Industry Profile Report populated successfully");
      }
      else
      {
        _notyf.Error("Industry Profile Report1 could not be populated");
      }

      return RedirectToAction("PopulateIndustryProfile", new { status = "success" });
    }
  }
}
