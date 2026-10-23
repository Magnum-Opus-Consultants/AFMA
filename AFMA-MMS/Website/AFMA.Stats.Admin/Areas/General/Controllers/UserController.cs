namespace AFMA.Stats.Admin.Areas.General.Controllers
{
  using AFMA.ApplicationLayer.Interface;
  using AFMA.Infrastructure.Settings;
  using Alpha.Services.Models.Data.PageData;
  using ApplicationLayer;
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.Extensions.Options;

  [Area("General")]
  [Authorize(Roles = "Admin")]
  public class UserController : Controller
  {
    private readonly ILogger<UserController> _logger;
    private readonly IApplicationLayer _applicationLayer;
    private readonly IIdentityLayer _identityLayer;
    private readonly IOptions<ApplicationSettings> _applicationSettings;

    public UserController(ILogger<UserController> logger, IApplicationLayer applicationLayer, IIdentityLayer identityLayer, IOptions<ApplicationSettings> applicationSettings)
    {
      _applicationLayer = applicationLayer;
      _logger = logger;
      _identityLayer = identityLayer;
      _applicationSettings = applicationSettings;
    }

    public async Task<IActionResult> Index(int? page, string? filterSearchText)
    {
      var pageNo = page ?? 1;
      var results = await _applicationLayer
        .GetUsersWithRoles(new AlphaPageRequest(pageNo, _applicationSettings.Value.PageSize), filterSearchText).ConfigureAwait(false);
      return View(results);
    }
  }
}
