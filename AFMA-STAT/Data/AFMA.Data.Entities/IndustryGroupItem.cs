namespace AFMA.Data.Entities
{
  /// <summary>
  /// The IndustryGroupItem class.
  /// </summary>
  public class IndustryGroupItem
  {
    public virtual int Id { get; set; }
    public virtual string? ItemName { get; set; }
    public virtual string? LinkedTable { get; set; }
    public virtual int LinkedId1 { get; set; }
    public virtual int LinkedId2 { get; set; }
    public virtual IndustryCostGroup IndustryCostGroup { get; set; }
    public virtual string? RunningCostType { get; set; }
    public virtual int RowOnRunningCostSheet { get; set; }
    public virtual int LinkedIndustryCostGroupId1 { get; set; }
    public virtual int LinkedIndustryCostGroupId2 { get; set; }
    public virtual bool Active { get; set; }
  }
}