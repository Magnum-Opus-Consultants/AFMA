namespace AFMA.Stats.Admin.Areas.Mycotoxin.Controllers
{
  using AFMA.ApplicationLayer;
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
  public class ClassController : Controller
  {
    private readonly ILogger<ClassController> _logger;
    private readonly IMycotoxinLayer _mycotoxinLayer;
    private readonly IOptions<ApplicationSettings> _applicationSettings;
    private readonly INotyfService _notyf;

    public ClassController(ILogger<ClassController> logger, IMycotoxinLayer mycotoxinLayer, IOptions<ApplicationSettings> applicationSettings, INotyfService notyf)
    {
      _logger = logger;
      _mycotoxinLayer = mycotoxinLayer;
      _applicationSettings = applicationSettings;
      _notyf = notyf;
    }
    
    public async Task<IActionResult> Index(int? page, string? filterSearchText)
    {
      var pageNo = page ?? 1;
      var result = await _mycotoxinLayer.GetMycotoxinClasses(new AlphaPageRequest(pageNo, _applicationSettings.Value.PageSize, "Name"), filterSearchText).ConfigureAwait(false);
      return View(result);
    }

    public async Task<IActionResult> Details(int id)
    {
      var result = await _mycotoxinLayer.GetMycotoxinClass(id).ConfigureAwait(false);
      return View(result);
    }

    public IActionResult Create()
    {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(MycotoxinClassModel mycotoxinClassModel)
    {
      var result = await _mycotoxinLayer.SaveOrUpdate(mycotoxinClassModel);

      if (result.Success)
      {
        _notyf.Success("Mycotoxin created successfully");
      }
      else
      {
        _notyf.Error($"Mycotoxin not created: {result.ErrorMessage}");
        return RedirectToAction("Index", "Class", new { area = "Mycotoxin" });
      }

      return RedirectToAction("Details", "Class", new { area = "Mycotoxin", id = result.SavedObjectId });
    }

    public async Task<IActionResult> Edit(int id)
    {
      var result = await _mycotoxinLayer.GetMycotoxinClass(id);
      return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(MycotoxinClassModel mycotoxinClassModel)
    {
      var result = await _mycotoxinLayer.SaveOrUpdate(mycotoxinClassModel);

      if (result.Success)
      {
        _notyf.Success("Mycotoxin Class updated successfully");
      }
      else
      {
        _notyf.Error($"Mycotoxin Class not updated: {result.ErrorMessage}");
      }

      return RedirectToAction("Details", "Class", new { area = "Mycotoxin", id = result.SavedObjectId });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int mycotoxinClassId)
    {
      var result = await _mycotoxinLayer.DeleteMycotoxinClass(mycotoxinClassId);
      if (result.Succeeded)
      {
        _notyf.Success("Mycotoxin Class deleted successfully");
      }
      else
      {
        _notyf.Error($"Mycotoxin Class not deleted: {result.ExceptionMessage}");
      }

      return RedirectToAction("Index", "Class", new { area = "Mycotoxin" });
    }
  }
}
