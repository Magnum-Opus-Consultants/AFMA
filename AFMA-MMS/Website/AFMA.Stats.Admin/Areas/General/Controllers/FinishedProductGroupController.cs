namespace AFMA.Stats.Admin.Areas.General.Controllers
{
  using AFMA.ApplicationLayer.Interface;
  using AFMA.ApplicationLayer.Model;
  using AFMA.Infrastructure.Settings;
  using Alpha.Services.Models.Data.PageData;
  using AspNetCoreHero.ToastNotification.Abstractions;
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.Extensions.Options;
  using NHibernate.Loader;

  [Area("General")]
  [Authorize(Roles = "Admin")]

  public class FinishedProductGroupController : Controller
  {
    private readonly ILogger<FinishedProductGroupController> _logger;
    private readonly IApplicationLayer _applicationLayer;
    private readonly INotyfService _notyf;
    private readonly IOptions<ApplicationSettings> _applicationSettings;

    public FinishedProductGroupController(ILogger<FinishedProductGroupController> logger, IApplicationLayer applicationLayer, INotyfService notyf, IOptions<ApplicationSettings> applicationSettings)
    {
      _logger = logger;
      _applicationLayer = applicationLayer;
      _notyf = notyf;
      _applicationSettings = applicationSettings;
    }

    public async Task<IActionResult> Index(int? page, int? filterProductGroupId, string? filterSearchText)
    {
      var pageNo = page ?? 1;
      var results = await _applicationLayer.GetFinishedProductGroups(new AlphaPageRequest(pageNo, _applicationSettings.Value.PageSize, "Name"), filterSearchText).ConfigureAwait(false);
      return View(results);
    }

    public IActionResult Create()
    {
      return View();
    }

    public async Task<IActionResult> Details(int id)
    {
      var result = await _applicationLayer.GetFinishedProductGroup(id).ConfigureAwait(false);
      return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProductGroupModel productGroupModel)
    {
      var result = await _applicationLayer.SaveOrUpdate(productGroupModel);

      if (result.Success)
      {
        _notyf.Success("Finished Product Group created successfully");
      }
      else
      {
        _notyf.Error($"Finished Product Group not created: {result.ErrorMessage}");
      }

      return RedirectToAction("Details", "FinishedProductGroup", new { area = "General", id = result.SavedObjectId });
    }

    public async Task<IActionResult> Edit(int id)
    {
      var result = await _applicationLayer.GetFinishedProductGroup(id);
      return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ProductGroupModel productGroupModel)
    {
      var result = await _applicationLayer.SaveOrUpdate(productGroupModel);

      if (result.Success)
      {
        _notyf.Success("Finished Product Group updated successfully");
      }
      else
      {
        _notyf.Error($"Finished Product Group not updated: {result.ErrorMessage}");
      }

      return RedirectToAction("Details", "FinishedProductGroup", new { area = "General", id = productGroupModel.Id });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int finishedProductGroupId)
    {
      var result = await _applicationLayer.DeleteFinishedProductGroup(finishedProductGroupId);
      if (result.Succeeded)
      {
        _notyf.Success("Finished Product Group deleted successfully");
      }
      else
      {
        _notyf.Error($"Finished Product Group not deleted: {result.ExceptionMessage}");
      }

      return RedirectToAction("Index", "FinishedProductGroup", new { area = "General" });
    }
  }
}
