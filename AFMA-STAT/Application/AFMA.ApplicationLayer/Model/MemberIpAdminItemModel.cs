namespace AFMA.ApplicationLayer.Model
{
  /// <summary>
  /// The MEmberIpAdminItemModel class.
  /// </summary>
  [MapTo(typeof(MemberIpAdminItem))]
  public class MemberIpAdminItemModel
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
    /// Gets or sets the Id of the admin item group.
    /// </summary>
    public int IpAdminGroupId { get; set; }

    /// <summary>
    /// Gets or sets the name of the admin item group.
    /// </summary>
    public string? IpAdminGroup { get; set; }

    /// <summary>
    /// Gets or sets the Id of the admin item.
    /// </summary>
    public int IpAdminItemId { get; set; }

    /// <summary>
    /// Gets or sets the name of the admin item.
    /// </summary>
    public string? IpAdminItem { get; set; }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    public decimal Value { get; set; }

    /// <summary>
    /// Gets or sets the item description.
    /// </summary>
    public string? Description { get; set; }

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
  }
}
