namespace AFMA.ApplicationLayer.Model
{
  public class FeedsalesReportGroupModel
  {
    public int Id { get; set; }
    public string? Name { get; set; }
    public bool Active { get; set; }
    public int FeedsalesReportGroupCategoryId { get; set; }
    public string? FeedsalesReportGroupCategory { get; set; }
    public virtual int ReportDisplayGroup { get; set; }
  }
}
