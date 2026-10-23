namespace AFMA.Stats.Client.Controllers
{
  using AFMA.ApplicationLayer.Interface;
  using AFMA.ApplicationLayer.Model;
  using Alpha.Services.Models.Data.PageData;
  using ApplicationLayer.Attributes;
  using ApplicationLayer.Helpers;
  using ApplicationLayer.ViewModels;
  using AspNetCoreHero.ToastNotification.Abstractions;
  using AFMA.Infrastructure.Settings;
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.AspNetCore.Mvc.Filters;
  using Microsoft.Extensions.Options;

  [Authorize(Roles="User")]
  [TypeFilter(typeof(ModulePermissionAttribute), Arguments = new object[] { ModuleEnum.Feedsales })]
  public class FeedsalesController : Controller
  {
    private readonly ILogger<FeedsalesController> _logger;
    private readonly IApplicationLayer _applicationLayer;
    private readonly IFeedsalesLayer _feedsalesLayer;
    private readonly IOptions<ApplicationSettings> _applicationSettings;
    private readonly INotyfService _notyf;
    private MemberAccessViewModel memberAccessViewModel;

    public FeedsalesController(
      ILogger<FeedsalesController> logger,
      IOptions<ApplicationSettings> applicationSettings,
      IApplicationLayer applicationLayer,
      INotyfService notyf,
      IFeedsalesLayer feedsalesLayer)
    {
      _applicationLayer = applicationLayer;
      _logger = logger;
      _feedsalesLayer = feedsalesLayer;
      _applicationSettings = applicationSettings;
      _notyf = notyf;
    }

    // Populate member access asynchronously per-request (avoids blocking a thread-pool thread
    // with .Result in the constructor, which starves the pool under concurrency).
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
      memberAccessViewModel = await _applicationLayer.GetCurrentMemberAccess().ConfigureAwait(false);
      await next().ConfigureAwait(false);
    }

    public async Task<IActionResult> Index(int? page, bool? filterIsSubmitted)
    {
      var pageNo = page ?? 1;
      var result = await _applicationLayer.GetFeedsalesReportsLookup(
        new AlphaPageRequest(pageNo, _applicationSettings.Value.PageSize, "DueDate", true), memberAccessViewModel.MemberId,
        memberAccessViewModel.MemberBranchId, filterIsSubmitted);
      return View(result);
    }

    public async Task<IActionResult> Guideline()
    {
      return View();
    }

    public async Task<IActionResult> Details(int id)
    {
      try
      {
        var result = await _feedsalesLayer.GetFeedsalesReport(id, memberAccessViewModel.MemberBranchId);
        var resultPrevious = await _feedsalesLayer.GetPreviousMonthFeedsalesReport(id, memberAccessViewModel.MemberBranchId).ConfigureAwait(false);
        result.ReportItemsPreviousMonth = resultPrevious.ReportItems;
        return View(result);
      }
      catch (Exception ex)
      {
        return RedirectToAction("ReportNotFound", "Account");
      }
    }

    public async Task<IActionResult> Edit(int id)
    {
      var result = await _feedsalesLayer.GetFeedsalesReport(id, memberAccessViewModel.MemberBranchId);
      return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, string differenceReason, string comment)
    {
      var referer = Request.Headers["Referer"].ToString();
      var redirectAction = "details";

      if (!string.IsNullOrEmpty(referer) && referer.Contains("manage"))
      {
        redirectAction = "manage";
      }

      var result = await _applicationLayer.EditFeedsalesReport(id, differenceReason, comment);

      if (result.Success)
      {
        _notyf.Success("Report updated successfully");
      }
      else
      {
        _notyf.Error($"Report could not bed updated: {result.ErrorMessage}");
      }

      return RedirectToAction(redirectAction, "Feedsales", new { id });
    }

    public async Task<IActionResult> Manage(int id)
    {
      var result = await _feedsalesLayer.GetFeedsalesReport(id, memberAccessViewModel.MemberBranchId);
      var resultPrevious = await _feedsalesLayer.GetPreviousMonthFeedsalesReport(id, memberAccessViewModel.MemberBranchId).ConfigureAwait(false);
      result.ReportItemsPreviousMonth = resultPrevious.ReportItems;
      return View(result);
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

    [HttpPost]
    public async Task<IActionResult> AddFinishedProduct(FeedsalesReportModel feedsalesReportModel)
    {
      var referer = Request.Headers["Referer"].ToString();

      var feedsalesReportItem = new FeedsalesReportItemModel
      {
        FeedsalesReportId = feedsalesReportModel.Id,
        MemberAccountId = feedsalesReportModel.MemberAccountId ?? await _applicationLayer.GetCurrentMemberAccountId().ConfigureAwait(false),
        MemberBranchId = feedsalesReportModel.MemberBranchId,
        MemberId = feedsalesReportModel.MemberId,
        MaterialId = null,
        ProductId = feedsalesReportModel.TProductId,
        Value = feedsalesReportModel.TAmount,
        Other = feedsalesReportModel.TPOther,
      };

      var result = await _applicationLayer.SaveOrUpdate(feedsalesReportItem)
        .ConfigureAwait(false);

      if (result.Success)
      {
        _notyf.Success("Finished Product added successfully");
      }
      else
      {
        _notyf.Error($"Finished Product could not bed added: {result.ErrorMessage}");
      }

      return Redirect($"{referer}#fp");
    }

    [HttpPost]
    public async Task<IActionResult> AddRawMaterial(FeedsalesReportModel feedsalesReportModel)
    {
      var referer = Request.Headers["Referer"].ToString();

      var feedsalesReportItem = new FeedsalesReportItemModel
      {
        FeedsalesReportId = feedsalesReportModel.Id,
        MemberAccountId = feedsalesReportModel.MemberAccountId ?? await _applicationLayer.GetCurrentMemberAccountId().ConfigureAwait(false),
        MemberBranchId = feedsalesReportModel.MemberBranchId,
        MemberId = feedsalesReportModel.MemberId,
        MaterialId = feedsalesReportModel.TMaterialId,
        ProductId = null,
        Value = feedsalesReportModel.TAmount,
        Other = feedsalesReportModel.TMOther,
      };

      var result = await _applicationLayer.SaveOrUpdate(feedsalesReportItem)
        .ConfigureAwait(false);

      if (result.Success)
      {
        _notyf.Success("Raw Material added successfully");
      }
      else
      {
        _notyf.Error($"Raw Material could not be added: {result.ErrorMessage}");
      }

      return Redirect($"{referer}#rm");
    }

    public async Task<IActionResult> DeleteFeedsalesReportItem(int tReportItemId, int feedsalesReportId, string tReportItemType)
    {
      var referer = Request.Headers["Referer"].ToString();

      var result = await _applicationLayer.DeleteFeedsalesReportItem(tReportItemId);

      if (result)
      {
        _notyf.Success("Report Item deleted successfully");
      }
      else
      {
        _notyf.Error("Report Item could not be deleted");
      }

      return Redirect($"{referer}#{tReportItemType}");
    }

    public async Task<IActionResult> Submit(int feedsalesReportId)
    {
      var result = await _applicationLayer.UpdateFeedsalesReportStatus(feedsalesReportId, true);

      if (result.Success)
      {
        _notyf.Success("Feedsales Report submitted successfully");
      }
      else
      {
        _notyf.Error($"Feedsales Report not submitted: {result.ErrorMessage}");
      }

      return RedirectToAction("Details", new { id = feedsalesReportId });
    }

    public async Task<IActionResult> Report()
    {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> FeedsalesTotalReport(ReportCriteriaModel reportCriteria)
    {
      reportCriteria.MemberId = memberAccessViewModel.MemberId;
      reportCriteria.MemberBranchId = memberAccessViewModel.MemberBranchId;
      if (!await Infrastructure.ReportThrottle.WaitAsync())
      {
        _notyf.Warning("The system is busy generating reports. Please try again in a moment.");
        return RedirectToAction(nameof(Report));
      }
      try
      {
        var report = await _feedsalesLayer.GenerateTotalFeedsalesReport(reportCriteria).ConfigureAwait(false);
        return File(report, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TotalFeedsales.xlsx");
      }
      catch (Exception ex)
      {
        _logger.LogWarning(ex, "Feedsales report generation failed for the selected criteria.");
        _notyf.Warning("No data found for the selected period, or the report could not be generated.");
        return RedirectToAction(nameof(Report));
      }
      finally
      {
        Infrastructure.ReportThrottle.Release();
      }
    }
  }
}
