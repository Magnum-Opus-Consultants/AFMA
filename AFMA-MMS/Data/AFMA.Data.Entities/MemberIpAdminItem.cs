namespace AFMA.Data.Entities
{
  /// <summary>
  /// The MemberIpAdminItem class.
  /// </summary>
  public class MemberIpAdminItem
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
    /// Gets or sets the admin item group.
    /// </summary>
    public virtual IpAdminGroup IpAdminGroup { get; set; }

    /// <summary>
    /// Gets or sets the admin item.
    /// </summary>
    public virtual IpAdminItem IpAdminItem { get; set; }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    public virtual decimal Value { get; set; }

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
  }
}
