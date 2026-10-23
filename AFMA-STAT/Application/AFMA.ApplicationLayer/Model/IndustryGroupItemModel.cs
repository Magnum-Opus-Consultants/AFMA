namespace AFMA.ApplicationLayer.Model
{
  using Helpers;

  /// <summary>
  /// The IndustryGroupItemModel class.
  /// </summary>
  [MapTo(typeof(IndustryGroupItem))]
  public class IndustryGroupItemModel
  {
    public int Id { get; set; }
    public string? ItemName { get; set; }
    public string? LinkedTable { get; set; }
    public int LinkedId1 { get; set; }
    public int LinkedId2 { get; set; }
    public bool Active { get; set; }
    public int IndustryCostGroupId { get; set; }
    public string? IndustryCostGroup { get; set; }
    public RunningCostType RunningCostType { get; set; }
    public int RowOnRunningCostSheet { get; set; }
    public int LinkedIndustryCostGroupId1 { get; set; }
    public int LinkedIndustryCostGroupId2 { get; set; }
  }
}
