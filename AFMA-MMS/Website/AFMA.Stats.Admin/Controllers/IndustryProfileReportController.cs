namespace AFMA.Stats.Admin.Controllers
{
  using AFMA.ApplicationLayer.Interface;
  using AFMA.ApplicationLayer.ViewModels;
  using AFMA.Infrastructure.Settings;
  using Alpha.Services.Models.Data.PageData;
  using ApplicationLayer.Helpers;
  using ApplicationLayer.Model;
  using AspNetCoreHero.ToastNotification.Abstractions;
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.EntityFrameworkCore;
  using Microsoft.Extensions.Options;
  using NuGet.Common;

  [Authorize(Roles = "Admin")]
  public class IndustryProfileReportController : Controller
  {
    private readonly ILogger<IndustryProfileReportController> _logger;
    private readonly IApplicationLayer _applicationLayer;
    private readonly IIndustryProfileLayer _industryProfileLayer;
    private readonly INotyfService _notyf;
    private readonly IOptions<ApplicationSettings> _applicationSettings;

    public IndustryProfileReportController(
      ILogger<IndustryProfileReportController> logger,
      IApplicationLayer applicationLayer,
      IIndustryProfileLayer industryProfileLayer,
      IOptions<ApplicationSettings> applicationSettings,
      INotyfService notyf)
    {
      _logger = logger;
      _applicationLayer = applicationLayer;
      _industryProfileLayer = industryProfileLayer;
      _applicationSettings = applicationSettings;
      _notyf = notyf;
    }

    public async Task<IActionResult> Index(int? page, int? filterMemberId, int? filterMemberBranchId, bool? filterIsSubmitted)
    {
      var pageNo = page ?? 1;
      var results = await _applicationLayer.GetIndustryProfileReports(new AlphaPageRequest(pageNo, _applicationSettings.Value.PageSize, "ReportYear", true), filterMemberId,
        filterMemberBranchId, filterIsSubmitted);
      return View(results);
    }

    public async Task<IActionResult> Create()
    {
      return View();
    }

    public async Task<IActionResult> Details(int id)
    {
      try
      {
        var report = await _applicationLayer.GetIndustryProfileReport(id).ConfigureAwait(false);

        var companyIpReportViewModel = new CompanyIndustryProfileViewModel
        {
          MemberBranchId = report.MemberBranchId,
          MemberBranch = report.MemberBranch,
          MemberId = report.MemberId,
          MemberName = report.Member, //TODO: rename this
          IndustryProfileReportId = report.Id,
          ReportYear = report.ReportYear.ToString(),
          MemberTypeId = report.MemberTypeId,
          MemberType = report.MemberType,
          CreatedByUser = report.CreatedBy,
          IsSubmitted = report.Submitted,
        };

        companyIpReportViewModel.MemberIpBaseItems = await _applicationLayer.GetMemberIpBaseItems(
          IndustryProfileDisplayType.Other,
          report.MemberId, report.MemberBranchId, report.Id, report.MemberTypeId).ConfigureAwait(false);

        companyIpReportViewModel.MemberIpAdminItems = await _applicationLayer.GetMemberIpAdminItems(
          IndustryProfileDisplayType.Other,
          report.MemberId, report.MemberBranchId, report.Id, report.MemberTypeId).ConfigureAwait(false);

        companyIpReportViewModel.MemberIpAdditionalItems = await _applicationLayer.GetMemberIpAdditionalItems(
          IndustryProfileDisplayType.Other,
          report.MemberId, report.MemberBranchId, report.Id, report.MemberTypeId).ConfigureAwait(false);

        companyIpReportViewModel.MemberRunningCosts = await _applicationLayer.GetMemberRunningCosts(
          IndustryProfileDisplayType.Other,
          report.MemberId, report.MemberBranchId, report.Id, report.MemberTypeId).ConfigureAwait(false);

        companyIpReportViewModel.MemberRunningCostsFleet = await _applicationLayer.GetMemberRunningCostsFleet(
          IndustryProfileDisplayType.Other,
          report.MemberId, report.MemberBranchId, report.Id, report.MemberTypeId).ConfigureAwait(false);

        companyIpReportViewModel.MemberRunningCostsSundries = await _applicationLayer.GetMemberRunningCostsSundries(
          IndustryProfileDisplayType.Other,
          report.MemberId, report.MemberBranchId, report.Id, report.MemberTypeId).ConfigureAwait(false);

        companyIpReportViewModel.MemberTotalRawMaterialCosts = await _applicationLayer.GetMemberTotalRawMaterialCosts(
          IndustryProfileDisplayType.Other,
          report.MemberId, report.MemberBranchId, report.Id, report.MemberTypeId).ConfigureAwait(false);

        companyIpReportViewModel.MemberPelletCostItems = await _applicationLayer.GetMemberPcbCostItems(PcbItem.Pellet,
            IndustryProfileDisplayType.Other, report.MemberId, report.MemberBranchId, report.Id, report.MemberTypeId)
          .ConfigureAwait(false);

        companyIpReportViewModel.MemberCrumblingCostItems = await _applicationLayer
          .GetMemberPcbCostItems(PcbItem.Crumbling, IndustryProfileDisplayType.Other, report.MemberId,
            report.MemberBranchId, report.Id, report.MemberTypeId).ConfigureAwait(false);

        companyIpReportViewModel.MemberBaggingCostItems = await _applicationLayer.GetMemberPcbCostItems(
            PcbItem.Bagging,
            IndustryProfileDisplayType.Other, report.MemberId, report.MemberBranchId, report.Id, report.MemberTypeId)
          .ConfigureAwait(false);

        companyIpReportViewModel.IndustryCostsGroups =
          await _applicationLayer.GetIndustryCostGroups(false).ConfigureAwait(false);

        return View(companyIpReportViewModel);

      }
      catch (DbUpdateException ex)
      {
        return View("Error");
      }
    }

    public async Task<IActionResult> EditIndustryProfileBaseInfo(int id, int branchId)
    {
      var report = await _applicationLayer.GetIndustryProfileReport(id);

      var companyIpReportViewModel = new CompanyIndustryProfileViewModel
      {
        MemberBranchId = report.MemberBranchId,
        MemberBranch = report.MemberBranch,
        MemberId = report.MemberId,
        MemberName = report.Member, //TODO: rename this
        IndustryProfileReportId = report.Id,
        ReportYear = report.ReportYear.ToString(),
        MemberTypeId = report.MemberTypeId,
        MemberType = report.MemberType,
        CreatedByUser = report.CreatedBy,
        MemberIpBaseItems = await _applicationLayer.GetMemberIpBaseItems(IndustryProfileDisplayType.Other,
          report.MemberId, report.MemberBranchId, report.Id, report.MemberTypeId).ConfigureAwait(false),
      };

      return View(companyIpReportViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> EditIndustryProfileBaseInfo(IFormCollection collection)
    {
      var reportId = Convert.ToInt32(collection["IndustryProfileReportId"]);
      var memberId = Convert.ToInt32(collection["MemberId"]);
      var memberBranchId = Convert.ToInt32(collection["MemberBranchId"]);
      var memberTypeId = Convert.ToInt32(collection["MemberTypeId"]);
      var memberBaseItems = await _applicationLayer.GetMemberIpBaseItems(IndustryProfileDisplayType.Other,
        memberId, memberBranchId, reportId, memberTypeId).ConfigureAwait(false);

      foreach (var item in memberBaseItems)
      {
        decimal.TryParse(collection[$"IPBaseItem_{item.Id}"], out decimal newValue);
        item.Value = newValue;
        await _applicationLayer.SaveOrUpdateIpBaseItem(item.Id, newValue);
      }

      return RedirectToAction("Details", "IndustryProfileReport", new { id = reportId });
    }

    public async Task<IActionResult> EditIndustryProfileBaseAdmin(int id, int branchId)
    {
      var report = await _applicationLayer.GetIndustryProfileReport(id);

      var companyIpReportViewModel = new CompanyIndustryProfileViewModel
      {
        MemberBranchId = report.MemberBranchId,
        MemberBranch = report.MemberBranch,
        MemberId = report.MemberId,
        MemberName = report.Member, //TODO: rename this
        IndustryProfileReportId = report.Id,
        ReportYear = report.ReportYear.ToString(),
        MemberTypeId = report.MemberTypeId,
        MemberType = report.MemberType,
        CreatedByUser = report.CreatedBy,
        MemberIpAdminItems = await _applicationLayer.GetMemberIpAdminItems(IndustryProfileDisplayType.Other,
          report.MemberId, report.MemberBranchId, report.Id, report.MemberTypeId).ConfigureAwait(false),
      };

      return View(companyIpReportViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> EditIndustryProfileBaseAdmin(IFormCollection collection)
    {
      var reportId = Convert.ToInt32(collection["IndustryProfileReportId"]);
      var memberId = Convert.ToInt32(collection["MemberId"]);
      var memberBranchId = Convert.ToInt32(collection["MemberBranchId"]);
      var memberTypeId = Convert.ToInt32(collection["MemberTypeId"]);
      var memberAdminItems = await _applicationLayer.GetMemberIpAdminItems(IndustryProfileDisplayType.Other,
        memberId, memberBranchId, reportId, memberTypeId).ConfigureAwait(false);

      foreach (var item in memberAdminItems)
      {
        decimal.TryParse(collection[$"IPAdminItem_{item.Id}"], out decimal newValue);
        item.Value = newValue;
        await _applicationLayer.SaveOrUpdateIpAdminItem(item.Id, newValue);
      }

      return RedirectToAction("Details", "IndustryProfileReport", new { id = reportId });
    }

    public async Task<IActionResult> EditIndustryProfileAdditional(int id, int branchId)
    {
      var report = await _applicationLayer.GetIndustryProfileReport(id);

      var companyIpReportViewModel = new CompanyIndustryProfileViewModel
      {
        MemberBranchId = report.MemberBranchId,
        MemberBranch = report.MemberBranch,
        MemberId = report.MemberId,
        MemberName = report.Member, //TODO: rename this
        IndustryProfileReportId = report.Id,
        ReportYear = report.ReportYear.ToString(),
        MemberTypeId = report.MemberTypeId,
        MemberType = report.MemberType,
        CreatedByUser = report.CreatedBy,
        MemberIpAdditionalItems = await _applicationLayer.GetMemberIpAdditionalItems(IndustryProfileDisplayType.Other,
          report.MemberId, report.MemberBranchId, report.Id, report.MemberTypeId).ConfigureAwait(false),
      };

      return View(companyIpReportViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> EditIndustryProfileAdditional(IFormCollection collection)
    {
      var reportId = Convert.ToInt32(collection["IndustryProfileReportId"]);
      var memberId = Convert.ToInt32(collection["MemberId"]);
      var memberBranchId = Convert.ToInt32(collection["MemberBranchId"]);
      var memberTypeId = Convert.ToInt32(collection["MemberTypeId"]);
      var memberAdditionalItems = await _applicationLayer.GetMemberIpAdditionalItems(IndustryProfileDisplayType.Other,
        memberId, memberBranchId, reportId, memberTypeId).ConfigureAwait(false);

      foreach (var item in memberAdditionalItems)
      {
        decimal.TryParse(collection[$"IPAdditionalItem_{item.Id}"], out decimal newValue);
        item.Value = newValue;
        await _applicationLayer.SaveOrUpdateIpAdditionalItem(item.Id, newValue).ConfigureAwait(false);
      }

      return RedirectToAction("Details", "IndustryProfileReport", new { id = reportId });
    }

    public async Task<IActionResult> EditIndustryCost(int id, int branchId)
    {
      var report = await _applicationLayer.GetIndustryProfileReport(id);

      CompanyIndustryProfileViewModel companyIpReportViewModel = new CompanyIndustryProfileViewModel
      {
        MemberBranchId = report.MemberBranchId,
        MemberBranch = report.MemberBranch,
        MemberId = report.MemberId,
        MemberName = report.Member, //TODO: rename this
        IndustryProfileReportId = report.Id,
        ReportYear = report.ReportYear.ToString(),
        MemberTypeId = report.MemberTypeId,
        MemberType = report.MemberType,
        CreatedByUser = report.CreatedBy,
        MemberRunningCosts = await _applicationLayer.GetMemberRunningCosts(IndustryProfileDisplayType.Other, report.MemberId, report.MemberBranchId, report.Id, report.MemberTypeId).ConfigureAwait(false),
      };

      return View(companyIpReportViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> EditIndustryCost(IFormCollection collection)
    {
      var reportId = Convert.ToInt32(collection["IndustryProfileReportId"]);
      var memberId = Convert.ToInt32(collection["MemberId"]);
      var memberBranchId = Convert.ToInt32(collection["MemberBranchId"]);
      var memberTypeId = Convert.ToInt32(collection["MemberTypeId"]);
      var memberRunningCosts = await _applicationLayer.GetMemberRunningCosts(IndustryProfileDisplayType.Other,
        memberId, memberBranchId, reportId, memberTypeId).ConfigureAwait(false);
      var runningCostsGroups = await _industryProfileLayer.GetRunningCostsGroups(false);

      foreach (var item in memberRunningCosts)
      {
        if (runningCostsGroups.First(x => x.Id == item.GroupId).ProductionEnabled)
        {
          decimal.TryParse(collection[$"RunningCostGroupProduction_{item.Id}"], out decimal newValue);
          item.ProductionValue = newValue;
          await _applicationLayer.SaveOrUpdateRunningCostItem(item.Id, 0, 0, 0, CostingGroup.ProductionValue, newValue)
            .ConfigureAwait(false);

          foreach (var subItem in item.SubItems.Where(x => x.ProductionEnabled))
          {
            decimal.TryParse(collection[$"RunningCostItemProduction_{subItem.GroupId}_{subItem.ItemId}"], out decimal newValue1);
            await _applicationLayer.SaveOrUpdateRunningCostItem(subItem.Id, subItem.GroupId, (int)subItem.ItemId!, subItem.IndustryProfileReportId, CostingGroup.ProductionValue, newValue1)
              .ConfigureAwait(false);
          }
        }

        if (runningCostsGroups.First(x => x.Id == item.GroupId).SalesEnabled)
        {
          decimal.TryParse(collection[$"RunningCostGroupSales_{item.Id}"], out decimal newValue);
          item.SalesValue = newValue;
          await _applicationLayer.SaveOrUpdateRunningCostItem(item.Id, 0, 0, 0, CostingGroup.SalesValue, newValue)
            .ConfigureAwait(false);

          foreach (var subItem in item.SubItems.Where(x => x.SalesEnabled))
          {
            decimal.TryParse(collection[$"RunningCostItemSales_{subItem.GroupId}_{subItem.ItemId}"], out decimal newValue1);
            await _applicationLayer.SaveOrUpdateRunningCostItem(subItem.Id, subItem.GroupId, (int)subItem.ItemId!, subItem.IndustryProfileReportId, CostingGroup.SalesValue, newValue1)
              .ConfigureAwait(false);
          }
        }

        if (runningCostsGroups.First(x => x.Id == item.GroupId).AdministrationEnabled)
        {
          decimal.TryParse(collection[$"RunningCostGroupAdministration_{item.Id}"], out decimal newValue);
          item.AdministrationValue = newValue;
          await _applicationLayer.SaveOrUpdateRunningCostItem(item.Id, 0, 0, 0, CostingGroup.AdministrationValue, newValue)
            .ConfigureAwait(false);

          foreach (var subItem in item.SubItems.Where(x => x.AdministrationEnabled))
          {
            decimal.TryParse(collection[$"RunningCostItemAdministration_{subItem.GroupId}_{subItem.ItemId}"], out decimal newValue1);
            await _applicationLayer.SaveOrUpdateRunningCostItem(subItem.Id, subItem.GroupId, (int)subItem.ItemId!, subItem.IndustryProfileReportId, CostingGroup.AdministrationValue, newValue1)
              .ConfigureAwait(false);
          }
        }

        if (runningCostsGroups.First(x => x.Id == item.GroupId).RoadTransportEnabled)
        {
          decimal.TryParse(collection[$"RunningCostGroupRoadTransport_{item.Id}"], out decimal newValue);
          item.RoadTransportValue = newValue;
          await _applicationLayer.SaveOrUpdateRunningCostItem(item.Id, 0, 0, 0, CostingGroup.RoadTransportValue, newValue)
            .ConfigureAwait(false);

          foreach (var subItem in item.SubItems.Where(x => x.RoadTransportEnabled))
          {
            decimal.TryParse(collection[$"RunningCostItemRoadTransport_{subItem.GroupId}_{subItem.ItemId}"], out decimal newValue1);
            await _applicationLayer.SaveOrUpdateRunningCostItem(subItem.Id, subItem.GroupId, (int)subItem.ItemId!, subItem.IndustryProfileReportId, CostingGroup.RoadTransportValue, newValue1)
              .ConfigureAwait(false);
          }
        }
      }

      return RedirectToAction("Details", "IndustryProfileReport", new { id = reportId });
    }

    public async Task<IActionResult> EditIndustryCostFleet(int id, int branchId)
    {
      var report = await _applicationLayer.GetIndustryProfileReport(id);

      CompanyIndustryProfileViewModel companyIpReportViewModel = new CompanyIndustryProfileViewModel
      {
        MemberBranchId = report.MemberBranchId,
        MemberBranch = report.MemberBranch,
        MemberId = report.MemberId,
        MemberName = report.Member, //TODO: rename this
        IndustryProfileReportId = report.Id,
        ReportYear = report.ReportYear.ToString(),
        MemberTypeId = report.MemberTypeId,
        MemberType = report.MemberType,
        CreatedByUser = report.CreatedBy,
        MemberRunningCostsFleet = await _applicationLayer.GetMemberRunningCostsFleet(IndustryProfileDisplayType.Other, report.MemberId, report.MemberBranchId, report.Id, report.MemberTypeId),
      };

      return View(companyIpReportViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> EditIndustryCostFleet(IFormCollection collection)
    {
      var reportId = Convert.ToInt32(collection["IndustryProfileReportId"]);
      var memberId = Convert.ToInt32(collection["MemberId"]);
      var memberBranchId = Convert.ToInt32(collection["MemberBranchId"]);
      var memberTypeId = Convert.ToInt32(collection["MemberTypeId"]);
      var memberRunningCostFleet = await _applicationLayer.GetMemberRunningCostsFleet(IndustryProfileDisplayType.Other,
        memberId, memberBranchId, reportId, memberTypeId).ConfigureAwait(false);

      foreach (var item in memberRunningCostFleet)
      {
        decimal.TryParse(collection[$"RunningCostRentalFleet_{item.Id}"], out decimal newValue);
        item.Value = newValue;
        await _applicationLayer.SaveOrUpdateRunningCostFleetItem(item.Id, newValue).ConfigureAwait(false);
      }

      return RedirectToAction("Details", "IndustryProfileReport", new { id = reportId });
    }

    public async Task<IActionResult> EditIndustryRunningCostSundries(int id, int branchId)
    {
      var report = await _applicationLayer.GetIndustryProfileReport(id);

      CompanyIndustryProfileViewModel companyIpReportViewModel = new()
      {
        MemberBranchId = report.MemberBranchId,
        MemberBranch = report.MemberBranch,
        MemberId = report.MemberId,
        MemberName = report.Member, //TODO: rename this
        IndustryProfileReportId = report.Id,
        ReportYear = report.ReportYear.ToString(),
        MemberTypeId = report.MemberTypeId,
        MemberType = report.MemberType,
        CreatedByUser = report.CreatedBy,
        MemberRunningCostsSundries = await _applicationLayer.GetMemberRunningCostsSundries(IndustryProfileDisplayType.Other, report.MemberId, report.MemberBranchId, report.Id, report.MemberTypeId),
      };

      return View(companyIpReportViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> EditIndustryRunningCostSundries(IFormCollection collection)
    {
      var reportId = Convert.ToInt32(collection["IndustryProfileReportId"]);
      var memberId = Convert.ToInt32(collection["MemberId"]);
      var memberBranchId = Convert.ToInt32(collection["MemberBranchId"]);
      var memberTypeId = Convert.ToInt32(collection["MemberTypeId"]);
      var memberRunningCostFSundries = await _applicationLayer.GetMemberRunningCostsSundries(IndustryProfileDisplayType.Other,
        memberId, memberBranchId, reportId, memberTypeId).ConfigureAwait(false);
      var groups = await _industryProfileLayer.GetRunningCostSundryExpense(false).ConfigureAwait(false);

      foreach (var item in memberRunningCostFSundries)
      {
        if (item.ProductionEnabled)
        {
          decimal.TryParse(collection[$"RunningCostSundryExpenseProduction_{item.Id}"], out decimal newValue);
          await _applicationLayer.SaveOrUpdateRunningCostSundryItem(item.Id, CostingGroup.ProductionValue, newValue).ConfigureAwait(false);
        }

        if (item.MarketingEnabled)
        {
          decimal.TryParse(collection[$"RunningCostSundryExpenseMarketing_{item.Id}"], out decimal newValue);
          await _applicationLayer.SaveOrUpdateRunningCostSundryItem(item.Id, CostingGroup.SalesValue, newValue).ConfigureAwait(false);
        }

        if (item.AdministrationEnabled)
        {
          decimal.TryParse(collection[$"RunningCostSundryExpenseAdministration_{item.Id}"], out decimal newValue);
          await _applicationLayer.SaveOrUpdateRunningCostSundryItem(item.Id, CostingGroup.AdministrationValue, newValue).ConfigureAwait(false);
        }

        if (item.RoadTransportEnabled)
        {
          decimal.TryParse(collection[$"RunningCostSundryExpenseRoadTransport_{item.Id}"], out decimal newValue);
          await _applicationLayer.SaveOrUpdateRunningCostSundryItem(item.Id, CostingGroup.RoadTransportValue, newValue).ConfigureAwait(false);
        }
      }

      return RedirectToAction("Details", "IndustryProfileReport", new { id = reportId });
    }

    public async Task<IActionResult> EditPcbCost(int id, PcbItem pcbItem, int branchId)
    {
      var report = await _applicationLayer.GetIndustryProfileReport(id);

      var memberPcbCostItems = await _applicationLayer.GetMemberPcbCostItems(pcbItem, IndustryProfileDisplayType.Other, report.MemberId,
        report.MemberBranchId, report.Id, report.MemberTypeId).ConfigureAwait(false);

      CompanyIndustryProfileViewModel companyIpReportViewModel = new CompanyIndustryProfileViewModel
      {
        MemberBranchId = report.MemberBranchId,
        MemberBranch = report.MemberBranch,
        MemberId = report.MemberId,
        MemberName = report.Member, //TODO: rename this
        IndustryProfileReportId = report.Id,
        ReportYear = report.ReportYear.ToString(),
        MemberTypeId = report.MemberTypeId,
        MemberType = report.MemberType,
        CreatedByUser = report.CreatedBy,
        MemberPcbCostItems = memberPcbCostItems.ToList(),
      };

      return View(companyIpReportViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> EditPcbCost(IFormCollection collection)
    {
      var reportId = Convert.ToInt32(collection["IndustryProfileReportId"]);
      var memberId = Convert.ToInt32(collection["MemberId"]);
      var memberBranchId = Convert.ToInt32(collection["MemberBranchId"]);
      var memberTypeId = Convert.ToInt32(collection["MemberTypeId"]);
      var pcbMainGroupId = Convert.ToInt32(collection["PcbMainGroupId"]);
      var memberPcbCostItems = await _applicationLayer.GetMemberPcbCostItems((PcbItem)pcbMainGroupId, IndustryProfileDisplayType.Other,
        memberId, memberBranchId, reportId, memberTypeId).ConfigureAwait(false);

      foreach (var item in memberPcbCostItems)
      {
        decimal.TryParse(collection[$"PCBCostItem_{item.Id}"], out decimal newValue);
        item.Value = newValue;
        await _applicationLayer.SaveOrUpdatePcbCostItem(item.Id, newValue).ConfigureAwait(false);
      }

      return RedirectToAction("Details", "IndustryProfileReport", new { id = reportId });
    }

    public async Task<IActionResult> Submit(int industryProfileReportId)
    {
      var result = await _industryProfileLayer.UpdateReportStatus(industryProfileReportId, true);

      if (result.Success)
      {
        _notyf.Success("Industry Profile Report submitted successfully");
      }
      else
      {
        _notyf.Error($"Industry Profile Report not submitted: {result.ErrorMessage}");
      }

      return RedirectToAction("Details", new { id = industryProfileReportId });
    }

    public async Task<IActionResult> Unsubmit(int industryProfileReportId)
    {
      var result = await _industryProfileLayer.UpdateReportStatus(industryProfileReportId, true);

      if (result.Success)
      {
        _notyf.Success("Industry Profile Report unsubmitted successfully");
      }
      else
      {
        _notyf.Error($"Industry Profile Report not unsubmitted: {result.ErrorMessage}");
      }

      return RedirectToAction("Details", new { id = industryProfileReportId });
    }

    public async Task<IActionResult> DeleteReport(int industryProfileReportId)
    {
      var result = await _industryProfileLayer.DeleteReport(industryProfileReportId);

      if (result.Succeeded)
      {
        _notyf.Success("Industry Profile Report deleted successfully");
      }
      else
      {
        _notyf.Error($"Industry Profile Report could not be deleted : {result.ExceptionMessage}");
      }

      return RedirectToAction("Index", "IndustryProfileReport");
    }

    public async Task<IActionResult> SummaryReport()
    {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> SummaryReport(ReportCriteriaModel reportCriteria)
    {
      var report = await _industryProfileLayer.GenerateIndustryProfileExcelReport(reportCriteria).ConfigureAwait(false);
      return File(report, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "IndustryProfileReport.xlsx");
    }
  }
}