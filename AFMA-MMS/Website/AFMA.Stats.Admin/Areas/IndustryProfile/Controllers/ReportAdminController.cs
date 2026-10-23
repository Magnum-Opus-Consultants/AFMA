namespace AFMA.Stats.Admin.Areas.IndustryProfile.Controllers
{
  using AFMA.ApplicationLayer.Interface;
  using AFMA.Infrastructure.Settings;
  using AspNetCoreHero.ToastNotification.Abstractions;
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.Extensions.Options;

  [Area("IndustryProfile")]
  [Authorize(Roles = "Admin")]
  public class ReportAdminController : Controller
  {
    private readonly ILogger<ReportAdminController> _logger;
    private readonly IApplicationLayer _applicationLayer;
    private readonly IIndustryProfileLayer _industryProfileLayer;
    private readonly INotyfService _notyf;
    private readonly IOptions<ApplicationSettings> _applicationSettings;

    public ReportAdminController(
      ILogger<ReportAdminController> logger,
      IApplicationLayer applicationLayer,
      IIndustryProfileLayer industryProfileLayer,
      INotyfService notyf,
      IOptions<ApplicationSettings> applicationSettings)
    {
      _logger = logger;
      _applicationLayer = applicationLayer;
      _notyf = notyf;
      _applicationSettings = applicationSettings;
      _industryProfileLayer = industryProfileLayer;
    }

    public async Task<IActionResult> Create()
    {
      var result = await _industryProfileLayer.CreateReport(115, 641, 2151, 2018, 1,
        "47732368-d227-4fe3-abe1-66a7481400b0",
        "Jacques Swart").ConfigureAwait(false);
      return View();
    }
  }
}
