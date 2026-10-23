namespace AFMA.Data.Entities
{
  public class PcbCostItemGroup
  {
    /// <summary>
    /// Gets or sets the Id of item group.
    /// </summary>
    public virtual int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the sub group.
    /// </summary>
    public virtual PcbCostSubGroup PcbCostSubGroup { get; set; }
    
    /// <summary>
    /// Gets or sets the header name.
    /// </summary>
    public virtual string? HeaderName { get; set; }
    
    /// <summary>
    /// Gets or sets the group name.
    /// </summary>
    public virtual string? ItemGroupName { get; set; }
    
    /// <summary>
    /// Gets or sets the group code/
    /// </summary>
    public virtual string? ItemGroupCode { get; set; }
    
    /// <summary>
    /// Gets or sets the group help text.
    /// </summary>
    public virtual string? ItemGroupHelpText { get; set; }
    
    /// <summary>
    /// Gets or sets the display order.
    /// </summary>
    public virtual int DisplayOrder { get; set; }
    
    /// <summary>
    /// Gets or sets the status.
    /// </summary>
    public virtual bool Active { get; set; }

    /// <summary>
    /// Gets or sets the list of Cost Items
    /// </summary>
    public virtual ICollection<PcbCostItem> PcbCostItems { get; set; }
  }
}
