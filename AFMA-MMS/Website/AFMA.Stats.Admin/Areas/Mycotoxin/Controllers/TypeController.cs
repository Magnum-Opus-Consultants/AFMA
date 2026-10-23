namespace AFMA.Stats.Admin.Areas.Mycotoxin.Controllers
{
  using AFMA.ApplicationLayer.Interface;
  using AFMA.ApplicationLayer.Model;
  using AFMA.Infrastructure.Settings;
  using Alpha.Services.Models.Data.PageData;
  using AspNetCoreHero.ToastNotification.Abstractions;
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.Extensions.Options;

  [Area("Mycotoxin")]
  [Authorize(Roles = "Admin")]
  public class TypeController : Controller
  {
    private readonly ILogger<TypeController> _logger;
    private readonly IMycotoxinLayer _mycotoxinLayer;
    private readonly IOptions<ApplicationSettings> _applicationSettings;
    private readonly INotyfService _notyf;

    public TypeController(ILogger<TypeController> logger, IMycotoxinLayer mycotoxinLayer, IOptions<ApplicationSettings> applicationSettings, INotyfService notyf)
    {
      _logger = logger;
      _mycotoxinLayer = mycotoxinLayer;
      _applicationSettings = applicationSettings;
      _notyf = notyf;
    }

    public async Task<IActionResult> Index(int? page, int? filterMycotoxinClassId, string? filterSearchText)
    {
      var pageNo = page ?? 1;
      var result = await _mycotoxinLayer.GetMycotoxinTypes(new AlphaPageRequest(pageNo, _applicationSettings.Value.PageSize, "Name"), filterMycotoxinClassId, filterSearchText).ConfigureAwait(false);
      return View(result);
    }

    public async Task<IActionResult> Details(int id)
    {
      var result = await _mycotoxinLayer.GetMycotoxinType(id).ConfigureAwait(false);
      return View(result);
    }

    public IActionResult Create()
    {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(MycotoxinTypeModel mycotoxinTypeModel)
    {
      var result = await _mycotoxinLayer.SaveOrUpdate(mycotoxinTypeModel);

      if (result.Success)
      {
        _notyf.Success("Mycotoxin Type created successfully");
      }
      else
      {
        _notyf.Error($"Mycotoxin Type not created: {result.ErrorMessage}");
        return RedirectToAction("Index", "Type", new { area = "Mycotoxin" });
      }

      return RedirectToAction("Details", "Type", new { area = "Mycotoxin", id = result.SavedObjectId });
    }

    public async Task<IActionResult> Edit(int id)
    {
      var result = await _mycotoxinLayer.GetMycotoxinType(id);
      return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(MycotoxinTypeModel mycotoxinTypeModel)
    {
      var result = await _mycotoxinLayer.SaveOrUpdate(mycotoxinTypeModel);

      if (result.Success)
      {
        _notyf.Success("Mycotoxin Type updated successfully");
      }
      else
      {
        _notyf.Error($"Mycotoxin Type not updated: {result.ErrorMessage}");
      }

      return RedirectToAction("Index", "Type", new { area = "Mycotoxin" });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int mycotoxinTypeId)
    {
      var result = await _mycotoxinLayer.DeleteMycotoxinType(mycotoxinTypeId);
      if (result.Succeeded)
      {
        _notyf.Success("Mycotoxin Type deleted successfully");
      }
      else
      {
        _notyf.Error($"Mycotoxin Type not deleted: {result.ExceptionMessage}");
      }

      return RedirectToAction("Index", "Type", new { area = "Mycotoxin" });
    }

  }
}
