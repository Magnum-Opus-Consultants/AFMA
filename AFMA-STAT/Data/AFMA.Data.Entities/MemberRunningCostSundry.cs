namespace AFMA.Data.Entities
{
  using AFMA.Data.Entities.Interface;

  /// <summary>
  /// The MemberRunningCostSundry class.
  /// </summary>
  public class MemberRunningCostSundry : IIndustryCostMultiple
  {
    /// <summary>
    /// Gets or sets the Id of the item.
    /// </summary>
    public virtual int Id { get; set; }

    /// <summary>
    /// Gets or sets the industry profile report.
    /// </summary>
    public virtual IndustryProfileReport IndustryProfileReport { get; set; }

    /// <summary>
    /// Gets or sets the member.
    /// </summary>
    public virtual Member Member { get; set; }

    /// <summary>
    /// Gets or sets the member branch.
    /// </summary>
    public virtual MemberBranch MemberBranch { get; set; }

    /// <summary>
    /// Gets or sets the running cost sundry expense..
    /// </summary>
    public virtual RunningCostSundryExpense RunningCostSundryExpense { get; set; }

    /// <summary>
    /// Gets or sets the production value.
    /// </summary>
    public virtual decimal ProductionValue { get; set; }

    /// <summary>
    /// Gets or sets the sales value;
    /// </summary>
    public virtual decimal SalesValue { get; set; }

    /// <summary>
    /// Gets or sets the administration value.
    /// </summary>
    public virtual decimal AdministrationValue { get; set; }

    /// <summary>
    /// Gets or sets the rod transport value.
    /// </summary>
    public virtual decimal RoadTransportValue { get; set; }

    /// <summary>
    /// Gets or sets the item description.
    /// </summary>
    public virtual string? ItemDescription { get; set; }

    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    public virtual bool Active { get; set; }

    /// <summary>
    /// Gets or sets the display order.
    /// </summary>
    public virtual int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the report year.
    /// </summary>
    public virtual int ReportYear { get; set; }
  }
}
