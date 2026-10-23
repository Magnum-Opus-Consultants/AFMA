namespace AFMA.Stats.Admin.Controllers
{
  using AFMA.Infrastructure.Settings;
  using Alpha.Services.Models.Data.PageData;
  using ApplicationLayer.Helpers;
  using ApplicationLayer.Interface;
  using ApplicationLayer.Model;
  using AspNetCoreHero.ToastNotification.Abstractions;
  using Microsoft.AspNetCore.Authorization;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.Extensions.Options;
  using OfficeOpenXml;
  using System.Drawing;

  [Authorize(Roles = "Admin")]
  public class FeedsalesReportController(
    ILogger<FeedsalesReportController> logger,
    IApplicationLayer applicationLayer,
    IFeedsalesLayer feedsalesLayer,
    INotyfService notyf,
    IOptions<ApplicationSettings> applicationSettings,
    INotificationService notificationService)
    : Controller
  {
    private readonly ILogger<FeedsalesReportController> _logger = logger;

    public async Task<IActionResult> Dashboard()
    {
      return View();
    }

    public async Task<IActionResult> GenerateDashboard()
    {
      var results = await applicationLayer.GenerateFeedsalesDashboard().ConfigureAwait(false);
      return Json(results);
    }

    public async Task<IActionResult> Index(int? page, int? filterMemberId, int? filterMemberBranchId, bool? filterIsSubmitted)
    {
      var pageNo = page ?? 1;
      var results = await applicationLayer.GetFeedsalesReportsLookup(
        new AlphaPageRequest(pageNo, applicationSettings.Value.PageSize, "Id", true), filterMemberId,
        filterMemberBranchId, filterIsSubmitted);
      return View(results);
    }

    public async Task<IActionResult> Details(int id)
    {
      var currentReport = await applicationLayer.GetFeedsalesReport(id);
      var previousReport = await applicationLayer.GetPreviousFeedsalesReport(currentReport.MemberBranchId, currentReport.InfoFor, 0, 0);
      currentReport.ReportItemsPreviousMonth = previousReport.ReportItems;
      return View(currentReport);
    }

    public async Task<IActionResult> Edit(int id)
    {
      var report = await applicationLayer.GetFeedsalesReport(id);
      //ViewBag.ProductGroups = _applicationLayer.GetFinishedProductGroups().Result.OrderBy(a => a.DisplayOrder)
      //  .ThenBy(n => n.Name);
      return View(report);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int feedsalesReportId, string differenceReason, string comment)
    {
      var referer = Request.Headers["Referer"].ToString();
      var result = await applicationLayer.EditFeedsalesReport(feedsalesReportId, differenceReason, comment)
        .ConfigureAwait(false);

      if (result.Success)
      {
        notyf.Success("Feedsales Report updated successfully");
      }
      else
      {
        notyf.Error($"Feedsales Report could not be updated: {result.ErrorMessage}");
      }

      return RedirectToAction("Details", "FeedsalesReport", new { area = "", Id = feedsalesReportId });
      //return Redirect($"{referer}#summary");
    }

    [HttpPost]
    public async Task<IActionResult> AddFinishedProduct(FeedsalesReportModel feedsalesReportModel)
    {
      var referer = Request.Headers["Referer"].ToString();

      var feedsalesReportItem = new FeedsalesReportItemModel
      {
        FeedsalesReportId = feedsalesReportModel.Id,
        MemberAccountId = feedsalesReportModel.MemberAccountId ?? await applicationLayer.GetCurrentMemberAccountId().ConfigureAwait(false),
        MemberBranchId = feedsalesReportModel.MemberBranchId,
        MemberId = feedsalesReportModel.MemberId,
        MaterialId = null,
        ProductId = feedsalesReportModel.TProductId,
        Value = feedsalesReportModel.TAmount,
        Other = feedsalesReportModel.TPOther,
      };

      var result = await applicationLayer.SaveOrUpdate(feedsalesReportItem)
        .ConfigureAwait(false);

      if (result.Success)
      {
        notyf.Success("Finished Product added successfully");
      }
      else
      {
        notyf.Error($"Finished Product could not bed added: {result.ErrorMessage}");
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
        MemberAccountId = feedsalesReportModel.MemberAccountId ?? await applicationLayer.GetCurrentMemberAccountId().ConfigureAwait(false),
        MemberBranchId = feedsalesReportModel.MemberBranchId,
        MemberId = feedsalesReportModel.MemberId,
        MaterialId = feedsalesReportModel.TMaterialId,
        ProductId = null,
        Value = feedsalesReportModel.TAmount,
        Other = feedsalesReportModel.TMOther,
      };

      var result = await applicationLayer.SaveOrUpdate(feedsalesReportItem)
        .ConfigureAwait(false);

      if (result.Success)
      {
        notyf.Success("Raw Material added successfully");
      }
      else
      {
        notyf.Error($"Raw Material could not be added: {result.ErrorMessage}");
      }

      return Redirect($"{referer}#rm");
    }

    public async Task<IActionResult> DeleteFeedsalesReportItem(int tReportItemId, int feedsalesReportId, string tReportItemType)
    {
      var referer = Request.Headers["Referer"].ToString();

      var result = await applicationLayer.DeleteFeedsalesReportItem(tReportItemId);

      if (result)
      {
        notyf.Success("Report Item deleted successfully");
      }
      else
      {
        notyf.Error("Report Item could not be deleted");
      }

      return Redirect($"{referer}#{tReportItemType}");
    }

    public async Task<IActionResult> DeleteFeedsalesReport(int feedsalesReportId)
    {
      var result = await applicationLayer.DeleteFeedsalesReport(feedsalesReportId);

      if (result.Succeeded)
      {
        notyf.Success("Feedsales Report deleted successfully");
      }
      else
      {
        notyf.Error($"Feedsales Report could not be deleted : {result.ExceptionMessage}");
      }

      return RedirectToAction("Index", "FeedsalesReport");
    }

    public async Task<IActionResult> Submissions(int? filterYear, int? filterMonth)
    {
      var result =
        await applicationLayer.GetFeedsalesSubmissions(new DateTime(filterYear ?? DateTime.Now.Year, filterMonth ?? DateTime.Now.Month, 1));
      return View(result);
    }

    public async Task<IActionResult> TotalFeedsalesAdminReportByProvince()
    {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> TotalFeedsalesAdminReportByProvince(ReportCriteriaModel reportCriteria)
    {
      System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
      var maroonHeader = ColorTranslator.FromHtml("#c0504d");
      var blueHeader = ColorTranslator.FromHtml("#4f81bd");
      var silverHeader = ColorTranslator.FromHtml("#999999");
      var greenHeader = ColorTranslator.FromHtml("#82bc4f");
      var startRow = 0;
      var endRow = 0;

      var feedsalesProvincesAsync = await applicationLayer.GetProvinces().ConfigureAwait(false);
      var feedsalesProvinces = feedsalesProvincesAsync.OrderBy(p => p.Description).ToList();

      var productGroupsAsync = await feedsalesLayer.GetFinishedProductGroups().ConfigureAwait(false);
      var productGroups = productGroupsAsync.Where(x => x.VisibleToMembers && !x.HideOnFeedsales);
      //var rawMaterials = new Service.MaterialService(new AFMA.Data.AFMAContext()).GetAll().OrderBy(o => o.Code);

      var finishedProductsAsync = await applicationLayer.GetFinishedProducts().ConfigureAwait(false);
      var finishedProducts = finishedProductsAsync.Where(x => x.Active && x.VisibleToMembers).OrderBy(fp => fp.Name).ToList();

      var rowIndex = 1;
      var colIndex = 1;
      var startCol = 1;
      var endCol = 1;

      using (ExcelPackage p = new ExcelPackage())
      {
        p.Workbook.Properties.Author = "AFMA";
        p.Workbook.Properties.Title = "AFMA Total Province Feedsales Report";
        p.Workbook.Properties.Company = "www.afma.co.za";

        p.Workbook.Worksheets.Add("Provincial Statistics");
        ExcelWorksheet ws = p.Workbook.Worksheets[0];
        ws.Name = "Provincial Statistics";

        //** SET DEFAULT COLUMN WIDTH'S AND HEIGHT's
        ws.Column(1).Width = 12;
        ws.Column(2).Width = 35;
        ws.Row(1).Height = 30;

        colIndex = 1;
        rowIndex = 1;

        ws.Cells[rowIndex, colIndex, rowIndex, colIndex + 1].Merge = true;
        var row1col1 = ws.Cells[rowIndex, colIndex];
        ws.Row(rowIndex).Height = 30;
        row1col1.Value =
          $"ANIMAL FEED SALES PER PROVINCE {reportCriteria.StartDate:MMM yyyy} - {reportCriteria.EndDate:MMM yyyy}";
        rowIndex++;
        colIndex = 1;

        //header row with dates
        ws.Cells[rowIndex, colIndex, rowIndex, colIndex + 1].Merge = true;
        var row2col1 = ws.Cells[rowIndex, colIndex];
        row2col1.Value = "All Members";
        colIndex++;
        colIndex++;

        //total header
        ws.Column(colIndex).Width = 12;
        var header_total = ws.Cells[rowIndex, colIndex];
        header_total.Style.Font.Color.SetColor(Color.White);
        header_total.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        header_total.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        header_total.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        header_total.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        header_total.Value = "TOTAL";
        colIndex++;

        //months headers
        foreach (var province in feedsalesProvinces)
        {
          ws.Column(colIndex).Width = 15;
          var header_province = ws.Cells[rowIndex, colIndex];
          header_province.Style.Font.Color.SetColor(Color.White);
          header_province.Style.WrapText = true;
          header_province.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
          header_province.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
          header_province.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          header_province.Style.Fill.BackgroundColor.SetColor(maroonHeader);
          header_province.Value = province.Description;
          colIndex++;
        }

        rowIndex++;
        colIndex = 1;

        foreach (var productGroup in productGroups.OrderBy(x => x.Code))
        {
          var product_group_code = ws.Cells[rowIndex, colIndex];
          product_group_code.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          product_group_code.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
          product_group_code.Style.Font.Color.SetColor(Color.White);
          product_group_code.Style.Fill.BackgroundColor.SetColor(silverHeader);
          product_group_code.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          product_group_code.Value = productGroup.Code;

          colIndex++;

          var product_group_name = ws.Cells[rowIndex, colIndex];
          product_group_name.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          product_group_name.Style.Fill.BackgroundColor.SetColor(blueHeader);
          product_group_name.Style.Font.Color.SetColor(Color.White);
          product_group_name.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          product_group_name.Value = productGroup.Name;
          colIndex++;

          endCol = colIndex + feedsalesProvinces.Count();

          var provinceAllTotal = ws.Cells[rowIndex, colIndex];
          provinceAllTotal.Style.Font.Color.SetColor(Color.White);
          provinceAllTotal.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          provinceAllTotal.Style.Fill.BackgroundColor.SetColor(blueHeader);
          provinceAllTotal.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          provinceAllTotal.Formula = $"=SUM({ws.Cells[rowIndex, colIndex + 1].Address}:{ws.Cells[rowIndex, endCol].Address})";

          colIndex++;

          foreach (var province in feedsalesProvinces)
          {
            var provinceTotal = ws.Cells[rowIndex, colIndex];
            provinceTotal.Style.Font.Color.SetColor(Color.White);
            provinceTotal.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            provinceTotal.Style.Fill.BackgroundColor.SetColor(blueHeader);
            provinceTotal.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            try
            {
              var totalValue = applicationLayer.GetTotalFeedsalesProductGroupByProvinceByYear(reportCriteria.StartDate,
                reportCriteria.EndDate, province.Id, productGroup.Id, 1);
              provinceTotal.Value = totalValue;

            }
            catch
            {
              provinceTotal.Value = 0;
            }
            colIndex++;

          }
          rowIndex++;
          colIndex = 1;

          //products
          foreach (var product in finishedProducts.Where(x => x.ProductGroupId == productGroup.Id))
          {
            var productCode = ws.Cells[rowIndex, colIndex];
            productCode.Style.Font.Color.SetColor(Color.Black);
            productCode.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            productCode.Style.Fill.BackgroundColor.SetColor(Color.White);
            productCode.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            productCode.Value = product.Code;

            colIndex++;

            var productName = ws.Cells[rowIndex, colIndex];
            productName.Style.Font.Color.SetColor(Color.Black);
            productName.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            productName.Style.Fill.BackgroundColor.SetColor(Color.White);
            productName.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            productName.Value = product.Name;

            colIndex++;
            endCol = colIndex + feedsalesProvinces.Count();

            var provinceTotal = ws.Cells[rowIndex, colIndex];
            provinceTotal.Style.Font.Color.SetColor(Color.Black);
            provinceTotal.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            provinceTotal.Style.Fill.BackgroundColor.SetColor(Color.White);
            provinceTotal.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            provinceTotal.Formula = $"=SUM({ws.Cells[rowIndex, colIndex + 1].Address}:{ws.Cells[rowIndex, endCol].Address})";

            colIndex++;

            foreach (var province in feedsalesProvinces)
            {
              var provinceTotal1 = ws.Cells[rowIndex, colIndex];
              provinceTotal1.Style.Font.Color.SetColor(Color.Black);
              provinceTotal1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
              provinceTotal1.Style.Fill.BackgroundColor.SetColor(Color.White);
              provinceTotal1.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              try
              {
                var totalValue = applicationLayer.GetTotalFeedsalesProductByProvinceByYear(reportCriteria.StartDate,
                  reportCriteria.EndDate, province.Id, product.Id, 1);
                provinceTotal1.Value = totalValue;

              }
              catch
              {
                provinceTotal1.Value = 0;
              }
              colIndex++;
            }

            colIndex = 1;
            rowIndex++;
          }

        }

        colIndex = 1;


        //BEGIN TOTAL SALES CURRENT YEAR
        ws.Cells[rowIndex, colIndex, rowIndex, colIndex + 1].Merge = true;
        var total_row_1 = ws.Cells[rowIndex, colIndex];
        total_row_1.Style.Font.Color.SetColor(Color.White);
        total_row_1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        total_row_1.Style.Fill.BackgroundColor.SetColor(blueHeader);
        total_row_1.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        total_row_1.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        total_row_1.Value = "TOTAL SALES";
        colIndex++;
        colIndex++;

        endCol = colIndex + feedsalesProvinces.Count();
        var grand_total = ws.Cells[rowIndex, colIndex];
        grand_total.Style.Font.Color.SetColor(Color.White);
        grand_total.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        grand_total.Style.Fill.BackgroundColor.SetColor(blueHeader);
        grand_total.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        grand_total.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
        grand_total.Style.Numberformat.Format = "#,#";
        grand_total.Formula = $"=SUM({ws.Cells[rowIndex, colIndex + 1].Address}:{ws.Cells[rowIndex, endCol].Address})";

        colIndex++;

        foreach (var province in feedsalesProvinces)
        {
          var total_row = ws.Cells[rowIndex, colIndex];
          total_row.Style.Font.Color.SetColor(Color.White);
          total_row.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          total_row.Style.Fill.BackgroundColor.SetColor(blueHeader);
          total_row.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          total_row.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
          total_row.Style.Numberformat.Format = "#,#";
          try
          {
            var totalValue =
              applicationLayer.GetTotalFeedsalesProductGroupByProvinceByYearTotal(reportCriteria.StartDate,
                reportCriteria.EndDate, province.Id, 1);
            total_row.Value = totalValue;
          }
          catch
          {
            total_row.Value = 0;
          }
          colIndex++;
        }

        //BEGIN PERCENTAGES CURRENT YEAR
        colIndex = 1;
        rowIndex++;

        ws.Cells[rowIndex, colIndex, rowIndex, colIndex + 1].Merge = true;
        var total_row_per_text = ws.Cells[rowIndex, colIndex];
        total_row_per_text.Style.Font.Color.SetColor(Color.White);
        total_row_per_text.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        total_row_per_text.Style.Fill.BackgroundColor.SetColor(blueHeader);
        total_row_per_text.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        total_row_per_text.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        total_row_per_text.Value = "PROVINCIAL SALES AS % OF TOTAL SALES";
        colIndex++;
        colIndex++;
        colIndex++;

        foreach (var province in feedsalesProvinces)
        {
          var total_row_per = ws.Cells[rowIndex, colIndex];
          total_row_per.Style.Font.Color.SetColor(Color.White);
          total_row_per.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          total_row_per.Style.Fill.BackgroundColor.SetColor(blueHeader);
          total_row_per.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          total_row_per.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
          total_row_per.Style.Numberformat.Format = "#,#0.00%";
          total_row_per.Formula = $"=({ws.Cells[rowIndex - 1, colIndex].Address}/{ws.Cells[rowIndex - 1, 3].Address})";
          colIndex++;
        }

        rowIndex++;
        colIndex = 1;


        //BEGIN PREVIOUS YEAR SALES
        ws.Cells[rowIndex, colIndex, rowIndex, colIndex + 1].Merge = true;
        var total_sales_previous = ws.Cells[rowIndex, colIndex];
        total_sales_previous.Style.Font.Color.SetColor(Color.White);
        total_sales_previous.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        total_sales_previous.Style.Fill.BackgroundColor.SetColor(blueHeader);
        total_sales_previous.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        total_sales_previous.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        total_sales_previous.Value = "TOTAL SALES - Previous Year";
        colIndex++;
        colIndex++;

        endCol = colIndex + feedsalesProvinces.Count();
        var grand_total_previous = ws.Cells[rowIndex, colIndex];
        grand_total_previous.Style.Font.Color.SetColor(Color.White);
        grand_total_previous.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        grand_total_previous.Style.Fill.BackgroundColor.SetColor(blueHeader);
        grand_total_previous.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        grand_total_previous.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
        grand_total_previous.Style.Numberformat.Format = "#,#";
        grand_total_previous.Formula = $"=SUM({ws.Cells[rowIndex, colIndex + 1].Address}:{ws.Cells[rowIndex, endCol].Address})";

        colIndex++;

        foreach (var province in feedsalesProvinces)
        {
          var total_row_value_previous = ws.Cells[rowIndex, colIndex];
          total_row_value_previous.Style.Font.Color.SetColor(Color.White);
          total_row_value_previous.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          total_row_value_previous.Style.Fill.BackgroundColor.SetColor(blueHeader);
          total_row_value_previous.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          total_row_value_previous.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
          total_row_value_previous.Style.Numberformat.Format = "#,#";
          try
          {
            DateTime sd = reportCriteria.StartDate.AddYears(-1);
            DateTime ed = reportCriteria.EndDate.AddYears(-1);

            var totalValue =
              applicationLayer.GetTotalFeedsalesProductGroupByProvinceByYearTotal(sd, ed, province.Id, 1);
            total_row_value_previous.Value = totalValue;
          }
          catch
          {
            total_row_value_previous.Value = 0;
          }
          colIndex++;
        }

        colIndex = 1;
        rowIndex++;

        ws.Cells[rowIndex, colIndex, rowIndex, colIndex + 1].Merge = true;
        var total_row_per_prev_text = ws.Cells[rowIndex, colIndex];
        total_row_per_prev_text.Style.Font.Color.SetColor(Color.White);
        total_row_per_prev_text.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        total_row_per_prev_text.Style.Fill.BackgroundColor.SetColor(blueHeader);
        total_row_per_prev_text.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        total_row_per_prev_text.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        total_row_per_prev_text.Value = "PROVINCIAL SALES AS % OF TOTAL SALES";
        colIndex++;
        colIndex++;
        colIndex++;

        foreach (var province in feedsalesProvinces)
        {
          var total_row_per = ws.Cells[rowIndex, colIndex];
          total_row_per.Style.Font.Color.SetColor(Color.White);
          total_row_per.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          total_row_per.Style.Fill.BackgroundColor.SetColor(blueHeader);
          total_row_per.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          total_row_per.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
          total_row_per.Style.Numberformat.Format = "#,#0.00%";
          total_row_per.Formula = $"=({ws.Cells[rowIndex - 1, colIndex].Address}/{ws.Cells[rowIndex - 1, 3].Address})";
          colIndex++;
        }

        var bin = await p.GetAsByteArrayAsync();

        return File(bin, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "FeedsalesProvince.xlsx");
      }

    }

    public async Task<IActionResult> TotalFeedsalesAdminReportByMember()
    {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> TotalFeedsalesAdminReportByMember(ReportCriteriaModel reportCriteria)
    {
      var mfi = new System.Globalization.DateTimeFormatInfo();

      var reportMonths = GeneralHelpers.MonthsBetween(reportCriteria.StartDate, reportCriteria.EndDate).ToList();

      var maroonHeader = ColorTranslator.FromHtml("#c0504d");
      var blueHeader = ColorTranslator.FromHtml("#4f81bd");
      var silverHeader = ColorTranslator.FromHtml("#999999");
      var greenHeader = ColorTranslator.FromHtml("#82bc4f");
      var startRow = 0;
      var endRow = 0;

      var feedsalesMembersAsync = await applicationLayer.GetMemberBranchesSummaryByMemberType(1);
      var feedsalesMembers = feedsalesMembersAsync.OrderBy(fm => fm.Name).ToList();

      int rowIndex = 1;
      int colIndex = 1;

      using (ExcelPackage p = new ExcelPackage())
      {

        p.Workbook.Properties.Author = "AFMA";
        p.Workbook.Properties.Title = "AFMA Total Member Feedsales Report";
        p.Workbook.Properties.Company = "www.afma.co.za";

        p.Workbook.Worksheets.Add("Feedsales");
        p.Workbook.Worksheets.Add("Raw Materials");

        ExcelWorksheet ws = p.Workbook.Worksheets[0];
        ExcelWorksheet ws2 = p.Workbook.Worksheets[1];
        ws.Name = "Feedsales";
        ws2.Name = "Raw Materials";

        //** SET DEFAULT COLUMN WIDTH'S AND HEIGHT's
        ws.Column(1).Width = 12;
        ws.Column(2).Width = 35;
        ws.Row(2).Height = 30;

        ws2.Column(1).Width = 12;
        ws2.Column(2).Width = 35;
        ws2.Row(2).Height = 30;

        #region FEEDSALES WORKSHEET

        colIndex = 1;
        rowIndex = 1;

        ws.Cells[rowIndex, colIndex, rowIndex, colIndex + 1].Merge = true;
        var row1col1 = ws.Cells[rowIndex, colIndex];
        ws.Row(rowIndex).Height = 30;
        row1col1.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        row1col1.Value = "ANIMAL FEED SALES STATISTICS BY MEMBERS";
        rowIndex++;
        colIndex = 1;

        //header row with dates
        ws.Cells[rowIndex, colIndex, rowIndex, colIndex + 1].Merge = true;
        var row2col1 = ws.Cells[rowIndex, colIndex];
        row2col1.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        row2col1.Value = "FEED SALES - ALL MEMBERS";
        colIndex++;
        colIndex++;

        //months headers
        foreach (var reportDate in reportMonths)
        {
          ws.Column(colIndex).Width = 12;
          var row2col2 = ws.Cells[rowIndex, colIndex];
          var _dt = $"{mfi.GetMonthName(reportDate.Month).Substring(0, 3).ToUpper()} {reportDate.Year}";
          row2col2.Style.Font.Color.SetColor(Color.White);
          row2col2.Style.WrapText = true;
          row2col2.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
          row2col2.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
          row2col2.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          row2col2.Style.Fill.BackgroundColor.SetColor(maroonHeader);
          row2col2.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          row2col2.Value = _dt;
          colIndex++;
        }

        rowIndex++;
        startRow = 3;
        endRow = startRow + feedsalesMembers.Count();

        foreach (var branch in feedsalesMembers.OrderBy(x => x.Member))
        {
          colIndex = 1;
          var branchCode = ws.Cells[rowIndex, colIndex];
          branchCode.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          branchCode.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
          branchCode.Style.Font.Color.SetColor(Color.White);
          branchCode.Style.Fill.BackgroundColor.SetColor(silverHeader);
          branchCode.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          branchCode.Value = branch.Id;

          colIndex++;

          var branchName = ws.Cells[rowIndex, colIndex];
          branchName.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          branchName.Style.Fill.BackgroundColor.SetColor(blueHeader);
          branchName.Style.Font.Color.SetColor(Color.White);
          branchName.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          branchName.Value = $"{branch.Member}: {branch.Name}";
          colIndex++;

          foreach (var reportDate in reportMonths)
          {
            //Logger.Write($"Getting report details for YEAR: {reportDate.Year}, MONTH: {reportDate.Month}, BRANCH: {branch.Id}");
            var memberTotal = ws.Cells[rowIndex, colIndex];
            memberTotal.Style.Font.Color.SetColor(Color.Black);
            memberTotal.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            memberTotal.Style.Fill.BackgroundColor.SetColor(Color.White);
            memberTotal.Style.Numberformat.Format = "#,#";
            memberTotal.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            try
            {
              var totalValue = applicationLayer.GetFeedsalesByMemberByDate(reportDate.Year, reportDate.Month, branch.Id);
              memberTotal.Value = totalValue;
            }
            catch
            {
              memberTotal.Value = 0;
            }

            colIndex++;
          }
          colIndex = 1;
          rowIndex++;
        }

        colIndex = 3;

        foreach (var reportDate in reportMonths)
        {
          var month_total = ws.Cells[rowIndex, colIndex];
          month_total.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          month_total.Style.Fill.BackgroundColor.SetColor(maroonHeader);
          month_total.Style.Font.Color.SetColor(Color.White);
          month_total.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          month_total.Style.Numberformat.Format = "#,#";
          month_total.Formula = $"=SUM({ws.Cells[startRow, colIndex].Address}:{ws.Cells[endRow - 1, colIndex].Address})";
          colIndex++;
        }

        #endregion

        #region RAW MATERIALS WORKSHEET

        colIndex = 1;
        rowIndex = 1;

        ws2.Cells[rowIndex, colIndex, rowIndex, colIndex + 1].Merge = true;
        var row1col1_2 = ws2.Cells[rowIndex, colIndex];
        ws2.Row(rowIndex).Height = 30;
        row1col1_2.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        row1col1_2.Value = "ANIMAL FEED SALES STATISTICS BY MEMBERS";
        rowIndex++;
        colIndex = 1;

        //header row with dates
        ws2.Cells[rowIndex, colIndex, rowIndex, colIndex + 1].Merge = true;
        var row2col1_2 = ws2.Cells[rowIndex, colIndex];
        row2col1_2.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        row2col1_2.Value = "RAW MATERIAL - ALL MEMBERS";
        colIndex++;
        colIndex++;

        //months headers
        foreach (var reportDate in reportMonths)
        {
          ws2.Column(colIndex).Width = 12;
          var row2col2_2 = ws2.Cells[rowIndex, colIndex];
          var _dt = $"{mfi.GetMonthName(reportDate.Month).Substring(0, 3).ToUpper()} {reportDate.Year}";
          row2col2_2.Style.Font.Color.SetColor(Color.White);
          row2col2_2.Style.WrapText = true;
          row2col2_2.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
          row2col2_2.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
          row2col2_2.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          row2col2_2.Style.Fill.BackgroundColor.SetColor(maroonHeader);
          row2col2_2.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          row2col2_2.Value = _dt;
          colIndex++;
        }

        rowIndex++;
        startRow = 3;
        endRow = startRow + feedsalesMembers.Count();

        foreach (var branch in feedsalesMembers.OrderBy(x => x.Member))
        {
          colIndex = 1;
          var branchCode2 = ws2.Cells[rowIndex, colIndex];
          branchCode2.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          branchCode2.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
          branchCode2.Style.Font.Color.SetColor(Color.White);
          branchCode2.Style.Fill.BackgroundColor.SetColor(silverHeader);
          branchCode2.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          branchCode2.Value = branch.Id;

          colIndex++;

          var branchName2 = ws2.Cells[rowIndex, colIndex];
          branchName2.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          branchName2.Style.Fill.BackgroundColor.SetColor(blueHeader);
          branchName2.Style.Font.Color.SetColor(Color.White);
          branchName2.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          branchName2.Value = $"{branch.Member}: {branch.Name}";
          colIndex++;

          foreach (var reportDate in reportMonths)
          {
            var memberTotal2 = ws2.Cells[rowIndex, colIndex];
            memberTotal2.Style.Font.Color.SetColor(Color.Black);
            memberTotal2.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            memberTotal2.Style.Fill.BackgroundColor.SetColor(Color.White);
            memberTotal2.Style.Numberformat.Format = "#,#";
            memberTotal2.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            try
            {
              var totalValue =
                applicationLayer.GetRawMaterialsByMemberByDate(reportDate.Year, reportDate.Month, branch.Id);
              memberTotal2.Value = totalValue;

            }
            catch
            {
              memberTotal2.Value = 0;
            }

            colIndex++;
          }
          colIndex = 1;
          rowIndex++;
        }

        colIndex = 3;

        foreach (var reportDate in reportMonths)
        {
          var month_total2 = ws2.Cells[rowIndex, colIndex];
          month_total2.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          month_total2.Style.Fill.BackgroundColor.SetColor(maroonHeader);
          month_total2.Style.Font.Color.SetColor(Color.White);
          month_total2.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          month_total2.Style.Numberformat.Format = "#,#";
          month_total2.Formula = $"=SUM({ws2.Cells[startRow, colIndex].Address}:{ws2.Cells[endRow - 1, colIndex].Address})";
          colIndex++;
        }

        #endregion

        var bin = await p.GetAsByteArrayAsync();

        return File(bin, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "FeedsalesMember.xlsx");
      }
    }

    public async Task<IActionResult> TotalFeedsalesAdminReport()
    {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> TotalFeedsalesAdminReport(ReportCriteriaModel reportCriteria)
    {
      var report = await feedsalesLayer.GenerateTotalFeedsalesReport(reportCriteria);
      return File(report, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Feedsales.xlsx");
    }

    public async Task<IActionResult> Submit(int feedsalesReportId)
    {
      var result = await applicationLayer.UpdateFeedsalesReportStatus(feedsalesReportId, true);

      if (result.Success)
      {
        notyf.Success("Feedsales Report submitted successfully");
      }
      else
      {
        notyf.Error($"Feedsales Report not submitted: {result.ErrorMessage}");
      }

      return RedirectToAction("Details", new { id = feedsalesReportId });
    }

    public async Task<IActionResult> Unsubmit(int feedsalesReportId)
    {
      var result = await applicationLayer.UpdateFeedsalesReportStatus(feedsalesReportId, false);

      if (result.Success)
      {
        notyf.Success("Feedsales Report unsubmitted successfully");
      }
      else
      {
        notyf.Error($"Feedsales Report not unsubmitted: {result.ErrorMessage}");
      }

      return RedirectToAction("Details", new { id = feedsalesReportId });
    }

    public async Task<IActionResult> SendReminder(int id, int reportMonth, int reportYear)
    {
      var result = await notificationService.SendFeedsalesSubmissionReminder(id);

      try
      {
        if (result)
        {
          notyf.Success("Feedsales submission reminder sent successfully");
        }
        else
        {
          notyf.Error("Feedsales submission reminder could not be sent");
        }
      }
      catch (Exception ex)
      {
        notyf.Error($"Feedsales submission reminder could not be sent: {ex.Message}");
      }

      return RedirectToAction("Submissions", "FeedsalesReport",
        new { filterYear = reportYear, filterMonth = reportMonth });
    }

    public async Task<IActionResult> Guideline()
    {
      return View();
    }

    public async Task<IActionResult> RawMaterialUsageReport()
    {
      return View();
    }

    public async Task<IActionResult> BranchReportSummary()
    {
      var result = await feedsalesLayer.GetReportBranchSummaryReport(ModuleEnum.Feedsales, DateTime.Now.Year);
      return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> RawMaterialUsageByMaterialAndDate(DateTime startDate, DateTime endDate, int[] materialId, int[] memberId)
    {
      var result = await feedsalesLayer.GenerateRawMaterialUsageReport(startDate, endDate, materialId, memberId).ConfigureAwait(false);
      return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "RawMaterualUsage.xlsx");
    }

    public IActionResult FinishedProductUsageReport()
    {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> FinishedProductUsageByProductAndDate(DateTime startDate, DateTime endDate, int[] productGroupId, int[] memberId)
    {
      var result = await feedsalesLayer.GenerateFinishedProductUsageReport(startDate, endDate, productGroupId, memberId).ConfigureAwait(false);
      return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "FinishedProductUsage.xlsx");
    }
  }
}
