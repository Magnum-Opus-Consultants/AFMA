namespace AFMA.Infrastructure.Interfaces
{
  public interface IIndustryCostBase
  {
    int Id { get; set; }
    int IndustryProfileReportId { get; set; }
    int? ItemId { get; set; }
    int ReportYear { get; set; }
    int GroupId { get; set; }
    int MemberId { get; set; }
    int MemberBranchId { get; set; }
  }
}
