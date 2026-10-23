namespace AFMA.Stats.Admin.Areas.Salmonella.Controllers
{
  using ApplicationLayer.Interface;
  using ApplicationLayer.Model;
  using AspNetCoreHero.ToastNotification.Abstractions;
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Mvc;

  [Area("Salmonella")]
  [Authorize(Roles = "Admin")]
  public class SampleTypeController : Controller
  {
    private readonly ILogger<SampleTypeController> _logger;
    private readonly IApplicationLayer _applicationLayer;
    private readonly INotyfService _notyf;

    public SampleTypeController(ILogger<SampleTypeController> logger, IApplicationLayer applicationLayer, INotyfService notyf)
    {
      _logger = logger;
      _applicationLayer = applicationLayer;
      _notyf = notyf;
    }

    public async Task<IActionResult> Index()
    {
      var result = await _applicationLayer.GetSampleTypes();
      return View(result);
    }

    public async Task<IActionResult> Create()
    {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(SalmonellaSampleTypeModel salmonellaSampleTypeModel)
    {
      var result = await _applicationLayer.SaveOrUpdate(salmonellaSampleTypeModel);

      if (result.Success)
      {
        _notyf.Success("Sample Type created successfully");
      }
      else
      {
        _notyf.Error($"Sample Type not created: {result.ErrorMessage}");
      }

      return RedirectToAction("Index", "SampleType", new { area = "Salmonella" });
    }

    public async Task<IActionResult> Edit(int id)
    {
      var result = await _applicationLayer.GetSampleType(id);
      return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(SalmonellaSampleTypeModel salmonellaSampleTypeModel)
    {
      var result = await _applicationLayer.SaveOrUpdate(salmonellaSampleTypeModel);

      if (result.Success)
      {
        _notyf.Success("Sample Type updated successfully");
      }
      else
      {
        _notyf.Error($"Sample Type not updated: {result.ErrorMessage}");
      }

      return RedirectToAction("Index", "SampleType", new { area = "Salmonella" });
    }
  }
}
