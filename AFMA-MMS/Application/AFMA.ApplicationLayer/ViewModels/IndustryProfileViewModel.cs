namespace AFMA.ApplicationLayer.ViewModels
{
  using AFMA.ApplicationLayer.Model;
  using Helpers;
  using System.Collections.Generic;

  public class CompanyIndustryProfileViewModel
  {
    [Display(Name = "Member")]
    public int MemberId { get; set; }

    [Display(Name = "Member Branch")]
    public int MemberBranchId { get; set; }

    [Display(Name = "Member")]
    public string? MemberName { get; set; }

    [Display(Name = "Member Branch")]
    public string? MemberBranch { get; set; }

    [Display(Name = "Report Year")]
    public string? ReportYear { get; set; }

    [Display(Name = "Created By")]
    public int CreatedBy { get; set; }

    [Display(Name = "Report No")]
    public int IndustryProfileReportId { get; set; }

    [Display(Name = "Submitted")]
    public bool IsSubmitted { get; set; }

    [Display(Name = "Member Type")]
    public int MemberTypeId { get; set; }

    [Display(Name = "Member Type")]
    public string? MemberType { get; set; }

    [Display(Name = "Created By")]
    public string? CreatedByUser { get; set; }

    public CurrentPageAction? CurrentPage { get; set; }

    public ICollection<MemberIpBaseItemModel> MemberIpBaseItems { get; set; }

    public ICollection<MemberIpAdminItemModel> MemberIpAdminItems { get; set; }

    public ICollection<MemberIpAdditionalItemModel> MemberIpAdditionalItems { get; set; }

    public ICollection<MemberRunningCostViewModel> MemberRunningCosts { get; set; }

    public ICollection<MemberRunningCostFleetModel> MemberRunningCostsFleet { get; set; }

    public ICollection<MemberRunningCostSundryModel> MemberRunningCostsSundries { get; set; }

    public ICollection<MemberTotalRawMaterialCostModel> MemberTotalRawMaterialCosts { get; set; }

    public List<MemberPcbCostItemModel> MemberPcbCostItems { get; set; }

    public ICollection<MemberPcbCostItemModel> MemberPelletCostItems { get; set; }

    public ICollection<MemberPcbCostItemModel> MemberCrumblingCostItems { get; set; }

    public ICollection<MemberPcbCostItemModel> MemberBaggingCostItems { get; set; }

    public ICollection<IndustryCostGroupModel> IndustryCostsGroups { get; set; }
  }

  public class MemberRunningCostViewModel
  {
    public MemberRunningCostViewModel()
    {
      SubItems = new List<MemberRunningCostViewModel>();
    }
    public int IndustryProfileReportId { get; set; }
    public IndustryProfileReport IndustryProfileReport { get; set; }
    public int Id { get; set; }
    public int MemberId { get; set; }
    public int MemberBranchId { get; set; }
    public string? GroupCode { get; set; }
    public string? GroupName { get; set; }
    public int? ItemId { get; set; }
    public int GroupId { get; set; }
    public decimal ProductionValue { get; set; }
    public bool ProductionEnabled { get; set; }
    public decimal SalesValue { get; set; }
    public bool SalesEnabled { get; set; }
    public decimal AdministrationValue { get; set; }
    public bool AdministrationEnabled { get; set; }
    public decimal RoadTransportValue { get; set; }
    public bool RoadTransportEnabled { get; set; }
    public CalculationType CalculationType { get; set; }
    public int ReportYear { get; set; }
    public bool Editable => this.CalculationType != CalculationType.Formula;
    public decimal Total => ProductionValue + SalesValue + AdministrationValue + RoadTransportValue;
    public ICollection<MemberRunningCostViewModel> SubItems { get; set; }
  }

  public class IndustryProfileTotalCalculationViewModel
  {
    public IndustryProfileTotalCalculationViewModel()
    {
      ItemIdsToSum = new List<int>();
    }

    public IndustryProfileSection IndustryProfileSection { get; set; }
    public ICollection<int> ItemIdsToSum { get; set; }
    public int[] ItemIdsArray => this.ItemIdsToSum.ToArray();
    public IndustryProfileSection DivideBySection { get; set; }
    public int DivideById { get; set; }
    public double Total { get; set; }
    public int ReportYear { get; set; }
  }

  public class IndustryProfileViewModel
  {
    public int ReportId { get; set; }
    public int MemberId { get; set; }
    public int MemberBranchId { get; set; }
    public string MemberName { get; set; }
    public string MemberBranchName { get; set; }
    public string ReportYear { get; set; }
    public int CreatedBy { get; set; }
    public string CreatedByUser { get; set; }
    public bool IsSubmitted { get; set; }
    public int MemberTypeId { get; set; }
    public string MemberType { get; set; }
    public ICollection<IpBaseGroupModel> IndustryProfileBaseGroups { get; set; }
    public ICollection<IpAdminGroupModel> IndustryProfileAdminGroups { get; set; }
    public ICollection<IpAdditionalGroupModel> IndustryProfileAdditionalGroups { get; set; }
    public ICollection<RunningCostGroupModel> RunningCostGroups { get; set; }
    public ICollection<RunningCostRentedFleetModel> RunningCostRentedFleet { get; set; }
    public ICollection<RunningCostSundryExpenseModel> RunningCostSundryExpenses { get; set; }
    public ICollection<PcbCostSubGroupModel> PCBCostSubGroups { get; set; }
    public ICollection<RawMaterialTotalCostModel> RawMaterialTotalCosts { get; set; }
  }

  public class CompanyIndustryProfileDashboardViewModel
  {
    public CompanyIndustryProfileDashboardViewModel()
    {
      Years = new List<CompanyIndustryProfileDashboardYearViewModel>();
      ReportYears = new List<int>();
    }

    public string Type { get; set; }
    public int GroupId { get; set; }
    public int ItemId { get; set; }
    public string GroupName { get; set; }
    public string ItemDescription { get; set; }
    public decimal Total { get { return Years.Sum(x => x.Value); } }
    public List<int> ReportYears { get; set; }
    public bool AddToTotal { get; set; }
    public bool SundryItem { get; set; }

    public decimal Average
    {
      get
      {
        try
        {
          return Years.Average(x => x.Value);
        }
        catch
        {
          return 0M;
        }

      }
    }
    //public Decimal Average => 0;
    public bool Readonly { get; set; }
    public DisplayValue DisplayValue { get; set; }
    public CalculationType CalculationType { get; set; }
    public int MemberId { get; set; }
    public int MemberBranchId { get; set; }
    public bool ProductionEnabled { get; set; }
    public bool SalesEnabled { get; set; }
    public bool AdministrationEnabled { get; set; }
    public bool RoadTransportEnabled { get; set; }
    public int RowOnIndustryCostSheet { get; set; }
    public int LinkedId1 { get; set; }
    public int MemberTypeId { get; set; }

    public ICollection<CompanyIndustryProfileDashboardYearViewModel> Years { get; set; }
  }

  public class CompanyIndustryProfileDashboardYearViewModel
  {
    public int Year { get; set; }
    public decimal Value { get; set; }
    public decimal ProductionValue { get; set; }
    public decimal SalesValue { get; set; }
    public decimal AdministrationValue { get; set; }
    public decimal RoadTransportValue { get; set; }
  }

  public class DashboardIndustryProfileViewModel
  {
    public int Year { get; set; }
    public CompanyIndustryProfileViewModel CompanyIndustryProfileModel { get; set; }
  }
}
