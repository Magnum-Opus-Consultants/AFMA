namespace AFMA.ApplicationLayer.ViewModels
{
  public class FeedsalesReportSummaryViewModel
  {
    public int TotalReports { get; set; }
    public int TotalSubmitted { get; set; }
    public int TotalOutstanding { get; set; }
    public int TotalOverdue { get; set; }
    public decimal TotalRawMaterial { get; set; }
    public decimal TotalFinishedProduct { get; set; }
  }
}
