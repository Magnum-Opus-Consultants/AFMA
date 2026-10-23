namespace AFMA.Stats.Admin.Areas.General.Controllers
{
  using ApplicationLayer.Interface;
  using ApplicationLayer.Model;
  using AspNetCoreHero.ToastNotification.Abstractions;
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Mvc;

  [Area("General")]
  [Authorize(Roles = "Admin")]
  public class ProvinceController : Controller
  {
    private readonly ILogger<ProvinceController> _logger;
    private readonly IApplicationLayer _applicationLayer;
    private readonly INotyfService _notyf;

    public ProvinceController(ILogger<ProvinceController> logger, IApplicationLayer applicationLayer, INotyfService notyf)
    {
      _logger = logger;
      _applicationLayer = applicationLayer;
      _notyf = notyf;
    }

    public async Task<IActionResult> Index()
    {
      var results = await _applicationLayer.GetProvinces();
      return View(results);
    }

    public async Task<IActionResult> Create()
    {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProvinceModel provinceModel)
    {
      var result = await _applicationLayer.SaveOrUpdate(provinceModel);

      if (result.Success)
      {
        _notyf.Success("Province created successfully");
      }
      else
      {
        _notyf.Error($"Province not created: {result.ErrorMessage}");
      }

      return RedirectToAction("Index", "Province", new { area = "General" });
    }

    public async Task<IActionResult> Edit(int id)
    {
      var result = await _applicationLayer.GetProvince(id);
      return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ProvinceModel provinceModel)
    {
      var result = await _applicationLayer.SaveOrUpdate(provinceModel);

      if (result.Success)
      {
        _notyf.Success("Province updated successfully");
      }
      else
      {
        _notyf.Error($"Province not updated: {result.ErrorMessage}");
      }

      return RedirectToAction("Index", "Province", new { area = "General" });
    }
  }
}
