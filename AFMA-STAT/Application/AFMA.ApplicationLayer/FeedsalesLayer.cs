namespace AFMA.ApplicationLayer
{
  using AFMA.ApplicationLayer.Model;
  using AFMA.ApplicationLayer.ViewModels;
  using AFMA.Data.Entities;
  using AFMA.Infrastructure.Options;
  using AFMA.Infrastructure.Settings;
  using Alpha.Services.NHibernate.Repository.Services;
  using AutoMapper;
  using Helpers;
  using Interface;
  using Microsoft.Extensions.Options;
  using NHibernate.Transform;
  using OfficeOpenXml;
  using System.Drawing;

  public class FeedsalesLayer : IFeedsalesLayer
  {
    private readonly IApplicationRepository<SessionFactory> _applicationRepository;
    private readonly IApplicationLayer _applicationLayer;
    private readonly IIdentityLayer _identityLayer;
    private readonly ApplicationSettings _applicationSettings;
    private readonly IMapper _mapper;

    public FeedsalesLayer(
      IApplicationRepository<SessionFactory> applicationRepository,
      IApplicationLayer applicationLayer,
      IIdentityLayer identityLayer,
      IOptions<ApplicationSettings> applicationSettings,
      IMapper mapper
      )
    {
      _applicationRepository = applicationRepository;
      _applicationLayer = applicationLayer;
      _identityLayer = identityLayer;
      _applicationSettings = applicationSettings.Value;
      _mapper = mapper;
    }

    /// <inheritdoc cref="GetFinishedProducts"/>
    public async Task<IEnumerable<ProductGroupModel>> GetFinishedProductGroups()
    {
      var result = await _applicationRepository.QueryAsync<ProductGroup>().ConfigureAwait(false);
      return _mapper.Map<IEnumerable<ProductGroup>, List<ProductGroupModel>>(result
        .Where(x => !x.HideOnFeedsales && x.VisibleToMembers).OrderBy(a => a.DisplayOrder).ThenBy(b => b.Name));
    }

    /// <inheritdoc cref="GetFeedsalesReport"/>
    public async Task<FeedsalesReportModel> GetFeedsalesReport(int id, int memberBranchId)
    {
      var result = await _applicationRepository.QuerySingleAsync<FeedsalesReport>(x => x.Id == id && x.MemberBranch.Id == memberBranchId);
      return _mapper.Map<FeedsalesReport, FeedsalesReportModel>(result);
    }

    public async Task<FeedsalesReportModel> GetPreviousMonthFeedsalesReport(int id, int memberBranchId)
    {
      var resultCurrentMonth = await _applicationRepository.QuerySingleAsync<FeedsalesReport>(x => x.Id == id && x.MemberBranch.Id == memberBranchId);
      var previousDate = resultCurrentMonth.InfoFor.AddMonths(-1);
      var resultsPreviousMonth = await _applicationRepository.QuerySingleAsync<FeedsalesReport>(x =>
          x.MemberBranch.Id == memberBranchId && x.ReportYear == previousDate.Year &&
          x.ReportMonth == previousDate.Month)
        .ConfigureAwait(false);

      return _mapper.Map<FeedsalesReport, FeedsalesReportModel>(resultsPreviousMonth);
    }

    public async Task<byte[]> GenerateTotalFeedsalesReport(ReportCriteriaModel reportCriteria)
    {
      var currentUser = await _identityLayer.Identity.GetCurrentUser().ConfigureAwait(false);
      var currentUserIsAdmin = await _identityLayer.Identity.UserManager.IsInRoleAsync(currentUser, "Admin");

      var productColor = ColorTranslator.FromHtml("#4f81bd");
      var mfi = new System.Globalization.DateTimeFormatInfo();

      var productGroupsAsync = await GetFinishedProductGroups().ConfigureAwait(false);
      var productGroups = productGroupsAsync.Where(x => x.Active && x.VisibleToMembers && !x.HideOnFeedsales);


      var materialsAsync = await _applicationLayer.GetRawMaterials().ConfigureAwait(false);
      var materials = materialsAsync.Where(x => x.Active && x.VisibleToMembers && !x.HideOnFeedsales).OrderBy(m => m.Name);
      var reportMonths = GeneralHelpers.MonthsBetween(reportCriteria.StartDate, reportCriteria.EndDate).ToList();

      var groupMonthTotal = new List<decimal>();

      var formattedEndDate = reportCriteria.EndDate.AddHours(23).AddMinutes(59).AddSeconds(59);

      //RAW MATERIAL STATISTICS FUNCTIONS
      var totalFeedsalesCurrent = "";
      var totalRawMaterialUsage = "";

      //RAW MATERIAL USAGE FUNCTIONS
      var reportGroups = await _applicationLayer.GetFeedsalesReportGroups().ConfigureAwait(false);

      var colFromHex = ColorTranslator.FromHtml("#999999");

      using (ExcelPackage p = new ExcelPackage())
      {
        p.Workbook.Properties.Author = "AFMA";
        p.Workbook.Properties.Title = "AFMA Total Feedsales Report";
        p.Workbook.Properties.Company = "www.afma.co.za";

        #region FEEDSALES STATISTICS REPORT

        p.Workbook.Worksheets.Add("Feedsales Statistics");
        var ws = p.Workbook.Worksheets[0];
        ws.Name = "Feedsales Statistics";

        //DEFAULT SETTINGS
        ws.DefaultRowHeight = 14.25;
        ws.Column(1).Width = 2;
        ws.Cells["A:XFD"].Style.Font.Name = "Arial";
        ws.Cells["A:XFD"].Style.Font.Size = 8F;

        var startColIndex = 2;
        var rowIndex = 1;
        var colIndex = 1;
        var mainBorderEndRow = 0;
        var mainBorderEndCol = 0;

        colIndex++;
        rowIndex++;

        //header row with dates
        ws.Cells[rowIndex, colIndex, rowIndex, colIndex + 1].Merge = true;
        var row2col1 = ws.Cells[rowIndex, colIndex];
        row2col1.Value = $"AFMA FEED SALES STATISTICS \r\n {reportCriteria.StartDate.ToString("MMM yyyy").ToUpper()} - {reportCriteria.EndDate.ToString("MMM yyyy")}".ToUpper();
        ws.Cells[rowIndex, colIndex, rowIndex, colIndex + 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        row2col1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        row2col1.Style.Fill.BackgroundColor.SetColor(Color.White);
        row2col1.Style.Font.Bold = true;
        row2col1.Style.WrapText = true;
        row2col1.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        row2col1.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        ws.Row(2).Height = 30;
        colIndex++;
        colIndex++;
        colFromHex = ColorTranslator.FromHtml("#c0504d");

        var _reportMonths = reportMonths;
        _reportMonths.Add(reportCriteria.EndDate.AddYears(-1));

        foreach (var reportDate in _reportMonths)
        {
          var row2col2 = ws.Cells[rowIndex, colIndex];
          var _dt = $"{mfi.GetMonthName(reportDate.Month).Substring(0, 3).ToUpper()} {reportDate.Year}";
          ws.Column(colIndex).Width = 10;
          row2col2.Style.Font.Color.SetColor(Color.White);
          row2col2.Style.WrapText = true;
          row2col2.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          row2col2.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
          row2col2.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
          row2col2.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          row2col2.Style.Fill.BackgroundColor.SetColor(colFromHex);
          row2col2.Value = _dt;
          colIndex++;
        }
        ws.Cells[rowIndex, colIndex - 1].Style.Fill.BackgroundColor.SetColor(Color.ForestGreen);

        var m2m_diff = ws.Cells[rowIndex, colIndex];
        m2m_diff.Style.Font.Color.SetColor(Color.White);
        m2m_diff.Style.WrapText = true;
        m2m_diff.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        m2m_diff.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        m2m_diff.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        m2m_diff.Style.Fill.BackgroundColor.SetColor(Color.ForestGreen);
        m2m_diff.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        m2m_diff.Value = "M on M % DIFF";

        colIndex++;

        var total_ytd_previous = ws.Cells[rowIndex, colIndex];
        total_ytd_previous.Style.Font.Color.SetColor(Color.White);
        total_ytd_previous.Style.WrapText = true;
        total_ytd_previous.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        total_ytd_previous.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        total_ytd_previous.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        total_ytd_previous.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        total_ytd_previous.Style.Fill.BackgroundColor.SetColor(Color.ForestGreen);
        total_ytd_previous.Value = $"TOTAL YTD {reportCriteria.StartDate.AddYears(-1).ToString("yyyy")}/{reportCriteria.StartDate.ToString("yyyy")}";
        ws.Column(colIndex).Width = 12;

        colIndex++;

        var total_ytd_current = ws.Cells[rowIndex, colIndex];
        total_ytd_current.Style.Font.Color.SetColor(Color.White);
        total_ytd_current.Style.WrapText = true;
        total_ytd_current.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        total_ytd_current.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        total_ytd_current.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        total_ytd_current.Style.Fill.BackgroundColor.SetColor(Color.ForestGreen);
        total_ytd_current.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        total_ytd_current.Value = $"TOTAL YTD {reportCriteria.StartDate:yyyy}/{reportCriteria.StartDate.AddYears(1):yyyy}";
        ws.Column(colIndex).Width = 12;

        colIndex++;

        var y2y_diff = ws.Cells[rowIndex, colIndex];
        y2y_diff.Style.Font.Color.SetColor(Color.White);
        y2y_diff.Style.WrapText = true;
        y2y_diff.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        y2y_diff.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        y2y_diff.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        y2y_diff.Style.Fill.BackgroundColor.SetColor(Color.ForestGreen);
        y2y_diff.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        y2y_diff.Value = "Y on Y % DIFF";
        ws.Column(colIndex).Width = 12;

        colIndex = startColIndex;
        //rowIndex++;                

        foreach (var group in productGroups.OrderBy(x => x.Code))
        {
          colFromHex = ColorTranslator.FromHtml("#4f81bd");
          colIndex = startColIndex;
          rowIndex++;
          var groupCode = ws.Cells[rowIndex, colIndex];
          groupCode.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          groupCode.Style.Font.Color.SetColor(Color.White);
          groupCode.Style.Fill.BackgroundColor.SetColor(colFromHex);
          groupCode.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          groupCode.Value = group.Code;
          colIndex++;
          var groupName = ws.Cells[rowIndex, colIndex];
          groupName.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          groupName.Style.Fill.BackgroundColor.SetColor(colFromHex);
          groupName.Style.Font.Color.SetColor(Color.White);
          groupName.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          groupName.Value = group.Name;
          ws.Column(colIndex).Width = 35;
          colIndex++;

          foreach (var reportDate in _reportMonths)
          {
            var groupTotal = ws.Cells[rowIndex, colIndex];
            groupTotal.Style.Font.Color.SetColor(Color.White);
            groupTotal.Style.Font.Size = 9F;
            groupTotal.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            groupTotal.Style.Fill.BackgroundColor.SetColor(colFromHex);
            groupTotal.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            var total_row = ws.Cells[rowIndex, colIndex];

            try
            {
              var totalValue = _applicationLayer.GetTotalFinishedProductByGroup(reportDate.Year, reportDate.Month,
                reportCriteria.MemberBranchId.GetValueOrDefault(), group.Id, currentUserIsAdmin);
              groupTotal.Style.Numberformat.Format = "#,#";
              groupTotal.Value = totalValue;
            }
            catch
            {
              groupTotal.Value = 0M;
            }
            colIndex++;
          }

          //m 2 m % diff total column
          var m2m_diff_prod_row = ws.Cells[rowIndex, colIndex];
          m2m_diff_prod_row.Style.Font.Color.SetColor(Color.White);
          m2m_diff_prod_row.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          m2m_diff_prod_row.Style.Fill.BackgroundColor.SetColor(colFromHex);
          m2m_diff_prod_row.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          m2m_diff_prod_row.Formula = $"({ws.Cells[rowIndex, colIndex - 2].Address}/{ws.Cells[rowIndex, colIndex - 1].Address})-1";
          m2m_diff_prod_row.Style.Numberformat.Format = "#,#0.0%";
          colIndex++;

          //total ytd previous total column
          var ytd_previous_total = ws.Cells[rowIndex, colIndex];
          ytd_previous_total.Style.Font.Color.SetColor(Color.White);
          ytd_previous_total.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          ytd_previous_total.Style.Fill.BackgroundColor.SetColor(colFromHex);
          ytd_previous_total.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          ytd_previous_total.Style.Numberformat.Format = "#,#";

          try
          {
            var previousStartDate = reportCriteria.StartDate.AddMonths(-12);
            var previousEndDate = reportCriteria.EndDate.AddMonths(-12);
            var previousTotalValue = _applicationLayer.GetFinishedProductGroupTotalSales(previousStartDate, previousEndDate, group.Id, 1, reportCriteria.MemberBranchId);
            ytd_previous_total.Value = previousTotalValue;

          }
          catch
          {
            ytd_previous_total.Value = 0;
          }

          colIndex++;

          //total ytd current total column
          var ytd_current_total = ws.Cells[rowIndex, colIndex];
          ytd_current_total.Style.Font.Color.SetColor(Color.White);
          ytd_current_total.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          ytd_current_total.Style.Fill.BackgroundColor.SetColor(colFromHex);
          ytd_current_total.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          ytd_current_total.Formula = $"=SUM({ws.Cells[rowIndex, 4].Address}:{ws.Cells[rowIndex, 2 + reportMonths.Count()].Address})";
          ytd_current_total.Style.Numberformat.Format = "#,#";
          colIndex++;

          //total y on y diff total column
          var y2y_diff_prod_row = ws.Cells[rowIndex, colIndex];
          y2y_diff_prod_row.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          y2y_diff_prod_row.Style.Font.Color.SetColor(Color.White);
          y2y_diff_prod_row.Style.Fill.BackgroundColor.SetColor(colFromHex);
          y2y_diff_prod_row.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          y2y_diff_prod_row.Formula = $"({ws.Cells[rowIndex, colIndex - 1].Address}/{ws.Cells[rowIndex, colIndex - 2].Address})-1";
          y2y_diff_prod_row.Style.Numberformat.Format = "#,#0.0%";
          colIndex++;

          rowIndex++;
          colIndex = startColIndex;

          ws.Cells[rowIndex, colIndex, rowIndex, colIndex + 1].Merge = true;
          var perHead = ws.Cells[rowIndex, colIndex];
          perHead.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          perHead.Style.Fill.BackgroundColor.SetColor(Color.Silver);
          perHead.Style.Font.Bold = true;
          ws.Cells[rowIndex, colIndex, rowIndex, colIndex + 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          perHead.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
          perHead.Value = "% GROWTH M-M";
          colIndex++;
          colIndex++;
          var firstMonthRow = true;

          foreach (var reportDate in _reportMonths.Take(_reportMonths.Count() - 1))
          {
            var perRow = ws.Cells[rowIndex, colIndex];
            perRow.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            perRow.Style.Fill.BackgroundColor.SetColor(Color.Silver);
            perRow.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            perRow.Style.Font.Bold = true;
            var cVal = ws.Cells[rowIndex - 1, colIndex];
            var pVal = ws.Cells[rowIndex - 1, colIndex - 1];
            if (firstMonthRow == true)
            {
              pVal = ws.Cells[rowIndex - 1, colIndex + reportMonths.Count() - 1];
              perRow.Value = "";
              firstMonthRow = false;
            }
            else
            {
              perRow.Formula = $"=({cVal.Address}/{pVal.Address})-1";
            }
            perRow.Style.Numberformat.Format = "#,#0.0%";

            colIndex++;
          }

          ws.Cells[rowIndex, colIndex, rowIndex, colIndex + 4].Merge = true;
          var spacers = ws.Cells[rowIndex, colIndex, rowIndex, colIndex + 4];
          spacers.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          spacers.Style.Fill.BackgroundColor.SetColor(Color.Silver);

          foreach (var product in group.Products.Where(f => f.ProductGroupId == group.Id))
          {
            colIndex = startColIndex;
            rowIndex++;
            var prodCode = ws.Cells[rowIndex, colIndex];
            prodCode.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            prodCode.Style.Fill.BackgroundColor.SetColor(Color.White);
            prodCode.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            prodCode.Value = product.Code;
            colIndex++;
            var prodName = ws.Cells[rowIndex, colIndex];
            prodName.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            prodName.Style.Fill.BackgroundColor.SetColor(Color.White);
            prodName.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            prodName.Value = product.Name;
            colIndex++;

            foreach (var reportDate in _reportMonths)
            {
              var prodTotal = ws.Cells[rowIndex, colIndex];
              prodTotal.Style.Font.Color.SetColor(Color.White);
              prodTotal.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
              prodTotal.Style.Fill.BackgroundColor.SetColor(colFromHex);
              prodTotal.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              //var groupDetail = feedsalesReport.Where(x => x.Month == reportDate.Month && x.Year == reportDate.Year).Select(d => d.Feedsales).Single();
              //var groupTotals = feedsalesReport.Where(x => x.Month == reportDate.Month && x.Year == reportDate.Year).SelectMany(d => d.Feedsales.Where(n => n.ProductGroupId == group.Id).Select(x => x.Items.Where(m => m.ProductId == product.Id)));

              //var db = new AFMA.Data.AFMAContext();

              //JS CHanged DueDate to Infor date

              var totalProducts = _applicationLayer.GetTotalFinishedProduct(reportDate.Year, reportDate.Month, product.Id, 1, reportCriteria.MemberBranchId);

              //if (BranchId == 0)
              //{
              //  i = (from report in db.FeedsalesReport
              //       join items in db.FeedsalesReportItem on report.Id equals items.ReportId
              //       join company in db.Companies on report.CompanyId equals company.Id
              //       where (report.Submitted == true && report.InforFor.Value.Month == reportDate.Month && report.InforFor.Value.Year == reportDate.Year) && items.ProductId == product.Id && company.MemberTypeId == 1
              //       select items.Value).DefaultIfEmpty().Sum();
              //}
              //else
              //{
              //  i = (from report in db.FeedsalesReport
              //       join items in db.FeedsalesReportItem on report.Id equals items.ReportId
              //       join company in db.Companies on report.CompanyId equals company.Id
              //       where (report.Submitted == true && report.InforFor.Value.Month == reportDate.Month && report.InforFor.Value.Year == reportDate.Year && report.BranchId == BranchId) && items.ProductId == product.Id && company.MemberTypeId == 1
              //       select items.Value).DefaultIfEmpty().Sum();
              //}

              var prodRow = ws.Cells[rowIndex, colIndex];
              prodRow.Style.Font.Color.SetColor(Color.Black);
              prodRow.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
              prodRow.Style.Fill.BackgroundColor.SetColor(Color.White);
              prodRow.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              prodRow.Style.Numberformat.Format = "#,#";
              prodRow.Value = totalProducts;
              colIndex++;
            }

            //M 2 M % DIFF column
            var m2m_diff_prod_row1 = ws.Cells[rowIndex, colIndex];
            m2m_diff_prod_row1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            m2m_diff_prod_row1.Style.Fill.BackgroundColor.SetColor(Color.White);
            m2m_diff_prod_row1.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            m2m_diff_prod_row1.Formula = String.Format("({0}/{1})-1", ws.Cells[rowIndex, colIndex - 2].Address, ws.Cells[rowIndex, colIndex - 1].Address);
            m2m_diff_prod_row1.Style.Numberformat.Format = "#,#0.00%";
            colIndex++;

            //total ytd previous year
            var ytd_previous = ws.Cells[rowIndex, colIndex];
            ytd_previous.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            ytd_previous.Style.Fill.BackgroundColor.SetColor(Color.White);
            ytd_previous.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            //ytd_previous.Formula = String.Format("({0}/{1})-1", ws.Cells[rowIndex, colIndex - 2].Address, ws.Cells[rowIndex, colIndex - 1].Address);
            ytd_previous.Style.Numberformat.Format = "#,#";

            try
            {
              var previousYtdStartDate = reportCriteria.StartDate.AddMonths(-12);
              var previousYtdEndDate = reportCriteria.EndDate.AddMonths(-12);
              var previousYtdTotalValue = _applicationLayer.GetFinishedProductsTotalSales(previousYtdStartDate, previousYtdEndDate, product.Id, 1, reportCriteria.MemberBranchId);
              ytd_previous.Value = previousYtdTotalValue;
            }
            catch
            {
              ytd_previous.Value = 0;
            }
            colIndex++;

            //total ytd current year
            var ytd_current = ws.Cells[rowIndex, colIndex];
            ytd_current.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            ytd_current.Style.Fill.BackgroundColor.SetColor(Color.White);
            ytd_current.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            ytd_current.Formula = $"=SUM({ws.Cells[rowIndex, 4].Address}:{ws.Cells[rowIndex, 2 + reportMonths.Count()].Address})";
            ytd_current.Style.Numberformat.Format = "#,#";
            colIndex++;

            //total y on y diff column
            var y2y_diff_prod_row1 = ws.Cells[rowIndex, colIndex];
            y2y_diff_prod_row1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            y2y_diff_prod_row1.Style.Fill.BackgroundColor.SetColor(Color.White);
            y2y_diff_prod_row1.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            y2y_diff_prod_row1.Formula = $"({ws.Cells[rowIndex, colIndex - 1].Address}/{ws.Cells[rowIndex, colIndex - 2].Address})-1";
            y2y_diff_prod_row1.Style.Numberformat.Format = "#,#0.0%";
            colIndex++;

          }
          colIndex = startColIndex;
          rowIndex++;
        }
        rowIndex++;

        //grand total colums

        colIndex = startColIndex;
        ws.Cells[rowIndex, colIndex, rowIndex, colIndex + 1].Merge = true;
        colFromHex = System.Drawing.ColorTranslator.FromHtml("#4f81bd");
        var grandtotal_heading = ws.Cells[rowIndex, colIndex];
        grandtotal_heading.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        grandtotal_heading.Style.Fill.BackgroundColor.SetColor(colFromHex);
        grandtotal_heading.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        ws.Cells[rowIndex, colIndex, rowIndex, colIndex + 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        grandtotal_heading.Value = $"TOTALS {reportCriteria.StartDate.Year}/{reportCriteria.EndDate.AddYears(1).Year}";

        colIndex = startColIndex + 2;

        foreach (var reportDate in reportMonths)
        {
          var grand_total_month = ws.Cells[rowIndex, colIndex];
          grand_total_month.Style.Font.Bold = true;
          grand_total_month.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          grand_total_month.Style.Fill.BackgroundColor.SetColor(Color.White);
          grand_total_month.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          grand_total_month.Style.Font.Color.SetColor(Color.Black);

          try
          {
            var totalValue = _applicationLayer.GetTotalFeedsalesGroupByMonth(reportDate.Year, reportDate.Month, 1, reportCriteria.MemberBranchId);
            grand_total_month.Value = totalValue;
          }
          catch
          {
            grand_total_month.Value = 0;
          }
          grand_total_month.Style.Numberformat.Format = "#,#";
          colIndex++;
        }

        var m2m_grandtotal = ws.Cells[rowIndex, colIndex];
        m2m_grandtotal.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        m2m_grandtotal.Style.Fill.BackgroundColor.SetColor(Color.White);
        m2m_grandtotal.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        m2m_grandtotal.Style.Font.Color.SetColor(Color.Black);
        m2m_grandtotal.Formula = String.Format("({0}/{1})-1", ws.Cells[rowIndex, colIndex - 1].Address, ws.Cells[rowIndex, colIndex - 2].Address);
        m2m_grandtotal.Style.Numberformat.Format = "#,#0.00%";
        colIndex++;

        DateTime startd_p = reportCriteria.StartDate.AddMonths(-12);
        DateTime endd_p = reportCriteria.EndDate.AddMonths(-12);
        var y2d_grandtotal_previous = ws.Cells[rowIndex, colIndex];
        y2d_grandtotal_previous.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        y2d_grandtotal_previous.Style.Fill.BackgroundColor.SetColor(Color.White);
        y2d_grandtotal_previous.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        y2d_grandtotal_previous.Style.Font.Color.SetColor(Color.Black);
        var totalValuePrevious = _applicationLayer.GetTotalFeedsalesGroupByYear(startd_p, endd_p, 1, reportCriteria.MemberBranchId);
        y2d_grandtotal_previous.Value = totalValuePrevious;
        y2d_grandtotal_previous.Style.Numberformat.Format = "#,#";
        colIndex++;


        var y2d_grandtotal_current = ws.Cells[rowIndex, colIndex];
        y2d_grandtotal_current.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        y2d_grandtotal_current.Style.Fill.BackgroundColor.SetColor(Color.White);
        y2d_grandtotal_current.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        y2d_grandtotal_current.Style.Font.Color.SetColor(Color.Black);
        y2d_grandtotal_current.Formula = $"=SUM({ws.Cells[rowIndex, 4].Address}:{ws.Cells[rowIndex, 2 + reportMonths.Count()].Address})";
        y2d_grandtotal_current.Style.Numberformat.Format = "#,#";
        totalFeedsalesCurrent = y2d_grandtotal_current.Address; //set the cell address for the feedsales total amount
        colIndex++;

        var y2y_grandtotal = ws.Cells[rowIndex, colIndex];
        y2y_grandtotal.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        y2y_grandtotal.Style.Fill.BackgroundColor.SetColor(Color.White);
        y2y_grandtotal.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        y2y_grandtotal.Formula = $"({ws.Cells[rowIndex, colIndex - 1].Address}/{ws.Cells[rowIndex, colIndex - 2].Address})-1";
        y2y_grandtotal.Style.Numberformat.Format = "#,#0.0%";

        mainBorderEndCol = colIndex;
        mainBorderEndRow = rowIndex + 1;

        rowIndex++;
        colIndex = startColIndex;

        ws.Cells[rowIndex, colIndex, rowIndex, colIndex + 1].Merge = true;
        var growth_grandtotal_heading = ws.Cells[rowIndex, colIndex];
        growth_grandtotal_heading.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        growth_grandtotal_heading.Style.Fill.BackgroundColor.SetColor(Color.Silver);
        growth_grandtotal_heading.Style.Font.Bold = true;
        growth_grandtotal_heading.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        ws.Cells[rowIndex, colIndex, rowIndex, colIndex + 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        growth_grandtotal_heading.Value = "% GROWTH M-M";

        colIndex = startColIndex + 2;
        Boolean firstMonthTotal = true;
        reportMonths.RemoveAt(reportMonths.Count() - 1); //remove the last month for totals
        foreach (var item in reportMonths)
        {
          var growth_grandtotal = ws.Cells[rowIndex, colIndex];
          growth_grandtotal.Style.Font.Bold = true;
          var cVal = ws.Cells[rowIndex - 1, colIndex];
          var pVal = ws.Cells[rowIndex - 1, colIndex - 1];
          if (firstMonthTotal == true)
          {
            pVal = ws.Cells[rowIndex - 1, colIndex + reportMonths.Count() - 1];
            growth_grandtotal.Value = "";
            firstMonthTotal = false;
          }
          else
          {
            growth_grandtotal.Formula = $"=({cVal.Address}/{pVal.Address})-1";
          }

          //if (firstMonthTotal)
          //{
          //    pVal = ws.Cells[rowIndex - 1, colIndex + reportMonths.Count()].Address;
          //    firstMonthTotal = false;
          //}
          //growth_grandtotal.Formula = String.Format("={0}/{1}-1", cVal, pVal);
          growth_grandtotal.Style.Numberformat.Format = "#,#0.0%";
          growth_grandtotal.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          growth_grandtotal.Style.Fill.BackgroundColor.SetColor(Color.Silver);
          growth_grandtotal.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          colIndex++;
        }

        //draw the main thick border
        ws.Cells[2, 2, mainBorderEndRow, mainBorderEndCol].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thick);

        #endregion

        #region RAW MATERIAL STATISTICS

        p.Workbook.Worksheets.Add("Raw Material Statistics");
        ExcelWorksheet ws1 = p.Workbook.Worksheets[1];
        ws1.Name = "Raw Material Statistics";

        //remove the last month added for comparison in previous report
        //reportMonths.RemoveAt(reportMonths.Count() - 1);

        //DEFAULT SETTINGS
        ws1.DefaultRowHeight = 14.25;
        ws1.Column(1).Width = 2;
        ws1.Cells["A:XFD"].Style.Font.Name = "Arial";
        ws1.Cells["A:XFD"].Style.Font.Size = 8F;

        startColIndex = 2;
        rowIndex = 2;
        colIndex = 1;
        mainBorderEndRow = 0;
        mainBorderEndCol = 0;

        colIndex++;

        ws1.Cells[rowIndex, colIndex, rowIndex, colIndex + 1].Merge = true;
        var header = ws1.Cells[rowIndex, colIndex];
        ws1.Row(rowIndex).Height = 30;
        header.Value = $"AFMA - RAW MATERIAL USAGE \r\n {reportCriteria.StartDate.ToString("MMM yyyy").ToUpper()} - {reportCriteria.EndDate.ToString("MMM yyyy").ToUpper()}";
        header.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        header.Style.Fill.BackgroundColor.SetColor(Color.White);
        header.Style.Font.Bold = true;
        header.Style.WrapText = true;
        header.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        header.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

        colIndex++;
        colIndex++;
        colFromHex = System.Drawing.ColorTranslator.FromHtml("#c0504d");

        var header_total_tons = ws1.Cells[rowIndex, colIndex];
        header_total_tons.Value = "TOTAL TONS";
        header_total_tons.Style.Font.Color.SetColor(Color.White);
        header_total_tons.Style.WrapText = true;
        header_total_tons.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        header_total_tons.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        header_total_tons.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        header_total_tons.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        header_total_tons.Style.Fill.BackgroundColor.SetColor(colFromHex);
        ws1.Column(colIndex).Width = 15;

        colIndex++;

        var header_makeup = ws1.Cells[rowIndex, colIndex];
        header_makeup.Style.Font.Color.SetColor(Color.White);
        header_makeup.Style.WrapText = true;
        header_makeup.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        header_makeup.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        header_makeup.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        header_makeup.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        header_makeup.Style.Fill.BackgroundColor.SetColor(colFromHex);
        header_makeup.Value = "% MAKEUP IN FEEDS";
        ws1.Column(colIndex).Width = 12;
        colIndex++;


        foreach (var reportDate in reportMonths)
        {
          var header_months = ws1.Cells[rowIndex, colIndex];
          var _dt = $"{mfi.GetMonthName(reportDate.Month).Substring(0, 3).ToUpper()} {reportDate.Year}";
          ws1.Column(colIndex).Width = 12;
          header_months.Style.Font.Color.SetColor(Color.White);
          header_months.Style.WrapText = true;
          header_months.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          header_months.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
          header_months.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
          header_months.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          header_months.Style.Fill.BackgroundColor.SetColor(colFromHex);
          header_months.Value = _dt;
          colIndex++;
        }

        //rowIndex++;
        colIndex = startColIndex;

        var finalRow = materials.Count() + 1;
        var rowTotal = finalRow + 2;
        var dataRowStart = ws1.Cells[rowIndex, 3].Address;
        var dataRowEnd = ws1.Cells[rowIndex + finalRow, colIndex].Address;

        //rowIndex++;
        rowTotal++;
        ws1.Cells[rowTotal, 2, rowTotal, 3].Merge = true;
        ws1.Cells[rowTotal, 2, rowTotal, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        var tot_raw_text = ws1.Cells[rowTotal, 2];
        //tot_raw_text.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        tot_raw_text.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        tot_raw_text.Style.Fill.BackgroundColor.SetColor(productColor);
        tot_raw_text.Style.Font.Color.SetColor(Color.White);
        tot_raw_text.Style.Font.Bold = true;
        tot_raw_text.Value = "TOTAL RAW MATERIAL USAGE";

        var total_raw_usage = ws1.Cells[rowTotal, 4];
        total_raw_usage.Formula = $"=SUM({ws1.Cells[3, 4].Address}:{ws1.Cells[finalRow + 1, 4].Address})";
        total_raw_usage.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        total_raw_usage.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        total_raw_usage.Style.Fill.BackgroundColor.SetColor(Color.White);
        total_raw_usage.Style.Font.Bold = true;
        totalRawMaterialUsage = ws1.Cells[rowTotal, 4].Address;
        total_raw_usage.Style.Numberformat.Format = "#,#";

        //percentage col
        var total_raw_percentage = ws1.Cells[rowTotal, 5];
        total_raw_percentage.Formula = $"={ws1.Cells[rowTotal, 4].Address}/{ws1.Cells[rowTotal + 1, 4].Address}";
        total_raw_percentage.Style.Numberformat.Format = "#,#0.00%";
        total_raw_percentage.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        total_raw_percentage.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        total_raw_percentage.Style.Fill.BackgroundColor.SetColor(Color.White);
        total_raw_percentage.Style.Font.Bold = true;

        rowIndex++;
        colIndex = startColIndex;

        ws1.Cells[rowTotal + 1, 2, rowTotal + 1, 3].Merge = true;
        ws1.Cells[rowTotal + 1, 2, rowTotal + 1, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        var tot_sales_text = ws1.Cells[rowTotal + 1, 2];
        tot_sales_text.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        tot_sales_text.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        tot_sales_text.Style.Fill.BackgroundColor.SetColor(productColor);
        tot_sales_text.Style.Font.Color.SetColor(Color.White);
        tot_sales_text.Style.Font.Bold = true;
        tot_sales_text.Value = "ANIMAL FEED SALES FOR PERIOD";

        var tot_sales_usage = ws1.Cells[rowTotal + 1, 4];
        tot_sales_usage.Formula = $"'{"Feedsales Statistics"}'!{totalFeedsalesCurrent}";
        tot_sales_usage.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        tot_sales_usage.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        tot_sales_usage.Style.Fill.BackgroundColor.SetColor(Color.White);
        tot_sales_usage.Style.Font.Bold = true;
        tot_sales_usage.Style.Numberformat.Format = "#,#";

        colIndex = startColIndex;

        ws1.Cells[rowTotal + 3, 2, rowTotal + 3, 3].Merge = true;
        var tot_maize_text = ws1.Cells[rowTotal + 3, 2];
        ws1.Cells[rowTotal + 3, 2, rowTotal + 3, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        tot_maize_text.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        tot_maize_text.Style.Fill.BackgroundColor.SetColor(productColor);
        tot_maize_text.Style.Font.Color.SetColor(Color.White);
        tot_maize_text.Style.Font.Bold = true;
        tot_maize_text.Value = "MAIZE PRODUCTS (TOTAL)";

        var tot_maize_usage = ws1.Cells[rowTotal + 3, 4];
        tot_maize_usage.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        tot_maize_usage.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        tot_maize_usage.Style.Fill.BackgroundColor.SetColor(Color.White);
        tot_maize_usage.Style.Font.Color.SetColor(Color.Black);
        tot_maize_usage.Style.Font.Bold = true;
        tot_maize_usage.Style.Numberformat.Format = "#,#";
        try
        {
          var totalValue = _applicationLayer.GetTotalRawMaterialUsageByCategoryByYear(reportCriteria.StartDate, reportCriteria.EndDate, 2, 1, reportCriteria.MemberBranchId);
          tot_maize_usage.Value = totalValue;
        }
        catch
        {
          tot_maize_usage.Value = 0;
        }

        colIndex++;
        var maize_percentage = ws1.Cells[rowTotal + 3, 5];
        maize_percentage.Formula = $"='Raw Material Usage'!{"D39"}";
        maize_percentage.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        maize_percentage.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        maize_percentage.Style.Numberformat.Format = "#,#0.00%";
        maize_percentage.Style.Fill.BackgroundColor.SetColor(Color.White);
        maize_percentage.Style.Font.Bold = true;

        colIndex = startColIndex;

        ws1.Cells[rowTotal + 4, 2, rowTotal + 4, 3].Merge = true;
        var tot_oilcake_text = ws1.Cells[rowTotal + 4, 2];
        ws1.Cells[rowTotal + 4, 2, rowTotal + 4, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        tot_oilcake_text.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        tot_oilcake_text.Style.Fill.BackgroundColor.SetColor(productColor);
        tot_oilcake_text.Style.Font.Color.SetColor(Color.White);
        tot_oilcake_text.Style.Font.Bold = true;
        tot_oilcake_text.Value = "OILCAKE AND FULLFAT (TOTAL)";

        var tot_oilcake_usage = ws1.Cells[rowTotal + 4, 4];
        tot_oilcake_usage.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        tot_oilcake_usage.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        tot_oilcake_usage.Style.Fill.BackgroundColor.SetColor(Color.White);
        tot_oilcake_usage.Style.Font.Color.SetColor(Color.Black);
        tot_oilcake_usage.Style.Font.Bold = true;
        tot_oilcake_usage.Style.Numberformat.Format = "#,#";
        try
        {
          var totalValue = _applicationLayer.GetTotalRawMaterialUsageByCategoryByYear(reportCriteria.StartDate, reportCriteria.EndDate, 1, 1, reportCriteria.MemberBranchId);
          tot_oilcake_usage.Value = totalValue;
        }
        catch
        {
          tot_oilcake_usage.Value = 0;
        }

        colIndex++;
        var oilcake_fullfat_percentage = ws1.Cells[rowTotal + 4, 5];
        oilcake_fullfat_percentage.Formula = $"='Raw Material Usage'!{"D41"}";
        oilcake_fullfat_percentage.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        oilcake_fullfat_percentage.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        oilcake_fullfat_percentage.Style.Numberformat.Format = "#,#0.00%";
        oilcake_fullfat_percentage.Style.Fill.BackgroundColor.SetColor(Color.White);
        oilcake_fullfat_percentage.Style.Font.Bold = true;

        colIndex = startColIndex;

        ws1.Cells[rowTotal + 5, 2, rowTotal + 5, 3].Merge = true;
        ws1.Cells[rowTotal + 5, 2, rowTotal + 5, 3].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        var tot_fishcake_text = ws1.Cells[rowTotal + 5, 2];
        tot_fishcake_text.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        tot_fishcake_text.Style.Fill.BackgroundColor.SetColor(productColor);
        tot_fishcake_text.Style.Font.Color.SetColor(Color.White);
        tot_fishcake_text.Value = "FISH MEAL (TOTAL)";
        tot_fishcake_text.Style.Font.Bold = true;

        var tot_fishcake_usage = ws1.Cells[rowTotal + 5, 4];
        tot_fishcake_usage.Formula = $"'{"Feedsales Statistics"}'!{totalFeedsalesCurrent}";
        tot_fishcake_usage.Style.Numberformat.Format = "#,#";
        tot_fishcake_usage.Style.Font.Bold = true;

        var tot_fishmeal_usage = ws1.Cells[rowTotal + 5, 4];
        tot_fishmeal_usage.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        tot_fishmeal_usage.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        tot_fishmeal_usage.Style.Fill.BackgroundColor.SetColor(Color.White);
        tot_fishmeal_usage.Style.Font.Color.SetColor(Color.Black);
        tot_fishcake_usage.Style.Font.Bold = true;
        tot_fishmeal_usage.Style.Numberformat.Format = "#,#";
        try
        {
          var totalValue = _applicationLayer.GetTotalRawMaterialUsageByCategoryByYear(reportCriteria.StartDate, reportCriteria.EndDate, 3, 1, reportCriteria.MemberBranchId);
          tot_fishmeal_usage.Value = totalValue;
        }
        catch
        {
          tot_fishmeal_usage.Value = 0;
        }

        colIndex++;
        var fishmeal_percentage = ws1.Cells[rowTotal + 5, 5];
        fishmeal_percentage.Formula = $"='Raw Material Usage'!{"D40"}";
        fishmeal_percentage.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        fishmeal_percentage.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        fishmeal_percentage.Style.Numberformat.Format = "#,#0.00%";
        fishmeal_percentage.Style.Fill.BackgroundColor.SetColor(Color.White);
        fishmeal_percentage.Style.Font.Bold = true;

        colIndex = startColIndex;

        foreach (var material in materials.OrderBy(x => x.Name))
        {
          var prod_row_code = ws1.Cells[rowIndex, colIndex];
          prod_row_code.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          prod_row_code.Style.Fill.BackgroundColor.SetColor(Color.Silver);
          prod_row_code.Style.Font.Color.SetColor(Color.White);
          prod_row_code.Style.Font.Bold = true;
          prod_row_code.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
          prod_row_code.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          prod_row_code.Value = material.Code;
          colIndex++;

          var prod_row_name = ws1.Cells[rowIndex, colIndex];
          prod_row_name.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          prod_row_name.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          prod_row_name.Style.Fill.BackgroundColor.SetColor(productColor);
          prod_row_name.Style.Font.Color.SetColor(Color.White);
          prod_row_name.Style.Font.Bold = true;
          prod_row_name.Value = material.Name;
          ws1.Column(colIndex).Width = 35;

          colIndex++;

          var total_tons_row = ws1.Cells[rowIndex, colIndex];
          total_tons_row.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          total_tons_row.Formula = $"=SUM({ws1.Cells[rowIndex, colIndex + 2].Address}:{ws1.Cells[rowIndex, colIndex + 2 + reportMonths.Count() - 1].Address})";
          total_tons_row.Style.Numberformat.Format = "#,#";
          colIndex++;

          var per_markup_row = ws1.Cells[rowIndex, colIndex];
          per_markup_row.Style.Font.Color.SetColor(Color.Black);
          per_markup_row.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          per_markup_row.Formula = $"=({ws1.Cells[rowIndex, colIndex - 1].Address}/{ws1.Cells[rowTotal, 4].Address})";
          per_markup_row.Style.Numberformat.Format = "#,#0.00%";

          colIndex = 6; //material report starts at col 6

          foreach (var reportDate in reportMonths)
          {
            var prodTotal = ws1.Cells[rowIndex, colIndex];
            prodTotal.Style.Font.Color.SetColor(Color.White);
            prodTotal.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            prodTotal.Style.Fill.BackgroundColor.SetColor(colFromHex);
            prodTotal.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

            //var db = new AFMA.Data.AFMAContext();


            //JS Changed DueDates to Info For Dates

            var totalMaterials = _applicationLayer.GetTotalRawMaterial(reportDate.Year, reportDate.Month, material.Id, 1, reportCriteria.MemberBranchId);
            //if (BranchId == 0)
            //{
            //  i = (from report in db.FeedsalesReport
            //       join items in db.FeedsalesReportItem on report.Id equals items.ReportId
            //       join company in db.Companies on report.CompanyId equals company.Id
            //       where (report.Submitted == true && report.InforFor.Value.Month == reportDate.Month && report.InforFor.Value.Year == reportDate.Year) && items.MaterialId == material.Id && items.MaterialId != 79 && company.MemberTypeId == 1
            //       select items.Value).DefaultIfEmpty().Sum();
            //}
            //else
            //{
            //  i = (from report in db.FeedsalesReport
            //       join items in db.FeedsalesReportItem on report.Id equals items.ReportId
            //       join company in db.Companies on report.CompanyId equals company.Id
            //       where (report.Submitted == true && report.InforFor.Value.Month == reportDate.Month && report.InforFor.Value.Year == reportDate.Year && report.BranchId == BranchId) && items.MaterialId == material.Id && company.MemberTypeId == 1
            //       select items.Value).DefaultIfEmpty().Sum();
            //}

            var prodRow = ws1.Cells[rowIndex, colIndex];
            prodRow.Style.Font.Color.SetColor(Color.Black);
            prodRow.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            prodRow.Style.Fill.BackgroundColor.SetColor(Color.White);
            prodRow.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            prodRow.Style.Numberformat.Format = "#,#0";
            prodRow.Value = totalMaterials;
            colIndex++;
          }
          rowIndex++;
          mainBorderEndCol = colIndex;
          colIndex = startColIndex;
        }

        //mainBorderEndCol = colIndex;
        mainBorderEndRow = rowIndex + 6; //take into account that the border must also be around the total columns

        //draw the main thick border
        ws1.Cells[2, 2, mainBorderEndRow, mainBorderEndCol - 1].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thick);

        #endregion;

        #region RAW MATERIAL USAGE

        var maroonHeader = ColorTranslator.FromHtml("#c0504d");
        var blueHeader = ColorTranslator.FromHtml("#4f81bd");
        var silverHeader = ColorTranslator.FromHtml("#999999");
        var greenHeader = ColorTranslator.FromHtml("#82bc4f");

        p.Workbook.Worksheets.Add("Raw Material Usage");
        var ws2 = p.Workbook.Worksheets[2];
        ws2.Name = "Raw Material Usage";

        //DEFAULT SETTINGS
        ws2.DefaultRowHeight = 14.25;
        ws2.Column(1).Width = 2;
        ws2.Cells["A:XFD"].Style.Font.Name = "Arial";
        ws2.Cells["A:XFD"].Style.Font.Size = 8F;

        startColIndex = 2;
        rowIndex = 2;
        colIndex = 1;
        mainBorderEndRow = 0;
        mainBorderEndCol = 0;

        //** SET DEFAULT COLUMN WIDTH'S
        ws2.Column(2).Width = 60;
        ws2.Column(3).Width = 12;
        ws2.Column(4).Width = 12;
        ws2.Column(6).Width = 12;
        ws2.Column(7).Width = 12;

        //colIndex = 1;
        //rowIndex = 1;
        colIndex++;

        var rmu_header = ws2.Cells[rowIndex, colIndex]; //header row for report
        ws2.Row(rowIndex).Height = 35;
        //ws2.Column(colIndex).Width = 60;
        rmu_header.Style.WrapText = true;
        rmu_header.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        rmu_header.Value = $"AFMA - RAW MATERIAL USAGE \r\n {reportCriteria.StartDate.ToString("MMM yyyy").ToUpper()} - {reportCriteria.EndDate.ToString("MMM yyyy").ToUpper()}";
        rmu_header.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        rmu_header.Style.Fill.BackgroundColor.SetColor(Color.White);
        rmu_header.Style.Font.Bold = true;
        rmu_header.Style.WrapText = true;
        rmu_header.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

        rowIndex++;
        rowIndex++;

        ws2.Cells[rowIndex, colIndex, rowIndex, colIndex + 5].Merge = true; //header row for oilcake usage
        var report_header = ws2.Cells[rowIndex, colIndex, rowIndex, colIndex + 5];
        report_header.Style.Font.Color.SetColor(Color.White);
        report_header.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        report_header.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        report_header.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        report_header.Style.Fill.BackgroundColor.SetColor(blueHeader);
        report_header.Style.Font.Bold = true;
        report_header.Value = "OILCAKE USAGE";

        rowIndex++;
        rowIndex++;
        colIndex = startColIndex;

        ws2.Cells[rowIndex, colIndex, rowIndex, colIndex + 2].Merge = true;
        var oilcake_current_year = ws2.Cells[rowIndex, colIndex, rowIndex, colIndex + 2];
        oilcake_current_year.Style.Font.Color.SetColor(Color.White);
        oilcake_current_year.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        oilcake_current_year.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        oilcake_current_year.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        oilcake_current_year.Style.Fill.BackgroundColor.SetColor(silverHeader);
        oilcake_current_year.Style.Font.Bold = true;
        oilcake_current_year.Value = $"{reportCriteria.StartDate.Year}/{reportCriteria.EndDate.Year}";
        //rowIndex++;

        colIndex++;
        colIndex++;
        colIndex++;
        colIndex++;

        ws2.Cells[rowIndex, colIndex, rowIndex, colIndex + 1].Merge = true;
        var oilcake_previous_year = ws2.Cells[rowIndex, colIndex, rowIndex, colIndex + 1];
        oilcake_previous_year.Style.Font.Color.SetColor(Color.White);
        oilcake_previous_year.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        oilcake_previous_year.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        oilcake_previous_year.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        oilcake_previous_year.Style.Fill.BackgroundColor.SetColor(silverHeader);
        oilcake_previous_year.Style.Font.Bold = true;
        oilcake_previous_year.Value = $"{reportCriteria.StartDate.AddYears(-1).Year}/{reportCriteria.EndDate.AddYears(-1).Year}";

        rowIndex++;
        rowIndex++;

        colIndex = startColIndex;

        var oilcake_rm_header1_1 = ws2.Cells[rowIndex, colIndex];
        ws2.Row(rowIndex).Height = 30;
        oilcake_rm_header1_1.Style.Font.Color.SetColor(Color.White);
        oilcake_rm_header1_1.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        oilcake_rm_header1_1.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        oilcake_rm_header1_1.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        oilcake_rm_header1_1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        oilcake_rm_header1_1.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        oilcake_rm_header1_1.Style.Font.Bold = true;
        oilcake_rm_header1_1.Value = "RAW MATERIAL";
        colIndex++;

        var oilcake_rm_header1_2 = ws2.Cells[rowIndex, colIndex];
        oilcake_rm_header1_2.Style.Font.Color.SetColor(Color.White);
        oilcake_rm_header1_2.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        oilcake_rm_header1_2.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        oilcake_rm_header1_2.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        oilcake_rm_header1_2.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        oilcake_rm_header1_2.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        oilcake_rm_header1_2.Style.Font.Bold = true;
        oilcake_rm_header1_2.Value = "TONS";
        colIndex++;

        var oilcake_rm_header1_3 = ws2.Cells[rowIndex, colIndex];
        oilcake_rm_header1_3.Style.Font.Color.SetColor(Color.White);
        oilcake_rm_header1_3.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        oilcake_rm_header1_3.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        oilcake_rm_header1_3.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        oilcake_rm_header1_3.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        oilcake_rm_header1_3.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        oilcake_rm_header1_3.Style.WrapText = true;
        oilcake_rm_header1_3.Style.Font.Bold = true;
        oilcake_rm_header1_3.Value = "% MAKEUP IN FEEDS";

        colIndex++;
        colIndex++;

        var oilcake_rm_header2_1 = ws2.Cells[rowIndex, colIndex];
        oilcake_rm_header2_1.Style.Font.Color.SetColor(Color.White);
        oilcake_rm_header2_1.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        oilcake_rm_header2_1.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        oilcake_rm_header2_1.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        oilcake_rm_header2_1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        oilcake_rm_header2_1.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        oilcake_rm_header2_1.Style.WrapText = true;
        oilcake_rm_header2_1.Style.Font.Bold = true;
        oilcake_rm_header2_1.Value = "TONS";

        colIndex++;

        var oilcake_rm_header2_2 = ws2.Cells[rowIndex, colIndex];
        oilcake_rm_header2_2.Style.Font.Color.SetColor(Color.White);
        oilcake_rm_header2_2.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        oilcake_rm_header2_2.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        oilcake_rm_header2_2.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        oilcake_rm_header2_2.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        oilcake_rm_header2_2.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        oilcake_rm_header2_2.Style.WrapText = true;
        oilcake_rm_header2_2.Style.Font.Bold = true;
        oilcake_rm_header2_2.Value = "% MAKEUP IN FEEDS";

        rowIndex++;
        colIndex = startColIndex;
        Int32 _startRow = rowIndex;
        foreach (var group in reportGroups.Where(x => x.FeedsalesReportGroupCategoryId == 1 && (x.ReportDisplayGroup == 1 || x.ReportDisplayGroup == 5))) //get list for oilcake only
        {
          var oilcake_name = ws2.Cells[rowIndex, colIndex];
          oilcake_name.Style.Font.Color.SetColor(Color.Black);
          oilcake_name.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          oilcake_name.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
          oilcake_name.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          oilcake_name.Style.Fill.BackgroundColor.SetColor(Color.White);
          oilcake_name.Value = group.Name;

          colIndex++;

          var oilcake_tons = ws2.Cells[rowIndex, colIndex];
          oilcake_tons.Style.Font.Color.SetColor(Color.Black);
          oilcake_tons.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          oilcake_tons.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          oilcake_tons.Style.Fill.BackgroundColor.SetColor(Color.White);
          oilcake_tons.Style.Numberformat.Format = "#,#";

          try
          {
            var totalValue = _applicationLayer.GetTotalRawMaterialUsageByYear(reportCriteria.StartDate, formattedEndDate, group.Id, group.ReportDisplayGroup, 1, reportCriteria.MemberBranchId);
            oilcake_tons.Value = totalValue;
          }
          catch
          {
            oilcake_tons.Value = 0;
          }


          colIndex++;

          var oilcake_percentage = ws2.Cells[rowIndex, colIndex];
          oilcake_percentage.Style.Font.Color.SetColor(Color.Black);
          oilcake_percentage.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          oilcake_percentage.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          oilcake_percentage.Style.Fill.BackgroundColor.SetColor(Color.White);
          oilcake_percentage.Style.Numberformat.Format = "#,#0.00%";
          oilcake_percentage.Formula = $"{ws2.Cells[rowIndex, colIndex - 1].Address}/'Raw Material Statistics'!{totalRawMaterialUsage}";

          colIndex++;
          colIndex++;

          //oilcake totals for previous year
          var oilcake_tons_previous = ws2.Cells[rowIndex, colIndex];
          oilcake_tons_previous.Style.Font.Color.SetColor(Color.Black);
          oilcake_tons_previous.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          oilcake_tons_previous.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          oilcake_tons_previous.Style.Fill.BackgroundColor.SetColor(Color.White);
          oilcake_tons_previous.Style.Numberformat.Format = "#,#";


          try
          {
            var sd = reportCriteria.StartDate.AddYears(-1);
            var ed = formattedEndDate.AddYears(-1);
            var totalValue = _applicationLayer.GetTotalRawMaterialUsageByYear(sd, ed, group.Id, group.ReportDisplayGroup, 1, reportCriteria.MemberBranchId);
            oilcake_tons_previous.Value = totalValue;
          }
          catch
          {
            oilcake_tons_previous.Value = 0;
          }

          colIndex++;

          var oilcake_percentage_previous = ws2.Cells[rowIndex, colIndex];
          oilcake_percentage_previous.Style.Font.Color.SetColor(Color.Black);
          oilcake_percentage_previous.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          oilcake_percentage_previous.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          oilcake_percentage_previous.Style.Fill.BackgroundColor.SetColor(Color.White);
          oilcake_percentage_previous.Style.Numberformat.Format = "#,#0.00%";
          oilcake_percentage_previous.Formula = $"{ws2.Cells[rowIndex, colIndex - 1].Address}/'Raw Material Statistics'!{totalRawMaterialUsage}";

          colIndex = startColIndex;
          rowIndex++;
        }
        var _endRow = rowIndex;

        colIndex = startColIndex;

        var oilcake_name_footer = ws2.Cells[rowIndex, colIndex];
        oilcake_name_footer.Style.Font.Color.SetColor(Color.White);
        oilcake_name_footer.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        oilcake_name_footer.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        oilcake_name_footer.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        oilcake_name_footer.Style.Font.Bold = true;
        oilcake_name_footer.Value = "TOTAL OILCAKE USED (TONS)";
        colIndex++;

        var oilcake_used_footer = ws2.Cells[rowIndex, colIndex];
        oilcake_used_footer.Style.Font.Color.SetColor(Color.White);
        oilcake_used_footer.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        oilcake_used_footer.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        oilcake_used_footer.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        oilcake_used_footer.Formula = $"=SUM({ws2.Cells[_startRow, 3].Address}:{ws2.Cells[_endRow - 1, 3].Address})";
        oilcake_used_footer.Style.Font.Bold = true;
        oilcake_used_footer.Style.Numberformat.Format = "#,#";

        colIndex++;

        var oilcake_percentage_footer = ws2.Cells[rowIndex, colIndex];
        oilcake_percentage_footer.Style.Font.Color.SetColor(Color.White);
        oilcake_percentage_footer.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        oilcake_percentage_footer.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        oilcake_percentage_footer.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        oilcake_percentage_footer.Formula = $"=SUM({ws2.Cells[_startRow, 4].Address}:{ws2.Cells[_endRow - 1, 4].Address})";
        oilcake_percentage_footer.Style.Font.Bold = true;
        oilcake_percentage_footer.Style.Numberformat.Format = "#,#0.00%";

        colIndex++;
        colIndex++;

        var oilcake_used_footer_previous = ws2.Cells[rowIndex, colIndex];
        oilcake_used_footer_previous.Style.Font.Color.SetColor(Color.White);
        oilcake_used_footer_previous.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        oilcake_used_footer_previous.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        oilcake_used_footer_previous.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        oilcake_used_footer_previous.Formula = $"=SUM({ws2.Cells[_startRow, 6].Address}:{ws2.Cells[_endRow - 1, 6].Address})";
        oilcake_used_footer_previous.Style.Font.Bold = true;
        oilcake_used_footer_previous.Style.Numberformat.Format = "#,#";

        colIndex++;

        var oilcake_percentage_footer_previous = ws2.Cells[rowIndex, colIndex];
        oilcake_percentage_footer_previous.Style.Font.Color.SetColor(Color.White);
        oilcake_percentage_footer_previous.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        oilcake_percentage_footer_previous.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        oilcake_percentage_footer_previous.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        oilcake_percentage_footer_previous.Formula = $"=SUM({ws2.Cells[_startRow, 7].Address}:{ws2.Cells[_endRow - 1, 7].Address})";
        oilcake_used_footer_previous.Style.Font.Bold = true;
        oilcake_percentage_footer_previous.Style.Numberformat.Format = "#,#0.00%";

        rowIndex++;
        rowIndex++;
        rowIndex++;

        colIndex = startColIndex;

        ws2.Cells[rowIndex, colIndex, rowIndex, colIndex + 5].Merge = true; //header row for maize usage
        var report_header_maize = ws2.Cells[rowIndex, colIndex, rowIndex, colIndex + 5];
        report_header_maize.Style.Font.Color.SetColor(Color.White);
        report_header_maize.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        report_header_maize.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        report_header_maize.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        report_header_maize.Style.Fill.BackgroundColor.SetColor(blueHeader);
        report_header_maize.Style.Font.Bold = true;
        report_header_maize.Value = "MAIZE USAGE";
        rowIndex++;
        rowIndex++;

        ws2.Cells[rowIndex, colIndex, rowIndex, colIndex + 2].Merge = true;
        var maize_current_year = ws2.Cells[rowIndex, colIndex, rowIndex, colIndex + 2];
        maize_current_year.Style.Font.Color.SetColor(Color.White);
        maize_current_year.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        maize_current_year.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        maize_current_year.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        maize_current_year.Style.Fill.BackgroundColor.SetColor(silverHeader);
        maize_current_year.Style.Font.Bold = true;
        maize_current_year.Value = $"{reportCriteria.StartDate.Year}/{reportCriteria.EndDate.Year}";

        colIndex++;
        colIndex++;
        colIndex++;
        colIndex++;

        ws2.Cells[rowIndex, colIndex, rowIndex, colIndex + 1].Merge = true;
        var maize_previous_year = ws2.Cells[rowIndex, colIndex, rowIndex, colIndex + 1];
        maize_previous_year.Style.Font.Color.SetColor(Color.White);
        maize_previous_year.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        maize_previous_year.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        maize_previous_year.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        maize_previous_year.Style.Fill.BackgroundColor.SetColor(silverHeader);
        maize_previous_year.Style.Font.Bold = true;
        maize_previous_year.Value = $"{reportCriteria.StartDate.AddYears(-1).Year}/{reportCriteria.EndDate.AddYears(-1).Year}";

        rowIndex++;
        rowIndex++;
        colIndex = startColIndex;

        var maize_rm_header1_1 = ws2.Cells[rowIndex, colIndex];
        ws2.Row(rowIndex).Height = 30;
        maize_rm_header1_1.Style.Font.Color.SetColor(Color.White);
        maize_rm_header1_1.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        maize_rm_header1_1.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        maize_rm_header1_1.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        maize_rm_header1_1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        maize_rm_header1_1.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        maize_rm_header1_1.Style.Font.Bold = true;
        maize_rm_header1_1.Value = "RAW MATERIAL";
        colIndex++;

        var maize_rm_header1_2 = ws2.Cells[rowIndex, colIndex];
        maize_rm_header1_2.Style.Font.Color.SetColor(Color.White);
        maize_rm_header1_2.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        maize_rm_header1_2.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        maize_rm_header1_2.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        maize_rm_header1_2.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        maize_rm_header1_2.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        maize_rm_header1_2.Style.Font.Bold = true;
        maize_rm_header1_2.Value = "TONS";
        colIndex++;

        var maize_rm_header1_3 = ws2.Cells[rowIndex, colIndex];
        maize_rm_header1_3.Style.Font.Color.SetColor(Color.White);
        maize_rm_header1_3.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        maize_rm_header1_3.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        maize_rm_header1_3.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        maize_rm_header1_3.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        maize_rm_header1_3.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        maize_rm_header1_3.Style.WrapText = true;
        maize_rm_header1_3.Style.Font.Bold = true;
        maize_rm_header1_3.Value = "% MAKEUP IN FEEDS";

        colIndex++;
        colIndex++;

        var maize_rm_header2_1 = ws2.Cells[rowIndex, colIndex];
        maize_rm_header2_1.Style.Font.Color.SetColor(Color.White);
        maize_rm_header2_1.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        maize_rm_header2_1.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        maize_rm_header2_1.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        maize_rm_header2_1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        maize_rm_header2_1.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        maize_rm_header2_1.Style.WrapText = true;
        maize_rm_header2_1.Style.Font.Bold = true;
        maize_rm_header2_1.Value = "TONS";

        colIndex++;

        var maize_rm_header2_2 = ws2.Cells[rowIndex, colIndex];
        maize_rm_header2_2.Style.Font.Color.SetColor(Color.White);
        maize_rm_header2_2.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        maize_rm_header2_2.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        maize_rm_header2_2.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        maize_rm_header2_2.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        maize_rm_header2_2.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        maize_rm_header2_2.Style.WrapText = true;
        maize_rm_header2_2.Style.Font.Bold = true;
        maize_rm_header2_2.Value = "% MAKEUP IN FEEDS";

        rowIndex++;
        colIndex = startColIndex;
        _startRow = rowIndex;

        //var ggg = reportGroups.Where(x => x.ReportDisplayGroup == 2 || x.ReportDisplayGroup == 5).ToList();

        foreach (var group in reportGroups.Where(x => x.FeedsalesReportGroupCategoryId == 2 || x.FeedsalesReportGroupCategoryId == 1 && (x.ReportDisplayGroup == 2 || x.ReportDisplayGroup == 5))) //get list for maize only
        {
          var maize_name = ws2.Cells[rowIndex, colIndex];
          maize_name.Style.Font.Color.SetColor(Color.Black);
          maize_name.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          maize_name.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
          maize_name.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          maize_name.Style.Fill.BackgroundColor.SetColor(Color.White);
          maize_name.Value = group.Name;

          colIndex++;

          var maize_tons = ws2.Cells[rowIndex, colIndex];
          maize_tons.Style.Font.Color.SetColor(Color.Black);
          maize_tons.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          maize_tons.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          maize_tons.Style.Fill.BackgroundColor.SetColor(Color.White);
          maize_tons.Style.Numberformat.Format = "#,#";

          try
          {
            var totalValue = _applicationLayer.GetTotalRawMaterialUsageByYear(reportCriteria.StartDate, formattedEndDate, group.Id, group.ReportDisplayGroup, 1, reportCriteria.MemberBranchId);
            maize_tons.Value = totalValue;
          }
          catch
          {
            maize_tons.Value = 0;
          }

          colIndex++;

          var maize_usage_percentage = ws2.Cells[rowIndex, colIndex];
          maize_usage_percentage.Style.Font.Color.SetColor(Color.Black);
          maize_usage_percentage.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          maize_usage_percentage.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          maize_usage_percentage.Style.Fill.BackgroundColor.SetColor(Color.White);
          maize_usage_percentage.Style.Numberformat.Format = "#,#0.00%";
          maize_usage_percentage.Formula =
            $"{ws2.Cells[rowIndex, colIndex - 1].Address}/'Raw Material Statistics'!{totalRawMaterialUsage}";

          colIndex++;
          colIndex++;

          //oilcake totals for previous year
          var maize_tons_previous = ws2.Cells[rowIndex, colIndex];
          maize_tons_previous.Style.Font.Color.SetColor(Color.Black);
          maize_tons_previous.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          maize_tons_previous.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          maize_tons_previous.Style.Fill.BackgroundColor.SetColor(Color.White);
          maize_tons_previous.Style.Numberformat.Format = "#,#";

          try
          {
            DateTime sd = reportCriteria.StartDate.AddYears(-1);
            DateTime ed = formattedEndDate.AddYears(-1);
            var totalValue = _applicationLayer.GetTotalRawMaterialUsageByYear(sd, ed, group.Id, group.ReportDisplayGroup, 1, reportCriteria.MemberBranchId);
            maize_tons_previous.Value = totalValue;
          }
          catch
          {
            maize_tons_previous.Value = 0;
          }

          colIndex++;

          var maize_percentage_previous = ws2.Cells[rowIndex, colIndex];
          maize_percentage_previous.Style.Font.Color.SetColor(Color.Black);
          maize_percentage_previous.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          maize_percentage_previous.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          maize_percentage_previous.Style.Fill.BackgroundColor.SetColor(Color.White);
          maize_percentage_previous.Style.Numberformat.Format = "#,#0.00%";
          maize_percentage_previous.Formula = $"{ws2.Cells[rowIndex, colIndex - 1].Address}/'Raw Material Statistics'!{totalRawMaterialUsage}";

          colIndex = startColIndex;
          rowIndex++;
        }
        _endRow = rowIndex;
        colIndex = startColIndex;

        var maize_name_footer = ws2.Cells[rowIndex, colIndex];
        maize_name_footer.Style.Font.Color.SetColor(Color.White);
        maize_name_footer.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        maize_name_footer.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        maize_name_footer.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        maize_name_footer.Style.Font.Bold = true;
        maize_name_footer.Value = "TOTAL MAIZE USED (TONS)";
        colIndex++;

        var maize_used_footer = ws2.Cells[rowIndex, colIndex];
        maize_used_footer.Style.Font.Color.SetColor(Color.White);
        maize_used_footer.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        maize_used_footer.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        maize_used_footer.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        maize_used_footer.Formula = $"=SUM({ws2.Cells[_startRow, 3].Address}:{ws2.Cells[_endRow - 1, 3].Address})";
        maize_used_footer.Style.Font.Bold = true;
        maize_used_footer.Style.Numberformat.Format = "#,#";
        colIndex++;

        var maize_percentage_footer = ws2.Cells[rowIndex, colIndex];
        maize_percentage_footer.Style.Font.Color.SetColor(Color.White);
        maize_percentage_footer.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        maize_percentage_footer.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        maize_percentage_footer.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        maize_percentage_footer.Formula = $"=SUM({ws2.Cells[_startRow, 4].Address}:{ws2.Cells[_endRow - 1, 4].Address})";
        maize_percentage_footer.Style.Font.Bold = true;
        maize_percentage_footer.Style.Numberformat.Format = "#,#0.00%";

        colIndex++;
        colIndex++;

        var maize_used_footer_previous = ws2.Cells[rowIndex, colIndex];
        maize_used_footer_previous.Style.Font.Color.SetColor(Color.White);
        maize_used_footer_previous.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        maize_used_footer_previous.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        maize_used_footer_previous.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        maize_used_footer_previous.Formula = $"=SUM({ws2.Cells[_startRow, 6].Address}:{ws2.Cells[_endRow - 1, 6].Address})";
        maize_used_footer_previous.Style.Font.Bold = true;
        maize_used_footer_previous.Style.Numberformat.Format = "#,#";

        colIndex++;

        var maize_percentage_footer_previous = ws2.Cells[rowIndex, colIndex];
        maize_percentage_footer_previous.Style.Font.Color.SetColor(Color.White);
        maize_percentage_footer_previous.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        maize_percentage_footer_previous.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        maize_percentage_footer_previous.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        maize_percentage_footer_previous.Formula = $"=SUM({ws2.Cells[_startRow, 7].Address}:{ws2.Cells[_endRow - 1, 7].Address})";
        maize_percentage_footer_previous.Style.Font.Bold = true;
        maize_percentage_footer_previous.Style.Numberformat.Format = "#,#0.00%";

        //footer

        colIndex = startColIndex;
        rowIndex++;
        rowIndex++;

        ws2.Cells[rowIndex, colIndex, rowIndex, colIndex + 2].Merge = true;
        var production_header = ws2.Cells[rowIndex, colIndex, rowIndex, colIndex + 2];
        production_header.Style.Font.Color.SetColor(Color.White);
        production_header.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        production_header.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        production_header.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        production_header.Style.Fill.BackgroundColor.SetColor(blueHeader);
        production_header.Style.Font.Bold = true;
        production_header.Value = "RAW MATERIAL USAGE AND FEED PRODUCTION";

        rowIndex++;
        colIndex = startColIndex;

        var maize_product_usage = ws2.Cells[rowIndex, colIndex];
        maize_product_usage.Style.Font.Color.SetColor(Color.White);
        maize_product_usage.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        maize_product_usage.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
        maize_product_usage.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        maize_product_usage.Style.Fill.BackgroundColor.SetColor(silverHeader);
        maize_product_usage.Style.Font.Bold = true;
        maize_product_usage.Value = "MAIZE PRODUCT USAGE";
        colIndex++;

        var maize_product_usage_value = ws2.Cells[rowIndex, colIndex];
        maize_product_usage_value.Style.Font.Color.SetColor(Color.Black);
        maize_product_usage_value.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        maize_product_usage_value.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
        maize_product_usage_value.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        maize_product_usage_value.Style.Fill.BackgroundColor.SetColor(Color.White);
        maize_product_usage_value.Style.Font.Bold = true;
        maize_product_usage_value.Style.Numberformat.Format = "#,#";
        maize_product_usage_value.Formula = $"{"=C36"}";

        colIndex++;
        var maize_product_usage_percent = ws2.Cells[rowIndex, colIndex];
        maize_product_usage_percent.Style.Font.Color.SetColor(Color.Black);
        maize_product_usage_percent.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        maize_product_usage_percent.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
        maize_product_usage_percent.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        maize_product_usage_percent.Style.Fill.BackgroundColor.SetColor(Color.White);
        maize_product_usage_percent.Style.Font.Bold = true;
        maize_product_usage_percent.Style.Numberformat.Format = "#,#0.00%";
        maize_product_usage_percent.Formula = $"{"=D36"}";

        rowIndex++;
        colIndex = startColIndex;

        var fishmeal_usage = ws2.Cells[rowIndex, colIndex];
        fishmeal_usage.Style.Font.Color.SetColor(Color.White);
        fishmeal_usage.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        fishmeal_usage.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
        fishmeal_usage.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        fishmeal_usage.Style.Fill.BackgroundColor.SetColor(silverHeader);
        fishmeal_usage.Style.Font.Bold = true;
        fishmeal_usage.Value = "FISHMEAL USAGE";
        colIndex++;

        var fishmeal_usage_value = ws2.Cells[rowIndex, colIndex];
        fishmeal_usage_value.Style.Font.Color.SetColor(Color.Black);
        fishmeal_usage_value.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        fishmeal_usage_value.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        fishmeal_usage_value.Style.Fill.BackgroundColor.SetColor(Color.White);
        fishmeal_usage_value.Style.Font.Bold = true;
        fishmeal_usage_value.Style.Numberformat.Format = "#,#";

        try
        {
          //DateTime sd = StartDate.AddYears(-1);
          //DateTime ed = EndDate.AddYears(-1);
          var totalValue = _applicationLayer.GetTotalRawMaterialUsageByYear(reportCriteria.StartDate, formattedEndDate, 21, 3, 1, reportCriteria.MemberBranchId); //reportGroupId hardcoded for fish meal
          fishmeal_usage_value.Value = totalValue;
        }
        catch
        {
          fishmeal_usage_value.Value = 0;
        }

        colIndex++;
        var fishmeal_usage_precentage = ws2.Cells[rowIndex, colIndex];
        fishmeal_usage_precentage.Style.Font.Color.SetColor(Color.Black);
        fishmeal_usage_precentage.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        fishmeal_usage_precentage.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
        fishmeal_usage_precentage.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        fishmeal_usage_precentage.Style.Fill.BackgroundColor.SetColor(Color.White);
        fishmeal_usage_precentage.Style.Font.Bold = true;
        fishmeal_usage_precentage.Style.Numberformat.Format = "#,#0.00%";
        fishmeal_usage_precentage.Formula = $"({ws2.Cells[rowIndex, colIndex - 1].Address}/'Raw Material Statistics'!{totalRawMaterialUsage})";

        rowIndex++;
        colIndex = startColIndex;

        var oilcake_product_usage = ws2.Cells[rowIndex, colIndex];
        oilcake_product_usage.Style.Font.Color.SetColor(Color.White);
        oilcake_product_usage.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        oilcake_product_usage.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
        oilcake_product_usage.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        oilcake_product_usage.Style.Fill.BackgroundColor.SetColor(silverHeader);
        oilcake_product_usage.Style.Font.Bold = true;
        oilcake_product_usage.Value = "OILCAKE USAGE";
        colIndex++;

        var oilcake_product_usage_value = ws2.Cells[rowIndex, colIndex];
        oilcake_product_usage_value.Style.Font.Color.SetColor(Color.Black);
        oilcake_product_usage_value.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        oilcake_product_usage_value.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
        oilcake_product_usage_value.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        oilcake_product_usage_value.Style.Fill.BackgroundColor.SetColor(Color.White);
        oilcake_product_usage_value.Style.Font.Bold = true;
        oilcake_product_usage_value.Style.Numberformat.Format = "#,#";
        oilcake_product_usage_value.Formula = $"{"=C19"}";
        colIndex++;

        var oilcake_product_usage_percent = ws2.Cells[rowIndex, colIndex];
        oilcake_product_usage_percent.Style.Font.Color.SetColor(Color.Black);
        oilcake_product_usage_percent.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        oilcake_product_usage_percent.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
        oilcake_product_usage_percent.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        oilcake_product_usage_percent.Style.Fill.BackgroundColor.SetColor(Color.White);
        oilcake_product_usage_percent.Style.Numberformat.Format = "#,#0.00%";
        oilcake_product_usage_percent.Style.Font.Bold = true;
        oilcake_product_usage_percent.Formula = $"{"=D19"}";

        rowIndex++;
        colIndex = startColIndex;

        var total_feedsales = ws2.Cells[rowIndex, colIndex];
        total_feedsales.Style.Font.Color.SetColor(Color.White);
        total_feedsales.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        total_feedsales.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
        total_feedsales.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        total_feedsales.Style.Fill.BackgroundColor.SetColor(silverHeader);
        total_feedsales.Style.Font.Bold = true;
        total_feedsales.Value = "TOTAL FEED SALES";
        colIndex++;

        var total_feedsales_value = ws2.Cells[rowIndex, colIndex];
        total_feedsales_value.Style.Font.Color.SetColor(Color.Black);
        total_feedsales_value.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        total_feedsales_value.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
        total_feedsales_value.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        total_feedsales_value.Style.Fill.BackgroundColor.SetColor(Color.White);
        total_feedsales_value.Style.Font.Bold = true;
        total_feedsales_value.Style.Numberformat.Format = "#,#";
        total_feedsales_value.Formula = $"'{"Feedsales Statistics"}'!{totalFeedsalesCurrent}";

        rowIndex++;

        mainBorderEndRow = rowIndex - 1;
        mainBorderEndCol = 7;

        //draw the main thick border
        ws2.Cells[2, 2, mainBorderEndRow, mainBorderEndCol].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thick);

        #endregion

        return p.GetAsByteArray();
      }
    }

    /// <inheritdoc cref="GetRawMaterialUsageReport"/>
    public async Task<byte[]> GetRawMaterialUsageReport(int rawMaterialId, DateTime startDate, DateTime endDate)
    {
      var rawMaterial = await _applicationLayer.GetRawMaterial(rawMaterialId).ConfigureAwait(false);

      var result =
        await _applicationRepository.QueryAsync<FeedsalesReportItem>(x =>
          x.Material!.Id == rawMaterialId && x.FeedsalesReport.InfoFor >= startDate &&
          x.FeedsalesReport.InfoFor <= endDate.AddHours(23)).ConfigureAwait(false);

      var reportData = new List<RawMaterialUsageReportViewModel>();

      foreach (var reportItem in result)
      {
        reportData.Add(new RawMaterialUsageReportViewModel
        {
          InfoFor = reportItem.FeedsalesReport.InfoFor,
          MemberBranchId = reportItem.MemberBranch.Id,
          MemberBranchName = reportItem.MemberBranch.Name,
          MemberId = reportItem.Member.Id,
          MemberName = reportItem.Member.Name,
          MaterialId = reportItem.Material.Id,
          MaterialName = $"{reportItem.Material.Name} ({reportItem.Material.Code})",
          Value = reportItem.Value!,
          ReportId = reportItem.FeedsalesReport.Id,
        });
      }

      var maroonHeader = ColorTranslator.FromHtml("#c0504d");
      var blueHeader = ColorTranslator.FromHtml("#4f81bd");
      var silverHeader = ColorTranslator.FromHtml("#999999");
      var greenHeader = ColorTranslator.FromHtml("#82bc4f");
      var startRow = 0;
      var endRow = 0;


      using (ExcelPackage p = new ExcelPackage())
      {
        p.Workbook.Properties.Author = "AFMA";
        p.Workbook.Properties.Title = "AFMA Raw Material Usage Report";
        p.Workbook.Properties.Company = "www.afma.co.za";

        p.Workbook.Worksheets.Add("RM Usage");
        ExcelWorksheet ws = p.Workbook.Worksheets[0];
        ws.Name = "RM Usage";

        //** SET DEFAULT COLUMN WIDTH'S AND HEIGHT's
        ws.Column(1).Width = 12;
        ws.Column(2).Width = 35;
        ws.Row(1).Height = 30;

        var colIndex = 1;
        var rowIndex = 1;
        var startCol = 1;
        var endCol = 1;

        ws.Cells[rowIndex, colIndex, rowIndex, colIndex + 1].Merge = true;
        var row1col1 = ws.Cells[rowIndex, colIndex];
        ws.Row(rowIndex).Height = 30;
        row1col1.Value =
          $"RAW MATERIAL USAGE {startDate:MMM yyyy} - {endDate:MMM yyyy}";
        rowIndex++;
        colIndex = 1;

        //header row
        ws.Column(colIndex).Width = 30;
        var row2col1 = ws.Cells[rowIndex, colIndex];
        row2col1.Style.Font.Color.SetColor(Color.White);
        row2col1.Style.WrapText = true;
        row2col1.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        row2col1.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        row2col1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        row2col1.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        row2col1.Value = "Raw Material";
        colIndex++;

        ws.Column(colIndex).Width = 30;
        var row2col2 = ws.Cells[rowIndex, colIndex];
        row2col2.Style.Font.Color.SetColor(Color.White);
        row2col2.Style.WrapText = true;
        row2col2.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        row2col2.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        row2col2.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        row2col2.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        row2col2.Value = "Member";
        colIndex++;

        ws.Column(colIndex).Width = 20;
        var row2col3 = ws.Cells[rowIndex, colIndex];
        row2col3.Style.Font.Color.SetColor(Color.White);
        row2col3.Style.WrapText = true;
        row2col3.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        row2col3.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        row2col3.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        row2col3.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        row2col3.Value = "Member Branch";
        colIndex++;

        ws.Column(colIndex).Width = 20;
        var row2col4 = ws.Cells[rowIndex, colIndex];
        row2col4.Style.Font.Color.SetColor(Color.White);
        row2col4.Style.WrapText = true;
        row2col4.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        row2col4.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        row2col4.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        row2col4.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        row2col4.Value = "Info For";
        colIndex++;

        ws.Column(colIndex).Width = 20;
        var row2col5 = ws.Cells[rowIndex, colIndex];
        row2col5.Style.Font.Color.SetColor(Color.White);
        row2col5.Style.WrapText = true;
        row2col5.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        row2col5.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        row2col5.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        row2col5.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        row2col5.Value = "Value Submitted";
        colIndex++;

        ws.Column(colIndex).Width = 15;
        var row2col6 = ws.Cells[rowIndex, colIndex];
        row2col6.Style.Font.Color.SetColor(Color.White);
        row2col6.Style.WrapText = true;
        row2col6.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        row2col6.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        row2col6.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        row2col6.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        row2col6.Value = "Report No";

        rowIndex++;
        colIndex = 1;

        foreach (var item in reportData.OrderBy(x => x.MemberName).ThenBy(x => x.MemberBranchId).ThenByDescending(x => x.InfoFor))
        {
          var rm_name = ws.Cells[rowIndex, colIndex];
          rm_name.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          rm_name.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
          rm_name.Style.Font.Color.SetColor(Color.White);
          rm_name.Style.Fill.BackgroundColor.SetColor(silverHeader);
          rm_name.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          rm_name.Value = item.MaterialName;

          colIndex++;

          var member_name = ws.Cells[rowIndex, colIndex];
          member_name.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          member_name.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
          member_name.Style.Font.Color.SetColor(Color.White);
          member_name.Style.Fill.BackgroundColor.SetColor(blueHeader);
          member_name.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          member_name.Value = item.MemberName;

          colIndex++;

          var member_branch_name = ws.Cells[rowIndex, colIndex];
          member_branch_name.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          member_branch_name.Style.Fill.BackgroundColor.SetColor(blueHeader);
          member_branch_name.Style.Font.Color.SetColor(Color.White);
          member_branch_name.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          member_branch_name.Value = item.MemberBranchName;
          colIndex++;

          var infofor_text = ws.Cells[rowIndex, colIndex];
          infofor_text.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          infofor_text.Style.Fill.BackgroundColor.SetColor(blueHeader);
          infofor_text.Style.Font.Color.SetColor(Color.White);
          infofor_text.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          infofor_text.Value = item.InfoForText;
          colIndex++;

          var value_submitted = ws.Cells[rowIndex, colIndex];
          value_submitted.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          value_submitted.Style.Fill.BackgroundColor.SetColor(blueHeader);
          value_submitted.Style.Font.Color.SetColor(Color.White);
          value_submitted.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          value_submitted.Value = item.Value;
          colIndex++;

          var report_id = ws.Cells[rowIndex, colIndex];
          report_id.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          report_id.Style.Fill.BackgroundColor.SetColor(blueHeader);
          report_id.Style.Font.Color.SetColor(Color.White);
          report_id.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          report_id.Value = item.ReportId;

          rowIndex++;
          colIndex = 1;
        }

        return await p.GetAsByteArrayAsync();
      }
    }

    /// <inheritdoc cref="GetFeedsalesReports"/>
    public async Task<IEnumerable<FeedsalesReportLookupModel>> GetFeedsalesReports(int year, int memberBranchId)
    {
      var result = await _applicationRepository.QueryAsync<FeedsalesReportLookup>(x => x.MemberBranch.Id == memberBranchId && x.ReportYear == year);
      return _mapper.Map<IEnumerable<FeedsalesReportLookup>, List<FeedsalesReportLookupModel>>(result);
    }

    /// <inheritdoc cref="GetFeedsalesReports"/>
    public async Task<IEnumerable<FeedsalesReportLookupModel>> GetFeedsalesReports(int year, int month, int memberBranchId)
    {
      var result = await _applicationRepository.QueryAsync<FeedsalesReportLookup>(x => x.MemberBranch.Id == memberBranchId && x.ReportMonth == month && x.ReportYear == year);
      return _mapper.Map<IEnumerable<FeedsalesReportLookup>, List<FeedsalesReportLookupModel>>(result);
    }

    #region Branch Summary Reports

    public async Task<BranchSummaryReportViewModel> GetReportBranchSummaryReport(ModuleEnum module, int year)
    {
      BranchSummaryReportViewModel report = new BranchSummaryReportViewModel
      {
        Module = module.ToString(),
        ReportName = "Branch Summary Report",
        Year = year,
      };

      var branches = await _applicationLayer.GetActiveMemberBranchesSummaryByModule((int)module).ConfigureAwait(false);

      if (branches != null)
      {
        foreach (var branch in branches)
        {
          MemberBranchViewModel memberBranch = new()
          {
            BranchId = branch.Id,
            BranchName = branch.Name,
            MemberName = branch.Member,
          };

          var feedsalesReports = await GetFeedsalesReports(year, branch.Id).ConfigureAwait(false);

          if (feedsalesReports != null)
          {
            for (int i = 1; i < 13; i++)
            {
              if (feedsalesReports.Any(x => x.ReportMonth == i))
              {
                var currentMonth = feedsalesReports.Single(x => x.ReportMonth == i);
                memberBranch.Reports.Add(new ReportViewModel
                {
                  ReportExists = true,
                  ReportId = currentMonth.Id,
                  ReportMonth = currentMonth.ReportMonth,
                  ReportYear = currentMonth.ReportYear,
                  Submitted = currentMonth.IsSubmitted,
                  SubmittedDate = currentMonth.SubmittedDate,
                });
              }
              else
              {
                memberBranch.Reports.Add(new ReportViewModel
                {
                  ReportExists = false,
                  ReportId = 0,
                  ReportMonth = i,
                  ReportYear = year,
                  Submitted = false,
                  SubmittedDate = null,
                });
              }
            }
          }

          report.MemberBranches.Add(memberBranch);
        }
      }

      return report;
    }

    #endregion

    #region RAW MATERIAL AND FINISHED PRODUCT USAGE REPORTS

    public async Task<byte[]> GenerateRawMaterialUsageReport(DateTime startDate, DateTime endDate, int[]? materialId, int[]? memberId)
    {
      var usageResults = GetRawMaterialUsageByMemberByStartAndEndDate(startDate, endDate, materialId, memberId).ToList();
      var reportMonths = GeneralHelpers.MonthsBetween(startDate, endDate);

      var maroonHeader = ColorTranslator.FromHtml("#c0504d");
      var pastelGreyHeader = ColorTranslator.FromHtml("#eaeaea");
      var pastelBlueHeader = ColorTranslator.FromHtml("#9ab7d3");
      var pastelPinkHeader = ColorTranslator.FromHtml("#f5d2d3");
      var tempTotal = 0M;

      using (ExcelPackage p = new ExcelPackage())
      {
        p.Workbook.Properties.Author = "AFMA";
        p.Workbook.Properties.Title = "AFMA Raw Material Usage Report";
        p.Workbook.Properties.Company = "www.afma.co.za";

        p.Workbook.Worksheets.Add("RM Usage");
        ExcelWorksheet ws = p.Workbook.Worksheets[0];
        ws.Name = "RM Usage";

        //** SET DEFAULT COLUMN WIDTH'S AND HEIGHT's
        ws.Column(1).Width = 12;
        ws.Column(2).Width = 40;
        ws.Row(1).Height = 30;

        var colIndex = 1;
        var rowIndex = 1;

        ws.Cells[rowIndex, colIndex, rowIndex, colIndex + 1].Merge = true;
        var row1col1 = ws.Cells[rowIndex, colIndex];
        ws.Row(rowIndex).Height = 30;
        row1col1.Value =
          $"RAW MATERIAL USAGE {startDate:MMM yyyy} - {endDate:MMM yyyy}";
        rowIndex++;
        colIndex = 1;

        //****** HEADER ROW ******//
        ws.Column(colIndex).Width = 30;
        var row2col1 = ws.Cells[rowIndex, colIndex];
        row2col1.Style.Font.Color.SetColor(Color.White);
        row2col1.Style.WrapText = true;
        row2col1.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        row2col1.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        row2col1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        row2col1.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        row2col1.Value = "Raw Material";
        colIndex++;

        ws.Column(colIndex).Width = 30;
        var row2col2 = ws.Cells[rowIndex, colIndex];
        row2col2.Style.Font.Color.SetColor(Color.White);
        row2col2.Style.WrapText = true;
        row2col2.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        row2col2.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        row2col2.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        row2col2.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        row2col2.Value = "Member/Branch";
        colIndex++;

        foreach (var reportMonth in reportMonths)
        {
          ws.Column(colIndex).Width = 20;
          var reportDateColumn = ws.Cells[rowIndex, colIndex];
          reportDateColumn.Style.Font.Color.SetColor(Color.White);
          reportDateColumn.Style.WrapText = true;
          reportDateColumn.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
          reportDateColumn.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
          reportDateColumn.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          reportDateColumn.Style.Fill.BackgroundColor.SetColor(maroonHeader);
          reportDateColumn.Value = reportMonth.ToString("MMM-yyyy");
          colIndex++;
        }

        ws.Column(colIndex).Width = 15;
        var member_total = ws.Cells[rowIndex, colIndex];
        member_total.Style.Font.Color.SetColor(Color.White);
        member_total.Style.WrapText = true;
        member_total.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        member_total.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        member_total.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        member_total.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        member_total.Value = "TOTALS";

        rowIndex++;
        colIndex = 1;

        //****** Data *******//
        foreach (var item in usageResults)
        {
          var rm_name = ws.Cells[rowIndex, colIndex];
          rm_name.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          rm_name.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
          rm_name.Style.Font.Color.SetColor(Color.Black);
          rm_name.Style.Fill.BackgroundColor.SetColor(pastelGreyHeader);
          rm_name.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          rm_name.Value = item.MaterialName;

          colIndex++;

          foreach (var member in item.Members)
          {
            tempTotal = 0;

            var member_name = ws.Cells[rowIndex, colIndex];
            member_name.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            member_name.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            member_name.Style.Font.Color.SetColor(Color.Black);
            member_name.Style.Fill.BackgroundColor.SetColor(pastelPinkHeader);
            member_name.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            member_name.Value = $"{member.MemberName} - {member.MemberBranchName}";

            colIndex++;

            foreach (var month in member.Usage)
            {
              var report_month = ws.Cells[rowIndex, colIndex];
              report_month.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
              report_month.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
              report_month.Style.Font.Color.SetColor(Color.Black);
              report_month.Style.Fill.BackgroundColor.SetColor(Color.White);
              report_month.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              report_month.Value = month.SubmittedValue;
              colIndex++;
              tempTotal += month.SubmittedValue;
            }

            var month_total = ws.Cells[rowIndex, colIndex];
            month_total.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            month_total.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            month_total.Style.Font.Color.SetColor(Color.Black);
            month_total.Style.Fill.BackgroundColor.SetColor(pastelBlueHeader);
            month_total.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            month_total.Value = tempTotal;
            colIndex++;
            rowIndex++;
            colIndex = 2;
          }

          colIndex = 1;

          //****** Yearly totals ********//

          var month_total_header0 = ws.Cells[rowIndex, colIndex];
          month_total_header0.Style.Font.Color.SetColor(Color.Black);
          month_total_header0.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
          month_total_header0.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
          month_total_header0.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          month_total_header0.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          month_total_header0.Style.Fill.BackgroundColor.SetColor(pastelBlueHeader);
          colIndex++;

          var month_total_header1 = ws.Cells[rowIndex, colIndex];
          month_total_header1.Style.Font.Color.SetColor(Color.Black);
          month_total_header1.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
          month_total_header1.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
          month_total_header1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          month_total_header1.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          month_total_header1.Style.Fill.BackgroundColor.SetColor(pastelBlueHeader);
          month_total_header1.Value = "TOTALS";
          colIndex++;

          foreach (var month in reportMonths)
          {
            var month_total = ws.Cells[rowIndex, colIndex];
            month_total.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            month_total.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            month_total.Style.Font.Color.SetColor(Color.Black);
            month_total.Style.Fill.BackgroundColor.SetColor(pastelBlueHeader);
            month_total.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            month_total.Value = usageResults.Where(x => x.MaterialId == item.MaterialId).SelectMany(a => a.Members).SelectMany(b => b.Usage).Where(x => x.InfoFor.Month == month.Month && x.InfoFor.Year == month.Year).Sum(x => x.SubmittedValue);
            colIndex++;
          }

          colIndex = 1;
          rowIndex++;
        }

        return await p.GetAsByteArrayAsync();
      }
    }

    private IEnumerable<RawMaterialUsageViewModel> GetRawMaterialUsageByMemberByStartAndEndDate(DateTime startDate, DateTime endDate, int[] materialId, int[] memberId)
    {
      try
      {
        var reportMonths = GeneralHelpers.MonthsBetween(startDate, endDate);
        var materials = _applicationLayer.GetRawMaterials().Result.OrderBy(name => name.Name);
        var memberBranches = _applicationLayer.GetMemberBranchesByModule((int)ModuleEnum.Feedsales).Result.OrderBy(name => name.Member);
        var materialUsageList = new List<RawMaterialUsageViewModel>();

        var usageResults = _applicationRepository.Session().CreateSQLQuery(
            "DECLARE @return_value int;EXEC @return_value = [dbo].[sp_GetRawMaterialUsageByMemberByStartAndEndDate] @StartDate=:startDate, @EndDate=:endDate, @MaterialIds=:materialId, @MemberIds=:memberId;SELECT 'Return Value' = @return_value")
          .SetDateTime("startDate", startDate)
          .SetDateTime("endDate", endDate)
          .SetParameter("materialId", string.Join(",", materialId))
          .SetParameter("memberId", string.Join(",", memberId))
          .SetResultTransformer(Transformers.AliasToBean<RawMaterialUsageModel>())
          .List<RawMaterialUsageModel>();

        foreach (var material in materials)
        {
          if (usageResults.Any(x => x.MaterialId == material.Id))
          {
            var materialUsage = new RawMaterialUsageViewModel();
            materialUsage.MaterialId = material.Id;
            materialUsage.MaterialName = material.Name!;

            materialUsage.Members = new List<UsageMemberViewModel>();

            foreach (var memberBranch in memberBranches)
            {
              if (usageResults.Any(x => x.MemberBranchId == memberBranch.Id))
              {
                var memberBranchUsage = new UsageMemberViewModel
                {
                  MemberId = memberBranch.MemberId,
                  MemberName = memberBranch.Member ?? string.Empty,
                  MemberBranchId = memberBranch.Id,
                  MemberBranchName = memberBranch.Name ?? string.Empty,
                  Usage = new List<UsageViewModel>()
                };

                foreach (var month in reportMonths)
                {
                  memberBranchUsage.Usage.Add(new UsageViewModel
                  {
                    InfoFor = month,
                    SubmittedValue = usageResults.FirstOrDefault(x => x.MemberBranchId == memberBranch.Id && x.Month == month.Month && x.Year == month.Year && x.MaterialId == material.Id)?.SubmittedValue ?? 0
                  });
                }

                if (memberBranchUsage.TotalSubmittedValue > 0)
                {
                  materialUsage.Members.Add(memberBranchUsage);
                }
              }
            }
            ;

            materialUsageList.Add(materialUsage);
          }
        }

        return materialUsageList;
      }
      catch
      {
        return new List<RawMaterialUsageViewModel>();
      }
    }

    public async Task<byte[]> GenerateFinishedProductUsageReport(DateTime startDate, DateTime endDate, int[] productGroupId, int[] memberId)
    {
      var usageResults = GetFinishedProductUsageByMemberByMonth(startDate, endDate, productGroupId, memberId).ToList();
      var reportMonths = GeneralHelpers.MonthsBetween(startDate, endDate);

      var maroonHeader = ColorTranslator.FromHtml("#c0504d");
      var pastelGreyHeader = ColorTranslator.FromHtml("#eaeaea");
      var pastelBlueHeader = ColorTranslator.FromHtml("#9ab7d3");
      var pastelPinkHeader = ColorTranslator.FromHtml("#f5d2d3");
      var pastelOrangeHeader = ColorTranslator.FromHtml("#f7e1d3");

      var tempTotal = 0M;

      using (ExcelPackage p = new ExcelPackage())
      {
        p.Workbook.Properties.Author = "AFMA";
        p.Workbook.Properties.Title = "AFMA Finished Products Usage Report";
        p.Workbook.Properties.Company = "www.afma.co.za";

        p.Workbook.Worksheets.Add("FP Usage");
        ExcelWorksheet ws = p.Workbook.Worksheets[0];
        ws.Name = "FP Usage";

        //** SET DEFAULT COLUMN WIDTH'S AND HEIGHT's
        ws.Column(1).Width = 12;
        ws.Column(2).Width = 40;
        ws.Row(1).Height = 30;

        var colIndex = 1;
        var rowIndex = 1;

        ws.Cells[rowIndex, colIndex, rowIndex, colIndex + 1].Merge = true;
        var row1col1 = ws.Cells[rowIndex, colIndex];
        ws.Row(rowIndex).Height = 30;
        row1col1.Value =
          $"FINISHED PRODUCT USAGE {startDate:MMM yyyy} - {endDate:MMM yyyy}";
        rowIndex++;
        colIndex = 1;

        //****** HEADER ROW ******//
        ws.Column(colIndex).Width = 30;
        var row2col1 = ws.Cells[rowIndex, colIndex];
        row2col1.Style.Font.Color.SetColor(Color.White);
        row2col1.Style.WrapText = true;
        row2col1.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        row2col1.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        row2col1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        row2col1.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        row2col1.Value = "Product";
        colIndex++;

        ws.Column(colIndex).Width = 30;
        var row2col2 = ws.Cells[rowIndex, colIndex];
        row2col2.Style.Font.Color.SetColor(Color.White);
        row2col2.Style.WrapText = true;
        row2col2.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        row2col2.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        row2col2.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        row2col2.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        row2col2.Value = "Member/Branch";
        colIndex++;

        foreach (var reportMonth in reportMonths)
        {
          ws.Column(colIndex).Width = 20;
          var reportDateColumn = ws.Cells[rowIndex, colIndex];
          reportDateColumn.Style.Font.Color.SetColor(Color.White);
          reportDateColumn.Style.WrapText = true;
          reportDateColumn.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
          reportDateColumn.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
          reportDateColumn.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          reportDateColumn.Style.Fill.BackgroundColor.SetColor(maroonHeader);
          reportDateColumn.Value = reportMonth.ToString("MMM-yyyy");
          colIndex++;
        }

        ws.Column(colIndex).Width = 15;
        var member_total = ws.Cells[rowIndex, colIndex];
        member_total.Style.Font.Color.SetColor(Color.White);
        member_total.Style.WrapText = true;
        member_total.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        member_total.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        member_total.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        member_total.Style.Fill.BackgroundColor.SetColor(maroonHeader);
        member_total.Value = "TOTALS";

        rowIndex++;
        colIndex = 1;

        //****** Data *******//

        foreach (var productGroup in usageResults)
        {
          ws.Cells[rowIndex, colIndex, rowIndex, colIndex + 2 + reportMonths.Count()].Merge = true;
          var group_name = ws.Cells[rowIndex, colIndex];
          ws.Row(rowIndex).Height = 35;
          group_name.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          group_name.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
          group_name.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
          group_name.Style.Font.Color.SetColor(Color.Black);
          group_name.Style.Font.Bold = true;
          group_name.Style.Fill.BackgroundColor.SetColor(pastelOrangeHeader);
          group_name.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          group_name.Value = productGroup.ProductGroupName;

          colIndex = 1;
          rowIndex++;

          foreach (var item in productGroup.Products)
          {
            var product_name = ws.Cells[rowIndex, colIndex];
            product_name.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            product_name.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            product_name.Style.Font.Color.SetColor(Color.Black);
            product_name.Style.Fill.BackgroundColor.SetColor(pastelGreyHeader);
            product_name.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            product_name.Value = item.ProductName;

            colIndex++;

            foreach (var member in item.Members)
            {
              tempTotal = 0;

              var member_name = ws.Cells[rowIndex, colIndex];
              member_name.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
              member_name.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
              member_name.Style.Font.Color.SetColor(Color.Black);
              member_name.Style.Fill.BackgroundColor.SetColor(pastelPinkHeader);
              member_name.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              member_name.Value = $"{member.MemberName} - {member.MemberBranchName}";

              colIndex++;

              foreach (var month in member.Usage)
              {
                var report_month = ws.Cells[rowIndex, colIndex];
                report_month.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                report_month.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                report_month.Style.Font.Color.SetColor(Color.Black);
                report_month.Style.Fill.BackgroundColor.SetColor(Color.White);
                report_month.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                report_month.Value = month.SubmittedValue;
                colIndex++;
                tempTotal += month.SubmittedValue;
              }

              var month_total = ws.Cells[rowIndex, colIndex];
              month_total.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
              month_total.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
              month_total.Style.Font.Color.SetColor(Color.Black);
              month_total.Style.Fill.BackgroundColor.SetColor(pastelBlueHeader);
              month_total.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              month_total.Value = tempTotal;
              colIndex++;
              rowIndex++;
              colIndex = 2;
            }

            colIndex = 1;

            //****** Yearly totals ********//

            var month_total_header0 = ws.Cells[rowIndex, colIndex];
            month_total_header0.Style.Font.Color.SetColor(Color.White);
            month_total_header0.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            month_total_header0.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            month_total_header0.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            month_total_header0.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            month_total_header0.Style.Fill.BackgroundColor.SetColor(pastelBlueHeader);
            colIndex++;

            var month_total_header1 = ws.Cells[rowIndex, colIndex];
            month_total_header1.Style.Font.Color.SetColor(Color.Black);
            month_total_header1.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            month_total_header1.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            month_total_header1.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            month_total_header1.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            month_total_header1.Style.Fill.BackgroundColor.SetColor(pastelBlueHeader);
            month_total_header1.Value = "TOTALS";
            colIndex++;

            foreach (var month in reportMonths)
            {
              var month_total = ws.Cells[rowIndex, colIndex];
              month_total.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
              month_total.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
              month_total.Style.Font.Color.SetColor(Color.Black);
              month_total.Style.Fill.BackgroundColor.SetColor(pastelBlueHeader);
              month_total.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              month_total.Value = usageResults.SelectMany(pr => pr.Products).Where(x => x.ProductId == item.ProductId).SelectMany(a => a.Members).SelectMany(b => b.Usage).Where(x => x.InfoFor.Month == month.Month && x.InfoFor.Year == month.Year).Sum(x => x.SubmittedValue);
              colIndex++;
            }

            colIndex = 1;
            rowIndex++;
          }

        }

        return await p.GetAsByteArrayAsync();
      }
    }

    private IEnumerable<FinishedProductGroupUsageViewModel> GetFinishedProductUsageByMemberByMonth(DateTime startDate, DateTime endDate, int[] productGroupId, int[] memberId)
    {
      try
      {
        var reportMonths = GeneralHelpers.MonthsBetween(startDate, endDate);
        var productGroups = _applicationLayer.GetFinishedProductGroups().Result.OrderBy(name => name.Name);
        var memberBranches = _applicationLayer.GetMemberBranchesByModule((int)ModuleEnum.Feedsales).Result.OrderBy(name => name.Member);
        var productUsageList = new List<FinishedProductGroupUsageViewModel>();

        var usageResults = _applicationRepository.Session().CreateSQLQuery(
            "DECLARE @return_value int;EXEC @return_value = [dbo].[sp_GetFinishedProductUsageByMemberByStartAndEndDate] @StartDate=:startDate, @EndDate=:endDate, @productGroupIds=:productGroupId, @memberIds=:memberId;SELECT 'Return Value' = @return_value")
          .SetDateTime("startDate", startDate)
          .SetDateTime("endDate", endDate)
          .SetParameter("productGroupId", string.Join(",", productGroupId))
          .SetParameter("memberId", string.Join(",", memberId))
          .SetResultTransformer(Transformers.AliasToBean<FinishedProductUsageModel>())
          .List<FinishedProductUsageModel>();

        foreach (var productGroup in productGroups)
        {
          if (usageResults.Any(x => x.ProductGroupId == productGroup.Id))
          {
            var productUsage = new FinishedProductGroupUsageViewModel();
            productUsage.ProductGroupId = productGroup.Id;
            productUsage.ProductGroupName = productGroup.Name!;

            foreach (var product in productGroup.Products)
            {
              if (usageResults.Any(x => x.ProductId == product.Id))
              {
                var finishedProductUsage = new FinishedProductUsageViewModel();
                finishedProductUsage.ProductId = product.Id;
                finishedProductUsage.ProductName = product.Name!;

                finishedProductUsage.Members = new List<UsageMemberViewModel>();

                foreach (var memberBranch in memberBranches)
                {
                  if (usageResults.Any(x => x.MemberBranchId == memberBranch.Id))
                  {
                    var memberBranchUsage = new UsageMemberViewModel
                    {
                      MemberId = memberBranch.MemberId,
                      MemberName = memberBranch.Member ?? string.Empty,
                      MemberBranchId = memberBranch.Id,
                      MemberBranchName = memberBranch.Name ?? string.Empty,
                      Usage = new List<UsageViewModel>()
                    };

                    foreach (var month in reportMonths)
                    {
                      memberBranchUsage.Usage.Add(new UsageViewModel
                      {
                        InfoFor = month,
                        SubmittedValue = usageResults.FirstOrDefault(x => x.MemberBranchId == memberBranch.Id && x.Month == month.Month && x.Year == month.Year && x.ProductId == product.Id)?.SubmittedValue ?? 0
                      });
                    }

                    if (memberBranchUsage.TotalSubmittedValue > 0)
                    {
                      finishedProductUsage.Members.Add(memberBranchUsage);
                    }
                  }

                }
                ;
                productUsage.Products.Add(finishedProductUsage);
              }
            }
            productUsageList.Add(productUsage);
          }
        }

        return productUsageList;
      }
      catch
      {
        return new List<FinishedProductGroupUsageViewModel>();
      }
    }

    #endregion
  }
}
