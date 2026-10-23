namespace AFMA.Stats.Admin.Controllers
{
  using AFMA.ApplicationLayer.Interface;
  using AFMA.Infrastructure.Settings;
  using Alpha.Services.Models.Data.PageData;
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.Extensions.Options;

  [Authorize(Roles = "Admin")]
  public class AuditController : Controller
  {
    private readonly ILogger<AuditController> _logger;
    private readonly IApplicationLayer _applicationLayer;
    private readonly IOptions<ApplicationSettings> _applicationSettings;

    public AuditController(ILogger<AuditController> logger, IApplicationLayer applicationLayer, IOptions<ApplicationSettings> applicationSettings)
    {
      _logger = logger;
      _applicationLayer = applicationLayer;
      _applicationSettings = applicationSettings;
    }

    public async Task<IActionResult> LoginHistory(int? page, int? filterMemberId, int? filterMemberBranchId, bool? filterIsSubmitted)
    {
      var pageNo = page ?? 1;
      var results = await _applicationLayer.GetLoginHistoryPaged(
        new AlphaPageRequest(pageNo, _applicationSettings.Value.PageSize, "LoginDate", true));
      return View(results);
    }

    public IActionResult About()
    {
      ViewBag.IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString();
      return View();
    }

    public async Task<IActionResult> MemberModuleAccess()
    {
      var result = await _applicationLayer.GenerateMemberModuleAccessMatrix();
      return View(result);
    }

    public async Task<IActionResult> AccountModuleAccess()
    {
      var result = await _applicationLayer.GenerateAccountModuleAccessMatrix();
      return View(result);
    }

    public async Task<IActionResult> MemberAccountAudit()
    {
      var result = await _applicationLayer.GetMemberAccountAudit();
      return View(result);
    }
  }
}
