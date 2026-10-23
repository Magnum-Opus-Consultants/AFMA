namespace AFMA.ApplicationLayer.Model
{
  /// <summary>
  /// The MemberPcbCostItemModel class.
  /// </summary>
  [MapTo(typeof(MemberPcbCostItem))]
  public class MemberPcbCostItemModel
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
    /// Gets or sets the Id of the member type.
    /// </summary>
    public int MemberTypeId { get; set; }

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
    /// Gets or sets the Id of the main group.
    /// </summary>
    public int PcbCostMainGroupId { get; set; }

    /// <summary>
    /// Gets or sets the name of the main group.
    /// </summary>
    public string? PcbCostMainGroup { get; set; }

    /// <summary>
    /// Gets or sets the Id of the cost item.
    /// </summary>
    public int PcbCostItemId { get; set; }

    /// <summary>
    /// Gets or sets the name of the cost item.
    /// </summary>
    public string? PcbCostItem { get; set; }

    /// <summary>
    /// Gets or sets the Id of the cost item group.
    /// </summary>
    public int PcbCostItemGroupId { get; set; }

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
    /// Gets or sets the read-only indicator.
    /// </summary>
    public bool Readonly { get; set; }

    /// <summary>
    /// Gets or sets the add to total indicator.
    /// </summary>
    public bool AddToTotal { get; set; }

    /// <summary>
    /// Gets or sets the display value indicator.
    /// </summary>
    public int DisplayValue { get; set; }
  }
}
