namespace AFMA.Stats.Admin.Areas.Salmonella.Controllers
{
  using AFMA.ApplicationLayer;
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
  public class SalmonellaGroupController : Controller
  {
    private readonly ILogger<SalmonellaGroupController> _logger;
    private readonly IApplicationLayer _applicationLayer;
    private readonly ISalmonellaLayer _salmonellaLayer;
    private readonly IOptions<ApplicationSettings> _applicationSettings;
    private readonly INotyfService _notyf;

    public SalmonellaGroupController(ILogger<SalmonellaGroupController> logger, IApplicationLayer applicationLayer, ISalmonellaLayer salmonellaLayer, IOptions<ApplicationSettings> applicationSettings, INotyfService notyf)
    {
      _logger = logger;
      _applicationLayer = applicationLayer;
      _notyf = notyf;
      _applicationSettings = applicationSettings;
      _salmonellaLayer = salmonellaLayer;
    }

    public async Task<IActionResult> Index(int? page, int? filterSalmonellaGroupId, string? filterSearchText)
    {
      var pageNo = page ?? 1;
      var result = await _salmonellaLayer.GetSalmonellaGroups(new AlphaPageRequest(pageNo, _applicationSettings.Value.PageSize, "Name"), filterSearchText).ConfigureAwait(false);
      return View(result);
    }

    public async Task<IActionResult> Details(int id)
    {
      var result = await _salmonellaLayer.GetSalmonellaGroup(id).ConfigureAwait(false);
      return View(result);
    }

    public async Task<IActionResult> Create()
    {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(SalmonellaGroupModel salmonellaGroupModel)
    {
      var result = await _applicationLayer.SaveOrUpdate(salmonellaGroupModel);

      if (result.Success)
      {
        _notyf.Success("Salmonella Group created successfully");
      }
      else
      {
        _notyf.Error($"Salmonella Group not created: {result.ErrorMessage}");
      }

      return RedirectToAction("Index", "SalmonellaGroup", new { area = "Salmonella" });
    }

    public async Task<IActionResult> Edit(int id)
    {
      var result = await _salmonellaLayer.GetSalmonellaGroup(id);
      return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(SalmonellaGroupModel salmonellaGroupModel)
    {
      var result = await _applicationLayer.SaveOrUpdate(salmonellaGroupModel);

      if (result.Success)
      {
        _notyf.Success("Salmonella Group updated successfully");
      }
      else
      {
        _notyf.Error($"Salmonella Group not updated: {result.ErrorMessage}");
      }

      return RedirectToAction("Index", "SalmonellaGroup", new { area = "Salmonella" });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int salmonellaGroupId)
    {
      var result = await _salmonellaLayer.DeleteSalmonellaGroup(salmonellaGroupId).ConfigureAwait(false);

      if (result.Succeeded)
      {
        _notyf.Success("Salmonella Group deleted successfully");
      }
      else
      {
        _notyf.Error($"Salmonella Group could not be deleted: {result.ExceptionMessage}");
      }

      return RedirectToAction("Index", "SalmonellaGroup", new { area = "Salmonella" });
    }
  }
}
