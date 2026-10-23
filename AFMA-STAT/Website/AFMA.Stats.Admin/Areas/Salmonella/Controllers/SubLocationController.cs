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
  public class SubLocationController : Controller
  {
    private readonly ILogger<SubLocationController> _logger;
    private readonly IApplicationLayer _applicationLayer;
    private readonly INotyfService _notyf;
    private readonly IOptions<ApplicationSettings> _applicationSettings;

    public SubLocationController(ILogger<SubLocationController> logger, IApplicationLayer applicationLayer, INotyfService notyf, IOptions<ApplicationSettings> applicationSettings)
    {
      _logger = logger;
      _applicationLayer = applicationLayer;
      _notyf = notyf;
      _applicationSettings = applicationSettings;
    }

    public async Task<IActionResult> Index(int? page, int? filterSalmonellaSampleTypeId, string? filterSearchText)
    {
      var pageNo = page ?? 1;
      var result = await _applicationLayer
        .GetSubLocations(new AlphaPageRequest(pageNo, _applicationSettings.Value.PageSize,"Name"),
          filterSalmonellaSampleTypeId, filterSearchText).ConfigureAwait(false);

      return View(result);
    }

    public async Task<IActionResult> Create()
    {
      return View();
    }

    public async Task<IActionResult> Details(int id)
    {
      var result = await _applicationLayer.GetSubLocation(id).ConfigureAwait(false);
      return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(SalmonellaSampleTypeSubItemModel salmonellaSampleTypeSubItemModel)
    {
      var result = await _applicationLayer.SaveOrUpdate(salmonellaSampleTypeSubItemModel);

      if (result.Success)
      {
        _notyf.Success("Sub location created successfully");
      }
      else
      {
        _notyf.Error($"Sub location not created: {result.ErrorMessage}");
      }

      return RedirectToAction("Index", "SubLocation", new { area = "Salmonella" });
    }

    public async Task<IActionResult> Edit(int id)
    {
      var result = await _applicationLayer.GetSubLocation(id);
      return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(SalmonellaSampleTypeSubItemModel salmonellaSampleTypeSubItemModel)
    {
      var result = await _applicationLayer.SaveOrUpdate(salmonellaSampleTypeSubItemModel);

      if (result.Success)
      {
        _notyf.Success("Sub location updated successfully");
      }
      else
      {
        _notyf.Error($"Sub location not updated: {result.ErrorMessage}");
      }

      return RedirectToAction("Details", "SubLocation", new { area = "Salmonella", salmonellaSampleTypeSubItemModel.Id });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int salmonellaSampleTypeSubItemId)
    {
      var result = await _applicationLayer.DeleteSubLocation(salmonellaSampleTypeSubItemId).ConfigureAwait(false);

      if (result.Succeeded)
      {
        _notyf.Success("Sub location deleted successfully");
      }
      else
      {
        _notyf.Error($"Sub location could not be deleted: {result.ExceptionMessage}");
      }

      return RedirectToAction("Index", "SubLocation", new { area = "Salmonella" });
    }
  }
}
