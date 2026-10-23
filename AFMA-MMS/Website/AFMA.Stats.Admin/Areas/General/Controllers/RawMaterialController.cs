namespace AFMA.Stats.Admin.Areas.General.Controllers
{
  using Alpha.Services.Models.Data.PageData;
  using ApplicationLayer.Interface;
  using ApplicationLayer.Model;
  using AspNetCoreHero.ToastNotification.Abstractions;
  using Infrastructure.Settings;
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.Extensions.Options;

  [Area("General")]
  [Authorize(Roles = "Admin")]
  public class RawMaterialController : Controller
  {
    private readonly ILogger<RawMaterialController> _logger;
    private readonly IApplicationLayer _applicationLayer;
    private readonly INotyfService _notyf;
    private readonly IOptions<ApplicationSettings> _applicationSettings;

    public RawMaterialController(ILogger<RawMaterialController> logger, IApplicationLayer applicationLayer, INotyfService notyf, IOptions<ApplicationSettings> applicationSettings)
    {
      _logger = logger;
      _applicationLayer = applicationLayer;
      _notyf = notyf;
      _applicationSettings = applicationSettings;
    }

    public async Task<IActionResult> Index(int? page, int? filterMaterialCategoryId, string? filterSearchText)
    {
      var pageNo = page ?? 1;
      var results = await _applicationLayer
        .GetRawMaterials(new AlphaPageRequest(pageNo, _applicationSettings.Value.PageSize, "Name"), filterMaterialCategoryId,
          filterSearchText).ConfigureAwait(false);
      return View(results);
    }

    public IActionResult Create()
    {
      return View();
    }

    public async Task<IActionResult> Details(int id)
    {
      var result = await _applicationLayer.GetRawMaterial(id).ConfigureAwait(false);
      return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(MaterialModel materialModel)
    {
      var result = await _applicationLayer.SaveOrUpdate(materialModel);

      if (result.Success)
      {
        _notyf.Success("Raw Material created successfully");
      }
      else
      {
        _notyf.Error($"Raw Material not created: {result.ErrorMessage}");
      }

      return RedirectToAction("Index", "RawMaterial", new { area = "General" });
    }

    public async Task<IActionResult> Edit(int id)
    {
      var result = await _applicationLayer.GetRawMaterial(id);
      return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(MaterialModel materialModel)
    {
      var result = await _applicationLayer.SaveOrUpdate(materialModel);

      if (result.Success)
      {
        _notyf.Success("Raw Material updated successfully");
      }
      else
      {
        _notyf.Error($"Raw Material not updated: {result.ErrorMessage}");
      }

      return RedirectToAction("Index", "RawMaterial", new { area = "General" });
    }

    public async Task<IActionResult> GenerateRawMaterialSelectList(int materialCategoryId, int? feedsalesReportId)
    {
      if (materialCategoryId > 0)
      {
        var result = await _applicationLayer.GetRawMaterials(materialCategoryId).ConfigureAwait(false);

        var r = result.OrderBy(o => o.Name).Select(x => new
        {
          x.Id,
          x.Code,
          Name = $"{x.Code} - {x.Name}",
        }).ToList();

        return Json(r);
      }

      var materials = await _applicationLayer.GetRawMaterials().ConfigureAwait(false);

      if (feedsalesReportId != null)
      {
        var reportItems = await _applicationLayer.GetFeedsalesReportItems(feedsalesReportId.GetValueOrDefault()).ConfigureAwait(false);
        var materialIds = reportItems.Select(ri => ri.MaterialId).ToArray();
        var results = materials.Where(x => x.AllowDuplicates || !materialIds.Contains(x.Id)).OrderBy(o => o.Name).Select(r => new
        {
          r.Id,
          r.Code,
          Name = $"{r.Code} - {r.Name}",
        });
        return Json(results);
      }

      return Json(materials.OrderBy(o => o.Name).Select(x => new
      {
        x.Id,
        x.Code,
        Name = $"{x.Code} - {x.Name}",
      }));

    }
    public async Task<IActionResult> IsOtherRawMaterial(int materialId)
    {
      var material = await _applicationLayer.GetRawMaterial(materialId).ConfigureAwait(false);
      return Json(new
      {
        material.Id,
        material.Other,
        material.AllowDuplicates,
        material.ShowAlert,
        material.AlertMessage,
        Name = material.Name.ToLower(),
      });
    }

    public async Task<IActionResult> ShowAlertMessage(int materialId)
    {
      var material = await _applicationLayer.GetRawMaterial(materialId).ConfigureAwait(false);
      return Json(new
      {
        material.Id,
        material.ShowAlert,
        material.AlertMessage,
        Name = material.Name.ToLower(),
      });
    }
  }
}
