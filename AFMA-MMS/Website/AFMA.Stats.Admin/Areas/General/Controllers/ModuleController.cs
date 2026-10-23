namespace AFMA.Stats.Admin.Areas.General.Controllers
{
  using ApplicationLayer.Interface;
  using ApplicationLayer.Model;
  using AspNetCoreHero.ToastNotification.Abstractions;
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Mvc;

  [Area("General")]
  [Authorize(Roles = "Admin")]
  public class ModuleController : Controller
  {
    private readonly ILogger<ModuleController> _logger;
    private readonly IApplicationLayer _applicationLayer;
    private readonly INotyfService _notyf;

    public ModuleController(ILogger<ModuleController> logger, IApplicationLayer applicationLayer, INotyfService notyf)
    {
      _logger = logger;
      _applicationLayer = applicationLayer;
      _notyf = notyf;
    }

    public async Task<IActionResult> Index()
    {
      var results = await _applicationLayer.GetModules();
      return View(results);
    }

    public async Task<IActionResult> Create()
    {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(ModuleModel moduleModel)
    {
      var result = await _applicationLayer.SaveOrUpdate(moduleModel);

      if (result.Success)
      {
        _notyf.Success("Module created successfully");
      }
      else
      {
        _notyf.Error($"Module not created: {result.ErrorMessage}");
      }

      return RedirectToAction("Index", "Module", new { area = "General" });
    }

    public async Task<IActionResult> Edit(int id)
    {
      var result = await _applicationLayer.GetModule(id);
      return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ModuleModel moduleModel)
    {
      var result = await _applicationLayer.SaveOrUpdate(moduleModel);

      if (result.Success)
      {
        _notyf.Success("Module updated successfully");
      }
      else
      {
        _notyf.Error($"Module not updated: {result.ErrorMessage}");
      }

      return RedirectToAction("Index", "Module", new { area = "General" });
    }
  }
}
