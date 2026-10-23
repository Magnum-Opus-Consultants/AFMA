namespace AFMA.Stats.Admin.Controllers
{
  using System.Diagnostics;
  using AFMA.ApplicationLayer.Interface;
  using AFMA.Stats.Admin.Models;
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.Extensions.Logging;

  [Authorize(Roles = "Admin")]
  public class HomeController : Controller
  {
    private readonly ILogger<HomeController> _logger;
    private readonly IIdentityLayer _identityLayer;

    public HomeController(ILogger<HomeController> logger,
      IIdentityLayer identityLayer)
    {
      _logger = logger;
      _identityLayer = identityLayer;
    }

    public async Task<IActionResult> Index()
    {
      return View();
    }

    public async Task SendTestEmail()
    {
      await _identityLayer.SendTestEmail();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
  }
}