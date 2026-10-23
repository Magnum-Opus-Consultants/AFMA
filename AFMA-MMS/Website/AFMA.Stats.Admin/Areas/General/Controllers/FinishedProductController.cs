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
  public class FinishedProductController : Controller
  {
    private readonly ILogger<FinishedProductController> _logger;
    private readonly IApplicationLayer _applicationLayer;
    private readonly INotyfService _notyf;
    private readonly IOptions<ApplicationSettings> _applicationSettings;

    public FinishedProductController(ILogger<FinishedProductController> logger, IApplicationLayer applicationLayer, INotyfService notyf, IOptions<ApplicationSettings> applicationSettings)
    {
      _logger = logger;
      _applicationLayer = applicationLayer;
      _notyf = notyf;
      _applicationSettings = applicationSettings;
    }

    public async Task<IActionResult> Index(int? page, int? filterProductGroupId, string? filterSearchText)
    {
      var pageNo = page ?? 1;
      var results = await _applicationLayer.GetFinishedProducts(new AlphaPageRequest(pageNo, _applicationSettings.Value.PageSize, "Name"), filterProductGroupId, filterSearchText).ConfigureAwait(false);
      return View(results);
    }

    public IActionResult Create()
    {
      return View();
    }

    public async Task<IActionResult> Details(int id)
    {
      var result = await _applicationLayer.GetFinishedProduct(id).ConfigureAwait(false);
      return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProductModel productModel)
    {
      var result = await _applicationLayer.SaveOrUpdate(productModel);

      if (result.Success)
      {
        _notyf.Success("Finished Product created successfully");
      }
      else
      {
        _notyf.Error($"Finished Product not created: {result.ErrorMessage}");
      }

      return RedirectToAction("Index", "FinishedProduct", new { area = "General" });
    }

    public async Task<IActionResult> Edit(int id)
    {
      var result = await _applicationLayer.GetFinishedProduct(id);
      return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ProductModel productModel)
    {
      var result = await _applicationLayer.SaveOrUpdate(productModel);

      if (result.Success)
      {
        _notyf.Success("Finished Product updated successfully");
      }
      else
      {
        _notyf.Error($"Finished Product not updated: {result.ErrorMessage}");
      }

      return RedirectToAction("Index", "FinishedProduct", new { area = "General" });
    }

    public async Task<IActionResult> GenerateFinishedProductSelectList(int productGroupId, int? feedsalesReportId)
    {
      var productsForCategory = await _applicationLayer.GetFinishedProducts(productGroupId).ConfigureAwait(false);

      if (feedsalesReportId != null)
      {
        var reportItems = await _applicationLayer.GetFeedsalesReportItems(feedsalesReportId.GetValueOrDefault()).ConfigureAwait(false);
        var productIds = reportItems.Select(ri => ri.ProductId).ToArray();
        var results = productsForCategory.Where(x => x.AllowDuplicates || !productIds.Contains(x.Id)).OrderBy(o => o.Name).ThenBy(p => p.DisplayOrder).Select(r => new
        {
          r.Id,
          r.Code,
          Name = $"{r.Code} - {r.Name}",
        });
        return Json(results);
      }

      return Json(productsForCategory.OrderBy(o => o.Name).ThenBy(p => p.DisplayOrder).Select(x => new
      {
        x.Id,
        x.Code,
        Name = $"{x.Code} - {x.Name}",
      }));
    }

    public async Task<IActionResult> IsOtherFinishedProduct(int productId)
    {
      var product = await _applicationLayer.GetFinishedProduct(productId).ConfigureAwait(false);
      return Json(new
      {
        product.Id,
        product.Other,
        product.AllowDuplicates,
        Name = product.Name.ToLower(),
      });
    }
  }
}
