namespace AFMA.ApplicationLayer.ViewModels
{
  public class AvailableFinishedProductsViewModel
  {
    public int Id { get; set; }
    public string Code { get; set; }
    public string Product { get; set; }
    public string FullProduct => $"{Code} - {Product}";
    public int MemberId { get; set; }
    public int MemberBranchId { get; set; }
    public int? MemberAccountId { get; set; }
    public int FeedsalesReportId { get; set; }
  }
}
