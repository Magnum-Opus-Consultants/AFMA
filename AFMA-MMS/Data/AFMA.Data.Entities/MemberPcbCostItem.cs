namespace AFMA.Data.Entities
{
  /// <summary>
  /// The MemberPcbCostItem class.
  /// </summary>
  public class MemberPcbCostItem
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
    /// Gets or sets the main group.
    /// </summary>
    public virtual PcbCostMainGroup PcbCostMainGroup { get; set; }

    /// <summary>
    /// Gets or sets the cost item.
    /// </summary>
    public virtual PcbCostItem PcbCostItem { get; set; }

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

    /// <summary>
    /// Gets or sets the read-only indicator.
    /// </summary>
    public virtual bool Readonly { get; set; }

    /// <summary>
    /// Gets or sets the add to total indicator.
    /// </summary>
    public virtual bool AddToTotal { get; set; }

    /// <summary>
    /// Gets or sets the display value indicator.
    /// </summary>
    public virtual int DisplayValue { get; set; }
  }
}
