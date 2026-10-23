namespace AFMA.Data.Entities
{
  public class FeedsalesReportGroup
  {
    public virtual int Id { get; set; }
    public virtual string? Name { get; set; }
    public virtual bool Active { get; set; }
    public virtual FeedsalesReportGroupCategory? FeedsalesReportGroupCategory { get; set; }
    public virtual int ReportDisplayGroup { get; set; }
  }
}
