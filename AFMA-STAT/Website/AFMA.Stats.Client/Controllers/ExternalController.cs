namespace AFMA.Stats.Client.Controllers
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

    public async Task<IActionResult> Index()
    {
      var results = await _externalApiService.GetFeedsalesDashboardDataAsync().ConfigureAwait(false);
      return View(results);
    }
  }
}
