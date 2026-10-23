namespace AFMA.ApplicationLayer.Model
{
  /// <summary>
  /// The MemberRunningCostFleetModel class.
  /// </summary>
  [MapTo(typeof(MemberRunningCostFleet))]
  public class MemberRunningCostFleetModel
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
    public string Member { get; set; }

    /// <summary>
    /// Gets or sets the Id of the member branch.
    /// </summary>
    public int MemberBranchId { get; set; }

    /// <summary>
    /// Gets or sets the name of the member branch.
    /// </summary>
    public string? MemberBranch { get; set; }

    /// <summary>
    /// Gets or sets the Id of the running cost fleet.
    /// </summary>
    public int RunningCostRentedFleetId { get; set; }

    /// <summary>
    /// Gets or sets the name of the running cost fleet.
    /// </summary>
    public string? RunningCostRentedFleet { get; set; }

    /// <summary>
    /// Gets or sets the code of the running cost fleet.
    /// </summary>
    public string? RunningCostRentedFleetCode { get; set; }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    public decimal Value { get; set; }

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
    /// Gets or sets the note.
    /// </summary>
    public string? Note { get; set; }
  }
}
