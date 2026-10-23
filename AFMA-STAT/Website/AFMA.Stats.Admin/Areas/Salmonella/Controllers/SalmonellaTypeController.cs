namespace AFMA.Stats.Admin.Areas.Salmonella.Controllers
{
  using Alpha.Services.Models.Data.PageData;
  using ApplicationLayer.Interface;
  using ApplicationLayer.Model;
  using AspNetCoreHero.ToastNotification.Abstractions;
  using Infrastructure.Settings;
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.Extensions.Options;

  [Area("Salmonella")]
  [Authorize(Roles = "Admin")]
  public class SalmonellaTypeController : Controller
  {
    private readonly ILogger<SalmonellaTypeController> _logger;
    private readonly IApplicationLayer _applicationLayer;
    private readonly IOptions<ApplicationSettings> _applicationSettings;
    private readonly ISalmonellaLayer _salmonellaLayer;
    private readonly INotyfService _notyf;

    public SalmonellaTypeController(ILogger<SalmonellaTypeController> logger, IApplicationLayer applicationLayer, INotyfService notyf, ISalmonellaLayer salmonellaLayer, IOptions<ApplicationSettings> applicationSettings)
    {
      _logger = logger;
      _applicationLayer = applicationLayer;
      _notyf = notyf;
      _salmonellaLayer = salmonellaLayer;
      _applicationSettings = applicationSettings;
    }

    public async Task<IActionResult> Index(int? page, int? filterSalmonellaGroupId, string? filterSearchText)
    {
      var pageNo = page ?? 1;
      var result = await _salmonellaLayer.GetSalmonellaTypes(new AlphaPageRequest(pageNo, _applicationSettings.Value.PageSize, "Name"), filterSalmonellaGroupId, filterSearchText).ConfigureAwait(false);
      return View(result);
    }

    public async Task<IActionResult> Details(int id)
    {
      var result = await _salmonellaLayer.GetSalmonellaType(id).ConfigureAwait(false);
      return View(result);
    }

    public async Task<IActionResult> Create()
    {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(SalmonellaTypeModel salmonellaTypeModel)
    {
      var result = await _applicationLayer.SaveOrUpdate(salmonellaTypeModel);

      if (result.Success)
      {
        _notyf.Success("Salmonella Type created successfully");
      }
      else
      {
        _notyf.Error($"Salmonella Type not created: {result.ErrorMessage}");
      }

      return RedirectToAction("Index", "SalmonellaType", new { area = "Salmonella" });
    }

    public async Task<IActionResult> Edit(int id)
    {
      var result = await _salmonellaLayer.GetSalmonellaType(id);
      return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(SalmonellaTypeModel salmonellaTypeModel)
    {
      var result = await _applicationLayer.SaveOrUpdate(salmonellaTypeModel);

      if (result.Success)
      {
        _notyf.Success("Salmonella Type updated successfully");
      }
      else
      {
        _notyf.Error($"Salmonella Type not updated: {result.ErrorMessage}");
      }

      return RedirectToAction("Index", "SalmonellaType", new { area = "Salmonella" });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int salmonellaTypeId)
    {
      var result = await _salmonellaLayer.DeleteSalmonellaType(salmonellaTypeId).ConfigureAwait(false);

      if (result.Succeeded)
      {
        _notyf.Success("Salmonella Type deleted successfully");
      }
      else
      {
        _notyf.Error($"Salmonella Type could not be deleted: {result.ExceptionMessage}");
      }

      return RedirectToAction("Index", "SalmonellaType", new { area = "Salmonella" });
    }
  }
}
