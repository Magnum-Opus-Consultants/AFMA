namespace AFMA.ApplicationLayer.ViewModels
{
  public class BranchSummaryReportViewModel
  {
    public string? ReportName { get; set; }
    public int Year { get; set; }
    public string? Module { get; set; }
    public ICollection<MemberBranchViewModel> MemberBranches { get; set; } = new List<MemberBranchViewModel>();
  }

  public class MemberBranchViewModel
  {
    public string? MemberName { get; set; }
    public int BranchId { get; set; }
    public string? BranchName { get; set; }
    public string? ContactName { get; set; }
    public int ReportExistsCount => Reports.Count(x => x.ReportExists);
    public int ReportNotExistsCount => Reports.Count(x => !x.ReportExists);
    public ICollection<ReportViewModel> Reports = new List<ReportViewModel>();
  }

  public class ReportViewModel
  {
    public bool ReportExists { get; set; }
    public int ReportId { get; set; }
    public int ReportYear { get; set; } = DateTime.Now.Year;
    public int ReportMonth { get; set; } = DateTime.Now.Month;
    public string ReportMonthName => System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(ReportMonth);
    public bool Submitted { get; set; }
    public DateTime? SubmittedDate { get; set; }
  }
}
