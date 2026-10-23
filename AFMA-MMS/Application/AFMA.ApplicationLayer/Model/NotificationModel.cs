namespace AFMA.ApplicationLayer.Model
{
  public sealed class NotificationModel
  {
    public int? Id { get; set; }
    public string? UserId { get; set; }
    public int? MemberId { get; set; }
    public int? MemberBranchId { get; set; }
    public int? MemberAccountId { get; set; }
    public int? SampleId { get; set; }
    public int? SampleDetectionId { get; set; }
    public int? ReportId { get; set; }
    public string? CustomText1 { get; set; }
    public string? CustomText2 { get; set; }
  }
}
