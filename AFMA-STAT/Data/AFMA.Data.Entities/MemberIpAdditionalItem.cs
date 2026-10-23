namespace AFMA.Data.Entities
{
  /// <summary>
  /// The MemberIpAdditionalItem class.
  /// </summary>
  public class MemberIpAdditionalItem
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
    /// Gets or sets the additional item group.
    /// </summary>
    public virtual IpAdditionalGroup IpAdditionalGroup { get; set; }
    
    /// <summary>
    /// Gets or sets the additional item.
    /// </summary>
    public virtual IpAdditionalItem IpAdditionalItem { get; set; }
    
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
