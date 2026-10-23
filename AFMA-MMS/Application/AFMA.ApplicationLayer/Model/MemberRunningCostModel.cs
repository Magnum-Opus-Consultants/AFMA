namespace AFMA.ApplicationLayer.Model
{
  /// <summary>
  /// The MemberRunningCostModel class.
  /// </summary>
  [MapTo(typeof(MemberRunningCost))]
  public class MemberRunningCostModel
  {
    /// <summary>
    /// Gets or sets the Id of the item.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the Id of the industry profile report.
    /// </summary>
    public int IndustryProfileReportId { get; set; }

    /// <summary>
    /// Gets or sets the Id of the member.
    /// </summary>
    public int MemberId { get; set; }

    /// <summary>
    /// Gets or sets the name of the member.
    /// </summary>
    public string? Member { get; set; }

    /// <summary>
    /// Gets or sets the Id of the member branch.
    /// </summary>
    public int MemberBranchId { get; set; }

    /// <summary>
    /// Gets or sets the name of the member branch.
    /// </summary>
    public string? MemberBranch { get; set; }

    /// <summary>
    /// Gets or sets the Id of the running cost group.
    /// </summary>
    public int RunningCostGroupId { get; set; }

    /// <summary>
    /// Gets or sets the name of the running cost group.
    /// </summary>
    public string? RunningCostGroup { get; set; }

    /// <summary>
    /// Gets or sets the Id of the running cost.
    /// </summary>
    public int? RunningCostId { get; set; }

    /// <summary>
    /// Gets or sets the name of the member branch.
    /// </summary>
    public string? RunningCost { get; set; }

    /// <summary>
    /// Gets or sets the production value.
    /// </summary>
    public decimal ProductionValue { get; set; }

    /// <summary>
    /// Gets or sets the sales valye;
    /// </summary>
    public decimal SalesValue { get; set; }

    /// <summary>
    /// Gets or sets the administration value.
    /// </summary>
    public decimal AdministrationValue { get; set; }

    /// <summary>
    /// Gets or sets the rod transport value.
    /// </summary>
    public decimal RoadTransportValue { get; set; }

    /// <summary>
    /// Gets or sets the item description.
    /// </summary>
    public string? ItemDescription { get; set; }

    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    public bool Active { get; set; }

    /// <summary>
    /// Gets or sets the display order.
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the calculation type.
    /// </summary>
    public int CalculationType { get; set; }

    /// <summary>
    /// Gets or sets the report year.
    /// </summary>
    public int ReportYear { get; set; }

    /// <summary>
    /// Gets the total.
    /// </summary>
    public decimal Total => ProductionValue + SalesValue + AdministrationValue + RoadTransportValue;
  }
}
