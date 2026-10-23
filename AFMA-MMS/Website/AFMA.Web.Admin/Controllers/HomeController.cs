namespace AFMA.Web.Admin.Controllers;
using AFMA.ApplicationLayer.Interface;
using AFMA.Web.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

[Authorize]
public class HomeController(IWimsApplicationLayer wimsApplicationLayer, ILogger<HomeController> logger) : Controller
{
  private readonly IWimsApplicationLayer _wimsApplicationLayer = wimsApplicationLayer;
  private readonly ILogger<HomeController> _logger = logger;

  public IActionResult Index()
  {
    return View();
  }

  public IActionResult Privacy()
  {
    return View();
  }

  [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
  public IActionResult Error()
  {
    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
  }
}
