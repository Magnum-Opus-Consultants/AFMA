namespace AFMA.ApplicationLayer.ViewModels
{
  using System;

  public class RawMaterialUsageViewModel
  {
    /// <summary>
    /// Get or sets sthe Raw Material Id
    /// </summary>
    public int MaterialId { get; set; }
    public string MaterialName { get; set; } = string.Empty;
    public ICollection<UsageMemberViewModel> Members { get; set; } = new List<UsageMemberViewModel>();

  }

  public class FinishedProductGroupUsageViewModel
  {
    public int ProductGroupId { get; set; }
    public string ProductGroupName { get; set; } = string.Empty;
    public ICollection<FinishedProductUsageViewModel> Products { get; set; } = new List<FinishedProductUsageViewModel>();
  }

  public class FinishedProductUsageViewModel
  {
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public ICollection<UsageMemberViewModel> Members { get; set; } = new List<UsageMemberViewModel>();

  }

  public class UsageMemberViewModel
  {
    public int MemberId { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public int MemberBranchId { get; set; }
    public string MemberBranchName { get; set; } = string.Empty;
    public ICollection<UsageViewModel> Usage { get; set; } = new List<UsageViewModel>();
    public decimal TotalSubmittedValue => Usage.Sum(u => u.SubmittedValue);
  }

  public class UsageViewModel
  {
    public int Id { get; set; }
    public DateTime InfoFor { get; set; }
    public int Month => InfoFor.Month;
    public int Year => InfoFor.Year;
    public decimal SubmittedValue { get; set; }
  }
}
