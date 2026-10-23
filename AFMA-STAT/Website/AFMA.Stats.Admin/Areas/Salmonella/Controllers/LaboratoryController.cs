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
  public class LaboratoryController : Controller
  {
    private readonly ILogger<LaboratoryController> _logger;
    private readonly IApplicationLayer _applicationLayer;
    private readonly INotyfService _notyf;
    private readonly IOptions<ApplicationSettings> _applicationSettings;

    public LaboratoryController(ILogger<LaboratoryController> logger, IApplicationLayer applicationLayer, INotyfService notyf, IOptions<ApplicationSettings> applicationSettings)
    {
      _logger = logger;
      _applicationLayer = applicationLayer;
      _notyf = notyf;
      _applicationSettings = applicationSettings;
    }

    public async Task<IActionResult> Index(int? page, string? filterSearchText)
    {
      var pageNo = page ?? 1;
      var results = await _applicationLayer.GetLaboratories(new AlphaPageRequest(pageNo, _applicationSettings.Value.PageSize, "Name"), filterSearchText).ConfigureAwait(false);
      return View(results);
    }

    public async Task<IActionResult> Details(int id)
    {
      var result = await _applicationLayer.GetLaboratory(id).ConfigureAwait(false);
      return View(result);
    }

    public async Task<IActionResult> Create()
    {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(LaboratoryModel laboratoryModel)
    {
      var result = await _applicationLayer.SaveOrUpdate(laboratoryModel);

      if (result.Success)
      {
        _notyf.Success("Laboratory created successfully");
      }
      else
      {
        _notyf.Error($"Laboratory not created: {result.ErrorMessage}");
      }

      return RedirectToAction("Index", "Laboratory", new { area = "Salmonella" });
    }

    public async Task<IActionResult> Edit(int id)
    {
      var result = await _applicationLayer.GetLaboratory(id);
      return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(LaboratoryModel laboratoryModel)
    {
      var result = await _applicationLayer.SaveOrUpdate(laboratoryModel);

      if (result.Success)
      {
        _notyf.Success("Laboratory updated successfully");
      }
      else
      {
        _notyf.Error($"Laboratory not updated: {result.ErrorMessage}");
      }

      return RedirectToAction("Index", "Laboratory", new { area = "Salmonella" });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int laboratoryId)
    {
      var result = await _applicationLayer.DeleteLaboratory(laboratoryId).ConfigureAwait(false);

      if (result.Succeeded)
      {
        _notyf.Success("Laboratory deleted successfully");
      }
      else
      {
        _notyf.Error($"Laboratory could not be deleted: {result.ExceptionMessage}");
      }

      return RedirectToAction("Index", "Laboratory", new { area = "Salmonella" });
    }
  }
}
