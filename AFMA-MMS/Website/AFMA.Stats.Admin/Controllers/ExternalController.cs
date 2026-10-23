namespace AFMA.Stats.Admin.Controllers
{
  using AFMA.ApplicationLayer.Interface;
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Mvc;

  [AllowAnonymous]
  public class ExternalController(
    ILogger<ExternalController> logger,
    IExternalApiService externalApiService)
    : Controller
  {
    private readonly ILogger<ExternalController> _logger = logger;
    private readonly IExternalApiService _externalApiService = externalApiService;

    public async Task<IActionResult> Feedsales()
    {
      var results = await _externalApiService.GetFeedsalesDashboardDataAsync().ConfigureAwait(false);
      return View(results);
    }

    public async Task<IActionResult> RawMaterial()
    {
      var results = await _externalApiService.GetFeedsalesDashboardDataAsync().ConfigureAwait(false);
      return View(results);
    }

  }

}
