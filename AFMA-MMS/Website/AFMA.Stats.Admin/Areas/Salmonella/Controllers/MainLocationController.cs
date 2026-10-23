namespace AFMA.Stats.Admin.Areas.Salmonella.Controllers
{
  using AFMA.Infrastructure.Settings;
  using Alpha.Services.Models.Data.PageData;
  using ApplicationLayer.Interface;
  using ApplicationLayer.Model;
  using AspNetCoreHero.ToastNotification.Abstractions;
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.Extensions.Options;

  [Area("Salmonella")]
  [Authorize(Roles = "Admin")]
  public class MainLocationController : Controller
  {
    private readonly ILogger<MainLocationController> _logger;
    private readonly IApplicationLayer _applicationLayer;
    private readonly INotyfService _notyf;
    private readonly IOptions<ApplicationSettings> _applicationSettings;

    public MainLocationController(ILogger<MainLocationController> logger, IApplicationLayer applicationLayer, INotyfService notyf, IOptions<ApplicationSettings> applicationSettings)
    {
      _logger = logger;
      _applicationLayer = applicationLayer;
      _notyf = notyf;
      _applicationSettings = applicationSettings;
    }

    public async Task<IActionResult> Index(int? page, int? filterSalmonellaSampleTypeId, string? filterSearchText)
    {
      var pageNo = page ?? 1;
      var result = await _applicationLayer.GetMainLocations(new AlphaPageRequest(pageNo, _applicationSettings.Value.PageSize, "Name"), filterSalmonellaSampleTypeId, filterSearchText).ConfigureAwait(false);
      return View(result);
    }

    public async Task<IActionResult> Create()
    {
      return View();
    }

    public async Task<IActionResult> Details(int id)
    {
      var result = await _applicationLayer.GetMainLocation(id).ConfigureAwait(false);
      return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(SalmonellaSampleTypeItemModel salmonellaSampleTypeItemModel)
    {
      var result = await _applicationLayer.SaveOrUpdate(salmonellaSampleTypeItemModel);

      if (result.Success)
      {
        _notyf.Success("Main location created successfully");
      }
      else
      {
        _notyf.Error($"Main location not created: {result.ErrorMessage}");
      }

      return RedirectToAction("Index", "MainLocation", new { area = "Salmonella" });
    }

    public async Task<IActionResult> Edit(int id)
    {
      var result = await _applicationLayer.GetMainLocation(id);
      return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(SalmonellaSampleTypeItemModel salmonellaSampleTypeItemModel)
    {
      var result = await _applicationLayer.SaveOrUpdate(salmonellaSampleTypeItemModel);

      if (result.Success)
      {
        _notyf.Success("Main location updated successfully");
      }
      else
      {
        _notyf.Error($"Main location not updated: {result.ErrorMessage}");
      }

      return RedirectToAction("Details", "MainLocation", new { area = "Salmonella", salmonellaSampleTypeItemModel.Id });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int salmonellaSampleTypeItemId)
    {
      var result = await _applicationLayer.DeleteMainLocation(salmonellaSampleTypeItemId).ConfigureAwait(false);

      if (result.Succeeded)
      {
        _notyf.Success("Main location deleted successfully");
      }
      else
      {
        _notyf.Error($"Main location could not be deleted: {result.ExceptionMessage}");
      }

      return RedirectToAction("Index", "MainLocation", new { area = "Salmonella" });
    }
  }
}
