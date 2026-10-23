namespace AFMA.ApplicationLayer.Model
{
  /// <summary>
  /// The MemberRunningCostSundry class.
  /// </summary>
  [MapTo(typeof(MemberRunningCostSundry))]
  public class MemberRunningCostSundryModel
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
    /// Gets or sets the Id of the running cost sundry expense..
    /// </summary>
    public int RunningCostSundryExpenseId { get; set; }

    /// <summary>
    /// Gets or sets the name of the running cost sundry expense..
    /// </summary>
    public string? RunningCostSundryExpense { get; set; }

    /// <summary>
    /// Gets or sets the production enabled indicator.
    /// </summary>
    public bool ProductionEnabled { get; set; }

    /// <summary>
    /// Gets or sets the production value.
    /// </summary>
    public decimal ProductionValue { get; set; }

    /// <summary>
    /// Gets or sets the marketing enabled indicator.
    /// </summary>
    public bool MarketingEnabled { get; set; }

    /// <summary>
    /// Gets or sets the sales value;
    /// </summary>
    public decimal SalesValue { get; set; }

    /// <summary>
    /// Gets or sets the administration enabled indicator.
    /// </summary>
    public bool AdministrationEnabled { get; set; }

    /// <summary>
    /// Gets or sets the administration value.
    /// </summary>
    public decimal AdministrationValue { get; set; }

    /// <summary>
    /// Gets or sets the road transport enabled indicator.
    /// </summary>
    public bool RoadTransportEnabled { get; set; }

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
    /// Gets or sets the report year.
    /// </summary>
    public int ReportYear { get; set; }
  }
}
