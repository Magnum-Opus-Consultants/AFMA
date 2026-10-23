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
  public class SettingController : Controller
  {
    private readonly ILogger<SettingController> _logger;
    private readonly IApplicationLayer _applicationLayer;
    private readonly INotyfService _notyf;
    private readonly IOptions<ApplicationSettings> _applicationSettings;

    public SettingController(ILogger<SettingController> logger, IApplicationLayer applicationLayer, INotyfService notyf, IOptions<ApplicationSettings> applicationSettings)
    {
      _logger = logger;
      _applicationLayer = applicationLayer;
      _notyf = notyf;
      _applicationSettings = applicationSettings;
    }

    public IActionResult Index()
    {
      return View();
    }
  }
}
