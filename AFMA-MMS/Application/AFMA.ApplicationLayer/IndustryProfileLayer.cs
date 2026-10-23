namespace AFMA.ApplicationLayer
{
  using AFMA.ApplicationLayer.Helpers;
  using AFMA.ApplicationLayer.Model;
  using AFMA.Data.Entities;
  using AFMA.Data.Entities.Interface;
  using AFMA.Infrastructure.Options;
  using AFMA.Infrastructure.Settings;
  using Alpha.Services.Models.Repository;
  using Alpha.Services.NHibernate.Repository.Services;
  using AutoMapper;
  using Interface;
  using Microsoft.AspNetCore.Mvc.Rendering;
  using Microsoft.Extensions.Options;
  using OfficeOpenXml;
  using System.Drawing;
  using ViewModels;

  /// <summary>
  /// The IndustryProfileLayer class.
  /// </summary>
  public class IndustryProfileLayer : IIndustryProfileLayer
  {
    private readonly IApplicationRepository<SessionFactory> _applicationRepository;
    private readonly IApplicationLayer _applicationLayer;
    private readonly ApplicationSettings _applicationSettings;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="IndustryProfileLayer"/> class.
    /// </summary>
    /// <param name="applicationRepository">The Application Repository.</param>
    /// <param name="applicationSettings">The Application Settings.</param>
    /// <param name="mapper">The Auto Mapper.</param>
    public IndustryProfileLayer(
      IApplicationRepository<SessionFactory> applicationRepository,
      IApplicationLayer applicationLayer,
      IOptions<ApplicationSettings> applicationSettings,
      IMapper mapper)
    {
      this._applicationRepository = applicationRepository;
      this._applicationLayer = applicationLayer;
      this._applicationSettings = applicationSettings.Value;
      this._mapper = mapper;
    }

    #region Select Lists

    public async Task<IEnumerable<SelectListItem>> GenerateReportYearSelectList(int? selectedValue)
    {
      var results = await _applicationRepository.QueryAsync<IndustryProfileReport>();
      var distinctYears = results.Distinct().ToList().OrderBy(x => x.ReportYear).Select(s => s.ReportYear).Distinct();
      return distinctYears.ToList().ToSelectList(x => x, x => x, selectedValue);
    }

    #endregion

    #region Reports

    public async Task<IEnumerable<IndustryProfileReportModel>> GetIndustryProfileReports(int memberId, int memberBranchId, int startYear, int endYear, int memberTypeId)
    {
      var result = await _applicationRepository.QueryAsync<IndustryProfileReport>(x => x.MemberBranch.Id == memberBranchId && x.ReportYear >= startYear && x.ReportYear <= endYear && x.MemberType.Id == memberTypeId).ConfigureAwait(false);
      return _mapper.Map<IEnumerable<IndustryProfileReport>, List<IndustryProfileReportModel>>(result);
    }

    public async Task<IEnumerable<IndustryProfileReportModel>> GetIndustryProfileReports(int reportYear)
    {
      var result = await _applicationRepository.QueryAsync<IndustryProfileReport>(x => x.ReportYear == reportYear).ConfigureAwait(false);
      return _mapper.Map<IEnumerable<IndustryProfileReport>, List<IndustryProfileReportModel>>(result);
    }

    public async Task<SaveOrUpdateResult> CreateReport(int memberId, int memberBranchId, int memberAccountId, int reportYear, int memberTypeId, string userId, string fullName)
    {
      var industryProfileReport = new IndustryProfileReportModel
      {
        AnnualProductionAverage = 7000000,
        MemberId = memberId,
        MemberBranchId = memberBranchId,
        MemberAccountId = memberAccountId,
        MemberTypeId = memberTypeId,
        ReportYear = reportYear,
        Submitted = false,
        CreatedBy = userId,
        Active = true,
      };

      var report = await _applicationRepository
        .SaveOrUpdateAndCommitAsync(
          _mapper.Map<IndustryProfileReportModel, IndustryProfileReport>(industryProfileReport)).ConfigureAwait(false);

      // Save IP Base Items
      var baseItems = await _applicationRepository.QueryAsync<IpBaseItem>().ConfigureAwait(false);
      foreach (var item in baseItems.Where(x => x.Active))
      {
        var memberIpBaseItem = new MemberIpBaseItemModel
        {
          IndustryProfileReportId = report.SavedObjectId,
          MemberId = memberId,
          MemberBranchId = memberBranchId,
          IpBaseGroupId = item.IpBaseGroup.Id,
          IpBaseItemId = item.Id,
          Value = 0,
          ItemDescription = item.ItemName,
          DisplayOrder = 0,
          Active = true,
        };

        var result = await _applicationRepository.SaveOrUpdateAndCommitAsync(_mapper.Map<MemberIpBaseItemModel, MemberIpBaseItem>(memberIpBaseItem)).ConfigureAwait(false);
      }

      // Save IP Admin Items
      var adminItems = await _applicationRepository.QueryAsync<IpAdminItem>().ConfigureAwait(false);
      foreach (var item in adminItems.Where(x => x.Active))
      {
        var memberIpAdminItem = new MemberIpAdminItemModel
        {
          IndustryProfileReportId = report.SavedObjectId,
          MemberId = memberId,
          MemberBranchId = memberBranchId,
          IpAdminGroupId = item.IpAdminGroup.Id,
          IpAdminItemId = item.Id,
          Value = 0,
          ItemDescription = item.ItemName,
          DisplayOrder = 0,
          Active = true,
        };

        var result = await _applicationRepository.SaveOrUpdateAndCommitAsync(_mapper.Map<MemberIpAdminItemModel, MemberIpAdminItem>(memberIpAdminItem)).ConfigureAwait(false);
      }

      // Save IP Admin Items
      var additionalItems = await _applicationRepository.QueryAsync<IpAdditionalItem>().ConfigureAwait(false);
      foreach (var item in additionalItems.Where(x => x.Active))
      {
        var memberIpAdditionalItem = new MemberIpAdditionalItemModel
        {
          IndustryProfileReportId = report.SavedObjectId,
          MemberId = memberId,
          MemberBranchId = memberBranchId,
          IpAdditionalGroupId = item.IpAdditionalGroup.Id,
          IpAdditionalItemId = item.Id,
          Value = 0,
          ItemDescription = item.ItemName,
          DisplayOrder = 0,
          Active = true,
        };

        var result = await _applicationRepository.SaveOrUpdateAndCommitAsync(_mapper.Map<MemberIpAdditionalItemModel, MemberIpAdditionalItem>(memberIpAdditionalItem)).ConfigureAwait(false);
      }

      // Save Running Costs
      var runningCostGroups = await _applicationRepository.QueryAsync<RunningCostGroup>().ConfigureAwait(false);
      foreach (var item in runningCostGroups.Where(x => x.Active))
      {
        var memberRunningCost = new MemberRunningCostModel
        {
          IndustryProfileReportId = report.SavedObjectId,
          MemberId = memberId,
          MemberBranchId = memberBranchId,
          AdministrationValue = 0,
          SalesValue = 0,
          ProductionValue = 0,
          RoadTransportValue = 0,
          RunningCostGroupId = item.Id,
          RunningCostId = null,
          ItemDescription = item.GroupName,
          DisplayOrder = 0,
          Active = true,
          CalculationType = item.CalculationType
        };

        var result1 = await _applicationRepository.SaveOrUpdateAndCommitAsync(_mapper.Map<MemberRunningCostModel, MemberRunningCost>(memberRunningCost)).ConfigureAwait(false);

        if (item.GroupHasSubItems)
        {
          foreach (var subItem in item.RunningCosts.Where(x => x.Active))
          {
            var memberRunningCostSubItems = new MemberRunningCostModel
            {
              IndustryProfileReportId = report.SavedObjectId,
              MemberId = memberId,
              MemberBranchId = memberBranchId,
              AdministrationValue = 0,
              SalesValue = 0,
              ProductionValue = 0,
              RoadTransportValue = 0,
              RunningCostGroupId = item.Id,
              RunningCostId = subItem.Id,
              ItemDescription = subItem.ItemName,
              DisplayOrder = 0,
              Active = true,
              CalculationType = item.CalculationType
            };

            var result2 = await _applicationRepository.SaveOrUpdateAndCommitAsync(_mapper.Map<MemberRunningCostModel, MemberRunningCost>(memberRunningCostSubItems)).ConfigureAwait(false);
          }
        }
      }

      // Save Running Costs Rented Fleet
      var runningCostFleet = await _applicationRepository.QueryAsync<RunningCostRentedFleet>().ConfigureAwait(false);
      foreach (var item in runningCostFleet.Where(x => x.Active))
      {
        var memberRunningCostFleet = new MemberRunningCostFleetModel
        {
          IndustryProfileReportId = report.SavedObjectId,
          MemberId = memberId,
          MemberBranchId = memberBranchId,
          RunningCostRentedFleetId = item.Id,
          Value = 0,
          ItemDescription = item.ItemName,
          DisplayOrder = 0,
          Active = true,
        };

        var result = await _applicationRepository.SaveOrUpdateAndCommitAsync(_mapper.Map<MemberRunningCostFleetModel, MemberRunningCostFleet>(memberRunningCostFleet)).ConfigureAwait(false);
      }

      // Save Running Costs Sundries
      var runningCostSundryExpenses = await _applicationRepository.QueryAsync<RunningCostSundryExpense>().ConfigureAwait(false);
      foreach (var item in runningCostSundryExpenses.Where(x => x.Active))
      {
        var memberRunningCostSundry = new MemberRunningCostSundryModel
        {
          IndustryProfileReportId = report.SavedObjectId,
          MemberId = memberId,
          MemberBranchId = memberBranchId,
          RunningCostSundryExpenseId = item.Id,
          ProductionValue = 0,
          SalesValue = 0,
          AdministrationValue = 0,
          RoadTransportValue = 0,
          ReportYear = 0,
          ItemDescription = item.ItemName,
          DisplayOrder = 0,
          Active = true,
        };

        var result = await _applicationRepository.SaveOrUpdateAndCommitAsync(_mapper.Map<MemberRunningCostSundryModel, MemberRunningCostSundry>(memberRunningCostSundry)).ConfigureAwait(false);
      }

      // Save PCB cost items
      var pcbCostItems = await _applicationRepository.QueryAsync<PcbCostItem>().ConfigureAwait(false);
      foreach (var item in pcbCostItems.Where(x => x.Active))
      {
        var memberPcbCostItem = new MemberPcbCostItemModel
        {
          IndustryProfileReportId = report.SavedObjectId,
          MemberId = memberId,
          MemberBranchId = memberBranchId,
          PcbCostItemGroupId = item.PcbCostItemGroup.Id,
          PcbCostMainGroupId = item.PcbCostItemGroup.PcbCostSubGroup.PcbCostMainGroup.Id,
          PcbCostItemId = item.Id,
          Value = 0,
          ItemDescription = item.ItemDescription,
          Active = true,
          DisplayOrder = 0,
          DisplayValue = 0,
          Readonly = item.ReadOnly,
          AddToTotal = item.AddToTotal,
        };

        var result = await _applicationRepository.SaveOrUpdateAndCommitAsync(_mapper.Map<MemberPcbCostItemModel, MemberPcbCostItem>(memberPcbCostItem)).ConfigureAwait(false);
      }

      return new SaveOrUpdateResult
      {
        SavedObjectId = report.SavedObjectId,
        ErrorMessage = null,
        Success = report.Succeeded,
      };
    }

    public async Task<RepositoryResult> DeleteReport(int reportId)
    {
      var report = await _applicationRepository.QuerySingleAsync<IndustryProfileReport>(x => x.Id == reportId);
      return _applicationRepository.DeleteAndCommit(report);
    }

    public async Task<SaveOrUpdateResult> UpdateReportStatus(int id, bool submitStatus)
    {
      var report = await _applicationRepository.QuerySingleAsync<IndustryProfileReport>(x => x.Id == id);

      if (submitStatus)
      {
        report.Submitted = true;
        //report.SubmittedDate = DateTime.Now;
      }
      else
      {
        report.Submitted = false;
        //report.SubmittedDate = null;
      }

      var result = await _applicationRepository.SaveOrUpdateAndCommitAsync(report);

      return new SaveOrUpdateResult
      {
        Success = result.Succeeded,
        ErrorMessage = result.ExceptionMessage,
        SavedObjectId = result.SavedObjectId,
      };
    }

    public async Task<IEnumerable<int>> GetReportYears(int startYear, int endYear, int memberId, int memberBranchId)
    {
      var result = await _applicationLayer.GetIndustryProfileReports(memberId, memberBranchId, true)
        .ConfigureAwait(false);

      return result.Where(x => x.ReportYear >= startYear && x.ReportYear <= endYear)
        .OrderBy(x => x.ReportYear).Select(a => a.ReportYear).Distinct().ToList();
    }

    public async Task<IEnumerable<CompanyIndustryProfileDashboardViewModel>> GetIndustryProfileReportsDashboard(int memberId, int memberBranchId, int yearStart, int yearEnd, int memberTypeId)
    {
      var dashboardViewModel = new List<CompanyIndustryProfileDashboardViewModel>();

      var reportYears = await GetReportYears(yearStart, yearEnd, memberId, memberBranchId).ConfigureAwait(false);

      //var reportsInRange = GetIndustryProfileReports(yearStart).Result.Select(a => a.Id).ToArray();

      var dashboardReports = new List<DashboardIndustryProfileViewModel>();

      foreach (int year in reportYears)
      {
        dashboardReports.Add(new DashboardIndustryProfileViewModel()
        {
          Year = year,
          CompanyIndustryProfileModel = await GetAllDashboardByYear(memberId, memberBranchId, year, memberTypeId)
        });
      }

      //need to get a list of all reports between dates
      //need to get a list of all the items available in all companies tables

      #region INDUSTRY PROFILE

      #region IP BASE

      var ipBaseItems = await _applicationLayer.GetIpBaseItems(false).ConfigureAwait(false);

      foreach (var item in ipBaseItems)
      {
        CompanyIndustryProfileDashboardViewModel vm = new CompanyIndustryProfileDashboardViewModel();
        vm.MemberTypeId = memberTypeId;
        vm.Type = "IndustryProfileBase";
        vm.GroupId = item.IpBaseGroupId;
        vm.GroupName = item.IpBaseGroup;
        vm.ItemDescription = item.ItemName;
        vm.ItemId = item.Id;
        vm.ReportYears = reportYears.ToList();

        foreach (int year in reportYears)
        {
          var yearModel = new CompanyIndustryProfileDashboardYearViewModel
          {
            Year = year,
            Value = dashboardReports.Where(x => x.Year == year)
              .SelectMany(s => s.CompanyIndustryProfileModel.MemberIpBaseItems)
              .Where(x => x.IpBaseItemId == item.Id).Sum(z => z.Value)
          };
          vm.Years.Add(yearModel);
        }

        dashboardViewModel.Add(vm);
      }

      #endregion

      #region IP ADMINISTRATION

      var ipAdminItems = await _applicationLayer.GetIpAdminItems().ConfigureAwait(false);

      foreach (var item in ipAdminItems)
      {
        var vm = new CompanyIndustryProfileDashboardViewModel();
        vm.MemberTypeId = memberTypeId;
        vm.Type = "IndustryProfileAdmin";
        vm.GroupId = item.IpAdminGroupId;
        vm.GroupName = item.IpAdminGroup;
        vm.ItemDescription = item.ItemName;
        vm.ItemId = item.Id;
        vm.ReportYears = reportYears.ToList();

        foreach (Int32 year in reportYears)
        {
          var yearModel = new CompanyIndustryProfileDashboardYearViewModel();
          yearModel.Year = year;
          //yearModel.Value = reports.CompanyIPAdminItems.Where(x => x.ItemId == item.Id).Sum(s => s.Value);
          yearModel.Value = dashboardReports.Where(x => x.Year == year)
              .SelectMany(s => s.CompanyIndustryProfileModel.MemberIpAdminItems)
              .Where(x => x.IpAdminItemId == item.Id).Sum(z => z.Value);
          vm.Years.Add(yearModel);
        }

        dashboardViewModel.Add(vm);
      }

      #endregion

      #region IP ADDITIONAL

      var ipAdditionalItems = await _applicationLayer.GetIpAdditionalItems(false).ConfigureAwait(false);

      foreach (var item in ipAdditionalItems)
      {
        var vm = new CompanyIndustryProfileDashboardViewModel();
        vm.Type = "IndustryProfileAdditional";
        vm.MemberTypeId = memberTypeId;
        vm.GroupId = item.IpAdditionalGroupId;
        vm.GroupName = item.IpAdditionalGroup;
        vm.ItemDescription = item.ItemName;
        vm.ItemId = item.Id;
        vm.ReportYears = reportYears.ToList();

        foreach (Int32 year in reportYears)
        {
          var yearModel = new CompanyIndustryProfileDashboardYearViewModel();
          yearModel.Year = year;
          yearModel.Value = dashboardReports.Where(x => x.Year == year)
              .SelectMany(s => s.CompanyIndustryProfileModel.MemberIpAdditionalItems)
              .Where(x => x.IpAdditionalItemId == item.Id).Sum(z => z.Value);
          vm.Years.Add(yearModel);
        }

        dashboardViewModel.Add(vm);
      }

      #endregion

      #endregion

      #region RAW MATERIAL TOTAL COST

      var rmTotalCostItems = await _applicationLayer.GetRawMaterialCost(false).ConfigureAwait(false);

      foreach (var item in rmTotalCostItems)
      {
        var vm = new CompanyIndustryProfileDashboardViewModel();
        vm.MemberTypeId = memberTypeId;
        vm.Type = "RawMaterialTotalCost";
        vm.GroupId = 0;
        vm.GroupName = "";
        vm.ItemDescription = item.ItemName;
        vm.ItemId = item.Id;
        vm.ReportYears = reportYears.ToList();

        foreach (int year in reportYears)
        {
          var yearModel = new CompanyIndustryProfileDashboardYearViewModel();
          yearModel.Year = year;
          yearModel.Value = dashboardReports.Where(x => x.Year == year)
              .SelectMany(s => s.CompanyIndustryProfileModel.MemberTotalRawMaterialCosts)
              .Where(x => x.RawMaterialTotalCostId == item.Id).Sum(z => z.Value);
          vm.Years.Add(yearModel);
        }

        dashboardViewModel.Add(vm);
      }

      #endregion

      #region RUNNING COST

      var runningCosts = await _applicationLayer.GetRunningCostGroups(false).ConfigureAwait(false);

      foreach (var item in runningCosts)
      {
        var vm = new CompanyIndustryProfileDashboardViewModel();
        vm.MemberTypeId = memberTypeId;
        vm.Type = "RunningCosts";
        vm.GroupId = item.Id;
        vm.GroupName = "";
        vm.ItemDescription = item.GroupName;
        vm.ProductionEnabled = item.ProductionEnabled;
        vm.SalesEnabled = item.SalesEnabled;
        vm.AdministrationEnabled = item.AdministrationEnabled;
        vm.RoadTransportEnabled = item.RoadTransportEnabled;
        vm.ItemId = item.Id;
        vm.ReportYears = reportYears.ToList();
        vm.CalculationType = (CalculationType)item.CalculationType;

        foreach (Int32 year in reportYears)
        {
          if (item.CalculationType == (int)CalculationType.Value)
          {
            if (item.Id == 12) //used to add Coal and Furnance Oil, Steam and Electricty as one total
            {
              var r = dashboardReports.Where(x => x.Year == year)
                  .Select(a => a.CompanyIndustryProfileModel).SelectMany(b => b.MemberRunningCosts)
                  .Where(y => y.GroupId == item.Id).ToList();

              var yearModel = new CompanyIndustryProfileDashboardYearViewModel();
              yearModel.Year = year;
              List<MemberRunningCostViewModel> _yearModel = dashboardReports.Where(x => x.Year == year)
                  .Select(a => a.CompanyIndustryProfileModel).SelectMany(b => b.MemberRunningCosts).Where(y => y.GroupId == item.Id).ToList();
              var _subTotal = _yearModel.SelectMany(p => p.SubItems).Sum(a => a.ProductionValue) +
                                  _yearModel.SelectMany(s => s.SubItems).Sum(a => a.SalesValue) +
                                  _yearModel.SelectMany(a => a.SubItems).Sum(a => a.AdministrationValue) +
                                  _yearModel.SelectMany(t => t.SubItems).Sum(a => a.RoadTransportValue);
              yearModel.Value = _subTotal;
              yearModel.ProductionValue =
                  _yearModel.SelectMany(z => z.SubItems).Sum(a => a.ProductionValue);
              yearModel.SalesValue = _yearModel.SelectMany(c => c.SubItems).Sum(a => a.SalesValue);
              yearModel.AdministrationValue =
                  _yearModel.SelectMany(d => d.SubItems).Sum(a => a.AdministrationValue);
              yearModel.RoadTransportValue =
                  _yearModel.SelectMany(s => s.SubItems).Sum(a => a.RoadTransportValue);
              vm.Years.Add(yearModel);
            }
            else
            {
              CompanyIndustryProfileDashboardYearViewModel yearModel = new CompanyIndustryProfileDashboardYearViewModel();
              yearModel.Year = year;
              List<MemberRunningCostViewModel> _yearModel = dashboardReports.Where(x => x.Year == year)
                  .Select(a => a.CompanyIndustryProfileModel).SelectMany(b => b.MemberRunningCosts)
                  .Where(y => y.GroupId == item.Id).ToList();
              Decimal _subTotal = _yearModel.Sum(a => a.ProductionValue) +
                                  _yearModel.Sum(a => a.SalesValue) +
                                  _yearModel.Sum(a => a.AdministrationValue) +
                                  _yearModel.Sum(a => a.RoadTransportValue);
              yearModel.Value = _subTotal;
              yearModel.ProductionValue = _yearModel.Sum(a => a.ProductionValue);
              yearModel.SalesValue = _yearModel.Sum(a => a.SalesValue);
              yearModel.AdministrationValue = _yearModel.Sum(a => a.AdministrationValue);
              yearModel.RoadTransportValue = _yearModel.Sum(a => a.RoadTransportValue);
              vm.Years.Add(yearModel);
            }
          }

          if (item.CalculationType == (int)CalculationType.Formula)
          {
            if (item.Id == 27)
            {
              var yearModel = new CompanyIndustryProfileDashboardYearViewModel();
              yearModel.Year = year;
              var _productionValue = await GetValueFromTable<MemberRunningCostSundry>(0, 0, 0, CostingGroup.ProductionValue, memberId, memberBranchId, year);
              var _administrationValue = await GetValueFromTable<MemberRunningCostSundry>(0, 0, 0, CostingGroup.AdministrationValue, memberId, memberBranchId, year);
              var _salesValue = await GetValueFromTable<MemberRunningCostSundry>(0, 0, 0, CostingGroup.SalesValue, memberId, memberBranchId, year);
              var _roadTransportValue = await GetValueFromTable<MemberRunningCostSundry>(0, 0, 0, CostingGroup.RoadTransportValue, memberId, memberBranchId, year);
              yearModel.Value = _productionValue + _administrationValue + _salesValue + _roadTransportValue;
              vm.Years.Add(yearModel);
            }
          }
        }

        dashboardViewModel.Add(vm);
      }

      #endregion

      #region RUNNING COST RENTED FLEET

      var runningCostsFleet = await _applicationLayer.GetRunningCostRentedFleetItems(false).ConfigureAwait(false);

      foreach (var item in runningCostsFleet)
      {
        var vm = new CompanyIndustryProfileDashboardViewModel();
        vm.MemberTypeId = memberTypeId;
        vm.Type = "RunningCostsFleet";
        vm.GroupId = 0;
        vm.GroupName = "";
        vm.ItemDescription = item.ItemName;
        vm.ItemId = item.Id;
        vm.ReportYears = reportYears.ToList();

        foreach (int year in reportYears)
        {
          var yearModel = new CompanyIndustryProfileDashboardYearViewModel();
          yearModel.Year = year;
          yearModel.Value = dashboardReports.Where(x => x.Year == year)
              .SelectMany(s => s.CompanyIndustryProfileModel.MemberRunningCostsFleet)
              .Where(x => x.RunningCostRentedFleetId == item.Id).Sum(z => z.Value);
          vm.Years.Add(yearModel);
        }

        dashboardViewModel.Add(vm);
      }

      #endregion

      #region RUNNING COST SUNDRIES

      var runningCostsSundry = await _applicationLayer.GetRunningCostSundryExpenseItems(false).ConfigureAwait(false);

      foreach (var item in runningCostsSundry)
      {
        var vm = new CompanyIndustryProfileDashboardViewModel();
        vm.MemberTypeId = memberTypeId;
        vm.Type = "RunningCostsSundry";
        vm.GroupId = 0;
        vm.GroupName = "";
        vm.ItemDescription = item.ItemName;
        vm.ItemId = item.Id;
        vm.ReportYears = reportYears.ToList();
        vm.ProductionEnabled = item.ProductionEnabled;
        vm.SalesEnabled = item.MarketingEnabled;
        vm.AdministrationEnabled = item.AdministrationEnabled;
        vm.RoadTransportEnabled = item.RoadTransportEnabled;

        foreach (int year in reportYears)
        {
          var yearModel = new CompanyIndustryProfileDashboardYearViewModel();
          yearModel.Year = year;
          List<MemberRunningCostSundryModel> _yearModel = dashboardReports.Where(x => x.Year == year)
              .Select(a => a.CompanyIndustryProfileModel).SelectMany(b => b.MemberRunningCostsSundries)
              .Where(y => y.RunningCostSundryExpenseId == item.Id).ToList();
          var _subTotal = _yearModel.Sum(a => a.ProductionValue) + _yearModel.Sum(a => a.SalesValue) +
                              _yearModel.Sum(a => a.AdministrationValue) +
                              _yearModel.Sum(a => a.RoadTransportValue);
          yearModel.Value = _subTotal;
          yearModel.ProductionValue = _yearModel.Sum(a => a.ProductionValue);
          yearModel.SalesValue = _yearModel.Sum(a => a.SalesValue);
          yearModel.AdministrationValue = _yearModel.Sum(a => a.AdministrationValue);
          yearModel.RoadTransportValue = _yearModel.Sum(a => a.RoadTransportValue);
          vm.Years.Add(yearModel);
        }

        dashboardViewModel.Add(vm);
      }

      #endregion

      #region PCB COST

      var pcbCostItems = await _applicationLayer.GetPcbCostItems(false).ConfigureAwait(false);

      foreach (var item in pcbCostItems)
      {
        var vm = new CompanyIndustryProfileDashboardViewModel();
        vm.MemberTypeId = memberTypeId;
        switch (item.PcbCostItemMainGroupId)
        {
          case 1:
            vm.Type = "PCBCostPelleting";
            break;

          case 2:
            vm.Type = "PCBCostCrumbling";
            break;

          case 3:
            vm.Type = "PCBCostBagging";
            break;
        }

        vm.GroupId = item.PcbCostItemGroupId;
        vm.GroupName = "";
        vm.ItemDescription = item.ItemDescription;
        vm.ItemId = item.Id;
        vm.ReportYears = reportYears.ToList();
        vm.AddToTotal = item.AddToTotal;
        vm.Readonly = item.ReadOnly;
        vm.MemberId = memberId;
        vm.MemberBranchId = memberBranchId;

        foreach (int year in reportYears)
        {
          var yearModel = new CompanyIndustryProfileDashboardYearViewModel();
          var t = dashboardReports.Where(x => x.Year == year)
              .SelectMany(s => s.CompanyIndustryProfileModel.MemberPcbCostItems).Where(x =>
                  x.PcbCostItemId == item.Id && x.PcbCostMainGroupId == item.PcbCostItemMainGroupId);

          if (memberId > 0 && memberBranchId > 0)
          {
            yearModel.Value = t.Where(x => x.MemberId == memberId && x.MemberBranchId == memberBranchId)
                .Sum(z => z.Value);
          }

          if (memberId > 0 && memberBranchId == 0)
          {
            yearModel.Value = t.Where(x => x.MemberId == memberId).Sum(z => z.Value);
          }

          if (memberId == 0 && memberBranchId == 0)
          {
            yearModel.Value = t.Sum(z => z.Value);
          }

          yearModel.Year = year;
          vm.Years.Add(yearModel);
        }

        dashboardViewModel.Add(vm);
      }

      #endregion

      #region INDUSTRY COSTS

      var industryCostItems = await _applicationLayer.GetIndustryCostItems(false).ConfigureAwait(false);

      foreach (var item in industryCostItems)
      {
        var vm = new CompanyIndustryProfileDashboardViewModel();
        vm.MemberTypeId = memberTypeId;
        vm.Type = "IndustryCost";
        vm.GroupId = item.IndustryCostGroupId;
        vm.GroupName = item.IndustryCostGroup;
        vm.ItemDescription = item.ItemName;
        vm.ItemId = item.Id;
        vm.RowOnIndustryCostSheet = item.RowOnRunningCostSheet;
        vm.ReportYears = reportYears.ToList();
        vm.LinkedId1 = item.LinkedId1;
        vm.SundryItem = item.ItemName.Contains("Sundries");

        foreach (int year in reportYears)
        {
          var yearValue = await GetCostFromTableArray(item.LinkedId1, item.LinkedId2, 0, item.LinkedTable, item.Id,
            item.RunningCostType, vm.SundryItem, year, item.IndustryCostGroupId, memberId, memberBranchId,
            memberTypeId).ConfigureAwait(false);
          var yearModel = new CompanyIndustryProfileDashboardYearViewModel();
          yearModel.Year = year;
          yearModel.Value = yearValue[0] + yearValue[1] + yearValue[2] + yearValue[3];
          yearModel.ProductionValue = yearValue[0];
          yearModel.SalesValue = yearValue[1];
          yearModel.AdministrationValue = yearValue[2];
          yearModel.RoadTransportValue = yearValue[3];

          if (yearModel.ProductionValue == 0 && yearModel.SalesValue == 0 &&
              yearModel.AdministrationValue == 0 && yearModel.RoadTransportValue == 0 && yearModel.Value == 0)
          {
            vm.Years.Add(yearModel);
          }
          else
          {
            vm.Years.Add(yearModel);
          }
        }

        dashboardViewModel.Add(vm);
      }

      #endregion

      return dashboardViewModel;
    }


    public async Task<CompanyIndustryProfileViewModel> GetAllDashboardByYear(int memberId, int memberBranchId, int reportYear, int memberTypeId)
    {
      var vm = new CompanyIndustryProfileViewModel();

      vm.MemberIpBaseItems = new List<MemberIpBaseItemModel>();
      vm.MemberIpBaseItems = await _applicationLayer.GetMemberIpBaseItems(IndustryProfileDisplayType.Dashboard,
        memberId, memberBranchId, 0, memberTypeId, reportYear);

      vm.MemberIpAdminItems = new List<MemberIpAdminItemModel>();
      vm.MemberIpAdminItems = await _applicationLayer.GetMemberIpAdminItems(IndustryProfileDisplayType.Dashboard, memberId, memberBranchId, 0, memberTypeId, reportYear);

      vm.MemberIpAdditionalItems = new List<MemberIpAdditionalItemModel>();
      vm.MemberIpAdditionalItems = await _applicationLayer.GetMemberIpAdditionalItems(IndustryProfileDisplayType.Dashboard, memberId, memberBranchId, 0, memberTypeId, reportYear);

      vm.MemberRunningCosts = new List<MemberRunningCostViewModel>();
      vm.MemberRunningCosts = await _applicationLayer.GetMemberRunningCosts(IndustryProfileDisplayType.Dashboard, memberId, memberBranchId, 0, memberTypeId, reportYear);

      vm.MemberRunningCostsFleet = new List<MemberRunningCostFleetModel>();
      vm.MemberRunningCostsFleet = await _applicationLayer.GetMemberRunningCostsFleet(IndustryProfileDisplayType.Dashboard, memberId, memberBranchId, 0, memberTypeId, reportYear);

      vm.MemberRunningCostsSundries = new List<MemberRunningCostSundryModel>();
      vm.MemberRunningCostsSundries = await _applicationLayer.GetMemberRunningCostsSundries(IndustryProfileDisplayType.Dashboard, memberId, memberBranchId, 0, memberTypeId, reportYear);

      vm.MemberTotalRawMaterialCosts = new List<MemberTotalRawMaterialCostModel>();
      vm.MemberTotalRawMaterialCosts = await _applicationLayer.GetMemberTotalRawMaterialCosts(IndustryProfileDisplayType.Dashboard, memberId, memberBranchId, 0, memberTypeId, reportYear);

      vm.MemberPcbCostItems = new List<MemberPcbCostItemModel>();
      vm.MemberPcbCostItems.AddRange(await _applicationLayer.GetMemberPcbCostItems(PcbItem.Pellet, IndustryProfileDisplayType.Dashboard, memberId, memberBranchId, 0, memberTypeId, reportYear));
      vm.MemberPcbCostItems.AddRange(await _applicationLayer.GetMemberPcbCostItems(PcbItem.Crumbling, IndustryProfileDisplayType.Dashboard, memberId, memberBranchId, 0, memberTypeId, reportYear));
      vm.MemberPcbCostItems.AddRange(await _applicationLayer.GetMemberPcbCostItems(PcbItem.Bagging, IndustryProfileDisplayType.Dashboard, memberId, memberBranchId, 0, memberTypeId, reportYear));

      vm.IndustryCostsGroups = new List<IndustryCostGroupModel>();
      vm.IndustryCostsGroups = await _applicationLayer.GetIndustryCostGroups(false);

      return vm;
    }


    #endregion

    public async Task<IEnumerable<PcbCostMainGroupModel>> GetPcbMainGroups(bool showAll = false)
    {
      var result = await _applicationRepository.QueryAsync<PcbCostMainGroup>();

      if (!showAll)
      {
        result = result.Where(x => x.Active);
      }

      return _mapper.Map<IEnumerable<PcbCostMainGroup>, List<PcbCostMainGroupModel>>(result);
    }

    public async Task<IEnumerable<PcbCostSubGroupModel>> GetPcbSubGroups()
    {
      var result = await _applicationRepository.QueryAsync<PcbCostSubGroup>();
      return _mapper.Map<IEnumerable<PcbCostSubGroup>, List<PcbCostSubGroupModel>>(result);
    }

    public async Task<IEnumerable<IndustryProfileTotalCalculationViewModel>> PopulateDefaultTotalValues(IndustryProfileDisplayType displayType, int memberTypeId, int reportId = 0, int memberBranchId = 0, int reportYear = 0)
    {
      try
      {
        var list = new List<IndustryProfileTotalCalculationViewModel>();
        //Pelleting Values - item group Ids 1 and 2 (Item ids 9,10,11,12,13,14,15,16,17,18,19
        var pelleting = new IndustryProfileTotalCalculationViewModel();

        pelleting.IndustryProfileSection = IndustryProfileSection.Pelleting;
        pelleting.ItemIdsToSum.Add(9);
        pelleting.ItemIdsToSum.Add(10);
        pelleting.ItemIdsToSum.Add(11);
        pelleting.ItemIdsToSum.Add(12);
        pelleting.ItemIdsToSum.Add(13);
        pelleting.ItemIdsToSum.Add(14);
        pelleting.ItemIdsToSum.Add(15);
        pelleting.ItemIdsToSum.Add(16);
        pelleting.ItemIdsToSum.Add(17);
        pelleting.ItemIdsToSum.Add(18);
        pelleting.ItemIdsToSum.Add(19);
        pelleting.ItemIdsToSum.Add(20);
        pelleting.ItemIdsToSum.Add(21);
        pelleting.ItemIdsToSum.Add(22);
        pelleting.ItemIdsToSum.Add(23);
        pelleting.ItemIdsToSum.Add(24);
        pelleting.ItemIdsToSum.Add(25);
        pelleting.ItemIdsToSum.Add(26);
        pelleting.ItemIdsToSum.Add(27);
        pelleting.ItemIdsToSum.Add(28);
        pelleting.ItemIdsToSum.Add(29);
        pelleting.ItemIdsToSum.Add(30);
        pelleting.ItemIdsToSum.Add(31);
        pelleting.ItemIdsToSum.Add(32);
        //pelleting.ItemIdsToSum.Add(97);
        pelleting.ItemIdsToSum.Add(98);
        pelleting.DivideBySection = IndustryProfileSection.IndustryProfile;
        pelleting.DivideById = 9;
        pelleting.ReportYear = reportYear;

        if (displayType == IndustryProfileDisplayType.Other)
        {
          var pp = await _applicationRepository.QueryAsync<MemberPcbCostItem>(x =>
              pelleting.ItemIdsArray.Contains(x.PcbCostItem.Id) && x.MemberBranch.Id == memberBranchId &&
              x.IndustryProfileReport.Id == reportId && x.IndustryProfileReport.MemberType!.Id == memberTypeId)
            .ConfigureAwait(false);

          var dd = await _applicationRepository.QueryAsync<MemberIpBaseItem>(x =>
              x.IpBaseItem.Id == pelleting.DivideById && x.MemberBranch.Id == memberBranchId &&
              x.IndustryProfileReport.Id == reportId && x.IndustryProfileReport.MemberType!.Id == memberTypeId)
            .ConfigureAwait(false);

          var p = pp.Sum(s => s.Value);
          var d = dd.Sum(s => s.Value);

          try
          {
            pelleting.Total = Convert.ToDouble(p / d);
          }
          catch
          {
            pelleting.Total = 0D;
          }
        }
        else
        {
          var pp = await _applicationRepository.QueryAsync<MemberPcbCostItem>(x =>
            pelleting.ItemIdsArray.Contains(x.PcbCostItem.Id) && x.IndustryProfileReport.ReportYear == reportYear &&
            x.IndustryProfileReport.MemberType!.Id == memberTypeId).ConfigureAwait(false);

          var dd = await _applicationRepository.QueryAsync<MemberIpBaseItem>(x =>
            x.IpBaseItem.Id == pelleting.DivideById && x.IndustryProfileReport.ReportYear == reportYear &&
            x.IndustryProfileReport.MemberType!.Id == memberTypeId).ConfigureAwait(false);

          var p = pp.Sum(s => s.Value);
          var d = dd.Sum(s => s.Value);

          try
          {
            pelleting.Total = Convert.ToDouble(p / d);
          }
          catch
          {
            pelleting.Total = 0D;
          }
        }

        list.Add(pelleting);

        //Crumbling Values - item group Ids 7 and 8 (Item ids 9,10,11,12,13,14,15,16,17,18,19
        IndustryProfileTotalCalculationViewModel crumbling = new IndustryProfileTotalCalculationViewModel();
        crumbling.IndustryProfileSection = IndustryProfileSection.Crumbling;
        crumbling.ItemIdsToSum.Add(39);
        crumbling.ItemIdsToSum.Add(40);
        crumbling.ItemIdsToSum.Add(41);
        crumbling.ItemIdsToSum.Add(42);
        crumbling.ItemIdsToSum.Add(43);
        crumbling.ItemIdsToSum.Add(44);
        crumbling.ItemIdsToSum.Add(45);
        crumbling.ItemIdsToSum.Add(46);
        crumbling.ItemIdsToSum.Add(47);
        crumbling.ItemIdsToSum.Add(48);
        crumbling.ItemIdsToSum.Add(49);
        crumbling.ItemIdsToSum.Add(50);
        crumbling.ItemIdsToSum.Add(51);
        crumbling.ItemIdsToSum.Add(52);
        crumbling.ItemIdsToSum.Add(53);
        crumbling.ItemIdsToSum.Add(54);
        crumbling.ItemIdsToSum.Add(55);
        crumbling.ItemIdsToSum.Add(56);
        crumbling.ItemIdsToSum.Add(58);
        //crumbling.ItemIdsToSum.Add(99);
        crumbling.ItemIdsToSum.Add(100);
        crumbling.DivideBySection = IndustryProfileSection.IndustryProfile;
        crumbling.DivideById = 10;
        crumbling.ReportYear = reportYear;

        if (displayType == IndustryProfileDisplayType.Other)
        {
          var pp = await _applicationRepository.QueryAsync<MemberPcbCostItem>(x =>
              crumbling.ItemIdsArray.Contains(x.PcbCostItem.Id) && x.MemberBranch.Id == memberBranchId &&
              x.IndustryProfileReport.Id == reportId && x.IndustryProfileReport.MemberType!.Id == memberTypeId)
            .ConfigureAwait(false);

          var dd = await _applicationRepository.QueryAsync<MemberIpBaseItem>(x =>
              x.IpBaseItem.Id == crumbling.DivideById && x.MemberBranch.Id == memberBranchId &&
              x.IndustryProfileReport.Id == reportId && x.IndustryProfileReport.MemberType!.Id == memberTypeId)
            .ConfigureAwait(false);

          var p = pp.Sum(s => s.Value);
          var d = dd.Sum(s => s.Value);

          try
          {
            crumbling.Total = Convert.ToDouble(p / d);
          }
          catch
          {
            crumbling.Total = 0D;
          }
        }
        else
        {
          var pp = await _applicationRepository.QueryAsync<MemberPcbCostItem>(x =>
            crumbling.ItemIdsArray.Contains(x.PcbCostItem.Id) && x.IndustryProfileReport.ReportYear == reportYear &&
            x.IndustryProfileReport.MemberType!.Id == memberTypeId).ConfigureAwait(false);

          var dd = await _applicationRepository.QueryAsync<MemberIpBaseItem>(x =>
            x.IpBaseItem.Id == crumbling.DivideById && x.IndustryProfileReport.ReportYear == reportYear &&
            x.IndustryProfileReport.MemberType!.Id == memberTypeId).ConfigureAwait(false);

          var p = pp.Sum(s => s.Value);
          var d = dd.Sum(s => s.Value);

          try
          {
            crumbling.Total = Convert.ToDouble(p / d);
          }
          catch
          {
            crumbling.Total = 0D;
          }
        }

        list.Add(crumbling);

        //Crumbling Values - item group Ids 7 and 8 (Item ids 9,10,11,12,13,14,15,16,17,18,19
        IndustryProfileTotalCalculationViewModel bagging = new IndustryProfileTotalCalculationViewModel();
        bagging.IndustryProfileSection = IndustryProfileSection.Bagging;
        bagging.ItemIdsToSum.Add(70);
        bagging.ItemIdsToSum.Add(71);
        bagging.ItemIdsToSum.Add(72);
        bagging.ItemIdsToSum.Add(73);
        bagging.ItemIdsToSum.Add(74);
        bagging.ItemIdsToSum.Add(75);
        bagging.ItemIdsToSum.Add(76);
        bagging.ItemIdsToSum.Add(77);
        bagging.ItemIdsToSum.Add(78);
        bagging.ItemIdsToSum.Add(79);
        bagging.ItemIdsToSum.Add(80);
        bagging.ItemIdsToSum.Add(81);
        bagging.ItemIdsToSum.Add(82);
        bagging.ItemIdsToSum.Add(83);
        bagging.ItemIdsToSum.Add(84);
        bagging.ItemIdsToSum.Add(85);
        bagging.ItemIdsToSum.Add(86);
        bagging.ItemIdsToSum.Add(87);
        bagging.ItemIdsToSum.Add(88);
        bagging.ItemIdsToSum.Add(89);
        bagging.ItemIdsToSum.Add(90);
        bagging.ItemIdsToSum.Add(91);
        bagging.ItemIdsToSum.Add(92);
        bagging.ItemIdsToSum.Add(93);
        //bagging.ItemIdsToSum.Add(101);
        bagging.ItemIdsToSum.Add(102);
        bagging.DivideBySection = IndustryProfileSection.IndustryProfile;
        bagging.DivideById = 6;
        bagging.ReportYear = reportYear;

        if (displayType == IndustryProfileDisplayType.Other)
        {
          var pp = await _applicationRepository.QueryAsync<MemberPcbCostItem>(x =>
              bagging.ItemIdsArray.Contains(x.PcbCostItem.Id) && x.MemberBranch.Id == memberBranchId &&
              x.IndustryProfileReport.Id == reportId && x.IndustryProfileReport.MemberType!.Id == memberTypeId)
            .ConfigureAwait(false);

          var dd = await _applicationRepository.QueryAsync<MemberIpBaseItem>(x =>
              x.IpBaseItem.Id == bagging.DivideById && x.MemberBranch.Id == memberBranchId &&
              x.IndustryProfileReport.Id == reportId && x.IndustryProfileReport.MemberType!.Id == memberTypeId)
            .ConfigureAwait(false);

          var p = pp.Sum(s => s.Value);
          var d = dd.Sum(s => s.Value);

          try
          {
            bagging.Total = Convert.ToDouble(p / d);
          }
          catch
          {
            bagging.Total = 0D;
          }
        }
        else
        {
          var pp = await _applicationRepository.QueryAsync<MemberPcbCostItem>(x =>
            bagging.ItemIdsArray.Contains(x.PcbCostItem.Id) && x.IndustryProfileReport.ReportYear == reportYear &&
            x.IndustryProfileReport.MemberType!.Id == memberTypeId).ConfigureAwait(false);

          var dd = await _applicationRepository.QueryAsync<MemberIpBaseItem>(x =>
            x.IpBaseItem.Id == bagging.DivideById && x.IndustryProfileReport.ReportYear == reportYear &&
            x.IndustryProfileReport.MemberType!.Id == memberTypeId).ConfigureAwait(false);

          var p = pp.Sum(s => s.Value);
          var d = dd.Sum(s => s.Value);

          try
          {
            bagging.Total = Convert.ToDouble(p / d);
          }
          catch
          {
            bagging.Total = 0D;
          }
        }

        list.Add(bagging);

        return list;
      }
      catch
      {
        throw new Exception("NO DATA FOR REPORT<br/><br/>Ensure there are report data for the date range you specified!");
      }
    }

    public async Task<decimal> GetValueFromMemberIpBaseTable(int reportId, int pcbMainGroupItemId, int[] years, int year = 0)
    {
      if (year == 0)
      {
        switch (pcbMainGroupItemId)
        {
          case 1:
            try
            {
              var s1 = await _applicationRepository.QueryAsync<MemberIpBaseItem>(x =>
                x.IndustryProfileReport.Id == reportId && x.IpBaseItem.Id == 9 && x.IpBaseGroup.Id == 3 &&
                x.IndustryProfileReport.Submitted).ConfigureAwait(false);
              return s1.Sum(sum => sum.Value);
            }
            catch
            {
              return 0;
            }
          case 2:
            try
            {
              var s2 = await _applicationRepository.QueryAsync<MemberIpBaseItem>(x =>
                x.IndustryProfileReport.Id == reportId && x.IpBaseItem.Id == 10 && x.IpBaseGroup.Id == 3 &&
                x.IndustryProfileReport.Submitted).ConfigureAwait(false);
              return s2.Sum(sum => sum.Value);
            }
            catch
            {
              return 0;
            }
          case 3:
            try
            {
              var s3 = await _applicationRepository.QueryAsync<MemberIpBaseItem>(x =>
                x.IndustryProfileReport.Id == reportId && x.IpBaseItem.Id == 6 && x.IpBaseGroup.Id == 2 &&
                x.IndustryProfileReport.Submitted).ConfigureAwait(false);
              return s3.Sum(sum => sum.Value);
            }
            catch
            {
              return 0;
            }
          default:
            return 0;
        }
      }
      else if (year == 999)
      {
        var average = 0M;

        foreach (int yr in years)
        {
          switch (pcbMainGroupItemId)
          {
            case 1:
              try
              {
                average += _applicationRepository.Query<MemberIpBaseItem>(x =>
                  x.IndustryProfileReport.ReportYear == yr && x.IpBaseItem.Id == 9 && x.IpBaseGroup.Id == 3 &&
                  x.IndustryProfileReport.Submitted).Sum(sum => sum.Value);
              }
              catch
              {
                return 0;
              }
              break;
            case 2:
              try
              {
                average += _applicationRepository.Query<MemberIpBaseItem>(x =>
                  x.IndustryProfileReport.ReportYear == yr && x.IpBaseItem.Id == 10 && x.IpBaseGroup.Id == 3 &&
                  x.IndustryProfileReport.Submitted).Sum(sum => sum.Value);
              }
              catch
              {
                return 0;
              }
              break;
            case 3:
              try
              {
                average += _applicationRepository.Query<MemberIpBaseItem>(x =>
                  x.IndustryProfileReport.ReportYear == yr && x.IpBaseItem.Id == 6 && x.IpBaseGroup.Id == 2 &&
                  x.IndustryProfileReport.Submitted).Sum(sum => sum.Value);
              }
              catch
              {
                return 0;
              }

              break;
            default:
              return 0;
          }
        }

        return decimal.Round(average / years.Length, 0);
      }
      else
      {
        try
        {
          switch (pcbMainGroupItemId)
          {
            case 1:
              try
              {
                return _applicationRepository.Query<MemberIpBaseItem>(x =>
                  x.IndustryProfileReport.ReportYear == year && x.IpBaseItem.Id == 9 && x.IpBaseGroup.Id == 3 &&
                  x.IndustryProfileReport.Submitted).Sum(sum => sum.Value);
              }
              catch
              {
                return 0;

              }

            case 2:
              try
              {
                return _applicationRepository.Query<MemberIpBaseItem>(x =>
                  x.IndustryProfileReport.ReportYear == year && x.IpBaseItem.Id == 10 && x.IpBaseGroup.Id == 3 &&
                  x.IndustryProfileReport.Submitted).Sum(sum => sum.Value);
              }
              catch
              {
                return 0;
              }

            case 3:
              try
              {
                return _applicationRepository.Query<MemberIpBaseItem>(x =>
                  x.IndustryProfileReport.ReportYear == year && x.IpBaseItem.Id == 6 && x.IpBaseGroup.Id == 2 &&
                  x.IndustryProfileReport.Submitted).Sum(sum => sum.Value);
              }
              catch
              {
                return 0;
              }

            default:
              return 0;
          }

          //return repo.GetAll().Where(x => x.IPCBReport.ReportYear == year && x.ItemGroupId == pcbMainGroupItemId && x.IPCBReport.Submitted == true).Sum(sum => sum.Value);
        }
        catch
        {
          return 0;
        }
      }
    }

    #region Running Costs Sundry Expenses

    public async Task<IEnumerable<RunningCostSundryExpenseModel>> GetRunningCostSundryExpense(bool showAll = true)
    {
      var result = await _applicationRepository.QueryAsync<RunningCostSundryExpense>().ConfigureAwait(false);

      if (showAll)
      {
        return _mapper.Map<IEnumerable<RunningCostSundryExpense>, List<RunningCostSundryExpenseModel>>(
          result.OrderBy(n => n.ItemName));
      }

      return _mapper.Map<IEnumerable<RunningCostSundryExpense>, List<RunningCostSundryExpenseModel>>(
        result.Where(x => x.Active).OrderBy(n => n.ItemName));
    }

    #endregion

    #region Pellet Crumbling and Baggging

    public async Task<IEnumerable<PcbCostSubGroupModel>> GetPcbCostSubGroups(bool showAll = true)
    {
      var result = await _applicationRepository.QueryAsync<PcbCostSubGroup>().ConfigureAwait(false);

      if (showAll)
      {
        return _mapper.Map<IEnumerable<PcbCostSubGroup>, List<PcbCostSubGroupModel>>(
          result.OrderBy(n => n.SubGroupName));
      }

      return _mapper.Map<IEnumerable<PcbCostSubGroup>, List<PcbCostSubGroupModel>>(
        result.Where(x => x.Active).OrderBy(n => n.SubGroupName));
    }

    #endregion

    #region Running Costs

    public async Task<IEnumerable<RunningCostGroupModel>> GetRunningCostsGroups(bool showAll = true)
    {
      var result = await _applicationRepository.QueryAsync<RunningCostGroup>().ConfigureAwait(false);

      if (showAll)
      {
        return _mapper.Map<IEnumerable<RunningCostGroup>, List<RunningCostGroupModel>>(
          result.OrderBy(n => n.DisplayOrder));
      }

      return _mapper.Map<IEnumerable<RunningCostGroup>, List<RunningCostGroupModel>>(
        result.Where(x => x.Active).OrderBy(n => n.DisplayOrder));
    }

    #endregion

    #region Settings

    public async Task<int> GetAnnualProductionAverage()
    {
      var result = await _applicationRepository
        .QuerySingleAsync<IndustryProfileSetting>(x => x.SettingName == "AnnualProductionAverage")
        .ConfigureAwait(false);
      return Convert.ToInt32(result.SettingValue);
    }

    private string GenerateMainTitle(string sheetName)
    {
      return $"AFMA Industry Benchmark {sheetName}";
    }

    private string GenerateSubTitle(string sheetName)
    {
      return $"{sheetName}";
    }

    #endregion

    #region Excel Reports

    public async Task<byte[]> GenerateIndustryProfileExcelReport(ReportCriteriaModel reportCriteria)
    {
      var memberType = "";

      reportCriteria.MemberTypeId ??= _applicationLayer.GetMemberMemberType((int)reportCriteria.MemberId).Result.Id;

      if (reportCriteria.MemberTypeId != null)
      {
        memberType = $" - {await _applicationLayer.GetMemberType((int)reportCriteria.MemberTypeId)}";
      }

      var reports = await GetIndustryProfileReportsDashboard((int)reportCriteria.MemberId, (int)reportCriteria.MemberBranchId, reportCriteria.StartYear, reportCriteria.EndYear, (int)reportCriteria.MemberTypeId).ConfigureAwait(false);

      var ipBaseGroups = await _applicationLayer.GetIpBaseGroups(false).ConfigureAwait(false);
      var ipAdminGroups = await _applicationLayer.GetIpAdminGroups(false).ConfigureAwait(false);
      var ipAdditionalGroups = await _applicationLayer.GetIpAdditionalGroups(false).ConfigureAwait(false);
      //var runningCostGroups = await _applicationLayer.GetIpBaseGroups(false).ConfigureAwait(false);
      var pcbMainGroups = await GetPcbMainGroups(false).ConfigureAwait(false);
      var industryGroups = await _applicationLayer.GetIndustryCostGroups(false).ConfigureAwait(false);
      var annualProductionAverage = await GetAnnualProductionAverage().ConfigureAwait(false);

      //ViewBag.ReportYears = new Service.IndustryProfileService().GetReportYears();

      var reportYears = await GetReportYears(reportCriteria.StartYear, reportCriteria.EndYear, (int)reportCriteria.MemberId, (int)reportCriteria.MemberBranchId).ConfigureAwait(false);

      //Total Cost Calculation
      var _totalPelletingCost = 0.00;
      var _totalCrumblingCost = 0.00;
      var _totalBaggingCost = 0.00;

      foreach (var year in reportYears)
      {
        _totalPelletingCost += PopulateDefaultTotalValues(IndustryProfileDisplayType.Dashboard, (int)reportCriteria.MemberTypeId, 0, 0, year).Result.Where(x => x.IndustryProfileSection == IndustryProfileSection.Pelleting).Sum(s => s.Total);
        _totalCrumblingCost += PopulateDefaultTotalValues(IndustryProfileDisplayType.Dashboard, (int)reportCriteria.MemberTypeId, 0, 0, year).Result.Where(x => x.IndustryProfileSection == IndustryProfileSection.Crumbling).Sum(s => s.Total);
        _totalBaggingCost += PopulateDefaultTotalValues(IndustryProfileDisplayType.Dashboard, (int)reportCriteria.MemberTypeId, 0, 0, year).Result.Where(x => x.IndustryProfileSection == IndustryProfileSection.Bagging).Sum(s => s.Total);
      }

      using (ExcelPackage p = new ExcelPackage())
      {
        p.Workbook.Properties.Author = "AFMA";
        p.Workbook.Properties.Title = "Industry Profiles and Costing Benchmarks";
        p.Workbook.Properties.Company = "AFMA";

        p.Workbook.Worksheets.Add("Industry Profile");
        p.Workbook.Worksheets.Add("Industry Cost");
        p.Workbook.Worksheets.Add("Running Costs");
        p.Workbook.Worksheets.Add("Running Costs - Sundries");
        p.Workbook.Worksheets.Add("Pellet Cost");
        p.Workbook.Worksheets.Add("Crumbling Cost");
        p.Workbook.Worksheets.Add("Bagging Cost");

        ExcelWorksheet ip = p.Workbook.Worksheets[0];
        ip.Name = "Industry Profile";
        ExcelWorksheet ic = p.Workbook.Worksheets[1];
        ic.Name = "Industry Cost";
        ExcelWorksheet rc = p.Workbook.Worksheets[2];
        rc.Name = "Running Costs";
        ExcelWorksheet rcs = p.Workbook.Worksheets[3];
        rcs.Name = "Running Costs - Sundries";
        ExcelWorksheet pcbP = p.Workbook.Worksheets[4];
        pcbP.Name = "Pellet Cost";
        ExcelWorksheet pcbC = p.Workbook.Worksheets[5];
        pcbC.Name = "Crumbling Cost";
        ExcelWorksheet pcbB = p.Workbook.Worksheets[6];
        pcbB.Name = "Bagging Cost";

        Int32 rowIndex = 1;
        Int32 colIndex = 1;

        float _headerFontSize = 17F;
        float _subheaderFontSize = 15F;
        float _groupHeaderFontSize = 14F;
        float _normalHeader = 13F;
        Boolean _headerBold = true;
        Color _headerColour = ColorTranslator.FromHtml("#c0bd66");

        #region Industry Profile

        ip.Column(1).Width = 70;
        ip.Column(2).Width = 18;
        ip.Column(3).Width = 15;
        ip.Column(4).Width = 15;
        ip.Column(5).Width = 15;
        ip.Column(6).Width = 15;
        ip.Column(7).Width = 15;
        ip.Column(8).Width = 15;
        ip.Column(9).Width = 15;
        ip.Column(10).Width = 15;

        #region Base

        rowIndex = 1;
        colIndex = 1;
        ip.Cells[rowIndex, colIndex].Value = GenerateMainTitle(memberType);
        ip.Cells[rowIndex, colIndex].Style.Font.Bold = _headerBold;
        ip.Cells[rowIndex, colIndex].Style.Font.Size = _headerFontSize;
        rowIndex++;
        rowIndex++;

        colIndex = 1;

        ip.Cells[rowIndex, colIndex].Style.Font.Bold = true;
        ip.Cells[rowIndex, colIndex].Style.Font.Size = _groupHeaderFontSize;
        ip.Cells[rowIndex, colIndex].Value = "BASE INFORMATION - ANNUAL";

        rowIndex++;
        colIndex = 1;

        foreach (var group in ipBaseGroups)
        {

          ip.Cells[rowIndex, colIndex].Style.Font.Bold = true;
          ip.Cells[rowIndex, colIndex].Style.Font.Size = _normalHeader;
          ip.Cells[rowIndex, colIndex].Value = group.GroupName;

          rowIndex++;
          colIndex = 1;


          ip.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          ip.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          ip.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
          ip.Cells[rowIndex, colIndex].Value = "Description";

          colIndex++;

          foreach (Int32 year in reportYears)
          {
            ip.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            ip.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            ip.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
            ip.Cells[rowIndex, colIndex].Value = year;

            colIndex++;
          }

          rowIndex++;
          colIndex = 1;

          foreach (var item in reports.Where(x => x.Type == "IndustryProfileBase" && x.GroupId == group.Id))
          {
            ip.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            ip.Cells[rowIndex, colIndex].Value = item.ItemDescription;

            colIndex++;

            foreach (var year in reportYears)
            {
              ip.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              ip.Cells[rowIndex, colIndex].Value = item.Years.Where(x => x.Year == year).Sum(d => d.Value);

              colIndex++;
            }
            rowIndex++;
            colIndex = 1;

          }

          //TOTALS

          ip.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          ip.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          ip.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
          if (group.Id == 1)
          {
            ip.Cells[rowIndex, colIndex].Value = "% Mill Utilisation";
          }
          else
          {
            ip.Cells[rowIndex, colIndex].Value = "TOTAL";
          }

          colIndex++;

          foreach (var year in reportYears)
          {
            ip.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            ip.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            ip.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);

            if (group.Id == 1)
            {
              try
              {
                ip.Cells[rowIndex, colIndex].Style.Numberformat.Format = "#0";
                ip.Cells[rowIndex, colIndex].Formula = $"={ip.Cells[rowIndex - 1, colIndex]}/{ip.Cells[rowIndex - 2, colIndex]}*100";
              }
              catch
              {
                ip.Cells[rowIndex, colIndex].Value = "0";
              }
            }
            else
            {
              var c = reports.Count(x => x.Type == "IndustryProfileBase" && x.GroupId == group.Id);
              ip.Cells[rowIndex, colIndex].Formula = $"SUM({ip.Cells[rowIndex - c, colIndex]}:{ip.Cells[rowIndex - 1, colIndex]})";
            }

            colIndex++;
          }


          rowIndex++;
          rowIndex++;
          colIndex = 1;
        }


        colIndex++;

        #endregion

        colIndex = 1;
        rowIndex++;
        rowIndex++;

        #region Admin

        ip.Cells[rowIndex, colIndex].Style.Font.Bold = true;
        ip.Cells[rowIndex, colIndex].Style.Font.Size = _groupHeaderFontSize;
        ip.Cells[rowIndex, colIndex].Value = "BASE ADMINISTERED COST COMPONENTS (ANNUAL)";

        rowIndex++;
        colIndex = 1;

        foreach (var group in ipAdminGroups)
        {

          ip.Cells[rowIndex, colIndex].Style.Font.Bold = true;
          ip.Cells[rowIndex, colIndex].Style.Font.Size = _normalHeader;
          ip.Cells[rowIndex, colIndex].Value = group.GroupName;

          rowIndex++;
          colIndex = 1;


          ip.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          ip.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          ip.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
          ip.Cells[rowIndex, colIndex].Value = "Description";

          colIndex++;

          //ip.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          //ip.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          //ip.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
          //ip.Cells[rowIndex, colIndex].Value = "Average";

          //colIndex++;

          foreach (Int32 year in reportYears)
          {
            ip.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            ip.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            ip.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
            ip.Cells[rowIndex, colIndex].Value = year;

            colIndex++;
          }

          rowIndex++;
          colIndex = 1;

          foreach (var item in reports.Where(x => x.Type == "IndustryProfileAdmin" && x.GroupId == group.Id))
          {
            ip.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            ip.Cells[rowIndex, colIndex].Value = item.ItemDescription;

            colIndex++;

            //ip.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            //ip.Cells[rowIndex, colIndex].Formula =
            //    $"@SUM({ip.Cells[rowIndex, colIndex + 1]}:{ip.Cells[rowIndex, colIndex + reportYears.Count()]})/{reportYears.Count()}";


            //colIndex++;

            foreach (var year in reportYears)
            {
              ip.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              ip.Cells[rowIndex, colIndex].Value = item.Years.Where(x => x.Year == year).Sum(d => d.Value);

              colIndex++;
            }
            rowIndex++;
            colIndex = 1;

          }

          //TOTALS

          ip.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          ip.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          ip.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
          ip.Cells[rowIndex, colIndex].Value = "TOTAL";

          colIndex++;

          //ip.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          //ip.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          //ip.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
          //ip.Cells[rowIndex, colIndex].Formula =
          //    $"@SUM({ip.Cells[rowIndex, colIndex + 1]}:{ip.Cells[rowIndex, colIndex + reportYears.Count()]})/{reportYears.Count()}";

          //colIndex++;

          foreach (Int32 year in reportYears)
          {
            ip.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            ip.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            ip.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);

            var c = reports.Count(x => x.Type == "IndustryProfileAdmin" && x.GroupId == group.Id);
            ip.Cells[rowIndex, colIndex].Formula = $"SUM({ip.Cells[rowIndex - c, colIndex]}:{ip.Cells[rowIndex - 1, colIndex]})";

            colIndex++;
          }

          rowIndex++;
          rowIndex++;
          colIndex = 1;
        }

        colIndex++;

        #endregion

        colIndex = 1;
        rowIndex++;
        rowIndex++;

        #region Additional

        ip.Cells[rowIndex, colIndex].Style.Font.Bold = true;
        ip.Cells[rowIndex, colIndex].Style.Font.Size = _groupHeaderFontSize;
        ip.Cells[rowIndex, colIndex].Value = "ADDITIONAL INFORMATION";

        rowIndex++;
        colIndex = 1;

        foreach (var group in ipAdditionalGroups)
        {

          ip.Cells[rowIndex, colIndex].Style.Font.Bold = true;
          ip.Cells[rowIndex, colIndex].Style.Font.Size = _normalHeader;
          ip.Cells[rowIndex, colIndex].Value = group.GroupName;

          rowIndex++;
          colIndex = 1;


          ip.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          ip.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          ip.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
          ip.Cells[rowIndex, colIndex].Value = "Description";

          colIndex++;

          foreach (Int32 year in reportYears)
          {
            ip.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            ip.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            ip.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
            ip.Cells[rowIndex, colIndex].Value = year;

            colIndex++;
          }

          rowIndex++;
          colIndex = 1;

          foreach (var item in reports.Where(x => x.Type == "IndustryProfileAdditional" && x.GroupId == group.Id))
          {
            ip.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            ip.Cells[rowIndex, colIndex].Value = item.ItemDescription;

            colIndex++;

            foreach (var year in reportYears)
            {
              ip.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              ip.Cells[rowIndex, colIndex].Value = item.Years.Where(x => x.Year == year).Sum(d => d.Value);

              colIndex++;
            }
            rowIndex++;
            colIndex = 1;

          }

          //TOTALS

          if (group.Id < 4)
          {
            ip.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            ip.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            ip.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
            ip.Cells[rowIndex, colIndex].Value = "TOTAL";

            colIndex++;

            foreach (Int32 year in reportYears)
            {
              ip.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              ip.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
              ip.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
              var c = reports.Count(x => x.Type == "IndustryProfileAdditional" && x.GroupId == group.Id);
              ip.Cells[rowIndex, colIndex].Formula = $"SUM({ip.Cells[rowIndex - c, colIndex]}:{ip.Cells[rowIndex - 1, colIndex]})";


              colIndex++;
            }

            if (group.Id == 1)
            {
              colIndex = 1;
              rowIndex++;
              ip.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              ip.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
              ip.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
              ip.Cells[rowIndex, colIndex].Value = "Investment per ton feed manufactured - R/ton";
              colIndex++;

              foreach (var year in reportYears)
              {
                ip.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                ip.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                ip.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
                ip.Cells[rowIndex, colIndex].Style.Numberformat.Format = "#0";
                ip.Cells[rowIndex, colIndex].Formula = $"={ip.Cells[rowIndex - 1, colIndex]}/{ip.Cells[14, colIndex]}";
                colIndex++;
              }

              rowIndex++;
              colIndex = 1;
              ip.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              ip.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
              ip.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
              ip.Cells[rowIndex, colIndex].Value = "Total Investment in feed manufacturing - As Industry per Annum (RAND)";
              colIndex++;

              foreach (var year in reportYears)
              {
                ip.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                ip.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                ip.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
                ip.Cells[rowIndex, colIndex].Style.Numberformat.Format = "#0";
                ip.Cells[rowIndex, colIndex].Formula = $"={ip.Cells[rowIndex - 1, colIndex]}*{annualProductionAverage}";
                colIndex++;
              }
            }

            if (group.Id == 2)
            {
              colIndex = 1;
              rowIndex++;
              ip.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              ip.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
              ip.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
              ip.Cells[rowIndex, colIndex].Value = "Raw Material Conversion rate to Feed (%)";
              colIndex++;

              foreach (var year in reportYears)
              {
                ip.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                ip.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                ip.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
                ip.Cells[rowIndex, colIndex].Style.Numberformat.Format = "##.## %";
                ip.Cells[rowIndex, colIndex].Formula = $"={ip.Cells[rowIndex - 1, colIndex]}/{ip.Cells[7, colIndex]}";
                colIndex++;
              }
            }

            if (group.Id == 3)
            {
              colIndex = 1;
              rowIndex++;
              ip.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              ip.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
              ip.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
              ip.Cells[rowIndex, colIndex].Value = "Total Average Raw Material Cost (R/ton)";
              colIndex++;

              foreach (var year in reportYears)
              {
                ip.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                ip.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                ip.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
                ip.Cells[rowIndex, colIndex].Style.Numberformat.Format = "R #0";
                ip.Cells[rowIndex, colIndex].Formula = $"={ip.Cells[rowIndex - 1, colIndex]}/{ip.Cells[112, colIndex]}";
                colIndex++;
              }
            }
          }

          rowIndex++;
          rowIndex++;
          colIndex = 1;
        }

        colIndex++;


        #endregion


        #endregion

        #region Industry Cost

        var icStartCol = 2;

        ic.Column(1).Width = 96;
        ic.Column(2).Width = 15;
        ic.Column(3).Width = 15;
        ic.Column(4).Width = 15;
        ic.Column(5).Width = 15;
        ic.Column(6).Width = 15;
        ic.Column(7).Width = 15;
        ic.Column(8).Width = 15;
        ic.Column(9).Width = 15;
        ic.Column(10).Width = 15;

        rowIndex = 1;
        colIndex = 1;
        ic.Cells[rowIndex, colIndex].Value = GenerateMainTitle(memberType);
        ic.Cells[rowIndex, colIndex].Style.Font.Bold = _headerBold;
        ic.Cells[rowIndex, colIndex].Style.Font.Size = _headerFontSize;

        rowIndex++;
        rowIndex++;

        colIndex = 1;

        ic.Cells[rowIndex, colIndex].Style.Font.Bold = true;
        ic.Cells[rowIndex, colIndex].Style.Font.Size = _groupHeaderFontSize;
        ic.Cells[rowIndex, colIndex].Value = "INDUSTRY COST";


        rowIndex++;
        colIndex = 1;

        foreach (var group in industryGroups)
        {
          ic.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          ic.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          ic.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
          ic.Cells[rowIndex, colIndex].Style.Font.Bold = true;
          ic.Cells[rowIndex, colIndex].Style.Font.Size = _normalHeader;
          ic.Cells[rowIndex, colIndex].Value = group.GroupName;

          colIndex++;

          foreach (Int32 year in reportYears)
          {
            ic.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            ic.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            ic.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
            ic.Cells[rowIndex, colIndex].Value = "";
            colIndex++;
          }

          rowIndex++;
          colIndex = 1;

          ic.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          ic.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          ic.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
          ic.Cells[rowIndex, colIndex].Style.Font.Size = _normalHeader;
          ic.Cells[rowIndex, colIndex].Value = "Description";

          colIndex++;

          foreach (Int32 year in reportYears)
          {
            ic.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            ic.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            ic.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
            ic.Cells[rowIndex, colIndex].Value = year;

            colIndex++;
          }

          rowIndex++;
          colIndex = 1;

          foreach (var item in reports.Where(x => x.Type == "IndustryCost" && x.GroupId == group.Id))
          {
            ic.Cells[rowIndex, colIndex].Style.Border
                .BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            ic.Cells[rowIndex, colIndex].Value = $"{item.ItemDescription}";

            colIndex++;

            var yearCount = 0;

            foreach (var year in reportYears)
            {
              ic.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              ic.Cells[rowIndex, colIndex].Style.Numberformat.Format = "R### ### ### ##0";

              if (item.RowOnIndustryCostSheet < 100)
              {
                if (item.ItemId == 36) //this is to add DIESEL and PETROL to 1 total
                {
                  ic.Cells[rowIndex, colIndex].Formula = $"'Running Costs'!{rc.Cells[item.RowOnIndustryCostSheet, icStartCol + yearCount]}+" +
                                                         $"'Running Costs'!{rc.Cells[item.RowOnIndustryCostSheet + 1, icStartCol + yearCount]}";
                }
                else if (item.ItemId == 46)
                {
                  ic.Cells[rowIndex, colIndex].Formula =
                      $"={ic.Cells[65, icStartCol + yearCount]}+{ic.Cells[70, icStartCol + yearCount]}";
                }
                else

                {
                  ic.Cells[rowIndex, colIndex].Formula = $"'Running Costs'!{rc.Cells[item.RowOnIndustryCostSheet, icStartCol + yearCount]}";
                }
              }
              else
              {
                ic.Cells[rowIndex, colIndex].Formula = $"={ic.Cells[item.RowOnIndustryCostSheet - 100, icStartCol + yearCount]}";
              }

              colIndex++;
              yearCount++;
            }

            colIndex = 1;
            rowIndex++;
          }

          ic.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          ic.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          ic.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
          ic.Cells[rowIndex, colIndex].Value = "TOTAL";

          colIndex++;

          var yearCount2 = 0;

          foreach (Int32 year in reportYears)
          {
            ic.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            ic.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            ic.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
            ic.Cells[rowIndex, colIndex].Style.Numberformat.Format = "R### ### ### ##0";

            var c = reports.Count(x => x.Type == "IndustryCost" && x.GroupId == group.Id);

            if (group.Id == 7)
            {
              ic.Cells[rowIndex, colIndex].Formula =
                  $"={ic.Cells[rowIndex - c, colIndex]}/('Industry Profile'!{rc.Cells[26, icStartCol + yearCount2]}+'Industry Profile'!{rc.Cells[27, icStartCol + yearCount2]}+'Industry Profile'!{rc.Cells[28, icStartCol + yearCount2]})";
              ic.Cells[rowIndex, 1].Value = "FEED DELIVERY COST (R/ton)";
              yearCount2++;
            }
            else
            {
              ic.Cells[rowIndex, colIndex].Formula = $"SUM({ic.Cells[rowIndex - c, colIndex]}:{ic.Cells[rowIndex - 1, colIndex]})";
            }


            colIndex++;
          }

          rowIndex++;
          rowIndex++;
          colIndex = 1;

        }

        #endregion

        #region Running Costs

        var sundriesStartCol = 11;
        var rentedFleetStartCol = 49;
        var totalItems = 0;

        rc.Column(1).Width = 45;
        rc.Column(2).Width = 18;
        rc.Column(3).Width = 15;
        rc.Column(4).Width = 15;
        rc.Column(5).Width = 15;
        rc.Column(6).Width = 15;
        rc.Column(7).Width = 15;
        rc.Column(8).Width = 15;
        rc.Column(9).Width = 15;
        rc.Column(10).Width = 15;

        rowIndex = 1;
        colIndex = 1;
        rc.Cells[rowIndex, colIndex].Value = GenerateMainTitle(memberType);
        rc.Cells[rowIndex, colIndex].Style.Font.Bold = _headerBold;
        rc.Cells[rowIndex, colIndex].Style.Font.Size = _headerFontSize;

        rowIndex++;

        rc.Cells[rowIndex, colIndex].Value = GenerateSubTitle("Running Cost");
        rc.Cells[rowIndex, colIndex].Style.Font.Bold = _headerBold;
        rc.Cells[rowIndex, colIndex].Style.Font.Size = _subheaderFontSize;
        rowIndex++;
        rowIndex++;

        colIndex = 1;

        rc.Cells[rowIndex, colIndex].Style.Font.Bold = true;
        rc.Cells[rowIndex, colIndex].Style.Font.Size = _groupHeaderFontSize;
        rc.Cells[rowIndex, colIndex].Value = "RUNNING COST";

        rowIndex++;
        colIndex = 1;

        rc.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        rc.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        rc.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
        rc.Cells[rowIndex, colIndex].Value = "Running Cost - Sundries";

        colIndex++;

        foreach (Int32 year in reportYears)
        {
          rc.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          rc.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          rc.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
          rc.Cells[rowIndex, colIndex].Value = year;
          colIndex++;
        }

        rowIndex++;
        colIndex = 1;

        foreach (var item in reports.Where(x => x.Type == "RunningCosts"))
        {
          if (item.ProductionEnabled)
          {
            totalItems++;

            rc.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            rc.Cells[rowIndex, colIndex].Value = $"{item.ItemDescription} (Production - Grinding and Mixing)";

            colIndex++;

            var yearCount = 2;

            foreach (var year in reportYears)
            {
              rc.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              rc.Cells[rowIndex, colIndex].Style.Numberformat.Format = "R### ### ### ##0";

              switch (item.ItemId)
              {
                case 27:
                  rc.Cells[rowIndex, colIndex].Formula = $"'Running Costs - Sundries'!{rcs.Cells[7, yearCount]}+" +
                                                         $"'Running Costs - Sundries'!{rcs.Cells[14, yearCount]}";
                  break;

                case 30:
                  rc.Cells[rowIndex, colIndex].Formula = $"={rc.Cells[rentedFleetStartCol, yearCount]}";
                  break;

                default:
                  rc.Cells[rowIndex, colIndex].Value = item.Years.Where(x => x.Year == year).Sum(d => d.ProductionValue);
                  break;
              }

              yearCount++;

              colIndex++;
            }

            rowIndex++;
            colIndex = 1;

          }

          if (item.SalesEnabled)
          {
            totalItems++;

            rc.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            rc.Cells[rowIndex, colIndex].Value = $"{item.ItemDescription} (Sales/Marketing)";

            colIndex++;

            //rc.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            //rc.Cells[rowIndex, colIndex].Style.Numberformat.Format = "R### ### ### ##0";
            //rc.Cells[rowIndex, colIndex].Formula =
            //    $"@SUM({rc.Cells[rowIndex, colIndex + 1]}:{rc.Cells[rowIndex, colIndex + reportYears.Count()]})/{reportYears.Count()}";

            //colIndex++;

            var yearCount = 2;

            foreach (var year in reportYears)
            {
              rc.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              rc.Cells[rowIndex, colIndex].Style.Numberformat.Format = "R### ### ### ##0";

              switch (item.ItemId)
              {
                case 27:
                  rc.Cells[rowIndex, colIndex].Formula =
                      rc.Cells[rowIndex, colIndex].Formula = $"'Running Costs - Sundries'!{rcs.Cells[11, yearCount]}+" +
                                                             $"'Running Costs - Sundries'!{rcs.Cells[15, yearCount]}";
                  break;

                case 30:
                  rc.Cells[rowIndex, colIndex].Formula = $"={rc.Cells[rentedFleetStartCol, yearCount]}";
                  break;

                default:
                  rc.Cells[rowIndex, colIndex].Value = item.Years.Where(x => x.Year == year).Sum(d => d.SalesValue);
                  break;
              }

              yearCount++;

              colIndex++;
            }

            rowIndex++;
            colIndex = 1;

          }

          if (item.AdministrationEnabled)
          {
            totalItems++;

            rc.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            rc.Cells[rowIndex, colIndex].Value = $"{item.ItemDescription} (Administration)";

            colIndex++;

            var yearCount = 2;

            foreach (var year in reportYears)
            {
              rc.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              rc.Cells[rowIndex, colIndex].Style.Numberformat.Format = "R### ### ### ##0";

              switch (item.ItemId)
              {
                case 27:
                  rc.Cells[rowIndex, colIndex].Formula =
                      $"'Running Costs - Sundries'!{rcs.Cells[5, yearCount]}+" +
                      $"'Running Costs - Sundries'!{rcs.Cells[6, yearCount]}+" +
                      $"'Running Costs - Sundries'!{rcs.Cells[8, yearCount]}+" +
                      $"'Running Costs - Sundries'!{rcs.Cells[10, yearCount]}+" +
                      $"'Running Costs - Sundries'!{rcs.Cells[12, yearCount]}+" +
                      $"'Running Costs - Sundries'!{rcs.Cells[16, yearCount]}";
                  break;

                case 30:
                  rc.Cells[rowIndex, colIndex].Formula = $"={rc.Cells[rentedFleetStartCol, yearCount]}";
                  break;

                default:
                  rc.Cells[rowIndex, colIndex].Value = item.Years.Where(x => x.Year == year).Sum(d => d.AdministrationValue);
                  break;
              }

              yearCount++;

              colIndex++;
            }

            rowIndex++;
            colIndex = 1;

          }

          if (item.RoadTransportEnabled)
          {
            totalItems++;

            rc.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            rc.Cells[rowIndex, colIndex].Value = $"{item.ItemDescription} (Road Transport)";

            colIndex++;

            var yearCount = 2;

            foreach (var year in reportYears)
            {
              rc.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              rc.Cells[rowIndex, colIndex].Style.Numberformat.Format = "R### ### ### ##0";

              switch (item.ItemId)
              {
                case 27:
                  rc.Cells[rowIndex, colIndex].Formula =
                      $"'Running Costs - Sundries'!{rcs.Cells[4, yearCount]}+" +
                      $"'Running Costs - Sundries'!{rcs.Cells[9, yearCount]}+" +
                      $"'Running Costs - Sundries'!{rcs.Cells[13, yearCount]}+" +
                      $"'Running Costs - Sundries'!{rcs.Cells[17, yearCount]}";
                  break;

                case 30:
                  rc.Cells[rowIndex, colIndex].Formula = $"={rc.Cells[rentedFleetStartCol, yearCount]}";
                  break;

                default:
                  rc.Cells[rowIndex, colIndex].Value = item.Years.Where(x => x.Year == year).Sum(d => d.RoadTransportValue);
                  break;
              }

              yearCount++;

              colIndex++;
            }

            rowIndex++;
            colIndex = 1;

          }

          if (!item.RoadTransportEnabled && !item.ProductionEnabled && !item.SalesEnabled && !item.AdministrationEnabled)
          {
            totalItems++;

            rc.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            rc.Cells[rowIndex, colIndex].Value = $"{item.ItemDescription}";

            colIndex++;

            var yearCount = 2;

            foreach (var year in reportYears)
            {
              rc.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              rc.Cells[rowIndex, colIndex].Style.Numberformat.Format = "R### ### ### ##0";

              switch (item.ItemId)
              {
                case 27:
                  break;

                case 30:
                  rc.Cells[rowIndex, colIndex].Formula = $"={rc.Cells[55, yearCount]}";
                  break;

                default:
                  break;
              }

              yearCount++;

              colIndex++;
            }

            rowIndex++;
            colIndex = 1;
          }
        }

        rc.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        rc.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        rc.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
        rc.Cells[rowIndex, colIndex].Value = "TOTAL";

        colIndex++;

        //rc.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        //rc.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        //rc.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
        //rc.Cells[rowIndex, colIndex].Style.Numberformat.Format = "R### ### ### ##0";
        //rc.Cells[rowIndex, colIndex].Formula =
        //    $"@SUM({rc.Cells[rowIndex, colIndex + 1]}:{rc.Cells[rowIndex, colIndex + reportYears.Count()]})/{reportYears.Count()}";
        //colIndex++;

        foreach (Int32 year in reportYears)
        {
          rc.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          rc.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          rc.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
          rc.Cells[rowIndex, colIndex].Style.Numberformat.Format = "R### ### ##0";
          var rcCount = reports.Count(x => x.Type == "RunningCosts");
          rc.Cells[rowIndex, colIndex].Formula =
              $"@SUM({rc.Cells[rowIndex - totalItems, colIndex]}:{rc.Cells[rowIndex - 1, colIndex]})";
          colIndex++;
        }

        rowIndex++;
        rowIndex++;
        colIndex = 1;

        rc.Cells[rowIndex, colIndex].Style.Font.Bold = true;
        rc.Cells[rowIndex, colIndex].Style.Font.Size = _groupHeaderFontSize;
        rc.Cells[rowIndex, colIndex].Value = "RUNNING COST - RENTED FLEET";

        rowIndex++;
        colIndex = 1;

        rc.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        rc.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        rc.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
        rc.Cells[rowIndex, colIndex].Value = "Description";

        colIndex++;

        //rc.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        //rc.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        //rc.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
        //rc.Cells[rowIndex, colIndex].Value = "Average";

        //colIndex++;

        foreach (Int32 year in reportYears)
        {
          rc.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          rc.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          rc.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
          rc.Cells[rowIndex, colIndex].Value = year;

          colIndex++;
        }

        rowIndex++;
        colIndex = 1;

        foreach (var item in reports.Where(x => x.Type == "RunningCostsFleet"))
        {
          rc.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          rc.Cells[rowIndex, colIndex].Value = item.ItemDescription;

          colIndex++;

          //rc.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          //rc.Cells[rowIndex, colIndex].Style.Numberformat.Format = "R### ### ##0";
          //rc.Cells[rowIndex, colIndex].Value = decimal.Round(item.Total, 0);

          //colIndex++;

          foreach (Int32 year in reportYears)
          {
            rc.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            rc.Cells[rowIndex, colIndex].Style.Numberformat.Format = "R### ### ##0";
            rc.Cells[rowIndex, colIndex].Value = item.Years.Where(x => x.Year == year).Sum(d => d.Value);
            colIndex++;
          }

          rowIndex++;
          colIndex = 1;

        }

        rc.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        rc.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        rc.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
        rc.Cells[rowIndex, colIndex].Value = "TOTAL";

        colIndex++;

        //rc.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        //rc.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        //rc.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
        //rc.Cells[rowIndex, colIndex].Style.Numberformat.Format = "R### ### ### ##0";
        //rc.Cells[rowIndex, colIndex].Formula =
        //    $"@SUM({rc.Cells[rowIndex, colIndex + 1]}:{rc.Cells[rowIndex, colIndex + reportYears.Count()]})/{reportYears.Count()}";
        //colIndex++;

        foreach (Int32 year in reportYears)
        {
          rc.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          rc.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          rc.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
          rc.Cells[rowIndex, colIndex].Style.Numberformat.Format = "R### ### ##0";
          var rfCount = reports.Count(x => x.Type == "RunningCostsFleet");
          rc.Cells[rowIndex, colIndex].Formula =
              $"@SUM({rc.Cells[rowIndex - rfCount, colIndex]}:{rc.Cells[rowIndex - 1, colIndex]})";
          colIndex++;
        }

        #endregion

        #region Running Cost - Sundries

        totalItems = 0;

        rcs.Column(1).Width = 45;
        rcs.Column(2).Width = 18;
        rcs.Column(3).Width = 15;
        rcs.Column(4).Width = 15;
        rcs.Column(5).Width = 15;
        rcs.Column(6).Width = 15;
        rcs.Column(7).Width = 15;
        rcs.Column(8).Width = 15;
        rcs.Column(9).Width = 15;
        rcs.Column(10).Width = 15;

        rowIndex = 1;
        colIndex = 1;
        rcs.Cells[rowIndex, colIndex].Value = GenerateMainTitle(memberType);
        rcs.Cells[rowIndex, colIndex].Style.Font.Bold = _headerBold;
        rcs.Cells[rowIndex, colIndex].Style.Font.Size = _headerFontSize;

        rowIndex++;
        rowIndex++;
        colIndex = 1;

        rcs.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        rcs.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        rcs.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
        rcs.Cells[rowIndex, colIndex].Value = "Running Costs - Sundries";

        colIndex++;

        //rcs.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        //rcs.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        //rcs.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
        //rcs.Cells[rowIndex, colIndex].Value = "Average";

        //colIndex++;

        foreach (Int32 year in reportYears)
        {
          rcs.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          rcs.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          rcs.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
          rcs.Cells[rowIndex, colIndex].Value = year;
          colIndex++;
        }

        rowIndex++;
        colIndex = 1;

        foreach (var item in reports.Where(x => x.Type == "RunningCostsSundry"))
        {

          if (item.ProductionEnabled)
          {
            totalItems++;
            rcs.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            rcs.Cells[rowIndex, colIndex].Value = $"{item.ItemDescription} (Production)";

            colIndex++;

            //rcs.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            //rcs.Cells[rowIndex, colIndex].Style.Numberformat.Format = "R### ### ### ##0";
            //rcs.Cells[rowIndex, colIndex].Formula = $"@SUM({rcs.Cells[rowIndex, colIndex + 1]}:{rcs.Cells[rowIndex, colIndex + reportYears.Count()]})/{reportYears.Count()}";

            //colIndex++;

            foreach (var year in reportYears)
            {
              rcs.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              rcs.Cells[rowIndex, colIndex].Style.Numberformat.Format = "R### ### ### ##0";
              rcs.Cells[rowIndex, colIndex].Value = item.Years.Where(x => x.Year == year).Sum(d => d.ProductionValue);
              colIndex++;
            }

            rowIndex++;
            colIndex = 1;
          }

          if (item.SalesEnabled)
          {
            totalItems++;
            rcs.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            rcs.Cells[rowIndex, colIndex].Value = $"{item.ItemDescription} (Sales/Marketing)";

            colIndex++;

            //rcs.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            //rcs.Cells[rowIndex, colIndex].Style.Numberformat.Format = "R### ### ### ##0";
            //rcs.Cells[rowIndex, colIndex].Formula = $"@SUM({rcs.Cells[rowIndex, colIndex + 1]}:{rcs.Cells[rowIndex, colIndex + reportYears.Count()]})/{reportYears.Count()}";

            //colIndex++;

            foreach (var year in reportYears)
            {
              rcs.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              rcs.Cells[rowIndex, colIndex].Style.Numberformat.Format = "R### ### ### ##0";
              rcs.Cells[rowIndex, colIndex].Value = item.Years.Where(x => x.Year == year).Sum(d => d.SalesValue);
              colIndex++;
            }

            rowIndex++;
            colIndex = 1;
          }

          if (item.AdministrationEnabled)
          {
            totalItems++;
            rcs.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            rcs.Cells[rowIndex, colIndex].Value = $"{item.ItemDescription} (Administration)";

            colIndex++;

            //rcs.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            //rcs.Cells[rowIndex, colIndex].Style.Numberformat.Format = "R### ### ### ##0";
            //rcs.Cells[rowIndex, colIndex].Formula = $"@SUM({rcs.Cells[rowIndex, colIndex + 1]}:{rcs.Cells[rowIndex, colIndex + reportYears.Count()]})/{reportYears.Count()}";

            //colIndex++;

            foreach (var year in reportYears)
            {
              rcs.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              rcs.Cells[rowIndex, colIndex].Style.Numberformat.Format = "R### ### ### ##0";
              rcs.Cells[rowIndex, colIndex].Value = item.Years.Where(x => x.Year == year).Sum(d => d.AdministrationValue);
              colIndex++;
            }

            rowIndex++;
            colIndex = 1;
          }

          if (item.RoadTransportEnabled)
          {
            totalItems++;
            rcs.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            rcs.Cells[rowIndex, colIndex].Value = $"{item.ItemDescription} (Road Transport)";

            colIndex++;

            //rcs.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            //rcs.Cells[rowIndex, colIndex].Style.Numberformat.Format = "R### ### ### ##0";
            //rcs.Cells[rowIndex, colIndex].Formula = $"@SUM({rcs.Cells[rowIndex, colIndex + 1]}:{rcs.Cells[rowIndex, colIndex + reportYears.Count()]})/{reportYears.Count()}";

            //colIndex++;

            foreach (var year in reportYears)
            {
              rcs.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              rcs.Cells[rowIndex, colIndex].Style.Numberformat.Format = "R### ### ### ##0";
              rcs.Cells[rowIndex, colIndex].Value = item.Years.Where(x => x.Year == year).Sum(d => d.RoadTransportValue);
              colIndex++;
            }

            rowIndex++;
            colIndex = 1;
          }
        }

        rcs.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        rcs.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        rcs.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
        rcs.Cells[rowIndex, colIndex].Value = "TOTAL";

        colIndex++;

        //rcs.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
        //rcs.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
        //rcs.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
        //rcs.Cells[rowIndex, colIndex].Style.Numberformat.Format = "R### ### ##0";
        //rcs.Cells[rowIndex, colIndex].Formula =
        //    $"@SUM({rcs.Cells[rowIndex, colIndex + 1]}:{rcs.Cells[rowIndex, colIndex + reportYears.Count()]})/{reportYears.Count()}";
        //colIndex++;

        foreach (var year in reportYears)
        {
          rcs.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
          rcs.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          rcs.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
          rcs.Cells[rowIndex, colIndex].Style.Numberformat.Format = "R### ### ##0";
          var rcsCount = reports.Count(x => x.Type == "RunningCostsSundry");
          rcs.Cells[rowIndex, colIndex].Formula =
              $"@SUM({rcs.Cells[rowIndex - totalItems, colIndex]}:{rcs.Cells[rowIndex - 1, colIndex]})";
          colIndex++;
        }

        #endregion

        #region Pellet Cost

        pcbP.Column(1).Width = 45;
        pcbP.Column(2).Width = 18;
        pcbP.Column(3).Width = 15;
        pcbP.Column(4).Width = 15;
        pcbP.Column(5).Width = 15;
        pcbP.Column(6).Width = 15;
        pcbP.Column(7).Width = 15;
        pcbP.Column(8).Width = 15;
        pcbP.Column(9).Width = 15;
        pcbP.Column(10).Width = 15;

        rowIndex = 1;
        colIndex = 1;
        pcbP.Cells[rowIndex, colIndex].Value = GenerateMainTitle(memberType);
        pcbP.Cells[rowIndex, colIndex].Style.Font.Bold = _headerBold;
        pcbP.Cells[rowIndex, colIndex].Style.Font.Size = _headerFontSize;

        rowIndex++;
        rowIndex++;

        foreach (var mainGroup in pcbMainGroups.Where(x => x.Id == 1))
        {
          pcbP.Cells[rowIndex, colIndex].Value = GenerateSubTitle(mainGroup.GroupName);
          pcbP.Cells[rowIndex, colIndex].Style.Font.Bold = _headerBold;
          pcbP.Cells[rowIndex, colIndex].Style.Font.Size = _subheaderFontSize;
          rowIndex++;

          foreach (var subGroup in mainGroup.SubGroups)
          {
            pcbP.Cells[rowIndex, colIndex].Value = GenerateSubTitle(subGroup.SubGroupName);
            pcbP.Cells[rowIndex, colIndex].Style.Font.Bold = _headerBold;
            pcbP.Cells[rowIndex, colIndex].Style.Font.Size = _subheaderFontSize;
            rowIndex++;

            foreach (var itemGroup in subGroup.PcbCostItemGroups.Where(x => x.Active == true))
            {
              pcbP.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              pcbP.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
              pcbP.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
              pcbP.Cells[rowIndex, colIndex].Value = itemGroup.HeaderName;

              colIndex++;

              foreach (Int32 year in reportYears)
              {
                pcbP.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                pcbP.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                pcbP.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
                pcbP.Cells[rowIndex, colIndex].Value = year;

                colIndex++;
              }

              rowIndex++;
              colIndex = 1;

              var xx = reports.Where(x => x.Type == "PCBCostPelleting");

              foreach (var item in reports.Where(x => x.Type == "PCBCostPelleting" && x.GroupId == itemGroup.Id))
              {
                pcbP.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                pcbP.Cells[rowIndex, colIndex].Value = item.ItemDescription;

                colIndex++;

                foreach (var year in reportYears)
                {
                  pcbP.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                  if (item.Readonly)
                  {
                    pcbP.Cells[rowIndex, colIndex].Value = "XXX";
                  }
                  else
                  {
                    if ((item.ItemId == 97 || item.ItemId == 99 || item.ItemId == 101))
                    {

                    }
                    else
                    {
                      pcbP.Cells[rowIndex, colIndex].Style.Numberformat.Format = "R### ### ##0";
                    }

                    pcbP.Cells[rowIndex, colIndex].Value = item.Years.Where(x => x.Year == year).Sum(d => d.Value);
                  }

                  colIndex++;
                }

                rowIndex++;
                colIndex = 1;
              }

              pcbP.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              pcbP.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
              pcbP.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
              pcbP.Cells[rowIndex, colIndex].Value = "TOTAL";

              colIndex++;

              foreach (Int32 year in reportYears)
              {
                pcbP.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                pcbP.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                pcbP.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
                pcbP.Cells[rowIndex, colIndex].Style.Numberformat.Format = "R### ### ##0";
                var pcbPCount = reports.Count(x =>
                    x.Type == "PCBCostPelleting" && x.GroupId == itemGroup.Id &&
                    x.AddToTotal);
                pcbP.Cells[rowIndex, colIndex].Formula = $"@SUM({pcbP.Cells[rowIndex - pcbPCount, colIndex]}:{pcbP.Cells[rowIndex - 1, colIndex]})";
                colIndex++;
              }

              rowIndex++;
              rowIndex++;
              colIndex = 1;
            }

            pcbP.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            pcbP.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            pcbP.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
            pcbP.Cells[rowIndex, colIndex].Value = "TONNAGE PELLETS PRODUCED";

            colIndex++;

            foreach (Int32 year in reportYears)
            {
              pcbP.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              pcbP.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
              pcbP.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
              pcbP.Cells[rowIndex, colIndex].Value = year;

              colIndex++;
            }

            rowIndex++;
            colIndex = 1;

            pcbP.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            pcbP.Cells[rowIndex, colIndex].Value = "Tonnage Pellets Produced";

            colIndex++;

            var pcbPStartCol = 2;

            foreach (var year in reportYears)
            {
              pcbP.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              pcbP.Cells[rowIndex, colIndex].Formula = $"'Industry Profile'!{ip.Cells[19, 0 + pcbPStartCol]}";
              pcbPStartCol++;
              colIndex++;
            }

            rowIndex++;
            colIndex = 1;

            pcbP.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            pcbP.Cells[rowIndex, colIndex].Value = "Cost Of Pelleting (R per ton)";

            colIndex++;

            pcbPStartCol = 2;

            foreach (var year in reportYears)
            {
              //var pTotals = new AFMA.Service.IndustryProfileService().PopulateDefaultTotalValues(AFMA.Core.IndustryProfileDisplayType.Dashboard, 0, 0, year).ToList();
              pcbP.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              pcbP.Cells[rowIndex, colIndex].Style.Numberformat.Format = "### ### ##0";
              pcbP.Cells[rowIndex, colIndex].Formula =
                  $"=({pcbP.Cells[28, 0 + pcbPStartCol]}+{pcbP.Cells[43, pcbPStartCol]})/{pcbP.Cells[rowIndex - 1, pcbPStartCol]}";
              pcbPStartCol++;
              colIndex++;
            }
          }
        }
        #endregion

        #region Crumbling Cost

        pcbC.Column(1).Width = 45;
        pcbC.Column(2).Width = 18;
        pcbC.Column(3).Width = 15;
        pcbC.Column(4).Width = 15;
        pcbC.Column(5).Width = 15;
        pcbC.Column(6).Width = 15;
        pcbC.Column(7).Width = 15;
        pcbC.Column(8).Width = 15;
        pcbC.Column(9).Width = 15;
        pcbC.Column(10).Width = 15;

        rowIndex = 1;
        colIndex = 1;
        pcbC.Cells[rowIndex, colIndex].Value = GenerateMainTitle(memberType);
        pcbC.Cells[rowIndex, colIndex].Style.Font.Bold = _headerBold;
        pcbC.Cells[rowIndex, colIndex].Style.Font.Size = _headerFontSize;

        rowIndex++;
        rowIndex++;

        foreach (var mainGroup in pcbMainGroups.Where(x => x.Id == 2))
        {
          pcbC.Cells[rowIndex, colIndex].Value = GenerateSubTitle(mainGroup.GroupName);
          pcbC.Cells[rowIndex, colIndex].Style.Font.Bold = _headerBold;
          pcbC.Cells[rowIndex, colIndex].Style.Font.Size = _subheaderFontSize;
          rowIndex++;

          foreach (var subGroup in mainGroup.SubGroups)
          {
            pcbC.Cells[rowIndex, colIndex].Value = GenerateSubTitle(subGroup.SubGroupName);
            pcbC.Cells[rowIndex, colIndex].Style.Font.Bold = _headerBold;
            pcbC.Cells[rowIndex, colIndex].Style.Font.Size = _subheaderFontSize;
            rowIndex++;

            foreach (var itemGroup in subGroup.PcbCostItemGroups.Where(x => x.Active == true))
            {
              pcbC.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              pcbC.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
              pcbC.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
              pcbC.Cells[rowIndex, colIndex].Value = itemGroup.HeaderName;

              colIndex++;

              foreach (Int32 year in reportYears)
              {
                pcbC.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                pcbC.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                pcbC.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
                pcbC.Cells[rowIndex, colIndex].Value = year;

                colIndex++;
              }

              rowIndex++;
              colIndex = 1;

              foreach (var item in reports.Where(x => x.Type == "PCBCostCrumbling" && x.GroupId == itemGroup.Id))
              {
                pcbC.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                pcbC.Cells[rowIndex, colIndex].Value = item.ItemDescription;

                colIndex++;

                foreach (Int32 year in reportYears)
                {
                  pcbC.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                  if (item.Readonly)
                  {
                    pcbC.Cells[rowIndex, colIndex].Value = "XXX";
                  }
                  else
                  {
                    if ((item.ItemId == 97 || item.ItemId == 99 || item.ItemId == 101))
                    {

                    }
                    else
                    {
                      pcbC.Cells[rowIndex, colIndex].Style.Numberformat.Format = "R### ### ##0";
                    }

                    pcbC.Cells[rowIndex, colIndex].Value = item.Years.Where(x => x.Year == year).Sum(d => d.Value);
                  }

                  colIndex++;
                }

                rowIndex++;
                colIndex = 1;
              }

              pcbC.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              pcbC.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
              pcbC.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
              pcbC.Cells[rowIndex, colIndex].Value = "TOTAL";

              colIndex++;

              foreach (Int32 year in reportYears)
              {
                pcbC.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                pcbC.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                pcbC.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
                pcbC.Cells[rowIndex, colIndex].Style.Numberformat.Format = "R### ### ##0";
                var pcbCCount = reports.Count(x =>
                    x.Type == "PCBCostCrumbling" && x.GroupId == itemGroup.Id &&
                    x.AddToTotal);
                pcbC.Cells[rowIndex, colIndex].Formula = $"@SUM({pcbC.Cells[rowIndex - pcbCCount, colIndex]}:{pcbC.Cells[rowIndex - 1, colIndex]})";
                colIndex++;
              }

              rowIndex++;
              rowIndex++;
              colIndex = 1;
            }

            pcbC.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            pcbC.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            pcbC.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
            pcbC.Cells[rowIndex, colIndex].Value = "TONNAGE CRUMBLERS PRODUCED";

            colIndex++;

            foreach (Int32 year in reportYears)
            {
              pcbC.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              pcbC.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
              pcbC.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
              pcbC.Cells[rowIndex, colIndex].Value = year;

              colIndex++;
            }

            rowIndex++;
            colIndex = 1;

            pcbC.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            pcbC.Cells[rowIndex, colIndex].Value = "Tonnage Crumbles Produced";

            colIndex++;

            var pcbCStartCol = 2;

            foreach (Int32 year in reportYears)
            {
              pcbC.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              pcbC.Cells[rowIndex, colIndex].Formula = $"'Industry Profile'!{ip.Cells[20, 0 + pcbCStartCol]}";
              pcbCStartCol++;
              colIndex++;
            }

            rowIndex++;
            colIndex = 1;

            pcbC.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            pcbC.Cells[rowIndex, colIndex].Value = "Cost Of Crumbling (R per ton)";

            colIndex++;

            pcbCStartCol = 2;

            foreach (Int32 year in reportYears)
            {
              //var cTotals = new AFMA.Service.IndustryProfileService().PopulateDefaultTotalValues(AFMA.Core.IndustryProfileDisplayType.Dashboard, 0, 0, year).ToList();
              pcbC.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              pcbC.Cells[rowIndex, colIndex].Style.Numberformat.Format = "### ### ##0";
              pcbC.Cells[rowIndex, colIndex].Formula =
                  $"=({pcbC.Cells[19, 0 + pcbCStartCol]}+{pcbC.Cells[34, pcbCStartCol]})/{pcbC.Cells[rowIndex - 1, pcbCStartCol]}";
              pcbCStartCol++;
              colIndex++;
            }
          }
        }
        #endregion

        #region Bagging Cost

        pcbB.Column(1).Width = 45;
        pcbB.Column(2).Width = 18;
        pcbB.Column(3).Width = 15;
        pcbB.Column(4).Width = 15;
        pcbB.Column(5).Width = 15;
        pcbB.Column(6).Width = 15;
        pcbB.Column(7).Width = 15;
        pcbB.Column(8).Width = 15;
        pcbB.Column(9).Width = 15;
        pcbB.Column(10).Width = 15;

        rowIndex = 1;
        colIndex = 1;
        pcbB.Cells[rowIndex, colIndex].Value = GenerateMainTitle(memberType);
        pcbB.Cells[rowIndex, colIndex].Style.Font.Bold = _headerBold;
        pcbB.Cells[rowIndex, colIndex].Style.Font.Size = _headerFontSize;

        rowIndex++;
        rowIndex++;

        foreach (var mainGroup in pcbMainGroups.Where(x => x.Id == 3))
        {
          pcbB.Cells[rowIndex, colIndex].Value = GenerateSubTitle(mainGroup.GroupName);
          pcbB.Cells[rowIndex, colIndex].Style.Font.Bold = _headerBold;
          pcbB.Cells[rowIndex, colIndex].Style.Font.Size = _subheaderFontSize;
          rowIndex++;

          foreach (var subGroup in mainGroup.SubGroups)
          {
            pcbB.Cells[rowIndex, colIndex].Value = GenerateSubTitle(subGroup.SubGroupName);
            pcbB.Cells[rowIndex, colIndex].Style.Font.Bold = _headerBold;
            pcbB.Cells[rowIndex, colIndex].Style.Font.Size = _subheaderFontSize;
            rowIndex++;

            foreach (var itemGroup in subGroup.PcbCostItemGroups.Where(x => x.Active == true))
            {
              pcbB.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              pcbB.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
              pcbB.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
              pcbB.Cells[rowIndex, colIndex].Value = itemGroup.HeaderName;

              colIndex++;

              foreach (Int32 year in reportYears)
              {
                pcbB.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                pcbB.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                pcbB.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
                pcbB.Cells[rowIndex, colIndex].Value = year;

                colIndex++;
              }

              rowIndex++;
              colIndex = 1;

              foreach (var item in reports.Where(x => x.Type == "PCBCostBagging" && x.GroupId == itemGroup.Id))
              {
                pcbB.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                pcbB.Cells[rowIndex, colIndex].Value = item.ItemDescription;

                colIndex++;

                foreach (Int32 year in reportYears)
                {
                  pcbB.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                  if (item.Readonly)
                  {
                    pcbB.Cells[rowIndex, colIndex].Value = "XXX";
                  }
                  else
                  {
                    if ((item.ItemId == 97 || item.ItemId == 99 || item.ItemId == 101))
                    {

                    }
                    else
                    {
                      pcbB.Cells[rowIndex, colIndex].Style.Numberformat.Format = "R### ### ##0";
                    }

                    pcbB.Cells[rowIndex, colIndex].Value = item.Years.Where(x => x.Year == year).Sum(d => d.Value);
                  }

                  colIndex++;
                }

                rowIndex++;
                colIndex = 1;
              }

              pcbB.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              pcbB.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
              pcbB.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
              pcbB.Cells[rowIndex, colIndex].Value = "TOTAL";

              colIndex++;

              foreach (Int32 year in reportYears)
              {
                pcbB.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                pcbB.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                pcbB.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
                pcbB.Cells[rowIndex, colIndex].Style.Numberformat.Format = "R### ### ##0";
                var pcbBCount = reports.Count(x =>
                    x.Type == "PCBCostBagging" && x.GroupId == itemGroup.Id &&
                    x.AddToTotal);
                pcbB.Cells[rowIndex, colIndex].Formula = $"@SUM({pcbB.Cells[rowIndex - pcbBCount, colIndex]}:{pcbB.Cells[rowIndex - 1, colIndex]})";
                colIndex++;
              }

              rowIndex++;
              rowIndex++;
              colIndex = 1;
            }

            pcbB.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            pcbB.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            pcbB.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
            pcbB.Cells[rowIndex, colIndex].Value = "TONNAGE MEAL BAGGED";

            colIndex++;

            foreach (Int32 year in reportYears)
            {
              pcbB.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              pcbB.Cells[rowIndex, colIndex].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
              pcbB.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.SetColor(_headerColour);
              pcbB.Cells[rowIndex, colIndex].Value = year;

              colIndex++;
            }

            rowIndex++;
            colIndex = 1;

            pcbB.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            pcbB.Cells[rowIndex, colIndex].Value = "Tonnage Meal Bagged";

            colIndex++;

            var pcbBStartCol = 2;

            foreach (Int32 year in reportYears)
            {
              pcbB.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              pcbB.Cells[rowIndex, colIndex].Formula = $"'Industry Profile'!{ip.Cells[13, 0 + pcbBStartCol]}";
              pcbBStartCol++;
              colIndex++;
            }

            rowIndex++;
            colIndex = 1;

            pcbB.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
            pcbB.Cells[rowIndex, colIndex].Value = "Cost Of Bagging (R per ton)";

            colIndex++;

            pcbBStartCol = 2;

            foreach (Int32 year in reportYears)
            {
              //var bTotals = new AFMA.Service.IndustryProfileService().PopulateDefaultTotalValues(AFMA.Core.IndustryProfileDisplayType.Dashboard, 0, 0, year).ToList();
              pcbB.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
              pcbB.Cells[rowIndex, colIndex].Style.Numberformat.Format = "### ### ##0";
              pcbB.Cells[rowIndex, colIndex].Formula =
                  $"=({pcbB.Cells[30, 0 + pcbBStartCol]}+{pcbC.Cells[45, pcbBStartCol]})/{pcbC.Cells[rowIndex - 1, pcbBStartCol]}";
              pcbBStartCol++;
              colIndex++;
            }
          }
        }
        #endregion

        return p.GetAsByteArray();
      }


    }

    #endregion

    public async Task<decimal> GetValueFromTable<TEntity>(int reportId, int groupId, int itemId, CostingGroup costingGroup, int memberId, int memberBranchId, int year = 0) where TEntity : class, IIndustryCostMultiple
    {
      var result = await _applicationRepository.QueryAsync<TEntity>().ConfigureAwait(false);
      var entity = result.Where(x => x.IndustryProfileReport.Submitted);

      switch (costingGroup)
      {
        case CostingGroup.AdministrationValue:
          if (year == 0)
          {
            return entity.Where(x => x.IndustryProfileReport.Id == reportId).Sum(sum => (decimal?)sum.AdministrationValue) ?? 0;
          }
          else
          {
            var t = entity.Where(x => x.IndustryProfileReport.ReportYear == year);

            if (memberId > 0 && memberBranchId > 0)
            {
              return t.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?)sum.AdministrationValue) ?? 0;
            }

            if (memberId > 0 && memberBranchId == 0)
            {
              return t.Where(x => x.Member.Id == memberId).Sum(sum => (decimal?)sum.AdministrationValue) ?? 0;
            }

            if (memberId == 0 && memberBranchId == 0)
            {
              return t.Sum(sum => (decimal?)sum.AdministrationValue) ?? 0;
            }

            return 0;
          }

        case CostingGroup.ProductionValue:
          if (year == 0)
          {
            return entity.Where(x => x.IndustryProfileReport.Id == reportId).Select(a => (decimal?)a.ProductionValue).Sum() ?? 0;
          }
          else
          {
            var t = entity.Where(x => x.IndustryProfileReport.ReportYear == year);

            if (memberId > 0 && memberBranchId > 0)
            {
              return t.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?)sum.ProductionValue) ?? 0;
            }

            if (memberId > 0 && memberBranchId == 0)
            {
              return t.Where(x => x.Member.Id == memberId).Sum(sum => (decimal?)sum.ProductionValue) ?? 0;
            }

            if (memberId == 0 && memberBranchId == 0)
            {
              return t.Sum(sum => (decimal?)sum.ProductionValue) ?? 0;
            }

            return 0;

            //return entity.Where(x => x.IPCBReport.ReportYear == year).Select(a => (decimal?)a.ProductionValue).Sum() ?? 0;
          }

        case CostingGroup.RoadTransportValue:
          if (year == 0)
          {
            return entity.Where(x => x.IndustryProfileReport.Id == reportId).Select(a => (decimal?)a.RoadTransportValue).Sum() ?? 0;
          }
          else
          {
            var t = entity.Where(x => x.IndustryProfileReport.ReportYear == year);

            if (memberId > 0 && memberBranchId > 0)
            {
              return t.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?)sum.RoadTransportValue) ?? 0;
            }

            if (memberId > 0 && memberBranchId == 0)
            {
              return t.Where(x => x.Member.Id == memberId).Sum(sum => (decimal?)sum.RoadTransportValue) ?? 0;
            }

            if (memberId == 0 && memberBranchId == 0)
            {
              return t.Sum(sum => (decimal?)sum.RoadTransportValue) ?? 0;
            }

            return 0;
            //return entity.Where(x => x.IPCBReport.ReportYear == year).Select(a => (decimal?)a.RoadTransportValue).Sum() ?? 0;
          }


        case CostingGroup.SalesValue:
          if (year == 0)
          {
            return entity.Where(x => x.IndustryProfileReport.Id == reportId).Select(a => (decimal?)a.SalesValue).Sum() ?? 0;
          }
          else
          {
            var t = entity.Where(x => x.IndustryProfileReport.ReportYear == year);

            if (memberId > 0 && memberBranchId > 0)
            {
              return t.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId)
                         .Sum(sum => (decimal?)sum.SalesValue) ?? 0;
            }

            if (memberId > 0 && memberBranchId == 0)
            {
              return t.Where(x => x.Member.Id == memberId).Sum(sum => (decimal?)sum.SalesValue) ?? 0;
            }

            if (memberId == 0 && memberBranchId == 0)
            {
              return t.Sum(sum => (decimal?)sum.SalesValue) ?? 0;
            }

            return 0;
          }

        default:
          return 0;
      }
    }

    public async Task<decimal[]> GetCostFromTableArray(int linkedId1, int? linkedId2, int reportId, string tableName, int industryCostId, RunningCostType runningCostType, bool sundryItem, int? reportYear, int? industryCostGroupId, int memberId = 0, int memberBranchId = 0, int memberTypeId = 0)
    {
      var memberRunningCosts = await _applicationRepository.QueryAsync<MemberRunningCost>().ConfigureAwait(false);
      var memberRunningCostsFleet = await _applicationRepository.QueryAsync<MemberRunningCostFleet>().ConfigureAwait(false);
      var memberRunningCostsSundry = await _applicationRepository.QueryAsync<MemberRunningCostSundry>().ConfigureAwait(false);

      decimal[] _total = new decimal[4];

      switch (tableName.ToUpper())
      {
        case "RUNNINGCOST":

          if (reportYear == null)
          {
            _total = new decimal[] { 0, 0, 0, 0 };
            try
            {
              var _rcsReports = memberRunningCostsSundry.Where(x => x.IndustryProfileReport.Id == reportId && x.IndustryProfileReport.Submitted && x.IndustryProfileReport.Member.MemberType.Id == memberTypeId);
              var _rcReports = memberRunningCosts.Where(x => x.IndustryProfileReport.Id == reportId && x.IndustryProfileReport.Submitted && x.IndustryProfileReport.Member.MemberType.Id == memberTypeId);

              if (memberId > 0 && memberBranchId == 0)
              {
                _total[0] = _rcReports.Where(x => x.Member.Id == memberId).Sum(sum => (decimal?)sum.ProductionValue ?? 0M);
                _total[1] = _rcReports.Where(x => x.Member.Id == memberId).Sum(sum => (decimal?)sum.SalesValue ?? 0M);
                _total[2] = _rcReports.Where(x => x.Member.Id == memberId).Sum(sum => (decimal?)sum.AdministrationValue ?? 0M);
                _total[3] = _rcReports.Where(x => x.Member.Id == memberId).Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M);

                if (sundryItem)
                {
                  _total[0] = _total[0] + _rcsReports.Where(x => x.Member.Id == memberId).Sum(sum => (decimal?)sum.ProductionValue ?? 0M);
                  _total[1] = _total[1] + _rcsReports.Where(x => x.Member.Id == memberId).Sum(sum => (decimal?)sum.SalesValue ?? 0M);
                  _total[2] = _total[2] + _rcsReports.Where(x => x.Member.Id == memberId).Sum(sum => (decimal?)sum.AdministrationValue ?? 0M);
                  _total[3] = _total[3] + _rcsReports.Where(x => x.Member.Id == memberId).Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M);
                }
              }

              if (memberId > 0 && memberBranchId > 0)
              {
                _total[0] = _rcReports.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?)sum.ProductionValue ?? 0M);
                _total[1] = _rcReports.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?)sum.SalesValue ?? 0M);
                _total[2] = _rcReports.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?)sum.AdministrationValue ?? 0M);
                _total[3] = _rcReports.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M);

                if (sundryItem)
                {
                  _total[0] = _total[0] + _rcsReports.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?)sum.ProductionValue ?? 0M);
                  _total[1] = _total[1] + _rcsReports.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?)sum.SalesValue ?? 0M);
                  _total[2] = _total[2] + _rcsReports.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?)sum.AdministrationValue ?? 0M);
                  _total[3] = _total[3] + _rcsReports.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M);
                }
              }

              if (memberId == 0 && memberBranchId == 0)
              {
                _total[0] = _rcReports.Sum(sum => (decimal?)sum.ProductionValue ?? 0M);
                _total[1] = _rcReports.Sum(sum => (decimal?)sum.SalesValue ?? 0M);
                _total[2] = _rcReports.Sum(sum => (decimal?)sum.AdministrationValue ?? 0M);
                _total[3] = _rcReports.Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M);

                if (sundryItem)
                {
                  _total[0] = _total[0] + _rcsReports.Sum(sum => (decimal?)sum.ProductionValue ?? 0M);
                  _total[1] = _total[1] + _rcsReports.Sum(sum => (decimal?)sum.SalesValue ?? 0M); _total[2] = _total[2] + _rcsReports.Sum(sum => (decimal?)sum.AdministrationValue ?? 0M); _total[3] = _total[3] + _rcsReports.Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M);
                }
              }

              return _total;
            }
            catch
            {
              return new decimal[] { 0, 0, 0, 0 };
            }
          }
          else
          {
            _total = new decimal[] { 0, 0, 0, 0 };

            try
            {
              var _rcsReports = memberRunningCostsSundry.Where(x => x.IndustryProfileReport.ReportYear == reportYear && x.IndustryProfileReport.Submitted && x.IndustryProfileReport.MemberType.Id == memberTypeId);
              var _rcReports = memberRunningCosts.Where(x => x.IndustryProfileReport.ReportYear == reportYear && x.IndustryProfileReport.Submitted && x.RunningCostGroup.Id == linkedId1 && x.IndustryProfileReport.MemberType.Id == memberTypeId);
              var _rc2Reports = new List<MemberRunningCost>();

              if (linkedId2 > 0)
              {
                _rc2Reports = memberRunningCosts.Where(x => x.IndustryProfileReport.ReportYear == reportYear && x.IndustryProfileReport.Submitted && x.IndustryProfileReport.MemberType.Id == memberTypeId && x.RunningCostGroup.Id == linkedId2).ToList();
              }

              if (memberId > 0 && memberBranchId == 0)
              {
                _total[0] = _rcReports.Where(x => x.Member.Id == memberId)
                    .Sum(sum => (decimal?)sum.ProductionValue ?? 0M);
                _total[1] = _rcReports.Where(x => x.Member.Id == memberId)
                    .Sum(sum => (decimal?)sum.SalesValue ?? 0M);
                _total[2] = _rcReports.Where(x => x.Member.Id == memberId)
                    .Sum(sum => (decimal?)sum.AdministrationValue ?? 0M);
                _total[3] = _rcReports.Where(x => x.Member.Id == memberId)
                    .Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M);

                if (linkedId2 > 0)
                {
                  _total[0] = _total[0] + _rc2Reports.Where(x => x.Member.Id == memberId)
                                  .Sum(sum => (decimal?)sum.ProductionValue) ?? 0M;
                  _total[1] = _total[1] + _rc2Reports.Where(x => x.Member.Id == memberId)
                                  .Sum(sum => (decimal?)sum.SalesValue) ?? 0M;
                  _total[2] = _total[2] + _rc2Reports.Where(x => x.Member.Id == memberId)
                                  .Sum(sum => (decimal?)sum.AdministrationValue) ?? 0M;
                  _total[3] = _total[3] + _rc2Reports.Where(x => x.Member.Id == memberId)
                                  .Sum(sum => (decimal?)sum.RoadTransportValue) ?? 0M;
                }

                if (sundryItem)
                {
                  _total[0] = _total[0] + _rcsReports.Where(x => x.Member.Id == memberId)
                                  .Sum(sum => (decimal?)sum.ProductionValue ?? 0M);
                  _total[1] = _total[1] + _rcsReports.Where(x => x.Member.Id == memberId)
                                  .Sum(sum => (decimal?)sum.SalesValue ?? 0M);
                  _total[2] = _total[2] + _rcsReports.Where(x => x.Member.Id == memberId)
                                  .Sum(sum => (decimal?)sum.AdministrationValue ?? 0M);
                  _total[3] = _total[3] + _rcsReports.Where(x => x.Member.Id == memberId)
                                  .Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M);
                }
              }

              if (memberId > 0 && memberBranchId > 0)
              {
                _total[0] = _rcReports.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId)
                    .Sum(sum => (decimal?)sum.ProductionValue ?? 0M);
                _total[1] = _rcReports.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId)
                    .Sum(sum => (decimal?)sum.SalesValue ?? 0M);
                _total[2] = _rcReports.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId)
                    .Sum(sum => (decimal?)sum.AdministrationValue ?? 0M);
                _total[3] = _rcReports.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId)
                    .Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M);

                if (linkedId2 > 0)
                {
                  _total[0] = _total[0] + _rc2Reports
                                  .Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId)
                                  .Sum(sum => (decimal?)sum.ProductionValue) ?? 0M;
                  _total[1] = _total[1] + _rc2Reports
                                  .Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId)
                                  .Sum(sum => (decimal?)sum.SalesValue) ?? 0M;
                  _total[2] = _total[2] + _rc2Reports
                                  .Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId)
                                  .Sum(sum => (decimal?)sum.AdministrationValue) ?? 0M;
                  _total[3] = _total[3] + _rc2Reports
                                  .Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId)
                                  .Sum(sum => (decimal?)sum.RoadTransportValue) ?? 0M;
                }

                if (sundryItem)
                {
                  _total[0] = _total[0] + _rcsReports
                                  .Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId)
                                  .Sum(sum => (decimal?)sum.ProductionValue ?? 0M);
                  _total[1] = _total[1] + _rcsReports
                                  .Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId)
                                  .Sum(sum => (decimal?)sum.SalesValue ?? 0M);
                  _total[2] = _total[2] + _rcsReports
                                  .Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId)
                                  .Sum(sum => (decimal?)sum.AdministrationValue ?? 0M);
                  _total[3] = _total[3] + _rcsReports
                                  .Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId)
                                  .Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M);
                }
              }

              if (memberId == 0 && memberBranchId == 0)
              {
                //_total[0] = _rcsReports.Sum(sum => (decimal?)sum.ProductionValue ?? 0M);
                //_total[0] = _total[0] + _rcReports.Sum(sum => (decimal?)sum.ProductionValue ?? 0M);

                _total[0] = _rcReports.Sum(sum => (decimal?)sum.ProductionValue ?? 0M);
                _total[1] = _rcReports.Sum(sum => (decimal?)sum.SalesValue ?? 0M);
                _total[2] = _rcReports.Sum(sum => (decimal?)sum.AdministrationValue ?? 0M);
                _total[3] = _rcReports.Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M);

                if (linkedId2 > 0)
                {
                  _total[0] = _total[0] + _rc2Reports.Sum(sum =>
                                  (decimal?)sum.ProductionValue ?? 0M);
                  _total[1] = _total[1] + _rc2Reports.Sum(sum => (decimal?)sum.SalesValue ?? 0M);
                  _total[2] = _total[2] + _rc2Reports.Sum(sum =>
                                  (decimal?)sum.AdministrationValue ?? 0M);
                  _total[3] = _total[3] + _rc2Reports.Sum(sum =>
                                  (decimal?)sum.RoadTransportValue ?? 0M);
                }

                //_total[1] = _rcsReports.Sum(sum => (decimal?)sum.SalesValue ?? 0M);
                //_total[1] = _total[1] + _rcReports.Sum(sum => (decimal?)sum.SalesValue ?? 0M);

                //_total[2] = _rcsReports.Sum(sum => (decimal?)sum.AdministrationValue ?? 0M);
                //_total[2] = _total[2] + _rcReports.Sum(sum => (decimal?)sum.AdministrationValue ?? 0M);

                //_total[3] = _rcsReports.Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M);
                //_total[3] = _total[3] + _rcReports.Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M);
              }

              return _total;
            }
            catch
            {
              return new decimal[] { 0, 0, 0, 0 };
            }
          }


        case "RUNNINGCOSTSUNDRY":
          {
            if (linkedId1 == 27 && industryCostGroupId == 2)
            {
              if (reportYear == null)
              {
                try
                {
                  var t = memberRunningCostsSundry.Where(x => x.IndustryProfileReport.Id == reportId && x.IndustryProfileReport.Submitted && x.IndustryProfileReport.MemberType.Id == memberTypeId);

                  if (memberId > 0 && memberBranchId > 0)
                  {
                    return new[]
                    {
                                        t.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?) sum.ProductionValue ?? 0M),
                                        0, 0, 0
                                    };
                  }

                  if (memberId > 0 && memberBranchId == 0)
                  {
                    return new[]
                    {
                                        t.Where(x => x.Member.Id == memberId).Sum(sum => (decimal?) sum.ProductionValue ?? 0M),
                                        0, 0, 0
                                    };
                  }

                  if (memberId == 0 && memberBranchId == 0)
                  {
                    return new[] { t.Sum(sum => (decimal?)sum.ProductionValue ?? 0M), 0, 0, 0 };
                  }
                }
                catch
                {
                  return new decimal[] { 0, 0, 0, 0 };
                }
              }
              else
              {
                try
                {
                  var t = memberRunningCostsSundry.Where(x => x.IndustryProfileReport.ReportYear == reportYear && x.IndustryProfileReport.Submitted && x.IndustryProfileReport.MemberType.Id == memberTypeId);

                  if (memberId > 0 && memberBranchId > 0)
                  {
                    return new[]
                    {
                        t.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?) sum.ProductionValue ?? 0M), 0, 0, 0 };
                  }

                  if (memberId > 0 && memberBranchId == 0)
                  {
                    return new[]
                    {
                                        t.Where(x => x.Member.Id == memberId).Sum(sum => (decimal?) sum.ProductionValue ?? 0M), 0, 0, 0 };
                  }

                  if (memberId == 0 && memberBranchId == 0)
                  {
                    return new[] { t.Sum(sum => (decimal?)sum.ProductionValue ?? 0M), 0, 0, 0 };
                  }
                }
                catch
                {
                  return new decimal[] { 0, 0, 0, 0 };
                }
              }
            }

            if (linkedId1 == 27 && industryCostGroupId == 3)
            {
              if (reportYear == null)
              {
                try
                {
                  var t = memberRunningCostsSundry.Where(x => x.IndustryProfileReport.Id == reportId && x.IndustryProfileReport.Submitted && x.IndustryProfileReport.MemberType.Id == memberTypeId);

                  if (memberId > 0 && memberBranchId > 0)
                  {
                    return new[]
                    {
                                        0,
                                        t.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?) sum.SalesValue ?? 0M), 0, 0 };
                  }

                  if (memberId > 0 && memberBranchId == 0)
                  {
                    return new[]
                    {
                                        0,
                                        t.Where(x => x.Member.Id == memberId).Sum(sum => (decimal?) sum.SalesValue ?? 0M), 0, 0 };
                  }

                  if (memberId == 0 && memberBranchId == 0)
                  {
                    return new[] { 0, t.Sum(sum => (decimal?)sum.SalesValue ?? 0M), 0, 0 };
                  }
                }
                catch
                {
                  return new decimal[] { 0, 0, 0, 0 };
                }
              }
              else
              {
                try
                {
                  var t = memberRunningCostsSundry.Where(x => x.IndustryProfileReport.ReportYear == reportYear && x.IndustryProfileReport.Submitted && x.IndustryProfileReport.MemberType.Id == memberTypeId);

                  if (memberId > 0 && memberBranchId > 0)
                  {
                    return new[]
                    {
                                        0,
                                        t.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?) sum.SalesValue ?? 0M), 0, 0 };
                  }

                  if (memberId > 0 && memberBranchId == 0)
                  {
                    return new[]
                    {
                                        0,
                                        t.Where(x => x.Member.Id == memberId).Sum(sum => (decimal?) sum.SalesValue ?? 0M), 0, 0 };
                  }

                  if (memberId == 0 && memberBranchId == 0)
                  {
                    return new[] { 0, t.Sum(sum => (decimal?)sum.SalesValue ?? 0M), 0, 0 };
                  }
                }
                catch
                {
                  return new decimal[] { 0, 0, 0, 0 };
                }
              }
            }

            if (linkedId1 == 27 && industryCostGroupId == 4)
            {
              if (reportYear == null)
              {
                try
                {
                  var t = memberRunningCostsSundry.Where(x => x.IndustryProfileReport.Id == reportId && x.IndustryProfileReport.Submitted && x.IndustryProfileReport.MemberType.Id == memberTypeId);

                  if (memberId > 0 && memberBranchId > 0)
                  {
                    return new[]
                    {
                                        0, 0,
                                        t.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?) sum.AdministrationValue ?? 0M), 0 };
                  }

                  if (memberId > 0 && memberBranchId == 0)
                  {
                    return new[]
                    {
                                        0, 0,
                                        t.Where(x => x.Member.Id == memberId).Sum(sum => (decimal?) sum.AdministrationValue ?? 0M), 0 };
                  }

                  if (memberId == 0 && memberBranchId == 0)
                  {
                    return new[] { 0, 0, t.Sum(sum => (decimal?)sum.AdministrationValue ?? 0M), 0 };
                  }
                }
                catch
                {
                  return new decimal[] { 0, 0, 0, 0 };
                }
              }
              else
              {
                try
                {
                  var t = memberRunningCostsSundry.Where(x => x.IndustryProfileReport.ReportYear == reportYear && x.IndustryProfileReport.Submitted && x.IndustryProfileReport.MemberType.Id == memberTypeId);
                  if (memberId > 0 && memberBranchId > 0)
                  {
                    return new[]
                    {
                                        0, 0,
                                        t.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?) sum.AdministrationValue ?? 0M), 0 };
                  }

                  if (memberId > 0 && memberBranchId == 0)
                  {
                    return new[]
                    {
                                        0, 0,
                                        t.Where(x => x.Member.Id == memberId).Sum(sum => (decimal?) sum.AdministrationValue ?? 0M), 0 };
                  }

                  if (memberId == 0 && memberBranchId == 0)
                  {
                    return new[] { 0, 0, t.Sum(sum => (decimal?)sum.AdministrationValue ?? 0M), 0 };
                  }
                }
                catch
                {
                  return new decimal[] { 0, 0, 0, 0 };
                }
              }
            }

            if (linkedId1 == 27 && industryCostGroupId == 5)
            {
              if (reportYear == null)
              {
                try
                {
                  var t = memberRunningCostsSundry.Where(x => x.IndustryProfileReport.Id == reportId && x.IndustryProfileReport.Submitted && x.IndustryProfileReport.MemberType.Id == memberTypeId);

                  if (memberId > 0 && memberBranchId > 0)
                  {
                    return new[]
                    {
                                        0, 0, 0,
                                        t.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?) sum.RoadTransportValue ?? 0M) };
                  }

                  if (memberId > 0 && memberBranchId == 0)
                  {
                    return new[]
                    {
                                        0, 0, 0,
                                        t.Where(x => x.Member.Id == memberId).Sum(sum => (decimal?) sum.RoadTransportValue ?? 0M) };
                  }

                  if (memberId == 0 && memberBranchId == 0)
                  {
                    return new[] { 0, 0, 0, t.Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M) };
                  }
                }
                catch
                {
                  return new decimal[] { 0, 0, 0, 0 };
                }
              }
              else
              {
                try
                {
                  var t = memberRunningCostsSundry.Where(x => x.IndustryProfileReport.ReportYear == reportYear && x.IndustryProfileReport.Submitted && x.IndustryProfileReport.MemberType.Id == memberTypeId);

                  if (memberId > 0 && memberBranchId > 0)
                  {
                    return new[]
                    {
                                        0, 0, 0,
                                        t.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId)
                                            .Sum(sum => (decimal?) sum.RoadTransportValue ?? 0M)
                                    };
                  }

                  if (memberId > 0 && memberBranchId == 0)
                  {
                    return new[]
                    {
                                        0, 0, 0,
                                        t.Where(x => x.Member.Id == memberId).Sum(sum =>
                                            (decimal?) sum.RoadTransportValue ?? 0M)
                                    };
                  }

                  if (memberId == 0 && memberBranchId == 0)
                  {
                    return new[] { 0, 0, 0, t.Sum(sum => (decimal?)sum.RoadTransportValue ?? 0M) };
                  }
                }
                catch
                {
                  return new decimal[] { 0, 0, 0, 0 };
                }
              }
            }

            return new decimal[] { 0, 0, 0, 0 };
          }

        case "RUNNINGCOSTFLEET":
          {
            if (linkedId1 == 0 && industryCostGroupId == 7)
            {
              if (reportYear == null)
              {
                try
                {
                  var t = memberRunningCostsSundry.Where(x => x.IndustryProfileReport.Id == reportId && x.IndustryProfileReport.Submitted && x.IndustryProfileReport.MemberType.Id == memberTypeId);
                  var r = memberRunningCosts.Where(x => x.IndustryProfileReport.Id == reportId && x.IndustryProfileReport.Submitted && x.IndustryProfileReport.MemberType.Id == memberTypeId);
                  var u = memberRunningCostsFleet.Where(x => x.IndustryProfileReport.Id == reportId && x.IndustryProfileReport.Submitted && x.IndustryProfileReport.MemberType.Id == memberTypeId);

                  if (memberId > 0 && memberBranchId > 0)
                  {
                    return new[]
                    {
                                        0, 0, 0,
                                        t.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId).Sum(sum => (decimal?) sum.RoadTransportValue) ?? 0M +
                                        u.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId)
                                            .Sum(sum => (decimal?) sum.Value) ?? 0M +
                                        r.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId)
                                            .Sum(sum => (decimal?) sum.RoadTransportValue) ?? 0M
                                    };
                  }

                  if (memberId > 0 && memberBranchId == 0)
                  {
                    return new[]
                    {
                                        0, 0, 0,
                                        t.Where(x => x.Member.Id == memberId).Sum(sum =>
                                            (decimal?) sum.RoadTransportValue) ?? 0M +
                                        u.Where(x => x.Member.Id == memberId)
                                            .Sum(sum => (decimal?) sum.Value) ?? 0M +
                                        r.Where(x => x.Member.Id == memberId)
                                            .Sum(sum => (decimal?) sum.RoadTransportValue) ?? 0M
                                    };
                  }

                  if (memberId == 0 && memberBranchId == 0)
                  {
                    return new[]
                    {
                                        0, 0, 0,
                                        t.Sum(sum => (decimal?) sum.RoadTransportValue) ?? 0M +
                                        u.Sum(sum => (decimal?) sum.Value) ?? 0M +
                                        r.Sum(sum => (decimal?) sum.RoadTransportValue) ?? 0M
                                    };
                  }
                }
                catch
                {
                  return new decimal[] { 0, 0, 0, 0 };
                }
              }
              else
              {
                try
                {
                  var t = memberRunningCostsSundry.Where(x => x.IndustryProfileReport.ReportYear == reportYear && x.IndustryProfileReport.Submitted && x.IndustryProfileReport.MemberType.Id == memberTypeId);
                  var u = memberRunningCostsFleet.Where(x => x.IndustryProfileReport.ReportYear == reportYear && x.IndustryProfileReport.Submitted && x.IndustryProfileReport.MemberType.Id == memberTypeId);
                  var r = memberRunningCosts.Where(x => x.IndustryProfileReport.ReportYear == reportYear && x.IndustryProfileReport.Submitted && x.IndustryProfileReport.MemberType.Id == memberTypeId);

                  if (memberId > 0 && memberBranchId > 0)
                  {
                    var x1 = t.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId)
                                 .Sum(sum => (decimal?)sum.RoadTransportValue) ?? 0M;

                    var x2 = u.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId)
                                 .Sum(sum => (decimal?)sum.Value) ?? 0M;

                    var x3 = r.Where(x => x.Member.Id == memberId && x.MemberBranch.Id == memberBranchId)
                                 .Sum(sum => (decimal?)sum.RoadTransportValue) ?? 0M;

                    return new[]
                    {
                                        0, 0, 0, x1 + x2 + x3
                                    };
                  }

                  if (memberId > 0 && memberBranchId == 0)
                  {
                    var x1 = t.Where(x => x.Member.Id == memberId)
                                 .Sum(sum => (decimal?)sum.RoadTransportValue) ?? 0M;

                    var x2 = u.Where(x => x.Member.Id == memberId)
                                 .Sum(sum => (decimal?)sum.Value) ?? 0M;

                    var x3 = r.Where(x => x.Member.Id == memberId)
                                 .Sum(sum => (decimal?)sum.RoadTransportValue) ?? 0M;

                    return new[]
                    {
                                        0, 0, 0, x1 + x2 + x3
                                    };
                  }

                  if (memberId == 0 && memberBranchId == 0)
                  {
                    var x1 = t.Sum(sum => (decimal?)sum.RoadTransportValue) ?? 0M;
                    var x2 = u.Sum(sum => (decimal?)sum.Value) ?? 0M;
                    var x3 = r.Sum(sum => (decimal?)sum.RoadTransportValue) ?? 0M;

                    return new[]
                    {
                                        0, 0, 0, x1 + x2 + x3
                                    };
                  }
                }
                catch (Exception ex)
                {
                  return new decimal[] { 0, 0, 0, 0 };
                }
              }
            }

            return new decimal[] { 0, 0, 0, 0 };
          }


        default:
          return new decimal[] { 0, 0, 0, 0 };
      }
    }


  }
}
