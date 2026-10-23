namespace AFMA.Data.Entities.Interface
{
  public interface IIndustryCostBase
  {
    int Id { get; set; }
    IndustryProfileReport IndustryProfileReport { get; set; }
    //int? ItemId { get; set; }
    int ReportYear { get; set; }
    //int GroupId { get; set; }
    Member Member { get; set; }
    MemberBranch MemberBranch { get; set; }
  }
}
