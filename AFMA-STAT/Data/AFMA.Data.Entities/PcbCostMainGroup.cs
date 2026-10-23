namespace AFMA.Data.Entities
{
  /// <summary>
  /// The PcbCostMainGroup class.
  /// </summary>
  public class PcbCostMainGroup
  {
    /// <summary>
    /// Gets or sets the Id of the group.
    /// </summary>
    public virtual int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the group code.
    /// </summary>
    public virtual string? GroupCode { get; set; }
    
    /// <summary>
    /// Gets or sets the group name.
    /// </summary>
    public virtual string? GroupName { get; set; }
    
    /// <summary>
    /// Gets or sets the display order.
    /// </summary>
    public virtual int DisplayOrder { get; set; }
    
    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    public virtual bool Active { get; set; }

    /// <summary>
    /// Gets the subgroups linked to this main group.
    /// </summary>
    public virtual ICollection<PcbCostSubGroup>? SubGroups { get; set; }
  }
}
